/// <summary>
/// ヘルプメッセージ
/// 
/// 2014/05/15
/// </summary>
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GUIHelpMessage : Singleton<GUIHelpMessage>
{
	#region フィールド＆プロパティ
	[SerializeField] bool _startActive = false;
	public bool StartActive { get { return _startActive; } }

	/// <summary>
	/// 通常色
	/// </summary>
	[SerializeField]
	private HelpColor _infoColor = null;
	public HelpColor InfoColor { get { return _infoColor; } }

	/// <summary>
	/// 警告色
	/// </summary>
	[SerializeField]
	private HelpColor _warningColor = null;
	public HelpColor WarningColor { get { return _warningColor; } }
	
	/// <summary>
	/// 色設定用
	/// </summary>
	[System.Serializable]
	public class HelpColor
	{
		public Colors message;
	}

	/// <summary>
	/// グラデーション等を含めた色設定用
	/// </summary>
	[System.Serializable]
	public class Colors
	{
		public Color gradientTop = Color.white;
		public Color gradientBottom = Color.white;
		public Color tint = Color.white;
	}

	// アタッチオブジェクト
	[SerializeField] AttachObject _attach = null;
	public AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UIPlayTween tween;
		public GUIScrollMessage scrollMessage;
		public UILabel messageLabel;
		public UIPlayTween warningEffectPlayTween;
	}

	/// <summary>
	/// 開始スクロール終了時の演出再生用
	/// </summary>
	private Action PlayStartScrollEffect = null;
	#endregion

	#region 初期化
	protected override void Awake()
	{
		base.Awake();

		// 表示設定
		this._Play(this.StartActive);
	}

	void Start()
	{
		this.Attach.scrollMessage.StartScrollFinishEvevnt += this.HandleStartScrollFinish;
	}
	#endregion

	#region 破棄
	/// <summary>
	/// 破棄
	/// </summary>
	void OnDestroy()
	{
		this.PlayStartScrollEffect = null;
	}
	#endregion

	#region NGUIリフレクション
	public void OnActive()
	{
		this._Play(true);
	}
	public void OnDeactive()
	{
		this._Play(false);
	}
	#endregion

	#region 通常再生
	/// <summary>
	/// 通常時の再生
	/// </summary>
	public static void Play(bool forward, string text)
	{
		if (Instance != null)	Instance._Play(forward, text);
	}
	void _Play(bool forward, string text)
	{
		this.PlayStartScrollEffect = null;
		this.SetColor(this.InfoColor);
		this._SetText(text);
		this._Play(forward);
	}
	public static void Play(bool forward)
	{
		if (Instance != null)	Instance._Play(forward);
	}
	void _Play(bool forward)
	{
		this.Attach.tween.Play(forward);
	}
	#endregion

	#region 警告再生
	/// <summary>
	/// 警告時の再生
	/// </summary>
	public static void PlayWarning(bool forward, string text)
	{
		if (Instance != null) Instance._PlayWarning(forward, text);
	}
	void _PlayWarning(bool forward, string text)
	{
		this.PlayStartScrollEffect = this.PlayWarningEffect;
		this.SetColor(this.WarningColor);
		this._SetText(text);
		this._Play(forward);
	}
	#endregion

	#region メッセージの設定
	public static void SetText(string text)
	{
		if (Instance != null) Instance._SetText(text);
	}
	void _SetText(string text)
	{
		if(this.Attach.scrollMessage.IsPlay && this.Attach.scrollMessage.Message.Equals(text))
		{
			// 再生中で同じ文字列の場合は再生させない
			return;
		}

		this.Attach.scrollMessage.SetMessage(text);
		this.Attach.scrollMessage.ReStart();
	}
	#endregion

	#region 色設定
	private void SetColor(HelpColor color)
	{
		UILabel message = this.Attach.messageLabel;
		if(message != null)
		{
			Colors messageColor = color.message;
			message.color = messageColor.tint;
			message.gradientTop = messageColor.gradientTop;
			message.gradientBottom = messageColor.gradientBottom;
		}
	}
	#endregion

	#region スクロール終了イベントハンドラー
	/// <summary>
	/// 開始スクロール終了ハンドラー
	/// </summary>
	private void HandleStartScrollFinish(object sender, EventArgs e)
	{
		// 演出再生
		if (this.PlayStartScrollEffect != null)
		{
			this.PlayStartScrollEffect();
		}
	}
	#endregion

	#region 演出
	/// <summary>
	/// 警告演出再生
	/// </summary>
	private void PlayWarningEffect()
	{
		if (this.Attach.warningEffectPlayTween == null) { return; }
		this.Attach.warningEffectPlayTween.Play(true);
	}
	#endregion
}
