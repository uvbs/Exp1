/// <summary>
/// マッチング中
/// 
/// 2014/12/10
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using Scm.Common.GameParameter;

public class GUIMatchingState : Singleton<GUIMatchingState>
{
	#region フィールド＆プロパティ
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
		public UILabel messageLabel;
		public UIPlayTween cancelActiveTween;
		public UIButton cancelButton;
	}

	// シリアライズされていないメンバー初期化
	void MemberInit()
	{
	}

	/// <summary>
	/// マッチング中かどうか
	/// </summary>
	public static bool IsMatching { get { return (MatchingStatus != MatchingStatus.Normal); } }
	// 現在のマッチング状態
	static MatchingStatus MatchingStatus { get { return (NetworkController.ServerValue != null ? NetworkController.ServerValue.MatchingStatus : MatchingStatus.Normal); } }
	// キャンセルボタンをアクティブにするかどうか
	static bool IsCancelActive
	{
		get
		{
			var s = NetworkController.ServerValue;
			if (s != null)
			{
				// false の時はチームに所属していてメンバーの場合のみ（リーダーじゃない）
				return (s.IsJoinedTeam ? s.IsTeamLeader : true);
			}
			return false;
		}
	}
	#endregion

	#region 初期化
	override protected void Awake()
	{
		base.Awake();
		this.MemberInit();

		// 表示設定
		SetMode(MatchingStatus);
	}
	#endregion

	#region モード設定
	public static void Close()
	{
		SetMode(MatchingStatus.Normal);
	}
	public static void SetMode(MatchingStatus status)
	{
		if (Instance != null) Instance._SetMode(status, IsCancelActive);
	}
	public void _SetMode(MatchingStatus status, bool isCancelActive)
	{
		// モード設定
		switch (status)
		{
		case MatchingStatus.Normal: this.SetActive(false, false, ""); break;
		case MatchingStatus.Entry: this.SetActive(true, isCancelActive, MasterData.GetText(TextType.TX123_MatchingState_Entry)); break;
		case MatchingStatus.Matching: this.SetActive(true, isCancelActive, MasterData.GetText(TextType.TX124_MatchingState_Matching)); break;
		case MatchingStatus.Waiting: this.SetActive(true, false, MasterData.GetText(TextType.TX125_MatchingState_Waiting)); break;
		case MatchingStatus.EnterField: this.SetActive(true, false, MasterData.GetText(TextType.TX126_MatchingState_EnterField)); break;
		}

		// マッチングボタン無効化
		GUILobbyResident.UpdateMatchingActive();
		GUILobbyResident.UpdateSingleButtonEnable();
		// ロビー選択ボタン無効化
		GUILobbyResident.UpdateLobbySelectActive();
		// 練習ボタン無効化
		GUILobbyResident.UpdateTrainingButtonActive();
		// ショップメニューボタン無効化
		GUILobbyResident.UpdateShopMenuButtonEnable();
		// マッチングが成立したならバトルフィールドに飛ばす
		if (status == MatchingStatus.EnterField)
		{
			LobbyMain.NextScene_Battle();
		}
	}
	void SetActive(bool isActive, bool isCancelActive, string message)
	{
		// アニメーション開始
		if (this.Attach.rootTween != null)
			this.Attach.rootTween.Play(isActive);
		else
			this.gameObject.SetActive(isActive);

		// UI更新
		{
			var t = this.Attach;
			// メッセージ更新
			if (t.messageLabel != null)
				t.messageLabel.text = message;
			// キャンセルボタン表示
			if (t.cancelButton != null)
				t.cancelButton.gameObject.SetActive(isCancelActive);
			if (t.cancelActiveTween != null)
				t.cancelActiveTween.Play(isCancelActive);
		}
	}
    #endregion

    #region NGUIリフレクション
    bool isQuick = true;
    public bool IsQuick { get { return this.isQuick; }set { this.isQuick = value; } }

    Action CancelDefult = () => {
        if (IsCancelActive)
            LobbyPacket.SendMatchingCancel();
    };

    Action CancelCallback = null;

	public void OnCancel()
	{
        Debug.Log("Cancel");
        if (!this.IsQuick)
        {
            this.CancelCallback.Invoke();
        }
        this.CancelDefult.Invoke();
	}

    public void SetCancelCallback(Action callback)
    {
        this.CancelCallback = callback;
    }

    /*public void OnCancel(Action callback)
    {
        if (IsCancelActive)
        {
            LobbyPacket.SendMatchingCancel();
            callback.Invoke();
        }
    }*/
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
		public bool executeMode;
		public MatchingStatus mode;
		public bool isCancelActive;
	}

	void DebugUpdate()
	{
		var t = this.DebugParam;
		if (t.executeClose)
		{
			t.executeClose = false;
			Close();
		}
		if (t.executeMode)
		{
			t.executeMode = false;
			this._SetMode(t.mode, t.isCancelActive);
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
