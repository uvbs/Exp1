/// <summary>
/// メッセージウィンドウ
/// 
/// 2014/06/12
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIMessageWindow : Singleton<GUIMessageWindow>
{
	#region フィールド＆プロパティ
	/// <summary>
	/// 初期化時のモード
	/// </summary>
	[SerializeField]
	Mode _startActiveMode = Mode.None;
	Mode StartActiveMode { get { return _startActiveMode; } }
	public enum Mode
	{
		None,
		Next,	// メッセージ送り
		OK,		// OKボタン
		YesNo,	// YesNoボタン
		Input,	// 入力ボタン
	}

	/// <summary>
	/// 初期化時のガイドモード
	/// </summary>
	[SerializeField]
	GuideMode _startActiveGuideMode = GuideMode.None;
	GuideMode StartActiveGuideMode { get { return _startActiveGuideMode; } }
	public enum GuideMode
	{
		None,
		Guide2D,		// ガイド2D
		Guide3D_UIBG,	// ガイド3D(3DガイドがBGより手前に表示)
		Guide3D_UIFG,	// ガイド3D(3DガイドがBGに隠れて表示)
	}

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
		public UILabel inputMessageLabel;
		public UILabel okLabel;
		public UILabel yesLabel;
		public UILabel noLabel;
		public UIInput input;
		public UIPanel bgSpritePanel;

		// メッセージ
		public Message message;
		[System.Serializable]
		public class Message
		{
			public UIPlayTween nextTween;
			public UIPlayTween okTween;
			public UIPlayTween yesNoTween;
		}
		// ガイド
		public Guide guide;
		[System.Serializable]
		public class Guide
		{
			public UIPlayTween iconTween;
			public UIPlayTween plateTween;
		}
		// BG
		public BG bg;
		[System.Serializable]
		public class BG
		{
			public GameObject normalGroup;
			public GameObject inputGroup;
		}
	}

	// 現在のモード
	Mode NowMode { get; set; }
	// 現在のガイドモード
	GuideMode NowGuideMode { get; set; }
	// 通常のBGかどうか
	bool IsNormalBG { get; set; }
	// NGUIに対するデリゲート
	System.Action onOK { get; set; }
	System.Action onYes { get; set; }
	System.Action onNo { get; set; }
	System.Action<string> onInputSubmit { get; set; }
	System.Action<string> onInputChange { get; set; }
	System.Action onCloseFinish { get; set; }
	// シリアライズされていないメンバーの初期化
	void MemberInit()
	{
		this.NowMode = (Mode)(-1);
		this.NowGuideMode = (GuideMode)(-1);
		this.IsNormalBG = false;
		this._ClearDelegate();
	}

	// 表示しているメッセージ
	string MessageText
	{
		set
		{
			if (this.Attach.messageLabel != null) this.Attach.messageLabel.text = value;
			if (this.Attach.inputMessageLabel != null) this.Attach.inputMessageLabel.text = value;
		}
	}
	// 表示しているOKボタン名
	string OKButtonText { set { if (this.Attach.okLabel != null) this.Attach.okLabel.text = value; } }
	// 表示しているYesボタン名
	string YesButtonText { set { if (this.Attach.yesLabel != null) this.Attach.yesLabel.text = value; } }
	// 表示しているNoボタン名
	string NoButtonText { set { if (this.Attach.noLabel != null) this.Attach.noLabel.text = value; } }
	// 入力モード時の表示している初期テキスト
	string InputDefaultText { set { if (this.Attach.input != null) this.Attach.input.value = value; } }
	// 入力モード時の入力文字が空だった時に表示するテキスト
	string InputEmptyText { set { if (this.Attach.input != null) this.Attach.input.defaultText = value; } }
	// 入力モード時の文字数制限
	static public int InputCharacterLimit
	{
		get { return Instance != null && Instance.Attach.input != null ? Instance.Attach.input.characterLimit : 0; }
		set { if (Instance != null && Instance.Attach.input != null) Instance.Attach.input.characterLimit = value; }
	}
	#endregion

	#region 初期化
	override protected void Awake()
	{
		base.Awake();
		this.MemberInit();

		// 表示設定
		this.SetActive(this.StartActiveMode, this.StartActiveGuideMode);
	}
	void _ClearDelegate()
	{
		this.onOK = delegate { };
		this.onYes = delegate { };
		this.onNo = delegate { };
		this.onInputSubmit = delegate { };
		this.onInputChange = delegate { };
		this.onCloseFinish = delegate { };
	}
	#endregion

	#region モード設定
	#region SetModeNext
	/// <summary>
	/// 全画面どこでもタッチで次へ進むメッセージ
	/// </summary>
	public static bool SetModeNext(string text, System.Action onOK)
	{
		return SetModeNext(text, true, GuideMode.None, onOK);
	}
	public static bool SetModeNext(string text, bool isClose, System.Action onOK)
	{
		return SetModeNext(text, isClose, GuideMode.None, onOK);
	}
	public static bool SetModeNext(string text, bool isClose, GuideMode guideMode, System.Action onOK)
	{
		if (Instance == null)
			return false;

		Instance.SetupModeNext(text, isClose, onOK);
		Instance.SetActive(Mode.Next, guideMode);
		return true;
	}
	void SetupModeNext(string text, bool isClose, System.Action onOK)
	{
		// デリゲートクリア
		this._ClearDelegate();
		// テキスト設定
		this.MessageText = text;
		// 次へボタン設定
		{
			// ボタンを押してウィンドウが閉じてからアクションを開始する、または
			// ボタンを押した瞬間にアクションを開始する(ウィンドウは閉じない)
			var action = (onOK != null ? onOK : delegate { });
			if (isClose)
				this.onOK = () => { Close(action); };
			else
				this.onOK = action;
		}
	}
	#endregion

	#region SetModeOK
	/// <summary>
	/// 確認ボタン付きメッセージ
	/// </summary>
	public static bool SetModeOK(string text, System.Action onOK)
	{
		return SetModeOK(text, MasterData.GetText(TextType.TX057_Common_YesButton), true, GuideMode.None, onOK);
	}
	public static bool SetModeOK(string text, bool isClose, System.Action onOK)
	{
		return SetModeOK(text, MasterData.GetText(TextType.TX057_Common_YesButton), isClose, GuideMode.None, onOK);
	}
	public static bool SetModeOK(string text, bool isClose, string okButton, System.Action onOK)
	{
		return SetModeOK(text, okButton, isClose, GuideMode.None, onOK);
	}
	public static bool SetModeOK(string text, string okButton, bool isClose, GuideMode guideMode, System.Action onOK)
	{
		if (Instance == null)
			return false;

		Instance.SetupModeOK(text, okButton, isClose, onOK);
		Instance.SetActive(Mode.OK, guideMode);
		return true;
	}
	void SetupModeOK(string text, string okButton, bool isClose, System.Action onOK)
	{
		// デリゲートクリア
		this._ClearDelegate();
		// テキスト設定
		this.MessageText = text;
		// OKボタン設定
		this.OKButtonText = okButton;
		{
			// ボタンを押してウィンドウが閉じてからアクションを開始する、または
			// ボタンを押した瞬間にアクションを開始する(ウィンドウは閉じない)
			var action = (onOK != null ? onOK : delegate { });
			if (isClose)
				this.onOK = () => { Close(action); };
			else
				this.onOK = action;
		}
	}
	#endregion

	#region SetModeYesNo
	/// <summary>
	/// YesNoメッセージ
	/// </summary>
	public static bool SetModeYesNo(string text, System.Action onYes, System.Action onNo)
	{
		return SetModeYesNo(text, MasterData.GetText(TextType.TX057_Common_YesButton), MasterData.GetText(TextType.TX058_Common_NoButton), true, GuideMode.None, onYes, onNo);
	}
	public static bool SetModeYesNo(string text, bool isClose, System.Action onYes, System.Action onNo)
	{
		return SetModeYesNo(text, MasterData.GetText(TextType.TX057_Common_YesButton), MasterData.GetText(TextType.TX058_Common_NoButton), isClose, GuideMode.None, onYes, onNo);
	}
	public static bool SetModeYesNo(string text, string yesButton, string noButton, GuideMode guideMode, System.Action onYes, System.Action onNo)
	{
		return SetModeYesNo(text, yesButton, noButton, true, GuideMode.None, onYes, onNo);
	}
	public static bool SetModeYesNo(string text, string yesButton, string noButton, bool isClose, GuideMode guideMode, System.Action onYes, System.Action onNo)
	{
		if (Instance == null)
			return false;

		Instance.SetupModeYesNo(text, yesButton, noButton, isClose, onYes, onNo);
		Instance.SetActive(Mode.YesNo, guideMode);
		return true;
	}
	void SetupModeYesNo(string text, string yesButton, string noButton, bool isClose, System.Action onYes, System.Action onNo)
	{
		// デリゲートクリア
		this._ClearDelegate();
		// テキスト設定
		this.MessageText = text;
		// Yesボタン設定
		this.YesButtonText = yesButton;
		{
			// ボタンを押してウィンドウが閉じてからアクションを開始する、または
			// ボタンを押した瞬間にアクションを開始する(ウィンドウは閉じない)
			var action = (onYes != null ? onYes : delegate { });
			if (isClose)
				this.onYes = () => { Close(action); };
			else
				this.onYes = action;
		}
		// Noボタン設定
		this.NoButtonText = noButton;
		{
			// ボタンを押してウィンドウが閉じてからアクションを開始する、または
			// ボタンを押した瞬間にアクションを開始する(ウィンドウは閉じない)
			var action = (onNo != null ? onNo : delegate { });
			if (isClose)
				this.onNo = () => { Close(action); };
			else
				this.onNo = action;
		}
	}
	#endregion

	#region SetModeInput
	/// <summary>
	/// 入力フィールド付きメッセージ
	/// </summary>
	public static bool SetModeInput(string text, string defaultText, string emptyText, System.Action onYes, System.Action onNo, System.Action<string> onSubmit, System.Action<string> onChange)
	{
		return SetModeInput(text, defaultText, emptyText, MasterData.GetText(TextType.TX057_Common_YesButton), MasterData.GetText(TextType.TX058_Common_NoButton), true, GuideMode.None, onYes, onNo, onSubmit, onChange);
	}
	public static bool SetModeInput(string text, string defaultText, string emptyText, bool isClose, System.Action onYes, System.Action onNo, System.Action<string> onSubmit, System.Action<string> onChange)
	{
		return SetModeInput(text, defaultText, emptyText, MasterData.GetText(TextType.TX057_Common_YesButton), MasterData.GetText(TextType.TX058_Common_NoButton), isClose, GuideMode.None, onYes, onNo, onSubmit, onChange);
	}
	public static bool SetModeInput(string text, string defaultText, string emptyText, string yesButton, string noButton, System.Action onYes, System.Action onNo, System.Action<string> onSubmit, System.Action<string> onChange)
	{
		return SetModeInput(text, defaultText, emptyText, yesButton, noButton, true, GuideMode.None, onYes, onNo, onSubmit, onChange);
	}
	public static bool SetModeInput(string text, string defaultText, string emptyText, string yesButton, string noButton, bool isClose, GuideMode guideMode, System.Action onYes, System.Action onNo, System.Action<string> onSubmit, System.Action<string> onChange)
	{
		if (Instance == null)
			return false;

		Instance.SetupModeInput(text, defaultText, emptyText, yesButton, noButton, isClose, onYes, onNo, onSubmit, onChange);
		Instance.SetActive(Mode.Input, guideMode);
		return true;
	}
	void SetupModeInput(string text, string defaultText, string emptyText, string yesButton, string noButton, bool isClose, System.Action onYes, System.Action onNo, System.Action<string> onSubmit, System.Action<string> onChange)
	{
		// デリゲートクリア
		this._ClearDelegate();
		// テキスト設定
		this.MessageText = text;
		// Yesボタン設定
		this.YesButtonText = yesButton;
		{
			// ボタンを押してウィンドウが閉じてからアクションを開始する、または
			// ボタンを押した瞬間にアクションを開始する(ウィンドウは閉じない)
			var action = (onYes != null ? onYes : delegate { });
			if (isClose)
				this.onYes = () => { Close(action); };
			else
				this.onYes = action;
		}
		// Noボタン設定
		this.NoButtonText = noButton;
		{
			// ボタンを押してウィンドウが閉じてからアクションを開始する、または
			// ボタンを押した瞬間にアクションを開始する(ウィンドウは閉じない)
			var action = (onNo != null ? onNo : delegate { });
			if (isClose)
				this.onNo = () => { Close(action); };
			else
				this.onNo = action;
		}
		// 文字列設定
		this.InputDefaultText = defaultText;
		this.InputEmptyText = emptyText;
		// 入力完了設定
		this.onInputSubmit = (onSubmit != null ? onSubmit : delegate { });
		// 入力変更設定
		this.onInputChange = (onChange != null ? onChange : delegate { });
	}
	#endregion

	#region Close
	/// <summary>
	/// ウィンドウを閉じる
	/// </summary>
	public static bool Close()
	{
		return Close(null);
	}
	public static bool Close(System.Action onFinish)
	{
		if (Instance == null)
			return false;

		Instance.SetupClose(onFinish);
		Instance.SetActive(Mode.None, GuideMode.None);
		return true;
	}
	void SetupClose(System.Action onFinish)
	{
		// デリゲートクリア
		this._ClearDelegate();
		// 閉じた後の処理設定
		this.onCloseFinish = (onFinish != null ? onFinish : delegate { });
	}
	#endregion

	#region SetActive
	/// <summary>
	/// モードごとのアクティブ設定
	/// </summary>
	void SetActive(Mode mode, GuideMode guideMode)
	{
		// 表示モード
		if (this.NowMode != mode)
		{
			this.NowMode = mode;

			var m = this.Attach.message;
			switch (NowMode)
			{
			case Mode.None: this.SetActiveMode(false, this.IsNormalBG, null); break;
			case Mode.Next: this.SetActiveMode(true, true, m.nextTween); break;
			case Mode.OK: this.SetActiveMode(true, true, m.okTween); break;
			case Mode.YesNo: this.SetActiveMode(true, true, m.yesNoTween); break;
			case Mode.Input: this.SetActiveMode(true, false, m.yesNoTween); break;
			}
		}

		// ガイドモード
		if (this.NowGuideMode != guideMode)
		{
			this.NowGuideMode = guideMode;

			var g = this.Attach.guide;
			switch (guideMode)
			{
			case GuideMode.None: this.SetActiveGuideMode(null, LayerNumber.UIFG); break;
			case GuideMode.Guide2D: this.SetActiveGuideMode(g.iconTween, LayerNumber.UIFG); break;
			case GuideMode.Guide3D_UIBG: this.SetActiveGuideMode(g.plateTween, LayerNumber.UIBG); break;
			case GuideMode.Guide3D_UIFG: this.SetActiveGuideMode(g.plateTween, LayerNumber.UIFG); break;
			}
		}
	}
	void SetActiveMode(bool isRootActive, bool isNormalBG, UIPlayTween activeTween)
	{
		// ルート表示
		if (this.Attach.rootTween != null)
			this.Attach.rootTween.Play(isRootActive);
		else
			this.gameObject.SetActive(isRootActive);
		if (!isRootActive)
			return;

		// BGアクティブ化
		{
			var bg = this.Attach.bg;
			this.IsNormalBG = isNormalBG;
			if (bg.normalGroup != null)
				bg.normalGroup.SetActive(isNormalBG);
			if (bg.inputGroup != null)
				bg.inputGroup.SetActive(!isNormalBG);
		}

		// ボタンアクティブ化
		{
			var m = this.Attach.message;
			var list = new List<UIPlayTween>();
			list.Add(m.nextTween);
			list.Add(m.okTween);
			list.Add(m.yesNoTween);
			foreach (var t in list)
			{
				if (t == null)
					continue;
				bool isActive = (activeTween == t);
				t.Play(isActive);
			}
		}
	}
	void SetActiveGuideMode(UIPlayTween activeTween, int layer)
	{
		// ガイドアクティブ化
		var g = this.Attach.guide;
		var list = new List<UIPlayTween>();
		list.Add(g.iconTween);
		list.Add(g.plateTween);
		foreach (var t in list)
		{
			if (t == null)
				continue;
			bool isActive = (activeTween == t);
			t.Play(isActive);
		}

		// パネルのレイヤーを変える
		{
			var p = this.Attach.bgSpritePanel;
			if (p != null)
			{
				NGUITools.SetLayer(p.gameObject, layer);
			}
		}
	}
	#endregion
	#endregion

	#region NGUIリフレクション
	public void OnNextOK()
	{
		this.onOK();
	}
	public void OnOK()
	{
		this.onOK();
	}
	public void OnYes()
	{
		this.onYes();
	}
	public void OnNo()
	{
		this.onNo();
	}
	public void OnInputYes()
	{
		this.onYes();
	}
	public void OnInputNo()
	{
		this.onNo();
	}
	public void OnInputSubmit()
	{
		if (UIInput.current != null)
		{
			UIInput.current.RemoveFocus();
			this.onInputSubmit(UIInput.current.value);
		}
	}
	public void OnInputChange()
	{
		if (UIInput.current != null)
		{
			this.onInputChange(UIInput.current.value);
		}
	}
	public void OnCloseFinish()
	{
		this.onCloseFinish();
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
		public bool executeMode;
		public Mode mode = Mode.None;
		public GuideMode guideMode = GuideMode.None;
		public bool isClose = true;
		public string text = "Text";
		public string inputDefaultText = "Default Text";
		public string inputEmptyText = "Empty Text";
		public string okButton = "OK";
		public string yesButton = "Yes";
		public string noButton = "No";
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
			switch (t.mode)
			{
			case Mode.None:
				Close();
				break;
			case Mode.Next:
				SetModeNext(t.text, t.isClose, t.guideMode, () => { Debug.Log("Next"); });
				break;
			case Mode.OK:
				SetModeOK(t.text, t.okButton, t.isClose, t.guideMode, () => { Debug.Log("OK"); });
				break;
			case Mode.YesNo:
				SetModeYesNo(t.text, t.yesButton, t.noButton, t.isClose, t.guideMode,
					() => { Debug.Log("Yes"); },
					() => { Debug.Log("No"); }
				);
				break;
			case Mode.Input:
				SetModeInput(t.text, t.inputDefaultText, t.inputEmptyText, t.yesButton, t.noButton, t.isClose, t.guideMode,
					() => { Debug.Log("Yes"); },
					() => { Debug.Log("No"); },
					(s) => { Debug.Log("OnSubmit:" + s); },
					(s) => { Debug.Log("OnChange:" + s); }
				);
				break;
			}
		}
	}
	void OnValidate()
	{
		this.DebugUpdate();
	}
#endif
	#endregion
}
