/// <summary>
/// ロゴ表示
/// 
/// 2015/11/13
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using XUI.Logo;

public class GUILogo : Singleton<GUILogo>
{
	#region フィールド＆プロパティ
	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	LogoView _viewSttach = null;
	LogoView ViewAttach { get { return _viewSttach; } }

	/// <summary>
	/// ロゴ表示リスト
	/// </summary>
	[SerializeField]
	List<Model> _logoList = null;
	List<Model> LogoList { get { return _logoList; } }

	/// <summary>
	/// 開始時のアクティブ状態
	/// </summary>
	[SerializeField]
	bool _isStartActive = false;
	bool IsStartActive { get { return _isStartActive; } }

	/// <summary>
	/// ロゴ表示が終わっているかどうか
	/// </summary>
	public static bool IsFinish
	{
		get
		{
			if (Instance == null) return true;
			if (Instance.Controller == null) return true;
			return Instance.Controller.IsFinish;
		}
	}

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
		this.MemberInit();

		// モデル生成
		var models = new Models();
		this.LogoList.ForEach((logo) => { models.Add(logo); });

		// ビュー生成
		IView view = null;
		if (this.ViewAttach != null)
		{
			view = this.ViewAttach.GetComponent(typeof(IView)) as IView;
		}

		// コントローラー生成
		var controller = new Controller(models, view);
		this.Controller = controller;
	}
	#endregion

	#region アクティブ設定
	public static void Open()
	{
		if (Instance != null) Instance.SetActive(true, false, true);
	}
	/// <summary>
	/// 閉じる
	/// </summary>
	public static void Close()
	{
		if (Instance != null) Instance.SetActive(false, false, false);
	}
	/// <summary>
	/// アクティブ設定
	/// </summary>
	void SetActive(bool isActive, bool isTweenSkip, bool isSetup)
	{
		if (this.Controller != null)
		{
			if (isSetup)
			{
				this.Controller.Setup();
			}
			this.Controller.SetActive(isActive, isTweenSkip);
		}
	}
	#endregion

	#region 更新
	void Update()
	{
		if (this.Controller != null)
		{
			this.Controller.Update();
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
		public bool executeClose;
		public bool executeActive;
	}
	void DebugUpdate()
	{
		var t = this.DebugParam;
		if (t.executeClose)
		{
			t.executeClose = false;
			FiberController.AddFiber(this.DebugCloseCoroutine());
		}
		if (t.executeActive)
		{
			t.executeActive = false;
			Open();
			this.Construct();
		}
	}
	IEnumerator DebugCloseCoroutine()
	{
		Close();
		yield break;
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
