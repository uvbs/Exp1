/// <summary>
/// アイテム選択リスト
/// 
/// 2016/03/24
/// </summary>
using UnityEngine;
using System;
using System.Collections.Generic;

public class GUISelectItemList : MonoBehaviour
{
	#region フィールド&プロパティ
	/// <summary>
	/// 選択アイテムリストビューアタッチ
	/// </summary>
	[SerializeField]
	private XUI.ItemList.ItemListView _itemListViewAttach = null;
	private XUI.ItemList.ItemListView ItemListViewAttach { get { return _itemListViewAttach; } }

	/// <summary>
	/// アイテムページ付スクロールビュー
	/// </summary>
	[SerializeField]
	private XUI.ItemList.GUIItemScrollView _itemScrollView = null;
	public XUI.ItemList.GUIItemScrollView ItemScrollView { get { return _itemScrollView; } }

	/// <summary>
	/// モデル
	/// </summary>
	private XUI.ItemList.IModel Model { get; set; }

	/// <summary>
	/// ビュー
	/// </summary>
	private XUI.ItemList.IView View { get; set; }

	/// <summary>
	/// アイテム選択制御
	/// </summary>
	private XUI.ItemList.ISelectListController Controller { get; set; }

	/// <summary>
	/// 登録されているアイテムが押された時の通知用
	/// </summary>
	public Action<GUIItem> OnItemClickEvent = (item) => { };

	/// <summary>
	/// 登録されているアイテムに変更があった時の通知用
	/// </summary>
	public Action<GUIItem> OnItemChangeEvent = (item) => { };

	/// <summary>
	/// 全てのアイテムが更新された時のイベント通知
	/// </summary>
	public event Action OnUpdateItemsEvent = () => { };
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

		// ビュー生成
		XUI.ItemList.IView view = null;
		if(this.ItemListViewAttach != null)
		{
			view = this.ItemListViewAttach.GetComponent(typeof(XUI.ItemList.IView)) as XUI.ItemList.IView;
		}
		this.View = view;

		// コントローラ
		var controller = new XUI.ItemList.SelectListController(this.Model, this.View);
		this.Controller = controller;
		this.Controller.OnItemClickEvent += this.HandleItemClick;
		this.Controller.OnItemChangeEvent += this.HandleItemChange;
		this.Controller.OnUpdateItemsEvent += this.HandleUpdateItems;
	}

	/// <summary>
	/// シリアライズされていないメンバー初期化
	/// </summary>
	private void MemberInit()
	{
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

		this.OnItemClickEvent = null;
		this.OnItemChangeEvent = null;
		this.OnUpdateItemsEvent = null;
	}
	#endregion

	#region アクティブ設定
	/// <summary>
	/// アクティブを設定する
	/// </summary>
	/// <param name="isActive"></param>
	public void SetActive(bool isActive)
	{
		this.View.SetActive(isActive);
	}
	#endregion

	#region アイテム総数設定
	/// <summary>
	/// アイテムの総数を設定する
	/// </summary>
	public void SetupCapacity(int capacity)
	{
		if (this.Controller == null) { return; }
		this.Controller.SetCapacity(capacity);
	}
	#endregion

	#region アイテム情報リスト
	/// <summary>
	/// 現ページ内のアイテムリストを取得する
	/// </summary>
	public List<GUIItem> GetNowPageList()
	{
		if (this.Controller == null) { return new List<GUIItem>(); }
		return this.Controller.GetNowPageList();
	}

	/// <summary>
	/// 追加されているアイテムリストを取得する
	/// </summary>
	public List<GUIItem> GetItemList()
	{
		if (this.Controller == null) { return new List<GUIItem>(); }
		return this.Controller.GetItemList();
	}

	/// <summary>
	/// 追加されているアイテム情報リストを取得する
	/// </summary>
	public List<ItemInfo> GetItemInfoList()
	{
		if (this.Controller == null) { return new List<ItemInfo>(); }
		return this.Controller.GetItemInfoList();
	}
	#endregion

	#region アイテム情報リスト追加
	/// <summary>
	/// アイテム情報追加
	/// </summary>
	public bool AddItem(ItemInfo itemInfo)
	{
		if (this.Controller == null) { return false; }
		return this.Controller.AddItem(itemInfo);
	}
	#endregion

	#region アイテム情報リスト削除
	/// <summary>
	/// アイテム情報削除
	/// </summary>
	public bool RemoveItem(ItemInfo itemInfo)
	{
		if (this.Controller == null) { return false; }
		return this.Controller.RemoveItem(itemInfo);
	}
	#endregion

	#region アイテム情報リストクリア
	/// <summary>
	/// アイテム情報をクリアする
	/// </summary>
	public bool ClearItem()
	{
		if (this.Controller == null) { return false; }
		return this.Controller.ClearItem();
	}
	#endregion

	#region アイテムイベント
	/// <summary>
	/// アイテムが押された時のハンドラー
	/// </summary>
	private void HandleItemClick(object sender, XUI.ItemList.SelectListController.ItemClickEventArgs e)
	{
		// 通知
		this.OnItemClickEvent(e.Item);
	}

	/// <summary>
	/// アイテムに変更があった時のハンドラー
	/// </summary>
	private void HandleItemChange(object sender, XUI.ItemList.SelectListController.ItemChangeEventArgs e)
	{
		// 通知
		this.OnItemChangeEvent(e.Item);
	}

	/// <summary>
	/// 全てのアイテムが更新された時のハンドラー
	/// </summary>
	private void HandleUpdateItems(object sender, EventArgs e)
	{
		// 通知
		this.OnUpdateItemsEvent();
	}
	#endregion

	#region スクロール
	/// <summary>
	/// スクロール更新
	/// </summary>
	public void UpdateScroll()
	{
		if (this.Controller == null) { return; }
		this.Controller.UpdateScroll();
	}

	/// <summary>
	/// テーブル整形
	/// </summary>
	public void Reposition()
	{
		if (this.Controller == null) { return; }
		this.Controller.Reposition();

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
			AddEvent(this.SetCapacity);
			AddEvent(this.AddItem);
			AddEvent(this.TouchRemoveItem);
		}

		[SerializeField]
		SetCapacityEvent _setCapacity = new SetCapacityEvent();
		public SetCapacityEvent SetCapacity { get { return _setCapacity; } set { _setCapacity = value; } }
		[System.Serializable]
		public class SetCapacityEvent : IDebugParamEvent
		{
			public event System.Action<int> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			int capacity = 1;

			public void Update()
			{
				if (this.execute)
				{
					this.execute = false;
					Execute(this.capacity);
				}
			}
		}

		[SerializeField]
		AddItemEvent _addItem = new AddItemEvent();
		public AddItemEvent AddItem { get { return _addItem; } set { _addItem = value; } }
		[System.Serializable]
		public class AddItemEvent : IDebugParamEvent
		{
			public event System.Action<ItemInfo> Execute = delegate { };
			[SerializeField]
			bool execute = false;

			public void Update()
			{
				if (this.execute)
				{
					this.execute = false;
					var info = new ItemInfo();
					info.DebugRandomSetup();
					info.DebugSetIndex(Time.frameCount);
					Execute(info);
				}
			}
		}

		[SerializeField]
		TouchRemoveItemEvent _touchRemoveItem = new TouchRemoveItemEvent();
		public TouchRemoveItemEvent TouchRemoveItem { get { return _touchRemoveItem; } set { _touchRemoveItem = value; } }
		[System.Serializable]
		public class TouchRemoveItemEvent : IDebugParamEvent
		{
			public event System.Action<bool> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			bool isTouchRemove = false;
			public Func<ItemInfo, bool> RemoveFunc = (info) => { return false; };

			public void Update()
			{
				if (this.execute)
				{
					execute = false;
					Execute(isTouchRemove);
				}
			}

			public void OnItemClick(GUIItem item)
			{
				RemoveFunc(item.GetItemInfo());
			}
		}
	}

	void DebugInit()
	{
		var d = this.DebugParam;

		d.ExecuteActive += () => { SetActive(true); };
		d.ExecuteClose += () => { SetActive(false); };

		d.SetCapacity.Execute += (capacity) =>
		{
			SetupCapacity(capacity);
		};

		d.AddItem.Execute += (itemInfo) =>
		{
			d.ReadMasterData();
			AddItem(itemInfo);
		};

		d.TouchRemoveItem.Execute += (isTouchRemove) =>
		{
			if (isTouchRemove)
			{
				OnItemClickEvent = d.TouchRemoveItem.OnItemClick;
				d.TouchRemoveItem.RemoveFunc = RemoveItem;
			}
			else
			{
				OnItemClickEvent = (info) => { };
				d.TouchRemoveItem.RemoveFunc = (info) => { return false; };
			}
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
