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
	public class Grapple : PlayerState
	{
		public override Character.StateProc StateProc { get { return Character.StateProc.Grapple; } }

		private IEnumerator updateFiber;

		private Player.PlayerStateAdapter playerAdapter;
		private float grappleDelay;

		public Grapple(Player.PlayerStateAdapter playerAdapter, float grappleDelay)
		{
			this.playerAdapter = playerAdapter;
			this.grappleDelay = grappleDelay;
			this.updateFiber = GrappleCoroutine();
		}

		public override bool Update()
		{
			return updateFiber.MoveNext();
		}

		protected IEnumerator GrappleCoroutine()
		{
			// 投げている最中.
			while(this.playerAdapter.Player.GrappleAttach != null)
			{
				yield return null;
			}
			// 硬直時間中.
			while(0 < this.grappleDelay)
			{
				this.grappleDelay -= Time.deltaTime;
				yield return null;
			}
		}

		public override bool IsSkillUsable() { return false; }	// ×スキル使用不可.
		public override bool CanFalter() { return false; }		// ×怯みナシ.
		public override bool CanBind() { return true; }			// ○マヒアリ.
		public override bool CanJump() { return false; }		// ×ジャンプナシ.
		public override bool CanWire() { return false; }		// ○ワイヤー移動ナシ.

#if UNITY_EDITOR && XW_DEBUG
		public override string GetStateInfoStr()
		{
			return base.GetStateInfoStr() + string.Format(" Delay {0:##0.000}", this.grappleDelay);
		}
#endif
	}
}