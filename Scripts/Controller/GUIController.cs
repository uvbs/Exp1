/// <summary>
/// GUIコントローラ
/// 
/// 2015/11/18
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;

public static class GUIController
{
	#region スクリーン制御
	/// <summary>
    /// 画面リスト
    /// </summary>
    private static LinkedList<GUIScreen> screens = new LinkedList<GUIScreen>();

	#region 開く
	/// <summary>
    /// 画面を開く
    /// </summary>
    /// <param name="screen"></param>
    public static void Open(GUIScreen screen)
    {
        if (screen == null) { return; }

        GUIScreen nowScreen;
        if (screens.Count > 0)
        {
            nowScreen = screens.Last.Value;

            // 現在開いているUIを閉じる
            if (screens.Count > 0)
            {
                if (nowScreen != null)
                {
                    nowScreen.Background();
                }
            }
        }

        // 新しいUIを開きリストに登録する
        screen.Open();
        screens.AddLast(screen);
    }
	#endregion

	#region 閉じる
	/// <summary>
    /// 表示している画面を閉じ、一つ前に表示していた画面を開く
    /// </summary>
    public static void Back()
    {
        // 現在開いているUIを閉じ、リストから削除
        if (screens.Count > 0)
        {
            GUIScreen nowScreen = screens.Last.Value;
            nowScreen.Close();
            screens.RemoveLast();
        }

        // 一つ前に表示していたUIを開く
        if (screens.Count > 0)
        {
            GUIScreen backScreen = screens.Last.Value;
            if (backScreen != null)
            {
                backScreen.ReOpen();
            }
        }
    }
	#endregion

	#region クリア
	/// <summary>
    /// 登録している全ての画面を閉じリストから削除する
    /// </summary>
    public static void Clear()
    {
        foreach (GUIScreen screen in screens)
        {
			screen.Clear();
        }

        screens.Clear();
	}
	#endregion

	/// <summary>
	/// 現在スクリーンが開いているかどうかを取得
	/// </summary>
	public static bool IsCurrentScreen
	{
		get { return screens.Count > 0; }
	}
	#endregion

	#region 一つのみ表示するUIの制御
	/// <summary>
	/// 表示しているUI
	/// </summary>
	private static GUISingle singleUI = null;

	/// <summary>
	/// 現在表示しているUIが存在していない場合のみ表示する
	/// </summary>
	/// <param name="ui"></param>
	public static void SingleOpen(GUISingle ui)
	{
		// すでに開いているUIが存在するなら無視をする
		if (singleUI != null) { return; }

		// 新しいUIを表示し登録
		ui.Open();
		singleUI = ui;
	}

	/// <summary>
	/// 現在開いているUIを閉じる
	/// </summary>
	public static void SingleClose()
	{
		if(singleUI != null)
		{
			singleUI.Close();
			singleUI = null;
		}
	}
	#endregion
}


/// <summary>
/// スクリーンクラス
/// GUIControllerで制御する場合に使用する
/// </summary>
public class GUIScreen
{
    /// <summary>
    /// 表示処理
    /// </summary>
    public Action Open { get; private set; }

    /// <summary>
    /// 閉じる処理
    /// </summary>
    public Action Close { get; private set; }

	/// <summary>
	/// 再表示処理
	/// </summary>
	public Action ReOpen { get; private set; }

	/// <summary>
	/// クリア処理
	/// </summary>
	public Action Clear { get; private set; }

	/// <summary>
	/// フォーカスが移った時の処理
	/// </summary>
	public Action Background { get; private set; }

	/// <summary>
    /// コンストラクタ
    /// </summary>
    public GUIScreen(Action open, Action close, Action reOpen, Action clear, Action background)
    {
        this.Open = open;
        this.Close = close;
		this.ReOpen = reOpen;
		this.Clear = clear;
		this.Background = background;
    }
	/// <summary>
	/// コンストラクタ
	/// ReOpenはopenをClearとBackgroundはcloseをデフォルトで設定を行う
	/// </summary>
	public GUIScreen(Action open, Action close) : this(open, close, open, close, close) { }
	/// <summary>
	/// コンストラクタ
	/// ClearとBackgroundはcloseをデフォルトで設定を行う
	/// </summary>
	public GUIScreen(Action open, Action close, Action reOpen) : this(open, close, reOpen, close, close) { }
	/// <summary>
	/// コンストラクタ
	/// Backgroundはcloseをデフォルトで設定を行う
	/// </summary>
	public GUIScreen(Action open, Action close, Action reOpen, Action clear) : this(open, close, reOpen, clear, close) { }
}

/// <summary>
/// 画面上に一つだけ表示したいUIを制御する時に使用する
/// </summary>
public class GUISingle
{
	/// <summary>
	/// 表示処理
	/// </summary>
	public Action Open { get; private set; }

	/// <summary>
	/// 閉じる処理
	/// </summary>
	public Action Close { get; private set; }

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="open"></param>
	/// <param name="close"></param>
	public GUISingle(Action open, Action close)
	{
		this.Open = open;
		this.Close = close;
	}
}