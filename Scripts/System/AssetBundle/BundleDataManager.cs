/// <summary>
/// アセットバンドルを複数管理する
/// 
/// 2015/06/02
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// アセットバンドルを複数管理する
/// </summary>
public class BundleDataManager
{
	Dictionary<string, BundleData> _bundleDataDict = new Dictionary<string, BundleData>();

	/// <summary>
	/// BundleData を取得する
	/// </summary>
	public T GetBundleData<T>(string bundleName) where T : BundleData, new()
	{
		BundleData bundleData = null;
		if (!this._bundleDataDict.TryGetValue(bundleName, out bundleData))
		{
			bundleData = new T();
			this._bundleDataDict.Add(bundleName, bundleData);
		}
		return bundleData as T;
	}
}

/// <summary>
/// アセットバンドル内のリソースデータ基底クラス
/// </summary>
public abstract class BundleData
{
	/// <summary>
	/// 読み込んだアセットバンドルを保持しておく用
	/// </summary>
	AssetReference _assetReference = null;

	/// <summary>
	/// コンストラクタ
	/// </summary>
	public BundleData() { }

	/// <summary>
	/// 全てのリソースを読み込んでいるかどうか
	/// </summary>
	protected abstract bool IsFinishAllResource();

	/// <summary>
	/// アセットの非同期読み込み
	/// </summary>
	protected void GetAssetAsync<T>(string bundleName, string assetPath, bool keepAssetReference, System.Action<T> callback) where T : UnityEngine.Object
	{
		var assetReference = this.GetAssetReference(bundleName, keepAssetReference);
		var fiber = assetReference.GetAssetAsync<GameObject>(assetPath,
			(GameObject go) =>
			{
				// 本来の型に変換する
				T resource = null;
				if (go != null)
				{
					resource = go as T;
					// Unity5 から as でコンポーネントを取得できないのが仕様になった
					if (resource == null)
						resource = go.GetComponent<T>();
				}
				else
				{
					// リソースが読み込めなかった
					Debug.LogWarning(string.Format(
						"Resource Loading Error:\r\n" +
						"bundleName = {0}, assetPath = {1}", bundleName, assetPath));
				}

				if (callback != null)
					callback(resource);

				// 保持している AssetReference 破棄チェック
				this.AssetReferenceClearCheck();
			});
		FiberController.AddFiber(fiber);
	}

    static void GetAssetAsync(string bundleName, string assetPath, bool keepAssetReference, System.Action callback)
    {

    }

	/// <summary>
	/// 保持してある AssetReference の取得
	/// 保持してなければ作成する
	/// </summary>
	AssetReference GetAssetReference(string bundleName, bool keepAssetReference)
	{
		// 保持している AssetReference があるかどうか
		AssetReference assetReference = this._assetReference;
		if (assetReference == null)
		{
			// アセットバンドルの読み込み開始
			assetReference = AssetReference.GetAssetReference(bundleName);
			// AssetReference を保持しておくかどうか
			// アセットバンドル内のリソースを一つだけ読み込んで使用する場合は false の方がいい
			// 同じアセットバンドル内で他のリソースも読み込む場合は true の方がいい
			if (keepAssetReference)
				this._assetReference = assetReference;
		}

		return assetReference;
	}

	/// <summary>
	/// 保持している AssetReference の破棄チェック
	/// 全てのリソースを読み込んでいる場合は不要になるので破棄する
	/// </summary>
	void AssetReferenceClearCheck()
	{
		if (this._assetReference == null)
			return;
		if (!this.IsFinishAllResource())
			return;

		this._assetReference = null;
	}
}
