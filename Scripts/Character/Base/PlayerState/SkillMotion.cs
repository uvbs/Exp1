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
    public class SkillMotion : PlayerState
    {
        public static int SkillNum = 0;
        public override Character.StateProc StateProc { get { return Character.StateProc.SkillMotion; } }

        private Player.PlayerStateAdapter playerAdapter;

        public SkillMotion(Player.PlayerStateAdapter playerAdapter)
        {
            SkillNum++;
            this.playerAdapter = playerAdapter;
        }

        public override bool Update()
        {
            this.playerAdapter.SkillMotionProc();
            return true;
        }
        public override void Finish()
        {
            SkillNum--;
            if (SkillNum == 0)
            {
                /*
                if(DetectRay.Instance.camType == DetectRay.CamType.Normal)
                {
                    OUILockon.LockNone();
                }*/
            }
            this.playerAdapter.StateSkillMotion_Finish();
        }


        public override bool IsSkillUsable() { return true; }	// ○スキル使用可(キャンセルorリンク).
        public override bool CanFalter() { return true; }		// ○怯みアリ(スーパーアーマー除く).
        public override bool CanBind() { return true; }			// ○マヒアリ.
        public override bool CanJump() { return true; }			// ○ジャンプアリ.
        public override bool CanWire() { return true; }			// ○ワイヤー移動アリ.
    }
}