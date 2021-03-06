/// <summary>
/// シンクロ合成表示
/// 
/// 2016/02/24
/// </summary>
using UnityEngine;
using System;
using System.Collections.Generic;

public class GUISynchro : Singleton<GUISynchro>
{
	#region フィールド&プロパティ
	/// <summary>
	/// ビューアタッチ
	/// </summary>
	[SerializeField]
	private XUI.Synchro.SynchroView _viewAttach = null;
	private XUI.Synchro.SynchroView ViewAttach { get { return _viewAttach; } }

	/// <summary>
	/// キャラページリスト
	/// </summary>
	[SerializeField]
	private GUICharaPageList _charaPageList = null;
	private GUICharaPageList CharaPageList { get { return _charaPageList; } }

	/// <summary>
	/// キャラアイテムリソース
	/// </summary>
	[SerializeField]
	private GameObject _charaItemResource = null;
	private GameObject CharaItemResource { get { return _charaItemResource; } }

	/// <summary>
	/// インスタンス化したベースキャラアイテムをアタッチする場所
	/// </summary>
	[SerializeField]
	private GameObject _attachBaseChara = null;
	private GameObject AttachBaseChara { get { return _attachBaseChara; } }

	/// <summary>
	/// インスタンス化した餌キャラアイテムをアタッチする場所
	/// </summary>
	[SerializeField]
	private GameObject _attachBaitChara = null;
	private GameObject AttachBaitChara { get { return _attachBaitChara; } }

	/// <summary>
	/// 開始時のアクティブ状態
	/// </summary>
	[SerializeField]
	private bool _isStartActive = false;
	private bool IsStartActive { get { return _isStartActive; } }

	/// <summary>
	/// 所持金表示フォーマット
	/// </summary>
	[SerializeField]
	private string _haveMoneyFormat = "{0:#,0}";
	private string HaveMoneyFormat { get { return _haveMoneyFormat; } }

	/// <summary>
	/// 費用表示フォーマット
	/// </summary>
	[SerializeField]
	private string _needMoneyFormat = "{0:#,0}";
	private string NeedMoneyFormat { get { return _needMoneyFormat; } }

	/// <summary>
	/// 追加料金表示フォーマット
	/// </summary>
	[SerializeField]
	private string _addOnChargeFormat = "{0:#,0}";
	private string AddOnChargeFormat { get { return _addOnChargeFormat; } }

	/// <summary>
	/// シンクロ値表示フォーマット
	/// </summary>
	[SerializeField]
	private string _synchroFormat = "{0:+#;-#;+0}";
	private string SynchroFormat { get { return _synchroFormat; } }

	/// <summary>
	/// シンクロ増加表示フォーマット
	/// </summary>
	[SerializeField]
	private string _synchroUpFormat = "? UP!!";
	private string SynchroUpFormat { get { return _synchroUpFormat; } }

	/// <summary>
	/// シンクロ増加値の最大時フォーマット
	/// </summary>
	[SerializeField]
	private string _synchroMaxFromat = "Max";
	private string SynchroMaxFromat { get { return _synchroMaxFromat; } }

	/// <summary>
	/// シンクロ残り合成回数の色
	/// </summary>
	[SerializeField]
	private SynchroRemainColorSettings _synchroRemainColor = null;
	private SynchroRemainColorSettings SynchroRemainColor { get { return _synchroRemainColor; } }
	[Serializable]
	class SynchroRemainColorSettings
	{
		public Color normal = Color.white;
		public Color warning = Color.red;
	}

	// プレイヤーステータス情報
	private PlayerStatusInfo PlayerStatusInfo { get { return NetworkController.ServerValue != null ? NetworkController.ServerValue.PlayerStatusInfo : new PlayerStatusInfo(); } }

	/// <summary>
	/// ベースキャラ
	/// </summary>
	private GUICharaItem BaseChara { get; set; }

	/// <summary>
	/// 餌キャラ
	/// </summary>
	private GUICharaItem BaitChara { get; set; }

	/// <summary>
	/// モデル
	/// </summary>
	private XUI.Synchro.IModel Model { get; set; }

	/// <summary>
	/// ビュー
	/// </summary>
	private XUI.Synchro.IView View { get; set; }

	/// <summary>
	/// コントローラ
	/// </summary>
	private XUI.Synchro.Controller Controller { get; set; }
	#endregion

	#region 初期化
	protected override void Awake()
	{
		base.Awake();
		this.MemberInit();
		this.BaseChara = CreateCharaItem(this.AttachBaseChara);
		this.BaitChara = CreateCharaItem(this.AttachBaitChara);
	}

	void Start()
	{
		this.Construct();

		// 初期アクティブ設定
		this.SetActive(this.IsStartActive, true);
	}

	/// <summary>
	/// キャラアイテム生成
	/// </summary>
	private GUICharaItem CreateCharaItem(GameObject attachObj)
	{
		GUICharaItem item = GUICharaItem.Create(this.CharaItemResource, attachObj.transform, 0);
		if (item == null)
		{
			Debug.LogWarning("CharaItem is null!!");
			return null;
		}

		return item;
	}

	private void Construct()
	{
		// モデル生成
		var model = new XUI.Synchro.Model();
		this.Model = model;
		this.Model.HaveMoneyFormat = this.HaveMoneyFormat;
		this.Model.NeedMoneyFormat = this.NeedMoneyFormat;
		this.Model.AddOnChargeFormat = this.AddOnChargeFormat;
		this.Model.SynchroFormat = this.SynchroFormat;
		this.Model.SynchroUpFormat = this.SynchroUpFormat;
		this.Model.SynchroMaxFormat = this.SynchroMaxFromat;
		this.Model.SynchroRemainColor = new XUI.Synchro.SynchroRemainColorSettings(this.SynchroRemainColor.normal, this.SynchroRemainColor.warning);
		
		// ビュー生成
		XUI.Synchro.IView view = null;
		if(this.ViewAttach != null)
		{
			view = this.ViewAttach.GetComponent(typeof(XUI.Synchro.IView)) as XUI.Synchro.IView;
		}
		this.View = view;

		// コントローラ生成
		var controller = new XUI.Synchro.Controller(model, view, this.CharaPageList, this.BaseChara, this.BaitChara);
		this.Controller = controller;
		this.Controller.OnFusion += this.HandleFusion;
		this.Controller.OnFusionCalc += this.HandleFusionCalc;
		this.Controller.OnPlayerCharacter += this.HandlePlayerCharacter;
	}

	/// <summary>
	/// シリアライズされていないメンバー初期化
	/// </summary>
	private void MemberInit()
	{
		this.BaseChara = null;

		this.Model = null;
		this.View = null;
		this.Controller = null;
	}

	/// <summary>
	/// 破棄
	/// </summary>
	void OnDestroy()
	{
		if (this.Controller != null)
		{
			this.Controller.Dispose();
		}
	}
	#endregion

	#region 有効無効
	void OnEnable()
	{
		GUICharaSimpleInfo.AddLockClickEvent(this.HandleLockClickEvent);
		GUICharaSimpleInfo.AddLockResponseEvent(this.HandleLockResponse);
	}
	void OnDisable()
	{
		GUICharaSimpleInfo.RemoveLockClickEvent(this.HandleLockClickEvent);
		GUICharaSimpleInfo.RemoveLockResponseEvent(this.HandleLockResponse);
	}
	#endregion

	#region アクティブ設定
	/// <summary>
	/// 閉じる
	/// </summary>
	public static void Close()
	{
		if (Instance != null) Instance.SetActive(false, false);
	}
	/// <summary>
	/// 開く
	/// </summary>
	public static void Open()
	{
		if (Instance != null)
		{
			Instance.Setup();
			Instance.SetActive(true, false);
		}
	}
	/// <summary>
	/// 開き直す
	/// </summary>
	public static void ReOpen()
	{
		if (Instance != null)
		{
			Instance.ReSetup();
			Instance.SetActive(true, false);
		}
	}
	/// <summary>
	/// アクティブ設定
	/// </summary>
	private void SetActive(bool isActive, bool isTweenSkip)
	{
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
	private void Setup()
	{
		if(this.Controller != null)
		{
			// コントローラ側初期化
			this.Controller.Setup();
		}

		var info = this.PlayerStatusInfo;
		this.SetupStatusInfo(info.GameMoney);

		// サーバからキャラボックス情報取得
		LobbyPacket.SendPlayerCharacterBox(this.Response);
	}

	/// <summary>
	/// 再初期化
	/// </summary>
	private void ReSetup()
	{
		// ステータス情報を更新する
		var info = this.PlayerStatusInfo;
		this.SetupStatusInfo(info.GameMoney);

		// サーバからキャラボックス情報取得
		LobbyPacket.SendPlayerCharacterBox(this.Response);
	}

	/// <summary>
	/// ステータス情報を更新する
	/// </summary>
	private void SetupStatusInfo(int haveMoney)
	{
		if(this.Controller != null)
		{
			this.Controller.SetupStatusInfo(haveMoney);
		}
	}
	#endregion

	#region 通信系
	#region PlayerCharacterBox パケット
	/// <summary>
	/// PlayerCharacterBoxReq パケットのレスポンス
	/// </summary>
	private void Response(LobbyPacket.PlayerCharacterBoxResArgs args)
	{
		this.SetupCapacity(args.Capacity, args.Count);

		// 所有キャラクター情報を取得する
		LobbyPacket.SendPlayerCharacterAll(this.Response);
	}
	/// <summary>
	/// 総数を設定する
	/// </summary>
	private void SetupCapacity(int capacity, int count)
	{
		if (this.Controller != null)
		{
			this.Controller.SetupCapacity(capacity, count);
		}
	}
	#endregion

	#region PlayerCharacterAll パケット
	/// <summary>
	/// PlayerCharacterAllReq パケットのレスポンス
	/// </summary>
	void Response(LobbyPacket.PlayerCharacterAllResArgs args)
	{
		// 通信中メッセージを閉じる
		GUISystemMessage.Close();

		this.SetupItem(args.List);
	}
	/// <summary>
	/// 個々のアイテムの設定をする
	/// </summary>
	void SetupItem(List<CharaInfo> list)
	{
		if (this.Controller != null)
		{
			this.Controller.SetupCharaInfoList(list);
		}
	}
	#endregion

	#region SynchroFusion パケット
	/// <summary>
	/// 合成イベントハンドラー
	/// </summary>
	void HandleFusion(object sender, XUI.Synchro.FusionEventArgs e)
	{
		LobbyPacket.SendSynchroFusion(e.BaseCharaUUID, e.BaitCharaUUID, this.Response);

		// 「通信中」表示
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX305_Network_Communication));
	}
	/// <summary>
	/// SynchroReq パケットのレスポンス
	/// </summary>
	void Response(LobbyPacket.SynchroFusionResArgs args)
	{
		// 所有キャラクター情報を取得する
		LobbyPacket.SendPlayerCharacterAll(this.Response);

		// ステータス情報を更新する
		var info = this.PlayerStatusInfo;
		this.SetupStatusInfo(info.GameMoney);

		// 合成結果
		this.FusionResult(args.Result, args.Money, args.Price, args.AddOnCharge, args.CharaInfo);
	}
	/// <summary>
	/// 合成結果
	/// </summary>
	void FusionResult(Scm.Common.GameParameter.PowerupResult result, int money, int price, int addOnCharge, CharaInfo charaInfo)
	{
		if (result == Scm.Common.GameParameter.PowerupResult.Fail)
		{
			// 失敗しているときは閉じる
			GUIController.Clear();

			// 警告ログ
			string msg = string.Format("SynchroFusionResponse Result={0} money={1} price={2} addOnCharge={3} synchroHitPoint={4} synchroAttack={5} synchroDefense={6} synchroExtra={7}",
				result, money, price, addOnCharge, charaInfo.SynchroHitPoint, charaInfo.SynchroAttack, charaInfo.SynchroDefense, charaInfo.SynchroExtra);
			Debug.LogWarning(msg);
			BugReportController.SaveLogFileWithOutStackTrace(msg);
			return;
		}

		if (this.Controller != null)
		{
			this.Controller.FusionResult(result, money, price, addOnCharge, charaInfo);
		}
	}
	#endregion

	#region SynchroFusionCalc パケット
	private void HandleFusionCalc(object sender, XUI.Synchro.FusionCalcEventArgs e)
	{
		LobbyPacket.SendSynchroFusionCalc(e.BaseCharaUUID, e.BaitCharaUUID, this.Response);
	}
	/// <summary>
	/// SynchroFusionCalcReq パケットのレスポンス
	/// </summary>
	void Response(LobbyPacket.SynchroFusionCalcResArgs args)
	{
		this.SynchroCalc(args.Result, args.Money, args.Price, args.AddOnCharge);
	}
	/// <summary>
	/// 合成試算結果
	/// </summary>
	void SynchroCalc(bool result, int money, int price, int addOnCharge)
	{
		if (this.Controller != null)
		{
			this.Controller.FusionCalcResult(result, money, price, addOnCharge);
		}
	}
	#endregion

	#region SetLockPlayerCharacter パケット
	/// <summary>
	/// ロック設定イベントハンドラー
	/// </summary>
	private void HandleLockClickEvent(CharaInfo obj)
	{
		// 「通信中」表示
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX305_Network_Communication));
	}

	/// <summary>
	/// SetLockPlayerCharacterReq パケットのレスポンス
	/// </summary>
	private void HandleLockResponse(LobbyPacket.SetLockPlayerCharacterResArgs args)
	{
		// 所有キャラクター情報を取得する
		LobbyPacket.SendPlayerCharacterAll(this.Response);
	}
	#endregion

	#region PlayerCharacter パケット
	private void HandlePlayerCharacter(object sender, XUI.Synchro.PlayerCharacterEventArgs e)
	{
		LobbyPacket.SendPlayerCharacter(e.UUID, this.Response);
		// 通信中の表示
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX305_Network_Communication));
	}
	/// <summary>
	/// PlayerCharacterReq パケットのレスポンス
	/// </summary>
	void Response(LobbyPacket.PlayerCharacterResArgs args)
	{
		// 通信中を閉じる
		GUISystemMessage.Close();
		this.SetupPlayerCharacterInfo(args.CharaInfo, args.SlotBonusHitPoint, args.SlotBonusAttack, args.SlotBonusDefense, args.SlotBonusExtra, args.SlotList);
	}
	/// <summary>
	/// プレイヤーキャラクター情報を設定
	/// </summary>
	void SetupPlayerCharacterInfo(CharaInfo info, int slotHitPoint, int slotAttack, int slotDefense, int slotExtra, List<CharaInfo> slotList)
	{
		if (this.Controller != null)
		{
			this.Controller.SetupPlayerCharacterInfo(info, slotHitPoint, slotAttack, slotDefense, slotExtra, slotList);
		}
	}
	#endregion
	#endregion


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
			this.AddEvent(this.CharaList);
			this.AddEvent(this.Synchro);
		}

		[SerializeField]
		StatusInfoEvent _statusInfo = new StatusInfoEvent();
		public StatusInfoEvent StatusInfo { get { return _statusInfo; } }
		[System.Serializable]
		public class StatusInfoEvent : IDebugParamEvent
		{
			public event System.Action<int> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			int haveMoney = 0;

			public void Update()
			{
				if (this.execute)
				{
					this.execute = false;
					this.Execute(this.haveMoney);
				}
			}
		}

		[SerializeField]
		CharaListEvent _charaList = new CharaListEvent();
		public CharaListEvent CharaList { get { return _charaList; } }
		[System.Serializable]
		public class CharaListEvent : IDebugParamEvent
		{
			public event System.Action<int> ExecuteCapacity = delegate { };
			public event System.Action<List<CharaInfo>> ExecuteList = delegate { };
			[SerializeField]
			bool executeDummy = false;
			[SerializeField]
			int capacity = 0;

			[SerializeField]
			bool executeList = false;
			[SerializeField]
			List<CharaInfo> list = new List<CharaInfo>();

			public void Update()
			{
				if (this.executeDummy)
				{
					this.executeDummy = false;

					var count = UnityEngine.Random.Range(0, this.capacity + 1);
					this.list.Clear();
					for (int i = 0; i < count; i++)
					{
						var info = new CharaInfo();
						var uuid = (ulong)(i + 1);
						info.DebugRandomSetup();
						info.DebugSetUUID(uuid);
						this.list.Add(info);
					}
					this.ExecuteCapacity(this.capacity);
					this.ExecuteList(new List<CharaInfo>(this.list.ToArray()));
				}
				if (this.executeList)
				{
					this.executeList = false;
					this.ExecuteList(new List<CharaInfo>(this.list.ToArray()));
				}
			}
		}

		[SerializeField]
		SynchroEvent _synchro = new SynchroEvent();
		public SynchroEvent Synchro { get { return _synchro; } }
		[System.Serializable]
		public class SynchroEvent : IDebugParamEvent
		{
			public class Args
			{
				public Scm.Common.GameParameter.PowerupResult Result { get; set; }
				public int HaveMoney { get; set; }
				public int Price { get; set; }
				public int AddOnCharge { get; set; }
				public CharaInfo CharaInfo { get; set; }
			}

			public event System.Action<Args> ExecuteCalc = delegate { };
			[SerializeField]
			bool executeCalc = false;
			public event System.Action<Args> ExecuteFusion = delegate { };
			[SerializeField]
			bool executeFusion = false;
			[SerializeField]
			Scm.Common.GameParameter.PowerupResult result = Scm.Common.GameParameter.PowerupResult.Fail;
			[SerializeField]
			int haveMoney = 0;
			[SerializeField]
			int price = 0;
			[SerializeField]
			int addOnCharge = 0;
			[SerializeField]
			int synchroHitPoint = 0;
			[SerializeField]
			int synchroAttack = 0;
			[SerializeField]
			int synchroDefense = 0;
			[SerializeField]
			int synchroExtra = 0;

			public void Update()
			{
				if (this.executeCalc)
				{
					this.executeCalc = false;
					var args = new Args();
					args.Result = this.result;
					args.HaveMoney = this.haveMoney;
					args.Price = this.price;
					args.AddOnCharge = this.addOnCharge;
					var info = new CharaInfo();
					info.DebugRandomSetup();
					info.DebugSetSynchroParam(this.synchroHitPoint, this.synchroAttack, this.synchroDefense, this.synchroExtra);
					this.ExecuteCalc(args);
				}
				if (this.executeFusion)
				{
					this.executeFusion = false;
					var args = new Args();
					args.Result = this.result;
					args.HaveMoney = this.haveMoney;
					args.Price = this.price;
					args.AddOnCharge = this.addOnCharge;
					var info = new CharaInfo();
					info.DebugRandomSetup();
					info.DebugSetSynchroParam(this.synchroHitPoint, this.synchroAttack, this.synchroDefense, this.synchroExtra);
					this.ExecuteFusion(args);
				}
			}
		}
	}
	void DebugInit()
	{
		var d = this.DebugParam;

		d.ExecuteClose += Close;
		d.ExecuteActive += () =>
		{
			d.ReadMasterData();
			Open();
		};
		d.StatusInfo.Execute += (haveMoney) => { this.SetupStatusInfo(haveMoney); };
		d.CharaList.ExecuteCapacity += (capacity) => { this.SetupCapacity(capacity, capacity); };
		d.CharaList.ExecuteList += (list) => { this.SetupItem(list); };
		d.Synchro.ExecuteCalc += (args) => { this.SynchroCalc((args.Result != Scm.Common.GameParameter.PowerupResult.Fail), args.HaveMoney, args.Price, args.AddOnCharge); };
		d.Synchro.ExecuteFusion += (args) => { this.FusionResult(args.Result, args.HaveMoney, args.Price, args.AddOnCharge, args.CharaInfo); };
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
}
