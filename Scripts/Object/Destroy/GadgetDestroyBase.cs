/// <summary>
/// Gadgetクラスを持つオブジェクトの専用破棄時処理を行う
/// EntrantTypeでは判定出来ないオブジェクト(シールドジェネレータや回復コンソール)
/// などの個別の破壊処理を行う
/// 
/// 2014/12/04
/// </summary>
using UnityEngine;
using System.Collections;

public abstract class GadgetDestroyBase : MonoBehaviour
{
	/// <summary>
	/// 破壊時処理
	/// </summary>
	public abstract void Destroy(Gadget gadget);
}
