/// <summary>
/// 勝敗メッセージアイテム
/// 
/// 2014/08/08
/// </summary>
using UnityEngine;
using System.Collections;
using System;

public class GUIJudgeMessageItem : GUIEffectMessageItem
{
	#region オブジェクト情報

	[System.Serializable]
	public class JudgeObjectInfo
	{
		[SerializeField]
		private GameObject judgeObj;
		public GameObject JudgeObj { get { return judgeObj; } }

		[SerializeField]
		private float showTime;
		public float ShowTime { get { return showTime; } }
	}

	#endregion

	#region フィールド&プロパティ

	/// <summary>
	/// 次のメッセージが再生されるまでの時間
	/// </summary>
	[SerializeField]
	private float nextPlayMessageTime;

	/// <summary>
	/// ゲームセット
	/// </summary>
	[SerializeField]
	private JudgeObjectInfo gameSetObjInfo;
	public JudgeObjectInfo GameSetObjInfo { get { return gameSetObjInfo; } }

	/// <summary>
	/// 引き分け
	/// </summary>
	[SerializeField]
	private JudgeObjectInfo drawObjInfo;
	public JudgeObjectInfo DrawObjInfo { get { return drawObjInfo; } }

	/// <summary>
	/// 勝利
	/// </summary>
	[SerializeField]
	private JudgeObjectInfo winObjInfo;
	public JudgeObjectInfo WinObjInfo { get { return winObjInfo; } }

	/// <summary>
	/// 敗北
	/// </summary>
	[SerializeField]
	private JudgeObjectInfo loseObjInfo;
	public JudgeObjectInfo LoseObjInfo { get { return loseObjInfo; } }

	/// <summary>
	/// 勝敗結果
	/// </summary>
	private JudgeTypeClient judgeType;

	/// <summary>
	/// 計測用
	/// </summary>
	private float waitTime;

	/// <summary>
	/// メッセージ更新用
	/// </summary>
	private Action messageUpdate = ()=>{};

	#endregion

	#region セットアップ

	/// <summary>
	/// セットアップ処理
	/// </summary>
	public virtual void Setup(JudgeTypeClient judgeType)
	{
		if(this.GameSetObjInfo.JudgeObj != null)
		{
			this.GameSetObjInfo.JudgeObj.SetActive(true);
		}
		if(this.DrawObjInfo.JudgeObj != null)
		{
			this.DrawObjInfo.JudgeObj.SetActive(false);
		}
		if(this.WinObjInfo.JudgeObj != null)
		{
			this.WinObjInfo.JudgeObj.SetActive(false);
		}
		if(this.LoseObjInfo.JudgeObj != null)
		{
			this.LoseObjInfo.JudgeObj.SetActive(false);
		}

		// ShowTimeが0に設定されていた時はGmaseSet,Win・Lose・Draw,GamseSet終了してから次のメッセージが表示されるまでの時間
		// の合計時間をセットする
		if(this.ShowTime == 0)
		{
			float judgeShowTime = 0;
			switch(judgeType)
			{
				case JudgeTypeClient.PlayerWin:
				case JudgeTypeClient.PlayerCompleteWin:
					judgeShowTime = this.WinObjInfo.ShowTime;
					break;
					
				case JudgeTypeClient.PlayerLose:
				case JudgeTypeClient.PlayerCompleteLose:
					judgeShowTime = this.LoseObjInfo.ShowTime;
					break;
					
				case JudgeTypeClient.Draw:
					judgeShowTime =	this.DrawObjInfo.ShowTime;
					break;
			}
			this.time = judgeShowTime + this.GameSetObjInfo.ShowTime + this.nextPlayMessageTime;
		}

		this.judgeType = judgeType;
		this.waitTime = 0;
		messageUpdate = GameSetUpdate;
	}

	#endregion

	#region 更新

	/// <summary>
	/// 更新処理
	/// </summary>
	protected override void Update ()
	{
		base.Update();

		messageUpdate();
	}

	/// <summary>
	/// GamseSetメッセージ表示時の更新処理
	/// </summary>
	private void GameSetUpdate()
	{
		this.waitTime += Time.deltaTime;
		if(this.waitTime >  this.GameSetObjInfo.ShowTime)
		{
			// GamseSetメッセージの表示時間を超えたらGamseSetのメッセージを非表示
			this.waitTime = 0;
			if(this.GameSetObjInfo.JudgeObj != null)
			{
				this.GameSetObjInfo.JudgeObj.SetActive(false);
			}

			this.messageUpdate = NextPlayMessage;
		}
	}

	/// <summary>
	/// 次のメッセージが表示されるまでの待機時間処理
	/// </summary>
	private void NextPlayMessage()
	{
		this.waitTime += Time.deltaTime;
		if(this.waitTime > this.nextPlayMessageTime)
		{
			// 待機時間を超えたら次のメッセージを表示
			switch(judgeType)
			{
				case JudgeTypeClient.PlayerWin:
				case JudgeTypeClient.PlayerCompleteWin:
					if(this.WinObjInfo.JudgeObj != null)
					{
						this.WinObjInfo.JudgeObj.SetActive(true);
					}
					break;
					
				case JudgeTypeClient.PlayerLose:
				case JudgeTypeClient.PlayerCompleteLose:
					if(this.LoseObjInfo.JudgeObj != null)
					{
						this.LoseObjInfo.JudgeObj.SetActive(true);
					}
					break;
					
				case JudgeTypeClient.Draw:
					if(this.DrawObjInfo.JudgeObj != null)
					{
						this.DrawObjInfo.JudgeObj.SetActive(true);
					}
					break;
			}

			this.messageUpdate = () =>{};
		}
	}
	
	#endregion
}
