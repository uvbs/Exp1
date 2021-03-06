/// <summary>
/// ローディング画面
/// 
/// 2013/08/22
/// </summary>
using UnityEngine;
using System.Collections;

public class GUILoading : MonoBehaviour
{
	#region フィールド＆プロパティ
	static private GUILoading instance;
	#endregion

	#region 初期化
	void Awake()
	{
		// インスタンス設定
		if (instance == null)
		{
			instance = this;
		}
	}
	public static void SetActive(bool active)
	{
		if (instance == null)
			{ return; }
		instance.gameObject.SetActive(active);
	}
	#endregion

}
