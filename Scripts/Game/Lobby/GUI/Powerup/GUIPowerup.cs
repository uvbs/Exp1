/// <summary>
/// 強化合成表示
/// 
/// 2016/01/08
/// </summary>
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.Packet;

public class GUIPowerup : Singleton<GUIPowerup>
{
	#region フィールド＆プロパティ
	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	XUI.Powerup.PowerupView _viewAttach = null;
	XUI.Powerup.PowerupView ViewAttach { get { return _viewAttach; } }

	/// <summary>
	/// キャラページリスト
	/// </summary>
	[SerializeField]
	GUICharaPageList _charaPageList = null;
	GUICharaPageList CharaPageList { get { return _charaPageList; } }

	/// <summary>
	/// 餌リスト
	/// </summary>
	[SerializeField]
	GUISelectCharaList _baitList = null;
	GUISelectCharaList BaitList { get { return _baitList; } }

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
	/// 餌の最大数
	/// </summary>
	[SerializeField]
	int _baitMaxCount = 10;
	int BaitMaxCount { get { return _baitMaxCount; } }

	/// <summary>
	/// レベル表示フォーマット
	/// </summary>
	[SerializeField]
	string _lvFormat = "{0}";
	string LvFormat { get { return _lvFormat; } }

	/// <summary>
	/// 経験値表示フォーマット
	/// </summary>
	[SerializeField]
	string _expFormat = "{0:#,0}";
	string ExpFormat { get { return _expFormat; } }

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
	XUI.Powerup.IController Controller { get; set; }
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
		// リスト設定
		if (this.BaitList != null)
		{
			this.BaitList.SetupCapacity(this.BaitMaxCount);
		}
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
		var model = new XUI.Powerup.Model();
		model.TakeExpFormat = this.ExpFormat;
		model.LvFormat = this.LvFormat;
		model.ExpFormat = this.ExpFormat;
		model.NextLvExpFormat = this.ExpFormat;
		model.AfterLvFormat = this.LvFormat;
		model.AfterExpFormat = this.ExpFormat;
		model.AfterNextLvExpFormat = this.ExpFormat;
		model.AfterOverflowExpFormat = this.ExpFormat;
		model.HaveMoneyFormat = this.HaveMoneyFormat;
		model.NeedMoneyFormat = this.NeedMoneyFormat;
		model.AddOnChargeFormat = this.AddOnChargeFormat;

		// ビュー生成
		XUI.Powerup.IView view = null;
		if (this.ViewAttach != null)
		{
			view = this.ViewAttach.GetComponent(typeof(XUI.Powerup.IView)) as XUI.Powerup.IView;
		}

		// コントローラー生成
		var controller = new XUI.Powerup.Controller(model, view, this.CharaPageList, this.BaseChara, this.BaitList);
		this.Controller = controller;
		this.Controller.OnFusion += this.HandleFusion;
		this.Controller.OnFusionCalc += this.HandleFusionCalc;
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

	#region Powerup パケット
	/// <summary>
	/// 合成イベントハンドラー
	/// </summary>
	void HandleFusion(object sender, XUI.Powerup.FusionEventArgs e)
	{
		LobbyPacket.SendPowerup(e.BaseCharaUUID, e.BaitCharaUUIDList.ToArray(), this.Response);

		// 「通信中」表示
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX305_Network_Communication));
	}
	/// <summary>
	/// PowerupReq パケットのレスポンス
	/// </summary>
	void Response(LobbyPacket.PowerupResArgs args)
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
	void FusionResult(Scm.Common.GameParameter.PowerupResult result, int money, int price, int addOnCharge, CharaInfo info)
	{
		// 失敗しているときは閉じる
		switch (result)
		{
			case Scm.Common.GameParameter.PowerupResult.Fail:
				GUIController.Clear();
				return;
		}

		if (this.Controller != null)
		{
			this.Controller.FusionResult(result, money, price, addOnCharge, info);
		}
	}
	#endregion

	#region PowerupCalc パケット
	/// <summary>
	/// 合成試算イベントハンドラー
	/// </summary>
	void HandleFusionCalc(object sender, XUI.Powerup.FusionCalcEventArgs e)
	{
		LobbyPacket.SendPowerupCalc(e.BaseCharaUUID, e.BaitCharaUUIDList.ToArray(), this.Response);
	}
	/// <summary>
	/// PowerupCalcReq パケットのレスポンス
	/// </summary>
	void Response(LobbyPacket.PowerupCalcResArgs args)
	{
		this.PowerupCalc(args.Exp, args.Money, args.Price, args.AddOnCharge);
	}
	/// <summary>
	/// 合成試算結果
	/// </summary>
	void PowerupCalc(int exp, int money, int price, int addOnCharge)
	{
		if (this.Controller != null)
		{
			this.Controller.FusionCalcResult(exp, money, price, addOnCharge);
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

	#region PlayerCharacter パケット
	/// <summary>
	/// プレイヤーキャラクター取得ハンドラー
	/// </summary>
	void HandlePlayerCharacter(object sender, XUI.Powerup.PlayerCharacterEventArgs e)
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
			this.AddEvent(this.Powerup);
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
		PowerupEvent _powerup = new PowerupEvent();
		public PowerupEvent Powerup { get { return _powerup; } }
		[System.Serializable]
		public class PowerupEvent : IDebugParamEvent
		{
			public class Args
			{
				public Scm.Common.GameParameter.PowerupResult Result { get; set; }
				public int TotalExp { get; set; }
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
			int totalExp = 0;
			[SerializeField]
			int haveMoney = 0;
			[SerializeField]
			int price = 0;
			[SerializeField]
			int addOnCharge = 0;
			[SerializeField]
			CharaInfo charaInfo = new CharaInfo();

			public void Update()
			{
				if (this.executeCalc)
				{
					this.executeCalc = false;
					var args = new Args();
					args.Result = this.result;
					args.TotalExp = this.totalExp;
					args.HaveMoney = this.haveMoney;
					args.Price = this.price;
					args.AddOnCharge = this.addOnCharge;
					args.CharaInfo = this.charaInfo;
					this.ExecuteCalc(args);
				}
				if (this.executeFusion)
				{
					this.executeFusion = false;
					var args = new Args();
					args.Result = this.result;
					args.TotalExp = this.totalExp;
					args.HaveMoney = this.haveMoney;
					args.Price = this.price;
					args.AddOnCharge = this.addOnCharge;
					args.CharaInfo = this.charaInfo;
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
		d.Powerup.ExecuteCalc += (args) => { this.PowerupCalc(args.TotalExp, args.HaveMoney, args.Price, args.AddOnCharge); };
		d.Powerup.ExecuteFusion += (args) => { this.FusionResult(args.Result, args.HaveMoney, args.Price, args.AddOnCharge, args.CharaInfo); };
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
