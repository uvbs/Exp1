/// <summary>
/// アイテム売却表示
/// 
/// 2016/04/08
/// </summary>
using UnityEngine;
using System;

namespace XUI.ItemSellSimple
{
	#region イベント引数
	public class HomeClickedEventArgs : EventArgs { }
	public class CloseClickedEventArgs : EventArgs { }

	/// <summary>
	/// アイテム売却数スライダー変化イベント引数
	/// </summary>
	public class SellItemCountSliderChangeEventArgs : EventArgs
	{
		public float Value { get; set; }
	}
	#endregion

	#region エフェクトタイプ
	/// <summary>
	/// 所持金のエフェクトタイプ
	/// </summary>
	public enum HaveMoneyEffectType : byte
	{
		None	= 0,
		Over	= 1,
		Add		= 2,
	}
	#endregion

	/// <summary>
	/// アイテム売却表示インターフェイス
	/// </summary>
	public interface IView
	{
		#region 閉じる
		// 閉じるイベント通知用
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

		#region アイテム名
		/// <summary>
		/// アイテム名の設定
		/// </summary>
		void SetItemName(string name);
		#endregion

		#region 所持数
		/// <summary>
		/// 所持数の設定
		/// </summary>
		void SetHaveItemCountLabel(string count);
		#endregion

		#region 売却数
		/// <summary>
		/// 売却数の設定
		/// </summary>
		void SetSellItemCount(string count);

		/// <summary>
		/// 売却数減らすボタンイベント
		/// </summary>
		event EventHandler OnSubSellCount;

		/// 売却数追加ボタンイベント
		/// </summary>
		event EventHandler OnAddSellCount;
		#endregion

		#region 売却額
		/// <summary>
		/// 売却額の設定
		/// </summary>
		void SetSoldPrice(string soldPrice);
		#endregion

		#region 売却数スライダー
		/// <summary>
		/// 売却数スライダーの値を設定
		/// </summary>
		void SetSellItemCountSliderValue(float value);

		/// <summary>
		/// 売却数スライダーの値変化イベント
		/// </summary>
		event EventHandler<SellItemCountSliderChangeEventArgs> OnSellItemCountSliderChange;
		#endregion

		#region 所持金
		/// <summary>
		/// 所持金設定
		/// </summary>
		void SetHaveMoney(int money, string format);
		#endregion

		#region 売却ボタン
		/// <summary>
		/// 売却ボタンイベント
		/// </summary>
		event EventHandler OnSell;

		/// <summary>
		/// 売却ボタンの有効設定
		/// </summary>
		void SetSellButtonEnable(bool isEnable);
		#endregion
	}

	/// <summary>
	/// アイテム売却表示
	/// </summary>
	public class ItemSellSimpleView : GUIViewBase, IView
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void OnDestroy()
		{
			this.OnClose = null;
			this.OnAddSellCount = null;
			this.OnSubSellCount = null;
			this.OnSellItemCountSliderChange = null;
			this.OnSell = null;
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

		#region 閉じるボタンイベント
		/// <summary>
		/// 閉じるイベント通知用
		/// </summary>
		public event EventHandler<CloseClickedEventArgs> OnClose = (sender, e) => { };

		/// <summary>
		/// 閉じるボタンイベント
		/// </summary>
		public void OnCloseEvent()
		{
			// 通知
			var eventArgs = new CloseClickedEventArgs();
			this.OnClose(this, eventArgs);
		}
		#endregion

		#region アイテム名
		/// <summary>
		/// アイテム名ラベル
		/// </summary>
		[SerializeField]
		private UILabel _itemNameLabel = null;
		private UILabel ItemNameLabel { get { return _itemNameLabel; } }

		/// <summary>
		/// アイテム名の設定
		/// </summary>
		public void SetItemName(string name)
		{
			if (this.ItemNameLabel == null) { return; }
			this.ItemNameLabel.text = name;
		}
		#endregion

		#region 所持数
		/// <summary>
		/// 所持数ラベル
		/// </summary>
		[SerializeField]
		private UILabel _haveItemCountLabel = null;
		private UILabel HaveItemCountLabel { get { return _haveItemCountLabel; } }

		/// <summary>
		/// 所持数の設定
		/// </summary>
		public void SetHaveItemCountLabel(string count)
		{
			if (this.HaveItemCountLabel == null) { return; }
			this.HaveItemCountLabel.text = count;
		}
		#endregion

		#region 売却数
		/// <summary>
		/// 売却数ラベル
		/// </summary>
		[SerializeField]
		private UILabel _sellItemCountLabel = null;
		private UILabel SellItemCountLabel { get { return _sellItemCountLabel; } }

		/// <summary>
		/// 売却数の設定
		/// </summary>
		public void SetSellItemCount(string count)
		{
			if (this.SellItemCountLabel == null) { return; }
			this.SellItemCountLabel.text = count;
		}

		/// <summary>
		/// 売却数減らすボタンイベント
		/// </summary>
		public event EventHandler OnSubSellCount = (sender, e) => { };

		/// <summary>
		/// 売却数減らすボタンクリックイベント(NGUIリフレクション)
		/// </summary>
		public void OnSubSellCountClickEvent()
		{
			// 通知
			this.OnSubSellCount(this, EventArgs.Empty);
		}

		/// <summary>
		/// 売却数追加ボタンイベント
		/// </summary>
		public event EventHandler OnAddSellCount = (sender, e) => { };

		/// <summary>
		/// 売却数追加ボタンクリックイベント(NGUIリフレクション)
		/// </summary>
		public void OnAddSellCountClickEvent()
		{
			// 通知
			this.OnAddSellCount(this, EventArgs.Empty);
		}
		#endregion

		#region 売却額
		/// <summary>
		/// 売却額ラベル
		/// </summary>
		[SerializeField]
		private UILabel _soldPriceLabel = null;
		private UILabel SoldPriceLabel { get { return _soldPriceLabel; } }

		/// <summary>
		/// 売却額の設定
		/// </summary>
		public void SetSoldPrice(string soldPrice)
		{
			if (this.SoldPriceLabel == null) { return; }
			this.SoldPriceLabel.text = soldPrice;
		}
		#endregion

		#region 売却数スライダー
		/// <summary>
		/// 売却数スライダー
		/// </summary>
		[SerializeField]
		private UISlider _sellItemCountSlider = null;
		private UISlider SellItemCountSlider { get { return _sellItemCountSlider; } }

		/// <summary>
		/// 売却数スライダーの値を設定
		/// </summary>
		public void SetSellItemCountSliderValue(float value)
		{
			if (this.SellItemCountSlider == null) { return; }
			this.SellItemCountSlider.value = value;
		}

		/// <summary>
		/// 売却数スライダーの値変化イベント
		/// </summary>
		public event EventHandler<SellItemCountSliderChangeEventArgs> OnSellItemCountSliderChange = (sender, e) => { };

		/// <summary>
		/// 売却数スライダーの値変化イベント(NGUIリフレクション)
		/// </summary>
		public void OnSellItemCountSliderChangeEvent()
		{
			if (this.SellItemCountSlider == null) { return; }

			// 通知
			var eventArgs = new SellItemCountSliderChangeEventArgs();
			eventArgs.Value = this.SellItemCountSlider.value;
			this.OnSellItemCountSliderChange(this, eventArgs);
		}
		#endregion

		#region 所持金
		/// <summary>
		/// 所持金ラベル
		/// </summary>
		[SerializeField]
		private UILabel _haveMoneyLabel = null;
		private UILabel HaveMoneyLabel { get { return _haveMoneyLabel; } }

		/// <summary>
		/// 所持金設定
		/// </summary>
		public void SetHaveMoney(int money, string format)
		{
			if (this.HaveMoneyLabel == null) { return; }
			this.HaveMoneyLabel.text = string.Format(format, money);
		}
		#endregion

		#region 売却ボタン
		/// <summary>
		/// 売却ボタンイベント
		/// </summary>
		public event EventHandler OnSell = (sender, e) => { };

		/// <summary>
		/// 売却ボタンクリックイベント(NGUIリフレクション)
		/// </summary>
		public void OnSellClickEvent()
		{
			// 通知
			this.OnSell(this, EventArgs.Empty);
		}

		/// <summary>
		/// 売却ボタン
		/// </summary>
		[SerializeField]
		private UIButton _sellButton = null;
		private UIButton SellButton { get { return _sellButton; } }

		/// <summary>
		/// 売却ボタンの有効設定
		/// </summary>
		public void SetSellButtonEnable(bool isEnable)
		{
			if (this.SellButton == null) { return; }
			this.SellButton.isEnabled = isEnable;
		}
		#endregion
	}
}