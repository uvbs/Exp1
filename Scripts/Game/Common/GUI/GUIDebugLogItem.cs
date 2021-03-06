/// <summary>
/// デバッグログアイテム
/// 
/// 2015/03/17
/// </summary>
using UnityEngine;
using System.Collections;

public class GUIDebugLogItem : MonoBehaviour
{
	#region フィールド＆プロパティ
	/// <summary>
	/// テキストメッセージ
	/// </summary>
	[SerializeField]
	string _text;
	string Text { get { return _text; } set { _text = value; } }

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		[SerializeField]
		UILabel _textLabel;
		public UILabel TextLabel { get { return _textLabel; } }
	}
	#endregion

	#region 初期化
	/// <summary>
	/// 作成
	/// </summary>
	public static GUIDebugLogItem Create(GameObject prefab, Transform parent, int itemIndex)
	{
		// インスタンス化
		var go = SafeObject.Instantiate(prefab) as GameObject;
		if (go == null)
			return null;

		// 名前
		go.name = string.Format("{0}{1}", prefab.name, itemIndex);
		// 親子付け
		go.transform.parent = parent;
		go.transform.localPosition = prefab.transform.localPosition;
		go.transform.localRotation = prefab.transform.localRotation;
		go.transform.localScale = prefab.transform.localScale;
		// 可視化
		go.SetActive(true);

		// コンポーネント取得
		var item = go.GetComponent(typeof(GUIDebugLogItem)) as GUIDebugLogItem;
		if (item == null)
			return null;
		// 値初期化
		item.ClearValue();

		return item;
	}
	/// <summary>
	/// クリア
	/// </summary>
	[System.Diagnostics.Conditional("XW_DEBUG")]
	public void ClearValue()
	{
		this.Setup("");
	}
	#endregion

	#region セットアップ
	/// <summary>
	/// 設定
	/// </summary>
	[System.Diagnostics.Conditional("XW_DEBUG")]
	public void Setup(string text)
	{
		this.SetText(text);
	}
	/// <summary>
	/// テキスト設定
	/// </summary>
	[System.Diagnostics.Conditional("XW_DEBUG")]
	public void SetText(string text)
	{
		this.Text = text;

		// 文字列が空なら非表示にする
		var isActive = !string.IsNullOrEmpty(text);
		this.gameObject.SetActive(isActive);

		var t = this.Attach;
		if (t.TextLabel != null)
			t.TextLabel.text = text;

		GUIDebugLog.Reposition();
	}
	#endregion
}
