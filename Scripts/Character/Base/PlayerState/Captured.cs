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
	public class Captured : PlayerState
	{
		const MotionState CapturedMotion = MotionState.damage_m;

		public override Character.StateProc StateProc { get { return Character.StateProc.Recoil; } }

		private IEnumerator updateFiber;

		private Player.PlayerStateAdapter playerAdapter;
		private Vector3 vec;
		private float time;

		public Captured(Player.PlayerStateAdapter playerAdapter, Vector3 vec)
		{
			this.playerAdapter = playerAdapter;
			this.vec = vec;
			this.updateFiber = CapturedCoroutine();
		}

		public override bool Update()
		{
			return updateFiber.MoveNext();
		}

		IEnumerator CapturedCoroutine()
		{
			float time = this.playerAdapter.Player.ScmAnimation.GetMotionTime(CapturedMotion);
			this.playerAdapter.PlayActionAnimation(CapturedMotion);

			this.playerAdapter.Player.CharacterMove.DirectionReset();
			
			if(0 < time)
			{
				Vector3 velocity = vec.normalized;
				float speed = vec.magnitude / time;
				float endTime = Time.time + time;			// 到着時刻.
				
				while(Time.time < endTime)
				{
					Vector3 movement;
					this.playerAdapter.CalculateMove(velocity * speed, out movement);
					this.playerAdapter.Player.MovePosition(movement);
					this.playerAdapter.Player.CharacterMove.GravityMag = 0;	// 重力無視.
					this.playerAdapter.Player.CharacterMove.UseInertia = false;
					
					yield return null;
				}
				
				// 壁に阻まれる場合があるので位置合わせはしない.
				//ResetTransform(position, rotation);
				this.playerAdapter.Player.CharacterMove.DirectionReset();
			}
			// 着地硬直なし.
			this.playerAdapter.SendMotion(MotionState.wait);
			yield return null;
		}

		public override bool IsSkillUsable() { return false; }	// ×スキル使用不可.
		public override bool CanFalter() { return true; }		// ○怯みアリ.
		public override bool CanBind() { return false; }		// △マヒモーションにはならない(引き寄せ終了後,効果時間内なら改めてマヒる).
		public override bool CanJump() { return false; }		// ×ジャンプしない.
		public override bool CanWire() { return false; }		// ×ワイヤー移動しない.
	}
}