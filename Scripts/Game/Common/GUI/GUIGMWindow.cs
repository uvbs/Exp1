/// <summary>
/// GMや告知などのメッセージの処理を行う.
/// .
/// 2014/05/22.
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Scm.Common.GameParameter;

public class GUIGMWindow : Singleton<GUIGMWindow>
{
	#region アタッチ.
	
	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[System.Serializable]
	public class Attach
	{
		/// <summary>
		/// ウィンドウ再生用Tween.
		/// </summary>
		[SerializeField]
		public UIPlayTween playTween;
		
		/// <summary>
		/// スクロールメッセージ.
		/// </summary>
		public GUIScrollMessage scrollMessage;
	}
	
	#endregion
	
	#region フィールド&プロパティ.
	
	/// <summary>
	/// アタッチ物.
	/// </summary>
	[SerializeField] Attach attach;
	
	/// <summary>
	/// メッセージ処理更新用.
	/// </summary>
	private IEnumerator messageUpdate = null;
	
	#endregion
	
	#region 開始.
	
	void Start ()
	{
		_Play(false);
	}
	
	#endregion
	
	#region 更新.
	
	void Update ()
	{
		if(this.messageUpdate == null)
			return;
		
		if(!this.messageUpdate.MoveNext())
		{
			_Play(false);
			this.messageUpdate = null;
		}
	}
	
	#endregion
	
	#region メッセージ追加.
	
	/// <summary>
	/// メッセージ追加.
	/// </summary>
	public static void AddMessage(ChatInfo chatInfo)
	{
		if(Instance == null)
			return;
		Instance._AddMessage(chatInfo);
	}
	
	/// <summary>
	/// メッセージ追加処理.
	/// </summary>
	private void _AddMessage(ChatInfo chatInfo)
	{
		// メッセージをコンバートする.
		Queue<string> messageQueue = ConvertNewLineToQueue(chatInfo.chatType, chatInfo.text);
		
		// 再生.
		_Play(true);
		
		// メッセージ更新コルーチンセット.
		this.messageUpdate = MessageCoroutine(messageQueue);
	}
	
	#endregion
	
	#region 再生.
	
	/// <summary>
	/// 再生.
	/// </summary>
	public static void Play(bool forward)
	{
		if(Instance == null)
			return;
		Instance._Play(forward);
	}
	
	/// <summary>
	/// 再生処理.
	/// </summary>
	private void _Play(bool forward)
	{
		this.attach.playTween.Play(forward);
	}
	
	#endregion
	
	#region メッセージ処理.
	
	/// <summary>
	/// メッセージ処理.
	/// </summary>
	IEnumerator MessageCoroutine(Queue<string> messageQueue)
	{
		while(messageQueue.Count != 0)
		{
			// メッセージセット.
			string message = messageQueue.Dequeue();
			this.attach.scrollMessage.SetMessage(message);
			
			// スクロール再生.
			this.attach.scrollMessage.ReStart();
			while(this.attach.scrollMessage.IsPlay)
			{
				// スクロールの再生が終了するまで待機.
				yield return 0;
			}
		}
	}
	
	/// <summary>
	/// 改行コードずつ1つの文字列に変換しキューに詰めて返す処理.
	/// </summary>
	/// <returns>
	private Queue<string> ConvertNewLineToQueue(ChatType chatType, string message)
	{
		Queue<string> messageQueue = new Queue<string>();
		string[] texts = message.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
		foreach(string text in texts)
		{
			// 空白文字は追加しない.
			if(text == "")
			{
				continue;
			}
			
			// 全体にChatType色を付ける.
			string textColor = GUIChat.AddColorCode(text, chatType);
			
			// 文字追加.
			messageQueue.Enqueue(textColor);
		}
		
		return messageQueue;
	}
	
	#endregion
}
