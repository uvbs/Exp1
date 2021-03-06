/// <summary>
/// プレイヤーのリザルト情報UI
/// 
/// 2014/11/10
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.Master;
using Scm.Common.GameParameter;

public class GUIResultPlayerInfoOld : MonoBehaviour
{
	#region エフェクト状態タイプ
	public enum EffectStateType
	{
		Score = 0,
		Gradebar,
		GradeScore,
		GradeState,

		None = -1
	}
	#endregion

	#region フィールド＆プロパティ
	#region スコア関係のアタッチオブジェクト
	[System.Serializable]
	public class ScoreAttachObject
	{
		/// <summary>
		/// 戦闘スコア
		/// </summary>
		public UILabel baseScoreLabel;
		/// <summary>
		/// 撃破数
		/// </summary>
		public UILabel killCountLabel;
		/// <summary>
		/// 撃破スコア
		/// </summary>
		public UILabel killScoreLabel;
		/// <summary>
		/// 破壊数
		/// </summary>
		public UILabel breakCountLabel;
		/// <summary>
		/// 破壊スコア
		/// </summary>
		public UILabel breakScoreLabel;
		/// <summary>
		/// 勝敗
		/// </summary>
		public UILabel judgeLabel;
		/// <summary>
		/// 勝敗スコア
		/// </summary>
		public UILabel judgeScoreLabel;
		/// <summary>
		/// トータルスコア
		/// </summary>
		public UILabel totalScoreLabel;
	}
	#endregion
	
	#region グレードポイント関係のアタッチオブジェクト
	[System.Serializable]
	public class GradeAttachObject
	{
		/// <summary>
		/// 昇格イベントの親オブジェクト
		/// </summary>
		public GameObject eventParent;
		/// <summary>
		/// 昇格イベントメッセージ
		/// </summary>
		public UILabel eventMsgLabel;
		/// <summary>
		/// 追加グレードポイント
		/// </summary>
		public UILabel addPointLabel;
		/// <summary>
		/// 現在のグレード
		/// </summary>
		public UILabel gradeLabel;
		/// <summary>
		/// グレードゲージのスライダー
		/// </summary>
		public UISlider gradeGaugeSlider;
		/// <summary>
		/// グレードゲージの最少値
		/// </summary>
		public UILabel gradeGaugeMin;
		/// <summary>
		/// グレードゲージの最大値
		/// </summary>
		public UILabel gradeGaugeMax;

		/// <summary>
		/// 現在のグレードポイント
		/// </summary>
		public UILabel gradePoint;
	}
	#endregion

	#region ランクアタッチオブジェクト
	[System.Serializable]
	public class RankAttachObject
	{
		public GameObject rankSObject;
		public GameObject rankAObject;
		public GameObject rankBObject;
		public GameObject rankCObject;
		public GameObject rankDObject;
		public GameObject rankEObject;
	}
	#endregion

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		/// <summary>
		/// 次の情報へ移るボタンのオブジェクト
		/// </summary>
		public GameObject nextButtonObject;
		/// <summary>
		/// スコア関係のアタッチオブジェクト
		/// </summary>
		public ScoreAttachObject score;
		/// <summary>
		/// グレード関係のアタッチオブジェクト
		/// </summary>
		public GradeAttachObject grade;
		/// <summary>
		/// ランク関係のアタッチオブジェクト
		/// </summary>
		public RankAttachObject rank;
		/// <summary>
		/// キャラボードを生成する親
		/// </summary>
		public Transform charaBoardParent;
		/// <summary>
		/// キャラボードを制御するTween
		/// </summary>
		public TweenPosition charaBoardTween;
		/// <summary>
		/// 開始時のPlayTween
		/// </summary>
		public UIPlayTween StartPlayTween;
		/// <summary>
		/// グレードゲージ終了後のPlayTween
		/// </summary>
		public UIPlayTween gaugeAfterEffectPlayTween;
		/// <summary>
		/// グレード状態(イベントやアップやダウンの表示)
		/// </summary>
		public GUIResultGradeStateOld gradeState;
		/// <summary>
		/// 昇格イベント継続中オブジェクト
		/// </summary>
		public GameObject ongoingEventObject;
	}

	/// <summary>
	/// グレードゲージエフェクトの期間
	/// </summary>
	[SerializeField]
	private float gxpGaugeEffectDuration = 0.5f;

	/// <summary>
	/// プレイヤーのリザルト情報
	/// </summary>
	private MemberInfo playerInfo;

	/// <summary>
	/// 戦闘開始時のグレードマスターデータ
	/// </summary>
	private PlayerGradeMasterData startGradeMasterData;

	/// <summary>
	/// 戦闘終了後の結果を反映したグレードマスターデータ
	/// </summary>
	private PlayerGradeMasterData endGradeMasterData;
	
	/// <summary>
	/// 開始時のグレードポイント
	/// </summary>
	private int startGradePoint;
	
	/// <summary>
	/// 開始+追加のグレードポイント
	/// </summary>
	private int endGradePoint;
	
	/// <summary>
	/// 追加グレードポイント
	/// </summary>
	private int addGradePoint;

	/// <summary>
	/// エフェクトのスキップが可能かどうか
	/// </summary>
	private bool isSkip;

	/// <summary>
	/// エフェクトの状態
	/// </summary>
	private EffectStateType effectState = EffectStateType.None;

	/// <summary>
	/// エフェクト更新用
	/// </summary>
	private Fiber effectFiber;

	#endregion

	#region 初期化
	void Awake()
	{
		if(this.Attach.rank != null)
		{
			// ランク関係のアクティブをOFFに設定
			RankAttachObject rank = this.Attach.rank;
			if(rank.rankSObject != null) rank.rankSObject.SetActive(false);
			if(rank.rankAObject != null) rank.rankAObject.SetActive(false);
			if(rank.rankBObject != null) rank.rankBObject.SetActive(false);
			if(rank.rankCObject != null) rank.rankCObject.SetActive(false);
			if(rank.rankDObject != null) rank.rankDObject.SetActive(false);
			if(rank.rankEObject != null) rank.rankEObject.SetActive(false);
		}
		// エフェクトのスキップフラグをOFFに設定
		this.isSkip = false;
	}
	#endregion
	
	#region セットアップ
	/// <summary>
	/// プレイヤーのリザルト情報をセットアップする
	/// </summary>
	/// <param name="info">
	/// プレイヤー情報
	/// </param>
	public void Setup(MemberInfo playerInfo, JudgeTypeClient judgeType)
	{
		// プレイヤー情報セット
		this.playerInfo = playerInfo;

		// NextボタンのアクティブをOFFにする
		if(this.Attach.nextButtonObject != null)
		{
			this.Attach.nextButtonObject.SetActive(false);
		}

		// 各スコアのセットアップ
		SetupScore(playerInfo, judgeType);

		// 各グレード情報のセットアップ
		SetupGrade(playerInfo);

		// ランク情報のセットアップ
		SetupRank(playerInfo);

		//if(this.Attach.charaBoardParent != null && this.Attach.charaBoardTween != null)
		//{
		//	// キャラボードを生成してセットアップ
		//	CharaBoard charaBoard = GUIResultOld.CharaBoard;
		//	if(charaBoard != null)
		//	{
		//		charaBoard.GetBoard(playerInfo.avatarType, false,
		//								 (GameObject resource) => { SetupCharaBoard(resource, this.Attach.charaBoardParent, this.Attach.charaBoardTween); });
		//	}
		//}

		// グレード状態が昇格イベント継続中ならイベント継続中メッセージを表示させる
		if(this.Attach.ongoingEventObject != null)
		{
			if(playerInfo.playerGradeState == PlayerGradeState.Ongoing)
			{
				this.Attach.ongoingEventObject.SetActive(true);
			}
			else
			{
				this.Attach.ongoingEventObject.SetActive(false);
			}
		}

		// エフェクト再生開始
		PlayEffect();
	}

	/// <summary>
	/// 各スコアのセットアップ
	/// </summary>
	private void SetupScore(MemberInfo playerInfo, JudgeTypeClient judgeType)
	{
		if(this.Attach.score == null) return;
		ScoreAttachObject score = this.Attach.score;

		// 戦闘関係
		if(score.baseScoreLabel != null)
		{
			score.baseScoreLabel.text = GetScoreFormat(ObsolateSrc.PointUnitName, playerInfo.baseScore);
		}
		// 撃破関係
		if(score.killCountLabel != null)
		{
			score.killCountLabel.text = GetScoreFormat(ObsolateSrc.KillUnitName, playerInfo.kill);
		}
		if(score.killScoreLabel != null)
		{
			score.killScoreLabel.text = GetScoreFormat(ObsolateSrc.PointUnitName, playerInfo.killScore);
		}
		// 破壊関係
		if(score.breakCountLabel != null)
		{
			score.breakCountLabel.text = GetScoreFormat(ObsolateSrc.BreakUnitName, playerInfo.subTowerDefeatCount);
		}
		if(score.breakScoreLabel != null)
		{
			score.breakScoreLabel.text = GetScoreFormat(ObsolateSrc.PointUnitName, playerInfo.subTowerDefeatScore);
		}
		// 勝敗関係
		if(score.judgeLabel != null)
		{
			switch(judgeType)
			{
				case JudgeTypeClient.PlayerCompleteWin:
					score.judgeLabel.text = ObsolateSrc.CompleteWinName;
					break;
				case JudgeTypeClient.PlayerWin:
					score.judgeLabel.text = ObsolateSrc.WinName;
					break;
				case JudgeTypeClient.PlayerCompleteLose:
					score.judgeLabel.text = ObsolateSrc.CompleteLoseName;
					break;
				case JudgeTypeClient.PlayerLose:
					score.judgeLabel.text = ObsolateSrc.LoseName;
					break;
				case JudgeTypeClient.Draw:
					score.judgeLabel.text = ObsolateSrc.DrawName;
					break;
				default:
					score.judgeLabel.text = "";
					break;
			}
		}
		if(score.judgeScoreLabel != null)
		{
			score.judgeScoreLabel.text = GetScoreFormat(ObsolateSrc.PointUnitName, playerInfo.winBonus);
		}
		// トータル関係
		if(score.totalScoreLabel != null)
		{
			score.totalScoreLabel.text = GetScoreFormat(ObsolateSrc.PointUnitName, playerInfo.score);
		}
	}

	/// <summary>
	/// 各グレード情報のセットアップ
	/// </summary>
	private void SetupGrade(MemberInfo playerInfo)
	{
		// グレードマスターデータの取得
		MasterData.TryGetPlayerGrade(playerInfo.startPlayerGradeID, out this.startGradeMasterData);
		MasterData.TryGetPlayerGrade(playerInfo.endPlayerGradeID, out this.endGradeMasterData);

		// グレードポイントのセット
		switch(playerInfo.playerGradeState)
		{
			case PlayerGradeState.Up:
			case PlayerGradeState.GradeUp:
			{
				// グレード。称号アップ時は終了時のグレードポイントを最大値にセットする
				this.endGradePoint = this.startGradeMasterData != null ? this.startGradeMasterData.UpgradePoint : 0;
				this.startGradePoint = playerInfo.startGradePoint;
				break;
			}
			case PlayerGradeState.Down:
			case PlayerGradeState.GradeDown:
			{
				// グレード、称号ダウン時は終了時のグレードポイントを0にセットする
				this.endGradePoint = 0;
				this.startGradePoint = playerInfo.startGradePoint;
				break;
			}
			case PlayerGradeState.Fail:
			case PlayerGradeState.Ongoing:
			{
				// イベント失敗または継続中は開始、終了時ともグレードポイントが最大値のまま
				int upgradePoint = this.startGradeMasterData != null ? this.startGradeMasterData.UpgradePoint : 0;
				this.startGradePoint = upgradePoint;
				this.endGradePoint = upgradePoint;
				break;
			}
			case PlayerGradeState.Normal:
			case PlayerGradeState.Occur:
			{
				// 通常と昇格イベント発生
				this.startGradePoint = playerInfo.startGradePoint;
				this.endGradePoint = playerInfo.endGradePoint;
				break;
			}
		}
		this.addGradePoint = this.endGradePoint - this.startGradePoint;

		if(this.Attach.grade == null) return;
		GradeAttachObject grade = this.Attach.grade;

		// グレード
		if(grade.gradeLabel != null && this.startGradeMasterData != null)
		{
			// Gradeと値の間は一つ空白をいれておく
			grade.gradeLabel.text = string.Format("{0} {1}", ObsolateSrc.GradeName, this.startGradeMasterData.Grade);
		}
		// グレード追加ポイント
		if(grade.addPointLabel != null)
		{
			grade.addPointLabel.text = string.Format("{0:+#;-#;##0}"+" "+ObsolateSrc.GradeUnitName, this.addGradePoint);
		}
		// グレードゲージ
		if(grade.gradeGaugeSlider != null)
		{
			float value = 0;
			if(this.startGradeMasterData != null && this.startGradeMasterData.UpgradePoint > 0)
			{
				value = (float)this.startGradePoint / (float)this.startGradeMasterData.UpgradePoint;
			}
			grade.gradeGaugeSlider.value = value;
		}
		// グレードゲージの最少と最大値
		if(grade.gradeGaugeMin != null) grade.gradeGaugeMin.text = "0";
		if(grade.gradeGaugeMax != null && this.startGradeMasterData != null)
		{
			grade.gradeGaugeMax.text = this.startGradeMasterData.UpgradePoint.ToString();
		}
		// 現在のグレードポイント
		if(grade.gradePoint != null)
		{
			grade.gradePoint.text = this.startGradePoint.ToString();
		}
	}

	/// <summary>
	/// ポイント反映後のグレード関係のセットアップ
	/// </summary>
	private void SetupEndGrade()
	{
		if(this.Attach.grade == null) return;
		GradeAttachObject grade = this.Attach.grade;

		// グレード
		if(grade.gradeLabel != null && this.endGradeMasterData != null)
		{
			// Gradeと値の間は一つ空白をいれておく
			grade.gradeLabel.text = string.Format("{0} {1}", ObsolateSrc.GradeName, this.endGradeMasterData.Grade);
		}
		// グレードゲージ
		if(grade.gradeGaugeSlider != null)
		{
			float value = 0;
			if(this.startGradeMasterData != null && this.startGradeMasterData.UpgradePoint > 0)
			{
				value = (float)this.endGradePoint / (float)this.startGradeMasterData.UpgradePoint;
			}
			grade.gradeGaugeSlider.value = value;
		}
		// 追加グレードポイント
		if(grade.addPointLabel != null)
		{
			this.Attach.grade.addPointLabel.text = string.Format("{0:+#;-#;##0}"+" "+ObsolateSrc.GradeUnitName, 0);
		}
		// 現在のグレードポイント
		if(grade.gradePoint != null)
		{
			grade.gradePoint.text = this.endGradePoint.ToString();
		}
	}
	
	/// <summary>
	/// ランク情報のセットアップ
	/// </summary>
	private void SetupRank(MemberInfo playerInfo)
	{
		if(this.Attach.rank == null) return;
		RankAttachObject rank = this.Attach.rank;

		// ランクによって表示するランクのスプライトを決める
		switch(playerInfo.battleRank)
		{
			case BattleRank.S:
				if(rank.rankSObject != null) rank.rankSObject.SetActive(true);
				break;
			case BattleRank.A:
				if(rank.rankAObject != null) rank.rankAObject.SetActive(true);
				break;
			case BattleRank.B:
				if(rank.rankBObject != null) rank.rankBObject.SetActive(true);
				break;
			case BattleRank.C:
				if(rank.rankCObject != null) rank.rankCObject.SetActive(true);
				break;
		}
	}

	/// <summary>
	/// キャラボードのセットアップ
	/// </summary>
	private void SetupCharaBoard(GameObject resource, Transform parent, TweenPosition tweenPosition)
	{
		// リソース読み込み完了
		if (resource == null) return;
		// インスタンス化
		var go = SafeObject.Instantiate(resource) as GameObject;
		if (go == null) return;
		
		// 名前設定
		go.name = resource.name;
		// 親子付け
		var t = go.transform;
		t.parent = parent;
		t.localPosition = Vector3.zero;
		t.localRotation = Quaternion.identity;
		t.localScale = Vector3.one;
		
		// 読み込みが完了してから再生を開始する
		tweenPosition.Play(true);
	}

	/// <summary>
	/// スコア値を単位付きの文字列に整形して取得する
	/// </summary>
	private string GetScoreFormat(string unit, int score)
	{
		return string.Format("{0}"+unit, score);
	}
	#endregion

	#region 更新

	void Update()
	{
		if(this.effectFiber != null && !this.effectFiber.IsFinished)
		{
			// エフェクト更新
			this.effectFiber.Update();
		}
	}

	#endregion

	#region エフェクト処理

	#region エフェクト再生

	/// <summary>
	/// エフェクトの再生
	/// </summary>
	public void PlayEffect()
	{
		// エフェクト状態変更
		ChangeEffectState(EffectStateType.Score);

		// エフェクトのスキップを可能状態に
		this.isSkip = true;
	}

	/// <summary>
	/// スコアエフェクトの再生
	/// </summary>
	private IEnumerator PlayScoreEffect()
	{
		if(this.Attach.StartPlayTween == null) yield break;
		this.Attach.StartPlayTween.Play(true);
	}

	/// <summary>
	/// グレードバーエフェクト再生
	/// </summary>
	private IEnumerator PlayGradeBarEffect()
	{
		var effectCoroutine = GradeEffectCoroutine();
		while(effectCoroutine.MoveNext())
		{
			// グレードバーエフェクトの再生が終了するまで待機
			yield return null;
		}

		// グレードバーエフェクト終了処理
		GradeBarFinished();
	}

	/// <summary>
	/// グレードスコアエフェクト再生
	/// </summary>
	private IEnumerator PlayGradeScoreEffect()
	{
		if(this.Attach.gaugeAfterEffectPlayTween == null) yield break;
		this.Attach.gaugeAfterEffectPlayTween.Play(true);
	}

	/// <summary>
	/// グレード状態のエフェクト再生
	/// </summary>
	private IEnumerator PlayGradeState()
	{
		// グレード状態のエフェクトのメッセージのセットアップとエフェクト再生
		bool gradeEnable = false;
		if(this.Attach.gradeState != null)
		{
			gradeEnable = this.Attach.gradeState.Setup(this.playerInfo, this.endGradeMasterData);
		}
		
		// グレード状態に変化が無い場合はメッセージの表示&エフェクトの必要がないので終了処理を行う
		if(!gradeEnable && this.Attach.nextButtonObject != null)
		{
			GradeStateEffectFinished();
		}

		yield break;
	}

	#endregion

	#region エフェクト終了時(NGUIリフレクション)

	/// <summary>
	/// スコアエフェクト終了時
	/// </summary>
	public void ScoreEffectFinished()
	{
		ChangeEffectState(EffectStateType.Gradebar);
	}

	/// <summary>
	/// グレードバーエフェクト終了時(プログラム側で呼び出している)
	/// </summary>
	private void GradeBarFinished()
	{
		// グレードバー関係の最終値をセットしておく
		SetupEndGrade();
		ChangeEffectState(EffectStateType.GradeScore);
	}

	/// <summary>
	/// グレードスコアエフェクト終了時
	/// </summary>
	public void GradeScoreEffectFinished()
	{
		// エフェクトの状態変更
		ChangeEffectState(EffectStateType.GradeState);
		// スキップが可能なエフェクトの再生が終わったのでスキップ処理を行わないようにする
		this.isSkip = false;
	}

	/// <summary>
	/// グレード状態エフェクト終了時
	/// </summary>
	public void GradeStateEffectFinished()
	{
		// エフェクト状態変更
		ChangeEffectState(EffectStateType.None);

		// グレードの状態が昇格イベント発生なら昇格イベント中のメッセージを表示させる
		if(this.playerInfo.playerGradeState == PlayerGradeState.Occur)
		{
			if(this.Attach.ongoingEventObject != null)
			{
				this.Attach.ongoingEventObject.SetActive(true);
			}
		}

		// エフェクト処理終了
		EndEffect();
	}

	/// <summary>
	/// 全てのエフェクトが終了した時に呼ぶ
	/// </summary>
	private void EndEffect()
	{
		// ボタン表示
		this.Attach.nextButtonObject.SetActive(true);
	}

	#endregion

	#region エフェクト状態変更

	/// <summary>
	/// エフェクト状態の変更処理
	/// </summary>
	/// <param name="state">State.</param>
	private void ChangeEffectState(EffectStateType state)
	{
		// 前回と同じエフェクトは再生させない(エフェクトのリプレイは考慮していないため)
		if(this.effectState == state) return;

		switch(state)
		{
			case EffectStateType.Score:
			{
				this.effectFiber = new Fiber(PlayScoreEffect());
				break;
			}
			case EffectStateType.Gradebar:
			{
				this.effectFiber = new Fiber(PlayGradeBarEffect());
				break;
			}
			case EffectStateType.GradeScore:
			{
				this.effectFiber = new Fiber(PlayGradeScoreEffect());
				break;
			}
			case EffectStateType.GradeState:
			{
				this.effectFiber = new Fiber(PlayGradeState());
				break;
			}
		}
		this.effectState = state;
	}

	#endregion

	#region グレードバーエフェクト処理

	/// <summary>
	/// グレード関係のエフェクト再生コルーチン
	/// </summary>
	IEnumerator GradeEffectCoroutine()
	{
		GradeAttachObject grade = this.Attach.grade;
		// Nullチェック
		if(this.startGradeMasterData == null || grade == null
		   || grade.gradeGaugeSlider == null || grade.gradePoint == null || grade.addPointLabel == null)
		{
			// Nullだった場合はエフェクト終了処理を行ってコルーチンを抜ける
			yield break;
		}
		
		// ゲージエフェクト再生
		float time = 0;
		// 追加分のポイントが0なら変動なしなのでエフェクトを再生させない
		if(this.addGradePoint == 0) time = this.gxpGaugeEffectDuration;
		while(time < this.gxpGaugeEffectDuration)
		{
			time += Time.deltaTime;
			
			// 補完.
			float reate = 0;
			if(this.gxpGaugeEffectDuration > 0)
			{
				reate = time / this.gxpGaugeEffectDuration;
			}
			else
			{
				// エフェクトの期間が0以下ならエフェクトをスキップさせる
				reate = 1f;
				time = this.gxpGaugeEffectDuration;
			}
			float gradeCount = Mathf.Lerp(this.startGradePoint, this.endGradePoint, reate);
			int addGradeCount = Mathf.FloorToInt(Mathf.Lerp(this.addGradePoint, 0, reate));
			
			// ゲージセット.
			float value = 0;
			if(this.startGradeMasterData.UpgradePoint > 0)
			{
				value = gradeCount / (float)this.startGradeMasterData.UpgradePoint;
			}
			grade.gradeGaugeSlider.value = value;
			
			// 現在と追加ポイントセット
			grade.gradePoint.text = Mathf.FloorToInt(gradeCount).ToString();
			grade.addPointLabel.text = string.Format("{0:+#;-#;##0}"+" "+ObsolateSrc.GradeUnitName, addGradeCount);
			
			yield return null;
		}
	}

	#endregion

	#region エフェクトスキップ

	/// <summary>
	/// エフェクトをスキップさせる
	/// </summary>
	public void SkipEffect()
	{
		// スキップが可能な状態か
		if(!this.isSkip) return;
		
		// 各UIPlayTweenの再生を終了させる
		// エフェクトを再再生させる必要があるなら初期の値を再度セットする必要性あり
		UIPlayTween playTween = this.Attach.StartPlayTween;
		if(playTween != null)
		{
			playTween.SetTweener(SkipTweener);
		}
		playTween = this.Attach.gaugeAfterEffectPlayTween;
		if(playTween != null)
		{
			playTween.SetTweener(SkipTweener);
		}
		
		// グレードゲージエフェクトの期間を0にセット
		this.gxpGaugeEffectDuration = 0;

		this.isSkip = false;
	}

	/// <summary>
	/// UIPlayTweenで再生させているTween系の
	/// Duration値とdelay値を0にしTweenの再生を終了させる
	/// </summary>
	private void SkipTweener(UITweener tweener)
	{
		tweener.duration = 0f;
		tweener.delay = 0f;
	}

	#endregion

	#endregion
}
