/// <summary>
/// チャットマクロアイテム
/// 
/// 2014/11/25
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.NGWord;

public class GUIChatMacroItem : MonoBehaviour
{
	#region フィールド＆プロパティ
	/// <summary>
	/// アイテム情報
	/// </summary>
	[SerializeField]
	ChatMacroInfo _itemInfo;
	ChatMacroInfo ItemInfo { get { return _itemInfo; } set { _itemInfo = value; } }

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UILabel buttonLabel;
		public UIInput input;
	}

	// ボタンの文字の長さ
	const int ButtonNameMaxLength = 8;
	// ボタン名
	string ButtonName
	{
		get
		{
			string text = this.ItemInfo.ButtonName;
			if (string.IsNullOrEmpty(text))
			{
				text = this.ItemInfo.Macro;
			}
			if (text.Length > GUIChat.MacroButtonNameMaxLength)
				text = text.Substring(0, GUIChat.MacroButtonNameMaxLength) + GUIChat.MacroButtonNameOverReplaceString;
			return text;
		}
	}

	/// <summary>
	/// チャットが入力中かどうか
	/// </summary>
	public bool IsInput
	{
		get
		{
			if (this.Attach.input == null)
				return false;
			return this.Attach.input.isSelected;
		}
	}

	// シリアライズされていないメンバーの初期化
	void MemberInit()
	{
	}
	#endregion

	#region 初期化
	public static GUIChatMacroItem Create(GameObject prefab, Transform parent, int itemIndex)
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
		var item = go.GetComponent(typeof(GUIChatMacroItem)) as GUIChatMacroItem;
		if (item == null)
			return null;
		// 値初期化
		item.ClearValue();

		return item;
	}
	public void ClearValue()
	{
	}
	#endregion

	#region セットアップ
	public void Setup(ChatMacroInfo info)
	{
		this.ItemInfo = info;

		// UI更新
		{
			var t = this.Attach;
			if (t.buttonLabel != null)
				t.buttonLabel.text = this.ButtonName;
		}
	}
	#endregion

	#region NGUIリフレクション
	public void OnMacro()
	{
		// チャット送信
		GUIChat.SendChat(this.ItemInfo.Macro);
		// マクロを閉じる
		if (ConfigFile.Option.IsMacroClose)
			GUIChat.SetMacroActive(false);
	}
	public void OnInputSelect()
	{
		var t = this.Attach;
		if (t.input != null)
			t.input.isSelected = true;
	}
	public void OnInputSubmit()
	{
		string text = UIInput.current.value;
		// NGワードチェック
		text = NGWord.DeleteNGWord(text);
		// 文字を消してフォーカスを外す
		UIInput.current.value = "";
		UIInput.current.RemoveFocus();
		if (string.IsNullOrEmpty(text))
			return;

		// マクロ設定
		this.ItemInfo.Setup("", text);
		// ボタン名更新
		var t = this.Attach;
		if (t.buttonLabel != null)
			t.buttonLabel.text = this.ButtonName;
	}
	#endregion
}
