/// <summary>
/// エフェクトメッセージ
/// 
/// 2014/07/29
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Master;

public class GUIEffectMessage : Singleton<GUIEffectMessage>
{
	#region メッセージタイプ

	public enum MsgType
	{
		None = -1,

		GameStart,
		LevelUP,
		TacticalInfo,
		TacticalWarning,
		TimeLater,
		TimeLimit,
		GameJudge,
		BreakInfo,
		BattleWait,
		Skill,
		Hit,

		Max,
	};

	#endregion

	#region アタッチオブジェクト

	[System.Serializable]
	public class AttachObject
	{
		/// <summary>
		/// 通常メッセージカテゴリー
		/// </summary>
		public GUIEffectMessageCategory normalCategory;
		/// <summary>
		/// バトル状態カテゴリー
		/// </summary>
		public GUIBattleStateMsgCategory battleStateCategory;
		/// <summary>
		/// 時間カテゴリー
		/// </summary>
		public GUITimeMsgCategory timeCategory;
		/// <summary>
		/// キルデスカテゴリー
		/// </summary>
		public GUIBreakMsgCategory breakCategory;
		/// <summary>
		/// スキル名カテゴリー
		/// </summary>
		public GUISkillMsgCategory skillCategory;
		/// <summary>
		/// ヒットカテゴリー
		/// </summary>
		public GUIHitMsgCategory hitCategory;
	}

	#endregion

	#region フィールド&プロパティ

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	private AttachObject attach;
	public AttachObject Attach { get {return attach; } }

	#endregion

	#region メッセージセット

	#region ゲームスタート

	/// <summary>
	/// ゲームスタートメッセージをセットする
	/// </summary>
	public static void SetGameStart()
	{
		if(Instance == null)
			return;
		Instance._SetGameStart();
	}
	public void _SetGameStart()
	{
		this.Attach.battleStateCategory.SetMessage(MsgType.GameStart);
	}

	#endregion

	#region レベルアップ

	/// <summary>
	/// レベルアップメッセージをセットする
	/// </summary>
	public static void SetLevelUp()
	{
		if(Instance == null)
			return;
		Instance._SetLevelUp();
	}
	public void _SetLevelUp()
	{
		this.Attach.normalCategory.SetMessage(MsgType.LevelUP);
	}

	#endregion

	#region 戦略

	/// <summary>
	/// 戦略メッセージセット
	/// </summary>
	public static void SetTacticalInfo(GUITacticalMessageItem.TacticalType tacticalType)
	{
		if(Instance == null)
			return;
		Instance.SetTactical(MsgType.TacticalInfo, tacticalType);
	}
	/// <summary>
	/// 戦略メッセージセット(警告)
	/// </summary>
	public static void SetTacticalWarning(GUITacticalMessageItem.TacticalType tacticalType)
	{
		if(Instance == null)
			return;
		Instance.SetTactical(MsgType.TacticalWarning, tacticalType);
	}
	public void SetTactical(MsgType msgType, GUITacticalMessageItem.TacticalType tacticalType)
	{
		this.Attach.battleStateCategory.SetTacticalMessage(msgType, tacticalType);
	}

	#endregion

	#region 時間

	/// <summary>
	/// 経過時間メッセージセット
	/// </summary>
	public static void SetTimeLater(BattleFieldTimeEventMasterData timeMasterData)
	{
		if(Instance == null)
			return;
		Instance.SetTimeMessage(MsgType.TimeLater, timeMasterData);
	}
	/// <summary>
	/// 残り時間メッセージセット
	/// </summary>
	public static void SetTimeLimit(BattleFieldTimeEventMasterData timeMasterData)
	{
		if(Instance == null)
			return;
		Instance.SetTimeMessage(MsgType.TimeLimit, timeMasterData);
	}
	public void SetTimeMessage(MsgType msgType, BattleFieldTimeEventMasterData timeMasterData)
	{
		this.Attach.timeCategory.SetTimeMessage(msgType, timeMasterData);
	}

	#endregion

	#region 勝敗

	/// <summary>
	/// 勝敗メッセージをセットする
	/// </summary>
	public static void SetJudge(JudgeTypeClient judgeType)
	{
		if(Instance == null)
			return;
		Instance._SetJudge(judgeType);
	}
	public void _SetJudge(JudgeTypeClient judgeType)
	{
		this.Attach.battleStateCategory.SetJudgeMessage(MsgType.GameJudge, judgeType);
	}

	#endregion

	#region キルデス

	/// <summary>
	/// キル情報メッセージをセットする
	/// </summary>
	public static void SetKillInfo(int killCount, AvatarType enemyType, int enemySkinId, string enemyName, CharaIcon charaIcon)
	{
		if(Instance == null)
			return;
		Instance._SetKillInfo(killCount, enemyType, enemySkinId, enemyName, charaIcon);
	}
	public void _SetKillInfo(int killCount, AvatarType enemyType, int enemySkinId, string enemyName, CharaIcon charaIcon)
	{
		this.Attach.breakCategory.SetKillInfoMessage(MsgType.BreakInfo, killCount, enemyType, enemySkinId, enemyName, charaIcon);
	}

	/// <summary>
	/// 死亡情報メッセージをセットする
	/// </summary>
	public static void SetDeadInfo(AvatarType enemyType, int enemySkinId, string enemyName, CharaIcon charaIcon)
	{
		if(Instance == null)
			return;
		Instance._SetDeadInfo(enemyType, enemySkinId, enemyName, charaIcon);
	}
	public void _SetDeadInfo(AvatarType enemyType, int enemySkinId, string enemyName, CharaIcon charaIcon)
	{
		this.Attach.breakCategory.SetDeadInfoMessage(MsgType.BreakInfo, enemyType, enemySkinId, enemyName, charaIcon);
	}

	#endregion

	#region 戦闘開始待ち

		/// <summary>
	/// バトル開始待ち時のメッセージをセットする
	/// </summary>
	public static void SetBattleWait(float showTime, BattleFieldType fieldType)
	{
		if(Instance == null)
			return;
		Instance._SetBattleWait(showTime, fieldType);
	}
	public void _SetBattleWait(float showTime, BattleFieldType fieldType)
	{
		this.Attach.battleStateCategory.SetBattleWaitMessage(MsgType.BattleWait, showTime, fieldType);
	}

	#endregion

	#region スキル
	
	/// <summary>
	/// スキルメッセージをセットする
	/// </summary>
	public static void SetSkill(string skillName)
	{
		if(Instance == null)
			return;
		Instance._SetSkill(skillName);
	}
	public void _SetSkill(string skillName)
	{
		this.Attach.skillCategory.SetSkill(skillName);
	}
	
	#endregion

	#region ヒット/コンボ
	
	/// <summary>
	/// ヒット/コンボメッセージをセットする
	/// </summary>
	public static void SetHit()
	{
		if(Instance == null)
			return;
		Instance._SetHit();
	}
	public void _SetHit()
	{
		this.Attach.hitCategory.SetHit();
	}
	
	#endregion

	#endregion

	#region 初期化
	
	protected override void Awake ()
	{
		base.Awake();
		Init();
	}

	/// <summary>
	/// 初期化処理
	/// </summary>
	private void Init()
	{
		// 各カテゴリーに対応したメッセージの制御タイプをセットする
		this.Attach.normalCategory.Init(GUIEffectMessageController.Type.Normal);
		this.Attach.battleStateCategory.Init(GUIEffectMessageController.Type.Overrite);
		this.Attach.timeCategory.Init(GUIEffectMessageController.Type.Overrite);
		this.Attach.breakCategory.Init(GUIEffectMessageController.Type.Stack);
		this.Attach.skillCategory.Init(GUIEffectMessageController.Type.Overrite);
		this.Attach.hitCategory.Init(GUIEffectMessageController.Type.Overrite);
	}

	#endregion

	#region デバッグ
#if UNITY_EDITOR && XW_DEBUG

	[System.Serializable]
	public class DebugParam
	{
		public bool execute;
		public Scm.Common.GameParameter.Language language = Scm.Common.GameParameter.Language.Japanese;
		public GUIEffectMessage.MsgType msgType;
		public GUITacticalMessageItem.TacticalType tacticalType;
		public JudgeTypeClient judgeType;
		public bool isDead;
		public AvatarType avatarType;
        public int skinId;
		public int killCount = 1;
		public string enemyName = "Enemy";
		public int battleWaitCount;
		public BattleFieldType battleFieldType;
	}

	[SerializeField]
	private DebugParam debug = new DebugParam();
	private bool isReadMasterData;

	private void DebugUpdate()
	{
		if(!this.debug.execute)
			return;
		this.debug.execute = false;

		if(!this.isReadMasterData)
		{
			MasterData.Read();
			this.isReadMasterData = true;
		}
		// 言語設定
		Scm.Common.Utility.Language = this.debug.language;

		DebugAddMessage(this.debug.msgType, this.debug.tacticalType, this.debug.judgeType);
	}
	private void DebugAddMessage(MsgType msgType, GUITacticalMessageItem.TacticalType tacticalType, JudgeTypeClient judgeType)
	{
		switch(msgType)
		{
			case MsgType.TimeLater:
			{
				BattleFieldTimeEventMasterData masterData;
				BattleFieldTimeEventMaster.Instance.TryGetMasterData(1, out masterData);
				SetTimeLater(masterData);
				break;
			}
			case MsgType.TimeLimit:
			{
				BattleFieldTimeEventMasterData masterData;
				BattleFieldTimeEventMaster.Instance.TryGetMasterData(3, out masterData);
				SetTimeLimit(masterData);
				break;
			}
			case MsgType.TacticalInfo:
			{
				SetTacticalInfo(tacticalType);
				break;
			}
			case MsgType.TacticalWarning:
			{
				SetTacticalWarning(tacticalType);
				break;
			}
			case MsgType.GameJudge:
			{
				SetJudge(judgeType);
				break;
			}
			case MsgType.BreakInfo:
			{
				if(debug.isDead)
				{
					SetDeadInfo(debug.avatarType, debug.skinId, debug.enemyName, null);
				}
				else
				{
					SetKillInfo(debug.killCount, debug.avatarType, debug.skinId, debug.enemyName, null);
				}
				break;
			}
			case MsgType.BattleWait:
			{
				SetBattleWait(debug.battleWaitCount, debug.battleFieldType);
				break;
			}
			case MsgType.GameStart:
			{
				SetGameStart();
				break;
			}
			case MsgType.LevelUP:
			{
				SetLevelUp();
				break;
			}
			case MsgType.Skill:
			{
				SetSkill("SkillName");
				break;
			}
			case MsgType.Hit:
			{
				SetHit();
				break;
			}
		}
	}

	private void OnValidate()
	{
		this.DebugUpdate();
	}

#endif
	#endregion
}