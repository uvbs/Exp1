/// <summary>
/// キャラ簡易情報
/// 
/// 2016/01/25
/// </summary>
using UnityEngine;
using System;
using System.Collections.Generic;

public class GUICharaSimpleInfo : Singleton<GUICharaSimpleInfo>
{
	#region フィールド&プロパティ
	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	XUI.CharaSimpleInfo.CharaSimpleInfoView _viewAttach = null;
	XUI.CharaSimpleInfo.CharaSimpleInfoView ViewAttach { get { return _viewAttach; } }

	/// <summary>
	/// 開始時のアクティブ状態
	/// </summary>
	[SerializeField]
	bool _isStartActive = false;
	bool IsStartActive { get { return _isStartActive; } }

	/// <summary>
	/// レベル表示フォーマット
	/// </summary>
	[SerializeField]
	string _levelFormat = "{0}/{1}";
	string LevelFormat { get { return _levelFormat; } }

	/// <summary>
	/// ステータス表示フォーマット
	/// </summary>
	[SerializeField]
	string _statusFormat = "{0}";
	string StatusFormat { get { return _statusFormat; } }

	/// <summary>
	/// モデル
	/// </summary>
	XUI.CharaSimpleInfo.IModel Model { get; set; }

	/// <summary>
	/// ビュー
	/// </summary>
	XUI.CharaSimpleInfo.IView View { get; set; }

	/// <summary>
	/// コントローラ
	/// </summary>
	XUI.CharaSimpleInfo.IController Controller { get; set; }

	/// <summary>
	/// お気に入りボタンが押された時の通知用
	/// </summary>
	private event Action<CharaInfo> OnLockClickEvent = (charaInfo) => { };

	/// <summary>
	/// SetLockPlayerCharacterパケットのレスポンスが返ってきた時の通知用
	/// </summary>
	private event Action<LobbyPacket.SetLockPlayerCharacterResArgs> OnLockResponseEvent = (args) => { };
	#endregion

	#region 初期化
	protected override void Awake()
	{
		base.Awake();

		Construct();
	}

	private void Construct()
	{
		this.MemberInit();

		// モデル生成
		var model = new XUI.CharaSimpleInfo.Model();
		this.Model = model;
		this.Model.LevelFormat = this.LevelFormat;
		this.Model.StatusFormat = this.StatusFormat;

		// ビュー生成
		XUI.CharaSimpleInfo.IView view = null;
		if(this.ViewAttach != null)
		{
			view = this.ViewAttach.GetComponent(typeof(XUI.CharaSimpleInfo.IView)) as XUI.CharaSimpleInfo.IView;
		}
		this.View = view;
		this.View.OnBGClickEvent += HandleBGClickEvent;
		this.View.OnFavoriteClickEvent += HandleLockClickEvent;

		// コントローラ生成
		var controller = new XUI.CharaSimpleInfo.Controller(model, view);
		this.Controller = controller;

		// 初期アクティブ設定
		this.SetActive(this.IsStartActive, true);
		// キャラ情報はNULLをセット
		SetCharaInfo(null);
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
	/// 破棄
	/// </summary>
	void OnDestroy()
	{
		if(this.Controller != null)
		{
			this.Controller.Dispose();
		}

		this.OnLockClickEvent = null;
		this.OnLockResponseEvent = null;
	}
	#endregion

	#region アクティブ設定
	/// <summary>
	/// 閉じる
	/// </summary>
	public static void Close()
	{
		if (Instance != null)
		{
			Instance.SetActive(false, false);
		}
	}
	
	/// <summary>
	/// 開く
	/// </summary>
	public static void Open()
	{
		if (Instance != null) Instance.SetActive(true, false);
	}

	/// <summary>
	/// 開く
	/// </summary>
	public static void Open(Vector3 position, CharaInfo charaInfo)
	{
		if (Instance == null) { return; }

		Setup(position, charaInfo);
		Instance.SetActive(true, false);
	}

	/// <summary>
	/// アクティブを設定する
	/// </summary>
	/// <param name="isActive"></param>
	private void SetActive(bool isActive, bool isTweenSkip)
	{
		this.View.SetActive(isActive, isTweenSkip);
	}
	#endregion

	#region 設定
	/// <summary>
	/// セットアップ
	/// </summary>
	public static void Setup(Vector3 position, CharaInfo charaInfo)
	{
		if (Instance == null) { return; }
		Instance._Setup(position, charaInfo);
	}
	private void _Setup(Vector3 position, CharaInfo charaInfo)
	{
		if(this.Controller != null)
		{
			this.Controller.Setup(position, charaInfo);
		}
		// 詳細なキャラクター情報を取得する
		Instance.SendPlayerCharacter();
	}
	#endregion

	#region キャラ情報
	/// <summary>
	/// キャラ情報セット
	/// </summary>
	/// <param name="charaInfo"></param>
	public static void SetCharaInfo(CharaInfo charaInfo)
	{
		if (Instance == null || Instance.Model == null) { return;}
		Instance.Model.CharaInfo = charaInfo;
	}

	/// <summary>
	/// キャラ情報取得
	/// </summary>
	/// <returns></returns>
	public static CharaInfo GetCharaInfo()
	{
		if (Instance == null || Instance.Model == null) { return null; }
		return Instance.Model.CharaInfo;
	}
	#endregion

	#region 位置
	/// <summary>
	/// 位置セット
	/// </summary>
	/// <param name="position"></param>
	public void SetPosition(Vector3 position)
	{
		if (this.Model == null) { return; }
		this.Model.Position = position;
	}
	#endregion

	#region 位置/サイズ
	/// <summary>
	/// ウィンドウサイズを取得する
	/// </summary>
	/// <returns></returns>
	public static Vector2 GetSize()
	{
		var size = new Vector2();
		if (Instance == null) { return size; }

		size.x = Instance.View.Width;
		size.y = Instance.View.Height;

		return size;
	}

	/// <summary>
	/// ウィンドウの四隅のワールド座標を取得
	/// </summary>
	/// <returns></returns>
	public static Vector3[] GetWorldCorners()
	{
		var worldCorners = new Vector3[4];
		if (Instance == null || Instance.View == null) { return worldCorners; }
		worldCorners = Instance.View.WorldCorners;

		return worldCorners;
	}
	#endregion

	#region 背景側が押された時
	/// <summary>
	/// 背景側が押された時に呼び出される
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void HandleBGClickEvent(object sender, EventArgs e)
	{
		// 閉じる
		GUIController.SingleClose();
	}
	#endregion

	#region お気に入りボタン
	/// <summary>
	/// お気に入りボタンが押された時に呼び出される
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void HandleLockClickEvent(object sender, EventArgs e)
	{
		// SetLockPlayerCharacterパケット送信
		SendSetLockPlayerCharacter();

		// 通知
		this.OnLockClickEvent(GetCharaInfo());
	}

	/// <summary>
	/// お気に入りボタン通知イベントに登録する
	/// </summary>
	/// <param name="favoriteClickEvent"></param>
	public static void AddLockClickEvent(Action<CharaInfo> favoriteClickEvent)
	{
		if (Instance == null) { return; }
		Instance.OnLockClickEvent += favoriteClickEvent;
	}

	/// <summary>
	/// お気に入りボタン通知イベントを削除する
	/// </summary>
	/// <param name="favoriteClickEvent"></param>
	public static void RemoveLockClickEvent(Action<CharaInfo> favoriteClickEvent)
	{
		if (Instance == null) { return; }
		Instance.OnLockClickEvent -= favoriteClickEvent;
	}
	#endregion

	#region 通信系

	#region SetLockPlayerCharacter パケット
	/// <summary>
	/// SetLockPlayerCharacterパケット送信
	/// </summary>
	private void SendSetLockPlayerCharacter()
	{
		CharaInfo info = GetCharaInfo();
		if (info == null) { return; }

		LobbyPacket.SendSetLockPlayerCharacter(info.UUID, !info.IsLock, this.Response);
	}

	/// <summary>
	/// SetLockPlayerCharacterパケットのレスポンス
	/// </summary>
	/// <param name="args"></param>
	private void Response(LobbyPacket.SetLockPlayerCharacterResArgs args)
	{
		SetLockResult(args.Result, args.UUID, args.IsLock);

		// 通知
		this.OnLockResponseEvent(args);
	}

	/// <summary>
	/// ロック設定結果
	/// </summary>
	/// <param name="isResult"></param>
	/// <param name="uuid"></param>
	/// <param name="isLock"></param>
	private void SetLockResult(bool isResult, ulong uuid, bool isLock)
	{
		if (!isResult) { return; }

		var info = GetCharaInfo();
		if (info == null) return;

		// サーバから受け取ったキャラが開いているキャラと同じかチェック
		if(info.UUID == uuid)
		{
			var newInfo = info.Clone();
			newInfo.IsLock = isLock;
			SetCharaInfo(newInfo);
		}
	}

	/// <summary>
	/// SetLockPlayerCharacterパケットのレスポンスイベント追加
	/// </summary>
	/// <param name="responseEvent"></param>
	public static void AddLockResponseEvent(Action<LobbyPacket.SetLockPlayerCharacterResArgs> responseEvent)
	{
		if (Instance == null) { return; }
		Instance.OnLockResponseEvent += responseEvent;
	}

	/// <summary>
	/// SetLockPlayerCharacterパケットのレスポンスイベント削除
	/// </summary>
	/// <param name="responseEvent"></param>
	public static void RemoveLockResponseEvent(Action<LobbyPacket.SetLockPlayerCharacterResArgs> responseEvent)
	{
		if (Instance == null) { return; }
		Instance.OnLockResponseEvent -= responseEvent;
	}
	#endregion

	#region PlayerCharacter パケット
	/// <summary>
	/// プレイヤーキャラクター情報を取得
	/// </summary>
	private void SendPlayerCharacter()
	{
		CharaInfo info = GetCharaInfo();
		if (info == null) { return; }

		// 通知
		LobbyPacket.SendPlayerCharacter(info.UUID, this.Response);
	}

	/// <summary>
	/// PlayerCharacterReq パケットのレスポンス
	/// </summary>
	void Response(LobbyPacket.PlayerCharacterResArgs args)
	{
		this.SetupPlayerCharacterInfo(args.CharaInfo, args.SlotBonusHitPoint, args.SlotBonusAttack, args.SlotBonusDefense, args.SlotBonusExtra, args.SlotList);
	}

	/// <summary>
	/// プレイヤーキャラクター情報を設定
	/// </summary>
	void SetupPlayerCharacterInfo(CharaInfo info, int slotHitPoint, int slotAttack, int slotDefense, int slotExtra, List<CharaInfo> slotList)
	{
		if (this.Controller != null)
		{
			this.Controller.SetupPlayerCharacterInfo(info, slotHitPoint, slotAttack, slotDefense, slotExtra, slotList);
		}
	}
	#endregion
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
		}

		[SerializeField]
		DummyEvent _dummy = new DummyEvent();
		public DummyEvent Dummy { get { return _dummy; } }
		[System.Serializable]
		public class DummyEvent : IDebugParamEvent
		{
			public event System.Action<CharaInfo> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			CharaInfo charaInfo;

			public void Update()
			{
				if(this.execute)
				{
					this.execute = false;
					var info = charaInfo.Clone();
					this.Execute(info);
				}
			}
		}
	}

	void DebugInit()
	{
		var d = this.DebugParam;

		d.ExecuteClose += () => { Close(); };
		d.ExecuteActive += () => { Open(); };

		d.Dummy.Execute += (info) =>
		{
			Open(new Vector3(0, 0, 0), info);
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
