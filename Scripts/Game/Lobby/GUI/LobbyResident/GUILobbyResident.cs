/// <summary>
/// ロビー常駐メニュー
/// 
/// 2016/06/02
/// </summary>
using UnityEngine;
using System.Collections.Generic;
using Scm.Common.Packet;
using XUI.LobbyResident;

public class GUILobbyResident : Singleton<GUILobbyResident>
{
	#region フィールド＆プロパティ
	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	LobbyResidentView _viewAttach = null;
	LobbyResidentView ViewAttach { get { return _viewAttach; } }

	/// <summary>
	/// 開始時のアクティブ状態
	/// </summary>
	[SerializeField]
	bool _isStartActive = false;
	bool IsStartActive { get { return _isStartActive; } }

	/// <summary>
	/// ロビー番号表示フォーマット
	/// </summary>
	[SerializeField]
	string _lobbyNoFormat = "{0:000}";
	string LobbyNoFormat { get { return _lobbyNoFormat; } }

	/// <summary>
	/// ロビーメンバー表示フォーマット
	/// </summary>
	[SerializeField]
	string _lobbyMemberFormat = "{0}/{1}";
	string LobbyMemberFormat { get { return _lobbyMemberFormat; } }

	/// <summary>
	/// 通知系表示フォーマット
	/// </summary>
	[SerializeField]
	string _alertFormat = "{0}";
	string AlertFormat { get { return _alertFormat; } }

	/// <summary>
	/// プレイヤー勝敗表示フォーマット
	/// </summary>
	[SerializeField]
	string _playerWinLoseFormat = "{0}";
	string PlayerWinLoseFormat { get { return _playerWinLoseFormat; } }

	// コントローラー
	public IController Controller { get; set; }
	// シリアライズされていないメンバーの初期化
	void MemberInit()
	{
		this.Controller = null;
	}

	/// <summary>
	/// ロビーセレクトが出来るかどうか
	/// </summary>
	public static bool CanLobbySelect
	{
		get
		{
			bool isActive = true;
			if (GUIMatchingState.IsMatching)
			{
				// マッチング中
				isActive = false;
			}
			return isActive;
		}
	}
	#endregion

	#region 初期化
	override protected void Awake()
	{
		base.Awake();
		this.MemberInit();
	}
	void Start()
	{
		this.Construct();
		// 初期アクティブ設定
		this._SetActive(this.IsStartActive, true, this.IsStartActive);
#if PLATFORM_XIAOYOU || ANDROID_XY
        LobbyPacket.SendReceiveWebStore(null);
#endif
        OnIn();
        //GuideFrame.Instance.GetGuideInfo();
	}
	void Construct()
	{
		// モデル生成
		var model = new Model();
		model.LobbyNoFormat = this.LobbyNoFormat;
		model.LobbyMemberFormat = this.LobbyMemberFormat;
		model.AlertFormat = this.AlertFormat;
		model.PlayerWinLoseFormat = this.PlayerWinLoseFormat;

		// ビュー生成
		IView view = null;
		if (this.ViewAttach != null)
		{
			view = this.ViewAttach.GetComponent(typeof(IView)) as IView;
		}

		// コントローラー生成
		var controller = new Controller(model, view);
		this.Controller = controller;
	}
	#endregion

	#region 破棄
	void OnDestroy()
	{
		if (this.Controller != null)
		{
			this.Controller.Dispose();
		}
	}
	#endregion

	#region アクティブ設定
	/// <summary>
	/// アクティブ設定
	/// </summary>
	public static void SetActive(bool isActive)
	{
		if (Instance != null) Instance._SetActive(isActive, false, isActive);
	}
	/// <summary>
	/// アクティブ設定
	/// </summary>
	void _SetActive(bool isActive, bool isTweenSkip, bool isSetup)
	{
		if (isSetup)
		{
			this.Setup();
		}

		if (this.Controller != null)
		{
			this.Controller.SetActive(isActive, isTweenSkip);
		}
	}
	#endregion

	#region 各種情報更新
	/// <summary>
	/// 初期設定
	/// </summary>
	void Setup()
	{
		if (this.Controller != null)
		{
			this.Controller.Setup();
		}
	}
	#endregion

	#region 更新
	/// <summary>
	/// 更新
	/// </summary>
	void Update()
	{
		if (this.Controller != null)
		{
			this.Controller.Update();
		}
	}
	#endregion

	#region 各状態を更新する
	/// <summary>
	/// ロビー選択の状態を更新する
	/// </summary>
	public static void UpdateLobbySelectActive()
	{
		if (Instance != null) Instance._UpdateLobbySelectActive();
	}
	void _UpdateLobbySelectActive()
	{
		if (this.Controller != null) this.Controller.UpdateLobbySelectActive();
	}
	/// <summary>
	/// マッチングの状態を更新する
	/// </summary>
	public static void UpdateMatchingActive()
	{
		if (Instance != null) Instance._UpdateMatchingActive();
	}
	void _UpdateMatchingActive()
	{
		if (this.Controller != null) this.Controller.UpdateMatchingActive();
	}
	/// <summary>
	/// 練習ボタンの状態を更新する
	/// </summary>
	public static void UpdateTrainingButtonActive()
	{
		if (Instance != null) Instance._UpdateTrainingButtonActive();
	}
	void _UpdateTrainingButtonActive()
	{
		if (this.Controller != null) this.Controller.UpdateTrainingButtonEnable();
	}

	/// <summary>
	/// チーム情報更新
	/// </summary>
	public static void UpdateTeamInfo(TeamParameter teamParameter, List<GroupMemberParameter> memberParameter)
	{
		if (Instance != null) Instance._UpdateTeamInfo(teamParameter, memberParameter);
	}
	void _UpdateTeamInfo(TeamParameter teamParameter, List<GroupMemberParameter> memberParameter)
	{
		if (this.Controller != null) this.Controller.UpdateTeamInfo(teamParameter, memberParameter);
	}
	/// <summary>
	/// シングルボタンの状態を更新する
	/// </summary>
	public static void UpdateSingleButtonEnable()
	{
		if (Instance != null) Instance._UpdateSingleButtonEnable();
	}
	void _UpdateSingleButtonEnable()
	{
		if (this.Controller != null) this.Controller.UpdateSingleButtonEnable();
	}
	/// <summary>
	/// ショップメニューボタンの状態を更新する
	/// </summary>
	public static void UpdateShopMenuButtonEnable()
	{
		if (Instance != null) Instance._UpdateShopMenuButtonEnable();
	}
	void _UpdateShopMenuButtonEnable()
	{
		if (this.Controller != null) this.Controller.UpdateShopMenuButtonEnable();
	}
	#endregion 各状態を更新する

	#region 表示直結系
	#region ロビー番号
	/// <summary>
	/// ロビー番号設定
	/// </summary>
	public static void SetLobbyNo(int lobbyNo)
	{
		if (Instance != null) Instance._SetLobbyNo(lobbyNo);
	}
	void _SetLobbyNo(int lobbyNo)
	{
		if (this.Controller != null)
		{
			this.Controller.SetLobbyNo(lobbyNo);
		}
	}
	#endregion ロビー番号

	#region ロビーメンバー
	/// <summary>
	/// ロビーの収容人数設定
	/// </summary>
	public static void SetLobbyMemberCapacity(int num)
	{
		if (Instance != null) Instance._SetLobbyMemberCapacity(num);
	}
	void _SetLobbyMemberCapacity(int num)
	{
		if (this.Controller != null)
		{
			this.Controller.SetLobbyMemberCapacity(num);
		}
	}
	#endregion ロビーメンバー

	#region 通知系
	/// <summary>
	/// 未取得アチーブメント数設定
	/// </summary>
	public static void SetAchieveUnreceived(int num)
	{
		if (Instance != null) Instance._SetAchieveUnreceived(num);
	}
	void _SetAchieveUnreceived(int num)
	{
		if (this.Controller != null)
		{
			this.Controller.SetAchieveUnreceived(num);
		}
	}
	/// <summary>
	/// 未読メール数設定
	/// </summary>
	public static void SetMailUnread(int num)
	{
		if (Instance != null) Instance._SetMailUnread(num);
	}
	void _SetMailUnread(int num)
	{
		if (this.Controller != null)
		{
			this.Controller.SetMailUnread(num);
		}
	}
	/// <summary>
	/// 未処理申請数設定
	/// </summary>
	public static void SetApplyUnprocessed(int num)
	{
		if (Instance != null) Instance._SetApplyUnprocessed(num);
	}
	void _SetApplyUnprocessed(int num)
	{
		if (this.Controller != null)
		{
			this.Controller.SetApplyUnprocessed(num);
		}
	}
	#endregion 通知系

	#region プレイヤー情報
	/// <summary>
	/// プレイヤー名設定
	/// </summary>
	public static void SetPlayerName(string name)
	{
		if (Instance != null) Instance._SetPlayerName(name);
	}
	void _SetPlayerName(string name)
	{
		if (this.Controller != null)
		{
			this.Controller.SetPlayerName(name);
		}
	}

    public static void SetIcon(UIAtlas pAtlas, string pSpriteName)
    {
        if (Instance != null)
        {
            var p =   (Instance.Controller as XUI.LobbyResident.Controller).GetView().GetPalyer();
            p.SetLeaderIcon(pAtlas, pSpriteName);
            p.SetLeaderIconEnable(true);
        }
    }
	/// <summary>
	/// プレイヤー勝利数設定
	/// </summary>
	public static void SetPlayerWin(int num)
	{
		if (Instance != null) Instance._SetPlayerWin(num);
	}
	void _SetPlayerWin(int num)
	{
		if (this.Controller != null)
		{
			this.Controller.SetPlayerWin(num);
		}
	}
	/// <summary>
	/// プレイヤー敗北数設定
	/// </summary>
	public static void SetPlayerLose(int num)
	{
		if (Instance != null) Instance._SetPlayerLose(num);
	}
	void _SetPlayerLose(int num)
	{
		if (this.Controller != null)
		{
			this.Controller.SetPlayerLose(num);
		}
	}
	#endregion プレイヤー情報
	#endregion 表示直結系

	#region デバッグ
#if UNITY_EDITOR && XW_DEBUG
	[SerializeField]
	GUIDebugParam _debugParam = new GUIDebugParam();
	GUIDebugParam DebugParam { get { return _debugParam; } }
	[System.Serializable]
	public class GUIDebugParam : GUIDebugParamBase
	{
		public GUIDebugParam()
		{
			this.AddEvent(this.StatusInfo);
			this.AddEvent(this.Alert);
			this.AddEvent(this.Player);
		}

		[SerializeField]
		StatusInfoEvent _statusInfo = new StatusInfoEvent();
		public StatusInfoEvent StatusInfo { get { return _statusInfo; } }
		[System.Serializable]
		public class StatusInfoEvent : IDebugParamEvent
		{
			public event System.Action<int, int> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			int lobbyNo = 0;
			[SerializeField]
			int lobbyMemberCapacity = 0;

			public void Update()
			{
				if (this.execute)
				{
					this.execute = false;
					this.Execute(this.lobbyNo, this.lobbyMemberCapacity);
				}
			}
		}

		[SerializeField]
		AlertEvent _alert = new AlertEvent();
		public AlertEvent Alert { get { return _alert; } }
		[System.Serializable]
		public class AlertEvent : IDebugParamEvent
		{
			public event System.Action<int, int, int> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			int achieveUnreceived = 0;
			[SerializeField]
			int mailUnread = 0;
			[SerializeField]
			int applyUnprocessed = 0;

			public void Update()
			{
				if (this.execute)
				{
					this.execute = false;

					this.Execute(this.achieveUnreceived, this.mailUnread, this.applyUnprocessed);
				}
			}
		}

		[SerializeField]
		PlayerEvent _player = new PlayerEvent();
		public PlayerEvent Player { get { return _player; } }
		[System.Serializable]
		public class PlayerEvent : IDebugParamEvent
		{
			public event System.Action<string, int, int> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			string name = "";
			[SerializeField]
			int win = 0;
			[SerializeField]
			int lose = 0;

			public void Update()
			{
				if (this.execute)
				{
					this.execute = false;

					this.Execute(this.name, this.win, this.lose);
				}
			}
		}
	}
	void DebugInit()
	{
		var d = this.DebugParam;

		d.ExecuteClose += () => { this._SetActive(false, false, false); };
		d.ExecuteActive += () =>
		{
			d.ReadMasterData();
			this._SetActive(true, false, true);
		};
		d.StatusInfo.Execute += (lobbyNo, lobbyMemberCapacity) => { SetLobbyNo(lobbyNo); SetLobbyMemberCapacity(lobbyMemberCapacity); };
		d.Alert.Execute += (achieveUnreceived, mailUnread, applyUnprocessed)=>
			{
				SetAchieveUnreceived(achieveUnreceived);
				SetMailUnread(mailUnread);
				SetApplyUnprocessed(applyUnprocessed);
			};
		d.Player.Execute += (name, win, lose) => { SetPlayerName(name); SetPlayerWin(win); SetPlayerLose(lose); };
	}
	bool _isDebugInit = false;
	void DebugUpdate()
	{
		if (!this._isDebugInit)
		{
			this._isDebugInit = true;
			this.DebugInit();
		}

		this.DebugParam.Update();
	}
	/// <summary>
	/// OnValidate はInspector上で値の変更があった時に呼び出される
	/// GameObject がアクティブ状態じゃなくても呼び出されるのでアクティブ化するときには有効
	/// </summary>
	void OnValidate()
	{
		if (Application.isPlaying)
		{
			this.DebugUpdate();
		}
	}
#endif
	#endregion

    #region other
    public void OnIn()
    {
        Debug.LogWarning("===> OnLobbyMenu");
//        if (true)
//        {
//            GuideFrame.Instance.SetDetail(GuideType.StartGame);
//        }
    }
    #endregion
}
