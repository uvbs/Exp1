/// <summary>
/// タイトルGUI
/// 
/// 2015/03/10
/// </summary>
using UnityEngine;
using System.Collections;

public class GUITitle : Singleton<GUITitle>
{
	#region フィールド&プロパティ
	/// <summary>
	/// タイトルビュー
	/// </summary>
	[SerializeField]
	XUI.Title.TitleView titleView = null;

	/// <summary>
	/// タイトルモデル
	/// </summary>
	[SerializeField]
	private XUI.Title.Model titleModel = null;

	/// <summary>
	/// モデル
	/// </summary>
	private XUI.Title.IModel model = null;

	/// <summary>
	/// ビュー
	/// </summary>
	private XUI.Title.IView view = null;

	/// <summary>
	/// コントローラ
	/// </summary>
	private XUI.Title.Controller controller = null;
	#endregion

	#region 初期化
	protected override void Awake()
	{
		base.Awake();

		Construct();
	}
	private void Construct()
	{
		// コントローラ生成
		this.view = this.titleView;
		this.model = this.titleModel;
		this.controller = new XUI.Title.Controller(this.model, this.view);
	}
	#endregion

	#region アクティブ設定
	/// <summary>
	/// アクティブの設定
	/// </summary>
	/// <param name="isActive"></param>
	public static void SetActive(bool isActive)
	{
		if (Instance == null) { return; }
		Instance.view.SetActive(isActive);
	}
	#endregion

	#region 開く
	/// <summary>
	/// 開始時状態のタイトルを開く
	/// </summary>
	public static void OpenStartTitle()
	{
		if (Instance == null) { return; }
		Instance.controller.OpenStartTitle();
	}

	/// <summary>
	/// 情報状態のタイトルを開く
	/// </summary>
	public static void OpenInfo()
	{
		if(Instance == null) { return; }
		Instance.controller.OpenInfo();
	}
	#endregion

	#region 閉じる
	/// <summary>
	/// タイトルを閉じる
	/// </summary>
	public static void Close()
	{
		if (Instance == null) { return; }
		Instance.controller.Close();
	}
	#endregion


	#region デバッグ
#if UNITY_EDITOR && XW_DEBUG
	[SerializeField]
	GUIDebugParam debugParam = new GUIDebugParam();
	GUIDebugParam DebugParam { get { return debugParam; } }
	[System.Serializable]
	public class GUIDebugParam : GUIDebugParamBase
	{
	}

	void DebugInit()
	{
		var d = this.DebugParam;

		d.ExecuteClose += Close;
		d.ExecuteActive += OpenStartTitle;
	}
	bool isDebugInit = false;
	void DebugUpdate()
	{
		if(!this.isDebugInit)
		{
			this.isDebugInit = true;
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
