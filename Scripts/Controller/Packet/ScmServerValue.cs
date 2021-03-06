using UnityEngine;
using System;
using System.Collections.Generic;
using Scm.Common.Packet;
using Scm.Common.PacketCode;
using Scm.Common.GameParameter;
using ExitGames.Client.Photon;
using Asobimo.Photon.Packet;

namespace Scm.Client
{
	/// <summary>
	/// サーバから送られてきた値をダイレクトに保存しておくクラス.
	/// </summary>
	public class ScmServerValue
	{
		public bool IsConnected { get { return GameListener.ConnectFlg; } }

		// PlayerId
		public int PlayerId { get; private set; }

		// 現在のエリア情報.
		public AreaType AreaType { get; private set; }
		public int BattleFieldId { get; private set; }
		public int FieldId { get { return (this.PlayerInfo != null) ? this.PlayerInfo.FieldId : 0; } }

		// 現在の自分の情報.
		public TeamType TeamType { get { return (this.PlayerInfo != null) ? this.PlayerInfo.TeamType : TeamType.Blue; } }
		public int InFieldId { get { return (this.PlayerInfo != null) ? this.PlayerInfo.InFieldId : 0; } }
		public PlayerInfo PlayerInfo { get; private set; }
		public PlayerStatusInfo PlayerStatusInfo { get; private set; }

		// 現在の自分のチーム情報
		public TeamParameter TeamParameter { get; private set; }
		public List<GroupMemberParameter> Members { get; private set; }

		// 現在のマッチング情報
		public MatchingStatus MatchingStatus { get; private set; }

		// 現在のバトル状態
		public FieldStateType FieldStateType { get; private set; }

		/// <summary>
		/// 自分がTeamに所属しているかどうか.
		/// </summary>
		public bool IsJoinedTeam
		{
			get
			{
				return ((this.TeamParameter != null) && (this.TeamParameter.TeamId != 0));
			}
		}
		/// <summary>
		/// 自分がTeamのLeaderかどうか.
		/// </summary>
		public bool IsTeamLeader
		{
			get
			{
				return ((this.Members != null) && 
						(0 < this.Members.Count) &&
						(this.Members[0].PlayerId == this.PlayerId));
			}
		}

		public ScmServerValue(int playerId)
		{
			this.PlayerId = playerId;
		}

		public void SetEnterLobbyResponse(EnterLobbyRes packet, PlayerInfo playerInfo)
		{
			this.AreaType = AreaType.Lobby;
			this.BattleFieldId = 0;

			this.PlayerInfo = playerInfo;
			this.PlayerStatusInfo = new PlayerStatusInfo(packet.GetPlayerStatusPacketParameter());
		}
		public void SetEnterFieldResponse(EnterFieldRes packet, PlayerInfo playerInfo)
		{
			this.AreaType = AreaType.Field;
			this.BattleFieldId = packet.BattleFieldId;

			this.PlayerInfo = playerInfo;
			this.PlayerStatusInfo = new PlayerStatusInfo();
		}
		public void SetReEnterFieldResponse(ReEnterFieldRes packet, PlayerInfo playerInfo)
		{
			this.AreaType = AreaType.Field;
			this.BattleFieldId = packet.BattleFieldId;

			this.PlayerInfo = playerInfo;
			this.PlayerStatusInfo = new PlayerStatusInfo();
		}
		public void SetRespawnPlayer(PlayerInfo playerInfo)
		{
			this.PlayerInfo = playerInfo;
		}
		public void SetTeamInfo(TeamInfoEvent packet)
		{
			this.TeamParameter = packet.GetTeamParameter();
			this.Members = packet.GetMembers();

			GUILobbyResident.UpdateTeamInfo(this.TeamParameter, this.Members);
		}
		public void SetMatchingStatus(MatchingStatus matchingStatus)
		{
			this.MatchingStatus = matchingStatus;
		}
		public void SetFieldStateType(FieldStateType fieldStateType)
		{
			this.FieldStateType = fieldStateType;
		}
		public void SetMoney(int money)
		{
			this.PlayerStatusInfo.SetGameMoney(money);
		}
	}
}
