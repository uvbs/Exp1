/// <summary>
/// バッテリー表示
/// 
/// 2015/12/08
/// </summary>
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GUIBattery : Singleton<GUIBattery>
{
	#region フィールド＆プロパティ
	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	XUI.Battery.BatteryView _viewAttach = null;
	XUI.Battery.BatteryView ViewAttach { get { return _viewAttach; } }

	/// <summary>
	/// 12時間表記用フォーマット
	/// </summary>
	[SerializeField]
	string _format12Hour = "hh:mm";
	string Format12Hour { get { return _format12Hour; } }

	/// <summary>
	/// 24時間表記用フォーマット
	/// </summary>
	[SerializeField]
	string _format24Hour = "HH:mm";
	string Format24Hour { get { return _format24Hour; } }

	/// <summary>
	/// バッテリーの残量によって色を変化させるリスト
	/// </summary>
	[SerializeField]
	List<XUI.Battery.LevelState> _batteryLevelList = new List<XUI.Battery.LevelState>();
	List<XUI.Battery.LevelState> BatteryLevelList { get { return _batteryLevelList; } }

	/// <summary>
	/// ネットワークの強さによって色を変化させるリスト
	/// </summary>
	[SerializeField]
	List<XUI.Battery.LevelState> _networkLevelList = new List<XUI.Battery.LevelState>();
	List<XUI.Battery.LevelState> NetworkLevelList { get { return _networkLevelList; } }

	// モデル
	XUI.Battery.IModel Model { get; set; }
	// ビュー
	XUI.Battery.IView View { get; set; }
	// コントローラー
	XUI.Battery.IController Contoller { get; set; }
	// シリアライズされていないメンバー初期化
	void MemberInit()
	{
		this.Model = null;
		this.View = null;
		this.Contoller = null;
	}
	#endregion

	#region 初期化
	override protected void Awake()
	{
		base.Awake();

//		PluginController.StartBatteryInfoListener(this.HandleBatteryInfo);
//		PluginController.StartNetworkInfoListener(this.HandleNetworkInfo);

		this.Construct();
	}
	void Construct()
	{
		this.MemberInit();

		// モデル生成
		var model = new XUI.Battery.Model();
		this.Model = model;
		this._SetTimeFormat(true);
		this.SetBatteryLevelList(this.BatteryLevelList);
		this.SetNetworkLevelList(this.NetworkLevelList);

		// ビュー生成
		XUI.Battery.IView view = null;
		if (this.ViewAttach != null)
		{
			view = this.ViewAttach.GetComponent(typeof(XUI.Battery.IView)) as XUI.Battery.IView;
		}
		this.View = view;

		// コントローラー生成
		var controller = new XUI.Battery.Controller(model, view);
		this.Contoller = controller;
	}
	#endregion

	#region 破棄
	void OnDestroy()
	{
//		PluginController.StopBatteryInfoListener();
//		PluginController.StopNetworkInfoListener();
	}
	#endregion

	#region アクティブ設定
	/// <summary>
	/// 閉じる
	/// </summary>
	public static void Close()
	{
		if (Instance != null) Instance._SetActive(false);
	}
	/// <summary>
	/// アクティブ設定
	/// </summary>
	public static void SetActive(bool isActive)
	{
		if (Instance != null) Instance._SetActive(isActive);
	}
	/// <summary>
	/// アクティブ設定
	/// </summary>
	void _SetActive(bool isActive)
	{
		if (this.View != null)
		{
			this.View.SetActive(isActive);
		}
	}
	#endregion

	#region バッテリー
	/// <summary>
	/// バッテリー情報通知
	/// </summary>
	void HandleBatteryInfo(BatteryInfo info)
	{
		this.SetBatteryInfo(info.level, info.state);
	}
	/// <summary>
	/// バッテリー情報設定
	/// </summary>
	void SetBatteryInfo(int level, BatteryInfo.BatteryState state)
	{
		if (this.Model != null)
		{
			this.Model.BatteryLevel = level;
			this.Model.BatteryState = state;
		}
	}
	/// <summary>
	/// バッテリーレベルリスト設定
	/// </summary>
	void SetBatteryLevelList(List<XUI.Battery.LevelState> list)
	{
		if (this.Model != null)
		{
			this.Model.SetBatteryLevelList(list);
		}
	}
	#endregion

	#region ネットワーク
	/// <summary>
	/// ネットワーク情報通知
	/// </summary>
	void HandleNetworkInfo(NetworkInfo info)
	{
		this.SetNetworkInfo(info.level, info.type);
	}
	/// <summary>
	/// ネットワーク情報設定
	/// </summary>
	void SetNetworkInfo(int level, NetworkInfo.NetworkType type)
	{
		if (this.Model != null)
		{
			this.Model.NetworkLevel = level;
			this.Model.NetworkType = type;
		}
	}
	/// <summary>
	/// ネットワークレベルリスト設定
	/// </summary>
	void SetNetworkLevelList(List<XUI.Battery.LevelState> list)
	{
		if (this.Model != null)
		{
			this.Model.SetNetworkLevelList(list);
		}
	}
	#endregion

	#region 時間設定
	/// <summary>
	/// 時間フォーマットを設定する
	/// </summary>
	public static void SetTimeFormat(bool is24Hour)
	{
		if (Instance != null) Instance._SetTimeFormat(is24Hour);
	}
	void _SetTimeFormat(bool is24Hour)
	{
		if (this.Model != null)
		{
			this.Model.TimeFormat = is24Hour ? this.Format24Hour : this.Format12Hour;
		}
	}
	/// <summary>
	/// 時間設定
	/// </summary>
	void SetDateTime(DateTime dateTime)
	{
		if (this.Model != null)
		{
			this.Model.DateTime = dateTime;
		}
	}
	#endregion

	#region 更新
	void Update()
	{
		this.SetDateTime(DateTime.Now);
#if UNITY_EDITOR && XW_DEBUG
		this.DebugUpdate();
#endif
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
		/// <summary>
		/// 初期化
		/// 機能を追加したらここで AddEvent をする
		/// </summary>
		public GUIDebugParam()
		{
			this.AddEvent(this.Battery);
			this.AddEvent(this.Network);
			this.AddEvent(this.Time);
			this.AddEvent(this.TimeFormat);
		}

		/// <summary>
		/// バッテリーイベント
		/// </summary>
		[SerializeField]
		BatteryEvent _battery = new BatteryEvent();
		public BatteryEvent Battery { get { return _battery; } }
		[System.Serializable]
		public class BatteryEvent : IDebugParamEvent
		{
			public event System.Action ExecuteLevelList = delegate { };
			[SerializeField]
			bool executeLevelList = false;

			public event System.Action<int, BatteryInfo.BatteryState> UpdateInfo = delegate { };
			[SerializeField]
			bool updateInfo = false;
			[SerializeField]
			[Range(0, 100)]
			int level = 0;
			[SerializeField]
			BatteryInfo.BatteryState state = BatteryInfo.BatteryState.STATUS_NONE;

			public void Update()
			{
				if (this.executeLevelList)
				{
					this.executeLevelList = false;
					this.ExecuteLevelList();
				}
				if (this.updateInfo)
				{
					this.UpdateInfo(this.level, this.state);
				}
			}
		}

		/// <summary>
		/// ネットワークイベント
		/// </summary>
		[SerializeField]
		NetworkEvent _network = new NetworkEvent();
		public NetworkEvent Network { get { return _network; } }
		[System.Serializable]
		public class NetworkEvent : IDebugParamEvent
		{
			public event System.Action ExecuteLevelList = delegate { };
			[SerializeField]
			bool executeLevelList = false;

			public event System.Action<int, NetworkInfo.NetworkType> UpdateInfo = delegate { };
			[SerializeField]
			bool updateInfo = false;
			[SerializeField]
			[Range(0, 100)]
			int level = 0;
			[SerializeField]
			NetworkInfo.NetworkType type = NetworkInfo.NetworkType.TYPE_NONE;

			public void Update()
			{
				if (this.executeLevelList)
				{
					this.executeLevelList = false;
					this.ExecuteLevelList();
				}
				if (this.updateInfo)
				{
					this.UpdateInfo(this.level, this.type);
				}
			}
		}

		/// <summary>
		/// 時間イベント
		/// </summary>
		[SerializeField]
		TimeEvent _time = new TimeEvent();
		public TimeEvent Time { get { return _time; } }
		[System.Serializable]
		public class TimeEvent : IDebugParamEvent
		{
			public event System.Action<int, int> UpdateTime = delegate { };
			[SerializeField]
			bool update = false;
			[SerializeField]
			[Range(0, 23)]
			int hour = 0;
			[SerializeField]
			[Range(0, 59)]
			int minute = 0;

			public void Update()
			{
				if (this.update)
				{
					this.UpdateTime(this.hour, this.minute);
				}
			}
		}

		/// <summary>
		/// 時間フォーマット
		/// </summary>
		[SerializeField]
		TimeFormatEvent _timeFormat = new TimeFormatEvent();
		public TimeFormatEvent TimeFormat { get { return _timeFormat; } }
		[System.Serializable]
		public class TimeFormatEvent : IDebugParamEvent
		{
			public event System.Action<bool> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			bool is24Hour = false;

			public void Update()
			{
				if (this.execute)
				{
					this.execute = false;
					this.Execute(this.is24Hour);
				}
			}
		}
	}
	void DebugInit()
	{
		var d = this.DebugParam;

		d.ExecuteClose += Close;
		d.ExecuteActive += () => { SetActive(true); };
		d.Battery.ExecuteLevelList += () =>
			{
				this.SetBatteryLevelList(this.BatteryLevelList);
			};
		d.Battery.UpdateInfo += (level, state) =>
			{
				level = Mathf.Clamp(level, 0, 100);
				this.SetBatteryInfo(level, state);
			};
		d.Network.ExecuteLevelList += () =>
			{
				this.SetNetworkLevelList(this.NetworkLevelList);
			};
		d.Network.UpdateInfo += (level, type) =>
			{
				level = Mathf.Clamp(level, 0, 100);
				this.SetNetworkInfo(level, type);
			};
		d.Time.UpdateTime += (hour, minute) =>
			{
				var now = DateTime.Now;
				this.SetDateTime(new System.DateTime(now.Year, now.Month, now.Day, hour, minute, now.Second, now.Millisecond));
			};
		d.TimeFormat.Execute += (is24Hour) =>
			{
				this._SetTimeFormat(is24Hour);
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
