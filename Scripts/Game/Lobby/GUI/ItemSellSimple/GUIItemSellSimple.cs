/// <summary>
/// アイテム売却
/// 
/// 2016/04/08
/// </summary>
using UnityEngine;
using System;
using XUI.ItemSellSimple;

public class GUIItemSellSimple : Singleton<GUIItemSellSimple>
{
	#region フィールド&プロパティ
	/// <summary>
	/// ビューアタッチ
	/// </summary>
	[SerializeField]
	private ItemSellSimpleView _viewAttach = null;
	private ItemSellSimpleView ViewAttach { get { return _viewAttach; } }

	/// <summary>
	/// 売却アイテムのリソース
	/// </summary>
	[SerializeField]
	private GameObject _sellItemResource = null;
	private GameObject SellItemResource { get { return _sellItemResource; } }

	/// <summary>
	/// インスタンス化した売却アイテムをアタッチする場所
	/// </summary>
	[SerializeField]
	private GameObject _attachSellItem = null;
	private GameObject AttachSellItem { get { return _attachSellItem; } }

	/// <summary>
	/// 開始時のアクティブ状態
	/// </summary>
	[SerializeField]
	private bool _isStartActive = false;
	private bool IsStartActive { get { return _isStartActive; } }

	/// <summary>
	/// 売却額フォーマット
	/// </summary>
	[SerializeField]
	private string _soldPriceFormat = "{0:#,0}";
	private string SoldPriceFormat { get { return _soldPriceFormat; } }

	/// <summary>
	/// 所持金表示フォーマット
	/// </summary>
	[SerializeField]
	private string _haveMoneyFormat = "{0:#,0}";
	private string HaveMoneyFormat { get { return _haveMoneyFormat; } }

	/// <summary>
	/// 売却アイテム
	/// </summary>
	private GUIItem SellItem { get; set; }

	// プレイヤーステータス情報
	private PlayerStatusInfo PlayerStatusInfo { get { return NetworkController.ServerValue != null ? NetworkController.ServerValue.PlayerStatusInfo : new PlayerStatusInfo(); } }

	/// <summary>
	/// コントローラ
	/// </summary>
	private IController Controller { get; set; }

	/// <summary>
	/// 売却通知用
	/// </summary>
	private event Action OnSellItemEvent = () => { };

	/// <summary>
	/// SellPlayerItemパケットの受信時の通知用
	/// </summary>
	private event Action OnSellItemResponseEvent = () => { };
	#endregion

	#region 初期化
	protected override void Awake()
	{
		base.Awake();
		this.MemberInit();
		this.CreateSellItem();
	}
	void Start()
	{
		this.Constrcut();
		// 初期化アクティブ設定
		this.SetActive(this.IsStartActive, true);
	}

	/// <summary>
	/// シリアライズされていないメンバー初期化
	/// </summary>
	private void MemberInit()
	{
		this.SellItem = null;
		this.Controller = null;
	}

	/// <summary>
	/// 売却アイテム生成
	/// </summary>
	private void CreateSellItem()
	{
		// 子を消す
		this.AttachSellItem.DestroyChild();

		GUIItem item = GUIItem.Create(this.SellItemResource, this.AttachSellItem.transform, 0);
		if (item == null)
		{
			Debug.LogWarning("GameItem is null!!");
		}
		this.SellItem = item;
	}

	private void Constrcut()
	{
		var model = new Model();
		model.SoldPriceFormat = this.SoldPriceFormat;
		model.HaveMoneyFormat = this.HaveMoneyFormat;

		// ビュー生成
		IView view = null;
		if(this.ViewAttach != null)
		{
			view = this.ViewAttach.GetComponent(typeof(IView)) as IView;
		}

		// コントローラ生成
		var controller = new Controller(model, view, this.SellItem);
		this.Controller = controller;
		this.Controller.OnSell += this.HandelSell;
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
		this.OnSellItemEvent = null;
		this.OnSellItemResponseEvent = null;
	}

	/// <summary>
	/// ステータス情報を更新する
	/// </summary>
	private void SetupStatusInfo(int haveMoney)
	{
		if (this.Controller != null)
		{
			this.Controller.SetupStatusInfo(haveMoney);
		}
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
	public static void Open(SetupParam param)
	{
		if (Instance != null)
		{
			Instance.SetActive(true, false);
			Instance._Setup(param);
		}
	}
	/// <summary>
	/// アクティブ設定
	/// </summary>
	void SetActive(bool isActive, bool isTweenSkip)
	{
		if (this.Controller == null) { return; }
		this.Controller.SetActive(isActive, isTweenSkip);
	}
	#endregion

	#region セットアップ
	/// <summary>
	/// データ初期化
	/// </summary>
	public static void Setup(SetupParam param)
	{
		if (Instance == null) { return; }
		Instance._Setup(param);
	}
	public void _Setup(SetupParam param)
	{
		if (this.Controller == null) { return; }
		this.Controller.Setup(param);

		// ステータス情報を更新する
		var info = this.PlayerStatusInfo;
		this.SetupStatusInfo(info.GameMoney);
	}
	#endregion

	#region 通信系
	#region SellPlayerItem パケット
	/// <summary>
	/// 売却イベントハンドラー
	/// </summary>
	private void HandelSell(object sender, SellItemEventArgs e)
	{
		LobbyPacket.SendSellPlayerItem(e.ItemMasterId, e.Stack, this.Response);

		// 通信中表示
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX305_Network_Communication));

		// 通知
		this.OnSellItemEvent();
	}
	/// <summary>
	/// SellPlayerItemReq パケットのレスポンス
	/// </summary>
	private void Response(LobbyPacket.SellPlayerItemResArgs e)
	{
		// 通信中閉じる
		GUISystemMessage.Close();

		if(!e.Result)
		{
			// 売却失敗

			// 警告ログ
			string msg = string.Format("SellPlayerItemResponse Result={0}, ItemMasterId={1}, Stack={2}, money={3}, soldPrice={4}",
				e.Result, e.ItemMasterId, e.Stack, e.Money, e.SoldPrice);
			Debug.LogWarning(msg);
			BugReportController.SaveLogFileWithOutStackTrace(msg);

			// 売却失敗時は画面を閉じる
			GUIController.SingleClose();
		}

		if (this.Controller != null)
		{
			this.Controller.SellResult(e.Result, e.ItemMasterId, e.Stack, e.Money, e.SoldPrice);
		}

		// 通知
		this.OnSellItemResponseEvent();
	}

	/// <summary>
	/// 売却イベントを登録する
	/// </summary>
	public static void AddSellItemEvent(Action sellItemEvent)
	{
		if (Instance == null) { return; }
		Instance.OnSellItemEvent += sellItemEvent;
	}
	/// <summary>
	/// 売却イベントを削除する
	/// </summary>
	public static void RemoveSellItemEvent(Action sellItemEvent)
	{
		if (Instance == null) { return; }
		Instance.OnSellItemEvent -= sellItemEvent;
	}

	/// <summary>
	/// SellPlayerItemパケット受信イベントを登録する
	/// </summary>
	public static void AddSellItemResponseEvent(Action sellItemResponseEvent)
	{
		if (Instance == null) { return; }
		Instance.OnSellItemResponseEvent += sellItemResponseEvent;
	}
	/// <summary>
	/// SellPlayerItemパケット受信イベントを削除する
	/// </summary>
	public static void RemoveSellItemResponseEvent(Action sellItemResponseEvent)
	{
		if (Instance == null) { return; }
		Instance.OnSellItemResponseEvent -= sellItemResponseEvent;
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
			this.AddEvent(this.Dummy);
			this.AddEvent(this.SellItemCount);
			this.AddEvent(this.Money);
		}

		[SerializeField]
		private DummyEvent _dummy = new DummyEvent();
		public DummyEvent Dummy { get { return _dummy; } }
		[System.Serializable]
		public class DummyEvent : IDebugParamEvent
		{
			public event System.Action<ItemInfo, int> Execute = delegate { };
			[SerializeField]
			bool execute = false;

			public void Update()
			{
				if(this.execute)
				{
					this.execute = false;

					var info = new ItemInfo();
					info.DebugRandomSetup();
					info.DebugSetIndex(1);

					int haveItemCount = UnityEngine.Random.Range(info.Stack, 1000);

					this.Execute(info, haveItemCount);
				}
			}
		}

		[SerializeField]
		private SellItemCountEvent _sellItemCount = new SellItemCountEvent();
		public SellItemCountEvent SellItemCount { get { return _sellItemCount; } }
		[System.Serializable]
		public class SellItemCountEvent : IDebugParamEvent
		{
			public event System.Action<ItemInfo, int> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			int sellCount = 0;

			public void Update()
			{
				if (this.execute)
				{
					this.execute = false;

					var info = new ItemInfo();
					info.DebugRandomSetup();
					info.DebugSetIndex(1);
					info.DebugSetStack(UnityEngine.Mathf.Min(info.Stack, this.sellCount));

					this.Execute(info, this.sellCount);
				}
			}
		}

		[SerializeField]
		private MoneyEvent _money = new MoneyEvent();
		public MoneyEvent Money { get { return _money; } }
		[System.Serializable]
		public class MoneyEvent : IDebugParamEvent
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
	}
	void DebugInit()
	{
		var d = this.DebugParam;

		d.ExecuteClose += Close;
		d.ExecuteActive += () =>
		{
			d.ReadMasterData();
			Open(new SetupParam());
		};

		d.Dummy.Execute += (itemInfo, haveItemCount) =>
		{
			var param = new SetupParam();
			param.ItemInfo = itemInfo;
			param.HaveItemCount = haveItemCount;
			this._Setup(param);
		};

		d.SellItemCount.Execute += (itemInfo, sellCount) =>
		{
			var param = new SetupParam();
			param.ItemInfo = itemInfo;
			param.HaveItemCount = sellCount;
			this._Setup(param);
		};

		d.Money.Execute += (money) =>
		{
			this.SetupStatusInfo(money);
		};
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
