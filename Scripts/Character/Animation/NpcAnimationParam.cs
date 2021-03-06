using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// NPCのアニメーションに関する設定を保持するクラス.
/// </summary>
static public class NpcAnimationParam
{
	#region Definition
	public enum NpcType
	{
		guard,
		mini,
		mob
	}
	
	public enum MotionState
	{
		wait,
		run,
		rotate,
		skill1,
		skill2,
		skill3,
		skill4,
		dead,
		Max,
		
		skill3_l,
		skill3_r,
		MixingMax
	}
	
	public enum MotionLayer : int
	{
		Move           = -1,
		Action         =  0,
		ActionParts1   =  1, // バルカン.
		ActionParts2   =  2, // ミサイルL.
		ActionParts3   =  3, // ミサイルR.
		ActionParts4   =  4, // フェイス.
		ReAction       =  5,
	}
	
	public class MixingParam
	{
		public MotionState State{ get; private set; }
		public MotionState SourceState{ get; private set; }
		public string MixingTransformName{ get; private set; }
		
		public MixingParam(MotionState state, MotionState sourceState, string mixingTransformName)
		{
			State = state;
			SourceState = sourceState;
			MixingTransformName = mixingTransformName;
		}
	}
	
	public class MotionParam
	{
		public MotionLayer Layer{ get; private set; }
		public float DelayTime{ get; private set; }
		
		public MotionParam(MotionLayer layer, float delayTime)
		{
			Layer = layer;
			DelayTime = delayTime;
		}
	}
	#endregion
	
	#region AnimationPack
	static private readonly AnimationPack[] DefaultPacks = new AnimationPack[(int)MotionState.Max]
	{
		// 共通アクション
		new AnimationPack(MotionState.wait.ToString(),			"wait",			WrapMode.Loop),
		new AnimationPack(MotionState.run.ToString(),			"run",			WrapMode.Loop),
		new AnimationPack(MotionState.rotate.ToString(),		"rotate",		WrapMode.Loop),
		new AnimationPack(MotionState.skill1.ToString(),		"skill1",		WrapMode.Default),
		new AnimationPack(MotionState.skill2.ToString(),		"skill2",		WrapMode.Default, false),
		new AnimationPack(MotionState.skill3.ToString(),		"skill3",		WrapMode.Default, false),
		new AnimationPack(MotionState.skill4.ToString(),		"skill4",		WrapMode.Default, false),
		new AnimationPack(MotionState.dead.ToString(),			"dead",			WrapMode.ClampForever)
	};
	#endregion
	
	#region MixingParam
	static private readonly MixingParam[] MixingParams = new MixingParam[(int)MotionState.MixingMax - (int)MotionState.Max - 1]
	{
		new MixingParam(MotionState.skill3_l, MotionState.skill3, "MainWeapon_L"),
		new MixingParam(MotionState.skill3_r, MotionState.skill3, "MainWeapon_R"),
	};

	#endregion
	
	#region MotionParam
	static private readonly SortedList<string, MotionParam> motionParams = new SortedList<string, MotionParam>();
	static private readonly MotionParam DefaultMotionParam  = new MotionParam(MotionLayer.Move, 0);
	static private void SetMotionParam()
	{
		//motionParams.Add(MotionState.wait.ToString(),	DefaultMotionParam);
		//motionParams.Add(MotionState.run.ToString(),	DefaultMotionParam);
		//motionParams.Add(MotionState.rotate.ToString(),	DefaultMotionParam);
		motionParams.Add(MotionState.skill1.ToString(),	new MotionParam(MotionLayer.Action,			0));
		motionParams.Add(MotionState.skill2.ToString(),	new MotionParam(MotionLayer.ActionParts1,	0));
		motionParams.Add(MotionState.skill3.ToString(),	new MotionParam(MotionLayer.ActionParts2,	0));
		motionParams.Add(MotionState.skill4.ToString(),	new MotionParam(MotionLayer.ActionParts4,	0));
		motionParams.Add(MotionState.dead.ToString(),		new MotionParam(MotionLayer.ReAction,		0));
		//motionParams.Add(MotionState.Max.ToString(),	DefaultMotionParam);
		motionParams.Add(MotionState.skill3_l.ToString(),	new MotionParam(MotionLayer.ActionParts2, 0));
		motionParams.Add(MotionState.skill3_r.ToString(),	new MotionParam(MotionLayer.ActionParts3, 0));
	}
	#endregion
	
	static NpcAnimationParam()
	{
		SetMotionParam();
	}
	
	#region public Method
	static public AnimationPack[] getAnimationPacks(NpcType npcType)
	{
		return DefaultPacks;
	}
	static public MixingParam[] getMixingParams(NpcType npcType)
	{
		return MixingParams;
	}
	static public MotionParam getMotionParam(NpcType npcType, string clipName)
	{
		MotionParam mParam;
		if(motionParams.TryGetValue(clipName, out mParam))
		{
			return mParam;
		}
		return DefaultMotionParam;
	}
	#endregion
}
