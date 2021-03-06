/// <summary>
/// 自滅系サウンド
/// 
/// 2013/09/09
/// </summary>
using UnityEngine;
using System.Collections;

public class SelfDestroySound : MonoBehaviour
{
	#region フィールド＆プロパティ
	private Manager Manager { get; set; }
	private string CueName { get; set; }
	private CriAtomSource Source { get; set; }
	#endregion

	#region 初期化
	public static SelfDestroySound Setup(GameObject go, Manager manager, string cueName)
	{
		// SelfDestroySoundコンポーネント追加.
		SelfDestroySound sound = go.AddComponent<SelfDestroySound>();
		sound.Manager = manager;
		sound.CueName = cueName;

		// CriAtomSourceコンポーネント追加.
		sound.Source = SoundController.AddSeSource(go, cueName);

		return sound;
	}
	#endregion

	#region 更新
	void Update()
	{
		CriAtomSource.Status status = Source.status;
		if ((status == CriAtomSource.Status.Stop) || (status == CriAtomSource.Status.PlayEnd)) {
			// サウンドの再生が終わったら消去.
			Destroy(this.gameObject);
		}
	}
	#endregion

	#region 破棄
	void OnDestroy()
	{
		this.Destroy();
	}
	void Destroy()
	{
		if (this.Manager)
			this.Manager.Destroy(this.gameObject);
		else
			Object.Destroy(this.gameObject);
	}
	#endregion
}
