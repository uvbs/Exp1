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
	public class Down : PlayerState
	{
		public override Character.StateProc StateProc { get { return Character.StateProc.Down; } }

		protected IEnumerator updateFiber;

		protected Player.PlayerStateAdapter playerAdapter;

		public Down(Player.PlayerStateAdapter playerAdapter, Vector3 blownVec, float bulletDirection, MotionState staMotion, MotionState midMotion, MotionState endMotion)
		{
			this.playerAdapter = playerAdapter;
			this.updateFiber = DownCoroutine(blownVec, bulletDirection, staMotion, midMotion, endMotion);
		}
		protected Down() { }

		public override bool Update()
		{
			return updateFiber.MoveNext();
		}
		public override void Finish()
		{
			this.ResetModelRotation();
		}

		private IEnumerator DownCoroutine(Vector3 blownVec, float bulletDirection, MotionState staMotion, MotionState midMotion, MotionState endMotion)
		{
			IEnumerator blownCoroutine = BlownCoroutine(blownVec, bulletDirection, staMotion, midMotion, endMotion);
			while(blownCoroutine.MoveNext()) { yield return null; }

			// DownTimer時間で起き上がる.
			float wakeTime = Time.time + GameConstant.DownTimer;
			while(Time.time < wakeTime) { yield return null; }

			this.playerAdapter.Player.Wake();
			yield return null;
		}

		protected IEnumerator BlownCoroutine(Vector3 blownVec, float bulletDirection, MotionState staMotion, MotionState midMotion, MotionState endMotion)
		{
			// モーションファイバー.
			IEnumerator motionFiber = this.playerAdapter.OrderAnimCoroutine(staMotion, midMotion);
			motionFiber.MoveNext();

			this.playerAdapter.Player.transform.rotation = Quaternion.Euler(new Vector3(0, bulletDirection + 180, 0));
			//this.SetNextRotation(this.playerAdapter.Player.transform.rotation);
			Vector3 velocity = this.playerAdapter.Player.transform.rotation * blownVec;

			{
				// 強制的に浮いた状態にする.
				this.playerAdapter.Player.CharacterMove.DirectionReset();
				this.playerAdapter.Player.CharacterMove.IsGrounded = false;

				Vector3 movement = Vector3.one;
				while(0 < movement.y || !this.playerAdapter.Player.CharacterMove.IsGrounded)
				{
					// 移動.
					this.playerAdapter.CalculateMove(velocity, out movement);
					this.playerAdapter.Player.MovePosition(movement);
					this.playerAdapter.Player.CharacterMove.GravityMag = 1;
					this.playerAdapter.Player.CharacterMove.UseInertia = false;
					// 角度.
					if(this.playerAdapter.Player.AvaterModel.ModelTransform)
					{
						this.playerAdapter.Player.AvaterModel.ModelTransform.LookAt(this.playerAdapter.Player.Position - velocity);
					}
					// モーション.
					motionFiber.MoveNext();

					yield return null;

					velocity = this.playerAdapter.Player.CharacterMove.Velocity;
				}

				this.DownBlownFinish(endMotion);
			}
		}

		private void DownBlownFinish(MotionState motion)
		{
			this.playerAdapter.SendMotion(motion);
			this.playerAdapter.Player.ScmAnimation.UpdateAnimation(motion, (int)MotionLayer.ReAction, 0f, PlayMode.StopAll);
			this.ResetModelRotation();
			this.playerAdapter.Player.CharacterMove.DirectionReset();
			EffectManager.CreateDown(this.playerAdapter.Player, GameConstant.EffectDown);
		}

		private void ResetModelRotation()
		{
			this.playerAdapter.SetModelRotation(Quaternion.identity);
		}

		public override bool IsSkillUsable() { return false; }	// ×スキル使用不可.
		public override bool CanFalter() { return false; }		// ×怯みナシ.
		public override bool CanBind() { return false; }		// △マヒモーションにはならない(起き上がり後,効果時間内なら改めてマヒる.マヒ時受け身不可).
		public override bool CanJump() { return false; }		// ×ジャンプしない.
		public override bool CanWire() { return false; }		// ×ワイヤー移動しない.
	}
}