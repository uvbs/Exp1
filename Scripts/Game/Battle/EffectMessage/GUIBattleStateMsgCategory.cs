/// <summary>
/// バトル状態エフェクトメッセージのカテゴリー処理
/// 
/// 2014/12/08
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;

public class GUIBattleStateMsgCategory : GUIEffectMessageCategory
{
	#region 定数

	/// <summary>
	/// [メインタワー攻撃メッセージ]がセット出来るまでの時間
	/// カウント開始は表示されてから(*表示終了後ではないので注意*)
	/// </summary>
	private const float SetMainTowerAtkTime = 15f;

	#endregion

	#region フィールド&プロパティ

	/// <summary>
	/// [メインタワー攻撃メッセージ]がセット出来るまでの時間カウント用
	/// </summary>
	private float setMainTowerAtkCount = 0f;

	#endregion

	#region 戦略メッセージのセット

	/// <summary>
	/// 戦略メッセージをセットする.
	/// </summary>
	public void SetTacticalMessage(GUIEffectMessage.MsgType msgType, GUITacticalMessageItem.TacticalType tacticalType)
	{
		// セット出来る状態なのかどうかチェックする
		if(!CheckSetTacticalMsg(tacticalType)) return;

		// プレハブ情報取得
		GUIEffectMsgItemBase itemResource;
		if(!this.msgResourceDictionary.TryGetValue(msgType, out itemResource))
			return;

		// メッセージアイテム生成
		GUIEffectMsgItemBase newItem = GUIEffectMsgItemBase.Create(itemResource, this.Attach.parentList, true);
		if(newItem == null)
			return;

		// 戦略メッセージアイテムに変換しセットアップ処理を行う
		GUITacticalMessageItem tacticalMsgItem = newItem as GUITacticalMessageItem;
		if(tacticalMsgItem == null)
			return;
		tacticalMsgItem.Setup(tacticalType);
		
		// メッセージコントロールクラスに登録
		this.controller.SetEffectMessage(tacticalMsgItem);
	}

	/// <summary>
	/// 戦略メッセージをセットするかどうかチェックする
	/// </summary>
	private bool CheckSetTacticalMsg(GUITacticalMessageItem.TacticalType tacticalType)
	{
		// バトルが終了している場合はメッセージを表示しない
		if (NetworkController.ServerValue != null && NetworkController.ServerValue.FieldStateType == FieldStateType.Extra)
			return false;

		if(tacticalType == GUITacticalMessageItem.TacticalType.MainTowerAtk)
		{
			// [メインタワー攻撃メッセージ]は他のメッセージが表示されているまたは
			// [メインタワー攻撃メッセージ]が指定数秒以内に表示されている時はセットさせない
			if(this.controller.ItemCount > 0 || this.setMainTowerAtkCount > 0)
			{
				return false;
			}
			else
			{
				this.setMainTowerAtkCount = SetMainTowerAtkTime;
				return true;
			}
		}

		return true;
	}

	#endregion

	#region 勝敗メッセージのセット

	/// <summary>
	/// 勝敗メッセージをセットする
	/// </summary>
	public void SetJudgeMessage(GUIEffectMessage.MsgType msgType, JudgeTypeClient judgeType)
	{
		// プレハブ情報取得
		GUIEffectMsgItemBase itemResource;
		if(!this.msgResourceDictionary.TryGetValue(msgType, out itemResource))
			return;
		
		// メッセージアイテム生成
		GUIEffectMsgItemBase newItem = GUIEffectMsgItemBase.Create(itemResource, this.Attach.parentList, true);
		if(newItem == null)
			return;

		// 勝敗メッセージアイテムに変換しセットアップ処理を行う
		GUIJudgeMessageItem judgeMsgItem = newItem as GUIJudgeMessageItem;
		if(judgeMsgItem == null)
			return;
		judgeMsgItem.Setup(judgeType);
		
		// メッセージコントロールクラスに登録
		this.controller.SetEffectMessage(judgeMsgItem);
	}

	#endregion

	#region ゲーム開始位待ちメッセージのセット

	/// <summary>
	/// ゲーム開始待ち時のメッセージをセットする
	/// </summary>
	public void SetBattleWaitMessage(GUIEffectMessage.MsgType msgType, float showTime, BattleFieldType fieldType)
	{
		// プレハブ情報取得
		GUIEffectMsgItemBase itemResource;
		if(!this.msgResourceDictionary.TryGetValue(msgType, out itemResource))
			return;
		
		// メッセージアイテム生成
		GUIEffectMsgItemBase newItem = GUIEffectMsgItemBase.Create(itemResource, this.Attach.parentList, true);
		if(newItem == null)
			return;

		// ゲーム開始待ち時のメッセージアイテムに変換しセットアップ処理を行う
		GUIBattleWaitMessageItem battleWaitMsgItem = newItem as GUIBattleWaitMessageItem;
		if(battleWaitMsgItem == null)
			return;
		battleWaitMsgItem.Setup(showTime, fieldType);
		
		// メッセージコントロールクラスに登録
		this.controller.SetEffectMessage(battleWaitMsgItem);
	}

	#endregion

	#region 更新
	
	/// <summary>
	/// 更新
	/// </summary>
	protected override void Update ()
	{
		// 時間更新
		MainTowerAtkTimeUpdate();

		base.Update();
	}

	/// <summary>
	/// [メインタワー攻撃メッセージ]の時間更新処理
	/// </summary>
	private void MainTowerAtkTimeUpdate()
	{
		if(this.setMainTowerAtkCount <= 0) return;
		this.setMainTowerAtkCount -= Time.deltaTime;
		this.setMainTowerAtkCount = Mathf.Max(this.setMainTowerAtkCount, 0);
	}

	#endregion
}
