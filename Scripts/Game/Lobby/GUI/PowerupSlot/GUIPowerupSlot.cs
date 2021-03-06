/// <summary>
/// 強化スロット表示
/// 
/// 2016/03/02
/// </summary>
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.Packet;
using XUI.PowerupSlot;

public class GUIPowerupSlot : Singleton<GUIPowerupSlot>
{
	#region フィールド＆プロパティ
	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	PowerupSlotView _viewAttach = null;
	PowerupSlotView ViewAttach { get { return _viewAttach; } }

	/// <summary>
	/// キャラページリスト
	/// </summary>
	[SerializeField]
	GUICharaPageList _charaPageList = null;
	GUICharaPageList CharaPageList { get { return _charaPageList; } }

	/// <summary>
	/// スロットリスト
	/// </summary>
	[SerializeField]
	GUICharaSlotList _slotList = null;
	GUICharaSlotList SlotList { get { return _slotList; } }

	/// <summary>
	/// ベースキャラアイテム
	/// </summary>
	[SerializeField]
	GameObject _baseCharaItem = null;
	GameObject BaseCharaItem { get { return _baseCharaItem; } }

	/// <summary>
	/// インスタンス化したベースキャラアイテムをアタッチする場所
	/// </summary>
	[SerializeField]
	GameObject _attachBaseChara = null;
	GameObject AttachBaseChara { get { return _attachBaseChara; } }

	/// <summary>
	/// 開始時のアクティブ状態
	/// </summary>
	[SerializeField]
	bool _isStartActive = false;
	bool IsStartActive { get { return _isStartActive; } }

	/// <summary>
	/// ステータス表示フォーマット
	/// </summary>
	[SerializeField]
	string _statusFormat = "{0}";
	string StatusFormat { get { return _statusFormat; } }

	/// <summary>
	/// ステータス試算後表示フォーマット
	/// </summary>
	[SerializeField]
	string _statusCalcFormat = "{0}";
	string StatusCalcFormat { get { return _statusCalcFormat; } }

	/// <summary>
	/// スロット数表示フォーマット
	/// </summary>
	[SerializeField]
	string _slotNumFormat = "{0}/{1}";
	string SlotNumFormat { get { return _slotNumFormat; } }

	/// <summary>
	/// 所持金表示フォーマット
	/// </summary>
	[SerializeField]
	string _haveMoneyFormat = "{0:#,0}";
	string HaveMoneyFormat { get { return _haveMoneyFormat; } }

	/// <summary>
	/// 費用表示フォーマット
	/// </summary>
	[SerializeField]
	string _needMoneyFormat = "{0:#,0}";
	string NeedMoneyFormat { get { return _needMoneyFormat; } }

	/// <summary>
	/// 追加料金表示フォーマット
	/// </summary>
	[SerializeField]
	string _addOnChargeFormat = "{0:#,0}";
	string AddOnChargeFormat { get { return _addOnChargeFormat; } }

	// プレイヤーステータス情報
	PlayerStatusInfo PlayerStatusInfo { get { return NetworkController.ServerValue != null ? NetworkController.ServerValue.PlayerStatusInfo : new PlayerStatusInfo(); } }

	// ベースキャラ
	GUICharaItem BaseChara { get; set; }
	// コントローラー
	IController Controller { get; set; }
	// シリアライズされていないメンバー初期化
	void MemberInit()
	{
		this.BaseChara = null;

		this.Controller = null;
	}
	#endregion

	#region 初期化
	override protected void Awake()
	{
		base.Awake();
		this.MemberInit();
		this.CreateBaseChara();
	}
	void Start()
	{
		this.Construct();
		// 初期アクティブ設定
		this.SetActive(this.IsStartActive, true, this.IsStartActive, false);
	}
	void CreateBaseChara()
	{
		if (this.AttachBaseChara == null)
		{
			Debug.LogWarning("AttachBaseChara is Null!!");
			return;
		}
		if (this.BaseCharaItem == null)
		{
			Debug.LogWarning("BaseCharaItem is Null!!");
			return;
		}
		// ベースキャラをインスタンス化
		var obj = SafeObject.Instantiate(this.BaseCharaItem);
		if (obj == null)
		{
			GameObject.Destroy(obj);
			return;
		}
		var go = obj as GameObject;
		if (go == null)
		{
			Debug.LogWarning(obj.name + " is Mismatch type!! [GameObject]");
			return;
		}

		// 子供を消す
		this.AttachBaseChara.DestroyChild();

		// 親子付け
		go.SetParentWithLayer(this.AttachBaseChara, false);
		// アクティブ化
		if (!go.activeSelf)
		{
			go.SetActive(true);
		}

		// コンポーネント取得
		this.BaseChara = go.GetComponentInChildren(typeof(GUICharaItem)) as GUICharaItem;
		if (this.BaseChara == null)
		{
			Debug.LogWarning("BaseChara is null!! go.GetComponentInChildren(typeof(GUICharaItem))");
			return;
		}
	}
	void Construct()
	{
		// モデル生成
		var model = new Model();
		model.StatusFormat = this.StatusFormat;
		model.StatusCalcFormat = this.StatusCalcFormat;
		model.SlotNumFormat = this.SlotNumFormat;
		model.HaveMoneyFormat = this.HaveMoneyFormat;
		model.NeedMoneyFormat = this.NeedMoneyFormat;
		model.AddOnChargeFormat = this.AddOnChargeFormat;

		// ビュー生成
		IView view = null;
		if (this.ViewAttach != null)
		{
			view = this.ViewAttach.GetComponent(typeof(IView)) as IView;
		}

		// コントローラー生成
		var controller = new Controller(model, view, this.CharaPageList, this.BaseChara, this.SlotList);
		this.Controller = controller;
		this.Controller.OnGetBaseCharaSlot += this.HandleGetBaseCharaSlot;
		this.Controller.OnOK += this.HandleOnOK;
		this.Controller.OnSlotCalc += this.HandleSlotCalc;
		this.Controller.OnPlayerCharacter += this.HandlePlayerCharacter;
	}
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
		if (Instance != null) Instance.SetActive(false, false, false, false);
	}
	/// <summary>
	/// 開く
	/// </summary>
	public static void Open()
	{
		if (Instance != null) Instance.SetActive(true, false, true, false);
	}
	/// <summary>
	/// 開き直す
	/// </summary>
	public static void ReOpen()
	{
		if (Instance != null) Instance.SetActive(true, false, false, true);
	}
	/// <summary>
	/// アクティブ設定
	/// </summary>
	void SetActive(bool isActive, bool isTweenSkip, bool isSetup, bool isSetupReOpen)
	{
		if (isSetup)
		{
			this.Setup();
		}
		else if (isSetupReOpen)
		{
			this.SetupReOpen();
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

		// ステータス情報を更新する
		var info = this.PlayerStatusInfo;
		this.SetupStatusInfo(info.GameMoney);

		// サーバーに問い合わせる
		LobbyPacket.SendPlayerCharacterBox(this.Response);
	}
	/// <summary>
	/// 開き直した時の設定
	/// </summary>
	void SetupReOpen()
	{
		// サーバーに問い合わせる
		LobbyPacket.SendPlayerCharacterBox(this.Response);
	}
	/// <summary>
	/// ステータス情報を更新する
	/// </summary>
	void SetupStatusInfo(int haveMoney)
	{
		if (this.Controller != null)
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
	void Response(LobbyPacket.PlayerCharacterBoxResArgs args)
	{
		GUIDebugLog.AddMessage(string.Format("PlayerCharacterBox:Capacity={0}", args.Capacity));

		this.SetupCapacity(args.Capacity, args.Count);

		// 所有キャラクター情報を取得する
		LobbyPacket.SendPlayerCharacterAll(this.Response);
	}
	/// <summary>
	/// 総数を設定する
	/// </summary>
	void SetupCapacity(int capacity, int count)
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
		GUIDebugLog.AddMessage(string.Format("PlayerCharacterAll:List.Count={0}", args.List.Count));

		// 「通信中」閉じる
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
			this.Controller.SetupItem(list);
		}
	}
	#endregion

	#region SetLockPlayerCharacter パケット
	/// <summary>
	/// ロック設定イベントハンドラー
	/// </summary>
	void HandleLockClickEvent(CharaInfo obj)
	{
		// 「通信中」表示
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX305_Network_Communication));
	}
	/// <summary>
	/// SetLockPlayerCharacterReq パケットのレスポンス
	/// </summary>
	void HandleLockResponse(LobbyPacket.SetLockPlayerCharacterResArgs args)
	{
		this.LockResult(args.Result, args.UUID, args.IsLock);
	}
	/// <summary>
	/// ロックの結果
	/// </summary>
	void LockResult(bool result, ulong uuid, bool isLock)
	{
		// 失敗しているときは閉じる
		if (!result)
		{
			GUIController.Clear();
			return;
		}

		// 所有キャラクター情報を取得する
		LobbyPacket.SendPlayerCharacterAll(this.Response);
	}
	#endregion

	#region GetPowerupSlot パケット
	/// <summary>
	/// ベースキャラスロット取得イベントハンドラー
	/// </summary>
	void HandleGetBaseCharaSlot(object sender, GetBaseCharaSlotEventArgs e)
	{
		LobbyPacket.SendGetPowerupSlot(e.BaseCharaUUID, this.Response);
	}
	/// <summary>
	/// GetPowerupSlotReq パケットのレスポンス
	/// </summary>
	void Response(LobbyPacket.GetPowerupSlotResArgs args)
	{
		GUIDebugLog.AddMessage(string.Format(
			"GetPowerupSlot:id={0} bHp={1} bAk={2} bDf={3} bEx={4} slot={5}",
			args.BaseCharaUUID, args.BonusHitPoint, args.BonusAttack, args.BonusDefense, args.BonusExtra, args.SlotInfoList.Count
			));

		this.BaseCharaChangeResult(args.BaseCharaUUID, args.BonusHitPoint, args.BonusAttack, args.BonusDefense, args.BonusExtra, args.SlotInfoList);
	}
	/// <summary>
	/// ベースキャラ変更結果
	/// </summary>
	void BaseCharaChangeResult(ulong baseCharaUUID, int bonusHitPoint, int bonusAttack, int bonusDefense, int bonusExtra, List<PowerupSlotCharaInfo> slotInfoList)
	{
		if (this.Controller != null)
		{
			this.Controller.GetBaseCharaSlotResult(baseCharaUUID, bonusHitPoint, bonusAttack, bonusDefense, bonusExtra, slotInfoList);
		}
	}
	#endregion

	#region SetPowerupSlot パケット
	/// <summary>
	/// OKボタンイベントハンドラー
	/// </summary>
	void HandleOnOK(object sender, OKEventArgs e)
	{
		LobbyPacket.SendSetPowerupSlot(e.BaseCharaUUID, e.SlotCharaUUIDList.ToArray(), this.Response);

		// 「通信中」表示
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX305_Network_Communication));
	}
	/// <summary>
	/// SetPowerupSlotReq パケットのレスポンス
	/// </summary>
	void Response(LobbyPacket.SetPowerupSlotResArgs args)
	{
		GUIDebugLog.AddMessage(string.Format(
			"SetPowerupSlot:ret={0} Avatar={1} UUID={2} money={3:#,0} price={4:#,0} add={5:#,0}",
			args.Result,
			args.CharaInfo.AvatarType, args.CharaInfo.UUID,
			args.Money, args.Price, args.AddOnCharge
			));

		// 「通信中」閉じる
		GUISystemMessage.Close();

		// 所有キャラクター情報を取得する
		LobbyPacket.SendPlayerCharacterAll(this.Response);

		// ステータス情報を更新する
		var info = this.PlayerStatusInfo;
		this.SetupStatusInfo(info.GameMoney);

		this.OKResult(args.Result, args.Money, args.Price, args.AddOnCharge, args.CharaInfo);
	}
	/// <summary>
	/// OKボタンの結果
	/// </summary>
	void OKResult(bool result, int money, int price, int addOnCharge, CharaInfo info)
	{
		// 失敗しているときは閉じる
		if (!result)
		{
			GUIController.Clear();
			return;
		}

		if (this.Controller != null)
		{
			this.Controller.OKResult(money, price, addOnCharge, info);
		}
	}
	#endregion

	#region SetPowerupSlotCalc パケット
	/// <summary>
	/// スロット試算イベントハンドラー
	/// </summary>
	void HandleSlotCalc(object sender, SlotCalcEventArgs e)
	{
		LobbyPacket.SendSetPowerupSlotCalc(e.BaseCharaUUID, e.SlotCharaUUIDList.ToArray(), this.Response);
	}
	/// <summary>
	/// SetPowerupSlotCalc パケットのレスポンス
	/// </summary>
	void Response(LobbyPacket.SetPowerupSlotCalcResArgs args)
	{
		GUIDebugLog.AddMessage(string.Format(
			"SetPowerupSlotCalc:ret={0} money={1} bHp={2} bAk={3} bDf={4} bEx={5} price={6} add={7}",
			args.Result, args.Money,
			args.BonusHitPoint, args.BonusAttack, args.BonusDefense, args.BonusExtra,
			args.Price, args.AddOnCharge
			));

		this.SlotCalcResult(args.Result, args.Money, args.BonusHitPoint, args.BonusAttack, args.BonusDefense, args.BonusExtra, args.Price, args.AddOnCharge);
	}
	/// <summary>
	/// スロット試算結果
	/// </summary>
	void SlotCalcResult(bool result, int money, int hitPointBonus, int attackBonus, int defenseBonus, int extraBonus, int price, int addOnCharge)
	{
		//// 失敗しているときは閉じる
		//if (!result)
		//{
		//	GUIController.Clear();
		//}

		if (this.Controller != null)
		{
			this.Controller.SlotCalcResult(money, hitPointBonus, attackBonus, defenseBonus, extraBonus, price, addOnCharge);
		}
	}
	#endregion

	#region PlayerCharacter パケット
	/// <summary>
	/// プレイヤーキャラクター取得ハンドラー
	/// </summary>
	void HandlePlayerCharacter(object sender, XUI.PowerupSlot.PlayerCharacterEventArgs e)
	{
		LobbyPacket.SendPlayerCharacter(e.BaseCharaUUID, this.Response);

		// 「通信中」表示
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX305_Network_Communication));
	}
	/// <summary>
	/// PlayerCharacterReq パケットのレスポンス
	/// </summary>
	void Response(LobbyPacket.PlayerCharacterResArgs args)
	{
		GUIDebugLog.AddMessage(string.Format(
			"PlayerCharacter:Avatar={0} UUID={1} slot={2}",
			args.CharaInfo.AvatarType, args.CharaInfo.UUID,
			args.SlotList.Count
			));

		// 通信中を閉じる
		GUISystemMessage.Close();

		// プレイヤーキャラクター取得結果
		this.PlayerCharacterResult(args.CharaInfo, args.SlotList);
	}
	/// <summary>
	/// プレイヤーキャラクター取得結果
	/// </summary>
	void PlayerCharacterResult(CharaInfo info, List<CharaInfo> slotList)
	{
		if (this.Controller != null)
		{
			this.Controller.PlayerCharacterResult(info, slotList);
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
			this.AddEvent(this.GetSlot);
			this.AddEvent(this.SetSlot);
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
		GetSlotEvent _getSlot = new GetSlotEvent();
		public GetSlotEvent GetSlot { get { return _getSlot; } }
		[System.Serializable]
		public class GetSlotEvent : IDebugParamEvent
		{
			public class Args
			{
				public ulong BaseCharaUUID { get; set; }
				public int BonusHitPoint { get; set; }
				public int BonusAttack { get; set; }
				public int BonusDefense { get; set; }
				public int BonusExtra { get; set; }
				public List<PowerupSlotCharaInfo> SlotInfoList { get; set; }
			}

			public event System.Action<Args> ExecuteDummy = delegate { };
			public event System.Action<Args> Execute = delegate { };

			[SerializeField]
			bool executeDummy = false;
			[SerializeField]
			bool execute = false;

			[SerializeField]
			int baseCharaUUID = 0;
			public int BaseCharaUUID { set { baseCharaUUID = value; } }
			[SerializeField]
			int bonusHitPoint = 0;
			[SerializeField]
			int bonusAttack = 0;
			[SerializeField]
			int bonusDefense = 0;
			[SerializeField]
			int bonusExtra = 0;
			[SerializeField]
			List<PowerupSlotCharaInfo> slotInfoList = new List<PowerupSlotCharaInfo>();

			public void Update()
			{
				if (this.executeDummy)
				{
					this.executeDummy = false;
					var args = new Args();
					this.baseCharaUUID = 0;
					this.bonusHitPoint = UnityEngine.Random.Range(-99, 99);
					this.bonusAttack = UnityEngine.Random.Range(-99, 99);
					this.bonusDefense = UnityEngine.Random.Range(-99, 99);
					this.bonusExtra = UnityEngine.Random.Range(-99, 99);
					args.BaseCharaUUID = (ulong)this.baseCharaUUID;
					args.BonusHitPoint = this.bonusHitPoint;
					args.BonusAttack = this.bonusAttack;
					args.BonusDefense = this.bonusDefense;
					args.BonusExtra = this.bonusExtra;
					args.SlotInfoList = this.slotInfoList;
					this.ExecuteDummy(args);
				}
				if (this.execute)
				{
					this.execute = false;
					var args = new Args();
					args.BaseCharaUUID = (ulong)this.baseCharaUUID;
					args.BonusHitPoint = this.bonusHitPoint;
					args.BonusAttack = this.bonusAttack;
					args.BonusDefense = this.bonusDefense;
					args.BonusExtra = this.bonusExtra;
					args.SlotInfoList = this.slotInfoList;
					this.Execute(args);
				}
			}
		}

		[SerializeField]
		SetSlotEvent _setSlot = new SetSlotEvent();
		public SetSlotEvent SetSlot { get { return _setSlot; } }
		[System.Serializable]
		public class SetSlotEvent : IDebugParamEvent
		{
			public class Args
			{
				public bool Result { get; set; }
				public int Money { get; set; }
				public CharaInfo Info { get; set; }
				public int BonusHitPoint { get; set; }
				public int BonusAttack { get; set; }
				public int BonusDefense { get; set; }
				public int BonusExtra { get; set; }
				public int Price { get; set; }
				public int AddOnCharge { get; set; }
			}

			public event System.Action<Args> ExecuteCalc = delegate { };
			[SerializeField]
			bool executeCalc = false;
			public event System.Action<Args> ExecuteOK = delegate { };
			[SerializeField]
			bool executeOK = false;
			[SerializeField]
			bool result = false;
			[SerializeField]
			int money = 0;
			[SerializeField]
			CharaInfo info = new CharaInfo();
			[SerializeField]
			int bonusHitPoint = 0;
			[SerializeField]
			int bonusAttack = 0;
			[SerializeField]
			int bonusDefense = 0;
			[SerializeField]
			int bonusExtra = 0;
			[SerializeField]
			int price = 0;
			[SerializeField]
			int addOnCharge = 0;

			public void Update()
			{
				if (this.executeCalc)
				{
					this.executeCalc = false;
					var args = new Args();
					args.Result = this.result;
					args.Money = this.money;
					args.Info = this.info.Clone();
					args.BonusHitPoint = this.bonusHitPoint;
					args.BonusAttack = this.bonusAttack;
					args.BonusDefense = this.bonusDefense;
					args.BonusExtra = this.bonusExtra;
					args.Price = this.price;
					args.AddOnCharge = this.addOnCharge;
					this.ExecuteCalc(args);
				}
				if (this.executeOK)
				{
					this.executeOK = false;
					var args = new Args();
					args.Result = this.result;
					args.Money = this.money;
					args.Info = this.info.Clone();
					args.BonusHitPoint = this.bonusHitPoint;
					args.BonusAttack = this.bonusAttack;
					args.BonusDefense = this.bonusDefense;
					args.BonusExtra = this.bonusExtra;
					args.Price = this.price;
					args.AddOnCharge = this.addOnCharge;
					this.ExecuteOK(args);
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
		d.GetSlot.ExecuteDummy += this.DebugGetSlotDummyExecute;
		d.GetSlot.Execute += (args) => { this.BaseCharaChangeResult(args.BaseCharaUUID, args.BonusHitPoint, args.BonusAttack, args.BonusDefense, args.BonusExtra, args.SlotInfoList); };
		d.SetSlot.ExecuteCalc += (args) => { this.SlotCalcResult(args.Result, args.Money, args.BonusHitPoint, args.BonusAttack, args.BonusDefense, args.BonusExtra, args.Price, args.AddOnCharge); };
		d.SetSlot.ExecuteOK += (args) => { this.OKResult(args.Result, args.Money, args.Price, args.AddOnCharge, args.Info); };
	}
	void DebugGetSlotDummyExecute(GUIDebugParam.GetSlotEvent.Args args)
	{
		var d = this.DebugParam;

		var powerupSlotNum = 0;
		if (this.BaseChara != null)
		{
			var info = this.BaseChara.GetCharaInfo();
			if (info != null)
			{
				args.BaseCharaUUID = info.UUID;
				d.GetSlot.BaseCharaUUID = (int)info.UUID;
				powerupSlotNum = info.PowerupSlotNum;
			}
		}
		if (this.CharaPageList != null && powerupSlotNum > 0)
		{
			var list = this.CharaPageList.GetCharaInfo();
			if (list != null)
			{
				// キャラリスト内のインデックスリストをシャッフルする
				var indexList = new List<int>();
				for (int i = 0; i < list.Count; i++) indexList.Add(i);
				indexList.Sort((a, b) => UnityEngine.Random.Range(-1, 2));
				// スロットに入れる数をランダム設定する
				var slotMax = Math.Min(powerupSlotNum, list.Count);
				var slotNum = UnityEngine.Random.Range(0, slotMax);
				// キャラリスト内で既にスロットに入っている物だけ摘出してスロット設定する
				var count = 0;
				args.SlotInfoList.Clear();
				for (int i = 0; i < indexList.Count; i++)
				{
					var index = indexList[i];
					var info = list[index];
					if (info == null) continue;
					if (!info.IsInSlot) continue;

					var slotInfo = new PowerupSlotCharaInfo();
					slotInfo.DebugRandomSetup();
					slotInfo.DebugSetUUID(info.UUID);
					slotInfo.DebugSetAvatarType(info.CharacterMasterID);
					args.SlotInfoList.Add(slotInfo);

					count++;
					if (slotNum <= count) break;
				}

				args.SlotInfoList = new List<PowerupSlotCharaInfo>(args.SlotInfoList);
			}
		}
		this.BaseCharaChangeResult(args.BaseCharaUUID, args.BonusHitPoint, args.BonusAttack, args.BonusDefense, args.BonusExtra, args.SlotInfoList);
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
