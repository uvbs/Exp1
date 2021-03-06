/// <summary>
/// GO-ONE専用の一時ソース
/// 2015/04/28
/// 
/// </summary>
using UnityEngine;
using System.Collections;

public class GoOneObsolateSrc
{
	/// <summary>
	/// GoOne用のバージョン.
	/// </summary>
	private const int GoOneVersion = 2;

	/// <summary>
	/// GoOne専用のバージョン情報を取得する
	/// </summary>
	public static string GetGoOneVersion()
	{
		string versionName = string.Empty;
#if GOONE
		versionName = GoOneVersion.ToString();
#endif
		return versionName;
	}
}
