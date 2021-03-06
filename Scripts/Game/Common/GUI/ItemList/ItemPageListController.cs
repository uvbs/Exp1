/// <summary>
/// アイテムページ付リスト制御
/// 
/// 2016/03/22
/// </summary>
using System;
using System.Collections.Generic;

namespace XUI.ItemList
{
	#region イベント引数
	/// <summary>
	/// 登録されているアイテムが押された時のディスパッチ
	/// </summary>
	public class ItemClickEventArgs : EventArgs
	{
		public GUIItem Item { get; private set; }
		public ItemClickEventArgs(GUIItem item)
		{
			this.Item = item;
		}
	}

	/// <summary>
	/// 登録されているアイテムに変更があった時のディスパッチ
	/// </summary>
	public class ItemChangeEventArgs : EventArgs
	{
		public GUIItem Item { get; private set; }
		public ItemChangeEventArgs(GUIItem item)
		{
			this.Item = item;
		}
	}
	#endregion

	/// <summary>
	/// アイテムページ付リスト制御インターフェイス
	/// </summary>
	public interface IPageListController
	{
		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();

		/// <summary>
		/// 更新できる状態かどうか
		/// </summary>
		bool CanUpdate { get; }

		/// <summary>
		/// データ初期化
		/// </summary>
		void Setup();

		#region Box総数
		/// <summary>
		/// Box総数設定
		/// </summary>
		void SetCapacity(int capacity, int itemCount);
		#endregion

		#region リストにアイテムをセット
		/// <summary>
		/// リストにアイテムを設定する
		/// </summary>
		void SetupItems(List<ItemInfo> itemInfoList);
		#endregion

		#region アイテム更新
		/// <summary>
		/// 全てのアイテムが更新された時のイベント通知
		/// </summary>
		event EventHandler OnUpdateItemsEvent;
		#endregion

		#region ページ
		/// <summary>
		/// 最初のページに設定
		/// </summary>
		void BackEndPage();
		#endregion

		#region アイテムイベント
		/// <summary>
		/// 登録されているアイテムが押された時のイベント通知用
		/// </summary>
		event EventHandler<ItemClickEventArgs> OnItemClickEvent;

		/// <summary>
		/// ページリストのアイテムに変更があった時のイベント通知用
		/// </summary>
		event EventHandler<ItemChangeEventArgs> OnItemChangeEvent;
		#endregion

		#region Box枠追加
		/// <summary>
		/// Box枠追加ボタンが押された時のイベント通知用
		/// </summary>
		event EventHandler OnAddCapacityClickEvent;
		#endregion

		#region ソート
		/// <summary>
		/// アイテムソート画面のOKボタンが押された時に呼ばれる
		/// </summary>
		void HandleOKClickEvent(ItemSort.OKClickEventArgs e);

		/// <summary>
		/// アイテムソート画面のソート項目に変化があった時に呼ばれる
		/// </summary>
		void HandleSortPatternChangeEvent();

		// アイテムソートに関わるデータをすべて同期させる
		void SyncItemSort();
		#endregion
	}

	/// <summary>
	/// アイテムページ付リスト制御
	/// </summary>
	public class PageListController : IPageListController
	{
		#region フィールド&プロパティ
		/// <summary>
		/// 共通モデル
		/// </summary>
		private readonly IModel _model;
		private IModel Model { get { return _model; } }

		/// <summary>
		/// 共通ビュー
		/// </summary>
		private readonly IView _view;
		private IView View { get { return _view; } }

		/// <summary>
		/// ページ付ビュー
		/// </summary>
		private readonly IItemPageListView _pageListView;
		private IItemPageListView PageListView { get { return _pageListView; } }

		/// <summary>
		/// 登録されているアイテムが押された時のイベント通知用
		/// </summary>
		public event EventHandler<ItemClickEventArgs> OnItemClickEvent = (sender, e) => { };

		/// <summary>
		/// ページリストのアイテムに変更があった時のイベント通知用
		/// </summary>
		public event EventHandler<ItemChangeEventArgs> OnItemChangeEvent;

		/// <summary>
		/// 所持数追加ボタンが押された時のイベント通知用
		/// </summary>
		public event EventHandler OnAddCapacityClickEvent = (sender, e) => { };

		/// <summary>
		/// 全てのアイテムが更新された時のイベント通知
		/// </summary>
		public event EventHandler OnUpdateItemsEvent = (sender, e) => { };

		/// <summary>
		/// 更新できる状態かどうか
		/// </summary>
		public bool CanUpdate
		{
			get
			{
				if (this.Model == null) return false;
				if (this.View == null) return false;
				if (this.PageListView == null) return false;
				return true;
			}
		}

		/// <summary>
		/// ソートタイプリスト
		/// </summary>
		LinkedList<ItemSort.SortPatternType> sortPatternList = new LinkedList<ItemSort.SortPatternType>();

		/// <summary>
		/// ソート項目をキーとした比較メソッド一覧
		/// </summary>
		Dictionary<ItemSort.SortPatternType, Func<ItemInfo, ItemInfo, bool, int>> compareDic = new Dictionary<ItemSort.SortPatternType, Func<ItemInfo, ItemInfo, bool, int>>();
		#endregion

		#region 初期化
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PageListController(IModel model, IView view, IItemPageListView pageListView)
		{
			if (model == null || view == null || pageListView == null) { return; }

			// ビュー設定
			this._view = view;
			this._pageListView = pageListView;
			this.PageListView.OnNextPageClickEvent += this.OnNextPage;
			this.PageListView.OnNextEndClickEvent += this.OnNextEndPage;
			this.PageListView.OnBackPageClickEvent += this.OnBackPage;
			this.PageListView.OnBackEndClickEvent += this.OnBackEndPage;
			this.PageListView.OnAddCapacityClickEvent += this.OnAddCapacityClick;
			this.PageListView.OnSortClickEvent += this.HandleSortClickEvent;

			// モデル設定
			this._model = model;
			this.Model.OnCapacityChange += this.HandleCapacityChange;
			this.Model.OnCountFormatChange += this.HandleStorageFormatChange;
			this.Model.OnCapacityFormatChange += this.HandleStorageFormatChange;
			this.Model.OnTotalCapacityChange += this.HandleTotalCapacityChange;
			this.Model.OnSetItemInfoListChange += this.HandleSetItemInfoListChange;
			this.Model.OnItemInfoListClear += this.HandleItemInfoListClear;
			this.Model.OnViewItemInfoListChange += this.HandleViewItemInfoListChange;

			// ページ付スクロールビューを初期化
			this.Model.ItemScrollView.Create(this.View.PageScrollViewAttach, null);

			// ソートタイプリスト初期化
			this.SetDefalutSortPatternList();
			InitSortCompareList();

			// データ初期化
			this.Setup();
		}

		/// <summary>
		/// データ初期化
		/// </summary>
		public void Setup()
		{
			if (!this.CanUpdate) { return; }
			// Box総数初期化
			this.SetCapacity(0, 0);
			// アイテム情報クリア
			this.ClearItemInfoList();
			// 1ページ目にセット
			this.BackEndPage();
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			if (this.CanUpdate)
			{
				this.Model.Dispose();
			}

			this.OnItemClickEvent = null;
			this.OnItemChangeEvent = null;
			this.OnAddCapacityClickEvent = null;
			this.OnUpdateItemsEvent = null;
		}

		/// <summary>
		/// ソートで使用する比較メソッドを登録する
		/// </summary>
		private void InitSortCompareList()
		{
			this.compareDic.Clear();
			this.compareDic.Add(ItemSort.SortPatternType.Name, (infoX, infoY, isAscend) => { return ItemInfo.NameCompare(infoX, infoY, isAscend); });
			this.compareDic.Add(ItemSort.SortPatternType.Type, (infoX, infoY, isAscend) => { return ItemInfo.TypeCompare(infoX, infoY, isAscend); });
			this.compareDic.Add(ItemSort.SortPatternType.Obtaining, (infoX, infoY, isAscend) => { return ItemInfo.IndexCompare(infoX, infoY, isAscend); });
		}
		/// <summary>
		/// デフォルトのソートリストセットする
		/// </summary>
		public void SetDefalutSortPatternList()
		{
			// ソート実行順にソートタイプを登録
			this.sortPatternList.Clear();
			this.sortPatternList.AddLast(ItemSort.SortPatternType.Name);
			this.sortPatternList.AddLast(ItemSort.SortPatternType.Type);
			this.sortPatternList.AddLast(ItemSort.SortPatternType.Obtaining);
		}
		#endregion

		#region Box総数設定
		/// <summary>
		/// Box総数設定
		/// </summary>
		public void SetCapacity(int capacity, int itemCount)
		{
			if (!this.CanUpdate) { return; }

			// オーバー分も含む総数セット
			int total = (itemCount > capacity) ? itemCount : capacity;
			this.SetTotalCapacity(total);

			// Box総数セット
			this.Model.Capacity = capacity;
		}

		/// <summary>
		/// Box総数変更ハンドラー
		/// </summary>
		private void HandleCapacityChange(object sender, EventArgs e)
		{
			this.SyncCapacity();
		}
		/// <summary>
		/// 容量表示形式変更ハンドラー
		/// </summary>
		private void HandleStorageFormatChange(object sender, EventArgs e)
		{
			this.SyncCapacity();
		}
		/// <summary>
		/// Box総数同期
		/// </summary>
		private void SyncCapacity()
		{
			if (!this.CanUpdate) { return; }
			// 容量表示設定
			int count = this.Model.ItemInfoList.Count;
			int capacity = this.Model.Capacity;
			string countFormat = string.Format(this.Model.CountFormat, count);
			string capacityFormat = string.Format(this.Model.CapacityFormat, capacity);
			StatusColor.Type colorType = capacity < count ? StatusColor.Type.CapacityOver : StatusColor.Type.CapacityNormal;
			this.PageListView.SetCapacity(countFormat, capacityFormat, colorType);
		}

		/// <summary>
		/// Box総数がオーバーした分も含む合計数
		/// </summary>
		private void SetTotalCapacity(int totalCapacity)
		{
			this.Model.TotalCapacity = totalCapacity;
		}

		/// <summary>
		/// Box総数がオーバーした分も含む合計数変更ハンドラー
		/// </summary>
		private void HandleTotalCapacityChange(object sender, EventArgs e)
		{
			this.SyncTotalCapacity();
		}
		/// <summary>
		/// Box総数がオーバーした分も含む合計数同期
		/// </summary>
		private void SyncTotalCapacity()
		{
			if (!this.CanUpdate) { return; }
			// ページスクロールビューのアイテム総数セット
			this.Model.ItemScrollView.Clear();
			this.Model.ItemScrollView.Setup(this.Model.TotalCapacity, 0);
		}
		#endregion

		#region リストにアイテムをセット
		/// <summary>
		/// リストにアイテムを設定する
		/// </summary
		public void SetupItems(List<ItemInfo> itemInfoList)
		{
			if (!this.CanUpdate) { return; }
			// アイテム情報リストセット
			this.SetItemInfoList(itemInfoList);
		}
		#endregion

		#region アイテム情報リスト
		/// <summary>
		/// アイテム情報リストセット
		/// </summary>
		private void SetItemInfoList(List<ItemInfo> itemInfoList)
		{
			this.Model.SetItemInfoList(itemInfoList);
		}

		/// <summary>
		/// アイテム情報リストクリア
		/// </summary>
		private void ClearItemInfoList()
		{
			this.Model.ClearItemInfoList();
		}

		/// <summary>
		/// アイテム情報リスト設定変更ハンドラー
		/// </summary>
		private void HandleSetItemInfoListChange(object sender, EventArgs e)
		{
			this.SyncItemInfoList();
		}
		/// <summary>
		/// アイテム情報リストクリアハンドラー
		/// </summary>
		private void HandleItemInfoListClear(object sender, EventArgs e)
		{
			this.SyncItemInfoList();
		}
		/// <summary>
		/// アイテム情報リスト同期
		/// </summary>
		private void SyncItemInfoList()
		{
			// アイテム総数同期
			this.SyncCapacity();
			// アイテム情報リストと表示アイテム情報リストを同期
			this.SetViewItemInfoList(this.Model.ItemInfoList);
		}
		#endregion

		#region 表示アイテム情報リスト
		/// <summary>
		/// 表示用のアイテム情報リストをセットする
		/// </summary>
		private void SetViewItemInfoList(List<ItemInfo> itemInfoList)
		{
			if (!this.CanUpdate) { return; }

			// ソート反映
			List<ItemInfo> viewList = this.SortExecute(itemInfoList);

			// セット
			this.Model.SetViewItemInfoList(viewList);
		}

		/// <summary>
		/// 表示アイテム情報リスト変更ハンドラー
		/// </summary>
		private void HandleViewItemInfoListChange(object sender, EventArgs e)
		{
			this.SyncViewItemInfoList();
		}
		/// <summary>
		/// 表示アイテム情報リスト同期
		/// </summary>
		private void SyncViewItemInfoList()
		{
			// アイテム更新
			this.UpdateItems();
		}
		#endregion

		#region ページ内アイテム更新
		/// <summary>
		/// 現ページ内のアイテム更新
		/// </summary>
		private void UpdateItems()
		{
			if (!this.CanUpdate) { return; }

			// 表示アイテム情報リストを取得
			List<ItemInfo> itemInfoList = this.Model.ViewItemInfoList;

			// 現ページの最初のアイテムインデックスを取得
			int startIndex = this.Model.ItemScrollView.NowPageStartIndex;

			// 現在のページ内のアイテムを更新
			for (int i = 0, max = this.Model.ItemScrollView.NowPageItemMax; i < max; i++)
			{
				int indexInTotal = i + startIndex;
				if (!this.Model.ItemScrollView.IsNowPage(indexInTotal))
				{
					// 現在のページ内のアイテムではない
					continue;
				}

				// スクロールビューからアイテムを取得
				int itemIndex = this.Model.ItemScrollView.GetItemIndex(indexInTotal);
				GUIItem item = this.Model.ItemScrollView.GetItem(itemIndex);
				if (item == null) { continue; }

				// 前回分のイベントは削除しておく
				item.OnItemClickEvent -= this.OnItemClick;
				item.OnItemChangeEvent -= this.OnItemChange;
				// アイテムボタンイベント登録
				item.OnItemClickEvent += this.OnItemClick;
				item.OnItemChangeEvent += this.OnItemChange;

				// アイテムの種類を設定
				if (itemInfoList.Count <= indexInTotal)
				{
					// 所持キャラ数よりも所有できる枠数の方が多ければ空枠とする
					item.SetState(Item.ItemStateType.Empty, null);
				}
				else
				{
					ItemInfo itemInfo = itemInfoList[indexInTotal];
					Item.ItemStateType itemState;
					if (itemInfo != null)
					{
						// アイテムが存在する枠
						itemState = Item.ItemStateType.ItemIcon;

						// アイテムセット
						item.SetState(itemState, itemInfo);
					}
				}
			}

			// 通知
			this.OnUpdateItemsEvent(this, EventArgs.Empty);
		}
		#endregion

		#region ページ
		/// <summary>
		/// 次のページ
		/// </summary>
		private void OnNextPage(object sender, EventArgs e)
		{
			if (!CanUpdate) { return; }
			bool isPageChange = this.Model.ItemScrollView.SetNextPage(1);
			if (isPageChange)
			{
				this.ChangePage();
			}
		}

		/// <summary>
		/// 最後のページ
		/// </summary>
		private void OnNextEndPage(object sender, EventArgs e)
		{
			if (!CanUpdate) { return; }
			// 最後のページにする
			bool isPageChange = this.Model.ItemScrollView.SetPage(this.Model.ItemScrollView.PageMax - 1, 0);
			if (isPageChange)
			{
				this.ChangePage();
			}

		}

		/// <summary>
		/// 戻るページ
		/// </summary>
		private void OnBackPage(object sender, EventArgs e)
		{
			if (!CanUpdate) { return; }
			// ページを一つ前に戻す
			bool isPageChange = this.Model.ItemScrollView.SetNextPage(-1);
			if (isPageChange)
			{
				this.ChangePage();
			}
		}

		/// <summary>
		/// 最初のページボタンが押された時に呼ばれる
		/// </summary>
		private void OnBackEndPage(object sender, EventArgs e)
		{
			this.BackEndPage();
		}

		/// <summary>
		/// 最初のページに設定
		/// </summary>
		public void BackEndPage()
		{
			if (!CanUpdate) { return; }
			// 最初のページにする
			var isPageChange = this.Model.ItemScrollView.SetPage(0, 0);
			if (isPageChange)
			{
				this.ChangePage();
			}
		}

		/// <summary>
		/// ページが切り替わった時の処理
		/// </summary>
		private void ChangePage()
		{
			// ページが切り替わったのでアイテムの更新を行う
			UpdateItems();
		}
		#endregion

		#region アイテムイベント
		/// <summary>
		/// 登録されているアイテムが押された時に呼び出される
		/// </summary>
		private void OnItemClick(GUIItem item)
		{
			// 通知
			var eventArgs = new ItemClickEventArgs(item);
			this.OnItemClickEvent(this, eventArgs);
		}

		/// <summary>
		/// 登録されいているアイテムに変更があった時に呼び出される
		/// </summary>
		private void OnItemChange(GUIItem item)
		{
			// 通知
			var eventArgs = new ItemChangeEventArgs(item);
			this.OnItemChangeEvent(this, eventArgs);
		}
		#endregion

		#region Box枠追加
		/// <summary>
		/// Box枠追加ボタンが押された時に呼び出される
		/// </summary>
		private void OnAddCapacityClick(object sender, EventArgs e)
		{
			// 通知
			this.OnAddCapacityClickEvent(this, EventArgs.Empty);
		}
		#endregion

		#region ソート
		/// <summary>
		/// ソートボタンが押された時に呼ばれる
		/// </summary>
		private void HandleSortClickEvent(object sender, EventArgs e)
		{
			// アイテムソート画面表示
			var singleUI = new GUISingle(GUIItemSort.Open, GUIItemSort.Close);
			GUIController.SingleOpen(singleUI);
		}

		/// <summary>
		/// ソートリストに登録されている順にソートを実行させる
		/// </summary>
		private List<ItemInfo> SortExecute(List<ItemInfo> itemInfoList)
		{
			var sortList = new List<ItemInfo>();

			// アイテム情報リスト取得
			if (itemInfoList == null) { return sortList; }
			itemInfoList.ForEach((info) => { sortList.Add(info); });

			// ソート
			this.Sort(sortList);

			return sortList;
		}

		/// <summary>
		/// ソート項目リストに沿って並び替えを行う
		/// </summary>
		private void Sort(List<ItemInfo> itemInfoList)
		{
			// 昇順/降順フラグ取得
			bool isAscend = GUIItemSort.GetIsAscend();

			// ソートを行う
			itemInfoList.MargeSort((x, y) =>
			{
				return this.ItemInfoCompare(x, y, isAscend);
			});
		}
		#endregion

		#region 比較
		/// <summary>
		/// アイテム情報の比較
		/// </summary>
		private int ItemInfoCompare(ItemInfo infoX, ItemInfo infoY, bool isAscend)
		{
			if (infoX == null || infoY == null) { return 0; }
			int ret = 0;
			foreach (ItemSort.SortPatternType type in this.sortPatternList)
			{
				Func<ItemInfo, ItemInfo, bool, int> compare;
				if(this.compareDic.TryGetValue(type, out compare))
				{
					ret = compare(infoX, infoY, isAscend);
					if(ret != 0)
					{
						return ret;
					}
				}
			}

			return ret;
		}
		#endregion

		#region アイテムソート画面
		/// <summary>
		/// アイテムソート画面のOKボタンが押された時に呼ばれる
		/// </summary>
		public void HandleOKClickEvent(ItemSort.OKClickEventArgs e)
		{
			// ソートリスト同期
			this.SyncSortPatternList();

			// キャラソート画面は閉じる
			GUIController.SingleClose();

			// 表示アイテム情報を更新
			this.SetViewItemInfoList(this.Model.ItemInfoList);
		}

		/// <summary>
		/// ソート項目リスト同期
		/// </summary>
		private void SyncSortPatternList()
		{
			// デフォルト順にリセット
			this.SetDefalutSortPatternList();

			var sortPattern = GUIItemSort.GetSortPattern();
			// 選択されているソートタイプが最優先にソートされるようにリストに登録
			if (this.sortPatternList.Contains(sortPattern))
			{
				this.sortPatternList.Remove(sortPattern);
			}
			this.sortPatternList.AddFirst(sortPattern);
		}

		/// <summary>
		/// アイテムソート画面のソート項目に変化があった時に呼ばれる
		/// </summary>
		public void HandleSortPatternChangeEvent()
		{
			// 選択されているソート項目同期
			this.SyncSelectSortPattern();
		}

		/// <summary>
		/// 選択されているソート項目を同期
		/// </summary>
		private void SyncSelectSortPattern()
		{
			if (!this.CanUpdate) { return; }

			// ソート項目表示設定
			var sortPattern = GUIItemSort.GetSortPattern();
			string typeName = MasterData.GetText(TextType.TX239_CharaList_Order);
			switch(sortPattern)
			{
				case ItemSort.SortPatternType.Name:
					typeName += MasterData.GetText(TextType.TX351_Sort_Name);
					break;
				case ItemSort.SortPatternType.Type:
					typeName += MasterData.GetText(TextType.TX260_Sort_Type);
					break;
				case ItemSort.SortPatternType.Obtaining:
					typeName += MasterData.GetText(TextType.TX261_Sort_Obtaining);
					break;
			}

			this.PageListView.SetSortTypeName(typeName);
		}

		// アイテムソートに関わるデータをすべて同期させる
		public void SyncItemSort()
		{
			// 選択されている項目とソートリストを同期
			this.SyncSortPatternList();
			this.SyncSelectSortPattern();
		}
		#endregion
	}
}