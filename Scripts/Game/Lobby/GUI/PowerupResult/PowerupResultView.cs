/// <summary>
/// 強化合成結果表示
/// 
/// 2016/01/28
/// </summary>
using UnityEngine;
using System;

namespace XUI
{
	namespace PowerupResult
	{
		/// <summary>
		/// 強化合成結果表示インターフェイス
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

			#region レベルアップグループ
			/// <summary>
			/// レベルアップのアクティブ化設定
			/// </summary>
			void SetLvUpActive(bool isActive);
			#endregion

			#region レベル
			/// <summary>
			/// 合成前のレベルを設定する
			/// </summary>
			void SetBeforeLv(int lv, string format);
			/// <summary>
			/// 合成後のレベルを設定する
			/// </summary>
			void SetAfterLv(int lv, string format);
			#endregion

			#region 経験値バー
			/// <summary>
			/// 経験値バーを設定する
			/// </summary>
			void SetExpSlider(float sliderValue);
			#endregion

			#region 生命力
			/// <summary>
			/// 合成後の生命力を設定する
			/// </summary>
			void SetHitPoint(int hp, string format);
			/// <summary>
			/// レベルアップによる生命力のアップ分を設定する
			/// </summary>
			void SetHitPointUp(int up, string format);
			#endregion

			#region 攻撃力
			/// <summary>
			/// 合成後の攻撃力を設定する
			/// </summary>
			void SetAttack(int attack, string format);
			/// <summary>
			/// レベルアップによる攻撃力のアップ分を設定する
			/// </summary>
			void SetAttackUp(int up, string format);
			#endregion

			#region 防御力
			/// <summary>
			/// 合成後の防御力を設定する
			/// </summary>
			void SetDefence(int defence, string format);
			/// <summary>
			/// レベルアップによる防御力のアップ分を設定する
			/// </summary>
			void SetDefenceUp(int up, string format);
			#endregion

			#region 特殊能力
			/// <summary>
			/// 合成後の特殊能力を設定する
			/// </summary>
			void SetExtra(int extra, string format);
			/// <summary>
			/// レベルアップによる特殊能力のアップ分を設定する
			/// </summary>
			void SetExtraUp(int up, string format);
			#endregion

			#region OKボタン
			/// <summary>
			/// OKボタンイベント
			/// </summary>
			event EventHandler OnOK;

			/// <summary>
			/// OKボタンの有効設定
			/// </summary>
			void SetOKButtonEnable(bool isEnable);
			#endregion

			#region 立ち絵
			/// <summary>
			/// 立ち絵設定
			/// </summary>
			void SetBoardRoot(Transform boardTrans);
			/// <summary>
			/// 立ち絵リプレイ
			/// </summary>
			void ReplayBoard(bool forward);
			#endregion
		}

		/// <summary>
		/// 強化合成結果表示
		/// </summary>
		public class PowerupResultView : GUIViewBase, IView
		{
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

			#region レベルアップグループ
			[SerializeField]
			GameObject _lvUpGroup = null;
			GameObject LvUpGroup { get { return _lvUpGroup; } }

			/// <summary>
			/// レベルアップのアクティブ化設定
			/// </summary>
			public void SetLvUpActive(bool isActive)
			{
				if (this.LvUpGroup != null)
				{
					this.LvUpGroup.SetActive(isActive);
				}
			}
			#endregion

			#region レベル
			[SerializeField]
			UILabel _beforeLvLabel = null;
			UILabel BeforeLvLabel { get { return _beforeLvLabel; } }
			[SerializeField]
			UILabel _afterLvLabel = null;
			UILabel AfterLvLabel { get { return _afterLvLabel; } }

			/// <summary>
			/// 合成前のレベルを設定する
			/// </summary>
			public void SetBeforeLv(int lv, string format)
			{
				if (this.BeforeLvLabel != null)
				{
					this.BeforeLvLabel.text = string.Format(format, lv);
				}
			}
			/// <summary>
			/// 合成後のレベルを設定する
			/// </summary>
			public void SetAfterLv(int lv, string format)
			{
				if (this.AfterLvLabel != null)
				{
					this.AfterLvLabel.text = string.Format(format, lv);
				}
			}
			#endregion

			#region 経験値バー
			[SerializeField]
			UIProgressBar _expSlider = null;
			UIProgressBar ExpSlider { get { return _expSlider; } }

			/// <summary>
			/// 経験値バーを設定する
			/// </summary>
			public void SetExpSlider(float sliderValue)
			{
				if (this.ExpSlider != null)
				{
					this.ExpSlider.value = sliderValue;
				}
			}
			#endregion

			#region 生命力
			[SerializeField]
			UILabel _hitPointLabel = null;
			UILabel HitPointLabel { get { return _hitPointLabel; } }
			[SerializeField]
			UILabel _hitPointUpLabel = null;
			UILabel HitPointUpLabel { get { return _hitPointUpLabel; } }

			/// <summary>
			/// 合成後の生命力を設定する
			/// </summary>
			public void SetHitPoint(int hp, string format)
			{
				if (this.HitPointLabel != null)
				{
					this.HitPointLabel.text = string.Format(format, hp);
				}
			}
			/// <summary>
			/// レベルアップによる生命力のアップ分を設定する
			/// </summary>
			public void SetHitPointUp(int up, string format)
			{
				if (this.HitPointUpLabel != null)
				{
					this.HitPointUpLabel.text = string.Format(format, up);
				}
			}
			#endregion

			#region 攻撃力
			[SerializeField]
			UILabel _attackLabel = null;
			UILabel AttackLabel { get { return _attackLabel; } }
			[SerializeField]
			UILabel _attackUpLabel = null;
			UILabel AttackUpLabel { get { return _attackUpLabel; } }

			/// <summary>
			/// 合成後の攻撃力を設定する
			/// </summary>
			public void SetAttack(int attack, string format)
			{
				if (this.AttackLabel != null)
				{
					this.AttackLabel.text = string.Format(format, attack);
				}
			}
			/// <summary>
			/// レベルアップによる攻撃力のアップ分を設定する
			/// </summary>
			public void SetAttackUp(int up, string format)
			{
				if (this.AttackUpLabel != null)
				{
					this.AttackUpLabel.text = string.Format(format, up);
				}
			}
			#endregion

			#region 防御力
			[SerializeField]
			UILabel _defenceLabel = null;
			UILabel DefenceLabel { get { return _defenceLabel; } }
			[SerializeField]
			UILabel _defenceUpLabel = null;
			UILabel DefenceUpLabel { get { return _defenceUpLabel; } }

			/// <summary>
			/// 合成後の防御力を設定する
			/// </summary>
			public void SetDefence(int defence, string format)
			{
				if (this.DefenceLabel != null)
				{
					this.DefenceLabel.text = string.Format(format, defence);
				}
			}
			/// <summary>
			/// レベルアップによる防御力のアップ分を設定する
			/// </summary>
			public void SetDefenceUp(int up, string format)
			{
				if (this.DefenceUpLabel != null)
				{
					this.DefenceUpLabel.text = string.Format(format, up);
				}
			}
			#endregion

			#region 特殊能力
			[SerializeField]
			UILabel _extraLabel = null;
			UILabel ExtraLabel { get { return _extraLabel; } }
			[SerializeField]
			UILabel _extraUpLabel = null;
			UILabel ExtraUpLabel { get { return _extraUpLabel; } }

			/// <summary>
			/// 合成後の特殊能力を設定する
			/// </summary>
			public void SetExtra(int extra, string format)
			{
				if (this.ExtraLabel != null)
				{
					this.ExtraLabel.text = string.Format(format, extra);
				}
			}
			/// <summary>
			/// レベルアップによる特殊能力のアップ分を設定する
			/// </summary>
			public void SetExtraUp(int up, string format)
			{
				if (this.ExtraUpLabel != null)
				{
					this.ExtraUpLabel.text = string.Format(format, up);
				}
			}
			#endregion

			#region OKボタン
			/// <summary>
			/// OKボタンイベント
			/// </summary>
			public event EventHandler OnOK = (sender, e) => { };

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
			/// OKボタンの有効設定
			/// </summary>
			public void SetOKButtonEnable(bool isEnable)
			{
				if (this.OKButton != null)
				{
					this.OKButton.isEnabled = isEnable;
				}
			}
			#endregion

			#region 立ち絵
			[SerializeField]
			Transform _boardRoot = null;
			Transform BoardRoot { get { return _boardRoot; } }
			[SerializeField]
			UIPlayTween _boardPlayTween = null;
			UIPlayTween BoardPlayTween { get { return _boardPlayTween; } }

			/// <summary>
			/// 立ち絵設定
			/// </summary>
			public void SetBoardRoot(Transform boardTrans)
			{
				this.RemoveBoard();
				if (this.BoardRoot != null && boardTrans != null)
				{
					boardTrans.parent = this.BoardRoot;
				}
			}
			void RemoveBoard()
			{
				if (this.BoardRoot != null)
				{
					for (int i = 0; i < this.BoardRoot.childCount; i++)
					{
						var child = this.BoardRoot.GetChild(i);
						UnityEngine.Object.Destroy(child.gameObject);
					}
				}
			}
			/// <summary>
			/// 立ち絵リプレイ
			/// </summary>
			public void ReplayBoard(bool forward)
			{
				if (this.BoardPlayTween != null)
				{
					this.BoardPlayTween.Play(forward);
				}
			}
			#endregion
		}
	}
}
