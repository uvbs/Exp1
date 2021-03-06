/// <summary>
/// アイテムストレージ
/// 
/// 2014/06/02
/// </summary>
using UnityEngine;
using System.Collections;

public class GUIItemStorage : Singleton<GUIItemStorage>
{
	#region フィールド＆プロパティ
	const string TitleMsg = "アイテムストレージ";
	const string HelpMsg = "アイテムストレージです";

	/// <summary>
	/// 初期化時のアクティブ状態
	/// </summary>
	[SerializeField]
	bool _isStartActive = false;
	public bool IsStartActive { get { return _isStartActive; } }

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	public AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UIPlayTween rootTween;
	}

	public bool IsActive { get; private set; }
	#endregion

	#region 初期化
	override protected void Awake()
	{
		base.Awake();

		// 表示設定
		this._SetActive(this.IsStartActive);
	}
	#endregion

	#region アクティブ設定
	public static void SetActive(bool isActive)
	{
		if (Instance == null)
			return;
		Instance._SetActive(isActive);
	}
	void _SetActive(bool isActive)
	{
		this.IsActive = isActive;

		// アクティブ化
		if (this.Attach.rootTween != null)
			this.Attach.rootTween.Play(isActive);
		else
			this.gameObject.SetActive(isActive);

		// その他UIの表示設定
		GUILobbyResident.SetActive(!isActive);
		GUIScreenTitle.Play(isActive, TitleMsg);
		GUIHelpMessage.Play(isActive, HelpMsg);
	}
	#endregion

	#region NGUIリフレクション
	public void OnHome()
	{
		this._SetActive(false);
	}
	public void OnClose()
	{
		this._SetActive(false);
		GUILobbyMenu.SetMode(GUILobbyMenu.MenuMode.Top);
	}
	#endregion
}
