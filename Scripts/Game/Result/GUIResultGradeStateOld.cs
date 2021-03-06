/// <summary>
/// リザルトのグレード状態(アップやダウン時の)表示を制御する
/// 
/// 2014/11/18
/// </summary>
using UnityEngine;
using System;
using System.Collections;
using Scm.Common.Master;
using Scm.Common.GameParameter;

public class GUIResultGradeStateOld : MonoBehaviour
{
	#region フィールド&プロパティ
	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public GameObject gradeUpMsgObject;
		public UILabel upMessageLabel;
		public GameObject gradeDownMsgObject;
		public UILabel downMessageLabel;
		public GameObject gradeEventMsgObject;
		public GameObject gradeMissMsgObject;
		public UIPlayTween startPlayTween;
		public UIPlayTween endPlayTween;
	}

	/// <summary>
	/// 表示時間
	/// </summary>
	[SerializeField]
	private float showTime = 5.0f;

	/// <summary>
	/// 時間更新処理用デリゲート
	/// </summary>
	private Action timeAction = ()=>{};

	/// <summary>
	/// 時間計測用
	/// </summary>
	private float timer;

	#endregion

	#region セットアップ
	/// <summary>
	/// セットアップ
	/// 戻り値:true = メッセージ表示あり false = メッセージ表示なし
	/// </summary>
	public bool Setup(MemberInfo info, PlayerGradeMasterData endGradeMasterData)
	{
		// グレード状態メッセージの表示が有効なのか無効なのか
		bool gradeEnable = true;

		// グレードの状態によって表示するメッセージを決める
		switch(info.playerGradeState)
		{
			// 昇格イベント発生.
			case PlayerGradeState.Occur:
			{
				SetupMessage(this.Attach.gradeEventMsgObject);
				break;
			}
			// 称号昇格またはグレード昇格.
			case PlayerGradeState.Up:
			case PlayerGradeState.GradeUp:
			{
				SetupMessage(this.Attach.gradeUpMsgObject);
				if(this.Attach.upMessageLabel != null && endGradeMasterData != null)
				{
					this.Attach.upMessageLabel.text = string.Format("グレードが{0}になりました", endGradeMasterData.Grade);
				}
				break;
			}
			// 称号降格またはグレード降格.
			case PlayerGradeState.Down:
			case PlayerGradeState.GradeDown:
			{
				SetupMessage(this.Attach.gradeDownMsgObject);
				if(this.Attach.downMessageLabel != null && endGradeMasterData != null)
				{
					this.Attach.downMessageLabel.text = string.Format("グレードが{0}になりました", endGradeMasterData.Grade);
				}
				break;
			}
			// 昇格失敗(イベント失敗).
			case PlayerGradeState.Fail:
			{
				SetupMessage(this.Attach.gradeMissMsgObject);
				break;
			}
			// それ以外の状態ではメッセージの表示が必要ない
			default:
			{
				gradeEnable = false;
				break;
			}
		}

		// 時間更新用デリゲートをリセットする
		this.timeAction = ()=>{};

		return gradeEnable;
	}

	/// <summary>
	/// メッセージのセットアップ処理
	/// </summary>
	private void SetupMessage(GameObject gradeMsgObject)
	{
		if(gradeMsgObject == null) return;
		if(this.Attach.startPlayTween == null) return;

		// メッセージを表示
		gradeMsgObject.SetActive(true);
		// エフェクトを再生させる
		this.Attach.startPlayTween.tweenTarget = gradeMsgObject;
		this.Attach.startPlayTween.Play(true);
	}
	#endregion

	#region 更新
	void Update()
	{
		this.timeAction();
	}

	/// <summary>
	/// 表示時間カウント更新
	/// </summary>
	private void UpdateShowTime()
	{
		this.timer -= Time.deltaTime;
		if(this.timer <= 0)
		{
			if(this.Attach.endPlayTween != null)
			{
				// 表示する時間が過ぎたら終了エフェクトを再生させる
				this.Attach.endPlayTween.tweenTarget = this.Attach.startPlayTween.tweenTarget;
				this.Attach.endPlayTween.Play(true);
			}
			this.timeAction = ()=>{};
		}
	}
	#endregion

	#region NGUIリフレクション
	/// <summary>
	/// 表示する時間のカウント開始
	/// </summary>
	public void ShowTimerStart()
	{
		this.timer = this.showTime;
		this.timeAction = UpdateShowTime;
	}
	#endregion
}
