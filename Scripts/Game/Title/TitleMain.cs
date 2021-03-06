/// <summary>
/// タイトルメイン処理
/// 
/// 2013/03/26
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TitleMain : SceneMain<TitleMain>
{
	#region 定数
	private const string SceneName = SceneController.SceneName.Title;

	/// <summary>
	/// 再接続を行う最大数
	/// </summary>
	private const int ReConnectMax = 3;
	#endregion

	#region フィールド＆プロパティ
	/// <summary>
	/// 演出用カメラ.
	/// </summary>
	[SerializeField]
	private ResultRotateCamera rotateCamera;
	#endregion

	#region 初期化
	static public void LoadScene()
	{
		FiberSet fiberSet = new FiberSet();
		fiberSet.AddFiber(ResourceLoad());
		SceneController.FadeSceneChange(SceneName, fiberSet);
	}
	/// <summary>
	/// リソース読み込みコルーチン
	/// </summary>
	static private IEnumerator ResourceLoad()
	{
		// BGM
		SoundController.PlayBGM(SoundController.BgmID.Title);
		
		// 課金初期化
		Asobimo.Purchase.PurchaseManager.Instance.Initialize();

		if(Scm.Client.GameListener.ConnectFlg)
		{
			NetworkController.Disconnect();
		}

		NetworkController.DestroyAll();
		MapManager.CreateTitle();
		while(!MapManager.Instance.MapExists)
		{
			// マップの読み込みが終わっていないので待つ.
			yield return null;
		}
		// カメラ設定.
		if(TitleMain.Instance)
		{
			TitleMain.Instance.SetCameraParameters();
		}
		GUILoading.SetActive(false);
	}
	/// <summary>
	/// カメラ設定.
	/// </summary>
	private void SetCameraParameters()
	{
		if(this.rotateCamera != null)
		{
			var cameraSetting = MapManager.Instance.Map.GetComponent<TitleCameraSetting>();
			if(cameraSetting != null)
			{
				this.rotateCamera.SetParameters(cameraSetting);
			}
		}
	}
	#endregion

	#region ISceneMain
	public override bool OnNetworkDisconnect()
	{
		return false;
	}

	public override void OnNetworkDisconnectByServer() { }
	#endregion

	#region 次のシーンへ遷移
	public static void NextScene()
	{
		if(Instance == null) return;
		Instance._NextScene();
	}
	private void _NextScene()
	{
//#if EJPL && !UNITY_EDITOR
//        int playeruid = int.Parse(PlayerPrefs.GetString("player_uid", "1"));
//        APaymentHelperDemo.Instance.EnterServer(playeruid, ScmParam.Net.UserName, System.DateTime.Now.Second);
//#endif
		// BGM停止
		SoundController.StopBGM();
		// ロード画面表示
		GUILoading.SetActive(true);
		// シーン切り替え
		LobbyPacket.SendEnterLobby();
		// デバッグログ非表示
		GUIDebugLog.Close();
	}
	#endregion
}
