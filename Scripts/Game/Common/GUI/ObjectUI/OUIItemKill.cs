/// <summary>
/// 3Dオブジェクトに対するUIアイテム
/// キル数
/// 
/// 2014/06/23
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;

public class OUIItemKill : OUIItemBase
{
	#region フィールド＆プロパティ
	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField] AttachObject _attach;
	public AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UILabel label;
	}
	#endregion

	#region 作成
	/// <summary>
	/// プレハブ取得
	/// </summary>
	public static OUIItemKill GetPrefab(GUIObjectUI.AttachObject.Prefab prefab, ObjectBase o)
	{
		// 戦闘中以外は作成しない
		if (ScmParam.Common.AreaType != AreaType.Field)
			return null;
		return prefab.kill.kill;
	}
	#endregion

	#region 更新
	public void UpdateUI(int killCount)
	{
		this.Attach.label.text = killCount.ToString();
	}
	#endregion
}
