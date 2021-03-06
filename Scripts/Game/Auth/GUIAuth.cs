/// <summary>
/// 認証
/// 
/// 2015/03/18
/// </summary>

// 接続先を選択したことにして開発サーバーに必ず繋ぐようにするか
#if UNITY_ANDROID && (!UNITY_EDITOR)
#define SERVERSELECT_SKIP
#endif

// AsobimoIDとTokenを強制的に書き換える
//#define ASOBIMO_ID_TOKEN_REWRITE

// XIGNCODEをオフにする
//#define XIGNCODE_OFF

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIAuth : Singleton<GUIAuth>
{
    public class AuthState
    {
        public bool Finished;
        public bool Success;
    }
    #region フィールド＆プロパティ
    const string TitleColorCode = "[00FFFF]";
    const string ProgressColorCode = "[FFFF00]";
    const string SuccessColorCode = "[00FF00]";
    const string FailureColorCode = "[FF0000]";
    const string ValueColorCode = "[FFFF00]";

#if ASOBIMO_ID_TOKEN_REWRITE
	// Nexus5
	const string RewriteAsobimoID = "HLphLFckNv";
	const string RewriteAsobimoToken = "7376f53da6ae49a78c61550dbb4a5810";
	// Nexus7 2013
	//const string RewriteAsobimoID = "gQkjhHtFc4";
	//const string RewriteAsobimoToken = "a2e0739de4f9fdf039cdcbd0c5a005ba";
#endif

#if !XW_DEBUG
	// 
	private const string AndroidMarketURL = "market://details?id=com.asobimo.xworld_official";

	private const string AppStoreURL = "itms-apps://itunes.apple.com/app/idXXXXXXX";
#else
    // 
    private const string AndroidMarketURL = "market://details?id=com.asobimo.xworld_debug";

    private const string AppStoreURL = "itms-apps://itunes.apple.com/app/idXXXXXXX";
#endif

    /// <summary>
    /// 初期化時の状態
    /// </summary>
    [SerializeField]
    Statement _startState = Statement.Start;
    Statement StartState { get { return _startState; } }

    /// <summary>
    /// プログレスバーの表示フォーマット
    /// </summary>
    [SerializeField]
    string _progressFormat = "{0:0.00}%";
    string ProgressFormat { get { return _progressFormat; } }

    /// <summary>
    /// アタッチオブジェクト
    /// </summary>
    [SerializeField]
    AttachObject _attach = null;
    AttachObject Attach { get { return _attach; } }
    [System.Serializable]
    public class AttachObject
    {
        [SerializeField]
        UIPlayTween _rootTween = null;
        public UIPlayTween RootTween { get { return _rootTween; } }

        [SerializeField]
        UILabel _progressTitleLabel = null;
        public UILabel ProgressTitleLabel { get { return _progressTitleLabel; } }

        [SerializeField]
        UISlider _progressSlider = null;
        public UISlider ProgressSlider { get { return _progressSlider; } }

        [SerializeField]
        UILabel _progressLabel = null;
        public UILabel ProgressLabel { get { return _progressLabel; } }

        [SerializeField]
        StateActiveGroup _stateActive = null;
        public StateActiveGroup StateActive { get { return _stateActive; } }
        [System.Serializable]
        public class StateActiveGroup
        {
            [SerializeField]
            GameObject _progressGroup = null;
            public GameObject ProgressGroup { get { return _progressGroup; } }
        }
    }

    // ステート
    Statement _state;
    Statement State
    {
        get { return _state; }
        set
        {
            _state = value;
            switch (_state)
            {
                case Statement.None: break;
                case Statement.Start: this.SetStart(); break;
                case Statement.Host: this.SetHost(); break;
                case Statement.Logo: this.SetLogo(); break;
                case Statement.Auth: this.SetAuth(); break;
                case Statement.WebAPICheck: this.SetWebAPICheck(); break;
                case Statement.CheckFileDL: this.SetCheckFileDL(); break;
                case Statement.Maintenance: this.SetCheckMaintenance(); break;
                case Statement.AppliVersion: this.SetAppliVersion(); break;
                case Statement.MasterDataDL: this.SetMasterDataDL(); break;
                case Statement.AssetBundleDL: this.SetAssetBundleDL(); break;
                case Statement.Advertise: this.SetAdvertise(); break;
                case Statement.Browser: this.SetBrowser(); break;
                case Statement.End: this.SetEnd(); break;
                default:
                    Debug.LogError("GUIAuth:Invalid State");
                    break;
            }
        }
    }
    public enum Statement
    {
        None,				// 非表示

        Start,				// 開始処理
        Host,				// 接続先選択
        Logo,				// ロゴ表示

        Auth,				// アソビモ認証
        WebAPICheck,		// WebAPI から各種設定取得
        CheckFileDL,		// チェックファイルDL
        MasterDataDL,		// マスターデータDL

        Maintenance,		// メンテナンス表示
        AppliVersion,		// アプリバージョンチェック表示

        AssetBundleDL,		// アセットバンドルDL

        Advertise,			// 広告処理
        Browser,			// 初回ブラウザ表示

        End,				// 終了処理
    }

    // 更新用ファイバー
    IEnumerator ProcFiber { get; set; }
    // 次の遷移に移るフラグ
    bool IsNextScene { get; set; }

    // マスターデータバージョン
    MasterDataVersions MasterDataVersions { get; set; }
    // アセットバンドルバージョン
    AssetBundleVersions AssetBundleVersions { get; set; }

    // TODO:デバッグ用アソビモID入力受け取り用
    string DebugAsobimoID { get; set; }
    // シリアライズされていないメンバーの初期化
    void MemberInit()
    {
        this.State = Statement.None;

        this.ProcFiber = null;
        this.IsNextScene = false;

        this.MasterDataVersions = new MasterDataVersions();
        this.AssetBundleVersions = new AssetBundleVersions();

        this.DebugAsobimoID = "";
    }

    // プログレス
    float Progress
    {
        get { return (this.Attach.ProgressSlider != null ? this.Attach.ProgressSlider.value : 0f); }
        set { if (this.Attach.ProgressSlider != null) this.Attach.ProgressSlider.value = value; }
    }
    // プログレスのテキスト
    string ProgressText
    {
        get { return (this.Attach.ProgressLabel != null ? this.Attach.ProgressLabel.text : ""); }
        set { if (this.Attach.ProgressLabel != null) this.Attach.ProgressLabel.text = value; }
    }
    // プログレスバーのタイトル
    string ProgressTitle
    {
        get { return (this.Attach.ProgressTitleLabel != null ? this.Attach.ProgressTitleLabel.text : ""); }
        set { if (this.Attach.ProgressTitleLabel != null) this.Attach.ProgressTitleLabel.text = value; }
    }
    // プログレススライダーのアクティブ設定
    bool ProgressSliderActive
    {
        get { return (this.Attach.ProgressSlider != null ? this.Attach.ProgressSlider.gameObject.activeSelf : false); }
        set { if (this.Attach.ProgressSlider != null) this.Attach.ProgressSlider.gameObject.SetActive(value); }
    }

    private bool confirmedDownload = false;

    #endregion

    #region 初期化
    /// <summary>
    /// 初期化
    /// </summary>
    override protected void Awake()
    {
        base.Awake();
        this.MemberInit();

        // 表示設定
        SetState(this.StartState);
    }
    #endregion

    #region ステート設定
    /// <summary>
    /// 閉じる
    /// </summary>
    public static void Close()
    {
        if (Instance != null) Instance.SetWindowActive(Statement.None);
    }
    /// <summary>
    /// ステート設定(詳細設定)
    /// </summary>
    public static void SetState(Statement state)
    {
        if (Instance != null) Instance._SetState(state);
    }
    /// <summary>
    /// ステート設定(大元)
    /// </summary>
    void _SetState(Statement state)
    {
        // ウィンドウアクティブ設定
        this.SetWindowActive(state);

        // ステートごとのアクティブ設定
        var s = this.Attach.StateActive;
        switch (this.State)
        {
            //case Statement.Start:
            case Statement.Logo:
            case Statement.None:
                this.SetStateActive(null);
                break;
            default:
                this.SetStateActive(s.ProgressGroup);
                break;
        }
    }
    /// <summary>
    /// ウィンドウアクティブ設定
    /// </summary>
    void SetWindowActive(Statement state)
    {
        this.State = state;

        var isActive = state != Statement.None;

        // アクティブ化
        if (this.Attach.RootTween != null)
            this.Attach.RootTween.Play(isActive);
        else
            this.gameObject.SetActive(isActive);
    }
    /// <summary>
    /// ステートごとのアクティブ設定
    /// </summary>
    /// <param name="activeGroup"></param>
    void SetStateActive(GameObject activeGroup)
    {
        var s = this.Attach.StateActive;
        var list = new List<GameObject>();
        list.Add(s.ProgressGroup);
        foreach (var t in list)
        {
            if (t == null)
                continue;
            var isActive = (activeGroup == t);
            t.SetActive(isActive);
        }
    }
    #endregion

    #region 更新
    void Update()
    {
        try
        {
            if (this.ProcFiber != null)
                this.ProcFiber.MoveNext();
        }
        catch (System.Exception e)
        {
            string msg = string.Format("State={0}\r\n{1}", this.State, e);
            BugReportController.SaveLogFile(msg);
            Debug.LogError(msg);
            GUIDebugLog.AddMessage(msg);
            GUISystemMessage.SetModeYesNo(
                MasterData.GetTextDefalut(TextType.TX155_Error_ScreenTitle, ObsolateSrc.Defalut_TX155_Error_ScreenTitle),
                MasterData.GetTextDefalut(TextType.TX156_Error, ObsolateSrc.Defalut_TX156_Error),
                MasterData.GetTextDefalut(TextType.TX159_Common_RetryButton, ObsolateSrc.Defalut_TX159_Common_RetryButton),
                MasterData.GetTextDefalut(TextType.TX158_Common_QuitButton, ObsolateSrc.Defalut_TX158_Common_QuitButton),
                this.SetStart,
                Application.Quit);
        }
    }
    #endregion

    #region NGUIリフレクション
    public void OnNext()
    {
        this.IsNextScene = true;
    }
    #endregion

    #region Statement.Start 開始処理
    void SetStart()
    {
        // 次のステートへ
        this.SetStartNextState();
    }
    void SetStartNextState()
    {
        // 次のステートへ
        this._SetState((Statement)(Statement.Start + 1));
    }
    #endregion

    #region Statement.Host 接続先選択
    void SetHost()
    {
        GUIDebugLog.SetActive(true);
        GUIDebugLog.AddMessage(TitleColorCode + "[b]【接続先選択】[/b]");

#if XW_DEBUG
        bool isProcess = true;
#else
		bool isProcess = false;
#endif
        if (isProcess)
        {
            this.ProcFiber = this.HostCoroutine();
        }
        else
        {
            GUIDebugLog.AddMessage(string.Format("...{0}スキップ[-]", SuccessColorCode));
            this.SetHostNextScate();
        }
    }
    void SetHostNextScate()
    {
        // 次のステートへ
        this._SetState((Statement)(Statement.Host + 1));
    }
    IEnumerator HostCoroutine()
    {
#if XW_DEBUG
        // プログレスバー情報設定
        this.ProgressTitle = MasterData.GetTextDefalut(TextType.TX147_Auth_Progress, ObsolateSrc.Defalut_TX147_Auth_Progress);
        this.ProgressSliderActive = false;

        // 接続先を選択するかどうか
        GUIDebugLog.SetActive(false);
        bool isSelect = false;
#if SERVERSELECT_SKIP
		isSelect = true;
#else

        GUIMessageWindow.SetModeYesNo(
            MasterData.GetText(TextType.TX568_LoginMessage).Replace("\\n", "\n"),
            MasterData.GetText(TextType.TX057_Common_YesButton).Replace("\\n", "\n"),
            MasterData.GetText(TextType.TX058_Common_NoButton).Replace("\\n", "\n"),
            true, GUIMessageWindow.GuideMode.None,
            () => { this.IsNextScene = true; isSelect = true; },
            () => { this.IsNextScene = true; isSelect = false; }
            );
        this.IsNextScene = false;
        while (!this.IsNextScene)
            yield return null;
#endif
        GUIDebugLog.AddMessage(string.Format("接続先選択：{0}", isSelect ? "する" : "しない"));

        // 接続先選択コルーチン
        if (isSelect)
        {
            var fiber = this.HostSelectCoroutine();
            while (fiber.MoveNext())
                yield return null;
        }
#else
		yield return null;
#endif

        // 次のステートへ
        this.SetHostNextScate();
    }
#if XW_DEBUG
    IEnumerator HostSelectCoroutine()
    {
        // 接続先ボタンを押すまで待機
        GUIDebugLog.SetActive(true);
#if SERVERSELECT_SKIP
		ScmParam.Debug.File.Host = ObsolateSrc.DevelopmentHost;
#else
        GUIDebugLog.ActiveHostButton(
            (int result) =>
            {
                this.IsNextScene = true;
                switch (result)
                {
                    case 0:
                        ScmParam.Debug.File.Host = ObsolateSrc.BetaGameServerHost;
                        ScmParam.Debug.File.Environment = Scm.Common.GameParameter.EnvironmentType.BetaTest;
                        break;
                    case 1:
                        ScmParam.Debug.File.Host = ObsolateSrc.TestGameServerHost;
                        ScmParam.Debug.File.Environment = Scm.Common.GameParameter.EnvironmentType.Develop;
                        break;
                    case 2:
                        ScmParam.Debug.File.Host = ObsolateSrc.PersonalGameServerHost;
                        ScmParam.Debug.File.Environment = Scm.Common.GameParameter.EnvironmentType.Personal;
                        break;
                }
            });
        this.IsNextScene = false;
        while (!this.IsNextScene)
            yield return null;
#endif
        GUIDebugLog.AddMessage(string.Format("Host：{0}", ScmParam.Debug.File.Host));
        GUIDebugLog.DeactiveHostButton();
        GUIDebugLog.SetScroll(0f, 1f);
        GUIDebugLog.SetActive(false);

        int characterLimit = GUIMessageWindow.InputCharacterLimit;
#if SERVERSELECT_SKIP
        this.DebugAsobimoID = ScmParam.Debug.File.AsobimoID;
        if (string.IsNullOrEmpty(this.DebugAsobimoID)) {
            this.DebugAsobimoID = "#" + RandomGenerator.Next().ToString("D9").Substring(0, 9);
            yield return null;
        }
#else
        // アソビモID入力
        bool isContinue = true;
        GUIMessageWindow.InputCharacterLimit = 10;
        while (isContinue)
        {
            this.DebugAsobimoID = ScmParam.Debug.File.AsobimoID;
            GUIMessageWindow.SetModeInput(
                "半角英数字10文字のIDを入力してください",
                ScmParam.Debug.File.AsobimoID,
                "半角英数字10文字のIDを入力してください", "OK", "OK", false, GUIMessageWindow.GuideMode.None,
                () => { this.CheckAsobimoID(ref isContinue); },
                () => { this.CheckAsobimoID(ref isContinue); },
                (text) => { this.DebugAsobimoID = text; },
                null
                );
            this.IsNextScene = false;
            while (!this.IsNextScene)
                yield return null;
            GUIMessageWindow.Close();

            if (isContinue)
            {
                GUISystemMessage.SetModeOK(
                    "Error", "半角英数字10文字で入力してください",
                    "OK",
                    () => { this.IsNextScene = true; }
                    );
                this.IsNextScene = false;
                while (!this.IsNextScene)
                    yield return null;
            }
        }
#endif
        GUIMessageWindow.InputCharacterLimit = characterLimit;
        ScmParam.Debug.File.AsobimoID = this.DebugAsobimoID;
        ScmParam.Debug.File.IsDebugMode = true;
        GUIDebugLog.AddMessage(string.Format("AsobimoID：{0}", this.DebugAsobimoID));
    }
#endif
#if XW_DEBUG
    void CheckAsobimoID(ref bool isContinue)
    {
        this.IsNextScene = true;
        if (this.DebugAsobimoID.Length == 10 || this.DebugAsobimoID.Length == 0)
        {
            isContinue = false;
        }
        else
        {
            isContinue = true;
        }
    }
#endif
    #endregion

    #region Statement.Logo ロゴ表示
    void SetLogo()
    {
        this.ProcFiber = this.LogoCoroutine();
    }
    void SetLogoNextState()
    {
        // デバッグウィンドウを所定の位置で開く
        GUIDebugLog.AddMessage(TitleColorCode + "[b]【各種情報】[/b]");
        GUIDebugLog.SetOKButton("次へ", () => { this.IsNextScene = true; });
        GUIDebugLog.SetActive(true, 235, 50, 810, 455);
#if XW_DEBUG
        GUIDebugLog.AddMessage(string.Format("接続先：{1}({0})", ScmParam.ConnectHost,
            ScmParam.ConnectHost == ObsolateSrc.TestGameServerHost ?
                "開発サーバー" :
                ScmParam.ConnectHost == ObsolateSrc.PersonalGameServerHost ?
                    "個人テストサーバー" :
                    ScmParam.ConnectHost == ObsolateSrc.BetaGameServerHost ?
                        "[FF0000]公開サーバー[-]" :
                        "その他のサーバー"));
#endif
        GUIDebugLog.AddMessage(string.Format("MD：{0}", this.MasterDataVersions.URL));
        GUIDebugLog.AddMessage(string.Format("AB：{0}", this.AssetBundleVersions.URL));

        // 次のステートへ
        this._SetState((Statement)(Statement.Logo + 1));
    }
    IEnumerator LogoCoroutine()
    {
//#if EJPL
//        //跳过自己的splash
//        yield return null;
//#else
        GUILogo.Open();
        while (!GUILogo.IsFinish)
        {
            yield return null;
        }
        GUILogo.Close();
//#endif
        // 次のステートへ
        this.SetLogoNextState();
    }
    IEnumerator LogoTimerCoroutine(float time)
    {
        this.IsNextScene = false;
        while (time > Time.time)
        {
            if (this.IsNextScene)
                break;
            yield return null;
        }
    }
    #endregion

    #region Statement.Auth アソビモ認証
    void SetAuth()
    {
        GUIDebugLog.SetActive(true);
        GUIDebugLog.AddMessage(TitleColorCode + "[b]【アソビモ認証】[/b]");

#if UNITY_EDITOR || UNITY_STANDALONE
        bool isProcess = true;
#elif UNITY_ANDROID
		bool isProcess = true;
#elif UNITY_IPHONE
		bool isProcess = true;
#else
		bool isProcess = true;
#endif

#if ANDROID_XY
        //LWZ:XIAOYOU auth
        if (isProcess)
        {
            this.ProcFiber = this.AuthCoroutine();
        }
        else
        {
            GUIDebugLog.AddMessage(string.Format("...{0}スキップ[-]", SuccessColorCode));
            this.SetAuthNextScate();
        }
#else
        if (isProcess)
        {
            this.ProcFiber = this.AuthCoroutine();
        }
        else
        {
            GUIDebugLog.AddMessage(string.Format("...{0}スキップ[-]", SuccessColorCode));
            this.SetAuthNextScate();
        }
#endif

    }

    void SetAuthNew()
    {

    }

    void SetAuthNextScate()
    {
        // 次のステートへ
        this._SetState((Statement)(Statement.Auth + 1));
    }

    IEnumerator AuthCoroutine()
    {
#if !UNITY_EDITOR && ANDROID_XY
        GUIDebugLog.AddMessage(""); // ここで追加したメッセージをループ処理内で内容を書き換える

        // プログレスバー情報設定
        this.ProgressTitle = MasterData.GetTextDefalut(TextType.TX147_Auth_Progress, ObsolateSrc.Defalut_TX147_Auth_Progress);
        this.ProgressSliderActive = false;

        //TalkingData Init
        AndroidTalkingDataSDKController.Instance.Init("xiaoyoupf");

        // GoogleID選択
        PluginController.AuthenticateGoogleID(null, true, false);

        // GoogleID取得中...
        bool isSuccess = true;
        while (true)
        {
            bool isFinish = false;

            // 失敗
            if (PluginController.AuthInfo.state >= AuthInfo.AuthState.STATE_ERROR_MIN)
            {
                isFinish = true;
                isSuccess = false;
            }
            // 成功
            else if (PluginController.AuthInfo.state == AuthInfo.AuthState.STATE_COMPLETE)
            {
                isFinish = true;
                isSuccess = true;
            }

            // 直前に追加したメッセージを書き換える
            GUIDebugLog.ChangeMessage(string.Format(
                "asobimoID : " + ValueColorCode + "{0}[-]\r\n" +
                "asobimoToken : " + ValueColorCode + "{1}[-]\r\n" +
                "authType : " + ValueColorCode + "{2}({3})[-]\r\n" +
                "authID : " + ValueColorCode + "{4}[-]\r\n" +
                "state : {6}{5}[-]",
                PluginController.AuthInfo.openID,
                PluginController.AuthInfo.token,
                PluginController.AuthInfo.authCreateType, (int)PluginController.AuthInfo.authCreateType,
                PluginController.AuthInfo.authID,
                PluginController.AuthInfo.state,
                (!isFinish ?
                    ProgressColorCode : // 終了していない時の色
                    isSuccess ?
                        SuccessColorCode :  // 成功した時の色
                        FailureColorCode    // 失敗した時の色
                )));

            if (isFinish)
                break;
            yield return null;
        }

#if XW_DEBUG
		// OKボタンを押すまで待つ
		var waitOK = this.WaitOKCoroutinue();
		while (waitOK.MoveNext())
			yield return null;
#endif

        if (!isSuccess)
        {
            var errorMessage = string.Format("{0}\r\nErrorCode:{1}",
                MasterData.GetTextDefalut(TextType.TX148_Auth_Error, ObsolateSrc.Defalut_TX148_Auth_Error),
                (int)PluginController.AuthInfo.state);
            GUISystemMessage.SetModeYesNo(
                MasterData.GetTextDefalut(TextType.TX155_Error_ScreenTitle, ObsolateSrc.Defalut_TX155_Error_ScreenTitle),
                errorMessage,
                MasterData.GetTextDefalut(TextType.TX159_Common_RetryButton, ObsolateSrc.Defalut_TX159_Common_RetryButton),
                MasterData.GetTextDefalut(TextType.TX158_Common_QuitButton, ObsolateSrc.Defalut_TX158_Common_QuitButton),
                this.SetAuth,
                Application.Quit);
            yield break;
        }

        // ゲームサーバーに認証パケットを送信する
        AuthRequest.StartForceRequest();

        // 次のステートへ
        this.SetAuthNextScate();

#else
        GUIDebugLog.AddMessage("AuthCoroutine");	// ここで追加したメッセージをループ処理内で内容を書き換える
        Debug.Log("===> AuthCoroutine");
        // プログレスバー情報設定
        this.ProgressTitle = MasterData.GetTextDefalut(TextType.TX147_Auth_Progress, ObsolateSrc.Defalut_TX147_Auth_Progress);
        this.ProgressSliderActive = false;

        //		// GoogleID選択
        //		PluginController.AuthenticateGoogleID(null, true, false);

        // GoogleID取得中...
        AuthState state = new AuthState();
        state.Success = true;
        state.Finished = false;
#if XW_DEBUG
        state.Success = true;
        state.Finished = true;
//#elif UNITY_IOS
//        state.Success = true;
//        state.Finished = true;
#elif EJPL
        while (null == APaymentHelperDemo.Instance)
        {
            Debug.Log("===> APaymentHelperDemo.Instance Waiting");
            yield return new WaitForSeconds(0.5f);
        }

#if !UNITY_EDITOR
        APaymentHelperDemo.Instance.DoLogin();

        while (!APaymentHelperDemo.bLogined)
        {
            Debug.Log("===> Waiting Login");
            yield return new WaitForSeconds(0.5f);
        }

        while ("" == APaymentHelperDemo.Uid)
        {
            Debug.Log("===> Waiting UID");
            yield return new WaitForSeconds(0.5f);
        }
#endif

#if !UNITY_EDITOR
        StartCoroutine(UnityXYManager.Instance.CoroutinePost_GetToken(APaymentHelperDemo.Uid, UnityXYManager.Instance.OnResponseGetToken, state));
#else
        PlayerPrefs.SetString("XYPlatformCode", "ch360");
        StartCoroutine(UnityXYManager.Instance.CoroutinePost_GetToken("360u_1335071296", UnityXYManager.Instance.OnResponseGetToken, state));
#endif
        while (!state.Finished)
        {
            Debug.Log("===> Waiting");
            yield return new WaitForSeconds(0.5f);
        }
#else
        LoginFrame.Instance.SetDetail(state);
        while (!state.Finished)
        {
            //Debug.Log("===> Waiting");
            yield return new WaitForSeconds(0.5f);
        }
        LoginFrame.Instance.Close();
        //        state.Finished = true;
        //        state.Success = true;
        Debug.LogError("===>" + state.Finished + "  " + state.Success);
#endif
        //			// 失敗
        //			if (PluginController.AuthInfo.state >= AuthInfo.AuthState.STATE_ERROR_MIN)
        //			{
        //				isFinish = true;
        //				isSuccess = false;
        //			}
        //			// 成功
        //			else if (PluginController.AuthInfo.state == AuthInfo.AuthState.STATE_COMPLETE)
        //			{
        //				isFinish = true;
        //				isSuccess = true;
        //			}
        //
        //			// 直前に追加したメッセージを書き換える
        //			GUIDebugLog.ChangeMessage(string.Format(
        //				"asobimoID : " + ValueColorCode + "{0}[-]\r\n" +
        //				"asobimoToken : " + ValueColorCode + "{1}[-]\r\n" +
        //				"authType : " + ValueColorCode + "{2}({3})[-]\r\n" +
        //				"authID : " + ValueColorCode + "{4}[-]\r\n" +
        //				"state : {6}{5}[-]",
        //				PluginController.AuthInfo.openID,
        //				PluginController.AuthInfo.token,
        //				PluginController.AuthInfo.authCreateType, (int)PluginController.AuthInfo.authCreateType,
        //				PluginController.AuthInfo.authID,
        //				PluginController.AuthInfo.state,
        //				(!isFinish ?
        //					ProgressColorCode :	// 終了していない時の色
        //					isSuccess ?
        //						SuccessColorCode :	// 成功した時の色
        //						FailureColorCode	// 失敗した時の色
        //				)));

#if XW_DEBUG
        // OKボタンを押すまで待つ
        var waitOK = this.WaitOKCoroutinue();
        while (waitOK.MoveNext())
            yield return null;
#endif

        if (!state.Success)
        {
            var errorMessage = string.Format("{0}\r\nErrorCode:{1}",
                MasterData.GetTextDefalut(TextType.TX148_Auth_Error, ObsolateSrc.Defalut_TX148_Auth_Error),
                (int)PluginController.AuthInfo.state);
            GUISystemMessage.SetModeYesNo(
                MasterData.GetTextDefalut(TextType.TX155_Error_ScreenTitle, ObsolateSrc.Defalut_TX155_Error_ScreenTitle),
                errorMessage,
                MasterData.GetTextDefalut(TextType.TX159_Common_RetryButton, ObsolateSrc.Defalut_TX159_Common_RetryButton),
                MasterData.GetTextDefalut(TextType.TX158_Common_QuitButton, ObsolateSrc.Defalut_TX158_Common_QuitButton),
                this.SetAuth,
                Application.Quit);
            yield break;
        }

        // ゲームサーバーに認証パケットを送信する
        AuthRequest.StartForceRequest();

        // 次のステートへ
        this.SetAuthNextScate();
#endif
    }
    #endregion

    #region Statement.WebAPICheck WebAPIアクセス
    void SetWebAPICheck()
    {
        GUIDebugLog.SetActive(true);
        GUIDebugLog.AddMessage(TitleColorCode + "[b]【WebAPIアクセス】[/b][-]");

#if ASOBIMO_ID_TOKEN_REWRITE
		// デバッグ用 AsobimoID と AsobimoToken を書き換える
		string asobimoID = RewriteAsobimoID;
		string asobimoToken = RewriteAsobimoToken;
		if (Asobimo.Auth.AuthManager.Instance != null)
		{
			PluginController.AuthInfo.asobimoID = asobimoID;
			PluginController.AuthInfo.asobimoToken = asobimoToken;
#if XW_DEBUG
			Asobimo.Auth.AuthManager.Instance.AsobimoAuth.SetAsobimoData(asobimoID, asobimoToken);
#endif
		}
#endif
#if UNITY_EDITOR || UNITY_STANDALONE
        bool isProcess = false;
#elif UNITY_ANDROID
		bool isProcess = true;
#elif UNITY_IPHONE
		bool isProcess = true;
#else
		bool isProcess = false;
#endif
        if (isProcess)
        {
            this.ProcFiber = this.WebAPICheckCoroutine();
        }
        else
        {
            GUIDebugLog.AddMessage(string.Format("...{0}スキップ[-]", SuccessColorCode));
            this.SetWebAPICheckNextState();
        }
    }
    void SetWebAPICheckNextState()
    {
        // 次のステートへ
        this._SetState((Statement)(Statement.WebAPICheck + 1));
    }
    IEnumerator WebAPICheckCoroutine()
    {
        // プログレスバー情報設定
        this.ProgressTitle = MasterData.GetTextDefalut(TextType.TX147_Auth_Progress, ObsolateSrc.Defalut_TX147_Auth_Progress);
        this.ProgressSliderActive = false;

        GUIDebugLog.AddMessage(1, "");	// ここで追加したメッセージをループ処理内で内容を書き換える
        GUIAsobimoWeb.StartAccess();
        while (!GUIAsobimoWeb.IsFinish)
        {
            // 直前に追加したメッセージを書き換える
            GUIDebugLog.ChangeMessage(1, string.Format("WebAPI...{1}{0}[-]",
                GUIAsobimoWeb.AccessState,
                (!GUIAsobimoWeb.IsFinish ?
                    ProgressColorCode :	// 終了していない時の色
                    GUIAsobimoWeb.IsResult ?
                        SuccessColorCode :	// 成功した時の色
                        FailureColorCode	// 失敗した時の色
                )));
            yield return null;
        }
        // 直前に追加したメッセージを書き換える
        GUIDebugLog.ChangeMessage(1, string.Format("WebAPI...{0}{1}({2})[-]",
            GUIAsobimoWeb.IsResult ? SuccessColorCode : FailureColorCode,
            GUIAsobimoWeb.IsResult ? "Success" : "Failed",
            GUIAsobimoWeb.AccessState
            ));

        string errorMessage = "";
        // 失敗したかどうか
        if (!GUIAsobimoWeb.IsResult)
        {
            errorMessage = string.Format("{0}\r\nErrorCode:{1}a{2}",
                MasterData.GetTextDefalut(TextType.TX149_Web_AuthError, ObsolateSrc.Defalut_TX149_Web_AuthError),
                (int)GUIAsobimoWeb.AccessState,
                GUIAsobimoWeb.HttpStatus
                );
        }

#if XW_DEBUG
        // OKボタンを押すまで待つ
        var waitOK = this.WaitOKCoroutinue();
        while (waitOK.MoveNext())
            yield return null;
#endif

        if (!GUIAsobimoWeb.IsResult)
        {
            GUISystemMessage.SetModeYesNo(
                MasterData.GetTextDefalut(TextType.TX155_Error_ScreenTitle, ObsolateSrc.Defalut_TX155_Error_ScreenTitle),
                errorMessage,
                MasterData.GetTextDefalut(TextType.TX159_Common_RetryButton, ObsolateSrc.Defalut_TX159_Common_RetryButton),
                MasterData.GetTextDefalut(TextType.TX158_Common_QuitButton, ObsolateSrc.Defalut_TX158_Common_QuitButton),
                this.SetWebAPICheck,
                Application.Quit);
            yield break;
        }

        // 次のステートへ
        this.SetWebAPICheckNextState();
    }
    #endregion

    #region Statement.CheckFileDL チェックファイルダウンロード
    void SetCheckFileDL()
    {
        GUIDebugLog.SetActive(true);
        GUIDebugLog.AddMessage(TitleColorCode + "[b]【チェックファイルダウンロード】[/b][-]");

#if UNITY_EDITOR || UNITY_STANDALONE
        bool isProcess = true;
#elif UNITY_ANDROID
		bool isProcess = true;
#elif UNITY_IPHONE
		bool isProcess = true;
#else
		bool isProcess = true;
#endif
        if (isProcess)
        {
            this.ProcFiber = this.CheckFileDLCoroutine();
        }
        else
        {
            GUIDebugLog.AddMessage(string.Format("...{0}スキップ[-]", SuccessColorCode));
            this.SetCheckFileDLNextState();
        }
    }
    void SetCheckFileDLNextState()
    {
        // 次のステートへ
        this._SetState((Statement)(Statement.CheckFileDL + 1));
    }
    IEnumerator CheckFileDLCoroutine()
    {
        // マスターデータ周りのデータを非同期読み込み開始
        GUIDebugLog.AddMessage(string.Format("...MasterData バージョンチェックファイル非同期読み込み開始"));
        this.MasterDataVersions.Read();
        // アセットバンドル周りのデータを非同期読み込み開始
        GUIDebugLog.AddMessage(string.Format("...AssetBundle バージョンチェックファイル非同期読み込み開始"));
        this.AssetBundleVersions.Read();

        // ファイルダウンロード
        var dlList = new List<DownloadParam>()
        {
            //new DownloadParam(SettingFile.URL, SettingFile.Path, SettingFile.Filename),
        };

        GUIDebugLog.AddMessage(string.Format("NewFile : " + ValueColorCode + "{0:00}[-]", dlList.Count));

        // ファイルダウンロード
        var downloadSet = new DownloadSet();
        var downloadingList = new LinkedList<DownloadSet.DownloadFile>();
        foreach (var param in dlList)
        {
            downloadingList.AddLast(downloadSet.AddDownload_File(param.Uri, param.Path));

            GUIDebugLog.AddMessage(string.Format(ValueColorCode + "{0}[-]", param.Path));
        }


#if XW_DEBUG
        // OKボタンを押すまで待つ
        var waitOK = this.WaitOKCoroutinue();
        while (waitOK.MoveNext())
            yield return null;
#endif

        // ダウンロード中...
        var fiberSet = new FiberSet();
        fiberSet.AddFiber(this.FileDownloadCoroutine(downloadSet));
        while (fiberSet.Update())
            yield return null;

        // ファイルダウンロード成否
        {
            bool isSuccess = true;
            foreach (var param in downloadingList)
            {
                // 完了待ち
                while (!param.Completed)
                    yield return null;
                // 成否判定
                if (param.IsSuccess)
                    continue;
                isSuccess = false;
            }

            if (!isSuccess)
            {
                Debug.LogError("CheckFileDLCoroutine: Download fail");
                //SettingFile.Delete();
                GUISystemMessage.SetModeYesNo(
                    MasterData.GetTextDefalut(TextType.TX155_Error_ScreenTitle, ObsolateSrc.Defalut_TX155_Error_ScreenTitle),
                    MasterData.GetTextDefalut(TextType.TX151_Download_Error, ObsolateSrc.Defalut_TX151_Download_Error),
                    MasterData.GetTextDefalut(TextType.TX159_Common_RetryButton, ObsolateSrc.Defalut_TX159_Common_RetryButton),
                    MasterData.GetTextDefalut(TextType.TX158_Common_QuitButton, ObsolateSrc.Defalut_TX158_Common_QuitButton),
                    this.SetCheckFileDL,
                    Application.Quit);
                yield break;
            }
        }

        //// 設定ファイル読み込み
        //try
        //{
        //	SettingFile.Instance.Read();
        //}
        //catch (System.Exception e)
        //{
        //	Debug.LogError(e);
        //	GUISystemMessage.SetModeYesNo(
        //		MasterData.GetTextDefalut(TextType.TX155_Error_ScreenTitle, ObsolateSrc.Defalut_TX155_Error_ScreenTitle),
        //		MasterData.GetTextDefalut(TextType.TX151_Download_Error, ObsolateSrc.Defalut_TX151_Download_Error),
        //		MasterData.GetTextDefalut(TextType.TX159_Common_RetryButton, ObsolateSrc.Defalut_TX159_Common_RetryButton),
        //		MasterData.GetTextDefalut(TextType.TX158_Common_QuitButton, ObsolateSrc.Defalut_TX158_Common_QuitButton),
        //		this.SetCheckFileDL,
        //		Application.Quit);
        //}

        // 次のステートへ
        this.SetCheckFileDLNextState();
    }
    #endregion

    #region Statement.MasterDataDL マスターデータダウンロード
    void RetrySetMasterDataDL()
    {
        this.MasterDataVersions.Read();
        this.SetMasterDataDL();
    }
    void SetMasterDataDL()
    {
        GUIDebugLog.SetActive(true);
        GUIDebugLog.AddMessage(TitleColorCode + "[b]【マスターデータダウンロード】[/b][-]");

#if UNITY_EDITOR || UNITY_STANDALONE
        bool isProcess = true;
        //bool isProcess = false;
#elif UNITY_ANDROID
		bool isProcess = true;
#elif UNITY_IPHONE
		bool isProcess = true;
#else
		bool isProcess = true;
#endif
        if (isProcess)
        {
            this.ProcFiber = this.MasterDataDLCoroutine();
        }
        else
        {
            GUIDebugLog.AddMessage(string.Format("...{0}スキップ[-]", SuccessColorCode));
            this.SetMasterDataDLNextState();
        }
    }
    void SetMasterDataDLNextState()
    {
        // 次のステートへ
        this._SetState((Statement)(Statement.MasterDataDL + 1));
    }
    IEnumerator MasterDataDLCoroutine()
    {
        // バージョンチェック
        {
            // なぜかまだ読み込みを始めていなかった場合
            if (this.MasterDataVersions.ReadindState == AssetBundleVersions.ReadState.Non)
            {
                this.MasterDataVersions.Read();
                BugReportController.SaveLogFile("MasterDataVersions.ReadState is Non");
            }
            // 読み込みが終わっていない
            if (this.MasterDataVersions.ReadindState == AssetBundleVersions.ReadState.Reading)
            {
                GUIDebugLog.AddMessage("...MasterData バージョンチェックファイル読込中");
            }
            while (this.MasterDataVersions.ReadindState == AssetBundleVersions.ReadState.Reading)
            {
                yield return null;
            }
            // 読み込みエラー
            if (this.MasterDataVersions.ReadindState < AssetBundleVersions.ReadState.Non)
            {
                Debug.Log(this.MasterDataVersions.ReadindState);

                Debug.LogError("MasterDataDLCoroutine: Download fail");
                GUISystemMessage.SetModeYesNo(
                    MasterData.GetTextDefalut(TextType.TX155_Error_ScreenTitle, ObsolateSrc.Defalut_TX155_Error_ScreenTitle),
                    MasterData.GetTextDefalut(TextType.TX151_Download_Error, ObsolateSrc.Defalut_TX151_Download_Error),
                    MasterData.GetTextDefalut(TextType.TX159_Common_RetryButton, ObsolateSrc.Defalut_TX159_Common_RetryButton),
                    MasterData.GetTextDefalut(TextType.TX158_Common_QuitButton, ObsolateSrc.Defalut_TX158_Common_QuitButton),
                    this.RetrySetMasterDataDL,
                    Application.Quit);
                yield break;
            }
            // 成功しているはずだが一応チェック
            if (this.MasterDataVersions.ReadindState != AssetBundleVersions.ReadState.Success)
            {
                BugReportController.SaveLogFile("ReadindState is " + this.MasterDataVersions.ReadindState);
            }
        }

        // ファイルリスト作成
        var dlList = this.MasterDataVersions.GetDownLoadParam();
        GUIDebugLog.AddMessage(string.Format("NewFile : " + ValueColorCode + "{0:00}[-]", dlList.Count));

        // ファイルダウンロード
        var downloadSet = new DownloadSet();
        var downloadingList = new LinkedList<DownloadSet.DownloadFile>();
        foreach (var param in dlList)
        {
            downloadingList.AddLast(downloadSet.AddDownload_File(param.Uri, param.Path));

#if XW_DEBUG
            var versionData = this.MasterDataVersions;
            var fileName = System.IO.Path.GetFileName(param.Path);
            var oldVersion = versionData.GetOldVersion(fileName);
            var newVersion = versionData.GetNewVersion(fileName);
            GUIDebugLog.AddMessage(string.Format(
                "(Old:{0}/New:{1})" + ValueColorCode + "{2}[-]...{3}",
                oldVersion <= -1 ? "None" : oldVersion.ToString("0000"),
                newVersion.ToString("0000"),
                fileName,
                newVersion <= oldVersion ?
                    SuccessColorCode + "OK[-]" :
                    FailureColorCode + "New[-]"
                ));
#endif
        }

        // Warning for resource download
        if (downloadingList.Count > 0) {
            bool quit = false;
            GUISystemMessage.SetModeYesNo(
                        MasterData.GetTextDefalut(TextType.TX606_NetworkFeeWarningTitle, ObsolateSrc.Default_TX606_NetworkFeeWarningTitle),
                        MasterData.GetTextDefalut(TextType.TX606_NetworkFeeWarning, ObsolateSrc.Default_TX606_NetworkFeeWarning),
                        MasterData.GetTextDefalut(TextType.TX057_Common_YesButton, ObsolateSrc.Default_TX057_Common_YesButton),
                        MasterData.GetTextDefalut(TextType.TX058_Common_NoButton, ObsolateSrc.Default_TX058_Common_YesButton),
                        () => confirmedDownload = true,
                        () => { Application.Quit(); confirmedDownload = true; quit = true; });
            while (!confirmedDownload) {
                yield return null;
            }
            if (quit) {
                yield break;
            }
        }

#if XW_DEBUG
        // OKボタンを押すまで待つ
        var waitOK = this.WaitOKCoroutinue();
        while (waitOK.MoveNext())
            yield return null;
#endif

        // ダウンロード中...
        var fiberSet = new FiberSet();
        fiberSet.AddFiber(this.FileDownloadCoroutine(downloadSet));
        while (fiberSet.Update())
            yield return null;

        // ファイルダウンロード成否
        {
            bool isSuccess = true;
            foreach (var param in downloadingList)
            {
                // 完了待ち
                while (!param.Completed)
                    yield return null;
                // 成否判定
                if (param.IsSuccess)
                {
                    // CRCチェック(2度DLを防ぐため,1つ失敗してもチェックは継続)
                    if (this.MasterDataVersions.Check(param.FilePath))
                        continue;
                }
                isSuccess = false;
            }

            if (!isSuccess)
            {
                Debug.LogError("MasterDataDLCoroutine: Download fail2");
                GUISystemMessage.SetModeYesNo(
                    MasterData.GetTextDefalut(TextType.TX155_Error_ScreenTitle, ObsolateSrc.Defalut_TX155_Error_ScreenTitle),
                    MasterData.GetTextDefalut(TextType.TX151_Download_Error, ObsolateSrc.Defalut_TX151_Download_Error),
                    MasterData.GetTextDefalut(TextType.TX159_Common_RetryButton, ObsolateSrc.Defalut_TX159_Common_RetryButton),
                    MasterData.GetTextDefalut(TextType.TX158_Common_QuitButton, ObsolateSrc.Defalut_TX158_Common_QuitButton),
                    this.RetrySetMasterDataDL,
                    Application.Quit);
                yield break;
            }
        }

        // マスターデータの読み込み
        MasterData.Read();

#if !LVL_OFF && !XW_DEBUG
		bool isFinish = false;
		// LVLチェック
		PluginController.LVLCheck(
			(bool flag, int code) =>
			{
				// flag == false ライセンスエラー
				// code は 16進で表示

				if (flag == false)
				{
					BugReportController.SaveLogFile("LVLCheck:" + string.Format("code=0x{0:X8}", code));
					GUISystemMessage.SetModeOK(
						MasterData.GetText(TextType.TX155_Error_ScreenTitle),
						MasterData.GetText(TextType.TX156_Error) + string.Format("\r\ncode=0x{0:X8}", code),
						MasterData.GetText(TextType.TX158_Common_QuitButton),
						true,
						Application.Quit);
				}
				else
				{
					isFinish = true;
				}
			}
		);
		while (!isFinish)
		{
			yield return null;
		}
#endif

#if !XIGNCODE_OFF
        //		// Xigncode 初期化
        //		PluginController.XigncodeInitialize(
        //			(int code, string info) =>
        //			{
        //				BugReportController.SaveLogFile("Xigncode:" + string.Format("code=0x{0:X8} info={1} ", code, info));
        //				GUISystemMessage.SetModeOK(
        //					MasterData.GetText(TextType.TX155_Error_ScreenTitle),
        //					string.Format(MasterData.GetText(TextType.TX555_XigncodeHandleMessage), code, info),
        //					true, null);
        //			}
        //		);
#endif

        // 次のステートへ
        this.SetMasterDataDLNextState();
    }
    #endregion

    #region Statement.Maintenance メンテナンス表示
    void SetCheckMaintenance()
    {
        GUIDebugLog.SetActive(true);
        GUIDebugLog.AddMessage(TitleColorCode + "[b]【メンテナンス表示】[/b][-]");

#if UNITY_EDITOR || UNITY_STANDALONE
        bool isProcess = false;
#elif UNITY_ANDROID
		bool isProcess = true;
#elif UNITY_IPHONE
		bool isProcess = true;
#else
		bool isProcess = false;
#endif
        if (isProcess)
        {
            // メンテナンス情報取得
            bool isMaintenance = true;
            {
                if (GUIAsobimoWeb.IsFinish && GUIAsobimoWeb.IsResult)
                {
                    isMaintenance = GUIAsobimoWeb.IsGameMaintenance;
                }
                else
                {
                    Debug.LogWarning("メンテナンス情報が取得できない");
                    GUIDebugLog.AddMessage(FailureColorCode + "メンテナンス情報が取得できない[-]");
                }
            }

            GUIDebugLog.AddMessage(string.Format("メンテナンス状態...{0}",
                isMaintenance ?
                    FailureColorCode + "メンテナンス中[-]" :	// 失敗した時の色
                    SuccessColorCode + "オフ[-]"	// 成功した時の色
                ));

            // メンテナンス中...
            if (isMaintenance)
            {
                GUISystemMessage.SetModeOK(MasterData.GetText(TextType.TX154_Infomation_ScreenTitle), MasterData.GetText(TextType.TX160_Meintenance_QuitApp), MasterData.GetText(TextType.TX158_Common_QuitButton), Application.Quit);
                return;
            }
        }
        else
        {
            GUIDebugLog.AddMessage(string.Format("...{0}スキップ[-]", SuccessColorCode));
        }

        // 次のステートへ
        this.SetCheckMaintenanceNextState();
    }
    void SetCheckMaintenanceNextState()
    {
        // 次のステートへ
        this._SetState((Statement)(Statement.Maintenance + 1));
    }
    #endregion

    #region Statement.AppliVersion アプリバージョンチェック
    void SetAppliVersion()
    {
        GUIDebugLog.SetActive(true);
        GUIDebugLog.AddMessage(TitleColorCode + "[b]【アプリバージョンチェック】[/b][-]");

#if UNITY_EDITOR || UNITY_STANDALONE
        bool isProcess = false;
#elif UNITY_ANDROID
		bool isProcess = true;
#elif UNITY_IPHONE
		bool isProcess = true;
#else
		bool isProcess = false;
#endif
        if (isProcess)
        {
            var versionName = PluginController.PackageInfo.versionName1;
            var isUpgrade = !MasterData.IsValidVersion(versionName);

            GUIDebugLog.AddMessage(string.Format("アプリバージョンチェック..." + ValueColorCode + "{0}[-]...{1}",
                versionName,
                isUpgrade ?
                    FailureColorCode + "古いバージョン[-]" :	// 失敗した時の色
                    SuccessColorCode + "最新バージョン[-]"	// 成功した時の色
                ));

            // アプリバージョン更新メッセージ
            if (isUpgrade)
            {
                ApplicationUpgradeMessage();
                return;
            }
        }
        else
        {
            GUIDebugLog.AddMessage(string.Format("...{0}スキップ[-]", SuccessColorCode));
        }

        // 次のステートへ
        this.SetAppliVersionNextState();
    }
    // アプリ更新.
    static public void ApplicationUpgradeMessage()
    {
        GUISystemMessage.SetModeOK(
            MasterData.GetText(TextType.TX154_Infomation_ScreenTitle),
            MasterData.GetText(TextType.TX146_AppliVersion),
            MasterData.GetText(TextType.TX157_Common_UpgradeButton),
            false,
            ApplicationUpgrade);
    }

    static void ApplicationUpgrade()
    {
        // 各プラットフォームのURLスキーム記載

#if !UNITY_EDITOR && UNITY_ANDROID
		string distributionCode = PlayerPrefs.GetString("XYPlatformCode", "xiaoyoupf");
        Application.OpenURL(MasterData.TryGetAppMarketUrl(distributionCode));
#endif

#if !UNITY_EDITOR && UNITY_IOS
		Application.OpenURL(AppStoreURL);
#endif

        //#if !UNITY_EDITOR && UNITY_ANDROID
        //		PluginController.StartDLInstaller("http://rs-jp.xworld.jp/Android/x_world.apk");
        //#endif
    }
    void SetAppliVersionNextState()
    {
        // 次のステートへ
        this._SetState((Statement)(Statement.AppliVersion + 1));
    }
    #endregion

    #region Statement.AssetBundleDL アセットバンドルダウンロード
    void RetrySetAssetBundleDL()
    {
        this.AssetBundleVersions.Read();
        this.SetAssetBundleDL();
    }
    void SetAssetBundleDL()
    {
        GUIDebugLog.SetActive(true);
        GUIDebugLog.AddMessage(TitleColorCode + "[b]【アセットバンドルダウンロード】[/b][-]");

        bool isProcess = true;
#if UNITY_EDITOR && XW_DEBUG
        if (AssetReference.LoadFromLocalFolder)
        {
            // 設定が目に見えないモノなので現在のモードを告知する(開発者用).
            Debug.Log("リソースロードモード : ローカルフォルダ");
            // サウンド設定.
            SoundController.ReStart();
            // アセットバンドルを使わない.
            isProcess = false;
        }
        else
        {
            // 設定が目に見えないモノなので現在のモードを告知する(開発者用).
            Debug.Log("リソースロードモード : アセットバンドル");
        }
#endif

        GUIDebugLog.AddMessage(string.Format("リソースロードモード...{0}",
            isProcess ?
                SuccessColorCode + "アセットバンドル[-]" :	// 成功した時の色
                FailureColorCode + "ローカルフォルダ[-]"	// 失敗した時の色
            ));

        if (isProcess)
        {
            this.ProcFiber = this.AssetBundleDLCoroutine();
        }
        else
        {
            GUIDebugLog.AddMessage(string.Format("...{0}スキップ[-]", SuccessColorCode));
            this.SetAssetBundleDLNextState();
        }
    }
    void SetAssetBundleDLNextState()
    {
        // 次のステートへ
        this._SetState((Statement)(Statement.AssetBundleDL + 1));
    }
    IEnumerator AssetBundleDLCoroutine()
    {
        // バージョンチェック
        {
            // なぜかまだ読み込みを始めていなかった場合
            if (this.AssetBundleVersions.ReadindState == AssetBundleVersions.ReadState.Non)
            {
                this.AssetBundleVersions.Read();
                BugReportController.SaveLogFile("AssetBundleVersions.ReadState is Non");
            }
            // 読み込みが終わっていない
            if (this.AssetBundleVersions.ReadindState == AssetBundleVersions.ReadState.Reading)
            {
                GUIDebugLog.AddMessage("...AssetBundle バージョンチェックファイル読込中");
            }
            while (this.AssetBundleVersions.ReadindState == AssetBundleVersions.ReadState.Reading)
            {
                yield return null;
            }
            // 読み込みエラー
            if (this.AssetBundleVersions.ReadindState < AssetBundleVersions.ReadState.Non)
            {
                Debug.Log(this.AssetBundleVersions.ReadindState);
                Debug.LogError("AssetBundleDLCoroutine: Download fail");
                GUISystemMessage.SetModeYesNo(MasterData.GetText(TextType.TX155_Error_ScreenTitle), MasterData.GetText(TextType.TX151_Download_Error), MasterData.GetText(TextType.TX159_Common_RetryButton), MasterData.GetText(TextType.TX158_Common_QuitButton), this.RetrySetAssetBundleDL, Application.Quit);
                yield break;
            }
            // 成功しているはずだが一応チェック
            if (this.AssetBundleVersions.ReadindState != AssetBundleVersions.ReadState.Success)
            {
                BugReportController.SaveLogFile("ReadindState is " + this.AssetBundleVersions.ReadindState);
            }
        }

        // ファイルリスト作成
        var dlList = this.AssetBundleVersions.GetDownLoadParam();

        GUIDebugLog.AddMessage(string.Format("NewFile : " + ValueColorCode + "{0:00}[-]", dlList.Count));

        // ファイルダウンロード
        var downloadSet = new DownloadSet();
        var downloadingList = new LinkedList<DownloadSet.DownloadFile>();
        foreach (var param in dlList)
        {
            downloadingList.AddLast(downloadSet.AddDownload_File(param.Uri, param.Path));

#if XW_DEBUG
            var versionData = this.AssetBundleVersions;
            var fileName = System.IO.Path.GetFileName(param.Path);
            var oldVersion = versionData.GetOldVersion(fileName);
            var newVersion = versionData.GetNewVersion(fileName);
            GUIDebugLog.AddMessage(string.Format(
                "(Old:{0}/New:{1})" + ValueColorCode + "{2}[-]...{3}",
                oldVersion <= -1 ? "None" : oldVersion.ToString("0000"),
                newVersion.ToString("0000"),
                fileName,
                newVersion <= oldVersion ?
                    SuccessColorCode + "OK[-]" :
                    FailureColorCode + "New[-]"
                ));
#endif
        }

        // Warning for resource download
        if (downloadingList.Count > 0 && (!confirmedDownload)) {
            bool quit = false;
            GUISystemMessage.SetModeYesNo(
                        MasterData.GetTextDefalut(TextType.TX606_NetworkFeeWarningTitle, ObsolateSrc.Default_TX606_NetworkFeeWarningTitle),
                        MasterData.GetTextDefalut(TextType.TX606_NetworkFeeWarning, ObsolateSrc.Default_TX606_NetworkFeeWarning),
                        MasterData.GetTextDefalut(TextType.TX057_Common_YesButton, ObsolateSrc.Default_TX057_Common_YesButton),
                        MasterData.GetTextDefalut(TextType.TX058_Common_NoButton, ObsolateSrc.Default_TX058_Common_YesButton),
                        () => confirmedDownload = true,
                        () => { Application.Quit(); confirmedDownload = true; quit = true; });
            while (!confirmedDownload) {
                yield return null;
            }
            if (quit) {
                yield break;
            }
        }

#if XW_DEBUG
        // OKボタンを押すまで待つ
        var waitOK = this.WaitOKCoroutinue();
        while (waitOK.MoveNext())
            yield return null;
#endif

        // ダウンロード中...
        var fiberSet = new FiberSet();
        fiberSet.AddFiber(this.FileDownloadCoroutine(downloadSet));
        while (fiberSet.Update())
            yield return null;

        // ファイルダウンロード成否
        {
            bool isSuccess = true;
            foreach (var param in downloadingList)
            {
                // 完了待ち
                while (!param.Completed)
                    yield return null;
                // 成否判定
                if (param.IsSuccess)
                {
                    // CRCチェック(2度DLを防ぐため,1つ失敗してもチェックは継続)
                    if (this.AssetBundleVersions.Check(param.FilePath))
                        continue;
                }
                isSuccess = false;
            }

            if (!isSuccess)
            {
                Debug.LogError("AssetBundleDLCoroutine: Download fail2");
                GUISystemMessage.SetModeYesNo(MasterData.GetText(TextType.TX155_Error_ScreenTitle), MasterData.GetText(TextType.TX151_Download_Error), MasterData.GetText(TextType.TX159_Common_RetryButton), MasterData.GetText(TextType.TX158_Common_QuitButton), this.RetrySetAssetBundleDL, Application.Quit);
                yield break;
            }
        }

        // 共有アセットバンドルはここで読み込んでしまう.
        AssetReference.SetSharedAssetReference();
        // サウンド設定.
        if (SoundController.Instance != null)
            SoundController.ReStart();
        // 次のステートへ
        this.SetAssetBundleDLNextState();
    }
    #endregion

    #region Statement.Advertise 広告処理
    void SetAdvertise()
    {
        GUIDebugLog.SetActive(true);
        GUIDebugLog.AddMessage(TitleColorCode + "[b]【広告処理】[/b][-]");

        // 次のステートへ
        this.SetAdvertiseNextState();
    }
    void SetAdvertiseNextState()
    {
        // 次のステートへ
        this._SetState((Statement)(Statement.Advertise + 1));
    }
    #endregion

    #region Statement.Browser 初回ブラウザ起動
    void SetBrowser()
    {
        GUIDebugLog.SetActive(true);
        GUIDebugLog.AddMessage(TitleColorCode + "[b]【初回ブラウザ起動】[/b][-]");

        // 次のステートへ
        this.SetBrowserNextState();
    }
    void SetBrowserNextState()
    {
        // 次のステートへ
        this._SetState((Statement)(Statement.Browser + 1));
    }
    #endregion

    #region Statement.End 終了処理
    void SetEnd()
    {
        GUIDebugLog.SetActive(true);
        GUIDebugLog.AddMessage(TitleColorCode + "[b]【終了】[/b][-]");

#if XW_DEBUG
        this.ProcFiber = this.EndCoroutine();
#else
		// ステート変更
		this.SetEndNextState();
#endif
    }
    void SetEndNextState()
    {
        GUIDebugLog.ClearOKButton();
        GUIDebugLog.DeactiveOKButton();
        GUIDebugLog.Close();

        AuthMain.NextScene();
    }
#if XW_DEBUG
    IEnumerator EndCoroutine()
    {
        // OKボタンを押すまで待つ
        var waitOK = this.WaitOKCoroutinue();
        while (waitOK.MoveNext())
            yield return null;

        // ステート変更
        this.SetEndNextState();
    }
#endif
    #endregion

    #region OKボタンを押すまで待つ
#if XW_DEBUG
    IEnumerator WaitOKCoroutinue()
    {
        if (ScmParam.Debug.File.IsAuthCheck)
        {
            // OKボタンを押すまで待つ
            GUIDebugLog.ActiveOKButton();
            this.IsNextScene = false;
            while (!this.IsNextScene)
                yield return null;
            GUIDebugLog.DeactiveOKButton();
            GUIDebugLog.SetScroll(0f, 1f);
        }
    }
#endif
    #endregion

    #region ファイルダウンロード
    IEnumerator FileDownloadCoroutine(DownloadSet downloadSet)
    {
        if (downloadSet.Count <= 0)
            yield break;

        GUIDebugLog.AddMessage(string.Format(
            "Path : " + ValueColorCode + "{0}{1}[-]\r\n" +
            "Num : " + ValueColorCode + "{2}[-]",
            System.IO.Directory.GetCurrentDirectory(), FileBase.GetDirectory(""), downloadSet.Count));
        GUIDebugLog.AddMessage("");	// ここで追加したメッセージをループ処理内で内容を書き換える

        // プログレスバー情報設定
        this.ProgressTitle = MasterData.GetTextDefalut(TextType.TX150_Download_Progress, ObsolateSrc.Defalut_TX150_Download_Progress);
        this.ProgressSliderActive = true;

        // ダウンロード
        var totalCount = downloadSet.Count;
        while (0 < downloadSet.Count)
        {
            downloadSet.Update();
            this.DownloadDebugMessage(downloadSet);
            var progress = ((totalCount - downloadSet.Count) + (downloadSet.TotalProgressPercentage * 0.01f)) / totalCount;
            this.Progress = progress;
            this.ProgressText = string.Format(this.ProgressFormat, progress * 100f);
            yield return null;
        }
        this.Progress = 1f;
        this.ProgressText = string.Format(this.ProgressFormat, 100f);
        GUIDebugLog.ChangeMessage("");

#if XW_DEBUG
        // OKボタンを押すまで待つ
        var waitOK = this.WaitOKCoroutinue();
        while (waitOK.MoveNext())
            yield return null;
#endif
    }
    [System.Diagnostics.Conditional("XW_DEBUG")]
    void DownloadDebugMessage(DownloadSet downloadSet)
    {
        var downloadingList = downloadSet.GetDownloadingList();
        string msg = "";
        foreach (var param in downloadingList)
        {
            msg += string.Format(
                "{0}..." + ValueColorCode + "{1:000}%[-]\r\n",
                param.Uri.AbsoluteUri,
                param.ProgressPercentage
                );
        }
        // 直前に追加したメッセージを書き換える
        GUIDebugLog.ChangeMessage(msg);
    }
    #endregion

    #region デバッグ
#if UNITY_EDITOR && XW_DEBUG
    [SerializeField]
    DebugParameter _debugParam = new DebugParameter();
    DebugParameter DebugParam { get { return _debugParam; } }
    [System.Serializable]
    public class DebugParameter
    {
        public bool ececuteClose;
        public bool executeState;
        public Statement state;
    }
    void DebugUpdate()
    {
        var t = this.DebugParam;
        if (t.ececuteClose)
        {
            t.ececuteClose = false;
            Close();
        }
        if (t.executeState)
        {
            t.executeState = false;
            this._SetState(t.state);
        }
    }
    /// <summary>
    /// OnValidate はInspector上で値の変更があった時に呼び出される
    /// GameObject がアクティブ状態じゃなくても呼び出されるのでアクティブ化するときには有効
    /// </summary>
    void OnValidate()
    {
        if (Application.isPlaying)
            this.DebugUpdate();
    }
#endif
    #endregion
}
