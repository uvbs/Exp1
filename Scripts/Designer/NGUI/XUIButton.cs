//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Similar to UIButtonColor, but adds a 'disabled' state based on whether the collider is enabled or not.
/// </summary>

[AddComponentMenu("Designer/NGUI/XUIButton")]
public class XUIButton : UIButton
{
	[SerializeField]
	private XUIButtonEventTrigger eventTrigger;
	
	/// <summary>
	/// Change the visual state.
	/// </summary>

	public override void SetState (State state, bool immediate)
	{
		base.SetState(state, immediate);
		
		switch (state)
		{
			case State.Normal:
				if(this.eventTrigger != null)
				{
					this.eventTrigger.Normal(this, immediate);
				}
				break;
			case State.Hover:
				if(this.eventTrigger != null)
				{
					this.eventTrigger.Hover(this, immediate);
				}
				break;
			case State.Pressed:
				if(this.eventTrigger != null)
				{
					this.eventTrigger.Pressed(this, immediate);
				}
				break;
			case State.Disabled:
				if(this.eventTrigger != null)
				{
					this.eventTrigger.Disabled(this, immediate);
				}
				break;
		}
	}
	
	void Start()
	{
		if(this.eventTrigger != null)
		{
			this.eventTrigger.Init();
		}
	}

#if UNITY_EDITOR
	
	void Reset()
	{
		if(this.eventTrigger != null)
		{
			this.eventTrigger.Reset();
		}
	}
	
	void OnValidate()
	{
		if(this.eventTrigger != null)
		{
			this.eventTrigger.OnValidate();
		}
	}

#endif

}
