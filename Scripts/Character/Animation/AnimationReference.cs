/// <summary>
/// アセットバンドルにモーションを含めるための参照保持クラス.
/// 
/// 2014/06/11
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Master;

public class AnimationReference : ScriptableObject
{
	#region 定義
	[System.Serializable]
	public class AnimationParam
	{
		[SerializeField]
		private float runSpeedMin = 0.1f;
		public float RunSpeedMin { get { return runSpeedMin; } }
		[SerializeField]
		private float runSpeed = 1f;
		public float RunSpeed { get { return runSpeed; } }
	}
	#endregion

	#region フィールド＆プロパティ
	// キャラ接頭語とモーション名の区切り文字
	// (別キャラモーションを流用する場合など,接頭語がキャラIDと一致している保証はない)
	const char MotionNameSeparator = '-';

	/// <summary>
	/// モーション関連のパラメータ.
	/// </summary>
	[SerializeField]
	private AnimationParam animationParam;
	public AnimationParam GetAnimationParam() { return animationParam; }

	/// <summary>
	/// Inspectorで登録されたAnimationClip.
	/// </summary>
	[SerializeField]
	private AnimationClip[] animationClipList;

	/// <summary>
	/// animationClipListにDB登録名から検索できるKeyをつけたDictionary.
	/// </summary>
	private Dictionary<string, AnimationClip> animationClipDic;
	private Dictionary<string, AnimationClip> AnimationClipDic
	{
		get
		{
			if(animationClipDic == null)
			{
				CreateDictionary();
			}
			return animationClipDic;
		}
	}
	#endregion

    public AnimationClip[] AnimationClipList {
        get {
            return animationClipList;
        }
    }

	/// <summary>
	/// MotionNameSeparatorを使ってDBのモーション名からClipを取り出せるDictionaryを作成.
	/// </summary>
	void CreateDictionary()
	{
		animationClipDic = new Dictionary<string, AnimationClip>();
		
		foreach(var animationClip in this.animationClipList)
		{
			try
			{
				string newName = animationClip.name.Substring(animationClip.name.IndexOf(MotionNameSeparator) + 1);
                animationClipDic.Add(newName, animationClip);
            }
			catch(System.Exception e)
			{
				Debug.LogError(e);
				BugReportController.SaveLogFile(e.ToString());
			}
		}
	}

	#region アニメーションクリップ
	/// <summary>
	/// アニメーションパックの読み込み
	/// </summary>
	public void AddAnimationPack(CharacterAnimation characterAnimation, AnimationPack[] packs)
	{
		// 再生中であれば停止する
		characterAnimation.Animation.Stop();

		CheckAnimationLength(characterAnimation, packs);

		// AnimationPack を全て読み込む
		foreach (var pack in packs)
		{
			AnimationClip animationClip;
			if(this.TryGetAnimationClip(pack.ClipName, out animationClip))
			{
				// 同じ登録名があるなら上書き
				Animation animation = characterAnimation.Animation;
				RemoveAnimation(animation, pack.ClipName);
				animation.AddClip(animationClip, pack.ClipName);
				animation[pack.ClipName].wrapMode = pack.WrapMode;
			}
			else
			{
				string eLog = "not found AnimationClip "+characterAnimation.gameObject.name+"@"+pack.ClipName;
				BugReportController.SaveLogFile(eLog);
				Debug.LogError(eLog);
			}
		}
	}

	/// <summary>
	/// アニメーション削除
	/// </summary>
	static public bool RemoveAnimation(Animation animation, string clipName)
	{
		// 存在すれば削除
		if (animation[clipName] == null)
			return false;

		{
			string wLog = "Warning AnimationClip Overlap "+animation.gameObject.name+"@"+clipName;
			Debug.LogWarning(wLog);
			BugReportController.SaveLogFile(wLog);
		}
		// 停止してから削除する
		animation.Stop(clipName);
		animation.RemoveClip(clipName);

		return true;
	}

	/// <summary>
	/// 登録してあるアニメーションクリップからクリップ名で検索して取得する.
	/// </summary>
	private bool TryGetAnimationClip(string name, out AnimationClip outParam)
	{
		return AnimationClipDic.TryGetValue(name, out outParam);
	}

	/// <summary>
	/// animationClipListの参照がnullになっていないかチェックする(主にエディタ用).
	/// </summary>
	public bool CheckAnimationReference(string name)
	{
		bool ret = true;
		foreach(var animationClip in animationClipList)
		{
			if(animationClip == null)
			{
				Debug.LogError("found null AnimationClip : " + name);
				ret = false;
			}
		}

		return ret;
	}

	[System.Diagnostics.Conditional("XW_DEBUG")]
	private void CheckAnimationLength(CharacterAnimation characterAnimation, AnimationPack[] packs)
	{
		if(this.animationClipList.Length != packs.Length)
		{
			Debug.Log(characterAnimation.gameObject.name + " AnimationReference " + this.animationClipList.Length + " != AnimationPack " + packs.Length);
		}
	}

	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	public void SortAnimationClip()
	{
		System.Array.Sort(animationClipList, (a, b) =>
		{
			if(a == null){ return 1; }
			if(b == null){ return -1; }
			return string.Compare(a.name, b.name);
		});
	}
	#endregion
}
