/// <summary>
/// 全保有プレイヤー情報取得パケット要求クラス
/// 
/// 2015/12/18
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Packet;

/// <summary>
/// Responseイベント引数
/// </summary>
public class RetainPlayerAllEventArgs : EventArgs
{
	private List<RetainPlayerInfo> retainPlayerInfoList;
	public List<RetainPlayerInfo> RetainPlayerInfoList { get { return retainPlayerInfoList; } }

	public RetainPlayerAllEventArgs(List<RetainPlayerInfo> infoList)
	{
		retainPlayerInfoList = infoList;
	}
}

/// <summary>
/// 全保有プレイヤー情報取得リクエストインターフェイス
/// </summary>
public interface IRetainPlayerAllRequest
{
	/// <summary>
	/// 送信
	/// </summary>
	void Send();

	/// <summary>
	/// 応答イベント
	/// </summary>
	event EventHandler<RetainPlayerAllEventArgs> ResponseEvent;
}

/// <summary>
/// 全保有プレイヤー情報取得パケット要求クラス
/// </summary>
public class RetainPlayerAllRequest : IRetainPlayerAllRequest, IPacketResponse<RetainPlayerAllRes>
{
	#region フィールド&プロパティ
	/// <summary>
	/// 応答イベント
	/// </summary>
	public event EventHandler<RetainPlayerAllEventArgs> ResponseEvent = (sender, e) => { };
	#endregion

	#region 送信
	/// <summary>
	/// 送信処理
	/// </summary>
	public void Send()
	{
		NetworkController.SendRetainPlayerAll(this);
	}
	#endregion

	#region IPacketResponse
	/// <summary>
	/// サーバからの応答
	/// </summary>
	/// <param name="packet"></param>
	public void Response(RetainPlayerAllRes packet)
	{
		var retainPlayerInfoList = new List<RetainPlayerInfo>();
		foreach(var info in packet.GetPlayerPackets())
		{
			retainPlayerInfoList.Add(new RetainPlayerInfo(info));
		}

		var eventArgs = new RetainPlayerAllEventArgs(retainPlayerInfoList);
		ResponseEvent(this, eventArgs);
	}
	#endregion
}
