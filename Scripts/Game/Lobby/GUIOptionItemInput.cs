/// <summary>
/// オプションの入力アイテム
/// 
/// 2015/01/21
/// </summary>
using UnityEngine;
using System.Collections;

public class GUIOptionItemInput : GUIOptionItemBase
{
	#region フィールド＆プロパティ
	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UILabel descLabel;	// 説明文
		public UIInput input;		// 入力
	}

	// 値が変化した時の処理
	System.Action<UIInput, string> ChangeFunc { get; set; }
	// シリアライズされていないメンバーの初期化
	void MemberInit()
	{
		this.ChangeFunc = delegate { };
	}
	#endregion

	#region 初期化
	void Awake()
	{
		this.MemberInit();
	}
	public static GUIOptionItemInput Create(GameObject prefab, Transform parent, int itemIndex)
	{
		// インスタンス化
		var go = GUIOptionItemBase.CreateBase(prefab, parent, itemIndex);
		if (go == null)
			return null;

		// コンポーネント取得
		var item = go.GetComponent(typeof(GUIOptionItemInput)) as GUIOptionItemInput;
		if (item == null)
			return null;
		// 値初期化
		item.ClearValue();

		return item;
	}
	public void ClearValue()
	{
		this.Setup("", "", "", null);
	}
	#endregion

	#region セットアップ
	public void Setup(string descText, string value, string emptyString, System.Action<UIInput, string> changeFunc)
	{
		this.Setup(descText, value, emptyString, UIInput.KeyboardType.Default, 0, changeFunc);
	}
	public void Setup(string descText, string value, string emptyString, UIInput.KeyboardType keyboardType, int limitLength, System.Action<UIInput, string> changeFunc)
	{
		this.ChangeFunc = (changeFunc != null ? changeFunc : delegate { });

		// UI更新
		{
			var t = this.Attach;
			if (t.descLabel != null)
				t.descLabel.text = descText;
			if (t.input != null)
			{
				t.input.defaultText = emptyString;
				t.input.value = value;
				t.input.keyboardType = keyboardType;
				t.input.characterLimit = limitLength;
			}
		}
	}
	#endregion

	#region NGUIリフレクション
	public void OnValueChange()
	{
	}
	public void OnSubmit()
	{
		if (UIInput.current == null)
			return;

		string text = UIInput.current.value;
		// NGワードチェック
		//text = NGWord.DeleteNGWord(text);
		// フォーカスを外す
		UIInput.current.RemoveFocus();

		this.ChangeFunc(UIInput.current, text);
	}
	#endregion
}
