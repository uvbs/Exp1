/// <summary>
/// フェード
/// 
/// 2014/08/20
/// </summary>
//#define DEBUGLOG
#define EVENTDELEGATE_CORRECTION

using UnityEngine;
using System.Collections;

public class GUIFade : Singleton<GUIFade>
{
	#region フィールド＆プロパティ
	/// <summary>
	/// デフォルトフェード時間
	/// </summary>
	const float DefaultFadeTime = 0.3f;

	/// <summary>
	/// デフォルトフェードカラー
	/// </summary>
	static Color DefaultColor { get { return Color.white; } }

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public GameObject root;
		public UIPlayTween fadePlayTween;
		public TweenColor fadeTweenColor;
		public GameObject loadingGroup;
		public UIProgressBar progressBar;
	}

	// フェード終了判定
	bool IsFinished { get; set; }
	/// <summary>
	/// フェード終了判定
	/// </summary>
	public static bool IsFinish { get { return (Instance != null ? Instance.IsFinished : true); } }

	// フェードの状態
	FadeState State { get; set; }
	public enum FadeState
	{
		None,
		LoadingFadeOut,
		LoadingFadeIn,
		FadeOut,
		FadeIn,
	}

	// フェード終了後のコールバック
	System.Action FinishFunc { get; set; }

	/// <summary>
	/// 進捗状態(0～1)
	/// </summary>
	public static float Progress
	{
		get { return (Instance != null) ? Instance.Attach.progressBar.value : 0; }
		set { if (Instance != null) Instance.Attach.progressBar.value = value; }
	}

	// シリアライズされていないメンバー変数の初期化
	void MemberInit()
	{
		this.IsFinished = true;
		this.State = FadeState.None;
		this.FinishFunc = null;
	}
	#endregion

	#region 初期化
	override protected void Awake()
	{
		base.Awake();
		// メンバー初期化
		this.MemberInit();

		// 表示設定
		this.SetActive(false);
	}
	#endregion

	#region アクティブ設定
	void SetActive(bool isActive)
	{
		// アクティブ設定
		this.Attach.root.SetActive(isActive);
		this.Attach.loadingGroup.SetActive(false);
	}
	#endregion

	#region フェードアウト設定
	/// <summary>
	/// フェードアウト設定
	/// 設定が正しく行われた場合は true を返す
	/// </summary>
	public static bool FadeOut(bool isLoading)
	{
		return (Instance != null ? Instance._FadeOut(isLoading, DefaultFadeTime, DefaultColor, null) : false);
	}
	public static bool FadeOut(bool isLoading, System.Action callback)
	{
		return (Instance != null ? Instance._FadeOut(isLoading, DefaultFadeTime, DefaultColor, callback) : false);
	}
	public static bool FadeOut(bool isLoading, float fadeTime)
	{
		return (Instance != null ? Instance._FadeOut(isLoading, fadeTime, DefaultColor, null) : false);
	}
	public static bool FadeOut(bool isLoading, float fadeTime, System.Action callback)
	{
		return (Instance != null ? Instance._FadeOut(isLoading, fadeTime, DefaultColor, callback) : false);
	}
	public static bool FadeOut(bool isLoading, Color color)
	{
		return (Instance != null ? Instance._FadeOut(isLoading, DefaultFadeTime, color, null) : false);
	}
	public static bool FadeOut(bool isLoading, Color color, System.Action callback)
	{
		return (Instance != null ? Instance._FadeOut(isLoading, DefaultFadeTime, color, callback) : false);
	}
	public static bool FadeOut(bool isLoading, float fadeTime, Color color)
	{
		return (Instance != null ? Instance._FadeOut(isLoading, fadeTime, color, null) : false);
	}
	public static bool FadeOut(bool isLoading, float fadeTime, Color color, System.Action callback)
	{
		return (Instance != null ? Instance._FadeOut(isLoading, fadeTime, color, callback) : false);
	}
	bool _FadeOut(bool isLoading, float fadeTime, Color color, System.Action callback)
	{
#if DEBUGLOG
		Debug.Log(string.Format("_FadeOut({0} {1} {2} {3})\r\n IsPlay={4}", fadeTime, color, isLoading, (callback != null ? callback.ToString() : ""), !this.IsFinished));
#endif
		// フェードアウト再生中
		this.IsFinished = false;
		// フェードアウト終了時に呼ぶコールバック
		this.FinishFunc = callback;
		// 進捗初期化
		Progress = 0f;

		if (isLoading)
		{
			// ローディング画面を表示する場合は
			// フェードアウト→フェードイン→ローディング画面の順番
			this.State = FadeState.LoadingFadeOut;
			this.PlayFadeOut(fadeTime, color, this.OnLoadingFadeOutFinish);
		}
		else
		{
			// フェードアウトのみの場合は
			// フェードアウトのみ行う
			this.State = FadeState.FadeOut;
			this.PlayFadeOut(fadeTime, color, this.OnFinish);
		}

		return true;
	}
	/// <summary>
	/// ローディング画面を表示するためのフェードアウト終了処理
	/// ローディング画面を表示してフェードインさせる
	/// </summary>
	void OnLoadingFadeOutFinish()
	{
		// ローディング画面表示
		this.Attach.loadingGroup.SetActive(true);

		// 状態設定
		this.State = FadeState.LoadingFadeIn;
		// フェードイン再生
		{
			var com = this.Attach.fadeTweenColor;
#if DEBUGLOG
			Debug.Log(string.Format("OnLoadingFadeOutFinish({0} {1})\r\n IsPlay={2}", com.duration, com.value, !this.IsFinished));
#endif
			Color color = new Color(com.value.r, com.value.g, com.value.b);
#if EVENTDELEGATE_CORRECTION
			StartCoroutine(this.FadeCoroutine(() => { this.PlayFadeIn(com.duration, color, true, this.OnFinish); }));
#else
			this.PlayFadeIn(com.duration, color, true, this.OnFinish);
#endif
		}
	}
	#endregion

	#region フェードイン設定
	/// <summary>
	/// フェードイン設定
	/// 設定が正しく行われた場合は true を返す
	/// </summary>
	public static bool FadeIn()
	{
		return (Instance != null ? Instance._FadeIn(DefaultFadeTime, DefaultColor) : false);
	}
	public static bool FadeIn(float fadeTime)
	{
		return (Instance != null ? Instance._FadeIn(fadeTime, DefaultColor) : false);
	}
	public static bool FadeIn(Color color)
	{
		return (Instance != null ? Instance._FadeIn(DefaultFadeTime, color) : false);
	}
	public static bool FadeIn(float fadeTime, Color color)
	{
		return (Instance != null ? Instance._FadeIn(fadeTime, color) : false);
	}
	bool _FadeIn(float fadeTime, Color color)
	{
#if DEBUGLOG
		Debug.Log(string.Format("_FadeIn({0} {1})\r\n IsPlay={2}", fadeTime, color, !this.IsFinished));
#endif
		// フェードイン再生中
		this.IsFinished = false;

		switch (this.State)
		{
		case FadeState.LoadingFadeOut:
		case FadeState.LoadingFadeIn:
			// 状態設定
			this.State = FadeState.FadeOut;
			// フェードアウト設定
			this.PlayFadeOut(fadeTime, color, this.OnLoadingFadeInFinish);
			break;
		case FadeState.FadeOut:
		case FadeState.FadeIn:
			// 状態設定
			this.State = FadeState.FadeIn;
			// フェードイン再生
			this.PlayFadeIn(fadeTime, color, false, this.OnFinish);
			break;
		}

		return true;
	}
	/// <summary>
	/// ローディング画面を表示した後のフェードイン終了処理
	/// ローディング画面を非表示にしてフェードインさせる
	/// </summary>
	void OnLoadingFadeInFinish()
	{
		// ローディング画面非表示
		this.Attach.loadingGroup.SetActive(false);

		// 状態設定
		this.State = FadeState.FadeIn;
		// フェードイン再生
		{
			var com = this.Attach.fadeTweenColor;
#if DEBUGLOG
			Debug.Log(string.Format("OnLoadingFadeInFinish({0} {1})\r\n IsPlay={2}", com.duration, com.value, !this.IsFinished));
#endif
			Color color = new Color(com.value.r, com.value.g, com.value.b);
#if EVENTDELEGATE_CORRECTION
			StartCoroutine(this.FadeCoroutine(() => { this.PlayFadeIn(com.duration, color, false, this.OnFinish); }));
#else
			this.PlayFadeIn(com.duration, color, false, this.OnFinish);
#endif
		}
	}
	#endregion

	#region EventDelegate.Execute 回避用のコルーチン
	IEnumerator FadeCoroutine(System.Action action)
	{
		yield return null;
		action();
	}
	#endregion

	#region フェード再生
	void PlayFadeOut(float fadeTime, Color color, EventDelegate.Callback onFinish)
	{
		PlayFade(true, fadeTime, color, false, onFinish);
	}
	void PlayFadeIn(float fadeTime, Color color, bool isLoading, EventDelegate.Callback onFinish)
	{
		PlayFade(false, fadeTime, color, isLoading, onFinish);
	}
	void PlayFade(bool isFadeOut, float fadeTime, Color color, bool isFadeInLoading, EventDelegate.Callback onFinish)
	{
		// TweenColor 設定
		{
			float toAlpha = (isFadeOut ? 1f : 0f);
			var com = this.Attach.fadeTweenColor;
			// アルファ値は現在の値をそのまま、カラーは引数を使う
			com.from = new Color(color.r, color.g, color.b, com.value.a);
			// FadeOut なのでアルファ値は 0f で固定、カラーは引数を使う
			com.to = new Color(color.r, color.g, color.b, toAlpha);
			com.duration = fadeTime;
		}

		// PlayTween 設定
		{
			var com = this.Attach.fadePlayTween;
			if (isFadeOut)
			{
				// フェードアウト終了時の処理は何もしない
				com.disableWhenFinished = AnimationOrTween.DisableCondition.DoNotDisable;
			}
			else
			{
				// フェードイン終了時の処理をロード画面を表示する時は読み込み中なのでこのオブジェクトは表示したまま
				// ロードを表示しない時は読み込み完了時なのでオブジェクトを非表示にする
				if (isFadeInLoading)
					com.disableWhenFinished = AnimationOrTween.DisableCondition.DoNotDisable;
				else
					com.disableWhenFinished = AnimationOrTween.DisableCondition.DisableAfterForward;
			}
			// 終了処理デリゲート設定
			if (onFinish != null)
				EventDelegate.Add(com.onFinished, onFinish, true);
			// 再生開始
			com.Play(true);
		}
	}
	#endregion

	#region NGUIリフレクション
	void OnFinish()
	{
#if DEBUGLOG
		Debug.Log(string.Format("OnFinish({0})\r\n IsPlay={1}", (this.FinishFunc != null ? this.FinishFunc.ToString() : ""), !this.IsFinished));
#endif
		// 終了
		this.IsFinished = true;

		// 終了処理
		if (this.FinishFunc != null)
			this.FinishFunc();
		this.FinishFunc = null;
	}
	#endregion

	#region デバッグ
#if UNITY_EDITOR && XW_DEBUG
	[SerializeField]
	DebugParameter _debugParam = new DebugParameter();
	DebugParameter DebugParam { get { return _debugParam; } }
	[System.Serializable]
	public class DebugParameter
	{
		public bool executeFadeOut;
		public bool executeFadeIn;

		public float fadeTime = DefaultFadeTime;
		public Color color = DefaultColor;
		public bool isLoading = false;
	}

	void DebugUpdate()
	{
		var t = this.DebugParam;
		if (t.executeFadeOut)
		{
			t.executeFadeOut = false;
			this._FadeOut(t.isLoading, t.fadeTime, t.color, null);
		}
		if (t.executeFadeIn)
		{
			t.executeFadeIn = false;
			this._FadeIn(t.fadeTime, t.color);
		}
	}
	void OnValidate()
	{
		this.DebugUpdate();
	}
#endif
	#endregion
}
