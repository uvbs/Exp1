/// <summary>
/// サイコマ内パラメータモニター
/// IsLock を true にするとモニタリングを停止するので "Set ScmParam" をすることによって書き換え可能になる
/// 
/// 2014/05/27
/// </summary>
using UnityEngine;
using System.Collections;

public class ScmParamMonitor : MonoBehaviour
{
#if UNITY_EDITOR && XW_DEBUG
	#region フィールド&プロパティ
	public bool IsWriteMode;

	public Monitor monitor = new Monitor();
	[System.Serializable]
	public class Monitor
	{
		public ScmParam.NetworkParam Net = new ScmParam.NetworkParam();
		public ScmParam.CommonParam Common = new ScmParam.CommonParam();
		public ScmParam.LobbyParam Lobby = new ScmParam.LobbyParam();
		public ScmParam.BattleParam Battle = new ScmParam.BattleParam();
		public ScmParam.DebugParam Debug = new ScmParam.DebugParam();
	}
	#endregion

	#region 更新
	void Awake()
	{
		this.UpdateMonitor();
	}
	void Update()
	{
		this.UpdateMonitor();
	}
	void UpdateMonitor()
	{
		if (this.IsWriteMode)
		{
			this.monitor.Net = ScmParam.Net;
			this.monitor.Common = ScmParam.Common;
			this.monitor.Lobby = ScmParam.Lobby;
			this.monitor.Battle = ScmParam.Battle;
			this.monitor.Debug = ScmParam.Debug;
		}
		else
		{
			this.monitor.Net = ScmParam.Net.Clone();
			this.monitor.Common = ScmParam.Common.Clone();
			this.monitor.Lobby = ScmParam.Lobby.Clone();
			this.monitor.Battle = ScmParam.Battle.Clone();
			this.monitor.Debug = ScmParam.Debug.Clone();
		}
	}
	#endregion
#else
	void Start()
	{
		// EditorOnlyにしておいて呼ばれないようにする.
		BugReportController.SaveLogFile("Please set EditorOnly tag");
		Object.Destroy(this.gameObject);
	}
#endif
}
