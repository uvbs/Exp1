/// <summary>
/// マスターデータのバージョン管理.
/// 
/// 2015/02/17
/// </summary>
using System;
using System.IO;
using System.Net;
using System.Collections.Generic;

public class MasterDataVersions : VersionFile
{
	#region 定数.
	// マスターデータおよびバージョン管理ファイルのURLフォルダ.
	public string URL { get { return Url; } }
	protected override string Url
	{
		get
		{
			return ObsolateSrc.MasterDataURL;
		}
	}

	// バージョン管理ファイルの名前.
	public const string FILENAME = "XwMasterDataVer.json";
	protected override string FileName { get { return FILENAME; } }
	
	// ローカルの保存用パス.
	public const string PATH = "Work/Master";
	protected override string Path { get { return PATH; } }

	// JSONデータの親キー.
	protected const string JSONKEY_ROOT = "MasterDataVer";
	protected override string JsonKey_Root { get { return JSONKEY_ROOT; } }

	// キーに追加する拡張子など.
	protected const string EXT = ".csv";
	protected override string Ext { get { return EXT; } }
	#endregion
}