/// <summary>
/// アイテムBox表示
/// 
/// 2016/03/29
/// </summary>
using UnityEngine;
using System;
using System.Collections.Generic;

namespace XUI.ItemBox
{
	#region イベント引数
	public class HomeClickedEventArgs : EventArgs { }
	public class CloseClickedEventArgs : EventArgs { }
	#endregion

	/// <summary>
	/// アイテムBox表示インターフェイス
	/// </summary>
	public interface IView
	{
		#region ホーム/閉じる
		// ホーム、閉じるイベント通知用
		event EventHandler<HomeClickedEventArgs> OnHome;
		event EventHandler<CloseClickedEventArgs> OnClose;
		#endregion

		#region アクティブ
		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		void SetActive(bool isActive, bool isTweenSkip);

		/// <summary>
		/// 現在のアクティブ状態を取得する
		/// </summary>
		GUIViewBase.ActiveState GetActiveState();
		#endregion

		#region モード
		/// <summary>
		/// 選択モード時のオブジェクト表示設定
		/// </summary>
		void SetSelectGroupActive(bool isActive);

		/// <summary>
		/// まとめて選択モード時のオブジェクト表示設定
		/// </summary>
		void SetMultiSellGroupActive(bool isActive);

		#region アイテム使用モードボタン
		/// <summary>
		/// 使用モードボタンイベント
		/// </summary>
		event EventHandler OnItemUseMode;

		/// <summary>
		/// 使用モードの有効設定
		/// </summary>
		void SetItemUseModeEnable(bool isEnable);
		#endregion

		#region 売却モードボタン
		/// <summary>
		/// 売却モードボタンイベント
		/// </summary>
		event EventHandler OnSellMode;

		/// <summary>
		/// 売却モードボタンの有効設定
		/// </summary>
		void SetSellModeEnable(bool isEnable);
		#endregion

		#region まとめて売却モードボタン
		/// <summary>
		/// まとめて売却モードボタンイベント
		/// </summary>	
		event EventHandler OnSellMultiMode;

		/// <summary>
		/// まとめて売却モードの有効設定
		/// </summary>
		void SetSellMultiModeEnable(bool isEnable);
		#endregion
		#endregion

		#region 実行ボタン
		/// <summary>
		/// 実行ボタンイベント
		/// </summary>
		event EventHandler OnExecute;

		/// <summary>
		/// 実行ボタンの有効設定
		/// </summary>
		void SetExecuteButtonEnable(bool isEnable);

		/// <summary>
		/// 実行ボタンのラベル名をセットする
		/// </summary>
		void SetExecuteLabel(string name);
		#endregion

		#region 所持金
		/// <summary>
		/// 所持金設定
		/// </summary>
		void SetHaveMoney(int money, string format);
		#endregion

		#region 総売却額
		/// <summary>
		/// 総売却額の設定
		/// </summary>
		void SetTotalSoldPrice(int price, string format);
		#endregion

		#region アイテム情報
		/// <summary>
		/// アイテム名の設定
		/// </summary>
		void SetItemName(string name);

		/// <summary>
		/// アイテム説明の設定
		/// </summary>
		void SetItemDescription(string description);

		/// <summary>
		/// アイテムのロックボタンイベント
		/// </summary>
		event EventHandler OnLock;

		/// <summary>
		/// アイテムのロック設定
		/// </summary>
		void SetItemLock(bool isLock);

		/// <summary>
		/// アイテム個数の設定
		/// </summary>
		void SetItemCount(string count);

		/// <summary>
		/// アイテム売却額の設定
		/// </summary>
		void SetItemSoldPrice(int price, string format);
		#endregion
	}

	/// <summary>
	/// アイテムBox表示
	/// </summary>
	public class ItemBoxView : GUIScreenViewBase, IView
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void OnDestroy()
		{
			this.OnHome = null;
			this.OnClose = null;
			this.OnItemUseMode = null;
			this.OnSellMode = null;
			this.OnSellMultiMode = null;
		}
		#endregion

		#region アクティブ
		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		public void SetActive(bool isActive, bool isTweenSkip)
		{
			this.SetRootActive(isActive, isTweenSkip);
		}
		/// <summary>
		/// 現在のアクティブ状態を取得する
		/// </summary>
		public GUIViewBase.ActiveState GetActiveState()
		{
			return this.GetRootActiveState();
		}
		#endregion

		#region ホーム、閉じるボタンイベント
		/// <summary>
		/// ホーム、閉じるイベント通知用
		/// </summary>
		public event EventHandler<HomeClickedEventArgs> OnHome = (sender, e) => { };
		public event EventHandler<CloseClickedEventArgs> OnClose = (sender, e) => { };

		/// <summary>
		/// ホームボタンイベント
		/// </summary>
		public override void OnHomeEvent()
		{
			// 通知
			var eventArgs = new HomeClickedEventArgs();
			this.OnHome(this, eventArgs);
		}

		/// <summary>
		/// 閉じるボタンイベント
		/// </summary>
		public override void OnCloseEvent()
		{
			// 通知
			var eventArgs = new CloseClickedEventArgs();
			this.OnClose(this, eventArgs);
		}
		#endregion

		#region 所持金
		/// <summary>
		/// 選択時の所持金設定ラベル
		/// </summary>
		[SerializeField]
		private UILabel _haveMoneyLabel = null;
		private UILabel HaveMoneyLabel { get { return _haveMoneyLabel; } }
		/// <summary>
		/// まとめて売却時の所持金設定ラベル
		/// </summary>
		[SerializeField]
		private UILabel _multiSellHaveMoneyLabel = null;
		private UILabel MultiSellHaveMoneyLabel { get { return _multiSellHaveMoneyLabel; } }

		/// <summary>
		/// 所持金設定
		/// </summary>
		public void SetHaveMoney(int money, string format)
		{
			if (this.HaveMoneyLabel != null)
			{
				this.HaveMoneyLabel.text = string.Format(format, money);
			}
			if (this.MultiSellHaveMoneyLabel != null)
			{
				this.MultiSellHaveMoneyLabel.text = string.Format(format, money);
			}
		}
		#endregion

		#region 総売却額
		[SerializeField]
		private UILabel _totalSoldPriceLabel = null;
		private UILabel TotalSoldPriceLabel { get { return _totalSoldPriceLabel; } }

		/// <summary>
		/// 総売却額の設定
		/// </summary>
		public void SetTotalSoldPrice(int price, string format)
		{
			if(this.TotalSoldPriceLabel != null)
			{
				this.TotalSoldPriceLabel.text = string.Format(format, price);
			}
		}
		#endregion

		#region モード
		/// <summary>
		/// モードアタッチオブジェクト
		/// </summary>
		[SerializeField]
		private ModeAttachObject _modeAttach = null;
		private ModeAttachObject ModeAttach { get { return _modeAttach; } }
		[Serializable]
		private class ModeAttachObject
		{
			[SerializeField]
			private UIPlayTween _selectPlayTween = null;
			public UIPlayTween SelectPlayTween { get { return _selectPlayTween; } }

			[SerializeField]
			private UIPlayTween _multiSellPlayTween = null;
			public UIPlayTween MultiSellPlayTween { get { return _multiSellPlayTween; } }

			/// <summary>
			/// アイテム使用モードボタン
			/// </summary>
			[SerializeField]
			private UIButton _itemUseButton = null;
			public UIButton ItemUseButton { get { return _itemUseButton; } }

			/// <summary>
			/// 売却モードボタン
			/// </summary>
			[SerializeField]
			private UIButton _sellButton = null;
			public UIButton SellButton { get { return _sellButton; } }

			/// <summary>
			/// まとめて売却ボタン
			/// </summary>
			[SerializeField]
			private UIButton _sellMultiButton = null;
			public UIButton SellMultiButton { get { return _sellMultiButton; } }
		}

		/// <summary>
		/// 選択モード時のオブジェクト表示設定
		/// </summary>
		public void SetSelectGroupActive(bool isActive)
		{
			if (this.ModeAttach == null) { return; }
			if (this.ModeAttach.SelectPlayTween != null)
			{
				this.ModeAttach.SelectPlayTween.Play(isActive);
			}
		}

		/// <summary>
		/// まとめて選択モード時のオブジェクト表示設定
		/// </summary>
		public void SetMultiSellGroupActive(bool isActive)
		{
			if (this.ModeAttach == null) { return; }
			if (this.ModeAttach.MultiSellPlayTween != null)
			{
				this.ModeAttach.MultiSellPlayTween.Play(isActive);
			}
		}

		#region アイテム使用モードボタン
		/// <summary>
		/// 使用モードボタンイベント
		/// </summary>
		public event EventHandler OnItemUseMode = (sender, e) => { };

		/// <summary>
		/// 使用モードボタンクリックイベント(NGUIリフレクション)
		/// </summary>
		public void OnItemUseModeClickEvent()
		{
			// 通知
			this.OnItemUseMode(this, EventArgs.Empty);
		}

		/// <summary>
		/// 使用モード表示の有効設定
		/// </summary>
		public void SetItemUseModeEnable(bool isEnable)
		{
			if (this.ModeAttach == null || this.ModeAttach.ItemUseButton == null) { return; }
			this.ModeAttach.ItemUseButton.isEnabled = !isEnable;
		}
		#endregion

		#region 売却モードボタン
		/// <summary>
		/// 売却モードボタンイベント
		/// </summary>
		public event EventHandler OnSellMode = (sender, e) => { };

		/// <summary>
		/// 売却モードボタンクリックイベント(NGUIリフレクション)
		/// </summary>
		public void OnSellModeClickEvent()
		{
			// 通知
			this.OnSellMode(this, EventArgs.Empty);
		}

		/// <summary>
		/// 売却モード表示の有効設定
		/// </summary>
		public void SetSellModeEnable(bool isEnable)
		{
			if (this.ModeAttach == null || this.ModeAttach.SellButton == null) { return; }
			this.ModeAttach.SellButton.isEnabled = !isEnable;
		}
		#endregion

		#region まとめて売却モードボタン
		/// <summary>
		/// まとめて売却モードボタンイベント
		/// </summary>	
		public event EventHandler OnSellMultiMode = (sender, e) => { };

		/// <summary>
		/// まとめて売却モードボタンクリックイベント(NGUIリフレクション)
		/// </summary>
		public void OnSellMultiModeClickEvent()
		{
			// 通知
			this.OnSellMultiMode(this, EventArgs.Empty);
		}

		/// <summary>
		/// まとめて売却モードの有効設定
		/// </summary>
		public void SetSellMultiModeEnable(bool isEnable)
		{
			if (this.ModeAttach == null || this.ModeAttach.SellMultiButton == null) { return; }
			this.ModeAttach.SellMultiButton.isEnabled = !isEnable;
		}
		#endregion
		#endregion

		#region 実行ボタン
		/// <summary>
		/// 実行ボタンイベント
		/// </summary>
		public event EventHandler OnExecute = (sender, e) => { };

		/// <summary>
		/// 実行ボタンクリックイベント(NGUIリフレクション)
		/// </summary>
		public void OnExecuteClickEvent()
		{
			// 通知
			this.OnExecute(this, EventArgs.Empty);
		}

		/// <summary>
		/// 実行ボタン
		/// </summary>
		[SerializeField]
		private XUIButton _executeButton = null;
		public XUIButton ExecuteButton { get { return _executeButton; } }

		/// <summary>
		/// 実行ボタンの有効設定
		/// </summary>
		public void SetExecuteButtonEnable(bool isEnable)
		{
			if (this.ExecuteButton == null) { return; }
			this.ExecuteButton.isEnabled = isEnable;
		}

		/// <summary>
		/// 実行ボタンのラベル
		/// </summary>
		[SerializeField]
		private UILabel _executeLabel = null;
		public UILabel ExecuteLabel { get { return _executeLabel; } }

		/// <summary>
		/// 実行ボタンのラベル名をセットする
		/// </summary>
		public void SetExecuteLabel(string name)
		{
			if (this.ExecuteLabel == null) { return; }
			this.ExecuteLabel.text = name;
		}
		#endregion

		#region アイテム情報
		/// <summary>
		/// アイテム情報アタッチオブジェクト
		/// </summary>
		[SerializeField]
		private ItemInfoAttachObject _itemInfoAttach = null;
		public ItemInfoAttachObject ItemInfoAttach { get { return _itemInfoAttach; } }
		[Serializable]
		public class ItemInfoAttachObject
		{
			/// <summary>
			/// アイテム名
			/// </summary>
			[SerializeField]
			private UILabel _nameLabel = null;
			public UILabel NameLabel { get { return _nameLabel; } }

			/// <summary>
			/// アイテム説明
			/// </summary>
			[SerializeField]
			private UILabel _descriptionLabel = null;
			public UILabel DescriptionLabel { get { return _descriptionLabel; } }

			/// <summary>
			/// ロックONスプライト
			/// </summary>
			[SerializeField]
			private UISprite _lockOnSprite = null;
			public UISprite LockOnSprite { get { return _lockOnSprite; } }

			/// <summary>
			/// ロックOFFスプライト
			/// </summary>
			[SerializeField]
			private UISprite _lockOffSprite = null;
			public UISprite LockOffSprite { get { return _lockOffSprite; } }

			/// <summary>
			/// ロックOFFの背景スプライト
			/// </summary>
			[SerializeField]
			private UISprite _lockOffBGSprite = null;
			public UISprite LockOffBGSprite { get { return _lockOffBGSprite; } }

			/// <summary>
			/// アイテム数ラベル
			/// </summary>
			[SerializeField]
			private UILabel _countLabel = null;
			public UILabel CountLabel { get { return _countLabel; } }

			/// <summary>
			/// 売却額
			/// </summary>
			[SerializeField]
			private UILabel _soldPriceLabel = null;
			public UILabel SoldPriceLabel { get { return _soldPriceLabel; } }
		}

		/// <summary>
		/// アイテム名の設定
		/// </summary>
		public void SetItemName(string name)
		{
			if (this.ItemInfoAttach == null || this.ItemInfoAttach.NameLabel == null) { return; }
			this.ItemInfoAttach.NameLabel.text = name;
		}

		/// <summary>
		/// アイテム説明の設定
		/// </summary>
		public void SetItemDescription(string description)
		{
			if (this.ItemInfoAttach == null || this.ItemInfoAttach.DescriptionLabel == null) { return; }
			this.ItemInfoAttach.DescriptionLabel.text = description;
		}

		/// <summary>
		/// アイテムのロックボタンイベント
		/// </summary>
		public event EventHandler OnLock = (sender, e) => { };

		/// <summary>
		/// アイテムロックボタンクリックイベント(NGUIリフレクション)
		/// </summary>
		public void OnLockClickEvent()
		{
			// 通知
			this.OnLock(this, EventArgs.Empty);
		}

		/// <summary>
		/// アイテムのロック設定
		/// </summary>
		public void SetItemLock(bool isLock)
		{
			if (this.ItemInfoAttach == null) { return; }

			if(this.ItemInfoAttach.LockOnSprite != null)
			{
				this.ItemInfoAttach.LockOnSprite.gameObject.SetActive(isLock);
			}
			if(this.ItemInfoAttach.LockOffSprite != null)
			{
				this.ItemInfoAttach.LockOffSprite.gameObject.SetActive(!isLock);
			}
			if(this.ItemInfoAttach.LockOffBGSprite != null)
			{
				this.ItemInfoAttach.LockOffBGSprite.gameObject.SetActive(!isLock);
			}
		}

		/// <summary>
		/// アイテム個数の設定
		/// </summary>
		public void SetItemCount(string count)
		{
			if (this.ItemInfoAttach == null || this.ItemInfoAttach.CountLabel == null) { return; }
			this.ItemInfoAttach.CountLabel.text = count;
		}

		/// <summary>
		/// アイテム売却額の設定
		/// </summary>
		public void SetItemSoldPrice(int price, string format)
		{
			if (this.ItemInfoAttach == null || this.ItemInfoAttach.SoldPriceLabel == null) { return; }
			this.ItemInfoAttach.SoldPriceLabel.text = string.Format(format, price);
		}
		#endregion
	}
}
