/// <summary>
/// 弾丸ホーミング
/// 
/// 2013/02/12
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Scm.Common.Master;
using Scm.Common.GameParameter;

public class BulletHomingBase : BulletBase
{
	#region bulletCurveリスト
	static private Vector3 GetBulletCurve(int bulletOffsetID)
	{
		Vector3 curve = new Vector3();
		SkillBulletCurveMasterData bulletCurve;
		if (MasterData.TryGetBulletCurve(bulletOffsetID, out bulletCurve))
		{
			curve.Set(bulletCurve.CurveX, bulletCurve.CurveY, bulletCurve.CurveZ);
			return curve;
		}
		return Vector3.zero;
	}
	#endregion
	
	#region フィールド＆プロパティ
	[SerializeField]
	private float speed;
	public  float Speed { get { return speed; } private set { speed = value; } }
	[SerializeField]
	private float homing;
	public  float Homing { get { return homing; } private set { homing = value; } }
	[SerializeField]
	private float distanceCounter;
	public  float DistanceCounter { get { return distanceCounter; } private set { distanceCounter = value; } }
	[SerializeField]
	private bool isPiercing;
	public  bool IsPiercing { get { return isPiercing; } private set { isPiercing = value; } }
	[SerializeField]
	private bool isWallPiercing;
	public  bool IsWallPiercing { get { return isWallPiercing; } private set { isWallPiercing = value; } }
	[SerializeField]
	private bool isSelfBullet;
	public  bool IsSelfBullet { get { return isSelfBullet; } private set { isSelfBullet = value; } }

	[SerializeField]
	private bool isHoming;
	public  bool IsHoming { get { return isHoming; } private set { isHoming = value; } }

	public Transform TargetNull { get; private set; }
	public Vector3 PrevPosition { get; private set; }
	public System.Action UpdateFunc { get; private set; }
	public Vector3 Curve { get; private set; }
	#endregion

	#region セットアップ
	protected void SetupHoming(Manager manager, ObjectBase target, Vector3? targetPosition, EntrantInfo caster, int skillID, IBulletSetMasterData bulletSet)
	{
		this.SetupBase(manager, target, targetPosition, caster, skillID, bulletSet);

		// 設定
		if (0 != this.Bullet.Speed)
			{ this.Speed = this.Bullet.Speed; }
		if (0 != this.Bullet.Homing)
			{ this.Homing = this.Bullet.Homing; }
		if (0 != this.Bullet.Range)
			{ this.DistanceCounter = this.Bullet.Range; }
		switch(this.Bullet.Type)
		{
		case SkillBulletMasterData.BulletType.Pierce:
		case SkillBulletMasterData.BulletType.SelfPierce:
		case SkillBulletMasterData.BulletType.FallPierce:
			this.IsPiercing = true;
			this.IsWallPiercing = false;
			break;
		case SkillBulletMasterData.BulletType.WallPierce:
		case SkillBulletMasterData.BulletType.WallAlong:	// 壁に当たっても消えない(沿って動く)ためココ.
			this.IsPiercing = true;
			this.IsWallPiercing = true;
			break;
		default:
			this.IsPiercing = false;
			this.IsWallPiercing = false;
			break;
		}
		this.IsSelfBullet = this is BulletSelf;
		this.Curve = GetBulletCurve(bulletSet.BulletOffsetID);
	}
	void Start()
	{
		this.IsHoming = false;
		if (this.Target && (0 != this.Homing))
		{
			this.IsHoming = true;
			// 喰らい判定はグローバル座標＋CharacterController(0,1,0)の位置なのに.
			// ホーミング対象はSpine(Root)Boneになっている.
			this.TargetNull = this.Target.transform.Search(BoneName.GetName(BoneType.Hit_Root));
			if (this.TargetNull == null)
			{
				this.TargetNull = this.Target.transform.Search(BoneName.GetName(BoneType.Root));
				if (this.TargetNull == null)
				{
					this.TargetNull = this.Target.transform;
				}
				Debug.LogWarning(BoneType.Hit_Root + " isn't found in " + this.Target.name + ", so using " + this.TargetNull.name);
			}
		}
		if (this.Rigidbody == null)
		{
			this.Rigidbody = this.gameObject.GetSafeComponentInChildren<Rigidbody>();
		}
		this.Rigidbody.sleepThreshold = 0;	//初期値は0.15なのでそれより遅いと当たらなくなる.

		this.PrevPosition = this.transform.position;
		this.UpdateFunc = this.InitUpdate;

		// TODO:子弾丸生成で衝突した際に少し進んでしまうのを回避
//		this.Update();
	}
	#endregion

	#region BulletBase
	protected override void Update()
	{
		this.UpdateFunc();
	}
	void InitUpdate()
	{
		// 子弾丸生成時で衝突した際に少し進んでしまうのを回避
		this.UpdateFunc = this.UpdateHoming;
	}
	void UpdateHoming()
	{
		base.Update();

		this.SphereCastCollision();

		try{
			// カーブ
			this.transform.Rotate(this.Curve * Time.deltaTime);

			// ホーミングチェック
			if (this.IsHoming)
				this.IsHoming = this.CheckHoming();
	
			// ホーミングさせる
			if (this.IsHoming)
				this.UpdateRotation();
		}catch(Exception e){
			// 仮.
			// ターゲットがいなくなった（現在の場合おそらくログアウト）時にエラーになる.
			// また、リスポーン時もホーミングしっぱなしになっています.
			BugReportController.SaveLogFileWithOutStackTrace(e.ToString());
			this.IsHoming = false;
		}
		
		// 移動する
		Vector3 movement;
		this.UpdatePosition(out movement);

		// 一定距離で消滅
		// 初期化時にタイマーが設定されてなければ何もしない
		if (0f < this.DistanceCounter)
		{
			this.DistanceCounter -= movement.magnitude;
			if (0f >= this.DistanceCounter)
				{ this.DestroyDistance(); }
		}

		// 位置更新
		this.PrevPosition = this.transform.position;
	}
	protected virtual bool SphereCastCollision()
	{
		bool isCollision = false;
		List<RaycastHit> hits;
		if(this.SphereCast(out hits))
		{
			foreach(RaycastHit hitInfo in hits)
			{
				isCollision |= this.CollisionProc(hitInfo.transform.gameObject, hitInfo.point, this.transform.rotation);
			}
		}

		return isCollision;
	}
	protected bool SphereCast(out List<RaycastHit> hits)
	{
		// 位置、方向、距離を求める
		// 距離が全く無ければ当たらない
		Ray ray = new Ray();
		float distance;
		{
			Vector3 direction = this.transform.position - this.PrevPosition;
			distance = direction.magnitude;
			if (distance < 0.001f)
			{
				hits = null;
				return false;
			}
			ray.direction = direction.normalized;
			ray.origin = this.PrevPosition;
		}

		// スフィアを飛ばして当たっているオブジェクトをすべて検出する
		// 距離ソート
		hits = new List<RaycastHit>(Physics.SphereCastAll(ray, this.Radius, distance, this.LayerMask));
		hits.Sort((x, y) => { return GameGlobal.AscendSort(x.distance, y.distance); });

		return true;
	}
	public override bool CollisionProc(GameObject hitObject, Vector3 position, Quaternion rotation)
	{
		// 当たり判定が有効かどうか
		// レイを飛ばした時に同じフレームで複数ヒットする場合があるため
		if (this.Collider && !this.Collider.enabled)
		{
			return false;
		}

		// 当たるかどうかのチェックと
		// 破棄するかどうかのチェック
		bool isDestroy = true;
		bool isDamage = true;
		{
			ObjectBase objectBase = ObjectCollider.GetCollidedObject(hitObject);
			if (objectBase)
			{
				if (!this.ObjectBaseCollision(objectBase, ref isDestroy, ref isDamage))
					{ return false; }
			}
			else
			{
				if (!this.EtcCollision())
					{ return false; }
			}
		}

		// ヒットパケットを送る
		if (isDamage)
		{
			this.SendHitPacket(hitObject, position, rotation, this.transform.rotation.eulerAngles.y);
		}
		else
		{
			this.CreateHitEffect(position, rotation);
		}

		// 破棄
		if (isDestroy)
		{
			this.DestroyHit();
		}

		return true;
	}
	bool ObjectBaseCollision(ObjectBase objectBase, ref bool isDestroy, ref bool isDamage)
	{
		if (objectBase == null)
			{ return false; }

		// タイプ別でヒットさせる
		switch (objectBase.EntrantType)
		{
		case EntrantType.Pc:
		case EntrantType.Npc:
		case EntrantType.MiniNpc:
		case EntrantType.Mob:
		case EntrantType.MainTower:
		case EntrantType.SubTower:
		case EntrantType.Tank:
		case EntrantType.Transporter:
        case EntrantType.Hostage:
			break;
		case EntrantType.Wall:
			// 破壊不能オブジェクトかつ自分弾丸系は無視する
			if (!objectBase.IsBreakable && this.IsSelfBullet)
				{ return false; }
			// 壁貫通弾なら当たらない
			if (this.IsWallPiercing)
				{ return false; }
			break;
		case EntrantType.Barrier:
			// 味方のバリアには当たらない
			if(this.CasterTeamType == objectBase.TeamType)
				{ return false; }
			break;
		default:
			// 上記以外だとヒットしない！
			return false;
		}

		switch (this.Bullet.Attacktype)
		{
		case SkillBulletMasterData.AttackType.None:
			switch (objectBase.EntrantType)
			{
			case EntrantType.Pc:
			case EntrantType.Npc:
			case EntrantType.MiniNpc:
			case EntrantType.Mob:
			case EntrantType.Tank:
            case EntrantType.Hostage:
				return false;
			default:
				isDamage = false;
				break;
			}
			break;
		case SkillBulletMasterData.AttackType.Enemy:
			// 自分自身には当たらない
			if (this.InFieldCasterID == objectBase.InFieldId)
				{ return false; }
			// 味方はダメージを食らわない
			if (this.CasterTeamType == objectBase.TeamType)
			{
				switch (objectBase.EntrantType)
				{
				case EntrantType.Pc:
				case EntrantType.Npc:
				case EntrantType.MiniNpc:
				case EntrantType.Mob:
				case EntrantType.Tank:
					return false;
				default:
					isDamage = false;
					break;
				}
			}
			else
			{
				// 無敵状態の敵PCには当たらない.
				if(objectBase.IsInvincible)
					{ return false; }
			}
			break;
		case SkillBulletMasterData.AttackType.Friend:
			// 敵はダメージを食らわない
			if (this.CasterTeamType != objectBase.TeamType)
				{ isDamage = false; }
			break;
		case SkillBulletMasterData.AttackType.All:
			break;
		}

		// 貫通弾処理
		if (this.IsPiercing)
		{
			// 既にヒットしている
			if (this.PierceingList.Contains(objectBase.InFieldId))
				return false;
			// 初めて食らうのでヒット済みリストに加える
			this.PierceingList.Add(objectBase.InFieldId);

			// タイプ別でヒットさせているので対象タイプのみ処理する
			switch (objectBase.EntrantType)
			{
			case EntrantType.Wall:
				// 破壊可能オブジェクトは貫通させる
				if (objectBase.IsBreakable)
				{
					isDestroy = false;
				}
				break;
			case EntrantType.Barrier:
				// 味方のバリアは貫通可能.それ以外は地形壁に準ずる
				if(this.CasterTeamType == objectBase.TeamType ||
					!this.EtcCollision())
				{
					isDestroy = false;
				}
				break;
			default:
				isDestroy = false;
				break;
			}
		}

		return true;
	}
	bool EtcCollision()
	{
		// 自分弾丸系なら当たらない
		if (this.IsSelfBullet)
			{ return false; }
		// 壁貫通弾なら当たらない
		if (this.IsWallPiercing)
			{ return false; }

		return true;
	}

	protected override void DestroyTimer()
	{
		// ロストエフェクト
		EffectManager.CreateLost(this.transform.position, this.transform.rotation, this.Bullet);

		base.DestroyTimer();
	}
	#endregion

	#region 破棄
	void DestroyHit()
	{
		base.DestroyByHit();

		if (this.Collider)
			this.Collider.enabled = false;
	}
	protected virtual void DestroyDistance()
	{
		// ロストエフェクト
		EffectManager.CreateLost(this.transform.position, this.transform.rotation, this.Bullet);

		base.DestroyByDistance();
	}
	#endregion

	#region チェック
	/// <summary>
	/// ホーミング出来るかどうか
	/// </summary>
	/// <returns></returns>
	bool CheckHoming()
	{
		// ターゲットが居ない
		if (this.Target == null)
			return false;
		// ターゲットが死んでいる
		if (this.Target.StatusType == StatusType.Dead)
			return false;
		// キャラクターの後ろに回ったらホーミングをやめる
		Vector3 pos = this.transform.InverseTransformPoint(this.Target.transform.position);
		if(pos.z < 0f)
			return false;

		return true;
	}
	#endregion

	#region 更新
	/// <summary>
	/// 回転更新
	/// </summary>
	protected virtual void UpdateRotation()
	{
		Vector3 direction = this.TargetNull.position - this.transform.position;
		float t = this.Homing * Mathf.Deg2Rad * Time.deltaTime;
		Vector3 rotation = Vector3.RotateTowards(this.transform.forward, direction, t, 1000f);
		this.transform.rotation = Quaternion.LookRotation(rotation);
	}
	/// <summary>
	/// 位置更新
	/// </summary>
	/// <param name="velocity"></param>
	protected virtual void UpdatePosition(out Vector3 movement)
	{
		this.Rigidbody.velocity = this.transform.forward * this.Speed;
		
		movement = this.transform.position - this.PrevPosition;
	}
	#endregion
}
