/// <summary>
/// リザルトUIの全体を管理するクラス
/// 
/// 2014/11/10
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Packet;

public class GUIResultOld : Singleton<GUIResultOld>
{
	#region 制御用ステート
	// リザルトシーンの制御用ステート
	public enum StateType
	{
		Player,
		Team,
		Rewards
	}
	#endregion

	#region フィールド＆プロパティ
	/// <summary>
	/// 初期化時のアクティブ状態
	/// </summary>
	[SerializeField]
	bool _isStartActive = false;
	bool IsStartActive { get { return _isStartActive; } }
	
	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public GUIPositionScroll scroll;
		public GUIResultPlayerInfoOld playerInfo;
		public GUIResultTeamInfoOld teamInfo;
		public GUIResultRewardsInfoOld rewardsInfo;
	}
	
	// アクティブ設定
	bool IsActive { get; set; }

	// キャラアイコン
	private CharaIcon charaIcon = null;
	/// <summary>
	/// TODO: 後々ScmParamにリザルトシーンで共通で使用するクラス等作成しそこからアイコンを取得する予定
	/// </summary>
	public static CharaIcon CharaIcon
	{
		get
		{
			if(Instance == null) return null;
			if (Instance.charaIcon == null)
			{
				Instance.charaIcon = new CharaIcon();
			}
			return Instance.charaIcon;
		}
	}

	/// <summary>
	/// キャラボード
	/// </summary>
	private CharaBoard charaBoard = null;
	/// <summary>
	/// TODO: 後々ScmParamにリザルトシーンで共通で使用するクラス等作成しそこからキャラボードを取得する予定
	/// </summary>
	public static CharaBoard CharaBoard
	{
		get
		{
			if(Instance == null) return null;
			if (Instance.charaBoard == null)
			{
				Instance.charaBoard = new CharaBoard();
			}
			return Instance.charaBoard;
		}
	}

	/// <summary>
	/// リザルトの状態
	/// </summary>
	public StateType State { get { return state; } }
	private StateType state = StateType.Player;

	/// <summary>
	/// プレイヤー情報
	/// </summary>
	private MemberInfo playerInfo = null;

	/// <summary>
	/// 勝敗
	/// </summary>
	private JudgeTypeClient judgeType = JudgeTypeClient.Unknown;
	#endregion
	
	#region 初期化
	override protected void Awake()
	{
		base.Awake();
		
		// 表示設定
		this._SetActive(this.IsStartActive);
	}
	#endregion
	
	#region アクティブ設定
	public static void SetActive(bool isActive)
	{
		if (Instance != null) Instance._SetActive(isActive);
	}
	void _SetActive(bool isActive)
	{
		this.IsActive = isActive;
	}
	#endregion

	#region セットアップ
	/// <summary>
	/// リザルトのセットアップを行う
	/// </summary>
	/// <param name="bridgingResultInfo">
	/// リザルト情報
	/// </param>
	public static void Setup(BridgingResultInfo bridgingResultInfo)
	{
		if(Instance == null) return;
		Instance._Setup(bridgingResultInfo);
	}
	private void _Setup(BridgingResultInfo bridgingResultInfo)
	{
		List<MemberInfo> infoList = bridgingResultInfo.MemberList;
		if(infoList != null)
		{
			// プレイヤー情報のセットアップ
			SetupPlayerInfo(infoList, bridgingResultInfo.InFieldPlayerID, bridgingResultInfo.JudgeTypeClient);

			// チームのセットアップ
			if(this.Attach.teamInfo != null)
			{
				this.Attach.teamInfo.Setup(infoList, bridgingResultInfo.InFieldPlayerID, bridgingResultInfo.JudgeTypeClient);
			}
		}

		//TODO: 3/27公開版コード
		/* 報酬情報画面表示しない
		List<BattleBonusPrizePacketParameter> prizeList = bridgingResultInfo.PrizeList;
		if(prizeList != null)
		{
			// 報酬情報のセットアップ
			if(this.Attach.rewardsInfo != null)
			{
				this.Attach.rewardsInfo.Setup(prizeList);
			}
		}
		*/

		StateChange_Team();
	}

	/// <summary>
	/// メンバーリストからプレイヤー情報を検索しセットアップを行う
	/// </summary>
	private void SetupPlayerInfo(List<MemberInfo> infoList, int playerID, JudgeTypeClient judgeType)
	{
		this.playerInfo = null;
		this.judgeType = JudgeTypeClient.Unknown;

		if(this.Attach.playerInfo == null) return;
		foreach(MemberInfo info in infoList)
		{
			if(info.inFieldID == playerID)
			{
				this.Attach.playerInfo.Setup(info, judgeType);

				//// ボイス.
				//PlayResultVoice(info, judgeType);

				this.playerInfo = info;
				this.judgeType = judgeType;
				return;
			}
		}
	}
	#endregion

	#region NGUIリフレクション
	/// <summary>
	/// プレイヤーリザルトに変更する
	/// </summary>
	public void StateChange_Player()
	{
		this.Attach.scroll.Setup((int)StateType.Player);
		this.state = StateType.Player;
	}
	/// <summary>
	/// チームリザルトに変更する
	/// </summary>
	public void StateChange_Team()
	{
		this.Attach.scroll.Setup((int)StateType.Team);
		// チーム情報のエフェクト再生
		if(this.Attach.teamInfo == null) return;
		this.state = StateType.Team;
		StartCoroutine(this.PlayTeamEffectCoroutine());
	}
	private IEnumerator PlayTeamEffectCoroutine()
	{
		if (this.state == StateType.Team)
		{
			while (GUIFade.Progress < 1 || !GUIFade.IsFinish)
			{
				yield return null;
			}

			this.Attach.teamInfo.playEffect();

			// ボイス.
			if (this.playerInfo != null && this.judgeType != JudgeTypeClient.Unknown)
			{
				PlayResultVoice(this.playerInfo, this.judgeType);
			}
		}
	}
	/// <summary>
	/// 報酬リザルトに変更する
	/// </summary>
	public void StateChange_Rewards()
	{
		this.Attach.scroll.Setup((int)StateType.Rewards);
		this.state = StateType.Rewards;
	}
	/// <summary>
	/// 次のシーンに変更する
	/// </summary>
	public void GotoNextScene()
	{
		ResultMain.GotoNextScene();
	}
	/// <summary>
	/// スキップボタンが押された時の処理
	/// </summary>
	public void OnSkip()
	{
		if(this.Attach.playerInfo != null)
		{
			this.Attach.playerInfo.SkipEffect();
		}
		if(this.Attach.teamInfo != null)
		{
			this.Attach.teamInfo.SkipEffect();
		}
	}
	#endregion

	#region デバッグ
#if UNITY_EDITOR && XW_DEBUG
	[System.Serializable]
	public class DebugMemberInfo
	{
		public AvatarType avatarType;
		public string name;
		public int battleScore;
		public int killCount;
		public int killScore;
		public int breakCount;
		public int breakScore;
		public int winBonus;
		public int totalScore;
		public Scm.Common.GameParameter.BattleRank rank;
		public JudgeTypeClient judgeType;
		public int startGradeMasterDataID;
		public int endGradeMasterDataID;
		public int startGradePoint;
		public int endGradePoint;
		public Scm.Common.GameParameter.PlayerGradeState gradeState;
	}
	[SerializeField]
	DebugParameter _debugParam = new DebugParameter();
	DebugParameter DebugParam { get { return _debugParam; } }
	[System.Serializable]
	public class DebugParameter
	{
		public bool execute;
		public bool isActive;
		//public bool playerInfoSetupExecute;
		//public DebugMemberInfo playerInfo;
	}
	private bool isReadMasterData;
	void DebugUpdate()
	{
		if(!this.isReadMasterData)
		{
			MasterData.Read();
			this.isReadMasterData = true;
		}

		var t = this.DebugParam;
		if (t.execute)
		{
			t.execute = false;
			{
				this._SetActive(t.isActive);
			}
		}
		/* 意図した動作しないので一旦封印 動作させるとたまにUITop-Resultのプレハブのデータが書き変わってしまうバグがある
		if(t.playerInfoSetupExecute)
		{
			t.playerInfoSetupExecute = false;
			{
				if(this.Attach.playerInfo != null)
				{
					//this.Attach.playerInfo.Setup(CreateMemberInfo(t.playerInfo), t.playerInfo.judgeType);
				}
			}
		}
		*/

	}
	/// <summary>
	/// OnValidate はInspector上で値の変更があった時に呼び出される
	/// GameObject がアクティブ状態じゃなくても呼び出されるのでアクティブ化するときには有効
	/// </summary>
	void OnValidate()
	{
		if(!Application.isPlaying) return;
		this.DebugUpdate();
	}

	/// <summary>
	/// メンバー情報生成
	/// </summary>
	MemberInfo CreateMemberInfo(DebugMemberInfo debugInfo)
	{
		MemberInfo info = new MemberInfo();
		info.avatarType = debugInfo.avatarType;
		info.name = debugInfo.name;
		info.baseScore = debugInfo.battleScore;
		info.kill = debugInfo.killCount;
		info.killScore = debugInfo.killScore;
		info.subTowerDefeatCount = debugInfo.breakCount;
		info.subTowerDefeatScore = debugInfo.breakScore;
		info.winBonus = debugInfo.winBonus;
		info.score = debugInfo.totalScore;
		info.battleRank = debugInfo.rank;
		info.startGradePoint = debugInfo.startGradePoint;
		info.endGradePoint = debugInfo.endGradePoint;
		info.playerGradeState = debugInfo.gradeState;
		if(debugInfo.startGradeMasterDataID <= 0)
		{
			debugInfo.startGradeMasterDataID = 1;
		}
		if(debugInfo.endGradeMasterDataID <= 0)
		{
			debugInfo.endGradeMasterDataID = 1;
		}
		info.startPlayerGradeID = debugInfo.startGradeMasterDataID;
		info.endPlayerGradeID = debugInfo.endGradeMasterDataID;


		return info;
	}
#endif
	#endregion

	#region ボイス再生
	private void PlayResultVoice(MemberInfo info, JudgeTypeClient judge)
	{
		var voice = CharacterVoice.Create(this.gameObject, info.avatarType);
		switch(judge)
		{
			case JudgeTypeClient.PlayerCompleteWin:
				voice.Play(CharacterVoice.CueName_win_complete);
				break;
			case JudgeTypeClient.PlayerWin:
				voice.Play(CharacterVoice.CueName_win);
				break;
			case JudgeTypeClient.Draw:
				voice.Play(CharacterVoice.CueName_draw);
				break;
			case JudgeTypeClient.PlayerLose:
				voice.Play(CharacterVoice.CueName_lose);
				break;
			case JudgeTypeClient.PlayerCompleteLose:
				voice.Play(CharacterVoice.CueName_lose_complete);
				break;
		}
	}
	#endregion
}
