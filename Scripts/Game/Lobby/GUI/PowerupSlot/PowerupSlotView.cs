/// <summary>
/// 強化スロット表示
/// 
/// 2016/03/02
/// </summary>
using UnityEngine;
using System;

namespace XUI.PowerupSlot
{
	/// <summary>
	/// 強化スロット表示インターフェイス
	/// </summary>
	public interface IView
	{
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

		#region ホーム、閉じるボタンイベント
		event EventHandler<EventArgs> OnHome;
		event EventHandler<EventArgs> OnClose;
		#endregion

		#region OKボタン
		event EventHandler<EventArgs> OnOK;

		/// <summary>
		/// OKボタンの有効化
		/// </summary>
		void SetOKButtonEnable(bool isEnable);
		#endregion

		#region スロット追加ボタン
		event EventHandler<EventArgs> OnAddSlot;

		/// <summary>
		/// スロット追加ボタンの有効化
		/// </summary>
		void SetAddSlotButtonEnable(bool isEnable);
		#endregion

		#region 生命力
		/// <summary>
		/// 生命力設定
		/// </summary>
		void SetHitPoint(int hp, string format);
		/// <summary>
		/// 生命力試算後設定
		/// </summary>
		void SetHitPointCalc(int hp, string format);
		/// <summary>
		/// 生命力試算後カラー設定
		/// </summary>
		void SetHitPointCalcColor(StatusColor.Type type);
		#endregion

		#region 攻撃力
		/// <summary>
		/// 攻撃力設定
		/// </summary>
		void SetAttack(int attack, string format);
		/// <summary>
		/// 攻撃力試算後設定
		/// </summary>
		void SetAttackCalc(int attack, string format);
		/// <summary>
		/// 攻撃力試算後カラー設定
		/// </summary>
		void SetAttackCalcColor(StatusColor.Type type);
		#endregion

		#region 防御力
		/// <summary>
		/// 防御力設定
		/// </summary>
		void SetDefense(int defense, string format);
		/// <summary>
		/// 防御力試算後設定
		/// </summary>
		void SetDefenseCalc(int defense, string format);
		/// <summary>
		/// 防御力試算後カラー設定
		/// </summary>
		void SetDefenseCalcColor(StatusColor.Type type);
		#endregion

		#region 特殊能力
		/// <summary>
		/// 特殊能力設定
		/// </summary>
		void SetExtra(int extra, string format);
		/// <summary>
		/// 特殊能力試算後設定
		/// </summary>
		void SetExtraCalc(int extra, string format);
		/// <summary>
		/// 特殊能力試算後カラー設定
		/// </summary>
		void SetExtraCalcColor(StatusColor.Type type);
		#endregion

		#region スロット数
		/// <summary>
		/// スロット数設定
		/// </summary>
		void SetSlotNum(int count, int capacity, string format);
		#endregion

		#region スロットリストフィルター
		/// <summary>
		/// スロットリストフィルターのアクティブ化
		/// </summary>
		void SetSlotFillActive(bool isActive);
		#endregion

		#region 所持金
		/// <summary>
		/// 所持金設定
		/// </summary>
		void SetHaveMoney(int money, string format);
		#endregion

		#region 費用
		/// <summary>
		/// 費用設定
		/// </summary>
		void SetNeedMoney(int money, string format);
		#endregion
	}

	/// <summary>
	/// 強化スロット表示
	/// </summary>
	public class PowerupSlotView : GUIScreenViewBase, IView
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void OnDestroy()
		{
			this.OnHome = null;
			this.OnClose = null;
			this.OnOK = null;
			this.OnAddSlot = null;
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
		public event EventHandler<EventArgs> OnHome = (sender, e) => { };
		public event EventHandler<EventArgs> OnClose = (sender, e) => { };

		/// <summary>
		/// ホームボタンイベント
		/// </summary>
		public override void OnHomeEvent()
		{
			// 通知
			var eventArgs = new EventArgs();
			this.OnHome(this, eventArgs);
		}

		/// <summary>
		/// 閉じるボタンイベント
		/// </summary>
		public override void OnCloseEvent()
		{
			// 通知
			var eventArgs = new EventArgs();
			this.OnClose(this, eventArgs);
		}
		#endregion

		#region OKボタン
		public event EventHandler<EventArgs> OnOK = (sender, e) => { };

		/// <summary>
		/// OKボタンイベント
		/// </summary>
		public void OnOKEvent()
		{
			// 通知
			var eventArgs = new EventArgs();
			this.OnOK(this, eventArgs);
		}

		[SerializeField]
		UIButton _okButton = null;
		UIButton OKButton { get { return _okButton; } }

		/// <summary>
		/// OKボタンの有効化
		/// </summary>
		public void SetOKButtonEnable(bool isEnable)
		{
			if (this.OKButton != null)
			{
				this.OKButton.isEnabled = isEnable;
			}
		}
		#endregion

		#region スロット追加ボタン
		public event EventHandler<EventArgs> OnAddSlot = (sender, e) => { };

		/// <summary>
		/// スロット追加ボタンイベント
		/// </summary>
		public void OnAddSlotEvent()
		{
			// 通知
			var eventArgs = new EventArgs();
			this.OnAddSlot(this, eventArgs);
		}

		[SerializeField]
		UIButton _addSlotButton = null;
		UIButton AddSlotButton { get { return _addSlotButton; } }

		/// <summary>
		/// スロット追加ボタンの有効化
		/// </summary>
		public void SetAddSlotButtonEnable(bool isEnable)
		{
			if (this.AddSlotButton != null)
			{
				this.AddSlotButton.isEnabled = isEnable;
			}
		}
		#endregion

		#region 生命力
		[SerializeField]
		UILabel _hitPointLabel = null;
		UILabel HitPointLabel { get { return _hitPointLabel; } }
		[SerializeField]
		UILabel _hitPointCalcLabel = null;
		UILabel HitPointCalcLabel { get { return _hitPointCalcLabel; } }
		[SerializeField]
		UILabel _hitPointCalcGrowLabel = null;
		UILabel HitPointCalcGrowLabel { get { return _hitPointCalcGrowLabel; } }
		[SerializeField]
		GameObject _hitPointCalcUpObject = null;
		GameObject HitPointCalcUpObject { get { return _hitPointCalcUpObject; } }
		[SerializeField]
		GameObject _hitPointCalcDownObject = null;
		GameObject HitPointCalcDownObject { get { return _hitPointCalcDownObject; } }

		/// <summary>
		/// 生命力設定
		/// </summary>
		public void SetHitPoint(int hp, string format)
		{
			if (this.HitPointLabel != null)
			{
				this.HitPointLabel.text = string.Format(format, hp);
			}
		}
		/// <summary>
		/// 生命力試算後設定
		/// </summary>
		public void SetHitPointCalc(int hp, string format)
		{
			if (this.HitPointCalcLabel != null)
			{
				this.HitPointCalcLabel.text = string.Format(format, hp);
			}
		}
		/// <summary>
		/// 生命力試算後カラー設定
		/// </summary>
		public void SetHitPointCalcColor(StatusColor.Type type)
		{
			StatusColor.Set(type, this.HitPointCalcLabel, this.HitPointCalcGrowLabel);

			bool isUp = false;
			bool isDown = false;
			if (type == StatusColor.Type.Up) isUp = true;
			else if (type == StatusColor.Type.Down) isDown = true;

			if (this.HitPointCalcUpObject != null)
			{
				this.HitPointCalcUpObject.SetActive(isUp);
			}
			if (this.HitPointCalcDownObject != null)
			{
				this.HitPointCalcDownObject.SetActive(isDown);
			}
		}
		#endregion

		#region 攻撃力
		[SerializeField]
		UILabel _attackLabel = null;
		UILabel AttackLabel { get { return _attackLabel; } }
		[SerializeField]
		UILabel _attackCalcLabel = null;
		UILabel AttackCalcLabel { get { return _attackCalcLabel; } }
		[SerializeField]
		UILabel _attackCalcGrowLabel = null;
		UILabel AttackCalcGrowLabel { get { return _attackCalcGrowLabel; } }
		[SerializeField]
		GameObject _attackCalcUpObject = null;
		GameObject AttackCalcUpObject { get { return _attackCalcUpObject; } }
		[SerializeField]
		GameObject _attackCalcDownObject = null;
		GameObject AttackCalcDownObject { get { return _attackCalcDownObject; } }

		/// <summary>
		/// 攻撃力設定
		/// </summary>
		public void SetAttack(int attack, string format)
		{
			if (this.AttackLabel != null)
			{
				this.AttackLabel.text = string.Format(format, attack);
			}
		}
		/// <summary>
		/// 攻撃力試算後設定
		/// </summary>
		public void SetAttackCalc(int attack, string format)
		{
			if (this.AttackCalcLabel != null)
			{
				this.AttackCalcLabel.text = string.Format(format, attack);
			}
		}
		/// <summary>
		/// 攻撃力試算後カラー設定
		/// </summary>
		public void SetAttackCalcColor(StatusColor.Type type)
		{
			StatusColor.Set(type, this.AttackCalcLabel, this.AttackCalcGrowLabel);

			bool isUp = false;
			bool isDown = false;
			if (type == StatusColor.Type.Up) isUp = true;
			else if (type == StatusColor.Type.Down) isDown = true;

			if (this.AttackCalcUpObject != null)
			{
				this.AttackCalcUpObject.SetActive(isUp);
			}
			if (this.AttackCalcDownObject != null)
			{
				this.AttackCalcDownObject.SetActive(isDown);
			}
		}
		#endregion

		#region 防御力
		[SerializeField]
		UILabel _defenseLabel = null;
		UILabel DefenseLabel { get { return _defenseLabel; } }
		[SerializeField]
		UILabel _defenseCalcLabel = null;
		UILabel DefenseCalcLabel { get { return _defenseCalcLabel; } }
		[SerializeField]
		UILabel _defenseCalcGrowLabel = null;
		UILabel DefenseCalcGrowLabel { get { return _defenseCalcGrowLabel; } }
		[SerializeField]
		GameObject _defenseCalcUpObject = null;
		GameObject DefenseCalcUpObject { get { return _defenseCalcUpObject; } }
		[SerializeField]
		GameObject _defenseCalcDownObject = null;
		GameObject DefenseCalcDownObject { get { return _defenseCalcDownObject; } }

		/// <summary>
		/// 防御力設定
		/// </summary>
		public void SetDefense(int defense, string format)
		{
			if (this.DefenseLabel != null)
			{
				this.DefenseLabel.text = string.Format(format, defense);
			}
		}
		/// <summary>
		/// 防御力試算後設定
		/// </summary>
		public void SetDefenseCalc(int defense, string format)
		{
			if (this.DefenseCalcLabel != null)
			{
				this.DefenseCalcLabel.text = string.Format(format, defense);
			}
		}
		/// <summary>
		/// 防御力試算後カラー設定
		/// </summary>
		public void SetDefenseCalcColor(StatusColor.Type type)
		{
			StatusColor.Set(type, this.DefenseCalcLabel, this.DefenseCalcGrowLabel);

			bool isUp = false;
			bool isDown = false;
			if (type == StatusColor.Type.Up) isUp = true;
			else if (type == StatusColor.Type.Down) isDown = true;

			if (this.DefenseCalcUpObject != null)
			{
				this.DefenseCalcUpObject.SetActive(isUp);
			}
			if (this.DefenseCalcDownObject != null)
			{
				this.DefenseCalcDownObject.SetActive(isDown);
			}
		}
		#endregion

		#region 特殊能力
		[SerializeField]
		UILabel _extraLabel = null;
		UILabel ExtraLabel { get { return _extraLabel; } }
		[SerializeField]
		UILabel _extraCalcLabel = null;
		UILabel ExtraCalcLabel { get { return _extraCalcLabel; } }
		[SerializeField]
		UILabel _extraCalcGrowLabel = null;
		UILabel ExtraCalcGrowLabel { get { return _extraCalcGrowLabel; } }
		[SerializeField]
		GameObject _extraCalcUpObject = null;
		GameObject ExtraCalcUpObject { get { return _extraCalcUpObject; } }
		[SerializeField]
		GameObject _extraCalcDownObject = null;
		GameObject ExtraCalcDownObject { get { return _extraCalcDownObject; } }

		/// <summary>
		/// 特殊能力設定
		/// </summary>
		public void SetExtra(int extra, string format)
		{
			if (this.ExtraLabel != null)
			{
				this.ExtraLabel.text = string.Format(format, extra);
			}
		}
		/// <summary>
		/// 特殊能力試算後設定
		/// </summary>
		public void SetExtraCalc(int extra, string format)
		{
			if (this.ExtraCalcLabel != null)
			{
				this.ExtraCalcLabel.text = string.Format(format, extra);
			}
		}
		/// <summary>
		/// 特殊能力試算後カラー設定
		/// </summary>
		public void SetExtraCalcColor(StatusColor.Type type)
		{
			StatusColor.Set(type, this.ExtraCalcLabel, this.ExtraCalcGrowLabel);

			bool isUp = false;
			bool isDown = false;
			if (type == StatusColor.Type.Up) isUp = true;
			else if (type == StatusColor.Type.Down) isDown = true;

			if (this.ExtraCalcUpObject != null)
			{
				this.ExtraCalcUpObject.SetActive(isUp);
			}
			if (this.ExtraCalcDownObject != null)
			{
				this.ExtraCalcDownObject.SetActive(isDown);
			}
		}
		#endregion

		#region スロット数
		[SerializeField]
		UILabel _slotNumLabel = null;
		UILabel SlotNumLabel { get { return _slotNumLabel; } }

		/// <summary>
		/// スロット数設定
		/// </summary>
		public void SetSlotNum(int count, int capacity, string format)
		{
			if (this.SlotNumLabel != null)
			{
				this.SlotNumLabel.text = string.Format(format, count, capacity);
			}
		}
		#endregion

		#region スロットリストフィルター
		[SerializeField]
		GameObject _slotFillObject = null;
		GameObject SlotFillObject { get { return _slotFillObject; } }

		/// <summary>
		/// スロットリストフィルターのアクティブ化
		/// </summary>
		public void SetSlotFillActive(bool isActive)
		{
			if (this.SlotFillObject != null)
			{
				this.SlotFillObject.SetActive(isActive);
			}
		}
		#endregion

		#region 所持金
		[SerializeField]
		UILabel _haveMoneyLabel = null;
		UILabel HaveMoneyLabel { get { return _haveMoneyLabel; } }

		/// <summary>
		/// 所持金設定
		/// </summary>
		public void SetHaveMoney(int money, string format)
		{
			if (this.HaveMoneyLabel != null)
			{
				this.HaveMoneyLabel.text = string.Format(format, money);
			}
		}
		#endregion

		#region 費用
		[SerializeField]
		UILabel _needMoneyLabel = null;
		UILabel NeedMoneyLabel { get { return _needMoneyLabel; } }

		/// <summary>
		/// 費用設定
		/// </summary>
		public void SetNeedMoney(int money, string format)
		{
			if (this.NeedMoneyLabel != null)
			{
				this.NeedMoneyLabel.text = string.Format(format, money);
			}
		}
		#endregion
	}
}
