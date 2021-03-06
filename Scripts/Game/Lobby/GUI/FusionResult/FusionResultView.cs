/// <summary>
/// 合成結果表示
/// 
/// 2016/05/17
/// </summary>
using UnityEngine;
using System;
using System.Collections;

namespace XUI.FusionResult
{
	/// <summary>
	/// 合成結果表示インターフェイス
	/// </summary>
	public interface IView
	{
		#region アクティブ
		/// <summary>
		/// ボード（立ち絵）が消されてからのアクティブ化時の通知用
		/// </summary>
		event EventHandler OnWaitDeleteBoardRootActive;

		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		void SetActive(bool isActive, bool isTweenSkip);

		/// <summary>
		/// 現在のアクティブ状態を取得する
		/// </summary>
		GUIViewBase.ActiveState GetActiveState();
		#endregion

		#region OKボタン
		/// <summary>
		/// OKボタン押下イベント通知用
		/// </summary>
		event EventHandler OnOK;
		#endregion

		#region リザルト状態
		/// <summary>
		/// 強化時の表示設定
		/// </summary>
		void SetPowerup(bool isActive);

		/// <summary>
		/// 進化時の表示設定
		/// </summary>
		void SetEvolution(bool isActive);

		/// <summary>
		/// ボーナス時の表示設定
		/// </summary>
		void SetBonus(bool isActive);
		#endregion

		#region 強化合成結果
		/// <summary>
		/// 強化合成結果通常設定
		/// </summary>
		void SetGood(bool isActive);

		/// <summary>
		/// 強化合成結果成功設定
		/// </summary>
		void SetBigSuccess(bool isActive);

		/// <summary>
		/// 強化合成結果大成功設定
		/// </summary>
		void SetSuperSuccess(bool isActive);
		#endregion

		#region キャラステータス
		#region ランク
		/// <summary>
		/// 変化前設定
		/// </summary>
		void SetRankBefore(string rank);

		/// <summary>
		/// 変化後設定
		/// </summary>
		void SetRankAfter(bool isActive, string rank);

		/// <summary>
		/// ランク色設定
		/// </summary>
		void SetRankColor(StatusColor.Type beforeType, StatusColor.Type afterType);
		#endregion

		#region レベル
		/// <summary>
		/// レベル変化前設定
		/// </summary>
		void SetLevelBefore(string lv);

		/// <summary>
		/// レベル変化後設定
		/// </summary>
		void SetLevelAfter(bool isActive, string lv);

		/// <summary>
		/// レベル色設定
		/// </summary>
		void SetLevelColor(StatusColor.Type type);

		/// <summary>
		/// レベルメッセージ設定
		/// </summary>
		void SetLevelMessage(string message);
		#endregion

		#region 経験値
		/// <summary>
		/// 経験値表示設定
		/// </summary>
		void SetExpActive(bool isActive);

		/// <summary>
		/// 経験値バー設定
		/// </summary>
		void SetExpBar(float value);
		#endregion

		#region シンクロ可能回数
		/// <summary>
		/// シンクロ可能回数変化前設定
		/// </summary>
		void SetSynchroRemainBefore(string remain);

		/// <summary>
		/// シンクロ可能回数変化後設定
		/// </summary>
		void SetSynchroRemainAfter(bool isActive, string remain);

		/// <summary>
		/// シンクロ可能回数色設定
		/// </summary>
		void SetSynchroRemainColor(StatusColor.Type type);
		#endregion

		#region 生命力
		/// <summary>
		/// 生命力設定
		/// </summary>
		void SetHitPointTotal(int total, string format, StatusColor.Type type);

		/// <summary>
		/// 生命力ベース設定
		/// </summary>
		void SetHitPointBase(int baseNum, string format, StatusColor.Type type);

		/// <summary>
		/// 生命力シンクロ設定
		/// </summary>
		void SetSynchroHitPoint(int synchro, string format, StatusColor.Type type);

		/// <summary>
		/// 生命力スロット設定
		/// </summary>
		void SetSlotHitPoint(int slot, string format, StatusColor.Type type);

		/// <summary>
		/// 生命力変化分設定
		/// </summary>
		void SetHitPointUp(int up, string format, StatusColor.Type type);

		/// <summary>
		/// 生命力の増減アイコン設定
		/// </summary>
		void SetHitPointIcon(FusionResultView.StatusIcon.Type type);
		#endregion

		#region 攻撃力
		/// <summary>
		/// 攻撃力設定
		/// </summary>
		void SetAttackTotal(int total, string format, StatusColor.Type type);

		/// <summary>
		/// 攻撃力ベース設定
		/// </summary>
		void SetAttackBase(int baseNum, string format, StatusColor.Type type);

		/// <summary>
		/// 攻撃力シンクロ設定
		/// </summary>
		void SetSynchroAttack(int synchro, string format, StatusColor.Type type);

		/// <summary>
		/// 攻撃力スロット設定
		/// </summary>
		void SetSlotAttack(int slot, string format, StatusColor.Type type);

		/// <summary>
		/// 攻撃力変化分設定
		/// </summary>
		void SetAttackUp(int up, string format, StatusColor.Type type);

		/// <summary>
		/// 攻撃力の増減アイコン設定
		/// </summary>
		void SetAttackIcon(FusionResultView.StatusIcon.Type type);
		#endregion

		#region 防御力
		/// <summary>
		/// 防御力設定
		/// </summary>
		void SetDefenseTotal(int total, string format, StatusColor.Type type);

		/// <summary>
		/// 防御力ベース設定
		/// </summary>
		void SetDefenseBase(int baseNum, string format, StatusColor.Type type);

		/// <summary>
		/// 防御力シンクロ設定
		/// </summary>
		void SetSynchroDefense(int synchro, string format, StatusColor.Type type);

		/// <summary>
		/// 防御力スロット設定
		/// </summary>
		void SetSlotDefense(int slot, string format, StatusColor.Type type);

		/// <summary>
		/// 防御力変化分設定
		/// </summary>
		void SetDefenseUp(int up, string format, StatusColor.Type type);

		/// <summary>
		/// 防御力の増減アイコン設定
		/// </summary>
		void SetDefenseIcon(FusionResultView.StatusIcon.Type type);
		#endregion

		#region 特殊能力
		/// <summary>
		/// 特殊能力設定
		/// </summary>
		void SetExtraTotal(int total, string format, StatusColor.Type type);

		/// <summary>
		/// 特殊能力ベース設定
		/// </summary>
		void SetExtraBase(int baseNum, string format, StatusColor.Type type);

		/// <summary>
		/// 特殊能力シンクロ設定
		/// </summary>
		void SetSynchroExtra(int synchro, string format, StatusColor.Type type);

		/// <summary>
		/// 特殊能力スロット設定
		/// </summary>
		void SetSlotExtra(int slot, string format, StatusColor.Type type);

		/// <summary>
		/// 特殊能力変化分設定
		/// </summary>
		void SetExtraUp(int up, string format, StatusColor.Type type);

		/// <summary>
		/// 特殊能力の増減アイコン設定
		/// </summary>
		void SetExtraIcon(FusionResultView.StatusIcon.Type type);
		#endregion
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
	/// 合成結果表示
	/// </summary>
	public class FusionResultView : GUIViewBase, IView
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void OnDestroy()
		{
			this.OnOK = null;
			this.OnWaitDeleteBoardRootActive = null;
		}
		#endregion

		#region アクティブ
		/// <summary>
		/// ボード（立ち絵）が消されてからのアクティブ化時の通知用
		/// </summary>
		public event EventHandler OnWaitDeleteBoardRootActive = (sender, e) => { };

		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		public void SetActive(bool isActive, bool isTweenSkip)
		{
			if (isActive)
			{
				// ボード削除
				this.RemoveBoard();
				// ボードが消されてからウィンドウをアクティブ化する
				FiberController.AddFiber(this.WaitDeleteBoardRootActiveCoroutine(isTweenSkip));
			}
			else
			{
				this.SetRootActive(isActive, isTweenSkip);
			}
		}

		/// <summary>
		/// ボード（立ち絵）が消されてからウィンドウをアクティブ化する
		/// </summary>
		IEnumerator WaitDeleteBoardRootActiveCoroutine(bool isTweenSkip)
		{
			// ボードが消されていなかったら待機
			while (this.BoardRoot != null && this.BoardRoot.childCount >= 1)
			{
				yield return null;
			}

			this.SetRootActive(true, isTweenSkip);

			// 通知
			this.OnWaitDeleteBoardRootActive(this, EventArgs.Empty);
		}

		/// <summary>
		/// 現在のアクティブ状態を取得する
		/// </summary>
		public GUIViewBase.ActiveState GetActiveState()
		{
			return this.GetRootActiveState();
		}
		#endregion

		#region OKボタン
		/// <summary>
		/// OKボタン押下イベント通知用
		/// </summary>
		public event EventHandler OnOK = (sender, e) => { };

		/// <summary>
		/// OKボタン押下(NGUIリフレクション)
		/// </summary>
		public void OnOkEvent()
		{
			// 通知
			this.OnOK(this, EventArgs.Empty);
		}
		#endregion

		#region リザルト状態
		/// <summary>
		/// リザルトアタッチ
		/// </summary>
		[SerializeField]
		private ResultModeAttachObject _resultModeAttach = null;
		private ResultModeAttachObject ResultModeAttach { get { return _resultModeAttach; } }
		[Serializable]
		public class ResultModeAttachObject
		{
			/// <summary>
			/// 強化親オブジェクト
			/// </summary>
			[SerializeField]
			private GameObject _powerupParentObj = null;
			public GameObject PowerupParentObj { get { return _powerupParentObj; } }

			/// <summary>
			/// 進化親オブジェクト
			/// </summary>
			[SerializeField]
			private GameObject _evolutionParentObj = null;
			public GameObject EvolutionParentObj { get { return _evolutionParentObj; } }

			/// <summary>
			/// ボーナス親オブジェクト
			/// </summary>
			[SerializeField]
			private GameObject _bonusParentObj = null;
			public GameObject BonusParentObj { get { return _bonusParentObj; } }
		}

		/// <summary>
		/// 強化合成時の表示設定
		/// </summary>
		public void SetPowerup(bool isActive)
		{
			if (this.ResultModeAttach == null || this.ResultModeAttach.PowerupParentObj == null) { return; }
			this.ResultModeAttach.PowerupParentObj.SetActive(isActive);
		}

		/// <summary>
		/// 進化合成時の表示設定
		/// </summary>
		public void SetEvolution(bool isActive)
		{
			if (this.ResultModeAttach == null || this.ResultModeAttach.EvolutionParentObj == null) { return; }
			this.ResultModeAttach.EvolutionParentObj.SetActive(isActive);
		}

		/// <summary>
		/// ボーナス時の表示設定
		/// </summary>
		public void SetBonus(bool isActive)
		{
			if (this.ResultModeAttach == null || this.ResultModeAttach.BonusParentObj == null) { return; }
			this.ResultModeAttach.BonusParentObj.SetActive(isActive);
		}
		#endregion

		#region 強化合成結果
		/// <summary>
		/// 強化合成結果アタッチ
		/// </summary>
		[SerializeField]
		private PowerupResultAttachObject _powerupResultAttach = null;
		private PowerupResultAttachObject PowerupResultAttach { get { return _powerupResultAttach; } }
		[Serializable]
		public class PowerupResultAttachObject
		{
			/// <summary>
			/// 通常
			/// </summary>
			[SerializeField]
			private GameObject _goodParentObj = null;
			public GameObject GoodParentObj { get { return _goodParentObj; } }

			/// <summary>
			/// 成功
			/// </summary>
			[SerializeField]
			private GameObject _bigSuccessParentObj = null;
			public GameObject BigSuccessParentObj { get { return _bigSuccessParentObj; } }

			/// <summary>
			/// 大成功
			/// </summary>
			[SerializeField]
			private GameObject _superSuccessParentObj = null;
			public GameObject SuperSuccessParentObj { get { return _superSuccessParentObj; } }
		}

		/// <summary>
		/// 強化合成結果通常設定
		/// </summary>
		public void SetGood(bool isActive)
		{
			if (this.PowerupResultAttach == null || this.PowerupResultAttach.GoodParentObj == null) { return; }
			this.PowerupResultAttach.GoodParentObj.SetActive(isActive);
		}

		/// <summary>
		/// 強化合成結果成功設定
		/// </summary>
		public void SetBigSuccess(bool isActive)
		{
			if (this.PowerupResultAttach == null || this.PowerupResultAttach.BigSuccessParentObj == null) { return; }
			this.PowerupResultAttach.BigSuccessParentObj.SetActive(isActive);
		}

		/// <summary>
		/// 強化合成結果大成功設定
		/// </summary>
		public void SetSuperSuccess(bool isActive)
		{
			if (this.PowerupResultAttach == null || this.PowerupResultAttach.SuperSuccessParentObj == null) { return; }
			this.PowerupResultAttach.SuperSuccessParentObj.SetActive(isActive);
		}
		#endregion

		#region キャラクターステータス
		[SerializeField]
		private CharaStatusAttachObject _charaStatusAttach = null;
		private CharaStatusAttachObject CharaStatusAttach { get { return _charaStatusAttach; } }
		[Serializable]
		public class CharaStatusAttachObject
		{
			/// <summary>
			/// ランク
			/// </summary>
			[SerializeField]
			private RankAttachObject _rankAttach = null;
			public RankAttachObject RankAttach { get { return _rankAttach; } }

			/// <summary>
			/// レベル
			/// </summary>
			[SerializeField]
			private LevelAttachObject _levelAttach = null;
			public LevelAttachObject LevelAttach { get { return _levelAttach; } }

			/// <summary>
			/// 経験値親オブジェクト
			/// </summary>
			[SerializeField]
			private GameObject _expParentObj = null;
			public GameObject ExpParentObj { get { return _expParentObj; } }

			/// <summary>
			/// 経験値バー
			/// </summary>
			[SerializeField]
			private UIProgressBar _expBar = null;
			public UIProgressBar ExpBar { get { return _expBar; } }

			/// <summary>
			/// シンクロ可能回数
			/// </summary>
			[SerializeField]
			private SynchroRemainAttachObject _synchroRemainAttach = null;
			public SynchroRemainAttachObject SynchroRemainAttach { get { return _synchroRemainAttach; } }

			/// <summary>
			/// 生命力
			/// </summary>
			[SerializeField]
			private PowerParamAttachObject _hitPointAttach = null;
			public PowerParamAttachObject HitPointAttach { get { return _hitPointAttach; } }

			/// <summary>
			/// 攻撃力
			/// </summary>
			[SerializeField]
			private PowerParamAttachObject _attackAttach = null;
			public PowerParamAttachObject AttackAttach { get { return _attackAttach; } }

			/// <summary>
			/// 防御力
			/// </summary>
			[SerializeField]
			private PowerParamAttachObject _defenseAttach = null;
			public PowerParamAttachObject DefenseAttach { get { return _defenseAttach; } }

			/// <summary>
			/// 特殊能力
			/// </summary>
			[SerializeField]
			private PowerParamAttachObject _extraAttach = null;
			public PowerParamAttachObject ExtraAttach { get { return _extraAttach; } }
		}

		/// <summary>
		/// ランクアタッチクラス
		/// </summary>
		[Serializable]
		public class RankAttachObject
		{
			/// <summary>
			/// 変化前値ラベル
			/// </summary>
			[SerializeField]
			private UILabel _beforeLabel = null;
			private UILabel BeforeLabel { get { return _beforeLabel; } }

			/// <summary>
			/// ベースの変化前の星スプライト
			/// </summary>
			[SerializeField]
			private UISprite _baseBeforeStarSprite = null;
			private UISprite BaseBeforeStarSprite { get { return _baseBeforeStarSprite; } }

			/// <summary>
			/// 変化後の親オブジェクト
			/// </summary>
			[SerializeField]
			private GameObject _afterParentObj = null;
			private GameObject AfterParentObj { get { return _afterParentObj; } }

			/// <summary>
			/// ベースの増加の星スプライト
			/// </summary>
			[SerializeField]
			private UISprite _baseUpStarSprite = null;
			private UISprite BaseUpStarSprite { get { return _baseUpStarSprite; } }

			/// <summary>
			/// エフェクト用増加の星スプライト
			/// </summary>
			[SerializeField]
			private UISprite _growUpStarSprite = null;
			private UISprite GrowUpStarSprite { get { return _growUpStarSprite; } }

			/// <summary>
			/// 変化後ラベル
			/// </summary>
			[SerializeField]
			private StatusLabel _afterLabel = null;
			public StatusLabel AfterLabel { get { return _afterLabel; } }

			/// <summary>
			/// 矢印
			/// </summary>
			[SerializeField]
			private GameObject _arrowObj = null;
			private GameObject ArrowObj { get { return _arrowObj; } }

			/// <summary>
			/// 変化前設定
			/// </summary>
			public void SetBefore(string rank)
			{
				if (this.BeforeLabel == null) { return; }
				this.BeforeLabel.text = rank;

			}
			/// <summary>
			/// 変化後のアクティブとテキスト設定
			/// </summary>
			public void SetAfterActive(bool isActive, string rank)
			{
				if (this.AfterParentObj != null)
				{
					this.AfterParentObj.SetActive(isActive);
				}
				if (this.AfterLabel != null)
				{
					this.AfterLabel.SetText(rank);
				}
			}
			/// <summary>
			/// 色の設定
			/// </summary>
			public void SetColor(StatusColor.Type beforeType, StatusColor.Type afterType)
			{
				if (this.BeforeLabel != null)
				{
					StatusColor.Set(beforeType, this.BeforeLabel, null);
				}
				if (this.AfterLabel != null)
				{
					this.AfterLabel.SetColor(afterType);
				}
				StatusColor.Set(beforeType, this.BaseBeforeStarSprite, null);
				StatusColor.Set(afterType, this.BaseUpStarSprite, this.GrowUpStarSprite);
			}
		}

		/// <summary>
		/// レベルアタッチクラス
		/// </summary>
		[Serializable]
		public class LevelAttachObject
		{
			/// <summary>
			/// 変化前ラベル
			/// </summary>
			[SerializeField]
			private UILabel _beforeLabel = null;
			private UILabel BeforeLabel { get { return _beforeLabel; } }

			/// <summary>
			/// 変化後の親オブジェクト
			/// </summary>
			[SerializeField]
			private GameObject _afterParentObj = null;
			private GameObject AfterParentObj { get { return _afterParentObj; } }

			/// <summary>
			/// 変化後値ラベル
			/// </summary>
			[SerializeField]
			private StatusLabel _afterLabel = null;
			private StatusLabel AfterLabel { get { return _afterLabel; } }

			/// <summary>
			/// 矢印
			/// </summary>
			[SerializeField]
			private GameObject _arrowObj = null;
			private GameObject ArrowObj { get { return _arrowObj; } }

			/// <summary>
			/// メッセージラベル
			/// </summary>
			[SerializeField]
			private UILabel _messageLabel = null;
			private UILabel MessageLabel { get { return _messageLabel; } }

			/// <summary>
			/// 変化前値設定
			/// </summary>
			public void SetBefore(string num)
			{
				if (this.BeforeLabel == null) { return; }
				this.BeforeLabel.text = num;
			}
			/// <summary>
			/// 変化後値設定
			/// </summary>
			public void SetAfter(bool isActive, string num)
			{
				if (this.AfterParentObj != null)
				{
					this.AfterParentObj.SetActive(isActive);
				}
				if (this.AfterLabel != null)
				{
					this.AfterLabel.SetText(num);
				}
			}
			/// <summary>
			/// 色設定
			/// </summary>
			public void SetColor(StatusColor.Type type)
			{
				if (this.AfterLabel == null) { return; }
				this.AfterLabel.SetColor(type);
			}
			/// <summary>
			/// メッセージ設定
			/// </summary>
			public void SetMessage(string message)
			{
				if (this.MessageLabel == null) { return; }
				this.MessageLabel.text = message;
			}
		}

		/// <summary>
		/// シンクロ可能回数アタッチ
		/// </summary>
		[Serializable]
		public class SynchroRemainAttachObject
		{
			/// <summary>
			/// 変化前ラベル
			/// </summary>
			[SerializeField]
			private UILabel _beforeLabel = null;
			private UILabel BeforeLabel { get { return _beforeLabel; } }

			/// <summary>
			/// 変化後の親オブジェクト
			/// </summary>
			[SerializeField]
			private GameObject _afterParentObj = null;
			private GameObject AfterParentObj { get { return _afterParentObj; } }

			/// <summary>
			/// 変化後値ラベル
			/// </summary>
			[SerializeField]
			private StatusLabel _afterLabel = null;
			private StatusLabel AfterLabel { get { return _afterLabel; } }

			/// <summary>
			/// 矢印
			/// </summary>
			[SerializeField]
			private GameObject _arrowObj = null;
			private GameObject ArrowObj { get { return _arrowObj; } }

			/// <summary>
			/// 変化前値設定
			/// </summary>
			public void SetBefore(string num)
			{
				if (this.BeforeLabel == null) { return; }
				this.BeforeLabel.text = num;
			}
			/// <summary>
			/// 変化後値設定
			/// </summary>
			public void SetAfter(bool isActive, string num)
			{
				if (this.AfterParentObj != null)
				{
					this.AfterParentObj.SetActive(isActive);
				}
				if (this.AfterLabel != null)
				{
					this.AfterLabel.SetText(num);
				}
			}
			/// <summary>
			/// 色設定
			/// </summary>
			public void SetColor(StatusColor.Type type)
			{
				if (this.AfterLabel == null) { return; }
				this.AfterLabel.SetColor(type);
			}
		}

		/// <summary>
		/// 能力パラメータアタッチクラス
		/// </summary>
		[Serializable]
		public class PowerParamAttachObject
		{
			/// <summary>
			/// 合計力
			/// </summary>
			[SerializeField]
			private StatusLabel _totalLabel = null;
			public StatusLabel TotalLabel { get { return _totalLabel; } }

			/// <summary>
			/// ベース
			/// </summary>
			[SerializeField]
			private StatusLabel _baseLabel = null;
			public StatusLabel BaseLabel { get { return _baseLabel; } }

			/// <summary>
			/// シンクロ
			/// </summary>
			[SerializeField]
			private StatusLabel _synchroLabel = null;
			public StatusLabel SynchroLabel { get { return _synchroLabel; } }

			/// <summary>
			/// スロット
			/// </summary>
			[SerializeField]
			private StatusLabel _slotLabel = null;
			public StatusLabel SlotLabel { get { return _slotLabel; } }

			/// <summary>
			/// 変化量
			/// </summary>
			[SerializeField]
			private StatusLabel _upLabel = null;
			public StatusLabel UpLabel { get { return _upLabel; } }

			/// <summary>
			/// 増減アイコン
			/// </summary>
			[SerializeField]
			private StatusIcon _icon = null;
			public StatusIcon Icon { get { return _icon; } }

			/// <summary>
			/// 合計値設定
			/// </summary>
			public void SetTotal(int total, string format, StatusColor.Type type)
			{
				if (this.TotalLabel == null) { return; }
				this.TotalLabel.SetText(string.Format(format, total));
				this.TotalLabel.SetColor(type);
			}
			/// <summary>
			/// ベース値設定
			/// </summary>
			public void SetBase(int baseNum, string format, StatusColor.Type type)
			{
				if (this.BaseLabel == null) { return; }
				this.BaseLabel.SetText(string.Format(format, baseNum));
				this.BaseLabel.SetColor(type);
			}
			/// <summary>
			/// シンクロ値設定
			/// </summary>
			public void SetSynchro(int synchro, string format, StatusColor.Type type)
			{
				if (this.SynchroLabel == null) { return; }
				this.SynchroLabel.SetText(string.Format(format, synchro));
				this.SynchroLabel.SetColor(type);
			}
			/// <summary>
			/// スロット値設定
			/// </summary>
			public void SetSlot(int slot, string format, StatusColor.Type type)
			{
				if (this.SlotLabel == null) { return; }
				this.SlotLabel.SetText(string.Format(format, slot));
				this.SlotLabel.SetColor(type);
			}
			/// <summary>
			/// 変化量値設定
			/// </summary>
			public void SetUp(int up, string format, StatusColor.Type type)
			{
				if (this.UpLabel == null) { return; }
				this.UpLabel.SetText(string.Format(format, up));
				this.UpLabel.SetColor(type);
			}
		}

		/// <summary>
		/// 各ステータスに使用するラベルクラス
		/// </summary>
		[Serializable]
		public class StatusLabel
		{
			/// <summary>
			/// ベースラベル
			/// </summary>
			[SerializeField]
			private UILabel _baseLabel = null;
			public UILabel BaseLabel { get { return _baseLabel; } }

			/// <summary>
			/// エフェクト用ラベル
			/// </summary>
			[SerializeField]
			private UILabel _growLabel = null;
			public UILabel GrowLabel { get { return _growLabel; } }

			/// <summary>
			/// テキストをラベルに設定
			/// </summary>
			public void SetText(string text)
			{
				if (this.BaseLabel == null) { return; }
				this.BaseLabel.text = text;
			}
			/// <summary>
			/// 色設定
			/// </summary>
			public void SetColor(StatusColor.Type type)
			{
				StatusColor.Set(type, this.BaseLabel, this.GrowLabel);
			}
		}

		/// <summary>
		/// ステータス増減アイコン
		/// </summary>
		[Serializable]
		public class StatusIcon
		{
			/// <summary>
			/// 増加アイコン
			/// </summary>
			[SerializeField]
			private GameObject _upObj = null;
			private GameObject UpObj { get { return _upObj; } }

			/// <summary>
			/// 減少アイコン
			/// </summary>
			[SerializeField]
			private GameObject _downObj = null;
			private GameObject DownObj { get { return _downObj; } }

			/// <summary>
			/// 表示するための種類
			/// </summary>
			public enum Type
			{
				None,
				Up,
				Down,
			}

			/// <summary>
			/// アイコン表示の設定
			/// </summary>
			public void SetIcon(Type type)
			{
				if (this.UpObj != null)
				{
					this.UpObj.SetActive(type == Type.Up);
				}
				if (this.DownObj != null)
				{
					this.DownObj.SetActive(type == Type.Down);
				}
			}
		}

		#region ランク
		/// <summary>
		/// 変化前設定
		/// </summary>
		public void SetRankBefore(string rank)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.RankAttach == null) { return; }
			status.RankAttach.SetBefore(rank);
		}

		/// <summary>
		/// 変化後設定
		/// </summary>
		public void SetRankAfter(bool isActive, string rank)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.RankAttach == null) { return; }
			status.RankAttach.SetAfterActive(isActive, rank);
		}

		/// <summary>
		/// ランク色設定
		/// </summary>
		public void SetRankColor(StatusColor.Type beforeType, StatusColor.Type afterType)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.RankAttach == null) { return; }
			status.RankAttach.SetColor(beforeType, afterType);
		}
		#endregion

		#region レベル
		/// <summary>
		/// レベル変化前設定
		/// </summary>
		public void SetLevelBefore(string lv)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.LevelAttach == null) { return; }
			status.LevelAttach.SetBefore(lv);
		}

		/// <summary>
		/// レベル変化後設定
		/// </summary>
		public void SetLevelAfter(bool isActive, string lv)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.LevelAttach == null) { return; }
			status.LevelAttach.SetAfter(isActive, lv);
		}

		/// <summary>
		/// レベル色設定
		/// </summary>
		public void SetLevelColor(StatusColor.Type type)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.LevelAttach == null) { return; }
			status.LevelAttach.SetColor(type);
		}

		/// <summary>
		/// レベルメッセージ設定
		/// </summary>
		public void SetLevelMessage(string message)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.LevelAttach == null) { return; }
			status.LevelAttach.SetMessage(message);
		}
		#endregion

		#region 経験値
		/// <summary>
		/// 経験値表示設定
		/// </summary>
		public void SetExpActive(bool isActive)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.ExpParentObj == null) { return; }
			status.ExpParentObj.SetActive(isActive);
		}

		/// <summary>
		/// 経験値バー設定
		/// </summary>
		public void SetExpBar(float value)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.ExpBar == null) { return; }
			status.ExpBar.value = value;
		}
		#endregion

		#region シンクロ可能回数
		/// <summary>
		/// シンクロ可能回数変化前設定
		/// </summary>
		public void SetSynchroRemainBefore(string remain)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.SynchroRemainAttach == null) { return; }
			status.SynchroRemainAttach.SetBefore(remain);
		}

		/// <summary>
		/// シンクロ可能回数変化後設定
		/// </summary>
		public void SetSynchroRemainAfter(bool isActive, string remain)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.SynchroRemainAttach == null) { return; }
			status.SynchroRemainAttach.SetAfter(isActive, remain);
		}

		/// <summary>
		/// シンクロ可能回数色設定
		/// </summary>
		public void SetSynchroRemainColor(StatusColor.Type type)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.SynchroRemainAttach == null) { return; }
			status.SynchroRemainAttach.SetColor(type);
		}
		#endregion

		#region 生命力
		/// <summary>
		/// 生命力合計設定
		/// </summary>
		public void SetHitPointTotal(int total, string format, StatusColor.Type type)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.HitPointAttach == null) { return; }
			status.HitPointAttach.SetTotal(total, format, type);
		}

		/// <summary>
		/// 生命力ベース設定
		/// </summary>
		public void SetHitPointBase(int baseNum, string format, StatusColor.Type type)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.HitPointAttach == null) { return; }
			status.HitPointAttach.SetBase(baseNum, format, type);
		}

		/// <summary>
		/// 生命力シンクロ設定
		/// </summary>
		public void SetSynchroHitPoint(int synchro, string format, StatusColor.Type type)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.HitPointAttach == null) { return; }
			status.HitPointAttach.SetSynchro(synchro, format, type);
		}

		/// <summary>
		/// 生命力スロット設定
		/// </summary>
		public void SetSlotHitPoint(int slot, string format, StatusColor.Type type)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.HitPointAttach == null) { return; }
			status.HitPointAttach.SetSlot(slot, format, type);
		}

		/// <summary>
		/// 生命力変化分設定
		/// </summary>
		public void SetHitPointUp(int up, string format, StatusColor.Type type)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.HitPointAttach == null) { return; }
			status.HitPointAttach.SetUp(up, format, type);
		}

		/// <summary>
		/// 生命力の増減アイコン設定
		/// </summary>
		public void SetHitPointIcon(StatusIcon.Type type)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.HitPointAttach == null) { return; }
			status.HitPointAttach.Icon.SetIcon(type);
		}
		#endregion

		#region 攻撃力
		/// <summary>
		/// 攻撃力合計設定
		/// </summary>
		public void SetAttackTotal(int total, string format, StatusColor.Type type)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.AttackAttach == null) { return; }
			status.AttackAttach.SetTotal(total, format, type);
		}

		/// <summary>
		/// 攻撃力ベース設定
		/// </summary>
		public void SetAttackBase(int baseNum, string format, StatusColor.Type type)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.AttackAttach == null) { return; }
			status.AttackAttach.SetBase(baseNum, format, type);
		}

		/// <summary>
		/// 攻撃力シンクロ設定
		/// </summary>
		public void SetSynchroAttack(int synchro, string format, StatusColor.Type type)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.AttackAttach == null) { return; }
			status.AttackAttach.SetSynchro(synchro, format, type);
		}

		/// <summary>
		/// 攻撃力スロット設定
		/// </summary>
		public void SetSlotAttack(int slot, string format, StatusColor.Type type)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.AttackAttach == null) { return; }
			status.AttackAttach.SetSlot(slot, format, type);
		}

		/// <summary>
		/// 攻撃力変化分設定
		/// </summary>
		public void SetAttackUp(int up, string format, StatusColor.Type type)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.AttackAttach == null) { return; }
			status.AttackAttach.SetUp(up, format, type);
		}

		/// <summary>
		/// 攻撃力の増減アイコン設定
		/// </summary>
		public void SetAttackIcon(StatusIcon.Type type)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.AttackAttach == null) { return; }
			status.AttackAttach.Icon.SetIcon(type);
		}
		#endregion

		#region 防御力
		/// <summary>
		/// 防御力合計設定
		/// </summary>
		public void SetDefenseTotal(int total, string format, StatusColor.Type type)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.DefenseAttach == null) { return; }
			status.DefenseAttach.SetTotal(total, format, type);
		}

		/// <summary>
		/// 防御力ベース設定
		/// </summary>
		public void SetDefenseBase(int baseNum, string format, StatusColor.Type type)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.DefenseAttach == null) { return; }
			status.DefenseAttach.SetBase(baseNum, format, type);
		}

		/// <summary>
		/// 防御力シンクロ設定
		/// </summary>
		public void SetSynchroDefense(int synchro, string format, StatusColor.Type type)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.DefenseAttach == null) { return; }
			status.DefenseAttach.SetSynchro(synchro, format, type);
		}

		/// <summary>
		/// 防御力スロット設定
		/// </summary>
		public void SetSlotDefense(int slot, string format, StatusColor.Type type)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.DefenseAttach == null) { return; }
			status.DefenseAttach.SetSlot(slot, format, type);
		}

		/// <summary>
		/// 防御力変化分設定
		/// </summary>
		public void SetDefenseUp(int up, string format, StatusColor.Type type)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.DefenseAttach == null) { return; }
			status.DefenseAttach.SetUp(up, format, type);
		}

		/// <summary>
		/// 防御力の増減アイコン設定
		/// </summary>
		public void SetDefenseIcon(StatusIcon.Type type)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.DefenseAttach == null) { return; }
			status.DefenseAttach.Icon.SetIcon(type);
		}
		#endregion

		#region 特殊能力
		/// <summary>
		/// 特殊能力合計設定
		/// </summary>
		public void SetExtraTotal(int total, string format, StatusColor.Type type)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.ExtraAttach == null) { return; }
			status.ExtraAttach.SetTotal(total, format, type);
		}

		/// <summary>
		/// 特殊能力ベース設定
		/// </summary>
		public void SetExtraBase(int baseNum, string format, StatusColor.Type type)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.ExtraAttach == null) { return; }
			status.ExtraAttach.SetBase(baseNum, format, type);
		}

		/// <summary>
		/// 特殊能力シンクロ設定
		/// </summary>
		public void SetSynchroExtra(int synchro, string format, StatusColor.Type type)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.ExtraAttach == null) { return; }
			status.ExtraAttach.SetSynchro(synchro, format, type);
		}

		/// <summary>
		/// 特殊能力スロット設定
		/// </summary>
		public void SetSlotExtra(int slot, string format, StatusColor.Type type)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.ExtraAttach == null) { return; }
			status.ExtraAttach.SetSlot(slot, format, type);
		}

		/// <summary>
		/// 特殊能力変化分設定
		/// </summary>
		public void SetExtraUp(int up, string format, StatusColor.Type type)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.ExtraAttach == null) { return; }
			status.ExtraAttach.SetUp(up, format, type);
		}

		/// <summary>
		/// 特殊能力の増減アイコン設定
		/// </summary>
		public void SetExtraIcon(StatusIcon.Type type)
		{
			var status = this.CharaStatusAttach;
			if (status == null || status.ExtraAttach == null) { return; }
			status.ExtraAttach.Icon.SetIcon(type);
		}
		#endregion
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
			if (this.BoardRoot != null && boardTrans != null)
			{
				boardTrans.parent = this.BoardRoot;
			}
		}
		void RemoveBoard()
		{
			if (this.BoardRoot != null)
			{
				this.BoardRoot.DestroyChildren();
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
