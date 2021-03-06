/// <summary>
/// リザルトメイン処理
/// 
/// 2013/06/12
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.GameParameter;
using XUI;

public class ResultMain : SceneMain<ResultMain>
{
	#region フィールド＆プロパティ
	const string SceneName = SceneController.SceneName.Result;

	private static BridgingResultInfo resultInfo;
	private bool isNextScene = false;
	#endregion

	#region 初期化
	static public void LoadScene(BridgingResultInfo bridgingResultInfo)
	{
		resultInfo = bridgingResultInfo;
        
		FiberSet fiberSet = new FiberSet();
		fiberSet.AddFiber(ResourceLoad());
		SceneController.FadeSceneChange(SceneName, fiberSet);
	}
	static private IEnumerator ResourceLoad()
	{
		// リザルトシーンに必要なデータを残しその他のデータを削除する
		NetworkController.DestroyBattle();
		yield break;
	}
	protected override void Awake()
	{
		base.Awake ();
	}
	void Start()
	{
		if(resultInfo != null)
		{
#if XW_DEBUG
			if(ScmParam.Debug.File.IsResultSkip)
			{
				// リザルトをスキップさせる(次のシーンに移る)
				_GotoNextScene();
				return;
			}
#endif
            //LWZ:TODO=>ResultPanel
            XUI.GUIResultShow.Instance.Show(resultInfo);
           
			//GUIResultOld.Setup(resultInfo);
			resultInfo = null;
            //Todo Lee 暂时放在这里
            ResidentArea.mCachDic.Clear();
		    if (null != GUIBattleDirectionTip.Instance)
		    {
                GUIBattleDirectionTip.Instance.targets.Clear();
		    }
            Gadget.DelateSetUp.Clear();
		}
		else
		{
			BugReportController.SaveLogFile("resultInfo null");
			_GotoNextScene();
		}
	}
	#endregion

	#region ISceneMain
	public override bool OnNetworkDisconnect()
	{
		return this.isNextScene;
	}

	public override void OnNetworkDisconnectByServer() { }
	#endregion
	
	#region 次のシーン
	public static void GotoNextScene()
	{
		if(Instance == null) return;
		Instance._GotoNextScene();
	}
	public void _GotoNextScene()
	{
		this.isNextScene = true;
		this.StartCoroutine(this.NextScene());
	}
	IEnumerator NextScene()
	{
		// ロード画面表示
		GUILoading.SetActive(true);
		yield return 0;
		
		// BGM
		SoundController.StopBGM();
		
		if (!Scm.Client.GameListener.ConnectFlg)
		{
			// 通信が切れている
			// タイトルへ戻る.
			GUISystemMessage.SetModeOK(
				MasterData.GetText(TextType.TX029_DisconnectTitle),
				MasterData.GetText(TextType.TX027_Disconnect),
				MasterData.GetText(TextType.TX031_DisconnectOK),
				TitleMain.LoadScene);
		}
		else
		{
			// シーン切り替え
			LobbyPacket.SendEnterLobby();
		}
	}
	#endregion

    void OnDestroy()
    {
        GUIResultShow.Unload();
    }
}
