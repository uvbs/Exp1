/// <summary>
/// エフェクトメッセージアイテム
/// アイテムの制御を行うクラス
/// 
/// 2014/07/29
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIEffectMessageItem : GUIEffectMsgItemBase
{
	#region フィールド&プロパティ

	/// <summary>
	/// 表示時間
	/// </summary>
	[SerializeField]
	private float showTime;
	public float ShowTime { get { return showTime; } }

	/// <summary>
	/// 計測用
	/// </summary>
	protected float time;

	#endregion

	#region セットアップ

	public override void Setup(bool isActive)
	{
		SetupBase(isActive);
		// 時間セット
		this.time = this.showTime;
	}

	#endregion

	#region 更新

	protected override void Update ()
	{
		// 時間更新
		TimeUpdate();
	}

	/// <summary>
	/// 時間更新
	/// </summary>
	private void TimeUpdate()
	{
		if(this.IsDelete) return;

		this.time -= Time.deltaTime;
		if(this.time < 0)
		{
			base.IsDelete = true;
		}
	}

	#endregion
}
