/// <summary>
/// NGUIデリゲートサポート
/// 
/// 2014/05/14
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIDelegate : MonoBehaviour
{
	#region フィールド＆プロパティ
	public bool isToggle = false;
	public bool isFalse { get { return false; } }
	public bool isTrue { get { return true; } }
	#endregion

	#region NGUIリフレクション
	public void OnToggle()
	{
		this.isToggle = !this.isToggle;
	}
	public void OnToggleFalse()
	{
		this.isToggle = false;
	}
	public void OnToggleTrue()
	{
		this.isToggle = true;
	}
	public void OnActive()
	{
		this.OnSetActive(true);
	}
	public void OnDeactive()
	{
		this.OnSetActive(false);
	}
	public void OnSetActive(bool active)
	{
		this.gameObject.SetActive(active);
	}
	public void OnButtonEnable()
	{
		this.OnButtonSetEnable(true);
	}
	public void OnButtonDisable()
	{
		this.OnButtonSetEnable(false);
	}
	public void OnButtonSetEnable(bool enable)
	{
		var com = this.gameObject.GetComponentInChildren<UIButton>();
		if (com != null)
			com.isEnabled = enable;
	}
	#endregion

	#region セットアップ
	#endregion
}
