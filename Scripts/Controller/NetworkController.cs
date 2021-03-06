/// <summary>
/// 通信コントローラー
/// 
/// 2012/12/10
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Asobimo.Photon.Packet;
using Scm.Common;
using Scm.Common.Packet;
using Scm.Common.PacketCode;
using Scm.Common.GameParameter;
using Scm.Common.Master;
using Scm.Client;

public class NetworkController : Singleton<NetworkController>
{
	#region フィールド＆プロパティ
	public CommonPacket CommonPacket { get; private set; }
	public LobbyPacket LobbyPacket { get; private set; }
	public BattlePacket BattlePacket { get; private set; }

	// サーバから受け取ったユーザの状態.
	private ScmServerValue serverValue = new ScmServerValue(0);
	static public ScmServerValue ServerValue
	{
		get { return (Instance != null ? Instance.serverValue : null); }
	}

	// 通信スレッド制御用.
	object lockobj = new object();
	System.Threading.Thread thread;

	// 強制的にPhoton の Service を呼ぶフラグ
	// Google 決済ダイアログが開いていると ApplicationContollert.IsPause が 立ってしまい
	// 決済すると通信が切れてしまうため
	static bool isForceService = false;
	static public bool IsForceService { get { return isForceService; } set { isForceService = value; } }
    static Queue<System.Action> dispatchQueue = new Queue<System.Action>();
	#endregion

	#region 初期化
	protected override void Awake()
	{
		base.Awake();

		this.CommonPacket = new CommonPacket();
		this.LobbyPacket = new LobbyPacket();
		this.BattlePacket = new BattlePacket();

		thread = new System.Threading.Thread(ThreadWork);
		thread.Start();
	}
	void Start()
	{
		this.Init();
	}
	/// <summary>
	/// 初期化
	/// </summary>
	void Init()
	{
		NetworkController.DestroyAll();
	}
	#endregion

	#region 破棄
	void OnDestroy()
	{
		// 強制ログアウト
		BattlePacket.SendExitField();
		thread.Abort();
	}
	public static void DestroyAll()
	{
		GameController.Instance.ObjectDestroy();
		Entrant.ClearEntrant();
	}
	public static void DestroyReset()
	{
		GameController.Instance.ObjectResetDestroy();
		Entrant.ClearEntrant();
	}
	public static void DestroyBattle()
	{
		GameController.Instance.ObjectBattleDestroy();
	}
	#endregion

	#region 更新
	private void ThreadWork()
	{
		while(true)
		{
            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
			//if(IsForceService || !ApplicationController.IsPause)
			{
				lock(lockobj)
				{
                    GameListener.Service();	
			        //Debug.Log("===> Peer");
				}
			}
            watch.Stop();
            System.Threading.Thread.Sleep(System.Math.Max(50 - (int)watch.ElapsedMilliseconds, 1));
        }
	}
	void Update()
	{
		this.UpdateService();
	}
	void UpdateService()
	{
		// パケット受信更新
		lock(lockobj)
		{
			this.UpdateOperationResponsePacket();
			this.UpdateEventPacket();
		}
        RequestManager.CheckTimeout();
	}
	#endregion

	#region 接続＆切断
	/// <summary>
	/// 接続
	/// </summary>
	public static void Connect()
	{
        if (string.IsNullOrEmpty(ScmParam.ConnectHost)) {
            ScmParam.ConnectHost = ScmParam.MasterHost;
        }
		// ゲームリスナー初期化と接続
		GUIDebugLog.AddMessage(string.Format("Host:{0}", ScmParam.ConnectHost));
		GameListener.Connect(ScmParam.ConnectHost);
	}
	/// <summary>
	/// 切断
	/// </summary>
	public static void Disconnect()
	{
        // ログアウト
        NetworkController.SendLogout();
	}

	#region IPhotonPeerListener.OnStatusChanged
	/// <summary>
	/// 接続
	/// </summary>
	public static void OSC_Connect()
	{
	}
	/// <summary>
	/// 切断
	/// </summary>
	public static void OSC_Disconnect()
	{
		NetworkController.Disconnected();
	}
	/// <summary>
	/// サーバから切断された.
	/// </summary>
	public static void OSC_DisconnectByServer()
	{
		NetworkController.DisconnectByServer();
	}
	#endregion

	/// <summary>
	/// 通信切断後処理.
	/// </summary>
	private static void Disconnected()
	{
		// シーンごとの処理をする.
		SceneController.OnNetworkDisconnect();
	}

	/// <summary>
	/// サーバから切断された際の終了処理.
	/// </summary>
	private static void DisconnectByServer()
	{
		// ログインフラグを落とす
		GameListener.LoginFlg = false;
		// シーンごとにの処理をする
		SceneController.OnNetworkDisconnectByServer();
	}
	#endregion

	#region パケット受信
	/// <summary>
	/// 応答パケット更新
	/// </summary>
	void UpdateOperationResponsePacket()
	{
		PacketBase packet;
		while (GameListener.GameCommunication.GetResponsePacket(out packet))
		{
			ResponseCode code = (ResponseCode)packet.Code;
			// 処理実行.
			PacketHandler.RunResponsePacket(code, packet);
		}
        bool hasData;
        lock (dispatchQueue) {
            hasData = dispatchQueue.Count > 0;
        }
        if (hasData) {
            Queue<System.Action> actionQueue = new Queue<System.Action>();
            System.Action action;
            lock (dispatchQueue) {
                while (dispatchQueue.Count > 0) {
                    action = dispatchQueue.Dequeue();
                    actionQueue.Enqueue(action);
                }
            }
            while (actionQueue.Count > 0) {
                action = actionQueue.Dequeue();
                action.Invoke();
            }
        }
        
	}

    public static void InvokeAsync(System.Action action) {
        lock (dispatchQueue) {
            dispatchQueue.Enqueue(action);
        }
    }

	/// <summary>
	/// 通知パケット更新
	/// </summary>
	void UpdateEventPacket()
	{
		PacketBase packet;
		while (GameListener.GameCommunication.GetEventPacket(out packet))
		{
			EventCode code = (EventCode)packet.Code;
			// 処理実行.
			PacketHandler.RunEventPacket(code, packet);
		}
	}
	#endregion

	#region 生成＆削除

	static public void RemovePlayer()
	{
		// 存在しない
		if (PlayerManager.Instance.Player != null)
		{
			// ゲームオブジェクト破棄
			PlayerManager.Instance.Destroy();
		}
	}
	#endregion

	#region Login パケット
	static IPacketResponse<LoginRes> LoginResponse { get; set; }
	public static void SendLogin(IPacketResponse<LoginRes> response)
	{
		LoginResponse = response;
		SendLogin();
	}
	/// <summary>
	/// Login 送信要求
	/// </summary>
	public static void SendLogin()
	{
		// ログイン
		PlatformType platformType = ApplicationController.PlatformType;
		MarketType marketType = MarketType.Unknown;
		byte authType = (byte)ApplicationController.PlatformType;
#if EJPL
        string token = PluginController.AuthInfo.EJInfo;
#else
        string token = PluginController.AuthInfo.token;
#endif
        byte distributionCode = AuthEntry.Instance.AuthMethod != null ? AuthEntry.Instance.AuthMethod.distributionCode : (byte)0;
		string versionName = PluginController.PackageInfo.versionName1;

#if XW_DEBUG
		// デバッグ時はdebug.jsonからIDを取得する
		// トークンは空に設定する
		if (ScmParam.Debug.File.IsDebugMode)
		{
			if (!string.IsNullOrEmpty(ScmParam.Debug.File.AsobimoID))	// 空文字じゃなければデバッグ用のAsobimoIDでログインする
			{
				// デバッグ用には # から始まる AsobimoID を Token に突っ込めば使えるようになる
				token = ScmParam.Debug.File.AsobimoID;
			}
		}
		GUIDebugLog.AddMessage(string.Format(
			"PlatformType:{0}({1}) MarketType:{2}({3}) AuthType:{4}({5}) Token:{6} VersinName:{7}",
			platformType, (int)platformType,
			marketType, (int)marketType,
			(Asobimo.Auth.AsobimoAuthCreateType)authType, authType,
			token, versionName));
#endif
        Debug.Log("VersionName=" + versionName + ",packageinfover=" + PluginController.PackageInfo.versionName1);
		NetworkController.SendLogin(platformType, marketType, authType, distributionCode, token, versionName);
	}
	/// <summary>
	/// Login 送信要求
	/// </summary>
	/// <param name="id"></param>
	/// <param name="token"></param>
	private static void SendLogin(PlatformType platformType, MarketType marketType, byte authType, byte distributionCode, string token, string versionName)
	{
		LoginReq packet = new LoginReq();
		packet.Language = Scm.Common.Utility.Language;
		packet.PlatformType = platformType;
		packet.MarketType = marketType;
		packet.AuthType = authType;
		packet.Token = token;
        packet.DistributionCode = distributionCode;
        packet.VersionName = versionName;
		packet.Cookie = XigncodePlugin.Instance.GetCookie(AuthRequest.Seed);
		GameListener.SendConnected(packet);
	}
	/// <summary>
	/// Login 受信応答
	/// </summary>
	/// <param name="packet"></param>
	public void OperationResponseLogin(LoginRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		if (LoginResponse != null)
		{
			LoginResponse.Response(packet);
		}
	}
	#endregion

	#region Logout パケット
	/// <summary>
	/// Logout 送信要求
	/// </summary>
	public static void SendLogout()
	{
		LogoutReq packet = new LogoutReq();
		GameListener.SendConnected(packet);
	}
	/// <summary>
	/// Logout 受信応答
	/// </summary>
	/// <param name="packet"></param>
	public void OperationResponseLogout(LogoutRes packet)
	{
		// パケットが違う
		if (packet == null)
			return;

		// ログインフラグを落とす
		GameListener.LoginFlg = false;
		// 切断
		GameListener.Disconnect();
	}
	#endregion

	#region ServerStatus パケット
	static IPacketResponse<ServerStatusRes> ServerStatusResponse { get; set; }
	public static void SendServerStatus(IPacketResponse<ServerStatusRes> response)
	{
		ServerStatusResponse = response;
		SendServerStatus();
	}
	/// <summary>
	/// ServerStatus 送信要求
	/// </summary>
	public static void SendServerStatus()
	{
		var packet = new ServerStatusReq();
		GameListener.SendConnected(packet);
	}
	/// <summary>
	/// ServerStatus 受信応答
	/// </summary>
	/// <param name="packet"></param>
	public void OperationResponseServerStatus(ServerStatusRes packet)
	{
		// パケッドが違う
		if (packet == null)
			return;

		//Id	int	サーバID
		//Name	string	サーバ名
		//ServerStatus	ServerStatus	サーバ状態

		if(ServerStatusResponse != null)
		{
			ServerStatusResponse.Response(packet);
		}
	}
	#endregion

	#region RetainPlayerAll パケット
	static IPacketResponse<RetainPlayerAllRes> RetainPlayerAllResponse { get; set; }
	public static void SendRetainPlayerAll(IPacketResponse<RetainPlayerAllRes> response)
	{
		RetainPlayerAllResponse = response;
		SendRetainPlayerAll();
	}
	/// <summary>
	/// RetainPlayerAll 送信要求
	/// </summary>
	public static void SendRetainPlayerAll()
	{
		var packet = new RetainPlayerAllReq();
		GameListener.SendConnected(packet);
	}
	/// <summary>
	/// RetainPlayerAll 受信応答
	/// </summary>
	/// <param name="packet">Packet.</param>
	public void OperationRetainPlayerAll(RetainPlayerAllRes packet)
	{
		// パケッドが違う
		if (packet == null)
			return;

		if(RetainPlayerAllResponse != null)
		{
			RetainPlayerAllResponse.Response(packet);
		}
	}
	#endregion

	#region CreatePlayer パケット
	static IPacketResponse<CreatePlayerRes> CreatePlayerResponse { get; set; }
	public static void SendCreatePlayer(IPacketResponse<CreatePlayerRes> response)
	{
		CreatePlayerResponse = response;
		SendCreatePlayer();
	}
	/// <summary>
	/// CreatePlayer 送信要求
	/// </summary>
	public static void SendCreatePlayer()
	{
		var packet = new CreatePlayerReq();
		packet.Name = ScmParam.Net.UserName;
#if PLATE_NUMBER_REVIEW
        packet.IsPlateNumberReview = true;
#endif
        GameListener.SendConnected(packet);
	}
	/// <summary>
	/// CreatePlayer 受信応答
	/// </summary>
	/// <param name="packet">Packet.</param>
	public void OperationCreatePlayer(CreatePlayerRes packet)
	{
		// パケットが違う
		if(packet == null)
			return;

		// タイトルUIの受信処理
		if(CreatePlayerResponse != null)
		{
            CreatePlayerResponse.Response(packet);
		}
	}
	#endregion

	#region SelectPlayer パケット
	static IPacketResponse<SelectPlayerRes> SelectPlayerResponse { get; set; }
	public static void SendSelectPlayer(int playerId, IPacketResponse<SelectPlayerRes> response)
	{
		SelectPlayerResponse = response;
		SendSelectPlayer(playerId);
	}
	/// <summary>
	/// SelectPlayer 送信要求
	/// </summary>
	public static void SendSelectPlayer(int playerId)
	{
		var packet = new SelectPlayerReq();
		packet.PlayerId = playerId;
		GameListener.SendConnected(packet);
	}
	/// <summary>
	/// SelectPlayer 受信応答
	/// </summary>
	/// <param name="packet">Packet.</param>
	public void OperationSelectPlayer(SelectPlayerRes packet)
	{
		// パケットが違う
		if(packet == null)
			return;

        //  UNDONE: Common.DLL: ScmServerValue()の引数を変更した場合の影響範囲が広いのでここでキャストしておく
		// 初期化.
		this.serverValue = new ScmServerValue((int)packet.PlayerId);

		if(SelectPlayerResponse != null)
		{
			SelectPlayerResponse.Response(packet);
		}
	}
    #endregion

    #region Recruitment
    public static void SendRecruitment(bool publish, string text) {
        var packet = new RecruitmentReq() {
            Command = publish ? RecruitmentReq.COMMAND_PUBLISH : RecruitmentReq.COMMAND_UNPUBLISH,
            Text = text
        };
        GameListener.SendConnected(packet);
    }
    #endregion

    #region エラーログ
    public static void SaveConflictPlayerIdLog(EntrantInfo conflictObj)
	{
		Player player = GameController.GetPlayer();
		if (player != null)
		{
			MapManager map = MapManager.Instance;
			BugReportController.SaveLogFile("CreateType Player inFieldId=" + ServerValue.InFieldId + "," + map.AreaType + ":" + map.FieldId +
				",Player name=" + player.UserName +
				",conflictObj Type=" + conflictObj.EntrantType + " name=" + conflictObj.UserName);
		}
		else
		{
			MapManager map = MapManager.Instance;
			BugReportController.SaveLogFile("CreateType Player inFieldId=" + ServerValue.InFieldId + "," + map.AreaType + ":" + map.FieldId +
				",conflictObj Type=" + conflictObj.EntrantType + " name=" + conflictObj.UserName);
		}
	}
	[System.Diagnostics.Conditional("DISABLE")]
	public static void SaveConflictIdLog(ObjectBase oldObj, EntrantInfo newObj)
	{
		Player player = GameController.GetPlayer();
		if (player != null)
		{
			BugReportController.SaveLogFile("InFieldId conflict! inFieldId=" + newObj.InFieldId + "," + newObj.AreaType + ":" + newObj.FieldId +
				",Player Id=" + ServerValue.InFieldId + " name=" + player.UserName +
				",oldobj Type=" + oldObj.EntrantType + " Id=" + oldObj.InFieldId + " name=" + oldObj.UserName +
				",newobj Type=" + newObj.EntrantType + " Id=" + newObj.InFieldId + " name=" + newObj.UserName);
		}
	}
	public static void SaveConflictIdLog(ObjectBase oldObj, ObjectBase newObj)
	{
		Player player = GameController.GetPlayer();
		if (player != null)
		{
			MapManager map = MapManager.Instance;
			BugReportController.SaveLogFile("InFieldId conflict! inFieldId=" + newObj.InFieldId + "," + map.AreaType + ":" + map.FieldId +
				",Player Id=" + ServerValue.InFieldId + " name=" + player.UserName +
				",oldobj Type=" + oldObj.EntrantType + " Id=" + oldObj.InFieldId + " name=" + oldObj.UserName +
				",newobj Type=" + newObj.EntrantType + " Id=" + newObj.InFieldId + " name=" + newObj.UserName);
		}
	}
	#endregion
}
