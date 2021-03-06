/// <summary>
/// ログインパケット要求クラス
/// 
/// 2015/12/16
/// </summary>
using System;
using System.Collections;
using Scm.Common.Packet;
using Scm.Common.GameParameter;

/// <summary>
/// Responseのイベント引数
/// </summary>
public class LoginRequestEventArgs : EventArgs
{
	public LoginResult result;
	public LoginResult Result { get { return result; } }

	public LoginRequestEventArgs(LoginResult result)
	{
		this.result = result;
	}
}

/// <summary>
/// ログインリクエストインターフェイス
/// </summary>
public interface ILoginRequest
{
	/// <summary>
	/// 送信
	/// </summary>
	void Send();

	/// <summary>
	/// 応答イベント
	/// </summary>
	event EventHandler<LoginRequestEventArgs> ResponseEvent;
}

/// <summary>
/// ログインパケット要求クラス
/// </summary>
public class LoginRequest : ILoginRequest, IPacketResponse<LoginRes>
{
	#region フィールド&プロパティ
	/// <summary>
	/// レスポンスイベント
	/// </summary>
	public event EventHandler<LoginRequestEventArgs> ResponseEvent = (sender, e) => { };
	#endregion

	#region 送信
	/// <summary>
	/// 送信処理
	/// </summary>
	public void Send()
	{
		NetworkController.SendLogin(this);
	}
	#endregion

	#region IPacketResponse
	/// <summary>
	/// サーバからの応答
	/// </summary>
	/// <param name="packet"></param>
	public void Response(LoginRes packet)
	{
		var eventArgs = new LoginRequestEventArgs(packet.LoginResult);
		ResponseEvent(this, eventArgs);
	}
	#endregion
}
