/// <summary>
/// タイトルシーンでのカメラパラメータを保持する.
/// 
/// 2015/05/29
/// </summary>
using UnityEngine;
using System.Collections;

public class TitleCameraSetting : MonoBehaviour
{
	/// <summary>
	/// 開始時の位置.
	/// </summary>
	[SerializeField]
	private Vector3 startPosition = Vector3.zero;
	public Vector3 StartPosition { get { return startPosition; } }

	/// <summary>
	/// 開始時の向き.
	/// </summary>
	[SerializeField]
	private Vector3 startRotation = Vector3.zero;
	public Vector3 StartRotation { get { return startRotation; } }

	/// <summary>
	/// 回転スピード.
	/// </summary>
	[SerializeField]
	float speed = 5f;
	public float Speed { get { return speed; } }

	/// <summary>
	/// 画角.
	/// </summary>
	[SerializeField]
	float fieldOfView = 50f;
	public float FieldOfView { get { return fieldOfView; } }
}
