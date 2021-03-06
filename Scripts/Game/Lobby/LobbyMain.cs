/// <summary>
/// ロビーメイン処理
/// 
/// 2013/08/13
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.GameParameter;
using XUI;
using XUI.Friends;
using XUI.LobbyResident;
using XUI.UserInfo;

public class LobbyMain : SceneMain<LobbyMain>
{
	#region 宣言
	[System.Serializable]
	public enum NextType
	{
		Title = 0,
		Battle,
		Lobby,
		Max,
	}
	#endregion

	#region フィールド＆プロパティ
	const string SceneName = SceneController.SceneName.Lobby;
	#endregion

	#region 初期化
	/// <summary>
	/// 初期化
	/// </summary>
	void Start()
	{
		// UI関連の初期化
		UIManager.SetModeType(UIModeType.Lobby);
		SetUITransitions();
	}
	/// <summary>
	/// シーンの読み込みを開始する
	/// </summary>
	static public void LoadScene(int fieldId, int mapID, PlayerInfo playerInfo, int lobbyCapacity, int mailUnread, int achieveUnreceived, string accountCode)
	{
		// エリアタイプ設定
		ScmParam.Common.AreaType = AreaType.Lobby;

		FiberSet fiberSet = new FiberSet();
		fiberSet.AddFiber(ResourceLoad(fieldId, mapID, playerInfo));
		fiberSet.AddFiber(LobbyUISetupCoroutine(lobbyCapacity, mailUnread, achieveUnreceived, accountCode));

		SceneController.FadeSceneChange(SceneName, fiberSet);
	}
	/// <summary>
	/// リソース読み込みコルーチン
	/// </summary>
	static private IEnumerator ResourceLoad(int fieldId, int mapID, PlayerInfo playerInfo)
	{
		// BGM
		SoundController.PlayBGM(SoundController.BgmID.Lobby);

		// マップの読み込み.
		NetworkController.DestroyAll();
		MapManager.Create(AreaType.Lobby, fieldId, mapID);
		while(!MapManager.Instance.MapExists)
		{
			// マップの読み込みが終わっていないので待つ.
			yield return null;
		}
        //暂时认为商店是最后一个加载
	    while (GUIShop.Instance == null)
	    {
	        yield return null;
	    }
		// フィールド内の参加者全員の情報を取得する
		CommonPacket.SendEntrantAll(fieldId);

		// プレイヤーキャラクターの読み込み.
		playerInfo.CreateObject();
		while(GameController.GetPlayer() == null)
		{
			// HACK: プレイヤー読み込みに失敗した場合はどうする？.
			// プレイヤーキャラクターの読み込みが終わっていないので待つ.
			yield return null;
	    }
#if EJPL && !UNITY_EDITOR
        int playeruid = int.Parse(PlayerPrefs.GetString("player_uid", "1"));
        APaymentHelperDemo.Instance.EnterServer(playeruid, GameController.GetPlayer().UserName, System.DateTime.Now.Second);
#endif
        GuideFrame.Instance.GetGuideInfo();
	}
	/// <summary>
	/// ロビーUI設定
	/// </summary>
	static IEnumerator LobbyUISetupCoroutine(int lobbyCapacity, int mailUnread, int achieveUnreceived, string accountCode)
	{
//		while (GUILobbyResident.Instance == null)
//		{
//			// ロビー常駐メニューの読み込みが終わっていないので待つ
//			yield return null;
//		}
//		GUILobbyResident.SetLobbyMemberCapacity(lobbyCapacity);
//		GUILobbyResident.SetMailUnread(mailUnread);
//		GUILobbyResident.SetAchieveUnreceived(achieveUnreceived);
//		GUILobbyResident.SetApplyUnprocessed(0);

//		while (GUIOption.Instance == null)
//		{
//			// オプションの読み込みが終わってないので待つ
//			yield return null;
//		}
//		GUIOption.SetAccountCode(accountCode);

		// TODO:仮に一度ここで更新する
//		while (GUIPlayerInfo.Instance == null) yield return null;
		while (GameController.GetPlayer() == null) yield return null;
		while (AtlasHold.Instance == null) yield return null;
		while (GUILobbyResident.Instance == null) yield return null;
//        Debug.LogError("===> Init Lobby");
//		GUIPlayerInfo.Sync();
	    {
            var p = GameController.GetPlayer();
	        if (p != null)
	        {
	            GUILobbyResident.SetPlayerName(p.UserName);
	            ScmParam.Battle.CharaIcon.GetIcon(p.AvatarType, p.SkinId, false, (a, s) =>
	            {
                    GUILobbyResident.SetIcon(a, s);
	            });
	        }
	    }

        //卸载之前场景的引用，释放图集资源
        GUILogo.Unload();
        LoginFrame.Instance = null;
        GUITitle.Unload();
        Resources.UnloadUnusedAssets();
	}
	/// <summary>
	/// ISceneMain Override
	/// ネットワーク切断状況を常に監視するかどうか
	/// </summary>
	public override bool OnNetworkDisconnect()
	{
		return true;
	}

	/// <summary>
	/// ISceneMain Override
	/// サーバからの切断による状況を常に監視するかどうか
	/// </summary>
	public override void OnNetworkDisconnectByServer() { }
	#endregion

	#region 終了処理
	void OnDestroy()
	{
        Debug.Log("===> Lobby End");
        GUIHelpMessage.Unload();
        GUILobbyResident.Unload();
        UserInfoController.Unload();
        GUIChatFrameController.Unload();
        GUIMatchingState.Unload();
        GUIBattery.Unload();
        LoadingIconController.Unload();
        TaskDailyView.Unload();
        GUITaskDaily.Unload();
        GUIBattleFieldSelect.Unload();
        GUICharacters.Unload();
        //FriendsController.Unload();
        GUIRankMatch.Unload();
        DungeonController.Unload();
        //GUITipMessage.Unload();
        GUIProductsWindow.Unload();
        GUILoginAward.Unload();
        GUICharacterDetial.Unload();
        SkinPreviewController.Unload();

        DeckEdit.Instance = null;
        RankBoardFrame.Instance = null;
        TopBottom.Instance = null;
        GUITeamMatch.Instance = null;
//	    ShopFrame.Instance = null;
        GUIShop.Unload();
        GUIFriends.Unload();

	    Resources.UnloadUnusedAssets();
	    GUIWebView.Close();
        PanelManager.Instance.layerList.Clear();
	}
	#endregion

	#region ログアウト
	static public void LobbyChange(int lobbyID)
	{
		if (Instance == null)
			{ return; }

		ScmParam.Lobby.LobbyType = (LobbyType)lobbyID;
		LobbyPacket.SendEnterLobby(lobbyID);
	}
	static public void CharacterChange(ulong characterUuID)
	{
		if (Instance == null)
		{ return; }

		CommonPacket.SendSetSymbolPlayerCharacterReq(characterUuID, null);
	}
	static public void NextScene_Title()
	{
		if (Instance != null)
		{
			TitleMain.LoadScene();
		}
	}
	static public void NextScene_Battle()
	{
		if (Instance != null)
		{
			BattlePacket.SendEnterField();
		}
	}
	#endregion

	#region UI遷移設定
	/// <summary>
	/// UI遷移設定
	/// </summary>
	void SetUITransitions()
	{
		//UIManager.SetTransition( 0, GUILobbyResident.Instance._OnLobbySelect, null);
		//UIManager.SetTransition(10, GUILobbyMenu.Instance._OnSymbolCharacter, null);
		//UIManager.SetTransition(11, GUILobbyMenu.Instance._OnDeckEdit, null);
		//UIManager.SetTransition(12, GUILobbyMenu.Instance._OnItemStorage, null);
		//UIManager.SetTransition(13, GUILobbyMenu.Instance._OnPlayerStatus, null);
	}
	#endregion
}
