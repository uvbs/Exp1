/// <summary>
/// プレイヤー選択状態クラス
/// 
/// 2015/12/21
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// プレイヤー選択状態クラス
/// </summary>
public class SelectPlayerState : IGameLoginState
{
	#region フィールド&プロパティ
	/// <summary>
	/// プレイヤー選択パケット要求
	/// </summary>
	private ISelectPlayerRequest selectPlayerRequest = null;

	/// <summary>
	/// 次の状態への遷移開始イベント
	/// </summary>
	public event EventHandler<StateBeginExitEventArgs> OnBeginExit = (sender, e) => { };

	/// <summary>
	/// 状態の実行フラグ
	/// </summary>
	private bool isExecute = true;

	/// <summary>
	/// 保有プレイヤー情報
	/// </summary>
	private List<RetainPlayerInfo> playerInfoList = new List<RetainPlayerInfo>();

	/// <summary>
	/// 選択されたプレイヤーID
	/// </summary>
	private int selectPlayerId = 0;

	/// <summary>
	/// 切断処理を実行するかどうか
	/// </summary>
	private bool isDisconnectExecute = true;

	/// <summary>
	/// エラー状態
	/// </summary>
	public GameLoginController.ErrorType ErrorState { get { return GameLoginController.ErrorType.None; } }
	#endregion

	#region 初期化
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public SelectPlayerState(List<RetainPlayerInfo> playerInfoList)
	{
		Init(playerInfoList);
		this.selectPlayerId = this.playerInfoList[0].PlayerID;
	}

	/// <summary>
	/// コンストラクタ
	/// </summary>
	public SelectPlayerState(int playerId)
	{
		Init(null);
		this.selectPlayerId = playerId;
	}

	/// <summary>
	/// 初期化
	/// </summary>
	/// <param name="playerInfoList"></param>
	private void Init(List<RetainPlayerInfo> playerInfoList)
	{
		this.selectPlayerRequest = new SelectPlayerRequest();
		this.isExecute = true;
		this.playerInfoList = playerInfoList;

		// 各イベント登録
		this.selectPlayerRequest.ResponseEvent += SelectPlayerResponse;
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
	/// プレイヤー選択レスポンス
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void SelectPlayerResponse(object sender, SelectPlayerEventArgs e)
	{
		if(e.Result)
		{
			// 成功時はサーバ情報チェック状態へ
			var eventArgs = new StateBeginExitEventArgs(new ServerStatusState());
			OnBeginExit(this, eventArgs);
		}
		else
		{
			// 決定ボタンが押されるまで切断されても切断処理を行わない
			this.isDisconnectExecute = false;

			// 失敗時はタイトル情報画面へ遷移
			GUISystemMessage.SetModeOK
				(MasterData.GetText(TextType.TX029_DisconnectTitle), MasterData.GetText(TextType.TX047_ReturnTitleInfo),
				  () => { GUITitle.OpenInfo(); }
				);
				BugReportController.SaveLogFile(string.Format("SelectPlayerRes PacketParameterError. Result={0}", e.Result));
				GUIDebugLog.AddMessage(string.Format("SelectPlayerRes PacketParameterError. Result={0}", e.Result));
		}

		this.isExecute = false;
	}
	#endregion

	#region 状態開始
	/// <summary>
	/// 状態の開始の処理
	/// </summary>
	public void Start()
	{
		// プレイヤー選択パケット送信
		this.selectPlayerRequest.Send(this.selectPlayerId);

		// サーバ接続メッセージ表示
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX041_Connecting));
		GUIDebugLog.AddMessage("プレイヤー情報取得中・・・");
	}
	#endregion

	#region 状態の実行
	/// <summary>
	/// 実行
	/// </summary>
	/// <returns></returns>
	public IEnumerable Execute()
	{
		// プレイヤー選択パケットの応答があるまで待機
		while(this.isExecute)
		{
			yield return null;
		}
	}
	#endregion

	#region 状態の終了
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
		// 切断処理実行フラグがOFFの場合は処理を行わない
		if (this.isDisconnectExecute)
		{
			// 切断されたら接続状態に遷移させる
			var eventArgs = new StateBeginExitEventArgs(new ConnectState());
			OnBeginExit(this, eventArgs);

			return true;
		}
		else
		{
			return false;
		}
	}
	#endregion
}
