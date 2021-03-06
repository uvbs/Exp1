/// <summary>
/// ロビーパケット解析
/// 
/// 2013/08/16
/// </summary>
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using ExitGames.Client.Photon;
using Asobimo.Photon.Packet;
using Scm.Common;
using Scm.Common.Packet;
using Scm.Common.PacketCode;
using Scm.Common.GameParameter;
using Scm.Client;

public class LobbyPacket
{
	#region EnterLobby パケット
	/// <summary>
	/// EnterLobby 送信要求
	/// プレイヤーログイン送信
	/// </summary>
	public static void SendEnterLobby()
	{
		// フィールドにログインする
		LobbyPacket.SendEnterLobby(0);
	}
	/// <summary>
	/// EnterLobby 送信要求
	/// プレイヤーログイン送信
	/// </summary>
	/// <param name="avatarType"></param>
	public static void SendEnterLobby(int lobbyID)
	{
		EnterLobbyReq packet = new EnterLobbyReq();
		packet.MasterId = 1;	//	仮値.
		packet.LobbyId = lobbyID;
		GameListener.SendConnected(packet);
	}
	/// <summary>
	/// EnterLobby 受信応答
	/// プレイヤーID設定
	/// プレイヤー、他プレイヤー情報取得パケット送信
	/// </summary>
	/// <param name="packet"></param>
	public void OperationResponseEnterLobby(EnterLobbyRes packet)
	{
		// パケッドが違う
		if (packet == null)
			return;

		switch (packet.EnterLobbyResult)
		{
			case EnterLobbyResult.Success:
				// 正常
				break;
			case EnterLobbyResult.LobbyFull:
				// ロビー満員
				GUIChat.AddSystemMessage(true, MasterData.GetText(TextType.TX115_EnterLobbyRes_LobbyFull));
				return;
			case EnterLobbyResult.InMatching:
				// マッチング中
				GUIChat.AddSystemMessage(true, MasterData.GetText(TextType.TX116_EnterLobbyRes_InMatching));
				return;
			default:
			case EnterLobbyResult.Fatal:
				// 不明なエラー
				GUIChat.AddSystemMessage(true, MasterData.GetText(TextType.TX114_EnterLobbyRes_Fatal));
				return;
		}

		// ログインフラグをON
		GameListener.LoginFlg = true;

		PlayerInfo playerInfo = EntrantInfo.CreatePlayer(packet.GetEntrantRes());
		// 値を保存.
		NetworkController.ServerValue.SetEnterLobbyResponse(packet, playerInfo);
             

		// プレイヤーIDを設定する
		ScmParam.Lobby.LobbyType = (LobbyType)playerInfo.FieldId;

		// アチーブ未取得件数取得
		int achieveUnreceived = 0;
		{
			var p = packet.GetCompleteAchievementEvent();
			achieveUnreceived = p.UnreceivedRewardCount;
		}

		// メール未読件数取得
		int mailUnread = 0;
		{
			var p = packet.GetMailBoxRes();
			foreach (var param in p.GetMailBoxParams())
			{
				mailUnread += param.Unread;
			}
		}

		// ロビーシーン読み込み.
		LobbyMain.LoadScene(playerInfo.FieldId, MapManager.LobbyMapTypeID, playerInfo, packet.Capacity, mailUnread, achieveUnreceived, packet.AccountCode);
	}
	#endregion

	#region ExitLobby パケット
	/// <summary>
	/// ExitLobby 受信応答
	/// プレイヤーログアウト
	/// </summary>
	/// <param name="packet"></param>
	public void OperationResponseExitLobby(ExitLobbyRes packet)
	{
		// 何もしない(送信されてこないはず).
		BugReportController.SaveLogFile("");
	}
	/// <summary>
	/// ExitLobby 受信通知
	/// 他プレイヤーログアウト
	/// </summary>
	/// <param name="packet"></param>
	public void EventExitLobby(ExitLobbyEvent packet)
	{
		// パケッドが違う
		if (packet == null)
			return;

		// 削除
		Entrant.RemoveEntrant(packet.InFieldId);
	}
	#endregion

	#region LobbyList パケット
	/// <summary>
	/// LobbyList 送信
	/// ロビーリスト送信
	/// </summary>
	/// <param name="startID"></param>
	/// <param name="count"></param>
	public static void SendLobbyList(int startID, int count)
	{
		LobbyListReq packet = new LobbyListReq();
		packet.StartId = startID;
		packet.Count = count;
		GameListener.Send(packet);
	}
	/// <summary>
	/// LobbyList 受信応答
	/// ロビーリスト
	/// </summary>
	/// <param name="packet"></param>
	public void OperationResponseLobbyList(LobbyListRes packet)
	{
		// パケッドが違う
		if (packet == null)
			return;

		// パケット変換
		List<LobbyInfo> list = new List<LobbyInfo>();
		foreach (var t in packet.GetLobbyListPackets())
			list.Add(new LobbyInfo(t));

		GUILobbySelect.SetupItem(list);
	}
	#endregion

	#region LobbyNum パケット
	/// <summary>
	/// LobbyNum 送信
	/// ロビー数送信
	/// </summary>
	public static void SendLobbyNum()
	{
		LobbyNumReq packet = new LobbyNumReq();
		GameListener.Send(packet);
	}
	/// <summary>
	/// LobbyNum 受信応答
	/// ロビー数
	/// </summary>
	/// <param name="packet"></param>
	public void OperationResponseLobbyNum(LobbyNumRes packet)
	{
		// パケッドが違う
		if (packet == null)
			return;

		// ロビー
		int currentLobbyIndex = (int)ScmParam.Lobby.LobbyType - 1;
		GUILobbySelect.Setup(packet.Num, currentLobbyIndex);
	}
	#endregion

	#region Ranking パケット
	/// <summary>
	/// Ranking 送信
	/// ランキング送信
	/// </summary>
	/// <param name="rankingType"></param>
	public static void SendRanking(int rankingId, RankingPeriodType periodType)
	{
		//Debug.Log(string.Format("SendRanking RankID:{0} PeriodType:{1}",rankingId,periodType));
		RankingReq packet = new RankingReq();
//		packet.RankingId = rankingId;
//		packet.RankingPeriodType = periodType;
		GameListener.Send(packet);
	}

    public void OperationResponseRanking(RankingRes packet)
    {
        // パケットが違う
        if (packet == null)
            return;
//        Debug.Log("OperationResponseRanking");
        var info = packet.GetRankingItemParameters();
        RankBoardFrame.Instance.SetDetail(info);
//        GUIRanking.SetupRankingListItem(info);
    }
	#endregion

	#region RankingList パケット
    /*
	/// <summary>
	/// RankingList 送信
	/// ランキングリスト送信
	/// </summary>
	/// <param name="rankingType"></param>
	/// <param name="startID"></param>
	/// <param name="count"></param>
	public static void SendRankingList(int rankingId, RankingPeriodType periodType, int startRank, int count)
	{
		//Debug.Log(string.Format("SendRankingList {0}/{1} s:{2} c:{3}",rankingType.ToString(),periodType.ToString() ,startRank , count));
		RankingListReq packet = new RankingListReq();
		packet.RankingId = rankingId;
		packet.RankingPeriodType = periodType;
		packet.StartIndex = startRank;
		packet.Count = count;
		GameListener.Send(packet);

	}


	/// <summary>
	/// RankingList 受信応答
	/// ランキングリスト
	/// </summary>
	/// <param name="packet"></param>
	public void OperationResponseRankingList(RankingListRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;
		//Debug.Log(string.Format("OperationResponseRankingList {0}/{1} Length:{2} ",packet.RankingType,packet.RankingPeriodType,packet.GetRankingPackets().Length ));
		
		var info = new RankingListInfo(packet);
		GUIRanking.SetupRankingListItem(info);
	}
    */
	#endregion

	#region Matching パケット
	/// <summary>
	/// Matching 送信
	/// マッチング
	/// </summary>
	public static void SendMatchingCancel()
	{
		var packet = new MatchingReq();
		packet.MatchingRequestType = MatchingRequestType.Cancel;
		packet.BattleFieldId = 0;
		GameListener.Send(packet);
	}

	public static void SendGuideMatchEntry()
    {
        var packet = new MatchingReq();
        packet.MatchingRequestType = MatchingRequestType.Entry;
        packet.BattleFieldId = 16;
        packet.ScoreType = (byte)ScoreType.QuickMatching;
        GameListener.Send(packet);
    }
	/// <summary>
	/// Matching 送信
	/// マッチング
	/// </summary>
	public static void SendMatchingEntry(BattleFieldType battleFieldType, ScoreType scoreType)
	{
		var packet = new MatchingReq();
		packet.MatchingRequestType = MatchingRequestType.Entry;
		packet.BattleFieldId = (int)battleFieldType;
        packet.ScoreType = (byte)scoreType;
//Request	MatchingRequestType	要求する内容(Entry/Cancel)
//BattleFieldId	int	バトルフィールドID
		GameListener.Send(packet);
	}
	public void OperationResponseMatching(MatchingRes packet)
	{
        Debug.Log("Matching operation...");
		// パケットが違う
		if (packet == null)
			return;
		// リクエスト失敗
		switch (packet.MatchingResult)
		{
		case MatchingResult.Fatal:
            GUITipMessage.Instance.Show(MasterData.GetText(TextType.TX117_MatchingRes_Fatal));
			return;
		case MatchingResult.EntryByTeamMemberFail:
            GUITipMessage.Instance.Show(MasterData.GetText(TextType.TX118_MatchingRes_EntryByTeamMemberFail));
			return;
		case MatchingResult.CancelByTeamMemberFail:
            GUITipMessage.Instance.Show(MasterData.GetText(TextType.TX119_MatchingRes_CancelByTeamMemberFail));
			return;
		case MatchingResult.CancelFail:
            GUITipMessage.Instance.Show(MasterData.GetText(TextType.TX120_MatchingRes_CancelFail));
			return;
        case MatchingResult.EntryNotEnoughEnergy:
            GUITipMessage.Instance.Show(MasterData.GetText(TextType.TX608_NotEnoughEnergy));
            return;
		}
//Request	MatchingRequestType	要求された内容
//Result	bool	結果(True=成功、False=失敗)
	}
	public void EventMatching(MatchingEvent packet)
	{
        Debug.Log("Matching event...");
        // パケットが違う
        if (packet == null)
			return;

		// マッチングステータスを更新する
		if (NetworkController.ServerValue != null)
			NetworkController.ServerValue.SetMatchingStatus(packet.MatchingStatus);

        Debug.Log("ServerValue: " + NetworkController.ServerValue);
        Debug.Log("MatchingStatus: " + packet.MatchingStatus);
		GUIMatchingState.SetMode(packet.MatchingStatus);
//MatchingStatus	MatchingStatus	状態（Entry／Waiting／EnterField）
//BattleFieldId	int	バトルフィールドID(MatchingStatusがEnterFieldの時のみ有効値が入る)
	}
	#endregion

	#region PlayerCharacterBox パケット
	/// <summary>
	/// PlayerCharacterBox 受信引数
	/// </summary>
	public class PlayerCharacterBoxResArgs : EventArgs
	{
		public int Capacity { get; set; }
		public int Count { get; set; }
	}
	static event System.Action<PlayerCharacterBoxResArgs> PlayerCharacterBoxResponse = null;
	/// <summary>
	/// PlayerCharacterBox 送信
	/// プレイヤーキャラボックス
	/// </summary>
	public static void SendPlayerCharacterBox(System.Action<PlayerCharacterBoxResArgs> response)
	{
		//if (PlayerCharacterBoxResponse == null)
		{
			var packet = new PlayerCharacterBoxReq();
			GameListener.Send(packet);
		}

		PlayerCharacterBoxResponse += response;
	}
	/// <summary>
	/// PlayerCharacterBox 受信応答
	/// プレイヤーキャラボックス
	/// </summary>
	public void OperationResponsePlayerCharacterBox(PlayerCharacterBoxRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		// パケット変換
		var eventArgs = new PlayerCharacterBoxResArgs();
		eventArgs.Capacity = packet.Capacity;
		eventArgs.Count = packet.Count;

		// 結果を通知する
		if (PlayerCharacterBoxResponse != null)
		{
			PlayerCharacterBoxResponse(eventArgs);
			PlayerCharacterBoxResponse = null;
		}
	}
	#endregion

	#region PlayerCharacterAll パケット
	/// <summary>
	/// PlayerCharacterAll 受信引数
	/// </summary>
	public class PlayerCharacterAllResArgs : EventArgs
	{
		public List<CharaInfo> List { get; set; }
		/// <summary>
		/// TODO:Obsolate いずれ消す
		/// </summary>
		public ulong SymbolPlayerCharacterUUID { get; set; }
	}
	static event System.Action<PlayerCharacterAllResArgs> PlayerCharacterAllResponse = null;
	/// <summary>
	/// PlayerCharacterAll 送信
	/// 全所有キャラクター情報
	/// </summary>
	public static void SendPlayerCharacterAll(System.Action<PlayerCharacterAllResArgs> response)
	{
        Debug.Log("GetCharaListReq~");
		//if (PlayerCharacterAllResponse == null)
		{
			var packet = new PlayerCharacterAllReq();
			GameListener.Send(packet);
		}

		PlayerCharacterAllResponse += response;
	}
	/// <summary>
	/// PlayerCharacterAll 受信応答
	/// 全所有キャラクター情報
	/// </summary>
	/// <param name="packet"></param>
	public void OperationResponsePlayerCharacterAll(PlayerCharacterAllRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		// パケット変換
		var eventArgs = new PlayerCharacterAllResArgs();
		eventArgs.List = new List<CharaInfo>();
		foreach (var t in packet.GetPlayerCharacterPackets())
		{
            if (!Scm.Common.Master.CharaMaster.Instance.IsValidCharacter(t.CharacterMasterId)) {
                continue;
            }
			eventArgs.List.Add(new CharaInfo(t, (ulong)packet.SymbolPlayerCharacterUuid));
		}
		eventArgs.SymbolPlayerCharacterUUID = (ulong)packet.SymbolPlayerCharacterUuid;

		// 結果を通知する
		if (PlayerCharacterAllResponse != null)
		{
			PlayerCharacterAllResponse(eventArgs);
			PlayerCharacterAllResponse = null;
		}
	}
	#endregion

	#region PlayerCharacter パケット
	public class PlayerCharacterResArgs : EventArgs
	{
		public CharaInfo CharaInfo { get; set; }
		public int SlotBonusHitPoint { get; set; }
		public int SlotBonusAttack { get; set; }
		public int SlotBonusDefense { get; set; }
		public int SlotBonusExtra { get; set; }
		public List<CharaInfo> SlotList { get; set; }
	}
	static event System.Action<PlayerCharacterResArgs> PlayerCharacterResponse = null;
	/// <summary>
	/// PlayerCharacter 送信
	/// 所有キャラクター情報
	/// </summary>
	public static void SendPlayerCharacter(ulong playerCharacterUUID, System.Action<PlayerCharacterResArgs> response)
	{
		//if (PlayerCharacterResponse == null)
		{
			var packet = new PlayerCharacterReq();
			packet.PlayerCharacterUuid = (long)playerCharacterUUID;
			GameListener.Send(packet);
		}

		PlayerCharacterResponse += response;
	}
	/// <summary>
	/// PlayerCharacter 受信応答
	/// 所有キャラクター情報
	/// </summary>
	/// <param name="packet"></param>
	public void OperationResponsePlayerCharacter(PlayerCharacterRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		// パケット変換
		var eventArgs = new PlayerCharacterResArgs();
		eventArgs.CharaInfo = new CharaInfo(packet.GetParam(), 0UL);
		eventArgs.SlotBonusHitPoint = packet.SlotBonusHitPoint;
		eventArgs.SlotBonusAttack = packet.SlotBonusAttack;
		eventArgs.SlotBonusDefense = packet.SlotBonusDefense;
		eventArgs.SlotBonusExtra = packet.SlotBonusExtra;
		eventArgs.SlotList = new List<CharaInfo>();
		foreach (var p in packet.GetPowerupSlotCharacterPackets())
		{
			eventArgs.SlotList.Add(new CharaInfo(p));
		}

		// 結果を通知する
		if (PlayerCharacterResponse != null)
		{
			PlayerCharacterResponse(eventArgs);
			PlayerCharacterResponse = null;
		}
	}
	#endregion

	#region PlayerStatus パケット
	/// <summary>
	/// PlayerStatus 送信
	/// プレイヤーステータス
	/// </summary>
	public static void SendPlayerStatus(int inFieldID)
	{
		var packet = new PlayerStatusReq();
		packet.InFieldId = (short)inFieldID;
		GameListener.Send(packet);
	}
	/// <summary>
	/// PlayerStatus 受信応答
	/// プレイヤーステータス
	/// </summary>
	/// <param name="packet"></param>
	public void OperationResponsePlayerStatus(PlayerStatusRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		// パラメータ変換
		var list = new List<PlayerStatusInfo>();
        list.Add(new PlayerStatusInfo(packet.GetPlayerStatusParam()));

		GUIPlayerStatus.Setup(list);
        //bool succeed = XUI.UserInfo.GUIUserInfo.Instance.SyncBasicDataModel(new PlayerStatusInfo(packet.GetPlayerStatusParam()));
        //if (succeed)
        //{
            //XUI.UserInfo.GUIUserInfo.Instance.SyncBasicDataView();
            //test
        //}
	}
	#endregion

    #region BattleHistory
    public static void SendBattleHistory()
    {
        var packet = new BattleHistoryReq();
        GameListener.Send(packet);
    }

    public void OperationResponseBattleHistory(BattleHistoryRes packet)
    {
        if (packet == null)
            return;
        var pack = packet.GetBattleHistoryRecords();
        //XUI.UserInfo.GUIUserInfo.Instance.SyncComBatGiansView(pack, null);
    }

    public static void SendBattleHistoryDetail(long battleID)
    {
        var packet = new BattleHistoryDetailReq();
        packet.BattleID = battleID;
        GameListener.Send(packet);
    }

    public void OperationResponseBattleHistoryDetail(BattleHistoryDetailRes packet)
    {
        if (packet == null)
            return;
        var pack = packet.GetBattleHistoryDetailItems();
        //XUI.UserInfo.GUIUserInfo.Instance.SyncComBatGiansView(null, pack);

    }

    public static void SendPlayerMiscInfo(long playerId, int serial)
    {
        var packet = new PlayerMiscInfoReq();
        packet.Serial = serial;
        packet.PlayerId = playerId;
        GameListener.Send(packet);
    }

    public void OperationResponsePlayerMiscInfo(PlayerMiscInfoRes packet)
    {
        if (packet == null)
            return;

        var quickInfo = packet.GetQuickMatching();
        var rankInfo = packet.GetRankMatching();
        //if (packet.Serial == 1)
           // XUI.UserInfo.GUIUserInfo.Instance.SyncCombatGiansPlayer(packet, quickInfo, rankInfo);
        //else if (packet.Serial == 0)
            //XUI.UserInfo.GUIUserInfo.Instance.SyncCharaStatisticsView(quickInfo, rankInfo);
    }
    #endregion

    #region SellMultiPlayerCharacterCalc パケット
    /// <summary>
	/// SellMultiPlayerCharacterCalc 受信引数
	/// </summary>
	public class SellMultiPlayerCharacterCalcResArgs : EventArgs
	{
		public bool Result { get; set; }
		public List<ulong> PlayerCharacterUuids { get; set; }
		public int Money { get; set; }
		public int SoldPrice { get; set; }
		public int AddOnCharge { get; set; }
	}
	static event System.Action<SellMultiPlayerCharacterCalcResArgs> SellMultiPlayerCharacterCalcResponse = null;
	/// <summary>
	/// SellMultiPlayerCharacterCalc 送信
	/// プレイヤーキャラクター売却試算(複数)
	/// </summary>
	public static void SendSellMultiPlayerCharacterCalc(ulong[] playerCharacterUuids, System.Action<SellMultiPlayerCharacterCalcResArgs> response)
	{
		//if (SellMultiPlayerCharacterCalcResponse == null)
		{
			var packet = new SellMultiPlayerCharacterCalcReq();
			packet.PlayerCharacterUuids = playerCharacterUuids.ToLongArray();
			GameListener.Send(packet);
		}

		SellMultiPlayerCharacterCalcResponse += response;
	}
	/// <summary>
	/// SellMultiPlayerCharacterCalc 受信応答
	/// プレイヤーキャラクター売却試算(複数)
	/// </summary>
	public void OperationResponseSellMultiPlayerCharacterCalc(SellMultiPlayerCharacterCalcRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		// パケット変換
		var eventArgs = new SellMultiPlayerCharacterCalcResArgs();
		eventArgs.Result = packet.Result;
		eventArgs.PlayerCharacterUuids = new List<ulong>(packet.PlayerCharacterUuids.ToULongArray());
		eventArgs.Money = packet.Money;
		eventArgs.SoldPrice = packet.SoldPrice;
		eventArgs.AddOnCharge = packet.AddOnCharge;

		// 結果を通知する
		if (SellMultiPlayerCharacterCalcResponse != null)
		{
			SellMultiPlayerCharacterCalcResponse(eventArgs);
			SellMultiPlayerCharacterCalcResponse = null;
		}
	} 
	#endregion

	#region SellMultiPlayerCharacter パケット
	/// <summary>
	/// SellMultiPlayerCharacter 受信引数
	/// </summary>
	public class SellMultiPlayerCharacterResArgs : EventArgs
	{
		public bool Result { get; set; }
		public List<ulong> PlayerCharacterUuids { get; set; }
		public int Money { get; set; }
		public int SoldPrice { get; set; }
		public int AddOnCharge { get; set; }
	}
	static event System.Action<SellMultiPlayerCharacterResArgs> SellMultiPlayerCharacterResponse = null;
	/// <summary>
	/// SellMultiPlayerCharacter 送信
	/// プレイヤーキャラクター売却(複数)
	/// </summary>
	public static void SendSellMultiPlayerCharacter(ulong[] playerCharacterUuids, System.Action<SellMultiPlayerCharacterResArgs> response)
	{
		//if (SellMultiPlayerCharacterResponse == null)
		{
			var packet = new SellMultiPlayerCharacterReq();
			packet.PlayerCharacterUuids = playerCharacterUuids.ToLongArray();
			GameListener.Send(packet);
		}

		SellMultiPlayerCharacterResponse += response;
	}
	/// <summary>
	/// SellMultiPlayerCharacter 受信応答
	/// プレイヤーキャラクター売却(複数)
	/// </summary>
	public void OperationResponseSellMultiPlayerCharacter(SellMultiPlayerCharacterRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		// パケット変換
		var eventArgs = new SellMultiPlayerCharacterResArgs();
		eventArgs.Result = packet.Result;
		eventArgs.PlayerCharacterUuids = new List<ulong>(packet.PlayerCharacterUuids.ToULongArray());
		eventArgs.Money = packet.Money;
		eventArgs.SoldPrice = packet.SoldPrice;
		eventArgs.AddOnCharge = packet.AddOnCharge;

		var t = NetworkController.ServerValue;
		if (t != null)
		{
			t.SetMoney(eventArgs.Money);
		}

		// 結果を通知する
		if (SellMultiPlayerCharacterResponse != null)
		{
			SellMultiPlayerCharacterResponse(eventArgs);
			SellMultiPlayerCharacterResponse = null;
		}
	}
	#endregion

	#region Powerup パケット
	/// <summary>
	/// Powerup 受信引数
	/// </summary>
	public class PowerupResArgs : EventArgs
	{
		public PowerupResult Result { get; set; }
		public int Money { get; set; }
		public int Price { get; set; }
		public int AddOnCharge { get; set; }
		public CharaInfo CharaInfo { get; set; }
	}
	static event System.Action<PowerupResArgs> PowerupResponse = null;
	/// <summary>
	/// Powerup 送信
	/// 強化合成
	/// </summary>
	public static void SendPowerup(ulong baseCharaUUID, ulong[] baitCharaUUIDs, System.Action<PowerupResArgs> response)
	{
		//if (PowerupResponse == null)
		{
			var packet = new PowerupReq();
			packet.BasePlayerCharacterUuid = (long)baseCharaUUID;
			packet.BaitPlayerCahracterUuids = baitCharaUUIDs.ToLongArray();
			GameListener.Send(packet);
		}

		PowerupResponse += response;
	}
	/// <summary>
	/// Powerup 受信応答
	/// 強化合成
	/// </summary>
	public void OperationResponsePowerup(PowerupRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		// パケット変換
		var eventArgs = new PowerupResArgs();
		eventArgs.Result = packet.PowerupResult;
		eventArgs.Money = packet.Money;
		eventArgs.Price = packet.Price;
		eventArgs.AddOnCharge = packet.AddOnCharge;
		eventArgs.CharaInfo = new CharaInfo(packet.GetParam());

		switch (eventArgs.Result)
		{
			case PowerupResult.Fail:
				XUI.GUIChatFrameController.AddSystemMessage(true, MasterData.GetText(TextType.TX307_PowerupRes_Fail));
				break;
		}

		var t = NetworkController.ServerValue;
		if (t != null)
		{
			t.SetMoney(eventArgs.Money);
		}

		// 結果を通知する
		if (PowerupResponse != null)
		{
			PowerupResponse(eventArgs);
			PowerupResponse = null;
		}
	}
	#endregion

	#region PowerupCalc パケット
	/// <summary>
	/// PowerupCalc 受信引数
	/// </summary>
	public class PowerupCalcResArgs : EventArgs
	{
		public int Exp { get; set; }
		public int Money { get; set; }
		public int Price { get; set; }
		public int AddOnCharge { get; set; }
	}
	static event System.Action<PowerupCalcResArgs> PowerupCalcResponse = null;
	/// <summary>
	/// PowerupCalc 送信
	/// 強化合成試算
	/// </summary>
	public static void SendPowerupCalc(ulong baseCharaUUID, ulong[] baitCharaUUIDs, System.Action<PowerupCalcResArgs> response)
	{
		//if (PowerupCalcResponse == null)
		{
			var packet = new PowerupCalcReq();
			packet.BasePlayerCharacterUuid = (long)baseCharaUUID;
			packet.BaitPlayerCahracterUuids = baitCharaUUIDs.ToLongArray();
			GameListener.Send(packet);
		}

		PowerupCalcResponse += response;
	}
	/// <summary>
	/// PowerupCalc 受信応答
	/// 強化合成試算
	/// </summary>
	public void OperationResponsePowerupCalc(PowerupCalcRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		// パケット変換
		var eventArgs = new PowerupCalcResArgs();
		eventArgs.Exp = packet.Exp;
		eventArgs.Money = packet.Money;
		eventArgs.Price = packet.Price;
		eventArgs.AddOnCharge = packet.AddOnCharge;

		// 結果を通知する
		if (PowerupCalcResponse != null)
		{
			PowerupCalcResponse(eventArgs);
			PowerupCalcResponse = null;
		}
	}
	#endregion

	#region Evolution パケット
	/// <summary>
	/// Evolution 受信引数
	/// </summary>
	public class EvolutionResArgs : EventArgs
	{
		public bool Result { get; set; }
		public int Money { get; set; }
		public int Price { get; set; }
		public int AddOnCharge { get; set; }
		public int SynchroBonus { get; set; }
		public CharaInfo CharaInfo { get; set; }
	}
	static event System.Action<EvolutionResArgs> EvolutionResponse = null;
	/// <summary>
	/// Evolution 送信
	/// 進化合成
	/// </summary>
	public static void SendEvolution(ulong baseCharaUUID, ulong[] baitCharaUUIDs, System.Action<EvolutionResArgs> response)
	{
		//if (EvolutionResponse == null)
		{
			var packet = new EvolutionReq();
			packet.BasePlayerCharacterUuid = (long)baseCharaUUID;
			packet.BaitPlayerCharacterUuids = baitCharaUUIDs.ToLongArray();
			GameListener.Send(packet);
		}

		EvolutionResponse += response;
	}
	/// <summary>
	/// Evolution 受信応答
	/// 進化合成
	/// </summary>
	public void OperationResponseEvolution(EvolutionRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		// パケット変換
		var eventArgs = new EvolutionResArgs();
		eventArgs.Result = packet.Result;
		eventArgs.Money = packet.Money;
		eventArgs.Price = packet.Price;
		eventArgs.AddOnCharge = packet.AddOnCharge;
		eventArgs.SynchroBonus = packet.SynchroBonus;
		eventArgs.CharaInfo = new CharaInfo(packet.GetParam());

		if(!eventArgs.Result)
		{
			// 失敗
			GUIChat.AddSystemMessage(true, MasterData.GetText(TextType.TX316_EvolutionRes_Fail));
		}

		var t = NetworkController.ServerValue;
		if (t != null)
		{
			t.SetMoney(eventArgs.Money);
		}

		// 結果を通知する
		if (EvolutionResponse != null)
		{
			EvolutionResponse(eventArgs);
			EvolutionResponse = null;
		}
	}
	#endregion

	#region EvolutionCalc パケット
	/// <summary>
	/// EvolutionCalc 受信引数
	/// </summary>
	public class EvolutionCalcResArgs : EventArgs
	{
		public bool Result { get; set; }
		public int Money { get; set; }
		public int Price { get; set; }
		public int AddOnCharge { get; set; }
	}
	static event System.Action<EvolutionCalcResArgs> EvolutionCalcResponse = null;
	/// <summary>
	/// EvolutionCalc 送信
	/// 進化合成試算
	/// </summary>
	public static void SendEvolutionCalc(ulong baseCharaUUID, ulong[] baitCharaUUIDs, System.Action<EvolutionCalcResArgs> response)
	{
		//if (EvolutionCalcResponse == null)
		{
			var packet = new EvolutionCalcReq();
			packet.BasePlayerCharacterUuid = (long)baseCharaUUID;
			packet.BaitPlayerCharacterUuids = baitCharaUUIDs.ToLongArray();
			GameListener.Send(packet);
		}

		EvolutionCalcResponse += response;
	}
	/// <summary>
	/// EvolutionCalc 受信応答
	/// 進化合成試算
	/// </summary>
	public void OperationResponseEvolutionCalc(EvolutionCalcRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		// パケット変換
		var eventArgs = new EvolutionCalcResArgs();
		eventArgs.Result = packet.Result;
		eventArgs.Money = packet.Money;
		eventArgs.Price = packet.Price;
		eventArgs.AddOnCharge = packet.AddOnCharge;

		// 結果を通知する
		if (EvolutionCalcResponse != null)
		{
			EvolutionCalcResponse(eventArgs);
			EvolutionCalcResponse = null;
		}
	}
	#endregion

	#region SynchroFusion パケット
	/// <summary>
	/// SynchroFusion 受信引数
	/// </summary>
	public class SynchroFusionResArgs : EventArgs
	{
		public PowerupResult Result { get; set; }
		public int Money { get; set; }
		public int Price { get; set; }
		public int AddOnCharge { get; set; }
		public CharaInfo CharaInfo { get; set; }
	}
	static event System.Action<SynchroFusionResArgs> SynchroFusionResponse = null;
	/// <summary>
	/// SynchroFusion 送信
	/// シンクロ合成
	/// </summary>
	public static void SendSynchroFusion(ulong baseCharaUUID, ulong baitCharaUUID, System.Action<SynchroFusionResArgs> response)
	{
		//if (SynchroFusionResponse == null)
		{
			var packet = new SynchroFusionReq();
			packet.BasePlayerCharacterUuid = (long)baseCharaUUID;
			packet.BaitPlayerCharacterUuid = (long)baitCharaUUID;
			GameListener.Send(packet);
		}

		SynchroFusionResponse += response;
	}
	/// <summary>
	/// SynchroFusion 受信応答
	/// シンクロ合成
	/// </summary>
	public void OperationResponseSynchroFusion(SynchroFusionRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		// パケット変換
		var eventArgs = new SynchroFusionResArgs();
		eventArgs.Result = packet.PowerupResult;
		eventArgs.Money = packet.Money;
		eventArgs.Price = packet.Price;
		eventArgs.AddOnCharge = packet.AddOnCharge;
		eventArgs.CharaInfo = new CharaInfo(packet.GetParam());

		if (eventArgs.Result == PowerupResult.Fail)
		{
			// 失敗
			GUIChat.AddSystemMessage(true, MasterData.GetText(TextType.TX329_SynchroFusionRes_Fail));
		}

		var t = NetworkController.ServerValue;
		if (t != null)
		{
			t.SetMoney(eventArgs.Money);
		}

		// 結果を通知する
		if (SynchroFusionResponse != null)
		{
			SynchroFusionResponse(eventArgs);
			SynchroFusionResponse = null;
		}
	}
	#endregion

	#region SynchroFusionCalc
	/// <summary>
	/// SynchroFusionCalc 受信引数
	/// </summary>
	public class SynchroFusionCalcResArgs : EventArgs
	{
		public bool Result { get; set; }
		public int Money { get; set; }
		public int Price { get; set; }
		public int AddOnCharge { get; set; }
	}
	static event System.Action<SynchroFusionCalcResArgs> SynchroFusionCalcResponse = null;
	/// <summary>
	/// SynchroFusionCalc 送信
	/// シンクロ合成試算
	/// </summary>
	public static void SendSynchroFusionCalc(ulong baseCharaUUID, ulong baitCharaUUID, System.Action<SynchroFusionCalcResArgs> response)
	{
		//if (SynchroFusionCalcResponse == null)
		{
			var packet = new SynchroFusionCalcReq();
			packet.BasePlayerCharacterUuid = (long)baseCharaUUID;
			packet.BaitPlayerCharacterUuid = (long)baitCharaUUID;
			GameListener.Send(packet);
		}

		SynchroFusionCalcResponse += response;
	}
	/// <summary>
	/// SynchroFusionCalc 受信応答
	/// シンクロ合成試算
	/// </summary>
	public void OperationResponseSynchroFusionCalc(SynchroFusionCalcRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		// パケット変換
		var eventArgs = new SynchroFusionCalcResArgs();
		eventArgs.Result = packet.Result;
		eventArgs.Money = packet.Money;
		eventArgs.Price = packet.Price;
		eventArgs.AddOnCharge = packet.AddOnCharge;

		// 結果を通知する
		if (SynchroFusionCalcResponse != null)
		{
			SynchroFusionCalcResponse(eventArgs);
			SynchroFusionCalcResponse = null;
		}
	}
	#endregion

	#region SetLockPlayerCharacter パケット
	/// <summary>
	/// SetLockPlayerCharacter 受信引数
	/// </summary>
	public class SetLockPlayerCharacterResArgs : EventArgs
	{
		public bool Result { get; set; }
		public ulong UUID { get; set; }
		public bool IsLock { get; set; }
	}
	static event System.Action<SetLockPlayerCharacterResArgs> SetLockPlayerCharacterResponse = null;
	/// <summary>
	/// SetLockPlayerCharacter 送信
	/// ロックキャラクター設定
	/// </summary>
	public static void SendSetLockPlayerCharacter(ulong uuid, bool isLock, System.Action<SetLockPlayerCharacterResArgs> response)
	{
		//if (SetLockPlayerCharacterResponse == null)
		{
			var packet = new SetLockPlayerCharacterReq();
			packet.PlayerCharacterUuid = (long)uuid;
			packet.LockFlag = isLock;
			GameListener.Send(packet);
		}

		SetLockPlayerCharacterResponse += response;
	}
	/// <summary>
	/// SetLockPlayerCharacter 受信応答
	/// ロックキャラクター設定
	/// </summary>
	public void OperationResponseSetLockPlayerCharacter(SetLockPlayerCharacterRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		// パケット変換
		var eventArgs = new SetLockPlayerCharacterResArgs();
		eventArgs.Result = packet.Result;
		eventArgs.UUID = (ulong)packet.PlayerCharacterUuid;
		eventArgs.IsLock = packet.LockFlag;

		if (!eventArgs.Result)
		{
			GUIChat.AddSystemMessage(true, MasterData.GetText(TextType.TX306_CharaLockRes_Fail));
		}

		// 結果を通知する
		if (SetLockPlayerCharacterResponse != null)
		{
			SetLockPlayerCharacterResponse(eventArgs);
			SetLockPlayerCharacterResponse = null;
		}
	}
	#endregion

	#region GetPowerupSlot パケット
	/// <summary>
	/// GetPowerupSlot 受信引数
	/// </summary>
	public class GetPowerupSlotResArgs : EventArgs
	{
		public ulong BaseCharaUUID { get; set; }
		public int BonusHitPoint { get; set; }
		public int BonusAttack { get; set; }
		public int BonusDefense { get; set; }
		public int BonusExtra { get; set; }
		public List<PowerupSlotCharaInfo> SlotInfoList { get; set; }
	}
	static event System.Action<GetPowerupSlotResArgs> GetPowerupSlotResponse = null;
	/// <summary>
	/// GetPowerupSlot 送信
	/// 強化スロット
	/// </summary>
	public static void SendGetPowerupSlot(ulong baseCharaUUID, System.Action<GetPowerupSlotResArgs> response)
	{
		//if (GetPowerupSlotResponse == null)
		{
			var packet = new GetPowerupSlotReq();
			packet.BasePlayerCharacterUuid = (long)baseCharaUUID;
			GameListener.Send(packet);
		}

		GetPowerupSlotResponse += response;
	}
	/// <summary>
	/// GetPowerupSlot 受信応答
	/// 強化スロット
	/// </summary>
	public void OperationResponseGetPowerupSlot(GetPowerupSlotRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		// パケット変換
		var eventArgs = new GetPowerupSlotResArgs();
		eventArgs.BaseCharaUUID = (ulong)packet.BasePlayerCharacterUuid;
		eventArgs.BonusHitPoint = packet.BonusHitPoint;
		eventArgs.BonusAttack = packet.BonusAttack;
		eventArgs.BonusDefense = packet.BonusDefense;
		eventArgs.BonusExtra = packet.BonusExtra;
		eventArgs.SlotInfoList = new List<PowerupSlotCharaInfo>();
		foreach (var p in packet.GetPlayerCharacterPackets())
		{
			eventArgs.SlotInfoList.Add(new PowerupSlotCharaInfo(p));
		}
		eventArgs.SlotInfoList.Sort((a, b) => a.SlotIndex - b.SlotIndex);

		// 結果を通知する
		if (GetPowerupSlotResponse != null)
		{
			GetPowerupSlotResponse(eventArgs);
			GetPowerupSlotResponse = null;
		}
	}
	#endregion

	#region SetPowerupSlot パケット
	/// <summary>
	/// SetPowerupSlot 受信引数
	/// </summary>
	public class SetPowerupSlotResArgs : EventArgs
	{
		public bool Result { get; set; }
		public int Money { get; set; }
		public int Price { get; set; }
		public int AddOnCharge { get; set; }
		public CharaInfo CharaInfo { get; set; }
	}
	static event System.Action<SetPowerupSlotResArgs> SetPowerupSlotResponse = null;
	/// <summary>
	/// SetPowerupSlot 送信
	/// 強化スロット
	/// </summary>
	public static void SendSetPowerupSlot(ulong baseCharaUUID, ulong[] slotCharaUUIDs, System.Action<SetPowerupSlotResArgs> response)
	{
		//if (SetPowerupSlotResponse == null)
		{
			var packet = new SetPowerupSlotReq();
			packet.BasePlayerCharacterUuid = (long)baseCharaUUID;
			packet.SlotPlayerCharacterUuids = slotCharaUUIDs.ToLongArray();
			GameListener.Send(packet);
		}

		SetPowerupSlotResponse += response;
	}
	/// <summary>
	/// SetPowerupSlot 受信応答
	/// 強化スロット
	/// </summary>
	public void OperationResponseSetPowerupSlot(SetPowerupSlotRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		// パケット変換
		var eventArgs = new SetPowerupSlotResArgs();
		eventArgs.Result = packet.Result;
		eventArgs.Money = packet.Money;
		eventArgs.Price = packet.Price;
		eventArgs.AddOnCharge = packet.AddOnCharge;
		eventArgs.CharaInfo = new CharaInfo(packet.GetParam());

		if (!eventArgs.Result)
		{
			//GUIChat.AddSystemMessage(true, MasterData.GetText(TextType.TX307_SetPowerupSlotRes_Fail));
			// TODO:テキストマスター化
			GUIChat.AddSystemMessage(true, "強化スロットの設定に失敗しました。");
		}

		var t = NetworkController.ServerValue;
		if (t != null)
		{
			t.SetMoney(eventArgs.Money);
		}

		// 結果を通知する
		if (SetPowerupSlotResponse != null)
		{
			SetPowerupSlotResponse(eventArgs);
			SetPowerupSlotResponse = null;
		}
	}
	#endregion

	#region SetPowerupSlotCalc パケット
	/// <summary>
	/// SetPowerupSlotCalc 受信引数
	/// </summary>
	public class SetPowerupSlotCalcResArgs : EventArgs
	{
		public bool Result { get; set; }
		public int Money { get; set; }
		public int Price { get; set; }
		public int AddOnCharge { get; set; }
		public int BonusHitPoint { get; set; }
		public int BonusAttack { get; set; }
		public int BonusDefense { get; set; }
		public int BonusExtra { get; set; }
	}
	static event System.Action<SetPowerupSlotCalcResArgs> SetPowerupSlotCalcResponse = null;
	/// <summary>
	/// SetPowerupSlotCalc 送信
	/// 強化スロット試算
	/// </summary>
	public static void SendSetPowerupSlotCalc(ulong baseCharaUUID, ulong[] slotCharaUUIDs, System.Action<SetPowerupSlotCalcResArgs> response)
	{
		//if (SetPowerupSlotCalcResponse == null)
		{
			var packet = new SetPowerupSlotCalcReq();
			packet.BasePlayerCharacterUuid = (long)baseCharaUUID;
			packet.SlotPlayerCharacterUuids = slotCharaUUIDs.ToLongArray();
			GameListener.Send(packet);
		}

		SetPowerupSlotCalcResponse += response;
	}
	/// <summary>
	/// SetPowerupSlotCalc 受信応答
	/// 強化スロット試算
	/// </summary>
	public void OperationResponseSetPowerupSlotCalc(SetPowerupSlotCalcRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		// パケット変換
		var eventArgs = new SetPowerupSlotCalcResArgs();
		eventArgs.Result = packet.Result;
		eventArgs.Money = packet.Money;
		eventArgs.Price = packet.Price;
		eventArgs.AddOnCharge = packet.AddOnCharge;
		eventArgs.BonusHitPoint = packet.BonusHitPoint;
		eventArgs.BonusAttack = packet.BonusAttack;
		eventArgs.BonusDefense = packet.BonusDefense;
		eventArgs.BonusExtra = packet.BonusExtra;

		if (!eventArgs.Result)
		{
			//GUIChat.AddSystemMessage(true, MasterData.GetText(TextType.TX307_SetPowerupSlotRes_Fail));
			// TODO:テキストマスター化
			GUIChat.AddSystemMessage(true, "強化スロットの試算に失敗しました。");
		}

		// 結果を通知する
		if (SetPowerupSlotCalcResponse != null)
		{
			SetPowerupSlotCalcResponse(eventArgs);
			SetPowerupSlotCalcResponse = null;
		}
	}
	#endregion

	#region AddPowerupSlot パケット
	/// <summary>
	/// AddPowerupSlot 受信引数
	/// </summary>
	public class AddPowerupSlotResArgs : EventArgs
	{
		public AddPowerupSlotResult Result { get; set; }
		public int SlotNum { get; set; }
	}
	static event System.Action<AddPowerupSlotResArgs> AddPowerupSlotResponse = null;
	/// <summary>
	/// AddPowerupSlot 送信
	/// 強化スロット追加
	/// </summary>
	public static void SendAddPowerupSlot(ulong baseCharaUUID, System.Action<AddPowerupSlotResArgs> response)
	{
		//if (AddPowerupSlotResponse == null)
		{
			var packet = new AddPowerupSlotReq();
			packet.BasePlayerCharacterUuid = (long)baseCharaUUID;
			GameListener.Send(packet);
		}

		AddPowerupSlotResponse += response;
	}
	/// <summary>
	/// AddPowerupSlot 受信応答
	/// 強化スロット追加
	/// </summary>
	public void OperationResponseAddPowerupSlot(AddPowerupSlotRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		// パケット変換
		var eventArgs = new AddPowerupSlotResArgs();
		eventArgs.Result = packet.AddPowerupSlotResult;
		eventArgs.SlotNum = packet.SlotNum;

		// 結果を通知する
		if (AddPowerupSlotResponse != null)
		{
			AddPowerupSlotResponse(eventArgs);
			AddPowerupSlotResponse = null;
		}
	}
	#endregion

	#region SetPlayerCharacterNewFlagAll パケット
	/// <summary>
	/// SetPlayerCharacterNewFlagAll 受信引数
	/// </summary>
	public class SetPlayerCharacterNewFlagAllResArgs : EventArgs
	{
		public bool Result { get; set; }
	}
	static event System.Action<SetPlayerCharacterNewFlagAllResArgs> SetPlayerCharacterNewFlagAllResponse = null;
	/// <summary>
	/// SetPlayerCharacterNewFlagAll 送信
	/// Newフラグ一括解除
	/// </summary>
	public static void SendSetPlayerCharacterNewFlagAll(System.Action<SetPlayerCharacterNewFlagAllResArgs> response)
	{
		//if (SetPlayerCharacterNewFlagAllResponse == null)
		{
			var packet = new SetPlayerCharacterNewFlagAllReq();
			GameListener.Send(packet);
		}

		SetPlayerCharacterNewFlagAllResponse += response;
	}
	/// <summary>
	/// SetPlayerCharacterNewFlagAll 受信応答
	/// Newフラグ一括解除
	/// </summary>
	public void OperationResponseSetPlayerCharacterNewFlagAll(SetPlayerCharacterNewFlagAllRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		// パケット変換
		var eventArgs = new SetPlayerCharacterNewFlagAllResArgs();
		eventArgs.Result = packet.Result;

		// 結果を通知する
		if (SetPlayerCharacterNewFlagAllResponse != null)
		{
			SetPlayerCharacterNewFlagAllResponse(eventArgs);
			SetPlayerItemNewFlagAllResponse = null;
		}
	}
	#endregion

	#region PlayerItemBox パケット
	/// <summary>
	/// PlayerItemBox 受信引数
	/// </summary>
	public class PlayerItemBoxResArgs : EventArgs
	{
		public int Capacity { get; set; }
		public int Count { get; set; }
	}
	static event System.Action<PlayerItemBoxResArgs> PlayerItemBoxResponse = null;
	/// <summary>
	/// PlayerItemBox 送信
	/// アイテムBOX情報
	/// </summary>
	public static void SendPlayerItemBox(System.Action<PlayerItemBoxResArgs> response)
	{
		//if (PlayerItemBoxResponse == null)
		{
			var packet = new PlayerItemBoxReq();
			GameListener.Send(packet);
		}

		PlayerItemBoxResponse += response;
	}
	/// <summary>
	/// PlayerItemBox 受信応答
	/// アイテムBOX情報
	/// </summary>
	public void OperationResponsePlayerItemBox(PlayerItemBoxRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		// パケット変換
		var eventArgs = new PlayerItemBoxResArgs();
		eventArgs.Capacity = packet.Capacity;
		eventArgs.Count = packet.Count;

		// 結果を通知する
		if (PlayerItemBoxResponse != null)
		{
			PlayerItemBoxResponse(eventArgs);
			PlayerItemBoxResponse = null;
		}
	}
	#endregion

	#region PlayerItemAll パケット
	/// <summary>
	/// PlayerItemAll 受信引数
	/// </summary>
	public class PlayerItemAllResArgs : EventArgs
	{
		public List<ItemInfo> List { get; set; }
	}
	static event System.Action<PlayerItemAllResArgs> PlayerItemAllResponse = null;
	/// <summary>
	/// PlayerItemAll 送信
	/// プレイヤーアイテム全部
	/// </summary>
	public static void SendPlayerItemAll(System.Action<PlayerItemAllResArgs> response)
	{
		//if (PlayerItemAllResponse == null)
		{
			var packet = new PlayerItemAllReq();
			GameListener.Send(packet);
		}

		PlayerItemAllResponse += response;
	}
	/// <summary>
	/// PlayerItemAll 受信応答
	/// プレイヤーアイテム全部
	/// </summary>
	public void OperationResponsePlayerItemAll(PlayerItemAllRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		// パケット変換
		var eventArgs = new PlayerItemAllResArgs();
		eventArgs.List = new List<ItemInfo>();
		foreach (var t in packet.GetPlayerItemParams())
		{
			eventArgs.List.Add(new ItemInfo(t));
		}

		// 結果を通知する
		if (PlayerItemAllResponse != null)
		{
			PlayerItemAllResponse(eventArgs);
			PlayerItemAllResponse = null;
		}
	}
	#endregion

	#region SellMultiPlayerItem パケット
	/// <summary>
	/// SellPlayerMultiItem 受信引数
	/// </summary>
	public class SellMultiPlayerItemResArgs : EventArgs
	{
		public bool Result { get; set; }
		public List<int> IndexList { get; set; }
		public int Money { get; set; }
		public int SoldPrice { get; set; }
	}
	static event System.Action<SellMultiPlayerItemResArgs> SellMultiPlayerItemResponse = null;
	/// <summary>
	/// SellMultiPlayerItem 送信
	/// アイテム売却(複数枠指定用)
	/// </summary>
	public static void SendSellMultiPlayerItem(int[] index, System.Action<SellMultiPlayerItemResArgs> response)
	{
		//if (SellMultiPlayerItemResponse == null)
		{
			var packet = new SellMultiPlayerItemReq();
			packet.Index = index;
			GameListener.Send(packet);
		}

		SellMultiPlayerItemResponse += response;
	}
	/// <summary>
	/// SellMultiPlayerItem 受信応答
	/// アイテム売却(複数枠指定用)
	/// </summary>
	public void OperationResponseSellMultiPlayerItem(SellMultiPlayerItemRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		// パケット変換
		var eventArgs = new SellMultiPlayerItemResArgs();
		eventArgs.Result = packet.Result;
		eventArgs.IndexList = new List<int>(packet.Index);
		eventArgs.Money = packet.Money;
		eventArgs.SoldPrice = packet.SoldPrice;

		var t = NetworkController.ServerValue;
		if (t != null)
		{
			t.SetMoney(eventArgs.Money);
		}

		// 結果を通知する
		if (SellMultiPlayerItemResponse != null)
		{
			SellMultiPlayerItemResponse(eventArgs);
			SellMultiPlayerItemResponse = null;
		}
	}
	#endregion

	#region SetLockPlayerItem パケット
	/// <summary>
	/// SetLockPlayerItem 受信引数
	/// </summary>
	public class SetLockPlayerItemResArgs : EventArgs
	{
		public bool Result { get; set; }
		public int ItemMasterId { get; set; }
		public bool IsLock { get; set; }
	}
	static event System.Action<SetLockPlayerItemResArgs> SetLockPlayerItemResponse = null;
	/// <summary>
	/// SetLockPlayerItem 送信
	/// プレイヤーアイテムロック設定
	/// </summary>
	public static void SendSetLockPlayerItem(int itemMasterId, bool lockFlag, System.Action<SetLockPlayerItemResArgs> response)
	{
		//if (SetLockPlayerItemResponse == null)
		{
			var packet = new SetLockPlayerItemReq();
			packet.ItemMasterId = itemMasterId;
			packet.LockFlag = lockFlag;
			GameListener.Send(packet);
		}

		SetLockPlayerItemResponse += response;
	}
	/// <summary>
	/// SetLockPlayerItem 受信応答
	/// プレイヤーアイテムロック設定
	/// </summary>
	public void OperationResponseSetLockPlayerItem(SetLockPlayerItemRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		// パケット変換
		var eventArgs = new SetLockPlayerItemResArgs();
		eventArgs.Result = packet.Result;
		eventArgs.ItemMasterId = packet.ItemMasterId;
		eventArgs.IsLock = packet.LockFlag;

		// 結果を通知する
		if (SetLockPlayerItemResponse != null)
		{
			SetLockPlayerItemResponse(eventArgs);
			SetLockPlayerItemResponse = null;
		}
	}
	#endregion

	#region SellPlayerItem パケット
	/// <summary>
	/// SellPlayerItem 受信引数
	/// </summary>
	public class SellPlayerItemResArgs : EventArgs
	{
		public bool Result { get; set; }
		public int ItemMasterId { get; set; }
		public int Stack { get; set; }
		public int Money { get; set; }
		public int SoldPrice { get; set; }
	}
	static event System.Action<SellPlayerItemResArgs> SellPlayerItemResponse = null;
	/// <summary>
	/// SellPlayerItem 送信
	/// アイテム売却(単種類用)
	/// </summary>
	public static void SendSellPlayerItem(int itemMasterId, int stack, System.Action<SellPlayerItemResArgs> response)
	{
		//if (SellPlayerItemResponse == null)
		{
			var packet = new SellPlayerItemReq();
			packet.ItemMasterId = itemMasterId;
			packet.Stack = stack;
			GameListener.Send(packet);
		}

		SellPlayerItemResponse += response;
	}
	/// <summary>
	/// SellPlayerItem 受信応答
	/// アイテム売却(単種類用)
	/// </summary>
	public void OperationResponseSellPlayerItem(SellPlayerItemRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		// パケット変換
		var eventArgs = new SellPlayerItemResArgs();
		eventArgs.Result = packet.Result;
		eventArgs.ItemMasterId = packet.ItemMasterId;
		eventArgs.Stack = packet.Stack;
		eventArgs.Money = packet.Money;
		eventArgs.SoldPrice = packet.SoldPrice;

		var t = NetworkController.ServerValue;
		if (t != null)
		{
			t.SetMoney(eventArgs.Money);
		}

		// 結果を通知する
		if (SellPlayerItemResponse != null)
		{
			SellPlayerItemResponse(eventArgs);
			SellPlayerItemResponse = null;
		}
	}
	#endregion

	#region SetPlayerItemNewFlagAll パケット
	/// <summary>
	/// SetPlayerItemNewFlagAll 受信引数
	/// </summary>
	public class SetPlayerItemNewFlagAllResArgs : EventArgs
	{
		public bool Result { get; set; }
	}
	static event System.Action<SetPlayerItemNewFlagAllResArgs> SetPlayerItemNewFlagAllResponse = null;
	/// <summary>
	/// SetPlayerItemNewFlagAll 送信
	/// Newフラグ一括解除
	/// </summary>
	public static void SendSetPlayerItemNewFlagAll(System.Action<SetPlayerItemNewFlagAllResArgs> response)
	{
		//if (SetPlayerItemNewFlagAllResponse == null)
		{
			var packet = new SetPlayerItemNewFlagAllReq();
			GameListener.Send(packet);
		}

		SetPlayerItemNewFlagAllResponse += response;
	}
	/// <summary>
	/// SetPlayerItemNewFlagAll 受信応答
	/// Newフラグ一括解除
	/// </summary>
	public void OperationResponseSetPlayerItemNewFlagAll(SetPlayerItemNewFlagAllRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		// パケット変換
		var eventArgs = new SetPlayerItemNewFlagAllResArgs();
		eventArgs.Result = packet.Result;

		// 結果を通知する
		if (SetPlayerItemNewFlagAllResponse != null)
		{
			SetPlayerItemNewFlagAllResponse(eventArgs);
			SetPlayerItemNewFlagAllResponse = null;
		}
	}
	#endregion

	#region MailBox パケット

	/// <summary>
	/// メール件数取得データ
	/// </summary>
	public class MailBoxResArgs : EventArgs
	{
		public class MailBoxInfo
		{
			/// <summary>
			/// メールタイプ
			/// </summary>
			public XUI.Mail.MailTabType Type { get; private set; }

			/// <summary>
			/// トータル件数
			/// </summary>
			public int Total { get; private set; }

			/// <summary>
			/// 未読件数
			/// </summary>
			public int Unread { get; private set; }

			/// <summary>
			/// ロック件数
			/// </summary>
			public int Locked { get; private set; }

			public MailBoxInfo()
			{

			}
			public MailBoxInfo(MailBoxParameter param)
			{
				// 変換
				Type = (XUI.Mail.MailTabType)param.MailBoxType;
				Total = param.Total;
				Unread = param.Unread;
				Locked = param.Locked;
			}
		}

		private List<MailBoxInfo> list = new List<MailBoxInfo>();

		public List<MailBoxInfo> List { get { return list; } }
	}

	private static event Action<MailBoxResArgs> MailBoxResponse = null;

	/// <summary>
	/// メール件数取得 送信
	/// </summary>
	public static void SendMailBox(Action<MailBoxResArgs> response)
	{
		//if(MailBoxResponse == null)
		{
			var packet = new MailBoxReq();
			GameListener.Send(packet);
		}

		MailBoxResponse += response;
	}

	/// <summary>
	/// メール件数取得 受信
	/// </summary>
	/// <param name="packet"></param>
	public static void OperationResponseMailBox(MailBoxRes packet)
	{
		if(packet == null) return;

		var args = new MailBoxResArgs();
		foreach(var p in packet.GetMailBoxParams()) {
			args.List.Add(new MailBoxResArgs.MailBoxInfo(p));
		}

		if(MailBoxResponse != null) {
			MailBoxResponse(args);
			MailBoxResponse = null;
		}
	}

	#endregion


	#region AdminMail パケット

	/// <summary>
	/// 運営メールデータ
	/// </summary>
	public class AdminMailResArgs : EventArgs
	{
		/// <summary>
		/// 開始位置
		/// </summary>
		public int Start { get; set; }
		
		/// <summary>
		/// 取得カウント
		/// </summary>
		public int Count { get; set; }

		// メールデータ。
		private List<MailInfo> list = new List<MailInfo>();

		public List<MailInfo> List { get { return list; } }
	}

	private static event Action<AdminMailResArgs> AdminMailResponse = null;

	/// <summary>
	/// 運営メール取得 送信
	/// </summary>
	public static void SendAdminMail(int start, int count, bool deleted, Action<AdminMailResArgs> response)
	{
		//if(AdminMailResponse == null)
		{
			var packet = new AdminMailReq()
			{
				Start = start,
				Count = count,
				Deleted = deleted
			};
			GameListener.Send(packet);
		}

		AdminMailResponse += response;
	}

	/// <summary>
	/// 運営メール取得 受信
	/// </summary>
	/// <param name="packet"></param>
	public static void OperationResponseAdminMail(AdminMailRes packet)
	{
		if(packet == null) return;
		
		var args = new AdminMailResArgs()
		{
			Start = packet.Start,
			Count = packet.Count,
		};

		// ローカルインデックス付きで作成
		var mails = packet.GetMailParams();
		for(int i=0;i<mails.Length;i++) {
			args.List.Add(new MailInfo(mails[i], packet.Start + i, MailInfo.MailType.Admin));
		}

		if(AdminMailResponse != null) {
			AdminMailResponse(args);
			AdminMailResponse = null;
		}
	}

	#endregion
	

	#region SetAdminMailReadFlag パケット

	/// <summary>
	/// 運営メール既読
	/// </summary>
	public class SetAdminMailReadFlagResArgs : EventArgs
	{
		/// <summary>
		/// インデックス
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// フラグ
		/// </summary>
		public bool ReadFlag { get; set; }
		
	}
	
	private static event Action<SetAdminMailReadFlagResArgs> SetAdminMailReadFlagResponse = null;

	/// <summary>
	/// 運営メール既読 送信
	/// </summary>
	public static void SendSetAdminMailReadFlag(int index, Action<SetAdminMailReadFlagResArgs> response)
	{
		//if(SetAdminMailReadFlagResponse == null)
		{
			var packet = new SetAdminMailReadFlagReq()
			{
				Index = index
			};
			GameListener.Send(packet);
		}

		SetAdminMailReadFlagResponse += response;
	}

	/// <summary>
	/// 運営メール既読 受信
	/// </summary>
	/// <param name="packet"></param>
	public static void OperationResponseSetAdminMailReadFlag(SetAdminMailReadFlagRes packet)
	{
		if(packet == null) return;

		var args = new SetAdminMailReadFlagResArgs()
		{
			Index = packet.Index,
			ReadFlag = packet.ReadFlag
		};
		
		if(SetAdminMailReadFlagResponse != null) {
			SetAdminMailReadFlagResponse(args);
			SetAdminMailReadFlagResponse = null;
		}
	}

	#endregion

	#region SetAdminMailReadFlagAll パケット

	/// <summary>
	/// 運営メール全既読
	/// </summary>
	public class SetAdminMailReadFlagAllResArgs : EventArgs
	{
		/// <summary>
		/// フラグ
		/// </summary>
		public bool Result { get; set; }

		public int Count { get; set; }
	}

	private static event Action<SetAdminMailReadFlagAllResArgs> SetAdminMailReadFlagAllResponse = null;

	/// <summary>
	/// 運営メール全既読 送信
	/// </summary>
	public static void SendSetAdminMailReadFlagAll(Action<SetAdminMailReadFlagAllResArgs> response)
	{
		//if(SetAdminMailReadFlagAllResponse == null)
		{
			var packet = new SetAdminMailReadFlagAllReq();
			GameListener.Send(packet);
		}

		SetAdminMailReadFlagAllResponse += response;
	}

	/// <summary>
	/// 運営メール全既読 受信
	/// </summary>
	/// <param name="packet"></param>
	public static void OperationResponseSetAdminMailReadFlagAll(SetAdminMailReadFlagAllRes packet)
	{
		if(packet == null) return;

		var args = new SetAdminMailReadFlagAllResArgs()
		{
			Result = packet.Result,
			Count = packet.ReadCount
		};

		if(SetAdminMailReadFlagAllResponse != null) {
			SetAdminMailReadFlagAllResponse(args);
			SetAdminMailReadFlagAllResponse = null;
		}
	}

	#endregion

	#region SetLockAdminMail パケット

	/// <summary>
	/// 運営メールロック
	/// </summary>
	public class SetLockAdminMailResArgs : EventArgs
	{
		/// <summary>
		/// 結果
		/// </summary>
		public MailLockResult Result { get; set; }

		/// <summary>
		/// インデックス
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// フラグ
		/// </summary>
		public bool Locked { get; set; }

	}

	private static event Action<SetLockAdminMailResArgs> SetLockAdminMailResponse = null;

	/// <summary>
	/// 運営メールロック 送信
	/// </summary>
	public static void SendSetLockAdminMail(int index, bool locked, Action<SetLockAdminMailResArgs> response)
	{
		//if(SetLockAdminMailResponse == null)
		{
			var packet = new SetLockAdminMailReq()
			{
				Index = index,
				LockFlag = locked
			};
			GameListener.Send(packet);
		}

		SetLockAdminMailResponse += response;
	}

	/// <summary>
	/// 運営メールロック 受信
	/// </summary>
	/// <param name="packet"></param>
	public static void OperationResponseSetLockAdminMail(SetLockAdminMailRes packet)
	{
		if(packet == null) return;

		var args = new SetLockAdminMailResArgs()
		{
			Result = packet.MailLockResult,
			Index = packet.Index,
			Locked = packet.LockFlag
		};

		if(SetLockAdminMailResponse != null) {
			SetLockAdminMailResponse(args);
			SetLockAdminMailResponse = null;
		}
	}

	#endregion

	#region DeleteAdminMail パケット

	/// <summary>
	/// 運営メール削除
	/// </summary>
	public class DeleteAdminMailResArgs : EventArgs
	{
		/// <summary>
		/// インデックス
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// フラグ
		/// </summary>
		public bool Result { get; set; }

	}

	private static event Action<DeleteAdminMailResArgs> DeleteAdminMailResponse = null;

	/// <summary>
	/// 運営メール削除 送信
	/// </summary>
	public static void SendDeleteAdminMail(int index, Action<DeleteAdminMailResArgs> response)
	{
		//if(DeleteAdminMailResponse == null)
		{
			var packet = new DeleteAdminMailReq()
			{
				Index = index,
			};
			GameListener.Send(packet);
		}

		DeleteAdminMailResponse += response;
	}

	/// <summary>
	/// 運営メール削除 受信
	/// </summary>
	/// <param name="packet"></param>
	public static void OperationResponseDeleteAdminMail(DeleteAdminMailRes packet)
	{
		if(packet == null) return;

		var args = new DeleteAdminMailResArgs()
		{
			Index = packet.Index,
			Result = packet.Result
		};

		if(DeleteAdminMailResponse != null) {
			DeleteAdminMailResponse(args);
			DeleteAdminMailResponse = null;
		}
	}

	#endregion

	#region DeleteAdminMailAll パケット

	/// <summary>
	/// 運営メール全削除
	/// </summary>
	public class DeleteAdminMailAllResArgs : EventArgs
	{
		/// <summary>
		/// フラグ
		/// </summary>
		public bool Result { get; set; }

		/// <summary>
		/// 削除件数
		/// </summary>
		public int Count { get; set; }
	}

	private static event Action<DeleteAdminMailAllResArgs> DeleteAdminMailAllResponse = null;

	/// <summary>
	/// 運営メール全削除 送信
	/// </summary>
	public static void SendDeleteAdminMailAll(Action<DeleteAdminMailAllResArgs> response)
	{
		//if(DeleteAdminMailAllResponse == null)
		{
			var packet = new DeleteAdminMailAllReq();
			GameListener.Send(packet);
		}

		DeleteAdminMailAllResponse += response;
	}

	/// <summary>
	/// 運営メール全削除 受信
	/// </summary>
	/// <param name="packet"></param>
	public static void OperationResponseDeleteAdminMailAll(DeleteAdminMailAllRes packet)
	{
		if(packet == null) return;

		var args = new DeleteAdminMailAllResArgs()
		{
			Result = packet.Result,
			Count = packet.DeleteCount
		};

		if(DeleteAdminMailAllResponse != null) {
			DeleteAdminMailAllResponse(args);
			DeleteAdminMailAllResponse = null;
		}
	}

	#endregion


	#region PresentMail パケット

	/// <summary>
	/// アイテムメールデータ
	/// </summary>
	public class PresentMailResArgs : EventArgs
	{
		/// <summary>
		/// 開始位置
		/// </summary>
		public int Start { get; set; }

		/// <summary>
		/// 取得カウント
		/// </summary>
		public int Count { get; set; }

		// メールデータ。
		private List<MailInfo> list = new List<MailInfo>();

		public List<MailInfo> List { get { return list; } }
	}

	private static event Action<PresentMailResArgs> PresentMailResponse = null;

	/// <summary>
	/// アイテムメール取得 送信
	/// </summary>
	public static void SendPresentMail(int start, int count, bool deleted, Action<PresentMailResArgs> response)
	{
		//if(PresentMailResponse == null)
		{
			var packet = new PresentMailReq()
			{
				Start = start,
				Count = count,
				Deleted = deleted
			};
			GameListener.Send(packet);
		}

		PresentMailResponse += response;
	}

	/// <summary>
	/// アイテムメールデータ取得 受信
	/// </summary>
	/// <param name="packet"></param>
	public static void OperationResponsePresentMail(PresentMailRes packet)
	{
		if(packet == null) return;

		var args = new PresentMailResArgs()
		{
			Start = packet.Start,
			Count = packet.Count,
		};

		// ローカルインデックス付きで作成
		var mails = packet.GetMailParams();
		for(int i = 0; i < mails.Length; i++) {
			args.List.Add(new MailInfo(mails[i], packet.Start + i, MailInfo.MailType.Present));
		}

		if(PresentMailResponse != null) {
			PresentMailResponse(args);
			PresentMailResponse = null;
		}
	}

	#endregion
	
	#region SetPresentMailReadFlag パケット

	/// <summary>
	/// プレゼントメール既読
	/// </summary>
	public class SetPresentMailReadFlagResArgs : EventArgs
	{
		/// <summary>
		/// インデックス
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// フラグ
		/// </summary>
		public bool ReadFlag { get; set; }

	}

	private static event Action<SetPresentMailReadFlagResArgs> SetPresentMailReadFlagResponse = null;

	/// <summary>
	/// プレゼントメール既読 送信
	/// </summary>
	public static void SendSetPresentMailReadFlag(int index, Action<SetPresentMailReadFlagResArgs> response)
	{
		//if(SetPresentMailReadFlagResponse == null)
		{
			var packet = new SetPresentMailReadFlagReq()
			{
				Index = index
			};
			GameListener.Send(packet);
		}

		SetPresentMailReadFlagResponse += response;
	}

	/// <summary>
	/// プレゼントメール既読 受信
	/// </summary>
	/// <param name="packet"></param>
	public static void OperationResponseSetPresentMailReadFlag(SetPresentMailReadFlagRes packet)
	{
		if(packet == null) return;

		var args = new SetPresentMailReadFlagResArgs()
		{
			Index = packet.Index,
			ReadFlag = packet.ReadFlag
		};

		if(SetPresentMailReadFlagResponse != null) {
			SetPresentMailReadFlagResponse(args);
			SetPresentMailReadFlagResponse = null;
		}
	}

	#endregion
	
	#region SetLockPresentMail パケット

	/// <summary>
	/// アイテムメールロック
	/// </summary>
	public class SetLockPresentMailResArgs : EventArgs
	{
		/// <summary>
		/// 結果
		/// </summary>
		public MailLockResult Result { get; set; }

		/// <summary>
		/// インデックス
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// フラグ
		/// </summary>
		public bool Locked { get; set; }

	}

	private static event Action<SetLockPresentMailResArgs> SetLockPresentMailResponse = null;

	/// <summary>
	/// アイテムメールロック 送信
	/// </summary>
	public static void SendSetLockPresentMail(int index, bool locked, Action<SetLockPresentMailResArgs> response)
	{
		//if(SetLockPresentMailResponse == null)
		{
			var packet = new SetLockPresentMailReq()
			{
				Index = index,
				LockFlag = locked
			};
			GameListener.Send(packet);
		}

		SetLockPresentMailResponse += response;
	}

	/// <summary>
	/// アイテムメールロック 受信
	/// </summary>
	/// <param name="packet"></param>
	public static void OperationResponseSetLockPresentMail(SetLockPresentMailRes packet)
	{
		if(packet == null) return;

		var args = new SetLockPresentMailResArgs()
		{
			Result = packet.MailLockResult,
			Index = packet.Index,
			Locked = packet.LockFlag
		};

		if(SetLockPresentMailResponse != null) {
			SetLockPresentMailResponse(args);
			SetLockPresentMailResponse = null;
		}
	}

	#endregion

	#region DeletePresentMail パケット

	/// <summary>
	/// アイテムメール削除
	/// </summary>
	public class DeletePresentMailResArgs : EventArgs
	{
		/// <summary>
		/// インデックス
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// フラグ
		/// </summary>
		public bool Result { get; set; }

	}

	private static event Action<DeletePresentMailResArgs> DeletePresentMailResponse = null;

	/// <summary>
	/// アイテムメール削除 送信
	/// </summary>
	public static void SendDeletePresentMail(int index, Action<DeletePresentMailResArgs> response)
	{
		//if(DeletePresentMailResponse == null)
		{
			var packet = new DeletePresentMailReq()
			{
				Index = index,
			};
			GameListener.Send(packet);
		}

		DeletePresentMailResponse += response;
	}

	/// <summary>
	/// アイテムメール削除 受信
	/// </summary>
	/// <param name="packet"></param>
	public static void OperationResponseDeletePresentMail(DeletePresentMailRes packet)
	{
		if(packet == null) return;

		var args = new DeletePresentMailResArgs()
		{
			Index = packet.Index,
			Result = packet.Result
		};

		if(DeletePresentMailResponse != null) {
			DeletePresentMailResponse(args);
			DeletePresentMailResponse = null;
		}
	}

	#endregion

	#region DeletePresentMailAll パケット

	/// <summary>
	/// アイテムメール削除
	/// </summary>
	public class DeletePresentMailAllResArgs : EventArgs
	{
		/// <summary>
		/// フラグ
		/// </summary>
		public bool Result { get; set; }

		/// <summary>
		/// 削除件数
		/// </summary>
		public int Count { get; set; }
	}

	private static event Action<DeletePresentMailAllResArgs> DeletePresentMailAllResponse = null;

	/// <summary>
	/// アイテムメール削除 送信
	/// </summary>
	public static void SendDeletePresentMailAll(Action<DeletePresentMailAllResArgs> response)
	{
		//if(DeletePresentMailAllResponse == null)
		{
			var packet = new DeletePresentMailAllReq();
			GameListener.Send(packet);
		}

		DeletePresentMailAllResponse += response;
	}

	/// <summary>
	/// アイテムメール削除 受信
	/// </summary>
	/// <param name="packet"></param>
	public static void OperationResponseDeletePresentMailAll(DeletePresentMailAllRes packet)
	{
		if(packet == null) return;

		var args = new DeletePresentMailAllResArgs()
		{
			Result = packet.Result,
			Count = packet.DeleteCount
		};

		if(DeletePresentMailAllResponse != null) {
			DeletePresentMailAllResponse(args);
			DeletePresentMailAllResponse = null;
		}
	}

	#endregion

	#region ReceivePresentMailItem パケット

	/// <summary>
	/// メールアイテム受け取り
	/// </summary>
	public class ReceivePresentMailItemResArgs : EventArgs
	{
		public ReceivePresentMailItemResult Result { get; set; }

	}

	private static event Action<ReceivePresentMailItemResArgs> ReceivePresentMailItemResponse = null;

	/// <summary>
	/// メールアイテム受け取り 送信
	/// </summary>
	public static void SendReceivePresentMailItem(int index, Action<ReceivePresentMailItemResArgs> response)
	{
		//if(ReceivePresentMailItemResponse == null)
		{
			var packet = new ReceivePresentMailItemReq()
			{
				Index = index
			};
			GameListener.Send(packet);
		}

		ReceivePresentMailItemResponse += response;
	}

	/// <summary>
	/// メールアイテム受け取り 受信
	/// </summary>
	/// <param name="packet"></param>
	public static void OperationResponseReceivePresentMailItem(ReceivePresentMailItemRes packet)
	{
		if(packet == null) return;

		var param = packet.GetPresentMailItemParam();
		var args = new ReceivePresentMailItemResArgs()
		{
			Result = param.ReceivePresentMailItemResult
		};

		if(ReceivePresentMailItemResponse != null) {
			ReceivePresentMailItemResponse(args);
			ReceivePresentMailItemResponse = null;
		}
	}

	#endregion

	#region ReceivePresentMailItemAll パケット

	/// <summary>
	/// メールアイテム全受け取り
	/// </summary>
	public class ReceivePresentMailItemAllResArgs : EventArgs
	{
		/// <summary>
		/// 受け取り件数
		/// </summary>
		public int Count { get; set; }

		/// <summary>
		/// 期限切れ件数
		/// </summary>
		public int ExpirationCount { get; set; }
	}

	private static event Action<ReceivePresentMailItemAllResArgs> ReceivePresentMailItemAllResponse = null;

	/// <summary>
	/// メールアイテム全受け取り 送信
	/// </summary>
	public static void SendReceivePresentMailItemAll(Action<ReceivePresentMailItemAllResArgs> response)
	{
		//if(ReceivePresentMailItemAllResponse == null)
		{
			var packet = new ReceivePresentMailItemAllReq();
			GameListener.Send(packet);
		}

		ReceivePresentMailItemAllResponse += response;
	}

	/// <summary>
	/// メールアイテム全受け取り 受信
	/// </summary>
	/// <param name="packet"></param>
	public static void OperationResponseReceivePresentMailItemAll(ReceivePresentMailItemAllRes packet)
	{
		if(packet == null) return;

		var args = new ReceivePresentMailItemAllResArgs()
		{
			Count = packet.ReceiveCount
		};
		
		// 期限切れで失敗した件数
		var results = packet.GetPresentMailItemParams();
		for(int i = 0; i < results.Length; i++) {
			if(results[i].ReceivePresentMailItemResult == ReceivePresentMailItemResult.Expiration) {
				args.ExpirationCount++;
			}
		}

		if(ReceivePresentMailItemAllResponse != null) {
			ReceivePresentMailItemAllResponse(args);
			ReceivePresentMailItemAllResponse = null;
		}
	}

	#endregion


	#region Achievement パケット

	/// <summary>
	/// アチーブ一覧取得
	/// </summary>
	public class AchievementResArgs : EventArgs {

		/// <summary>
		/// アチーブメント未取得数
		/// </summary>
		public int AchievemrntRewardCount { get; set; }

		/// <summary>
		/// アチーブメント情報
		/// </summary>
		private List<AchievementInfo> list = new List<AchievementInfo>();
		public List<AchievementInfo> List { get { return list; } }
	}

	private static event Action<AchievementResArgs> AchievementResponse = null;

	/// <summary>
	/// アチーブ一覧取得 送信
	/// </summary>
	public static void SendAchievement( Action<AchievementResArgs> response ) {

		if( AchievementResponse == null ) {
			var packet = new AchievementReq();
			GameListener.Send( packet );
		}

		AchievementResponse += response;
	}

	/// <summary>
	/// アチーブ一覧取得 受信
	/// </summary>
	public static void OperationResponseAchievement( AchievementRes packet ) {

		if( packet == null ) return;

		var param = packet.GetAchievementParams();
		var args = new AchievementResArgs() {
			AchievemrntRewardCount = packet.UnreceivedRewardCount
		};

		// パケット変換
		foreach( AchievementParameter p in param ) {
			args.List.Add( new AchievementInfo( p ));
		}

		if( AchievementResponse != null ) {
			AchievementResponse( args );
			AchievementResponse = null;
		}
	}

	#endregion

	#region CompleteAchievement パケット

	private static string CompleteAchievementString = "アチーブメント「{0}」を達成しました！";

	/// <summary>
	/// 新たに達成したアチーブの通知 受信
	/// </summary>
	public static void OperationResponseCompleteAchievement( CompleteAchievementEvent packet ) {

		if( packet == null ) return;

		var AchievementMasterIds = packet.AchievementMasterIds;

		// とりあえず直接チャットへ ※仮実装
		int max = AchievementMasterIds.Length;
		Scm.Common.XwMaster.AchievementMasterData data;
		for( int i = 0 ; i < max ; i++ ) {
			if( MasterData.TryGetAchievement( AchievementMasterIds[i], out data ) ) {
				GUIChat.AddSystemMessage( false, String.Format( CompleteAchievementString, data.Title ));
			}
		}
	}

	#endregion

	#region ReceiveAchievementReward パケット

	/// <summary>
	/// アチーブ報酬受取
	/// </summary>
	public class ReceiveAchievementRewardResArgs : EventArgs {

		/// <summary>
		/// アチーブメント未取得数
		/// </summary>
		public int AchievemrntRewardCount { get; set; }

		/// <summary>
		/// アチーブメント報酬リスト
		/// </summary>
		private List<AchievementRewardInfo> list = new List<AchievementRewardInfo>();
		public List<AchievementRewardInfo> List { get { return list; } }
	}

	private static event Action<ReceiveAchievementRewardResArgs> ReceiveAchievementRewardResponse = null;

	/// <summary>
	/// アチーブ報酬受取 送信
	/// </summary>
	public static void SendReceiveAchievementReward( int achievementMasterId, Action<ReceiveAchievementRewardResArgs> response ) {

		if( ReceiveAchievementRewardResponse == null ) {
			var packet = new ReceiveAchievementRewardReq() {
				AchievementMasterId = achievementMasterId
			};
			GameListener.Send( packet );
		}

		ReceiveAchievementRewardResponse += response;
	}

	/// <summary>
	/// アチーブ報酬受取 受信
	/// </summary>
	/// <param name="packet"></param>
	public static void OperationResponseReceiveAchievementReward( ReceiveAchievementRewardRes packet ) {

		if( packet == null ) return;

		var param = packet.GetAchievementRewardParams();
		var args = new ReceiveAchievementRewardResArgs() {
			AchievemrntRewardCount = packet.UnreceivedRewardCount
		};

		// パケット変換
		foreach( AchievementRewardParameter p in param ) {
			args.List.Add( new AchievementRewardInfo( p ));
		}

		if( ReceiveAchievementRewardResponse != null ) {
			ReceiveAchievementRewardResponse( args );
			ReceiveAchievementRewardResponse = null;
		}
	}

	#endregion

	#region ReceiveAchievementRewardAll パケット

	/// <summary>
	/// アチーブ報酬一括受取
	/// </summary>
	public class ReceiveAchievementRewardAllResArgs : EventArgs {

		public int ReceiveCount { get; set; }
		public AchievementRewardParameter[] Parameters { get; set; }
	}

	private static event Action<ReceiveAchievementRewardAllResArgs> ReceiveAchievementRewardAllResponse = null;

	/// <summary>
	/// アチーブ報酬一括受取 送信
	/// </summary>
	public static void SendReceiveAchievementRewardAll( Action<ReceiveAchievementRewardAllResArgs> response ) {

		if( ReceiveAchievementRewardAllResponse == null ) {
			var packet = new ReceiveAchievementRewardAllReq();
			GameListener.Send( packet );
		}

		ReceiveAchievementRewardAllResponse += response;
	}

	/// <summary>
	/// アチーブ報酬一括受取 受信
	/// </summary>
	public static void OperationResponseReceiveAchievementRewardAll( ReceiveAchievementRewardAllRes packet ) {

		if( packet == null ) return;

		var args = new ReceiveAchievementRewardAllResArgs() {
			ReceiveCount = packet.ReceiveCount,
			Parameters = packet.GetAchievementRewardParams()
		};

		if( ReceiveAchievementRewardAllResponse != null ) {
			ReceiveAchievementRewardAllResponse( args );
			ReceiveAchievementRewardAllResponse = null;
		}
	}

	#endregion

	#region ReceiveWebStore パケット

	/// <summary>
	/// Webストア購入情報受取
	/// </summary>
	public class ReceiveWebStoreResArgs : EventArgs
	{

		public bool Result { get; set; }
	}

	private static event Action<ReceiveWebStoreResArgs> ReceiveWebStoreResponse = null;

	/// <summary>
	/// Webストア購入情報受取 送信
	/// </summary>
	public static void SendReceiveWebStore(Action<ReceiveWebStoreResArgs> response)
	{

		//if (ReceiveWebStoreResponse == null)
		{
			var packet = new ReceiveWebStoreReq();
			GameListener.Send(packet);
		}

		ReceiveWebStoreResponse += response;
	}

	/// <summary>
	/// Webストア購入情報受取 受信
	/// </summary>
	public static void OperationResponseReceiveWebStore(ReceiveWebStoreRes packet)
	{

		if (packet == null) return;

		var args = new ReceiveWebStoreResArgs()
		{
			Result = packet.Result,
		};

		if (ReceiveWebStoreResponse != null)
		{
			ReceiveWebStoreResponse(args);
			ReceiveWebStoreResponse = null;
		}
	}

    public class LoginBonuseAllResArgs : EventArgs
    {
        public int ReceiveCount { get; set; }
        public LoginBonusParameter[] Parameters { get; set; }
    }

    private static event Action<LoginBonuseAllResArgs> LoginBonuseListResponse = null;
    /// <summary>
    /// 请求登陆奖励列表
    /// </summary>
    public static void SendLoginBonuseList(Action<LoginBonuseAllResArgs> response)
    {
        var packet = new LoginBonusReq();
        GameListener.Send(packet);

        LoginBonuseListResponse += response;
    }

    /// <summary>
    /// 返回登陆奖励列表
    /// </summary>
    public static void OperationResponseLoginBonuseList(LoginBonusRes packet)
    {
        if (packet == null) return;
        LoginBonusParameter[] result = packet.GetLoginBonusParams();
        var args = new LoginBonuseAllResArgs()
        {
            ReceiveCount = packet.GetLoginBonusParams().Length,
            Parameters = packet.GetLoginBonusParams(),
        };

        if (LoginBonuseListResponse != null)
        {
            LoginBonuseListResponse(args);
            LoginBonuseListResponse = null;
        }
    }


    private static event Action<LoginBonusRewardRes> LoginBonuseRewardResponse = null;
    /// <summary>
    /// 领取登陆奖励
    /// </summary>
    public static void SendLoginBonuseReward(Action<LoginBonusRewardRes> response)
    {
        var packet = new LoginBonusRewardReq();
        GameListener.Send(packet);

        LoginBonuseRewardResponse += response;
    }

    /// <summary>
    /// 返回登陆奖励
    /// </summary>
    public static void OperationResponseLoginBonuseReward(LoginBonusRewardRes packet)
    {
        if (packet == null) return;
        
        if (LoginBonuseRewardResponse != null)
        {
            LoginBonuseRewardResponse(packet);
            LoginBonuseRewardResponse = null;
        }
    }

	#endregion

    #region FriendApply
    public static void EventFriend(FriendEvent e)
    {
        
        switch (e.Command)
        {
            case FriendEvent.EVENT_ADD_FRIEND_REQUEST:
                Debug.Log("Friend Apply!");
                //Net.Network.Instance.StartCoroutine(XUI.Friends.FriendsController.Instance.GetApplyList());
                Net.Network.Instance.StartCoroutine(XDATA.PlayerData.Instance.GetApplyList());
                GUIMessageWindow.SetModeOK("有人申请添加好友！", null);
                break;
            case FriendEvent.EVENT_REQUEST_ACCEPTED:
                Debug.Log("Friend Apply Accepted!");
                //Net.Network.Instance.StartCoroutine(XUI.Friends.FriendsController.Instance.GetFriendsList());
                Net.Network.Instance.StartCoroutine(XDATA.PlayerData.Instance.GetFriendsList());
                break;
            case FriendEvent.EVENT_DELETE_FRIEND:
                Debug.Log("Friend Deleted!");
                Net.Network.Instance.StartCoroutine(XDATA.PlayerData.Instance.GetFriendsList());
                break;
        }
        Debug.Log("has applyed!");
    }

    #endregion

    #region Get guide
    /// <summary>
    /// get guide 
    /// </summary>
    public class GetGuideResArgs : EventArgs
    {
        public bool result { get; set; }
        public int step { get; set; }
        
    }
    static event System.Action<GetGuideResArgs> GetGuideResponse = null;

    /// <summary>
    /// get guide
    /// </summary>
    /// <param name="response"></param>
    public static void SendGetGuide(System.Action<GetGuideResArgs> response)
    {
        var packet = new GetGuideReq();
        GameListener.Send(packet);

        GetGuideResponse += response;
    }

    /// <summary>
    /// response get guide
    /// </summary>
    /// <param name="packet"></param>
    public static void OperationResponseGetGuide(GetGuideRes packet)
    {
        if (packet == null)
            return;

        var eventArgs = new GetGuideResArgs();
        eventArgs.step = packet.Step;
        eventArgs.result = packet.Result;
        if (GetGuideResponse != null)
        {
            GetGuideResponse(eventArgs);
            GetGuideResponse = null;
        }
    }
    #endregion

    #region Set guide
    /// <summary>
    /// get guide 
    /// </summary>
    public class SetGuideResArgs : EventArgs
    {
        public bool result { get; set; }
        public int step { get; set; }
    }
    static event System.Action<SetGuideResArgs> SetGuideResponse = null;

    /// <summary>
    /// get guide
    /// </summary>
    /// <param name="step"></param>
    /// <param name="response"></param>
    public static void SendSetGuide(int step, System.Action<SetGuideResArgs> response)
    {
        var packet = new SetGuideReq();
        packet.Step = step;
        GameListener.Send(packet);

        SetGuideResponse += response;
    }

    /// <summary>
    /// response get guide
    /// </summary>
    /// <param name="packet"></param>
    public static void OperationResponseSetGuide(SetGuideRes packet)
    {
        if (packet == null)
            return;

        var eventArgs = new SetGuideResArgs();
        eventArgs.result = packet.Result;
        eventArgs.step = packet.Step;

        if (SetGuideResponse != null)
        {
            SetGuideResponse(eventArgs);
            SetGuideResponse = null;
        }
    }
    #endregion

    #region Recruitment
    public static void EventRecruitment(RecruitmentEvent e) {
        foreach (var rec in e.GetUpdatedRecruitments()) {
            Debug.Log("<color=#00ff00> update rec:" + rec.InFieldId + ",text=" + rec.Text + "<color>");
            Entrant.UpdateRecruitment(rec);
        }
        foreach (var rec in e.GetDeletedRecruitments()) {
            Entrant.RemoveRecruitment(rec);
        }
    }

    public static void OperationResponseRecruitment(RecruitmentRes res) {

    }
    #endregion

    #region Gift Package
    private static event Action<GiftPackageRes> GiftPackageResponse = null;
    /// <summary>
    /// 领取登陆奖励
    /// </summary>
    public static void SendGiftPackage(string code, Action<GiftPackageRes> response)
    {
        var packet = new GiftPackageReq()
        {
            GiftCode = code,
        };
        GameListener.Send(packet);

        GiftPackageResponse += response;
    }

    /// <summary>
    /// 返回登陆奖励
    /// </summary>
    public static void OperationResponseGiftPackage(GiftPackageRes packet)
    {
        if (packet == null) return;

        if (GiftPackageResponse != null)
        {
            GiftPackageResponse(packet);
            GiftPackageResponse = null;
        }
    }

    #endregion
}
