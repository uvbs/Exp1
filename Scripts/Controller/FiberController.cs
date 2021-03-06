/// <summary>
/// Fiberコントローラ.
/// GameObjectの消滅やScene移行によって中断したくない処理を登録する.
/// 
/// 2014/05/27
/// </summary>
using UnityEngine;
using System.Collections;

/// <summary>
/// Fiberコントローラ.
/// GameObjectの消滅やScene移行によって中断したくない処理を登録する.
/// </summary>
public class FiberController : MonoBehaviour
{
	/// <summary>
	/// 実行するFiberSet.
	/// </summary>
	// 別スレッドからアクセスする可能性があるのでstatic.
	// MonoBehaviour(UnityEngine.Object)は別スレッドからアクセスするとエラーになる動作が多いため.
	static private readonly FiberSet fiberSet = new FiberSet();

	// シーンチェンジ時にGameObjectを削除させないかどうか
	[SerializeField]
	private bool _isDontDestroyOnLoad = true;
	public bool IsDontDestroyOnLoad { get { return _isDontDestroyOnLoad; } }

	void Awake()
	{
		// インスタンスを保存して,アクセス時にnullチェック→無ければ生成しようと思ったが.
		// UnityEngine.Objectは別スレッドから比較演算を行うとエラーになるので断念.
		if (this.IsDontDestroyOnLoad)
		{
			Object.DontDestroyOnLoad(this.gameObject);
		}
	}

	/// <summary>
	/// Fiberを実行する.このスクリプトがHierarchy上に存在している必要がある.
	/// </summary>
	void Update()
	{
		lock(fiberSet)
		{
			fiberSet.Update();
		}
	}

	/// <summary>
	/// コルーチンをFiberControllerに追加する.
	/// </summary>
	static public Fiber AddFiber(IEnumerator coroutine)
	{
		lock(fiberSet)
		{
			return fiberSet.AddFiber(coroutine);
		}
	}

	/// <summary>
	/// FiberをFiberControllerに追加する.
	/// </summary>
	static public Fiber AddFiber(Fiber fiber)
	{
		lock(fiberSet)
		{
			return fiberSet.AddFiber(fiber);
		}
	}

	/// <summary>
	/// FiberをFiberControllerから削除する.
	/// AddFiberした際の戻り値を引数に指定.
	/// </summary>
	static public bool Remove(Fiber fiber)
	{
		lock(fiberSet)
		{
			return fiberSet.Remove(fiber);
		}
	}

	/// <summary>
	/// FiberがFiberControllerで実行中かどうか.
	/// AddFiberした際の戻り値を引数に指定.
	/// </summary>
	static public bool Contains(Fiber fiber)
	{
		return fiberSet.Contains(fiber);
	}
}