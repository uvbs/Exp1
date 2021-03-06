/// <summary>
/// 
/// 
/// 
/// </summary>
using UnityEngine;
using System.Collections;

namespace PersonState
{
	public abstract class PersonState : PlayerState.IPlayerState
	{
		public abstract Character.StateProc StateProc { get; }

		public abstract bool Update();
		public virtual void Finish() { }

		public virtual bool IsSkillUsable() { return false; }

        /* デバッグ表示用.とりあえずPersonのものは使う予定なし.
        #if UNITY_EDITOR && XW_DEBUG
                private float time;
                protected PersonState()
                {
                    this.time = Time.time;
                }
                public virtual string GetStateInfoStr()
                {
                    return string.Format("{0, 15}:{2, 5} Time {1:##0.000}", this.GetType().Name, Time.time - this.time, IsSkillUsable());
                }
        #endif
        */
        public abstract bool CanFalter();
        public abstract bool CanBind();
        public abstract bool CanJump();
        public abstract bool CanWire();
    }
}