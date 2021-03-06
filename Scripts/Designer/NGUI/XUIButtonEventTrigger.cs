/// <summary>
/// XUIButtonイベント時に各スクリプトを制御する.
/// .
/// 2014/06/13.
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class XUIButtonEventTrigger
{
	#region フィールド&プロパティ.
	
	#region 各XUIButtonイベントリスト.
	
	[SerializeField]
	private List<XUILabelButtonEvent> labelList = new List<XUILabelButtonEvent>();
	[SerializeField]
	private List<XUISpriteButtonEvent> spriteList = new List<XUISpriteButtonEvent>();
	[SerializeField]
	private List<XUITweenPositionButtonEvent> tweenPositionList = new List<XUITweenPositionButtonEvent>();
	[SerializeField]
	private List<XUITweenRotationButtonEvent> tweenRotationList = new List<XUITweenRotationButtonEvent>();
	[SerializeField]
	private List<XUITweenScaleButtonEvent> tweenScaleList = new List<XUITweenScaleButtonEvent>();
	[SerializeField]
	private List<XUITweenColorButtonEvent> tweenColorList = new List<XUITweenColorButtonEvent>();
	[SerializeField]
	private List<XUITweenAlphaButtonEvent> tweenAlphaList = new List<XUITweenAlphaButtonEvent>();
	
	#endregion
	#endregion
	
	#region 初期化.
	
	public void Init()
	{
		foreach(XUITweenPositionButtonEvent tweenPositionEvent in this.tweenPositionList)
		{
			if(tweenPositionEvent != null)
			{
				tweenPositionEvent.Init();
			}
		}
		foreach(XUITweenRotationButtonEvent tweenRotationEvent in this.tweenRotationList)
		{
			if(tweenRotationEvent != null)
			{
				tweenRotationEvent.Init();
			}
		}
		foreach(XUITweenScaleButtonEvent tweenScaleEvent in this.tweenScaleList)
		{
			if(tweenScaleEvent != null)
			{
				tweenScaleEvent.Init();
			}
		}
		foreach(XUITweenColorButtonEvent tweenColorEvent in this.tweenColorList)
		{
			if(tweenColorEvent != null)
			{
				tweenColorEvent.Init();
			}
		}
		foreach(XUITweenAlphaButtonEvent tweenAlphaEvent in this.tweenAlphaList)
		{
			if(tweenAlphaEvent != null)
			{
				tweenAlphaEvent.Init();
			}
		}
	}
	
	#endregion
	
	#region XUIButtonイベント.
	
	/// <summary>
	/// XUIButtonのNormalイベント時に呼ばれるメソッド.
	/// </summary>
	public void Normal(XUIButton button, bool immediate)
	{
		NormalEventTrigger(this.labelList, button, immediate);
		NormalEventTrigger(this.spriteList, button, immediate);
		NormalEventTrigger(this.tweenPositionList, button, immediate);
		NormalEventTrigger(this.tweenRotationList, button, immediate);
		NormalEventTrigger(this.tweenScaleList, button, immediate);
		NormalEventTrigger(this.tweenColorList, button, immediate);
		NormalEventTrigger(this.tweenAlphaList, button, immediate);
	}
	
	/// <summary>
	/// XUIButtonのNormalイベントを実行する.
	/// </summary>
	private static void NormalEventTrigger<T>(List<T> eventList, XUIButton button, bool immediate) where T : IXUIButtonEvent
	{
		foreach(T eventObj in eventList)
		{
			if(eventObj != null)
			{
				eventObj.OnNormal(button, immediate);
			}
		}
	}
	
	/// <summary>
	/// XUIButtonのHoverイベント時に呼ばれるメソッド.
	/// </summary>
	public void Hover(XUIButton button, bool immediate)
	{
		HoverEventTrigger(this.labelList, button, immediate);
		HoverEventTrigger(this.spriteList, button, immediate);
		HoverEventTrigger(this.tweenPositionList, button, immediate);
		HoverEventTrigger(this.tweenRotationList, button, immediate);
		HoverEventTrigger(this.tweenScaleList, button, immediate);
		HoverEventTrigger(this.tweenColorList, button, immediate);
		HoverEventTrigger(this.tweenAlphaList, button, immediate);
	}
	
	/// <summary>
	/// XUIButtonのHoverイベントを実行する.
	/// </summary>
	private static void HoverEventTrigger<T>(List<T> eventList, XUIButton button, bool immediate) where T : IXUIButtonEvent
	{
		foreach(T eventObj in eventList)
		{
			if(eventObj != null)
			{
				eventObj.OnHover(button, immediate);
			}
		}
	}
	
	/// <summary>
	/// XUIButtonのPressedイベント時に呼ばれるメソッド.
	/// </summary>
	public void Pressed(XUIButton button, bool immediate)
	{
		PressedEventTrigger(this.labelList, button, immediate);
		PressedEventTrigger(this.spriteList, button, immediate);
		PressedEventTrigger(this.tweenPositionList, button, immediate);
		PressedEventTrigger(this.tweenRotationList, button, immediate);
		PressedEventTrigger(this.tweenScaleList, button, immediate);
		PressedEventTrigger(this.tweenColorList, button, immediate);
		PressedEventTrigger(this.tweenAlphaList, button, immediate);
	}
	
	/// <summary>
	/// XUIButtonのPressedイベントを実行する.
	/// </summary>
	private static void PressedEventTrigger<T>(List<T> eventList, XUIButton button, bool immediate) where T : IXUIButtonEvent
	{
		foreach(T eventObj in eventList)
		{
			if(eventObj != null)
			{
				eventObj.OnPressed(button, immediate);
			}
		}
	}
	
	/// <summary>
	/// XUIButtonのDisabledイベント時に呼ばれるメソッド.
	/// </summary>
	public void Disabled(XUIButton button, bool immediate)
	{
		DisabledEventTrigger(this.labelList, button, immediate);
		DisabledEventTrigger(this.spriteList, button, immediate);
		DisabledEventTrigger(this.tweenPositionList, button, immediate);
		DisabledEventTrigger(this.tweenRotationList, button, immediate);
		DisabledEventTrigger(this.tweenScaleList, button, immediate);
		DisabledEventTrigger(this.tweenColorList, button, immediate);
		DisabledEventTrigger(this.tweenAlphaList, button, immediate);
	}
	
	/// <summary>
	/// XUIButtonのDisabledイベントを実行する.
	/// </summary>
	private static void DisabledEventTrigger<T>(List<T> eventList, XUIButton button, bool immediate) where T : IXUIButtonEvent
	{
		foreach(T eventObj in eventList)
		{
			if(eventObj != null)
			{
				eventObj.OnDisabled(button, immediate);
			}
		}
	}

	#endregion
	
	#region エディタ.
	
	/*
	 * インスペクタ上でイベント処理を行う対象オブジェクトが設定された時(ロードされた時)に
	 * 対象オブジェクトの対象データをコピーする処理群.
	 * */
	
#if UNITY_EDITOR
	
	/*
	 * キャッシュデータ群.
	 * 対象オブジェクトに変化があったかどうか比べるために必要.
	 * 
	 * */
	
	private List<XUILabelButtonEvent> cacheLabelList = new List<XUILabelButtonEvent>();
	private List<XUISpriteButtonEvent> cacheSpriteList = new List<XUISpriteButtonEvent>();
	private List<XUITweenPositionButtonEvent> cacheTweenPositionList = new List<XUITweenPositionButtonEvent>();
	private List<XUITweenRotationButtonEvent> cacheTweenRotationList = new List<XUITweenRotationButtonEvent>();
	private List<XUITweenScaleButtonEvent> cacheTweenScaleList = new List<XUITweenScaleButtonEvent>();
	private List<XUITweenColorButtonEvent> cacheTweenColorList = new List<XUITweenColorButtonEvent>();
	private List<XUITweenAlphaButtonEvent> cacheTweenAlphaList = new List<XUITweenAlphaButtonEvent>();
	
	public void Reset()
	{
		/* 
		 * 対象オブジェクトに変化があったかどうか比べるため現在のデータをコピーしておく.
		 * 本来は比較する対象オブジェクトのみキャッシュしておけば良いが他に比較する対象物が増えた時のために
		 * クラスごとキャッシュしておく.
		 * 
		*/
		CacheEventList(ref this.labelList, ref this.cacheLabelList);
		CacheEventList(ref this.spriteList, ref this.cacheSpriteList);
		CacheEventList(ref this.tweenPositionList, ref this.cacheTweenPositionList);
		CacheEventList(ref this.tweenRotationList, ref this.cacheTweenRotationList);
		CacheEventList(ref this.tweenScaleList, ref this.cacheTweenScaleList);
		CacheEventList(ref this.tweenColorList, ref this.cacheTweenColorList);
		CacheEventList(ref this.tweenAlphaList, ref this.cacheTweenAlphaList);
	}
	
	public void OnValidate()
	{
		// 比較する対象物に変化があれば対象オブジェクトの対象データをコピーする処理群.
		LabelEventValidate();
		SpriteEventValidate();
		TweenPositionEventValidate();
		TweenRotationEventValidate();
		TweenScaleEventValidate();
		TweenColorEventValidate();
		TweenAlphaEventValidate();
	}
	
	#region 各OnValidate処理.
	
	/// <summary>
	/// ラベル.
	/// </summary>
	private void LabelEventValidate()
	{
		// 設定されていないので何も処理を行わない
		if(this.labelList.Count == 0) return;

		// 各パラメータにコピーするかチェック.
		if(this.cacheLabelList.Count == this.labelList.Count)
		{
			for(int index = 0; index < this.labelList.Count; ++index)
			{
				if(this.labelList[index].TargetLabel == null)	continue;
				if(this.labelList[index].TargetLabel != this.cacheLabelList[index].TargetLabel)
				{
					// ラベルのパラメータをコピーする.
					this.labelList[index].LabelCopyParameters();
				}
			}
		}
		// キャッシュ処理.
		CacheEventList(ref this.labelList, ref this.cacheLabelList);
	}
	
	/// <summary>
	/// スプライト.
	/// </summary>
	private void SpriteEventValidate()
	{
		// 設定されていないので何も処理を行わない
		if(this.spriteList.Count == 0) return;

		// 各パラメータにコピーするかチェック.
		if(this.cacheSpriteList.Count == this.spriteList.Count)
		{
			for(int index = 0; index < this.spriteList.Count; ++index)
			{
				if(this.spriteList[index].TargetSprite == null)	continue;
				if(this.spriteList[index].TargetSprite != this.cacheSpriteList[index].TargetSprite)
				{
					// スプライトのパラメータをコピーする.
					this.spriteList[index].SpriteCopyParameters();
				}
			}
		}
		// キャッシュ処理.
		CacheEventList(ref this.spriteList, ref this.cacheSpriteList);
	}
	
	/// <summary>
	/// TweenPosition.
	/// </summary>
	private void TweenPositionEventValidate()
	{
		// 設定されていないので何も処理を行わない
		if(this.tweenPositionList.Count == 0) return;

		// 各パラメータにコピーするかチェック.
		if(this.cacheTweenPositionList.Count == this.tweenPositionList.Count)
		{
			for(int index = 0; index < this.tweenPositionList.Count; ++index)
			{
				if(this.tweenPositionList[index].TargetTweenPosition == null)	continue;
				if(this.tweenPositionList[index].TargetTweenPosition != this.cacheTweenPositionList[index].TargetTweenPosition)
				{
					// TweenPositionのパラメータをコピーする.
					this.tweenPositionList[index].TransformCopyParameters();
				}
			}
		}
		// キャッシュ処理.
		CacheEventList(ref this.tweenPositionList, ref this.cacheTweenPositionList);
	}
	
	/// <summary>
	/// TweenRotation.
	/// </summary>
	private void TweenRotationEventValidate()
	{
		// 設定されていないので何も処理を行わない
		if(this.tweenRotationList.Count == 0) return;

		// 各パラメータにコピーするかチェック.
		if(this.cacheTweenRotationList.Count == this.tweenRotationList.Count)
		{
			for(int index = 0; index < this.tweenRotationList.Count; ++index)
			{
				if(this.tweenRotationList[index].TargetTweenRotation == null)	continue;
				if(this.tweenRotationList[index].TargetTweenRotation != this.cacheTweenRotationList[index].TargetTweenRotation)
				{
					// TweenRotationのパラメータをコピーする.
					this.tweenRotationList[index].TransformCopyParameters();
				}
			}
		}
		// キャッシュ処理.
		CacheEventList(ref this.tweenRotationList, ref this.cacheTweenRotationList);
	}
	
	/// <summary>
	/// TweenScale.
	/// </summary>
	private void TweenScaleEventValidate()
	{
		// 設定されていないので何も処理を行わない
		if(this.tweenScaleList.Count == 0) return;

		// 各パラメータにコピーするかチェック.
		if(this.cacheTweenScaleList.Count == this.tweenScaleList.Count)
		{
			for(int index = 0; index < this.tweenScaleList.Count; ++index)
			{
				if(this.tweenScaleList[index].TargetTweenScale == null)	continue;
				if(this.tweenScaleList[index].TargetTweenScale != this.cacheTweenScaleList[index].TargetTweenScale)
				{
					// TweenScaleのパラメータをコピーする.
					this.tweenScaleList[index].TransformCopyParameters();
				}
			}
		}
		// キャッシュ処理.
		CacheEventList(ref this.tweenScaleList, ref this.cacheTweenScaleList);
	}
	
	/// <summary>
	/// TweenColor.
	/// </summary>
	private void TweenColorEventValidate()
	{
		// 設定されていないので何も処理を行わない
		if(this.tweenColorList.Count == 0) return;

		// 各パラメータにコピーするかチェック.
		if(this.cacheTweenColorList.Count == this.tweenColorList.Count)
		{
			for(int index = 0; index < this.tweenColorList.Count; ++index)
			{
				if(this.tweenColorList[index].TargetTweenColor == null)	continue;
				if(this.tweenColorList[index].TargetTweenColor != this.cacheTweenColorList[index].TargetTweenColor)
				{
					// TweenColorのパラメータをコピーする.
					this.tweenColorList[index].WidgetCopyParameters();
				}
			}
		}
		// キャッシュ処理.
		CacheEventList(ref this.tweenColorList, ref this.cacheTweenColorList);
	}
	
	/// <summary>
	/// TweenAlpha.
	/// </summary>
	private void TweenAlphaEventValidate()
	{
		// 設定されていないので何も処理を行わない
		if(this.tweenAlphaList.Count == 0) return;

		// 各パラメータにコピーするかチェック.
		if(this.cacheTweenAlphaList.Count == this.tweenAlphaList.Count)
		{
			for(int index = 0; index < this.tweenAlphaList.Count; ++index)
			{
				if(this.tweenAlphaList[index].TargetTweenAlpha == null)	continue;
				if(this.tweenAlphaList[index].TargetTweenAlpha != this.cacheTweenAlphaList[index].TargetTweenAlpha)
				{
					// TweenAlphaのパラメータをコピーする.
					this.tweenAlphaList[index].WidgetCopyParameters();
				}
			}
		}
		// キャッシュ処理.
		CacheEventList(ref this.tweenAlphaList, ref this.cacheTweenAlphaList);
	}

	#endregion
	
	#region キャッシュ処理.
	
	/// <summary>
	/// イベントリストのキャッシュを行う.
	/// </summary>
	private static void CacheEventList<T>(ref List<T> srcList, ref List<T> destList) where T : ICloneable
	{
		destList = new List<T>();
		foreach(T eventObj in srcList)
		{
			destList.Add((T)eventObj.Clone());
		}
	}
	
	#endregion

#endif
	#endregion
	
}
