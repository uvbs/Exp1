/// <summary>
/// マッチングメニュー
/// 
/// 2014/12/10
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Master;

public class GUIMatching : Singleton<GUIMatching>
{
	#region フィールド＆プロパティ
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
		[SerializeField]
		UIPlayTween _rootTween;
		public UIPlayTween RootTween { get { return _rootTween; } }

		[SerializeField]
		GameObject _homeButtonGroup;
		public GameObject HomeButtonGroup { get { return _homeButtonGroup; } }

		[SerializeField]
		GameObject _closeButtonGroup;
		public GameObject CloseButtonGroup { get { return _closeButtonGroup; } }

		[SerializeField]
		UILabel _battleNameLabel;
		public UILabel BattleNameLabel { get { return _battleNameLabel; } }

		[SerializeField]
		UILabel _timeLimitLabel;
		public UILabel TimeLimitLabel { get { return _timeLimitLabel; } }

		[SerializeField]
		UILabel _conditionLabel;
		public UILabel ConditionLabel { get { return _conditionLabel; } }

		[SerializeField]
		UILabel _mapInfoLabel;
		public UILabel MapInfoLabel { get { return _mapInfoLabel; } }
	}

	// アクティブ化フラグ
	bool IsActive { get; set; }
	// ホームボタンを押した時のデリゲート
	System.Action OnHomeFunction { get; set; }
	// 閉じるボタンを押した時のデリゲート
	System.Action OnCloseFunction { get; set; }
	// シリアライズされていないメンバー初期化
	void MemberInit()
	{
		this.IsActive = false;
		this.OnHomeFunction = delegate { };
		this.OnCloseFunction = delegate { };
	}
	#endregion

	#region 初期化
	override protected void Awake()
	{
		base.Awake();
		this.MemberInit();

		// 表示設定
		SetActive(this.IsStartActive);
	}
	#endregion

	#region アクティブ設定
	/// <summary>
	/// 閉じる
	/// </summary>
	public static void Close()
	{
		if (Instance != null) Instance.SetWindowActive(false);
	}
	/// <summary>
	/// アクティブ化
	/// </summary>
	public static void SetActive(bool isActive)
	{
		SetActive(isActive, true, true, null, null);
	}
	/// <summary>
	/// アクティブ化(詳細設定)
	/// </summary>
	public static void SetActive(bool isActive, bool isUseHome, bool isUseClose, System.Action onHome, System.Action onClose)
	{
		if (Instance != null) Instance._SetActive(isActive, isUseHome, isUseClose, onHome, onClose);
	}
	/// <summary>
	/// アクティブ化(大元)
	/// </summary>
	void _SetActive(bool isActive, bool isUseHome, bool isUseClose, System.Action onHome, System.Action onClose)
	{
		this.OnHomeFunction = (onHome != null ? onHome : delegate { });
		this.OnCloseFunction = (onClose != null ? onClose : delegate { });

		// UI設定
		{
			var t = this.Attach;
			if (t.HomeButtonGroup != null)
				t.HomeButtonGroup.SetActive(isUseHome);
			if (t.CloseButtonGroup != null)
				t.CloseButtonGroup.SetActive(isUseClose);
		}

		// ウィンドウアクティブ設定
		this.SetWindowActive(isActive);

		if (isActive)
		{
			Setup();
		}
	}
	/// <summary>
	/// ウィンドウアクティブ設定
	/// </summary>
	void SetWindowActive(bool isActive)
	{
		this.IsActive = isActive;

		// アクティブ化
		if (this.Attach.RootTween != null)
			this.Attach.RootTween.Play(isActive);
		else
			this.gameObject.SetActive(isActive);

		// その他UIの表示設定
		if (isActive)
			GUILobbyMenu.Close();
		GUILobbyResident.SetActive(!isActive);
		GUIScreenTitle.Play(isActive, MasterData.GetText(TextType.TX121_Matching_ScreenTitle));
		GUIHelpMessage.Play(isActive, MasterData.GetText(TextType.TX122_Matching_HelpMessage));
	}
	#endregion

	#region セットアップ
	private void Setup()
	{
		// UNDONE:マッチングするバトルフィールドIDは固定
		BattleFieldMasterData masterData;
		if(!MasterData.TryGetBattleField((int)BattleFieldType.BF008_Shiwasu2, out masterData))
			return;
		// バトルモード名
		if(this.Attach.BattleNameLabel != null)
		{
			this.Attach.BattleNameLabel.text = masterData.Name;
		}
		// バトル時間
		if(this.Attach.TimeLimitLabel != null)
		{
			string timeLimitText = string.Format("{0}{1}",
			                                     (masterData.GameTime / 60).ToString(),MasterData.GetText(TextType.TX026_Minutes));
			this.Attach.TimeLimitLabel.text = timeLimitText;
		}
		// 勝利条件
		if(this.Attach.ConditionLabel != null)
		{
            this.Attach.ConditionLabel.text = BattleMain.GetRuleInfo(BattleMain.GetPlayerTeamType(), masterData.BattleRule);
		}
		// マップ情報
		if(this.Attach.MapInfoLabel != null)
		{
			this.Attach.MapInfoLabel.text = masterData.Info;
		}
	}
	#endregion

	#region NGUIリフレクション
	/// <summary>
	/// ホームボタンを押した時
	/// </summary>
	public void OnHome()
	{
		Close();
		this.OnHomeFunction();
	}
	/// <summary>
	/// 閉じるボタンを押した時
	/// </summary>
	public void OnClose()
	{
		Close();
		this.OnCloseFunction();
	}
	/// <summary>
	/// OKボタンを押した時
	/// </summary>
	public void OnOK()
	{
		Close();
		// UNDONE:マッチングするバトルフィールドIDは固定
		LobbyPacket.SendMatchingEntry(BattleFieldType.BF008_Shiwasu2, Scm.Common.GameParameter.ScoreType.QuickMatching);
	}
	public void OnOK_Cooperation()
	{
		Close();
		// UNDONE:マッチングするバトルフィールドIDは固定
		LobbyPacket.SendMatchingEntry(BattleFieldType.BF010_Cooperation, Scm.Common.GameParameter.ScoreType.QuickMatching);
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
		public bool ececuteClose;
		public bool executeActive;
		public bool isActive;
		public bool isActiveUseHome;
		public bool isActiveUseClose;
	}
	void DebugUpdate()
	{
		var t = this.DebugParam;
		if (t.ececuteClose)
		{
			t.ececuteClose = false;
			Close();
		}
		if (t.executeActive)
		{
			t.executeActive = false;
			this._SetActive(true, t.isActiveUseHome, t.isActiveUseClose,
				() => { Debug.Log("OnHome"); },
				() => { Debug.Log("OnClose"); }
				);
		}
	}
	void OnValidate()
	{
		if (Application.isPlaying)
			this.DebugUpdate();
	}
#endif
	#endregion
}
