/// <summary>
/// チームリザルト表示時に各メンバーごとのリザルト表示を制御するアイテム
/// 
/// 2014/11/12
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIResultInfoItem : MonoBehaviour
{
	#region フィールド&プロパティ
	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UILabel rankLabel;
		public UILabel nameLabel;
		public UILabel scoreLabel;
		public UILabel killDeathLabel;
		public UILabel synonymLabel;
		public UISprite charaIconSprite;
		public List<GameObject> scoreRankList;
		public TweenPosition effectTween;
		public TweenAlpha effectAlphaTween;
		public TweenColor playerColorTween;
	}

	/// <summary>
	/// キャラアイコンのセットアップ時のエラー処理に使用
	/// </summary>
	private AvatarType avatarType;
	#endregion

	#region 生成
	/// <summary>
	/// チームリザルト情報アイテムを生成しセットアップを行う
	/// </summary>
	public static GUIResultInfoItem Create(GUIResultInfoItem prefab, Transform parent, MemberInfo info, int playerID, int index)
	{
		if(prefab == null) return null;

		// インスタンス生成
		GUIResultInfoItem item = SafeObject.Instantiate(prefab) as GUIResultInfoItem;
		if(item != null)
		{
			// アクティブをONにする
			item.gameObject.SetActive(true);
			// 名前
			item.name = string.Format("{0}{1}", prefab.name, index);
			// 親子付け
			item.transform.parent = parent;
			item.transform.localPosition = Vector3.zero;
			item.transform.localRotation = Quaternion.identity;
			item.transform.localScale = Vector3.one;

			// セットアップ
			item.Setup(info, playerID, index);
		}

		return item;
	}
	#endregion

	#region セットアップ
	/// <summary>
	/// セットアップ
	/// </summary>
	public void Setup(MemberInfo info, int playerID, int index)
	{
		// ランク
		if(this.Attach.rankLabel != null)
		{
			this.Attach.rankLabel.text = info.rank.ToString();
		}
		// 名前
		if(this.Attach.nameLabel != null)
		{
			this.Attach.nameLabel.text = info.name;
		}
		// スコア
		if(this.Attach.scoreLabel != null)
		{
			this.Attach.scoreLabel.text = string.Format("{0}", info.score);
		}
		// キルデス
		if(this.Attach.killDeathLabel != null)
		{
			this.Attach.killDeathLabel.text = string.Format("{0}/{1}", info.kill, info.death);
		}
		// キャラアイコン
		if(this.Attach.charaIconSprite != null)
		{
			CharaIcon charaIcon = GUIResultOld.CharaIcon;
			if(charaIcon != null)
			{
				avatarType = info.avatarType;
				charaIcon.GetIcon(info.avatarType, info.skinId, false, SetIconSprite);
			}
		}
		// 1～3位に王冠表示
		if(info.rank > 0 && info.rank <= 3)
		{
			GameObject scoreRank = this.Attach.scoreRankList[info.rank-1];
			if(scoreRank != null)
			{
				scoreRank.SetActive(true);
			}
		}

		// エフェクト用のTweenのStartDelayを設定する
		if(this.Attach.effectTween != null && this.Attach.effectAlphaTween != null)
		{
			// 開始時間をずらしていく
			// TweenPosition
			float delay = this.Attach.effectTween.delay;
			this.Attach.effectTween.delay = (index+1) * delay;
			// TweenAlpha
			delay = this.Attach.effectAlphaTween.delay;
			this.Attach.effectAlphaTween.delay = (index+1) * delay;
		}

		if(info.inFieldID == playerID)
		{
			// プレイヤーエフェクトを再生
			if(this.Attach.playerColorTween != null)
			{
				this.Attach.playerColorTween.Play(true);
			}
		}
	}

	/// <summary>
	/// キャラアイコンのセットアップ
	/// </summary>
	public void SetIconSprite(UIAtlas atlas, string spriteName)
	{
		UISprite sprite = this.Attach.charaIconSprite;
		// アトラス設定
		sprite.atlas = atlas;
		// スプライト設定
		sprite.spriteName = spriteName;

		AvatarType avatarType = this.avatarType;
		// アトラス内にアイコンが含まれているかチェック
		if (sprite.GetAtlasSprite() == null)
		{
			// アトラスとスプライト名が両方共設定されていてスプライトがない場合はエラー扱いｌ
			if (atlas != null && !string.IsNullOrEmpty(spriteName))
			{
				Debug.LogWarning(string.Format(
					"GUIResultInfoItem.SetupSprite:\r\n" +
					"Sprite Not Found!! AvatarType = {0} SpriteName = {1}", avatarType, spriteName));
			}
		}
	}
	#endregion
}
