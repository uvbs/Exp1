/// <summary>
/// 
/// 
/// 
/// </summary>
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.Master;
using Scm.Common.Packet;
using Scm.Common.GameParameter;

namespace PlayerState
{
	public class WireDash : PlayerState
	{
		public const float WireSpeed = 75;
		public const float WireApproachLength = 2f;

		public override Character.StateProc StateProc { get { return Character.StateProc.UserJump; } }
		private bool isSkillUsable = false;

		private IEnumerator updateFiber;

		private Player.PlayerStateAdapter playerAdapter;
		private Vector3 position;
		private Vector3 vec;
		float wireSpeed;

		public WireDash(Player.PlayerStateAdapter playerAdapter, Vector3 position, Quaternion rotation, Vector3 vec, float wireSpeed)
		{
			this.playerAdapter = playerAdapter;
			this.position = position;
			this.vec = vec;
			this.wireSpeed = wireSpeed;
			this.updateFiber = WireProcCoroutine();
		}

		public override bool Update()
		{
			return updateFiber.MoveNext();
		}
		public override void Finish()
		{
			this.playerAdapter.ResetAnimation();
		}

		IEnumerator WireProcCoroutine()
		{
			// 飛び始める前に一定時間硬直2015/06/04.
			this.playerAdapter.Player.CharacterMove.Velocity = this.playerAdapter.Player.CharacterMove.Velocity * 0.3f;
			this.playerAdapter.Player.CharacterMove.GravityMag = 0;
			float startTime = Time.time;
			while((Time.time - startTime) < 0.3)
			{
				yield return true;
			}
			startTime = Time.time;

			float time = vec.magnitude / wireSpeed;	// 所要時間

			this.playerAdapter.Player.CharacterMove.DirectionReset();
			this.isSkillUsable = true;

			{
				Vector3 velocity = vec.normalized;
				float endTime = Time.time + time;			// 到着時刻.

				// モーション.
				IEnumerator jumpProcAnimFiber = this.playerAdapter.OrderAnimCoroutine(MotionState.maneuver_f_up_sta, MotionState.maneuver_f_up_lp);
				// エフェクト.
				EffectManager.Create(GameConstant.EffectJumpBase, this.playerAdapter.Player.transform.position, this.playerAdapter.Player.transform.rotation);
				EffectManager.Create(GameConstant.EffectJumpDirect, this.playerAdapter.Player.transform.position, Quaternion.LookRotation(velocity));

				while(Time.time < endTime)
				{
					vec = position - this.playerAdapter.Player.transform.position;
					velocity = velocity.normalized;
					float distance = vec.magnitude - WireApproachLength;
					if(wireSpeed * Time.deltaTime < distance)
					{
						Vector3 movement;
						this.playerAdapter.CalculateMove(velocity * wireSpeed, out movement);
						this.playerAdapter.Player.MovePosition(movement);

						// 中断時に上方向のベクトルは消去する.
						Vector3 vel = this.playerAdapter.Player.CharacterMove.Velocity;
						vel.y = Mathf.Min(0, vel.y);
						this.playerAdapter.Player.CharacterMove.Velocity = vel;

						this.playerAdapter.Player.CharacterMove.GravityMag = 0;	// 重力無視.
						this.playerAdapter.Player.CharacterMove.UseInertia = false;
					}
					else
					{
						Vector3 movement = velocity * distance;
						this.playerAdapter.Player.MovePosition(movement);
						break;
					}

					jumpProcAnimFiber.MoveNext();

					yield return null;
				}

				// 壁に阻まれる場合があるので位置合わせはしない.
				this.playerAdapter.Player.CharacterMove.DirectionReset();
			}

			// 落下判定.
			if(this.playerAdapter.Player.CharacterMove.IsGrounded)
			{
				// 着地硬直なし.
				this.playerAdapter.SendMotion(MotionState.wait);
				yield return null;
			}
			else
			{
				// Fallコルーチン始動.
				this.playerAdapter.SetFallProcCoroutine(MotionState.maneuver_f_dw_sta, MotionState.jump_dw_lp);
			}
		}


		public override bool IsSkillUsable() { return isSkillUsable; }	// ○途中からスキル使用可.ただし空中使用可のスキルのみ.
		public override bool CanFalter() { return true; }		// ○怯みアリ.
		public override bool CanBind() { return true; }			// ○マヒアリ(ダウン落下に移行).
		public override bool CanJump() { return true; }			// ○ジャンプアリ(連続ジャンプもありうる).
		public override bool CanWire() { return true; }			// ○ワイヤー移動アリ.
	}
}