/// <summary>
/// 
/// 
/// 
/// </summary>
using UnityEngine;
using System.Collections;

namespace PlayerState
{
	public interface IPlayerState
	{
		Character.StateProc StateProc { get; }
		bool Update();
		void Finish();
	}

	public abstract class PlayerState : IPlayerState
	{
		public abstract Character.StateProc StateProc { get; }

		public abstract bool Update();
		public virtual void Finish() { }

		// State変更可否(最終的には変更可否だけでなく,変更処理そのものを行った方が良いかも).
		public abstract bool IsSkillUsable();
		public abstract bool CanFalter();
		public abstract bool CanBind();
		public abstract bool CanJump();
		public abstract bool CanWire();

#if UNITY_EDITOR && XW_DEBUG
		// デバッグ表示用.
		private float time;
		protected PlayerState()
		{
			this.time = Time.time;
		}
		public virtual string GetStateInfoStr()
		{
			return string.Format("{0, 15}:{2, 5} Time {1:##0.000}", this.GetType().Name, Time.time - this.time, IsSkillUsable());
		}
#endif
	}
}