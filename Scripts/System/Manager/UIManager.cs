/// <summary>
/// UIマネージャー
/// 
/// 2015/06/10
/// </summary>
/// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManager : Manager
{
	#region フィールド＆プロパティ
	public static UIManager Instance;
	public LinkedList<GameObject> Link { get; private set; }

	UIModeType modeType;

	class Actions
	{
		public System.Action preprocess { get; private set; }
		public System.Action postprocess { get; private set; }

		public Actions(System.Action pre, System.Action post)
		{
			preprocess = pre;
			postprocess = post;
		}
	}
	Dictionary<int, Actions> transitions;
	#endregion

	#region 初期化
	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		Link = new LinkedList<GameObject>();

		modeType = UIModeType.None;
		transitions = new Dictionary<int, Actions>();
	}
	protected override void Setup(GameObject go)
	{
		base.Setup(go);
		Link.AddLast(go);
	}
	#endregion

	#region 削除
	public override void Destroy(GameObject go)
	{
		Link.Remove(go);
		base.Destroy(go);
	}
	#endregion

	#region MyRegion

	/// <summary>
	/// モードの設定
	/// </summary>
	public static void SetModeType(UIModeType type)
	{
		if (Instance != null)
		{
			Instance.modeType = type;
			Instance.transitions.Clear();
		}
	}

	/// <summary>
	/// 番号とそれに付随する関数の登録
	/// </summary>
	/// <param name="num"></param>
	/// <param name="func"></param>
	public static void SetTransition(int num, System.Action preFunc, System.Action postFunc)
	{
		if (Instance != null)
		{
			Actions a;
			if (!Instance.transitions.TryGetValue(num, out a))
			{
				Instance.transitions.Add(num, new Actions(preFunc, postFunc));
			}
			else
			{
				Debug.LogWarning("既に登録済みです: " + num);
			}
		}
	}

	/// <summary>
	/// 遷移リクエスト
	/// </summary>
	public static void RequestTransition(int num)
	{
		if (Instance == null)
		{
			return;
		}
		// この条件は仮
		if (Instance.modeType != UIModeType.Lobby)
		{
			return;
		}

// ここでロック云々の処理を行う

		// 遷移関数を呼び出す
		Actions a;
		if (Instance.transitions.TryGetValue(num, out a))
		{
			a.preprocess();
		}
	}
	#endregion
}
