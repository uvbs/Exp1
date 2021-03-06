/// <summary>
/// アセットバンドルの参照クラス.
/// 
/// 2014/05/27
/// </summary>
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// アセットバンドルを扱う場合は必ずこのクラスを通すこと.
/// </summary>
public class AssetReference
{
#if UNITY_EDITOR && XW_DEBUG
	// エディタ上で使う変数.
	const string LoadAssetPathUnused = "Assets/ScmAssets/Bundle_Unused/";

	/// <summary>
	/// リソースのロードをアセットバンドルではなくローカルフォルダから行う.
	/// </summary>
	static public bool LoadFromLocalFolder
	{
		get
		{
			return ScmParam.Debug.File.LoadFromLocalFolder;
		}
	}
#endif
	public const string LoadAssetPath = "Assets/ScmAssets/Bundle/";
	private const string SharedAssetName = "_share";
	public const string CommonAssetName = "common";
	public const string BundleExt = ".unity3d";

	#region enum LoadState
	/// <summary>
	/// 現在のローディング状態
	/// </summary>
	public enum LoadState
	{
		Non,
		Wait,
		NowLoading,
		Success,
		Failed,
	};
	#endregion

	#region StaticMember
	/// <summary>
	/// アセットバンドルの参照キャッシュ.
	/// </summary>
	// 使用しなくなった際もkey文字列とnull参照サイズのメモリを喰ったままになるため,扱う数が莫大になる場合は定期的にクリーン処理を入れる.
	// 同じPathのアセットバンドルを再ロードする場合は上書きになるため,何度も繰り返しても問題ない.
	static private Dictionary<string, WeakReference> bundleCache = new Dictionary<string, WeakReference>();

	/// <summary>
	/// 共有アセットバンドル.
	/// 参照を持っていることに意味がある.
	/// </summary>
	#pragma warning disable 0414
	static private AssetReference sharedAssetReference;
	static private AssetReference commonAssetReference;

	/// <summary>
	/// Sets the dependent.
	/// </summary>
	static public void SetSharedAssetReference()
	{
		sharedAssetReference = GetAssetReference(SharedAssetName);
		commonAssetReference = GetAssetReference(CommonAssetName);
	}

	/// <summary>
	/// AssetReferenceを取得する.
	/// </summary>
	static public AssetReference GetAssetReference(string bundlePath)
	{
		AssetReference assetReference = null;
		
		WeakReference wref;
		if(AssetReference.bundleCache.TryGetValue(bundlePath, out wref))
		{
			if(wref.IsAlive)
			{
				// 一応この瞬間にnullになる可能性を考慮して,ここではreturnしない.
				assetReference = wref.Target as AssetReference;
			}
		}
		
		if(assetReference == null)
		{
			// privateコンストラクタと合わせてここでしかAssetReferenceを取得できないようにし,1Path1Referenceを徹底する.
			assetReference = new AssetReference(bundlePath);
			AssetReference.bundleCache[bundlePath] = new WeakReference(assetReference);
		}
		
		return assetReference;
	}

	/// <summary>
	/// 【危険】アセットバンドルの参照をクリアする.
	/// アセットバンドルの使用中に呼ぶとおかしくなる.
	/// アセットバンドルダウンロード直後などに呼ぶ.
	/// </summary>
	static public void ReferenceClear()
	{
		AssetReference.bundleCache.Clear();
	}

	/// <summary>
	/// アセットバンドルのPathを取得する(主にアセットバンドルと同時にDLしてくる独自ファイル読み込み用).
	/// </summary>
	static public string GetAssetBundlePath(string filename)
	{
#if UNITY_EDITOR && XW_DEBUG
		if(LoadFromLocalFolder)
		{
			return GetLocalAssetBundlePath(filename);
		}
#endif
		return FileBase.GetFilePath(AssetBundleVersions.PATH, filename);
	}

#if UNITY_EDITOR && XW_DEBUG
	/// <summary>
	/// 【Editor専用】アセットバンドルのLocalPathを取得する.
	/// </summary>
	static private string GetLocalAssetBundlePath(string filename)
	{
		string ret = LoadAssetPath + filename;
		if(System.IO.File.Exists(System.IO.Path.GetFullPath(ret)))
		{
			return ret;
		}
		else
		{
			//Debug.LogWarning("【UnusedAssetBundle】" + filename);
			// 開発用に未使用アセット内も探す.
			string unused = LoadAssetPathUnused + filename;
			if(System.IO.File.Exists(System.IO.Path.GetFullPath(unused)))
			{
				ret = unused;
			}
			return ret;
		}
	}
#endif
	#endregion

	#region NonStaticMember 
	// フィールド.
	private readonly string bundlePath;
	private AssetBundle assetBundle;
	// 自分の依存しているアセットバンドル.
	private AssetReference dependentAssetReference;

	// プロパティ

	/// <summary>
	/// アセットバンドルのロード状態.
	/// </summary>
	public LoadState loadState { get; private set; }
	
	/// <summary>
	/// 読み込んでいるアセットバンドルのパス.
	/// </summary>
	public string BundlePath { get { return bundlePath; } }
	
	/// <summary>
	/// 新たにアセットバンドルの読み込みを開始して良いかどうか.
	/// </summary>
	private bool CanStartLoadBundle
	{
		get
		{
			if(loadState == LoadState.Non)
			{
				return true;
			}
			return false;
		}
	}

	/// <summary>
	/// アセットバンドルの読み込みが正常に終了したかどうか.
	/// </summary>
	public bool IsFinish
	{
		get
		{
			if(loadState == LoadState.Success || loadState == LoadState.Failed)
			{
				return true;
			}
			return false;
		}
	}

	// GetAssetReference()以外でインスタンスを作るとマズいのでprivate.
	private AssetReference(string bundlePath)
	{
		this.loadState = LoadState.Non;
		this.bundlePath = bundlePath;
		this.StartLoadAssetBundle();
	}

	~AssetReference()
	{
		// GCスレッドではUnload出来ないのでコルーチンに任せる.
		//Debug.Log("~"+this.bundlePath);
		FiberController.AddFiber(AssetReference.Dispose(this.assetBundle));
	}

	static private IEnumerator Dispose(AssetBundle assetBundle)
	{
		if(assetBundle)
		{
			//1PathにつきReferenceの実体は1つしかないはずなので,GCっぽくBundleUnloadできるはず.
			//Debug.Log("Dispose");
			assetBundle.Unload(false);
			assetBundle = null;
		}
		yield break;
	}

	public void StartLoadAssetBundle()
	{
		if(this.CanStartLoadBundle)
		{
			FiberController.AddFiber(this.LoadAssetBundle());
		}
	}
	private IEnumerator LoadAssetBundle()
	{
#if UNITY_EDITOR && XW_DEBUG
		if(LoadFromLocalFolder)
		{
			loadState = LoadState.Success;
			yield break;
		}
#endif
        Debug.Log("<color=#00ff00>load from remote:" + this.bundlePath + "</color>");
        loadState = LoadState.NowLoading;

		// 依存しているアセットバンドルも読む.
		if(this.bundlePath != SharedAssetName)
		{
			this.dependentAssetReference = GetAssetReference(SharedAssetName);	// UNDONE: 今のところ全て共有の直子とする.
		}

        string fullpath = FileBase.GetFilePath(AssetBundleVersions.PATH, this.bundlePath + BundleExt);

        Debug.Log("Loading: " + fullpath);
        using (WWW www = new WWW("file:" + fullpath)) 
        {
            //Debug.Log(www.url);
            while (!www.isDone) {
                yield return null;
            }

            try {
                AssetBundle bundle = www.assetBundle;       // wwwで失敗した場合は？.多重読み込みの場合も問題に.ここが結構重い?

                assetBundle = bundle;
            } catch (Exception e) {
                BugReportController.SaveLogFile(e.ToString());
                loadState = LoadState.Failed;
                yield break;
            }
        }

		if(this.assetBundle == null)
		{
			loadState = LoadState.Failed;
			yield break;
		}

		// 依存しているアセットバンドルの状態チェック
		if(this.dependentAssetReference != null)
		{
			while(!this.dependentAssetReference.IsFinish)
			{
				yield return null;
			}
			if(this.dependentAssetReference.loadState != LoadState.Success)
			{
				// 依存しているアセットバンドルが失敗.
				loadState = LoadState.Failed;
				yield break;
			}
		}

		// 成功.
		loadState = LoadState.Success;
	}

	/// <summary>
	/// アセットを読み込む.
	/// </summary>
	public T GetAsset<T>(string assetPath) where T : UnityEngine.Object
	{
#if UNITY_EDITOR && XW_DEBUG
		if(LoadFromLocalFolder)
		{
			// リソーステスト用.作成時にいちいちアセットバンドルにしてたら手間なので.
			//string path = LoadAssetPath + this.bundlePath + "/" + assetPath;
			string path = GetLocalAssetBundlePath(this.bundlePath + "/" + assetPath);
			T asset = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
			if(asset == null)
			{
				string log = "Asset Load Failed : " + path;
				Debug.LogError(log);
				BugReportController.SaveLogFile(log);
			}
			return asset;
		}
#endif
		if((this.loadState == LoadState.Success))
		{
			assetPath = LoadAssetPath + bundlePath + "/" + assetPath;
			if(this.assetBundle.Contains(assetPath))
			{
				var obj = this.assetBundle.LoadAsset(assetPath, typeof(UnityEngine.Object));
				T asset = obj as T;

				// HACK: Unity5でコンポーネントを直接LoadAssetAsync出来なくなった件の応急処置.
				if(asset == null)
				{
					GameObject go = obj as GameObject;
					if(go != null)
					{
						asset = go.GetComponent<T>();
					}
				}
				return asset;
			}
			else
			{
				// アセットバンドル内に存在しない.
				string log = "Asset Not Contained : " + this.bundlePath + "@" + assetPath;
				Debug.LogError(log);
				BugReportController.SaveLogFile(log);
			}
		}
		else
		{
			// LoadStateがSuccessじゃない.
			string log = "Can't Load. State : " + this.loadState + ", " + this.bundlePath + "@" + assetPath;
			Debug.LogError(log);
			BugReportController.SaveLogFile(log);
		}
		return null;
	}

	/// <summary>
	/// アセットを非同期読み込みする.
	/// </summary>
	public IEnumerator GetAssetAsync<T>(string assetPath, System.Action<T> callback) where T : UnityEngine.Object
	{
#if UNITY_EDITOR && XW_DEBUG
		if(LoadFromLocalFolder)
		{
			// リソーステスト用.作成時にいちいちアセットバンドルにしてたら手間なので.
			//string path = LoadAssetPath + this.bundlePath + "/" + assetPath;
			string path = GetLocalAssetBundlePath(this.bundlePath + "/" + assetPath);
			T asset = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
			if(asset == null)
			{
				string log = "Asset Load Failed : " + path;
				Debug.LogError(log);
				BugReportController.SaveLogFile(log);
			}
            // 成功ならasset 失敗ならnullを返す.
            Debug.Log("<color=#00ff00>GetAssetAsync:" + bundlePath + "@" + assetPath + ":" + asset + "</color>");
            callback(asset);
            yield break;
		}
#endif
		// アセットバンドルの読み込み中なら終了まで待つ.
		while(!this.IsFinish)
		{
			yield return null;
		}
		if(this.loadState != LoadState.Success)
		{
			// アセットバンドル読み込みに失敗.
			string log = "Null AssetBundle : " + this.bundlePath + ",state : " + this.loadState;
			Debug.LogError(log);
			BugReportController.SaveLogFile(log);
            Debug.Log("<color=#ff0000>GetAssetAsync:" + bundlePath + "@" + assetPath + ":failed</color>");
            callback(null);
            yield break;
		}

		assetPath = LoadAssetPath + bundlePath + "/" + assetPath;
		if(this.assetBundle.Contains(assetPath))
		{
            // HACK: Huhao,2016/09/30  Load from remote asset bundle using LoadAssetAsync involving the "GameObject references runtime script in scene file, fixing." bug,
            //  which may cause game hang up.
            // REF: https://forum.unity3d.com/threads/asset-bundle-errors-on-load-gameobject-references-runtime-script-in-scene-file-fixing.316801/
            UnityEngine.Object obj = this.assetBundle.LoadAsset(assetPath, typeof(UnityEngine.Object));
            T asset = obj as T;
			// HACK: Unity5でコンポーネントを直接LoadAssetAsync出来なくなった件の応急処置.
			if(asset == null)
			{
                GameObject go = asset as GameObject;
                if (go != null)
				{
					asset = go.GetComponent<T>();
				}
			}

			if(asset == null)
			{
				// アセットが見つからない(ContainsはtrueなのでType間違い?).
				string log = "Asset Not Found : " + this.bundlePath + "@" + assetPath;
				Debug.LogError(log);
				BugReportController.SaveLogFile(log);
				callback(null);
			}
			else
			{
				// 正常に読み込めた.
				callback(asset);
                Debug.Log("Loaded asset:" + assetPath + ":" + asset);
			}
		}
		else
		{
			// アセットバンドル内に存在しない.
			string log = "Asset Not Contained : " + this.bundlePath + "@" + assetPath;
			Debug.LogError(log);
			BugReportController.SaveLogFile(log);
			callback(null);
		}
	}
	#endregion
}