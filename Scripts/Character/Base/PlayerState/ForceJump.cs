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
	public class ForceJump : PlayerState
	{
		public override Character.StateProc StateProc { get { return Character.StateProc.Jump; } }

		private IEnumerator updateFiber;

		private Player.PlayerStateAdapter playerAdapter;
		private Vector3 end;
		private Vector3 velocity;

		public ForceJump(Player.PlayerStateAdapter playerAdapter, Vector3 position, Quaternion rotation, Vector3 vec)
		{
			this.playerAdapter = playerAdapter;
			this.end = position;
			this.velocity = vec;
			this.updateFiber = JumpProcCoroutine();
		}

		public override bool Update()
		{
			return updateFiber.MoveNext();
		}
		public override void Finish()
		{
			this.playerAdapter.ResetAnimation();
		}

		IEnumerator JumpProcCoroutine()
		{
			const float fixedTime = 0.05f;				// 位置計算間隔.低フレームレート時の計算ズレ,障害物超え失敗を防ぐ.
			const float GravityMag = 10f;				// 重力倍率.カッコよく見せるための嘘数値.
			const float JumpTime = 1;					// 所要時間(現在1秒固定).

			float startTime = Time.time;
			Vector3 start = this.playerAdapter.Player.transform.position;
			float calculateTime = fixedTime;
			float maxG = this.playerAdapter.Player.CharacterMove.GravityBaseY * GravityMag * JumpTime * 0.5f;	// 終了時の重力=-開始時の上方向の力.

			this.playerAdapter.ForceSendMovePacket();

			{
				// エフェクト.
				velocity.y -= maxG;
				EffectManager.Create(GameConstant.EffectJumpBase, this.playerAdapter.Player.transform.position, this.playerAdapter.Player.transform.rotation);
				EffectManager.Create(GameConstant.EffectJumpDirect, this.playerAdapter.Player.transform.position, Quaternion.LookRotation(velocity));
				
				// ベクトル設定.
				this.playerAdapter.Player.CharacterMove.DirectionReset();
				this.playerAdapter.Player.CharacterMove.IsGrounded = false;
				this.playerAdapter.Player.CharacterMove.Velocity = velocity;
			}

			// 上昇中.
			IEnumerator jumpProcAnimFiber = this.playerAdapter.JumpProcAnimUp();
			Vector3 outMove = Vector3.up;
			while(0 < outMove.y)
			{
				float elapsedTime = Time.time - startTime;
				for(; calculateTime < elapsedTime; calculateTime += fixedTime)
				{
					float t = calculateTime / JumpTime;
					// 目的地への直線移動と重力の放物線から位置を求める
					Vector3 pos = Vector3.Lerp(start, end, t) + new Vector3(0, (t * t - t) * maxG, 0);
					Vector3 move = pos - this.playerAdapter.Player.transform.position;
					this.playerAdapter.Player.CharacterMove.CalculateMove(move / fixedTime, fixedTime, out outMove);
					this.playerAdapter.Player.MovePosition(outMove, false);
				}

				this.playerAdapter.Player.CharacterMove.GravityMag = GravityMag;
				this.playerAdapter.Player.CharacterMove.UseInertia = false;
				jumpProcAnimFiber.MoveNext();
				
				yield return null;
			}

			// 下降中.
			jumpProcAnimFiber = this.playerAdapter.JumpProcAnimDown();
			while(calculateTime < JumpTime)
			{
				float elapsedTime = Mathf.Min(Time.time - startTime, JumpTime);
				for(; calculateTime < elapsedTime; calculateTime += fixedTime)
				{
					float t = calculateTime / JumpTime;
					// 目的地への直線移動と重力の放物線から位置を求める
					Vector3 pos = Vector3.Lerp(start, end, t) + new Vector3(0, (t * t - t) * maxG, 0);
					Vector3 move = pos - this.playerAdapter.Player.transform.position;
					this.playerAdapter.Player.CharacterMove.CalculateMove(move / fixedTime, fixedTime, out outMove);
					this.playerAdapter.Player.MovePosition(outMove, true);
				}

				this.playerAdapter.Player.CharacterMove.GravityMag = GravityMag;
				this.playerAdapter.Player.CharacterMove.UseInertia = false;
				jumpProcAnimFiber.MoveNext();

				yield return null;
			}

			this.playerAdapter.Player.MovePosition(end - this.playerAdapter.Player.transform.position, true);
			this.playerAdapter.Player.CharacterMove.Velocity = new Vector3(0, maxG, 0);

			// 落下に移行.次フレームに回すとVelocityがリセットされることがあるので即実行.
			this.playerAdapter.SetFallProcCoroutine(jumpProcAnimFiber);

			yield return null;
		}
		
		public override bool IsSkillUsable() { return false; }	// ×スキル使用不可.
		public override bool CanFalter() { return true; }		// ○怯みアリ.
		public override bool CanBind() { return true; }			// ○マヒアリ(ダウン落下に移行).
		public override bool CanJump() { return true; }			// ○ジャンプアリ(連続ジャンプもありうる).
		public override bool CanWire() { return false; }		// ○ワイヤー移動ナシ.
	}
}