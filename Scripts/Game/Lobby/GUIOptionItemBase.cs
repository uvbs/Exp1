/// <summary>
/// オプションベースアイテム
/// 
/// 2015/01/08
/// </summary>
using UnityEngine;
using System.Collections;

public abstract class GUIOptionItemBase : MonoBehaviour
{
	#region 初期化
	protected static GameObject CreateBase(GameObject prefab, Transform parent, int itemIndex)
	{
		// インスタンス化
		var go = SafeObject.Instantiate(prefab) as GameObject;
		if (go == null)
			return null;

		// 名前
		go.name = string.Format("{0:00}_{1}", itemIndex, prefab.name);
		// 親子付け
		go.transform.parent = parent;
		go.transform.localPosition = prefab.transform.localPosition;
		go.transform.localRotation = prefab.transform.localRotation;
		go.transform.localScale = prefab.transform.localScale;
		// 可視化
		go.SetActive(true);

		return go;
	}
	#endregion
}
