/// <summary>
/// アイテムソート
/// 
/// 2016/04/11
/// </summary>
using UnityEngine;
using System;
using XUI.ItemSort;

public class GUIItemSort : Singleton<GUIItemSort>
{
	#region フィールド&プロパティ
	/// <summary>
	/// ビューアタッチ
	/// </summary>
	[SerializeField]
	private ItemSortView _viewAttach = null;
	private ItemSortView  ViewAttach { get { return _viewAttach; } }

	/// <summary>
	/// コントローラ
	/// </summary>
	private IController Controller { get; set; }

	/// <summary>
	/// 開始時のアクティブ状態
	/// </summary>
	[SerializeField]
	private bool _isStartActive = false;
	private bool IsStartActive { get { return _isStartActive; } }

	/// <summary>
	/// 現在有効状態になっているソート項目が変化された時の通知
	/// </summary>
	private event Action OnSortPatternChangeEvent = () => { };

	/// <summary>
	/// OKボタンが押され時のイベント通知
	/// </summary>
	private event Action<OKClickEventArgs> OKClickEvent = (e) => { };
	#endregion

	#region 初期化
	protected override void Awake()
	{
		base.Awake();
		this.MemberInit();
		this.Construct();
	}

	void Start()
	{
		if (this.Controller != null)
		{
			// 初期アクティブ設定
			this.Controller.SetActive(this.IsStartActive, true);

			// セットアップ
			this.Controller.Setup(SortPatternType.Name, true);
		}
	}

	private void Construct()
	{
		// モデル生成
		var model = new Model();

		// ビュー生成
		IView view = null;
		if (this.ViewAttach != null)
		{
			view = this.ViewAttach.GetComponent(typeof(IView)) as IView;
		}

		// コントローラ生成
		var controller = new Controller(model, view);
		this.Controller = controller;
		this.Controller.OKClickEvent += this.HandleOKClickEvent;
		this.Controller.OnSortPatternChangeEvent += this.HandleSortPatternChangeEvent;
	}

	/// <summary>
	/// シリアライズされていないメンバー初期化
	/// </summary>
	private void MemberInit()
	{
		this.Controller = null;
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

		this.OnSortPatternChangeEvent = null;
		this.OKClickEvent = null;
	}
	#endregion

	#region アクティブ設定
	/// <summary>
	/// 開く
	/// </summary>
	public static void Open()
	{
		if (Instance == null) { return; }
		if (Instance.Controller != null)
		{
			Instance.Controller.SetActive(true, true);
		}
	}

	/// <summary>
	/// 閉じる
	/// </summary>
	public static void Close()
	{
		if (Instance == null) { return; }
		if (Instance.Controller != null)
		{
			Instance.Controller.SetActive(false, true);
		}
	}
	#endregion

	#region セットアップ
	/// <summary>
	/// データセットアップ処理
	/// </summary>
	public static void Setup(SortPatternType pattern, bool isAscend)
	{
		if (Instance == null || Instance.Controller == null) { return; }
		Instance.Controller.Setup(pattern, isAscend);
	}
	#endregion

	#region ソート項目
	/// <summary>
	/// 現在有効状態になっているソート項目の取得
	/// </summary>
	public static SortPatternType GetSortPattern()
	{
		if (Instance == null || Instance.Controller == null)
		{
			return SortPatternType.None;
		}
		return Instance.Controller.SortPattern;
	}

	private void HandleSortPatternChangeEvent(object sender, EventArgs e)
	{
		// 通知
		this.OnSortPatternChangeEvent();
	}

	/// <summary>
	/// 現在有効状態になっているソート項目が変化された時のイベント登録
	/// </summary>
	public static void AddSortPatternChangeEvent(Action chnageEvent)
	{
		if (Instance == null) { return; }
		Instance.OnSortPatternChangeEvent += chnageEvent;
	}
	/// <summary>
	/// 現在有効状態になっているソート項目が変化された時のイベント削除
	/// </summary>
	public static void RemoveSortPatternChangeEvent(Action chnageEvent)
	{
		if (Instance == null) { return; }
		Instance.OnSortPatternChangeEvent -= chnageEvent;
	}
	#endregion

	#region 昇順/降順
	/// <summary>
	/// 昇順か降順で並び替えるかのフラグ取得
	/// </summary>
	public static bool GetIsAscend()
	{
		if (Instance == null || Instance.Controller == null) { return false; }
		return Instance.Controller.IsAscend;
	}
	#endregion

	#region OKボタン
	/// <summary>
	/// OKボタンクリックイベント登録
	/// </summary>
	public static void AddOKClickEvent(Action<OKClickEventArgs> okClickEvent)
	{
		if (Instance == null) { return; }
		Instance.OKClickEvent += okClickEvent;
	}
	/// <summary>
	/// OKボタンクリックイベント削除
	/// </summary>
	public static void RemoveOKClickEvent(Action<OKClickEventArgs> okClickEvent)
	{
		if (Instance == null) { return; }
		Instance.OKClickEvent -= okClickEvent;
	}

	/// <summary>
	/// OKボタンが押された時に呼び出される
	/// </summary>
	private void HandleOKClickEvent(object sender, OKClickEventArgs e)
	{
		// 通知
		this.OKClickEvent(e);
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
		}
	}

	void DebugInit()
	{
		var d = this.DebugParam;

		d.ExecuteClose += () => { Close(); };
		d.ExecuteActive += () => { Open(); };
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
