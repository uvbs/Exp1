/// <summary>
/// キャラBOX
/// 
/// 2016/04/29
/// </summary>
using UnityEngine;
using System;
using System.Collections.Generic;
using Scm.Common;
using Scm.Common.Packet;
using XUI.CharacterBox;

public class GUICharacterBox : Singleton<GUICharacterBox>
{
	#region フィールド&プロパティ
	/// <summary>
	/// ビューアタッチ
	/// </summary>
	[SerializeField]
	private CharacterBoxView _viewAttach = null;
	private CharacterBoxView ViewAttach { get { return _viewAttach; } }

	/// <summary>
	/// キャラページリスト
	/// </summary>
	[SerializeField]
	private GUICharaPageList _charaPageList = null;
	private GUICharaPageList CharaPageList { get { return _charaPageList; } }

	/// <summary>
	/// キャラ売却リスト
	/// </summary>
	[SerializeField]
	private GUISelectCharaList _sellCharaList = null;
	private GUISelectCharaList SellCharaList { get { return _sellCharaList; } }

	/// <summary>
	/// キャラアイテムのリソース
	/// </summary>
	[SerializeField]
	private GameObject _charaItemResource = null;
	private GameObject CharaItemResource { get { return _charaItemResource; } }

	/// <summary>
	/// インスタンス化したベースキャラをアタッチする場所
	/// </summary>
	[SerializeField]
	private GameObject _attachBaseChara = null;
	private GameObject AttachBaseChara { get { return _attachBaseChara; } }

	/// <summary>
	/// 開始時のアクティブ状態
	/// </summary>
	[SerializeField]
	private bool _isStartActive = false;
	private bool IsStartActive { get { return _isStartActive; } }

	/// <summary>
	/// まとめて売却できるキャラ最大数
	/// </summary>
	[SerializeField]
	private int _multiSellCharaMax = 100;
	public int MultiSellCharaMax { get { return _multiSellCharaMax; } }

	/// <summary>
	/// フォーマット
	/// </summary>
	[SerializeField]
	private FormatAttachObject _formatAttach = null;
	private FormatAttachObject FormatAttach { get { return _formatAttach; } }
	[Serializable]
	public class FormatAttachObject
	{
		/// <summary>
		/// 所持金表示フォーマット
		/// </summary>
		[SerializeField]
		private string _haveMoneyFormat = "{0:#,0}";
		public string HaveMoneyFormat { get { return _haveMoneyFormat; } }

		/// <summary>
		/// キャラ売却額表示フォーマット
		/// </summary>
		[SerializeField]
		private string _soldPriceFormat = "{0:#,0}";
		public string SoldPriceFormat { get { return _soldPriceFormat; } }

		/// <summary>
		/// キャラ総売却額表示フォーマット
		/// </summary>
		[SerializeField]
		private string _totalSoldPriceFormat = "{0:#,0}";
		public string TotalSoldPriceFormat { get { return _totalSoldPriceFormat; } }

		/// <summary>
		/// 経験値
		/// </summary>
		[SerializeField]
		private string _expFormat = "{0:#,0}";
		public string ExpFormat { get { return _expFormat; } }

		/// <summary>
		/// 生命力
		/// </summary>
		[SerializeField]
		private string _hitPointFormat = "{0}";
		public string HitPointFormat { get { return _hitPointFormat; } }

		/// <summary>
		/// 攻撃力
		/// </summary>
		[SerializeField]
		private string _attackFormat = "{0}";
		public string AttackFormat { get { return _attackFormat; } }

		/// <summary>
		/// 防御力
		/// </summary>
		[SerializeField]
		private string _dfenceFormat = "{0}";
		public string DefenceFormat { get { return _dfenceFormat; } }

		/// <summary>
		/// 特殊能力
		/// </summary>
		[SerializeField]
		private string _extraFormat = "{0}";
		public string ExtraFormat { get { return _extraFormat; } }
	}

	// プレイヤーステータス情報
	private PlayerStatusInfo PlayerStatusInfo { get { return NetworkController.ServerValue != null ? NetworkController.ServerValue.PlayerStatusInfo : new PlayerStatusInfo(); } }

	/// <summary>
	/// ベースキャラ
	/// </summary>
	private GUICharaItem BaseChara { get; set; }

	/// <summary>
	/// コントローラ
	/// </summary>
	private IController Controller { get; set; }
	#endregion

	#region 初期化
	protected override void Awake()
	{
		base.Awake();
		this.MemberInit();
		this.CreateBaseChara();
	}
	void Start()
	{
		this.Constrcut();
		// 売却リスト設定
		if (this.SellCharaList != null)
		{
			this.SellCharaList.SetupCapacity(this.MultiSellCharaMax);
		}
		// 初期化アクティブ設定
		this.SetActive(this.IsStartActive, true);
		if (this.IsStartActive)
		{
			this.Setup();
		}
	}

	/// <summary>
	/// シリアライズされていないメンバー初期化
	/// </summary>
	private void MemberInit()
	{
		this.BaseChara = null;
		this.Controller = null;
	}
	/// <summary>
	/// ベースキャラ生成
	/// </summary>
	private void CreateBaseChara()
	{
		// 子を消す
		this.AttachBaseChara.DestroyChild();

		GUICharaItem item = GUICharaItem.Create(this.CharaItemResource, this.AttachBaseChara.transform, 0);
		if (item == null)
		{
			Debug.LogWarning("GameItem is null!!");
		}
		this.BaseChara = item;
	}
	private void Constrcut()
	{
		var model = new Model();
		var formatAttach = this.FormatAttach;
		if (formatAttach != null)
		{
			model.HaveMoneyFormat = formatAttach.HaveMoneyFormat;
			model.SoldPriceFormat = formatAttach.SoldPriceFormat;
			model.TotalSoldPriceFormat = formatAttach.TotalSoldPriceFormat;
			model.ExpFormat = formatAttach.ExpFormat;
			model.HitPointFormat = formatAttach.HitPointFormat;
			model.AttackFormat = formatAttach.AttackFormat;
			model.DefenceFormat = formatAttach.DefenceFormat;
			model.ExtraFormat = formatAttach.ExtraFormat;
		}

		// ビュー生成
		IView view = null;
		if (this.ViewAttach != null)
		{
			view = this.ViewAttach.GetComponent(typeof(IView)) as IView;
		}

		// コントローラ生成
		var controller = new Controller(model, view, this.CharaPageList, this.SellCharaList, this.BaseChara);
		this.Controller = controller;
		this.Controller.OnSellMultiCalc += this.HandleSellMultiCalc;
		this.Controller.OnSellMulti += this.HandleSellMulti;
		this.Controller.OnCharaLock += this.HandleCharaLock;
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
		GUICharaSimpleInfo.AddLockClickEvent(this.HandleSimpleInfoCharaLock);
		GUICharaSimpleInfo.AddLockResponseEvent(this.Response);
	}
	void OnDisable()
	{
		GUICharaSimpleInfo.RemoveLockClickEvent(this.HandleSimpleInfoCharaLock);
		GUICharaSimpleInfo.RemoveLockResponseEvent(this.Response);
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
	void SetActive(bool isActive, bool isTweenSkip)
	{
		if (this.Controller != null)
		{
			this.Controller.SetActive(isActive, isTweenSkip);
		}
	}
	#endregion

	#region 各情報更新
	/// <summary>
	/// 初期設定
	/// </summary>
	private void Setup()
	{
		if (this.Controller != null)
		{
			this.Controller.Setup();
		}

		// ステータス情報を更新する
		var info = this.PlayerStatusInfo;
		this.SetupStatusInfo(info.GameMoney);

		// サーバからキャラBOX情報を取得
		LobbyPacket.SendPlayerCharacterBox(this.Response);
	}
	/// <summary>
	/// 再初期化
	/// </summary>
	private void ReSetup()
	{
		if (this.Controller != null)
		{
			this.Controller.ReSetup();
		}

		// ステータス情報を更新する
		var info = this.PlayerStatusInfo;
		this.SetupStatusInfo(info.GameMoney);

		// サーバからアイテムBOX情報を取得
		LobbyPacket.SendPlayerCharacterBox(this.Response);
	}
	/// <summary>
	/// ステータス情報を更新する
	/// </summary>
	/// <param name="haveMoney"></param>
	private void SetupStatusInfo(int haveMoney)
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
	private void Response(LobbyPacket.PlayerCharacterBoxResArgs args)
	{
		this.SetupCapacity(args.Capacity, args.Count);

		// 所有キャラ情報を取得する
		LobbyPacket.SendPlayerCharacterAll(this.Response);
	}
	/// <summary>
	/// 総数を設定する
	/// </summary>
	private void SetupCapacity(int capacity, int itemCount)
	{
		if (this.Controller == null) { return; }
		this.Controller.SetupCapacity(capacity, itemCount);
	}
	#endregion

	#region PlayerCharacterAll パケット
	/// <summary>
	/// PlayerCharacterAllReq パケットのレスポンス
	/// </summary>
	private void Response(LobbyPacket.PlayerCharacterAllResArgs args)
	{
		// 通信中メッセージを閉じる
		GUISystemMessage.Close();

		this.SetupItem(args.List);
	}
	/// <summary>
	/// 個々のアイテムの設定をする
	/// </summary>
	private void SetupItem(List<CharaInfo> list)
	{
		if (this.Controller == null) { return; }
		this.Controller.SetupCharaInfoList(list);
	}
	#endregion

	#region SellMultiCalc パケット
	private void HandleSellMultiCalc(object sender, XUI.CharacterBox.SellMultiCalcEventArgs e)
	{
		LobbyPacket.SendSellMultiPlayerCharacterCalc(e.SellCharaUUIDList.ToArray(), this.Response);
	}
	/// <summary>
	/// SellMultiCalcReq パケットのレスポンス
	/// </summary>
	void Response(LobbyPacket.SellMultiPlayerCharacterCalcResArgs args)
	{
		this.SellMultiCalc(args.Result, args.PlayerCharacterUuids, args.Money, args.SoldPrice, args.AddOnCharge);
	}
	/// <summary>
	/// 合成試算結果
	/// </summary>
	void SellMultiCalc(bool result, List<ulong> sellUUIDList, int money, int soldPrice, int addOnCharge)
	{
		if (this.Controller != null)
		{
			this.Controller.MultiSellCalcResult(result, sellUUIDList, money, soldPrice, addOnCharge);
		}
	}
	#endregion

	#region SellMulti パケット
	/// <summary>
	/// まとめて売却イベントハンドラー
	/// </summary>
	private void HandleSellMulti(object sender, XUI.CharacterBox.SellMultiEventArgs e)
	{
		LobbyPacket.SendSellMultiPlayerCharacter(e.SellCharaUUIDList.ToArray(), this.Response);

		// 通信中表示
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX305_Network_Communication));
	}
	/// <summary>
	/// SellMultiReq パケットのレスポンス
	/// </summary>
	void Response(LobbyPacket.SellMultiPlayerCharacterResArgs args)
	{
		// キャラBOX情報を取得し直す
		LobbyPacket.SendPlayerCharacterBox(this.Response);

		// ステータス情報を更新
		var info = this.PlayerStatusInfo;
		this.SetupStatusInfo(info.GameMoney);

		this.SellMulti(args.Result, args.PlayerCharacterUuids, args.Money, args.SoldPrice, args.AddOnCharge);
	}
	/// <summary>
	/// 合成試算結果
	/// </summary>
	void SellMulti(bool result, List<ulong> sellUUIDList, int money, int soldPrice, int addOnCharge)
	{
		if (!result)
		{
			// 売却失敗時は画面を閉じる
			GUIController.Clear();

			// 警告ログ
			string msg = string.Format("SellMultiPlayerCharacterResponse Result={0} UUID={1} money={2} soldPrice={3} addOnCharge",
				result, sellUUIDList.ToArray().ToStringArray(), money, soldPrice, addOnCharge);
			Debug.LogWarning(msg);
			BugReportController.SaveLogFileWithOutStackTrace(msg);
			return;
		}

		if (this.Controller != null)
		{
			this.Controller.MultiSellResult(result, sellUUIDList, money, soldPrice, addOnCharge);
		}
	}
	#endregion

	#region SetLockPlayerCharacter パケット
	/// <summary>
	/// キャラロックイベントハンドラー
	/// </summary>
	private void HandleCharaLock(object sender, CharaLockEventArgs e)
	{
		// ロックパケット送信
		LobbyPacket.SendSetLockPlayerCharacter(e.UUID, e.IsLock, this.Response);

		// 「通信中」表示
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX305_Network_Communication));
	}
	/// <summary>
	/// キャラ簡易情報側のロックイベントハンドラー
	/// </summary>
	private void HandleSimpleInfoCharaLock(CharaInfo obj)
	{
		// 「通信中」表示
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX305_Network_Communication));
	}
	/// <summary>
	/// SetLockPlayerCharacterReq パケットのレスポンス
	/// </summary>
	private void Response(LobbyPacket.SetLockPlayerCharacterResArgs args)
	{
		// 所有アイテム情報を取得し直す
		LobbyPacket.SendPlayerCharacterAll(this.Response);
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
			public event System.Action<int, int> ExecuteCapacity = delegate { };
			public event System.Action<List<CharaInfo>> ExecuteList = delegate { };
			[SerializeField]
			bool executeDummy = false;
			[SerializeField]
			int capacity = 0;
			[SerializeField]
			int itemCount = 0;

			[SerializeField]
			bool executeList = false;
			[SerializeField]
			List<CharaInfo> list = new List<CharaInfo>();

			public void Update()
			{
				if(this.executeDummy)
				{
					this.executeDummy = false;

					var count = this.itemCount;
					this.list.Clear();
					for(int i = 0; i < count; ++i)
					{
						var info = new CharaInfo();
						info.DebugRandomSetup();
						this.list.Add(info);
					}
					this.ExecuteCapacity(this.capacity, this.itemCount);
					this.ExecuteList(new List<CharaInfo>(this.list.ToArray()));
				}
				if(this.executeList)
				{
					this.executeList = false;
					this.ExecuteList(new List<CharaInfo>(this.list.ToArray()));

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
		d.CharaList.ExecuteCapacity += (capacity, itemCount) => { this.SetupCapacity(capacity, itemCount); };
		d.CharaList.ExecuteList += (list) => { this.SetupItem(list); };
	}
	bool _isDebugInit = false;
	void DebugUpdate()
	{
		if(!this._isDebugInit)
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
