/// <summary>
/// キャラクターBOX表示
/// 
/// 2016/04/29
/// </summary>
using UnityEngine;
using System;

namespace XUI.CharacterBox
{
	#region イベント引数
	public class HomeClickedEventArgs : EventArgs { }
	public class CloseClickedEventArgs : EventArgs { }
	#endregion

	/// <summary>
	/// キャラクターBOX表示インターフェイス
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

		#region キャラ詳細モードボタン
		/// <summary>
		/// キャラ詳細モードボタンイベント
		/// </summary>	
		event EventHandler OnCharaInfoMode;

		/// <summary>
		/// キャラ詳細モードの有効設定
		/// </summary>
		void SetCharaInfoModeEnable(bool isEnable);
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

		#region キャラ情報
		/// <summary>
		/// キャラ名の設定
		/// </summary>
		void SetCharaName(string name);

		/// <summary>
		/// キャラのロックボタンイベント
		/// </summary>
		event EventHandler OnLock;

		/// <summary>
		/// キャラのロック設定
		/// </summary>
		void SetCharaLock(bool isLock);

		/// <summary>
		/// キャラ売却額の設定
		/// </summary>
		void SetCharaSoldPrice(int price, string format);

		/// <summary>
		/// リビルドタイム設定
		/// </summary>
		void SetRebuildTime(string time);

		/// <summary>
		/// コスト設定
		/// </summary>
		void SetCost(string cost);

		/// <summary>
		/// 経験値設定
		/// </summary>
		void SetExp(string exp);

		/// <summary>
		/// シンクロ可能回数設定
		/// </summary>
		void SetSynchroRemain(string synchroRemain);

		/// <summary>
		/// 生命力設定
		/// </summary>
		void SetHitPoint(string hitPoint);

		/// <summary>
		/// 攻撃力設定
		/// </summary>
		void SetAttack(string attack);

		/// <summary>
		/// 防御力設定
		/// </summary>
		void SetDefence(string defence);

		/// <summary>
		/// 特殊能力設定
		/// </summary>
		void SetExtra(string extra);
		#endregion
	}

	/// <summary>
	/// キャラクターBOX表示
	/// </summary>
	public class CharacterBoxView : GUIScreenViewBase, IView
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void OnDestroy()
		{
			this.OnHome = null;
			this.OnClose = null;
			this.OnCharaInfoMode = null;
			this.OnSellMultiMode = null;
			this.OnExecute = null;
			this.OnLock = null;
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
			if (this.TotalSoldPriceLabel != null)
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
			/// キャラ詳細ボタン
			/// </summary>
			[SerializeField]
			private UIButton _charaInfoButton = null;
			public UIButton CharaInfoButton { get { return _charaInfoButton; } }

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

		#region キャラ詳細モードボタン
		/// <summary>
		/// キャラ詳細モードボタンイベント
		/// </summary>	
		public event EventHandler OnCharaInfoMode = (sender, e) => { };

		/// <summary>
		/// キャラ詳細モードボタンクリックイベント(NGUIリフレクション)
		/// </summary>
		public void OnCharaInfoModeClickEvent()
		{
			// 通知
			this.OnCharaInfoMode(this, EventArgs.Empty);
		}

		/// <summary>
		/// キャラ詳細モードの有効設定
		/// </summary>
		public void SetCharaInfoModeEnable(bool isEnable)
		{
			if (this.ModeAttach == null || this.ModeAttach.CharaInfoButton == null) { return; }
			this.ModeAttach.CharaInfoButton.isEnabled = !isEnable;
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

		#region キャラ情報
		/// <summary>
		/// キャラ情報アタッチオブジェクト
		/// </summary>
		[SerializeField]
		private CharaInfoAttachObject _charaInfoAttach = null;
		public CharaInfoAttachObject CharaInfoAttach { get { return _charaInfoAttach; } }
		[Serializable]
		public class CharaInfoAttachObject
		{
			/// <summary>
			/// キャラ名
			/// </summary>
			[SerializeField]
			private UILabel _nameLabel = null;
			public UILabel NameLabel { get { return _nameLabel; } }

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
			/// 売却額
			/// </summary>
			[SerializeField]
			private UILabel _soldPriceLabel = null;
			public UILabel SoldPriceLabel { get { return _soldPriceLabel; } }

			/// <summary>
			/// リビルドタイムラベル
			/// </summary>
			[SerializeField]
			private UILabel _rebuildTimeLabel = null;
			public UILabel RebuildTimeLabel { get { return _rebuildTimeLabel; } }

			/// <summary>
			/// コストラベル
			/// </summary>
			[SerializeField]
			private UILabel _costLabel = null;
			public UILabel CostLabel { get { return _costLabel; } }

			/// <summary>
			/// 経験値ラベル
			/// </summary>
			[SerializeField]
			private UILabel _expLabel = null;
			public UILabel ExpLabel { get { return _expLabel; } }

			/// <summary>
			/// シンクロ可能回数ラベル
			/// </summary>
			[SerializeField]
			private UILabel _synchroRemainLabel = null;
			public UILabel SynchroRemainLabel { get { return _synchroRemainLabel; } }

			/// <summary>
			/// 生命力ラベル
			/// </summary>
			[SerializeField]
			private UILabel _hitPointLabel = null;
			public UILabel HitPointLabel { get { return _hitPointLabel; } }

			/// <summary>
			/// 攻撃力ラベル
			/// </summary>
			[SerializeField]
			private UILabel _attackLabel = null;
			public UILabel AttackLabel { get { return _attackLabel; } }

			/// <summary>
			/// 防御力ラベル
			/// </summary>
			[SerializeField]
			private UILabel _defenceLabel = null;
			public UILabel DefenceLabel { get { return _defenceLabel; } }

			/// <summary>
			/// 特殊能力ラベル
			/// </summary>
			[SerializeField]
			private UILabel _extraLabel = null;
			public UILabel ExtraLabel { get { return _extraLabel; } }
		}

		/// <summary>
		/// キャラ名の設定
		/// </summary>
		public void SetCharaName(string name)
		{
			if (this.CharaInfoAttach == null || this.CharaInfoAttach.NameLabel == null) { return; }
			this.CharaInfoAttach.NameLabel.text = name;
		}

		/// <summary>
		/// キャラのロックボタンイベント
		/// </summary>
		public event EventHandler OnLock = (sender, e) => { };

		/// <summary>
		/// キャラロックボタンクリックイベント(NGUIリフレクション)
		/// </summary>
		public void OnLockClickEvent()
		{
			// 通知
			this.OnLock(this, EventArgs.Empty);
		}

		/// <summary>
		/// キャラのロック設定
		/// </summary>
		public void SetCharaLock(bool isLock)
		{
			if (this.CharaInfoAttach == null) { return; }

			if (this.CharaInfoAttach.LockOnSprite != null)
			{
				this.CharaInfoAttach.LockOnSprite.gameObject.SetActive(isLock);
			}
			if (this.CharaInfoAttach.LockOffSprite != null)
			{
				this.CharaInfoAttach.LockOffSprite.gameObject.SetActive(!isLock);
			}
			if (this.CharaInfoAttach.LockOffBGSprite != null)
			{
				this.CharaInfoAttach.LockOffBGSprite.gameObject.SetActive(!isLock);
			}
		}

		/// <summary>
		/// キャラ売却額の設定
		/// </summary>
		public void SetCharaSoldPrice(int price, string format)
		{
			if (this.CharaInfoAttach == null || this.CharaInfoAttach.SoldPriceLabel == null) { return; }
			this.CharaInfoAttach.SoldPriceLabel.text = string.Format(format, price);
		}

		/// <summary>
		/// リビルドタイム設定
		/// </summary>
		public void SetRebuildTime(string time)
		{
			if (this.CharaInfoAttach == null || this.CharaInfoAttach.RebuildTimeLabel == null) { return; }
			this.CharaInfoAttach.RebuildTimeLabel.text = time;
		}

		/// <summary>
		/// コスト設定
		/// </summary>
		public void SetCost(string cost)
		{
			if (this.CharaInfoAttach == null || this.CharaInfoAttach.CostLabel == null) { return; }
			this.CharaInfoAttach.CostLabel.text = cost;
		}

		/// <summary>
		/// 経験値設定
		/// </summary>
		public void SetExp(string exp)
		{
			if (this.CharaInfoAttach == null || this.CharaInfoAttach.ExpLabel == null) { return; }
			this.CharaInfoAttach.ExpLabel.text = exp;
		}

		/// <summary>
		/// シンクロ可能回数設定
		/// </summary>
		public void SetSynchroRemain(string synchroRemain)
		{
			if (this.CharaInfoAttach == null || this.CharaInfoAttach.SynchroRemainLabel == null) { return; }
			this.CharaInfoAttach.SynchroRemainLabel.text = synchroRemain;
		}

		/// <summary>
		/// 生命力設定
		/// </summary>
		public void SetHitPoint(string hitPoint)
		{
			if (this.CharaInfoAttach == null || this.CharaInfoAttach.HitPointLabel == null) { return; }
			this.CharaInfoAttach.HitPointLabel.text = hitPoint;
		}

		/// <summary>
		/// 攻撃力設定
		/// </summary>
		public void SetAttack(string attack)
		{
			if (this.CharaInfoAttach == null || this.CharaInfoAttach.AttackLabel == null) { return; }
			this.CharaInfoAttach.AttackLabel.text = attack;
		}

		/// <summary>
		/// 防御力設定
		/// </summary>
		public void SetDefence(string defence)
		{
			if (this.CharaInfoAttach == null || this.CharaInfoAttach.DefenceLabel == null) { return; }
			this.CharaInfoAttach.DefenceLabel.text = defence;
		}

		/// <summary>
		/// 特殊能力設定
		/// </summary>
		public void SetExtra(string extra)
		{
			if (this.CharaInfoAttach == null || this.CharaInfoAttach.ExtraLabel == null) { return; }
			this.CharaInfoAttach.ExtraLabel.text = extra;
		}
		#endregion
	}
}
