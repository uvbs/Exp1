/// <summary>
/// 掴みアタッチTransform用スクリプト
/// 
/// 2013/10/04 作成
/// 2014/02/13 スクリプトのAdd位置をtargetAttachからcasterAttachに変更
///            (投げ途中で対象が消滅した際に,モーションは終わっていないのに投げ状態が解除されてしまっていたため)
/// </summary>
using UnityEngine;
using System.Collections.Generic;
using Scm.Common.Master;
using Scm.Common.GameParameter;

public class GrappleAttach : MonoBehaviour
{
	public ObjectBase Caster { get; private set; }
	public ObjectBase Target { get; private set; }

	public HitInfo HitInfo { get; private set; }

	public SkillGrappleMasterData GrappleData{ get; private set; }

	private Transform targetAttach;
	private LinkedList<SkillGrappleBulletSetMasterData> bulletList;
	
	private float elapsedTime;

	/// <summary>
	/// 正常終了したかどうか(規定時間が終わり,Targetを吹き飛ばしたかどうか).
	/// </summary>
	public bool IsFinish { get; private set; }

	#region 作成.
	/// <summary>
	/// Hitinfo.bulletIDを見て投げ処理を開始する.
	/// </summary>
	static public GrappleAttach Create(ObjectBase caster, ObjectBase target, HitInfo hitinfo)
	{
		if(caster != null)
		{
			SkillGrappleMasterData grappleData;
			if(SkillGrappleMaster.Instance.TryGetMasterDataBySkillBulletId(hitinfo.bulletID, out grappleData))
			{
				Transform casterAttach = caster.transform.Search(grappleData.OffenseAttachNull);
				if(casterAttach == null)
				{
					casterAttach = caster.transform;
				}
				GrappleAttach newGrapple = casterAttach.gameObject.AddComponent<GrappleAttach>();
				newGrapple.Setup(caster, target, hitinfo, grappleData);

				return newGrapple;
			}
		}
		return null;
	}

	private void Setup(ObjectBase caster, ObjectBase target, HitInfo hitinfo, SkillGrappleMasterData grappleData)
	{
		this.Caster = caster;
		this.Target = target;
		this.GrappleData = grappleData;
		this.HitInfo = hitinfo;
		this.IsFinish = false;
		
		// アタッチ先を取得する.
		this.targetAttach = target.transform.Search(grappleData.DefenseAttachNull);
		if(this.targetAttach == null)
		{
			this.targetAttach = target.transform;
		}
		
		// 投げ中ダメージを設定する（条件はSendHitと同じ）.
		if( (target is Player) || (caster is Player && !(target is Person)))
		{
			List<SkillGrappleBulletSetMasterData> bullets;
			if(SkillGrappleBulletSetMaster.Instance.TryGetChildBulletSet(grappleData.ID, out bullets))
			{
				bullets.Sort((x, y) => {
					if(x.TimeLag < y.TimeLag){ return -1; }
					else{ return 1; }
				});
				this.bulletList = new LinkedList<SkillGrappleBulletSetMasterData>(bullets);
			}
		}

		// 投げ登録.
		BattleMain.Instance.BulletMonitor.AddGrapple(this);

		// 攻撃側モーションの再生.
		this.Caster.Grapple(this);

		// 守備側モーションの再生.
		this.Target.Grapple(this);	// 含 弾丸消滅.

		//player.InvincibleCounter = grappleData.AttachTime;
	}
	#endregion

	#region 初期化
	void Awake()
	{
	}
	void Start()
	{
		// 以前はここで Grapple(), PlayAnimation を行っていたが
		// 同フレーム内で発生した GrappleAttach 同士の実行順が
		// 生成順=パケット受信順 と一致しないため Setup() 内に移動.
	}
	#endregion

	#region 更新
	void Update()
	{
		// 投げ中ダメージの発生.
		if(this.bulletList != null)
		{
			var bullet = bulletList.First;
			while(bullet != null)
			{
				if(elapsedTime < bullet.Value.TimeLag)
				{
					break;
				}
				if(this.Target)
				{
					BattlePacket.SendHit(this.Caster.EntrantInfo, 0, bulletList.First.Value.ChildSkillBulletID, this.Target, this.transform.position, this.HitInfo.bulletDirection);
				}
				bulletList.RemoveFirst();
				bullet = bulletList.First;
			}
		}
		
		// 時間が過ぎたらアタッチを解く.
		if(this.GrappleData.AttachTime <= elapsedTime)
		{
			this.IsFinish = true;
			Destroy();
		}

		// 攻撃側が死んだ時点で投げ中断.
		if(this.Caster.StatusType == StatusType.Dead)
		{
			// 中断.
			Destroy();
		}

		// 経過時間加算.
		elapsedTime += Time.deltaTime;
	}
	
	void LateUpdate()
	{
		// ボーンアタッチ演出.
		if(this.targetAttach != null && this.Target != null)
		{
			this.Target.transform.rotation = this.transform.rotation/* * Quaternion.Euler(attachRotation)*/;
			Vector3 offset = this.transform.position - this.targetAttach.transform.position;
			
			Player player = this.Target as Player;
			if(player != null)
			{
				player.MovePosition(offset, true);
			}
			else
			{
				this.Target.transform.position += offset;
			}
		}
	}
	#endregion

	#region OnDestroy
	public void Destroy()
	{
		Destroy(this);
	}
	
	void OnDestroy()
	{
		if(this.Caster != null)
		{
			// ディレイタイムのセット.
			this.Caster.GrappleFinish(this);
		}
		if(this.Target != null)
		{
			// 吹き飛び.
			this.Target.GrappleFinish(this);
		}
		BattleMain.Instance.BulletMonitor.RemoveGrapple(this);
	}

	#endregion
}
