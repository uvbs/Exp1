/// <summary>
/// Transformのアタッチを切り、EffectManagerの子にする.
/// 
/// 2014/01/25
/// </summary>
using UnityEngine;

/// <summary>
/// Transformのアタッチを切り、EffectManagerの子にする.
/// </summary>
public class AttachBreaker : MonoBehaviour, IInterrupt
{
	public void Interrupt()
	{
		this.Break();
	}
	
	public void Break()
	{
		try
		{
			this.transform.parent = EffectManager.Instance.gameObject.transform;
		}
		catch(System.Exception e)
		{
			BugReportController.SaveLogFile(e.ToString());
		}
	}
}
