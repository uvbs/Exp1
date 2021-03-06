/// <summary>
/// タワーゲージ
/// 
/// 2013/03/14
/// </summary>
using UnityEngine;
using System.Collections;

public class GUITowerGauge : MonoBehaviour
{
	#region フィールド＆プロパティ
	// 揺れ時間.
	const float ShakeTime = 1.0f;
	// 揺れ幅.
	const float Shake = 2.0f;
	// 揺れスピード.
	const float ShakeSpeed = 2.0f;

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public GameObject gaugeRoot;
		public UISprite gaugeSprite;
		public GameObject destructionGroup;
	}

	// 現在揺れている時間.
	float ShakeCount { get; set; }
	// 揺れファイバー
	IEnumerator ShakeFiber { get; set; }

	void MemberInit()
	{
		this.ShakeCount = 0f;
		this.ShakeFiber = null;
	}
	#endregion

	#region 初期化
	void Start()
	{
		this.MemberInit();

		this.SetActive(false);
	}
	public void SetActive(bool isActive)
	{
		this.Attach.gaugeRoot.SetActive(isActive);
	}
	#endregion

	#region 更新
	public void UpdateGauge(int point, int maxPoint)
	{
		this.SetActive(true);

		//ゲージ更新
		float fillAmount = 0f;
		if (0 < maxPoint)
			fillAmount = (float)point / (float)maxPoint;
		this.Attach.gaugeSprite.fillAmount = fillAmount;

		// 破壊状態表示
		bool isActive = (0 >= point);
		this.Attach.destructionGroup.SetActive(isActive);
	}
	#endregion

	#region 揺れ演出.
	void Update()
	{
		if (this.ShakeFiber != null)
			this.ShakeFiber.MoveNext();
	}
	public void ShakeGauge()
	{
		this.ShakeCount = ShakeTime;
		if (this.ShakeFiber != null)
		{
			return;
		}
		// OnValidate 内から呼び出すとコルーチンが停止してしまうためファイバーに変更
		this.ShakeFiber = ShakeCoroutine();
	}
	IEnumerator ShakeCoroutine()
	{
		Vector3 startPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, this.transform.localPosition.z);
		float shakeSin;
		float shakeCos;
		while (this.ShakeCount > 0)
		{
			this.ShakeCount -= Time.deltaTime;
			shakeSin = Mathf.Sin(Time.frameCount * ShakeSpeed) * Shake;
			shakeCos = Mathf.Cos(Time.frameCount * ShakeSpeed) * Shake;
			this.transform.localPosition = new Vector3(startPosition.x + shakeCos, startPosition.y + shakeSin, startPosition.z);
			yield return 0;
		}
		this.transform.localPosition = startPosition;
		this.ShakeFiber = null;
	}
	#endregion
}
