/// <summary>
/// スクリーンタイトル
/// 
/// 2014/05/15
/// </summary>
using UnityEngine;
using System.Collections;

public class GUIScreenTitle : Singleton<GUIScreenTitle>
{
	#region フィールド＆プロパティ
	[SerializeField] bool _startActive;
	public bool StartActive { get { return _startActive; } }

	// アタッチオブジェクト
	[SerializeField] AttachObject _attach;
	public AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UIPlayTween tween;
		public UILabel label;
	}
	#endregion

	#region 初期化
	protected override void Awake()
	{
		base.Awake();

		// 表示設定
		this._Play(this.StartActive);
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

	#region 設定
	public static void Play(bool forward, string text)
	{
		if (Instance == null)
			return;
		Instance._Play(forward, text);
	}
	public static void Play(bool forward)
	{
		if (Instance == null)
			return;
		Instance._Play(forward);
	}
	public static void SetText(string text)
	{
		if (Instance == null)
			return;
		Instance._SetText(text);
	}
	void _Play(bool forward, string text)
	{
		this._SetText(text);
		this._Play(forward);
	}
	void _Play(bool forward)
	{
		this.Attach.tween.Play(forward);
	}
	void _SetText(string text)
	{
		this.Attach.label.text = text;
	}
	#endregion
}
