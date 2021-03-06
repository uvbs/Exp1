/// <summary>
/// 合成メッセージ
/// 
/// 2016/05/10
/// </summary>
using UnityEngine;
using System;
using XUI.FusionMessage;

public class GUIFusionMessage : Singleton<GUIFusionMessage>
{
	#region フィールド&プロパティ
	/// <summary>
	/// ビューアタッチ
	/// </summary>
	[SerializeField]
	private FusionMessageView _viewAttach = null;
	private FusionMessageView ViewAttach { get { return _viewAttach; } }

	/// <summary>
	/// 開始時のアクティブ状態
	/// </summary>
	[SerializeField]
	private bool _isStartActive = false;
	private bool IsStartActive { get { return _isStartActive; } }

	/// <summary>
	/// フォーマットアタッチ
	/// </summary>
	[SerializeField]
	private FormatAttachObject _formatAttach = null;
	private FormatAttachObject FormatAttach { get { return _formatAttach; } }
	[Serializable]
	public class FormatAttachObject
	{
		[SerializeField]
		private string _total = "{0}";
		public string Total { get { return _total; } }

		[SerializeField]
		private string _base = "{0}";
		public string Base { get { return _base; } }

		[SerializeField]
		private string _synchro = "{0}%";
		public string Synchro { get { return _synchro; } }

		[SerializeField]
		private string _slot = "{0}";
		public string Slot { get { return _slot; } }

		[SerializeField]
		private string _up = "{0}";
		public string Up { get { return _up; } }

		[SerializeField]
		private string _pending = "? UP!!";
		public string Pending { get { return _pending; } }
	}

	/// <summary>
	/// コントローラ
	/// </summary>
	private IController Controller { get; set; }
	#endregion

	#region 初期化
	protected override void Awake()
	{
		base.Awake();
		this.MemberInit();
	}
	void Start()
	{
		this.Constrcut();
		// 初期化アクティブ設定
		this.SetActive(this.IsStartActive, true);
	}

	/// <summary>
	/// シリアライズされていないメンバー初期化
	/// </summary>
	private void MemberInit()
	{
		this.Controller = null;
	}
	private void Constrcut()
	{
		// モデル生成
		var model = new Model();
		if(this.FormatAttach != null)
		{
			model.TotalStatusFormat = this.FormatAttach.Total;
			model.BaseStatusFormat = this.FormatAttach.Base;
			model.SynchroFormat = this.FormatAttach.Synchro;
			model.SlotFormat = this.FormatAttach.Slot;
			model.UpFormat = this.FormatAttach.Up;
			model.PendingFormat = this.FormatAttach.Pending;
		}

		// ビュー生成
		IView view = null;
		if(this.ViewAttach != null)
		{
			view = this.ViewAttach.GetComponent(typeof(IView)) as IView;
		}

		// コントローラ生成
		var controller = new Controller(model, view);
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
	public static void Open()
	{
		if (Instance != null)
		{
			Instance.Setup(ModeType.None, null, string.Empty);
		}
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

	#region OKボタン
	/// <summary>
	/// OKボタンモードで表示
	/// </summary>
	public static void OpenOK(string message, Action onOK)
	{
		if (Instance == null) { return; }
		Instance._OpenOK(null, message, true, onOK);
	}
	public static void OpenOK(string message, bool isClose, Action onOK)
	{
		if(Instance == null) { return; }
		Instance._OpenOK(null, message, isClose, onOK);
	}
	public static void OpenOK(SetupParam param, string message, bool isClose, Action onOK)
	{
		if (Instance == null) { return; }
		Instance._OpenOK(param, message, isClose, onOK);
	}
	private void _OpenOK(SetupParam param, string message, bool isClose, Action onOK)
	{
		if(this.Controller != null)
		{
			// ボタン押下時のイベント登録
			this.Controller.SetOkButtonEvent(onOK, isClose);
			this.Controller.SetYesButtonEvent(null, false);
			this.Controller.SetNoButtonEvent(null, false);
		}
		// セット
		if(param == null)
		{
			this.ChangeMode(ModeType.OK, message);
		}
		else
		{
			this.Setup(ModeType.OK, param, message);
		}
	}
	#endregion

	#region YesNoボタン
	/// <summary>
	/// YesNoボタンモードで表示
	/// </summary>
	public static void OpenYesNo(string message, Action onYes, Action onNo)
	{
		if (Instance == null) { return; }
		Instance._OpenYesNo(null, message, true, onYes, onNo);
	}
	public static void OpenYesNo(string message, bool isClose, Action onYes, Action onNo)
	{
		if (Instance == null) { return; }
		Instance._OpenYesNo(null, message, isClose, onYes, onNo);
	}
	public static void OpenYesNo(SetupParam param, string message, bool isClose, Action onYes, Action onNo)
	{
		if (Instance == null) { return; }
		Instance._OpenYesNo(param, message, isClose, onYes, onNo);
	}
	private void _OpenYesNo(SetupParam param, string message, bool isClose, Action onYes, Action onNo)
	{
		if (this.Controller != null)
		{
			// ボタン押下時のイベント登録
			this.Controller.SetOkButtonEvent(null, false);
			this.Controller.SetYesButtonEvent(onYes, isClose);
			this.Controller.SetNoButtonEvent(onNo, isClose);
		}
		// セットアップ
		if(param == null)
		{
			this.ChangeMode(ModeType.YesNo, message);
		}
		else
		{
			this.Setup(ModeType.YesNo, param, message);
		}
	}
	#endregion

	#region セットアップ
	/// <summary>
	/// セットアップ
	/// </summary>
	private void Setup(ModeType modeType, SetupParam param, string message)
	{
		if (this.Controller == null) { return; }
		this.Controller.Setup(modeType, param, message);

		// アクティブ設定
		this.SetActive(true, false);
	}
	#endregion

	#region モード切替
	/// <summary>
	/// モード切替処理
	/// </summary>
	private void ChangeMode(ModeType modeType, string message)
	{
		if (this.Controller == null) { return; }
		this.Controller.SetMessage(message);
		this.Controller.ChangeMode(modeType);

		// アクティブ設定
		this.SetActive(true, false);
	}
	#endregion

	#region パラメータ設定
	/// <summary>
	/// パラメータのみ設定する
	/// </summary>
	public static void SetupParameter(SetupParam param)
	{
		if (Instance == null) { return; }
		Instance._SetupParameter(param);
	}
	private void _SetupParameter(SetupParam param)
	{
		if (this.Controller == null) { return; }
		this.Controller.SetupParameter(param);
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
			this.AddEvent(this.OpenMode);
		}

		[SerializeField]
		ModeEvent _openMode = new ModeEvent();
		public ModeEvent OpenMode { get { return _openMode; } }
		[System.Serializable]
		public class ModeEvent : IDebugParamEvent
		{
			public event System.Action<ModeType, SetupParam, bool, string> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			ModeType mode = ModeType.None;
			[SerializeField]
			bool isClose = true;
			[SerializeField]
			string message = "１行１６文字で3行表示できるよ！";
			[SerializeField]
			bool isDummy = true;
			[SerializeField]
			private Param param = new Param();
			[Serializable]
			public class Param
			{
				public int rankBefore = 0;
				public int rankAfter = 0;
				public int levelBefore = 0;
				public int levelAfter = 0;
				public int exp = 0;
				public int totalExp = 0;
				public int nextLvTotalExp = 0;
				public int synchroRemainBefore = 0;
				public int synchroRemainAfter = 0;
				public int hitPointBefore = 0;
				public int hitPointAfter = 0;
				public int hitPointBaseBefore = 0;
				public int hitPointBaseAfter = 0;
				public int synchroHitPoint = 0;
				public int slotHitPointBefore = 0;
				public int slotHitPointAfter = 0;
				public int attackBefore = 0;
				public int attackAfter = 0;
				public int attackBaseBefore = 0;
				public int attackBaseAfter = 0;
				public int synchroAttack = 0;
				public int slotAttackBefore = 0;
				public int slotAttackAfter = 0;
				public int defenseBefore = 0;
				public int defenseAfter = 0;
				public int defenseBaseBefore = 0;
				public int defenseBaseAfter = 0;
				public int synchroDefense = 0;
				public int slotDefenseBefore = 0;
				public int slotDefenseAfter = 0;
				public int extraBefore = 0;
				public int extraAfter = 0;
				public int extraBaseBefore = 0;
				public int extraBaseAfter = 0;
				public int synchroExtra = 0;
				public int slotExtraBefore = 0;
				public int slotExtraAfter = 0;
				
				public bool isSynchroFusion = false;

				public void SetDummy()
				{
					this.rankBefore = UnityEngine.Random.Range(1, 6);
					this.rankAfter = UnityEngine.Random.Range(1, 6);
					this.levelBefore = UnityEngine.Random.Range(1, 100);
					this.levelAfter = UnityEngine.Random.Range(1, 100);
					this.totalExp = UnityEngine.Random.Range(0, 10000);
					this.exp = UnityEngine.Random.Range(this.totalExp, 10000);
					this.nextLvTotalExp = UnityEngine.Random.Range(this.exp, 10000);
					this.synchroRemainBefore = UnityEngine.Random.Range(0, MasterDataCommonSetting.Fusion.MaxSynchroCount+1);
					this.synchroRemainAfter = UnityEngine.Random.Range(0, MasterDataCommonSetting.Fusion.MaxSynchroCount+1);
					this.hitPointBefore = UnityEngine.Random.Range(0, 10000);
					this.hitPointAfter = UnityEngine.Random.Range(0, 10000);
					this.hitPointBaseBefore = UnityEngine.Random.Range(0, 10000);
					this.hitPointBaseAfter = UnityEngine.Random.Range(0, 10000);
					this.synchroHitPoint = UnityEngine.Random.Range(0, CharaInfo.SynchroMaxHitPoint+1);
					this.slotHitPointBefore = UnityEngine.Random.Range(0, 10000);
					this.slotHitPointAfter = UnityEngine.Random.Range(0, 10000);
					this.attackBefore = UnityEngine.Random.Range(0, 10000);
					this.attackAfter = UnityEngine.Random.Range(0, 10000);
					this.attackBaseBefore = UnityEngine.Random.Range(0, 10000);
					this.attackBaseAfter = UnityEngine.Random.Range(0, 10000);
					this.synchroAttack = UnityEngine.Random.Range(0, CharaInfo.SynchroMaxAttack + 1);
					this.slotAttackBefore = UnityEngine.Random.Range(0, 10000);
					this.slotAttackAfter = UnityEngine.Random.Range(0, 10000);
					this.defenseBefore = UnityEngine.Random.Range(0, 10000);
					this.defenseAfter = UnityEngine.Random.Range(0, 10000);
					this.defenseBaseBefore = UnityEngine.Random.Range(0, 10000);
					this.defenseBaseAfter = UnityEngine.Random.Range(0, 10000);
					this.synchroDefense = UnityEngine.Random.Range(0, CharaInfo.SynchroMaxDefense + 1);
					this.slotDefenseBefore = UnityEngine.Random.Range(0, 10000);
					this.slotDefenseAfter = UnityEngine.Random.Range(0, 10000);
					this.extraBefore = UnityEngine.Random.Range(0, 10000);
					this.extraAfter = UnityEngine.Random.Range(0, 10000);
					this.extraBaseBefore = UnityEngine.Random.Range(0, 10000);
					this.extraBaseAfter = UnityEngine.Random.Range(0, 10000);
					this.synchroExtra = UnityEngine.Random.Range(0, CharaInfo.SynchroMaxExtra + 1);
					this.slotExtraBefore = UnityEngine.Random.Range(0, 10000);
					this.slotExtraAfter = UnityEngine.Random.Range(0, 10000);
					
					isSynchroFusion = UnityEngine.Random.Range(0, 2) == 1 ? true : false;
				}

				public SetupParam GetConvertSetupParam()
				{
					var param = new SetupParam();
					param.RankBefore = this.rankBefore;
					param.RankAfter = this.rankAfter;
					param.LevelBefore = this.levelBefore;
					param.LevelAfter = this.levelAfter;
					param.Exp = this.exp;
					param.TotalExp = this.totalExp;
					param.NextLvTotalExp = this.nextLvTotalExp;
					param.SynchroRemainBefore = this.synchroRemainBefore;
					param.SynchroRemainAfter = this.synchroRemainAfter;
					param.HitPointBefore = this.hitPointBefore;
					param.HitPointAfter = this.hitPointAfter;
					param.HitPointBaseBefore = this.hitPointBaseBefore;
					param.HitPointBaseAfter = this.hitPointBaseAfter;
					param.SynchroHitPoint = this.synchroHitPoint;
					param.SlotHitPointBefore = this.slotHitPointBefore;
					param.SlotHitPointAfter = this.slotHitPointAfter;
					param.AttackBefore = this.attackBefore;
					param.AttackAfter = this.attackAfter;
					param.AttackBaseBefore = this.attackBaseBefore;
					param.AttackBaseAfter = this.attackBaseAfter;
					param.SynchroAttack = this.synchroAttack;
					param.SlotAttackBefore = this.slotAttackBefore;
					param.SlotAttackAfter = this.slotAttackAfter;
					param.DefenseBefore = this.defenseBefore;
					param.DefenseAfter = this.defenseAfter;
					param.DefenseBaseBefore = this.defenseBaseBefore;
					param.DefenseBaseAfter = this.defenseBaseAfter;
					param.SynchroDefense = this.synchroDefense;
					param.SlotDefenseBefore = this.slotDefenseBefore;
					param.SlotDefenseAfter = this.slotDefenseAfter;
					param.ExtraBefore = this.extraBefore;
					param.ExtraAfter = this.extraAfter;
					param.ExtraBaseBefore = this.extraBaseBefore;
					param.ExtraBaseAfter = this.extraBaseAfter;
					param.SynchroExtra = this.synchroDefense;
					param.SlotExtraBefore = this.slotExtraBefore;
					param.SlotExtraAfter = this.slotExtraAfter;
					param.IsSynchroFusion = this.isSynchroFusion;

					return param;
				}
			}

			public void Update()
			{
				if(this.execute)
				{
					this.execute = false;
					if(this.isDummy)
					{
						this.param.SetDummy();
					}
					SetupParam param = this.param.GetConvertSetupParam();
					this.Execute(this.mode, param, this.isClose, this.message);
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
			Open();
		};
		d.OpenMode.Execute += (mode, param, isClose, message) =>
		{
			d.ReadMasterData();
			switch(mode)
			{
				case ModeType.None: Open(); break;
				case ModeType.OK: OpenOK(param, message, isClose, () => { Debug.Log("OK"); }); break;
				case ModeType.YesNo: OpenYesNo(param, message, isClose, () => { Debug.Log("Yes"); }, () => { Debug.Log("No"); }); break;
			};
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
