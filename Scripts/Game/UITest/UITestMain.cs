/// <summary>
/// UIテスト処理
/// 
/// 2016/07/03
/// </summary>
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UITestMain : MonoBehaviour
{
#if XW_DEBUG
	#region フィールド＆プロパティ
	const string SceneName = "UITest";

	// シリアライズされていないメンバーの初期化
	void MemberInit()
	{
	}
	#endregion

	#region シーン追加
	public static void SceneAdditive()
	{
		// 同期シーンロード命令.
		SceneManager.LoadScene(SceneName, LoadSceneMode.Additive);
	}
	public static IEnumerator SceneAdditiveAsync()
	{
		// 非同期シーンロード命令.
		AsyncOperation loadScene = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive);
		loadScene.allowSceneActivation = false;
		
		loadScene.allowSceneActivation = true;
		// シーンロードが終わるまで待つ.
		while(!loadScene.isDone)
			yield return null;
	}
	#endregion

	#region 初期化
	void Awake()
	{
	}
	void Start()
	{
		// デバッグファイル読み込み
		try
		{
			DebugFile.Instance.Read();
			ScmParam.Debug.File = DebugFile.Instance.Clone();
		}
		catch (System.Exception e)
		{
			Debug.LogWarning(DebugFile.Filename + "でエラー\r\n" + e);
		}

		// 言語設定
		Scm.Common.Utility.Language = ApplicationController.Language;

		MasterData.Read();

        string acfPath = @"E:\work\x-world\src\scm_client\trunk\game\Assets\ScmAssets\Bundle\X_world_Voice_p001.acb";
        CriAtomEx.RegisterAcf(null, acfPath);
        CriAtomEx.AttachDspBusSetting("DspBusSetting_0");
    }

    void OnGUI() {
        if (GUI.Button(new Rect(0, 0, 100, 40), "Test")) {
            CriAtomSource atomSource = gameObject.GetComponent<CriAtomSource>();
            CriAtomExAcb atomExAcb = CriAtom.GetAcb(atomSource.cueSheet);
            if (atomExAcb != null) {
                atomSource.Stop();
                atomSource.Play();
            }
        }
    }
	#endregion
#endif
}
