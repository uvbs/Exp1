using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;

public class LogFileController : Singleton<LogFileController>
{
	
	#region フィールド
	
	/// <summary>
	/// 作成するディレクトリ名.
	/// </summary>
	private const string createDirectory = "Work/Log";
	
	/// <summary>
	/// 書き込みor読み込み先パス.
	/// </summary>
	private static string logPath = "";
	
	/// <summary>
	/// 書き込むファイル名.
	/// </summary>
	private string writeFileName = "";
	
	#endregion
	
	#region メソッド
	
	/// <summary>
	/// ディレクトリ及びファイルの削除を行う.
	/// </summary>
	void Start ()
	{
		try
		{
			// 保存するディレクトを作成.
			logPath = GameGlobal.CreateDirectory(createDirectory);
			DirectoryInfo dir = new DirectoryInfo(logPath);
			
			// 書き込むファイル名.
			this.writeFileName = String.Format("{0}-{1}-{2}", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString());
			this.writeFileName += ".txt";
			logPath = Path.Combine(logPath, this.writeFileName);
			
			// 1週間前のファイルを削除.
			DateTime dateTime = DateTime.Now.AddDays(-7);
			foreach(FileInfo file in dir.GetFiles())
			{
				if(file.CreationTime <= dateTime)
				{
					file.Delete();
				}
			}
		}
		catch(ArgumentException e)
		{
			throw new ArgumentException("LogFileController CreateDirectory path abnormal. Path=" + logPath + e);
		}
		catch(Exception e)
		{
			throw new Exception("LogFileController CreateDirectory Exception. " + e);
		}
	}
	
	/// <summary>
	/// ログファイル作成.
	/// </summary>
	/// <param name='msg'>
	/// ファイルに記載したい内容.
	/// </param>
	public static void Log(string msg)
	{
		msg = "Time:" + DateTime.Now.ToString() + " [NORMAL]" + ">" + msg + "\r\n";
		using(StreamWriter write = new StreamWriter(logPath, true))
		{
			write.Write(msg);
		}
	}
	
	/// <summary>
	/// 警告ログファイル作成.
	/// </summary>
	/// <param name='msg'>
	/// ファイルに記載したい内容.
	/// </param>
	public static void LogWarnig(string msg)
	{
		msg = "Time:" + DateTime.Now.ToString() + "[WARNING]" + ">" + msg + "\r\n";
		using(StreamWriter write = new StreamWriter(logPath, true))
		{
			write.Write(msg);
		}
	}
	
	/// <summary>
	/// エラーログファイル作成.
	/// </summary>
	/// <param name='msg'>
	/// ファイルに記載したい内容.
	/// </param>
	public static void LogError(string msg)
	{
		msg = "Time:" + DateTime.Now.ToString() + "[Error]" + ">" + msg + "\r\n";
		using(StreamWriter write = new StreamWriter(logPath, true))
		{
			write.Write(msg);
		}
	}
	
	#endregion
}
