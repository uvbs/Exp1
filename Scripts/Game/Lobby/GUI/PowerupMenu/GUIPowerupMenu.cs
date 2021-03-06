/// <summary>
/// 強化メニュー
/// 
/// 2016/03/18
/// </summary>
using UnityEngine;
using System;

using XUI.PowerupMenu;

public class GUIPowerupMenu : Singleton<GUIPowerupMenu>
{
	#region フィールド＆プロパティ
	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	PowerupMenuView _viewAttach = null;
	PowerupMenuView ViewAttach { get { return _viewAttach; } }


	/// <summary>
	/// </summary>
	[SerializeField]
	bool _isStartActive = false;
	bool IsStartActive { get { return _isStartActive; } }


	// コントローラー
	IController Controller { get; set; }

	// シリアライズされていないメンバー初期化
	void MemberInit()
	{
		this.Controller = null;
	}
	#endregion

	#region 初期化
	override protected void Awake()
	{
		base.Awake();
		this.MemberInit();

	}
	void Start()
	{
		this.Construct();
		// 初期アクティブ設定
		this.SetActive(this.IsStartActive, true, this.IsStartActive);
	}
	void Construct()
	{
		// モデル生成
		var model = new Model();

		// ビュー生成
		IView view = null;
		if (this.ViewAttach != null)
		{
			view = this.ViewAttach.GetComponent(typeof(IView)) as IView;
		}

		// コントローラー生成
		var controller = new Controller(model, view);
		this.Controller = controller;

	}
	#endregion

	#region アクティブ設定
	/// <summary>
	/// 閉じる
	/// </summary>
	public static void Close()
	{
		if (Instance != null) Instance.SetActive(false, false, false);
	}
	/// <summary>
	/// 開く
	/// </summary>
	public static void Open()
	{
		if (Instance != null) Instance.SetActive(true, false, true);
	}
	/// <summary>
	/// 開き直す
	/// </summary>
	public static void ReOpen()
	{
		if (Instance != null) Instance.SetActive(true, false, false);
	}
	/// <summary>
	/// アクティブ設定
	/// </summary>
	void SetActive(bool isActive, bool isTweenSkip, bool isSetup)
	{
		if (isSetup)
		{
			this.Setup();
		}

		if (this.Controller != null)
		{
			this.Controller.SetActive(isActive, isTweenSkip);
		}
	}
	#endregion

	#region 各種情報更新
	/// <summary>
	/// 初期設定
	/// </summary>
	void Setup()
	{
		if (this.Controller != null)
		{
			this.Controller.Setup();
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

		}

	}
	void DebugInit()
	{

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