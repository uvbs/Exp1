/// <summary>
/// 進化合成結果表示
/// 
/// 2016/03/03
/// </summary>
using UnityEngine;
using System.Collections;

public class GUIEvolutionResult : Singleton<GUIEvolutionResult>
{
	#region フィールド&プロパティ
	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	private XUI.EvolutionResult.EvolutionResultView _viewAttach = null;
	private XUI.EvolutionResult.EvolutionResultView ViewAttach { get { return _viewAttach; } }

	/// <summary>
	/// レベル表示フォーマット
	/// </summary>
	[SerializeField]
	private string _lvFormat = "Lv.{0}";
	private string LvFormat { get { return _lvFormat; } }

	/// <summary>
	/// ステータス表示フォーマット
	/// </summary>
	[SerializeField]
	private string _statusFormat = "{0}";
	private string StatusFormat { get { return _statusFormat; } }

	/// <summary>
	/// ステータスアップ表示フォーマット
	/// </summary>
	[SerializeField]
	private string _statusUpFormat = "{0} up";
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
	private XUI.EvolutionResult.IModel Model { get; set; }
	/// <summary>
	/// ビュー
	/// </summary>
	private XUI.EvolutionResult.IVIew View { get; set; }
	/// <summary>
	/// コントローラ
	/// </summary>
	private XUI.EvolutionResult.IController Controller { get; set; }
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
		var model = new XUI.EvolutionResult.Model();
		this.Model = model;
		this.Model.SynchroRemainUpFormat = this.StatusUpFormat;
		this.Model.LvFormat = this.StatusFormat;
		this.Model.HitPointFormat = this.StatusFormat;
		this.Model.AttackFormat = this.StatusFormat;
		this.Model.DefenceFormat = this.StatusFormat;
		this.Model.ExtraFormat = this.StatusFormat;

		// ビュー生成
		XUI.EvolutionResult.IVIew view = null;
		if(this.ViewAttach != null)
		{
			view = this.ViewAttach.GetComponent(typeof(XUI.EvolutionResult.IVIew)) as XUI.EvolutionResult.IVIew;
		}
		this.View = view;

		// コントローラ生成
		var controller = new XUI.EvolutionResult.Controller(model, view, this.CharaBoard);
		this.Controller = controller;
	}

	/// <summary>
	/// 破棄
	/// </summary>
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
	public static void Open(XUI.EvolutionResult.SetupParam param)
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
	private void _Open(XUI.EvolutionResult.SetupParam param)
	{
		this.SetActive(true, false);
		this.Setup(param);
	}
	/// <summary>
	/// アクティブ設定
	/// </summary>
	private void SetActive(bool isActive, bool isTweenSkip)
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
	private void Setup(XUI.EvolutionResult.SetupParam param)
	{
		if(this.Controller != null)
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
			public event System.Action<XUI.EvolutionResult.SetupParam> Execute = delegate { };
			[SerializeField]
			bool executeDummy = false;
			[SerializeField]
			bool execute = false;

			[SerializeField]
			AvatarType avatarType = AvatarType.None;
			[SerializeField]
			int beforeRank = 0;
			[SerializeField]
			int afterRank = 0;
			[SerializeField]
			int synchroRemain = 0;
			[SerializeField]
			int synchroRemainUp = 0;
			[SerializeField]
			int lv = 0;
			[SerializeField]
			int hitPoint = 0;
			[SerializeField]
			int attack = 0;
			[SerializeField]
			int defence = 0;
			[SerializeField]
			int extra = 0;

			public void Update()
			{
				if (this.executeDummy)
				{
					this.executeDummy = false;

					this.avatarType = (AvatarType)UnityEngine.Random.Range((int)AvatarType.Begin, (int)AvatarType.End + 1);

					this.beforeRank = UnityEngine.Random.Range(1, 6);
					this.afterRank = this.beforeRank;
					if (UnityEngine.Random.Range(0, 2) == 1)
					{
						this.afterRank = UnityEngine.Random.Range(this.beforeRank, 6);
					}

					this.synchroRemain = UnityEngine.Random.Range(0, CharaInfo.SynchroMaxCount);
					this.synchroRemainUp = 0;
					if (UnityEngine.Random.Range(0, 2) == 1)
					{
						this.synchroRemainUp = UnityEngine.Random.Range(0, this.synchroRemain + 1);
					}

					this.lv = UnityEngine.Random.Range(0, 11);
					this.hitPoint = UnityEngine.Random.Range(0, 1000);
					this.attack = UnityEngine.Random.Range(0, 1000);
					this.defence = UnityEngine.Random.Range(0, 1000);
					this.extra = UnityEngine.Random.Range(0, 1000);

					this.execute = true;
				}
				if(this.execute)
				{
					this.execute = false;
					var args = new XUI.EvolutionResult.SetupParam();
					args.AvatarType = this.avatarType;
					args.BeforeRank = this.beforeRank;
					args.AfterRank = this.afterRank;
					args.SynchroRemain = this.synchroRemain;
					args.SynchroRemainUp = this.synchroRemainUp;
					args.Lv = this.lv;
					args.HitPoint = this.hitPoint;
					args.Attack = this.attack;
					args.Defence = this.defence;
					args.Extra = this.extra;
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
