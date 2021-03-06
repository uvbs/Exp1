/// <summary>
/// 移動スティックイベント
/// 
/// 2014/06/21
/// </summary>
using UnityEngine;
using System.Collections;

public class GUIMoveStickEvent : MonoBehaviour
{
	#region フィールド＆プロパティ
	[SerializeField] GUIMoveStick _moveStick;
	public GUIMoveStick MoveStick { get { return _moveStick; } }
	#endregion

	#region NGUIリフレクション
	void OnDrag(Vector2 delta)
	{
		this.MoveStick.OnDrag(delta);
	}
	void OnDragStart()
	{
		this.MoveStick.OnDragStart();
	}
	void OnDragEnd()
	{
		this.MoveStick.OnDragEnd();
	}
	#endregion
}
