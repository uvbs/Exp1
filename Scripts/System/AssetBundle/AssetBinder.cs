/// <summary>
/// 共有アセットバンドル用,UnityEngine.Objectを保持するだけ.
/// 
/// 2014/06/12
/// </summary>
using UnityEngine;
using System;
using System.Collections;

public class AssetBinder : ScriptableObject
{
	/// <summary>
	/// アセットの参照を置くだけ.
	/// 多数のアセットバンドルで使われるものを追加する.
	/// もし参照が切れても特に害はない.
	/// </summary>
	[SerializeField]
	private UnityEngine.Object[] assets;

#if UNITY_EDITOR
	public UnityEngine.Object[] Assets { get { return assets; } set { assets = value; } }
#endif
}
