/// <summary>
/// チャットログ
/// 
/// 2014/06/09
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIChatLog : Singleton<GUIChatLog>
{
	#region フィールド＆プロパティ
	/// <summary>
	/// 初期化時のアクティブ状態
	/// </summary>
	[SerializeField] bool _isStartActive = false;
	public bool IsStartActive { get { return _isStartActive; } }

	/// <summary>
	/// スクロールビュー
	/// </summary>
	[SerializeField] GUIItemScrollView _itemScrollView;
	public GUIItemScrollView ItemScrollView { get { return _itemScrollView; } }
	[System.Serializable]
	public class GUIItemScrollView : ScrollView<GUIChatItem, LinkedList<GUIChatItem>>
	{
		protected override GUIChatItem Create(GameObject prefab, Transform parent, int itemIndex)
		{
			return GUIChatItem.Create(prefab, parent, itemIndex);
		}
		protected override void Add(GUIChatItem item)
		{
			this.ItemList.AddLast(item);
		}
		public void RemoveFirst()
		{
			var item = this.ItemList.First.Value;
			Object.Destroy(item.gameObject);
			this.ItemList.RemoveFirst();
		}
	}

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField] AttachObject _attach;
	public AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UIPlayTween rootTween;
		public UIScrollBar scrollBar;
		public UIDragObject dragObject;
		public UIDragResize dragResize;
		public SpringPosition spring;
		public ScrollViewAttach scrollView;
	}

	public bool IsActive { get; private set; }
	public int Count { get; private set; }
	#endregion

	#region 初期化
	override protected void Awake()
	{
		base.Awake();

		// スクロールビューを初期化
		this.ItemScrollView.Create(this.Attach.scrollView);
		// アイテム全削除
		this.ItemScrollView.DestroyItem();
		// チャットログからアイテム作成
		ScmParam.Common.ChatLog.ForEach(this.AddItem);
		// テーブル整形
		this.OnReposition();
		this.ItemScrollView.SetScroll(0, 1f);
		// ウィンドウをドラッグして画面外に出た時に
		// 画面内に戻ってくる処理の終了処理設定
		this.Attach.spring.onFinished = this.SetPosition;

		// コンフィグから設定する
#if UNITY_EDITOR
		if (ApplicationController.Instance != null)
#endif
		{
			// 位置設定
			Transform t = this.Attach.dragObject.target.transform;
			t.localPosition = new Vector3(ConfigFile.System.LogPosX, ConfigFile.System.LogPosY, t.localPosition.z);
			// サイズ設定
			UIWidget w = this.Attach.dragResize.target;
			w.width = ConfigFile.System.LogWidth;
			w.height = ConfigFile.System.LogHeight;
			w.UpdateAnchors();
			// テーブル整形
			this.OnReposition();
		}

		// 表示設定
		bool isActive = this.IsStartActive;
#if UNITY_EDITOR
		if (ApplicationController.Instance != null)
#endif
		{
			isActive = ConfigFile.System.IsOpenLog;
		}
		// バトル中は表示をオフにする
		if (ScmParam.Common.AreaType == Scm.Common.GameParameter.AreaType.Field)
		{
			isActive = false;
		}
		this._SetActive(isActive);
	}
	#endregion

	#region アクティブ設定
	public static void SetActive(bool isActive)
	{
		if (Instance != null)	Instance._SetActive(isActive);
	}
	void _SetActive(bool isActive)
	{
		this.IsActive = isActive;

		// コンフィグ設定
		ConfigFile.System.IsOpenLog = isActive;
		// アニメーション開始
		this.Attach.rootTween.Play(this.IsActive);
	}
	public static void Toggle()
	{
		if (Instance != null)	Instance._SetActive(!Instance.IsActive);
	}
	#endregion

	#region メッセージ
	public static void AddMessage(ChatInfo chatInfo)
	{
		ScmParam.Common.ChatLog.Add(chatInfo);
		if (Instance != null)	Instance._AddMessage(chatInfo);
	}
	void _AddMessage(ChatInfo chatInfo)
	{
		this.AddItem(chatInfo, this.Count);
		this.OnReposition();
	}
	void AddItem(ChatInfo chatInfo, int index)
	{
		// ログが最大数を超えていたら削除
		if (this.Count >= ScmParam.Common.ChatLog.LogMax)
			this.ItemScrollView.RemoveFirst();

		// アイテム追加
		var t = this.ItemScrollView.AddItem(index);
		t.Setup(chatInfo);
		this.Count = index + 1;
	}
	#endregion

	#region NGUIリフレクション
	public void OnClose()
	{
		this._SetActive(false);
	}
	[ContextMenu("Reposition")]
	public void OnReposition()
	{
		this.ItemScrollView.Reposition();
		this.ScrollEnd();
	}
	public void SetPosition()
	{
		Transform t = this.Attach.dragObject.target.transform;
		ConfigFile.System.LogPosX = t.localPosition.x;
		ConfigFile.System.LogPosY = t.localPosition.y;
	}
	public void SetSize()
	{
		UIWidget w = this.Attach.dragResize.target;
		ConfigFile.System.LogWidth = w.width;
		ConfigFile.System.LogHeight = w.height;
	}
	#endregion

	#region スクロールバー操作
	void ScrollEnd()
	{
        float k = 1.0f - (0.1f * Attach.scrollBar.barSize);
        if (this.ItemScrollView.VScrollValue <= k)
        {
            UIScrollView sv = this.ItemScrollView.Attach.scrollView;
//          sv.UpdatePosition();
            sv.UpdateScrollbars();
            return;
        }

		this.ItemScrollView.SetScroll(0, 1f);
	}
	#endregion
}



/// <summary>
/// チャットログ
/// </summary>
[System.Serializable]
public class ChatLog
{
	#region フィールド&プロパティ
	/// <summary>
	/// ログの保存最大数
	/// </summary>
	[SerializeField] int _logMax = 100;
	public int LogMax { get { return _logMax; } }

	/// <summary>
	/// アイテムリスト
	/// </summary>
	[SerializeField] LinkedList<ChatInfo> _itemList = new LinkedList<ChatInfo>();
	LinkedList<ChatInfo> ItemList { get { return _itemList; } }

	public int Count { get { return ItemList.Count; } }
	#endregion

	#region 外部からの操作
	/// <summary>
	/// クローン
	/// </summary>
	public ChatLog Clone()
	{
		var t = (ChatLog)MemberwiseClone();
		if (this.ItemList != null)
			t._itemList = new LinkedList<ChatInfo>(this.ItemList);
		return t;
	}
	/// <summary>
	/// 要素追加
	/// </summary>
	public void Add(ChatInfo chatInfo)
	{
		if (chatInfo == null)
			return;

		// ログの保存数が超えていたら古いのから削除する
		if (this.ItemList.Count >= this.LogMax)
			this.ItemList.RemoveFirst();

		// 後ろの追加する
		this.ItemList.AddLast(chatInfo);
	}
	/// <summary>
	/// コレクションの全要素に対して指定した処理を実行する
	/// </summary>
	public void ForEach(System.Action<ChatInfo, int> action)
	{
		int i = 0;
		foreach (var t in this.ItemList)
		{
			action(t, i);
			i++;
		}
	}
	#endregion
}
