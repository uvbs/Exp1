/// <summary>
/// 弾丸エフェクト
/// 
/// 2012/12/17
/// </summary>
using UnityEngine;
using System.Collections;

using Scm.Common.Master;
using Scm.Common.GameParameter;

public class BulletSelf : BulletHomingBase
{
	#region 静的メソッド
	public static bool Setup(GameObject go, ObjectBase target, Vector3? targetPosition, EntrantInfo caster, int skillID, IBulletSetMasterData bulletSet)
	{
		// コンポーネント取得
		BulletSelf bullet = go.GetSafeComponent<BulletSelf>();
		if (bullet == null)
		{
			GameObject.Destroy(go);
			return false;
		}
		bullet.SetupHoming(null, target, targetPosition, caster, skillID, bulletSet);
		{
			var character = caster.GameObject as Character;
			if (character && character.SkillMotionParam.SelfBulletList != null)
			{
				character.SkillMotionParam.SelfBulletList.Add(bullet);
			}
		}

		// キャスターに親子付けする
		if(caster.GameObject != null)
		{
			bullet.transform.parent = caster.GameObject.transform;
		}
#if UNITY_EDITOR || UNITY_STANDALONE || VIEWER
		GameGlobal.AddCubePolygon(go.transform, bullet.BoxCollider, false);
		GameGlobal.AddSpherePolygon(go.transform, bullet.SphereCollider, false);
#endif

		return true;
	}
	#endregion

	#region BulletBase
	protected override void Destroy()
	{
		var character = this.Caster.GameObject as Character;
		if (character && character.SkillMotionParam.SelfBulletList != null)
		{
			character.SkillMotionParam.SelfBulletList.Remove(this);
		}

		base.Destroy();
	}
	#endregion

	#region BulletHomingBase
	protected override void UpdateRotation()
	{
		Vector3 direction = this.TargetNull.position - this.transform.position;
		float t = this.Homing * Mathf.Deg2Rad * Time.deltaTime;
		Vector3 forward = this.transform.forward;
		Vector3 rotation = Vector3.RotateTowards(forward, direction, t, 1000f);
		rotation.y = 0;
		rotation.Normalize();

		Character character = this.Caster.GameObject as Character;
		if (character != null)
		{
			character.SetRotation(Quaternion.LookRotation(rotation));
			character.SetNextRotation(character.Rotation);
		}
	}
	protected override void UpdatePosition(out Vector3 movement)
	{
		Vector3 forward = this.transform.forward;
//		forward.y = 0;
//		forward.Normalize();
		movement = forward * this.Speed * Time.deltaTime;
		// 移動
		Character character = this.Caster.GameObject as Character;
		if (character != null)
		{
			character.MovePosition(movement);
			character.SetNextPosition(character.Position);
		}
	}
	#endregion
}
