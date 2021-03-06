/// <summary>
/// XUIButtonイベント時にTweenRotationの制御を行うスクリプト.
/// 
/// 2014/06/11.
/// </summary>
using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class XUITweenRotationButtonEvent : IXUIButtonEvent, ICloneable
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
	private TweenRotation targetTweenRotation;
	public TweenRotation TargetTweenRotation { get { return targetTweenRotation; } }

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
		if(this.targetTweenRotation != null)
		{
			this.normal.SetParameter(this.targetTweenRotation.to, this.targetTweenRotation.style);
			this.normal.isEnable = false;
			this.defaultFrom = this.targetTweenRotation.from;
			this.defaultDuration = this.targetTweenRotation.duration;
		}
	}
	
	#endregion
	
	#region XUIButtonイベント.
	
	/// <summary>
	/// Normalイベント以外のイベント処理が実行された時のみNormalイベント処理を行う.
	/// </summary>
	public void OnNormal(XUIButton button, bool immediate)
	{
		if(this.targetTweenRotation != null)
		{
			SetTweenRotationData(this.normal, this.defaultFrom, this.defaultDuration, button, immediate);
			this.normal.isEnable = false;
		}
	}
	
	public void OnHover(XUIButton button, bool immediate)
	{
		if(this.targetTweenRotation == null)
			return;
		
		if(this.hover.isEnable)
		{
			// Hoverイベントが有効ならイベント処理を行う.
			SetTweenRotationData(this.hover, this.targetTweenRotation.value.eulerAngles, button.duration, button, immediate);
			
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
		if(this.targetTweenRotation == null)
			return;
		
		if(this.pressed.isEnable)
		{
			// Hoverイベントが有効ならイベント処理を行う.
			SetTweenRotationData(this.pressed, this.targetTweenRotation.value.eulerAngles, button.duration, button, immediate);
			
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
		if(this.targetTweenRotation == null)
			return;
		
		if(this.disabled.isEnable)
		{
			// Hoverイベントが有効ならイベント処理を行う.
			SetTweenRotationData(this.disabled, this.targetTweenRotation.value.eulerAngles, button.duration, button, immediate);
			
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
	
	#region TweenRotationに各データをセット.
	
	private void SetTweenRotationData(EventData eventData, Vector3 from, float duration, XUIButton button, bool immediate)
	{
		if(!eventData.isEnable)
		{
			return;
		}
		
		// TweenRotation再生.
		this.targetTweenRotation.style = eventData.playStyle;
		this.targetTweenRotation.from = from;
		this.targetTweenRotation.to = eventData.endTo;
		this.targetTweenRotation.duration = duration;
		this.targetTweenRotation.ResetToBeginning();
		this.targetTweenRotation.Play(true);
		
		if(immediate)
		{
			this.targetTweenRotation.value = Quaternion.Euler(this.targetTweenRotation.to);
			this.targetTweenRotation.enabled = false;
		}
	}
	
	#endregion
	
	#region Inspectorメニュー
	
	/// <summary>
	/// TweenするオブジェクトのRotationをセットする.
	/// </summary>
	public void TransformCopyParameters()
	{
		if(this.targetTweenRotation == null)
			return;
		
		Transform transform = this.targetTweenRotation.gameObject.transform;
		this.hover.SetParameter(transform.localRotation.eulerAngles, this.targetTweenRotation.style);
		this.pressed.SetParameter(transform.localRotation.eulerAngles, this.targetTweenRotation.style);
		this.disabled.SetParameter(transform.localRotation.eulerAngles, this.targetTweenRotation.style);
	}
	
	#endregion
	
	#region 複製.
	
	public XUITweenRotationButtonEvent Clone()
	{
		return (XUITweenRotationButtonEvent)MemberwiseClone();
	}
	
	object ICloneable.Clone()
	{
		return Clone();
	}
	
	#endregion
}
