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
	public class Move : PlayerState
	{
		public override Character.StateProc StateProc { get { return Character.StateProc.Move; } }

		private Player.PlayerStateAdapter playerAdapter;
		private IEnumerator updateFiber;

		public Move(Player.PlayerStateAdapter playerAdapter)
		{
			this.playerAdapter = playerAdapter;
			this.updateFiber = MoveFiber();
		}

		public override bool Update()
		{
			return updateFiber.MoveNext();
		}

		private IEnumerator MoveFiber()
		{
			this.playerAdapter.ResetAnimation();
			while(true)
			{
				this.playerAdapter.MoveProc();
				yield return null;
			}
		}


		public override bool IsSkillUsable() { return true; }	// ○スキル使用可.
		public override bool CanFalter() { return true; }		// ○怯みアリ.
		public override bool CanBind() { return true; }			// ○マヒアリ.
		public override bool CanJump() { return true; }			// ○ジャンプアリ.
		public override bool CanWire() { return true; }			// ○ワイヤー移動アリ.
	}
}