/// <summary>
/// FingerGesturesのタッチフィルター
/// 
/// 2013/01/18
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIGlobalTouchFilter : MonoBehaviour
{
	#region フィールド＆プロパティ
	private List<int> CurrentTouchIDList{get;set;}
	#endregion

	#region 初期化
	void Awake()
	{
		CurrentTouchIDList = new List<int>();
	}
	#endregion
	#region MonoBehaviourリフレクション
	void OnEnable()
	{
		if (TouchSystem.Instance)
			TouchSystem.Instance.GlobalTouchFilter += OnGlobalTouchFilter;
	}
	void OnDisable()
	{
		if (TouchSystem.Instance)
			TouchSystem.Instance.GlobalTouchFilter -= OnGlobalTouchFilter;
	}
	#endregion

	#region NGUI
	void OnPress(bool isDown)
	{
		int touchID = UICamera.currentTouchID;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
		// NGUI のタッチIDがマウス押下時に -1, -2, -3 を返すのでその対処
		if (touchID < 0)
			touchID = Mathf.Abs(touchID) - 1;
#endif

		if (isDown)
		{
			if( ! this.CurrentTouchIDList.Contains(touchID) )
				this.CurrentTouchIDList.Add(touchID);
		}
		else
		{
			if( this.CurrentTouchIDList.Contains(touchID))
				this.CurrentTouchIDList.Remove(touchID);
		}
	}
	#endregion

	#region FingerGestures
	/// <summary>
	/// タッチフィルター
	/// </summary>
	/// <returns>true=フィルターオフ false=フィルターオン</returns>
	/// <param name="fingerIndex"></param>
	/// <param name="position"></param>
	bool OnGlobalTouchFilter(int fingerIndex, Vector2 position)
	{
		if( 0 < this.CurrentTouchIDList.Count )
		{
			// NGUI でボタンを押されていたらフィルターをオンにする
			if (this.CurrentTouchIDList.Contains(fingerIndex))
				return false;
		}
		return true;
	}
	#endregion
}
