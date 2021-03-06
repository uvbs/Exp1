/// <summary>
/// アイテム
/// 
/// 2016/03/17
/// </summary>
using UnityEngine;
using System;

public class GUIItem : MonoBehaviour
{
	#region フィールド&プロパティ
	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	private XUI.Item.ItemView _viewAttach = null;
	private XUI.Item.ItemView ViewAttach { get { return _viewAttach; } }

	/// <summary>
	/// ドラッグ用
	/// </summary>
	[SerializeField]
	private UIDragScrollView _dragScrollView = null;
	private UIDragScrollView DragScrollView { get { return _dragScrollView; } }

	/// <summary>
	/// モデル
	/// </summary>
	private XUI.Item.IModel Model { get; set; }

	/// <summary>
	/// ビュー
	/// </summary>
	private XUI.Item.IView View { get; set; }

	/// <summary>
	/// コントローラ
	/// </summary>
	private XUI.Item.IController Controller { get; set; }

	/// <summary>
	/// アイテムアイコン
	/// </summary>
	private ItemIcon ItemIcon { get { return ScmParam.Lobby.ItemIcon; } }

	/// <summary>
	/// アイテムが押された時の通知用
	/// </summary>
	public event Action<GUIItem> OnItemClickEvent = (item) => { };

	/// <summary>
	/// アイテムに変化があった時の通知用
	/// </summary>
	public event Action<GUIItem> OnItemChangeEvent = (item) => { };
	#endregion

	#region 生成
	/// <summary>
	/// アイテムの生成
	/// </summary>
	public static GUIItem Create(GameObject prefab, Transform parent, int itemIndex)
	{
		// インスタンス化
		var go = SafeObject.Instantiate(prefab) as GameObject;
		if (go == null) { return null; }

		// 親子付け
		go.SetParentWithLayer(parent.gameObject, false);
		// 名前
		go.name = string.Format("{0}{1}", prefab.name, itemIndex);
		// 可視化
		if (!go.activeSelf)
		{
			go.SetActive(true);
		}

		// コンポーネント取得
		var item = go.GetComponentInChildren(typeof(GUIItem)) as GUIItem;
		if (item == null) { return null; }

		// ドラッグによるスクロール機能をOFFに設定
		if (item.DragScrollView != null)
		{
			item.DragScrollView.enabled = false;
		}

		return item;
	}
	#endregion

	#region 初期化
	void Awake()
	{
		this.Construct();
	}

	private void Construct()
	{
		this.MemberInit();

		// モデル生成
		var model = new XUI.Item.Model();
		this.Model = model;
		this.Model.OnSetItemInfoChange += this.OnItemChange;

		// ビュー生成
		XUI.Item.IView view = null;
		if(this.ViewAttach != null)
		{
			view = this.ViewAttach.GetComponent(typeof(XUI.Item.IView)) as XUI.Item.IView;
		}
		this.View = view;
		this.View.OnItemClickEvent += this.OnItemClick;
		
		// コントローラ生成
		var controller = new XUI.Item.Controller(model, view, this.ItemIcon);
		this.Controller = controller;
	}

	/// <summary>
	/// シリアライズされていないメンバー初期化
	/// </summary>
	private void MemberInit()
	{
		this.Model = null;
		this.View = null;
		this.Controller = null;
	}

	/// <summary>
	/// データ初期化
	/// </summary>
	public void Setup()
	{
		if (this.Controller == null) { return; }
		this.Controller.Setup();
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
		this.View.SetActive(isActive);
	}
	#endregion

	#region アイテム状態
	/// <summary>
	/// アイテムの状態をセット
	/// </summary>
	public void SetState(XUI.Item.ItemStateType stateType)
	{
		if (this.Controller == null) { return; }
		this.Controller.SetItemState(stateType);
	}
	/// <summary>
	/// アイテムの状態をセット
	/// </summary>
	public void SetState(XUI.Item.ItemStateType stateType, ItemInfo itemInfo)
	{
		if (this.Controller == null) { return; }
		this.SetItemInfo(itemInfo);
		this.Controller.SetItemState(stateType);
	}

	/// <summary>
	/// アイテムの状態を取得する
	/// </summary>
	public XUI.Item.ItemStateType GetState()
	{
		return this.Controller.GetItemState();
	}
	#endregion

	#region アイテム情報
	/// <summary>
	/// アイテム情報をセットする
	/// </summary>
	public void SetItemInfo(ItemInfo itemInfo)
	{
		if (this.Controller == null) { return; }
		this.Controller.SetItemInfo(itemInfo);
	}

	/// <summary>
	/// アイテム情報の取得
	/// </summary>
	public ItemInfo GetItemInfo()
	{
		if (this.Controller == null) { return null; }
		return this.Controller.GetItemInfo();
	}
	#endregion

	#region アイテム無効状態
	/// <summary>
	/// アイテムの無効状態をセットする
	/// </summary>
	public void SetDisableState(XUI.Item.DisableType state)
	{
		if (this.Controller == null) { return; }
		this.Controller.SetDisableState(state);
	}

	/// <summary>
	/// アイテムの無効状態を取得する
	/// </summary>
	public XUI.Item.DisableType GetDisableState()
	{
		if (this.Controller == null) { return XUI.Item.DisableType.None; }
		return this.Controller.GetDisableState();
	}

	/// <summary>
	/// 素材無効状態をセットする
	/// </summary>
	public void SetBaitState(int baitIndex)
	{
		if (this.Controller == null) { return; }
		this.Controller.SetBaitState(baitIndex);
	}
	#endregion

	#region 選択
	/// <summary>
	/// 選択設定
	/// </summary>
	public void SetSelect(bool isSelect)
	{
		if (this.Controller == null) { return; }
		this.Controller.SetSelect(isSelect);
	}

	/// <summary>
	/// 選択フラグを取得
	/// </summary>
	public bool GetSelect()
	{
		if (this.Controller == null) { return false; }
		return this.Controller.GetSelect();
	}
	#endregion

	#region スクロール
	/// <summary>
	/// ドラッグによるスクロールの有効設定
	/// </summary>
	public void SetDragScrollEnable(bool isEnable)
	{
		if (this.DragScrollView == null) { return; }
		this.DragScrollView.enabled = isEnable;
	}
	#endregion

	#region イベント
	/// <summary>
	/// アイテムが押された時に呼び出される
	/// </summary>
	private void OnItemClick(object sender, EventArgs e)
	{
		// イベント通知
		this.OnItemClickEvent(this);
	}

	/// <summary>
	/// アイテムに変化があった時に呼び出される
	/// </summary>
	private void OnItemChange(object sender, EventArgs e)
	{
		// イベント通知
		this.OnItemChangeEvent(this);
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
			this.AddEvent(this.State);
			this.AddEvent(this.Disable);
			this.AddEvent(this.Select);
		}

		[SerializeField]
		StateEvent _state = new StateEvent();
		public StateEvent State { get { return _state; } set { _state = value; } }
		[System.Serializable]
		public class StateEvent : IDebugParamEvent
		{
			public event System.Action<XUI.Item.ItemStateType, ItemInfo, int> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			XUI.Item.ItemStateType stateType = XUI.Item.ItemStateType.Empty;
			[SerializeField]
			ItemInfo itemInfo = null;
			[SerializeField]
			int stack = 0;

			public void Update()
			{
				if (this.execute)
				{
					this.execute = false;
					ItemInfo info = null;
					if (stateType != XUI.Item.ItemStateType.Empty)
					{
						info = itemInfo.Clone();
					}
					this.Execute(stateType, info, stack);
				}
			}
		}

		[SerializeField]
		DisableEvent _disable = new DisableEvent();
		public DisableEvent Disable { get { return _disable; } set { _disable = value; } }
		[System.Serializable]
		public class DisableEvent : IDebugParamEvent
		{
			public event System.Action<XUI.Item.DisableType, int> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			XUI.Item.DisableType state = XUI.Item.DisableType.None;
			[SerializeField]
			int selectNumber = 1;

			public void Update()
			{
				if (this.execute)
				{
					this.execute = false;
					this.Execute(this.state, this.selectNumber);
				}
			}
		}

		[SerializeField]
		SelectEvent _select = new SelectEvent();
		public SelectEvent Select { get { return _select; } set { _select = value; } }
		[System.Serializable]
		public class SelectEvent : IDebugParamEvent
		{
			public event System.Action<bool> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			bool isSelect = false;

			public void Update()
			{
				if (this.execute)
				{
					this.execute = false;
					this.Execute(this.isSelect);
				}
			}
		}
	}

	void DebugInit()
	{
		var d = this.DebugParam;

		d.ExecuteClose += () => { SetActive(false); };
		d.ExecuteActive += () => { SetActive(true); };

		d.State.Execute += (stateType, info, stack) =>
		{
			d.ReadMasterData();
			SetState(stateType, info);
		};

		d.Disable.Execute += (state, selectNumber) =>
		{
			if (state == XUI.Item.DisableType.Bait)
			{
				int index = selectNumber - 1;
				SetBaitState(index);
			}
			else
			{
				SetDisableState(state);
			}
		};

		d.Select.Execute += (isSelect) =>
		{
			SetSelect(isSelect);
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
