/// <summary>
/// シンクロ合成結果表示
/// 
/// 2016/03/08
/// </summary>
using UnityEngine;

public class GUISynchroResult : Singleton<GUISynchroResult>
{
	#region フィールド&プロパティ
	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	private XUI.SynchroResult.SynchroResultView _viewAttach = null;
	private XUI.SynchroResult.SynchroResultView ViewAttach { get { return _viewAttach; } }

	/// <summary>
	/// ステータス表示フォーマット
	/// </summary>
	[SerializeField]
	private string _statusFormat = "{0}";
	private string StatusFormat { get { return _statusFormat; } }

	/// <summary>
	/// シンクロステータス表示フォーマット
	/// </summary>
	[SerializeField]
	private string _synchroStatusFormat = "{0:+#;-#;+0}";
	private string SynchroStatusFormat { get { return _synchroStatusFormat; } }

	/// <summary>
	/// ステータスアップ表示フォーマット
	/// </summary>
	[SerializeField]
	private string _statusUpFormat = "{0}up!";
	private string StatusUpFormat { get { return _statusUpFormat; } }

	/// <summary>
	/// 開始時のアクティブ状態
	/// </summary>
	[SerializeField]
	private bool _isStartActive = false;
	private bool IsStartActive { get { return _isStartActive; } }

	/// <summary>
	/// キャラボード
	/// </summary>
	private CharaBoard CharaBoard { get { return ScmParam.Lobby.CharaBoard; } }

	/// <summary>
	/// モデル
	/// </summary>
	private XUI.SynchroResult.IModel Model { get; set; }
	/// <summary>
	/// ビュー
	/// </summary>
	private XUI.SynchroResult.IVIew View { get; set; }
	/// <summary>
	/// コントローラ
	/// </summary>
	private XUI.SynchroResult.IController Controller { get; set; }
	#endregion

	#region 初期化
	protected override void Awake()
	{
		base.Awake();
		this.MemberInit();
	}
	/// <summary>
	/// シリアライズされていないメンバー初期化
	/// </summary>
	private void MemberInit()
	{
		this.Model = null;
		this.View = null;
		this.Controller = null;
	}

	void Start()
	{
		this.Construct();
		// 初期アクティブ設定
		this.SetActive(this.IsStartActive, true);
	}
	private void Construct()
	{
		// モデル生成
		var model = new XUI.SynchroResult.Model();
		this.Model = model;
		this.Model.HitPointFormat = this.StatusFormat;
		this.Model.SynchroHitPointFormat = this.SynchroStatusFormat;
		this.Model.HitPointUpFormat = this.StatusUpFormat;
		this.Model.AttackFormat = this.StatusFormat;
		this.Model.SynchroAttackFormat = this.SynchroStatusFormat;
		this.Model.AttackUpFormat = this.StatusUpFormat;
		this.Model.DefenceFormat = this.StatusFormat;
		this.Model.SynchroDefenceFormat = this.SynchroStatusFormat;
		this.Model.DefenceUpFormat = this.StatusUpFormat;
		this.Model.ExtraFormat = this.StatusFormat;
		this.Model.SynchroExtraFormat = this.SynchroStatusFormat;
		this.Model.ExtraUpFormat = this.StatusUpFormat;

		// ビュー生成
		XUI.SynchroResult.IVIew view = null;
		if(this.ViewAttach != null)
		{
			view = this.ViewAttach.GetComponent(typeof(XUI.SynchroResult.IVIew)) as XUI.SynchroResult.IVIew;
		}
		this.View = view;

		// コントローラ生成
		var controller = new XUI.SynchroResult.Controller(model, view, this.CharaBoard);
		this.Controller = controller;
	}

	void OnDestroy()
	{
		if (this.Controller != null)
		{
			this.Controller.Dispose();
		}
	}
	#endregion

	#region アクティブ設定
	/// <summary>
	/// 閉じる
	/// </summary>
	public static void Close()
	{
		if (Instance != null) Instance.SetActive(false, false);
	}
	/// <summary>
	/// 開く
	/// </summary>
	public static void Open(XUI.SynchroResult.SetupParam param)
	{
		if (Instance != null) Instance._Open(param);
	}
	/// <summary>
	/// 開き直す
	/// </summary>
	public static void ReOpen()
	{
		if (Instance != null) Instance.SetActive(true, false);
	}
	/// <summary>
	/// 開く
	/// </summary>
	void _Open(XUI.SynchroResult.SetupParam param)
	{
		this.SetActive(true, false);
		this.Setup(param);
	}
	/// <summary>
	/// アクティブ設定
	/// </summary>
	void SetActive(bool isActive, bool isTweenSkip)
	{
		if (this.Controller != null)
		{
			this.Controller.SetActive(isActive, isTweenSkip);
		}
	}
	#endregion

	#region 設定
	/// <summary>
	/// 初期設定
	/// </summary>
	private void Setup(XUI.SynchroResult.SetupParam param)
	{
		if (this.Controller != null)
		{
			this.Controller.Setup(param);
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
			this.AddEvent(this.Result);
		}

		[SerializeField]
		ResultEvent _result = new ResultEvent();
		public ResultEvent Result { get { return _result; } }
		[System.Serializable]
		public class ResultEvent : IDebugParamEvent
		{
			public event System.Action<XUI.SynchroResult.SetupParam> Execute = delegate { };
			[SerializeField]
			bool executeDummy = false;
			[SerializeField]
			bool execute = false;

			[SerializeField]
			AvatarType avatarType = AvatarType.None;
			[SerializeField]
			int hitPoint = 0;
			[SerializeField]
			int synchroHitPoint = 0;
			[SerializeField]
			int hitPointUp = 0;
			[SerializeField]
			int attack = 0;
			[SerializeField]
			int synchroAttack = 0;
			[SerializeField]
			int attackUp = 0;
			[SerializeField]
			int defence = 0;
			[SerializeField]
			int synchroDefence = 0;
			[SerializeField]
			int defenceUp = 0;
			[SerializeField]
			int extra = 0;
			[SerializeField]
			int synchroExtra = 0;
			[SerializeField]
			int extraUp = 0;

			public void Update()
			{
				if (this.executeDummy)
				{
					this.executeDummy = false;

					this.avatarType = (AvatarType)UnityEngine.Random.Range((int)AvatarType.Begin, (int)AvatarType.End + 1);

					this.hitPoint = UnityEngine.Random.Range(0, 1000);
					this.synchroHitPoint = UnityEngine.Random.Range(0, CharaInfo.SynchroMaxHitPoint+1);
					this.hitPointUp = UnityEngine.Random.Range(1, 4);
					this.attack = UnityEngine.Random.Range(0, 1000);
					this.synchroAttack = UnityEngine.Random.Range(0, CharaInfo.SynchroMaxAttack + 1);
					this.attackUp = UnityEngine.Random.Range(1, 4);
					this.defence = UnityEngine.Random.Range(0, 1000);
					this.synchroDefence = UnityEngine.Random.Range(0, CharaInfo.SynchroMaxDefense + 1);
					this.defenceUp = UnityEngine.Random.Range(1, 4);
					this.extra = UnityEngine.Random.Range(0, 1000);
					this.synchroExtra = UnityEngine.Random.Range(0, CharaInfo.SynchroMaxExtra + 1);
					this.extraUp = UnityEngine.Random.Range(1, 4);

					this.execute = true;
				}
				if (this.execute)
				{
					this.execute = false;
					var args = new XUI.SynchroResult.SetupParam();
					args.AvatarType = this.avatarType;
					args.HitPoint = this.hitPoint;
					args.SynchroHitPoint = this.synchroHitPoint;
					args.HitPointUp = this.hitPointUp;
					args.Attack = this.attack;
					args.SynchroAttack = this.synchroAttack;
					args.AttackUp = this.attackUp;
					args.Defence = this.defence;
					args.SynchroDefence = this.synchroDefence;
					args.DefenceUp = this.defenceUp;
					args.Extra = this.extra;
					args.SynchroExtra = this.synchroExtra;
					args.ExtraUp = this.extraUp;
					this.Execute(args);
				}
			}

		}
	}

	void DebugInit()
	{
		var d = this.DebugParam;

		d.ExecuteClose += Close;
		d.ExecuteActive += () =>
		{
			d.ReadMasterData();
			this.SetActive(true, false);
		};

		d.Result.Execute += (args) => { this.Setup(args); };
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
