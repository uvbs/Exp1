/// <summary>
/// 押下中に登録されているイベントデリゲートを呼び出す処理
/// 
/// 2014/10/21
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Designer/NGUI/XUIHoldPressTrigger")]
public class XUIHoldPressTrigger : MonoBehaviour
{
	#region フィールド&プロパティ

	/// <summary>
	/// 現在適用されているスクリプト
	/// </summary>
	static public XUIHoldPressTrigger current;

	/// <summary>
	/// 押下中に呼び出すイベントデリゲート間隔
	/// </summary>
	public float interval = 0.2f;

	/// <summary>
	/// 押下状態中に呼び出すイベントデリゲートリスト
	/// </summary>
	public List<EventDelegate> onHoldPress = new List<EventDelegate>();

	/// <summary>
	/// 押下状態
	/// </summary>
	public bool Pressed { get; private set; }

	/// <summary>
	/// 時間計測用
	/// </summary>
	private float timer;

	#endregion

	#region 初期化

	void Start()
	{
		this.Pressed = false;
	}

	#endregion

	#region 押下時

	void OnPress (bool pressed)
	{
		if (current != null) return;
		current = this;
		this.Pressed = pressed;
		if(pressed)
		{
			// 押された直後は呼び出す
			EventDelegate.Execute(onHoldPress);
			this.timer = 0;
		}
		current = null;
	}

	#endregion

	#region 更新

	void Update()
	{
		if(!this.Pressed) return;

		// 設定されている間隔ごとにイベントデリゲートを呼び出す
		this.timer += Time.deltaTime;
		if(this.timer > this.interval)
		{
			this.timer = 0;
			EventDelegate.Execute(this.onHoldPress);
		}
	}

	#endregion
}
