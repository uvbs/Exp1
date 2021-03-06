/// <summary>
/// キャラボード(スキルボタン付き)
/// 
/// 2014/07/16
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.Master;

[System.Serializable]
public class CharaBoard
{
	#region フィールド＆プロパティ
	/// <summary>
	/// 現在管理中のキャラ情報ディクショナリ
    /// Avatar<->Skin<->Info
	/// </summary>
	Dictionary<AvatarType, Dictionary<int, Infomation>> InfoDict { get; set; }
	[System.Serializable]
	public class Infomation
	{
		public AvatarType avatarType;
        public int skinId;
		public string bundleName;
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
        this.InfoDict = new Dictionary<AvatarType, Dictionary<int, Infomation>>();
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
	public CharaBoard() { this.MemberInit(); }
	/// <summary>
	/// クリア
	/// </summary>
	public void Clear()
	{
		// リソースは参照切るだけで消える？
		this.MemberInit();
	}
	#endregion

	#region ボード取得
	/// <summary>
	/// ボードを取得する
	/// 同じアセットバンドル内で違うリソースを読み込む場合は keepAssetReference を true にしておくと読み込み済みのアセットバンドルを保持しておく
	/// その際アセットバンドル内の全てのリソースを読み終わったら内部でアセットバンドルを破棄している
	/// </summary>
	public void GetBoard(AvatarType avatarType, int skinId, bool keepAssetReference, System.Action<GameObject> callback)
	{
		// キャラ情報を取得する
		Infomation info;
		if (!this.GetCharaInfo(avatarType, skinId, out info))
		{
			Debug.LogWarning(string.Format(
				"Invalid Character ID\r\n" +
				"CharacterID = {0}({1})", (int)avatarType, avatarType));
			if (callback != null)
				callback(null);
			return;
		}

		var bundleData = this.BundleDataManager.GetBundleData<CharaBoardBundleData>(info.bundleName);
		if (bundleData != null)
		{
			bundleData.GetBoard(info.bundleName, keepAssetReference, callback);
		}
	}
	/// <summary>
	/// カットインを取得する
	/// 同じアセットバンドル内で違うリソースを読み込む場合は keepAssetReference を true にしておくと読み込み済みのアセットバンドルを保持しておく
	/// その際アセットバンドル内の全てのリソースを読み終わったら内部でアセットバンドルを破棄している
	/// </summary>
	public void GetCutIn(AvatarType avatarType, int skinId, bool keepAssetReference, System.Action<GameObject> callback)
	{
		// キャラ情報を取得する
		Infomation info;
		if (!this.GetCharaInfo(avatarType, skinId, out info))
		{
			Debug.LogWarning(string.Format(
				"Invalid Character ID\r\n" +
				"CharacterID = {0}({1})", (int)avatarType, avatarType));
			if (callback != null)
				callback(null);
			return;
		}

		var bundleData = this.BundleDataManager.GetBundleData<CharaBoardBundleData>(info.bundleName);
		if (bundleData != null)
		{
			bundleData.GetCutIn(info.bundleName, keepAssetReference, callback);
		}
	}

    public void GetGuideBoard(string bundleName, bool keepAssetReference, System.Action<GameObject> callback)
    {
        var bundleData = this.BundleDataManager.GetBundleData<CharaBoardBundleData>(bundleName);
        if (bundleData != null)
        {
            bundleData.GetBoard(bundleName, keepAssetReference, callback);
        }
    }
	#endregion

	#region キャラ情報
	/// <summary>
	/// キャラ情報を追加する
	/// 既に追加されているか追加に失敗した場合は false を返す
	/// </summary>
	public bool AddCharaInfo(AvatarType avatarType, int skinId, string bundleName)
	{
        // 登録されているかどうか
        Dictionary<int, Infomation> skinDict;
        if (!this.InfoDict.TryGetValue(avatarType, out skinDict)) {
            skinDict = new Dictionary<int, Infomation>();
            this.InfoDict.Add(avatarType, skinDict);
        }
		if (skinDict.ContainsKey(skinId))
			return false;
		// 不正なパラメータ
		if (string.IsNullOrEmpty(bundleName))
			return false;

		// 追加
		var info = new Infomation() { avatarType = avatarType, skinId = skinId, bundleName = bundleName, };
        skinDict.Add(skinId, info);
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
        if (!CharaMaster.Instance.IsValidCharacter((int)avatarType)) {
            return false;
        }
        AvatarMasterData avatar;
        if (!MasterData.TryGetAvatar((int)avatarType, skinId, out avatar)) {
            return false;
        }
		return this.AddCharaInfo(avatarType, skinId, avatar.BoardAssetPath);
	}
	/// <summary>
	/// キャラ情報を取得する
	/// 失敗した場合は false を返す
	/// </summary>
	bool GetCharaInfo(AvatarType avatarType, int skinId, out Infomation info)
	{
        // AvatarType をキーにキャラ情報を取得する
        Dictionary<int, Infomation> skinDict;
        if (!this.InfoDict.TryGetValue(avatarType, out skinDict)) {
            skinDict = new Dictionary<int, Infomation>();
            this.InfoDict.Add(avatarType, skinDict);
        }
		if (skinDict.TryGetValue(skinId, out info))
			return true;

		// 取得できなかったのでマスターデータから追加する
		if (!this.AddCharaInfo(avatarType, skinId))
		{
			// 何らかの追加エラー
			// マスターデータにデータが存在しないか
			// アセットバンドルのパスやアイコンファイル名が不正かも
			return false;
		}
		if (!skinDict.TryGetValue(skinId, out info))
		{
			// 何らかの不明なエラー
			return false;
		}

		return true;
	}
	#endregion
}



/// <summary>
/// アセットバンドル内のキャラボードデータ
/// </summary>
public class CharaBoardBundleData : BundleData
{
	const string BoardAssetPath = "Board.prefab";
	const string CutInAssetPath = "CutIn.prefab";

	GameObject board = null;
	GameObject cutIn = null;

	/// <summary>
	/// 全てのリソースを読み込んでいるかどうか
	/// </summary>
	protected override bool IsFinishAllResource()
	{
		if (this.board == null)
			return false;
		if (this.cutIn == null)
			return false;
		return true;
	}

	/// <summary>
	/// キャラボードを取得する
	/// </summary>
	public void GetBoard(string bundleName, bool keepAssetReference, System.Action<GameObject> callback)
	{
		if (this.board == null)
		{
			this.GetAssetAsync<GameObject>(bundleName, BoardAssetPath, keepAssetReference,
				(GameObject resource) =>
				{
					this.board = resource;
					if (callback != null)
						callback(resource);
				});
		}
		else
		{
			if (callback != null)
				callback(this.board);
		}
	}

    //新皮肤也要有board获取，这里要对getboard做扩展

	/// <summary>
	/// カットインを取得する
	/// </summary>
	public void GetCutIn(string bundleName, bool keepAssetReference, System.Action<GameObject> callback)
	{
		if (this.cutIn == null)
		{
			this.GetAssetAsync<GameObject>(bundleName, CutInAssetPath, keepAssetReference,
				(GameObject resource) =>
				{
					this.cutIn = resource;
					if (callback != null)
						callback(resource);
				});
		}
		else
		{
			if (callback != null)
				callback(this.cutIn);
		}
	}
}
