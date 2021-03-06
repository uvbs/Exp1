/// <summary>
/// 進化素材リスト
/// 
/// 2016/02/08
/// </summary>
using UnityEngine;
using System;
using System.Collections.Generic;

public class GUIEvolutionMaterialList : MonoBehaviour
{
	#region フィールド&プロパティ
	/// <summary>
	/// 進化素材リストビューアタッチ
	/// </summary>
	[SerializeField]
	private XUI.CharaList.EvolutionMaterialListView _materialListViewAttach = null;
	private XUI.CharaList.EvolutionMaterialListView MaterialListViewAttach { get { return _materialListViewAttach; } }

	/// <summary>
	/// キャラアイテムページ付スクロールビュー
	/// </summary>
	[SerializeField]
	private XUI.CharaList.GUIItemScrollView _itemScrollView = null;
	public XUI.CharaList.GUIItemScrollView ItemScrollView { get { return _itemScrollView; } }

	/// <summary>
	/// モデル
	/// </summary>
	private XUI.CharaList.IModel Model { get; set; }

	/// <summary>
	/// ビュー
	/// </summary>
	private XUI.CharaList.IView View { get; set; }

	/// <summary>
	/// 進化素材リストビュー
	/// </summary>
	private XUI.CharaList.IEvolutionMaterialListView MaterialListView { get; set; }

	/// <summary>
	/// 進化素材リストコントローラ
	/// </summary>
	private XUI.CharaList.IEvolutionMaterialListController Controller { get; set; }

	/// <summary>
	/// 登録されているアイテムが押された時の通知用
	/// </summary>
	public Action<GUICharaItem> OnItemClickEvent = (item) => { };

	/// <summary>
	/// 登録されているアイテムが長押しされた時の通知用
	/// </summary>
	public event Action<GUICharaItem> OnItemLongPressEvent = (item) => { };

	/// <summary>
	/// 登録されているアイテムに変更があった時の通知用
	/// </summary>
	public event Action<GUICharaItem> OnItemChangeEvent = (item) => { };

	/// <summary>
	/// 全てのアイテムが更新された時のイベント通知用
	/// </summary>
	public event Action OnUpdateItemsEvent = () => { };

	/// <summary>
	/// 更新できる状態かどうか
	/// </summary>
	public bool CanUpdate
	{
		get
		{
			if (this.Model == null) return false;
			if (this.View == null) return false;
			if (this.MaterialListView == null) return false;
			return true;
		}
	}
	#endregion

	#region 初期化
	void Awake()
	{
		this.MemberInit();
		this.Construct();
	}

	void Start(){}

	private void Construct()
	{
		// モデル生成
		this.Model = new XUI.CharaList.Model(this.ItemScrollView);

		// ビュー生成
		XUI.CharaList.IEvolutionMaterialListView materialListView = null;
		XUI.CharaList.IView view = null;
		if(this.MaterialListViewAttach != null)
		{
			materialListView = this.MaterialListViewAttach.GetComponent(typeof(XUI.CharaList.IEvolutionMaterialListView)) as XUI.CharaList.IEvolutionMaterialListView;
			view = this.MaterialListViewAttach.GetComponent(typeof(XUI.CharaList.IView)) as XUI.CharaList.IView;
		}
		this.MaterialListView = materialListView;
		this.View = view;

		// コントローラ
		var controller = new XUI.CharaList.EvolutionMaterialListController(this.Model, this.View, this.MaterialListView);
		this.Controller = controller;
		this.Controller.OnItemClickEvent += OnItemClick;
		this.Controller.OnItemLongPressEvent += OnItemLongPress;
		this.Controller.OnItemChangeEvent += OnItemChange;
		this.Controller.OnUpdateItemsEvent += OnUpdateItems;
	}

	/// <summary>
	/// シリアライズされていないメンバー初期化
	/// </summary>
	private void MemberInit()
	{
		this.Model = null;
		this.MaterialListView = null;
		this.View = null;
		this.Controller = null;
	}

	/// <summary>
	/// 破棄
	/// </summary>
	void OnDestroy()
	{
		if (this.Controller != null)
		{
			this.Controller.Dispose();
		}

		this.OnItemClickEvent = null;
		this.OnItemLongPressEvent = null;
		this.OnItemChangeEvent = null;
	}
	#endregion

	#region アクティブ設定
	/// <summary>
	/// アクティブを設定する
	/// </summary>
	/// <param name="isActive"></param>
	public void SetActive(bool isActive)
	{
		if (this.View == null) { return; }
		this.View.SetActive(isActive);
	}
	#endregion

	#region アイテム総数
	/// <summary>
	/// アイテム総数をセットする
	/// </summary>
	public void SetCapacity(int capacity)
	{
		if (this.Controller == null) { return; }
		this.Controller.SetupCapacity(capacity);
	}
	#endregion

	#region アイテム設定/取得
	/// <summary>
	/// アイテムを設定する
	/// </summary>
	public void SetupItems(CharaInfo baseCharaInfo, Dictionary<int, List<CharaInfo>> haveCharaInfoDic)
	{
		if (this.Controller == null) { return; }
		this.Controller.SetupItems(baseCharaInfo, haveCharaInfoDic);
	}

	/// <summary>
	/// 現ページ内のキャラアイテムリストを取得
	/// </summary>
	public List<GUICharaItem> GetNowPageItemList()
	{
		if(this.Model == null)
		{
			return new List<GUICharaItem>();
		}
		return this.Model.GetNowPageItemList();
	}
	#endregion

	#region 素材
	/// <summary>
	/// 進化素材を選択されている素材欄にセットする
	/// </summary>
	public bool SetBaitMaterial(CharaInfo charaInfo)
	{
		if (this.Controller == null) { return false; }
		return this.Controller.SetBaitMaterial(charaInfo);
	}

	/// <summary>
	/// セットされている素材を解除する
	/// </summary>
	public GUICharaItem RemoveBaitMaterial(CharaInfo charaInfo)
	{
		if (this.Controller == null) { return null; }
		return this.Controller.RemoveBaitMaterial(charaInfo);
	}
	#endregion

	#region アイテム選択
	/// <summary>
	/// 選択アイテム
	/// </summary>
	public GUICharaItem SelectItem
	{
		get
		{
			if (this.Controller == null) { return null; }
			return this.Controller.SelectItem;
		}
		set
		{
			if (this.Controller == null) { return; }
			this.Controller.SelectItem = value;
		}
	}

	/// <summary>
	/// 次選択するアイテムを取得する
	/// </summary>
	public GUICharaItem GetNextSelectItem()
	{
		if (this.Controller == null) { return null; }
		return this.Controller.GetNextSelectItem();
	}
	#endregion

	#region クリア
	/// <summary>
	/// 進化素材情報を削除
	/// </summary>
	public bool ClearMaterial()
	{
		if (this.Controller == null) { return false; }
		return this.Controller.ClearMaterial();
	}
	#endregion

	#region 進化可能
	/// <summary>
	/// 進化可能状態か
	/// </summary>
	public bool CanEvolution
	{
		get
		{
			if (this.Controller == null) { return false; }
			return this.Controller.CanEvolution;
		}
	}
	#endregion

	#region キャラ情報取得
	/// <summary>
	/// キャラアイコンが空状態以外のキャラ情報一覧を取得する
	/// </summary>
	/// <returns></returns>
	public List<CharaInfo> GetCharaInfoList()
	{
		if (this.Model == null) { return new List<CharaInfo>(); }

		var infoList = new List<CharaInfo>();
		foreach (var info in this.Model.CharaInfoList)
		{
			if (info == null) { continue; };
			infoList.Add(info);
		}

		return infoList;
	}

	/// <summary>
	/// 追加されている餌キャラ情報を取得する
	/// </summary>
	public List<CharaInfo> GetBaitCharaInfoList()
	{
		if (this.Model == null) { return new List<CharaInfo>(); }

		var infoList = new List<CharaInfo>();
		foreach (var info in this.Model.CharaInfoList)
		{
			if (info == null || info.UUID <= 0) { continue; };
			infoList.Add(info);
		}

		return infoList;
	}

	/// <summary>
	/// 追加されているキャラ素材のUUIDリストを取得する
	/// </summary>
	/// <returns></returns>
	public List<ulong> GetBaitCharaUUIDList()
	{
		if (this.Model == null)
		{
			return new List<ulong>();
		}

		var infoList = new List<ulong>();
		foreach (var info in this.Model.CharaInfoList)
		{
			if (info == null || info.UUID <= 0) { continue; };
			infoList.Add(info.UUID);
		}

		return infoList;
	}

	/// <summary>
	/// セットされている素材キャラ情報のみを更新する
	/// </summary>
	public void UpdateBaitMaterialCharaInfo(List<CharaInfo> charaInfoList)
	{
		if (this.Controller == null) { return; }
		this.Controller.UpdateBaitMaterialCharaInfo(charaInfoList);
	}
	#endregion

	#region アイテムイベント
	/// <summary>
	/// アイテムが押された時に呼び出される
	/// </summary>
	private void OnItemClick(object sender, XUI.CharaList.ItemClickEventArgs e)
	{
		// 通知
		this.OnItemClickEvent(e.Item);
	}

	/// <summary>
	/// アイテムが長押しされた時に呼び出される
	/// </summary>
	private void OnItemLongPress(object sender, XUI.CharaList.ItemClickEventArgs e)
	{
		// 通知
		this.OnItemLongPressEvent(e.Item);
	}

	/// <summary>
	/// アイテムに変更があった時に呼び出される
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void OnItemChange(object sender, XUI.CharaList.ItemChangeEventArgs e)
	{
		// 通知
		this.OnItemChangeEvent(e.Item);
	}

	/// <summary>
	/// 全てのアイテムが更新された時のイベント通知
	/// </summary>
	private void OnUpdateItems(object sender, EventArgs e)
	{
		// 通知
		this.OnUpdateItemsEvent();
	}
	#endregion


	#region デバッグ
#if UNITY_EDITOR && XW_DEBUG
	[SerializeField]
	GUIDebugParam _debugParam = new GUIDebugParam();
	GUIDebugParam DebugParam { get { return _debugParam; } }
	[System.Serializable]
	public class GUIDebugParam : GUIDebugParamBase
	{
		public GUIDebugParam()
		{
			AddEvent(this.Dummy);
			AddEvent(this.SetMaterial);
		}

		[SerializeField]
		DummyEvent _dummy = new DummyEvent();
		public DummyEvent Dummy { get { return _dummy; } set { _dummy = value; } }
		[System.Serializable]
		public class DummyEvent : IDebugParamEvent
		{
			public event System.Action<CharaInfo, Dictionary<int, List<CharaInfo>>> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			AvatarType avatarType = AvatarType.Begin;
			[SerializeField]
			int rank = 1;

			public void Update()
			{
				if (this.execute)
				{
					MasterData.Read();
					this.execute = false;

					var list = this.GetMaterialList((int)avatarType, rank);
					var baseCharaInfo = new CharaInfo();
					baseCharaInfo.DebugSetAvatarType((int)avatarType);
					baseCharaInfo.DebugSetRank(rank);
					Execute(baseCharaInfo, list);
				}
			}

			/// <summary>
			/// 素材一覧を取得
			/// </summary>
			private Dictionary<int, List<CharaInfo>> GetMaterialList(int baseCharaMasterDataId, int baseRank)
			{
				var materialDic = new Dictionary<int, List<CharaInfo>>();
				var dataList = new List<Scm.Common.XwMaster.CharaEvolutionMaterialMasterData>();
				if (!MasterData.TryGetCharaEvolutionMaterial(baseCharaMasterDataId,  Mathf.Min(baseRank+1, 5), out dataList)) { return materialDic; }
				
				foreach (var data in dataList)
				{
					if (!materialDic.ContainsKey(data.MaterialCharacterMasterId))
					{
						materialDic.Add(data.MaterialCharacterMasterId, new List<CharaInfo>());
					}
				}

				return materialDic;
			}
		}

		[SerializeField]
		SetMaterialEvent _setMaterial = new SetMaterialEvent();
		public SetMaterialEvent SetMaterial { get { return _setMaterial; } set { _setMaterial = value; } }
		[System.Serializable]
		public class SetMaterialEvent : IDebugParamEvent
		{
			public event System.Action<CharaInfo> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			AvatarType avatarType = AvatarType.Begin;
			[SerializeField]
			int rank = 1;

			public void Update()
			{
				if (this.execute)
				{
					MasterData.Read();
					this.execute = false;
					var info = CreateCharaInfo();

					Execute(info);
				}
			}
			CharaInfo CreateCharaInfo()
			{
				var info = new CharaInfo();
				info.DebugRandomSetup();
				info.DebugSetAvatarType((int)avatarType);
				info.DebugSetRank(rank);
				return info;
			}
		}

	}

	void DebugInit()
	{
		var d = this.DebugParam;

		d.ExecuteActive += () => { SetActive(true); };
		d.ExecuteClose += () => { SetActive(false); };

		d.Dummy.Execute += (baseCharaInfo, list) =>
		{
			SetCapacity(5);
			SetupItems(baseCharaInfo, list);

			// 一番最初に追加されたアイテムを選択状態にする
			List<GUICharaItem> itemList = this.Model.GetNowPageItemList();
			if (itemList.Count > 0)
			{
				GUICharaItem nextSelectItem = itemList[0];
				this.SelectItem = nextSelectItem;
			}
		};

		d.SetMaterial.Execute += (info) =>
		{
			if(SetBaitMaterial(info))
			{
				this.SelectItem = this.GetNextSelectItem();
			}
		};
	}

	bool _isDebugInit = false;
	void DebugUpdate()
	{
		if (!this._isDebugInit)
		{
			this._isDebugInit = true;
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
