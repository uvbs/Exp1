/// <summary>
/// タッチエフェクト
/// 
/// 2014/12/01
/// </summary>
using UnityEngine;
using System.Collections;

public class GUITouchEffect : Singleton<GUITouchEffect>
{
	#region フィールド&プロパティ
	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public GameObject prefab;
		public Transform parent;
	}

	/// <summary>
	/// 2Dカメラ
	/// </summary>
	Camera _screenCamera = null;
	Camera ScreenCamera
	{
		get
		{
			if (_screenCamera == null)
			{
				_screenCamera = NGUITools.FindCameraForLayer(this.gameObject.layer);
			}
			return _screenCamera;
		}
	}
	#endregion

	#region 初期化
	override protected void Awake()
	{
		base.Awake();
	}
	#endregion

	#region アクティブ
	void OnEnable()
	{
		UICamera.onPress += OnPress;
	}

	void OnDisable()
	{
		UICamera.onPress -= OnPress;
	}
	#endregion

	#region エフェクト再生
	/// <summary>
	/// エフェクトの再生
	/// </summary>
	private void Play(Vector3 screenPosition)
	{
		// NULLチェック
		if(this.Attach.prefab == null || this.Attach.parent == null)
			return;

		// 各カメラの取得
		Camera screenCamera = this.ScreenCamera;
		if(screenCamera == null) return;

		// NGUI上の座標に変換する
		Vector3 position;
		position = screenCamera.ScreenToWorldPoint(screenPosition);

		// エフェクトアイテム生成
		GUITouchEffectItem.Create(this.Attach.prefab, this.Attach.parent, position);
	}
	#endregion

	#region タップ
	/// <summary>
	/// タップされた時に呼び出される
	/// </summary>
	public void OnPress(GameObject go, bool isState)
	{
		// タップが離された時にエフェクトを再生させる
		if(UICamera.currentTouch == null) return;
		if(UICamera.currentTouch.pressStarted || isState) return;
		Play(UICamera.currentTouch.pos);
	}
	#endregion

    void Update()
    {
#if UNITY_ANDROID
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("===> Back"); 
            GUISystemMessage.SetModeYesNo("", "是否退出游戏", "是", "否", () =>
            {
                APaymentHelperDemo.Instance.DoLogout();
            }, () => { });
        }
#endif
    }
}
