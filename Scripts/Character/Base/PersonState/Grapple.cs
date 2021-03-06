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

namespace PersonState
{
	public class Grapple : PersonState
	{
		public override Character.StateProc StateProc { get { return Character.StateProc.Grapple; } }

		private Person.PersonStateAdapter pSAdapter;
        private IEnumerator updateFiber;

        private float grappleDelay;

        public Grapple(Person.PersonStateAdapter personStateAdapter, float grappleDelay)
		{
			this.pSAdapter = personStateAdapter;
            if (personStateAdapter.Person.EntrantInfo.NeedCalcInertia && GameController.IsRoomOwner()) {
                _needAnim = true;
                this.grappleDelay = grappleDelay;
                this.updateFiber = GrappleCoroutine();
            }
		}

		public override bool Update()
		{
            if (_needAnim) {
                return updateFiber.MoveNext();
            } else {
                return this.pSAdapter.HasGrappleAttach;
            }
		}

        protected IEnumerator GrappleCoroutine() {
            // 投げている最中.
            while (this.pSAdapter.Person.GrappleAttach != null) {
                yield return null;
            }
            // 硬直時間中.
            while (0 < this.grappleDelay) {
                this.grappleDelay -= Time.deltaTime;
                yield return null;
            }
        }


        public override bool IsSkillUsable() { return false; }  // ×スキル使用不可.
        public override bool CanFalter() { return false; }      // ×怯みナシ.
        public override bool CanBind() { return true; }         // ○マヒアリ.
        public override bool CanJump() { return false; }        // ×ジャンプナシ.
        public override bool CanWire() { return false; }        // ○ワイヤー移動ナシ.

        private bool _needAnim = false;
    }
}