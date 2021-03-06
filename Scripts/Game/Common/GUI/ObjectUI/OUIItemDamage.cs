/// <summary>
/// 3Dオブジェクトに対するUIアイテム
/// ダメージ
/// 
/// 2014/06/23
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;

public class OUIItemDamage : OUIItemBase
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
		public GameObject prefab;
		public Transform root;
	}
	#endregion

	#region 作成
	/// <summary>
	/// プレハブ取得
	/// </summary>
	public static OUIItemDamage GetPrefab(GUIObjectUI.AttachObject.Prefab prefab, ObjectBase o)
	{
		// 戦闘中以外は作成しない
		if (ScmParam.Common.AreaType != AreaType.Field)
			return null;
		return prefab.damage.damage;
	}
	#endregion

	#region ダメージ表示
	public void Damage(int damage, DamageType damageType, bool atkisPlayer, bool defisPlayer)
	{
		GUIDamageLetter.CreateDamageLetter(this.Attach.prefab, this.Attach.root.gameObject, damage, damageType, atkisPlayer, defisPlayer);
	}
	#endregion

	#region OUIItemBase override
	public override void Destroy(float timer)
	{
		// ダメージ数値はオブジェクトが消えても数秒間は生きている
		Object.Destroy(this.gameObject, timer);
	}
	protected override void SetActive(bool isActive)
	{
		// ダメージ数値は常に表示
		base.SetActive(true);
	}
	#endregion
}
