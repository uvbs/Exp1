/// <summary>
/// テレポータベース
/// 
/// 2013/04/03
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.Master;
using Scm.Common.GameParameter;

public class Transporter : Gadget
{
	#region フィールド
	// Transportに必要な秒数.
	const float RequiredTransportTime = 3f;
	// Transport起動前の待ち時間.
	const float WaitTimeStart = 0f;
	// Transport成功時の待ち時間(短いと送信→受信の間に再度カウントダウンが始まってしまう).
	const float WaitTimeSuccess = 5f;
	// Transport失敗時の待ち時間.
	const float WaitTimeFailure = 0f;

	// Transport起動中のコルーチン管理.
	Fiber transportFiber;
	#endregion

	#region コリジョン
	void OnTriggerEnter(Collider collider)
	{
	}
	void OnTriggerStay(Collider collider)
	{
		if(transportFiber != null && !transportFiber.IsFinished)
		{
			// Transport起動カウント.
			transportFiber.Update();
		}
		else
		{
			// Transport起動カウント開始.
			// OnTriggerEnter()で行わないのは,カウント中にスキル使用などでリセットされた場合に入りなおさなくても再起動させるため.
			Player player = collider.gameObject.GetComponent<Player>();
			if ( player != null &&
				(player.TeamType == this.TeamType || this.TeamType == TeamType.Unknown) &&
				 player.IsTransportable())
			{
				transportFiber = new Fiber(TransportFiber(player, player.Position));
			}
		}
	}
	void OnTriggerExit(Collider collider)
	{
		if(transportFiber != null && !transportFiber.IsFinished &&
			collider.gameObject.GetComponent<Player>() != null)
		{
			// ゲージをリセットしてカウント中断.
			GUITransporterInfo.SetActive(false);
			transportFiber = null;
		}
	}
	#endregion

	#region コルーチン
	IEnumerator TransportFiber(Player player, Vector3 playerPos)
	{
		float remainingTime = RequiredTransportTime + WaitTimeStart;
		while(this != null && player != null && player.IsTransportable())
		{
			remainingTime -= Time.deltaTime;
			if(remainingTime <= 0)
			{
				BattlePacket.SendTransport(this.InFieldId);
				yield return new WaitSeconds(WaitTimeSuccess);
				yield break;
			}
			else if(remainingTime < RequiredTransportTime)
			{
				GUITransporterInfo.SetTimer(RequiredTransportTime*10f, remainingTime*10f);
			}
			yield return null;
		}
		// HACK: 後々リスポンゲージとは別物になる予定.現状だと干渉する可能性アリ.
		GUITransporterInfo.SetActive(false);
		yield return new WaitSeconds(WaitTimeFailure);
		transportFiber = null;
		yield break;
	}
	#endregion
}
