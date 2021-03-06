/// <summary>
/// 3Dオブジェクトに対するUIアイテム
/// 状態アイコン
/// 
/// 2014/06/23
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;

public class OUIItemStatus : OUIItemBase
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
		public UISprite sprite;
	}
	#endregion

	#region 作成
	/// <summary>
	/// プレハブ取得
	/// </summary>
	public static OUIItemStatus GetPrefab(GUIObjectUI.AttachObject.Prefab prefab, ObjectBase o)
	{
		// ロビー以外は作成しない
		if (ScmParam.Common.AreaType != AreaType.Lobby)
			return null;
		return prefab.status.status;
	}
	#endregion

	#region 更新
	public void UpdateUI()
	{
	}
	#endregion
}
