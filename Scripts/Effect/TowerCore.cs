/// <summary>
/// TowerCore演出用スクリプト
/// 
/// 2013/10/04
/// </summary>
using UnityEngine;
using System.Collections.Generic;

public class TowerCore : MonoBehaviour
{
	#region フィールド＆プロパティ
	[SerializeField]
	private ParticleSystem particle;
	[SerializeField]
	private float defaultStartSize;
	[SerializeField]
	private List<TowerCoreParam> param;
	
	[System.Serializable]
	private class TowerCoreParam
	{
		public float hpRatio = 0;
		public float startSize = 0;
	}
	#endregion

	#region 初期化
	void Start()
	{
		Transform parent = this.transform.parent;
		while(parent != null)
		{
			TowerBase tower = parent.GetComponent<TowerBase>();
			if(tower != null)
			{
				tower.SetTowerCore(this);
				break;
			}
			parent = parent.parent;
		}
	}
	#endregion
	
	#region ダメージ演出
	private float nowStartSize = -1f;
	public void SetDamage(TowerBase tower)
	{
		try
		{
			if(particle != null)
			{
				float hpRatio = (float)tower.HitPoint / tower.MaxHitPoint;
				float sSize = defaultStartSize;
				
				foreach(TowerCoreParam p in this.param)
				{
					if(hpRatio < p.hpRatio)
					{
						sSize = p.startSize;
					}
				}
				
				if(nowStartSize != sSize)
				{
					nowStartSize = sSize;
					particle.startSize = sSize;
				}
			}
		}
		catch(System.Exception e)
		{
			BugReportController.SaveLogFileWithOutStackTrace(e.ToString());
		}
	}
	#endregion
}
