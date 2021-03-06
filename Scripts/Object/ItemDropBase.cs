/// <summary>
/// アイテムベース.
/// 
/// 2013/05/016
/// </summary>
using UnityEngine;
using System.Collections;

using Scm.Common.Master;
using Scm.Common.GameParameter;

public class ItemDropBase : ObjectBase
{
	#region 定数
	private const string GetEffectFileName = "Common/item_hit_001";
	private const int ResetCoolTime = 2;							// 仮)クールタイム回復.
	#endregion

	#region フィールド＆プロパティ
	public ItemMasterData ItemData { get; set; }
	protected bool IsHit { get; set; }

	/// <summary>
	/// モデルの描画を行うか否か.
	/// </summary>
	public override bool IsDrawEnable { get { return true; } }
	#endregion

	#region セットアップ
	public static void Setup(GameObject go, Manager manager, ItemMasterData itemData, ItemDropInfo info)
	{
		ItemDropBase item = go.GetComponent<ItemDropBase>();
		if (item == null)
		{
			manager.Destroy(go);
			return;
		}
		item.SetupBase(manager, info);
		item.ItemData = itemData;

		item.SetupCompleted();
	}
	protected override string GetObjectUIPath()
	{
		if (this.ItemData == null)
			return null;

		// ObjectUIテーブルを取得する
		ObjectUserInterfaceMasterData data;
		if (!MasterData.TryGetObjectUI(this.ItemData.UserInterfaceId, out data))
			return null;

		return GameGlobal.GetObjectUIPath(data);
	}
	#endregion

	#region 削除
	protected override void Destroy()
	{
		base.Destroy();
		base.Remove();
	}
	public virtual void DestroyAnimation()
	{
		this.Destroy();
	}
	#endregion

	#region ObjectBase Override
	#region アイテムゲットパケット
	/// <summary>
	/// アイテム取得処理.
	/// </summary>
	public void ItemGet(ObjectBase user, ItemSpecialEffect specialEffect)
	{
		// アイテム効果.
		this.ItemEffect(user, specialEffect);

		// エフェクト生成.
		Transform transform;
		transform = user.transform.Search(BoneName.GetName(BoneType.Head));
		CreateItemGetEffect(transform.position, transform.rotation);

		// アイテム削除.
		this.Destroy();
	}
	#endregion
	#endregion

	#region アイテム取得関連.
	/// <summary>
	/// アイテムエフェクト生成.
	/// </summary>
	protected void CreateItemGetEffect(Vector3 position, Quaternion rotation)
	{
		EffectManager.CreateSelfDestroy(position, rotation, GetEffectFileName, false);
	}
	
	/// <summary>
	/// アイテム効果処理.
	/// </summary>
	private void ItemEffect(ObjectBase user, ItemSpecialEffect specialEffect)
	{
		if(user is Player)
		{
			// アイテム取得者のみ.
			if(specialEffect == ItemSpecialEffect.ClearSkillCD)
			{
				GUISkill.ClearCoolTime();
			}
		}
	}
	#endregion
}