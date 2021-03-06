/// <summary>
/// テレポータベース
/// 
/// 2013/04/03
/// </summary>
using UnityEngine;
using Scm.Common.Master;
using Scm.Common.GameParameter;

public class WarpBase : Gadget
{
	#region コリジョン
	void OnTriggerEnter(Collider collider)
	{
		Player player = collider.gameObject.GetComponent<Player>();
		if (player != null && (this.TeamType == player.TeamType || this.TeamType == TeamType.Unknown))
		{
			BattlePacket.SendWarp(this.InFieldId);
		}
        if (player == null) {
            Person person = collider.gameObject.GetComponent<Person>();
            if (person != null && person.EntrantInfo.NeedCalcInertia && GameController.IsRoomOwner()) {
                BattlePacket.SendProxyWarp(this.InFieldId, person.InFieldId);
            }
        }
	}
	#endregion
}
