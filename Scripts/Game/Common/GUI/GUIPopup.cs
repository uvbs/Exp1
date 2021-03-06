/// <summary>
/// ポップアップシステム
/// 
/// 2014/06/18
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public class GUIPopup : MonoBehaviour
{
	#region フィールド＆プロパティ
	/// <summary>
	/// 表示するアイテムの最大数
	/// </summary>
	[SerializeField]
	int _itemMax = 3;
	public int ItemMax { get { return _itemMax; } set { _itemMax = value; } }

	/// <summary>
	/// アイテムの静止時間
	/// </summary>
	[SerializeField]
	float _timer = 5f;
	public float Timer { get { return _timer; } set { _timer = value; } }

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	public AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public GameObject prefab;
		public Transform root;
	}

	public List<GUIPopupItem> ItemMoveList { get; private set; }
	public float Counter { get; private set; }
	IPopupQueue PopupQueue { get; set; }
	bool IsDestroy { get; set; }

	/// <summary>
	/// Tweenが全て終了しているかどうか
	/// </summary>
	public bool IsFinished
	{
		get
		{
			bool isFinished = true;
			foreach (var item in this.ItemMoveList)
			{
				if (item.IsFinished)
					continue;
				isFinished = false;
				break;
			}
			return isFinished;
		}
	}
	#endregion

	#region 初期化
	/// <summary>
	/// セットアップ
	/// </summary>
	public void Setup(IPopupQueue queue)
	{
		this.PopupQueue = queue;
		this.Start();
	}
	void Start()
	{
		this.ItemMoveList = new List<GUIPopupItem>(this.ItemMax);
		// テーブル内にある全てのアイテムを削除
		this.DestroyItem();
	}
	/// <summary>
	/// テーブル内にあるアイテムを全削除
	/// </summary>
	void DestroyItem()
	{
		if (this.Attach.root == null)
			return;

		Transform transform = this.Attach.root;
		for (int i = 0, max = transform.childCount; i < max; i++)
		{
			var child = transform.GetChild(i);
			Object.Destroy(child.gameObject);
		}
	}
	#endregion

	#region 更新
	void Update()
	{
		// 移動が終了していない
		if (!this.IsFinished)
			return;
		// キュー処理
		if (this.QueueProc())
			return;
		// アイテム自体がひとつも出ていない
		if (this.ItemMoveList.Count <= 0)
			return;

		// アイテムを次のモードへ移行するためのカウンター
		this.Counter -= Time.deltaTime;
		if (this.Counter > 0f)
			return;
		this.Counter = this.Timer;

		// 次のモードへ移行する
		this.SetNextMode();
	}
	/// <summary>
	/// キュー処理、キューの処理をしたかどうかを返す
	/// </summary>
	bool QueueProc()
	{
		// キューが存在するかどうか
		if (!this.PopupQueue.IsQueue)
			return false;

		// アイテムをポップアップさせない場合はキューを削除する
		if (this.ItemMax <= 0)
		{
			this.PopupQueue.Clear();
			return false;
		}

		// アイテムが最大数出ている時は
		// 最初のアイテムが消えてから新しいアイテムを追加する
		if (this.ItemMoveList.Count >= this.ItemMax)
		{
			this.SetNextMode();
			return true;
		}

		// キューから取り出して再生する
		var item = this.PopupQueue.Create(this.Attach.prefab, this.Attach.root);
	    if (null == item)
	    {
	        return false;
	    }
		item.Play(this.ItemMoveList.Count, this.OnEndFinish);
		// アイテムの再生をリストで管理するため追加する
		this.ItemMoveList.Add(item);
		// タイマー設定
		this.Counter = this.Timer;

		return true;
	}
	void OnEndFinish(GUIPopupItem item)
	{
		this.ItemMoveList.Remove(item);
		item.Destroy();
	}
	void SetNextMode()
	{
		// アイテムを次のモードに移行させる
		try
		{
			// ToArray で参照を作っている理由は
			// 読み込みなどで一瞬止まると SetNextMode 内の PlayTween.Play を実行した瞬間に
			// コールバック関数（OnEndFinish）が呼ばれてしまい ItemMoveList.Remove が走ってしまうため
			// 参照リストを作成して対処している
			foreach (var i in this.ItemMoveList.ToArray())
			{
				i.SetNextMode();
			}
		}
		catch (System.Exception e)
		{
			Debug.LogWarning("GUIPopup.SetNextMode\r\n" + e);
		}
	}
	#endregion
}



/// <summary>
/// ポップアップアイテム作成キュー
/// </summary>
public interface IPopupQueue
{
	/// <summary>
	/// キューが存在するかどうか
	/// </summary>
	bool IsQueue { get; }
	/// <summary>
	/// 全てのキューをクリアする
	/// </summary>
	void Clear();
	/// <summary>
	/// キューから取り出してアイテムを生成する
	/// </summary>
	GUIPopupItem Create(GameObject prefab, Transform parent);
}
