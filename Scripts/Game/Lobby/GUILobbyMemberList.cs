/// <summary>
/// ロビー内メンバーリスト
/// 
/// 2014/06/09
/// </summary>
using UnityEngine;
using System.Collections;

public class GUILobbyMemberList : Singleton<GUILobbyMemberList>
{
	#region フィールド＆プロパティ
	const string TitleMsg = "ロビー内メンバーリスト";
	const string HelpMsg = "ロビー内メンバーリストです";

	/// <summary>
	/// 初期化時のアクティブ状態
	/// </summary>
	[SerializeField]
	bool _isStartActive = false;
	bool IsStartActive { get { return _isStartActive; } }

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UIPlayTween rootTween;
		public PageScrollViewAttach pageScrollView;
	}

	// アクティブ情報
	bool IsActive { get; set; }
	// 閉じるボタンを押した時のデリゲート
	System.Action OnCloseFunction { get; set; }
	// シリアライズされていないメンバーの初期化
	void MemberInit()
	{
		this.IsActive = false;
		this.OnCloseFunction = delegate { };
	}
	#endregion

	#region 初期化
	override protected void Awake()
	{
		base.Awake();

		// 表示設定
		this._SetActive(this.IsStartActive, null);
	}
	#endregion

	#region アクティブ設定
	public static void SetActive(bool isActive)
	{
		SetActive(isActive, null);
	}
	public static void SetActive(bool isActive, System.Action onClose)
	{
		if (Instance != null) Instance._SetActive(isActive, onClose);
	}
	void _SetActive(bool isActive, System.Action onClose)
	{
		this.IsActive = isActive;
		this.OnCloseFunction = (onClose != null ? onClose : delegate { });

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
		this._SetActive(false, null);
	}
	public void OnClose()
	{
		this.OnCloseFunction();
		this._SetActive(false, null);
	}
	public void OnReposition()
	{
		//this.ItemScrollView.Reposition();
		//this.ItemScrollView.CenterOnInTotal(NowLobbyIndex);
		// TODO:仮Reposition
		var t = this.Attach.pageScrollView;
		t.table.Reposition();
		if (t.scrollView.horizontalScrollBar != null)
			t.scrollView.horizontalScrollBar.value = 0f;
		if (t.scrollView.verticalScrollBar != null)
			t.scrollView.verticalScrollBar.value = 0f;
	}
	#endregion
}
