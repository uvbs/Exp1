/// <summary>
/// サーバ情報パケット要求クラス
/// 
/// 2015/12/21
/// </summary>
using System;
using System.Collections;
using Scm.Common.GameParameter;

/// <summary>
/// サーバ情報パケット要求クラス
/// </summary>
public class ServerStatusState : IGameLoginState
{
	#region フィールド&プロパティ
	/// <summary>
	/// サーバ情報パケット要求
	/// </summary>
	private IServerStatusRequest serverStatusRequest = null;

	/// <summary>
	/// 次の状態への遷移開始イベント
	/// </summary>
	public event EventHandler<StateBeginExitEventArgs> OnBeginExit = (sender, e) => { };

	/// <summary>
	/// 状態の実行フラグ
	/// </summary>
	private bool isExecute = true;

	/// <summary>
	/// エラー状態
	/// </summary>
	public GameLoginController.ErrorType ErrorState { get { return GameLoginController.ErrorType.None; } }
	#endregion

	#region 初期化
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public ServerStatusState()
	{
		this.serverStatusRequest = new ServerStatusRequest();
		this.isExecute = true;

		// 各イベント登録
		this.serverStatusRequest.ResponseEvent += ServerStatusResponse;
	}
	#endregion

	#region 破棄
	/// <summary>
	/// 破棄
	/// </summary>
	public void Dispose()
	{
		this.OnBeginExit = null;
	}
	#endregion

	#region イベント
	/// <summary>
	/// サーバ情報レスポンス
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ServerStatusResponse(object sender, ServerStatusEventArgs e)
	{
		foreach(var server in e.ServerStatusList)
		{
			switch(server.Status)
			{
				// サーバの状態に問題なし
				case ServerStatus.Ready:
					Ready();
					break;

				// 再参戦可能
				case ServerStatus.ReEntry:
					ReEntry();
					break;

				// メンテナンス
				case ServerStatus.Maintenance:
					Maintenance();
					break;
			}
		}

		this.isExecute = false;
	}

	/// <summary>
	/// サーバ正常
	/// </summary>
	private void Ready()
	{
		// ログイン完了 ロビーシーンに遷移
		TitleMain.NextScene();

		// 接続中メッセージを閉じる
		GUISystemMessage.Close();
	}

	/// <summary>
	/// 再参戦
	/// </summary>
	private void ReEntry()
	{
		// 再参戦状態へ
		var eventArgs = new StateBeginExitEventArgs(new ReEntryState());
		OnBeginExit(this, eventArgs);
	}

	/// <summary>
	/// サーバメンテナンス
	/// </summary>
	private void Maintenance()
	{
		// メッセージ表示 タイトルへ戻る
		GUISystemMessage.SetModeOK(MasterData.GetText(TextType.TX048_MainteTitle),
					   MasterData.GetText(TextType.TX049_Mainte),
					   GUITitle.OpenInfo);
	}
	#endregion

	#region 状態開始
	/// <summary>
	/// 状態の開始
	/// </summary>
	public void Start()
	{
		// サーバ情報パケット送信
		this.serverStatusRequest.Send();

		GUIDebugLog.AddMessage("サーバステータス取得中・・・");
	}
	#endregion

	#region 状態の実行
	/// <summary>
	/// 実行
	/// </summary>
	/// <returns></returns>
	public IEnumerable Execute()
	{
		while(this.isExecute)
		{
			yield return null;
		}
	}
	#endregion

	#region 状態終了
	/// <summary>
	/// 状態の終了処理
	/// </summary>
	public void Finish() { }
	#endregion

	#region 切断
	/// <summary>
	/// サーバ切断時処理
	/// </summary>
	public bool Disconnected()
	{
		// 切断されたら接続状態に遷移させる
		var eventArgs = new StateBeginExitEventArgs(new ConnectState());
		OnBeginExit(this, eventArgs);

		return true;
	}
	#endregion
}
