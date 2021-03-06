/*
using UnityEngine;

public class VoiceChatController
{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
	static public void Start(){ }
	static public void Stop(){ }
#elif UNITY_ANDROID
	static private AndroidJavaObject voiceChat;
	static private AndroidJavaObject VoiceChat
	{
		get
		{
			if(voiceChat == null)
			{
				// Activityではないのでここでキャッシュしてもロストの心配はないはず.
				// Activityはライフサイクルに従い再生成されることがあり、その時に参照をロストする.
				voiceChat = new AndroidJavaObject("com.asobimo.TalkTest.VoiceChat");
			}
			return voiceChat;
		}
	}
	static public void Start()
	{
		VoiceChat.Call("start"); // 開始.
	}
	
	static public void Stop()
	{
		VoiceChat.Call("finish"); // 終了.
	}
#else
	static public void Start(){ }
	static public void Stop(){ }
#endif
}
*/