/// <summary>
/// 強化合成表示
/// 
/// 2016/01/08
/// </summary>
using UnityEngine;
using System;

namespace XUI
{
	namespace Powerup
	{
		public class HomeClickedEventArgs : EventArgs { }
		public class CloseClickedEventArgs : EventArgs { }

		public class FusionClickedEventArgs : EventArgs { }

		/// <summary>
		/// 強化合成表示インターフェイス
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
			event EventHandler<HomeClickedEventArgs> OnHome;
			event EventHandler<CloseClickedEventArgs> OnClose;
			#endregion

			#region 合成ボタン
			event EventHandler<FusionClickedEventArgs> OnFusion;

			/// <summary>
			/// 合成ボタンの有効化
			/// </summary>
			void SetFusionButtonEnable(bool isEnable);
			#endregion

			#region 全てを外すボタン
			event EventHandler<EventArgs> OnAllClear;

			/// <summary>
			/// 全てを外すボタンの有効化
			/// </summary>
			void SetAllClearButtonEnable(bool isEnable);
			#endregion

			#region 獲得経験値
			/// <summary>
			/// 獲得経験値設定
			/// </summary>
			void SetTakeExp(long exp, string format);
			/// <summary>
			/// 獲得経験値カラー設定
			/// </summary>
			void SetTakeExpColor(StatusColor.Type type);
			#endregion

			#region ベース情報モード
			/// <summary>
			/// 合成前情報と合成後情報の切り替え
			/// </summary>
			void SetBaseInfoMode(BaseInfoMode mode);
			#endregion

			#region 合成前
			#region レベル
			/// <summary>
			/// レベル設定
			/// </summary>
			void SetLv(int lv, string format);
			#endregion

			#region 経験値
			/// <summary>
			/// 経験値設定
			/// </summary>
			void SetExp(long exp, string format);
			#endregion

			#region 次のレベルまでの経験値
			/// <summary>
			/// 次のレベルまでの経験値設定
			/// </summary>
			void SetNextLvExp(long exp, string format);
			#endregion

			#region 経験値バー
			/// <summary>
			/// 経験値バー設定
			/// </summary>
			void SetExpSlider(float slider);
			#endregion
			#endregion

			#region 合成後
			#region 合成後のレベル
			/// <summary>
			/// 合成後のレベル設定
			/// </summary>
			void SetAfterLv(int lv, string format);
			/// <summary>
			/// 合成後のレベルカラー設定
			/// </summary>
			void SetAfterLvColor(StatusColor.Type type);
			#endregion

			#region 合成後の経験値
			/// <summary>
			/// 合成後の経験値設定
			/// </summary>
			void SetAfterExp(long exp, string format);
			/// <summary>
			/// 合成後の経験値カラー設定
			/// </summary>
			void SetAfterExpColor(StatusColor.Type type);
			#endregion

			#region 合成後の次のレベルまでの経験値
			/// <summary>
			/// 合成後の次のレベルまでの経験値設定
			/// </summary>
			void SetAfterNextLvExp(long exp, string format);
			#endregion

			#region 合成後の経験値バー
			/// <summary>
			/// 合成後の経験値バー設定
			/// </summary>
			void SetAfterExpSlider(float expSlider, float takeExpSlider);
			#endregion

			#region 最大レベルかどうか
			/// <summary>
			/// 最大レベルかどうかの切り替え
			/// </summary>
			void SetLvMax(bool isLvMax);
			#endregion

			#region 合成後の余剰経験値
			/// <summary>
			/// 合成後の余剰経験値設定
			/// </summary>
			void SetAfterOverflowExp(int overflowExp, string format);
			#endregion
			#endregion

			#region 素材リストフィルター
			/// <summary>
			/// 素材リストフィルターのアクティブ化
			/// </summary>
			void SetBaitFillActive(bool isActive);
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
		/// 強化合成表示
		/// </summary>
		public class PowerupView : GUIScreenViewBase, IView
		{
			#region 破棄
			/// <summary>
			/// 破棄
			/// </summary>
			void OnDestroy()
			{
				this.OnHome = null;
				this.OnClose = null;
				this.OnFusion = null;
				this.OnAllClear = null;
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

			#region 合成ボタン
			public event EventHandler<FusionClickedEventArgs> OnFusion = (sender, e) => { };

			/// <summary>
			/// 合成ボタンイベント
			/// </summary>
			public void OnFusionEvent()
			{
				// 通知
				var eventArgs = new FusionClickedEventArgs();
				this.OnFusion(this, eventArgs);
			}

			[SerializeField]
			UIButton _fusionButton = null;
			UIButton FusionButton { get { return _fusionButton; } }

			/// <summary>
			/// 合成ボタンの有効化
			/// </summary>
			public void SetFusionButtonEnable(bool isEnable)
			{
				if (this.FusionButton != null)
				{
					this.FusionButton.isEnabled = isEnable;
				}
			}
			#endregion

			#region 全てを外すボタン
			public event EventHandler<EventArgs> OnAllClear = (sender, e) => { };

			/// <summary>
			/// 全てを外すボタンイベント
			/// </summary>
			public void OnAllClearEvent()
			{
				// 通知
				var eventArgs = new FusionClickedEventArgs();
				this.OnAllClear(this, eventArgs);
			}

			[SerializeField]
			UIButton _allClearButton = null;
			UIButton AllClearButton { get { return _allClearButton; } }

			/// <summary>
			/// 全てを外すボタンの有効化
			/// </summary>
			public void SetAllClearButtonEnable(bool isEnable)
			{
				if (this.AllClearButton != null)
				{
					this.AllClearButton.isEnabled = isEnable;
				}
			}
			#endregion

			#region 獲得経験値
			[SerializeField]
			UILabel _takeExpLabel = null;
			UILabel TakeExpLabel { get { return _takeExpLabel; } }

			[SerializeField]
			UILabel _takeExpGrowLabel = null;
			UILabel TakeExpGrowLabel { get { return _takeExpGrowLabel; } }

			/// <summary>
			/// 獲得経験値設定
			/// </summary>
			public void SetTakeExp(long exp, string format)
			{
				if (this.TakeExpLabel != null)
				{
					this.TakeExpLabel.text = string.Format(format, exp);
				}
			}
			/// <summary>
			/// 獲得経験値カラー設定
			/// </summary>
			public void SetTakeExpColor(StatusColor.Type type)
			{
				StatusColor.Set(type, this.TakeExpLabel, this.TakeExpGrowLabel);
			}
			#endregion

			#region ベース情報モード
			[SerializeField]
			GameObject _beforeGroup = null;
			GameObject BeforeGroup { get { return _beforeGroup; } }
			[SerializeField]
			GameObject _afterGroup = null;
			GameObject AfterGroup { get { return _afterGroup; } }

			/// <summary>
			/// 合成前情報と合成後情報の切り替え
			/// </summary>
			public void SetBaseInfoMode(BaseInfoMode mode)
			{
				var list = new System.Collections.Generic.List<GameObject>();
				list.Add(this.BeforeGroup);
				list.Add(this.AfterGroup);

				GameObject activeGroup = null;
				switch (mode)
				{
					case BaseInfoMode.Before:
						activeGroup = this.BeforeGroup;
						break;
					case BaseInfoMode.After:
						activeGroup = this.AfterGroup;
						break;
				}

				foreach (var t in list)
				{
					if (t == null) continue;
					var isActive = (t == activeGroup ? true : false);
					t.SetActive(isActive);
				}
			}
			#endregion

			#region 合成前
			#region レベル
			[SerializeField]
			UILabel _lvLabel = null;
			UILabel LvLabel { get { return _lvLabel; } }

			/// <summary>
			/// レベル設定
			/// </summary>
			public void SetLv(int lv, string format)
			{
				if (this.LvLabel != null)
				{
					this.LvLabel.text = string.Format(format, lv);
				}
			}
			#endregion

			#region 経験値
			[SerializeField]
			UILabel _expLabel = null;
			UILabel ExpLabel { get { return _expLabel; } }

			/// <summary>
			/// 経験値設定
			/// </summary>
			public void SetExp(long exp, string format)
			{
				if (this.ExpLabel != null)
				{
					this.ExpLabel.text = string.Format(format, exp);
				}
			}
			#endregion

			#region 次のレベルまでの経験値
			[SerializeField]
			UILabel _nextLvExpLabel = null;
			UILabel NextLvExpLabel { get { return _nextLvExpLabel; } }

			/// <summary>
			/// 次のレベルまでの経験値設定
			/// </summary>
			public void SetNextLvExp(long exp, string format)
			{
				if (this.NextLvExpLabel != null)
				{
					this.NextLvExpLabel.text = string.Format(format, exp);
				}
			}
			#endregion

			#region 経験値バー
			[SerializeField]
			UIProgressBar _expSlider = null;
			UIProgressBar ExpSlider { get { return _expSlider; } }
			/// <summary>
			/// 経験値バー設定
			/// </summary>
			public void SetExpSlider(float expSlider)
			{
				if (this.ExpSlider != null)
				{
					this.ExpSlider.value = expSlider;
				}
			}
			#endregion
			#endregion

			#region 合成後
			#region 合成後のレベル
			[SerializeField]
			UILabel _afterLvLabel = null;
			UILabel AfterLvLabel { get { return _afterLvLabel; } }

			[SerializeField]
			UILabel _afterLvGrowLabel = null;
			UILabel AfterLvGrowLabel { get { return _afterLvGrowLabel; } }

			/// <summary>
			/// 合成後のレベル設定
			/// </summary>
			public void SetAfterLv(int lv, string format)
			{
				if (this.AfterLvLabel != null)
				{
					this.AfterLvLabel.text = string.Format(format, lv);
				}
			}
			/// <summary>
			/// 合成後のレベルカラー設定
			/// </summary>
			public void SetAfterLvColor(StatusColor.Type type)
			{
				StatusColor.Set(type, this.AfterLvLabel, this.AfterLvGrowLabel);
			}
			#endregion

			#region 合成後の経験値
			[SerializeField]
			UILabel _afterExpLabel = null;
			UILabel AfterExpLabel { get { return _afterExpLabel; } }

			[SerializeField]
			UILabel _afterExpGrowLabel = null;
			UILabel AfterExpGrowLabel { get { return _afterExpGrowLabel; } }

			/// <summary>
			/// 合成後の経験値設定
			/// </summary>
			public void SetAfterExp(long exp, string format)
			{
				if (this.AfterExpLabel != null)
				{
					this.AfterExpLabel.text = string.Format(format, exp);
				}
			}
			/// <summary>
			/// 合成後の経験値カラー設定
			/// </summary>
			public void SetAfterExpColor(StatusColor.Type type)
			{
				StatusColor.Set(type, this.AfterExpLabel, this.AfterExpGrowLabel);
			}
			#endregion

			#region 合成後の次のレベルまでの経験値
			[SerializeField]
			UILabel _afterNextLvExpLabel = null;
			UILabel AfterNextLvExpLabel { get { return _afterNextLvExpLabel; } }

			/// <summary>
			/// 合成後の次のレベルまでの経験値設定
			/// </summary>
			public void SetAfterNextLvExp(long exp, string format)
			{
				if (this.AfterNextLvExpLabel != null)
				{
					this.AfterNextLvExpLabel.text = string.Format(format, exp);
				}
			}
			#endregion

			#region 合成後の経験値バー
			[SerializeField]
			UIProgressBar _afterExpSlider = null;
			UIProgressBar AfterExpSlider { get { return _afterExpSlider; } }
			[SerializeField]
			UIProgressBar _afterTakeExpSlider = null;
			UIProgressBar AfterTakeExpSlider { get { return _afterTakeExpSlider; } }

			/// <summary>
			/// 合成後の経験値バー設定
			/// </summary>
			public void SetAfterExpSlider(float expSlider, float takeExpSlider)
			{
				if (this.AfterExpSlider != null)
				{
					this.AfterExpSlider.value = expSlider;
				}
				if (this.AfterTakeExpSlider != null)
				{
					this.AfterTakeExpSlider.value = takeExpSlider;
				}
			}
			#endregion

			#region 最大レベルかどうか
			[SerializeField]
			GameObject _lvHalfwayGroup = null;
			GameObject LvHalfwayGroup { get { return _lvHalfwayGroup; } }
			[SerializeField]
			GameObject _lvMaxGroup = null;
			GameObject LvMaxGroup { get { return _lvMaxGroup; } }

			/// <summary>
			/// 最大レベルかどうかの切り替え
			/// </summary>
			public void SetLvMax(bool isLvMax)
			{
				if (isLvMax)
				{
					if (this.LvHalfwayGroup != null) this.LvHalfwayGroup.SetActive(false);
					if (this.LvMaxGroup != null) this.LvMaxGroup.SetActive(true);
				}
				else
				{
					if (this.LvHalfwayGroup != null) this.LvHalfwayGroup.SetActive(true);
					if (this.LvMaxGroup != null) this.LvMaxGroup.SetActive(false);
				}
			}
			#endregion

			#region 合成後の余剰経験値
			[SerializeField]
			UILabel _afterOverflowExpLabel = null;
			UILabel AfterOverflowExpLabel { get { return _afterOverflowExpLabel; } }

			/// <summary>
			/// 合成後の余剰経験値設定
			/// </summary>
			public void SetAfterOverflowExp(int overflowExp, string format)
			{
				if (this.AfterOverflowExpLabel != null)
				{
					this.AfterOverflowExpLabel.text = string.Format(format, overflowExp);
				}
			}
			#endregion
			#endregion

			#region 素材リストフィルター
			[SerializeField]
			GameObject _baitFillObject = null;
			GameObject BaitFillObject { get { return _baitFillObject; } }

			/// <summary>
			/// 素材リストフィルターのアクティブ化
			/// </summary>
			public void SetBaitFillActive(bool isActive)
			{
				if (this.BaitFillObject != null)
				{
					this.BaitFillObject.SetActive(isActive);
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
}
