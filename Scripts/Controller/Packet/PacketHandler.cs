/// <summary>
/// Packet関連の処理情報をまとめたクラス.
/// 2014/03/11 パケット追加時にコード変更が必要な個所が3ファイルに散っていたのでまとめた.
/// 
/// 
/// 2014/03/11
/// </summary>

using System;
using System.Collections.Generic;
using Asobimo.Photon.Packet;
using Scm.Common.Packet;
using Scm.Common.PacketCode;
using UnityEngine;

namespace Scm.Client
{
	static public class PacketHandler
	{
		static private LobbyPacket LobbyPacket { get { return NetworkController.Instance.LobbyPacket; } }
		static private CommonPacket CommonPacket { get { return NetworkController.Instance.CommonPacket; } }
		static private BattlePacket BattlePacket { get { return NetworkController.Instance.BattlePacket; } }
		
		#region Response
		
		/// <summary>
		/// パケットコードに対応するパケットクラスのインスタンスを生成.
		/// </summary>
		/// <returns>
		/// 対応するパケットクラスが存在する　= true, 存在しない = false
		/// </returns>
		/// <param name='code'>
		/// パケットコード.
		/// </param>
		/// <param name='packet'>
		/// 生成したパケットクラスのインスタンス.
		/// </param>
		static public bool TryCreatePacket(ResponseCode code, out PacketBase packet)
		{
			#region switch(ResponseCode)
			switch(code)
			{
				// ネットワークコントローラー
				case ResponseCode.Login:						packet = new LoginRes();	return true;
				case ResponseCode.Logout:						packet = new LogoutRes();	return true;
				case ResponseCode.ServerStatus:					packet = new ServerStatusRes();	return true;
				case ResponseCode.ReEnterField:					packet = new ReEnterFieldRes();	return true;
				case ResponseCode.RetainPlayerAll:				packet = new RetainPlayerAllRes(); return true;
				case ResponseCode.CreatePlayer:					packet = new CreatePlayerRes(); return true;
				case ResponseCode.SelectPlayer:					packet = new SelectPlayerRes(); return true;
				// ロビー
				case ResponseCode.EnterLobby:					packet = new EnterLobbyRes();	return true;
				case ResponseCode.ExitLobby:					packet = new ExitLobbyRes();	return true;
				case ResponseCode.LobbyList:					packet = new LobbyListRes();	return true;
				case ResponseCode.LobbyNum:						packet = new LobbyNumRes();	return true;
				case ResponseCode.Ranking:						packet = new RankingRes();	return true;
				//case ResponseCode.RankingList:					packet = new RankingListRes();	return true;
				case ResponseCode.Matching:						packet = new MatchingRes();	return true;
				case ResponseCode.PlayerCharacterStorage:		packet = new PlayerCharacterBoxRes(); return true;
				case ResponseCode.PlayerCharacterAll:			packet = new PlayerCharacterAllRes(); return true;
				case ResponseCode.PlayerCharacter:				packet = new PlayerCharacterRes(); return true;
				case ResponseCode.PlayerStatus:					packet = new PlayerStatusRes(); return true;
                case ResponseCode.BattleHistory:                packet = new BattleHistoryRes(); return true;
                case ResponseCode.BattleHistoryDetail:          packet = new BattleHistoryDetailRes(); return true;
                case ResponseCode.PlayerMiscInfo:               packet = new PlayerMiscInfoRes(); return true;
				case ResponseCode.SellMultiPlayerCharacterCalc:	packet = new SellMultiPlayerCharacterCalcRes(); return true;
				case ResponseCode.SellMultiPlayerCharacter:		packet = new SellMultiPlayerCharacterRes(); return true;
				case ResponseCode.SetSymbolPlayerCharacter:		packet = new SetSymbolPlayerCharacterRes(); return true;
				case ResponseCode.Powerup:						packet = new PowerupRes(); return true;
				case ResponseCode.PowerupCalc:					packet = new PowerupCalcRes(); return true;
				case ResponseCode.Evolution:					packet = new EvolutionRes(); return true;
				case ResponseCode.EvolutionCalc:				packet = new EvolutionCalcRes(); return true;
				case ResponseCode.SynchroFusion:				packet = new SynchroFusionRes(); return true;
				case ResponseCode.SynchroFusionCalc:			packet = new SynchroFusionCalcRes(); return true;
				case ResponseCode.SetLockPlayerCharacter:		packet = new SetLockPlayerCharacterRes(); return true;
				case ResponseCode.GetPowerupSlot:				packet = new GetPowerupSlotRes(); return true;
				case ResponseCode.SetPowerupSlot:				packet = new SetPowerupSlotRes(); return true;
				case ResponseCode.SetPowerupSlotCalc:			packet = new SetPowerupSlotCalcRes(); return true;
				case ResponseCode.AddPowerupSlot:				packet = new AddPowerupSlotRes(); return true;
				case ResponseCode.SetPlayerCharacterNewFlagAll: packet = new SetPlayerCharacterNewFlagAllRes(); return true;
				case ResponseCode.PlayerItemBox:				packet = new PlayerItemBoxRes(); return true;
				case ResponseCode.PlayerItemAll:				packet = new PlayerItemAllRes(); return true;
				case ResponseCode.SellMultiPlayerItem:			packet = new SellMultiPlayerItemRes(); return true;
				case ResponseCode.SetLockPlayerItem:			packet = new SetLockPlayerItemRes(); return true;
				case ResponseCode.SellPlayerItem:				packet = new SellPlayerItemRes(); return true;
				case ResponseCode.SetPlayerItemNewFlagAll:		packet = new SetPlayerItemNewFlagAllRes(); return true;
				case ResponseCode.MailBox:						packet = new MailBoxRes(); return true;
				case ResponseCode.AdminMail:					packet = new AdminMailRes(); return true;
				case ResponseCode.SetAdminMailReadFlag:			packet = new SetAdminMailReadFlagRes(); return true;
				case ResponseCode.SetAdminMailReadFlagAll:		packet = new SetAdminMailReadFlagAllRes(); return true;
				case ResponseCode.SetLockAdminMail:				packet = new SetLockAdminMailRes(); return true;
				case ResponseCode.DeleteAdminMail:				packet = new DeleteAdminMailRes(); return true;
				case ResponseCode.DeleteAdminMailAll:			packet = new DeleteAdminMailAllRes(); return true;
				case ResponseCode.PresentMail:					packet = new PresentMailRes(); return true;
				case ResponseCode.SetPresentMailReadFlag:		packet = new SetPresentMailReadFlagRes(); return true;
				case ResponseCode.SetLockPresentMail:			packet = new SetLockPresentMailRes(); return true;
				case ResponseCode.DeletePresentMail:			packet = new DeletePresentMailRes(); return true;
				case ResponseCode.DeletePresentMailAll:			packet = new DeletePresentMailAllRes(); return true;
				case ResponseCode.ReceivePresentMailItem:		packet = new ReceivePresentMailItemRes(); return true;
				case ResponseCode.ReceivePresentMailItemAll:	packet = new ReceivePresentMailItemAllRes(); return true;
				case ResponseCode.Achievement:					packet = new AchievementRes(); return true;
				case ResponseCode.SetAchievementNewFlagAll:		packet = new CompleteAchievementEvent(); return true;
				case ResponseCode.ReceiveAchievementReward:		packet = new ReceiveAchievementRewardRes(); return true;
				case ResponseCode.ReceiveAchievementRewardAll:	packet = new ReceiveAchievementRewardAllRes(); return true;
				case ResponseCode.ReceiveWebStore:				packet = new ReceiveWebStoreRes(); return true;
				// 共通
				case ResponseCode.Entrant:						packet = new EntrantRes();	return true;
				case ResponseCode.EntrantAll:					packet = new EntrantAllRes();	return true;
				case ResponseCode.Echo:							packet = new EchoRes();	return true;
				case ResponseCode.CharacterDeckNum:				packet = new CharacterDeckNumRes(); return true;
				case ResponseCode.CharacterDeckList:			packet = new CharacterDeckListRes(); return true;
				case ResponseCode.CharacterDeck:				packet = new CharacterDeckRes(); return true;
				case ResponseCode.SetCharacterDeck:				packet = new SetCharacterDeckRes(); return true;
				case ResponseCode.SelectCharacterDeck:			packet = new SelectCharacterDeckRes(); return true;
				case ResponseCode.SelectCharacter:				packet = new SelectNextDeckCharacterRes(); return true;
				case ResponseCode.Auth:							packet = new AuthRes(); return true;
				// バトル
				case ResponseCode.EnterField:					packet = new EnterFieldRes();	return true;
				case ResponseCode.ExitField:					packet = new ExitFieldRes();	return true;
				case ResponseCode.Hit:							packet = new HitRes();	return true;
				case ResponseCode.Judge:						packet = new JudgeRes();	return true;
				case ResponseCode.RemainingTime:				packet = new RemainingTimeRes();	return true;
				case ResponseCode.Warp:							packet = new WarpRes();	return true;
				case ResponseCode.TeamSkillPoint:				packet = new TeamSkillPointRes();	return true;
				case ResponseCode.GameResult:					packet = new GameResultRes();	return true;
				case ResponseCode.ItemGet:						packet = new ItemGetRes();	return true;
				case ResponseCode.KillSelf:						packet = new KillSelfRes();	return true;
				case ResponseCode.Buff:							packet = new BuffRes();	return true;
				case ResponseCode.TargetMarker:					packet = new TargetMarkerRes(); return true;
				case ResponseCode.TargetMarkerAll:				packet = new TargetMarkerAllRes(); return true;
				case ResponseCode.Transport:					packet = new TransportRes(); return true;
				case ResponseCode.SideGauge:					packet = new SideGaugeRes();	return true;
            // チーム
#if OLD_TEAM_LOGIC
                case ResponseCode.TeamCreate:					packet = new TeamCreateRes(); return true;
				case ResponseCode.TeamRemoveMember:				packet = new TeamRemoveMemberRes(); return true;
				case ResponseCode.TeamSearch:					packet = new TeamSearchRes(); return true;
				case ResponseCode.TeamJoin:						packet = new TeamJoinRes(); return true;
				case ResponseCode.TeamMember:					packet = new TeamMemberRes(); return true;
#endif
                case ResponseCode.SetCurrentAvatar:             packet = new SetCurrentAvatarRes(); return true;
                case ResponseCode.GetCharacterAvatarAll:        packet = new GetCharacterAvatarAllRes(); return true;
                case ResponseCode.GetCharacterReplayVoiceAll:   packet = new GetCharacterReplayVoiceAllRes (); return true;
                case ResponseCode.GetCharacterStoryAll:         packet = new GetCharacterStoryAllRes (); return true;
                case ResponseCode.GetCharacterWallpaperAll:     packet = new GetCharacterWallpaperAllRes() ; return true;
                case ResponseCode.TeamInvite:                   packet = new TeamInviteRes(); return true;
                case ResponseCode.TeamMember:                   packet = new TeamMemberRes(); return true;
                case ResponseCode.TeamOperation:                packet = new TeamOperationRes(); return true;
                case ResponseCode.LoginBonus:                   packet = new LoginBonusRes(); return true;
                case ResponseCode.LoginBonusReward:             packet = new LoginBonusRewardRes(); return true;
                case ResponseCode.ProxyReq:                     packet = new ProxyRes(); return true;
                case ResponseCode.GetGuideStep:                 packet = new GetGuideRes(); return true;
                case ResponseCode.SetGuideStep:                 packet = new SetGuideRes(); return true;
                case ResponseCode.ServerMapping:                packet = new ServerMappingRes(); return true;
                case ResponseCode.Recruitment:                  packet = new RecruitmentRes(); return true;
                case ResponseCode.GiftPackage:                  packet = new GiftPackageRes(); return true;
                case ResponseCode.ObtainedAllCharacterAvatar:   packet = new ObtainedCharacterAvatarAllRes(); return true;

#if XW_DEBUG || ENABLE_GM_COMMAND
				case ResponseCode.GmCommand:					packet = new GmCommandRes(); return true;
#endif
				default:
                    Debug.Log("===> " + code);
					packet = null;
					return false;
			}
#endregion
		}
		
		/// <summary>
		/// Eventパケットの処理を実行する.
		/// </summary>
		/// パケットコード.
		/// </param>
		/// パラメータセット済みのパケット.
		/// </param>
		static public void RunResponsePacket(ResponseCode code, PacketBase packet)
		{
			#region switch(ResponseCode)
			switch(code)
			{
			// ネットワークコントローラ解析
			case ResponseCode.Login:						NetworkController.Instance.OperationResponseLogin(packet as LoginRes);	break;
			case ResponseCode.Logout:						NetworkController.Instance.OperationResponseLogout(packet as LogoutRes);	break;
			case ResponseCode.ServerStatus:					NetworkController.Instance.OperationResponseServerStatus(packet as ServerStatusRes);	break;
			case ResponseCode.RetainPlayerAll:				NetworkController.Instance.OperationRetainPlayerAll(packet as RetainPlayerAllRes); break;
			case ResponseCode.CreatePlayer:					NetworkController.Instance.OperationCreatePlayer(packet as CreatePlayerRes); break;
			case ResponseCode.SelectPlayer:					NetworkController.Instance.OperationSelectPlayer(packet as SelectPlayerRes); break;
			// ロビーパケット解析
			case ResponseCode.EnterLobby:					LobbyPacket.OperationResponseEnterLobby(packet as EnterLobbyRes);	break;
			case ResponseCode.ExitLobby:					LobbyPacket.OperationResponseExitLobby(packet as ExitLobbyRes);	break;
			case ResponseCode.LobbyList:					LobbyPacket.OperationResponseLobbyList(packet as LobbyListRes);	break;
			case ResponseCode.LobbyNum:						LobbyPacket.OperationResponseLobbyNum(packet as LobbyNumRes);	break;
			case ResponseCode.Ranking:						LobbyPacket.OperationResponseRanking(packet as RankingRes);	break;
			//case ResponseCode.RankingList:					LobbyPacket.OperationResponseRankingList(packet as RankingListRes);	break;
			case ResponseCode.Matching:						LobbyPacket.OperationResponseMatching(packet as MatchingRes);	break;
			case ResponseCode.PlayerCharacterStorage:		LobbyPacket.OperationResponsePlayerCharacterBox(packet as PlayerCharacterBoxRes); break;
			case ResponseCode.PlayerCharacterAll:			LobbyPacket.OperationResponsePlayerCharacterAll(packet as PlayerCharacterAllRes); break;
			case ResponseCode.PlayerCharacter:				LobbyPacket.OperationResponsePlayerCharacter(packet as PlayerCharacterRes); break;
			case ResponseCode.PlayerStatus:					LobbyPacket.OperationResponsePlayerStatus(packet as PlayerStatusRes); break;
            case ResponseCode.BattleHistory:                LobbyPacket.OperationResponseBattleHistory(packet as BattleHistoryRes); break;
            case ResponseCode.BattleHistoryDetail:          LobbyPacket.OperationResponseBattleHistoryDetail(packet as BattleHistoryDetailRes); break;
            case ResponseCode.PlayerMiscInfo:               LobbyPacket.OperationResponsePlayerMiscInfo(packet as PlayerMiscInfoRes); break;
               
			case ResponseCode.SellMultiPlayerCharacterCalc: LobbyPacket.OperationResponseSellMultiPlayerCharacterCalc(packet as SellMultiPlayerCharacterCalcRes); break;
			case ResponseCode.SellMultiPlayerCharacter:		LobbyPacket.OperationResponseSellMultiPlayerCharacter(packet as SellMultiPlayerCharacterRes); break;
			case ResponseCode.Powerup:						LobbyPacket.OperationResponsePowerup(packet as PowerupRes); break;
			case ResponseCode.PowerupCalc:					LobbyPacket.OperationResponsePowerupCalc(packet as PowerupCalcRes); break;
			case ResponseCode.Evolution:					LobbyPacket.OperationResponseEvolution(packet as EvolutionRes); break;
			case ResponseCode.EvolutionCalc:				LobbyPacket.OperationResponseEvolutionCalc(packet as EvolutionCalcRes); break;
			case ResponseCode.SynchroFusion:				LobbyPacket.OperationResponseSynchroFusion(packet as SynchroFusionRes); break;
			case ResponseCode.SynchroFusionCalc:			LobbyPacket.OperationResponseSynchroFusionCalc(packet as SynchroFusionCalcRes); break;
			case ResponseCode.SetLockPlayerCharacter:		LobbyPacket.OperationResponseSetLockPlayerCharacter(packet as SetLockPlayerCharacterRes); break;
			case ResponseCode.GetPowerupSlot:				LobbyPacket.OperationResponseGetPowerupSlot(packet as GetPowerupSlotRes); break;
			case ResponseCode.SetPowerupSlot:				LobbyPacket.OperationResponseSetPowerupSlot(packet as SetPowerupSlotRes); break;
			case ResponseCode.SetPowerupSlotCalc:			LobbyPacket.OperationResponseSetPowerupSlotCalc(packet as SetPowerupSlotCalcRes); break;
			case ResponseCode.AddPowerupSlot:				LobbyPacket.OperationResponseAddPowerupSlot(packet as AddPowerupSlotRes); break;
			case ResponseCode.SetPlayerCharacterNewFlagAll:	LobbyPacket.OperationResponseSetPlayerCharacterNewFlagAll(packet as SetPlayerCharacterNewFlagAllRes); break;
			case ResponseCode.PlayerItemBox:				LobbyPacket.OperationResponsePlayerItemBox(packet as PlayerItemBoxRes); break;
			case ResponseCode.PlayerItemAll:				LobbyPacket.OperationResponsePlayerItemAll(packet as PlayerItemAllRes); break;
			case ResponseCode.SellMultiPlayerItem:			LobbyPacket.OperationResponseSellMultiPlayerItem(packet as SellMultiPlayerItemRes); break;
			case ResponseCode.SetLockPlayerItem:			LobbyPacket.OperationResponseSetLockPlayerItem(packet as SetLockPlayerItemRes); break;
			case ResponseCode.SellPlayerItem:				LobbyPacket.OperationResponseSellPlayerItem(packet as SellPlayerItemRes); break;
			case ResponseCode.SetPlayerItemNewFlagAll:		LobbyPacket.OperationResponseSetPlayerItemNewFlagAll(packet as SetPlayerItemNewFlagAllRes); break;
			case ResponseCode.MailBox:						LobbyPacket.OperationResponseMailBox(packet as MailBoxRes); break;
			case ResponseCode.AdminMail:					LobbyPacket.OperationResponseAdminMail(packet as AdminMailRes); break;
			case ResponseCode.SetAdminMailReadFlag:			LobbyPacket.OperationResponseSetAdminMailReadFlag(packet as SetAdminMailReadFlagRes); break;
			case ResponseCode.SetAdminMailReadFlagAll:		LobbyPacket.OperationResponseSetAdminMailReadFlagAll(packet as SetAdminMailReadFlagAllRes); break;
			case ResponseCode.SetLockAdminMail:				LobbyPacket.OperationResponseSetLockAdminMail(packet as SetLockAdminMailRes); break;
			case ResponseCode.DeleteAdminMail:				LobbyPacket.OperationResponseDeleteAdminMail(packet as DeleteAdminMailRes); break;
			case ResponseCode.DeleteAdminMailAll:			LobbyPacket.OperationResponseDeleteAdminMailAll(packet as DeleteAdminMailAllRes); break;
			case ResponseCode.PresentMail:					LobbyPacket.OperationResponsePresentMail(packet as PresentMailRes); break;
			case ResponseCode.SetPresentMailReadFlag:		LobbyPacket.OperationResponseSetPresentMailReadFlag(packet as SetPresentMailReadFlagRes); break;
			case ResponseCode.SetLockPresentMail:			LobbyPacket.OperationResponseSetLockPresentMail(packet as SetLockPresentMailRes); break;
			case ResponseCode.DeletePresentMail:			LobbyPacket.OperationResponseDeletePresentMail(packet as DeletePresentMailRes); break;
			case ResponseCode.DeletePresentMailAll:			LobbyPacket.OperationResponseDeletePresentMailAll(packet as DeletePresentMailAllRes); break;
			case ResponseCode.ReceivePresentMailItem:		LobbyPacket.OperationResponseReceivePresentMailItem(packet as ReceivePresentMailItemRes); break;
			case ResponseCode.ReceivePresentMailItemAll:	LobbyPacket.OperationResponseReceivePresentMailItemAll(packet as ReceivePresentMailItemAllRes); break;
			case ResponseCode.Achievement:					LobbyPacket.OperationResponseAchievement( packet as AchievementRes); break;
			case ResponseCode.SetAchievementNewFlagAll:		LobbyPacket.OperationResponseCompleteAchievement( packet as CompleteAchievementEvent); break;
			case ResponseCode.ReceiveAchievementReward:		LobbyPacket.OperationResponseReceiveAchievementReward( packet as ReceiveAchievementRewardRes); break;
			case ResponseCode.ReceiveAchievementRewardAll:	LobbyPacket.OperationResponseReceiveAchievementRewardAll(packet as ReceiveAchievementRewardAllRes); break;
			case ResponseCode.ReceiveWebStore:				LobbyPacket.OperationResponseReceiveWebStore(packet as ReceiveWebStoreRes); break;
            case ResponseCode.LoginBonus:                   LobbyPacket.OperationResponseLoginBonuseList(packet as LoginBonusRes); break;
            case ResponseCode.LoginBonusReward:             LobbyPacket.OperationResponseLoginBonuseReward(packet as LoginBonusRewardRes); break;          
			// 共通パケット解析
			case ResponseCode.Entrant:						CommonPacket.OperationResponseEntrant(packet as EntrantRes);	break;
			case ResponseCode.EntrantAll:					CommonPacket.OperationResponseEntrantAll(packet as EntrantAllRes);	break;
			case ResponseCode.Echo:							CommonPacket.OperationResponseEcho(packet as EchoRes);	break;
			case ResponseCode.CharacterDeckNum:				CommonPacket.OperationResponseCharacterDeckNum(packet as CharacterDeckNumRes); break;
			case ResponseCode.CharacterDeckList:			CommonPacket.OperationResponseCharacterDeckList(packet as CharacterDeckListRes); break;
			case ResponseCode.CharacterDeck:				CommonPacket.OperationResponseCharacterDeck(packet as CharacterDeckRes); break;
			case ResponseCode.SetCharacterDeck:				CommonPacket.OperationResponseSetCharacterDeck(packet as SetCharacterDeckRes); break;
			case ResponseCode.SelectCharacterDeck:			CommonPacket.OperationResponseSelectCharacterDeck(packet as SelectCharacterDeckRes); break;
			case ResponseCode.SetSymbolPlayerCharacter:		CommonPacket.OperationSetSymbolPlayerCharacterRes(packet as SetSymbolPlayerCharacterRes); break;
#if OLD_TEAM_LOGIC
            case ResponseCode.TeamCreate:					CommonPacket.OperationResponseTeamCreate(packet as TeamCreateRes); break;
			case ResponseCode.TeamRemoveMember:				CommonPacket.OperationResponseTeamRemoveMember(packet as TeamRemoveMemberRes); break;
			case ResponseCode.TeamSearch:					CommonPacket.OperationResponseTeamSearch(packet as TeamSearchRes); break;
			case ResponseCode.TeamJoin:						CommonPacket.OperationResponseTeamJoin(packet as TeamJoinRes); break;
			case ResponseCode.TeamMember:					CommonPacket.OperationResponseTeamMember(packet as TeamMemberRes); break;
#endif
			case ResponseCode.Auth:							CommonPacket.OperationResponseAuth(packet as AuthRes); break;
            case ResponseCode.SetCurrentAvatar:             CommonPacket.OperationResponseSetCurrentAvatar(packet as SetCurrentAvatarRes); break;
            case ResponseCode.GetCharacterAvatarAll:        CommonPacket.OperationResponseGetCharacterAvatarAll(packet as GetCharacterAvatarAllRes); break;
            case ResponseCode.GetCharacterReplayVoiceAll:   CommonPacket.OperationResponseGetCharacterReplayVoiceAll(packet as GetCharacterReplayVoiceAllRes); break;
            case ResponseCode.GetCharacterStoryAll:         CommonPacket.OperationResponseGetCharacterStoryAll(packet as GetCharacterStoryAllRes); break;
            case ResponseCode.GetCharacterWallpaperAll:     CommonPacket.OperationResponseGetCharacterWallpaperAll(packet as GetCharacterWallpaperAllRes); break;
            case ResponseCode.ObtainedAllCharacterAvatar:   CommonPacket.OperationResponseObtainedCharacterAvatarAll(packet as ObtainedCharacterAvatarAllRes); break;
			// バトルパケット解析
			case ResponseCode.EnterField:					BattlePacket.OperationResponseEnterField(packet as EnterFieldRes);	break;
			case ResponseCode.ReEnterField:					BattlePacket.OperationResponseReEnterField(packet as ReEnterFieldRes);	break;
			case ResponseCode.ExitField:					BattlePacket.OperationResponseExitField(packet as ExitFieldRes);	break;
			case ResponseCode.Hit:							BattlePacket.OperationResponseHit(packet as HitRes);	break;
			case ResponseCode.Judge:						BattlePacket.OperationResponseJudge(packet as JudgeRes);	break;
			case ResponseCode.RemainingTime:				BattlePacket.OperationResponseRemainingTime(packet as RemainingTimeRes);	break;
			case ResponseCode.Warp:							BattlePacket.OperationResponseWarp(packet as WarpRes);	break;
			case ResponseCode.TeamSkillPoint:				BattlePacket.OperationResponseTeamSkillPoint(packet as TeamSkillPointRes);	break;
			case ResponseCode.GameResult:					BattlePacket.OperationResponseGameResult(packet as GameResultRes);	break;
			case ResponseCode.ItemGet:						BattlePacket.OperationResponseItemGet(packet as ItemGetRes);	break;
			case ResponseCode.KillSelf:						BattlePacket.OperationResponseKillSelf(packet as KillSelfRes);	break;
			case ResponseCode.Buff:							BattlePacket.OperationResponseBuff(packet as BuffRes);		break;
			case ResponseCode.TargetMarkerAll:				BattlePacket.OperationResponseTargetMarkerAll(packet as TargetMarkerAllRes); break;
			case ResponseCode.TargetMarker:					BattlePacket.OperationResponseTargetMarker(packet as TargetMarkerRes); break;
			case ResponseCode.Transport:					BattlePacket.OperationResponseTransport(packet as TransportRes); break;
			case ResponseCode.SelectCharacter:				BattlePacket.OperationResponseSelectCharacter(packet as SelectNextDeckCharacterRes); break;
			case ResponseCode.SideGauge:					BattlePacket.OperationResponseSideGauge(packet as SideGaugeRes);	break;
            case ResponseCode.TeamInvite:                   CommonPacket.OnResponseTeamInvite(packet as TeamInviteRes); break;
            case ResponseCode.TeamMember:                   CommonPacket.OnResponseGetTeamInfo(packet as TeamMemberRes);    break;
            case ResponseCode.TeamOperation:                CommonPacket.OnResponseTeamOperation(packet as TeamOperationRes);    break;
            case ResponseCode.GetGuideStep:                 CommonPacket.OperationResponseGetGuide(packet as GetGuideRes); break;
            case ResponseCode.SetGuideStep:                 CommonPacket.OperationResponseSetGuide(packet as SetGuideRes); break;
            case ResponseCode.ProxyReq:                     BattlePacket.OnProxyRes(packet as ProxyRes); break;
            case ResponseCode.ServerMapping:                CommonPacket.OperationResponseServerMapping(packet as ServerMappingRes); break;
            case ResponseCode.Recruitment:                  LobbyPacket.OperationResponseRecruitment(packet as RecruitmentRes); break;
            case ResponseCode.GiftPackage:                  LobbyPacket.OperationResponseGiftPackage(packet as GiftPackageRes); break;
#if XW_DEBUG || ENABLE_GM_COMMAND
			case ResponseCode.GmCommand:					CommonPacket.OperationResponseGmCommand(packet as GmCommandRes);	break;
#endif
			default:
                Debug.Log("===> " + code);
				BugReportController.SaveLogFile(code + " : not found ResponseCode for run");
				break;
			}
			#endregion
		}
		
		#endregion
		
		#region Event
		
		/// <summary>
		/// パケットコードに対応するパケットクラスのインスタンスを生成.
		/// </summary>
		/// <returns>
		/// 対応するパケットクラスが存在する　= true, 存在しない = false
		/// </returns>
		/// <param name='code'>
		/// パケットコード.
		/// </param>
		/// <param name='packet'>
		/// out 生成したパケットクラスのインスタンス.
		/// </param>
		static public bool TryCreatePacket(EventCode code, out PacketBase packet)
		{
			#region switch(EventCode)
			switch(code)
			{
				// 共通
				case EventCode.Entrant:				packet = new EntrantEvent();	return true;
				case EventCode.Move:				packet = new MoveEvent();	return true;
				case EventCode.Chat:				packet = new ChatEvent();	return true;
				case EventCode.PlayerDisplayState:	packet = new PlayerDisplayStateEvent(); return true;
				case EventCode.TeamInfo:			packet = new TeamInfoEvent(); return true;
                case EventCode.CompleteAchievement: packet = new CompleteAchievementEvent(); return true;
				// ロビー
				case EventCode.ExitLobby:			packet = new ExitLobbyEvent();	return true;
				case EventCode.Matching:			packet = new MatchingEvent();	return true;
				// バトル
				case EventCode.ExitField:			packet = new ExitFieldEvent();	return true;
				case EventCode.Attack:				packet = new AttackEvent();	return true;
				case EventCode.Hit:					packet = new HitEvent();	return true;
				case EventCode.Respawn:				packet = new RespawnEvent();	return true;
				case EventCode.SkillMotion:			packet = new SkillMotionEvent();	return true;
				case EventCode.Judge:				packet = new JudgeEvent();	return true;
				case EventCode.RemainingTime:		packet = new RemainingTimeEvent();	return true;
				case EventCode.Warp:				packet = new WarpEvent();	return true;
				case EventCode.LevelUp:				packet = new LevelUpEvent();	return true;
				case EventCode.TeamSkill:			packet = new TeamSkillEvent();	return true;
				case EventCode.Exp:				    packet = new ExpEvent();	return true;
				case EventCode.TeamSkillPoint:		packet = new TeamSkillPointEvent();	return true;
				case EventCode.ItemGet:				packet = new ItemGetEvent();	return true;
				case EventCode.Motion:				packet = new MotionEvent();	return true;
				case EventCode.SkillCharge:			packet = new SkillChargeEvent();	return true;
				case EventCode.Score:				packet = new ScoreEvent();	return true;
				case EventCode.Buff:				packet = new BuffEvent();	return true;
				case EventCode.SkillCastMarker:		packet = new SkillCastMarkerEvent();	return true;
				case EventCode.TargetMarker:		packet = new TargetMarkerEvent(); return true;
				case EventCode.Transport:			packet = new TransportEvent(); return true;
				case EventCode.SideGauge:			packet = new SideGaugeEvent(); return true;
                case EventCode.SkillGauge:          packet = new SkillGaugeEvent(); return true;
                case EventCode.ResidentAreaSideGauge: packet = new ResidentAreaSideGaugeEvent(); return true;
                case EventCode.EntrantActive:       packet = new EntrantActiveEvent(); return true;
                case EventCode.SideGaugePercent:    packet = new SideGaugePercentEvent(); return true;
                case EventCode.BonusTime:           packet = new BonusTimeEvent(); return true;
                case EventCode.FriendReq:           packet = new FriendEvent(); return true;
                case EventCode.TeamInvite:          packet = new TeamInviteEvent(); return true;
                case EventCode.TeamMember:          packet = new TeamMemberEvent(); return true;
                case EventCode.StatusChanged:       packet = new StatusChangedEvent() ; return true;
                case EventCode.Recruitment:         packet = new RecruitmentEvent(); return true;
				default:
                    Debug.Log("===> " + code);
					packet = null;
					return false;
			}
			#endregion
		}
		
		/// <summary>
		/// Eventパケットの処理を実行する.
		/// </summary>
		/// パケットコード.
		/// </param>
		/// パラメータセット済みのパケット.
		/// </param>
		static public void RunEventPacket(EventCode code, PacketBase packet)
		{
			#region switch(EventCode)
			switch(code)
			{
			// 共通パケット解析
			case EventCode.Entrant:				CommonPacket.EventEntrant(packet as EntrantEvent);		break;
			case EventCode.Move:				CommonPacket.EventMove(packet as MoveEvent);	break;
			case EventCode.Chat:				CommonPacket.EventChat(packet as ChatEvent);	break;
			case EventCode.PlayerDisplayState:	CommonPacket.EventPlayerDisplayState(packet as PlayerDisplayStateEvent); break;
#if OLD_TEAM_LOGIC
            case EventCode.TeamInfo:			CommonPacket.EventTeamInfo(packet as TeamInfoEvent); break;
#endif
            case EventCode.CompleteAchievement: break;
			// ロビーパケット解析
			case EventCode.ExitLobby:			LobbyPacket.EventExitLobby(packet as ExitLobbyEvent);	break;
			case EventCode.Matching:			LobbyPacket.EventMatching(packet as MatchingEvent);	break;
			// バトルパケット解析
			case EventCode.ExitField:			BattlePacket.EventExitField(packet as ExitFieldEvent);	break;
			case EventCode.Attack:				BattlePacket.EventAttack(packet as AttackEvent);	break;
			case EventCode.Hit:					BattlePacket.EventHit(packet as HitEvent);	break;
			//case EventCode.Status:			BattlePacket.EventStatus(packet as StatusEvent);	break;
			//case EventCode.Effect:			BattlePacket.EventEffect(packet as EffectEvent);	break;
			case EventCode.Respawn:				BattlePacket.EventRespawn(packet as RespawnEvent);	break;
			//case EventCode.Reset:				BattlePacket.EventReset(packet as ResetEvent);		break;
			case EventCode.SkillMotion:			BattlePacket.EventSkillMotion(packet as SkillMotionEvent);	break;
			case EventCode.Judge:				BattlePacket.EventJudge(packet as JudgeEvent);		break;
			case EventCode.RemainingTime:		BattlePacket.EventRemainingTime(packet as RemainingTimeEvent);	break;
			case EventCode.Warp:				BattlePacket.EventWarp(packet as WarpEvent);	break;
			case EventCode.LevelUp:				BattlePacket.EventLevelUp(packet as LevelUpEvent);	break;
			case EventCode.TeamSkill:			BattlePacket.EventTeamSkill(packet as TeamSkillEvent);	break;
			case EventCode.Exp:	    			BattlePacket.EventExp(packet as ExpEvent);	break;
			case EventCode.TeamSkillPoint:		BattlePacket.EventTeamSkillPoint(packet as TeamSkillPointEvent);	break;
			case EventCode.ItemGet:				BattlePacket.EventItemGet(packet as ItemGetEvent);	break;
			case EventCode.Motion:				BattlePacket.EventMotion(packet as MotionEvent);	break;
			case EventCode.SkillCharge:			BattlePacket.EventSkillCharge(packet as SkillChargeEvent);	break;
			case EventCode.Score:				BattlePacket.EventScore(packet as ScoreEvent);		break;
			case EventCode.Buff:				BattlePacket.EventBuff(packet as BuffEvent);		break;
			case EventCode.SkillCastMarker:		BattlePacket.EventSkillCastMarker(packet as SkillCastMarkerEvent);	break;
			case EventCode.TargetMarker:		BattlePacket.EventTargetMarker(packet as TargetMarkerEvent); break;
			case EventCode.Transport:			BattlePacket.EventTransport(packet as TransportEvent); break;
			case EventCode.SideGauge:			BattlePacket.EventSideGauge(packet as SideGaugeEvent);	break;
            case EventCode.SkillGauge:          BattlePacket.EventSkillGauge(packet as SkillGaugeEvent); break;
            case EventCode.ResidentAreaSideGauge: BattlePacket.EventResidentAreaSideGauge(packet as ResidentAreaSideGaugeEvent); break;
            case EventCode.EntrantActive:       BattlePacket.EntrantActive(packet as EntrantActiveEvent); break;

            case EventCode.SideGaugePercent: BattlePacket.EventSideGaugePercent(packet as SideGaugePercentEvent); break;
            case EventCode.BonusTime:           BattlePacket.EventBonusTime(packet as BonusTimeEvent); break;
            case EventCode.FriendReq:           LobbyPacket.EventFriend(packet as FriendEvent); break;
            case EventCode.TeamInvite:          CommonPacket.OnEventTeamInvite(packet as TeamInviteEvent); break;
            case EventCode.TeamMember:          CommonPacket.OnEventInviteAccOrRej(packet as TeamMemberEvent); break;
            case EventCode.TeamInfo:            CommonPacket.OnEventTeamInfo(packet as TeamInfoEvent); break;
            case EventCode.StatusChanged:       CommonPacket.OnEventStatusChanged(packet as StatusChangedEvent); break;
            case EventCode.Recruitment:         LobbyPacket.EventRecruitment(packet as RecruitmentEvent); break;
			default:
                Debug.Log("===> " + code);
				BugReportController.SaveLogFile(code + " : not found EventCode for run");
				break;
			}
			#endregion
		}
		
		#endregion
	}
}
