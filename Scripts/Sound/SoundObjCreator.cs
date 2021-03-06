/// <summary>
/// SelfDestroySoundを作成する.
/// 
/// 2013/11/05
/// </summary>
using UnityEngine;
using System.Collections;

public class SoundObjCreator : MonoBehaviour
{
	#region フィールド＆プロパティ
	[SerializeField]
	private string cueName;
	[SerializeField]
	private float delayTime;
	#endregion

	#region 初期化
	void Start()
	{
		SoundController.Instance.StartCoroutine(CreateSeCoroutine(this.delayTime, this.transform.position, this.transform.rotation, this.cueName));
	}
	
	IEnumerator CreateSeCoroutine(float delayTime, Vector3 position, Quaternion rotation, string cueName)
	{
		yield return new WaitForSeconds(delayTime);
		
		SoundController.CreateSeObject(position, rotation, cueName);
	}
	#endregion
}
