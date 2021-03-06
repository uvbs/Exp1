using System;
using System.Collections;
using Scm.Common.Packet;
using Scm.Common.GameParameter;

/// <summary>
/// Responseイベント引数
/// </summary>
public class CreatePlayerEventArgs : EventArgs
{
	private CreatePlayerResult result;
	public CreatePlayerResult Result { get { return result; } }

	private int playerId;
	public int PlayerId { get { return playerId; } }

	public CreatePlayerEventArgs(CreatePlayerResult result, int playerId)
	{
		this.result = result;
		this.playerId = playerId;
	}
}

/// <summary>
/// プレイヤー生成リクエストインターフェイス
/// </summary>
public interface ICreatePlayerRequest
{
	/// <summary>
	/// 送信
	/// </summary>
	void Send();

	/// <summary>
	/// 応答イベント
	/// </summary>
	event EventHandler<CreatePlayerEventArgs> ResponseEvent;
}

/// <summary>
/// プレイヤー生成パケット要求クラス
/// </summary>
public class CreatePlayerRequest : ICreatePlayerRequest, IPacketResponse<CreatePlayerRes>
{
	#region フィールド&プロパティ
	/// <summary>
	/// 応答イベント
	/// </summary>
	public event EventHandler<CreatePlayerEventArgs> ResponseEvent = (sender, e) => { };
	#endregion

	#region 送信
	public void Send()
	{
		NetworkController.SendCreatePlayer(this);
	}
	#endregion

	#region IPacketResponse
	/// <summary>
	/// サーバからの応答
	/// </summary>
	/// <param name="packet"></param>
	public void Response(CreatePlayerRes packet)
	{
        //  UNDONE: Common.DLL: CreatePlayerEventArgs() の第2引数を変更した場合の影響範囲が広いのでここでキャストしておく
		var eventArgs = new CreatePlayerEventArgs(packet.CreatePlayerResult, (int)packet.Id);
		ResponseEvent(this, eventArgs);
	}
	#endregion
}
