/// <summary>
/// Npc(タンク,タワーなどの移動や攻撃を行うもの)ベース
/// 
/// 2014/07/30
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.Master;

public abstract class NpcBase : Gadget
{
	#region フィールド＆プロパティ
	protected FiberSet fiberSet = new FiberSet();
	public CharacterAnimation Animation { get; protected set; }

	#endregion

	#region 初期化
	protected override void Setup(Manager manager, ObjectMasterData objectData, EntrantInfo info, AssetReference assetReference)
	{
		base.Setup(manager, objectData, info, assetReference);

		this.NextPosition = this.transform.position;
		this.Animation = this.gameObject.GetComponent<CharacterAnimation>();
	}
	#endregion

	#region 更新
	protected virtual void Update()
	{
		fiberSet.Update();
	}
	#endregion

	#region ObjectBase Override
	#region スキルモーション処理
	public override bool SkillMotion(int skillID, ObjectBase target, Vector3 position, Quaternion rotation)
	{
		SkillMasterData skill;
		if(MasterData.TryGetSkill(skillID, out skill))
		{
			this.PlaySkillMotion(skill);
			this.AddSkillEffectFiber(this.fiberSet, skill);
		}
		return SkillMotionBase(skillID, target, position, rotation);
	}
	protected virtual void PlaySkillMotion(SkillMasterData skill)
	{
		if(this.Animation != null)
		{
			if (skill.Motion != null)
			{
				this.Animation.Play(skill.Motion.File, 1);
			}
		}
	}
	protected void AddSkillEffectFiber(FiberSet fiberSet, SkillMasterData skill)
	{
		if (skill.MotionEffectSetList != null)
		{
			foreach (var effectSet in skill.MotionEffectSetList)
			{
				fiberSet.AddFiber(this.SkillMotion_EffectCoroutine(effectSet));
			}
		}
	}
	private IEnumerator SkillMotion_EffectCoroutine(SkillMotionEffectSetMasterData effectSet)
	{
		yield return new WaitSeconds(effectSet.Timig);
		EffectManager.CreateLocus(this, effectSet.Effect);
	}
	#endregion
	#endregion
}
