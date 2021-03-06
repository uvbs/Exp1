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
	public class Fall : PlayerState
	{
		const float LandingStopSpeed = 9;	// これ以上速い速度で落ちたら着地硬直が発生.

		public override Character.StateProc StateProc { get { return Character.StateProc.UserJump; } }

		private IEnumerator updateFiber;

		private Player.PlayerStateAdapter playerAdapter;
		private Vector3 end;
		private Vector3 velocity;

		public Fall(Player.PlayerStateAdapter playerAdapter, IEnumerator fallAnimFiber)
		{
			this.playerAdapter = playerAdapter;
			this.updateFiber = FallProcCoroutine(fallAnimFiber);
		}

		public override bool Update()
		{
			return updateFiber.MoveNext();
		}
		public override void Finish()
		{
			this.playerAdapter.ResetAnimation();
		}

		IEnumerator FallProcCoroutine(IEnumerator fallAnimFiber)
		{
			// 自由落下.
			float maxFallSpeed2 = this.playerAdapter.Player.CharacterMove.Velocity.sqrMagnitude;
			while(!this.playerAdapter.Player.CharacterMove.IsGrounded)
			{
				maxFallSpeed2 = Mathf.Max(maxFallSpeed2, this.playerAdapter.Player.CharacterMove.Velocity.sqrMagnitude);
				fallAnimFiber.MoveNext();
				yield return null;
			}

			// 着地硬直.
			if(LandingStopSpeed * LandingStopSpeed < maxFallSpeed2)
			{
				this.playerAdapter.SendMotion(MotionState.jump_end);
				this.playerAdapter.Player.ScmAnimation.UpdateAnimation(MotionState.jump_end, (int)MotionLayer.ReAction);
				float nextTime = Time.time + this.playerAdapter.Player.ScmAnimation.GetAnimationLength(MotionState.jump_end.ToString());
				// エフェクト.
				EffectManager.CreateDown(this.playerAdapter.Player, GameConstant.EffectDown);
				this.playerAdapter.Player.CharacterMove.DirectionReset();
				while(Time.time < nextTime) { yield return null; }
			}
		}

		public override bool IsSkillUsable() { return true; }	// ○スキル使用可.ただし空中使用可のスキルのみ.
		public override bool CanFalter() { return true; }		// ○怯みアリ.
		public override bool CanBind() { return true; }			// ○マヒアリ(ダウン落下に移行).
		public override bool CanJump() { return true; }			// ○ジャンプアリ(連続ジャンプもありうる).
		public override bool CanWire() { return true; }			// ○ワイヤー移動アリ.
	}
}