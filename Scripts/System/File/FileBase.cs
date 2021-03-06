/// <summary>
/// ファイルベース
/// 
/// 2013/09/04
/// </summary>
using UnityEngine;
using System.Collections;
using System.IO;

public class FileBase
{
	#region 継承
	protected virtual void Decode(string encodeString)
	{
	}
	protected virtual void Encode(out string encodeString)
	{
		encodeString = "";
	}
    #endregion

    #region IO
    protected bool Read(string directory, string filename) {
        string path = GetFilePath(directory, filename);

        // ファイルが存在するか
        if (!IsExistFile(path)) {
            UnityEngine.Debug.Log("<color=#00ff00>file:" + path + " not exists</color>");
            return false;
        }
		// ファイル読み込み
		string encodeString = "";
		{
			FileInfo fi = new FileInfo(path);
			using(StreamReader sr = new StreamReader(fi.OpenRead()))
			{
				encodeString = sr.ReadToEnd();
				sr.Close();
			}
		}

		// デコード
		this.Decode(encodeString);
		return true;
	}
	protected void Write(string directory, string filename)
	{
		string path = GetFilePath(directory, filename);

		// ディレクトリ作成
		CreateDirectory(directory);
		// エンコード
		string encodeString;
		this.Encode(out encodeString);

		// ファイル書き込み
		{
			FileInfo fi = new FileInfo(path);
			using(StreamWriter sw = fi.CreateText())
			{
				sw.Write(encodeString);
				sw.Flush();
				sw.Close();
			}
		}
	}
	protected static void Delete(string directory, string filename)
	{
		string path = GetFilePath(directory, filename);
		File.Delete(path);
	}
	#endregion

	#region Directory
	/// <summary>
	/// ディレクトリ取得
	/// </summary>
	/// <returns></returns>
	/// <param name="directoryName"></param>
	public static string GetDirectory(string directoryName)
	{
		string directoryPath = "";
		switch(Application.platform)
		{
		case RuntimePlatform.Android:
		case RuntimePlatform.IPhonePlayer:
			directoryPath = Path.Combine(Application.persistentDataPath, directoryName);
			break;
		default:
			directoryPath = Path.Combine("", directoryName);
			break;
		}
		return directoryPath;
	}
	/// <summary>
	/// ディレクトリが存在するかどうか
	/// </summary>
	/// <returns></returns>
	/// <param name="directoryName"></param>
	public static bool IsExistDirectory(string directoryName)
	{
		string directoryPath = GetDirectory(directoryName);
		return File.Exists(directoryPath);
	}
	/// <summary>
	/// ディレクトリ作成
	/// </summary>
	/// <returns></returns>
	/// <param name="directoryName"></param>
	public static bool CreateDirectory(string directoryName)
	{
		if (IsExistDirectory(directoryName))
			{ return false; }

		string directoryPath = GetDirectory(directoryName);
		Directory.CreateDirectory(directoryPath);
		return true;
	}
	#endregion

	#region File
	/// <summary>
	/// ファイルパス取得
	/// </summary>
	/// <returns></returns>
	/// <param name="directoryName"></param>
	/// <param name="fileName"></param>
	public static string GetFilePath(string directoryName, string fileName)
	{
		string directoryPath = GetDirectory(directoryName);
		return Path.Combine(directoryPath, fileName);
	}
	/// <summary>
	/// ファイルが存在するかどうか
	/// </summary>
	/// <returns></returns>
	/// <param name="directoryName"></param>
	/// <param name="fileName"></param>
	public static bool IsExistFile(string directoryName, string fileName)
	{
		string filePath = GetFilePath(directoryName, fileName);
		return IsExistFile(filePath);
	}
	public static bool IsExistFile(string filePath)
	{
		return File.Exists(filePath);
	}
	#endregion
}
