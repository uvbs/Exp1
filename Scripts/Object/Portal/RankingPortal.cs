/// <summary>
/// ランキングポータル
/// 
/// 2013/09/25
/// </summary>
using UnityEngine;
using System.Collections;

public class RankingPortal : PortalBase
{
	#region PortalBase Override
	protected override bool CollisionEnter(GameObject hitObject, Vector3 position, Quaternion rotation)
	{
		Player player = hitObject.GetComponentInChildren<Player>();
		if (player == null)
			{ return false; }

		GUIPopupWindow.Title = "ランキング";
		GUIPopupWindow.Text = "ランキングを表示します";
		GUIPopupWindow.SetDelegateOK(this.OnOK);
		GUIPopupWindow.SetActive(true);

		return true;
	}
	void OnOK()
	{

		GUIPopupWindow.SetActive(false);
		GUILobbyMenu.SetMode(GUILobbyMenu.MenuMode.None);
	}
	protected override bool CollisionExit(GameObject hitObject, Vector3 position, Quaternion rotation)
	{
		Player player = hitObject.GetComponentInChildren<Player>();
		if (player == null)
			{ return false; }

		GUIPopupWindow.SetActive(false);
		
		return true;
	}
	#endregion
}
