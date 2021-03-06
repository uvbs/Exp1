/// <summary>
/// 進化合成結果表示
/// 
/// 2016/03/03
/// </summary>
using UnityEngine;
using System;

namespace XUI.EvolutionResult
{
	/// <summary>
	/// 進化合成結果表示インターフェイス
	/// </summary>
	public interface IVIew
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

		#region 進化グループ
		/// <summary>
		/// 進化グループのアクティブ化設定
		/// </summary>
		void SetEvolutionActive(bool isActive);
		#endregion

		#region ランクアップグループ
		/// <summary>
		/// ランクアップのアクティブ化設定
		/// </summary>
		void SetRankUpActive(bool isActive);
		#endregion

		#region ランク
		/// <summary>
		/// 合成前のランクを設定する
		/// </summary>
		void SetBeforeRank(int rank);

		/// <summary>
		/// 合成後のランクを設定する
		/// </summary>
		void SetAfterRank(int rank);
		#endregion

		#region シンクロ合成残り回数
		/// <summary>
		/// シンクロ合成残り回数を設定する
		/// </summary>
		void SetSynchroRemain(int remain);

		/// <summary>
		/// シンクロ合成残り回数アップ分を設定する
		/// </summary>
		void SetSynchroRemainUp(int up, string format);
		#endregion

		#region 強化レベル
		/// <summary>
		/// 強化レベルを設定する
		/// </summary>
		void SetLv(int lv, string format);
		#endregion

		#region 生命力
		/// <summary>
		/// 生命力を設定する
		/// </summary>
		void SetHitPoint(int hitPoint, string format);
		#endregion

		#region 攻撃力
		/// <summary>
		/// 攻撃力を設定する
		/// </summary>
		void SetAttack(int attack, string format);
		#endregion

		#region 防御力
		/// <summary>
		/// 防御力を設定する
		/// </summary>
		void SetDefence(int defence, string format);
		#endregion

		#region 特殊能力
		/// <summary>
		/// 特殊能力を設定する
		/// </summary>
		void SetExtra(int extra, string format);
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
	/// 進化合成結果表示
	/// </summary>
	public class EvolutionResultView : GUIViewBase, IVIew
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void OnDestroy()
		{
			this.OnOK = null;
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

		#region 進化グループ
		[SerializeField]
		private GameObject _evolutionGroup = null;
		private GameObject EvolutionGroup { get { return _evolutionGroup; } }

		/// <summary>
		/// 進化グループのアクティブ化設定
		/// </summary>
		public void SetEvolutionActive(bool isActive)
		{
			if(this.EvolutionGroup != null)
			{
				this.EvolutionGroup.SetActive(isActive);
			}
		}
		#endregion
		
		#region ランクアップグループ
		[SerializeField]
		private GameObject _rankUpGroup = null;
		private GameObject RankUpGroup { get { return _rankUpGroup; } }

		/// <summary>
		/// ランクアップのアクティブ化設定
		/// </summary>
		public void SetRankUpActive(bool isActive)
		{
			if (this.RankUpGroup != null)
			{
				this.RankUpGroup.SetActive(isActive);
			}
		}
		#endregion

		#region ランク
		[SerializeField]
		private UILabel _beforeRankLabel = null;
		private UILabel BeforeRankLabel { get { return _beforeRankLabel; } }
		[SerializeField]
		private UILabel _afterRankLabel = null;
		private UILabel AfterRankLabel { get { return _afterRankLabel; } }

		/// <summary>
		/// 合成前のランクを設定する
		/// </summary>
		public void SetBeforeRank(int rank)
		{
			if(this.BeforeRankLabel != null)
			{
				this.BeforeRankLabel.text = rank.ToString();
			}
		}
		/// <summary>
		/// 合成後のランクを設定する
		/// </summary>
		public void SetAfterRank(int rank)
		{
			if(this.AfterRankLabel != null)
			{
				this.AfterRankLabel.text = rank.ToString();
			}
		}
		#endregion

		#region シンクロ合成残り回数
		[SerializeField]
		private UILabel _synchroRemainLabel = null;
		private UILabel SynchroRemainLabel { get { return _synchroRemainLabel; } }
		[SerializeField]
		private UILabel _synchroRemainUpLabel = null;
		private UILabel SynchroRemainUpLabel { get { return _synchroRemainUpLabel; } }

		/// <summary>
		/// シンクロ合成残り回数を設定する
		/// </summary>
		public void SetSynchroRemain(int remain)
		{
			if(this.SynchroRemainLabel != null)
			{
				this.SynchroRemainLabel.text = remain.ToString();
			}
		}
		/// <summary>
		/// シンクロ合成残り回数アップ分を設定する
		/// </summary>
		public void SetSynchroRemainUp(int up, string format)
		{
			if(this.SynchroRemainUpLabel != null)
			{
				this.SynchroRemainUpLabel.text = string.Format(format, up);
			}
		}
		#endregion

		#region 強化レベル
		[SerializeField]
		private UILabel _lvLabel = null;
		private UILabel LvLabel { get { return _lvLabel; } }

		/// <summary>
		/// 強化レベルを設定する
		/// </summary>
		public void SetLv(int lv, string format)
		{
			if(this.LvLabel != null)
			{
				this.LvLabel.text = string.Format(format, lv);
			}
		}
		#endregion

		#region 生命力
		[SerializeField]
		private UILabel _hitPointLabel = null;
		private UILabel HitPointLabel { get { return _hitPointLabel; } }

		/// <summary>
		/// 生命力を設定する
		/// </summary>
		public void SetHitPoint(int hitPoint, string format)
		{
			if(this.HitPointLabel != null)
			{
				this.HitPointLabel.text = string.Format(format, hitPoint);
			}
		}
		#endregion

		#region 攻撃力
		[SerializeField]
		private UILabel _attackLabel = null;
		private UILabel AttackLabel { get { return _attackLabel; } }

		/// <summary>
		/// 攻撃力を設定する
		/// </summary>
		public void SetAttack(int attack, string format)
		{
			if(this.AttackLabel != null)
			{
				this.AttackLabel.text = string.Format(format, attack);
			}
		}
		#endregion

		#region 防御力
		[SerializeField]
		private UILabel _defenceLabel = null;
		private UILabel DefenceLabel { get { return _defenceLabel; } }

		/// <summary>
		/// 防御力を設定する
		/// </summary>
		public void SetDefence(int defence, string format)
		{
			if(this.DefenceLabel != null)
			{
				this.DefenceLabel.text = string.Format(format, defence);
			}
		}
		#endregion

		#region 特殊能力
		[SerializeField]
		private UILabel _extraLabel = null;
		private UILabel ExtraLabel { get { return _extraLabel; } }

		/// <summary>
		/// 特殊能力を設定する
		/// </summary>
		public void SetExtra(int extra, string format)
		{
			if(this.ExtraLabel != null)
			{
				this.ExtraLabel.text = string.Format(format, extra);
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
			this.OnOK(this, EventArgs.Empty);
		}

		[SerializeField]
		private UIButton _okButton = null;
		private UIButton OKButton { get { return _okButton; } }

		/// <summary>
		/// OKボタンの有効設定
		/// </summary>
		public void SetOKButtonEnable(bool isEnable)
		{
			if(this.OKButton != null)
			{
				this.OKButton.isEnabled = isEnable;
			}
		}
		#endregion

		#region 立ち絵
		[SerializeField]
		private Transform _boardRoot = null;
		private Transform BoardRoot { get { return _boardRoot; } }
		[SerializeField]
		private UIPlayTween _boardPlayTween = null;
		private UIPlayTween BoardPlayTween { get { return _boardPlayTween; } }

		/// <summary>
		/// 立ち絵設定
		/// </summary>
		public void SetBoardRoot(Transform boardTrans)
		{
			this.RemoveBoard();
			if(this.BoardRoot != null && boardTrans != null)
			{
				boardTrans.parent = this.BoardRoot;
			}
		}
		/// <summary>
		/// 立ち絵削除
		/// </summary>
		private void RemoveBoard()
		{
			if(this.BoardRoot != null)
			{
				for(int i = 0; i < this.BoardRoot.childCount; ++i)
				{
					var child = this.BoardRoot.GetChild(i);
					Destroy(child.gameObject);
				}
			}
		}
		/// <summary>
		/// 立ち絵リプレイ
		/// </summary>
		public void ReplayBoard(bool forward)
		{
			if(this.BoardPlayTween != null)
			{
				this.BoardPlayTween.Play(forward);
			}
		}
		#endregion
	}
}