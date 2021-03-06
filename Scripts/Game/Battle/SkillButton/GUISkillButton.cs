/// <summary>
/// スキルボタン
/// 
/// 2013/01/17
/// </summary>
using UnityEngine;
using System.Collections;

using Scm.Common.Master;

public class GUISkillButton : GUIRepeatButton
{
    private const float SPECIAL_SKILL_CHARGE_SPEED = 50.0f;

    #region フィールド＆プロパティ
    [SerializeField]
	private UISprite sprite;
	public  UISprite Sprite { get { return sprite; } private set { sprite = value; } }
	[SerializeField]
	private SkillButtonType skillButtonType;
	public  SkillButtonType SkillButtonType { get { return skillButtonType; } private set { skillButtonType = value; } }
	[SerializeField]
	private float coolTimeCounter;
	public float CoolTimeCounter {
        get { return coolTimeCounter; }
        set { coolTimeCounter = value; }
    }
    [SerializeField]
    private float coolTime;
	public  float CoolTime
	{
		get { return coolTime; }
		set
		{
            if (skillButtonType != SkillButtonType.SpecialSkill) {
                if (StoreSkill)
                {
                }
                else
                {
                    coolTime = value;
                    coolTimeCounter = value;
                }
            }
			if (value > 0f)
			{
				isCounter = true;
				// CoolTimeを設定してからUpdate()を通る前にスキルを使用される
				// 可能性があるためここで設定
				IsSkillEnable = false;
			}
		}
	}
	[SerializeField]
	private bool isCounter = true;
	public  bool IsCounter { get { return isCounter; } set { isCounter = value; } }
	[SerializeField]
	private UILabel label;
	public  UILabel Label { get { return label; } private set { label = value; } }

    [SerializeField]
    TweenColor _recastTweenColor;
    public TweenColor RecastTweenColor { get { return _recastTweenColor; } }

    [SerializeField]
    UILabel _storeLabel;
    public UILabel StoreLabel { get { return _storeLabel; } }

	private UIButton Button { get; set; }
	public bool IsActive { get; set; }
	// スキルが使用可能か.
	bool isSkillEnable;
	bool IsSkillEnable
	{
		get { return isSkillEnable; }
		set
		{
			if(isSkillEnable != value)
			{
				if(value)
				{
					if(isUsableLevel && !isSeal)
					{
						isSkillEnable = true;
						this.FullChargeEffect();
                        if (enabledEffect != null) {
                            enabledEffect.SetActive(true);
                        }
						if (this.RecastTweenColor != null)
							this.RecastTweenColor.PlayReverse();
					}
				}
				else
				{
					isSkillEnable = false;
                    if (enabledEffect != null) {
                        enabledEffect.SetActive(false);
                    }
                    if (this.RecastTweenColor != null)
						this.RecastTweenColor.PlayForward();

                    Debug.LogWarning("Link Tip Close");
                    GUISkill.Instance.ShowLinkTip(false);
				}
			}
		}
	}
	// 使用可能レベルに達しているか.
	bool isUsableLevel;
	bool IsUsableLevel
	{
		get { return isUsableLevel; }
		set
		{
			isUsableLevel = value;
			if(isUsableLevel == false)
			{
				IsSkillEnable = false;
			}
		}
	}
	// 封印状態か.
	bool isSeal;
	bool IsSeal
	{
		get { return isSeal; }
		set
		{
			isSeal = value;
			if(isSeal == true)
			{
				IsSkillEnable = false;
			}
		}
	}

    [SerializeField]
    GameObject enabledEffect;

	/// <summary>
	/// スキルアイコン
	/// </summary>
	private SkillIcon SkillIcon { get { return ScmParam.Battle.SkillIcon; } }

    public bool StoreSkill = false;
    public bool LinkSkill = false;
    private int MaxStoreCount = 0;
    private int CurStoreCount = 0;
	private int CurStoreCountTemp = 0;
    private float StoreTime;
	private float StoreTimeTemp;
    private float RepeatTimeCounter = 0.5f;
	#endregion

    #region 初期化
    protected override void Start()
    {
        base.Start();

        this.Button = this.gameObject.GetSafeComponentInChildren<UIButton>();
        this.IsActive = true;
        this.isSkillEnable = true;
        SetStoreSkill(true);

    }
    public void ChangeButtonName(AvatarType avatarType, int lv)
    {
        //if (NetworkController.Instance == null)
        //	return;
        if (this.Label == null)
            return;

        // ラベルセット
        CharaButtonSetMasterData buttonSet;
        if (MasterData.TryGetCharaButtonSet((int)avatarType, lv, out buttonSet))
        {
            switch (this.SkillButtonType)
            {
                case SkillButtonType.Normal:
                    this.Label.text = buttonSet.AttackButton.ButtonName;
                    break;
                case SkillButtonType.Skill1:
                    this.Label.text = buttonSet.Skill1Button.ButtonName;
                    break;
                case SkillButtonType.Skill2:
                    this.Label.text = buttonSet.Skill2Button.ButtonName;
                    break;
                case SkillButtonType.SpecialSkill:
                    this.Label.text = buttonSet.SpecialSkillButton.ButtonName;
                    break;
                case SkillButtonType.TechnicalSkill:
                    this.Label.text = buttonSet.TechnicalSkillButton.ButtonName;
                    break;
            }
        }
        else
        {
            this.Label.text = string.Empty;
        }
        // アイコンセット(以前の表示が残らないようにbuttonSetがnullの場合でも動くようにする).
        SkillIcon.GetSkillIcon(buttonSet, lv, this.SkillButtonType, true, SetIcon);
        // 使用可能レベル判定.
        this.IsUsableLevel = ObsolateSrc.SkillUsableLevel.IsSkillUsable(this.SkillButtonType, lv);

        SetStoreSkill();
    }

	private void SetStoreSkill(bool isFirstTime=false)
    {
		StartCoroutine(SetStoreSkillIE(isFirstTime));
    }



	private IEnumerator SetStoreSkillIE(bool isFirstTime=false)
    {
        while (null == GameController.GetPlayer())
        {
            yield return 0;
        }
        Player player = GameController.GetPlayer();
        var skillData = player.GetSkillData(this);
        ReleaseButton = skillData.AccumulateFlag;
        StoreSkill = (skillData.ChargeCount > 0);
        LinkSkill = (skillData.LinkSkillID > 0);
        if (StoreSkill)
        {
            if (null != StoreLabel) StoreLabel.gameObject.SetActive(true);
			StoreTime = skillData.CoolTime;
			if (isFirstTime)
				CoolTimeCounter = StoreTime;
			else
            	CoolTimeCounter = StoreTimeTemp;
            MaxStoreCount = skillData.ChargeCount;
			if(isFirstTime) CurStoreCount = skillData.ChargeCount; //这里判断是否是第一次.
			else CurStoreCount = CurStoreCountTemp;
            if (null != StoreLabel)
            {
                StoreLabel.text = CurStoreCount.ToString();
            }
        }
        else
        {
            if (null != StoreLabel) StoreLabel.gameObject.SetActive(false);
        }
    }

    private void SetIcon(UIAtlas atlas, string iconName)
    {
        if (this.background == null) return;
        this.background.atlas = atlas;
        this.background.spriteName = iconName;
    }
    // 封印状態かどうか調べる.
    private bool CheckSeal()
    {
        Player player = GameController.GetPlayer();
        return player != null ? player.IsSealSkillButton(this.SkillButtonType) : false;
    }
    #endregion

    #region 更新
    protected override void Update()
    {
        // HACK: 20150423 プレイヤー生成＆破棄時,バフパケット時だけチェックすれば良いはずだが.念のため毎回チェック.
        this.IsSeal = CheckSeal();

		if (this.IsCounter)
		{
            if (this.SkillButtonType == SkillButtonType.SpecialSkill) {
                this.CoolTimeCounter -= Time.deltaTime * SPECIAL_SKILL_CHARGE_SPEED;
                this.coolTime = Scm.Common.Utility.MAX_SPECIAL_SKILL_GAUGE;
            } else { 
                this.CoolTimeCounter -= Time.deltaTime;
                this.RepeatTimeCounter -= Time.deltaTime;
            }
		}

        if (StoreSkill)
        {
            Update_StoreSkill();
        }
        else
        {
            Update_CommonSkill();
        }
        base.Update();
    }

    private void Update_StoreSkill()
    {
		CurStoreCountTemp = CurStoreCount;
		StoreTimeTemp = CoolTimeCounter;
        if (CurStoreCount == MaxStoreCount)
        {
            this.CoolTimeCounter = StoreTime;
        }

        if (0f >= this.CoolTimeCounter)
        {
            CurStoreCount = Mathf.Min(CurStoreCount + 1, MaxStoreCount);
            this.CoolTimeCounter = StoreTime;
            if (null != StoreLabel)
            {
                StoreLabel.text = CurStoreCount.ToString();
            }
        }

        this.Button.disabledColor = (this.IsActive ? Color.white : Color.gray);
        if (CurStoreCount < 1)
        {
            this.IsSkillEnable = false;
            float fillAmount = 0f;
            if (0f < this.StoreTime)
                fillAmount = this.CoolTimeCounter / this.StoreTime;
            this.Sprite.fillAmount = fillAmount;
        }
        else
        {
            this.IsSkillEnable = true;
            this.Sprite.fillAmount = 0;
        }
    }

    private void Update_CommonSkill()
    {
        if (0f >= this.CoolTimeCounter)
        {
            this.CoolTimeCounter = 0f;
            this.IsSkillEnable = true;
        }
        else
        {
            this.IsSkillEnable = false;
        }
        this.Button.disabledColor = (this.IsActive ? Color.white : Color.gray);

        {
            float fillAmount = 0f;
            if (0f < this.CoolTime)
                fillAmount = this.CoolTimeCounter / this.CoolTime;
            this.Sprite.fillAmount = fillAmount;
        }

        base.Update();
    }
    #endregion

    public void AddGauge(int gaugeValue) {
        this.CoolTimeCounter = Mathf.Max(0, this.CoolTimeCounter - gaugeValue);
        this.coolTime = Scm.Common.Utility.MAX_SPECIAL_SKILL_GAUGE;
    }

	#region GUIRepeatButton Override
	protected override void Send()
	{
	    if (StoreSkill)
	    {
            if (CurStoreCount <= 0 || RepeatTimeCounter > 0)
	        {
                Debug.Log("===> Not Ok");
	            return;
	        }
	    }
		if(!this.IsSkillEnable)
			return;

        Player player = GameController.GetPlayer();
        if (null == player)
            return;

		if (player.TrySetSkill(this)) {
		    if (LinkSkill)
		    {
		        Debug.LogWarning("Link Tip Show");
                GUISkill.Instance.ShowLinkTip(true);
		    }
		    if(StoreSkill){
                RepeatTimeCounter = 0.5f;
		        CurStoreCount--;
		        if (null != StoreLabel)
		        {
		            StoreLabel.text = CurStoreCount.ToString();
		        }
		    }
		    if (this.SkillButtonType == SkillButtonType.SpecialSkill) {
                ResetGauge();
            }
        }
	}
	#endregion

    public void ResetGauge() {
        CoolTimeCounter = Scm.Common.Utility.MAX_SPECIAL_SKILL_GAUGE;
        coolTime = Scm.Common.Utility.MAX_SPECIAL_SKILL_GAUGE;
    }

    #region チャージエフェクト
    [SerializeField]
	private UISprite background;
	const float EffectTime = 0.3f;
	const float EffectEndAlpha = 0f;
	const float EffectEndScale = 2f;
	/// <summary>
	/// 再使用可能状態になった場合の演出(中身は仮).
	/// </summary>
	private void FullChargeEffect()
	{
		if(background == null)
		{
			background = transform.SafeFindChild("Background").GetComponent<UISprite>();
		}
		if(background)
		{
			GameObject popup = Object.Instantiate(background.gameObject) as GameObject;
			popup.transform.parent = background.transform.parent;
			popup.transform.localPosition = background.transform.localPosition;
			popup.transform.localRotation = background.transform.localRotation;
			popup.transform.localScale = background.transform.localScale;
			
			// 演出
            TweenAlpha.Begin(popup, EffectTime, EffectEndAlpha);
            TweenScale.Begin(popup, EffectTime, popup.transform.localScale * EffectEndScale);
            // スプライトの色は元の色に戻す
            if (this.RecastTweenColor != null)
                TweenColor.Begin(popup, 0f, this.RecastTweenColor.from);
            Destroy(popup, EffectTime);
        }
    }
    #endregion

    #region デバッグ
#if UNITY_EDITOR && XW_DEBUG
    [SerializeField]
    DebugParameter _debugParam = new DebugParameter();
    DebugParameter DebugParam { get { return _debugParam; } }
    [System.Serializable]
    public class DebugParameter
    {
        public bool execute;
        public float coolTime;
    }
    void DebugUpdate()
    {
        var t = this.DebugParam;
        if (t.execute)
        {
            t.execute = false;
            {
                this.CoolTime = t.coolTime;
            }
        }
    }
    /// <summary>
    /// OnValidate はInspector上で値の変更があった時に呼び出される
    /// GameObject がアクティブ状態じゃなくても呼び出されるのでアクティブ化するときには有効
    /// </summary>
    void OnValidate()
    {
        this.DebugUpdate();
    }
#endif
    #endregion
}
