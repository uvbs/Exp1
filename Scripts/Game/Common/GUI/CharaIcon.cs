/// <summary>
/// キャラアイコン
/// 
/// 2014/07/16
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.Master;

[System.Serializable]
public class CharaIcon
{
	#region フィールド＆プロパティ
	/// <summary>
	/// 現在管理中のキャラ情報ディクショナリ
	/// </summary>
	Dictionary<AvatarType, Infomation> InfoDict { get; set; }
	[System.Serializable]
	public class Infomation
	{
		public AvatarType avatarType;
		public string bundleName;
		public string spriteName;
	}

	/// <summary>
	/// 複数のアセットバンドルの管理
	/// </summary>
	BundleDataManager BundleDataManager { get; set; }

	/// <summary>
	/// メンバー初期化
	/// </summary>
	void MemberInit()
	{
		this.InfoDict = new Dictionary<AvatarType, Infomation>();
		this.BundleDataManager = new BundleDataManager();
	}

#if UNITY_EDITOR && XW_DEBUG
	[SerializeField]
	DebugParam _debug = new DebugParam();
	[System.Serializable]
	public class DebugParam
	{
		public List<Infomation> charaList = new List<Infomation>();
	}
#endif
	#endregion

	#region 初期化
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public CharaIcon() { this.MemberInit(); }
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
	public void GetIcon(AvatarType avatarType, int skinId, bool keepAssetReference, System.Action<UIAtlas, string> callback)
	{
        
		// キャラ情報を取得する
		Infomation info;
		if (!this.GetCharaInfo(avatarType, skinId, out info))
		{
			Debug.LogWarning(string.Format(
				"Invalid Character ID\r\n" +
				"CharacterID = {0}({1})", (int)avatarType, avatarType));
			if (callback != null)
				callback(null, "");
			return;
		}

        
        AvatarMasterData avatar;
        if (!MasterData.TryGetAvatar((int)avatarType, skinId, out avatar))
        {
            return;
        }
        /*
	    if (callback != null)
	    {
	        callback(AtlasHold.Instance.GetHeroIcon(avatar.IconFile), avatar.IconFile);
	    }
        */
        var bundleData = this.BundleDataManager.GetBundleData<CharaIconBundleData>(avatar.IconAssetPath);
		if (bundleData != null)
		{
            bundleData.GetIcon(avatar.IconAssetPath, keepAssetReference,
				(UIAtlas res) =>
				{
					if (callback != null)
                        callback(res, avatar.IconFile);
				});
		}
	}

    public void GetBustIcon(AvatarType avatarType, int skinId, bool keepAssetReference, System.Action<UIAtlas, string> callback)
    {
        Infomation info;
        if (!this.GetCharaInfo(avatarType, skinId, out info))
        {
            Debug.LogWarning(string.Format(
                "Invalid Character ID\r\n" +
                "CharacterID = {0}({1})", (int)avatarType, avatarType));
            if (callback != null)
                callback(null, "");
            return;
        }

        AvatarMasterData avatar;
        if (!MasterData.TryGetAvatar((int)avatarType, skinId, out avatar))
        {
            return;
        }

        var bundleData = this.BundleDataManager.GetBundleData<CharaIconBundleData>(avatar.IconAssetPath);
        if (bundleData != null)
        {
            bundleData.GetBustIcon(avatar.ImagePath, keepAssetReference,
                (UIAtlas res) =>
                {
                    if (callback != null)
                        callback(res, avatar.IconFile);
                });
        }
    }

	/// <summary>
	/// モノクロアイコンを取得する
	/// 同じアセットバンドル内で違うリソースを読み込む場合は keepAssetReference を true にしておくと読み込み済みのアセットバンドルを保持しておく
	/// その際アセットバンドル内の全てのリソースを読み終わったら内部でアセットバンドルを破棄している
	/// </summary>
	public void GetMonoIcon(AvatarType avatarType, int skinId, bool keepAssetReference, System.Action<UIAtlas, string> callback)
	{
		// キャラ情報を取得する
		Infomation info;
		if (!this.GetCharaInfo(avatarType, skinId, out info))
		{
			Debug.LogWarning(string.Format(
				"Invalid Character ID\r\n" +
				"CharacterID = {0}({1})", (int)avatarType, skinId));
			if (callback != null)
				callback(null, "");
			return;
		}

		var bundleData = this.BundleDataManager.GetBundleData<CharaIconBundleData>(info.bundleName);
		if (bundleData != null)
		{
			bundleData.GetMonoIcon(info.bundleName, keepAssetReference,
				(UIAtlas res) =>
				{
					if (callback != null)
						callback(res, info.spriteName);
				});
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

	#region キャラ情報
	/// <summary>
	/// キャラ情報を追加する
	/// 既に追加されているか追加に失敗した場合は false を返す
	/// </summary>
	public bool AddCharaInfo(AvatarType avatarType, string bundleName, string spriteName)
	{
		// 登録されているかどうか
		if (this.InfoDict.ContainsKey(avatarType))
			return false;
		// 不正なパラメータ
		if (string.IsNullOrEmpty(bundleName))
			return false;
		// 不正なパラメータ
		if (string.IsNullOrEmpty(spriteName))
			return false;

		// 追加
		var info = new Infomation() { avatarType = avatarType, bundleName = bundleName, spriteName = spriteName, };
		this.InfoDict.Add(avatarType, info);
#if UNITY_EDITOR && XW_DEBUG
		this._debug.charaList.Add(info);
#endif
		return true;
	}
	/// <summary>
	/// マスターデータからキャラ情報を追加する
	/// 追加に失敗した場合は false を返す
	/// </summary>
	bool AddCharaInfo(AvatarType avatarType, int skinId)
	{
		// マスターデータから追加する
		CharaMasterData data;
		if (!MasterData.TryGetChara((int)avatarType, out data))
			return false;
        AvatarMasterData avatar;
        if (!MasterData.TryGetAvatar((int)avatarType, skinId, out avatar)) {
            return false;
        }
		return this.AddCharaInfo(avatarType, avatar.IconAssetPath, avatar.IconFile);
	}
	/// <summary>
	/// キャラ情報を取得する
	/// 失敗した場合は false を返す
	/// </summary>
	bool GetCharaInfo(AvatarType avatarType, int skinId, out Infomation info)
	{
		// AvatarType をキーにキャラ情報を取得する
		if (this.InfoDict.TryGetValue(avatarType, out info))
			return true;

		// 取得できなかったのでマスターデータから追加する
		if (!this.AddCharaInfo(avatarType, skinId))
		{
			// 何らかの追加エラー
			// マスターデータにデータが存在しないか
			// アセットバンドルのパスやアイコンファイル名が不正かも
			return false;
		}
		if (!this.InfoDict.TryGetValue(avatarType, out info))
		{
			// 何らかの不明なエラー
			return false;
		}

		return true;
	}
	#endregion
}



/// <summary>
/// アセットバンドル内のキャラアイコンデータ
/// </summary>
public class CharaIconBundleData : BundleData
{
	const string IconAssetPath = "Atlas.prefab";
	const string MonoIconAssetPath = "AtlasMono.prefab";

	UIAtlas icon = null;
	UIAtlas monoIcon = null;
    UIAtlas bustIcon = null;
	/// <summary>
	/// 全てのリソースを読み込んでいるかどうか
	/// </summary>
	protected override bool IsFinishAllResource()
	{
		if (this.icon == null)
			return false;
		if (this.monoIcon == null)
			return false;
		return true;
	}

	/// <summary>
	/// アイコンを取得する
	/// </summary>
	public void GetIcon(string bundleName, bool keepAssetReference, System.Action<UIAtlas> callback)
	{
		if (this.icon == null)
		{
			this.GetAssetAsync<UIAtlas>(bundleName, IconAssetPath, keepAssetReference,
				(UIAtlas resource) =>
				{
					this.icon = resource;
					if (callback != null)
						callback(resource);
				});
		}
		else
		{
			if (callback != null)
				callback(this.icon);
		}
	}

    public void GetBustIcon(string bundleName, bool keepAssetReference, System.Action<UIAtlas> callback)
    {
        if (this.bustIcon == null)
        {
            this.GetAssetAsync<UIAtlas>(bundleName, IconAssetPath, keepAssetReference,
                (UIAtlas resource) =>
                {
                    this.bustIcon = resource;
                    if (callback != null)
                        callback(resource);
                });
        }
        else
        {
            if (callback != null)
                callback(this.bustIcon);
        }
    }


    /// <summary>
    /// モノクロアイコンを取得する
    /// </summary>
    public void GetMonoIcon(string bundleName, bool keepAssetReference, System.Action<UIAtlas> callback)
	{
		if (this.monoIcon == null)
		{
			this.GetAssetAsync<UIAtlas>(bundleName, MonoIconAssetPath, keepAssetReference,
				(UIAtlas resource) =>
				{
					this.monoIcon = resource;
					if (callback != null)
						callback(resource);
				});
		}
		else
		{
			if (callback != null)
				callback(this.monoIcon);
		}
	}
}
