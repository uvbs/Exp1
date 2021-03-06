/// <summary>
/// デバッグログ
/// 
/// 2015/03/17
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIDebugLog : Singleton<GUIDebugLog>
{
	#region フィールド＆プロパティ
	/// <summary>
	/// 初期化時のアクティブ状態
	/// </summary>
	[SerializeField]
	bool _isStartActive = false;
	bool IsStartActive { get { return _isStartActive; } }

	/// <summary>
	/// スクロールビュー
	/// </summary>
	[SerializeField]
	GUIItemScrollView _itemScrollView;
	GUIItemScrollView ItemScrollView { get { return _itemScrollView; } }
	[System.Serializable]
	public class GUIItemScrollView : ScrollView<GUIDebugLogItem, List<GUIDebugLogItem>>
	{
		public int Count { get { return this.ItemList.Count; } }

		protected override GUIDebugLogItem Create(GameObject prefab, Transform parent, int itemIndex)
		{
			return GUIDebugLogItem.Create(prefab, parent, itemIndex);
		}
		protected override void Add(GUIDebugLogItem item)
		{
			this.ItemList.Add(item);
		}
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
		[SerializeField]
		UIPlayTween _rootTween;
		public UIPlayTween RootTween { get { return _rootTween; } }

		[SerializeField]
		UIButton _okButton;
		public UIButton OKButton { get { return _okButton; } }

		[SerializeField]
		UILabel _okButtonLabel;
		public UILabel OKButtonLabel { get { return _okButtonLabel; } }

		[SerializeField]
		GameObject _hostGroup = null;
		public GameObject HostGroup { get { return _hostGroup; } }

		[SerializeField]
		UIDragObject _dragObject;
		public UIDragObject DragObject { get { return _dragObject; } }

		[SerializeField]
		UIDragResize _dragResize;
		public UIDragResize DragResize { get { return _dragResize; } }

		[SerializeField]
		ScrollViewAttach _scrollView;
		public ScrollViewAttach ScrollView { get { return _scrollView; } }
	}

	// アクティブフラグ
	bool IsActive { get; set; }
	// OKボタンを押した時のデリゲート
	System.Action OnOKFunction { get; set; }
	// 接続先ボタンを押した時のデリゲート
	System.Action<int> OnHostFunction { get; set; }
	// メッセージ変更用のアイテムディクショナリ
	Dictionary<int, GUIDebugLogItem> ItemDict { get; set; }
	// シリアライズされていないメンバーの初期化
	void MemberInit()
	{
		this.IsActive = false;
		this.OnOKFunction = delegate { };
		this.OnHostFunction = delegate { };
		this.ItemDict = new Dictionary<int, GUIDebugLogItem>();
	}
	#endregion

	#region 初期化
	/// <summary>
	/// 初期化
	/// </summary>
	override protected void Awake()
	{
		base.Awake();
		this.MemberInit();

		// スクロールビューを初期化
		this.ItemScrollView.Create(this.Attach.ScrollView);
		// テーブル内初期化
		this.Clear();

		// 表示設定
		SetActive(this.IsStartActive);
		ClearOKButton();
		DeactiveOKButton();
		DeactiveHostButton();
	}
	/// <summary>
	/// テーブル内初期化
	/// </summary>
	void Clear()
	{
		this.ItemScrollView.Clear();

		// テーブル整形
		this.OnReposition();
		this.ItemScrollView.SetScroll(0, 1f);
	}
	#endregion

	#region アクティブ設定
	/// <summary>
	/// 閉じる
	/// </summary>
	[System.Diagnostics.Conditional("XW_DEBUG")]
	public static void Close()
	{
		if (Instance != null) Instance.SetWindowActive(false);
	}
	/// <summary>
	/// アクティブ化
	/// </summary>
	[System.Diagnostics.Conditional("XW_DEBUG")]
	public static void SetActive(bool isActive)
	{
		if (Instance != null) Instance._SetActive(isActive, false, 0, 0, 0, 0);
	}
	/// <summary>
	/// アクティブ化(詳細設定)
	/// </summary>
	[System.Diagnostics.Conditional("XW_DEBUG")]
	public static void SetActive(bool isActive, int x, int y, int width, int height)
	{
		if (Instance != null) Instance._SetActive(isActive, true, x, y, width, height);
	}
	/// <summary>
	/// アクティブ化(大元)
	/// </summary>
	void _SetActive(bool isActive, bool isResize, int x, int y, int width, int height)
	{
		// リサイズ設定
		if (isResize)
			this._SetResize(x, y, width, height);

		// ウィンドウアクティブ設定
		this.SetWindowActive(isActive);
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
	}
	#endregion

	#region OKボタン
	/// <summary>
	/// OKボタンの設定をクリアする
	/// </summary>
	[System.Diagnostics.Conditional("XW_DEBUG")]
	public static void ClearOKButton()
	{
		if (Instance != null)
		{
			var t = Instance.Attach;
			if (t.OKButton != null)
				Instance._SetOKButton("OK", null);
		}
	}
	/// <summary>
	/// OKボタンの設定
	/// </summary>
	[System.Diagnostics.Conditional("XW_DEBUG")]
	public static void SetOKButton(string buttonName, System.Action onOK)
	{
		if (Instance != null) Instance._SetOKButton(buttonName, onOK);
	}
	/// <summary>
	/// OKボタンのアクティブ設定
	/// </summary>
	void _SetOKButton(string buttonName, System.Action onOK)
	{
		this.OnOKFunction = (onOK != null ? onOK : delegate { });

		// UI設定
		{
			var t = this.Attach;
			if (t.OKButtonLabel != null)
				t.OKButtonLabel.text = buttonName;
		}
	}
	/// <summary>
	/// OKボタン非表示
	/// </summary>
	[System.Diagnostics.Conditional("XW_DEBUG")]
	public static void DeactiveOKButton()
	{
		if (Instance != null) Instance._SetActiveOKButton(false);
	}
	/// <summary>
	/// OKボタン表示
	/// </summary>
	[System.Diagnostics.Conditional("XW_DEBUG")]
	public static void ActiveOKButton()
	{
		if (Instance != null) Instance._SetActiveOKButton(true);
	}
	/// <summary>
	/// OKボタンのアクティブ設定
	/// </summary>
	/// <param name="isActive"></param>
	void _SetActiveOKButton(bool isActive)
	{
		var t = this.Attach;
		if (t.OKButton != null)
			t.OKButton.gameObject.SetActive(isActive);
	}
	#endregion

	#region 接続先選択設定
	/// <summary>
	/// 接続先ボタン非表示
	/// </summary>
	[System.Diagnostics.Conditional("XW_DEBUG")]
	public static void DeactiveHostButton()
	{
		if (Instance != null) Instance._SetActiveHostButton(false, null);
	}
	/// <summary>
	/// 接続先ボタン表示
	/// </summary>
	[System.Diagnostics.Conditional("XW_DEBUG")]
	public static void ActiveHostButton(System.Action<int> onHost)
	{
		if (Instance != null) Instance._SetActiveHostButton(true, onHost);
	}
	/// <summary>
	/// 接続先ボタンアクティブ設定
	/// </summary>
	/// <param name="isActive"></param>
	void _SetActiveHostButton(bool isActive, System.Action<int> onHost)
	{
		this.OnHostFunction = (onHost != null ? onHost : delegate { });

		var t = this.Attach;
		if (t.HostGroup != null)
			t.HostGroup.SetActive(isActive);
	}
	#endregion

	#region 位置サイズ設定
	/// <summary>
	/// 位置とサイズを設定する
	/// 1280*720を基本とした位置設定
	/// 解像度が変わっても同じサイズや位置になるように内部で調整する
	/// </summary>
	[System.Diagnostics.Conditional("XW_DEBUG")]
	public static void SetResize(int x, int y, int width, int height)
	{
		if (Instance != null) Instance._SetResize(x, y, width, height);
	}
	/// <summary>
	/// 位置とサイズを設定する
	/// </summary>
	void _SetResize(int x, int y, int width, int height)
	{
		this._SetPosition(x, y);
		this._SetSize(width, height);
	}
	/// <summary>
	/// 位置設定
	/// 1280*720を基本とした位置設定
	/// 解像度が変わっても同じサイズや位置になるように内部で調整する
	/// </summary>
	[System.Diagnostics.Conditional("XW_DEBUG")]
	public static void SetPosition(int x, int y)
	{
		if (Instance != null) Instance._SetPosition(x, y);
	}
	/// <summary>
	/// 位置設定
	/// </summary>
	void _SetPosition(int x, int y)
	{
		var t = this.Attach;
		if (t.DragObject != null && t.DragObject.target != null)
		{
			// アスペクト比による調整
			// UIRoot で縦比率調整が入っている前提
			var root = t.DragObject.contentRect.root;
			if (root != null)
			{
				float initialAspectX = (float)root.manualWidth / NGUITools.screenSize.x;
				float initialAspectY = (float)root.manualHeight / NGUITools.screenSize.y;
				float aspect = initialAspectY / initialAspectX;
				x = (int)(x * aspect);
			}

			var trans = t.DragObject.target.transform;
			trans.localPosition = new Vector3(x, -y, trans.localPosition.z);
		}
	}
	/// <summary>
	/// サイズ設定
	/// 1280*720を基本とした位置設定
	/// 解像度が変わっても同じサイズや位置になるように内部で調整する
	/// </summary>
	[System.Diagnostics.Conditional("XW_DEBUG")]
	public static void SetSize(int width, int height)
	{
		if (Instance != null) Instance._SetSize(width, height);
	}
	/// <summary>
	/// サイズ設定
	/// </summary>
	void _SetSize(int width, int height)
	{
		var t = this.Attach;
		if (t.DragResize != null && t.DragResize.target != null)
		{
			// アスペクト比による調整
			// UIRoot で縦比率調整が入っている前提
			var root = t.DragResize.target.root;
			if (root != null)
			{
				float initialAspectX = (float)root.manualWidth / NGUITools.screenSize.x;
				float initialAspectY = (float)root.manualHeight / NGUITools.screenSize.y;
				float aspect = initialAspectY / initialAspectX;
				width = (int)(width * aspect);
			}

			var w = t.DragResize.target;
			w.width = width;
			w.height = height;
			w.UpdateAnchors();
		}
	}
	#endregion

	#region メッセージ追加
	/// <summary>
	/// メッセージ追加
	/// </summary>
	//[System.Diagnostics.Conditional("XW_DEBUG")]
	public static void AddMessage(string text)
	{
		if (Instance != null) Instance._AddMessage(0, text);
        Debug.Log(text);
	}

    /// <summary>
    /// メッセージ追加
    /// item = 追加したアイテムを取得する
    /// </summary>
    //[System.Diagnostics.Conditional("XW_DEBUG")]
	public static void AddMessage(int indent, string text)
	{
		if (Instance != null) Instance._AddMessage(indent, text);
        Debug.Log(new string('-', indent) + text);
	}
	/// <summary>
	/// メッセージ追加
	/// </summary>
	void _AddMessage(int indent, string text)
	{
		string tab = "";
		for (int i = 0; i < indent; i++)
		{
			tab += "----";
		}
		this.ItemDict[indent] = this.AddItem(tab+text, this.ItemScrollView.Count);
		Reposition();
	}
	/// <summary>
	/// アイテム追加
	/// 追加したアイテムを返す
	/// </summary>
	GUIDebugLogItem AddItem(string text, int index)
	{
		// アイテム追加
		var item = this.ItemScrollView.AddItem(index);
		item.Setup(text);
		return item;
	}
	#endregion

	#region メッセージ変更
	/// <summary>
	/// 直前に追加したメッセージの内容を変更する
	/// </summary>
	[System.Diagnostics.Conditional("XW_DEBUG")]
	public static void ChangeMessage(string text)
	{
		Instance._ChangeMessage(0 ,text);
	}
	/// <summary>
	/// アイテムを指定してメッセージの内容を変更する
	/// </summary>
	[System.Diagnostics.Conditional("XW_DEBUG")]
	public static void ChangeMessage(int indent, string text)
	{
		if (Instance != null) Instance._ChangeMessage(indent, text);
	}
	/// <summary>
	/// アイテムを指定してメッセージの内容を変更する
	/// </summary>
	void _ChangeMessage(int indent, string text)
	{
		GUIDebugLogItem item;
		if (this.ItemDict.TryGetValue(indent, out item))
		{
			string tab = "";
			for (int i = 0; i < indent; i++)
			{
				tab += "----";
			}
			// テキストを変更する
			item.SetText(tab + text);
		}
	}
	#endregion

	#region 再配置
	/// <summary>
	/// 再配置
	/// </summary>
	[System.Diagnostics.Conditional("XW_DEBUG")]
	public static void Reposition()
	{
		if (Instance != null) Instance._Reposition();
	}
	/// <summary>
	/// 再配置
	/// </summary>
	void _Reposition()
	{
		this.ItemScrollView.Reposition();
		this.ScrollEnd();
	}
	/// <summary>
	/// スクロールを一番下に持っていく
	/// </summary>
	void ScrollEnd()
	{
		if (this.ItemScrollView.VScrollValue <= 0.9f)
		{
			var t = this.ItemScrollView.Attach;
			if (t.scrollView != null)
			{
				//t.scrollView.UpdatePosition();
				t.scrollView.UpdateScrollbars();
			}
			return;
		}

		this.ItemScrollView.SetScroll(0, 1f);
	}
	/// <summary>
	/// スクロールの設定
	/// </summary>
	[System.Diagnostics.Conditional("XW_DEBUG")]
	public static void SetScroll(float x, float y)
	{
		if (Instance != null) Instance._SetScroll(x, y);
	}
	/// <summary>
	/// スクロールの設定
	/// </summary>
	void _SetScroll(float x, float y)
	{
		this.ItemScrollView.SetScroll(x, y);
	}
	#endregion

	#region NGUIリフレクション
	/// <summary>
	/// 閉じるボタンを押した時
	/// </summary>
	public void OnClose()
	{
		Close();
	}
	/// <summary>
	/// クリアボタンを押した時
	/// </summary>
	public void OnClear()
	{
		this.Clear();
	}
	/// <summary>
	/// OKボタンを押した時
	/// </summary>
	public void OnOK()
	{
		this.OnOKFunction();
	}
	/// <summary>
	/// 接続先ボタン(公開)を押した時
	/// </summary>
	public void OnHostOpen()
	{
		this.OnHostFunction(0);
	}
	/// <summary>
	/// 接続先ボタン(公開テスト)を押した時
	/// </summary>
	public void OnHostOpenTest()
	{
		this.OnHostFunction(1);
	}
	/// <summary>
	/// 接続先ボタン(開発)を押した時
	/// </summary>
	public void OnHostDevelopment()
	{
		this.OnHostFunction(2);
	}
	/// <summary>
	/// 再配置処理
	/// </summary>
	public void OnReposition()
	{
		this._Reposition();
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
		public bool executeResize;
		public Rect resizeRect;
		public bool executeItem;
		public string itemText;
		public bool executeOK;
		public string okButtonName;
	}
	void DebugUpdate()
	{
		var t = this.DebugParam;
		if (t.executeClose)
		{
			t.executeClose = false;
			Close();
		}
		if (t.executeActive)
		{
			t.executeActive = false;
			SetActive(true);
		}
		if (t.executeResize)
		{
			t.executeResize = false;
			SetResize((int)t.resizeRect.x, (int)t.resizeRect.y, (int)t.resizeRect.width, (int)t.resizeRect.height);
		}
		if (t.executeItem)
		{
			t.executeItem = false;
			AddMessage(t.itemText);
		}
		if (t.executeOK)
		{
			t.executeOK = false;
			this._SetOKButton(t.okButtonName, () => { Debug.Log("OK"); });
		}
	}
	/// <summary>
	/// OnValidate はInspector上で値の変更があった時に呼び出される
	/// GameObject がアクティブ状態じゃなくても呼び出されるのでアクティブ化するときには有効
	/// </summary>
	void OnValidate()
	{
		if (Application.isPlaying)
			this.DebugUpdate();
	}
#endif
	#endregion
}
