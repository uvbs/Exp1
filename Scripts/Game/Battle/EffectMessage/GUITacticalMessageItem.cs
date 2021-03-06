using UnityEngine;
using System.Collections;
using Scm.Common.Master;
using System.Collections.Generic;

public class GUITacticalMessageItem : GUIEffectMessageItem
{
	#region 戦略種類

	public enum TacticalType
	{
		TeamSkill,
		SubTowerDst,
		GuardianDst,
		MainTowerHalfDamege,
		ShieldGenDst,
		HealPodDst,
		MainTowerAtk,
		PoiseTeamSkill,
	}

	#endregion

	#region アタッチオブジェクト

	[System.Serializable]
	public class AttachTacticalObject
	{
		[SerializeField]
		private UILabel messageLabel;
		public UILabel MessageLabel { get { return messageLabel; } }

		[SerializeField]
		private GameObject infoObject;
		public GameObject InfoObject { get { return infoObject; } }

		[SerializeField]
		private GameObject warningObject;
		public GameObject WarningObject { get { return warningObject; } }

		[SerializeField]
		private GameObject dangerObject;
		public GameObject DangerObject { get { return dangerObject; } }
	}

	#endregion

	#region フィールド&プロパティ

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	private AttachTacticalObject attachTacticalObj;
	public AttachTacticalObject AttachTacticalObj { get { return attachTacticalObj; } }

	/// <summary>
	/// 戦略タイプ
	/// </summary>
	protected TacticalType tacticalType;

	/// <summary>
	/// 自チーム表示するテキストタイプリスト
	/// </summary>
	protected static Dictionary<TacticalType, TextType> infoTextTypeList;

	/// <summary>
	/// 敵チーム表示するテキストタイプリスト
	/// </summary>
	protected static Dictionary<TacticalType, TextType> warningTeamTextTypeList;

	#endregion

	#region 初期化

	void Awake()
	{
		SetTextType();

		// 全てのメッセージの重要度表示をOFFにしておく
		if(this.AttachTacticalObj.InfoObject != null)
		{
			this.AttachTacticalObj.InfoObject.SetActive(false);
		}
		if(this.AttachTacticalObj.WarningObject != null)
		{
			this.AttachTacticalObj.WarningObject.SetActive(false);
		}
		if(this.AttachTacticalObj.DangerObject != null)
		{
			this.AttachTacticalObj.DangerObject.SetActive(false);
		}
	}

	#endregion
	
	#region セットアップ

	public virtual void Setup (TacticalType tacticalType)
	{
		this.tacticalType = tacticalType;

		// ラベルの設定
		if(this.attachTacticalObj.MessageLabel == null)
			return;

		// テキストタイプ取得
		TextType textType;
		if(!TryGetTextType(this.MsgType, tacticalType, out textType))
		{
			string debugMsg = "NotFound TextType MsgType = " + this.MsgType;
			Debug.LogWarning(debugMsg);
			BugReportController.SaveLogFile(debugMsg);
			return;
		}

		// テキストマスターデータ取得
		string message;
		if(tacticalType != TacticalType.PoiseTeamSkill)
		{
			message = MasterData.GetText(textType);
		}
		else
		{
			// チームスキル発動までのメッセージ(現在は残り1ブレイクで固定)
			message = MasterData.GetText(textType, new string[] {"1"});
		}
		this.attachTacticalObj.MessageLabel.text = message;

		// 戦略タイプによってメッセージの重要度表示を決める
		SetImportanceLevel();
	}

	/// <summary>
	/// 戦略タイプによってメッセージの重要度表示を決める
	/// </summary>
	private void SetImportanceLevel()
	{
		switch(tacticalType)
		{
			case TacticalType.MainTowerHalfDamege:
			{
				if(this.AttachTacticalObj.DangerObject != null)
				{
					this.AttachTacticalObj.DangerObject.SetActive(true);
				}
				break;
			}

			case TacticalType.TeamSkill:
			case TacticalType.PoiseTeamSkill:
			{
				if(this.AttachTacticalObj.WarningObject != null)
				{
					this.AttachTacticalObj.WarningObject.SetActive(true);
				}
				break;
			}

			// 意図しないタイプが来た場合でもInfoを表示するようにする(何も表示しないのはまずいので)
			default:
			{
				if(this.AttachTacticalObj.InfoObject != null)
				{
					this.AttachTacticalObj.InfoObject.SetActive(true);
				}
				break;
			}
		}
	}

	#endregion

	#region テキストタイプ

	/// <summary>
	/// 各テキストタイプをリストに保存する
	/// </summary>
	private static void SetTextType()
	{
		if(infoTextTypeList == null)
		{
			infoTextTypeList = new Dictionary<TacticalType, TextType>();
			infoTextTypeList.Add(TacticalType.TeamSkill, TextType.TX017_TeamSkill);
			infoTextTypeList.Add(TacticalType.GuardianDst, TextType.TX019_EnemyGuardianDestroy);
			infoTextTypeList.Add(TacticalType.SubTowerDst, TextType.TX021_EnemySubTowerDestroy);
			infoTextTypeList.Add(TacticalType.MainTowerHalfDamege, TextType.TX023_MainTowerDamageHalf);
			infoTextTypeList.Add(TacticalType.ShieldGenDst, TextType.TX032_EnemyShieldGenDestroy);
			infoTextTypeList.Add(TacticalType.HealPodDst, TextType.TX034_EnemyHealPodDestroy);
			infoTextTypeList.Add(TacticalType.MainTowerAtk, TextType.TX036_MainTowerAttack);
			infoTextTypeList.Add(TacticalType.PoiseTeamSkill, TextType.TX039_PoiseTeamSkill);
		}
		
		if(warningTeamTextTypeList == null)
		{
			warningTeamTextTypeList = new Dictionary<TacticalType, TextType>();
			warningTeamTextTypeList.Add(TacticalType.TeamSkill, TextType.TX018_EnemyTeamSkill);
			warningTeamTextTypeList.Add(TacticalType.GuardianDst, TextType.TX020_GuardianDestroy);
			warningTeamTextTypeList.Add(TacticalType.SubTowerDst, TextType.TX022_SubTowerDestroy);
			warningTeamTextTypeList.Add(TacticalType.MainTowerHalfDamege, TextType.TX023_MainTowerDamageHalf);
			warningTeamTextTypeList.Add(TacticalType.ShieldGenDst, TextType.TX033_ShieldGenDestroy);
			warningTeamTextTypeList.Add(TacticalType.HealPodDst, TextType.TX035_HealPodDestroy);
			warningTeamTextTypeList.Add(TacticalType.MainTowerAtk, TextType.TX036_MainTowerAttack);
			warningTeamTextTypeList.Add(TacticalType.PoiseTeamSkill, TextType.TX040_PoiseEnemyTeamSkill);
		}
	}

	/// <summary>
	/// 味方側敵チーム側の判定を行いTextTypeを取得する
	/// </summary>
	private static bool TryGetTextType(GUIEffectMessage.MsgType msgType, TacticalType tacticalType, out TextType textType)
	{
		string debugMsg = "NotFound TextType TacticalType = ";
		bool isSuccess = true;

		switch(msgType)
		{
			case GUIEffectMessage.MsgType.TacticalInfo:
			{
				// 通常情報メッセージ
				if(!infoTextTypeList.TryGetValue(tacticalType, out textType))
				{
					debugMsg += tacticalType;
					Debug.LogWarning(debugMsg);
					BugReportController.SaveLogFile(debugMsg);
					isSuccess = false;
				}
				break;
			}

			case GUIEffectMessage.MsgType.TacticalWarning:
			{
				// 警告メッセージ
				if(!warningTeamTextTypeList.TryGetValue(tacticalType, out textType))
				{
					debugMsg += tacticalType;
					Debug.LogWarning(debugMsg);
					BugReportController.SaveLogFile(debugMsg);
					isSuccess = false;
				}
				break;
			}

			default:
			{
				textType = TextType.None;
				isSuccess = false;
				break;
			}
		}

		return isSuccess;
	}

	#endregion
}
