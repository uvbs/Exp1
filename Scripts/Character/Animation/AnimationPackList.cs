//
// 
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Master;

#region AnimationPack
/// <summary>
/// アニメーションパッククラス
/// </summary>
public class AnimationPack
{
	public string ClipName { get; private set; }
	public string AnimName { get; private set; }
	public WrapMode WrapMode { get; private set; }
	public bool IsLoadErrorMsg { get; private set; }

	public AnimationPack(string clipName, string animName, WrapMode wrapMode=WrapMode.Once, bool isLoadErrorMsg=true)
	{
		this.ClipName = clipName;
		this.AnimName = animName;
		this.WrapMode = wrapMode;
		this.IsLoadErrorMsg = isLoadErrorMsg;
	}
}
#endregion

#region AnimationPackList
/// <summary>
/// アニメーションパックリストクラス.
/// </summary>
public static class AnimationPackList
{
	/// <summary>
	/// マスタデータで定義しているラップモードIDをUnityのWrapModeに変換するためのテーブル
	/// </summary>
	public static WrapMode[] MasterDataWrapMode = new WrapMode[]{
		WrapMode.Default,
		WrapMode.Once,
		WrapMode.Loop,
		WrapMode.Default,
		WrapMode.PingPong,
		WrapMode.Default,
		WrapMode.Default,
		WrapMode.Default,
		WrapMode.ClampForever
	};
	
	/// <summary>
	/// Packs the specified avatarType
	/// </summary>
	/// <param name='avatarType'>
	/// AvatarType
	/// </param>
	public static AnimationPack[] Packs(AvatarType avatarType)
	{
		List<AnimationPack> ret = new List<AnimationPack>();
		
		Dictionary<int, AnimationPackMasterData> animationPacks;
		
		// 全キャラ共通アニメーションパック取得
		if (MasterData.TryGetCommonAnimationPack(out animationPacks))
		{
			foreach (var elem in animationPacks.Values)
			{
				ret.Add(new AnimationPack(elem.AnimationName, elem.AnimationName, MasterDataWrapMode[elem.WrapModeID]));
			}
		}
		
		// キャラ固有アニメーションパック取得
		if (MasterData.TryGetAnimationPack(avatarType, out animationPacks))
		{
			foreach (var elem in animationPacks.Values)
			{
				ret.Add(new AnimationPack(elem.AnimationName, elem.AnimationName, MasterDataWrapMode[elem.WrapModeID]));
			}
		}
		
		return ret.ToArray();
	}
}
#endregion