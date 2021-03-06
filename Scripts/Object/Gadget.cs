/// <summary>
/// ObjectMasterData を持つ オブジェクトのベースクラス
/// 
/// 2014/08/20
/// </summary>

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Master;
using Scm.Common.GameParameter;
using UnityEngine.Assertions.Must;

public abstract class Gadget : ObjectBase
{
	#region フィールド＆プロパティ
	public ObjectMasterData ObjectData { get; private set; }
	public short InFieldParentId { get; private set; }

	/// <summary>
	/// モデルの描画を行うか否か.
	/// </summary>
	public override bool IsDrawEnable { get { return !IsDisappear; } }
	#endregion

	#region セットアップ
	public static void Setup(GameObject go, Manager manager, ObjectMasterData objectData, EntrantInfo info, AssetReference assetReference = null, bool checkDelate = true)
	{
		Gadget gadget = go.GetComponent<Gadget>();
		if (gadget == null)
		{
			manager.Destroy(go);
			return;
		}

        //Todo: Must Has Simple Way, Refact Later
        //Lee Add For DelateSetUp,Cause Server Send packet Without Logic
	    if (checkDelate)
	    {
	        if (gadget.GetType() == typeof (ResidentArea))
	        {
//	            Debug.Log("===> Find ResidentArea SetUp");
                //keep code order
	            gadget.Setup(manager, objectData, info, assetReference);
	            go.SetActive(false);
	            DelateSetUp.Add(info.InFieldId, () =>
	            {
	                Setup(go, manager, objectData, info, assetReference, false);
	            });
                ResidentArea.CacheResidentArea(gadget as ResidentArea);
                return;
	        }
            //...else type
            gadget.Setup(manager, objectData, info, assetReference);
	    }
	    //End
        
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || VIEWER
		GameGlobal.AddSpherePolygon(go.transform, go.GetComponentInChildren<SphereCollider>(), true);
		GameGlobal.AddCubePolygon(go.transform, go.GetComponentInChildren<BoxCollider>(), true);
		gadget.CheckType();
#endif
		gadget.SetupCompleted();
	}

    //For Delate SetUp Object
    public static Dictionary<int, Action> DelateSetUp= new Dictionary<int, Action>();
    //End

	protected override string GetObjectUIPath()
	{
		if (this.ObjectData == null)
			return null;

		// ObjectUIテーブルを取得する
		ObjectUserInterfaceMasterData data;
		if (!MasterData.TryGetObjectUI(this.ObjectData.UserInterfaceId, out data))
			return null;

		return GameGlobal.GetObjectUIPath(data);
	}
	/// <summary>
	/// EntrantTypeとアタッチされているスクリプトの対応が正しいかチェックする.エディタかXW_DEBUGビルド専用.
	/// </summary>
	[System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("XW_DEBUG")]
	private void CheckType()
	{
		switch(this.EntrantType)
		{
		case EntrantType.MainTower:		if(this is MainTower){ return; } break;
		case EntrantType.SubTower:		if(this is SubTower){ return; } break;
		case EntrantType.Jump:			if(this is WarpBase){ return; } break;
		case EntrantType.Transporter:	if(this is Transporter){ return; } break;
		case EntrantType.Tank:			if(this is TankBase){ return; } break;
		case EntrantType.Wall:			if(this is WallBase){ return; } break;
		case EntrantType.Barrier:		if(this is Barrier){ return; } break;
		//case EntrantType.Start:			if(this is Start){ return; } break;
		case EntrantType.Respawn:		if(this is RespawnBase){ return; } break;
		case EntrantType.FieldPortal:	if(this is BattlePortal){ return; } break;
		case EntrantType.RankingPortal:	if(this is RankingPortal){ return; } break;
        case EntrantType.Hostage:       if(this is Hostage) { return; } break;
        case EntrantType.ResidentArea:  if (this is ResidentArea) { return; } break;
        }
		Debug.LogError("EntrantType doesn't accord! ID="+this.Id+", Type="+this.EntrantType+", "+this.ToString());
	}
	#endregion

	#region 初期化
	protected virtual void Setup(Manager manager, ObjectMasterData objectData, EntrantInfo info, AssetReference assetReference)
	{
		base.SetupBase(manager, info);

		this.InFieldParentId = info.InFieldParentId;

		this.SetMainAssetBundle(assetReference);
		this.ObjectData = objectData;
		this.IsBreakable = objectData.IsBreakable;

		// 出現エフェクト.
		this.CreatePopEffect();

		List<ObjectSkillBulletSetMasterData> oSBSDataList;
		if(ObjectSkillBulletSetMaster.Instance.TryGetSkillBulletSet(objectData.ID, out oSBSDataList))
		{
			foreach(var oSBSData in oSBSDataList)
			{
				this.CreateLocalSkillBullet(oSBSData);
			}
		}
	}
	protected void CreateLocalSkillBullet(IBulletSetMasterData bulletSet)
	{
		StartCoroutine(LocalSkillBulletFiber(bulletSet));
	}
	private IEnumerator LocalSkillBulletFiber(IBulletSetMasterData bulletSet)
	{
		if(0 < bulletSet.ShotTiming)
		{
			yield return new WaitForSeconds(bulletSet.ShotTiming);
		}

		Vector3 position = this.transform.position;
		Quaternion rotation = this.transform.rotation;
		// 弾丸補正.
		GameGlobal.AddOffset(bulletSet, ref position, ref rotation, Vector3.one);
		
		this.CreateBullet(null, null, this.transform.position, this.transform.rotation, 0, bulletSet);
	}
	#endregion

	#region ObjectBase Override
	#region 生成＆破壊時エフェクト.
	protected override void CreatePopEffect()
	{
		this.CreateEffect(ObjectData.BirthFile);
		this.CreateSe(ObjectData.BirthSeFile);
	}
	protected override void CreateBrokenEffect()
	{
		this.CreateEffect(ObjectData.LostFile);
		this.CreateSe(ObjectData.LostSeFile);
	}
	#endregion
	#region 削除
	/// <summary>
	/// 削除を行う.
	/// </summary>
	protected override void Destroy()
	{
        //Remove The Action
	    DelateSetUp.Remove(InFieldId);
		this.CreateBrokenEffect();

		// 専用破壊スクリプトが存在するなら専用の破壊処理を行う
		GadgetDestroyBase gadgetDestroy = this.gameObject.GetComponent<GadgetDestroyBase>();
		if(gadgetDestroy != null)
		{
			gadgetDestroy.Destroy(this);
		}

		base.Destroy();
		base.Remove();
	}
	/// <summary>
	/// 削除されたときに呼ばれる(シーンチェンジによる破棄などでも呼ばれる).
	/// </summary>
	//protected override void OnDestroy();
	#endregion
	#endregion

	#region 召喚NPC判定
	public bool IsChildOfPlayer()
	{
		Player player = GameController.GetPlayer();
		if(player != null && player.InFieldId == this.InFieldParentId)
		{
			return true;
		}
		return false;
	}
	public bool IsIndependentNpc()
	{
		return this.InFieldParentId == 0;
	}
	#endregion
}
