/// <summary>
/// マップの中心点(0,0,0)でカメラを回転するスクリプト.
/// .
/// 2013/03/25.
/// </summary>
using UnityEngine;
using System.Collections;

public class ResultRotateCamera : MonoBehaviour
{
	/// <summary>
	/// 開始時の位置.
	/// </summary>
	[SerializeField]
	private Vector3 startPosition = Vector3.zero;
	
	/// <summary>
	/// 開始時の向き.
	/// </summary>
	[SerializeField]
	private Vector3 startRotation = Vector3.zero;
	
	/// <summary>
	/// 回転スピード.
	/// </summary>
	[SerializeField]
	float speed = 5.0f;
	
	void Start ()
	{
		SetParameters();
	}

	public void SetParameters(TitleCameraSetting setting)
	{
		startPosition = setting.StartPosition;
		startRotation = setting.StartRotation;
		this.speed = setting.Speed;
		var camera = this.GetComponent<Camera>();
		if(camera != null)
		{
			camera.fieldOfView = setting.FieldOfView;
		}
		SetParameters();
	}

	void SetParameters()
	{
		this.transform.localPosition = startPosition;
		this.transform.localRotation = Quaternion.Euler(startRotation);
	}
	
	void Update ()
	{
		this.transform.RotateAround(Vector3.zero, Vector3.up, speed*Time.deltaTime);
	}
}
