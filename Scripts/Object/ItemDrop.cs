/// <summary>
/// アイテムドロップ.
/// 
/// 2013/05/16
/// </summary>
using UnityEngine;

public class ItemDrop : ItemDropBase
{
	#region 定位.
	private const float SqrHitSize = 2.0f * 2.0f;
	private const float Small = 4.0f;
	#endregion
	
	#region フィールド&プロパティ.
	[SerializeField]
	private float gravity = -9.8f;
	[SerializeField]
	private float velocityY = 10f;
	[SerializeField]
	private byte bouncingMax = 1;
	[SerializeField]
	private float bouncing = 0.8f;
	private float startPosY;
	private float startTime;
	private byte bouncingCnt = 0;
	private float scale = 1.0f;
	private bool isTimeout = false;
	#endregion
	
	#region ItemBase Override
	public override void DestroyAnimation()
	{
		Entrant.RemoveEntrant(this.EntrantInfo, false);
		this.isTimeout = true;
	}
	#endregion

	#region メソッド
	void Start()
	{
		this.startPosY = this.transform.position.y;
		this.startTime = Time.time;
		this.IsHit = false;
	}
	
	void Update()
	{
		// ドロップ時のアニメーション.
		DropAnimation();
				
		if(this.isTimeout)
		{
			// 自然消滅時の処理.
			this.TimeoutDestroy();
		}
		else
		{
			// 衝突判定.
			this.Hit();
		}
	}
	
	/// <summary>
	/// ドロップ時のアニメーション処理.
	/// </summary>
	private void DropAnimation()
	{
		if(this.bouncingMax > bouncingCnt)
		{
			float time = Time.time - startTime;
			float moveY = (this.startPosY + (this.velocityY * time) + (0.5f * this.gravity * (time*time)));
			if(moveY < this.startPosY)
			{
				// 初期位置より下に行くと初期化する.
				moveY = this.startPosY;
				this.startTime = Time.time;
				if(this.bouncingCnt == 0)	this.IsHit = true;
				this.bouncingCnt++;
				this.velocityY *= this.bouncing;
			}
			this.transform.position = new Vector3(this.transform.position.x, moveY, this.transform.position.z);
		}
	}
	
	/// <summary>
	/// Y座標無視の衝突判定を行う処理.
	/// </summary>
	private void Hit()
	{
		if(this.IsHit)
		{
			// プレイヤー検索.
			Player player = GameController.GetPlayer();
			if (player == null) return;
			
			// Y座標無視でプレイヤーとの判定を行う.
			Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
			Vector2 itemPos = new Vector2(this.transform.position.x, this.transform.position.z);
			if(Vector2.SqrMagnitude(playerPos-itemPos) < SqrHitSize)
			{
				// 取得すればサーバにアイテム取得パケットを送信.
				this.IsHit = false;
				BattlePacket.SendItemGet(this.InFieldId);	
			}
		}
	}

	/// <summary>
	/// 制限時間による削除処理.
	/// </summary>
	private void TimeoutDestroy()
	{
		// サイズを徐々に小さくする.
		this.transform.localScale = new Vector3(this.scale, this.scale, this.scale);
		this.scale -= Small * Time.deltaTime;
		if(this.scale < 0)
		{
			this.isTimeout = false;
			this.Destroy();
		}
		
	}
	#endregion
}
