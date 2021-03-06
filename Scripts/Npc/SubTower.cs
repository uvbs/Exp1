/// <summary>
/// サブタワー
/// 
/// 2013/03/18
/// </summary>
using UnityEngine;
using System.Collections;

using Scm.Common.GameParameter;

public class SubTower : TowerBase
{
	protected override string AnimationDamage  { get{ return "subTower_damage"; } }
	protected override string AnimationDestroy { get{ return "subTower_destroy"; } }
	
	#region ObjectBase Override
	#region 破壊処理
	protected override void Destroy()
	{
		base.Destroy();
		// 画面エフェクト
		{
			Player player = GameController.GetPlayer();
			if (player != null)
			{
				if(player.TeamType == this.TeamType)
				{
					// 撃破演出
					GUIEffectMessage.SetTacticalWarning(GUITacticalMessageItem.TacticalType.SubTowerDst);
				}
				else
				{
					// 被撃破演出
					GUIEffectMessage.SetTacticalInfo(GUITacticalMessageItem.TacticalType.SubTowerDst);
				}
			}
		}
	}
	#endregion
	#endregion
}
