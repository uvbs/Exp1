/// <summary>
/// アイテムページ付リスト
/// 
/// 2016/03/23
/// </summary>
using UnityEngine;
using System;
using System.Collections.Generic;

public class GUIItemPageList : MonoBehaviour
{
	#region フィールド&プロパティ
	/// <summary>
	/// ページビュー
	/// </summary>
	[SerializeField]
	private XUI.ItemList.ItemPageListView _pageViewAttach = null;
	private XUI.ItemList.ItemPageListView PageViewAttach { get { return _pageViewAttach; } }

	/// <summary>
	/// アイテムページ付スクロールビュー
	/// </summary>
	[SerializeField]
	private XUI.ItemList.GUIItemScrollView _itemScrollView = null;
	private XUI.ItemList.GUIItemScrollView ItemScrollView { get { return _itemScrollView; } }

	/// <summary>
	/// 容量表示フォーマット
	/// </summary>
	[SerializeField]
	private string _countFormat = "{0] ";
	private string CountFormat { get { return _countFormat; } }

	/// <summary>
	/// 最大容量表示フォーマット
	/// </summary>
	[SerializeField]
	private string _capacityFormat = "/ {0}";
	private string CapacityFormat { get { return _capacityFormat; } }

	/// <summary>
	/// モデル
	/// </summary>
	private XUI.ItemList.IModel Model { get; set; }

	/// <summary>
	/// ビュー
	/// </summary>
	private XUI.ItemList.IView View { get; set; }

	/// <summary>
	/// ページビュー
	/// </summary>
	private XUI.ItemList.IItemPageListView PageView { get; set; }

	/// <summary>
	/// ページリストコントローラ
	/// </summary>
	private XUI.ItemList.IPageListController Controller { get; set; }

	/// <summary>
	/// 登録されているアイテムが押された時の通知用
	/// </summary>
	public event Action<GUIItem> OnItemClickEvent = (item) => { };

	/// <summary>
	/// ページリストのアイテムに変更があった時の通知用
	/// </summary>
	public event Action<GUIItem> OnItemChangeEvent = (item) => { };

	/// <summary>
	/// 全てのアイテムが更新された時のイベント通知
	/// </summary>
	public event Action OnUpdateItemsEvent = () => { };

	/// <summary>
	/// Box枠追加ボタンが押された時のイベント通知用
	/// </summary>
	public event Action OnAddCapacityClickEvent = () => { };
	#endregion

	#region 初期化
	void Awake()
	{
		this.Construct();
	}

	private void Construct()
	{
		this.MemberInit();

		// モデル生成
		this.Model = new XUI.ItemList.Model(this.ItemScrollView);
		this.Model.CountFormat = this.CountFormat;
		this.Model.CapacityFormat = this.CapacityFormat;
		
		// ビュー生成
		XUI.ItemList.IItemPageListView pageView = null;
		XUI.ItemList.IView view = null;
		if(this.PageViewAttach != null)
		{
			pageView = this.PageViewAttach.GetComponent(typeof(XUI.ItemList.IItemPageListView)) as XUI.ItemList.IItemPageListView;
			view = this.PageViewAttach.GetComponent(typeof(XUI.ItemList.IView)) as XUI.ItemList.IView;
		}
		this.PageView = pageView;
		this.View = view;

		// コントローラ
		var controller = new XUI.ItemList.PageListController(this.Model, this.View, this.PageView);
		this.Controller = controller;
		this.Controller.OnItemClickEvent += this.HandleItemClick;
		this.Controller.OnItemChangeEvent += this.HandleItemChange;
		this.Controller.OnUpdateItemsEvent += this.HandleUpdateItems;
		this.Controller.OnAddCapacityClickEvent += this.HandleAddCapacityClick;
	}

	/// <summary>
	/// シリアライズされていないメンバー初期化
	/// </summary>
	private void MemberInit()
	{
		this.Model = null;
		this.PageView = null;
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

		this.OnItemClickEvent = null;
		this.OnItemChangeEvent = null;
		this.OnUpdateItemsEvent = null;
		this.OnAddCapacityClickEvent = null;
	}
	#endregion

	#region アクティブ設定
	/// <summary>
	/// アクティブを設定する
	/// </summary>
	public void SetActive(bool isActive)
	{
		if (this.View == null) { return; }
		this.View.SetActive(isActive);
	}
	#endregion

	#region セットアップ
	/// <summary>
	/// データ初期化
	/// </summary>
	public void Setup()
	{
		if (this.Controller == null) { return; }
		this.Controller.Setup();
	}
	#endregion

	#region Box総数設定
	/// <summary>
	/// Box総数を設定する
	/// </summary>
	public void SetupCapacity(int capacity, int itemCount)
	{
		if (this.Controller == null) { return; }
		this.Controller.SetCapacity(capacity, itemCount);
	}
	#endregion

	#region リストにアイテムをセット
	/// <summary>
	/// リストにアイテムを設定する
	/// </summary>
	public void SetupItems(List<ItemInfo> itemInfoList)
	{
		if (this.Controller == null) { return; }
		var infoList = (itemInfoList != null) ? itemInfoList : new List<ItemInfo>();
		this.Controller.SetupItems(infoList);
	}
	#endregion

	#region アイテム情報取得
	/// <summary>
	/// 現ページ内のアイテムリストを取得する
	/// </summary>
	public List<GUIItem> GetNowPageItemList()
	{
		if (this.Model == null) { return new List<GUIItem>(); }
		return this.Model.GetNowPageItemList();
	}

	/// <summary>
	/// 登録されているアイテム情報取得
	/// </summary>
	public List<ItemInfo> GetItemInfoList()
	{
		if (this.Model == null) { return new List<ItemInfo>(); }

		var infoList = new List<ItemInfo>();
		foreach(var info in this.Model.ItemInfoList)
		{
			if (info == null) { continue; }
			infoList.Add(info.Clone());
		}
		return infoList;
	}
	#endregion

	#region ページ
	/// <summary>
	/// 1ページ目にセットする
	/// </summary>
	public void BackEndPage()
	{
		if (this.Controller == null) { return; }
		this.Controller.BackEndPage();
	}
	#endregion

	#region アイテムイベント
	/// <summary>
	/// 登録されているアイテムが押された時に呼び出される
	/// </summary>
	private void HandleItemClick(object sender, XUI.ItemList.ItemClickEventArgs e)
	{
		// 通知
		this.OnItemClickEvent(e.Item);
	}

	/// <summary>
	/// ページリストのアイテムに変更があった時に呼び出される
	/// </summary>
	private void HandleItemChange(object sender, XUI.ItemList.ItemChangeEventArgs e)
	{
		// 通知
		this.OnItemChangeEvent(e.Item);
	}
	
	/// <summary>
	/// 全てのアイテムが変更された時に呼び出される
	/// </summary>
	private	void HandleUpdateItems(object sender, EventArgs e)
	{
		// 通知
		this.OnUpdateItemsEvent();
	}
	#endregion

	#region Box枠追加
	/// <summary>
	/// Box枠追加ボタンが押された時のハンドラー
	/// </summary>
	private void HandleAddCapacityClick(object sender, EventArgs e)
	{
		// 通知
		this.OnAddCapacityClickEvent();
	}
	#endregion

	#region 有効無効
	void OnEnable()
	{
		if (this.Controller == null) { return; }
		// イベント登録
		GUIItemSort.AddOKClickEvent(this.Controller.HandleOKClickEvent);
		GUIItemSort.AddSortPatternChangeEvent(this.Controller.HandleSortPatternChangeEvent);
		// 同期
		this.Controller.SyncItemSort();
	}
	void OnDisable()
	{
		if (this.Controller == null) { return; }
		GUIItemSort.RemoveOKClickEvent(this.Controller.HandleOKClickEvent);
		GUIItemSort.RemoveSortPatternChangeEvent(this.Controller.HandleSortPatternChangeEvent);
	}
	#endregion

	#region NEWフラグ一括削除
	/// <summary>
	/// NEWフラグの一括解除を行う
	/// </summary>
	public void DeleteAllNewFlag()
	{
		// Newフラグ一括解除パケット送信
		LobbyPacket.SendSetPlayerItemNewFlagAll(null);
	}
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
			AddEvent(this.Dummy);
		}

		[SerializeField]
		DummyEvent _dummy = new DummyEvent();
		public DummyEvent Dummy { get { return _dummy; } set { _dummy = value; } }
		[System.Serializable]
		public class DummyEvent : IDebugParamEvent
		{
			public event System.Action<List<ItemInfo>, int, int> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			private List<ItemInfo> itemInfoList = new List<ItemInfo>();

			public void Update()
			{
				if (this.execute)
				{
					this.execute = false;
					int capacity = UnityEngine.Random.Range(0, 999);
					int createMax = UnityEngine.Random.Range(0, 999);
					CreateData(createMax);

					Execute(this.itemInfoList, capacity, createMax);
				}
			}
			void CreateData(int createMax)
			{
				for (int count = 1; count <= createMax; ++count)
				{
					var info = new ItemInfo();
					var index = count;
					info.DebugRandomSetup();
					info.DebugSetIndex(index);
					this.itemInfoList.Add(info);
				}
			}
		}
	}

	void DebugInit()
	{
		var d = this.DebugParam;

		d.ExecuteActive += () => { d.ReadMasterData(); SetActive(true); };
		d.ExecuteClose += () => { SetActive(false); };

		d.Dummy.Execute += (itemInfoList, capacity, createMax) =>
		{
			SetupCapacity(capacity, createMax);
			SetupItems(itemInfoList);
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
