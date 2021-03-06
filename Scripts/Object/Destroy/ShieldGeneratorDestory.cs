/// <summary>
/// シールドジェネレータ専用の破壊時処理
/// 
/// 2014/12/04
/// </summary>
using UnityEngine;
using System.Collections;

public class ShieldGeneratorDestory : GadgetDestroyBase
{
	/// <summary>
	/// 破壊時処理
	/// </summary>
	public override void Destroy(Gadget gadget)
	{
		// プレイヤー取得
		Player player = GameController.GetPlayer();
		if(player == null) return;

		// 破壊メッセージを表示する
		if(player.TeamType == gadget.TeamType)
		{
			// 破壊メッセージ
			GUIEffectMessage.SetTacticalWarning(GUITacticalMessageItem.TacticalType.ShieldGenDst);
		}
		else
		{
			// 被破壊メッセージ
			GUIEffectMessage.SetTacticalInfo(GUITacticalMessageItem.TacticalType.ShieldGenDst);
		}
	}
}
