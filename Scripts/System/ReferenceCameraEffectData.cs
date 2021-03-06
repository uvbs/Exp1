/// <summary>
/// ReferenceEffectDataのカメラ用
/// 
/// 2012/11/27
/// </summary>

using UnityEngine;
using System.Collections;

public class ReferenceCameraEffectData : ReferenceEffectData 
{
	#region フィールド＆プロパティ
	GameObject cameraEffect;
	#endregion

	#region コンストラクタ＆デストラクタ
	protected override void Awake() { }	// 自動生成しない
	#endregion

	#region Unityリフレクション
	void OnDestroy()
	{
		if(cameraEffect != null)
		{
			GameObject.Destroy(cameraEffect);
		}
	}
	#endregion

	#region Method
	protected override void Setup(GameObject go, Transform attach)
	{
		CharacterCamera cc = GameController.CharacterCamera;
		if(cc != null)
		{
			cameraEffect = go;
			// 親子付け
			go.transform.parent = cc.transform;
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
		}
	}
	#endregion
}
