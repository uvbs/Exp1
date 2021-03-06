/// <summary>
/// プレイヤー情報表示
/// 
/// 2015/12/10
/// </summary>
using UnityEngine;
using System;
using System.Collections;

public class GUIPlayerInfo : Singleton<GUIPlayerInfo>
{
	#region フィールド＆プロパティ
	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	XUI.PlayerInfo.PlayerInfoView _viewAttach = null;
	XUI.PlayerInfo.PlayerInfoView ViewAttach { get { return _viewAttach; } }

	/// <summary>
	/// 名前表示フォーマット
	/// </summary>
	[SerializeField]
	string _nameFormat = "{0}";
	string NameFormat { get { return _nameFormat; } }

	/// <summary>
	/// グレード表示フォーマット
	/// </summary>
	[SerializeField]
	string _gradeFormat = "{0}";
	string GradeFormat { get { return _gradeFormat; } }

	/// <summary>
	/// レベル表示フォーマット
	/// </summary>
	[SerializeField]
	string _lvFormat = "{0}";
	string LvFormat { get { return _lvFormat; } }

	/// <summary>
	/// 経験値表示フォーマット
	/// </summary>
	[SerializeField]
	string _expFormat = "{0:0.00}%";
	string ExpFormat { get { return _expFormat; } }

	/// <summary>
	/// 現在のスタミナ表示フォーマット
	/// </summary>
	[SerializeField]
	string _staminaFormat = "{0}/{1}";
	string StaminaFormat { get { return _staminaFormat; } }

	/// <summary>
	/// スタミナ回復までの残り時間表示フォーマット
	/// </summary>
	[SerializeField]
	string _staminaTimeFormat = "{0:00}:{1:00}";
	string StaminaTimeFormat { get { return _staminaTimeFormat; } }

	// モデル
	XUI.PlayerInfo.IModel Model { get; set; }
	// ビュー
	XUI.PlayerInfo.IView View { get; set; }
	// コントローラー
	XUI.PlayerInfo.IController Contoller { get; set; }
	// シリアライズされていないメンバー初期化
	void MemberInit()
	{
		this.Model = null;
		this.View = null;
		this.Contoller = null;
	}

	// キャラアイコン
	CharaIcon CharaIcon { get { return ScmParam.Lobby.CharaIcon; } }
	#endregion

	#region 初期化
	override protected void Awake()
	{
		base.Awake();
		this.Construct();
	}
	void Construct()
	{
		this.MemberInit();

		// モデル生成
		var model = new XUI.PlayerInfo.Model();
		this.Model = model;
		model.NameFormat = this.NameFormat;
		model.GradeFormat = this.GradeFormat;
		model.LvFormat = this.LvFormat;
		model.ExpFormat = this.ExpFormat;
		model.StaminaFormat = this.StaminaFormat;
		model.StaminaTimeFormat = this.StaminaTimeFormat;

		// ビュー生成
		XUI.PlayerInfo.IView view = null;
		if (this.ViewAttach != null)
		{
			view = this.ViewAttach.GetComponent(typeof(XUI.PlayerInfo.IView)) as XUI.PlayerInfo.IView;
		}
		this.View = view;

		// コントローラー生成
		var controller = new XUI.PlayerInfo.Controller(model, view, this.CharaIcon);
		this.Contoller = controller;
	    gameObject.SetActive(false);
	}
	#endregion

	#region アクティブ設定
	/// <summary>
	/// 閉じる
	/// </summary>
	public static void Close()
	{
		if (Instance != null) Instance._SetActive(false);
	}
	/// <summary>
	/// アクティブ設定
	/// </summary>
	public static void SetActive(bool isActive)
	{
		if (Instance != null) Instance._SetActive(isActive);
	}
	/// <summary>
	/// アクティブ設定
	/// </summary>
	void _SetActive(bool isActive)
	{
		if (this.View != null)
		{
			this.View.SetActive(isActive);
		}
	}
	#endregion

	#region 更新
	void OnEnable()
	{
		this.SyncStaminaTime(DateTime.Now);
	}
	void Update()
	{
		if (this.Contoller != null)
		{
			this.Contoller.Update();
		}
	}
	#endregion

	#region 同期
	static public void Sync()
	{
		if (Instance != null) Instance._Sync();
	}
	public void _Sync()
	{
		var p = GameController.GetPlayer();
		if (p != null)
		{
			this._SetAvatarType(p.AvatarType, p.SkinId);
			this._SetName(p.UserName);
			this.SetGrade(UnityEngine.Random.Range(1, 50));
			this.SetLv(UnityEngine.Random.Range(1, 99));
			var expMin = UnityEngine.Random.Range(0, 999999);
			var expMax = UnityEngine.Random.Range(0, 999999) + expMin;
			var exp = UnityEngine.Random.Range(expMin, expMax);
			this.SetExp(exp, expMin, expMax);
			var staminaMax = UnityEngine.Random.Range(1, 999);
			var staminaMin = UnityEngine.Random.Range(0, staminaMax);
			this.SetStamina(staminaMin, staminaMax);
			this.SetStaminaRecoveryTime(DateTime.Now.AddSeconds(UnityEngine.Random.Range(60f, 9999f)));
			this.SyncStaminaTime(DateTime.Now);
		}
	}
	#endregion

	#region アバタータイプ
	static public void SetAvatarType(AvatarType avatarType, int skinId)
	{
		if (Instance != null) Instance._SetAvatarType(avatarType, skinId);
	}
	void _SetAvatarType(AvatarType avatarType, int skinId)
	{
        if (this.Model != null) {
            this.Model.AvatarType = avatarType;
            this.Model.SkinId = skinId;
        }
	}
	#endregion

	#region 名前
	static public void SetName(string name)
	{
		if (Instance != null) Instance._SetName(name);
	}
	void _SetName(string name)
	{
		if (this.Model != null) this.Model.Name = name;
	}
	#endregion

	#region グレード
	void SetGrade(int grade)
	{
		if (this.Model != null) this.Model.Grade = grade;
	}
	void SetGradeFormat(string format)
	{
		if (this.Model != null) this.Model.GradeFormat = format;
	}
	#endregion

	#region プレイヤーレベル
	void SetLv(int lv)
	{
		if (this.Model != null) this.Model.Lv = lv;
	}
	void SetLvFormat(string format)
	{
		if (this.Model != null) this.Model.LvFormat = format;
	}
	#endregion

	#region プレイヤー経験値
	void SetExp(long exp, long expMin, long expMax)
	{
		if (this.Model != null)
		{
			this.Model.Exp = exp;
			this.Model.ExpMin = expMin;
			this.Model.ExpMax = expMax;
		}
	}
	void SetExpFormat(string format)
	{
		if (this.Model != null) this.Model.ExpFormat = format;
	}
	#endregion

	#region スタミナ
	void SetStamina(int stamina, int staminaMax)
	{
		if (this.Model != null)
		{
			this.Model.Stamina = stamina;
			this.Model.StaminaMax = staminaMax;
		}
	}
	void SetStaminaFormat(string format)
	{
		if (this.Model != null) this.Model.StaminaFormat = format;
	}
	#endregion

	#region スタミナ回復までの残り時間
	void SetStaminaTimeFormat(string format)
	{
		if (this.Model != null) this.Model.StaminaTimeFormat = format;
	}
	void SetStaminaRecoveryTime(DateTime recoveryTime)
	{
		if (this.Model != null) this.Model.StaminaRecoveryTime = recoveryTime;
	}
	void SyncStaminaTime(DateTime nowTime)
	{
		if (this.Model != null)
		{
			this.Model.SyncStaminaTime(nowTime);
		}
	}
	#endregion

	#region デバッグ
#if UNITY_EDITOR && XW_DEBUG
	[SerializeField]
	GUIDebugParam _debugParam = new GUIDebugParam();
	GUIDebugParam DebugParam { get { return _debugParam; } }
	[System.Serializable]
	public class GUIDebugParam : GUIDebugParamBase
	{
		public GUIDebugParam()
		{
			this.AddEvent(this.Chara);
			this.AddEvent(this.Name);
			this.AddEvent(this.Grade);
			this.AddEvent(this.Lv);
			this.AddEvent(this.Exp);
			this.AddEvent(this.Stamina);
		}

		[SerializeField]
		CharaEvent _chara = new CharaEvent();
		public CharaEvent Chara { get { return _chara; } set { _chara = value; } }
		[System.Serializable]
		public class CharaEvent : IDebugParamEvent
		{
			public event System.Action<AvatarType, int> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			AvatarType avatarType = AvatarType.None;
            [SerializeField]
            int skinId = 0;

            public void Update()
			{
				if (this.execute)
				{
					this.execute = false;
					this.Execute(this.avatarType, this.skinId);
				}
			}
		}

		[SerializeField]
		NameEvent _name = new NameEvent();
		public NameEvent Name { get { return _name; } }
		[System.Serializable]
		public class NameEvent : IDebugParamEvent
		{
			public event System.Action<string> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			string name = "";

			public void Update()
			{
				if (this.execute)
				{
					this.execute = false;
					this.Execute(this.name);
				}
			}
		}

		[SerializeField]
		GradeEvent _grade = new GradeEvent();
		public GradeEvent Grade { get { return _grade; } }
		[System.Serializable]
		public class GradeEvent : IDebugParamEvent
		{
			public event System.Action<int> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			int grade = 0;

			public void Update()
			{
				if (this.execute)
				{
					this.execute = false;
					this.Execute(this.grade);
				}
			}
		}

		[SerializeField]
		LvEvent _lv = new LvEvent();
		public LvEvent Lv { get { return _lv; } }
		[System.Serializable]
		public class LvEvent : IDebugParamEvent
		{
			public event System.Action<int> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			int lv = 0;

			public void Update()
			{
				if (this.execute)
				{
					this.execute = false;
					this.Execute(this.lv);
				}
			}
		}

		[SerializeField]
		ExpEvent _exp = new ExpEvent();
		public ExpEvent Exp { get { return _exp; } }
		[System.Serializable]
		public class ExpEvent : IDebugParamEvent
		{
			public event System.Action<long, long, long> ExecuteExp = delegate { };
			[SerializeField]
			bool executeExp = false;
			[SerializeField]
			long min = 0;
			[SerializeField]
			long now = 0;
			[SerializeField]
			long max = 0;

			public event System.Action<long, long, long> UpdateSlider = delegate { };
			[SerializeField]
			bool updateSlider = false;
			[SerializeField]
			[Range(0f, 1f)]
			float slider = 0f;

			public void Update()
			{
				if (this.executeExp)
				{
					this.executeExp = false;
					this.ExecuteExp(this.now, this.min, this.max);
				}
				if (this.updateSlider)
				{
					this.now = (long)Mathf.Lerp(this.min, this.max, this.slider);
					this.UpdateSlider(this.now, this.min, this.max);
				}
			}
		}

		[SerializeField]
		StaminaEvent _stamina = new StaminaEvent();
		public StaminaEvent Stamina { get { return _stamina; } }
		[System.Serializable]
		public class StaminaEvent : IDebugParamEvent
		{
			public event System.Action<int, int> ExecuteStamina = delegate { };
			[SerializeField]
			bool executeStamina = false;
			[SerializeField]
			int now = 0;
			[SerializeField]
			int max = 0;

			public event System.Action<float> ExecuteTime = delegate { };
			[SerializeField]
			bool executeTime = false;
			[SerializeField]
			float timeSecond = 0f;

			public void Update()
			{
				if (this.executeStamina)
				{
					this.executeStamina = false;
					this.ExecuteStamina(this.now, this.max);
				}
				if (this.executeTime)
				{
					this.executeTime = false;
					this.ExecuteTime(this.timeSecond);
				}
			}
		}
	}
	void DebugInit()
	{
		var d = this.DebugParam;

		d.ExecuteClose += Close;
		d.ExecuteActive += () => { SetActive(true); };
		d.Chara.Execute += (avatarType, skinId) =>
			{
				d.ReadMasterData();
				SetAvatarType(avatarType, skinId);
			};
		d.Name.Execute += SetName;
		d.Grade.Execute += (grade) =>
			{
				this.SetGrade(grade);
				this.SetGradeFormat(this.GradeFormat);
			};
		d.Lv.Execute += (lv) =>
			{
				this.SetLv(lv);
				this.SetLvFormat(this.LvFormat);
			};
		d.Exp.ExecuteExp += (now, min, max) =>
			{
				this.SetExp(now, min, max);
				this.SetExpFormat(this.ExpFormat);
			};
		d.Exp.UpdateSlider += (now, min, max) =>
			{
				this.SetExp(now, min, max);
			};
		d.Stamina.ExecuteStamina += (now, max) =>
			{
				this.SetStamina(now, max);
				this.SetStaminaFormat(this.StaminaFormat);
			};
		d.Stamina.ExecuteTime += (timeSecond) =>
			{
				this.SetStaminaRecoveryTime(DateTime.Now.AddSeconds(timeSecond));
				this.SyncStaminaTime(DateTime.Now);
				this.SetStaminaTimeFormat(this.StaminaTimeFormat);
			};
	}
	bool _isDebugInit = false;
	void DebugUpdate()
	{
		if (!this._isDebugInit)
		{
			this._isDebugInit = true;
			this.DebugInit();
		}

		this.DebugParam.Update();
	}
	/// <summary>
	/// OnValidate はInspector上で値の変更があった時に呼び出される
	/// GameObject がアクティブ状態じゃなくても呼び出されるのでアクティブ化するときには有効
	/// </summary>
	void OnValidate()
	{
		if (Application.isPlaying)
		{
			this.DebugUpdate();
		}
	}
#endif
	#endregion
}
