/// <summary>
/// バトルポータル
/// 
/// 2013/08/20
/// </summary>
using UnityEngine;
using System.Collections;

public class BattlePortal : PortalBase
{
	#region PortalBase Override
	protected override bool CollisionEnter(GameObject hitObject, Vector3 position, Quaternion rotation)
	{
		Player player = hitObject.GetComponentInChildren<Player>();
		if (player == null)
			{ return false; }
		if (NetworkController.Instance == null)
			{ return false; }

		// TODO:マップ番号直書き
/*
		string mapID = this.ObjectData.Name.Substring(this.ObjectData.Name.Length-3);
		int mapNo;
		if (!int.TryParse(mapID, out mapNo))
			{ return false; }
		LobbyMain.NextSceneType(LobbyMain.NextType.Battle);
*/

		return true;
	}
	#endregion
}
