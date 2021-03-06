/// <summary>
/// バトルパケット解析
/// 
/// 2013/08/16
/// </summary>
using UnityEngine;
using System.Collections.Generic;

using Scm.Common.Master;
using Scm.Common.Packet;
using Scm.Common.GameParameter;
using Scm.Client;

public class BattlePacket
{
    #region EnterField パケット
    /// <summary>
    /// EnterField 送信要求
    /// プレイヤーログイン送信
    /// </summary>
    public static void SendEnterField()
    {
        Debug.Log("Send Enter_1...");
        //if (GUIMatchingState.IsMatchingSuccess)
        //{
        //    SendEnterFieldMatching();
        //}
        //else
        {
            // フィールドにログインする
            // Matching の時はIDを指定してもサーバー側で使わないだけなので
            // とりあえず同じ処理にする
            SendEnterField(ScmParam.Battle.BattleFieldType, ScmParam.Battle.ScoreType);
        }
    }
    /// <summary>
    /// EnterField 送信要求
    /// プレイヤーログイン送信
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="avatarType"></param>
    /// <param name="fieldId"></param>
    static void SendEnterField(BattleFieldType battleFieldType, ScoreType scoreType)
    {
        Debug.Log("Send Enter_2...");
        EnterFieldReq packet = new EnterFieldReq();
        packet.BattleFieldId = (int)battleFieldType;
        packet.ScoreType = (int)scoreType;
        GameListener.SendConnected(packet);
    }
    /// <summary>
    /// EnterField(Audiences) 送信要求
    /// </summary>
    public static void SendEnterFieldAudiences(BattleFieldType battleFieldType, ScoreType scoreType)
    {
        Debug.Log("Send Enter_3...");
        EnterFieldReq packet = new EnterFieldReq();
        packet.BattleFieldId = (int)battleFieldType;
        packet.IsAudiences = true;
        packet.ScoreType = (int)scoreType;
        GameListener.SendConnected(packet);
    }
    /// <summary>
    /// EnterField 送信要求
    /// プレイヤーログイン送信
    /// </summary>
    static void SendEnterFieldMatching()
    {
        Debug.Log("Send Enter_4...");
        EnterFieldReq packet = new EnterFieldReq();
        GameListener.SendConnected(packet);
    }
    /// <summary>
    /// EnterField 受信応答
    /// プレイヤーID設定
    /// プレイヤー、他プレイヤー情報取得パケット送信
    /// </summary>
    /// <param name="packet"></param>
    public void OperationResponseEnterField(EnterFieldRes packet)
    {
        Debug.Log("Operation Enter...");
        // パケッドが違う
        if (packet == null)
            return;
        
        
        if (!packet.Result)
        {
            // 失敗
            GUIChat.AddSystemMessage(true, MasterData.GetText(TextType.TX127_EnterFieldRes_Fail));
            return;
        }

        // ログインフラグをON
        GameListener.LoginFlg = true;

        PlayerInfo playerInfo = EntrantInfo.CreatePlayer(packet.GetEntrantRes());
        int fieldId = playerInfo.FieldId;
        if (packet.IsAudiences == true)
        {
            playerInfo = null;
        }

        // TODO: 経験値初期化 いずれ1ユーザに複数のプレイヤーキャラを持つ仕組みに変える予定
        CharaLevelMasterData charaLv = null;
        if (MasterData.TryGetCharaLv(playerInfo.Id, playerInfo.Level, out charaLv))
        {
            PlayerManager.Instance.SetupExp(0, charaLv.NextExp);
        }
        else
        {
            BugReportController.SaveLogFileWithOutStackTrace("Fail EnterFieldRes : " + playerInfo.Id + "," + playerInfo.Level);
            PlayerManager.Instance.SetupExp(0, 0);
        }

        // 値を保存.
        NetworkController.ServerValue.SetEnterFieldResponse(packet, playerInfo);

        BattleFieldMasterData bfData;
        if (MasterData.TryGetBattleField(packet.BattleFieldId, out bfData))
        {
            // バトルシーン読み込み.
            BattleMain.LoadScene(fieldId, bfData.MapID, playerInfo, (BattleFieldType)packet.BattleFieldId);

            //LWZ:LoadingField
            //BattleMain.LoadScene(fieldId, bfData.MapID, playerInfo, (BattleFieldType)packet.BattleFieldId, packet);
        }
        else
        {
            //BugReportController.SaveLogFile(packet.Parameters);
        }
    }
    #endregion

    #region ReEnterField パケット
    static IPacketResponse<ReEnterFieldRes> ReEnterFieldResponse { get; set; }
    public static void SendReEnterField(IPacketResponse<ReEnterFieldRes> response)
    {
        ReEnterFieldResponse = response;
        SendReEnterField();
    }
    /// <summary>
    /// ReEnterField 送信要求
    /// </summary>
    public static void SendReEnterField()
    {
        var packet = new ReEnterFieldReq();
        GameListener.SendConnected(packet);
    }
    /// <summary>
    /// ReEnterField 受信応答
    /// </summary>
    /// <param name="packet"></param>
    public void OperationResponseReEnterField(ReEnterFieldRes packet)
    {
        // パケットエラー.
        if (packet == null)
            return;

        if (ReEnterFieldResponse != null)
        {
            ReEnterFieldResponse.Response(packet);
        }

        // 失敗
        if (packet.ReEnterFieldResult != ReEnterFieldResult.Success)
            return;

        // ログインフラグをON
        GameListener.LoginFlg = true;

        PlayerInfo playerInfo = EntrantInfo.CreatePlayer(packet.GetEntrantRes());

        // 値を保存.
        NetworkController.ServerValue.SetReEnterFieldResponse(packet, playerInfo);

        // TODO: 経験値セット いずれ1ユーザに複数のプレイヤーキャラを持つ仕組みに変える予定
        CharaLevelMasterData charaLv = null;
        if (MasterData.TryGetCharaLv(playerInfo.Id, playerInfo.Level, out charaLv))
        {
            PlayerManager.Instance.SetupExp(packet.Exp, charaLv.NextExp);
        }
        else
        {
            BugReportController.SaveLogFileWithOutStackTrace("Fail ReEnterFieldRes : " + playerInfo.Id + "," + playerInfo.Level);
            PlayerManager.Instance.SetupExp(packet.Exp, 0);
        }

        BattleFieldMasterData bfData;
        if (MasterData.TryGetBattleField(packet.BattleFieldId, out bfData))
        {
            // バトルシーン読み込み.
            BattleMain.ReEntryLoadScene(playerInfo.FieldId, bfData.MapID, playerInfo, (BattleFieldType)packet.BattleFieldId);
        }
    }
    #endregion

    #region ExitField パケット
    /// <summary>
    /// ExitField 送信
    /// プレイヤーログアウト送信
    /// </summary>
    public static void SendExitField()
    {
        ExitFieldReq packet = new ExitFieldReq();
        GameListener.Send(packet);
    }
    /// <summary>
    /// ExitField 受信応答
    /// プレイヤーログアウト
    /// </summary>
    /// <param name="packet"></param>
    public void OperationResponseExitField(ExitFieldRes packet)
    {
        // パケッドが違う
        if (packet == null)
            return;
        // 次のシーンへ
        BattleMain.IsExitField = true;
    }
    /// <summary>
    /// ExitField 受信通知
    /// 他プレイヤーログアウト
    /// </summary>
    /// <param name="packet"></param>
    public void EventExitField(ExitFieldEvent packet)
    {
        // パケッドが違う
        if (packet == null)
            return;

        // 削除
        Entrant.RemoveEntrant(packet.InFieldId);
    }
    #endregion

    #region Attack パケット
    /// <summary>
    /// Attack 送信
    /// 攻撃情報送信
    /// </summary>
    /// <param name="skillID"></param>
    /// <param name="target"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public static void SendAttack(ObjectBase target, Vector3 position, Quaternion rotation, int skillID, IBulletSetMasterData ibulletSet)
    {
        SkillBulletSetMasterData bulletSet = ibulletSet as SkillBulletSetMasterData;
        if (bulletSet == null)
            return;
        AttackReq packet = new AttackReq();
        packet.SkillId = skillID;
        packet.BulletSetId = bulletSet.ID;
        packet.InFieldTargetId = ObjectBase.CastInFieldID(target ? target.InFieldId : GameConstant.InvalidID);
        packet.StartPos = new float[] { position.x, position.y, position.z };
        packet.Direction = new float[] { rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z };
        GameListener.Send(packet);
    }
    /// <summary>
    /// Attack 送信
    /// 攻撃情報送信
    /// </summary>
    /// <param name="skillID"></param>
    /// <param name="target"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="caster"></param>
    public static void SendAttack(ObjectBase target, Vector3 position, Quaternion rotation, int skillID, IBulletSetMasterData ibulletSet, ObjectBase caster)
    {
        SkillBulletSetMasterData bulletSet = ibulletSet as SkillBulletSetMasterData;
        if (bulletSet == null)
            return;
        AttackReq packet = new AttackReq();
        packet.SkillId = skillID;
        packet.BulletSetId = bulletSet.ID;
        packet.InFieldTargetId = ObjectBase.CastInFieldID(target ? target.InFieldId : GameConstant.InvalidID);
        packet.StartPos = new float[] { position.x, position.y, position.z };
        packet.Direction = new float[] { rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z };
        packet.AttackerStartPos = new float[] { caster.transform.position.x, caster.transform.position.y, caster.transform.position.z };
        packet.AttackerDirection = caster.transform.rotation.eulerAngles.y;
        GameListener.Send(packet);
    }
    /// <summary>
    /// Attack 通知
    /// 他プレイヤー攻撃
    /// </summary>
    /// <param name="packet"></param>
    public void EventAttack(AttackEvent packet)
    {
        // パケッドが違う
        if (packet == null)
            return;
        // 存在していない
        ObjectBase objectBase = GameController.SearchInFieldID(packet.InFieldId);
        if (objectBase == null)
            return;

        ObjectBase target = GameController.SearchInFieldID(packet.InFieldTargetId);
        Vector3 position = new Vector3(packet.StartPos[0], packet.StartPos[1], packet.StartPos[2]);
        Quaternion rotation = Quaternion.Euler(packet.Direction[0], packet.Direction[1], packet.Direction[2]);
        Quaternion casterRot = Quaternion.Euler(0f, packet.AttackerDirection, 0f);
        if (packet.AttackerStartPos != null)
        {
            Vector3 casterPos = new Vector3(packet.AttackerStartPos[0], packet.AttackerStartPos[1], packet.AttackerStartPos[2]);
            objectBase.Attack(packet.BulletSetId, target, position, rotation, casterPos, casterRot);
        }
        else
        {
            objectBase.Attack(packet.BulletSetId, target, position, rotation, null, casterRot);
        }
    }
    #endregion

    #region Hit パケット
    /// <summary>
    /// Hit 送信
    /// ヒット判定情報送信
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="skillID"></param>
    public static void SendHit(EntrantInfo caster, int skillID, int bulletID, ObjectBase target, Vector3 position, float bulletDirection)
    {
        HitReq packet = CreateHitReq(caster, skillID, bulletID, target, position, bulletDirection);
        if (packet == null) {
            return;
        }
        GameListener.Send(packet);
    }

    private static HitReq CreateHitReq(EntrantInfo caster, int skillID, int bulletID, ObjectBase target, Vector3 position, float bulletDirection) {
        HitReq packet = new HitReq();
        if (caster != null) {
            packet.InFieldId = ObjectBase.CastInFieldID(caster.InFieldId);
        }
        packet.SkillId = skillID;
        packet.BulletId = bulletID;
        // ターゲットがいる場合
        if (target) {
            Player player = target as Player;
            if (player) {
                // プレイヤーのみガードフラグを設定する
                packet.PostureType = player.Posture;
                packet.BulletDirection = bulletDirection;
            } else {
                if (target.InFieldId == GameConstant.InvalidID) {
                    // 無効なオブジェクトにヒット(これをこのまま送ると自分に当たってしまう).
                    return null;
                }
                // 非プレイヤー以外でターゲットを指定する
                packet.InFieldTargetId = ObjectBase.CastInFieldID(target.InFieldId);
            }
        }

        packet.Position = new float[] { position.x, position.y, position.z };
        return packet;
    }

    public static void ProxySendHit(int inFieldId, EntrantInfo caster, int skillID, int bulletID, ObjectBase target, Vector3 position, float bulletDirection) {
        HitReq packet = CreateHitReq(caster, skillID, bulletID, target, position, bulletDirection);
        if (packet == null) {
            return;
        }
        ProxyReq req = new ProxyReq() {
            ActualCode = packet.Code,
            InFieldId = inFieldId,
            ActualPacket = packet.GetPacket()
        };
        if (inFieldId == req.InFieldId) {
            // The bot is the target
            packet.InFieldTargetId = 0;
        }
        GameListener.Send(req);
    }

    /// <summary>
    /// Hit 受信応答
    /// プレイヤーが当たった
    /// </summary>
    /// <param name="packet"></param>
    public void OperationResponseHit(HitRes packet)
    {
        // パケッドが違う
        if (packet == null)
            return;
        // 存在していない
        Player player = GameController.GetPlayer();
        if (player == null)
            return;

        // ダメージ処理
        player.Hit(new HitInfo(packet));
    }
    /// <summary>
    /// Hit 受信通知
    /// 他プレイヤーが当たった
    /// </summary>
    /// <param name="packet"></param>
    public void EventHit(HitEvent packet)
    {
        // パケッドが違う
        if (packet == null)
            return;

        DamageEvent damageEvent = packet.GetDamageEvent();

        EntrantInfo entrant;
        if (Entrant.TryGetEntrant(damageEvent.InFieldId, out entrant))
        {
            entrant.Hit(new HitInfo(packet));
        }

        /* 経験値、お金仮表示
        if (NetworkController.ServerValue.InFieldId == packet.InFieldAttackerId)
        {
            if (packet.Exp > 0)
            {
                string str = MasterData.GetText(TextType.TX010_GetExp, new string[] { packet.Exp.ToString() });
                //GUIChat.AddMessage(str);
            }

            if (packet.Money > 0)
            {
                string str = MasterData.GetText(TextType.TX011_GetMoney, new string[] { packet.Money.ToString() });
                //GUIChat.AddMessage(str);
            }
        }
        */
    }
    #endregion
    //  UNDONE: Common.DLL: 使用しなくなったパケットの関連コードをコメントアウト
    /*
    #region Status パケット
    public void EventStatus(StatusEvent packet)
    {
        // パケッドが違う
        if (packet == null)
            return;
        // オブジェクトが存在しない
        ObjectBase objectBase = GameController.SearchInFieldID(packet.InFieldId);
        if (objectBase == null)
            return;

        objectBase.Status(new HitInfo(packet));
    }
    #endregion

    #region Effect パケット
    public void EventEffect(EffectEvent packet)
    {
        // パケッドが違う
        if (packet == null)
            return;
        // オブジェクトが存在しない
        ObjectBase objectBase = GameController.SearchInFieldID(packet.InFieldId);
        if (objectBase == null)
            return;

        objectBase.Effect(new HitInfo(packet));
    }
    #endregion
    */
    #region Respawn パケット
    /// <summary>
    /// Respawn 受信通知
    /// 全キャラクター共通リスポーン処理
    /// </summary>
    /// <param name="packet"></param>
    public void EventRespawn(RespawnEvent packet)
    {
        // パケットが違う
        if (packet == null)
            return;

        if (packet.OldInFieldId == 0)
        {
            // 0の場合は自分のことと見なす.
            NetworkController.RemovePlayer();
            PlayerInfo playerInfo = EntrantInfo.CreatePlayer(packet.GetEntrantRes(), (RespawnFactorType)packet.RespawnFactorType == RespawnFactorType.Dead);
            NetworkController.ServerValue.SetRespawnPlayer(playerInfo);
            playerInfo.CreateObject();
        }
        else
        {
            // 0以外の場合は他人のことと見なす.
            Entrant.RemoveEntrant(packet.OldInFieldId);
            EntrantInfo info = EntrantInfo.Create(packet.GetEntrantRes(), true);
            info.CreateObject();
        }
    }
    #endregion

    //  UNDONE: Common.DLL: 使用しなくなったパケットの関連コードをコメントアウト
    /*
    #region Reset パケット
    /// <summary>
    /// Reset 受信通知
    /// </summary>
    /// <param name="packet"></param>
    public void EventReset(ResetEvent packet)
    {
        // パケッドが違う
        if (packet == null)
            return;

        // バトルUIリセット
        BattleMain.UIReset();
        // オブジェクトリセット
        NetworkController.DestroyReset();
        // EnterField
        BattlePacket.SendEnterField(ScmParam.Battle.BattleFieldType);
    }
    #endregion
    */

    #region SkillMotion パケット
    /// <summary>
    /// SkillMotion 送信
    /// スキルモーション送信
    /// </summary>
    /// <param name="skillID"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public static void SendSkillMotion(int skillID, Vector3 position, Quaternion rotation)
    {
        SkillMotionReq packet = new SkillMotionReq();
        packet.SkillId = skillID;
        packet.Position = new float[] { position.x, position.y, position.z };
        packet.Rotation = rotation.eulerAngles.y;
        GameListener.Send(packet);
    }
    /// <summary>
    /// SkillMotion 受信通知
    /// スキルモーション処理
    /// </summary>
    /// <param name="packet"></param>
    public void EventSkillMotion(SkillMotionEvent packet)
    {
        // パケッドが違う
        if (packet == null)
            return;
        // 存在してない
        ObjectBase objectBase = GameController.SearchInFieldID(packet.InFieldId);
        if (objectBase == null)
            return;

        objectBase.SkillMotion(
            packet.SkillId,
            null,
            new Vector3(packet.StartPos[0], packet.StartPos[1], packet.StartPos[2]),
            Quaternion.Euler(0f, packet.Direction, 0f)
        );
    }
    #endregion

    #region Judge パケット
    /// <summary>
    /// Judge 送信
    /// 現在の審判情報を知りたいときに送る
    /// </summary>
    public static void SendJudge()
    {
        JudgeReq packet = new JudgeReq();
        GameListener.Send(packet);
    }
    /// <summary>
    /// Judge 受信応答
    /// 現在の審判情報を取得する
    /// </summary>
    /// <param name="packet"></param>
    public void OperationResponseJudge(JudgeRes packet)
    {
        // パケッドが違う
        if (packet == null)
            return;

        // 残り時間のパケットを処理する
        this.EventRemainingTime(packet.GetRemainingTimeEvent());
        // 結果を送る
        BattleMain.Judge(packet.JudgeType, packet.HasNextMatch);
    }
    /// <summary>
    /// Judge 受信通知
    /// サーバーから審判情報が送られてくる
    /// </summary>
    /// <param name="packet"></param>
    public void EventJudge(JudgeEvent packet)
    {
        // パケッドが違う
        if (packet == null)
            return;

        // 残り時間のパケットを処理する
        this.EventRemainingTime(packet.GetRemainingTimeEvent());
        // 結果を送る
        BattleMain.Judge(packet.JudgeType, packet.HasNextMatch);
    }
    #endregion

    #region RemainingTime パケット
    /// <summary>
    /// RemainingTime 送信
    /// 現在の残り時間を知りたいときに送る
    /// </summary>
    public static void SendRemainingTime(int fieldId)
    {
        RemainingTimeReq packet = new RemainingTimeReq();
        packet.FieldId = fieldId;
        GameListener.Send(packet);
    }
    /// <summary>
    /// RemainingTime 受信応答
    /// 現在の残り時間を取得する
    /// </summary>
    /// <param name="packet"></param>
    public void OperationResponseRemainingTime(RemainingTimeRes packet)
    {
        // パケッドが違う
        if (packet == null)
            return;

        // バトル状態保存
        if (NetworkController.ServerValue != null)
            NetworkController.ServerValue.SetFieldStateType(packet.FieldStateType);

        if (BattleMain.TimeController != null)
        {
            // 時間更新
            BattleMain.TimeController.SetRemainingTime((float)packet.Second, (float)packet.RoundSecond, packet.FieldStateType);
            Debug.LogWarning("===>R " + packet.FieldStateType);
        }
    }
    /// <summary>
    /// RemainingTime 受信通知
    /// サーバーからゲームの残り時間が送られてくる
    /// </summary>
    /// <param name="packet"></param>
    public void EventRemainingTime(RemainingTimeEvent packet)
    {
        // パケッドが違う
        if (packet == null)
            return;

        if (NetworkController.ServerValue != null)
        {
            // 状態を保存
            NetworkController.ServerValue.SetFieldStateType(packet.FieldStateType);
        }
        if (BattleMain.TimeController != null)
        {
            // 時間更新
            BattleMain.TimeController.SetRemainingTime((float)packet.Second, packet.RoundSecond, packet.FieldStateType);
            Debug.LogWarning("===>E " + packet.FieldStateType);
        }
    }
    #endregion

    #region Warp パケット
    /// <summary>
    /// Warp 送信
    /// ワープ送信
    /// </summary>
    public static void SendWarp(int id)
    {
        WarpReq packet = new WarpReq();
        packet.InFieldId = (short)id;

        GameListener.Send(packet);
    }

    /// <summary>
    /// Send proxy warp request
    /// </summary>
    /// <param name="id"></param>
    public static void SendProxyWarp(int warpId, int personId) {
        WarpReq packet = new WarpReq();
        packet.InFieldId = (short)warpId;
        ProxyReq req = new ProxyReq() {
            ActualCode = packet.Code,
            ActualPacket = packet.GetPacket(),
            InFieldId = (short)personId
        };

        GameListener.Send(req);
    }

    /// <summary>
    /// Warp 受信応答
    /// ワープ情報を取得する
    /// </summary>
    /// <param name="packet"></param>
    public void OperationResponseWarp(WarpRes packet)
    {
        // パケッドが違う
        if (packet == null)
            return;

        // オブジェクトが存在しない
        ObjectBase objectBase = GameController.GetPlayer();
        if (objectBase == null)
            return;

        switch (packet.WarpType)
        {
            case WarpType.Warp:
                objectBase.Warp(new Vector3(packet.Position[0], packet.Position[1], packet.Position[2]), Quaternion.Euler(0f, packet.Rotation, 0f));
                break;
            case WarpType.Jump:
                objectBase.Jump(new Vector3(packet.Position[0], packet.Position[1], packet.Position[2]), Quaternion.Euler(0f, packet.Rotation, 0f));
                break;
            case WarpType.Wire:
                objectBase.Wire(new Vector3(packet.Position[0], packet.Position[1], packet.Position[2]), Quaternion.Euler(0f, packet.Rotation, 0f));
                break;
            case WarpType.Capture:
                objectBase.Captured(new Vector3(packet.Position[0], packet.Position[1], packet.Position[2]), Quaternion.Euler(0f, packet.Rotation, 0f));
                break;
        }
    }
    /// <summary>
    /// Warp 受信通知
    /// サーバーからワープ情報が送られてくる
    /// </summary>
    /// <param name="packet"></param>
    public void EventWarp(WarpEvent packet)
    {
        // パケッドが違う
        if (packet == null)
            return;
        // オブジェクトが存在しない

        ObjectBase objectBase = GameController.SearchInFieldID(packet.InFieldId);
        if (objectBase == null)
            return;

        switch (packet.WarpType)
        {
            case WarpType.Warp:
                objectBase.Warp(new Vector3(packet.Position[0], packet.Position[1], packet.Position[2]), Quaternion.Euler(0f, packet.Rotation, 0f));
                break;
            case WarpType.Jump:
                objectBase.Jump(new Vector3(packet.Position[0], packet.Position[1], packet.Position[2]), Quaternion.Euler(0f, packet.Rotation, 0f));
                break;
            case WarpType.Wire:
                objectBase.Wire(new Vector3(packet.Position[0], packet.Position[1], packet.Position[2]), Quaternion.Euler(0f, packet.Rotation, 0f));
                break;
            case WarpType.Capture:
                objectBase.Captured(new Vector3(packet.Position[0], packet.Position[1], packet.Position[2]), Quaternion.Euler(0f, packet.Rotation, 0f));
                break;
        }
    }
    #endregion

    #region LevelUp パケット
    /// <summary>
    /// LevelUp 受信通知.
    /// </summary>
    /// <param name='packet'>
    public void EventLevelUp(LevelUpEvent packet)
    {
        // パケッドが違う
        if (packet == null)
            return;

        ObjectBase objectBase = GameController.SearchInFieldID(packet.InFieldId);
        if (objectBase == null)
            return;

        // レベルアップ処理
        objectBase.LevelUp(packet.Level, packet.HitPoint, packet.MaxHitPoint);

        // レベルアップメッセージ
        string str = MasterData.GetText(TextType.TX009_LevelUp, new string[] { objectBase.UserNameWithTeamColor, packet.Level.ToString() });
        if (!string.IsNullOrEmpty(str))
        {
            GUIChat.AddSystemMessage(false, str);
        }
    }
    #endregion

    #region KillSelf パケット
    /// <summary>
    /// killself 送信.
    /// </summary>
    public static void SendKillSelf()
    {
        KillSelfReq packet = new KillSelfReq();
        GameListener.Send(packet);
    }

    /// <summary>
    /// Send killself request for a bot
    /// </summary>
    /// <param name="inFieldId"></param>
    public static void ProxySendKillSelf(int inFieldId) {
        KillSelfReq packet = new KillSelfReq();
        ProxyReq req = new ProxyReq() {
            InFieldId = inFieldId,
            ActualCode = packet.Code,
            ActualPacket = packet.GetPacket()
        };
        GameListener.Send(req);
    }

    /// <summary>
    /// StockItemUse 受信通知
    /// </summary>
    public void OperationResponseKillSelf(KillSelfRes packet)
    {
        // パケットが違う
        if (packet == null)
            return;

        GUIRespawnInfo.SetRespawnTime(packet.RespawnTime, packet.RemainingTime);
    }
    #endregion

    #region TeamSkill パケット
    /// <summary>
    /// TeamSkill 送信
    /// チームスキル送信
    /// </summary>
    public static void SendTeamSkill(EntrantInfo caster, int bulletSetID, ObjectBase target, Vector3 position, Quaternion rotation)
    {
        TeamSkillReq packet = new TeamSkillReq();
        packet.InFieldCasterId = ObjectBase.CastInFieldID(caster.InFieldId);
        packet.BulletSetId = bulletSetID;
        packet.InFieldTargetId = ObjectBase.CastInFieldID(target.InFieldId);
        packet.Position = new float[] { position.x, position.y, position.z };
        packet.Direction = new float[] { rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z };
        GameListener.Send(packet);
    }
    /// <summary>
    /// TeamSkill 受信通知
    /// サーバーからチームスキル情報が送られてくる
    /// </summary>
    /// <param name="packet"></param>
    public void EventTeamSkill(TeamSkillEvent packet)
    {
        // パケッドが違う
        if (packet == null)
            return;

        Player player = GameController.GetPlayer();
        ObjectBase caster = GameController.SearchInFieldID(packet.InFieldCasterId);
        if (player == null)
            return;
        if (caster == null)
            return;

        // メッセージ表示
        if (caster.TeamType == player.TeamType)
            GUIEffectMessage.SetTacticalInfo(GUITacticalMessageItem.TacticalType.TeamSkill);
        else
            GUIEffectMessage.SetTacticalWarning(GUITacticalMessageItem.TacticalType.TeamSkill);

        // チームポイント設定
        GUITacticalGauge.TeamSkillPoint(caster.TeamType, packet.Point);

        GUIMapWindow.SetTeamSkillBreakCount(caster.TeamType.GetClientTeam(), GUITacticalGauge.GetSideGaugeCount(caster.TeamType.GetClientTeam()));
    }
    #endregion

    #region Exp パケット
    /// <summary>
    /// Bonus受信通知.
    /// </summary>
    /// <param name='packet'>
    /// Packet.
    /// </param>
    public void EventExp(ExpEvent packet)
    {
        // パケッドが違う
        if (packet == null)
            return;
        // プレイヤー取得
        Player player = GameController.GetPlayer();
        if (player == null)
            return;

        // 経験値
        player.AddExp(packet.Exp);

        // メッセージ
        //{
        //    var str = MasterData.GetText(TextType.TX012_KillBonus);
        //    GUIChat.AddSystemMessage(false, str);
        //}
        //if (packet.Exp > 0)
        //{
        //    var str = MasterData.GetText(TextType.TX010_GetExp, new string[] { packet.Exp.ToString() });
        //    GUIChat.AddSystemMessage(false, str);
        //}
        //// 試遊会Varではお金の使い道がないのでチャットでの表示は封印する.
        //if (packet.Money > 0)
        //{
        //    var str = MasterData.GetText(TextType.TX011_GetMoney, new string[] { packet.Money.ToString() });
        //    //GUIChat.AddSystemMessage(false, str);
        //}
    }
    #endregion

    #region TeamSkillPointパケット
    /// <summary>
    /// TeamSkillPoint 送信
    /// </summary>
    public static void SendTeamSkillPoint()
    {
        TeamSkillPointReq packet = new TeamSkillPointReq();
        GameListener.Send(packet);
    }
    /// <summary>
    /// TeamSkillPoint 受信応答
    /// </summary>
    /// <param name="packet"></param>
    public void OperationResponseTeamSkillPoint(TeamSkillPointRes packet)
    {
        // パケットが違う
        if (packet == null)
            return;

        GUITacticalGauge.TeamSkillPoint(TeamType.Red, packet.RedPoint);
        GUITacticalGauge.TeamSkillPoint(TeamType.Blue, packet.BluePoint);

        GUIMapWindow.SetTeamSkillBreakCount(TeamType.Red.GetClientTeam(), GUITacticalGauge.GetSideGaugeCount(TeamType.Red.GetClientTeam()));
        GUIMapWindow.SetTeamSkillBreakCount(TeamType.Blue.GetClientTeam(), GUITacticalGauge.GetSideGaugeCount(TeamType.Blue.GetClientTeam()));
    }
    /// <summary>
    /// TeamSkillPoint 受信通知
    /// サーバーからチームスキルポイントが送られてくる
    /// </summary>
    /// <param name="packet"></param>
    public void EventTeamSkillPoint(TeamSkillPointEvent packet)
    {
        // パケットが違う
        if (packet == null)
            return;
        // チームスキルポイントの設定をする
        GUITacticalGauge.TeamSkillPoint(packet.TeamType, packet.Point);

        // チームスキル発動カウント設定
        GUIMapWindow.SetTeamSkillBreakCount(packet.TeamType.GetClientTeam(), GUITacticalGauge.GetSideGaugeCount(packet.TeamType.GetClientTeam()));

        // 正規化したポイントを取得する
        int point = GUITacticalGauge.GetNormalizeSideGaugePoint(packet.TeamType, packet.Point);

        // 残り1ブレイクかどうか判定
        if (point >= 100)
        {
            if (packet.TeamType.GetClientTeam() == TeamTypeClient.Friend)
            {
                // 自チーム側のチームスキル発動前メッセージ
                GUIEffectMessage.SetTacticalInfo(GUITacticalMessageItem.TacticalType.PoiseTeamSkill);
            }
            else
            {
                // 敵チーム側のチームスキル発動前メッセージ
                GUIEffectMessage.SetTacticalWarning(GUITacticalMessageItem.TacticalType.PoiseTeamSkill);
            }
        }
    }
    #endregion

    #region ItemGet パケット.
    /// <summary>
    /// ItemGet 送信.
    /// アイテム取得した時に送る.
    /// </summary>
    public static void SendItemGet(int inFieldItemId)
    {
        ItemGetReq packet = new ItemGetReq();
        packet.InFieldItemId = ObjectBase.CastInFieldID(inFieldItemId);
        GameListener.Send(packet);
    }
    /// <summary>
    /// ItemGet 受信応答
    /// アイテム取得した情報を取得する
    /// </summary>
    /// <param name="packet"></param>
    public void OperationResponseItemGet(ItemGetRes packet)
    {
        // パケッドが違う
        if (packet == null)
            return;
        // アイテム取得
        ItemDropBase item = GameController.SearchInFieldID(packet.InFieldItemId) as ItemDropBase;
        if (item == null)
            return;
        // プレイヤー取得
        Player player = GameController.GetPlayer();
        if (player == null)
            return;

        // アイテム取得.
        item.ItemGet(player, packet.ItemSpecialEffect);
    }
    /// <summary>
    /// ItemGet 受信通知
    /// サーバーからアイテム取得情報が送られてくる
    /// </summary>
    /// <param name="packet"></param>
    public void EventItemGet(ItemGetEvent packet)
    {
        // パケッドが違う
        if (packet == null)
            return;
        // アイテム取得
        ObjectBase item = GameController.SearchInFieldID(packet.InFieldItemId);
        if (item == null)
            return;
        // アイテムオブジェクト
        ItemDropBase itemBase = item as ItemDropBase;
        if (itemBase == null)
            return;

        if (packet.InFieldUserId == 0)
        {
            // アイテム消滅.
            itemBase.DestroyAnimation();
        }
        else
        {
            // アイテム取得
            ObjectBase user = GameController.SearchInFieldID(packet.InFieldUserId);
            if (user == null)
                return;

            itemBase.ItemGet(user, packet.ItemSpecialEffect);
        }
    }
    #endregion

    #region GameResultパケット
    /// <summary>
    /// GameResult 送信
    /// </summary>
    public static void SendGameResult()
    {
        GameResultReq packet = new GameResultReq();
        GameListener.Send(packet);
    }
    /// <summary>
    /// GameResult 受信応答
    /// </summary>
    /// <param name="packet"></param>
    public void OperationResponseGameResult(GameResultRes packet)
    {
        // パケットが違う
        if (packet == null)
            return;
        //packet.GetGameResultPackets.
        //packet.GetGameResultPackets()[0].
        //packet.GetCharacterParameters
        // リザルト情報.
        BridgingResultInfo resultInfo = new BridgingResultInfo();
        resultInfo.Setup(NetworkController.ServerValue.InFieldId, packet.Judge, packet.GetGameResultPackets(), packet.GetRewardBattleResultParameter());
        resultInfo.Setup(packet.GameSeconds, packet.BattleFieldId, packet.ScoreType, packet.GetCharacterParameters());
       
        BattleMain.GameResult(resultInfo);
    }
    #endregion

    #region Motion パケット
    /// <summary>
    /// Motion 送信
    /// モーション送信
    /// </summary>
    public static void SendMotion(Scm.Common.GameParameter.MotionState motionstate)
    {
        MotionReq packet = new MotionReq();
        packet.MotionState = motionstate;
        GameListener.Send(packet);
    }

    public static void ProxySendMotion(int inFieldId, Scm.Common.GameParameter.MotionState motionstate) {
        MotionReq packet = new MotionReq();
        packet.MotionState = motionstate;
        ProxyReq req = new ProxyReq() {
            ActualCode = packet.Code,
            ActualPacket = packet.GetPacket(),
            InFieldId = inFieldId
        };
        GameListener.Send(req);
    }

    /// <summary>
    /// Motion 受信
    /// モーション処理
    /// </summary>
    public void EventMotion(MotionEvent packet)
    {
        // パケットが違う
        if (packet == null)
            return;
        // 存在してない
        ObjectBase objectBase = GameController.SearchInFieldID(packet.InFieldId);
        if (objectBase == null)
            return;

        objectBase.Motion(packet.MotionState);
    }
    #endregion

    #region SkillCharge パケット
    /// <summary>
    /// SkillCharge 送信
    /// </summary>
    public static void SendSkillCharge(int skillID, ObjectBase target, bool isCharge)
    {
        SkillChargeReq packet = new SkillChargeReq();
        packet.SkillId = skillID;
        packet.InFieldTargetId = ObjectBase.CastInFieldID(target != null ? target.InFieldId : GameConstant.InvalidID);
        packet.Continue = isCharge;
        GameListener.Send(packet);
    }
    /// <summary>
    /// SkillCharge 受信通知
    /// </summary>
    public void EventSkillCharge(SkillChargeEvent packet)
    {
        // パケットが違う
        if (packet == null)
            return;
        // 存在してない
        ObjectBase objectBase = GameController.SearchInFieldID(packet.InFieldId);
        if (objectBase == null)
            return;
        ObjectBase target = GameController.SearchInFieldID(packet.InFieldTargetId);

        objectBase.SkillCharge(packet.SkillId, target, packet.Continue);
    }
    #endregion
    //  UNDONE: Common.DLL: 使用しなくなったパケットの関連コードをコメントアウト
    /*
    #region StockItemUse パケット
    /// <summary>
    /// StockItemUse 送信
    /// </summary>
    public static void SendStockItemUse(byte useStockId)
    {
        StockItemUseReq packet = new StockItemUseReq();
        packet.UseStockId = useStockId;
        GameListener.Send(packet);
    }
    /// <summary>
    /// StockItemUse 受信通知
    /// </summary>
    public void OperationResponseStockItemUse(StockItemUseRes packet)
    {
        // パケットが違う
        if (packet == null)
            return;

        // プレイヤー取得
        Player player = GameController.GetPlayer();
        if (player == null)
            return;

        // 仕様削除のため整理する
    }
    #endregion
    */
    #region Score パケット
    /// <summary>
    /// EventScore 受信通知
    /// </summary>
    public void EventScore(ScoreEvent packet)
    {
        // パケッドが違う
        if (packet == null)
            return;

        ScorePacketParameter[] scoreParams = packet.GetScoreParams();

        List<MemberInfo> memberList = new List<MemberInfo>();
        foreach (var elem in scoreParams)
        {
            AvatarInfo avatarInfo;
            if (Entrant.TryGetEntrant<AvatarInfo>(elem.InFieldId, out avatarInfo))
            {
                MemberInfo info = new MemberInfo();
                info.inFieldID = avatarInfo.InFieldId;
                info.tacticalId = avatarInfo.TacticalId;
                info.avatarType = (AvatarType)avatarInfo.Id;
                info.name = avatarInfo.UserName;
                info.teamType = avatarInfo.TeamType.GetClientTeam();
                info.score = elem.Score;
                info.kill = elem.Kill;
                info.death = elem.Death;
                info.avatarInfo = avatarInfo;

                info.attack = elem.Attack;
                info.controlTime = elem.ControlTime;

                memberList.Add(info);
                
                avatarInfo.KillCount = elem.Kill;
            }
        }

        // スコア順位計算
        {
            // スコアで降順に並び替えて順位付け
            memberList.Sort((a, b) => { return GameGlobal.DescendSort(a.score, b.score); });

            if (0 < memberList.Count)
            {
                // 先頭のランクを決定
                memberList[0].avatarInfo.ScoreRank = (memberList[0].score <= 0) ? -1 : 1;
                int rank = 1;
                for (int i = 1; i < memberList.Count; i++)
                {
                    // スコアが０なら問答無用で何もなし
                    if (memberList[i].score <= 0)
                    {
                        memberList[i].avatarInfo.ScoreRank = -1;
                        continue;
                    }

                    if (memberList[i].score < memberList[i - 1].score)
                    {
                        rank += 1;
                    }

                    memberList[i].avatarInfo.ScoreRank = rank;
                }
            }
        }

        GUIMapWindow.SetMemberList(memberList);
    }
    #endregion

    #region Buff パケット
    /// <summary>
    /// Buffリクエスト 送信
    /// </summary>
    /// <param name="caster"></param>
    public static void SendBuff(short inFieldId)
    {
        BuffReq packet = new BuffReq();
        packet.InFieldId = inFieldId;

        GameListener.Send(packet);
    }
    /// <summary>
    /// Buff 受信応答
    /// </summary>
    public void OperationResponseBuff(BuffRes packet)
    {
        // パケットが不正.
        if (packet == null)
            return;

        ObjectBase objectBase = GameController.SearchInFieldID(packet.InFieldId);
        // 対象が存在しない.
        if (objectBase == null)
            return;

        // バフ処理.
        objectBase.ChangeBuff(null, packet.GetBuffPackets(), packet.GetSpBuffPackets(), packet.Speed);
    }
    /// <summary>
    /// EventBuff 受信通知
    /// </summary>
    public void EventBuff(BuffEvent packet)
    {
        // パケットが不正.
        if (packet == null)
            return;

        ObjectBase objectBase = GameController.SearchInFieldID(packet.InFieldId);
        // 対象が存在しない.
        if (objectBase == null)
            return;

        // バフ処理.
        objectBase.ChangeBuff(packet.NewBuffTypes, packet.GetBuffPackets(), packet.GetSpBuffPackets(), packet.Speed);
    }
    #endregion

    //  UNDONE: Common.DLL: 使用しなくなったパケットの関連コードをコメントアウト
    /*
    #region BuyStockItem パケット
    /// <summary>
    /// BuyStockItem 送信
    /// </summary>
    public static void SendBuyStockItem(int buyShopObjectId, int buyStockId)
    {
        BuyStockItemReq packet = new BuyStockItemReq();
        packet.BuyShopObjectId = buyShopObjectId;
        packet.BuyStockItemId = buyStockId;
        GameListener.Send(packet);
    }
    /// <summary>
    /// BuyStockItem 受信通知
    /// </summary>
    public void OperationResponseBuyStockItem(BuyStockItemRes packet)
    {
        // パケットが違う
        if (packet == null)
            return;

        // 仕様上必要なくなったので整理するために削除する
    }
    #endregion
    */
    #region SkillCastMarker パケット
    /// <summary>
    /// SkillCastMarker 送信要求
    /// </summary>
    public static void SendSkillCastMarker(int skillCastMarkerID, Vector3 position, Quaternion rotation)
    {
        SkillCastMarkerReq packet = new SkillCastMarkerReq();
        packet.SkillCastMarkerId = skillCastMarkerID;
        packet.Position = new float[] { position.x, position.y, position.z };
        packet.Rotation = rotation.eulerAngles.y;
        GameListener.Send(packet);
    }
    /// <summary>
    /// SkillCastMarker 受信通知
    /// </summary>
    public void EventSkillCastMarker(SkillCastMarkerEvent packet)
    {
        // パケッドが違う
        if (packet == null)
            return;

        ObjectBase caster = GameController.SearchInFieldID(packet.InFieldId);
        if (caster != null)
        {
            SkillCastMarkerMasterData marker;
            if (SkillCastMarkerMaster.Instance.TryGetMasterData(packet.SkillCastMarkerId, out marker))
            {
                Vector3 position = new Vector3(packet.Position[0], packet.Position[1], packet.Position[2]);
                Quaternion rotation = Quaternion.Euler(0f, packet.Rotation, 0f);
                EffectManager.CreateCastMarker(caster, position, rotation, marker);
            }
        }
    }
    #endregion

    #region TargetMarker パケット
    public static void SendTargetMarker(int targetFieldID, TargetMarkerActionType actionType)
    {
        TargetMarkerReq packet = new TargetMarkerReq();

        packet.InFieldId = (short)targetFieldID;
        packet.TargetMarkerActionType = actionType;

        GameListener.Send(packet);
    }
    public static void OperationResponseTargetMarker(TargetMarkerRes packet)
    {
        // パケットが違う
        if (packet == null)
            return;

        //GUIMapWindow.RecieveTargetMarkerRes(packet);
    }

    public void EventTargetMarker(TargetMarkerEvent packet)
    {
        // パケットが違う
        if (packet == null)
            return;

        //TargetMarkerInfo info = new TargetMarkerInfo(packet);
        //GUIMapWindow.ReciveTargetMarkerEvent(info);
    }
    public static void SendTargetMarkerAll()
    {
        TargetMarkerAllReq packet = new TargetMarkerAllReq();
        GameListener.Send(packet);
    }
    public void OperationResponseTargetMarkerAll(TargetMarkerAllRes packet)
    {
        // パケットが違う
        if (packet == null)
            return;
        //foreach( var eventPacket in packet.GetTargetMarkerEventArray() )
        //{
        //TargetMarkerInfo info = new TargetMarkerInfo(eventPacket);
        //GUIMapWindow.ReciveTargetMarkerEvent(info);
        //}
    }
    #endregion

    #region Transport パケット
    /// <summary>
    /// Transport 送信
    /// </summary>
    public static void SendTransport(int inFieldID)
    {
        TransportReq packet = new TransportReq();
        packet.InFieldId = ObjectBase.CastInFieldID(inFieldID);
        //InFieldId	short	トランスポータのInFieldId
        GameListener.Send(packet);
    }
    /// <summary>
    /// TransportRes 受信通知
    /// </summary>
    public void OperationResponseTransport(TransportRes packet)
    {
        // パケットが違う
        if (packet == null)
            return;

        if (!packet.Result)
        {
            Debug.Log("OperationResponseTransport:Fail");
            return;
        }

        //InFieldId	short	トランスポータのInFieldId
        //Result	bool	成否
        //Deck	CharacterDeckRes	現在のデッキ情報

        // Playerの表示を消してMapWindowウィンドウ開く
        var player = GameController.GetPlayer();
        if (player != null)
        {
            //player.gameObject.SetActive(false);
            Entrant.RemoveEntrant(player.InFieldId);
        }
        GUIMapWindow.SetMode(GUIMapWindow.MapMode.Transport);

        // デッキ情報を保持する
        var deckInfo = new DeckInfo(packet.GetCharacterDeckRes());
        GUIDeckEdit.SetDeck(deckInfo, deckInfo.CurrentSlotIndex);
    }
    /// <summary>
    /// TransportEvent 受信通知
    /// </summary>
    public void EventTransport(TransportEvent packet)
    {
        // パケットが違う
        if (packet == null)
            return;

        // 他人の場合は消す
        var player = GameController.GetPlayer();
        if (player == null || player.InFieldId != packet.InFieldId)
        {
            Entrant.RemoveEntrant(packet.InFieldId);
        }

        //InFieldId	short	TransportReqを送信したプレイヤーのInFieldId
    }
    #endregion

    #region SelectCharacterパケット
    public static void SendSelectCharacter(int deckIdx, int respawnInFieldID)
    {
        SelectNextDeckCharacterReq packet = new SelectNextDeckCharacterReq();
        packet.Index = deckIdx;
        packet.InFieldId = ObjectBase.CastInFieldID(respawnInFieldID);

        Player player = GameController.GetPlayer();
        if (player)
        {
            player.SetBlockSendMoveTime(GameConstant.BlockSendMoveTime_Respawn);
        }
        GameListener.Send(packet);
    }
    /// <summary>
    /// SelectNextDeckCharacterRes 受信通知
    /// </summary>
    public void OperationResponseSelectCharacter(SelectNextDeckCharacterRes packet)
    {
        // パケットが違う
        if (packet == null)
            return;

        if (!packet.Result)
        {
            Debug.Log("OperationResponseSelectCharacter:Fail");
            GUIMapWindow.FailSendCharacter();
            return;
        }



        //Index	int	デッキのスロットindex(0～3)
        //Result	bool	選択結果(true=成功、false=失敗)

    }
    #endregion

    #region SideGauge パケット
    /// <summary>
    /// SideGauge 送信
    /// </summary>
    public static void SendSideGauge()
    {
        SideGaugeReq packet = new SideGaugeReq();
        GameListener.Send(packet);
    }
    /// <summary>
    /// SideGaugeRes 受信通知
    /// </summary>
    public void OperationResponseSideGauge(SideGaugeRes packet)
    {
        // パケットが違う
        if (packet == null)
            return;

        GUITacticalGauge.SideGauge(TeamType.Red, packet.RedRemain, packet.RedTotal, packet.RoundIndex);
        GUITacticalGauge.SideGauge(TeamType.Blue, packet.BlueRemain, packet.BlueTotal, packet.RoundIndex);

        GUIMapWindow.SetTeamSkillBreakCount(TeamType.Red.GetClientTeam(), GUITacticalGauge.GetSideGaugeCount(TeamType.Red.GetClientTeam()));
        GUIMapWindow.SetTeamSkillBreakCount(TeamType.Blue.GetClientTeam(), GUITacticalGauge.GetSideGaugeCount(TeamType.Blue.GetClientTeam()));
    }
    /// <summary>
    /// SideGaugeEvent 受信通知
    /// </summary>
    public void EventSideGauge(SideGaugeEvent packet)
    {
        // パケットが違う
        if (packet == null)
            return;

        GUITacticalGauge.SideGauge(TeamType.Red, packet.RedRemain, packet.RedTotal, packet.RoundIndex);
        GUITacticalGauge.SideGauge(TeamType.Blue, packet.BlueRemain, packet.BlueTotal, packet.RoundIndex);

        GUIMapWindow.SetTeamSkillBreakCount(TeamType.Red.GetClientTeam(), GUITacticalGauge.GetSideGaugeCount(TeamType.Red.GetClientTeam()));
        GUIMapWindow.SetTeamSkillBreakCount(TeamType.Blue.GetClientTeam(), GUITacticalGauge.GetSideGaugeCount(TeamType.Blue.GetClientTeam()));
    }

    /// <summary>
    /// Skill Gauge Event Notification
    /// </summary>
    /// <param name="packet"></param>
    public void EventSkillGauge(SkillGaugeEvent packet)
    {
        // パケットが違う
        if (packet == null)
            return;

        GUISkillButton button = GUISkill.FindButton(x => x.SkillButtonType == SkillButtonType.SpecialSkill);
        if (button != null)
        {
            button.AddGauge(packet.GaugeValue);
            PlayerManager.Instance.Player.GaugeIncreased(packet.GaugeValue);
        }
    }

    /// <summary>
    /// Called when resident area's side gauge or stand by value is changed
    /// </summary>
    /// <param name="packet"></param>
    public void EventResidentAreaSideGauge(ResidentAreaSideGaugeEvent packet)
    {
        // パケットが違う
        if (packet == null)
            return;

        // TODO
        ResidentArea.OnHoldRefresh(packet);
    }

    public void EntrantActive(EntrantActiveEvent packet)
    {
        // パケットが違う
        if (packet == null)
            return;

        Debug.LogError("===>Resident " + packet.Active + " " + packet.Code + " " + packet.InFieldId);
        // TODO
        ResidentArea.OnActiveRefresh(packet.Active, packet.InFieldId);
    }

    ///<summary>
    ///Called when side gauge's percent
    ///</summary>
    ///<param name="packet"></param>
    public void EventSideGaugePercent(SideGaugePercentEvent packet)
    {
       
        GUITacticalGauge.Instance.SetHostageState(packet.Percent);
    }

    public void EventBonusTime(BonusTimeEvent packet) {
        GUITacticalGauge.Instance.ShowEscortTime(packet.BonusTime);
        Debug.Log("<color=#00ff00>bonus time:" + packet.BonusTime + "</color>");
    }

    /// <summary>
    /// All proxy res
    /// </summary>
    /// <param name="packet"></param>
    public void OnProxyRes(ProxyRes packet) {
        Debug.Log("Proxy packet res:" + packet.ActualCode);
    }

    #endregion
}
