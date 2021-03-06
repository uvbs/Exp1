/// <summary>
/// アイテムBOX制御
/// 
/// 2016/03/30
/// </summary>
using UnityEngine;
using System;
using System.Collections.Generic;
using Scm.Common.XwMaster;

namespace XUI.ItemBox
{
	#region イベント引数
	/// <summary>
	/// まとめて売却イベント引数
	/// </summary>
	public class SellMultiEventArgs : EventArgs
	{
		public List<int> IndexList { get; set; }
	}

	/// <summary>
	/// ロックイベント引数
	/// </summary>
	public class ItemLockEventArgs : EventArgs
	{
		public int ItemMasterId { get; set; }
		public bool IsLock { get; set; }
	}
	#endregion

	/// <summary>
	/// アイテムBOX制御インターフェイス
	/// </summary>
	public interface IController
	{
		#region 更新チェック
		/// <summary>
		/// 更新できる状態かどうか
		/// </summary>
		bool CanUpdate { get; }
		#endregion

		#region 初期化
		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();

		/// <summary>
		/// データ初期化
		/// </summary>
		void Setup();

		/// <summary>
		/// 再初期化
		/// </summary>
		void ReSetup();
		
		/// <summary>
		/// ステータス情報設定
		/// </summary>
		void SetupStatusInfo(int haveMoney);
		#endregion

		#region アクティブ設定
		/// <summary>
		/// アクティブ設定
		/// </summary>
		void SetActive(bool isActive, bool isTweenSkip);
		#endregion

		#region BOX総数
		/// <summary>
		/// BOXの総数設定
		/// </summary>
		void SetupCapacity(int capacity, int itemCount);
		#endregion

		#region アイテム情報リスト
		/// <summary>
		/// アイテム情報リスト設定
		/// </summary>
		void SetupItemInfoList(List<ItemInfo> itemInfoList);
		#endregion

		#region アイテムロック
		/// <summary>
		/// アイテムロックイベント
		/// </summary>
		event EventHandler<ItemLockEventArgs> OnItemLock;
		#endregion

		#region まとめて売却
		/// <summary>
		/// まとめて売却イベント
		/// </summary>
		event EventHandler<SellMultiEventArgs> OnSellMulti;

		/// <summary>
		/// まとめて売却結果
		/// </summary>
		void SellMultiResult(bool result, List<int> indexList, int money, int soldPrice);
		#endregion
	}

	/// <summary>
	/// アイテムBOX制御
	/// </summary>
	public class Controller : IController
	{
		#region フィールド&プロパティ
		/// <summary>
		/// モデル
		/// </summary>
		private readonly IModel _model;
		private IModel Model { get { return _model; } }

		/// <summary>
		/// ビュー
		/// </summary>
		private readonly IView _view;
		private IView View { get { return _view; } }

		/// <summary>
		/// 更新できる状態かどうか
		/// </summary>
		public bool CanUpdate
		{
			get
			{
				if (this.Model == null) { return false; }
				if (this.View == null) { return false; }
				return true;
			}
		}

		/// <summary>
		/// アイテムページリスト
		/// </summary>
		private readonly GUIItemPageList _itemList;
		private GUIItemPageList ItemList { get { return _itemList; } }

		/// <summary>
		/// ベースアイテム
		/// </summary>
		private readonly GUIItem _baseItem;
		private GUIItem BaseItem { get { return _baseItem; } }

		/// <summary>
		/// ベースアイテムのインデックス
		/// </summary>
		private int BaseItemIndex
		{
			get
			{
				int index = -1;
				var info = this.BaseItemInfo;
				if (info != null) { index = info.Index; }
				return index;
			}
		}

		/// <summary>
		/// ベースアイテムのマスターデータID
		/// </summary>
		private int BaseItemMasterID
		{
			get
			{
				int masterID = 0;
				var info = this.BaseItemInfo;
				if (info != null) { masterID = info.ItemMasterID; }
				return masterID;
			}
		}

		/// <summary>
		/// ベースアイテム情報
		/// </summary>
		private ItemInfo BaseItemInfo { get { return (this.BaseItem != null ? this.BaseItem.GetItemInfo() : null); } }

		/// <summary>
		/// ベースアイテムが空かどうか
		/// </summary>
		private bool IsEmptyBaseItem { get { return (this.BaseItemInfo == null ? true : false); } }

		/// <summary>
		/// 売却アイテムリスト
		/// </summary>
		private readonly GUISelectItemList _sellItemList;
		private GUISelectItemList SellItemList { get { return _sellItemList; } }

		/// <summary>
		/// Boxのモード
		/// </summary>
		private ModeType Mode { get; set; }
		private enum ModeType : byte
		{
			ItemUse,		// アイテム選択モード
			Sell,			// 売却モード
			SellMulti,		// まとめて売却モード
		}

		/// <summary>
		/// BOXモード切替リスト
		/// </summary>
		private Dictionary<ModeType, Action<bool>> changeModeExecuteDic = new Dictionary<ModeType, Action<bool>>();

		/// <summary>
		/// アイテムロックイベント
		/// </summary>
		public event EventHandler<ItemLockEventArgs> OnItemLock = (sender, e) => { };

		/// <summary>
		/// まとめて売却イベント
		/// </summary>
		public event EventHandler<SellMultiEventArgs> OnSellMulti = (sender, e) => { };
		#endregion

		#region 初期化
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Controller(IModel model, IView view, GUIItemPageList itemList, GUISelectItemList sellItemList, GUIItem baseItem)
		{
			if (model == null || view == null) { return; }

			// ページリストとベースアイテムと売却アイテムリストをセット
			this._itemList = itemList;
			this._baseItem = baseItem;
			this._sellItemList = sellItemList;

			// ビュー設定
			this._view = view;
			// 各ボタンイベント登録
			this.View.OnHome += this.HandleHome;
			this.View.OnClose += this.HandleClose;
			this.View.OnItemUseMode += this.HandleItemUseMode;
			this.View.OnSellMode += HandleSellMode;
			this.View.OnSellMultiMode += HandleSellMultiMode;
			this.View.OnExecute += this.HandleExecute;
			this.View.OnLock += this.HandleLock;

			// モデル設定
			this._model = model;
			// 所持金イベント登録
			this.Model.OnHaveMoneyChange += this.HandleHaveMoneyChange;
			this.Model.OnHaveMoneyFormatChange += this.HandleHaveMoneyFormatChange;
			// 総売却額イベント登録
			this.Model.OnTotalSoldPriceChange += this.HandleTotalSoldPriceChange;
			this.Model.OnTotalSoldPriceFormatChange += this.HandleTotalSoldPriceFormatChange;
			// アイテム情報リストイベント登録
			this.Model.OnItemInfoListChange += this.HandleItemInfoListChange;
			this.Model.OnClearItemInfoList += this.HandleClearItemInfoList;
			// ベースアイテムステータスイベント登録
			this.Model.OnItemNameChange += this.HandleItemNameChange;
			this.Model.OnDescriptionChange += this.HandleDescriptionChange;
			this.Model.OnLockChange += this.HandleIsLockChange;
			this.Model.OnItemCountChange += this.HandleItemCountChange;
			this.Model.OnSoldPriceChange += this.HandleSoldPriceChange;

			// アイテムページリスト設定
			if(this.ItemList != null)
			{
				// イベント登録
				this.ItemList.OnItemClickEvent += this.HandleItemListItemClick;
				this.ItemList.OnUpdateItemsEvent += this.HandleItemListUpdateItems;
			}
			// ベースアイテム設定
			if(this.BaseItem != null)
			{
				// イベント登録
				this.BaseItem.OnItemChangeEvent += this.HandleBaseItemChange;
				this.BaseItem.OnItemClickEvent += this.HandleBaseItemClick;
				this.BaseItem.SetSelect(true);
			}
			// 売却リスト設定
			if(this.SellItemList != null)
			{
				// イベント登録
				this.SellItemList.OnItemClickEvent += this.HandleSellListItemClick;
				this.SellItemList.OnUpdateItemsEvent += this.HandleSellListUpdateItems;
			}

			// モード切替実行処理をリストに登録
			SetChangeModeExecuteList();

			// 同期
			this.SyncBaseItemStatus();
			this.SyncHaveMoney();
			this.SyncTotalSoldPrice();
			this.UpdateExecuteButtonEnable();
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			if(this.CanUpdate)
			{
				this.Model.Dispose();
			}

			this.OnSellMulti = null;
			this.OnItemLock = null;
		}

		/// <summary>
		/// モード切替実行処理をリストに登録する
		/// </summary>
		private void SetChangeModeExecuteList()
		{
			this.changeModeExecuteDic.Clear();
			this.changeModeExecuteDic.Add(ModeType.ItemUse, this.ItemUseMode);
			this.changeModeExecuteDic.Add(ModeType.Sell, this.SellMode);
			this.changeModeExecuteDic.Add(ModeType.SellMulti, this.SellMultiMode);
		}

		/// <summary>
		/// データ初期化
		/// </summary>
		public void Setup()
		{
			// アイテム情報リスト初期化
			this.ClearItemInfoList();
			// アイテム情報モードに設定
			this.SetMode(ModeType.ItemUse);
		}

		/// <summary>
		/// 再初期化
		/// </summary>
		public void ReSetup()
		{
			// アイテム情報リスト初期化
			this.ClearItemInfoList();
		}

		/// <summary>
		/// ステータス情報設定
		/// </summary>
		public void SetupStatusInfo(int haveMoney)
		{
			if (this.CanUpdate)
			{
				this.Model.HaveMoney = haveMoney;
			}
		}
		#endregion

		#region アクティブ設定
		/// <summary>
		/// アクティブ設定
		/// </summary>
		public void SetActive(bool isActive, bool isTweenSkip)
		{
			if (this.CanUpdate)
			{
				this.View.SetActive(isActive, isTweenSkip);

				// その他UIの表示設定
				GUILobbyResident.SetActive(!isActive);
				// タイトル設定
				GUIScreenTitle.Play(isActive, MasterData.GetText(TextType.TX343_ItemBox_Select_Title));
				// 各メッセージの状態を設定
				this.UpdateMessage();
			}
		}
		#endregion

		#region 各状態を更新する
		/// <summary>
		/// メッセージの状態を更新する
		/// </summary>
		private void UpdateMessage()
		{
			if(!this.CanUpdate) {return;}

			var state = this.View.GetActiveState();
			bool isActive = false;
			if (state == GUIViewBase.ActiveState.Opened || state == GUIViewBase.ActiveState.Opening)
			{
				isActive = true;
			}

			string help = string.Empty;
			switch(this.Mode)
			{
				case ModeType.ItemUse:
					help = MasterData.GetText(TextType.TX360_ItemBox_ItemUse_Help);
					break;
				case ModeType.Sell:
					help = MasterData.GetText(TextType.TX345_ItemBox_Sell_Help);
					break;
				case ModeType.SellMulti:
					help = MasterData.GetText(TextType.TX346_ItemBox_MultiSell_Help);
					break;
			}

			// メッセージセット
			GUIHelpMessage.Play(isActive, help);
		}
		#endregion

		#region BOX総数
		/// <summary>
		/// BOXの総数設定
		/// </summary>
		public void SetupCapacity(int capacity, int itemCount)
		{
			if(this.ItemList != null)
			{
				this.ItemList.SetupCapacity(capacity, itemCount);
			}
		}
		#endregion

		#region アイテム情報リスト
		#region セット
		/// <summary>
		/// アイテム情報リスト設定
		/// </summary>
		public void SetupItemInfoList(List<ItemInfo> itemInfoList)
		{
			if (!this.CanUpdate) { return; }
			this.Model.SetItemInfoList(itemInfoList);
		}

		/// <summary>
		/// アイテム情報リスト変更ハンドラー
		/// </summary>
		private void HandleItemInfoListChange(object sender, EventArgs e)
		{
			this.SyncItemInfoList();
		}
		/// <summary>
		/// アイテム情報リスト同期
		/// </summary>
		private void SyncItemInfoList()
		{
			if (!this.CanUpdate) { return; }

			// アイテム一覧取得
			List<ItemInfo> itemInfoList = this.Model.GetItemInfoList();

			// ベースアイテム情報更新
			this.UpdateBaseItemInfo();
			// アイテムページリスト設定
			this.SetItemList(itemInfoList);
		}
		#endregion

		#region クリア
		/// <summary>
		/// アイテム情報リストクリア
		/// </summary>
		private void ClearItemInfoList()
		{
			if (!this.CanUpdate) { return; }
			this.Model.ClearItemInfoList();
		}

		/// <summary>
		/// アイテム情報リストクリアハンドラー
		/// </summary>
		private void HandleClearItemInfoList(object sender, EventArgs e)
		{
			this.SyncClearItemInfoList();
		}
		/// <summary>
		/// アイテム情報リストクリア同期
		/// </summary>
		private void SyncClearItemInfoList()
		{
			// ベース/ページ/売却アイテムリストを削除する
			this.ClearSellItemList();
			this.ClearBaseItem();
			this.ClearItemList();
		}
		#endregion

		#region 取得
		/// <summary>
		/// 指定された種類のアイテム数を取得する
		/// </summary>
		private int GetItemCount(int itemMasterId)
		{
			int itemCount = 0;
			if (!this.CanUpdate) { return itemCount; }
			
			// 同種類のアイテム一覧を取得する
			Dictionary<int, ItemInfo> indexItemDic = new Dictionary<int, ItemInfo>();
			if(this.Model.TryGetItemInfoByMasterId(itemMasterId, out indexItemDic))
			{
				itemCount = indexItemDic.Values.Count;
			}

			return itemCount;
		}
		#endregion
		#endregion

		#region アイテムページリスト
		#region セット
		/// <summary>
		/// アイテムリストの設定
		/// </summary>
		private void SetItemList(List<ItemInfo> itemInfoList)
		{
			if (this.ItemList == null) { return; }
			this.ItemList.SetupItems(itemInfoList);
		}
		#endregion

		#region クリア
		/// <summary>
		/// アイテムリストクリア
		/// </summary>
		private void ClearItemList()
		{
			if (this.ItemList == null) { return; }
			// データ初期化
			this.ItemList.Setup();
		}
		#endregion

		#region アイテム設定
		/// <summary>
		/// アイテムページリスト内のアイテムが押下された時のハンドラー
		/// </summary>
		private void HandleItemListItemClick(GUIItem item)
		{
			// アイテム設定
			this.SetItemListItem(item);
		}

		/// <summary>
		/// アイテムページリスト内のアイテム設定
		/// </summary>
		private void SetItemListItem(GUIItem item)
		{
			if (item == null) { return; }

			var disableState = item.GetDisableState();
			var info = item.GetItemInfo();
			if (info == null) { return; }

			switch(disableState)
			{
				case Item.DisableType.None:
					if (this.Mode == ModeType.ItemUse || this.Mode == ModeType.Sell)
					{
						// 選択モード時
						if (!info.IsEmpty)
						{
							// ベースアイテムに設定
							this.SetBaseItem(info);
						}
					}
					else if (this.Mode == ModeType.SellMulti)
					{
						// まとめて選択時は売却リストに追加
						this.AddSellItem(info);
					}
					break;
				case Item.DisableType.Bait:
					// 売却リストから外す
					this.RemoveSellItem(info);
					break;
				case Item.DisableType.Select:
					// ベースアイテムに設定されているアイテムならベースアイテムを外す
					this.ClearBaseItem();
					this.UpdateItemList();
					break;
			}
		}
		#endregion

		#region ページ内アイテム更新
		/// <summary>
		/// ページ内のアイテムが全て更新された時のハンドラー
		/// </summary>
		private void HandleItemListUpdateItems()
		{
			this.UpdateItemList();
		}

		/// <summary>
		/// ページ内アイテムリストの更新処理
		/// </summary>
		private void UpdateItemList()
		{
			if (this.ItemList == null) { return; }

			// 無効タイプ更新
			this.UpdateItemListDisable();
		}

		#region 無効設定
		/// <summary>
		/// ページ内リストのアイテムの無効タイプを更新する
		/// </summary>
		private void UpdateItemListDisable()
		{
			switch(this.Mode)
			{
				case ModeType.ItemUse:
				case ModeType.Sell:
				{
					// アイテム選択時の無効設定を行う
					var list = this.ItemList.GetNowPageItemList();
					list.ForEach(this.SetSelectItemDisableState);
					break;
				}
				case ModeType.SellMulti:
				{
					// まとめて売却時の無効設定を行う
					foreach (var item in this.ItemList.GetNowPageItemList())
					{
						this.SetMultiSellItemDisableState(item, this.GetSellItemIndexList());
					}
					break;
				}
			}
		}
		/// <summary>
		/// アイテム選択時の無効設定
		/// </summary>
		private void SetSelectItemDisableState(GUIItem item)
		{
			if (item == null) { return; }

			// 無効タイプを設定する
			Item.DisableType disableType;
			this.GetSelectItemDisableType(item.GetItemInfo(), out disableType);
			item.SetDisableState(disableType);
		}
		/// <summary>
		/// アイテム選択時の無効タイプを取得する
		/// </summary>
		private void GetSelectItemDisableType(ItemInfo itemInfo, out Item.DisableType disableType)
		{
			disableType = Item.DisableType.None;
			if (itemInfo == null) { return; }

			// 以下無効にするかチェック
			// 優先順位があるので注意
			bool isSelect = itemInfo.Index == this.BaseItemIndex;
			if (isSelect)
			{
				// アイテム選択中
				disableType = Item.DisableType.Select;
			}
		}
		/// <summary>
		/// まとめて選択時の無効設定
		/// </summary>
		private void SetMultiSellItemDisableState(GUIItem item, List<int> sellIndexList)
		{
			if (item == null) { return; }

			// 無効タイプを設定する
			Item.DisableType disableType;
			int sellIndex = -1;
			this.GetMultiSellItemDisableType(item.GetItemInfo(), sellIndexList, out disableType, out sellIndex);
			// 無効タイプを設定する
			if (sellIndex >= 0)
			{
				// 売却のインデックスがある場合は餌
				item.SetBaitState(sellIndex);
			}
			else
			{
				// それ以外は無効タイプを設定する
				item.SetDisableState(disableType);
			}
		}
		/// <summary>
		/// まとめて売却時の無効タイプを取得する
		/// </summary>
		private void GetMultiSellItemDisableType(ItemInfo itemInfo, List<int> sellIndexList, out Item.DisableType disableType, out int sellIndex)
		{
			disableType = Item.DisableType.None;
			sellIndex = -1;
			if (itemInfo == null) { return; }

			// 以下無効にするかチェック
			// 優先順位があるので注意
			sellIndex = sellIndexList.FindIndex((index) => { return index == itemInfo.Index; });
			if (sellIndex >= 0)
			{
				// 売却アイテム選択中
				disableType = Item.DisableType.Bait;
			}
			else if (itemInfo.IsLock) disableType = Item.DisableType.Lock;
		}
		#endregion
		#endregion
		#endregion

		#region ベースアイテム
		#region ベースアイテム設定
		/// <summary>
		/// ベースアイテムを設定
		/// </summary>
		private void SetBaseItem(ItemInfo itemInfo)
		{
			// ベースアイテム状態設定
			this.SetBaseItemState(itemInfo);
			// ページ内アイテムリストの更新
			this.UpdateItemList();
		}
		#endregion

		#region アイテム状態
		/// <summary>
		/// ベースアイテム状態設定
		/// </summary>
		private void SetBaseItemState(ItemInfo itemInfo)
		{
			var state = Item.ItemStateType.ItemIcon;
			if(itemInfo == null || itemInfo.IsEmpty)
			{
				state = Item.ItemStateType.FillEmpty;
			}
			this.BaseItem.SetState(state, itemInfo);
		}	
		#endregion

		#region ベースアイテム外す
		/// <summary>
		/// ベースアイテムを外す
		/// </summary>
		private void ClearBaseItem()
		{
			this.SetBaseItemState(null);
		}
		#endregion

		#region ベースアイテム同期
		/// <summary>
		/// ベースアイテムの変更ハンドラー
		/// </summary>
		private void HandleBaseItemChange(GUIItem item)
		{
			this.SyncBaseItem();
		}

		/// <summary>
		/// ベースアイテム同期
		/// </summary>
		private void SyncBaseItem()
		{
			// ベースアイテムのステータスセット
			this.SetBaseItemStatus();
			// 実行ボタン更新
			this.UpdateExecuteButtonEnable();
		}

		/// <summary>
		/// ベースアイテムのステータス情報をセットする
		/// </summary>
		private void SetBaseItemStatus()
		{
			string name = string.Empty;
			string description = string.Empty;
			bool isLock = false;
			int itemCount = 0;
			int soldPrice = 0;

			var baseItemInfo = this.BaseItemInfo;
			if(baseItemInfo != null)
			{
				// アイテムマスターデータ取得
				ItemMasterData masterData;
				if (this.TryGetItemMasterData(baseItemInfo.ItemMasterID, out masterData))
				{
					// ベースアイテム情報セット
					name = masterData.Name;
					description = masterData.Description;
					isLock = baseItemInfo.IsLock;
					itemCount = this.GetItemCount(baseItemInfo.ItemMasterID);
					soldPrice = masterData.SellGameMoney;
				}
			}

			// データセット
			this.Model.ItemName = name;
			this.Model.Description = description;
			this.Model.IsLock = isLock;
			this.Model.ItemCount = itemCount;
			this.Model.SoldPrice = soldPrice;
		}
		#endregion

		#region ベースアイテム更新
		/// <summary>
		/// ベースアイテム情報更新
		/// </summary>
		private void UpdateBaseItemInfo()
		{
			if (!this.CanUpdate) { return; }
			if (this.IsEmptyBaseItem) { return; }

			// アイテム情報取得
			ItemInfo newInfo;
			if(this.Model.TryGetItemInfo(this.BaseItemInfo.ItemMasterID, this.BaseItemInfo.Index, out newInfo))
			{
				// 更新
				this.SetBaseItemState(newInfo);
			}
			else
			{
				// 存在しない場合はベースアイテムを外す
				this.ClearBaseItem();
			}
		}
		#endregion

		#region ベースアイテム押下
		/// <summary>
		/// ベースアイテム押下ハンドラー
		/// </summary>
		private void HandleBaseItemClick(GUIItem item)
		{
			if(!this.IsEmptyBaseItem)
			{
				// ベースアイテムが設定されていたらベースアイテムを外す
				this.ClearBaseItem();
				// ページ内アイテムリストの更新
				this.UpdateItemList();
			}
		}
		#endregion

		#region ベースアイテムステータス
		/// <summary>
		/// ベースアイテム名変更ハンドラー
		/// </summary>
		private void HandleItemNameChange(object sender, EventArgs e)
		{
			this.SyncItemName();
		}
		/// <summary>
		/// ベースアイテム名同期
		/// </summary>
		private void SyncItemName()
		{
			if (!this.CanUpdate) { return; }
			this.View.SetItemName(this.Model.ItemName);
		}
		
		/// <summary>
		/// ベースアイテム説明変更ハンドラー
		/// </summary>
		private void HandleDescriptionChange(object sender, EventArgs e)
		{
			this.SyncDescription();
		}
		/// <summary>
		/// ベースアイテム説明同期
		/// </summary>
		private void SyncDescription()
		{
			if (!this.CanUpdate) { return; }
			this.View.SetItemDescription(this.Model.Description);
		}

		/// <summary>
		/// ベースアイテムロック変更ハンドラー
		/// </summary>
		private void HandleIsLockChange(object sender, EventArgs e)
		{
			this.SyncIsLock();
		}
		/// <summary>
		/// ベースアイテムロック同期
		/// </summary>
		private void SyncIsLock()
		{
			if (!this.CanUpdate) { return; }
			this.View.SetItemLock(this.Model.IsLock);
		}

		/// <summary>
		/// ベースアイテムの個数変更ハンドラー
		/// </summary>
		private void HandleItemCountChange(object sender, EventArgs e)
		{
			this.SyncItemCount();
		}
		/// <summary>
		/// ベースアイテムの個数同期
		/// </summary>
		private void SyncItemCount()
		{
			if (!this.CanUpdate) { return; }
			this.View.SetItemCount(this.Model.ItemCount.ToString());
		}

		/// <summary>
		/// ベースアイテムの売却額変更ハンドラー
		/// </summary>
		private void HandleSoldPriceChange(object sender, EventArgs e)
		{
			this.SyncSoldPrice();
		}
		/// <summary>
		/// ベースアイテムの売却額同期
		/// </summary>
		private void SyncSoldPrice()
		{
			if (!this.CanUpdate) { return; }
			this.View.SetItemSoldPrice(this.Model.SoldPrice, this.Model.SoldPriceFormat);
		}

		/// <summary>
		/// 全てのベースアイテムのステータスを同期する
		/// </summary>
		private void SyncBaseItemStatus()
		{
			this.SyncItemName();
			this.SyncDescription();
			this.SyncIsLock();
			this.SyncItemCount();
			this.SyncSoldPrice();
		}
		#endregion

		#region ベースアイテムロック
		/// <summary>
		/// アイテムロックボタンクリックイベントハンドラー
		/// </summary>
		private void HandleLock(object sender, EventArgs e)
		{
			// ベースアイテムが設定されている状態かチェック
			if (this.IsEmptyBaseItem) { return; }

			// 通知
			var eventArgs = new ItemLockEventArgs();
			eventArgs.ItemMasterId = this.BaseItemInfo.ItemMasterID;
			eventArgs.IsLock = !this.BaseItemInfo.IsLock;
			this.OnItemLock(this, eventArgs);
		}
		#endregion
		#endregion

		#region 売却リスト
		#region 追加/削除
		/// <summary>
		/// 売却リストに追加する
		/// </summary>
		private void AddSellItem(ItemInfo itemInfo)
		{
			if (this.SellItemList == null || itemInfo == null) { return; }
			
			// 追加
			if(this.SellItemList.AddItem(itemInfo))
			{
				// 売却リストの選択アイテム更新
				this.UpdateSelectSellItemList();
				// 売却リストのスクロール更新
				this.SellItemList.UpdateScroll();
				// ページ内リストの更新
				this.UpdateItemList();
			}
		}

		/// <summary>
		/// 売却リストから削除する
		/// </summary>
		private void RemoveSellItem(ItemInfo itemInfo)
		{
			if (this.SellItemList == null || itemInfo == null) { return; }
			
			// 削除
			if(this.SellItemList.RemoveItem(itemInfo))
			{
				// 売却リストの選択アイテム更新
				this.UpdateSelectSellItemList();
				// ページ内リストの更新
				this.UpdateItemList();
			}
		}
		#endregion

		#region クリア
		/// <summary>
		/// 売却リストクリア
		/// </summary>
		private void ClearSellItemList()
		{
			if (this.SellItemList == null) { return; }
			this.SellItemList.ClearItem();
			this.SellItemList.UpdateScroll();

			// 売却リストの選択アイテム更新
			this.UpdateSelectSellItemList();
		}
		#endregion

		#region 選択
		/// <summary>
		/// 売却リストの選択アイテム更新
		/// </summary>
		private void UpdateSelectSellItemList()
		{
			if (this.SellItemList == null) { return; }
			List<GUIItem> itemList = this.SellItemList.GetNowPageList();
			int nextSetIndex = this.SellItemList.GetItemList().Count;

			for(int index = 0; index < itemList.Count; ++index)
			{
				bool isSelect = false;
				if(index == nextSetIndex)
				{
					isSelect = true;
				}
				itemList[index].SetSelect(isSelect);
			}
		}
		#endregion

		#region アイテム押下イベント
		/// <summary>
		/// 売却リスト内のアイテムが押された時のハンドラー
		/// </summary>
		private void HandleSellListItemClick(GUIItem item)
		{
			if (item == null || item.GetItemInfo() == null) { return; }
			ItemInfo itemInfo = item.GetItemInfo();
			if(itemInfo.Index > 0)
			{
				// アイテムが追加されていたら削除する
				this.RemoveSellItem(itemInfo);
			}
		}
		#endregion

		#region 同期
		/// <summary>
		/// 売却リスト内のアイテムが全て更新された時のハンドラー
		/// </summary>
		private void HandleSellListUpdateItems()
		{
			this.SyncSellList();
		}

		/// <summary>
		/// 売却リスト同期
		/// </summary>
		private void SyncSellList()
		{
			// 試算
			this.MultiSellCalc();
			this.SyncHaveMoney();
			// 実行ボタン有効設定更新
			this.UpdateExecuteButtonEnable();
		}
		#endregion

		#region 取得
		/// <summary>
		/// 売却アイテムのインデックスリストを取得する
		/// </summary>
		private List<int> GetSellItemIndexList()
		{
			var indexList = new List<int>();
			if (this.SellItemList == null) { return indexList; }

			List<ItemInfo> infoList = this.SellItemList.GetItemInfoList();
			if(infoList != null)
			{
				infoList.ForEach(t => { indexList.Add(t.Index); });
			}

			return indexList;
		}
		#endregion
		#endregion

		#region Boxモード
		#region BOXモード設定処理
		/// <summary>
		/// BOXモードの設定
		/// </summary>
		private void SetMode(ModeType mode)
		{
			// 更新
			this.Mode = mode;

			foreach(KeyValuePair<ModeType, Action<bool>> kvp in this.changeModeExecuteDic)
			{
				bool isChange = false;
				if(kvp.Key == mode)
				{
					// 切り替る項目と一致していたら切替フラグをONにする
					isChange = true;
				}
				kvp.Value(isChange);
			}

			if (this.CanUpdate)
			{
				// モードによって表示物を切り替える
				if (mode == ModeType.SellMulti)
				{
					this.View.SetMultiSellGroupActive(true);
					this.View.SetSelectGroupActive(false);
				}
				else
				{
					this.View.SetMultiSellGroupActive(false);
					this.View.SetSelectGroupActive(true);
				}
			}

			// メッセージ更新
			this.UpdateMessage();
		}
		/// <summary>
		/// BOXモードの切り替え
		/// </summary>
		private void ChangeMode(ModeType mode)
		{
			// 同モードが指定されている場合は切替処理を行わない
			if (this.Mode == mode) { return; }
			// モード切替
			SetMode(mode);
		}

		/// <summary>
		/// アイテム選択モード切替処理
		/// </summary>
		private void ChangeSelectMode()
		{
			if (!this.CanUpdate) { return; }

			// 売却リスト初期化
			this.ClearSellItemList();
			// ページリスト内アイテムリスト更新
			this.UpdateItemList();

			// 全体の表示の切り替え
			this.View.SetMultiSellGroupActive(false);
			this.View.SetSelectGroupActive(true);
		}
		/// <summary>
		/// まとめて売却モード切替処理
		/// </summary>
		private void ChangeMultiSellMode()
		{
			if (!this.CanUpdate) { return; }

			// ベースアイテム外す
			this.ClearBaseItem();
			// ページリスト内アイテムリスト更新
			this.UpdateItemList();

			// 全体の表示の切り替え
			this.View.SetSelectGroupActive(false);
			this.View.SetMultiSellGroupActive(true);
		}

		#region 新
		/// <summary>
		/// アイテム使用モード処理
		/// </summary>
		private void ItemUseMode(bool isChange)
		{
			if (!this.CanUpdate) { return; }

			if (isChange)
			{
				// ベースアイテム外す
				this.ClearBaseItem();
				// 売却リスト初期化
				this.ClearSellItemList();
				// ページリスト内アイテムリスト更新
				this.UpdateItemList();

				// 実行ボタン名セット
				this.View.SetExecuteLabel(MasterData.GetText(TextType.TX354_ItemBox_ItemUse));
			}

			// ボタンの表示の切り替え
			this.View.SetItemUseModeEnable(isChange);
		}

		/// <summary>
		/// 売却モード処理
		/// </summary>
		private void SellMode(bool isChange)
		{
			if (!this.CanUpdate) { return; }

			if (isChange)
			{
				// ベースアイテム外す
				this.ClearBaseItem();
				// 売却リスト初期化
				this.ClearSellItemList();
				// ページリスト内アイテムリスト更新
				this.UpdateItemList();

				// 実行ボタン名セット
				this.View.SetExecuteLabel(MasterData.GetText(TextType.TX359_ItemBox_Sell));
			}

			// ボタンの表示の切り替え
			this.View.SetSellModeEnable(isChange);
		}

		/// <summary>
		/// まとめて売却モード処理
		/// </summary>
		private void SellMultiMode(bool isChange)
		{
			if (!this.CanUpdate) { return; }

			if (isChange)
			{
				// ベースアイテム外す
				this.ClearBaseItem();
				// ページリスト内アイテムリスト更新
				this.UpdateItemList();
				
				// 実行ボタン名セット
				this.View.SetExecuteLabel(MasterData.GetText(TextType.TX353_ItemBox_MultiSell));
			}

			// ボタンの表示の切り替え
			this.View.SetSellMultiModeEnable(isChange);
		}
		#endregion
		#endregion

		#region モードボタン押下イベント
		/// <summary>
		/// アイテム使用モードボタン押下イベントハンドラー
		/// </summary>
		private void HandleItemUseMode(object sender, EventArgs e)
		{
			// アイテム使用モードに切り替える
			this.ChangeMode(ModeType.ItemUse);
		}

		/// <summary>
		/// 売却モードボタン押下イベントハンドラー
		/// </summary>
		private void HandleSellMode(object sender, EventArgs e)
		{
			// 売却モードに切り替える
			this.ChangeMode(ModeType.Sell);
		}

		/// <summary>
		/// まとめて売却ボタン押下イベントハンドラー
		/// </summary>
		private void HandleSellMultiMode(object sender, EventArgs e)
		{
			// まとめて売却モードに切り替える
			this.ChangeMode(ModeType.SellMulti);
		}

		/// <summary>
		/// キャンセルボタン押下イベントハンドラー
		/// </summary>
		private void HandleCancel(object sender, EventArgs e)
		{
			// アイテム選択モードに切り替える
			this.ChangeMode(ModeType.ItemUse);
		}
		#endregion
		#endregion

		#region まとめて売却
		#region まとめて売却試算
		/// <summary>
		/// まとめて売却の試算処理
		/// </summary>
		private void MultiSellCalc()
		{
			if (this.SellItemList == null) { return; }

			int totalSoldPrice = 0;
			// 売却アイテムリスト取得
			List<ItemInfo> sellInfoList = this.SellItemList.GetItemInfoList();
			foreach(var info in sellInfoList)
			{
				if (info == null) { continue; }

				// アイテムマスターデータ取得
				ItemMasterData masterData;
				if(this.TryGetItemMasterData(info.ItemMasterID, out masterData))
				{
					// 売却額計算
					totalSoldPrice += masterData.SellGameMoney * info.Stack;
				}
			}

			// データセット
			this.Model.TotalSoldPrice = totalSoldPrice;
		}
		#endregion

		#region 売却処理
		/// <summary>
		/// まとめて売却処理
		/// </summary>
		private void SellMulti()
		{
			if (this.SellItemList == null) { return; }

			// 売却リストにアイテムがセットされているかチェック
			if (this.GetSellItemIndexList().Count <= 0) { return; }

			// 売却チェック処理へ
			this.CheckSell();

		}
		/// <summary>
		/// 売却チェック
		/// </summary>
		private void CheckSell()
		{
			GUIMessageWindow.SetModeYesNo(MasterData.GetText(TextType.TX416_ItemBox_CheckSell), true, this.CheckHaveMoney, null);
		}
		/// <summary>
		/// 所持金チェック処理
		/// </summary>
		private void CheckHaveMoney()
		{
			if (!this.CanUpdate) { return; }
			if (this.Model.HaveMoney >= MasterDataCommonSetting.Player.PlayerMaxGameMoney)
			{
				// 所持金が上限に達しているので確認メッセージを表示
				GUIMessageWindow.SetModeYesNo(MasterData.GetText(TextType.TX347_SellItem_HaveMoneyOver), true, this.SellMultiExecute, null);
			}
			else
			{
				// まとめて売却実行
				this.SellMultiExecute();
			}
		}
		/// <summary>
		/// まとめて売却実行
		/// </summary>
		private void SellMultiExecute()
		{
			// 通知
			var eventArgs = new SellMultiEventArgs();
			eventArgs.IndexList = this.GetSellItemIndexList();

			this.OnSellMulti(this, eventArgs);
		}

		/// <summary>
		/// まとめて売却結果
		/// </summary>
		public void SellMultiResult(bool result, List<int> indexList, int money, int soldPrice)
		{
			if (!result) { return; }

			// 売却リストクリア
			this.ClearSellItemList();
			// ページ内アイテム更新
			this.UpdateItemList();

			// 売却額リセット
			this.Model.TotalSoldPrice = 0;
		}
		#endregion
		#endregion

		#region 売却
		/// <summary>
		/// 売却画面表示処理
		/// </summary>
		private void OpenSell()
		{
			if (!this.CanUpdate || this.IsEmptyBaseItem) { return; }

			// 売却データセット
			var param = new ItemSellSimple.SetupParam();
			param.ItemInfo = this.BaseItemInfo;
			// 売却するアイテム数取得
			var indexItemInfoDic = new Dictionary<int, ItemInfo>();
			this.Model.TryGetItemInfoByMasterId(this.BaseItemMasterID, out indexItemInfoDic);
			foreach(var info in indexItemInfoDic.Values)
			{
				if (info == null) { continue; }
				param.HaveItemCount += info.Stack;
			}

			// 画面表示
			var screen = new GUISingle(() => { GUIItemSellSimple.Open(param); }, GUIItemSellSimple.Close);
			GUIController.SingleOpen(screen);
		}
		#endregion

		#region 実行ボタン
		/// <summary>
		/// 実行ボタンが押された時のイベントハンドラー
		/// </summary>
		private void HandleExecute(object sender, EventArgs e)
		{
			switch(this.Mode)
			{
				case ModeType.ItemUse:
					break;
				case ModeType.Sell:
					// 売却画面表示
					this.OpenSell();
					break;
				case ModeType.SellMulti:
					// まとめて売却処理
					this.SellMulti();
					break;
			}
		}

		/// <summary>
		/// 実行ボタン有効設定
		/// </summary>
		private void UpdateExecuteButtonEnable()
		{
			if (!this.CanUpdate) { return; }

			bool isEnable = false;
			switch(this.Mode)
			{
				case ModeType.ItemUse:
				case ModeType.Sell:
				{
					if (!this.IsEmptyBaseItem && !this.BaseItemInfo.IsLock)
					{
						isEnable = true;
					}
					break;
				}
				case ModeType.SellMulti:
				{
					if (this.GetSellItemIndexList().Count > 0)
					{
						isEnable = true;
					}
					break;
				}
			}

			// ボタン有効設定
			this.View.SetExecuteButtonEnable(isEnable);
		}
		#endregion

		#region 所持金
		/// <summary>
		/// 所持金変更ハンドラー
		/// </summary>
		private void HandleHaveMoneyChange(object sender, EventArgs e)
		{
			this.SyncHaveMoney();
		}
		/// <summary>
		/// 所持金フォーマット変更ハンドラー
		/// </summary>
		private void HandleHaveMoneyFormatChange(object sender, EventArgs e)
		{
			this.SyncHaveMoney();
		}

		/// <summary>
		/// 所持金同期
		/// </summary>
		private void SyncHaveMoney()
		{
			if (!this.CanUpdate) { return; }
			this.View.SetHaveMoney(this.Model.HaveMoney, this.Model.HaveMoneyFormat);
		}
		#endregion

		#region 総売却額
		/// <summary>
		/// 総売却額変更ハンドラー
		/// </summary>
		private void HandleTotalSoldPriceChange(object sender, EventArgs e)
		{
			this.SyncTotalSoldPrice();
		}
		/// <summary>
		/// 総売却額フォーマット変更ハンドラー
		/// </summary>
		private void HandleTotalSoldPriceFormatChange(object sender, EventArgs e)
		{
			this.SyncTotalSoldPrice();
		}
		/// <summary>
		/// 総売却額同期
		/// </summary>
		private void SyncTotalSoldPrice()
		{
			if (!this.CanUpdate) { return; }
			this.View.SetTotalSoldPrice(this.Model.TotalSoldPrice, this.Model.TotalSoldPriceFormat);
		}
		#endregion

		#region ホーム、閉じるボタンイベント
		/// <summary>
		/// ホーム
		/// </summary>
		private void HandleHome(object sender, HomeClickedEventArgs e)
		{
			if(this.ItemList != null)
			{
				// Newフラグ一括解除
				this.ItemList.DeleteAllNewFlag();
			}

			GUIController.Clear();
		}

		/// <summary>
		/// 閉じる
		/// </summary>
		private void HandleClose(object sender, CloseClickedEventArgs e)
		{
			if (this.ItemList != null)
			{
				// Newフラグ一括解除
				this.ItemList.DeleteAllNewFlag();
			}

			GUIController.Back();
		}
		#endregion

		#region マスターデータ
		/// <summary>
		/// アイテムマスターデータ取得
		/// 取得失敗時はエラーログを出力
		/// </summary>
		private bool TryGetItemMasterData(int masterId, out ItemMasterData masterData)
		{
			if(!MasterData.TryGetItem(masterId, out masterData))
			{
				// 取得失敗
				string msg = string.Format("ItemMasterData NotFound. ID = {0}", masterId);
				//BugReportController.SaveLogFile(msg);
				UnityEngine.Debug.LogWarning(msg);
				return false;
			}

			return true;
		}
		#endregion
	}
}
