/// <summary>
/// ゲームにログインするための制御クラス
/// 
/// 2015/12/16
/// </summary>
using System;
using System.Collections;

/// <summary>
/// 状態が別の状態へ遷移したがっている場合にディスパッチされるEventArgs
/// </summary>
public class StateBeginExitEventArgs : EventArgs
{
	/// <summary>
	/// 次の遷移先
	/// </summary>
	public IGameLoginState NextState { get; private set; }

	public StateBeginExitEventArgs(IGameLoginState nextState)
	{
		NextState = nextState;
	}
}

/// <summary>
/// ゲームログイン状態インターフェイス
/// </summary>
public interface IGameLoginState
{
	/// <summary>
	/// 破棄
	/// </summary>
	void Dispose();

	/// <summary>
	/// 状態の開始の処理
	/// </summary>
	void Start();

	/// <summary>
	/// 実行
	/// </summary>
	/// <returns></returns>
	IEnumerable Execute();

	/// <summary>
	/// 状態の終了処理
	/// </summary>
	void Finish();

	/// <summary>
	/// サーバが切断された時に呼ばれる
	/// </summary>
	/// <returns>
	/// true = 再接続回数をカウントする
	/// false = 再接続回数をカウントしない
	/// </returns>
	bool Disconnected();

	/// <summary>
	/// 次の状態への遷移開始の準備ができたらディスパッチされる
	/// </summary>
	event EventHandler<StateBeginExitEventArgs> OnBeginExit;

    /// <summary>
    /// エラー状態
    /// </summary>
    GameLoginController.ErrorType ErrorState { get; }
}

/// <summary>
/// ゲームにログインするための制御クラス
/// </summary>
public class GameLoginController : IOnNetworkDisconnect, IOnNetworkDisconnectByServer
{
    #region エラー状態
    public enum ErrorType
    {
        None,
        Xigncode,           // Xigncodeのチェックでサーバーからエラーを返している(この状態の時は認証処理を行う)
    }
    #endregion

    #region フィールド&プロパティ
    /// <summary>
    /// ゲームにログインするための試行回数
    /// </summary>
    private const int LoginRequestCount = 3;

	/// <summary>
	/// ログイン試行回数
	/// </summary>
	private int loginCount = 0;

	/// <summary>
	/// 現在の状態
	/// </summary>
	private IGameLoginState state = null;
	private IGameLoginState State
	{
		get { return this.state; }
		set
		{
			this.state = value;
            UnityEngine.Debug.Log("LoginState: " + this.state);
			if (this.state != null)
			{
				this.state.OnBeginExit += HandleStateBeginExit;
				this.state.Start();

				// TODO: ゲームサーバ接続時によるタイムアウト処理が本実装になるまでの仮処理
				// 切断処理セット
				var connectState = this.state as ConnectState;
				if (connectState != null)
				{
					connectState.OnDiscoonect = Disconnect;
				}
			}
		}
	}

	/// <summary>
	/// 次の状態
	/// </summary>
	private IGameLoginState nextState = null;

	/// <summary>
	/// 終了通知用
	/// </summary>
	private Action<ErrorType> FinishEvevnt = (errorState) => {};
	#endregion

	#region 初期化
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public GameLoginController(Action<ErrorType> finishEvevnt)
	{
		this.loginCount = 0;
		// 切断処理登録
		SceneController.AddDisconnect(this);
		SceneController.AddDisconnectByServer(this);
		
		// 始めに開始する状態をセット
		this.State = new ConnectState();
		this.nextState = null;
		this.loginCount++;

		// プレイヤー名初期化
		ScmParam.Net.UserName = string.Empty;

		// デバッグログ表示
		GUIDebugLog.SetActive(true);

		this.FinishEvevnt = finishEvevnt;
	}

	/// <summary>
	/// デストラクタ
	/// </summary>
	~GameLoginController()
	{
		this.Dispose();
		this.FinishEvevnt = null;
	}

	/// <summary>
	/// 破棄
	/// </summary>
	public void Dispose()
	{
		if(this.State != null)
		{
			this.State.Dispose();
		}
	}
	#endregion

	#region リセット
	/// <summary>
	/// 状態をリセットし始めの状態から開始させる
	/// </summary>
	private void ResetState()
	{
		this.nextState = new ConnectState();
		this.loginCount++;
	}
	#endregion

	#region 実行
	/// <summary>
	/// ログイン実行
	/// </summary>
	/// <returns></returns>
	public IEnumerable Execute()
	{
		while (true)
		{
			// 次の状態がセットされるか実行の停止があるまで現行の状態を実行する
			var e = this.State.Execute().GetEnumerator();
			while (this.State != null && this.nextState == null && e.MoveNext())
			{
				yield return e.Current;
			}

			// 現行の状態の終了処理
			if (this.State != null)
			{
				this.State.Finish();
			}

			// 次の状態が存在しなければ実行処理終了
			if (this.nextState == null)
			{
				// 終了処理
				EndExecute();
				yield break;
			}

			// 状態の遷移
			this.State.Dispose();
			this.State = this.nextState;
            UnityEngine.Debug.Log("NextState...");
			this.nextState = null;
		}
	}

	/// <summary>
	/// 実行終了処理
	/// </summary>
	private void EndExecute()
	{
		// 切断処理削除
		SceneController.RemoveDisconnect(this);
		SceneController.RemoveDisconnectByServer(this);

		// デバッグログを閉じる
		GUIDebugLog.Close();

		// 破棄
		this.Dispose();

		// 終了通知
		var errorState = this.State != null ? this.State.ErrorState : ErrorType.None;
		if(this.FinishEvevnt != null)
		{
			this.FinishEvevnt(errorState);
		}
	}
	#endregion

	#region ハンドラ
	/// <summary>
	/// 遷移をしたがっている現行の状態を扱う
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void HandleStateBeginExit(object sender, StateBeginExitEventArgs e)
	{
		this.nextState = e.NextState;
        UnityEngine.Debug.Log("NextState: " + e.NextState);
	}
	#endregion

	#region IOnNetworkDisconnect
	/// <summary>
	/// 通信切断された時
	/// </summary>
	public void Disconnect()
	{	
		// 切断時処理
		if (this.State.Disconnected())
		{
			// ログイン試行回数チェック
			if (this.loginCount < LoginRequestCount)
			{
				GUIDebugLog.AddMessage("Disconnect ReConnectStart");
				this.loginCount++;
			}
			else
			{
				// 試行回数を超えているので強制ログイン処理終了
				this.State.Finish();
				this.State = null;
				this.nextState = null;

				// 切断メッセージ表示
				GUISystemMessage.SetModeOK
				(
					MasterData.GetText(TextType.TX029_DisconnectTitle),
					MasterData.GetText(TextType.TX027_Disconnect),
					MasterData.GetText(TextType.TX031_DisconnectOK),
					() =>
					{

						// 切断された場合はタイトル情報画面に飛ばす
						GUITitle.OpenInfo();
					}
				);
			}
		}
	}
	#endregion

	#region IOnNetworkDisconnectByServer
	/// <summary>
	/// サーバからによる通信切断が行われた時
	/// </summary>
	public void DisconnectByServer()
	{
		SceneController.RemoveDisconnect(this);

		// サーバから切断された場合はログイン処理を終了させる
		this.State.Finish();
		this.State = null;
		this.nextState = null;

		// 切断メッセージ表示
		GUISystemMessage.SetModeOK
		(
			MasterData.GetText(TextType.TX029_DisconnectTitle),
			MasterData.GetText(TextType.TX027_Disconnect),
			MasterData.GetText(TextType.TX031_DisconnectOK),
			() =>
			{
				// 切断された場合はタイトル情報画面に飛ばす
				GUITitle.OpenInfo();
			}
		);
	}
	#endregion
}