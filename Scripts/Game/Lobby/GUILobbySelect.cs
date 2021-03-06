/// <summary>
/// ロビー選択
/// 
/// 2014/05/29
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Packet;

public class GUILobbySelect : Singleton<GUILobbySelect>
{
	#region フィールド＆プロパティ
	// 更新ボタンの無効時間
	static readonly float UpdateButtonDisableTime = 1f;

	/// <summary>
	/// 初期化時のアクティブ状態
	/// </summary>
	[SerializeField]
	bool _isStartActive = false;
	bool IsStartActive { get { return _isStartActive; } }

	/// <summary>
	/// ページ付きスクロールビュー
	/// </summary>
	[SerializeField]
	GUIItemScrollView _itemScrollView;
	GUIItemScrollView ItemScrollView { get { return _itemScrollView; } }
	[System.Serializable]
	public class GUIItemScrollView : PageScrollView<GUILobbySelectItem>
	{
		protected override GUILobbySelectItem Create(GameObject prefab, Transform parent, int itemIndex)
		{
			return GUILobbySelectItem.Create(prefab, parent, itemIndex);
		}
		protected override void ClearValue(GUILobbySelectItem item)
		{
			item.ClearValue();
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
		// 基本セット
		public UIPlayTween rootTween;
		public GameObject homeButtonGroup;
		public GameObject closeButtonGroup;

		public UILabel nowContentLabel;
		public UILabel contentLabel;
		public XUIButton updateButton;
		public UIButton okButton;
		public PageScrollViewAttach pageScrollView;
	}

	// アクティブフラグ
	bool IsActive { get; set; }
	// ホームボタンを押した時のデリゲート
	System.Action OnHomeFunction { get; set; }
	// 閉じるボタンを押した時のデリゲート
	System.Action OnCloseFunction { get; set; }
	// OKボタンを押した時のデリゲート
	System.Action<int, LobbyInfo> OnOKFunction { get; set; }

	// 現在のロビーインデックス
	int CurrentLobbyIndex { get; set; }

	/// <summary>
	/// 最初に全体のロビーの数を確保しておく
	/// サーバーからのレスポンス時に中身を入れる
	/// </summary>
	List<LoadLobbyInfo> LobbyInfoList { get; set; }
	[System.Serializable]
	public class LoadLobbyInfo
	{
		[SerializeField]
		bool _isLoad = false;
		public bool IsLoad { get { return _isLoad; } set { _isLoad = value; } }

		[SerializeField]
		LobbyInfo _lobbyInfo = new LobbyInfo();
		public LobbyInfo LobbyInfo { get { return _lobbyInfo; } set { _lobbyInfo = value; } }
	}

	// パケット受信待ちフラグ
	bool IsLoading { get; set; }
	// 現在選択しているトータルインデックス
	int SelectIndexInTotal { get; set; }

	// シリアライズされていないメンバーの初期化
	void MemberInit()
	{
		this.IsActive = false;
		this.OnHomeFunction = delegate { };
		this.OnCloseFunction = delegate { };
		this.OnOKFunction = delegate { };

		this.CurrentLobbyIndex = -1;

		this.LobbyInfoList = new List<LoadLobbyInfo>();

		this.IsLoading = false;
		this.SelectIndexInTotal = -1;
	}

	// 選択されているロビー情報
	LobbyInfo SelectInfo
	{
		get
		{
			if (this.SelectIndexInTotal < 0)
				return null;	// 範囲外
			if (this.SelectIndexInTotal >= this.LobbyInfoList.Count)
				return null;	// 範囲外
			var info = this.LobbyInfoList[this.SelectIndexInTotal];
			if (!info.IsLoad)
				return null;	// 読み込み終わってない
			return info.LobbyInfo;
		}
	}

	// 選択されているアイテム
	GUILobbySelectItem SelectItem
	{
		get
		{
			int itemIndex = this.ItemScrollView.GetItemIndex(this.SelectIndexInTotal);
			var item = this.ItemScrollView.GetItem(itemIndex);
			return item;
		}
	}

	// アイテムを選択することが出来るかどうか
	bool CanSelectOK
	{
		get
		{
			// 選択している情報が有るかどうか
			var info = this.SelectInfo;
			if (info == null)
				return false;
			return true;
		}
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

		// ページ付きスクロールビューを初期化
		this.ItemScrollView.Create(this.Attach.pageScrollView, this.UpdateVisibleItem);
		// 表示設定
		SetActive(this.IsStartActive);
	}
	/// <summary>
	/// テーブル内をクリア
	/// </summary>
	void Clear()
	{
		this._SetSelectItem(null);
		this.ItemScrollView.Clear();
		this.LobbyInfoList.Clear();
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
		SetActive(isActive, true, true, null, null, null);
	}
	/// <summary>
	/// アクティブ化(詳細設定)
	/// </summary>
	public static void SetActive(bool isActive, bool isUseHome, bool isUseClose, System.Action onHome, System.Action onClose, System.Action<int, LobbyInfo> onOK)
	{
		if (Instance != null) Instance._SetActive(isActive, isUseHome, isUseClose, onHome, onClose, onOK);
	}
	/// <summary>
	/// アクティブ化(大元)
	/// </summary>
	void _SetActive(bool isActive, bool isUseHome, bool isUseClose, System.Action onHome, System.Action onClose, System.Action<int, LobbyInfo> onOK)
	{
		this.OnHomeFunction = (onHome != null ? onHome : delegate { });
		this.OnCloseFunction = (onClose != null ? onClose : delegate { });
		this.OnOKFunction = (onOK != null ? onOK : delegate { });

		// UI設定
		{
			var t = this.Attach;
			if (t.homeButtonGroup != null)
				t.homeButtonGroup.SetActive(isUseHome);
			if (t.closeButtonGroup != null)
				t.closeButtonGroup.SetActive(isUseClose);

			// 更新ボタン有効設定
			if (t.updateButton != null)
				t.updateButton.isEnabled = isActive;
		}

		// ウィンドウアクティブ設定
		this.SetWindowActive(isActive);

		if (isActive)
		{
			// 設定クリア
			this.Clear();
			// ロビーの総数を取得する
			LobbyPacket.SendLobbyNum();
		}
	}
	/// <summary>
	/// ウィンドウアクティブ設定
	/// </summary>
	void SetWindowActive(bool isActive)
	{
		this.IsActive = isActive;

		// アクティブ化
		if (this.Attach.rootTween != null)
			this.Attach.rootTween.Play(isActive);
		else
			this.gameObject.SetActive(isActive);

		// その他UIの表示設定
		GUILobbyResident.SetActive(!isActive);
		GUIScreenTitle.Play(isActive, MasterData.GetText(TextType.TX141_LobbySelect_ScreenTitle));
		GUIHelpMessage.Play(isActive, MasterData.GetText(TextType.TX142_LobbySelect_HelpMessage));
	}
	#endregion

	#region 更新ボタン無効タイマー
	IEnumerator UpdateButtonDisableTimer()
	{
		if (!this.Attach.updateButton.isEnabled)
			yield break;

		float disableTimer = GUILobbySelect.UpdateButtonDisableTime;

		this.Attach.updateButton.isEnabled = false;

		while (0f < disableTimer)
		{
			disableTimer -= Time.deltaTime;
			yield return null;
		}
		this.Attach.updateButton.isEnabled = true;
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
		// 選択できるかどうか
		if (!this.CanSelectOK)
			return;

		var info = this.SelectInfo;
		if (info == null)
			return;

		Close();

		if (!GUILobbyResident.CanLobbySelect)
		{
			// ロビー移動できない状態
			GUIChat.AddSystemMessage(true, MasterData.GetText(TextType.TX116_EnterLobbyRes_InMatching));
			return;
		}

		int currentLobbyID = this.CurrentLobbyIndex + 1;
		this.OnOKFunction(currentLobbyID, info);
	}
	/// <summary>
	/// 次へボタンを押した時
	/// </summary>
	public void OnNext()
	{
		// ページを一つ次に進める
		var isPageChange = this.ItemScrollView.SetNextPage(1);
		if (isPageChange)
		{
			// 選択しているアイテムが同じページならそこにカーソルを合わせる
			if (this.ItemScrollView.IsNowPage(this.SelectIndexInTotal))
				this.ItemScrollView.CenterOn(this.ItemScrollView.GetItemIndex(this.SelectIndexInTotal));
			// ページが切り替わったのでアイテムの更新を行う
			this.UpdateItem(this.SelectIndexInTotal);
		}
	}
	/// <summary>
	/// 前へボタンを押した時
	/// </summary>
	public void OnBack()
	{
		// ページを一つ前に戻す
		var isPageChange = this.ItemScrollView.SetNextPage(-1);
		if (isPageChange)
		{
			// 選択しているアイテムが同じページならそこにカーソルを合わせる
			if (this.ItemScrollView.IsNowPage(this.SelectIndexInTotal))
				this.ItemScrollView.CenterOn(this.ItemScrollView.GetItemIndex(this.SelectIndexInTotal));
			// ページが切り替わったのでアイテムの更新を行う
			this.UpdateItem(this.SelectIndexInTotal);
		}
	}
	/// <summary>
	/// 更新ボタンを押した時
	/// </summary>
	public void OnUpdate()
	{
		// 読み込みフラグを初期化する
		foreach (var t in this.LobbyInfoList)
			t.IsLoad = false;

		// アイテム更新
		var selectIndexInTotal = this.SelectIndexInTotal;
		// 一旦選択状態を外して表示情報を消す
		// 選択されているアイテムがページ外の時に情報が更新されないのでその対応
		this._SetSelectItem(null);
		this.SelectIndexInTotal = selectIndexInTotal;
		// ページ内のすべてのアイテムを読込中にする
		this.UpdateItem(this.SelectIndexInTotal);
		// 現在表示されているアイテムの情報を更新する
		this.UpdateVisibleItem();

		// 更新ボタン一定時間無効化
		StartCoroutine(this.UpdateButtonDisableTimer());
	}
	/// <summary>
	/// 現在のロビーボタンを押した時
	/// </summary>
	public void OnNowLobby()
	{
		// インデックスが有効な値なら現在位置にカーソルを合わせる
		if (this.ItemScrollView.IsValid(this.CurrentLobbyIndex))
		{
			var isPageChange = this.ItemScrollView.CenterOnInTotal(this.CurrentLobbyIndex);
			if (isPageChange)
			{
				// ページが切り替わったのでアイテムの更新を行う
				this.UpdateItem(this.SelectIndexInTotal);
			}
		}
	}
	/// <summary>
	/// 現在位置ボタンを押した時
	/// </summary>
	public void OnCurrent()
	{
		// インデックスが有効な値なら現在位置にカーソルを合わせる
		if (this.ItemScrollView.IsValid(this.SelectIndexInTotal))
		{
			var isPageChange = this.ItemScrollView.CenterOnInTotal(this.SelectIndexInTotal);
			if (isPageChange)
			{
				// ページが切り替わったのでアイテムの更新を行う
				this.UpdateItem(this.SelectIndexInTotal);
			}
		}
	}
	/// <summary>
	/// スクロールトップ
	/// </summary>
	public void OnTop()
	{
		this.ItemScrollView.CenterOn(0);
	}
	/// <summary>
	/// スクロールボトム
	/// </summary>
	public void OnBottom()
	{
		this.ItemScrollView.CenterOn(this.ItemScrollView.NowPageItemMax - 1);
	}
	/// <summary>
	/// テーブル再配置
	/// </summary>
	public void OnReposition()
	{
		this.ItemScrollView.Reposition();
	}
	#endregion

	#region セットアップ
	/// <summary>
	/// アイテムの総数を設定する
	/// </summary>
	public static void Setup(int total, int currentLobbyIndex)
	{
		if (Instance != null) Instance._Setup(total, currentLobbyIndex);
	}
	/// <summary>
	/// アイテムの総数を設定する
	/// </summary>
	void _Setup(int total, int currentLobbyIndex)
	{
		this.CurrentLobbyIndex = currentLobbyIndex;

		// ロビーリストの数を調整する
		if (total > this.LobbyInfoList.Count)
		{
			// リストの数が足りないので拡張する
			for (int i = this.LobbyInfoList.Count; i < total; i++)
				this.LobbyInfoList.Add(new LoadLobbyInfo());
		}
		else if (total < this.LobbyInfoList.Count)
		{
			// リストの数が多いので切り捨てる
			this.LobbyInfoList = this.LobbyInfoList.GetRange(0, total);
		}

		// ページスクロールビューのセットアップ
		this.ItemScrollView.Setup(total, this.CurrentLobbyIndex);

		// アイテム更新
		this.UpdateItem(this.CurrentLobbyIndex);

		// UI設定
		var t = this.Attach;
		if (t.nowContentLabel != null)
			t.nowContentLabel.text = string.Format(ObsolateSrc.GUILobbySelectComment.NowContentFormat, this.CurrentLobbyIndex + 1);
	}
	#endregion

	#region 個々のアイテム設定
	/// <summary>
	/// 個々のアイテムを設定する
	/// </summary>
	public static void SetupItem(List<LobbyInfo> list)
	{
		if (Instance != null) Instance._SetupItem(list);
	}
	/// <summary>
	/// 個々のアイテムを設定する
	/// </summary>
	void _SetupItem(List<LobbyInfo> list)
	{
		// ロビー情報を更新する
		foreach (var t in list)
		{
			int indexInTotal = t.LobbyID - 1;
			if (indexInTotal < 0)
				continue;	// インデックスがマイナスになるのはおかしい

			if (indexInTotal < this.LobbyInfoList.Count)
			{
				// ロビー情報を読み込み済みにして情報を更新する
				var info = this.LobbyInfoList[indexInTotal];
				info.IsLoad = true;
				info.LobbyInfo = t;
			}
			else
			{
				// リスト以上のインデックスが来たらリストを拡張する
				var info = new LoadLobbyInfo();
				info.IsLoad = true;
				info.LobbyInfo = t;
				this.LobbyInfoList.Add(info);
			}
		}

		// ロード終了
		this.IsLoading = false;
		// 現在表示されているアイテムで読み込まれていないのがあった場合はまた送信する
		this.UpdateVisibleItem();
	}
	/// <summary>
	/// ロビー情報リストを参照して現在のページのアイテムを全て更新する
	/// </summary>
	void UpdateItem(int selectIndexInTotal)
	{
		int startIndex = this.ItemScrollView.NowPageStartIndex;
		for (int itemIndex = 0, max = this.ItemScrollView.NowPageItemMax; itemIndex < max; itemIndex++)
		{
			// アイテムを取得する
			var item = this.ItemScrollView.GetItem(itemIndex);
			if (item == null)
				continue;

			// 念の為現在のページのアイテムかどうか
			int indexInTotal = startIndex + itemIndex;
			if (!this.ItemScrollView.IsNowPage(indexInTotal))
				continue;	// 現在のページじゃないのはおかしい

			// アイテムの中身を更新
			if (indexInTotal < 0 || indexInTotal >= this.LobbyInfoList.Count)
				continue;	// 範囲外はおかしい
			var info = this.LobbyInfoList[indexInTotal];
			item.Setup(info.IsLoad, indexInTotal, info.LobbyInfo);

			// 選択されているアイテムが有る場合は更新する
			if (selectIndexInTotal == indexInTotal)
			{
				this._SetSelectItem(item);
			}
		}
	}
	/// <summary>
	/// 表示されているアイテムの更新をする
	/// 中身が空ならサーバーに見えている分の情報を問い合わせる
	/// </summary>
	void UpdateVisibleItem()
	{
		if (this.IsLoading)
			return;

		int startID = 0;
		int count = 0;

		// リストの先頭から現在表示されているアイテムをチェックして
		// 読み込み個数を取得する
		int startIndex = this.ItemScrollView.NowPageStartIndex;
		int visibleStartItemIndex = -1;
		for (int itemIndex = 0, max = this.ItemScrollView.NowPageItemMax; itemIndex < max; itemIndex++)
		{
			// アイテムを取得する
			var item = this.ItemScrollView.GetItem(itemIndex);
			if (item == null)
				continue;

			// 表示範囲外かどうか
			if (!this.ItemScrollView.IsVisible(item))
			{
				// visibleStartItemIndex が設定されている場合はそれ以降のアイテムは範囲外なので抜ける
				if (visibleStartItemIndex != -1)
					break;
				else
					continue;
			}

			// 表示されているアイテムの開始インデックス設定
			if (visibleStartItemIndex == -1)
				visibleStartItemIndex = itemIndex;

			// 表示範囲内なのでこのアイテムのロビー情報を取得する
			int indexInTotal = startIndex + itemIndex;
			if (indexInTotal < 0 || indexInTotal >= this.LobbyInfoList.Count)
				continue;	// 範囲外はおかしい
			var info = this.LobbyInfoList[indexInTotal];
			// アイテムを更新する
			item.Setup(info.IsLoad, indexInTotal, info.LobbyInfo);
			// 選択しているアイテム選択情報を更新する
			if (this.SelectIndexInTotal == indexInTotal)
				this._SetSelectItem(item);

			if (startID == 0 && info.IsLoad)
				continue;	// 既にロビー情報があるので新たに読み込まない

			// 表示されているアイテムで初めて情報がないアイテムIDを控えておく
			// そのアイテムID以降はサーバーに問い合わせる
			if (startID == 0)
				startID = indexInTotal + 1;
			count++;
		}
		if (count <= 0)
			return;

		// サーバーへ問い合わせる
		this.IsLoading = true;
		LobbyPacket.SendLobbyList(startID, count);
	}
	#endregion

	#region 選択したアイテム設定
	/// <summary>
	/// 選択したアイテムを設定する
	/// </summary>
	public static void SetSelectItem(GUILobbySelectItem item)
	{
		if (Instance != null) Instance._SetSelectItem(item);
	}
	/// <summary>
	/// 選択したアイテムを設定する
	/// </summary>
	void _SetSelectItem(GUILobbySelectItem item)
	{
		// アイテム枠の更新
		// 同じアイテムを選択したとしても必ず行うようにする
		// ページ切り替えでアイテムが更新される場合があるため
		this.UpdateSelectItemFrame(item, this.SelectItem);

		// 選択しているインデックスの更新
		bool isBeforeSame = false;
		{
			var indexInTotal = (item == null ? -1 : item.IndexInTotal);
			if (this.SelectIndexInTotal == indexInTotal)
				isBeforeSame = true;
			this.SelectIndexInTotal = indexInTotal;
		}

		// アイテム情報を更新
		this.UpdateSelectItemInfo(item);

		// 前回と同じアイテムならこれ以降の更新は行わない
		if (isBeforeSame)
			return;
	}
	/// <summary>
	/// 選択枠の更新
	/// </summary>
	void UpdateSelectItemFrame(GUILobbySelectItem selectItem, GUILobbySelectItem beforeItem)
	{
		// 選択したアイテムが以前と違うアイテムなら
		// 以前のアイテムの選択枠と非表示にする
		if (beforeItem != null)
		{
			if (beforeItem != selectItem)
			{
				beforeItem.SetSelectSpriteActive(false);
			}
		}

		// 選択枠の表示
		if (selectItem != null)
		{
			selectItem.SetSelectSpriteActive(true);
		}
	}
	/// <summary>
	/// 選択したアイテムの情報を更新する
	/// </summary>
	void UpdateSelectItemInfo(GUILobbySelectItem selectItem)
	{
		// OKボタンが押せるかどうか
		if (this.Attach.okButton != null)
			this.Attach.okButton.isEnabled = this.CanSelectOK;

		string content = "";
		if (selectItem != null)
		{
			var info = selectItem.LobbyInfo;
			if (selectItem.IsLoad)
				content = string.Format(ObsolateSrc.GUILobbySelectComment.ContentFormat, info.LobbyID, info.Num, info.Capacity);
		}

		var t = this.Attach;
		if (t.contentLabel != null)
			t.contentLabel.text = content;
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
		public bool isActive;
		public bool isActiveHomeButton;
		public bool isActiveCloseButton;
		public bool executeItem;
		public int itemCurrentLobbyIndex;
		public List<LobbyInfo> itemList;
		public bool executeCreateItem;
		public int createLobbyMax;
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
			this._SetActive(t.isActive, t.isActiveHomeButton, t.isActiveCloseButton,
				() => { Debug.Log("OnHome"); },
				() => { Debug.Log("OnClose"); },
				(currentLobbyID, lobbyInfo) =>
				{
					Debug.Log(string.Format("OnOK(currentLobbyID:{0} selectLobbyID:{1} {2}/{3})", currentLobbyID, lobbyInfo.LobbyID, lobbyInfo.Num, lobbyInfo.Capacity));
				});
		}
		if (t.executeItem)
		{
			t.executeItem = false;

			this._Setup(t.itemList.Count, t.itemCurrentLobbyIndex);
			this._SetupItem(t.itemList);
			this.UpdateItem(t.itemCurrentLobbyIndex);
		}
		if (t.executeCreateItem)
		{
			t.executeCreateItem = false;

			t.itemList.Clear();
			for (int i = 0, max = t.createLobbyMax; i < max; i++)
			{
				var capacity = Random.Range(0, 999);
				var info = new LobbyInfo(
					i + 1,
					capacity,
					Random.Range(0, capacity)
					);
				t.itemList.Add(info);
			}
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
