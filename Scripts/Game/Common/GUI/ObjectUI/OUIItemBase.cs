/// <summary>
/// 3Dオブジェクトに対するUIアイテムベース
/// 
/// 2014/06/22
/// </summary>
using UnityEngine;
using System.Collections;

public abstract class OUIItemBase : MonoBehaviour
{
	#region フィールド＆プロパティ
	protected OUIItemRoot ItemRoot { get; private set; }
	bool IsInRangeDraw { get; set; }
	#endregion

	#region 作成
	public static OUIItemBase Create(OUIItemRoot itemRoot, OUIItemBase prefab, Transform parent, bool isInRangeDraw, Vector3 offset)
	{
		var com = SafeObject.Instantiate(prefab) as OUIItemBase;
		if (com == null)
		{
			Debug.LogWarning("OUIItemBase.Create: Not Found OUIItemBase");
			return null;
		}
		com.gameObject.SetActive(true);

		// 親子付け
		var t = com.transform;
		t.parent = parent;
		t.localPosition = Vector3.zero;
		t.localRotation = Quaternion.identity;
		t.localScale = Vector3.one;

		// 設定
		com.name = prefab.name;
		com.ItemRoot = itemRoot;
		com.IsInRangeDraw = isInRangeDraw;
		com.transform.localPosition = offset;

		return com;
	}
	#endregion

	#region 削除
	public virtual void Destroy(float timer)
	{
		Object.Destroy(this.gameObject);
	}
	#endregion

	#region アクティブ設定
	public virtual void SetActive(bool isLockon, bool isInRange)
	{
		bool isActive = false;
		if (isLockon)
		{
			// ロックオン時は常に表示する
			isActive = true;
		}
		else if (this.IsInRangeDraw)
		{
			// プレイヤーが範囲内に入っていた時に表示するかしないか
			isActive = isInRange;
		}
		this.SetActive(isActive);
	}
	protected virtual void SetActive(bool isActive)
	{
		this.gameObject.SetActive(isActive);
	}
	#endregion
}
