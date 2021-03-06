/// <summary>
/// サイコマンダーズベースアニメーション
/// 
/// 2012/12/03
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.GameParameter;
using Scm.Common.Master;

public class ScmAnimation : CharacterAnimation
{
	#region フィールド＆プロパティ
	public bool IsActive { get; protected set; }

	// 合成アニメーションのルートボーン.
	static readonly string[] boneList = new string[]{"Spine", "Bit_Root"};	// 上半身, 小物.

	// 何度も使う準定数なため,あらかじめキャッシュ.
	static readonly string MotionName_Wait = MotionName.GetName(MotionState.wait);

	private const string CombinedAnimSuffix = "_Spine";
	LayerAnimation moveLayerAnimation;
	LayerAnimation actionLayerAnimation;

	// モーション合成ファイバー.
	FiberSet motionFiber = new FiberSet();
	#endregion
	
	#region MonoBehaviourリフレクション
	void Awake()
	{
		moveLayerAnimation.clipName = MotionName_Wait;
		moveLayerAnimation.layer = (int)MotionLayer.Move;
		moveLayerAnimation.fadeLength = 0.3f;
		moveLayerAnimation.playMode = PlayMode.StopSameLayer;

		actionLayerAnimation.clipName = string.Empty;
		actionLayerAnimation.layer = (int)MotionLayer.Action;
		actionLayerAnimation.fadeLength = 0.3f;
		actionLayerAnimation.playMode = PlayMode.StopSameLayer;
	}
	public void Setup(Animation animation)
	{
		base.SetAnimation(animation);
	}

	#region モーション読み込み.
	public bool LoadAnimationAssetBundle(Character character, AnimationReference animationReference)
	{
		// アニメーション設定読み込み.
		AvatarType avatarType = character.AvatarType;
		AnimationPack[] packs = AnimationPackList.Packs(avatarType);
		// AnimationPack がない場合は何もしない.
		if (packs.Length < 1)
		{
			return false;
		}

		// アニメーション読み込み.
		if(animationReference != null && this.Animation != null)
		{
			// 読み込みを開始するのでアニメーションを非アクティブにする.
			this.IsActive = false;

			this.SetAnimationParam(animationReference.GetAnimationParam());
			animationReference.AddAnimationPack(this, packs);

			// 全て終了したのでアクティブにする.
			this.IsActive = true;
			this.MotionMixing(avatarType);
		}
		return true;
	}

    //LWZ:add for create chara model
    public bool LoadAnimationAssetBundle(CharaInfo charaInfo, AnimationReference animationReference)
    {
        // アニメーション設定読み込み.
        AvatarType avatarType = charaInfo.AvatarType;
        AnimationPack[] packs = AnimationPackList.Packs(avatarType);
        // AnimationPack がない場合は何もしない.
        if (packs.Length < 1)
        {
            return false;
        }

        // アニメーション読み込み.
        if (animationReference != null && this.Animation != null)
        {
            // 読み込みを開始するのでアニメーションを非アクティブにする.
            this.IsActive = false;

            this.SetAnimationParam(animationReference.GetAnimationParam());
            animationReference.AddAnimationPack(this, packs);

            // 全て終了したのでアクティブにする.
            this.IsActive = true;
            this.MotionMixing(avatarType);
        }
        return true;
    }
    #endregion

    private void MotionMixing(AvatarType avatarType)
	{
		// 対象のボーンを検索.
		List<Transform> transformList = new List<Transform>();
		foreach(string b in boneList)
		{
			Transform t = this.gameObject.transform.Search(b);
			if(t == null)
			{
				Debug.LogWarning("bone not found : " + this.gameObject.name + " [" + avatarType + "] " + b);
				continue;
			}
			transformList.Add(gameObject.transform.Search(b));
		}

		// 合成用上半身モーションを生成
		List<string> animStateNameList = new List<string>();
		{
			// 全キャラ共通.
			Dictionary<int, AnimationPackMasterData> dic;
			if (MasterData.TryGetCommonUpperCombineAnimationPack(out dic))
			{
				foreach (var elem in dic.Values)
				{
					animStateNameList.Add(elem.AnimationName);
				}
			}
			// キャラ個別.
			if (MasterData.TryGetUpperCombineAnimationPack(avatarType, out dic))
			{
				foreach (var elem in dic.Values)
				{
					animStateNameList.Add(elem.AnimationName);
				}
			}
		}
		
		foreach(string animStateName in animStateNameList)
		{
			AnimationState state = this.Animation[animStateName];
			if(state == null)
			{
				// なぜか読み込めていない
				BugReportController.SaveLogFile("not found "+animStateName+" motion");
				continue;	// 現在ないものはスキップ.
			}
			else
			{
				string CombinedAnimName = animStateName + CombinedAnimSuffix;
				// 新アニメーションとして登録
				this.Animation.AddClip(state.clip, CombinedAnimName);
				this.Animation[CombinedAnimName].layer = (int)MotionLayer.Action;
				this.Animation[CombinedAnimName].blendMode = AnimationBlendMode.Blend;
				this.Animation[CombinedAnimName].wrapMode = this.Animation[animStateName].wrapMode;
			
				// アニメーション適用ボーンを設定
				foreach(Transform t in transformList)
				{
					this.Animation[CombinedAnimName].AddMixingTransform(t);	// おそらくここでアニメーション作り直しっぽい動作をする。重い.
				}
				continue;
			}
		}

        /* 新アニメーションがないキャラへの対応.
		MotionState[] newRunAnimStateList = {   MotionState.run_001_f,
												MotionState.run_001_b,
												MotionState.run_001_r,
												MotionState.run_001_l};
		AnimationState oldAnim = this.Animation[MotionName.GetName(MotionState.run)];
		if(oldAnim != null)
		{
			foreach(MotionState newRunAnimState in newRunAnimStateList)
			{
				string newAnimStateName = MotionName.GetName(newRunAnimState);
				if(this.Animation[newAnimStateName] == null)
				{
					this.Animation.AddClip(oldAnim.clip, newAnimStateName);
					this.Animation[newAnimStateName].wrapMode = oldAnim.wrapMode;
				}
			}
		}
		*/

        // アニメーションイベントの設定.
        //StartCoroutine(SetAnimationEvent(avatarType));
        Net.Network.Instance.StartCoroutine(SetAnimationEvent(avatarType));
    }
	private IEnumerator SetAnimationEvent(AvatarType avatarType)
	{
        Debug.Log("Starting coroutine.");
		/* ReferenceDataなどで読んだTransformは一度描画するまでAnimationから認識されない？.
		 * Animation.AddClipやAnimationClip.AddEventなどを呼ぶとTransformを探索し直してくれる模様.
		 * そのため応急処置としてアニメーションイベントの設定をLateUpdateのタイミングまで遅延.
		 * 例)実機,またはアセットバンドル環境でp018式神ようこの狐パーツが高確率で動かなくなる.
		 */
		yield return new WaitForEndOfFrame();

		AnimationAct.SetAnimationEvent(avatarType, this.Animation);
	}
	#endregion

	#region 移動モーション更新
	public void UpdateMoveAnimation(MotionState motionState, float animationSpeedRatio, int layer, float fadeLength=0.2f, PlayMode playMode=PlayMode.StopSameLayer)
	{
		string clipName = MotionName.GetName(motionState);

		moveLayerAnimation.clipName = clipName;
		moveLayerAnimation.fadeLength = fadeLength;
		moveLayerAnimation.layer = layer;
		moveLayerAnimation.playMode = playMode;

		if (!this.IsActive)
			return;
		if (this.Animation == null)
			return;

		AnimationState animationState = this.Animation[clipName];
		if (animationState != null)
		{
			switch(motionState)
			{
			default:
				return;
			case MotionState.wait:
				animationState.speed = 1f;
				break;
			case MotionState.run:
			case MotionState.run_001_f:
			case MotionState.run_001_b:
			case MotionState.run_001_r:
			case MotionState.run_001_l:
				animationState.speed = Mathf.Max(this.RunSpeed * animationSpeedRatio, this.RunSpeedMin);
				break;
			}
		}
	}
	#endregion

	#region 仮想メソッド
	public virtual AnimationState GetAnimationState(string motionState)
	{
		if (this.Animation == null)
			return null;
		return this.Animation[motionState];
	}
	public virtual float GetMotionTime(MotionState motionState)
	{
		string clipName = MotionName.GetName(this.ReplaceMotionState(motionState));
		AnimationState s = GetAnimationState(clipName);
		if (s == null)
			return 0f;
		return s.length;
	}

	public virtual void UpdateAnimation(MotionState motionState, int layer, float fadeLength=0f, PlayMode playMode=PlayMode.StopSameLayer, QueueMode queueMode=QueueMode.PlayNow)
	{
		motionState = this.ReplaceMotionState(motionState);
		switch (motionState)
		{
		case MotionState.wait:
//		case MotionState.walk:
		case MotionState.run:
		case MotionState.run_001_f:
		case MotionState.run_001_b:
		case MotionState.run_001_r:
		case MotionState.run_001_l:
			this.UpdateMoveAnimation(motionState, 1f, layer, fadeLength, playMode);
			break;
		case MotionState.down_mid:
			this.UpdateAnimation(MotionName.GetName(MotionState.down_mid), layer, fadeLength, playMode, queueMode, GameConstant.DownMotionSpeed_Normal);
			break;
		case MotionState.down_mid_spin:
		case MotionState.dead_mid:
			this.UpdateAnimation(MotionName.GetName(MotionState.down_mid), layer, fadeLength, playMode, queueMode, GameConstant.DownMotionSpeed_Spin);
			break;
		default:
			this.UpdateAnimation(MotionName.GetName(motionState), layer, fadeLength, playMode, queueMode);
			break;
		}
	}
	public virtual void UpdateAnimation(string clipName, int layer, float fadeLength=0f, PlayMode playMode=PlayMode.StopSameLayer, QueueMode queueMode=QueueMode.PlayNow, float speed = 1f)
	{
		actionLayerAnimation.clipName = clipName;
		actionLayerAnimation.fadeLength = fadeLength;
		actionLayerAnimation.layer = layer;
		actionLayerAnimation.playMode = playMode;

		if (!this.IsActive)
			return;
//		if (this.IsPlayingAdditiveLayer)
//			return;
		if (this.Animation == null)
			return;
		
		AnimationState animationState = this.Animation[clipName];
		if (animationState != null)
		{
			animationState.time = 0.0f;
			animationState.speed = speed;
		}
		animationState = this.Animation[clipName + CombinedAnimSuffix];
		if (animationState != null)
		{
			animationState.time = 0.0f;
			animationState.speed = speed;
		}
	}
	#endregion

	#region Private Method
	/// <summary>
	/// ゲーム上で使っているモーション名を実際のモーション名に読み替える.
	/// </summary>
	private MotionState ReplaceMotionState(MotionState motionState)
	{
		switch (motionState)
		{
		case MotionState.down_sta:
		case MotionState.down_sta_spin:
		case MotionState.dead_sta:
			return MotionState.down_sta;
		case MotionState.down_end:
		case MotionState.dead_end:
			return MotionState.down_end;
		default:
			return motionState;
		}
	}

	public void ResetAnimation()
	{
		this.Play(moveLayerAnimation.clipName, 0, moveLayerAnimation.layer, moveLayerAnimation.playMode);
	}

	void Play(string clipName, float fadeLength, int layer, PlayMode playMode)
	{
		this.PlayFade(clipName, layer, fadeLength, playMode);
	}

	/// <summary>
	/// 移動モーションと行動モーションを合成して再生する.
	/// </summary>
	public void UpdateCombinedAnimation()
	{
		if(this.Animation == null)
			{ return; }

		// 移動モーション
		this.Play(moveLayerAnimation.clipName, moveLayerAnimation.fadeLength, moveLayerAnimation.layer, moveLayerAnimation.playMode);
		moveLayerAnimation.playMode = PlayMode.StopSameLayer;
		
		// 行動モーション
		if(!string.IsNullOrEmpty(actionLayerAnimation.clipName))
		{
			string actionMotionName = actionLayerAnimation.clipName;
			string combAnimName = actionMotionName + CombinedAnimSuffix;
			if(this.Animation[combAnimName] != null)
			{
				bool isCombined = false;
				if(moveLayerAnimation.clipName != MotionName_Wait)
				{
					actionMotionName = combAnimName;
					isCombined = true;
				}
				// モーション合成同期コルーチンを登録.
				motionFiber.AddFiber(SyncCombinedAnim(actionLayerAnimation.clipName, combAnimName, isCombined));
			}
			else
			{
				// 移動モーション抑制コルーチンを登録.
				motionFiber.AddFiber(StopMoveAnim(actionLayerAnimation.clipName));
			}
			this.Play(actionMotionName, actionLayerAnimation.fadeLength, actionLayerAnimation.layer, actionLayerAnimation.playMode);
			// 連続再生防止.
			actionLayerAnimation.clipName = string.Empty;
		}

		motionFiber.Update();
	}
	
	/// <summary>
	/// モーション合成同期コルーチン.
	/// </summary>
	IEnumerator SyncCombinedAnim(string actionAnimName, string combinedAnimName, bool isCombined)
	{
		// 合成元,合成先のモーションが再生されている限り有効.
		while(this.Animation.IsPlaying(actionAnimName) ||
			this.Animation.IsPlaying(combinedAnimName))
		{
			// 再生時間を同期.
			float time = Mathf.Max(this.Animation[actionAnimName].time, this.Animation[combinedAnimName].time);
			this.Animation[actionAnimName].time = time;
			this.Animation[combinedAnimName].time = time;
			
			// 待機,移動が切り替わった場合.
			bool combine = (moveLayerAnimation.clipName != MotionName_Wait);
			if(isCombined != combine)
			{
				// 切り替わり先のモーションを再生.
				isCombined = combine;
				if(isCombined)
				{
					this.Play(combinedAnimName, actionLayerAnimation.fadeLength, actionLayerAnimation.layer, actionLayerAnimation.playMode);
				}
				else
				{
					this.Play(actionAnimName, actionLayerAnimation.fadeLength, actionLayerAnimation.layer, actionLayerAnimation.playMode);
				}
			}
			
			yield return null;
		}
	}

	/// <summary>
	/// 移動モーション抑制コルーチン.
	/// 2014/06/25 低優先度レイヤーのモーションでも,アニメーションイベント(足音とか)の実行をしてしまうようなので抑制.
	/// </summary>
	IEnumerator StopMoveAnim(string actionAnimName)
	{
		// 特定のモーションが再生されている限り有効.
		while(this.Animation.IsPlaying(actionAnimName))
		{
			this.Stop(moveLayerAnimation.clipName);
			yield return null;
		}
	}

	struct LayerAnimation{
		public string clipName;
		public int layer;
		public float fadeLength;
		public PlayMode playMode;
	}
	#endregion
}
