/// <summary>
/// Bug report controller.
/// 
/// 2012/12/12
/// </summary>

using System;
using System.IO;
using System.Threading;
using System.Collections;
using UnityEngine;

public class BugReportController : Singleton<BugReportController>
{
	#region 定数

	const string sendURL = ObsolateSrc.BugReportURL;
	const string reportFileName = "bugreport.dat";

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
	// プラグインなし.
#elif UNITY_ANDROID
	const string reportFileNameJ = "bugreportJ.dat";
#elif UNITY_IPHONE
	const string reportFileNameC = "bugreportC.dat";
#endif

	#endregion

	#region プロパティ

	static string currentPath;
	static string CurrentPath
	{
		get
		{
			if( currentPath == null )
			{
				#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
					currentPath = Directory.GetCurrentDirectory();
				#else
					currentPath = Application.persistentDataPath;
				#endif
			}
			return currentPath;
		}
	}

	#endregion

	#region Unityリフレクション
	protected override void Awake()
	{
		// バグレポートのパス設定＆プラグイン用バグレポートパスの設定.
	#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
//		string[] sendFilePaths = { };
	#elif UNITY_ANDROID
		string pluginReportPath = Path.Combine(CurrentPath, reportFileNameJ);
//		string[] sendFilePaths = { pluginReportPath };
	#elif UNITY_IPHONE
		string pluginReportPath = Path.Combine(CurrentPath, reportFileNameC);
//		string[] sendFilePaths = { pluginReportPath };
	#endif
/* バグレポートを送る機能を封印
		string reportPath = Path.Combine(CurrentPath, reportFileName);

		// バグレポートの送信.
		{
			var file = new FileInfo(reportPath);
			if(file.Exists)
			{
				string separator = "\r\n\r\n";
				string log = separator + MakeBugReportHeader();
				using(StreamReader sReader = new StreamReader(file.OpenRead()))
				{
					log += sReader.ReadToEnd() + separator + "\r\n";
					sReader.Close();
				}
				file.Delete();

				StartCoroutine(SendBugReport(log));
			}
		}
		// プラットフォームごとのバグレポートの送信.
		foreach(string sendPath in sendFilePaths)
		{
			var file = new FileInfo(sendPath);
			if(file.Exists)
			{
				string log;
				using(StreamReader sReader = new StreamReader(file.OpenRead()))
				{
					log = sReader.ReadToEnd();
					sReader.Close();
				}
				file.Delete();

				StartCoroutine(SendBugReport(log));
			}
		}
 */
		// バグレポート処理の設定.
		Application.logMessageReceived += BugReportController.HandleLog;

		// プラグインバグレポート処理の設定.
	#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
//	#elif UNITY_ANDROID
//		PluginController.SetPluginExceptionHandler(pluginReportPath);
//	#elif UNITY_IPHONE
//		PluginController.SetPluginExceptionHandler(pluginReportPath);
	#endif
	}

	public void OnDestroy()
	{
		Application.logMessageReceived -= BugReportController.HandleLog;
	}

	#endregion

	#region Method

	/// <summary>
	/// Unity内部から呼ばれるハンドラ.
	/// </summary>
	private static void HandleLog (string logString, string stackTrace, LogType type)
	{
		// とりあえずExceptionのみ処理.
		if(type == LogType.Exception)
		{
			SaveLogFileWithOutStackTrace(logString+"\r\n"+stackTrace);
		}

		DisplayMessage(logString, stackTrace, type);
	}

	/// <summary>
	/// ログを画面上に表示する.
	/// </summary>
	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	private static void DisplayMessage (string logString, string stackTrace, LogType type)
	{
		StringReader sr = new StringReader(logString);
		GUIDebugMessage.AddMsg(sr.ReadLine());
	}

	/// <summary>
	/// バグレポートを作成する.
	/// </summary>
	/// <param name='log'>
	/// バグレポートに記載したい内容.
	/// </param>
	public static void SaveLogFile(string log)
	{
		SaveLogFileWithOutStackTrace(log+"\r\n"+Environment.StackTrace);
	}

	/// <summary>
	/// バグレポートをスタックトレース抜きで作成する.
	/// </summary>
	/// <param name='log'>
	/// バグレポートに記載したい内容.
	/// </param>
	public static void SaveLogFileWithOutStackTrace(string log)
	{
		string header = "---" + "\r\n" +
			"Time:"+DateTime.Now.ToString("s")+"\r\n";

		string filename = Path.Combine(CurrentPath, reportFileName);
		
		try
		{
			WriteLogFile(filename, header+log);
		}
		catch
		{
			// 書き込み失敗？.
			if(Instance != null)
			{
				// 書き込みリトライコルーチンを起動.
				Instance.StartCoroutine(SaveLogFileRetryCoroutine(filename, header+log));
			}
		}
	}

	/// <summary>
	/// バグレポートをファイルに書き込む.
	/// </summary>
	private static void WriteLogFile(string filename, string log)
	{
		var file = new FileInfo(filename);

		using(StreamWriter sWriter = file.AppendText())
		using (TextWriter writerSync = TextWriter.Synchronized(sWriter))
		{
			writerSync.Write(log+"\r\n");
			writerSync.Flush();
			writerSync.Close();
		}
	}

	/// <summary>
	/// バグレポート書き込みリトライコルーチン.
	/// </summary>
	private static IEnumerator SaveLogFileRetryCoroutine(string filename, string log)
	{
		const int   RetryCount    = 3;		// リトライ回数.
		const float RetryInterval = 5f;		// リトライ間隔(秒).

		for(int i = 0; i < RetryCount; ++i)
		{
			yield return new WaitForSeconds(RetryInterval);
			try
			{
				WriteLogFile(filename, log);
				break;
			}
			catch
			{
				continue;
			}
		}
	}

	private string MakeBugReportHeader()
	{
		// string.Concat呼出1回に変換されるため,ループ内でない限りは+演算子の方が高速.
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
		string header = "PLATFORM:"+Application.platform.ToString()+"\r\n" +
			"VERSION:XW_DEBUG"+DateTime.Now.ToString("MM")+"\r\n" +
			"OS:"+SystemInfo.operatingSystem+"\r\n" +
			"Model:"+SystemInfo.deviceModel+"\r\n" +
			"Type:"+SystemInfo.deviceType+"\r\n" +
			"Lang:"+Application.systemLanguage+"\r\n" +
			"Name:"+SystemInfo.deviceName+"\r\n" +
			"CPU:"+SystemInfo.processorType+" : "+SystemInfo.processorCount+"\r\n" +
			"Memory:"+SystemInfo.systemMemorySize+"\r\n" +
			"Uni:"+SystemInfo.deviceUniqueIdentifier;
#else
		string header = "PLATFORM:"+Application.platform.ToString()+"\r\n" +
			"VERSION:"+PluginController.PackageInfo.versionName1+"\r\n" +
			"Code:"+PluginController.PackageInfo.versionCode+"\r\n" +
			"OS:"+SystemInfo.operatingSystem+"\r\n" +
			"Model:"+SystemInfo.deviceModel+"\r\n" +
			"Type:"+SystemInfo.deviceType+"\r\n" +
			"Lang:"+Application.systemLanguage+"\r\n" +
			"CPU:"+SystemInfo.processorType+" : "+SystemInfo.processorCount+"\r\n" +
			"Memory:"+SystemInfo.systemMemorySize+"\r\n" +
			"Uni:"+SystemInfo.deviceUniqueIdentifier;
#endif
		return header;
	}

	/// <summary>
	/// バグレポートを送信する.
	/// </summary>
	private IEnumerator SendBugReport(string log)
	{
		var wwwForm = new WWWForm();
		wwwForm.AddField( "log", log );
		var gettext = new WWW(sendURL, wwwForm);
		yield return gettext;
	}

	#endregion
}
