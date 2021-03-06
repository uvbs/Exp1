/// <summary>
/// キャラスロットリスト制御
/// 
/// 2016/05/24
/// </summary>
using System;
using System.Collections.Generic;

namespace XUI.CharaList
{
	/// <summary>
	/// キャラスロットリスト制御インターフェイス
	/// </summary>
	public interface ICharaSlotListController
	{
		#region 更新チェック
		/// <summary>
		/// 更新できる状態かどうか
		/// </summary>
		bool CanUpdate { get; }
		#endregion

		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();
		#endregion

		#region アイテムとスロット数
		/// <summary>
		/// アイテムの総数
		/// </summary>
		int Capacity { get; }

		/// <summary>
		/// スロット解放数
		/// </summary>
		int SlotCount { get; }
		#endregion

		#region アイテムの総数設定
		/// <summary>
		/// アイテム総数設定
		/// </summary>
		void SetupCapacity(int capacity, int slotCount);
		#endregion

		#region キャラアイテム設定
		/// <summary>
		/// キャラアイテム設定
		/// </summary>
		void SetupItems(List<CharaInfo> charaInfoList);
		#endregion

		#region キャラ追加
		/// <summary>
		/// 空スロット枠にキャラを追加する
		/// </summary>
		bool AddChara(CharaInfo charaInfo);
		#endregion

		#region キャラ削除
		/// <summary>
		/// スロットに刺さっているキャラを外す
		/// </summary>
		bool RemoveChara(CharaInfo charaInfo);
		#endregion

		#region クリア
		/// <summary>
		/// スロットキャラ情報のみクリア
		/// </summary>
		bool ClearChara();

		/// <summary>
		/// スロット情報クリア
		/// </summary>
		void ClearSlot();
		#endregion

		#region キャラ情報取得
		/// <summary>
		/// 現ページ内のキャラアイテムリストを返す
		/// </summary>
		List<GUICharaItem> GetNowPageItemList();

		/// <summary>
		/// スロットに刺さっているキャラ情報リストを取得する
		/// </summary>
		List<CharaInfo> GetCharaInfoList();
		#endregion

		#region テーブル整形
		/// <summary>
		/// テーブル整形
		/// </summary>
		void Reposition();
		#endregion

		#region アイテムボタン有効設定
		/// <summary>
		/// ページ内の全アイテムのボタン有効設定
		/// </summary>
		void SetItemsButtonEnable(bool isEnable);
		#endregion

		#region イベント
		/// <summary>
		/// 登録されているキャラアイテムが押された時のイベント通知用
		/// </summary>
		event EventHandler<ItemClickEventArgs> OnItemClickEvent;

		/// <summary>
		/// 登録されているキャラアイテムが長押しされた時のイベント通知用
		/// </summary>
		event EventHandler<ItemClickEventArgs> OnItemLongPressEvent;

		/// <summary>
		/// 登録されているキャラアイテムに変更があった時のイベント通知用
		/// </summary>
		event EventHandler<ItemChangeEventArgs> OnItemChangeEvent;

		/// <summary>
		/// 全てのアイテムが更新された時のイベント通知
		/// </summary>
		event EventHandler OnUpdateItemsEvent;
		#endregion
	}

	/// <summary>
	/// キャラスロットリスト制御
	/// </summary>
	public class CharaSlotListController : ICharaSlotListController
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
		/// 登録されているキャラアイテムが押された時のイベント通知用
		/// </summary>
		public event EventHandler<ItemClickEventArgs> OnItemClickEvent = (sender, e) => { };

		/// <summary>
		/// 登録されているキャラアイテムが長押しされた時のイベント通知用
		/// </summary>
		public event EventHandler<ItemClickEventArgs> OnItemLongPressEvent = (sender, e) => { };

		/// <summary>
		/// 登録されているキャラアイテムに変更があった時のイベント通知用
		/// </summary>
		public event EventHandler<ItemChangeEventArgs> OnItemChangeEvent = (sender, e) => { };

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
				return true;
			}
		}

		/// <summary>
		/// アイテムの総数
		/// </summary>
		public int Capacity
		{
			get
			{
				if (!this.CanUpdate) { return 0; }
				return this.Model.Capacity;
			}
		}

		/// <summary>
		/// スロット解放数
		/// </summary>
		private int _slotCount = 0;
		public int SlotCount { get { return _slotCount; } private set { _slotCount = value; } }
		#endregion

		#region 初期化
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CharaSlotListController(IModel model, IView view)
		{
			if (model == null || view == null) { return; }

			// ビュー設定
			this._view = view;

			// モデル設定
			this._model = model;
			this.Model.OnCapacityChange += this.HandleCapacityChange;
			this.Model.OnCharaInfoListChange += this.HandleCharaInfoListChange;
			this.Model.OnAddCharaInfoChange += this.HandleAddOwnCharaInfoChange;
			this.Model.OnRemoveCharaInfoChange += this.HandleRemoveOwnCharaInfoChange;
			this.Model.OnClearCharaInfoChange += this.HandleClearCharaInfoChange;

			// ページ付スクロールビューを初期化
			this.Model.ItemScrollView.Create(this.View.PageScrollViewAttach, null);
			this.Reposition();

			// 同期
			this.SyncCharaInfoList();
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
			this.OnItemLongPressEvent = null;
			this.OnItemChangeEvent = null;
			this.OnUpdateItemsEvent = null;
		}
		#endregion

		#region アイテムの総数設定
		/// <summary>
		/// アイテム総数設定
		/// </summary>
		public void SetupCapacity(int capacity, int slotCount)
		{
			if (!this.CanUpdate) { return; }
			int beforeCapacity = this.Model.Capacity;
			int beforeSlotCount = this.SlotCount;

			this.SlotCount = slotCount;
			this.Model.Capacity = capacity;;

			if(beforeCapacity == capacity && beforeSlotCount != slotCount)
			{
				// アイテム総数に変更がなくスロット解放数に変更があった場合はキャラ情報を同期させる
				this.SyncCharaInfoList();
			}
		}

		/// <summary>
		/// アイテム総数変更イベントハンドラー
		/// </summary>
		private void HandleCapacityChange(object sender, EventArgs e)
		{
			SyncCapacity();
		}

		/// <summary>
		/// アイテム総数同期
		/// </summary>
		private void SyncCapacity()
		{
			if (!CanUpdate) { return; }
			// ページスクロールビューの総数セット
			this.Model.ItemScrollView.Setup(this.Model.Capacity, -1);
			// デーブル整形
			this.Reposition();
		}
		#endregion

		#region キャラアイテム設定
		/// <summary>
		/// キャラアイテム設定
		/// </summary>
		public void SetupItems(List<CharaInfo> charaInfoList)
		{
			if (!this.CanUpdate) { return; }

			// キャラ情報クリア
			this.Model.CharaInfoList.Clear();

			// 追加可能なキャラかどうかチェック
			List<CharaInfo> slotCharaInfoList = new List<CharaInfo>();
			foreach(var info in charaInfoList)
			{
				if(this.IsAddChara(info))
				{
					// 追加
					slotCharaInfoList.Add(info);
				}
			}

			// セット
			this.Model.SetCharaInfoList(slotCharaInfoList);
		}

		/// <summary>
		/// キャラアイテム設定変更イベントハンドラー
		/// </summary>
		private void HandleCharaInfoListChange(object sender, EventArgs e)
		{
			this.SyncCharaInfoList();
		}
		#endregion

		#region キャラ追加
		/// <summary>
		/// 空スロット枠にキャラを追加する
		/// </summary>
		public bool AddChara(CharaInfo charaInfo)
		{
			if (!this.CanUpdate) { return false; }

			// 追加可能状態かチェック
			if (!this.IsAddChara(charaInfo)) { return false; }
			// 追加
			this.Model.AddCharaInfo(charaInfo);

			return true;
		}

		/// <summary>
		/// キャラ情報追加イベントハンドラー
		/// </summary>
		private void HandleAddOwnCharaInfoChange(object sender, EventArgs e)
		{
			SyncCharaInfoList();
		}

		/// <summary>
		/// キャラが追加できるかどうかチェック
		/// </summary>
		private bool IsAddChara(CharaInfo charaInfo)
		{
			if (charaInfo == null) { return false; }
			// オーバーしていないかチェック
			if (this.Model.CharaInfoList.Count >= this.SlotCount) { return false; }
			// 同キャラが含まれていないかチェック
			if(this.Model.CharaInfoList.Exists((info) => { return info.UUID == charaInfo.UUID; }))
			{
				return false;
			}

			return true;
		}
		#endregion

		#region キャラ削除
		/// <summary>
		/// スロットに刺さっているキャラを外す
		/// </summary>
		public bool RemoveChara(CharaInfo charaInfo)
		{
			if (!this.CanUpdate) { return false; }

			// 削除するキャラ情報検索
			CharaInfo removeInfo = null;
			foreach (var info in this.Model.CharaInfoList)
			{
				if (info == null) continue;
				if (charaInfo.UUID == info.UUID)
				{
					removeInfo = info;
					break;
				}
			}

			// 削除
			return this.Model.RemoveCharaInfo(removeInfo);
		}

		/// <summary>
		/// キャラ情報削除イベントハンドラー
		/// </summary>
		private void HandleRemoveOwnCharaInfoChange(object sender, EventArgs e)
		{
			SyncCharaInfoList();
		}
		#endregion

		#region クリア
		/// <summary>
		/// スロットキャラ情報のみクリア
		/// </summary>
		public bool ClearChara()
		{
			if (!this.CanUpdate) { return false; }

			// キャラ情報がセットされているか検索
			bool isExecute = false;
			foreach (var info in this.Model.CharaInfoList)
			{
				if (info != null)
				{
					// ひとつでもキャラ情報が存在するならクリア処理を行う
					isExecute = true;
					break;
				}
			}

			if (isExecute)
			{
				this.Model.ClearCharaInfo();
			}

			return isExecute;
		}

		/// <summary>
		/// スロット情報クリア
		/// </summary>
		public void ClearSlot()
		{
			if (!this.CanUpdate) { return; }

			// 解放数リセット
			this.SlotCount = 0;
			// スロットキャラ情報クリア
			this.Model.ClearCharaInfo();
		}

		/// <summary>
		/// キャラ情報リストクリアイベントハンドラー
		/// </summary>
		private void HandleClearCharaInfoChange(object sender, EventArgs e)
		{
			SyncCharaInfoList();
		}
		#endregion

		#region キャラ情報取得
		/// <summary>
		/// 現ページ内のキャラアイテムリストを返す
		/// </summary>
		public List<GUICharaItem> GetNowPageItemList()
		{
			if (!this.CanUpdate)
			{
				return new List<GUICharaItem>();
			}
			return this.Model.GetNowPageItemList();
		}

		/// <summary>
		/// スロットに刺さっているキャラ情報リストを取得する
		/// </summary>
		public List<CharaInfo> GetCharaInfoList()
		{
			if (this.Model == null)
			{
				return new List<CharaInfo>();
			}

			List<CharaInfo> infoList = new List<CharaInfo>();
			foreach (var info in this.Model.CharaInfoList)
			{
				if (info == null) { continue; }
				infoList.Add(info);
			}

			return infoList;
		}
		#endregion

		#region キャラ情報更新
		/// <summary>
		/// キャラ情報同期
		/// </summary>
		private void SyncCharaInfoList()
		{
			UpdateItems();
		}

		/// <summary>
		/// 現ページ内に表示されているアイテムを更新する
		/// </summary>
		private void UpdateItems()
		{
			if (!CanUpdate) { return; }

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
				GUICharaItem item = this.Model.ItemScrollView.GetItem(itemIndex);
				if (item == null) { continue; }

				// 前回分のイベントは削除しておく
				item.OnItemClickEvent -= OnItemClick;
				item.OnItemLongPressEvent -= OnItemLongPress;
				item.OnItemChangeEvent -= OnItemChange;
				// アイテムボタンイベント登録
				item.OnItemClickEvent += OnItemClick;
				item.OnItemLongPressEvent += OnItemLongPress;
				item.OnItemChangeEvent += OnItemChange;

				if(this.SlotCount <= indexInTotal)
				{
					// スロット解放数を超えた分はフレーム枠とする
					item.SetState(CharaItem.Controller.ItemStateType.Frame, null);
				}
				else if(this.Model.CharaInfoList.Count <= indexInTotal)
				{
					// スロット空アイコン枠
					item.SetState(CharaItem.Controller.ItemStateType.FillEmpty, null);
				}
				else
				{
					// キャラが埋まっているスロット枠
					CharaInfo charaInfo = this.Model.CharaInfoList[indexInTotal];
					item.SetState(CharaItem.Controller.ItemStateType.Icon, charaInfo);
				}
			}

			// イベント通知
			this.OnUpdateItemsEvent(this, EventArgs.Empty);
		}
		#endregion

		#region テーブル整形
		/// <summary>
		/// テーブル整形
		/// </summary>
		public void Reposition()
		{
			if (!this.CanUpdate) { return; }
			this.Model.ItemScrollView.Reposition();
			this.Model.ItemScrollView.ScrollReset();
		}
		#endregion

		#region アイテムボタン有効設定
		/// <summary>
		/// ページ内の全アイテムのボタン有効設定
		/// </summary>
		public void SetItemsButtonEnable(bool isEnable)
		{
			if (!this.CanUpdate) { return; }
			foreach (var item in this.Model.GetPageItemList())
			{
				if (item == null) { continue; }
				item.SetButtonEnable(isEnable);
			}
		}
		#endregion

		#region アイテムイベント
		/// <summary>
		/// アイテムが押された時に呼び出される
		/// </summary>
		private void OnItemClick(GUICharaItem item)
		{
			// 通知
			var eventArgs = new ItemClickEventArgs(item);
			this.OnItemClickEvent(this, eventArgs);
		}

		/// <summary>
		/// アイテムが長押しされた時に呼び出される
		/// </summary>
		private void OnItemLongPress(GUICharaItem item)
		{
			// 通知
			var eventArgs = new ItemClickEventArgs(item);
			this.OnItemLongPressEvent(this, eventArgs);
		}

		/// <summary>
		/// アイテムに変更があった時に呼び出される
		/// </summary>
		/// <param name="item"></param>
		private void OnItemChange(GUICharaItem item)
		{
			// 通知
			var eventArgs = new ItemChangeEventArgs(item);
			this.OnItemChangeEvent(this, eventArgs);
		}
		#endregion
	}

}
