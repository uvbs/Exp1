/// <summary>
/// 3Dオブジェクトに対するUIアイテム
/// ランキング上位者の特殊アイコン
/// 
/// 2014/06/23
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;

public class OUIItemRanking : OUIItemBase
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
	public static OUIItemRanking GetPrefab(GUIObjectUI.AttachObject.Prefab prefab, ObjectBase o)
	{
		// ロビー以外は作成しない
		if (ScmParam.Common.AreaType != AreaType.Lobby)
			return null;
		return prefab.ranking.ranking;
	}
	#endregion

	#region 更新
	public void UpdateUI()
	{
	}
	#endregion
}
