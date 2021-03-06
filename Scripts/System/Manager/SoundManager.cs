/// <summary>
/// サウンドマネージャー
/// 
/// 2013/09/09
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common;
using Scm.Common.GameParameter;

public class SoundManager : Manager
{
	#region フィールド＆プロパティ
	public static SoundManager Instance;

	public LinkedList<GameObject> Link { get; private set; }
	#endregion

	#region 初期化
	void Awake()
	{
		if (Instance == null)
			Instance = this;

		this.Link = new LinkedList<GameObject>();
	}
	protected override void Setup(GameObject go)
	{
		base.Setup(go);

		this.Link.AddLast(go);
	}
	#endregion

	#region 削除
	public override void Destroy(GameObject go)
	{
		this.Link.Remove(go);

		base.Destroy(go);
	}
	#endregion

	#region
	/// <summary>
	/// 音が鳴り終わると自滅する音源オブジェクトを作成する。空文字やnull文字では処理を行わない(nullを返す).
	/// </summary>
	public static GameObject CreateSeObject(Vector3 position, Quaternion rotation, string cueName)
	{
		GameObject go = null;
		if(!string.IsNullOrEmpty(cueName))
		{
			go = new GameObject(cueName);
			go.transform.position = position;
			go.transform.rotation = rotation;
			SelfDestroySound.Setup(go, Instance, cueName);
			
			Instance.Setup(go);
		}
		
		return go;
	}
	#endregion
}
