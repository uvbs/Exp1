/// <summary>
/// ボタン無効スクリプト
/// 
/// 2014/12/22
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIButtonDisable : MonoBehaviour
{
	#region フィールド＆プロパティ
	[SerializeField]
	List<ButtonSetting> _buttonList = new List<ButtonSetting>();
	List<ButtonSetting> ButtonList { get { return _buttonList; } }
	[System.Serializable]
	public class ButtonSetting
	{
		public UIButton button = null;
		public bool disable = true;
		public ButtonSetting()
		{
			this.button = null;
			this.disable = true;
		}
	}
	#endregion

	#region MonoBehaviourリフレクション
	void Start()
	{
		this.Execute();
	}
	void Execute()
	{
		if (this.ButtonList != null)
		{
			foreach (var s in this.ButtonList)
			{
				if (s.button == null)
					continue;
				s.button.isEnabled = !s.disable;
			}
		}
	}
	#endregion

	#region デバッグ
#if UNITY_EDITOR && XW_DEBUG
	[SerializeField]
	DebugParameter _debugParam = new DebugParameter();
	DebugParameter DebugParam { get { return _debugParam; } set { _debugParam = value; } }
	[System.Serializable]
	public class DebugParameter
	{
		public bool isExecute;
	}
	void DebugUpdate()
	{
		var t = this.DebugParam;
		if (t.isExecute)
		{
			t.isExecute = false;
			this.Execute();
		}
	}
	void OnValidate()
	{
		this.DebugUpdate();
	}
#endif
	#endregion
}
