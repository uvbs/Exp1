/// <summary>
/// 弾丸監視クラス
/// 
/// 2014/01/23
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Master;

public class BulletMonitor
{
	#region 内部クラス定義

	/// <summary>
	/// 生成待ち子弾丸.
	/// </summary>
	private class WaitingChildBullet
	{
		// 子弾丸データ.
		public EntrantInfo Caster { get; private set; }
		public ObjectBase Target { get; private set; }
		public Vector3 Position { get; private set; }
		public Quaternion Rotation { get; private set; }
		public Vector3 Scale { get; private set; }
		public int SkillID { get; private set; }
		public SkillBulletBulletSetMasterData ChildBulletSet { get; private set; }
		
		/// <summary>
		/// 生成するかどうか.
		/// </summary>
		public bool Enable { get; private set; }
		
		public WaitingChildBullet(EntrantInfo caster, ObjectBase target, Vector3 position, Quaternion rotation, Vector3 scale, int skillID, SkillBulletBulletSetMasterData childBulletSet)
		{
			this.Caster = caster;
			this.Target = target;
			this.Position = position;
			this.Rotation = rotation;
			this.Scale = scale;
			this.SkillID = skillID;
			this.ChildBulletSet = childBulletSet;
			
			this.Enable = true;
		}
		
		public void DestroyByCaster()
		{
			if(this.ChildBulletSet.Bullet.IsVanishable)
			{
				this.Enable = false;
			}
		}
	}

	#endregion

	#region フィールド＆プロパティ

	/// <summary>
	/// 弾丸リスト.
	/// </summary>
	private Dictionary<int, LinkedList<BulletBase>> bulletDic = new Dictionary<int, LinkedList<BulletBase>>();

	/// <summary>
	/// 子弾丸生成待ちリスト.
	/// </summary>
	private Dictionary<int, LinkedList<WaitingChildBullet>> waitingChildBulletDic = new Dictionary<int, LinkedList<WaitingChildBullet>>();

	/// <summary>
	/// 掴みアタッチリスト.
	/// </summary>
	private Dictionary<int, GrappleAttach> grappleAttachDic = new Dictionary<int, GrappleAttach>();
	#endregion
	
	#region method

	/// <summary>
	/// 弾丸リストを初期化する.
	/// </summary>
	public void Clear()
	{
		bulletDic.Clear();
		waitingChildBulletDic.Clear();
		grappleAttachDic.Clear();
	}

	#region 弾丸
	public void AddBullet(BulletBase bullet)
	{
		if(bullet.Caster != null)
		{
			LinkedList<BulletBase> bulletList;
			if(!bulletDic.TryGetValue(bullet.Caster.InFieldId, out bulletList))
			{
				bulletList = new LinkedList<BulletBase>();
				bulletDic[bullet.Caster.InFieldId] = bulletList;
			}
			bulletList.AddLast(bullet);
		}
	}
	public void RemoveBullet(BulletBase bullet)
	{
		if(bullet.Caster != null)
		{
			LinkedList<BulletBase> bulletList;
			if(bulletDic.TryGetValue(bullet.Caster.InFieldId, out bulletList))
			{
				bulletList.Remove(bullet);
			}
		}
	}
	#endregion
	
	#region 子弾丸
	/// <summary>
	/// 子弾丸を ChildBulletSet.TimeLag 秒後に生成する.
	/// </summary>
	public void AddChildBullet(EntrantInfo caster, ObjectBase target, Vector3 position, Quaternion rotation, Vector3 scale, int skillID, SkillBulletBulletSetMasterData childBulletSet)
	{
		if(0f < childBulletSet.ShotTiming)
		{
			// Waitリストに追加.
			LinkedList<WaitingChildBullet> wcbList;
			if(!waitingChildBulletDic.TryGetValue(caster.InFieldId, out wcbList))
			{
				wcbList = new LinkedList<WaitingChildBullet>();
				waitingChildBulletDic[caster.InFieldId] = wcbList;
			}
			wcbList.AddLast(new WaitingChildBullet(caster, target, position, rotation, scale, skillID, childBulletSet));
			BattleMain.Instance.StartCoroutine(CreateChildBulletCoroutine(wcbList.Last));
		}
		else
		{
			// 子弾丸を即時作成.
			CreateChildBullet(caster, target, position, rotation, scale, skillID, childBulletSet);
		}
	}
	
	/// <summary>
	/// 子弾丸生成待ち.
	/// </summary>
	IEnumerator CreateChildBulletCoroutine(LinkedListNode<WaitingChildBullet> wcbNode)
	{
		var wcb = wcbNode.Value;
		
		yield return new WaitForSeconds(wcb.ChildBulletSet.ShotTiming);
				
		if(wcb.Enable)	// 怯みなどによるキャンセルが無かったか.
		{
			// 子弾丸を生成.
			CreateChildBullet(wcb.Caster, wcb.Target, wcb.Position, wcb.Rotation, wcb.Scale, wcb.SkillID, wcb.ChildBulletSet);
		}
		
		// Waitリストから消去.
		LinkedList<WaitingChildBullet> wcbList = wcbNode.List;
		if(wcbList != null)
		{
			wcbList.Remove(wcbNode);	// nodeからの消去はO(1).
		}
	}
	
	/// <summary>
	/// 子弾丸を生成.
	/// </summary>
	private void CreateChildBullet(EntrantInfo caster, ObjectBase target, Vector3 position, Quaternion rotation, Vector3 scale, int skillID, SkillBulletBulletSetMasterData childBulletSet)
	{
		if(caster != null)
		{
			// 発射位置と角度補正.
			// 自分弾丸系から子弾丸を生成すると
			// キャラクターのスケール値が入っているのでそれを使用しないようにする
			GameGlobal.AddOffset(childBulletSet, ref position, ref rotation, Vector3.one);
	
			// 弾丸生成.
			if(caster.GameObject != null)
			{
				caster.GameObject.CreateBullet(target, null, position, rotation, skillID, childBulletSet);
			}
			else
			{
				CreateBullet(caster, target, null, position, rotation, skillID, childBulletSet);
			}
		}
	}

	// HACK: 応急処置.casterのObjectBaseが既に消去済みの場合に子弾丸を生成する.
	// 本来はObjectBaseのデータを保持するクラス(現在はEntrantInfoクラス)で行うべき.
	// ObjectBase.CreateBullet()は消去.
	private void CreateBullet(EntrantInfo caster, ObjectBase target, Vector3? targetPosition, Vector3 position, Quaternion rotation, int skillID, SkillBulletBulletSetMasterData childBulletSet)
	{
		SkillBulletMasterData bullet = childBulletSet.Bullet;
		if (bullet == null)
			return;
		// 弾丸発射音
		SoundController.CreateSeObject(position, rotation, bullet.ShotSeFile);

		// 子弾丸は通信を行わない.

		// 弾丸生成
		switch (bullet.Type)
		{
		case SkillBulletMasterData.BulletType.Normal:
		case SkillBulletMasterData.BulletType.Pierce:
		case SkillBulletMasterData.BulletType.WallPierce:
			EffectManager.CreateBulletAmmo(caster, target, targetPosition, position, rotation, skillID, childBulletSet);
			break;
		case SkillBulletMasterData.BulletType.Birth:
		case SkillBulletMasterData.BulletType.Nearest:
			EffectManager.CreateBulletBirth(caster, target, targetPosition, position, rotation, skillID, childBulletSet);
			break;
		// 本体がいないので自分弾丸系は無視する.
		//case SkillBulletMasterData.BulletType.SelfNormal:
		//case SkillBulletMasterData.BulletType.SelfPierce:
		//	this.CreateBulletSelf(target, position, rotation, skillID, bulletSet);
		//	break;
		case SkillBulletMasterData.BulletType.FallNormal:
		case SkillBulletMasterData.BulletType.FallPierce:
			EffectManager.CreateBulletFall(caster, target, targetPosition, position, rotation, skillID, childBulletSet);
			break;
		case SkillBulletMasterData.BulletType.WallAlong:
			EffectManager.CreateBulletAlong(caster, target, targetPosition, position, rotation, skillID, childBulletSet);
			break;
		}
	}
	#endregion

	#region 掴みアタッチ.
	public void AddGrapple(GrappleAttach grapple)
	{
		if(grapple.Caster != null)
		{
			// Casterの以前の投げが続行中の場合は中断.
			GrappleAttach oldGrapple;
			if(grappleAttachDic.TryGetValue(grapple.Caster.InFieldId, out oldGrapple))
			{
				oldGrapple.Destroy();
			}
			// Casterの新しい投げとして登録.
			grappleAttachDic[grapple.Caster.InFieldId] = grapple;
		}
	}
	public void RemoveGrapple(GrappleAttach grapple)
	{
		if(grapple.Caster != null)
		{
			GrappleAttach oldGrapple;
			if(grappleAttachDic.TryGetValue(grapple.Caster.InFieldId, out oldGrapple))
			{
				if(oldGrapple == grapple)
				{
					grappleAttachDic.Remove(grapple.Caster.InFieldId);
				}
			}
		}
	}
	#endregion

	#region 消滅.
	public void DestroyBullets(ObjectBase caster)
	{
		if(caster != null)
		{
			// 弾丸.
			LinkedList<BulletBase> bulletList;
			if(bulletDic.TryGetValue(caster.InFieldId, out bulletList))
			{
				LinkedListNode<BulletBase> bulletNode = bulletList.First;
				while(bulletNode != null)
				{
					if(bulletNode.Value != null)
					{
						bulletNode.Value.DestroyByCaster();
					}
					bulletNode = bulletNode.Next;
				}
				bulletList.Clear();
			}

			// 子弾丸.
			LinkedList<WaitingChildBullet> wcbList;
			if(waitingChildBulletDic.TryGetValue(caster.InFieldId, out wcbList))
			{
				foreach(var wcb in wcbList)
				{
					wcb.DestroyByCaster();
				}
				wcbList.Clear();
			}

			// 掴みアタッチ.
			GrappleAttach grapple;
			if(grappleAttachDic.TryGetValue(caster.InFieldId, out grapple))
			{
				if(grapple != null)
				{
					grapple.Destroy();
				}
			}
		}
	}
	#endregion
	#endregion
}
