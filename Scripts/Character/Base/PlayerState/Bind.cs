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
	public class Bind : PlayerState
	{
		public override Character.StateProc StateProc { get { return Character.StateProc.Recoil; } }

		private IEnumerator updateFiber;

		private Player.PlayerStateAdapter playerAdapter;
		private MotionState motion;

		public Bind(Player.PlayerStateAdapter playerAdapter, MotionState motion)
		{
			this.playerAdapter = playerAdapter;
			this.motion = motion;
			this.updateFiber = BindCoroutine();
		}

		public override bool Update()
		{
			return updateFiber.MoveNext();
		}

		IEnumerator BindCoroutine()
		{
			// モーション
			this.playerAdapter.Player.ScmAnimation.UpdateAnimation(motion, (int)MotionLayer.ReAction, 0f, PlayMode.StopAll);
			this.playerAdapter.SendMotion(motion);

			while(this.playerAdapter.Player.IsParalysis)
			{
				yield return null;
			}
			this.playerAdapter.ResetAnimation();
		}

		public override bool IsSkillUsable() { return false; }	// ×スキル使用不可.
		public override bool CanFalter() { return true; }		// ○怯みアリ.
		public override bool CanBind() { return false; }		// △再マヒはしない(効果時間は長い方になる).
		public override bool CanJump() { return false; }		// ×ジャンプしない.
		public override bool CanWire() { return false; }		// ×ワイヤー移動しない.
	}
}