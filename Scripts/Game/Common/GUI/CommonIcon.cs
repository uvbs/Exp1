/// <summary>
/// 共通アイコン
/// 
/// 2016/05/27
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.XwMaster;

[System.Serializable]
public class CommonIcon
{
	#region フィールド＆プロパティ
	/// <summary>
	/// 複数のアセットバンドルの管理
	/// </summary>
	BundleDataManager BundleDataManager { get; set; }

	/// <summary>
	/// メンバー初期化
	/// </summary>
	void MemberInit()
	{
		this.BundleDataManager = new BundleDataManager();
	}
	#endregion

	#region 初期化
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public CommonIcon() { this.MemberInit(); }
	/// <summary>
	/// クリア
	/// </summary>
	public void Clear()
	{
		// リソースは参照切るだけで消える？
		this.MemberInit();
	}
	#endregion

	#region アイコン取得
	/// <summary>
	/// アイコンを取得する
	/// 同じアセットバンドル内で違うリソースを読み込む場合は keepAssetReference を true にしておくと読み込み済みのアセットバンドルを保持しておく
	/// その際アセットバンドル内の全てのリソースを読み終わったら内部でアセットバンドルを破棄している
	/// </summary>
	public void GetIcon(int iconMasterID, bool keepAssetReference, System.Action<UIAtlas, string> callback)
	{
		string bundleName, spriteName;

		// アイコン情報を取得する
		if (!this.GetInfo(iconMasterID, out bundleName, out spriteName))
		{
			if (callback != null) callback(null, "");
			return;
		}

		var bundleData = this.BundleDataManager.GetBundleData<CommonIconBundleData>(bundleName);
		if (bundleData != null)
		{
			bundleData.GetIcon(bundleName, keepAssetReference,
				(UIAtlas resource) => { if (callback != null) callback(resource, spriteName); });
		}
	}
	/// <summary>
	/// モノクロアイコンを取得する
	/// 同じアセットバンドル内で違うリソースを読み込む場合は keepAssetReference を true にしておくと読み込み済みのアセットバンドルを保持しておく
	/// その際アセットバンドル内の全てのリソースを読み終わったら内部でアセットバンドルを破棄している
	/// </summary>
	public void GetMonoIcon(int iconMasterID, bool keepAssetReference, System.Action<UIAtlas, string> callback)
	{
		string bundleName, spriteName;

		// アイコン情報を取得する
		if (!this.GetInfo(iconMasterID, out bundleName, out spriteName))
		{
			if (callback != null) callback(null, "");
			return;
		}

		var bundleData = this.BundleDataManager.GetBundleData<CommonIconBundleData>(bundleName);
		if (bundleData != null)
		{
			bundleData.GetMonoIcon(bundleName, keepAssetReference,
				(UIAtlas resource) => { if (callback != null) callback(resource, spriteName); });
		}
	}
	/// <summary>
	/// アイコンのスプライト設定
	/// </summary>
	public static bool SetIconSprite(UISprite setSp, UIAtlas atlas, string spriteName)
	{
		if (setSp == null)
			return false;

		// アトラス設定
		setSp.atlas = atlas;
		// スプライト設定
		setSp.spriteName = spriteName;

		// アトラス内にアイコンが含まれているかチェック
		if (setSp.GetAtlasSprite() == null)
		{
			// アトラスとスプライト名が両方共設定されていてスプライトがない場合はエラー扱いｌ
			if (atlas != null && !string.IsNullOrEmpty(spriteName))
			{
				Debug.LogWarning(string.Format(
					"CharaIcon.SetIconSprite:\r\n" +
					"Sprite Not Found!! SpriteName = {0}", spriteName));
				return false;
			}
		}

		return true;
	}
	#endregion

	#region アイコン情報
	/// <summary>
	/// アイコン情報を取得する
	/// 失敗した場合は false を返す
	/// </summary>
	bool GetInfo(int iconMasterID, out string bundleName, out string spriteName)
	{
		bundleName = string.Empty;
		spriteName = string.Empty;

		// アイコン情報を取得する
		IconMasterData data;
		if (!MasterData.TryGetIcon(iconMasterID, out data))
		{
			Debug.LogWarning(string.Format(
				"Invalid ID\r\n" +
				"CharacterID = {0}", iconMasterID));
			return false;
		}

		bundleName = data.AssetPath;
		spriteName = data.Filename;

		return true;
	}
	#endregion
}



/// <summary>
/// アセットバンドル内の共通アイコンデータ
/// </summary>
public class CommonIconBundleData : BundleData
{
	const string IconAssetPath = "Atlas.prefab";
	const string MonoIconAssetPath = "AtlasMono.prefab";

	UIAtlas _icon = null;
	UIAtlas Icon { get { return _icon; } set { _icon = value; } }

	UIAtlas _monoIcon = null;
	UIAtlas MonoIcon { get { return _monoIcon; } set { _monoIcon = value; } }

	/// <summary>
	/// 全てのリソースを読み込んでいるかどうか
	/// </summary>
	protected override bool IsFinishAllResource()
	{
		if (this.Icon == null) return false;
		if (this.MonoIcon == null) return false;
		return true;
	}

	/// <summary>
	/// アイコンを取得する
	/// </summary>
	public void GetIcon(string bundleName, bool keepAssetReference, System.Action<UIAtlas> callback)
	{
		if (this.Icon == null)
		{
			this.GetAssetAsync<UIAtlas>(bundleName, IconAssetPath, keepAssetReference,
				(UIAtlas resource) =>
				{
					this.Icon = resource;
					if (callback != null) callback(resource);
				});
		}
		else
		{
			if (callback != null) callback(this.Icon);
		}
	}

	/// <summary>
	/// モノクロアイコンを取得する
	/// </summary>
	public void GetMonoIcon(string bundleName, bool keepAssetReference, System.Action<UIAtlas> callback)
	{
		if (this.MonoIcon == null)
		{
			this.GetAssetAsync<UIAtlas>(bundleName, MonoIconAssetPath, keepAssetReference,
				(UIAtlas resource) =>
				{
					this.MonoIcon = resource;
					if (callback != null) callback(resource);
				});
		}
		else
		{
			if (callback != null) callback(this.MonoIcon);
		}
	}
}
