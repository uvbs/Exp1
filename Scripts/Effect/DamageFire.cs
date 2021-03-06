/// <summary>
/// タワー炎上演出用スクリプト
/// 
/// 2013/10/04
/// </summary>
using UnityEngine;
using System.Collections.Generic;

public class DamageFire : MonoBehaviour
{
	#region フィールド＆プロパティ
	[SerializeField]
	private ParticleSystem particle;
	[SerializeField]
	private float defaultEmissionRate;
	[SerializeField]
	private List<DamageFireParam> param;
	
	[System.Serializable]
	private class DamageFireParam
	{
		public float hpRatio = 0;
		public float emissionRate = 0;
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
				tower.SetDamageFire(this);
				break;
			}
			parent = parent.parent;
		}
	}
	#endregion
	
	#region ダメージ演出
	private float nowEmissionRate = -1f;
	public void SetDamage(TowerBase tower)
	{
		try
		{
			if(particle != null)
			{
				float hpRatio = (float)tower.HitPoint / tower.MaxHitPoint;
				float emRate = defaultEmissionRate;
				
				foreach(DamageFireParam p in this.param)
				{
					if(hpRatio < p.hpRatio)
					{
						emRate = p.emissionRate;
					}
				}
				
				if(nowEmissionRate != emRate)
				{
					nowEmissionRate = emRate;
					var rate = particle.emission.rate;
					rate.constantMax = emRate;
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
