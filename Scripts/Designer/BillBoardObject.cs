//=============================================================================
/*
 \brief         ビルボード
                現在使用されているメインカメラに対して、関連付けられたGameObjectが正面を向くように向きを変更する
                Unityの Camera.mainCamera でカメラを発見するのでタグがMainCameraになったカメラと現在のメインカメラが一致する必要がある
 $Date$			2011/09/28
 $Author$		okabe
*/
//=============================================================================

using UnityEngine;
using System.Collections;

public class BillBoardObject : MonoBehaviour
{
	public bool m_yRotation;
	private Transform m_transform;
	private Quaternion m_base;
	
	void Awake()
	{
		m_transform = gameObject.transform;
		//現在使ってる計算式だとローカル軸でのY軸をこっちに向けてしまうので辻褄あわせ
		m_base = Quaternion.FromToRotation( new Vector3(0.0f,0.0f,1.0f), new Vector3(0.0f,1.0f,0.0f)) * Quaternion.AngleAxis(-180.0f,new Vector3(1.0f,0.0f,0.0f)) * Quaternion.AngleAxis(180.0f,new Vector3(0.0f,1.0f,0.0f));
		//m_base = Quaternion.identity;
		Rotate();
	}


	void Update()
	{
		Rotate();
	}
	
	void Rotate()
	{
		Camera _camera = Camera.main;
		
		if( _camera == null)
		{
			return;
		}
		
		if(m_yRotation)
		{
			Vector3 _forward = (_camera.transform.position - m_transform.position);
			Vector3 _base = Vector3.forward;
			
			_forward.y = 0.0f;
			
			_forward = _forward.normalized;
			Vector3 _axis = Vector3.Cross(_base, _forward);
			
			float _angle = Vector3.Angle( _base, _forward);
			
			if(_axis.y < 0.0f)
			{
				_angle = -_angle;
			}
			m_transform.rotation = Quaternion.AngleAxis( _angle, Vector3.up);
			
		}
		else
		{
			Vector3 _forward = (_camera.transform.position - m_transform.position).normalized;
			m_transform.rotation = Quaternion.LookRotation(_camera.transform.up, _forward) * m_base;
		}
	}
}
