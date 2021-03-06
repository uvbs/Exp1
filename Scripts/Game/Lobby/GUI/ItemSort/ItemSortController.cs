/// <summary>
/// アイテムソート制御
/// 
/// 2016/04/11
/// </summary>
using System;
using System.Collections.Generic;

namespace XUI.ItemSort
{
	/// <summary>
	/// ソート項目タイプ
	/// </summary>
	public enum SortPatternType
	{
		None,					// 設定なし
		Name,					// 名前
		Type,					// 種類
		Obtaining,				// 入手
	}

	#region イベント引数
	/// <summary>
	/// OKボタンイベント通知時の引数
	/// </summary>
	public class OKClickEventArgs : EventArgs
	{
		private readonly SortPatternType _sortPattern;
		public SortPatternType SortPattern { get { return _sortPattern; } }

		private readonly bool _isAscend;
		public bool IsAscend { get { return _isAscend; } }

		public OKClickEventArgs(SortPatternType sortPattern, bool isAscend)
		{
			this._sortPattern = sortPattern;
			this._isAscend = isAscend;
		}
	}
	#endregion

	/// <summary>
	/// アイテムソート制御インターフェイス
	/// </summary>
	public interface IController
	{
		#region 更新チェック
		/// <summary>
		/// 更新できる状態かどうか
		/// </summary>
		bool CanUpdate { get; }
		#endregion

		#region アクティブ設定
		/// <summary>
		/// アクティブを設定する
		/// </summary>
		void SetActive(bool isActive, bool isTweenSkip);
		#endregion

		#region 初期化
		/// <summary>
		/// データセットアップ処理
		/// </summary>
		void Setup(SortPatternType pattern, bool isAscend);

		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();
		#endregion

		#region ソート項目
		/// <summary>
		/// 現在有効状態になっているソート項目
		/// </summary>
		SortPatternType SortPattern { get; }

		/// <summary>
		/// 現在有効状態になっているソート項目が変化された時の通知
		/// </summary>
		event EventHandler OnSortPatternChangeEvent;
		#endregion

		#region 昇順/降順
		/// <summary>
		/// 昇順か降順で並び替えるか
		/// </summary>
		bool IsAscend { get; }
		#endregion

		#region OKボタン
		/// <summary>
		/// OKボタンを押した時のイベント通知
		/// </summary>
		event EventHandler<OKClickEventArgs> OKClickEvent;
		#endregion
	}

	/// <summary>
	/// アイテムソート制御
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
		/// ソート項目変更イベント
		/// </summary>
		public event EventHandler OnSortPatternChangeEvent = (sender, e) => { };

		/// <summary>
		/// 現在有効状態になっているソート項目
		/// </summary>
		private SortPatternType _sortPattern = SortPatternType.None;
		public SortPatternType SortPattern
		{
			get { return this._sortPattern; }
			private set
			{
				if (this._sortPattern != value)
				{
					this._sortPattern = value;

					// 通知
					this.OnSortPatternChangeEvent(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// 昇順か降順で並び替えるか
		/// </summary>
		public bool IsAscend
		{
			get
			{
				if (!this.CanUpdate) { return false; }
				return this.Model.IsAscend;
			}
		}

		/// <summary>
		/// OKボタンを押した時のイベント通知
		/// </summary>
		public event EventHandler<OKClickEventArgs> OKClickEvent = (seder, e) => { };
		#endregion

		#region 初期化
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Controller(IModel model, IView view)
		{
			if (model == null || view == null) { return; }

			// ビュー設定
			this._view = view;
			this.View.OnCloseClickEvent += this.HandleCloseClickEvent;
			this.View.OnNameClickEvent += this.HandleNameClickEvent;
			this.View.OnTypeClickEvent += this.HandleTypeClickEvent;
			this.View.OnObtainingClickEvent += this.HandleObtainingClickEvent;
			this.View.OnAscendClickEvent += this.HandleAscendClickEvent;
			this.View.OnDescendClickEvent += this.HandleDescendClickEvent;
			this.View.OnOkClickEvent += this.HandleOkClickEvent;
			
			// モデル設定
			this._model = model;
			this.Model.OnIsAscendChange += this.HandleIsAscendChange;

			// ソート項目初期化
			this.InitSortPattern();

			// 同期
			this.SyncIsAscend();
		}

		/// <summary>
		/// データセットアップ処理
		/// </summary>
		public void Setup(SortPatternType pattern, bool isAscend)
		{
			if (!this.CanUpdate) { return; }

			this.SetupSortPattern(pattern);
			this.Model.IsAscend = isAscend;
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

			this.OnSortPatternChangeEvent = null;
			this.OKClickEvent = null;
		}
		#endregion

		#region アクティブ設定
		/// <summary>
		/// アクティブを設定する
		/// </summary>
		public void SetActive(bool isActive, bool isTweenSkip)
		{
			if (!this.CanUpdate) { return; }
			this.View.SetActive(isActive, isTweenSkip);

			// 同期
			//this.SetupSortPattern(this.SortPattern);
			//this.SyncIsAscend();
			//this.SyncIsSelectDisable();
		}
		#endregion

		#region 閉じるボタン
		/// <summary>
		/// 閉じるボタンが押された時に呼び出される
		/// </summary>
		private void HandleCloseClickEvent(object sender, EventArgs e)
		{
			// 閉じる
			GUIController.SingleClose();
		}
		#endregion

		#region ソート項目
		/// <summary>
		/// ソート項目有効設定リスト
		/// </summary>
		private Dictionary<SortPatternType, Action<bool>> setPatternEnableList = new Dictionary<SortPatternType, Action<bool>>();

		/// <summary>
		/// 一時フラグ(OKボタンが押されるまでに変化されたフラグを保持しておくデータ)
		/// </summary>
		private SortPatternType tempSortPattern = SortPatternType.Name;

		/// <summary>
		/// ソート項目の初期化
		/// </summary>
		private void InitSortPattern()
		{
			if (!this.CanUpdate) { return; }

			// ソート項目有効設定リストを作成
			this.setPatternEnableList.Clear();
			this.setPatternEnableList.Add(SortPatternType.Name, this.View.SetNameEnable);
			this.setPatternEnableList.Add(SortPatternType.Type, this.View.SetTypeEnable);
			this.setPatternEnableList.Add(SortPatternType.Obtaining, this.View.SetObtainingEnable);
		}

		/// <summary>
		/// ソート項目のセットアップ
		/// </summary>
		private void SetupSortPattern(SortPatternType pattern)
		{
			this.ChangeSortPattern(pattern, false);
		}

		/// <summary>
		/// ソート項目切替処理
		/// </summary>
		private void ChangeSortPattern(SortPatternType pattern, bool isUpdateTemp)
		{
			// 有効設定
			foreach (KeyValuePair<SortPatternType, Action<bool>> kvp in this.setPatternEnableList)
			{
				bool isEnable = false;
				if (kvp.Key == pattern)
				{
					// 切り替る項目と一致していたら有効状態にする
					isEnable = true;
				}
				kvp.Value(isEnable);
			}

			if (isUpdateTemp)
			{
				// 一時データのみ更新
				this.tempSortPattern = pattern;
			}
			else
			{
				// 現項目と一時用データを更新
				this.SortPattern = pattern;
				this.tempSortPattern = pattern;
			}
		}

		#region イベント
		/// <summary>
		/// 名前ボタンが押された時に呼び出される
		/// </summary>
		private void HandleNameClickEvent(Object sender, EventArgs e)
		{
			this.ChangeSortPattern(SortPatternType.Name, true);
		}
		/// <summary>
		/// 種類ボタンが押された時に呼び出される
		/// </summary>
		private void HandleTypeClickEvent(Object sender, EventArgs e)
		{
			this.ChangeSortPattern(SortPatternType.Type, true);
		}
		/// <summary>
		/// 入手ボタンが押された時に呼び出される
		/// </summary>
		private void HandleObtainingClickEvent(Object sender, EventArgs e)
		{
			this.ChangeSortPattern(SortPatternType.Obtaining, true);
		}
		#endregion
		#endregion

		#region 昇順/降順
		/// <summary>
		/// 一時フラグ(OKボタンが押されるまでに変化されたフラグを保持しておくデータ)
		/// </summary>
		private bool tempIsAscend = false;

		/// <summary>
		/// 昇順フラグに変化があった時に呼び出される
		/// </summary>
		private void HandleIsAscendChange(object sender, EventArgs e)
		{
			this.SyncIsAscend();
		}

		/// <summary>
		/// 昇順/降順同期
		/// </summary>
		private void SyncIsAscend()
		{
			if (!this.CanUpdate) { return; }
			this.View.SetAscendEnable(this.Model.IsAscend);
			this.View.SetDescendEnable(!this.Model.IsAscend);
			this.tempIsAscend = this.Model.IsAscend;
		}

		/// <summary>
		/// 昇順ボタンが押された時に呼ばれる
		/// </summary>
		private void HandleAscendClickEvent(object sender, EventArgs e)
		{
			if (!this.CanUpdate) { return; }
			// 一時フラグと表示切替
			this.tempIsAscend = true;
			this.View.SetAscendEnable(this.tempIsAscend);
			this.View.SetDescendEnable(!this.tempIsAscend);
		}
		/// <summary>
		/// 降順ボタンが押された時に呼び出される
		/// </summary>
		private void HandleDescendClickEvent(object sender, EventArgs e)
		{
			if (!this.CanUpdate) { return; }
			// 一時フラグと表示切替
			this.tempIsAscend = false;
			this.View.SetAscendEnable(this.tempIsAscend);
			this.View.SetDescendEnable(!this.tempIsAscend);
		}
		#endregion

		#region OKボタン
		/// <summary>
		/// OKボタンが押された時に呼び出される
		/// </summary>
		private void HandleOkClickEvent(object sender, EventArgs e)
		{
			if (!CanUpdate) { return; }

			// 一時データから元データに更新
			Setup(this.tempSortPattern, this.tempIsAscend);

			// 通知
			var eventArgs = new OKClickEventArgs(this.SortPattern, this.Model.IsAscend);
			this.OKClickEvent(this, eventArgs);
		}
		#endregion
	}
}