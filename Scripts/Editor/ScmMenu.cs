/// <summary>
/// エディターメニュー
/// 
/// 2013/05/02
/// </summary>
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

static public class ScmMenu
{
	#region Map
	#region Add BoxCollider
	/// <summary>
	/// ReferenceData 内のメッシュに合わせて BoxCollider を追加する
	/// </summary>
	[MenuItem("Scm/Map/Add BoxCollider")]
	static public void AddBoxCollider()
	{
		var gos = new List<GameObject>(Selection.gameObjects);
		foreach (var go in gos)
		{
			// ReferenceData のデータを読み込む
			ReferenceData rdata = CreatePrefabReferenceData(go);
			if (rdata == null)
			{ continue; }

			// メッシュレンダラーがあるオブジェクト取得
			MeshRenderer mr = go.GetComponentInChildren<MeshRenderer>();
			if (mr == null)
			{ continue; }

			// メッシュからコライダーを作成する
			{
				BoxCollider meshBc = mr.gameObject.AddComponent<BoxCollider>();
				BoxCollider rootBc = go.GetComponent<BoxCollider>();
				if (rootBc == null)
				{
					rootBc = go.AddComponent<BoxCollider>();
				}
				Transform t = mr.transform;
				rootBc.size = new Vector3(meshBc.size.x * t.localScale.x, meshBc.size.y * t.localScale.y, meshBc.size.z * t.localScale.z);
				rootBc.center = new Vector3(meshBc.center.x * t.localScale.x, meshBc.center.y * t.localScale.y, meshBc.center.z * t.localScale.z);
				// メッシュに追加したコライダー破棄
				UnityEngine.Object.DestroyImmediate(meshBc);
			}

			// 読み込んだデータ破棄
			RemoveChild(rdata.gameObject);
		}
	}
	#endregion

	#region Add SphereCollider
	/// <summary>
	/// ReferenceData 内のメッシュに合わせて SphereCollider を追加する
	/// </summary>
	[MenuItem("Scm/Map/Add SphereCollider")]
	static public void AddSphereCollider()
	{
		var gos = new List<GameObject>(Selection.gameObjects);
		foreach (var go in gos)
		{
			// ReferenceData のデータを読み込む
			ReferenceData rdata = CreatePrefabReferenceData(go);
			if (rdata == null)
			{ continue; }
			rdata.enabled = true;

			// メッシュレンダラーがあるオブジェクト取得
			MeshRenderer mr = go.GetComponentInChildren<MeshRenderer>();
			if (mr == null)
			{ continue; }

			// メッシュからコライダーを作成する
			{
				SphereCollider meshSc = mr.gameObject.AddComponent<SphereCollider>();
				SphereCollider rootSc = go.GetComponent<SphereCollider>();
				if (rootSc == null)
				{
					rootSc = go.AddComponent<SphereCollider>();
				}
				Transform t = mr.transform;
				rootSc.radius = meshSc.radius * Mathf.Max(t.localScale.x, Mathf.Max(t.localScale.y, t.localScale.z));
				rootSc.center = new Vector3(meshSc.center.x * t.localScale.x, meshSc.center.y * t.localScale.y, meshSc.center.z * t.localScale.z);
				// メッシュに追加したコライダー破棄
				UnityEngine.Object.DestroyImmediate(meshSc);
			}

			// 読み込んだデータ破棄
			RemoveChild(rdata.gameObject);
		}
	}
	#endregion
	#endregion

	#region UI
	#region Add/Delete GlobalTouchFilter
	/// <summary>
	/// 選択しているオブジェクトの子供に Collider が付いている場合は GUIGlobalTouchFilter を追加
	/// </summary>
	[MenuItem("Scm/UI/Add GlobalTouchFilter")]
	static public void AddGlobalTouchFilter()
	{
		// 開始ログ
		Debug.Log("Add GUIGlobalTouchFilter");
		int count = 0;

		var gos = new List<GameObject>(Selection.gameObjects);
		foreach (var go in gos)
		{
			// Collider が付いているオブジェクトにだけスクリプトをつける
			var coms = new List<Collider>(go.GetComponentsInChildren<Collider>(true));
			foreach (var com in coms)
			{
				count++;
				com.gameObject.AddComponent<GUIGlobalTouchFilter>();
				Debug.Log("\r\n" + GetPath(go, com.transform));
			}
		}

		// 終了ログ
		Debug.Log("Add Count = " + count);
	}
	/// <summary>
	/// 選択しているオブジェクトの子供をに付いている GUIGlobalTouchFilter を削除する
	/// </summary>
	[MenuItem("Scm/UI/Delete GlobalTouchFilter")]
	static public void DeleteGlobalTouchFilter()
	{
		// 開始ログ
		Debug.Log("Del GUIGlobalTouchFilter");
		int count = 0;

		var gos = new List<GameObject>(Selection.gameObjects);
		foreach (var go in gos)
		{
			// スクリプトが付いているオブジェクトを消す
			var coms = new List<GUIGlobalTouchFilter>(go.GetComponentsInChildren<GUIGlobalTouchFilter>(true));
			foreach (var com in coms)
			{
				count++;
				Debug.Log("\r\n" + GetPath(go, com.transform));
				UnityEngine.Object.DestroyImmediate(com);
			}
		}

		// 終了ログ
		Debug.Log("Delete Count = " + count);
	}
	#endregion

	#region To XUIButton/UIButton
	/// <summary>
	/// UIButton を XUIButton に置き換える
	/// </summary>
	[MenuItem("Scm/UI/To XUIButton")]
	public static void ToXUIButton()
	{
		// 開始ログ
		Debug.Log("To XUIButton");
		int count = 0;

		var gos = new List<GameObject>(Selection.gameObjects);
		foreach (var go in gos)
		{
			var coms = new List<UIButton>(go.GetComponentsInChildren<UIButton>(true));
			foreach (var com in coms)
			{
				if (com is XUIButton)
					continue;
				count++;
				Debug.Log("\r\n" + GetPath(go, com.transform));
				NGUIEditorTools.ReplaceClass(com, typeof(XUIButton));
			}
		}

		// 終了ログ
		Debug.Log("UIButton = " + count);
	}
	/// <summary>
	/// XUIButton を UIButton に置き換える
	/// </summary>
	[MenuItem("Scm/UI/To UIButton")]
	public static void ToUIButton()
	{
		// 開始ログ
		Debug.Log("To UIButton");
		int count = 0;

		var gos = new List<GameObject>(Selection.gameObjects);
		foreach (var go in gos)
		{
			var coms = new List<XUIButton>(go.GetComponentsInChildren<XUIButton>(true));
			foreach (var com in coms)
			{
				count++;
				Debug.Log("\r\n" + GetPath(go, com.transform));
				NGUIEditorTools.ReplaceClass(com, typeof(UIButton));
			}
		}

		// 終了ログ
		Debug.Log("UIButton = " + count);
	}
	#endregion

	#region Check TweenGroup
	/// <summary>
	/// TweenGroup番号のチェック
	/// </summary>
	[MenuItem("Scm/UI/Check TweenGroup")]
	public static void CheckTweenGroup()
	{
		var gos = new List<GameObject>(Selection.gameObjects);
		foreach (var go in gos)
		{
			// PlayTween系のグループを確認
			{
				var coms = new List<UIPlayTween>(go.GetComponentsInChildren<UIPlayTween>(true));
				foreach (var com in coms)
				{
					Debug.Log(string.Format("\r\nUIPlayTween({0}):{1}", com.tweenGroup, GetPath(go, com.transform)));
				}
			}
			// UITweener系のグループを確認
			{
				var coms = new List<UITweener>(go.GetComponentsInChildren<UITweener>(true));
				foreach (var com in coms)
				{
					Debug.Log(string.Format("\r\nUITweener({0}):{1}", com.tweenGroup, GetPath(go, com.transform)));
				}
			}
		}
	}
	#endregion

	#region ignoreTimeScale ON/OFF
	/// <summary>
	/// Tween系のignoreTimeScaleのオンオフ設定
	/// </summary>
	[MenuItem("Scm/UI/ignoreTimeScale On (NGUI Default)")]
	public static void TweenIgnoreTimeScaleOn()
	{
		TweenIgnoreTimeScale(true);
	}
	[MenuItem("Scm/UI/ignoreTimeScale Off")]
	public static void TweenIgnoreTimeScaleOff()
	{
		TweenIgnoreTimeScale(false);
	}
	public static void TweenIgnoreTimeScale(bool isTimeScale)
	{
		int count = 0;
		var gos = new List<GameObject>(Selection.gameObjects);
		foreach (var go in gos)
		{
			// UITweener系のグループを確認
			var coms = new List<UITweener>(go.GetComponentsInChildren<UITweener>(true));
			foreach (var com in coms)
			{
				count++;
				com.ignoreTimeScale = isTimeScale;
				Debug.Log(string.Format("\r\nUITweener:{0}", GetPath(go, com.transform)));
			}
		}

		Debug.Log(string.Format("UITweener.ignoreTimeScale = {0}\r\ncount = {1}", isTimeScale, count));
	}
	#endregion

	#region Check Label
	/// <summary>
	/// ラベルの内容チェック
	/// </summary>
	[MenuItem("Scm/UI/Check Label")]
	public static void CheckLabel()
	{
		int count = 0;
		var gos = new List<GameObject>(Selection.gameObjects);
		var textList = new List<string>();
		foreach (var go in gos)
		{
			// コンポーネント取得
			var coms = new List<UILabel>(go.GetComponentsInChildren<UILabel>(true));
			foreach (var com in coms)
			{
				count++;
				var path = GetPath(go, com.transform);
				Debug.Log(string.Format("{0}\r\n{1}", com.text, path));
				textList.Add(com.text + "\t" + com.name + "\t" + path);
			}
		}

		Debug.Log(string.Format("\r\nUILabel count = {0}", count));

		string totalText = "Text\tLabelName\tPath\r\n";
		//textList.ForEach(text => totalText += text + "\r\n");
		//Debug.Log(string.Format("エクセルコピペ用\r\n{0}", totalText));

		var totalTextList = new List<string>();
		string t = "";
		for (int i = 0; i < textList.Count; i++)
		{
			var text = textList[i];
			if (t.Length >= 15000 || i+1 >= textList.Count)
			{
				totalTextList.Add(t);
				t = "";
			}
			else
			{
				t += text + "\r\n";
			}
		}
		Debug.Log("エクセルコピペ用\r\n" + totalText);
		totalTextList.ForEach(text => Debug.Log(text));
	}
	#endregion

	#region Check GUIObjectUISettings
	/// <summary>
	/// GUIObjectUISettings がどのGameObjectに付いているかチェック
	/// </summary>
	[MenuItem("Scm/UI/Check GUIObjectUISettings (Project内全部)")]
	public static void CheckObjectUISettings()
	{
		int count = 0;
		// アセット内のすべてのパスを取得する
		var assetPathlist = new List<string>(AssetDatabase.GetAllAssetPaths());
		assetPathlist.Sort();
		foreach (var assetPath in assetPathlist)
		{
			// アセットを読み込む
			var obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
			var go = obj as GameObject;
			if (go == null)
			{
				// GameObject以外の表示
				//Debug.Log(obj.name);
				continue;
			}

			// コンポーネント取得
			var coms = new List<GUIObjectUISettings>(go.GetComponentsInChildren<GUIObjectUISettings>(true));
			foreach (var com in coms)
			{
				count++;
				Debug.Log(string.Format("{0}\r\n{1}\r\n{2}\r\n", go.name, assetPath, com.name));
			}
		}

		Debug.Log(string.Format("\r\nGUIObjectUISettings count = {0}", count));
	}
	#endregion

	#region Change TweenGroup
	/// <summary>
	/// TweenGroup番号変更
	/// </summary>
	[MenuItem("Scm/UI/Change TweenGroup")]
	public static void ChangeTweenGroup()
	{
		var gos = new List<GameObject>(Selection.gameObjects);
		int tweenGroup = int.MaxValue;
		foreach (var go in gos)
		{
			// UITweener系のグループを確認
			var coms = new List<UITweener>(go.GetComponents<UITweener>());
			foreach (var com in coms)
			{
				if (tweenGroup == int.MaxValue)
				{
					Debug.Log(string.Format("\r\nUITweener({0}):{1}", com.tweenGroup, GetPath(go, com.transform)));
					tweenGroup = com.tweenGroup;
					continue;
				}
				Debug.Log(string.Format("\r\nUITweener({0}->{1}):{2}", com.tweenGroup, tweenGroup, GetPath(go, com.transform)));
				com.tweenGroup = tweenGroup;
			}
		}
	}
	#endregion
	#endregion

	#region Common
	#region Create Prefab
	/// <summary>
	/// ReferenceData からプレハブを生成する
	/// </summary>
	[MenuItem("Scm/Common/Create Prefab (ReferenceData)")]
	static public void CreatePrefabReferenceData()
	{
		foreach (GameObject go in Selection.gameObjects)
		{
			CreatePrefabReferenceData(go);
		}
	}
	#endregion

	#region Create null
	/// <summary>
	/// Hierarchy 内の null チェック
	/// </summary>
	[MenuItem("Scm/Common/Check null (Hierarchy内)")]
	public static void CheckNullHierarchy()
	{
		CheckNull(new List<GameObject>(Selection.gameObjects));
	}
	static void CheckNull(List<GameObject> gos)
	{
		int go_count = 0, component_count = 0, null_count = 0;
		foreach (var go in gos)
		{
			go_count++;
			CheckNull(go, go.transform, ref component_count, ref null_count);
		}
		Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} null", go_count, component_count, null_count));
	}
	static void CheckNull(GameObject go, Transform t, ref int component_count, ref int null_count)
	{
		var coms = new List<Component>(t.GetComponents<Component>());
		foreach (var com in coms)
		{
			component_count++;
			if (com != null)
				continue;

			null_count++;
			Debug.Log(string.Format("\r\n{0}", GetPath(go, t)));
		}
		for (int i = 0, max = t.childCount; i < max; i++)
		{
			CheckNull(go, t.GetChild(i), ref component_count, ref null_count);
		}
	}
	/// <summary>
	/// Project 内の null チェック
	/// </summary>
	[MenuItem("Scm/Common/Check null (Project内全部) 重いよ！")]
	public static void CheckNullProject()
	{
		var gos = new List<GameObject>();
		// アセット内のすべてのパスを取得する
		var assetPathlist = new List<string>(AssetDatabase.GetAllAssetPaths());
		assetPathlist.Sort();
		foreach (var assetPath in assetPathlist)
		{
			// アセットを読み込む
			var obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
			var go = obj as GameObject;
			if (go == null)
			{
				// GameObject以外の表示
				//Debug.Log(obj.name);
				continue;
			}
			// GameObject ならnull checkをする
			gos.Add(go);
		}
		CheckNull(gos);
		Debug.Log(string.Format("{0} / {1} (GameObject Count / Asset Count)", gos.Count, assetPathlist.Count));
		/*
				int go_count = 0, component_count = 0, null_count = 0;
				var assetPathlist = new List<string>(AssetDatabase.GetAllAssetPaths());
				foreach (var assetPath in assetPathlist)
				{
					var go = Resources.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
					//var go = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
					if (go == null)
						continue;
					go_count++;
					CheckNull(go, go.transform, ref component_count, ref null_count);
				}
				Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} null", go_count, component_count, null_count));
				Debug.Log(string.Format("{0} / {1} (GameObject Count / Asset Count)", go_count, assetPathlist.Count));
		*/
	}
	#endregion

	#region Seach Component
	/// <summary>
	/// 一時的に一括で何か処理をしたい時に書き換えて使うと便利かも
	/// 例えば古いレイヤー番号から新しいレイヤー番号に切り替えるとか
	/// SearchProcRoot と SearchProcChild に処理したいのを書き換えればOKなはず
	/// </summary>
	[MenuItem("Scm/Common/Seach Component(Hierarchy内)")]
	static public void SeachComponentHierarchy()
	{
		SearchComponentProcRoot(new List<GameObject>(Selection.gameObjects));
	}
	[MenuItem("Scm/Common/Seach Component(Project内全部) 重いよ！")]
	public static void SeachComponentProject()
	{
		var gos = new List<GameObject>();
		// アセット内のすべてのパスを取得する
		var assetPathlist = new List<string>(AssetDatabase.GetAllAssetPaths());
		assetPathlist.Sort();
		// GameObject を抜き出す
		foreach (var assetPath in assetPathlist)
		{
			// アセットを読み込む
			var obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
			var go = obj as GameObject;
			if (go == null)
			{
				// GameObject以外
				continue;
			}
			gos.Add(go);
		}
		// 実際の処理
		SearchComponentProcRoot(gos);
		Debug.Log(string.Format("{0} / {1} (GameObject Count / Asset Count)", gos.Count, assetPathlist.Count));
	}
	static void SearchComponentProcRoot(List<GameObject> gos)
	{
		// --------------------------------------------
		// 変数は必要に応じて変える
		// --------------------------------------------
		int go_count = 0, component_count = 0;
		
		foreach (var go in gos)
		{
			go_count++;

			// --------------------------------------------
			// 再帰処理の引数は必要に応じて変える
			// --------------------------------------------
			SearchComponentProcChild(go, go.transform, ref component_count);
		}
		
		// --------------------------------------------
		// メッセージ表示は必要に応じて変える
		// --------------------------------------------
		Debug.Log(string.Format("Searched {0} GameObjects, {1} Compornents", go_count, component_count));
	}
	// --------------------------------------------
	// 引数は必要に応じて変える
	// --------------------------------------------
	static void SearchComponentProcChild(GameObject rootGo, Transform t, ref int component_count)
	{
		// --------------------------------------------
		// 実際の処理は必要に応じて変える
		// --------------------------------------------
		var coms = t.gameObject.GetComponents<GUIButtonDisable>();
		if (coms != null)
		{
			foreach (var com in coms)
			{
				component_count++;
				Debug.Log(rootGo.name + "\r\n" + GetPath(t.gameObject, com.transform));
			}
		}

		// --------------------------------------------
		// 再帰処理の引数は必要に応じて変える
		// --------------------------------------------
		for (int i = 0, max = t.childCount; i < max; i++)
		{
			SearchComponentProcChild(rootGo, t.GetChild(i), ref component_count);
		}
	}
	#endregion

	#region Seach MasterTextSetterList
	/// <summary>
	/// MasterTextSetterList のチェック
	/// </summary>
	[MenuItem("Scm/Common/Seach MasterTextSetterList(Hierarchy内)")]
	static public void SeachMasterTextSetterListHierarchy()
	{
		SearchMasterTextSetterListProcRoot(new List<GameObject>(Selection.gameObjects));
	}
	[MenuItem("Scm/Common/Seach MasterTextSetterList(Project内全部) 重いよ！")]
	public static void SeachMasterTextSetterListProject()
	{
		var gos = new List<GameObject>();
		// アセット内のすべてのパスを取得する
		var assetPathlist = new List<string>(AssetDatabase.GetAllAssetPaths());
		assetPathlist.Sort();
		// GameObject を抜き出す
		foreach (var assetPath in assetPathlist)
		{
			// アセットを読み込む
			var obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
			var go = obj as GameObject;
			if (go == null)
			{
				// GameObject以外
				continue;
			}
			gos.Add(go);
		}
		// 実際の処理
		SearchMasterTextSetterListProcRoot(gos);
		Debug.Log(string.Format("{0} / {1} (GameObject Count / Asset Count)", gos.Count, assetPathlist.Count));
	}
	static void SearchMasterTextSetterListProcRoot(List<GameObject> gos)
	{
		// --------------------------------------------
		// 変数は必要に応じて変える
		// --------------------------------------------
		int go_count = 0, comp_count = 0;
		var textList = new List<string>();

		foreach (var go in gos)
		{
			go_count++;

			// --------------------------------------------
			// 再帰処理の引数は必要に応じて変える
			// --------------------------------------------
			SearchMasterTextSetterListProcChild(go, ref comp_count, ref textList);
		}

		// --------------------------------------------
		// メッセージ表示は必要に応じて変える
		// --------------------------------------------
		textList.Sort();
		string totalText = "TextMasterID\tLabelName\tPath\r\n";
		textList.ForEach(text => totalText += text + "\r\n");
		Debug.Log(string.Format("エクセルコピペ用\r\n{0}", totalText));
		Debug.Log(string.Format("Searched {0} GameObjects, {1} Components", go_count, comp_count));
	}
	// --------------------------------------------
	// 引数は必要に応じて変える
	// --------------------------------------------
	static void SearchMasterTextSetterListProcChild(GameObject rootGo, ref int comp_count, ref List<string> textList)
	{
		// --------------------------------------------
		// 実際の処理は必要に応じて変える
		// --------------------------------------------
		var list = new List<MasterTextSetterList>(rootGo.GetComponentsInChildren<MasterTextSetterList>(true));
		foreach (var com in list)
		{
			foreach (var t in com.List)
			{
				string labelName = "", path = "";
				if (t.Label != null)
				{
					labelName = t.Label.name;
					path = GetPath(rootGo, t.Label.transform);
				}
				var text = string.Format("{0:0000}\t{1}", t.TextMasterID, labelName);
				Debug.Log(text + "\r\n" + path);
				textList.Add(text + "\t" + path);
			}
		}
		comp_count += list.Count;

		// --------------------------------------------
		// 再帰処理の引数は必要に応じて変える
		// --------------------------------------------
	}
	#endregion
	#endregion

	#region Public
	/// <summary>
	/// ReferenceData からプレハブを生成する
	/// </summary>
	/// <returns></returns>
	/// <param name="go"></param>
	static public ReferenceData CreatePrefabReferenceData(GameObject go)
	{
		ReferenceData com = go.GetComponent<ReferenceData>();
		if (com == null)
		{ return null; }

		com.CreatePrefab();
		com.enabled = true;
		return com;
	}
	/// <summary>
	/// 子供を生成する
	/// </summary>
	/// <returns></returns>
	/// <param name="parent"></param>
	static public GameObject AddChild(GameObject parent)
	{
		GameObject go = new GameObject();

		if (parent != null)
		{
			Transform t = go.transform;
			t.parent = parent.transform;
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
			go.layer = parent.layer;
		}
		return go;
	}
	/// <summary>
	/// 子供を削除する
	/// </summary>
	/// <param name="go"></param>
	static public void RemoveChild(GameObject go)
	{
		Transform t = go.transform;
		for (int i = 0; i < t.childCount; i++)
		{
			Transform child = t.GetChild(i);
			UnityEngine.Object.DestroyImmediate(child.gameObject);
		}
	}
	/// <summary>
	/// 相対パス名を取得する
	/// </summary>
	public static string GetPath(GameObject root, Transform transform)
	{
		string path = "";

		if (transform.parent != null)
		{
			bool isSelect = (root == transform.gameObject);
			if (!isSelect)
			{
				path += GetPath(root, transform.parent);
				path += "/";
			}
		}
		path += transform.name;
		return path;
	}
	#endregion

	#region 一括検索(HierarchyとProject内全部)
#if false
	/// <summary>
	/// 一時的に一括で何か処理をしたい時に書き換えて使うと便利かも
	/// 例えば古いレイヤー番号から新しいレイヤー番号に切り替えるとか
	/// SearchProcRoot と SearchProcChild に処理したいのを書き換えればOKなはず
	/// </summary>
	[MenuItem("Scm/Seach(Hierarchy内)")]
	static public void SeachHierarchy()
	{
		SearchProcRoot(new List<GameObject>(Selection.gameObjects));
	}
	[MenuItem("Scm/Seach(Project内全部) 重いよ！")]
	public static void SeachProject()
	{
		var gos = new List<GameObject>();
		// アセット内のすべてのパスを取得する
		var assetPathlist = new List<string>(AssetDatabase.GetAllAssetPaths());
		assetPathlist.Sort();
		// GameObject を抜き出す
		foreach (var assetPath in assetPathlist)
		{
			// アセットを読み込む
			var obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
			var go = obj as GameObject;
			if (go == null)
			{
				// GameObject以外
				continue;
			}
			gos.Add(go);
		}
		// 実際の処理
		SearchProcRoot(gos);
		Debug.Log(string.Format("{0} / {1} (GameObject Count / Asset Count)", gos.Count, assetPathlist.Count));
	}
	static void SearchProcRoot(List<GameObject> gos)
	{
		// --------------------------------------------
		// 変数は必要に応じて変える
		// --------------------------------------------
		int go_count = 0, uibg = 0, ui3d = 0, ui2d = 0, uifg = 0;

		foreach (var go in gos)
		{
			go_count++;

			// --------------------------------------------
			// 再帰処理の引数は必要に応じて変える
			// --------------------------------------------
			SearchProcChild(go, go.transform, ref uibg, ref ui3d, ref ui2d, ref uifg);
		}

		// --------------------------------------------
		// メッセージ表示は必要に応じて変える
		// --------------------------------------------
		Debug.Log(string.Format("Searched {0} GameObjects, {1} UIBG, {2} UI3D, {3} UI2D, {4} UIFG", go_count, uibg, ui3d, ui2d, uifg));
	}
	// --------------------------------------------
	// 引数は必要に応じて変える
	// --------------------------------------------
	static void SearchProcChild(GameObject rootGo, Transform t, ref int uibg, ref int ui3d, ref int ui2d, ref int uifg)
	{
		// --------------------------------------------
		// 実際の処理は必要に応じて変える
		// --------------------------------------------
		switch (t.gameObject.layer)
		{
		case LayerNumber.UIBG_Old: t.gameObject.layer = LayerNumber.UIBG; uibg++; break;
		case LayerNumber.UI3D_Old: t.gameObject.layer = LayerNumber.UI3D; ui3d++; break;
		case LayerNumber.UI2D_Old: t.gameObject.layer = LayerNumber.UI2D; ui2d++; break;
		case LayerNumber.UIFG_Old: t.gameObject.layer = LayerNumber.UIFG; uifg++; break;
		}

		// --------------------------------------------
		// 再帰処理の引数は必要に応じて変える
		// --------------------------------------------
		for (int i = 0, max = t.childCount; i < max; i++)
		{
			SearchProcChild(rootGo, t.GetChild(i), ref uibg, ref ui3d, ref ui2d, ref uifg);
		}
	}
#endif
	#endregion
}
