/// <summary>
/// キャラページ付リスト
/// 
/// 2016/01/12
/// </summary>
using UnityEngine;
using System;
using System.Collections.Generic;

public class GUICharaPageList : MonoBehaviour
{
	#region フィールド&プロパティ
	/// <summary>
	/// ページビュー
	/// </summary>
	[SerializeField]
	XUI.CharaList.CharaPageListView _pageViewAttach = null;
	XUI.CharaList.CharaPageListView PageViewAttach { get { return _pageViewAttach; } }

	/// <summary>
	/// キャラアイテムページ付スクロールビュー
	/// </summary>
	[SerializeField]
	private XUI.CharaList.GUIItemScrollView _itemScrollView = null;
	public XUI.CharaList.GUIItemScrollView ItemScrollView { get { return _itemScrollView; } }

	/// <summary>
	/// 容量表示フォーマット
	/// </summary>
	[SerializeField]
	string _countFormat = "{0} ";
	string CountFormat { get { return _countFormat; } }

	/// <summary>
	/// 最大容量表示フォーマット
	/// </summary>
	[SerializeField]
	private string _capacityFormat = "/ {0}";
	private string CapacityFormat { get { return _capacityFormat; } }

	/// <summary>
	/// モデル
	/// </summary>
	XUI.CharaList.IModel Model { get; set; }

	/// <summary>
	/// ビュー
	/// </summary>
	XUI.CharaList.IView View { get; set; }

	/// <summary>
	/// ページビュー
	/// </summary>
	XUI.CharaList.ICharaPageListView PageView { get; set; }

	/// <summary>
	/// ページリストコントローラ
	/// </summary>
	XUI.CharaList.IPageListController Controller { get; set; }

	/// <summary>
	/// 登録されているアイテムが押された時の通知用
	/// </summary>
	public event Action<GUICharaItem> OnItemClickEvent = (item) => { };

	/// <summary>
	/// 登録されているアイテムが長押しされた時の通知用
	/// </summary>
	public event Action<GUICharaItem> OnItemLongPressEvent = (item) => { };

	/// <summary>
	/// ページリストのアイテムに変更があった時の通知用
	/// </summary>
	public event Action<GUICharaItem> OnItemChangeEvent = (item) => { };

	/// <summary>
	/// 所持数追加ボタンが押された時のイベント通知用
	/// </summary>
	public event Action OnAddCapacityClickEvent = () => { };

	/// <summary>
	/// 全てのアイテムが更新された時のイベント通知
	/// </summary>
	public event Action OnUpdateItemsEvent = () => { };
	#endregion

	#region 初期化
	void Awake()
	{
		Construct();
	}

	void Start()
	{
		this.Setup();
	}

	private void Construct()
	{
		MemberInit();

		// モデル生成
		this.Model = new XUI.CharaList.Model(this.ItemScrollView);
		this.Model.CountFormat = this.CountFormat;
		this.Model.CapacityFormat = this.CapacityFormat;

		// ビュー生成
		XUI.CharaList.ICharaPageListView pageView = null;
		XUI.CharaList.IView view = null;
		if (this.PageViewAttach != null)
		{
			pageView = this.PageViewAttach.GetComponent(typeof(XUI.CharaList.ICharaPageListView)) as XUI.CharaList.ICharaPageListView;
			view = this.PageViewAttach.GetComponent(typeof(XUI.CharaList.IView)) as XUI.CharaList.IView;
		}
		this.PageView = pageView;
		this.View = view;

		// コントローラ
		var controller = new XUI.CharaList.PageListController(this.Model, this.View, this.PageView);
		this.Controller = controller;
		this.Controller.OnItemClickEvent += OnItemClick;
		this.Controller.OnItemLongPressEvent += OnItemLongPress;
		this.Controller.OnItemChangeEvent += OnItemChange;
		this.Controller.OnAddCapacityClickEvent += OnAddCapacityClick;
		this.Controller.OnUpdateItemsEvent += OnUpdateItems;
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
	#endregion

	#region アクティブ設定
	/// <summary>
	/// アクティブを設定する
	/// </summary>
	/// <param name="isActive"></param>
	public void SetActive(bool isActive)
	{
		if (this.View == null) { return; }
		this.View.SetActive(isActive);
	}
	#endregion

	#region セットアップ
	/// <summary>
	/// データを初期化する
	/// </summary>
	public void Setup()
	{
		if (this.Controller != null)
		{
			// データ初期化
			this.Controller.Setup();
		}
	}
	#endregion

	#region アイテム総数設定
	/// <summary>
	/// アイテムの総数を設定する
	/// </summary>
	public void SetupCapacity(int capacity, int itemCount)
	{
		if (this.Model == null) { return; }

		// オーバー分も含む総数セット
		//int total = (itemCount > capacity) ? itemCount : capacity;
		// 売り切り版仕様 キャラ追加分のみ枠を表示
		int total = itemCount;
		this.Model.TotalCapacity = total;

		this.Model.Capacity = capacity;
	}

	/// <summary>
	/// アイテム総数を取得
	/// </summary>
	public int GetCapacity()
	{
		if (this.Model == null) { return 0; }
		return this.Model.Capacity;
	}
	#endregion

	#region キャラアイテムセット
	/// <summary>
	/// 所有キャラをアイテム化してスクロールビューにセットする
	/// </summary>
	/// <param name="charaList"></param>
	/// <param name="mainOwnCharaId"></param>
	public void SetupItems(List<CharaInfo> charaList)
	{
		if (this.Controller == null) { return; }
		var list = (charaList != null) ? charaList : new List<CharaInfo>();
		this.Controller.SetupItems(list);
	}
	#endregion

	#region キャラアイテム更新
	/// <summary>
	/// 現ページ内のアイテム更新
	/// </summary>
	public void UpdateItems()
	{
		if (this.Controller == null) { return; }
		this.Controller.UpdateItems();
	}
	#endregion

	#region キャラ情報取得
	/// <summary>
	/// 現ページ内のキャラアイテムリストを返す
	/// </summary>
	public List<GUICharaItem> GetNowPageItemList()
	{
		if(this.Model == null)
		{
			return new List<GUICharaItem>();
		}
		return this.Model.GetNowPageItemList();
	}

	/// <summary>
	/// 全登録されているキャラ情報取得
	/// </summary>
	/// <returns></returns>
	public List<CharaInfo> GetCharaInfo()
	{
		if(this.Model == null)
		{
			return new List<CharaInfo>();
		}

		var infoList = new List<CharaInfo>();
		foreach(var info in this.Model.CharaInfoList)
		{
			if (info == null) { continue; }
			infoList.Add(info.Clone());
		}
		return infoList;
	}
	#endregion

	#region 表示キャラ情報
	/// <summary>
	/// 表示キャラ情報リストをセットする
	/// </summary>
	public void SetViewCharaInfoList(List<CharaInfo> charaInfoList)
	{
		if (this.Controller == null) { return; }
		this.Controller.SetViewCharaInfoList(charaInfoList);
	}
	#endregion

	#region アイテムイベント
	/// <summary>
	/// 登録されているアイテムが押された時に呼び出される
	/// </summary>
	/// <param name="item"></param>
	private void OnItemClick(object sender, XUI.CharaList.ItemClickEventArgs e)
	{
		// 通知
		this.OnItemClickEvent(e.Item);
	}

	/// <summary>
	/// 登録されているアイテムが長押しされた時に呼び出される
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void OnItemLongPress(object sender, XUI.CharaList.ItemClickEventArgs e)
	{
		// 通知
		this.OnItemLongPressEvent(e.Item);
	}

	/// <summary>
	/// ページリストのアイテムに変更があった時に呼び出される
	/// </summary>
	private void OnItemChange(object sender, XUI.CharaList.ItemChangeEventArgs e)
	{
		// 通知
		this.OnItemChangeEvent(e.Item);
	}

	/// <summary>
	/// 全てのアイテムが更新された時に呼び出される
	/// </summary>
	private void OnUpdateItems(object sender, EventArgs e)
	{
		// 通知
		this.OnUpdateItemsEvent();
	}
	#endregion

	#region 所持数追加
	/// <summary>
	/// 所持数追加ボタンが押された時に呼び出される
	/// </summary>
	private void OnAddCapacityClick(object sender, EventArgs e)
	{
		// 通知
		this.OnAddCapacityClickEvent();
	}

	/// <summary>
	/// 所持数追加の有効設定
	/// </summary>
	public void SetAddCapacityEnable(bool isEnable)
	{
		if (this.Controller == null) { return; }
		this.Controller.SetAddCapacityEnable(isEnable);
	}
	#endregion

	#region 有効無効
	void OnEnable()
	{
		if (this.Controller == null) { return; }
		// イベント登録
		GUICharaSort.AddOKClickEvent(this.Controller.HandleOKClickEvent);
		GUICharaSort.AddSortPatternChangeEvent(this.Controller.HandleSortPatternChangeEvent);
		// 同期
		this.Controller.SyncCharaSort();
	}
	void OnDisable()
	{
		if (this.Controller == null) { return; }
		GUICharaSort.RemoveOKClickEvent(this.Controller.HandleOKClickEvent);
		GUICharaSort.RemoveSortPatternChangeEvent(this.Controller.HandleSortPatternChangeEvent);
	}
	#endregion

	#region ページ
	/// <summary>
	/// 最初のページに設定
	/// </summary>
	public void BackEndPage()
	{
		if (this.Controller == null) { return; }
		this.Controller.BackEndPage();
	}
	#endregion

	#region NEWフラグ一括削除
	/// <summary>
	/// NEWフラグの一括解除を行う
	/// </summary>
	public void DeleteAllNewFlag()
	{
		// Newフラグ一括解除パケット送信
		LobbyPacket.SendSetPlayerCharacterNewFlagAll(null);
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
			public event System.Action<List<CharaInfo>, int> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			private List<CharaInfo> charaInfoList = new List<CharaInfo>();

			public void Update()
			{
				if (this.execute)
				{
					MasterData.Read();
					this.execute = false;
					var capacity = UnityEngine.Random.Range(0, 999);
					var createMax = UnityEngine.Random.Range(0, capacity);
					CreateData(capacity, createMax);

					Execute(this.charaInfoList, capacity);
				}
			}
			void CreateData(int capacity, int createMax)
			{
				int maxCharaID = 9999;

				List<Scm.Common.Master.CharaMasterData> dataList = new List<Scm.Common.Master.CharaMasterData>();
				for (int i = 0; i <= maxCharaID; i++)
				{
					Scm.Common.Master.CharaMasterData data;
					if (MasterData.TryGetChara(i, out data))
					{
						dataList.Add(data);
					}
				}

				this.charaInfoList.Clear();
				for (int i = 0; i < createMax; i++)
				{
					var info = new CharaInfo();
					var uuid = (ulong)i + 1;
					info.DebugRandomSetup();
					info.DebugSetUUID(uuid);
					this.charaInfoList.Add(info);
				}
			}
		}
	}

	void DebugInit()
	{
		var d = this.DebugParam;

		d.ExecuteActive += () => { SetActive(true); };
		d.ExecuteClose += () => { SetActive(false); };

		d.Dummy.Execute += (charaInfoList, capacity) =>
		{
			SetupCapacity(capacity, capacity);
			SetupItems(charaInfoList);
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
