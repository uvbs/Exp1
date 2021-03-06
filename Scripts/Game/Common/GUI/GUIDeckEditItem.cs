/// <summary>
/// デッキ編集アイテム
/// 
/// 2014/11/11
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.Master;

public class GUIDeckEditItem : MonoBehaviour
{
	#region フィールド＆プロパティ
	/// <summary>
	/// リビルドタイムの表示フォーマット
	/// </summary>
	[SerializeField]
	string _rebuildTimeCountFormat = "{0:0}";
	string RebuildTimeCountFormat { get { return _rebuildTimeCountFormat; } }

	/// <summary>
	/// キャラ情報
	/// </summary>
	[SerializeField]
	CharaInfo _charaInfo = new CharaInfo();
	public CharaInfo CharaInfo { get { return _charaInfo; } private set { _charaInfo = value; } }

    public CharaIcon CharaIcon;
    public SkillIcon SkillIcon;
	/// <summary>
	/// スタイル情報
	/// </summary>
	[SerializeField]
	StyleInfo _styleInfo = new StyleInfo();
	StyleInfo StyleInfo { get { return _styleInfo; } set { _styleInfo = value; } }

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach = new AttachObject();
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		// 状況によってアクティブ化するものしないもの
		public GameObject charaActiveGroup = null;
		public GameObject leaderActiveGroup = null;
		public ModeActiveGroup modeActive = new ModeActiveGroup();
		[System.Serializable]
		public class ModeActiveGroup
		{
			public GameObject editGroup = null;
			public GameObject charaSelectGroup = null;
		}

		public UISprite iconSprite = null;
		public UISprite styleSprite = null;
		public UISprite selectSprite = null;
		public UILabel costLabel = null;
		public UILabel rebuildTimeLabel = null;
		public UILabel nameLabel = null;
		public UILabel descriptionLabel = null;
		public UILabel lvLabel = null;
		public UIButton deleteButton = null;

		// スキルアイコン関連
		public SkillIconSprite skillIcon = new SkillIconSprite();
		[System.Serializable]
		public class SkillIconSprite
		{
			public UISprite normal = null;
			public UISprite skill1 = null;
			public UISprite skill2 = null;
			public UISprite specialSkill = null;
			public UISprite technicalSkill = null;
		}

		// リビルドタイムカウンター
		public RebuildTimeCounter rebuildTimeCounter = new RebuildTimeCounter();
		[System.Serializable]
		public class RebuildTimeCounter
		{
			public GameObject activeGroup = null;	// リビルドタイム発生時のみ表示する
			public UILabel countLabel = null;
			public UISprite countSprite = null;
		}
	}

	// リビルドタイムの残り時間
	public float RemainingRebuildTime { get; private set; }
	// 更新ファイバー
	Fiber UpdateFiber { get; set; }

	// キャラクターコスト
	int Cost { get { return this.CharaInfo.DeckCost; } }
	// キャラクターLv
	int Lv { get { return this.CharaInfo.PowerupLevel; } }
	// スキルアイコンレベル
	int SkillIconLv { get { return 1; } }
	// 最大リビルドタイム
	float RebuildTime { get { return this.CharaInfo.RebuildTime; } }
	// スライダーで使用するリビルドタイムの値
	float RebuildTimeSliderValue { get { return (this.RebuildTime == 0f ? 0f : this.RemainingRebuildTime / (float)this.RebuildTime); } }
	// シリアライズされていないメンバーの初期化
	void MemberInit()
	{
		this.RemainingRebuildTime = 0f;
		this.UpdateFiber = null;
	}
	#endregion

	#region 初期化
	/// <summary>
	/// 初期化
	/// </summary>
	void Awake()
	{
		this.MemberInit();
	}
	/// <summary>
	/// アイテム作成
	/// </summary>
	public static GUIDeckEditItem Create(GameObject prefab, Transform parent, int itemIndex)
	{
		// インスタンス化
		var go = SafeObject.Instantiate(prefab) as GameObject;
		if (go == null)
			return null;

		// 名前
		go.name = string.Format("{0}{1}", prefab.name, itemIndex);
		// 親子付け
		go.transform.parent = parent;
		go.transform.localPosition = prefab.transform.localPosition;
		go.transform.localRotation = prefab.transform.localRotation;
		go.transform.localScale = prefab.transform.localScale;
		// 可視化
		go.SetActive(true);

		// コンポーネント取得
		var item = go.GetComponent(typeof(GUIDeckEditItem)) as GUIDeckEditItem;
		if (item == null)
		{
			Debug.LogWarning("GUIDeckEditItem not Found!");
			return null;
		}
		// 値初期化
		item.ClearValue();

		return item;
	}
	/// <summary>
	/// クリア
	/// </summary>
	public void ClearValue()
	{
		this.Setup(null, null, null);
		this.ClearCharaIcon();
		this.ClearStyleIcon();
		this.ClearSkillIcon();
	}
	#endregion

	#region セットアップ

    public void Refresh()
    {
        Setup(this.CharaInfo, this.CharaIcon, this.SkillIcon);
    }

	/// <summary>
	/// セットアップ
	/// </summary>
	public void Setup(CharaInfo info, CharaIcon charaIcon, SkillIcon skillIcon)
	{
		this.CharaInfo = (info != null ? info : new CharaInfo());
	    this.CharaIcon = charaIcon;
        this.SkillIcon = skillIcon;

		// UI更新
		{
			var t = this.Attach;
			// スロットが空ならキャラグループを消す
			if (t.charaActiveGroup != null)
				t.charaActiveGroup.SetActive(!this.CharaInfo.IsDeckSlotEmpty);

			// リーダー時に表示にするグループ
			if (t.leaderActiveGroup != null)
				t.leaderActiveGroup.SetActive(this.CharaInfo.IsDeckSlotLeader);

			// コスト設定
			if (t.costLabel != null)
				t.costLabel.text = this.Cost.ToString();

			// リビルドタイム設定
			if (t.rebuildTimeLabel != null)
				t.rebuildTimeLabel.text = this.RebuildTime.ToString();

			// 名前設定
			if (t.nameLabel != null)
				t.nameLabel.text = this.CharaInfo.Name;

			// UNDONE:キャラのタイプ情報は仮
			string typeDescription = "";
			{
				Scm.Common.Master.CharaMasterData data;
				if (MasterData.TryGetChara((int)this.CharaInfo.AvatarType, out data))
					typeDescription = data.Description;
			}
			if (t.descriptionLabel != null)
				t.descriptionLabel.text = typeDescription;

			// レベル設定
			if (t.lvLabel != null)
				t.lvLabel.text = this.Lv.ToString();

			// 削除ボタン設定
			if (t.deleteButton != null)
				t.deleteButton.gameObject.SetActive(!this.CharaInfo.IsDeckSlotLeader);
		}

		// キャラアイコン設定
		this.SetCharaIcon(charaIcon);

		// スタイルアイコン設定
		this.SetStyleIcon(this.CharaInfo.AvatarType);

		// スキルアイコン設定
		CharaLevelMasterData charaLv;
		if (MasterData.TryGetCharaLv((int)this.CharaInfo.AvatarType, this.SkillIconLv, out charaLv))
		{
			CharaButtonSetMasterData buttonSet;
			if (MasterData.TryGetCharaButtonSet(charaLv, out buttonSet))
			{
				this.SetSkillIcon(skillIcon, buttonSet, this.SkillIconLv);
			}
		}

		// リビルドタイム設定
		this.SetRemainingRebuildTime(this.CharaInfo.RemainingRebuildTime);
		// リビルトタイム更新コルーチン
		// デッキ画面を閉じていても時間が経過するようにコルーチンで動かす
		FiberController.Remove(this.UpdateFiber);
		this.UpdateFiber = FiberController.AddFiber(this.RebuildTimeUpdateCoroutine());

		// モードごとアクティブ設定
		var m = this.Attach.modeActive;
		switch (GUIDeckEdit.NowMode)
		{
		case GUIDeckEdit.DeckMode.None: this.SetModeActive(null); break;
		case GUIDeckEdit.DeckMode.Edit: this.SetModeActive(m.editGroup); break;
		case GUIDeckEdit.DeckMode.CharaSelect: this.SetModeActive(m.charaSelectGroup); break;
		}
	}
	/// <summary>
	/// モードごとのアクティブ設定
	/// </summary>
	void SetModeActive(GameObject activeGroup)
	{
		var m = this.Attach.modeActive;

		// グループの表示設定
		var list = new List<GameObject>();
		list.Add(m.editGroup);
		list.Add(m.charaSelectGroup);
		foreach (var t in list)
		{
			if (t == null)
				continue;
			bool isActive = (activeGroup == t);
			t.SetActive(isActive);
		}
	}
	/// <summary>
	/// 選択枠の表示設定
	/// </summary>
	public void SetSelectSpriteActive(bool isActive)
	{
		if (this.Attach.selectSprite != null)
			this.Attach.selectSprite.gameObject.SetActive(isActive);
	}
	#endregion

	#region リビルドタイム設定
	/// <summary>
	/// リビルドタイム更新用コルーチン
	/// デッキ画面を閉じていてもコルーチンで動かすため
	/// </summary>
	IEnumerator RebuildTimeUpdateCoroutine()
	{
		while (this.RemainingRebuildTime > 0f)
		{
			// リビルドタイム更新
			this.RemainingRebuildTime -= Time.deltaTime;
			this.SetRemainingRebuildTime(this.RemainingRebuildTime);
			yield return null;
		}
		this.SetRemainingRebuildTime(this.RemainingRebuildTime);
	}
	/// <summary>
	/// 残りリビルドタイムによる設定
	/// </summary>
	void SetRemainingRebuildTime(float remainingRebuildTime)
	{
		this.RemainingRebuildTime = Mathf.Max(0f, remainingRebuildTime);

		// リビルドタイム更新
		var t = this.Attach.rebuildTimeCounter;
		if (t.countSprite != null)
			t.countSprite.fillAmount = this.RebuildTimeSliderValue;
		if (t.countLabel != null)
			t.countLabel.text = string.Format(this.RebuildTimeCountFormat, this.RemainingRebuildTime);
		// リビルドタイムが発生している時だけ表示する
		var isActive = (this.RemainingRebuildTime > 0f);
		if (t.activeGroup != null)
			t.activeGroup.SetActive(isActive);
	}
	#endregion

	#region キャラアイコン設定
	/// <summary>
	/// キャラアイコンをクリアする
	/// </summary>
	void ClearCharaIcon()
	{
		this.SetCharaIcon(null, "");
	}
	/// <summary>
	/// キャラアイコンを設定する
	/// </summary>
	/// <param name="charaIcon"></param>
	void SetCharaIcon(CharaIcon charaIcon)
	{
		if (charaIcon == null)
			return;
		if (this.CharaInfo.AvatarType == AvatarType.None)
			this.ClearCharaIcon();
		else
			charaIcon.GetIcon(this.CharaInfo.AvatarType, this.CharaInfo.SkinId, false, this.SetCharaIcon);
	}
	/// <summary>
	/// 読み込んだアトラスからキャラアイコンを設定する
	/// </summary>
	void SetCharaIcon(UIAtlas atlas, string spriteName)
	{
		var sp = this.Attach.iconSprite;
		if (sp == null)
			return;

		// アトラス設定
		sp.atlas = atlas;
		// スプライト設定
		sp.spriteName = spriteName;

		// アトラス内にアイコンが含まれているかチェック
		if (sp.GetAtlasSprite() == null)
		{
			// アトラスとスプライト名が両方共設定されていてスプライトがない場合はエラー扱いｌ
			if (atlas != null && !string.IsNullOrEmpty(spriteName))
			{
				Debug.LogWarning(string.Format(
					"SetIconSprite:\r\n" +
					"Sprite Not Found!! AvatarType = {0} SpriteName = {1}", this.CharaInfo.AvatarType, spriteName));
			}
		}
	}
	#endregion

	#region スタイルアイコン設定
	/// <summary>
	/// スタイルアイコンクリア
	/// </summary>
	void ClearStyleIcon()
	{
		this.SetStyleIcon(AvatarType.None);
	}
	/// <summary>
	/// スタイルアイコンの設定
	/// </summary>
	void SetStyleIcon(AvatarType avatarType)
	{
		// スタイルアイコン設定
		var sp = this.Attach.styleSprite;
		if (sp == null)
			return;

		// スタイル情報を取得する
		StyleInfo info = null;
		bool isEnable = false;
		this.StyleInfo = null;
		if (ObsolateSrc.TryGetStyleInfo(avatarType, out info))
		{
			this.StyleInfo = info;
			isEnable = true;
		}
		// スタイルアイコン表示設定
		sp.gameObject.SetActive(isEnable);
		if (isEnable)
		{
			// スプライト変更
			// 同じアトラス内にあることを前提とする
			if (info != null)
			{
				sp.spriteName = info.iconName;

				// アトラス内にアイコンが含まれているかチェック
				if (sp.GetAtlasSprite() == null)
				{
					// アトラスとスプライト名が両方共設定されていてスプライトがない場合はエラー扱いｌ
					if (sp.atlas != null && !string.IsNullOrEmpty(info.iconName))
					{
						Debug.LogWarning(string.Format(
							"SetStyle:\r\n" +
							"Sprite Not Found!! StyleType = {0} SpriteName = {1}", info.type, info.iconName));
					}
				}
			}
		}
	}
	#endregion

	#region スキルアイコン設定
	/// <summary>
	/// スキルアイコンをクリアする
	/// </summary>
	void ClearSkillIcon()
	{
		var t = this.Attach.skillIcon;
		this.SetSkillIcon(null, "", t.normal);
		this.SetSkillIcon(null, "", t.skill1);
		this.SetSkillIcon(null, "", t.skill2);
		this.SetSkillIcon(null, "", t.specialSkill);
		this.SetSkillIcon(null, "", t.technicalSkill);
	}
	/// <summary>
	/// スキルアイコンを設定する
	/// </summary>
	/// <param name="charaIcon"></param>
	void SetSkillIcon(SkillIcon skillIcon, CharaButtonSetMasterData data, int lv)
	{
		if (skillIcon == null)
			return;

		var t = this.Attach.skillIcon;
		if (t.normal != null)
			skillIcon.GetSkillIcon(data, lv, SkillButtonType.Normal, false, (atlas, spriteName) => { this.SetSkillIcon(atlas, spriteName, t.normal); });
		if (t.skill1 != null)
			skillIcon.GetSkillIcon(data, lv, SkillButtonType.Skill1, false, (atlas, spriteName) => { this.SetSkillIcon(atlas, spriteName, t.skill1); });
		if (t.skill2 != null)
			skillIcon.GetSkillIcon(data, lv, SkillButtonType.Skill2, false, (atlas, spriteName) => { this.SetSkillIcon(atlas, spriteName, t.skill2); });
		if (t.specialSkill != null)
			skillIcon.GetSkillIcon(data, lv, SkillButtonType.SpecialSkill, false, (atlas, spriteName) => { this.SetSkillIcon(atlas, spriteName, t.specialSkill); });
		if (t.technicalSkill != null)
			skillIcon.GetSkillIcon(data, lv, SkillButtonType.TechnicalSkill, false, (atlas, spriteName) => { this.SetSkillIcon(atlas, spriteName, t.technicalSkill); });
	}
	/// <summary>
	/// 読み込んだアトラスからスキルアイコンを設定する
	/// </summary>
	void SetSkillIcon(UIAtlas atlas, string spriteName, UISprite sp)
	{
		if (sp == null)
			return;

		// アトラス設定
		sp.atlas = atlas;
		// スプライト設定
		sp.spriteName = spriteName;

		// アトラス内にアイコンが含まれているかチェック
		if (sp.GetAtlasSprite() == null)
		{
			// アトラスとスプライト名が両方共設定されていてスプライトがない場合はエラー扱いｌ
			if (atlas != null && !string.IsNullOrEmpty(spriteName))
			{
				Debug.LogWarning(string.Format(
					"SetIconSprite:\r\n" +
					"Sprite Not Found!! AvatarType = {0} SpriteName = {1}", this.CharaInfo.AvatarType, spriteName));
			}
		}
	}
	#endregion

	#region NGUIリフレクション
	/// <summary>
	/// アイテムを選択した時
	/// </summary>
	public void OnButtonClick()
	{
		GUIDeckEdit.SetSelectItem(this);
	}
	/// <summary>
	/// 削除ボタンを押した時
	/// </summary>
	public void OnButtonDelete()
	{
		GUIDeckEdit.SetDeleteItem(this);
        //Select This For Get New One
        GUIDeckEdit.SetSelectItem(this);
	}
	#endregion
}
