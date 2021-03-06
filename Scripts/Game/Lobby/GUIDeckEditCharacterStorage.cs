/// <summary>
/// キャラクターストレージ
/// 
/// 2014/06/03
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIDeckEditCharacterStorage : Singleton<GUIDeckEditCharacterStorage>
{
	#region フィールド＆プロパティ
	/// <summary>
	/// 所持数表示フォーマット
	/// </summary>
	[SerializeField]
	string _storageFormat = "{0} / {1}";
	string StorageFormat { get { return _storageFormat; } }

	/// <summary>
	/// コスト表示フォーマット
	/// </summary>
	[SerializeField]
	string _costFormat = "{0}";
	string CostFormat { get { return _costFormat; } }

	/// <summary>
	/// リビルドタイム表示フォーマット
	/// </summary>
	[SerializeField]
	string _rebuildTimeFormat = "{0}";
	string RebuildTimeFormat { get { return _rebuildTimeFormat; } }

	/// <summary>
	/// ページ付きスクロールビュー
	/// </summary>
	[SerializeField]
	GUIItemScrollView _itemScrollView;
	GUIItemScrollView ItemScrollView { get { return _itemScrollView; } }
	[System.Serializable]
	public class GUIItemScrollView : PageScrollView<GUICharacterStorageItem>
	{
		protected override GUICharacterStorageItem Create(GameObject prefab, Transform parent, int itemIndex)
		{
			return GUICharacterStorageItem.Create(prefab, parent, itemIndex);
		}
		protected override void ClearValue(GUICharacterStorageItem item)
		{
			item.ClearValue();
		}
	}

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach = null;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		[SerializeField]
		UIPlayTween _rootTween = null;
		public UIPlayTween RootTween { get { return _rootTween; } }
		[SerializeField]
		GameObject _homeButtonGroup = null;
		public GameObject HomeButtonGroup { get { return _homeButtonGroup; } }
		[SerializeField]
		GameObject _closeButtonGroup = null;
		public GameObject CloseButtonGroup { get { return _closeButtonGroup; } }

		[SerializeField]
		UIPlayTween _boardTweenA = null;
		public UIPlayTween BoardTweenA { get { return _boardTweenA; } }
		[SerializeField]
		UIPlayTween _boardTweenB = null;
		public UIPlayTween BoardTweenB { get { return _boardTweenB; } }

		[SerializeField]
		Transform _boardRootA = null;
		public Transform BoardRootA { get { return _boardRootA; } }
		[SerializeField]
		Transform _boardRootB = null;
		public Transform BoardRootB { get { return _boardRootB; } }

		[SerializeField]
		UILabel _storageLabel = null;
		public UILabel StorageLabel { get { return _storageLabel; } }
		[SerializeField]
		UIButton _okButton = null;
		public UIButton OkButton { get { return _okButton; } }

		[SerializeField]
		Infomation _info = null;
		public Infomation Info { get { return _info; } }
		[System.Serializable]
		public class Infomation
		{
			[SerializeField]
			UILabel _nameLabel = null;
			public UILabel NameLabel { get { return _nameLabel; } }
			[SerializeField]
			UILabel _aliasLabel = null;
			public UILabel AliasLabel { get { return _aliasLabel; } }
			[SerializeField]
			UILabel _costLabel = null;
			public UILabel CostLabel { get { return _costLabel; } }
			[SerializeField]
			UILabel _rebuildTimeLabel = null;
			public UILabel RebuildTimeLabel { get { return _rebuildTimeLabel; } }
		}

		[SerializeField]
		MasterTextSetterList _masterText = null;
		public MasterTextSetterList MasterText { get { return _masterText; } }

		public PageScrollViewAttach pageScrollView = null;
	}

	// 現在のモード
	OpenMode Mode { get; set; }
	public enum OpenMode
	{
		None,
		LobbyChara,
		DeckEdit,
	}
	// ホームボタンを押した時のデリゲート
	System.Action OnHomeFunction { get; set; }
	// 閉じるボタンを押した時のデリゲート
	System.Action OnCloseFunction { get; set; }
	// OKボタンを押した時のデリゲート
	System.Action<ulong, CharaInfo> OnOKFunction { get; set; }
	// 変更対象キャラクターID
	ulong ChangeTargetOwnCharaID { get; set; }
	// 使用中のキャラクターIDリスト
	List<ulong> UsedOwnCharaIDList { get; set; }

	// キャラクター枠の数
	int Capacity { get; set; }

	// 所有しているキャラクターのアイテムリスト
	List<CharaInfo> CharaList { get; set; }
	// 現在選択しているトータルインデックス
	int SelectIndexInTotal { get; set; }

	// PlayTweenの切り替えフラグ(演出用)
	bool IsTweenSwitch { get; set; }

    //private bool HasCallServer = false;

	// シリアライズされていないメンバーの初期化
	void MemberInit()
	{
		this.Mode = OpenMode.None;
		this.OnHomeFunction = delegate { };
		this.OnCloseFunction = delegate { };
		this.OnOKFunction = delegate { };
		this.ChangeTargetOwnCharaID = 0;
		this.UsedOwnCharaIDList = null;

		this.Capacity = 0;

		this.CharaList = new List<CharaInfo>();
		this.SelectIndexInTotal = -1;
		this.IsTweenSwitch = false;
        //this.HasCallServer = false;
	}

	// キャラアイコン
	CharaIcon CharaIcon { get { return ScmParam.Lobby.CharaIcon; } }
	// キャラボード
	CharaBoard CharaBoard { get { return ScmParam.Lobby.CharaBoard; } }

	// 追加枠のインデックス
	int AddIndexInTotal
	{
		get
		{
			// 売り切り版 追加枠は表示しない
			//return ItemScrollView.Total - 1;
			return ItemScrollView.Total;
		}
	}

	// 選択されているキャラ情報
	CharaInfo SelectInfo
	{
		get
		{
			if (this.SelectIndexInTotal < 0)
				return null;	// 範囲外
			if (this.SelectIndexInTotal >= this.CharaList.Count)
				return null;	// 範囲外
			var info = this.CharaList[this.SelectIndexInTotal];
			return info;
		}
	}

	// 選択されているアイテム
	GUICharacterStorageItem SelectItem
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
			// 選択しているアイテムが有るかどうか
			var info = this.SelectInfo;
			if (info == null)
				return false;
			// 変更対象ではないキャラクター
			if (this.ChangeTargetOwnCharaID != info.UUID)
			{
				// 既に使用中のキャラかどうか
				if (this.UsedOwnCharaIDList != null)
				{
					if (this.UsedOwnCharaIDList.Contains(info.UUID))
						return false;
				}
			}
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

		// マスターデータからテキストをセットする
		// Itemの中にマスターデータから引っ張ってくるものがあるため
		// this.ItemScrollView.Create を実行してしまうとマスターデータから引っ張ってくる前に
		// Item を作ってしまうので先に明示的に更新をかける必要がある
		var t = this.Attach;
		if (t.MasterText)
			t.MasterText.UpdateText();

		// ページ付きスクロールビューを初期化
		this.ItemScrollView.Create(this.Attach.pageScrollView, null);
		// 最初は閉じておく
		Close();
	}
	/// <summary>
	/// テーブル内をクリア
	/// </summary>
	void Clear()
	{
		this._SetSelectItem(null);
		this.ItemScrollView.Clear();
	}
	#endregion

	#region モード設定
	/// <summary>
	/// ウィンドウを閉じる
	/// </summary>
	public static void Close()
	{
	    if (Instance != null)
	    {
	        Instance.SetWindowActive(OpenMode.None, false);
            //Instance.HasCallServer = false;
	    }
	}
	/// <summary>
	/// ロビーキャラクターモード
	/// </summary>
	public static void SetModeLobbyChara(System.Action onHome, System.Action onClose, System.Action<ulong, CharaInfo> onOK)
	{
		if (Instance != null) Instance._SetMode(OpenMode.LobbyChara, 0, null, true, true, onHome, onClose, onOK);
	}
	/// <summary>
	/// デッキ編集モード
	/// </summary>
	public static void SetModeDeckEdit(ulong changeBeforeOwnCharaID, List<ulong> usedOwnCharaIDList, System.Action onHome, System.Action onClose, System.Action<ulong, CharaInfo> onOK)
	{
	    if (Instance != null)
	    {
	        Instance._SetMode(OpenMode.DeckEdit, changeBeforeOwnCharaID, usedOwnCharaIDList, false, true, onHome, onClose, onOK);
	    }
	}
	/// <summary>
	/// モード設定(大元)
	/// </summary>
	void _SetMode(OpenMode mode, ulong changeTargetOwnCharaID, List<ulong> usedOwnCharaIDList, bool isUseHome, bool isUseClose, System.Action onHome, System.Action onClose, System.Action<ulong, CharaInfo> onOK)
	{
		this.OnHomeFunction = (onHome != null ? onHome : delegate { });
		this.OnCloseFunction = (onClose != null ? onClose : delegate { });
		this.OnOKFunction = (onOK != null ? onOK : delegate { });
		this.ChangeTargetOwnCharaID = changeTargetOwnCharaID;	// ロビーキャラクターの時は後で設定する
		this.UsedOwnCharaIDList = usedOwnCharaIDList;

		bool isActive = mode != OpenMode.None;

		// UI設定
		{
			var t = this.Attach;
			if (t.HomeButtonGroup != null)
				t.HomeButtonGroup.SetActive(isUseHome);
			if (t.CloseButtonGroup != null)
				t.CloseButtonGroup.SetActive(isUseClose);
		}

		if (isActive)
		{
            if (/*!HasCallServer*/true)
		    {
		        // 設定クリア
		        this.Clear();
		        // キャラクターの総数を取得する
		        LobbyPacket.SendPlayerCharacterBox(this.Response);
		        // ボード（立ち絵）が消されてからウィンドウをアクティブ化する
		        FiberController.AddFiber(this.WaitDeleteBoardWindowActiveCoroutine(mode));
		        //HasCallServer = true;
		    }
		    else
		    {
                _SetSelectItem(null);
		        RefreshCapacity();
		    }
		}
		else
		{
			// ウィンドウアクティブ設定
			this.SetWindowActive(mode, false);
		}
	}
	/// <summary>
	/// ボード（立ち絵）が消されてからウィンドウをアクティブ化する
	/// 
	/// ボードに TweenColor を入れてウィンドウの開閉と連動させた場合
	/// UIPlayTween.Update 内の mTweens の更新処理で null チェックをしていないため
	/// ボードを消してから UIPlayTween.Play をさせると mTweens の中に
	/// 消されたボードの TweenColor を参照して Missing でエラーになる
	/// 
	/// また、削除後に PlayTween.Play をしても、それが同フレームで行っていたら Unity(5.3.2f1現在) の仕様上
	/// その場ですぐ消えるわけではなく数フレーム遅れて消されるため解決にはならない
	/// 
	/// それを回避するためにコルーチンを使いボードが消されているかをチェックして
	/// 消されていたら PlayTween.Play をするようにした
	/// </summary>
	IEnumerator WaitDeleteBoardWindowActiveCoroutine(OpenMode mode)
	{
		// ボードが消されていなかったら待機
		var t = this.Attach;
		while (
			(t.BoardRootA != null && t.BoardRootA.childCount >= 1) ||
			(t.BoardRootB != null && t.BoardRootB.childCount >= 1)
			)
		{
			yield return null;
		}

		this.SetWindowActive(mode, true);
	}
	/// <summary>
	/// ウィンドウアクティブ設定
	/// </summary>
	void SetWindowActive(OpenMode mode, bool isActive)
	{
		this.Mode = mode;

		// アクティブ化
		if (this.Attach.RootTween != null)
			this.Attach.RootTween.Play(isActive);
		else
			this.gameObject.SetActive(isActive);

		// その他UIの表示設定
		GUILobbyResident.SetActive(!isActive);
		string title = "";
		string help = "";
		switch (mode)
		{
		default:
		case OpenMode.None:
			break;
		case OpenMode.LobbyChara:
			title = MasterData.GetText(TextType.TX051_CS_LobbyCharaTitle);
			help = MasterData.GetText(TextType.TX052_CS_LobbyCharaHelp);
			break;
		case OpenMode.DeckEdit:
			title = MasterData.GetText(TextType.TX053_CS_DeckEditTitle);
			help = MasterData.GetText(TextType.TX054_CS_DeckEditHelp);
			break;
		}
		GUIScreenTitle.Play(isActive, title);
		GUIHelpMessage.Play(isActive, help);
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
	/// OK ボタンを押した時
	/// </summary>
	public void OnOK()
	{
		// 選択できるかどうか
		if (!this.CanSelectOK)
			return;

		var info = this.SelectInfo;
		if (info == null)
			return;

//		Close();
		this.OnOKFunction(this.ChangeTargetOwnCharaID, info);
        //NewID
	    UsedOwnCharaIDList.Remove(this.ChangeTargetOwnCharaID);
	    this.ChangeTargetOwnCharaID = info.UUID;
        UsedOwnCharaIDList.Add(this.ChangeTargetOwnCharaID);
	}
	/// <summary>
	/// 次へボタンを押した時
	/// </summary>
	public void OnNext()
	{
		// ページを一つ次に進める
		var isPageChange = this.ItemScrollView.SetNextPage(1);
		if (isPageChange)
			this.ChangePage(this.SelectIndexInTotal);
	}
	/// <summary>
	/// 最後のページボタンを押した時
	/// </summary>
	public void OnNextEnd()
	{
		// 最後のページにする
		var isPageChange = this.ItemScrollView.SetPage(this.ItemScrollView.PageMax-1, 0);
		if (isPageChange)
			this.ChangePage(this.SelectIndexInTotal);
	}
	/// <summary>
	/// 前へボタンを押した時
	/// </summary>
	public void OnBack()
	{
		// ページを一つ前に戻す
		var isPageChange = this.ItemScrollView.SetNextPage(-1);
		if (isPageChange)
			this.ChangePage(this.SelectIndexInTotal);
	}
	/// <summary>
	/// 最初のページボタンを押した時
	/// </summary>
	public void OnBackEnd()
	{
		// 最初のページにする
		var isPageChange = this.ItemScrollView.SetPage(0, 0);
		if (isPageChange)
			this.ChangePage(this.SelectIndexInTotal);
	}
	/// <summary>
	/// 現在位置ボタンを押した時
	/// </summary>
	[ContextMenu("Current")]
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
	[ContextMenu("Top")]
	public void OnTop()
	{
		this.ItemScrollView.CenterOn(0);
	}
	/// <summary>
	/// スクロールボトム
	/// </summary>
	[ContextMenu("Bottom")]
	public void OnBottom()
	{
		this.ItemScrollView.CenterOn(this.ItemScrollView.NowPageItemMax - 1);
	}
	/// <summary>
	/// テーブル再配置
	/// </summary>
	[ContextMenu("Reposition")]
	public void OnReposition()
	{
		this.ItemScrollView.Reposition();
	}
	/// <summary>
	/// ページが切り替わった時の処理
	/// </summary>
	void ChangePage(int selectIndexInTotal)
	{
		// 選択しているアイテムが同じページならそこにカーソルを合わせる
		if (this.ItemScrollView.IsNowPage(selectIndexInTotal))
			this.ItemScrollView.CenterOn(this.ItemScrollView.GetItemIndex(selectIndexInTotal));
		// ページが切り替わったのでアイテムの更新を行う
		this.UpdateItem(selectIndexInTotal);
	}
	#endregion

	#region アイテムの総数設定
	void Response(LobbyPacket.PlayerCharacterBoxResArgs args)
	{
		this._SetupCapacity(args.Capacity, args.Count, true);
	}
	/// <summary>
	/// アイテムの総数を設定する
	/// </summary>
	public static void SetupCapacity(int capacity, int itemCount, bool isSendPacket)
	{
		if (Instance != null) Instance._SetupCapacity(capacity, itemCount, isSendPacket);
	}

    public static void RefreshCapacity()
    {
        if (Instance != null) Instance._SetupItem(Instance.CharaList, Instance.ChangeTargetOwnCharaID);
    }
	/// <summary>
	/// アイテムの総数を設定する
	/// </summary>
	void _SetupCapacity(int capacity, int itemCount, bool isSendPacket)
	{
		// 売り切り版 キャラ追加分数ボックス枠を表示
		//this.Capacity = capacity;
		this.Capacity = itemCount;

		// ページスクロールビューのセットアップ
		//this.ItemScrollView.Setup(capacity + 1, this.SelectIndexInTotal);
		this.ItemScrollView.Setup(itemCount, this.SelectIndexInTotal);

		// UI更新
		{
			var t = this.Attach;
			if (t.StorageLabel != null)
				t.StorageLabel.text = string.Format(this.StorageFormat, 0, capacity);
		}

		if (isSendPacket)
		{
			// 所有キャラクター情報を取得する
			LobbyPacket.SendPlayerCharacterAll(this.Response);
		}
	}
	#endregion

	#region 個々のアイテム設定
	void Response(LobbyPacket.PlayerCharacterAllResArgs args)
	{
		this._SetupItem(args.List, args.SymbolPlayerCharacterUUID);
	}
	/// <summary>
	/// 個々のアイテムを設定する
	/// </summary>
	public static void SetupItem(List<CharaInfo> list, ulong mainOwnCharaID)
	{
		if (Instance != null) Instance._SetupItem(list, mainOwnCharaID);
	}
	/// <summary>
	/// 個々のアイテムを設定する
	/// </summary>
	void _SetupItem(List<CharaInfo> list, ulong mainOwnCharaID)
	{
		this.CharaList = (list != null ? list : new List<CharaInfo>());

		// ロビーキャラクターモードならメインキャラクター情報をここで設定する
		if (this.Mode == OpenMode.LobbyChara)
		{
			this.ChangeTargetOwnCharaID = mainOwnCharaID;
			this.UsedOwnCharaIDList = new List<ulong>();
			this.UsedOwnCharaIDList.Add(mainOwnCharaID);
		}

		// UI更新
		{
			var t = this.Attach;
			if (t.StorageLabel != null)
				t.StorageLabel.text = string.Format(this.StorageFormat, this.CharaList.Count, this.Capacity);
		}

		// 変更対象のリストのインデックスを取得する
		int selectIndexInTotal = -1;
		for (int i = 0; i < this.CharaList.Count; i++)
		{
			var info = this.CharaList[i];
			if (this.ChangeTargetOwnCharaID != info.UUID)
				continue;
			selectIndexInTotal = i;
			break;
		}

		// ページスクロールビュー内での移動を行う
		this.ItemScrollView.CenterOnInTotal(selectIndexInTotal);
		// アイテム更新
		this.UpdateItem(selectIndexInTotal);
	}
	/// <summary>
	/// アイテムを更新する
	/// </summary>
	void UpdateItem(int selectIndexInTotal)
	{
		int startIndex = this.ItemScrollView.NowPageStartIndex;
		for (int i = 0, max = this.ItemScrollView.NowPageItemMax; i < max; i++)
		{
			// 現在のページのアイテムかどうか
			int indexInTotal = i + startIndex;
			if (!this.ItemScrollView.IsNowPage(indexInTotal))
				continue;
			// 現在のページのアイテムなのでアイテムを取得する
			int itemIndex = this.ItemScrollView.GetItemIndex(indexInTotal);
			var item = this.ItemScrollView.GetItem(itemIndex);
			if (item == null)
				continue;

			// キャラクター所持リスト
			if (this.AddIndexInTotal <= indexInTotal)
			{
				// キャラクター追加枠
				item.Setup(GUICharacterStorageItem.ItemType.Add, indexInTotal);
			}
			else if (this.CharaList.Count <= indexInTotal)
			{
				// キャラクターがない枠
				item.Setup(GUICharacterStorageItem.ItemType.Empty, indexInTotal);
			}
			else
			{
				// キャラクターが存在する枠
				var info = this.CharaList[indexInTotal];
				// 使用中かどうか
				var isUsed = false;
				if (this.UsedOwnCharaIDList != null)
					isUsed = this.UsedOwnCharaIDList.Contains(info.UUID);
				item.Setup(GUICharacterStorageItem.ItemType.Icon, indexInTotal, isUsed, info, this.CharaIcon);
			}

			// 選択されているアイテムがある場合は更新する
			if (selectIndexInTotal == indexInTotal)
			{
				this._SetSelectItem(item);
			}
		}
	}
	#endregion

	#region 選択したアイテム設定
	/// <summary>
	/// 選択したアイテムを設定する
	/// </summary>
	public static void SetSelectItem(GUICharacterStorageItem item)
	{
	    if (Instance != null)
	    {
	        if (item.IsUsed)
	        {
	            return;
	        }
	        if (Instance._SetSelectItem(item))
	        {
                Instance.OnOK();
	        }
	    }
	}
	/// <summary>
	/// 選択したアイテムを設定する
	/// </summary>
	public bool _SetSelectItem(GUICharacterStorageItem item)
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
			return false;

		// 立ち絵更新
		this.UpdateSelectItemBoard(item);
	    return true;
	}
	/// <summary>
	/// 選択枠の更新
	/// </summary>
	void UpdateSelectItemFrame(GUICharacterStorageItem selectItem, GUICharacterStorageItem beforeItem)
	{
		// 選択したアイテムが以前と違うアイテムなら
		// 以前のアイテムの選択枠と非表示にする
		if (beforeItem != null)
		{
			if (beforeItem != selectItem)
			{
//				beforeItem.SetSelectSpriteActive(false);
                beforeItem.SetUse(false);
			}
		}

		// 選択枠の表示
		if (selectItem != null)
		{
//			selectItem.SetSelectSpriteActive(true);
            selectItem.SetUse(true);
		}
	}
	/// <summary>
	/// 選択したアイテムの情報を更新する
	/// </summary>
	void UpdateSelectItemInfo(GUICharacterStorageItem selectItem)
	{
		// OKボタンが押せるかどうか
		if (this.Attach.OkButton != null)
			this.Attach.OkButton.isEnabled = this.CanSelectOK;

		var t = this.Attach.Info;
		if (selectItem != null)
		{
			// UNDONE:キャラのタイプ情報は仮
			string typeDescription = "";
			{
				Scm.Common.Master.CharaMasterData data;
				if (MasterData.TryGetChara((int)selectItem.CharaInfo.AvatarType, out data))
					typeDescription = data.Description;
			}
			if (t.NameLabel != null) t.NameLabel.text = selectItem.CharaInfo.Name;
			if (t.AliasLabel != null) t.AliasLabel.text = typeDescription;
			if (t.CostLabel != null) t.CostLabel.text = string.Format(this.CostFormat, selectItem.CharaInfo.DeckCost);
			if (t.RebuildTimeLabel != null) t.RebuildTimeLabel.text = string.Format(this.RebuildTimeFormat, selectItem.CharaInfo.RebuildTime);
		}
		else
		{
			if (t.NameLabel != null) t.NameLabel.text = "";
			if (t.AliasLabel != null) t.AliasLabel.text = "";
			if (t.CostLabel != null) t.CostLabel.text = "";
			if (t.RebuildTimeLabel != null) t.RebuildTimeLabel.text = "";
		}
	}
	/// <summary>
	/// 選択したアイテムのキャラボードを更新する
	/// </summary>
	void UpdateSelectItemBoard(GUICharacterStorageItem selectItem)
	{
	    this.Attach.BoardRootA.gameObject.SetActive(false);
	    this.Attach.BoardRootB.gameObject.SetActive(false);
        return;
		// 切り替え
		var tweenOn = (this.IsTweenSwitch ? this.Attach.BoardTweenA : this.Attach.BoardTweenB);
		var transOn = (this.IsTweenSwitch ? this.Attach.BoardRootA : this.Attach.BoardRootB);
		var tweenOff = (!this.IsTweenSwitch ? this.Attach.BoardTweenA : this.Attach.BoardTweenB);
		var transOff = (!this.IsTweenSwitch ? this.Attach.BoardRootA : this.Attach.BoardRootB);

		// 立ち絵読み込み
		if (selectItem != null)
			this.CharaBoard.GetBoard(selectItem.CharaInfo.AvatarType, selectItem.CharaInfo.SkinId, true, (GameObject resource) => { this.CreateBoard(selectItem.CharaInfo.AvatarType, resource, transOn, tweenOn); });

		// 削除する
		for (int i = 0, max = transOff.childCount; i < max; i++)
		{
			var child = transOff.GetChild(i);
			Object.Destroy(child.gameObject);
		}

		// オフにする時はすぐに再生させる
		if (tweenOff != null) tweenOff.Play(false);

		// Tween切り替え
		this.IsTweenSwitch = !this.IsTweenSwitch;
	}
	/// <summary>
	/// キャラボードのインスタンス化
	/// </summary>
	void CreateBoard(AvatarType avatarType, GameObject resource, Transform parent, UIPlayTween playTween)
	{
		// リソース読み込み完了
		if (resource == null)
			return;
		// インスタンス化
		var go = SafeObject.Instantiate(resource) as GameObject;
		if (go == null)
			return;

		// 読み込み中に別のキャラに変更していたら破棄する
		var info = this.SelectInfo;
		if (info != null && info.AvatarType != avatarType)
		{
			Object.Destroy(go);
			return;
		}

		// 名前設定
		go.name = resource.name;
		// 親子付け
		var t = go.transform;
		t.parent = parent;
		t.localPosition = Vector3.zero;
		t.localRotation = Quaternion.identity;
		t.localScale = Vector3.one;

		// 読み込みが完了してから再生を開始する
		if (playTween != null) playTween.Play(true);
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
		public bool setDummy;		// ダミーデータ設定
		public bool ececuteClose;	// 閉じる実行
		public bool executeMode;	// モード選択実行
		public Mode mode;
		[System.Serializable]
		public class Mode
		{
			public OpenMode mode;
			public bool isUseHome;
			public bool isUseClose;
			public ulong changeTargetOwnCharaID;
			public List<ulong> usedOwnCharaIDList;
		}
		public bool executeItem;	// アイテム設定実行
		public Item item;
		[System.Serializable]
		public class Item
		{
			public int capacity;
			public List<CharaInfo> itemList = new List<CharaInfo>();

			public bool executeCreateItemList;	// リストをランダムで生成するのを実行
			public CreateItem createItemList;
			[System.Serializable]
			public class CreateItem
			{
				public int num;
				public int listMaxCharaID;
			}
		}

		public bool IsReadMasterData { get; set; }
	}
	void DebugUpdate()
	{
		var d = this.DebugParam;
		if (d.ececuteClose)
		{
			d.ececuteClose = false;
			Close();
		}
		if (d.executeMode)
		{
			d.executeMode = false;
			this.DebugModeExec();
		}
		if (d.executeItem)
		{
			d.executeItem = false;
			this.DebugItemExec();
		}
		if (d.item.executeCreateItemList)
		{
			d.item.executeCreateItemList = false;
			this.DebugCreateExec();
		}
		if (d.setDummy)
		{
			d.setDummy = false;

			var capacity = Random.Range(0, 999);
			var num = Random.Range(0, capacity);
			var ownCharaID1 = (ulong)Random.Range(0, num);
			var ownCharaID2 = (ulong)Random.Range(0, num);
			var ownCharaID3 = (ulong)Random.Range(0, num);
			var ownCharaID4 = (ulong)Random.Range(0, num);

			this.DebugPrefabUpdate();

			var mode = d.mode;
			mode.mode = OpenMode.LobbyChara;
			mode.isUseHome = true;
			mode.isUseClose = true;
			mode.changeTargetOwnCharaID = ownCharaID1;
			mode.usedOwnCharaIDList.Clear();
			mode.usedOwnCharaIDList.Add(ownCharaID1);
			mode.usedOwnCharaIDList.Add(ownCharaID2);
			mode.usedOwnCharaIDList.Add(ownCharaID3);
			mode.usedOwnCharaIDList.Add(ownCharaID4);
			this.DebugModeExec();

			var create = d.item.createItemList;
			create.num = num;
			create.listMaxCharaID = 9999;
			this.DebugCreateExec();

			var item = d.item;
			item.capacity = capacity;
			this.DebugItemExec();
		}
	}
	void DebugModeExec()
	{
		var mode = this.DebugParam.mode;

		this.DebugPrefabUpdate();
		this._SetMode(mode.mode, mode.changeTargetOwnCharaID, mode.usedOwnCharaIDList, mode.isUseHome, mode.isUseClose,
			() => { Debug.Log("OnHome"); },
			() => { Debug.Log("OnClose"); },
			(changeTargetOwnCharaID, charaInfo) => { Debug.Log("OnOK"); }
			);
	}
	void DebugItemExec()
	{
		var item = this.DebugParam.item;
		var mode = this.DebugParam.mode;

		this.DebugPrefabUpdate();
		this._SetupCapacity(item.capacity, item.capacity, false);
		this._SetupItem(item.itemList, mode.changeTargetOwnCharaID);
	}
	void DebugCreateExec()
	{
		var item = this.DebugParam.item;
		var create = item.createItemList;

		if (this.DebugPrefabUpdate())
		{
			List<Scm.Common.Master.CharaMasterData> dataList = new List<Scm.Common.Master.CharaMasterData>();
			for (int i = 0; i <= create.listMaxCharaID; i++)
			{
				Scm.Common.Master.CharaMasterData data;
				if (MasterData.TryGetChara(i, out data))
				{
					dataList.Add(data);
				}
			}
			item.itemList.Clear();
			for (int i = 0; i < create.num; i++)
			{
				var index = Random.Range(0, dataList.Count - 1);
				var data = dataList[index];

				var info = new CharaInfo();
				var uuid = (ulong)(i + 1);
				info.DebugRandomSetup();
				info.DebugSetUUID(uuid);
				info.DebugSetAvatarType(data.ID);
				item.itemList.Add(info);
			}
		}
	}
	bool DebugPrefabUpdate()
	{
		string err = null;
		if (Object.FindObjectOfType(typeof(FiberController)) == null)
			err += "FiberController.prefab を入れて下さい\r\n";
		if (MasterData.Instance == null)
			err += "MasterData.prefab を入れて下さい\r\n";
		if (!string.IsNullOrEmpty(err))
		{
			Debug.LogWarning(err);
			return false;
		}

		var t = this.DebugParam;
		if (!t.IsReadMasterData)
		{
			MasterData.Read();
			t.IsReadMasterData = true;
		}
		return t.IsReadMasterData;
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

