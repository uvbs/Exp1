/// <summary>
/// エフェクトメッセージアイテムのインターフェイス&基底クラス
/// 
/// 2014/12/05
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// エフェクトメッセージアイテムのインターフェイス
/// </summary>
public interface IEffectMessageItem
{
	/// <summary>
	/// 削除するかどうかのフラグ
	/// </summary>
	bool IsDelete { get; set; }
	
	/// <summary>
	/// Activeのセット
	/// </summary>
	void SetActive(bool isActive);

	/// <summary>
	/// 削除機能
	/// </summary>
	void Delete();
}


/// <summary>
/// エフェクトメッセージアイテムの基底クラス
/// アイテムをGUIEffectMessageControllerやGUIEffectMessageCategoryで制御
/// 管理する場合はこの基底クラスを継承してアイテムクラスを作成する
/// </summary>
public class GUIEffectMsgItemBase : MonoBehaviour, IEffectMessageItem
{
	#region アタッチオブジェクト
	
	[System.Serializable]
	public class AttachObject
	{
		// GUIEffectMessageItem以外のゲームオブジェクトを生成する場合はこのリストに登録する
		public List<GameObject> subPrefabList = new List<GameObject>();
	}
	
	#endregion

	#region フィールド&プロパティ
	
	/// <summary>
	/// エフェクトメッセージの種類
	/// </summary>
	[SerializeField]
	private GUIEffectMessage.MsgType msgType;
	public GUIEffectMessage.MsgType MsgType { get { return msgType; } }

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	private AttachObject attach;
	protected AttachObject Attach { get { return attach; } }

	/// <summary>
	/// 削除フラグ
	/// </summary>
	public bool IsDelete { get; set; }

	/// <summary>
	/// サブゲームオブジェクト
	/// GUIEffectMessageItem以外のゲームオブジェクトを
	/// 生成する場合はこのリストに登録する
	/// </summary>
	private List<GameObject> subObjectList = new List<GameObject>();

	#endregion

	#region 生成
	
	/// <summary>
	/// GUIEffectMsgItemBasの生成
	/// </summary>
	public static GUIEffectMsgItemBase Create(GUIEffectMsgItemBase itemResource, List<GameObject> parentList, bool isActive)
	{
		if(itemResource == null)
			return null;
		
		// アイテム生成
		GUIEffectMsgItemBase newItem = SafeObject.Instantiate(itemResource) as GUIEffectMsgItemBase;
		if(newItem == null)
			return null;
		
		// 生成したアイテムの親子付け
		AddParent(itemResource.gameObject, newItem.gameObject, parentList);
		
		// サブオブジェクトの生成
		foreach(GameObject objResource in newItem.Attach.subPrefabList)
		{
			// 生成し親子付を行う
			GameObject newObject = SafeObject.Instantiate(objResource) as GameObject;
			AddParent(objResource, newObject, parentList);
			// 生成したオブジェクトをリストに登録
			newItem.subObjectList.Add(newObject);
		}
		
		// セットアップ
		newItem.Setup(isActive);
		
		return newItem;
	}
	
	/// <summary>
	/// 生成したゲームオブジェクトの親子付けを行う
	/// </summary>
	protected static bool AddParent(GameObject prefab, GameObject item, List<GameObject> parentList)
	{
		GameObject parent = null;
		// 親子付けする同レイヤーのオブジェクトを検索する
		foreach(GameObject obj in parentList)
		{
			if(item.layer == obj.layer)
			{
				parent = obj;
				break;
			}
		}
		
		// 親子付け
		if(parent == null)
			return false;
		
		item.transform.parent = parent.transform;
		item.transform.localPosition = prefab.transform.localPosition;
		item.transform.localScale = Vector3.one;
		item.transform.localRotation = Quaternion.identity;
		
		return true;
	}
	
	#endregion

	#region セットアップ

	public virtual void Setup(bool isActive)
	{
		SetupBase(isActive);
	}

	protected void SetupBase(bool isActive)
	{
		// 表示の設定
		SetActive(isActive);
		this.IsDelete = false;
	}
	
	#endregion

	#region アクティブ
	
	public void SetActive(bool isActive)
	{
		this.gameObject.SetActive(isActive);
		foreach(GameObject obj in this.subObjectList)
		{
			obj.SetActive(isActive);
		}
	}
	
	#endregion

	#region 更新
	protected virtual void Update (){}
	#endregion

	#region 削除

	public virtual void Delete()
	{
		DeleteBase();
	}

	protected void DeleteBase()
	{
		// 自身のオブジェクトとサブオブジェクトの削除
		GameObject.Destroy(this.gameObject);
		foreach(GameObject obj in this.subObjectList)
		{
			GameObject.Destroy(obj);
		}
		// Hieralchy上のGameObjectが削除されたのでリスト内のオブジェクトも削除しておく
		this.subObjectList.Clear();
	}
	
	#endregion
}