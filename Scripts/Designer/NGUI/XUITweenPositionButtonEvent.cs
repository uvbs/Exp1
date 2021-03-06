/// <summary>
/// XUIButtonイベント時にTweenPositionの制御を行うスクリプト.
/// 
/// 2014/06/11.
/// </summary>
using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class XUITweenPositionButtonEvent : IXUIButtonEvent, ICloneable
{
	#region イベントデータ
	
	[System.Serializable]
	public class EventData
	{
		/// <summary>
		/// データのセットを有効にするかどうか.
		/// </summary>
		public bool isEnable;
		
		/// <summary>
		/// 終了値.
		/// </summary>
		public Vector3 endTo;
		
		/// <summary>
		/// 再生モード.
		/// </summary>
		public UITweener.Style playStyle;
		
		/// <summary>
		/// 各パラメターをセットする.
		/// </summary>
		public void SetParameter(Vector3 to, UITweener.Style style)
		{
			endTo = to;
			playStyle = style;
		}
	}
	
	#endregion
	
	#region フィールド&プロパティ
	
	[SerializeField]
	private TweenPosition targetTweenPosition;
	public TweenPosition TargetTweenPosition { get { return targetTweenPosition; } } 

	private EventData normal = new EventData();
	
	public EventData hover;
	
	public EventData pressed;
	
	public EventData disabled;
	
	/// <summary>
	/// Tween期間のデフォルト値.
	/// </summary>
	private float defaultDuration;
	
	/// <summary>
	/// TweenのFromデフォルト値..
	/// </summary>
	private Vector3 defaultFrom;

	#endregion
	
	#region 初期化.
	
	public void Init()
	{
		// ノーマルイベントデータのセット.
		if(this.targetTweenPosition != null)
		{
			this.normal.SetParameter(this.targetTweenPosition.to, this.targetTweenPosition.style);
			this.normal.isEnable = false;
			this.defaultFrom = this.targetTweenPosition.from;
			this.defaultDuration = this.targetTweenPosition.duration;
		}
	}
	
	#endregion
	
	#region XUIButtonイベント.
	
	/// <summary>
	/// Normalイベント以外のイベント処理が実行された時のみNormalイベント処理を行う.
	/// </summary>
	public void OnNormal(XUIButton button, bool immediate)
	{
		if(this.targetTweenPosition != null)
		{
			SetTweenPositionData(this.normal, this.defaultFrom, this.defaultDuration, button, immediate);
			this.normal.isEnable = false;
		}
	}
	
	public void OnHover(XUIButton button, bool immediate)
	{
		if(this.targetTweenPosition == null)
			return;
		
		if(this.hover.isEnable)
		{
			// Hoverイベントが有効ならイベント処理を行う.
			SetTweenPositionData(this.hover, this.targetTweenPosition.value, button.duration, button, immediate);
			
			// Hoverイベント終了後にNormalイベント処理を行わせる.
			this.normal.isEnable = true;
		}
		else
		{
			if(this.normal.isEnable)
			{
				// 主に前回のイベント時にNormalイベントを有効にしているのに前回のイベントからすぐに.
				// このメソッドのイベントが呼ばれてイベントを無効にしていた場合.
				OnNormal(button, immediate);
			}
			this.normal.isEnable = false;
		}
	}
	
	public void OnPressed(XUIButton button, bool immediate)
	{
		if(this.targetTweenPosition == null)
			return;
		
		if(this.pressed.isEnable)
		{
			// Hoverイベントが有効ならイベント処理を行う.
			SetTweenPositionData(this.pressed, this.targetTweenPosition.value, button.duration, button, immediate);
			
			// Hoverイベント終了後にNormalイベント処理を行わせる.
			this.normal.isEnable = true;
		}
		else
		{
			if(this.normal.isEnable)
			{
				// 主に前回のイベント時にNormalイベントを有効にしているのに前回のイベントからすぐに.
				// このメソッドのイベントが呼ばれて無効にしていた場合.
				OnNormal(button, immediate);
			}
			this.normal.isEnable = false;
		}
	}
	
	public void OnDisabled(XUIButton button, bool immediate)
	{
		if(this.targetTweenPosition == null)
			return;
		
		if(this.disabled.isEnable)
		{
			// Hoverイベントが有効ならイベント処理を行う.
			SetTweenPositionData(this.disabled, this.targetTweenPosition.value, button.duration, button, immediate);
			
			// Hoverイベント終了後にNormalイベント処理を行わせる.
			this.normal.isEnable = true;
		}
		else
		{
			if(this.normal.isEnable)
			{
				// 主に前回のイベント時にNormalイベントを有効にしているのに前回のイベントからすぐに.
				// このメソッドのイベントが呼ばれて無効にしていた場合.
				OnNormal(button, immediate);
			}
			this.normal.isEnable = false;
		}
	}
	
	#endregion
	
	#region TweenPositionに各データをセット.
	
	private void SetTweenPositionData(EventData eventData, Vector3 from, float duration, XUIButton button, bool immediate)
	{
		if(!eventData.isEnable)
		{
			return;
		}
		
		// TweenPosition再生.
		this.targetTweenPosition.style = eventData.playStyle;
		this.targetTweenPosition.from = from;
		this.targetTweenPosition.to = eventData.endTo;
		this.targetTweenPosition.duration = duration;
		this.targetTweenPosition.ResetToBeginning();
		this.targetTweenPosition.Play(true);
		
		if(immediate)
		{
			this.targetTweenPosition.value = this.targetTweenPosition.to;
			this.targetTweenPosition.enabled = false;
		}
	}
	
	#endregion
	
	#region Inspectorメニュー
	
	/// <summary>
	/// TweenするオブジェクトのPositionをセットする.
	/// </summary>
	public void TransformCopyParameters()
	{
		if(this.targetTweenPosition == null)
			return;
		
		Transform transform = this.targetTweenPosition.gameObject.transform;
		this.hover.SetParameter(transform.localPosition, this.targetTweenPosition.style);
		this.pressed.SetParameter(transform.localPosition, this.targetTweenPosition.style);
		this.disabled.SetParameter(transform.localPosition, this.targetTweenPosition.style);
	}
	
	#endregion
	
	#region 複製.
	
	public XUITweenPositionButtonEvent Clone()
	{
		return (XUITweenPositionButtonEvent)MemberwiseClone();
	}
	
	object ICloneable.Clone()
	{
		return Clone();
	}
	
	#endregion
}