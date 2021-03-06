/// <summary>
/// リスポーン情報
/// 
/// 2014/07/21
/// </summary>
using UnityEngine;
using System.Collections;

public class GUIRespawnInfo : Singleton<GUIRespawnInfo>
{
	#region フィールド＆プロパティ
	/// <summary>
	/// リスポーン表示フォーマット
	/// </summary>
	[SerializeField]
	string _respawnFormat = "{0:00.000}";
	string RespawnFormat { get { return _respawnFormat; } }

	/// <summary>
	/// 初期化時のアクティブ状態
	/// </summary>
	[SerializeField]
	bool _isStartActive = false;
	bool IsStartActive { get { return _isStartActive; } }

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UIPlayTween rootTween;
		public UISlider remainingSlider;
		public UILabel remainingLabel;
	}

	// アクティブ設定
	bool IsActive { get; set; }
	// リスポーン時間
	float RespawnTime { get; set; }
	// 残り時間
	float RemainingTime { get; set; }
	// スライダーの値
	float SliderValue { get { return (0f < RespawnTime ? RemainingTime / RespawnTime : 0f); } }

	// シリアライズされていないメンバー初期化
	void MemberInit()
	{
		this.IsActive = false;
		this.RespawnTime = 0f;
		this.RemainingTime = 0f;
	}
	#endregion

	#region 初期化
	override protected void Awake()
	{
		base.Awake();
		this.MemberInit();

		// 表示設定
		this._SetActive(this.IsStartActive);
	}
	#endregion

	#region アクティブ設定
	public static void SetActive(bool isActive)
	{
		if (Instance != null) Instance._SetActive(isActive);
	}
	void _SetActive(bool isActive)
	{
		this.IsActive = isActive;

		// アニメーション開始
		this.Attach.rootTween.Play(this.IsActive);
	}
	#endregion

	#region 設定
	public static void SetRespawnTime(float respawnTime, float remainingTime)
	{
		if (Instance != null) Instance._SetRespawnTime(respawnTime, remainingTime);
	}
	void _SetRespawnTime(float respawnTime, float remainingTime)
	{
		this.RespawnTime = respawnTime * 0.1f;
		this.RemainingTime = remainingTime * 0.1f;
		this.SliderUpdate();
		this.LabelUpdate();

		// 時間が０でも開くように変更
		// ０で返って来た時に次の遷移に移らないため
		//if (0f < this.RemainingTime)
		{
			this._SetActive(true);
		}
	}
	#endregion

	#region 更新
	void Update()
	{
		if( !IsActive )
			return;

		this.SliderUpdate();
		this.LabelUpdate();

		this.RemainingTime -= Time.deltaTime;
		if (0f >= this.RemainingTime)
		{
			this.RemainingTime = 0f;
			this._SetActive(false);

			// 0になった時にまだ出撃画面を開いてなければ開く
			if( GUIMapWindow.Mode != GUIMapWindow.MapMode.Respawn && GUIDeckEdit.NowMode == GUIDeckEdit.DeckMode.None )
				GUIMapWindow.SetMode(GUIMapWindow.MapMode.Respawn);
		}
	}
	void SliderUpdate()
	{
		if (this.Attach.remainingSlider != null)
			this.Attach.remainingSlider.value = this.SliderValue;
	}
	void LabelUpdate()
	{
		if (this.Attach.remainingLabel != null)
			this.Attach.remainingLabel.text = string.Format(this.RespawnFormat, this.RemainingTime);
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
		public float respawnTime = 10;
		public float remainingTime = 10;
	}
	void DebugUpdate()
	{
		var t = this.DebugParam;
		if (t.execute)
		{
			t.execute = false;
			this._SetRespawnTime(t.respawnTime * 10, t.remainingTime * 10);
		}
	}
	void OnValidate()
	{
		this.DebugUpdate();
	}
#endif
	#endregion
}
