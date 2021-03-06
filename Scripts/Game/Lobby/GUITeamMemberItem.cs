/// <summary>
/// チームメンバーアイテム
/// 
/// 2014/12/05
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.Packet;

public class GUITeamMemberItem : MonoBehaviour
{
	#region フィールド＆プロパティ
	private GroupMemberParameter memberParam;
	public long PlayerId { get { return memberParam != null ? memberParam.PlayerId : 0; } }
	public string PlayerName { get { return memberParam != null ? memberParam.PlayerName : string.Empty; } }
	public bool IsLeader { get; private set; }

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	private Attach _attach;
	public  Attach attach { get { return _attach; } }
	[System.Serializable]
	public class Attach
	{
		public UIButton uiButton;
		public UILabel nameLabel;
		public UISprite Icon_Leader;
		public UISprite Icon_Invitation;
		public UISprite Logout;
	}
	#endregion

	public void SetMemberParam(GroupMemberParameter memberParam, bool isLeader)
	{
		this.memberParam = memberParam;
		this.IsLeader = isLeader;
		DisplayMemberParam();
	}
	public void DisplayMemberParam()
	{
		if(this.memberParam != null)
		{
			if(this.attach.uiButton != null) { this.attach.uiButton.enabled = true; }
			if(this.attach.nameLabel != null) { this.attach.nameLabel.text = this.memberParam.PlayerName; }
			if(this.attach.Icon_Leader != null) { this.attach.Icon_Leader.enabled = this.IsLeader; }
			if(this.attach.Icon_Invitation != null) { this.attach.Icon_Invitation.enabled= false; }
			if(this.attach.Logout != null) { this.attach.Logout.enabled = !this.memberParam.IsLogin; }
		}
		else
		{
			if(this.attach.uiButton != null) { this.attach.uiButton.enabled = false; }
			if(this.attach.nameLabel != null) { this.attach.nameLabel.text = string.Empty; }
			if(this.attach.Icon_Leader != null) { this.attach.Icon_Leader.enabled = false; }
			if(this.attach.Icon_Invitation != null) { this.attach.Icon_Invitation.enabled= false; }
			if(this.attach.Logout != null) { this.attach.Logout.enabled = true; }
		}
	}
}
