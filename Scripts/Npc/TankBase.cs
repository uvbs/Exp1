/// <summary>
/// タンクベース
/// 
/// 2013/03/08
/// </summary>
using UnityEngine;
using Scm.Common.Master;

public class TankBase : NpcBase
{
	#region フィールド＆プロパティ

	// 経験値処理
	protected override Vector3 ExpEffectOffsetMin{ get{ return new Vector3(0.0f, 1.5f, 0.0f); } }
	protected override Vector3 ExpEffectOffsetMax{ get{ return new Vector3(0.0f, 1.5f, 0.0f); } }

	#endregion
}
