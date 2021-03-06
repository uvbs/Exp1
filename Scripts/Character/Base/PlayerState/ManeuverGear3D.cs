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
	public class ManeuverGear3D : PlayerState
	{
		const float FixedTime = 0.05f;
		const float VelocityMag_Start = 0.3f;
		const float JumpTiming = 0.3f;
		const float MaxManeuverTime = 5f;
		const float GearReelPower = 40f;

		public override Character.StateProc StateProc { get { return Character.StateProc.UserJump; } }
		private bool isSkillUsable = false;

		private IEnumerator updateFiber;

		private Player.PlayerStateAdapter playerStateAdapter;
		private Vector3 anchorPos;
		private float gravityMag;
		private float lastWireLength;

		public ManeuverGear3D(Player.PlayerStateAdapter playerStateAdapter,
			Vector3 anchorPos, float gravityMag, float lastWireLength)
		{
			this.playerStateAdapter = playerStateAdapter;
			this.anchorPos = anchorPos;
			this.gravityMag = gravityMag;
			this.lastWireLength = lastWireLength;
			this.updateFiber = ManeuverGear3DFiber();
		}

		public override bool Update()
		{
			return updateFiber.MoveNext();
		}
		public override void Finish()
		{
			this.playerStateAdapter.ResetAnimation();
		}

		/// <summary>
		/// 立体起動(振り子運動)コルーチン.
		/// </summary>
		IEnumerator ManeuverGear3DFiber()
		{
			float startTime = Time.time;
			float calculateTime = FixedTime;

			// 飛び始める前に一定時間硬直2015/06/04.
			this.playerStateAdapter.Player.CharacterMove.Velocity = this.playerStateAdapter.Player.CharacterMove.Velocity * VelocityMag_Start;
			this.playerStateAdapter.Player.CharacterMove.GravityMag = 0;
			while((Time.time - startTime) < JumpTiming)
			{
				yield return true;
			}
			startTime = Time.time;

			// 開始時の座標を送信.
			this.playerStateAdapter.ForceSendMovePacket();

			float wireLength = Vector3.Distance(anchorPos, this.playerStateAdapter.Player.transform.position);
			Vector3 targetPoint = (anchorPos - this.playerStateAdapter.Player.transform.position) * 3 + this.playerStateAdapter.Player.transform.position;
			targetPoint.y = this.playerStateAdapter.Player.transform.position.y;

			Vector3 targetVec = anchorPos - this.playerStateAdapter.Player.transform.position;
			targetVec.y = 0;
			targetVec.Normalize();
			Matrix4x4 matrix = new Matrix4x4();
			matrix.SetTRS(Vector3.zero, Quaternion.LookRotation(targetVec), Vector3.one);
			Vector3 targetVecR = matrix.MultiplyVector(Vector3.right);
			targetVecR.x = Mathf.Abs(targetVecR.x);
			targetVecR.y = 0f;
			targetVecR.z = Mathf.Abs(targetVecR.z);

			Vector3 vec = new Vector3();
			IEnumerator animFiber = this.playerStateAdapter.OrderAnimCoroutine(MotionState.maneuver_up_sta, MotionState.maneuver_up_lp);

			Vector3 outMove;
			while((Time.time - startTime) < MaxManeuverTime)
			{
				animFiber.MoveNext();

				float elapsedTime = Time.time - startTime;
				// ジョイパッドの方向入力
				//Vector3 inputDirection = player.GetMoveStickRotation();
				//inputDirection = new Vector3(inputDirection.x * targetVecR.x, 0, inputDirection.z * targetVecR.z);
				
				for(; calculateTime < elapsedTime; calculateTime += FixedTime)
				{
					// ワイヤー巻き取り.
					if(lastWireLength < wireLength)
					{
						Vector3 gearReelForce = (anchorPos - this.playerStateAdapter.Player.transform.position).normalized;
						this.playerStateAdapter.Player.CharacterMove.CalculateMoveImpulse(gearReelForce * GearReelPower, FixedTime, out outMove);
						if(this.playerStateAdapter.Player.CharacterMove.MovePosition(outMove) != CollisionFlags.None)
						{
							this.playerStateAdapter.SetFallProcCoroutine(MotionState.maneuver_dw_sta, MotionState.jump_dw_lp);
							yield return false;
						}
						this.playerStateAdapter.SetPosition(this.playerStateAdapter.Player.transform.position);
						wireLength = Vector3.Distance(anchorPos, this.playerStateAdapter.Player.transform.position);
					}
					else
					{
						isSkillUsable = true;
					}

					// 入力分.
					/*
					Vector3 input = inputDirection * 30f * FixedTime;
					this.CharacterMove.CalculateMoveImpulse(input, 1/FixedTime, FixedTime, out outMove);
					this.CharacterMove.MovePosition(outMove);
					//Vector3 gearForceI = (anchorPos - this.transform.position).normalized * ((anchorPos - this.transform.position).magnitude - wireLength);
					//this.CharacterMove.MovePosition(gearForceI);
					*/
					
					// 慣性移動分.
					Vector3 inertialPos = this.playerStateAdapter.Player.transform.position + vec;
					inertialPos.y += this.playerStateAdapter.Player.CharacterMove.GravityBaseY * gravityMag * FixedTime;

					// ワイヤー抵抗分.
					Vector3 relativeVector = anchorPos - inertialPos;
					Vector3 gearForce = relativeVector * (1 - wireLength / relativeVector.magnitude);

					// 移動値.
					vec = inertialPos + gearForce - this.playerStateAdapter.Player.transform.position;

					// 移動処理.
					Vector3 prePos = this.playerStateAdapter.Player.transform.position;
					this.playerStateAdapter.Player.CharacterMove.CalculateMove(vec / FixedTime, FixedTime, out outMove);
					CollisionFlags collisionFlags = this.playerStateAdapter.Player.CharacterMove.MovePosition(outMove);
					this.playerStateAdapter.SetPosition(this.playerStateAdapter.Player.transform.position);
					if(collisionFlags == CollisionFlags.CollidedSides || 0 < outMove.y)
					{
						// 側面が何かに当たるor上昇状態になったら終了.
						this.playerStateAdapter.SetFallProcCoroutine(MotionState.maneuver_dw_sta, MotionState.jump_dw_lp);
						break;
					}
					vec = this.playerStateAdapter.Player.Position - prePos;
				}

				// キャラクターの回転角度.
				if(this.playerStateAdapter.Player.AvaterModel.ModelTransform)
				{
					Vector3 lPoint = new Vector3(targetPoint.x, this.playerStateAdapter.Player.AvaterModel.ModelTransform.position.y, targetPoint.z);
					this.playerStateAdapter.Player.transform.LookAt(lPoint);
				}

				this.playerStateAdapter.Player.CharacterMove.UseInertia = false;
				this.playerStateAdapter.Player.CharacterMove.GravityMag = gravityMag;
				this.playerStateAdapter.Player.CharacterMove.Velocity = new Vector3(this.playerStateAdapter.Player.CharacterMove.Velocity.x, 0, this.playerStateAdapter.Player.CharacterMove.Velocity.z);

				yield return true;
			}

			this.playerStateAdapter.SetFallProcCoroutine(MotionState.maneuver_dw_sta, MotionState.jump_dw_lp);
		}

		public override bool IsSkillUsable() { return isSkillUsable; }	// ○途中からスキル使用可.ただし空中使用可のスキルのみ.
		public override bool CanFalter() { return true; }		// ○怯みアリ.
		public override bool CanBind() { return true; }			// ○マヒアリ(ダウン落下に移行).
		public override bool CanJump() { return true; }			// ○ジャンプアリ(連続ジャンプもありうる).
		public override bool CanWire() { return true; }			// ○ワイヤー移動アリ.
	}
}