/// <summary>
/// シーンコントローラ
/// 
/// 2014/08/06
/// </summary>
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Packet;

public static class SceneController
{
	#region 宣言
	public static class SceneName
	{
		public const string Auth		= "ScmAuth";
		public const string Battle		= "ScmBattle";
		public const string Tutorial	= "ScmTutorial";
		public const string Title		= "ScmTitle";
		public const string Lobby		= "ScmLobby";
		public const string Result		= "ScmResult";
		// TODO:仮でビュアー用のシーン追加(後々本番用アプリとデバック用アプリに切り分ける予定)
		public const string Viewer		= "ScmViewer";
	}
	#endregion

	#region フィールド
	static public ISceneMain NowScene { get; set; }
	static private Fiber sceneChangeFiber;
	static private List<IOnNetworkDisconnect> _onNetworkDisconnectList = new List<IOnNetworkDisconnect>();
	static private List<IOnNetworkDisconnect> OnNetworkDisconnectList { get { return _onNetworkDisconnectList; } set { _onNetworkDisconnectList = value; } }
	static private List<IOnNetworkDisconnectByServer> _onNetworkDisconnectByServerList = new List<IOnNetworkDisconnectByServer>();
	static private List<IOnNetworkDisconnectByServer> OnNetworkDisconnectByServerList { get { return _onNetworkDisconnectByServerList; } set { _onNetworkDisconnectByServerList = value; } }
    #endregion

    #region Method
    /// <summary>
    /// LWZ:Anime Scene Change
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="fiber"></param>
    static public void AnimeSceneChange(EnterFieldRes enterFieldRes, string sceneName, FiberSet fiber = null)
    {
        FiberController.AddFiber(_AnimeSceneChange(enterFieldRes, sceneName, fiber));
    }

    static private IEnumerator _AnimeSceneChange(EnterFieldRes enterFieldRes, string sceneName, FiberSet fiberInFade)
    {
        var beforeSceneName = SceneManager.GetActiveScene().name;

        // 非同期シーンロード命令.
        AsyncOperation loadScene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        loadScene.allowSceneActivation = false;

        //LWZ: loading anime

        GUIAnimeRankIn.Instance.Show(enterFieldRes);

        // 旧シーンの削除.
        if (NowScene != null)
        {
            MonoBehaviour mb = NowScene as MonoBehaviour;
            if (mb != null)
            {
                GameObject.Destroy(mb.gameObject);
            }
        }
        if (!string.IsNullOrEmpty(beforeSceneName))
        {
            SceneManager.UnloadScene(beforeSceneName);
        }

        // fiberInFadeより先にtrueにしておかないと,同時実行可能な非同期処理の数に引っかかりデッドロック状態になることがある.
        loadScene.allowSceneActivation = true;
        // シーンロードが終わるまで待つ.
        while (!loadScene.isDone)
        {
            // 進捗状況設定
            GUIFade.Progress = GetSceneChangeProgress(loadScene.progress, 1f, 0f, 0f);
            yield return null;
        }
        // アクティブシーンを切り替える
        var now = SceneManager.GetSceneByName(sceneName);
        if (now.IsValid())
        {
            bool ret = SceneManager.SetActiveScene(now);
            if (!ret)
            {
                // エラー
                BugReportController.SaveLogFile("SetActiveScene=Failed");
                GUIDebugLog.AddMessage("SetActiveScene=Failed");
                Debug.Log("SetActiveScene=Failed");
            }
        }
        else
        {
            // エラー
            BugReportController.SaveLogFile(string.Format("SceneManager.GetSceneByName({0})=Failed", sceneName));
            GUIDebugLog.AddMessage(string.Format("SceneManager.GetSceneByName({0})=Failed", sceneName));
            Debug.Log(string.Format("SceneManager.GetSceneByName({0})=Failed", sceneName));
        }

        // メッセージウィンドウを閉じる(GUISystemMessageは現状閉じていない).
        GUIMessageWindow.Close();

        // 使用済みアセットのアンロード.
        {
            AsyncOperation unload = Resources.UnloadUnusedAssets();
            while (!unload.isDone)
            {
                GUIFade.Progress = GetSceneChangeProgress(loadScene.progress, 1f, unload.progress, 0f);
                yield return null;
            }
        }

        // 完全にフェードアウトしている最中.
        if (fiberInFade != null)
        {
            float progress = 0.1f;
            // フェード中処理.
            while (fiberInFade.Update())
            {
                // 進捗状況設定
                GUIFade.Progress = GetSceneChangeProgress(loadScene.progress, 1f, 1f, progress);
                progress = Mathf.Min(progress + 0.1f, 1f);
                yield return null;
            }
        }

        // 進捗状況設定
        GUIFade.Progress = 1f;

        // フェードイン開始.
        GUIFade.FadeIn();

        GUIAnimeRankIn.Instance.Close();
        /*
        //save befor scene name
        var beforeSceneName = SceneManager.GetActiveScene().name;

        //load scene async
        AsyncOperation loadScene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        loadScene.allowSceneActivation = false;

        //loading anime

        //destroy old scene
        if(NowScene != null)
        {
            MonoBehaviour mb = NowScene as MonoBehaviour;
            if(mb != null)
            {
                GameObject.Destroy(mb.gameObject);
            }
        }
        if (!string.IsNullOrEmpty(beforeSceneName))
        {
            SceneManager.UnloadScene(beforeSceneName);
        }

        //
        loadScene.allowSceneActivation = true;

        var now = SceneManager.GetSceneByName(sceneName);
        if (now.IsValid())
        {
            bool ret = SceneManager.SetActiveScene(now);
            if (!ret)
            {
                // エラー
                BugReportController.SaveLogFile("SetActiveScene=Failed");
                GUIDebugLog.AddMessage("SetActiveScene=Failed");
                Debug.Log("SetActiveScene=Failed");
            }
        }
        else
        {
            // エラー
            BugReportController.SaveLogFile(string.Format("SceneManager.GetSceneByName({0})=Failed", sceneName));
            GUIDebugLog.AddMessage(string.Format("SceneManager.GetSceneByName({0})=Failed", sceneName));
            Debug.Log(string.Format("SceneManager.GetSceneByName({0})=Failed", sceneName));
        }

        // メッセージウィンドウを閉じる(GUISystemMessageは現状閉じていない).
        GUIMessageWindow.Close();

       
        // 完全にフェードアウトしている最中.
        if (fiber != null)
        {
            // フェード中処理.
            while (fiber.Update())
            {
               
                yield return null;
            }
        }
        GUIFade.FadeIn();*/
    }

	/// <summary>
	/// 通常のシーンチェンジ（フェードしつつのシーンチェンジ）を行う.各SceneMainのLoadSceneから呼ぶ.
	/// </summary>
	static public void FadeSceneChange(string sceneName, FiberSet fiberInFade = null)
	{
#if XW_DEBUG
		// チュートリアル開始判定.
		if (ScmParam.Debug.File.IsTutorial)	// debug.jsonのフラグがオン.
		{
			if (sceneName != SceneName.Tutorial && NowScene is TitleMain && TutorialMain.Instance == null)	// 現在タイトルシーンで,チュートリアルを開始していない.
			{
				TutorialMain.LoadScene();
				return;
			}
		}
#endif
		FiberController.AddFiber(WaitSceneChange(sceneName, fiberInFade));
	}

	static private IEnumerator WaitSceneChange(string sceneName, FiberSet fiberInFade)
	{
        Debug.Log("Waiting...");
		// 既にシーンロード中なら待つ.
		while(FiberController.Contains(sceneChangeFiber))
		{
            Debug.Log("Contains...");
			yield return null;
		}
		sceneChangeFiber = FiberController.AddFiber(_FadeSceneChange(sceneName, fiberInFade));
	}
	static private IEnumerator _FadeSceneChange(string sceneName, FiberSet fiberInFade)
	{
		var beforeSceneName = SceneManager.GetActiveScene().name;

		// 非同期シーンロード命令.
		AsyncOperation loadScene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
		loadScene.allowSceneActivation = false;

        //LWZ: loading anime
        
		// フェードアウト開始.
		GUIFade.FadeOut(true);
		while(!GUIFade.IsFinish)
		{
			// 進捗状況設定
			GUIFade.Progress = GetSceneChangeProgress(loadScene.progress, 0f, 0f, 0f);
			yield return null;
		}

		// 旧シーンの削除.
		if(NowScene != null)
		{
			MonoBehaviour mb = NowScene as MonoBehaviour;
			if(mb != null)
			{
				GameObject.Destroy(mb.gameObject);
			}
		}
		if (!string.IsNullOrEmpty(beforeSceneName))
		{
			SceneManager.UnloadScene(beforeSceneName);
		}

		// fiberInFadeより先にtrueにしておかないと,同時実行可能な非同期処理の数に引っかかりデッドロック状態になることがある.
		loadScene.allowSceneActivation = true;
		// シーンロードが終わるまで待つ.
		while(!loadScene.isDone)
		{
			// 進捗状況設定
			GUIFade.Progress = GetSceneChangeProgress(loadScene.progress, 1f, 0f, 0f);
			yield return null;
		}
		// アクティブシーンを切り替える
		var now = SceneManager.GetSceneByName(sceneName);
		if (now.IsValid())
		{
			bool ret = SceneManager.SetActiveScene(now);
			if (!ret)
			{
				// エラー
				BugReportController.SaveLogFile("SetActiveScene=Failed");
				GUIDebugLog.AddMessage("SetActiveScene=Failed");
				Debug.Log("SetActiveScene=Failed");
			}
		}
		else
		{
			// エラー
			BugReportController.SaveLogFile(string.Format("SceneManager.GetSceneByName({0})=Failed", sceneName));
			GUIDebugLog.AddMessage(string.Format("SceneManager.GetSceneByName({0})=Failed", sceneName));
			Debug.Log(string.Format("SceneManager.GetSceneByName({0})=Failed", sceneName));
		}

		// メッセージウィンドウを閉じる(GUISystemMessageは現状閉じていない).
		GUIMessageWindow.Close();

		// 使用済みアセットのアンロード.
		{
			AsyncOperation unload = Resources.UnloadUnusedAssets();
			while(!unload.isDone)
			{
				GUIFade.Progress = GetSceneChangeProgress(loadScene.progress, 1f, unload.progress, 0f);
				yield return null;
			}
		}

		// 完全にフェードアウトしている最中.
		if(fiberInFade != null)
		{
			float progress = 0.1f;
			// フェード中処理.
			while(fiberInFade.Update())
			{
				// 進捗状況設定
				GUIFade.Progress = GetSceneChangeProgress(loadScene.progress, 1f, 1f, progress);
				progress = Mathf.Min(progress + 0.1f, 1f);
				yield return null;
			}
		}

		// 進捗状況設定
		GUIFade.Progress = 1f;

		// フェードイン開始.
		GUIFade.FadeIn();
	}

	/// <summary>
	/// シーン遷移の進行度を計算する.
	/// </summary>
	static private float GetSceneChangeProgress(float load, float destroy, float unLoad, float fiber)
	{
		const float LoadRatio = 0.3f;
		const float DestroyRatio = 0.1f;
		const float UnLoadRatio = 0.2f;
		const float FiberRatio = 0.4f; 

		return (load * LoadRatio + destroy * DestroyRatio + unLoad * UnLoadRatio + fiber * FiberRatio);
	}

	/// <summary>
	/// 現在シーン上にあるSceneMainをセットする.SceneMain.Awake()から呼ぶ.
	/// </summary>
	static public void SetNowScene(ISceneMain sceneMain)
	{
		// GUIControllerに登録している表示UIを全て閉じる
		GUIController.Clear();
		GUIController.SingleClose();

		// 旧シーンの削除.
		if(NowScene != null)
		{
			MonoBehaviour mb = NowScene as MonoBehaviour;
			if(mb != null)
			{
				GameObject.Destroy(mb.gameObject);
			}
		}
		// 新シーンの登録.
		NowScene = sceneMain;
		if(!Scm.Client.GameListener.ConnectFlg)
		{
			// シーン開始時に通信が切れていた場合,通信切断時の処理を行う.
			OnNetworkDisconnect();
		}
	}
	/// <summary>
	/// 通信切断時の処理.
	/// </summary>
	static public void OnNetworkDisconnect()
	{
		// 登録されているイベントに通知する
		foreach (var t in OnNetworkDisconnectList)
		{
			t.Disconnect();
		}

		if(NowScene != null)
		{
			if(NowScene.OnNetworkDisconnect())
			{
				// タイトルへ戻る.
				GUISystemMessage.SetModeOK(
					MasterData.GetText(TextType.TX029_DisconnectTitle),
					MasterData.GetText(TextType.TX027_Disconnect),
					MasterData.GetText(TextType.TX031_DisconnectOK),
					() =>
					{
#if UNITY_EDITOR
#else
						TitleMain.LoadScene();
#endif
					}
				);
			}
		}
	}

	/// <summary>
	/// サーバからの通信切断時の処理.
	/// </summary>
	static public void OnNetworkDisconnectByServer()
	{
		// 登録されているイベントに通知する
		foreach (var t in OnNetworkDisconnectByServerList)
		{
			t.DisconnectByServer();
		}

		if(NowScene != null)
		{
			NowScene.OnNetworkDisconnectByServer();
		}
	}

	/// <summary>
	/// 通信切断された時のイベント登録
	/// </summary>
	static public void AddDisconnect(IOnNetworkDisconnect e)
	{
		if (OnNetworkDisconnectList != null)
			OnNetworkDisconnectList.Add(e);
	}

	/// <summary>
	/// 通信切断された時のイベント削除
	/// </summary>
	static public void RemoveDisconnect(IOnNetworkDisconnect e)
	{
		if (OnNetworkDisconnectList != null)
			OnNetworkDisconnectList.Remove(e);
	}

	/// <summary>
	/// サーバから通信切断された時のイベント登録
	/// </summary>
	static public void AddDisconnectByServer(IOnNetworkDisconnectByServer e)
	{
		if (OnNetworkDisconnectByServerList != null)
			OnNetworkDisconnectByServerList.Add(e);
	}

	/// <summary>
	/// サーバから通信切断された時のイベント削除
	/// </summary>
	static public void RemoveDisconnectByServer(IOnNetworkDisconnectByServer e)
	{
		if (OnNetworkDisconnectByServerList != null)
			OnNetworkDisconnectByServerList.Remove(e);
	}
	#endregion
}

/// <summary>
/// 通信切断された時のインターフェイス
/// </summary>
public interface IOnNetworkDisconnect
{
	void Disconnect();
}

/// <summary>
/// サーバからの通信切断された時のインターフェイス
/// </summary>
public interface IOnNetworkDisconnectByServer
{
	void DisconnectByServer();
}
