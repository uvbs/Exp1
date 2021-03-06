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
	public class Dead : Down
	{
		public override Character.StateProc StateProc { get { return Character.StateProc.Dead; } }

		public Dead(Player.PlayerStateAdapter playerAdapter, Vector3 blownVec, float bulletDirection, MotionState staMotion, MotionState midMotion, MotionState endMotion)
		{
			this.playerAdapter = playerAdapter;
			this.updateFiber = DeadCoroutine(blownVec, bulletDirection, staMotion, midMotion, endMotion);
		}
		public Dead(Player.PlayerStateAdapter playerAdapter, IEnumerator uniqueMotionFiber)
		{
			this.playerAdapter = playerAdapter;
			this.updateFiber = DeadCoroutine(uniqueMotionFiber);
		}

		private IEnumerator DeadCoroutine(Vector3 blownVec, float bulletDirection, MotionState staMotion, MotionState midMotion, MotionState endMotion)
		{
			IEnumerator blownCoroutine = BlownCoroutine(blownVec, bulletDirection, staMotion, midMotion, endMotion);
			while(blownCoroutine.MoveNext()) { yield return null; }

			// 時間経過では終了しない.
			while(true) { yield return null; }
		}
		private IEnumerator DeadCoroutine(IEnumerator uniqueMotionFiber)
		{
			while(uniqueMotionFiber.MoveNext()) { yield return null; }
			
			// 時間経過では終了しない.
			while(true) { yield return null; }
		}
	}
}