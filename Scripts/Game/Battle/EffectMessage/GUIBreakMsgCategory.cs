/// <summary>
/// キルデスメッセージカテゴリーの処理
/// 
/// 2014/12/08
/// </summary>
using UnityEngine;
using System.Collections;

public class GUIBreakMsgCategory : GUIEffectMessageCategory
{
	#region キル情報メッセージのセット

	/// <summary>
	/// キル情報メッセージをセット
	/// </summary>
	public void SetKillInfoMessage(GUIEffectMessage.MsgType msgType, int killCount, AvatarType enemyType, int enemySkinId, string enemyName, CharaIcon charaIcon)
	{
		// プレハブ情報取得
		GUIEffectMsgItemBase itemResource;
		if(!this.msgResourceDictionary.TryGetValue(msgType, out itemResource))
			return;
		
		// メッセージアイテム生成
		GUIEffectMsgItemBase newItem = GUIEffectMsgItemBase.Create(itemResource, this.Attach.parentList, true);
		if(newItem == null)
			return;

		// キル情報メッセージアイテムに変換しキル情報のセットアップ処理を行う
		GUIBreakInfoMessageItem breakInfoMsgItem = newItem as GUIBreakInfoMessageItem;
		if(breakInfoMsgItem == null)
			return;
		breakInfoMsgItem.KilledSetup(killCount, enemyType, enemySkinId, enemyName, charaIcon);
		
		// メッセージコントロールクラスに登録
		this.controller.SetEffectMessage(breakInfoMsgItem);
	}

	#endregion

	#region 死亡情報メッセージのセット

	/// <summary>
	/// 死亡情報メッセージをセット
	/// </summary>
	public void SetDeadInfoMessage(GUIEffectMessage.MsgType msgType, AvatarType enemyType, int enemySkinId, string enemyName, CharaIcon charaIcon)
	{
		// プレハブ情報取得
		GUIEffectMsgItemBase itemResource;
		if(!this.msgResourceDictionary.TryGetValue(msgType, out itemResource))
			return;
		
		// メッセージアイテム生成
		GUIEffectMsgItemBase newItem = GUIEffectMsgItemBase.Create(itemResource, this.Attach.parentList, true);
		if(newItem == null)
			return;

		// キル情報メッセージアイテムに変換し死亡情報のセットアップ処理を行う
		GUIBreakInfoMessageItem breakInfoMsgItem = newItem as GUIBreakInfoMessageItem;
		if(breakInfoMsgItem == null)
			return;
		breakInfoMsgItem.DeadSetup(enemyType, enemySkinId, enemyName, charaIcon);
		
		// メッセージコントロールクラスに登録
		this.controller.SetEffectMessage(breakInfoMsgItem);
	}

	#endregion
}
