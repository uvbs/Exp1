/// <summary>
/// ロビーメニュー
/// 
/// 2014/05/15
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.GameParameter;

public class GUILobbyMenu : Singleton<GUILobbyMenu>
{
	#region フィールド＆プロパティ
	/// <summary>
	/// 開始時のモード
	/// </summary>
	[SerializeField]
	MenuMode _startMode = MenuMode.Top;
	MenuMode StartMode { get { return _startMode; } }
	[System.Serializable]
	public enum MenuMode
	{
		None,
		Top,
		Community,
		Journal,
		Option,
	}

	// アタッチオブジェクト
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UIPlayTween rootTween;
		public Transform tutorialTarget;

		// トグルグループ
		public Toggle toggle;
		[System.Serializable]
		public class Toggle
		{
			public UIToggle top;
			public UIToggle community;
			public UIToggle journal;
			public UIToggle option;
		}
	}

	// 現在のモード
	MenuMode Mode { get; set; }
	// 前回のモード
	MenuMode BeforeMode { get; set; }

	/// <summary>
	/// ボタンの有効設定
	/// </summary>
	public static bool IsBattleButtonEnable
	{
		get { return false; }
		set { }
	}
	public static bool IsMainButtonEnable
	{
		get { return false; }
		set { }
	}
	public static UIButton BattleButton { get { return null; } }
	// TODO:チュートリアルのターゲット
	public static Transform TutorialTarget { get { return (Instance != null ? Instance.Attach.tutorialTarget : null); } }

	// シリアライズされていないメンバーの初期化
	void MemberInit()
	{
		this.Mode = MenuMode.None;
		this.BeforeMode = MenuMode.None;
	}
	#endregion

	#region 初期化
	void Start()
	{
		this.MemberInit();
		this._SetMode(this.StartMode);
	}
	#endregion

	#region モード設定
	/// <summary>
	/// ウィンドウを閉じる
	/// </summary>
	public static void Close()
	{
		if (Instance != null) Instance.SetWindowActive(MenuMode.None, false);
	}
	/// <summary>
	/// ウィンドウを前回のモードで開く
	/// </summary>
	public static void Active()
	{
		if (Instance != null) Instance.SetWindowActive(Instance.BeforeMode, false);
	}
	/// <summary>
	/// モード設定
	/// </summary>
	/// <param name="mode"></param>
	public static void SetMode(MenuMode mode)
	{
		if (Instance != null) Instance._SetMode(mode);
	}
	/// <summary>
	/// モード設定
	/// </summary>
	void _SetMode(MenuMode mode)
	{
		if (mode == this.Mode)
			return;

		// ウィンドウアクティブ設定
		this.SetWindowActive(mode, mode != MenuMode.None);

		// モードごとのアクティブ設定
		var t = this.Attach.toggle;
		GUIScreenTitle.Play(false);
		switch (mode)
		{
		case MenuMode.None: this.SetModeActive(null); break;
		case MenuMode.Top: this.SetModeActive(t.top); break;
		case MenuMode.Community: this.SetModeActive(t.community); break;
		case MenuMode.Journal: this.SetModeActive(t.journal); break;
		case MenuMode.Option: this.SetModeActive(t.option); break;
		}
	}
	/// <summary>
	/// ウィンドウアクティブ設定
	/// </summary>
	void SetWindowActive(MenuMode mode, bool isActive)
	{
		this.BeforeMode = this.Mode;
		this.Mode = mode;

		// アクティブ化
		if (this.Attach.rootTween != null)
			this.Attach.rootTween.Play(isActive);
		else
			this.gameObject.SetActive(isActive);
	}
	/// <summary>
	/// モードごとのアクティブ設定
	/// </summary>
	void SetModeActive(UIToggle toggle)
	{
		var t = this.Attach;
		if (toggle == null)
		{
			// アクティブトグルのオフ
			var active = UIToggle.GetActiveToggle(t.toggle.top.group);
			if (active != null)
				active.value = false;
		}
		else
		{
			// トグルオン
			toggle.value = true;
		}
	}
	#endregion

	#region トグル表示
	public static void Toggle()
	{
		if (Instance != null) Instance._Toggle();
	}
	void _Toggle()
	{
		var mode = MenuMode.None;
		// 現在のモードが非表示
		if (this.Mode == MenuMode.None)
		{
			// 基本は前回のモードを使用するけど前回のモードも非表示なら Top にする
			mode = (this.BeforeMode != MenuMode.None ? this.BeforeMode : MenuMode.Top);
		}
		this._SetMode(mode);
	}
	#endregion

	#region NGUIデリゲート
	#region トグル
	public void OnToggleTop() { this._SetMode(MenuMode.Top); }
	public void OnToggleCommunity() { this._SetMode(MenuMode.Community); }
	public void OnToggleJournal() { this._SetMode(MenuMode.Journal); }
	public void OnToggleOption() { this._SetMode(MenuMode.Option); }
	#endregion

	#region トップメニュー
	public void _OnCharacterStorage()
	{
		this._SetMode(MenuMode.None);
		GUICharacterStorage.SetModeLobbyChara(
			null,
			() =>
			{
				// メニューに戻る
				this._SetMode(MenuMode.Top);
			},
			(changeTargetOwnCharaID, charaInfo) =>
			{
				if (charaInfo == null)
					return;
				if (changeTargetOwnCharaID == charaInfo.UUID)
					return;
				// キャラクター変更をする
				GUILoading.SetActive(true);
				LobbyMain.CharacterChange(charaInfo.UUID);
			});
	}
	public void _OnSymbolCharacter() {

		// シンボルキャラクターメニューを開く
		this._SetMode( MenuMode.None );
		GUIController.Open( new GUIScreen(
			GUISymbolChara.Open,
			GUISymbolChara.Close,
			GUISymbolChara.ReOpen,
			GUISymbolChara.Close
		));
	}
	public void _OnDeckEdit()
	{
//		this._SetMode(MenuMode.None);
//		GUIDeckEdit.SetModeEdit(true,
//			null,
//			() =>
//			{
//				// メニューに戻る
//				this._SetMode(MenuMode.Top);
//			});
	}
	public void _OnItemStorage()
	{
		//this._SetMode(MenuMode.None);
		//GUIItemStorage.SetActive(true);
		this._SetMode(MenuMode.None);
		GUIController.Open(new GUIScreen(
			GUIItemBox.Open,
			() =>
			{
				GUIItemBox.Close();
				this._SetMode(MenuMode.Top);
			},
			GUIItemBox.ReOpen,
			GUIItemBox.Close
			));
	}
	public void _OnPlayerStatus()
	{
		this._SetMode(MenuMode.None);
		GUIPlayerStatus.SetMode(GUIPlayerStatus.Mode.Player);
	}
	public void OnNews()
	{
		this._SetMode(MenuMode.None);
		GUINews.SetActive(true);
	}
	public void OnPowerup()
	{
		this._SetMode(MenuMode.None);
		GUIController.Open(new GUIScreen(
			GUIPowerupMenu.Open,
			() =>
			{
				GUIPowerupMenu.Close();
				this._SetMode(MenuMode.Top);
			},
			GUIPowerupMenu.ReOpen,
			GUIPowerupMenu.Close,
			GUIPowerupMenu.Close
			));
	}
	public void OnCharaBox()
	{
		this._SetMode(MenuMode.None);
		GUIController.Open(new GUIScreen(
			GUICharacterBox.Open,
			() =>
			{
				GUICharacterBox.Close();
				this._SetMode(MenuMode.Top);
			},
			GUICharacterBox.ReOpen,
			GUICharacterBox.Close,
			GUICharacterBox.Close
			));
	}
	#endregion

	#region メニュー用ラッパ関数
	public void OnCharacterStorage()
	{
		//UIManager.RequestTransition(10);
		this._OnSymbolCharacter();
	}
	public void OnDeckEdit()
	{
		//UIManager.RequestTransition(11);
		this._OnDeckEdit();
	}
	public void OnItemStorage()
	{
		//UIManager.RequestTransition(12);
		this._OnItemStorage();
	}
	public void OnPlayerStatus()
	{
		//UIManager.RequestTransition(13);
		this._OnPlayerStatus();
	}
	#endregion

	#region コミュニティーメニュー
	public void OnLobbyMemberList()
	{
		this._SetMode(MenuMode.None);
		GUILobbyMemberList.SetActive(true, () => { this._SetMode(MenuMode.Community); });
	}
	public void OnFriendList()
	{
		this._SetMode(MenuMode.None);
		GUIFriend.SetActive(true);
	}
	public void OnGuildCommand()
	{
		this._SetMode(MenuMode.None);
		GUIGuild.SetActive(true);
	}
	public void OnPartyCommand()
	{
		this._SetMode(MenuMode.None);
		GUIParty.SetActive(true);
	}
	public void OnApplication()
	{
		this._SetMode(MenuMode.None);
		GUIApplication.SetActive(true);
	}
	#endregion

	#region ジャーナルメニュー
	public void OnAchievement()
	{
		this._SetMode(MenuMode.None);
		//GUIAchievement.SetActive(true);
	}
	public void OnRanking()
	{
		this._SetMode(MenuMode.None);
		//GUILoginBonus.SetActive(true);

	}
	public void OnLibrary()
	{
		this._SetMode(MenuMode.None);
		GUILibrary.SetActive(true);
	}
	#endregion

	#region オプションメニュー
	public void OnOption()
	{
		this._SetMode(MenuMode.None);
		GUIOption.SetActive(true, true, true, () => { this._SetMode(MenuMode.Option); });
	}
	public void OnLobbySelect()
	{
		this._SetMode(MenuMode.None);
		GUILobbySelect.SetActive(true, true, true,
			null,
			() =>
			{
				this._SetMode(MenuMode.Option);
			},
			(currentLobbyID, lobbyInfo) =>
			{
				if (currentLobbyID == lobbyInfo.LobbyID)
					return;
				LobbyMain.LobbyChange(lobbyInfo.LobbyID);
			});
	}
	public void OnLogout()
	{
		this._SetMode(MenuMode.None);

		// メッセージウィンドウ設定
		GUIMessageWindow.SetModeYesNo(MasterData.GetText(TextType.TX128_NextLogout),
			() => { LobbyMain.NextScene_Title(); },
			() => { this._SetMode(MenuMode.Option); }
		);
	}
	public void OnBlackList()
	{
		this._SetMode(MenuMode.None);
		GUIBlackList.SetActive(true);
	}
	#endregion
	#endregion
}
