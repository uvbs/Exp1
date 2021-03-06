/// <summary>
/// 認証メイン処理
/// 
/// 2013/10/25
/// </summary>
using UnityEngine;
using System.Collections;

public class AuthMain : SceneMain<AuthMain>
{
	#region フィールド＆プロパティ
	const string SceneName = SceneController.SceneName.Auth;
	#endregion

	#region 初期化
	static public void LoadScene()
	{
		SceneController.FadeSceneChange(SceneName, null);
	}
	protected override void Awake()
	{
		base.Awake();

		if (Scm.Client.GameListener.ConnectFlg)
		{
			NetworkController.Disconnect();
		}
		NetworkController.DestroyAll();
	}
	#endregion

	#region SceneMain
	public override bool OnNetworkDisconnect()
	{
		return false;
	}

	public override void OnNetworkDisconnectByServer() { }
	#endregion

	#region 次のシーンへ移る
	public static void NextScene()
	{
		if (Instance != null) Instance._NextScene();
	}
	public void _NextScene()
	{
#if VIEWER
		// コンパイルシンボルにVIEWERがあるならビュワーシーンへ
		ViewerMain.LoadScene();
#else
		TitleMain.LoadScene();
#endif
	}
	#endregion
}
