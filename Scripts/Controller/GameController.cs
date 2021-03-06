/// <summary>
/// ゲームコントローラー
/// 
/// 2013/01/10
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.Master;
using Scm.Common.GameParameter;

public class GameController : Singleton<GameController>
{
	#region フィールド＆プロパティ
	private CharacterCamera charaCamera;
	public  CharacterCamera CharaCamera
	{
		get
		{
			if (charaCamera == null)
			{
				charaCamera = Object.FindObjectOfType(typeof(CharacterCamera)) as CharacterCamera;
			}
			return charaCamera;
		}
		set
		{
			charaCamera = value;
		}
	}
	public static CharacterCamera CharacterCamera
	{
		get
		{
			if (Instance == null)
				return null;
			return Instance.CharaCamera;
		}
	}
	private static LayerMask playerLayerMask = 0;
	public  static LayerMask PlayerLayerMask
	{
		get
		{
			if (GameController.playerLayerMask != 0)
			{
				return playerLayerMask;
			}
			for(int i=0; i<32; i++)
			{
				bool isIgnore = Physics.GetIgnoreLayerCollision(LayerNumber.Player, i);
				if (isIgnore)
					{ continue; }
				playerLayerMask |=  (1 << i);
			}
			return playerLayerMask;
		}
	}
	private static LayerMask bulletLayerMask = 0;
	public  static LayerMask BulletLayerMask
	{
		get
		{
			if (GameController.bulletLayerMask != 0)
			{
				return bulletLayerMask;
			}
			for(int i=0; i<32; i++)
			{
				bool isIgnore = Physics.GetIgnoreLayerCollision(LayerNumber.Bullet, i);
				if (isIgnore)
					{ continue; }
				bulletLayerMask |=  (1 << i);
			}
			return bulletLayerMask;
		}
	}
	private static LayerMask meteorLayerMask = 0;
	public  static LayerMask MeteorLayerMask
	{
		get
		{
			if (GameController.meteorLayerMask != 0)
			{
				return meteorLayerMask;
			}
			for(int i=0; i<32; i++)
			{
				bool isIgnore = Physics.GetIgnoreLayerCollision(LayerNumber.IgnoreRaycast, i);
				if (isIgnore)
					{ continue; }
				meteorLayerMask |=  (1 << i);
			}
			return meteorLayerMask;
		}
	}
	#endregion

	#region 破棄
	public void ObjectDestroy()
	{
		MapManager.Instance.DestroyAll();
		PlayerManager.Instance.DestroyAll();
		PersonManager.Instance.DestroyAll();
		ObjectManager.Instance.DestroyAll();
		NpcManager.Instance.DestroyAll();
		EffectManager.Instance.DestroyAll();
		SoundManager.Instance.DestroyAll();
	}
	public void ObjectResetDestroy()
	{
		MapManager.Instance.DestroyAll();
		PlayerManager.Instance.DestroyAll();
		PersonManager.Instance.DestroyAll();
		ObjectManager.Instance.DestroyAll();
		NpcManager.Instance.DestroyAll();
		EffectManager.Instance.DestroyAll();
	}
	public void ObjectBattleDestroy()
	{
		PlayerManager.Instance.DestroyAll();
		PersonManager.Instance.DestroyAll();
		EffectManager.Instance.DestroyAll();
		SoundManager.Instance.DestroyAll();
	}
	#endregion

	#region オブジェクト検索
	/// <summary>
	/// プレイヤー取得
	/// </summary>
	/// <returns></returns>
	public static Player GetPlayer()
	{
		if (PlayerManager.Instance == null)
			return null;
		return PlayerManager.Instance.Player;
	}

    /// <summary>
    /// Test if this player is the room owner: I.E: responsible for all bot's inertia calculations
    /// </summary>
    /// <returns></returns>
    public static bool IsRoomOwner() {
        Player p = GetPlayer();
        if (p == null) {
            return false;
        }
        return Entrant.IsFirstPlayerEntrant(p.InFieldId);
    }

    /// <summary>
    /// フィールド内IDからオブジェクトを検索する
    /// </summary>
    /// <returns></returns>
    /// <param name="inFieldID"></param>
    public static ObjectBase SearchInFieldID(int inFieldID)
	{
		EntrantInfo entrant;
		Entrant.TryGetEntrant(inFieldID, out entrant);

		return entrant != null ? entrant.GameObject : null;
	}
	/// <summary>
	/// ロックオンするターゲットのリストを取得する
	/// </summary>
	/// <returns></returns>
	/// <param name="caster"></param>
	/// <param name="range"></param>
	public static List<ObjectBase> SearchLockonTarget(ObjectBase caster, float range)
	{
		Camera camera = Camera.main;
		if (camera == null)
			return new List<ObjectBase>();

		return Entrant.FindAll((ObjectBase objectBase) => { return GameController.IsLockonTarget(objectBase, caster, camera, range); });
	}
	/// <summary>
	/// ロックオンするターゲットのリストを取得する(右側のみ).
	/// </summary>
	/// <returns></returns>
	/// <param name="caster"></param>
	/// <param name="range"></param>
	public static List<ObjectBase> SearchLockonTargetRight(ObjectBase caster, float range)
	{
		Camera camera = Camera.main;
		if (camera == null)
		{
			return new List<ObjectBase>();
		}

		Vector3 right = camera.transform.right;
		Vector3 position = caster.transform.position;
		return Entrant.FindAll((ObjectBase objectBase) =>
		{
			Vector3 direction = objectBase.transform.position - position;
			return GameController.IsLockonTarget(objectBase, caster, camera, range) && (0 <= Vector3.Dot(right, direction));
		});
	}
	/// <summary>
	/// ロックオンするターゲットのリストを取得する(左側のみ).
	/// </summary>
	/// <returns></returns>
	/// <param name="caster"></param>
	/// <param name="range"></param>
	public static List<ObjectBase> SearchLockonTargetLeft(ObjectBase caster, float range)
	{
		Camera camera = Camera.main;
		if (camera == null)
		{
			return new List<ObjectBase>();
		}

		Vector3 right = camera.transform.right;
		Vector3 position = caster.transform.position;
		return Entrant.FindAll((ObjectBase objectBase) =>
		{
			Vector3 direction = objectBase.transform.position - position;
			return GameController.IsLockonTarget(objectBase, caster, camera, range) && (Vector3.Dot(right, direction) <= 0);
		});
	}
	/// <summary>
	/// ターゲットがロックオンできるかどうか
	/// </summary>
	/// <returns></returns>
	/// <param name="target"></param>
	/// <param name="caster"></param>
	/// <param name="camera"></param>
	/// <param name="range"></param>
	public static bool IsLockonTarget(ObjectBase target, ObjectBase caster, Camera camera, float range)
	{
		return GameController.IsLockonTarget(target, caster, caster.transform.position, camera.transform.forward, camera.fieldOfView, camera.farClipPlane, range);
	}
	public static bool IsLockonTarget(ObjectBase target, ObjectBase caster, Transform point, float angle, float distance, float range)
	{
		return GameController.IsLockonTarget(target, caster, point.position, point.forward, angle, distance, range);
	}
	public static bool IsLockonTarget(ObjectBase target, ObjectBase caster, Vector3 position, Vector3 forward, float angle, float distance, float range)
	{
		// 同じチーム
		if (target.TeamType == caster.TeamType)
			{ return false; }
		// 特定オブジェクト
		switch(target.EntrantType)
		{
		case EntrantType.Unknown:
		case EntrantType.Jump:
		case EntrantType.Item:
		case EntrantType.Wall:
		case EntrantType.Barrier:
		case EntrantType.Start:
		case EntrantType.Respawn:
        case EntrantType.ResidentArea:
			return false;
		case EntrantType.MainTower:
		case EntrantType.SubTower:
		case EntrantType.Tank:
		case EntrantType.Npc:
		case EntrantType.MiniNpc:
		case EntrantType.Mob:
		case EntrantType.Transporter:
        case EntrantType.Hostage:
			if (!target.IsBreakable)
			{
				// 破壊不可能なものを対象から外す
				return false;
			}
			break;
		}
		// 既に死んでいる.
		if (target.StatusType == StatusType.Dead)
			{ return false; }
		// 消えている.
		if (target.IsDisappear)
			{ return false; }
		// 距離の範囲外
		{
			float dis2 = (target.transform.position - caster.transform.position).sqrMagnitude;
			if (dis2 >= range * range)
				{ return false; }
		}
		// 視界の範囲外
		if(!GameGlobal.IsInRange(position, forward, target.transform.position, angle, distance))
			{ return false; }
		return true;
	}
	/// <summary>
	/// ターゲットが投げられるかどうか
	/// </summary>
	public static bool IsGrappleTarget(ObjectBase target, ObjectBase caster, Vector3 position, float range)
	{
		// 同じチーム
		if (target.TeamType == caster.TeamType)
			{ return false; }
		// 特定オブジェクト
		switch(target.EntrantType)
		{
		case EntrantType.Unknown:
		case EntrantType.Jump:
		case EntrantType.Transporter:
		case EntrantType.Item:
		case EntrantType.Wall:
		case EntrantType.Barrier:
		case EntrantType.Start:
		case EntrantType.Respawn:
		case EntrantType.MainTower:
		case EntrantType.SubTower:
		case EntrantType.Npc:
		case EntrantType.Tank:
        case EntrantType.Hostage:   // Grabbing a host may cause bug
            return false;
		case EntrantType.MiniNpc:
		case EntrantType.Mob:
			if (!target.IsBreakable)
			{
				// 破壊不可能なものを対象から外す
				return false;
			}
			break;
		}
		// 距離の範囲外
		{
			float dis2 = (target.transform.position - caster.transform.position).sqrMagnitude;
			if (dis2 >= range * range)
				{ return false; }
		}
		return true;
	}
	/// <summary>
	/// 優先順位に基づいて味方のターゲットを取得する
	/// </summary>
	/// <returns></returns>
	/// <param name="caster"></param>
	/// <param name="bulletRange"></param>
	/// <param name="targetPriority"></param>
	public static ObjectBase SearchFriendPriority(ObjectBase caster, float bulletRange, List<TargetType> targetPriority)
	{
		// 射程内にいる味方をリスト化する
		List<ObjectBase> list = Entrant.FindAll(
			(ObjectBase o) =>
			{
				// 味方じゃない
				if (o.TeamType != caster.TeamType)
					return false;
				// 距離が射程外
				float distance = Vector3.Distance(o.transform.position, caster.transform.position);
				if (distance > bulletRange)
					return false;
				return true;
			}
		);

		// 味方が居ない
		if (0 >= list.Count)
		{
			// 自分にターゲットするかどうか
			int index = targetPriority.IndexOf(TargetType.Self);
			if (0 <= index)
				return caster;
			return null;
		}

		// 昇順ソート
		list.Sort((x, y) => { return GameGlobal.AscendSort(caster.transform.position, x.transform, y.transform); });
		// 優先順位に基いてターゲットを決める
		ObjectBase target = null;
		foreach(TargetType targetType in targetPriority)
		{
			int index = -1;
			switch(targetType)
			{
			case TargetType.Self:
				// 自分自身
				target = caster;
				break;
			case TargetType.Friend_Player:
				// 味方プレイヤー
				index = list.FindIndex(
					(ObjectBase o) => { return o.EntrantType == EntrantType.Pc; }
				);
				break;
			case TargetType.Friend_NPC:
				// 味方NPC
				index = list.FindIndex(
					(ObjectBase o) => { return o.EntrantType == EntrantType.Npc || o.EntrantType == EntrantType.Hostage; }
				);
				break;
			case TargetType.Friend_Tower:
				// 味方タワー
				index = list.FindIndex(
					(ObjectBase o) => { return o.EntrantType == EntrantType.MainTower || o.EntrantType == EntrantType.SubTower; }
				);
				break;
			}
			if (0 > index)
				continue;
			target = list[index];
			break;
		}

		return target;
	}
	/// <summary>
	/// 近接自動ターゲットオブジェクトを取得する
	/// </summary>
	/// <returns></returns>
	/// <param name="caster"></param>
	/// <param name="bulletRange"></param>
	const float NearAngle = 90f;
	public static ObjectBase SearchNearTarget(ObjectBase caster, float bulletRange, float angle = NearAngle)
	{
		// 近場にいる敵ターゲットをリスト化する
		List<ObjectBase> list = Entrant.FindAll(
			(ObjectBase o) => { return GameController.IsLockonTarget(o, caster, caster.transform, angle, bulletRange, bulletRange); }
		);
		// ターゲットが居ない
		if (0 >= list.Count)
		{
			return null;
		}

		// 昇順ソート
		list.Sort((x, y) => { return GameGlobal.AscendSort(caster.transform.position, x.transform, y.transform); });
		ObjectBase target = list[0];

		return target;
	}
	/// <summary>
	/// 投げ技ターゲットオブジェクトを取得する
	/// </summary>
	/// <returns></returns>
	/// <param name="caster"></param>
	/// <param name="bulletRange"></param>
	public static ObjectBase SearchGrappleTarget(ObjectBase caster, float bulletRange)
	{
		// 近場にいる敵ターゲットをリスト化する
		List<ObjectBase> list = Entrant.FindAll(
			(ObjectBase o) => { return GameController.IsGrappleTarget(o, caster, caster.transform.position, bulletRange); }
		);
		// ターゲットが居ない
		if (0 >= list.Count)
		{
			return null;
		}

		// 昇順ソート
		list.Sort((x, y) => { return GameGlobal.AscendSort(caster.transform.position, x.transform, y.transform); });
		ObjectBase target = list[0];

		return target;
	}
	/// <summary>
	/// 近接自動ターゲットオブジェクトを取得する(skillIDから全ての弾丸で一番飛距離が長いものを判定として使う)
	/// </summary>
	/// <returns></returns>
	/// <param name="caster"></param>
	/// <param name="skillID"></param>
	public static ObjectBase SearchNearTarget(ObjectBase caster, SkillMasterData skillData)
	{
		// 最大射程検索
		float bulletRange = 0f;
		{
			// 弾丸セットの中で一番飛距離が長いものを射程とする
			List<SkillBulletSetMasterData> bulletSetList = skillData.BulletSetList;
			foreach (var bulletSet in bulletSetList)
			{
				bulletRange = Mathf.Max(bulletRange, bulletSet.Bullet.Range);
			}
		}
		if (bulletRange <= 0f)
			{ return null; }

		return SearchNearTarget(caster, bulletRange);
	}
	#endregion
}
