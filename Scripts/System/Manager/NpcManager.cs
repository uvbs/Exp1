/// <summary>
/// NPCマネージャー
/// 
/// 2012/12/26
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.Master;
using Scm.Common.GameParameter;

public class NpcManager : Manager
{
	#region フィールド＆プロパティ
	public static NpcManager Instance;

	private Dictionary<int, Npc> Dict { get; set; }

	[SerializeField]
	private GameObject fontPrefab;
	public  GameObject FontPrefab { get{ return fontPrefab;} }
	[SerializeField]
	private GameObject gaugePrefab;
	public  GameObject GaugePrefab { get{ return gaugePrefab;} }
	[SerializeField]
	private GameObject shadowPrefab;
	public  GameObject ShadowPrefab { get{ return shadowPrefab;} }
	#endregion

	#region 初期化
	void Awake()
	{
		if (Instance == null)
			Instance = this;

		this.Dict = new Dictionary<int, Npc>();
	}
	protected override void Setup(GameObject go)
	{
		base.Setup(go);

		Npc newNpc = go.GetSafeComponent<Npc>();
		if (newNpc)
		{
			if(this.Dict.ContainsKey(newNpc.InFieldId))
			{
				if(this.Dict[newNpc.InFieldId] != null)
				{
					// 同じInFieldIdのNpcが存在する.
					NetworkController.SaveConflictIdLog(this.Dict[newNpc.InFieldId], newNpc);
					this.Dict[newNpc.InFieldId].Remove();
				}
			}
			this.Dict[newNpc.InFieldId] = newNpc;
		}
		else
		{
			Debug.LogWarning(string.Format("Npc({0}) コンポーネントが見つからない", go.name));
		}
	}
	#endregion

	#region 削除
	public override void Destroy(GameObject go)
	{
		Npc npc = go.GetSafeComponent<Npc>();
		if(npc != null)
		{
			this.Remove(npc);
		}
		base.Destroy(go);
	}
	private void Remove(Npc npc)
	{
		Npc outNpc;
		if (this.Dict.TryGetValue(npc.InFieldId, out outNpc))
		{
			if(npc == outNpc)
			{
				this.Dict.Remove(npc.InFieldId);
			}
		}
	}
	#endregion

	#region 作成
	public bool Create(NpcInfo info)
	{
		// 存在していない.
		if (!info.IsInArea)
			return false;
		// 死亡している
		if (info.StatusType == StatusType.Dead)
			return false;

		string npcName = string.Empty;

		ObjectMasterData objectData;
		if (MasterData.Instance != null)
		{
			if (MasterData.TryGetObject(info.Id, out objectData))
			{
				if (!string.IsNullOrEmpty(objectData.Filename[(int)info.TeamType.GetClientTeam()]))
				{
					npcName = objectData.Filename[(int)info.TeamType.GetClientTeam()];
					info.UserName = objectData.Name;
				}
				else
				{
					// プレハブ名が無効
					BugReportController.SaveLogFile("prefab name is Empty. objectdata id = " + info.Id);
					return false;
				}
			}
			else
			{
				// オブジェクトデータがない
				BugReportController.SaveLogFile("not found objectdata id = " + info.Id);
				return false;
			}
		}
		else
		{
			// マスターデータがない
			BugReportController.SaveLogFile("MasterData.Instance = null");
			return false;
		}

		// 生成.
		AssetReference assetReference = AssetReference.GetAssetReference(objectData.AssetPath);
		StartCoroutine(assetReference.GetAssetAsync<GameObject>(NpcName.GetCharacterPath(npcName), (GameObject resource) =>
			{
				GetAssetCallBack(resource, objectData, info, assetReference);
			}
		));

		return true;
	}

	// アセットロード後の処理.
	private void GetAssetCallBack(GameObject resource, ObjectMasterData objectData, NpcInfo info, AssetReference assetReference)
	{
		StartCoroutine(InstantiateCoroutine(resource, objectData, info,assetReference));
	}
	private IEnumerator InstantiateCoroutine(GameObject resource, ObjectMasterData objectData, NpcInfo info, AssetReference assetReference)
	{
		while(!MapManager.Instance.MapExists)
		{
			// マップの読み込みが終わっていないので待つ.
			yield return null;
		}

		// 読み込み中にいなくなっていたら生成しない.
		if (Entrant.Exists(info))
		{
            this.Instantiate(resource, info.StartPosition, Quaternion.Euler(0f, info.StartRotation, 0f), (GameObject go) => {
                Npc.Setup(go, this, objectData, info, assetReference);
            });
        }
    }
	#endregion
}
