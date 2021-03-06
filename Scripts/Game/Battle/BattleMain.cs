/// <summary>
/// バトルメイン処理
/// 
/// 2013/01/07
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common;
using Scm.Common.GameParameter;
using Scm.Common.Packet;
using XUI;

public class BattleMain : SceneMain<BattleMain>
{
	#region フィールド＆プロパティ
	const string SceneName = SceneController.SceneName.Battle;

	public float resultChangeTime = 15f;

	public bool IsExitFieldResponse { get; set; }
	public static bool IsExitField { get { return (Instance != null ? Instance.IsExitFieldResponse : false); } set { if (Instance != null) Instance.IsExitFieldResponse = value; } }

	public TeamType PlayerTeamType { get; private set; }

	// 弾丸管理.
	private BulletMonitor bulletMonitor = new BulletMonitor();
	public BulletMonitor BulletMonitor { get { return bulletMonitor; } }

	/// <summary>
	/// バトル中に使うキャラアイコン
	/// </summary>
	public static CharaIcon CharaIcon { get { return ScmParam.Battle.CharaIcon; } }

	/// <summary>
	/// バトル中に使う状態アイコン
	/// </summary>
	StateIcon stateIcon = null;
	StateIcon _StateIcon
	{
		get
		{
			if(stateIcon == null)
			{
				stateIcon = new StateIcon();
			}
			return stateIcon;
		}
	}
	public static StateIcon StateIcon { get { return (Instance != null ? Instance._StateIcon : null); } }

	/// <summary>
	/// 戦闘時間の制御
	/// </summary>
	BattleTimeController _battleTimeController = null;
	BattleTimeController BattleTimeController
	{
		get
		{
			if(_battleTimeController == null)
			{
				_battleTimeController = new BattleTimeController();
			}
			return _battleTimeController;
		}
	}
	public static BattleTimeController TimeController { get { return (Instance != null ? Instance.BattleTimeController : null); } }

	/// <summary>
	/// サウンド
	/// </summary>
	BattleSound _battleSound = null;
	BattleSound BattleSound
	{
		get
		{
			if(_battleSound == null)
			{
				_battleSound = new BattleSound();
			}
			return _battleSound;
		}
	}
	public static BattleSound Sound { get { return (Instance != null ? Instance.BattleSound : null); } }
	#endregion

	#region 初期化
	static public void LoadScene(int fieldId, int mapID, PlayerInfo playerInfo, BattleFieldType battleFieldType)
	{
		// エリアタイプ設定
		ScmParam.Common.AreaType = AreaType.Field;
		// バトルフィールド設定
		ScmParam.Battle.BattleFieldType = battleFieldType;
		// マッチング状態を元に戻す
		if (NetworkController.ServerValue != null)
			NetworkController.ServerValue.SetMatchingStatus(MatchingStatus.Normal);

		FiberSet fiberSet = new FiberSet();
		fiberSet.AddFiber(ResourceLoad(fieldId, mapID, playerInfo));

        //fade out and in
        SceneController.FadeSceneChange(SceneName, fiberSet);
	}

    static public void LoadScene(int fieldId, int mapID, PlayerInfo playerInfo, BattleFieldType battleFieldType, EnterFieldRes enterFieldRes)
    {
        // エリアタイプ設定
        ScmParam.Common.AreaType = AreaType.Field;
        // バトルフィールド設定
        ScmParam.Battle.BattleFieldType = battleFieldType;
        // マッチング状態を元に戻す
        if (NetworkController.ServerValue != null)
            NetworkController.ServerValue.SetMatchingStatus(MatchingStatus.Normal);

        FiberSet fiberSet = new FiberSet();
        fiberSet.AddFiber(ResourceLoad(fieldId, mapID, playerInfo));

        //fade out and in
        //SceneController.FadeSceneChange(SceneName, fiberSet);

        //anime in
        SceneController.AnimeSceneChange(enterFieldRes, SceneName, fiberSet);
    }

    static private IEnumerator ResourceLoad(int fieldId, int mapID, PlayerInfo playerInfo)
	{
        Debug.Log("Rank loading...");
		// バトル用のBGMに確実に切り替わるためBGMの停止は無し.
		// もし停止する場合はRemainingTimeResの受信タイミングに気を付ける.

		// マップの読み込み.
		NetworkController.DestroyAll();
		MapManager.Create(AreaType.Field, fieldId, mapID);
		while(!MapManager.Instance.MapExists)
		{
			// マップの読み込みが終わっていないので待つ.
			yield return null;
		}
        //暂时这样写，这个是最后一个加载的
        while (GUIHelpMessage.Instance == null)
        {
            yield return null;
        }
		// フィールド内の参加者全員の情報を取得する
		CommonPacket.SendEntrantAll(fieldId);

		// プレイヤーキャラクターの読み込み.
		if(playerInfo != null)
		{
			playerInfo.CreateObject();
			while(GameController.GetPlayer() == null)
			{
				// HACK: プレイヤー読み込みに失敗した場合はどうする？.
				// プレイヤーキャラクターの読み込みが終わっていないので待つ.
				yield return null;
			}
		}
		// マルチタップを念のため有効にしておく
		Input.multiTouchEnabled = true;
	}
	/// <summary>
	/// 再参戦時のシーン読み込み
	/// </summary>
	/// <param name="fieldId"></param>
	/// <param name="mapID"></param>
	/// <param name="playerInfo"></param>
	static public void ReEntryLoadScene(int fieldId, int mapID, PlayerInfo playerInfo, BattleFieldType battleFieldType)
	{
		// エリアタイプ設定
		ScmParam.Common.AreaType = AreaType.Field;
		// バトルフィールド設定
		ScmParam.Battle.BattleFieldType = battleFieldType;
		// マッチング状態を元に戻す
		if (NetworkController.ServerValue != null)
			NetworkController.ServerValue.SetMatchingStatus(MatchingStatus.Normal);

		FiberSet fiberSet = new FiberSet();
		//fiberSet.AddFiber(ResourceLoad(fieldId, mapID, playerInfo));
		fiberSet.AddFiber(ReEntryResourceLoad(fieldId,mapID,playerInfo));

		SceneController.FadeSceneChange(SceneName, fiberSet);
	}
	static IEnumerator ReEntryResourceLoad(int fieldId, int mapID,PlayerInfo playerInfo)
	{

		// バトル用のBGMに確実に切り替わるためBGMの停止は無し.
		// もし停止する場合はRemainingTimeResの受信タイミングに気を付ける.

		// マップの読み込み.
		NetworkController.DestroyAll();
		MapManager.Create(AreaType.Field, fieldId, mapID);
		while(!MapManager.Instance.MapExists)
		{
			// マップの読み込みが終わっていないので待つ.
			yield return null;
		}

        while (GUIHelpMessage.Instance == null)
	    {
	        yield return null;
	    }
		// フィールド内の参加者全員の情報を取得する
		CommonPacket.SendEntrantAll(fieldId);

		// カメラ設定
		var cc = GameController.CharacterCamera;
		if(cc != null)
		{
			cc.SetReEntryCamera(playerInfo.StartPosition,new Vector3(0f,playerInfo.StartRotation,0f));
		}

		// プレイヤーキャラクターの読み込み.
		if(playerInfo != null)
		{
			// EntrantInfoに登録.StatusType.NotInAreaのはずなので実体は作られない.
			playerInfo.CreateObject();
		}

		while (GUIMapWindow.Instance == null)
			yield return null;

		// リスポーン画面開く
		GUIMapWindow.SetMode(GUIMapWindow.MapMode.Respawn);
	}

	void Start()
	{
		// UIモードの変更
		UIManager.SetModeType(UIModeType.Battle);

		// 弾丸リストのクリア.
		this.bulletMonitor.Clear();

		// 通信情報を元にUI系をセット.
		var serverValue = NetworkController.ServerValue;

		// 時間の初期設定を行う
		this.BattleTimeController.SetupStartTimer(serverValue.BattleFieldId);

		// チームタイプ設定
		this.SetTeamType(serverValue.TeamType);

		// 共通情報
		// 現在の残り時間を取得する
		BattlePacket.SendRemainingTime(serverValue.FieldId);

		// タワー戦情報
		// チームスキルポイントを取得する
		BattlePacket.SendTeamSkillPoint();

		// サイドゲージ情報
		// サイドゲージ系の情報を取得する
		BattlePacket.SendSideGauge();
	}
	#endregion

	#region 終了処理
	void OnDestroy()
	{
		ScmParam.Battle.CharaIcon.Clear();
		ScmParam.Battle.CharaBoard.Clear();
		ScmParam.Battle.SkillIcon.Clear();
		_StateIcon.Clear();
        //卸载图集引用
        GUIDeckEdit.Unload();
        GUINextRoundInfo.Unload();
        GUIBattleDirectionTip.Unload();
//        GUIMapWindow.Unload();
        GUISpSkillCutIn.Unload();
        GUITutorialNotice.Unload();
        GUIGMWindow.Unload();
        GUISkill.Unload();
        GUIBreakInfo.Unload();
        GUITransporterInfo.Unload();
        GUIStateMessage.Unload();
//        GUIMinimap.Unload();
        GUITacticalGauge.Unload();
        GUIRespawnInfo.Unload();
        GUIBattlePlayerInfo.Unload();
        GUIChatFrameController.Unload();
        GUIHelpMessage.Unload();
	    Resources.UnloadUnusedAssets();

	    GUITargetButton.Instance = null;
        Debug.LogError("===> Battle Clear");

	}
	#endregion

	#region SceneMain
	/// <summary>
	/// サーバから切断されたかどうかのフラグ
	/// </summary>
	private bool isDisConnectByServer = false;
	public override bool OnNetworkDisconnect()
	{
		// サーバからによる切断が行われていれば再接続処理は行わない
		if(this.isDisConnectByServer)
		{
			return true;
		}
		else
		{
			// 再接続開始
			ReLoginRequst.StartRequest();
			return false;
		}
	}

	public override void OnNetworkDisconnectByServer()
	{
		this.isDisConnectByServer = true;
	}
	#endregion

	#region 更新
	void Update()
	{
		this.UpdateCheatCheck();
		// バトル時間
		this.BattleTimeController.Update();
	}

	private float m_time = 0;		//経過時間保存
	private float m_interval = 0;	//チェックする時間間隔
	void UpdateCheatCheck()
	{
		//時間更新
		m_time += Time.deltaTime;
		//チェックする時間間隔を経過してれば処理をする
		if (m_interval < m_time)
		{
			AndroidRootCheck.RootCheck();
			//時間リセット
			m_time = 0;
			//次にチェックする時間を設定
			m_interval = Random.Range(1.0f, 2.0f) * 60f;
		}
	}
	#endregion

	#region NGUIリフレクション
	public void OnLogout()
	{
		// デバッグログ非表示
		GUIDebugLog.Close();
		TitleMain.LoadScene();
	}
	public void OnResult()
	{
		// リザルトパケット送信
		this.resultChangeTime = 0f;
		this.SendGameResultPacket();
	}
	#endregion

	#region ログアウト
	void ExitField(BridgingResultInfo resultInfo)
	{
		this.StartCoroutine(this.ExitFieldCoroutine(resultInfo));
	}
	IEnumerator ExitFieldCoroutine(BridgingResultInfo resultInfo)
	{
		if (Scm.Client.GameListener.ConnectFlg)
		{
			this.IsExitFieldResponse = false;
			// ログアウト
			BattlePacket.SendExitField();
			while (!this.IsExitFieldResponse)
			{
				yield return 0;
			}
		}

		// ロード画面表示
		GUILoading.SetActive(true);
		yield return 0;

		NextScene(resultInfo);
	}
	void NextScene(BridgingResultInfo resultInfo)
	{
		// カメラ初期化
		GameController.Instance.CharaCamera = null;
		// シーン切り替え
		{
			// resultSceneにはBGMそのままで
			//SoundController.Instance.StopBGM();

			if(resultInfo.MemberList.Count > 0 && resultInfo.IsPlayerInfo)
			{
				ResultMain.LoadScene(resultInfo);
			}
			else
			{
				// リザルト時に使用する情報取得に失敗している場合はロビーシーンに移す
				NextLobby();
			}
		}
	}
	public void NextLobby()
	{
		// デバッグログ非表示
		GUIDebugLog.Close();

		// ロード画面表示
		GUILoading.SetActive(true);

		// BGM停止
		SoundController.StopBGM();

		// データ削除
		NetworkController.DestroyBattle();

		// シーン切り替え
		LobbyPacket.SendEnterLobby();
	}
	#endregion

	#region チーム設定
	public void SetTeamType(TeamType teamType)
	{
		this.PlayerTeamType = teamType;
	}
	#endregion
	
	#region 勝敗パケット
	public static void Judge(JudgeType judgeType, bool hasNextMatch)
	{
        // TODO: process hasNextMatch
        if (Instance == null)
			return;

		// BGM
		JudgeTypeClient judgeTypeClient = judgeType.GetClientJudge();
		switch (judgeTypeClient)
		{
			// 自チーム勝利
			case JudgeTypeClient.PlayerWin:
			case JudgeTypeClient.PlayerCompleteWin:
				Sound.PlayWinBgm();
				break;
			// 自チーム敗北
			case JudgeTypeClient.PlayerLose:
			case JudgeTypeClient.PlayerCompleteLose:
				Sound.PlayLoseBgm();
				break;
			// 引き分け
			case JudgeTypeClient.Draw:
				Sound.PlayDrawBgm();
				break;
		}

		// 勝敗エフェクトメッセージセット
		GUIEffectMessage.SetJudge(judgeTypeClient);

        if (hasNextMatch) {
            GUINextRoundInfo.Show(true);
            return;
        }

		// リザルトパケット送信
		Instance.SendGameResultPacket();
	}

	void SendGameResultPacket()
	{
		if (Scm.Client.GameListener.ConnectFlg)
		{
			BattlePacket.SendGameResult();
		}
		else
		{
			BridgingResultInfo resultInfo = new BridgingResultInfo();
			resultInfo.Setup(NetworkController.ServerValue.InFieldId, JudgeType.Unknown, null, null);

			BattleMain.GameResult(resultInfo);
		}
	}
	#endregion

	#region リザルトパケット
	public static void GameResult(BridgingResultInfo bridgingResultInfo)
	{
		// リザルトシーンへ移行する
		if (Instance != null)	// 既にBattleシーンでない場合(自力で出るなど)のエラー対策.
		{
			Instance.StartCoroutine(Instance.GameResultCoroutine(Instance.resultChangeTime, bridgingResultInfo));
		}
	}
	IEnumerator GameResultCoroutine(float timer, BridgingResultInfo resultInfo)
	{
		if (0f < timer)
			yield return new WaitForSeconds(timer);

		// 切断後リザルトシーンへ
		Instance.ExitField(resultInfo);
	}
	#endregion

	#region バトルUIリセット
	public static void UIReset()
	{
		if (Instance == null)
			return;

		// リスポーン表示オフ
		GUIRespawnInfo.SetRespawnTime(0f, 0f);
		// トランスポーター表示オフ
		GUITransporterInfo.SetTimer(0f, 0f);
		// クールタイム初期化
		GUISkill.ClearCoolTime();
	}
    #endregion

    #region Helper Functions

    /// <summary>
    /// Can be called before start
    /// </summary>
    /// <returns></returns>
    public static TeamType GetPlayerTeamType() {
        var serverValue = NetworkController.ServerValue;
        if (serverValue != null) {
            return serverValue.TeamType;
        }
        return Instance.PlayerTeamType;
    }

    public static string GetRuleInfo(TeamType teamType, Scm.Common.Master.BattleRuleMasterData ruleData) {
        if (teamType == TeamType.Red && (!string.IsNullOrEmpty(ruleData.RedInfo))) {
            return ruleData.RedInfo;
        }
        return ruleData.Info;
    }

    public static string GetRuleMessage(TeamType teamType, Scm.Common.Master.BattleRuleMasterData ruleData) {
        if (teamType == TeamType.Red && (!string.IsNullOrEmpty(ruleData.RedMessage))) {
            return ruleData.RedMessage;
        }
        return ruleData.Message;
    }

    #endregion
}
