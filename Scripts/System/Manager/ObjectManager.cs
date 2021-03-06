/// <summary>
/// オブジェクトマネージャー
/// 
/// 2013/02/19
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Master;
using Scm.Common.GameParameter;

public class ObjectManager : Manager
{
	#region フィールド＆プロパティ
	public static ObjectManager Instance;

	private Dictionary<int, ObjectBase> Dict { get; set; }
	public int Count { get{ return Dict.Count; } }
	#endregion

	#region 初期化
	void Awake()
	{
		if (Instance == null)
			Instance = this;

		this.Dict = new Dictionary<int, ObjectBase>();
	}
	protected override void Setup(GameObject go)
	{
		base.Setup(go);

		ObjectBase com = go.GetComponent<ObjectBase>();
		if (com)
		{
			if(this.Dict.ContainsKey(com.InFieldId))
			{
				if(this.Dict[com.InFieldId] != null)
				{
					// 同じInFieldIdのObjectBaseが存在する.
					NetworkController.SaveConflictIdLog(this.Dict[com.InFieldId], com);
					this.Dict[com.InFieldId].Remove();
				}
			}
			this.Dict[com.InFieldId] = com;
		}
		else
		{
			Debug.LogWarning(string.Format("ObjectBase({0}) コンポーネントが見つからない", go.name));
		}
	}
	#endregion

	#region 削除
	public override void Destroy(GameObject go)
	{
		ObjectBase com = go.GetComponent<ObjectBase>();
		if (com)
		{
			this.Remove(com);
		}

		base.Destroy(go);
	}
	private void Remove(ObjectBase objectBase)
	{
		ObjectBase outObj;
		if (this.Dict.TryGetValue(objectBase.InFieldId, out outObj))
		{
			if(objectBase == outObj)
			{
				this.Dict.Remove(objectBase.InFieldId);
			}
		}
	}
	#endregion

	#region 作成
	public bool CreateObject(EntrantInfo info)
	{
		// 存在していない.
		if (!info.IsInArea)
			return false;

		// オブジェクトデータを取得
		ObjectMasterData objectData;
		if (MasterData.TryGetObject(info.Id, out objectData))
		{
			return this.CreateObject(objectData, info, (GameObject go) => {});
		}
		return false;
	}
	public bool CreateItem(ItemDropInfo info)
	{
		// アイテムデータを取得.
		ItemMasterData itemData = null;
		if(ItemMaster.Instance.TryGetMasterData(info.Id, out itemData))
		{
			return this.CreateItem(itemData, info, (GameObject go) => {});
		}
		return false;
	}
	public bool CreateObject(ObjectMasterData objectData, EntrantInfo info, System.Action<GameObject> callback)
	{
		// 存在していない.
		if (!info.IsInArea)
			return false;
		// 死亡している
		if (info.StatusType == StatusType.Dead)
			return false;

		// プレハブ名が無効
		if (string.IsNullOrEmpty(objectData.Filename[(int)info.TeamType.GetClientTeam()]))
			return false;
		string path = GameConstant.ObjectPath.AssetPath + objectData.Filename[(int)info.TeamType.GetClientTeam()];
		info.UserName = objectData.Name;

		// インスタンス化
		Vector3 position = info.StartPosition;
		Quaternion rotation = Quaternion.Euler(0f, info.StartRotation, 0f);

		AssetReference assetReference = AssetReference.GetAssetReference(GameConstant.ObjectPath.BundlePath);
		this.StartCoroutine(assetReference.GetAssetAsync<GameObject>(path + ".prefab", (GameObject resource) =>
		{
			if (Entrant.Exists(info))
			{
				this.Instantiate(resource, position, rotation,
					(GameObject go) => { callback(go); Gadget.Setup(go, this, objectData, info);}
				);
			}
		}));

		return true;
	}
	public bool CreateItem(ItemMasterData itemData, ItemDropInfo info, System.Action<GameObject> callback)
	{
		// プレハブ名が無効.
		if(string.IsNullOrEmpty(itemData.ItemFileName))
			return false;
		string path = GameConstant.ObjectPath.AssetPath + itemData.ItemFileName;
		info.UserName = itemData.Name;

		// インスタンス化
		Vector3 position = info.StartPosition;
		Quaternion rotation = Quaternion.Euler(0f, info.StartRotation, 0f);

		ObjectBase obj;
		if(this.Dict.TryGetValue(info.InFieldId, out obj))
		{
			// InFieldIdが被った場合,古いIDを削除し,新しいIDを割り当てる.
			string msg;
			ItemDrop item = obj as ItemDrop;
			if(item == null)
			{
				msg = string.Format("NotFound ItemDropObject.");
				Debug.LogWarning(msg);
				BugReportController.SaveLogFileWithOutStackTrace(msg);
				return false;
			}
			Destroy(item.gameObject);
			
			// メッセージ表示.
			msg = string.Format("Same ItemDrop exists. InFieldId = {0}.", info.InFieldId);
			Debug.LogWarning(msg);
			BugReportController.SaveLogFileWithOutStackTrace(msg);
		}
		AssetReference assetReference = AssetReference.GetAssetReference(GameConstant.ObjectPath.BundlePath);
		this.StartCoroutine(assetReference.GetAssetAsync<GameObject>(path + ".prefab", (GameObject resource) =>
		{
			if (Entrant.Exists(info))
			{
				this.Instantiate(resource, position, rotation,
					(GameObject go) => {callback(go); ItemDropBase.Setup(go, this, itemData, info);}
				);
			}
		}));

		return true;
	}
	#endregion

	#region デバッグ
#if UNITY_EDITOR && XW_DEBUG
	[SerializeField]
	DebugParam _debug;
	[System.Serializable]
	public class DebugParam
	{
		public bool execute;
		public EntrantInfo entrantInfo;
	}
	void DebugUpdate()
	{
		var t = this._debug;
		if (t.execute)
		{
			t.execute = false;
			t.entrantInfo.CreateObject();
		}
	}
	void OnValidate()
	{
		this.DebugUpdate();
	}
#endif
	#endregion
}
