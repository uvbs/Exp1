/// <summary>
/// ポップアップウィンドウ
/// 
/// 2014/06/21
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIPopupWindow : Singleton<GUIPopupWindow>
{
	#region フィールド＆プロパティ
	/// <summary>
	/// 初期化時のアクティブ状態
	/// </summary>
	[SerializeField] bool _isStartActive = false;
	public bool IsStartActive { get { return _isStartActive; } }

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField] AttachObject _attach;
	public AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UIPlayTween rootTween;
		public UILabel titleLabel;
		public UILabel messageLabel;
	}

	/// <summary>
	/// タイトル設定
	/// </summary>
	public static string Title
	{
		get { return (Instance != null ? Instance.TitleMsg : ""); }
		set { if (Instance != null) Instance.TitleMsg = value; }
	}
	string _titleMsg = "";
	string TitleMsg	
	{
		get { return _titleMsg; }
		set
		{
			_titleMsg = value;
			this.Attach.titleLabel.text = value;
		}
	}

	/// <summary>
	/// テキスト設定
	/// </summary>
	public static string Text
	{
		get { return (Instance != null ? Instance.TextMsg : ""); }
		set { if (Instance != null) Instance.TextMsg = value; }
	}
	string _textMsg = "";
	string TextMsg
	{
		get { return _textMsg; }
		set
		{
			_textMsg = value;
			this.Attach.messageLabel.text = value;
		}
	}

	public System.Action onOK { get; private set; }
	public bool IsActive { get; private set; }
	#endregion

	#region 初期化
	override protected void Awake()
	{
		base.Awake();

		// デリゲート設定
		this._ClearDelegate();
		// 表示設定
		this._SetActive(this.IsStartActive);
	}
	#endregion

	#region デリゲート設定
	public static void ClearDelegate()
	{
		if (Instance == null)
			return;
		Instance._ClearDelegate();
	}
	void _ClearDelegate()
	{
		this.onOK = this.Empty;
	}
	public static void SetDelegateOK(System.Action onOK)
	{
		if (Instance == null)
			return;
		Instance.onOK = (onOK != null ? onOK : Instance.Empty);
	}
	void Empty() {}
	#endregion

	#region モード設定
	public static void SetActive(bool isActive)
	{
		if (Instance == null)
			return;
		Instance._SetActive(isActive);
	}
	void _SetActive(bool isActive)
	{
		this.IsActive = isActive;

		// アニメーション開始
		this.Attach.rootTween.Play(this.IsActive);
	}
	#endregion

	#region NGUIリフレクション
	public void OnOK()
	{
		this.onOK();
	}
	#endregion
}
