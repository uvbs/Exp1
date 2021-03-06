/// <summary>
/// 自滅系エフェクト
/// 
/// 2012/12/26
/// </summary>
using UnityEngine;
using System.Collections;

public class SelfDestroyEffect : MonoBehaviour
{
	#region フィールド＆プロパティ
	public Manager Manager { get; private set; }
	public string BundlePath { get; private set; }
	public string FilePath { get; private set; }
	public bool IsPrefabValue { get; private set; }
	#endregion

	#region 初期化
	public static bool Setup(GameObject go, Manager manager, string bundlePath, string fileName, bool isPrefabValue)
	{
		// コンポーネント取得
		SelfDestroyEffect effect = go.GetSafeComponent<SelfDestroyEffect>();
		if (effect == null)
		{
			if (manager)
				manager.Destroy(go);
			else
				Object.Destroy(go);
			return false;
		}
		effect.Manager = manager;
		effect.BundlePath = bundlePath;
		effect.FilePath = fileName;
		effect.IsPrefabValue = isPrefabValue;

		effect.CreateEffect();

		return true;
	}
	#endregion

	#region FilePathのGameObjectを作成.
	/// <summary>
	/// FilePathのGameObject生成.
	/// </summary>
	private void CreateEffect()
	{
		this.enabled = false;	// 読み込み完了までUpdate()を呼ばないようにする.

		if (this.IsPrefabValue)
		{
			ResourceLoad.Instantiate(this.BundlePath, this.FilePath, null, InstantiateCallBack);
		}
		else
		{
			ResourceLoad.Instantiate(this.BundlePath, this.FilePath, this.transform.position, this.transform.rotation, null, InstantiateCallBack);
		}
	}
	/// <summary>
	/// FilePathのGameObject生成後.
	/// </summary>
	private void InstantiateCallBack(GameObject go)
	{
		// 既にHierarchy上に存在しない.
		if(this == null)
		{
			Object.Destroy(go);	// 生成したObjectも破棄する.
			string mes = "this gameobject is already destroyed.";
			Debug.LogError(mes);
			BugReportController.SaveLogFile(mes);
			return;
		}
		// GameObject生成失敗.
		if(go == null)
		{
			this.enabled = true;	// 自滅可能状態にしておく.
			string mes = "Failed to load " + this.FilePath;
			Debug.LogError(mes);
			BugReportController.SaveLogFile(mes);
			return;
		}
		// 生成したGameObjectの設定.
		go.transform.parent = this.transform;
		this.name = go.name;
		this.enabled = true;
	}
	#endregion

	#region 破棄
	void Update()
	{
		if (0 < this.transform.childCount)
			return;

		Object.Destroy(this.gameObject);
	}
	void OnDestroy()
	{
		if (this.Manager)
			this.Manager.Destroy(this.gameObject);
	}
	#endregion
}
