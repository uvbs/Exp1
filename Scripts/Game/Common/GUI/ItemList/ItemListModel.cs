/// <summary>
/// アイテムリストデータ
/// 
/// 2016/03/22
/// </summary>
using UnityEngine;
using System;
using System.Collections.Generic;

namespace XUI.ItemList
{
	/// <summary>
	/// アイテムリストデータインターフェイス
	/// </summary>
	public interface IModel
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();
		#endregion

		#region ページ付スクロールビュー
		/// <summary>
		/// アイテムページ付スクロールビュー
		/// </summary>
		GUIItemScrollView ItemScrollView { get; }

		/// <summary>
		/// アイテムページリストのアイテム一覧を取得
		/// </summary>
		List<GUIItem> GetItemList();

		/// <summary>
		/// 現ページ内のアイテムリストを返す
		/// </summary>
		/// <returns></returns>
		List<GUIItem> GetNowPageItemList();
		#endregion

		#region アイテム情報リスト
		/// <summary>
		/// アイテム情報リスト
		/// </summary>
		List<ItemInfo> ItemInfoList { get; }

		/// <summary>
		/// アイテム情報リストにセットする
		/// </summary>
		/// <param name="info"></param>
		event EventHandler OnSetItemInfoListChange;
		void SetItemInfoList(List<ItemInfo> charaInfoList);

		/// <summary>
		/// アイテム情報リスト全削除
		/// </summary>
		event EventHandler OnItemInfoListClear;
		void ClearItemInfoList();

		/// <summary>
		/// アイテム情報リスト追加
		/// </summary>
		event EventHandler OnAddItemInfoList;
		void AddItemInfoList(ItemInfo itemInfo);

		/// <summary>
		/// アイテム情報リスト削除
		/// </summary>
		event EventHandler OnRemoveItemInfoList;
		bool RemoveItemInfoList(ItemInfo itemInfo);

		/// <summary>
		/// アイテム情報リスト内に存在するか
		/// </summary>
		bool ItemInfoListContains(ItemInfo itemInfo);

		/// <summary>
		/// アイテム情報リスト内に存在しているかチェックしその情報を返す
		/// </summary>
		ItemInfo ItemInfoListFind(ItemInfo itemInfo);
		#endregion

		#region 表示アイテム情報リスト
		/// <summary>
		/// 表示アイテム情報リスト
		/// </summary>
		List<ItemInfo> ViewItemInfoList { get; }

		/// <summary>
		/// 表示アイテム情報リストにセットする
		/// </summary>
		/// <param name="info"></param>
		event EventHandler OnViewItemInfoListChange;
		void SetViewItemInfoList(List<ItemInfo> viewItemInfoList);
		#endregion

		#region Box数
		/// <summary>
		/// Box総数
		/// </summary>
		event EventHandler OnCapacityChange;
		int Capacity { get; set; }

		/// <summary>
		/// Box総数がオーバーした分も含む合計数
		/// </summary>
		event EventHandler OnTotalCapacityChange;
		int TotalCapacity { get; set; }
		#endregion

		#region 容量
		/// <summary>
		/// 容量表示形式
		/// </summary>
		event EventHandler OnCountFormatChange;
		string CountFormat { get; set; }

		/// <summary>
		/// 最大容量表示形式
		/// </summary>
		event EventHandler OnCapacityFormatChange;
		string CapacityFormat { get; set; }
		#endregion
	}

	/// <summary>
	/// アイテムリストデータ
	/// </summary>
	public class Model : IModel
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			this.OnSetItemInfoListChange = null;
			this.OnItemInfoListClear = null;
			this.OnAddItemInfoList = null;
			this.OnRemoveItemInfoList = null;
			this.OnViewItemInfoListChange = null;
			this.OnCapacityChange = null;
			this.OnTotalCapacityChange = null;
			this.OnCountFormatChange = null;
			this.OnCapacityFormatChange = null;
		}
		#endregion

		#region 初期化
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Model(GUIItemScrollView itemScrollView)
		{
			_itemScrollView = itemScrollView;
		}
		#endregion

		#region ページ付スクロールビュー
		/// <summary>
		/// アイテムページ付スクロールビュー
		/// </summary>
		private readonly GUIItemScrollView _itemScrollView;
		public GUIItemScrollView ItemScrollView { get { return _itemScrollView; } }

		/// <summary>
		/// アイテムページリストのアイテム一覧を取得
		/// </summary>
		public List<GUIItem> GetItemList()
		{
			var itemList = new List<GUIItem>();
			foreach(var item in this.ItemScrollView.ItemList)
			{
				if (item == null) { continue; }
				itemList.Add(item);
			}

			return itemList;
		}

		/// <summary>
		/// 現ページ内のアイテムリストを返す
		/// </summary>
		public List<GUIItem> GetNowPageItemList()
		{
			var itemList = new List<GUIItem>();

			// 現ページの最初のアイテムインデックスを取得
			int startIndex = this.ItemScrollView.NowPageStartIndex;

			// 現在のページ内のアイテムを更新
			for (int i = 0, max = this.ItemScrollView.NowPageItemMax; i < max; i++)
			{
				int indexInTotal = i + startIndex;
				if (!this.ItemScrollView.IsNowPage(indexInTotal))
				{
					// 現在のページ内のアイテムではない
					continue;
				}

				// スクロールビューからアイテムを取得
				int itemIndex = this.ItemScrollView.GetItemIndex(indexInTotal);
				GUIItem item = this.ItemScrollView.GetItem(itemIndex);

				// リストに追加
				itemList.Add(item);
			}

			return itemList;
		}
		#endregion

		#region アイテム情報
		/// <summary>
		/// アイテム情報リスト
		/// </summary>
		private List<ItemInfo> _itemInfoList = new List<ItemInfo>();
		public List<ItemInfo> ItemInfoList
		{
			get { return _itemInfoList; }
		}

		/// <summary>
		/// アイテム情報リストセット
		/// </summary>
		public event EventHandler OnSetItemInfoListChange = (sender, e) => { };
		public void SetItemInfoList(List<ItemInfo> itemInfoList)
		{
			if (this.ItemInfoList == null) { return; }
			this.ItemInfoList.Clear();
			foreach(var info in itemInfoList)
			{
				if(info == null)
				{
					this.ItemInfoList.Add(info);
				}
				else
				{
					this.ItemInfoList.Add(info.Clone());
				}
			}

			// 通知
			this.OnSetItemInfoListChange(this, EventArgs.Empty);
		}

		/// <summary>
		/// アイテム情報リストクリア
		/// </summary>
		public event EventHandler OnItemInfoListClear = (sender, e) => { };
		public void ClearItemInfoList()
		{
			// クリア
			this.ItemInfoList.Clear();
			// 通知
			OnItemInfoListClear(this, EventArgs.Empty);
		}

		/// <summary>
		/// アイテム情報リスト追加
		/// </summary>
		public event EventHandler OnAddItemInfoList = (sender, e) => { };
		public void AddItemInfoList(ItemInfo itemInfo)
		{
			if(itemInfo == null)
			{
				this.ItemInfoList.Add(itemInfo);
			}
			else
			{
				this.ItemInfoList.Add(itemInfo.Clone());
			}

			// 通知
			this.OnAddItemInfoList(this, EventArgs.Empty);
		}

		/// <summary>
		/// アイテム情報リスト削除
		/// </summary>
		public event EventHandler OnRemoveItemInfoList = (sender, e) => { };
		public bool RemoveItemInfoList(ItemInfo itemInfo)
		{
			// 削除
			bool isRemove = this.ItemInfoList.Remove(itemInfo);

			if(isRemove)
			{
				// 通知
				this.OnRemoveItemInfoList(this, EventArgs.Empty);
			}

			return isRemove;
		}

		/// <summary>
		/// アイテム情報リスト内に存在するか
		/// </summary>
		public bool ItemInfoListContains(ItemInfo itemInfo)
		{
			foreach(var info in this.ItemInfoList)
			{
				if (info == null) { continue; }
				if(info.Index == itemInfo.Index)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// アイテム情報リスト内に存在しているかチェックしその情報を返す
		/// </summary>
		public ItemInfo ItemInfoListFind(ItemInfo itemInfo)
		{
			ItemInfo match = null;
			foreach (var info in this.ItemInfoList)
			{
				if (info == null) { continue; }
				if (info.Index == itemInfo.Index)
				{
					match = info;
					break;
				}
			}

			return match;
		}
		#endregion

		#region 表示アイテム情報
		/// <summary>
		/// 表示アイテム情報リスト
		/// </summary>
		private List<ItemInfo> _viewItemInfoList = new List<ItemInfo>();
		public List<ItemInfo> ViewItemInfoList { get { return _viewItemInfoList; } }

		/// <summary>
		/// 表示アイテム情報リストにセットする
		/// </summary>
		public event EventHandler OnViewItemInfoListChange = (sender, e) => { };
		public void SetViewItemInfoList(List<ItemInfo> viewItemInfoList)
		{
			if (this.ViewItemInfoList == null) { return; }
			this.ViewItemInfoList.Clear();
			foreach(var info in viewItemInfoList)
			{
				// リストに登録
				this.ViewItemInfoList.Add(info);
			}

			// 通知
			this.OnViewItemInfoListChange(this, EventArgs.Empty);
		}
		#endregion

		#region Box枠数
		/// <summary>
		/// Boxの総数
		/// </summary>
		public event EventHandler OnCapacityChange = (sender, e) => { };
		private int _capacity = 0;
		public int Capacity
		{
			get { return _capacity; }
			set
			{
				if (_capacity != value)
				{
					// 変更された時のみセット&通知
					_capacity = value;
					OnCapacityChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Box総数がオーバーした分も含む合計数
		/// </summary>
		public event EventHandler OnTotalCapacityChange = (sender, e) => { };
		private int _totalCapacity = 0;
		public int TotalCapacity
		{
			get { return _totalCapacity; }
			set
			{
				if(_totalCapacity != value)
				{
					_totalCapacity = value;

					// 通知
					OnTotalCapacityChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region 容量フォーマット
		/// <summary>
		/// 容量表示形式
		/// </summary>
		public event EventHandler OnCountFormatChange = (sender, e) => { };
		private string _countFormat = "";
		public string CountFormat
		{
			get { return _countFormat; }
			set
			{
				if (_countFormat != value)
				{
					_countFormat = value;

					// 通知
					this.OnCountFormatChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// 最大容量表示形式
		/// </summary>
		public event EventHandler OnCapacityFormatChange = (sender, e) => { };
		private string _capacityFormat = "";
		public string CapacityFormat
		{
			get { return _capacityFormat; }
			set
			{
				if (_capacityFormat != value)
				{
					_capacityFormat = value;

					// 通知
					this.OnCapacityFormatChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion
	}

	/// <summary>
	/// アイテムリストのページ付スクロールビュークラス
	/// </summary>
	[Serializable]
	public class GUIItemScrollView : PageScrollView<GUIItem>
	{
		protected override GUIItem Create(GameObject prefab, Transform parent, int itemIndex)
		{
			var item = GUIItem.Create(prefab, parent, itemIndex);
			item.SetState(Item.ItemStateType.FillEmpty);
			return item;
		}
		protected override void ClearValue(GUIItem item)
		{
			// データ初期化
			item.Setup();
		}
	}
}
