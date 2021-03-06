/// <summary>
/// メッセージ表示を制御するクラス
/// 
/// 2014/07/30
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#region 上書き制御クラス

public class GUIEffectMsgOverrite : GUIEffectMessageController
{
	#region 定数

	/// <summary>
	/// スタック最大数
	/// </summary>
	private const int StackOver = 1;

	#endregion

	#region フィールド&プロパティ

	/// <summary>
	/// アイテムキュー
	/// </summary>
	private Queue<IEffectMessageItem> itemQueue = new Queue<IEffectMessageItem>();

	/// <summary>
	/// メッセージがセットされている数
	/// </summary>
	public override int ItemCount{ get{ return this.itemQueue.Count; } }

	#endregion

	#region セット

	public override void SetEffectMessage(IEffectMessageItem item)
	{
		if(this.itemQueue.Count >= StackOver)
		{
			// 最大数まで溜まっている場合は削除
			IEffectMessageItem deleteitem = this.itemQueue.Dequeue();
			deleteitem.Delete();
		}

		// 追加
		this.itemQueue.Enqueue(item);

		// 更新処理セット
		SetUpdateMessage();
	}

	#endregion

	#region 更新

	/// <summary>
	/// メッセージ更新処理
	/// </summary>
	protected override IEnumerator UpdateMessage()
	{
		// キューに追加されているアイテムが全て削除されるまで待機
		yield return new QueueDeleteWait(this.itemQueue);

		yield break;
	}

	#endregion
}

#endregion

#region 積み重ね制御クラス

public class GUIEffectMsgStack : GUIEffectMessageController
{
	#region フィールド&プロパティ

	/// <summary>
	/// アイテムキュー
	/// </summary>
	private Queue<IEffectMessageItem> itemQueue = new Queue<IEffectMessageItem>();

	/// <summary>
	/// メッセージがセットされている数
	/// </summary>
	public override int ItemCount{ get{ return this.itemQueue.Count; } }
	
	#endregion

	#region セット
	
	public override void SetEffectMessage(IEffectMessageItem item)
	{
		// 追加
		item.SetActive(false);
		this.itemQueue.Enqueue(item);
		
		// 更新処理セット
		SetUpdateMessage();
	}
	
	#endregion

	#region 更新
	
	/// <summary>
	/// メッセージ更新処理
	/// </summary>
	protected override IEnumerator UpdateMessage()
	{
		while(this.itemQueue.Count > 0)
		{
			IEffectMessageItem item = this.itemQueue.Dequeue();
			item.SetActive(true);

			// エフェクトメッセージのアイテムが削除されるまで待機する
			yield return new WaitEffectItem(item);

			yield return null;
		}

		yield break;
	}
	
	#endregion
	
}

#endregion

#region 通常制御クラス

public class GUIEffectMsgNormal : GUIEffectMessageController
{
	#region フィールド&プロパティ

	/// <summary>
	/// メッセージアイテムキュー
	/// </summary>
	private Queue<IEffectMessageItem> itemQueue = new Queue<IEffectMessageItem>();

	/// <summary>
	/// メッセージがセットされている数
	/// </summary>
	public override int ItemCount{ get{ return this.itemQueue.Count; } }

	#endregion

	#region セット
	
	public override void SetEffectMessage(IEffectMessageItem item)
	{
		// 追加
		this.itemQueue.Enqueue(item);
		
		// 更新処理セット
		SetUpdateMessage();
	}
	
	#endregion
	
	#region 更新
	
	/// <summary>
	/// メッセージ更新処理
	/// </summary>
	protected override IEnumerator UpdateMessage()
	{
		// キューに追加されているアイテムが全て削除されるまで待機
		yield return new QueueDeleteWait(this.itemQueue);
		
		yield break;
	}
	
	#endregion
}

#endregion

#region メッセージ制御基底クラス

public abstract class GUIEffectMessageController
{
	#region 制御の種類

	public enum Type
	{
		/// <summary>
		/// 上書き
		/// 古いメッセージは消去して新しいメッセージを表示
		/// </summary>
		Overrite,
		
		/// <summary>
		/// 積み重ねる
		/// 追加順にメッセージを表示していく
		/// </summary>
		Stack,
		
		/// <summary>
		/// 通常
		/// メッセージが追加されたらすぐに表示する
		/// </summary>
		Normal,
	}

	#endregion

	#region フィールド&プロパティ

	/// <summary>
	/// コルーチン制御
	/// </summary>
	protected FiberSet fiberSet = new FiberSet();

	/// <summary>
	/// 更新チェック用
	/// </summary>
	protected Fiber updateFiber;

	/// <summary>
	/// メッセージがセットされている数
	/// </summary>
	public abstract int ItemCount { get; }

	#endregion

	#region セット

	public abstract void SetEffectMessage(IEffectMessageItem item);

	/// <summary>
	/// コルーチンにメッセージ更新をセットする
	/// </summary>
	protected void SetUpdateMessage()
	{
		if(!this.fiberSet.Contains(this.updateFiber))
		{
			this.updateFiber = this.fiberSet.AddFiber(UpdateMessage());
		}
	}

	#endregion

	#region 生成

	/// <summary>
	/// メッセージ制御生成クラス
	/// </summary>
	public static GUIEffectMessageController CreateController(Type controllerType)
	{
		GUIEffectMessageController controller = null;
		switch(controllerType)
		{
			case Type.Overrite:
				controller = new GUIEffectMsgOverrite();
				break;
			case Type.Stack:
				controller = new GUIEffectMsgStack();
				break;
			case Type.Normal:
				controller = new GUIEffectMsgNormal();
				break;
		}

		return controller;
	}
	
	#endregion

	#region 更新

	/// <summary>
	/// フィーバー更新
	/// </summary>
	public virtual void Update()
	{
		this.fiberSet.Update();
	}

	/// <summary>
	/// メッセージ更新処理
	/// </summary>
	/// <returns>The message.</returns>
	protected virtual IEnumerator UpdateMessage(){ yield break; }

	#endregion

	#region フィーバークラス群

	// キューに追加されているアイテムが全て削除されるまで待機するクラス
	protected class QueueDeleteWait : IFiberWait
	{
		private Queue<IEffectMessageItem> itemQueue = new Queue<IEffectMessageItem>();

		public QueueDeleteWait(Queue<IEffectMessageItem> messageQueue)
		{
			itemQueue = messageQueue;
		}

		#region IFiberWait
		
		public bool IsWait
		{
			get
			{
				if(this.itemQueue.Count > 0)
				{
					IEffectMessageItem item  = this.itemQueue.Peek();
					if(item.IsDelete)
					{
						// アイテム削除
						item.Delete();
						this.itemQueue.Dequeue();
					}
				}

				// 追加されているアイテムが全て削除されるまで待機
				return (this.itemQueue.Count > 0);
			}
		}

		#endregion
	}

	// エフェクトメッセージアイテムの削除フラグがONになるまで待機するクラス
	protected class WaitEffectItem : IFiberWait
	{
		private IEffectMessageItem messageItem;

		public WaitEffectItem(IEffectMessageItem item)
		{
			this.messageItem = item;
		}

		#region IFiberWait

		public bool IsWait
		{
			get
			{
				bool isWait = (!this.messageItem.IsDelete);
				if(this.messageItem.IsDelete)
				{
					// アイテム削除
					this.messageItem.Delete();
				}

				// 削除されるまで待機
				return isWait;
			}
		}

		#endregion
	}

	#endregion
}

#endregion