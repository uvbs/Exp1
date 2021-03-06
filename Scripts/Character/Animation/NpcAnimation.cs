/// <summary>
/// Npcアニメーション.
/// 
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NpcAnimation : CharacterAnimation
{
	const float fadeTime = 0.3f;
	public NpcAnimationParam.NpcType npcType = NpcAnimationParam.NpcType.guard;
	[SerializeField]
	private string clipPath;

	public bool IsActive { get; protected set; }

	#region 初期化
	protected void Start()
	{
		// アニメーション読み込み.
		AnimationPack[] packs =  NpcAnimationParam.getAnimationPacks(this.npcType);
		// AnimationPack がない場合は何もしない
		if (0 < packs.Length)
		{
			this.StartCoroutine(LoadAnimationAssetBundle(packs));
		}
	}

	#region モーション読み込み.
	IEnumerator LoadAnimationAssetBundle(AnimationPack[] packs)
	{
		Npc npc = this.gameObject.GetSafeComponent<Npc>();
		while(npc.MainAssetBundle == null)
		{
			yield return null;
		}
		while(this.Animation == null)
		{
			this.SetAnimation(this.gameObject.GetComponentInChildren<Animation>());
			yield return null;
		}

		/* 非同期だとＴ字状態がはっきり見えてしまう.
		player.MainAssetBundle.GetAssetAsync<AnimationReference>(NpcName.AnimationPath, (AnimationReference animationReference) =>
		{
			if(this != null)
			{
				// 読み込みを開始するのでアニメーションを非アクティブにする
				this.IsActive = false;
		
				animationReference.AddAnimationPack(this, packs);
		
				// 全て終了したのでアクティブにする
				this.IsActive = true;
			}
		});
		*/

		// ひとまず同期読みで対応.最終的には読み終わりまで表示しない(表示スクリプトを止めるorモデル読みより先に読んでおく)が必要.
		AnimationReference animationReference = npc.MainAssetBundle.GetAsset<AnimationReference>(NpcName.AnimationPath);
		if(animationReference != null)
		{
			// 読み込みを開始するのでアニメーションを非アクティブにする
			this.IsActive = false;

			this.SetAnimationParam(animationReference.GetAnimationParam());
			animationReference.AddAnimationPack(this, packs);

			// 全て終了したのでアクティブにする
			this.IsActive = true;
			this.MotionMixing();
		}
	}
	#endregion

	// モーション合成.
	private void MotionMixing()
	{
		// 初期アニメーションの設定.
		AnimationFade(NpcAnimationParam.MotionState.wait);

		foreach(NpcAnimationParam.MixingParam mParam in NpcAnimationParam.getMixingParams(npcType))
		{
			AnimationState sourceState = this.Animation[mParam.SourceState.ToString()];
			if(sourceState == null)
			{
				// AnimationStateが存在しない.
				//Debug.Log("not found AnimationState["+mParam.SourceState+"]");
			}
			else
			{
				Transform mixTF = gameObject.transform.Search(mParam.MixingTransformName);
				if(mixTF == null)
				{
					// Transformが存在しない.
					Debug.Log("not found Transform["+mParam.MixingTransformName+"]");
				}
				else
				{
					// 新アニメーションとして登録.
					this.Animation.AddClip(sourceState.clip, mParam.State.ToString());
					this.Animation[mParam.State.ToString()].blendMode = AnimationBlendMode.Blend;
					this.Animation[mParam.State.ToString()].wrapMode = sourceState.wrapMode;

					// アニメーション適用ボーンを設定.
					this.Animation[mParam.State.ToString()].AddMixingTransform(mixTF);	// おそらくここでアニメーション作り直しっぽい動作をする。重い.
				}
			}
			continue;
		}
	}
	#endregion

	#region public Mothod
	/// <summary>
	/// Animations the fade.
	/// </summary>
	public void AnimationFade(NpcAnimationParam.MotionState motionState)
	{
		AnimationFade(motionState.ToString());
	}
	private void AnimationFade(string clipName)
	{
		if(IsActive)
		{
			NpcAnimationParam.MotionParam mParam = NpcAnimationParam.getMotionParam(npcType, clipName);
			this.PlayFade(clipName, (int)mParam.Layer, fadeTime, PlayMode.StopSameLayer);
		}
	}

	/// <summary>
	/// Animations the nofade.
	/// </summary>
	public void AnimationCut(NpcAnimationParam.MotionState motionState)
	{
		AnimationCut(motionState.ToString());
	}
	public void AnimationCut(string clipName)
	{
		if(IsActive)
		{
			NpcAnimationParam.MotionParam mParam = NpcAnimationParam.getMotionParam(npcType, clipName);
			this.Play(clipName, (int)mParam.Layer, PlayMode.StopSameLayer);
			if (this.Animation[clipName])
			{
				this.Animation[clipName].time = 0;
			}
		}
	}

	public void DelayedAnimationAttack(NpcAnimationParam.MotionState motionState, float delayTime)
	{
		StartCoroutine(this.AnimationAttackCoroutine(motionState ,delayTime));
	}
	IEnumerator AnimationAttackCoroutine(NpcAnimationParam.MotionState motionState, float time)
	{
		yield return new WaitForSeconds(time);
		AnimationCut(motionState);
	}

	public float GetAnimationLength(NpcAnimationParam.MotionState motionState)
	{
		return this.GetAnimationLength(motionState.ToString());
	}
	#endregion
}
