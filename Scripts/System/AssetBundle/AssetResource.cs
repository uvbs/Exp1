/// <summary>
/// アセットバンドル内のリソース読み込み
/// リソース読み込みラッパークラス
/// キャラクターアイコンなどで同じリソースを複数回取得したい場合に使用する
/// 
/// コンストラクタで読み込みを開始し FiberController で読み込みをするので
/// FiberController が存在する限りリソースの読み込みは保証される
/// 
/// 2014/07/16
/// </summary>
using UnityEngine;
using System.Collections;

public class AssetResource<T> where T : UnityEngine.Object
{
	#region フィールド＆プロパティ
	/// <summary>
	/// 読み込みが終了したかどうか
	/// </summary>
	public bool IsFinish { get; private set; }
	/// <summary>
	/// 読み込み対象のアセットパス
	/// </summary>
	public string AssetPath { get; private set; }

	// アセットバンドルから読み込んだリソース
	T Resource { get; set; }

	// メンバー初期化
	public void MemberInit()
	{
		this.IsFinish = false;
		this.AssetPath = null;
		this.Clear();
	}
	#endregion

	#region 初期化
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public AssetResource(AssetReference assetReference, string assetPath, System.Action<AssetResource<T>, T> callback)
	{
		this.MemberInit();
		this.AssetPath = assetPath;
		// 読み込みを開始して FiberController に読み込み処理を委譲する
		FiberController.AddFiber(this.GetAssetAsync(assetReference, assetPath, callback));
	}
	/// <summary>
	/// AssetReference.GetAssetAsync のラッパーメソッド
	/// </summary>
	IEnumerator GetAssetAsync(AssetReference assetReference, string assetPath, System.Action<AssetResource<T>, T> callback)
	{
		this.Clear();
		var fiber = assetReference.GetAssetAsync<GameObject>(assetPath, (GameObject go) =>
			{
				// 読み込み完了
				this.IsFinish = true;
				// 本来の型に変換する
				T resource = null;
				if (go != null)
				{
					resource = go as T;
					// Unity5 から as でコンポーネントを取得できないのが仕様になった
					if (resource == null)
						resource = go.GetComponent<T>();
				}
				this.Resource = resource;
				if (callback != null)
					callback(this, resource);
			});
		return fiber;
	}
	/// <summary>
	/// リソースクリア
	/// </summary>
	public void Clear()
	{
		// アセットバンドルから読み込んだリソースは null を入れたらクリアされる？
		this.Resource = null;
	}
	#endregion

	#region リソース取得
	/// <summary>
	/// リソースを取得する
	/// 読み込みが終了しているか返す(終了していても読み込みエラーの時はリソースは null)
	/// </summary>
	public bool GetResource(out T resource)
	{
		resource = this.Resource;
		return this.IsFinish;
	}
	/// <summary>
	/// リソースの読み込みが終わるまで待ってから取得する
	/// リソースが null の場合でも callback は呼ぶ
	/// </summary>
	public IEnumerator GetResource(System.Action<AssetResource<T>, T> callback)
	{
		while (!this.IsFinish)
			yield return null;

		if (callback != null)
			callback(this, this.Resource);
	}
	#endregion
}


