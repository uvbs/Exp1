/// <summary>
/// ロビーセレクトアイテム
/// 
/// 2013/09/20
/// </summary>
using UnityEngine;
using System.Collections;

public class GUILobbySelectItem : MonoBehaviour
{
	#region フィールド＆プロパティ
	/// <summary>
	/// ロビー名フォーマット
	/// </summary>
	[SerializeField]
	string _lobbyNameFormat = "No.{0:000}";
	string LobbyNameFormat { get { return _lobbyNameFormat; } }

	/// <summary>
	/// ユーザー数フォーマット
	/// </summary>
	[SerializeField]
	string _userCountFormat = "{0:00}/{1:00}";
	string UserCountFormat { get { return _userCountFormat; } }

	/// <summary>
	/// ロードフラグ
	/// </summary>
	[SerializeField]
	bool _isLoad;
	public bool IsLoad { get { return _isLoad; } private set { _isLoad = value; } }

	/// <summary>
	/// ストレージ内全ての中からのインデックス
	/// </summary>
	[SerializeField]
	int _indexInTotal = -1;
	public int IndexInTotal { get { return _indexInTotal; } private set { _indexInTotal = value; } }

	/// <summary>
	/// ロビー情報
	/// </summary>
	[SerializeField]
	LobbyInfo _lobbyInfo;
	public LobbyInfo LobbyInfo { get { return _lobbyInfo; } private set { _lobbyInfo = value; } }

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UIButton button;
		public UILabel nameLabel;
		public UILabel userLabel;
		public UISprite selectSprite;
		public GameObject loadingGroup;
	}
	#endregion

	#region 初期化
	public static GUILobbySelectItem Create(GameObject prefab, Transform parent, int itemIndex)
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
		var item = go.GetSafeComponentInChildren<GUILobbySelectItem>();
		if (item == null)
			return null;
		// 値初期化
		item.ClearValue();

		return item;
	}
	public void ClearValue()
	{
		this.SetSelectSpriteActive(false);
		this.Setup(false, -1, null);
	}
	#endregion

	#region セットアップ
	public void Setup(bool isLoad, int indexInTotal, LobbyInfo info)
	{
		this.IsLoad = isLoad;
		this.IndexInTotal = indexInTotal;
		this.LobbyInfo = (info != null ? info : new LobbyInfo());

		this.SetSelectSpriteActive(false);

		// UI設定
		{
			var t = this.Attach;
			if (t.button != null)
				t.button.isEnabled = isLoad;

			if (t.nameLabel != null)
				t.nameLabel.text = (!isLoad ? "" : string.Format(this.LobbyNameFormat, this.LobbyInfo.LobbyID));

			if (t.userLabel != null)
				t.userLabel.text = (!isLoad ? "" : string.Format(this.UserCountFormat, this.LobbyInfo.Num, this.LobbyInfo.Capacity));

			if (t.loadingGroup != null)
				t.loadingGroup.SetActive(!isLoad);
		}
	}
	/// <summary>
	/// 選択枠の表示設定
	/// </summary>
	public void SetSelectSpriteActive(bool isActive)
	{
		if (this.Attach.selectSprite != null)
			this.Attach.selectSprite.gameObject.SetActive(isActive);
	}
	#endregion

	#region NGUIリフレクション
	/// <summary>
	/// アイテムを押した時
	/// </summary>
	public void OnSelect()
	{
		GUILobbySelect.SetSelectItem(this);
	}
	#endregion
}
