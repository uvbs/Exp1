/// <summary>
/// チャットアイテム
/// 
/// 2014/06/09
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIChatItem : MonoBehaviour
{
	#region フィールド＆プロパティ
	/// <summary>
	/// チャット情報
	/// </summary>
	[SerializeField] ChatInfo _chatInfo;
	public ChatInfo ChatInfo { get { return _chatInfo; } }

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField] AttachObject _attach;
	public AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UILabel nameLabel;
		public UILabel textLabel;
	}

	float DefaultHeight { get; set; }
	#endregion

	#region 初期化
	void Awake()
	{
		this.DefaultHeight = this.Attach.textLabel.transform.localPosition.y;
	}
	public static GUIChatItem Create(GameObject prefab, Transform parent, int itemIndex)
	{
		UnityEngine.Assertions.Assert.IsNotNull(prefab, "GUIChatItem:'prefab' Not Found!!");

		// インスタンス化
		var go = SafeObject.Instantiate(prefab) as GameObject;
		UnityEngine.Assertions.Assert.IsNotNull(go, "GUIChatItem:'SafeObject.Instantiate(prefab) as GameObject' Not Found!!");

		// 名前
		go.name = string.Format("{0}{1}", prefab.name, itemIndex);
		// 親子付け
		go.transform.SetParent(parent, false);
		// 可視化
		if (!go.activeSelf)
		{
			go.SetActive(true);
		}

		// コンポーネント取得
		var item = go.GetComponent(typeof(GUIChatItem)) as GUIChatItem;
		UnityEngine.Assertions.Assert.IsNotNull(item, "GUIChatItem:'go.GetComponent(typeof(GUIChatItem)) as GUIChatItem' Not Found!!");
		// 値初期化
		item.Awake();
		item.ClearValue();

		return item;
	}
	public void ClearValue()
	{
		this.Setup(null);
	}
	#endregion

	#region セットアップ
	public void Setup(ChatInfo chatInfo)
	{
		this._chatInfo = chatInfo;

		// チャット文字列更新
		bool isName = false;
		{
			string name = "";
			string text = "";
			if (chatInfo != null)
			{
				if (!string.IsNullOrEmpty(chatInfo.name))
				{
					isName = true;
					name = chatInfo.name;
				}
				text = chatInfo.text;
			}
			this.Attach.nameLabel.text = name;
			this.Attach.textLabel.text = text;
		}
		// チャット文字位置調整
		{
			Vector3 p = this.Attach.textLabel.transform.localPosition;
			if (isName)
				this.Attach.textLabel.transform.localPosition = new Vector3(p.x, this.DefaultHeight, p.z);
			else
				this.Attach.textLabel.transform.localPosition = new Vector3(p.x, 0f, p.z);
		}
	}
	#endregion
}
