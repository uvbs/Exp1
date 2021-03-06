/// <summary>
/// パーティクルを途中中断させる.
/// 
/// 2014/01/25
/// </summary>
using UnityEngine;

/// <summary>
/// パーティクルを途中中断させる.
/// </summary>
public class ParticleStoper : MonoBehaviour, IInterrupt
{
	public void Interrupt()
	{
		this.Stop();
	}
	
	public void Stop()
	{
		var particles = this.GetComponentsInChildren<ParticleSystem>();
		
		foreach(var particle in particles)
		{
			particle.Stop();
		}
	}
}
