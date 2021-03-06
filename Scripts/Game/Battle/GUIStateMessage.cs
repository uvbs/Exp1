/// <summary>
/// 状態メッセージ(バフやデバフの効果説明)
/// 
/// 2014/07/24
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Master;
using Scm.Common.GameParameter;

public class GUIStateMessage : Singleton<GUIStateMessage>
{
	#region アタッチオブジェクト
	
	[System.Serializable]
	public class AttachObject
	{
		public UILabel msgLabel;
		public GUIBuffIconItem iconItem;
		public GameObject groupTop;
		public UISprite lineTopSprite;
		public UISprite lineBottomSprite;
	}

	#endregion

	#region 定数

	/// <summary>
	/// 削除時間のスピードアップを開始する時のキューの数
	/// </summary>
	private const int SpeedUpStack = 3;

	/// <summary>
	/// 最低削除時間
	/// </summary>
	private const float WaitTimeMin = 0.1f;

	#endregion

	#region フィールド&プロパティ

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	private AttachObject attach;
	public AttachObject Attach { get { return attach; } }

	/// <summary>
	/// UIPlayTween制御
	/// </summary>
	[SerializeField]
	private PlayTweenController tweenController;

	/// <summary>
	/// 表示してからの削除時間
	/// </summary>
	private float deleteTime;
	
	/// <summary>
	/// 表示するバフ情報リスト
	/// </summary>
	private LinkedList<BuffInfo> buffInfoList = new LinkedList<BuffInfo>();

	#endregion

	#region 初期化

	protected override void Awake()
	{
		base.Awake();

		if(this.Attach.groupTop != null)
		{
			// 全エフェクトにかかる時間を取得
			this.Attach.groupTop.SetActive(true);
			this.deleteTime = this.tweenController.GetEffectTime(this.gameObject);
			this.Attach.groupTop.SetActive(false);
		}
	}

	#endregion

	#region バフ情報セット

	/// <summary>
	/// 状態メッセージをセットする
	/// </summary>
	/// <param name="infoList">
	/// バフがかかった順に並び替えされたバフ情報リスト
	/// </param>
	public static void SetStateMessage(LinkedList<BuffInfo> infoList)
	{
		if(Instance == null)
			return;

		Instance._SetStateMessage(infoList);
	}
	private void _SetStateMessage(LinkedList<BuffInfo> infoList)
	{
		// 表示するバフリストを更新
		UpdateBuffInfoList(infoList);
	}

	/// <summary>
	/// 表示するバフリストを更新する
	/// 表示するバフリストに同タイプかつレベルが低いものが存在した場合レベルが低いバフを消して高い方を表示する
	/// </summary>
	private void UpdateBuffInfoList(LinkedList<BuffInfo> infoList)
	{
		// バフがかかった順に格納する用
		LinkedList<BuffType> buffTypeList = new LinkedList<BuffType>();
		// 検索用に表示するバフリストをDictionarynにセットする
		Dictionary<BuffType, BuffInfo> buffDictionary = new Dictionary<BuffType, BuffInfo>();
		foreach(BuffInfo info in this.buffInfoList)
		{
			buffDictionary.Add(info.buffType, info);
			buffTypeList.AddLast(info.buffType);
		}
		
		foreach(BuffInfo info in infoList)
		{
			// 表示するかどうかチェック
			if(!AddListCheck(info)) continue;

			BuffInfo buffInfo;
			if(buffDictionary.TryGetValue(info.buffType, out buffInfo))
			{
				if(buffInfo.level < info.level)
				{
					// 同タイプかつ表示するバフのレベルが低い
					buffDictionary[info.buffType] = info;
					// 表示するバフの方がレベルが低いので削除しレベルの高い方をセットする
					buffTypeList.Remove(info.buffType);
					buffTypeList.AddLast(info.buffType);
				}
			}
			else
			{
				// 表示するバフリストに新しく追加するバフが存在しない
				buffDictionary.Add(info.buffType, info);
				buffTypeList.AddLast(info.buffType);
			}
		}
		
		// 表示するバフリストを最新に更新する
		LinkedList<BuffInfo> newBuffInfoList = new LinkedList<BuffInfo>();
		foreach(BuffType type in buffTypeList)
		{
			BuffInfo buffInfo;
			if(buffDictionary.TryGetValue(type, out buffInfo))
			{
				newBuffInfoList.AddLast(buffInfo);
			}
		}
		this.buffInfoList = newBuffInfoList;
	}

	/// <summary>
	/// 表示リストに追加するかどうかチェック
	/// </summary>
	private bool AddListCheck(BuffInfo info )
	{
		// 新しくかかったバフのみセットする
		if(!info.isNew) return false;

		// マスターデータが存在しない場合は表示しない
		StateEffectMasterData masterData;
		if(!MasterData.TryGetStateEffect((int)info.buffType, out masterData))
		{
			// マスターデータが見つからない
			return false;
		}

		// メッセージが空文字の場合は表示しない
		if(string.IsNullOrEmpty(masterData.DefaultDisplayMessage))
		{
			return false;
		}

		return true;
	}
	
	#endregion

	#region 更新

	void Update ()
	{
		UpdateMessage();
	}

	#endregion

	#region メッセージ更新

	/// <summary>
	/// イベント中かどうか
	/// </summary>
	private bool isEvent = false;

	/// <summary>
	/// 削除時間計測用
	/// </summary>
	private float time = 0;

	/// <summary>
	/// メッセージ更新処理
	/// </summary>
	private void UpdateMessage()
	{
		// 表示するメッセージがなければ更新処理を行わない
		if(this.buffInfoList.Count <= 0)
			return;

		if(this.isEvent)
		{
			// エフェクト処理
			// 各Tweenの更新
			this.tweenController.Update();

			// 何らかの原因でUIPlayTween再生中にエラーが起きた場合表示がOFFにならないので
			// 念のために時間経過で削除するようにしている
			this.time += Time.deltaTime;
			if(this.time > this.deleteTime)
			{
				this.isEvent = false;

				// リストから処理
				this.buffInfoList.RemoveFirst();
				// 表示OFF
				this.Attach.groupTop.SetActive(false);
			}
		}
		else
		{
			// メッセージ設定処理
			BuffInfo buffInfo = this.buffInfoList.First.Value;
			// メッセージセットアップ
			if(!MessageSetup(buffInfo))
			{
				// メッセージのセットアップに失敗
				// 失敗した場合は表示しない
				this.buffInfoList.RemoveFirst();
				return;
			}
			// メッセージ表示ON
			this.Attach.groupTop.SetActive(true);

			// エフェクト初期化
			this.time = 0;
			this.isEvent = true;
			this.tweenController.Init();
		}
	}

	#endregion

	#region メッセージ処理

	/// <summary>
	/// メッセージのセットアップ
	/// </summary>
	private bool MessageSetup(BuffInfo buffInfo)
	{
		// マスターデータ取得
		StateEffectMasterData masterData;
		if(!MasterData.TryGetStateEffect((int)buffInfo.buffType, out masterData))
		{
			// マスターデータが見つからない
			return false;
		}

		// メッセージセット
		if(this.Attach.msgLabel != null)
		{
			string msg;
			masterData.TryGetDisplayMessage(buffInfo.level, out msg);
			this.Attach.msgLabel.text = msg;
		}

		// アイコンセット
		if(this.Attach.iconItem != null)
		{
			this.Attach.iconItem.SetUp(masterData);
		}

		// ラインの色セット
		Color32 color = new Color32((byte)masterData.Color.R, (byte)masterData.Color.G, (byte)masterData.Color.B, (byte)masterData.Color.A);
		if(this.Attach.lineTopSprite != null)
		{
			this.Attach.lineTopSprite.color = color;
		}
		if(this.Attach.lineBottomSprite != null)
		{
			this.Attach.lineBottomSprite.color = color;
		}

		return true;
	}

	#endregion

	#region Tween制御クラス群

	// 制御情報クラス
	[System.Serializable]
	public class PlayTweenInfo
	{
		/// <summary>
		/// 再生するUIPlayTween
		/// </summary>
		[SerializeField]
		private UIPlayTween tween;
		public UIPlayTween PlayTween { get { return tween; } }

		/// <summary>
		/// 次のUIPlayTweenを再生するまでの時間
		/// </summary>
		[SerializeField]
		private float nextPlayTime;
		public float NextPlayTime { get { return nextPlayTime; } }

		/// <summary>
		/// 終了時メソッド呼び出し用
		/// </summary>
		private EventDelegate finsh = new EventDelegate();
		public EventDelegate Finsh { get { return finsh; } set { finsh = value; } }
	}

	// 各UIPlayTween制御クラス
	[System.Serializable]
	public class PlayTweenController
	{
		/// <summary>
		/// 再生するUIPlayTweenリスト
		/// </summary>
		[SerializeField]
		private List<PlayTweenInfo> tweenList = new List<PlayTweenInfo>();

		/// <summary>
		/// 現在再生するUIPlayTweenキュー
		/// </summary>
		private Queue<PlayTweenInfo> playTweenQueue = new Queue<PlayTweenInfo>();

		/// <summary>
		/// 次のUIPlayTweenを再生するまでの時間の計測用
		/// </summary>
		private float time;

		/// <summary>
		/// 時間を更新するかどうか
		/// </summary>
		private bool isTimeUpdate = false;

		/// <summary>
		/// 更新処理
		/// このメソッドを毎フレーム呼ばないと次のUIPlayTweenの再生がされない
		/// </summary>
		public void Update()
		{
			if(this.isTimeUpdate)
			{
				if(this.playTweenQueue.Count == 0)
					return;
				
				PlayTweenInfo tweenInfo = this.playTweenQueue.Peek();
				time += Time.deltaTime;
				if(this.time > tweenInfo.NextPlayTime)
				{
					this.playTweenQueue.Dequeue();
					Play();
				}
			}
		}

		/// <summary>
		/// 初期化処理
		/// </summary>
		public void Init()
		{
			playTweenQueue.Clear();
			foreach(PlayTweenInfo info in this.tweenList)
			{
				// 各UIPlayTweenに終了時に呼ぶメソッドを設定
				EventDelegate finsh = new EventDelegate(TweenFinsh);
				info.PlayTween.onFinished.Clear();
				info.PlayTween.onFinished.Add(finsh);
				this.playTweenQueue.Enqueue(info);
			}

			// 再生
			Play();
		}

		/// <summary>
		/// 再生処理
		/// </summary>
		private void Play()
		{
			this.isTimeUpdate = false;
			
			// 再生
			if(this.playTweenQueue.Count == 0)
				return;
			
			PlayTweenInfo tweenInfo = this.playTweenQueue.Peek();
			tweenInfo.PlayTween.Play(true);
		}

		/// <summary>
		/// UIPlayTweenの終了時処理
		/// </summary>
		public void TweenFinsh()
		{
			this.isTimeUpdate = true;
			this.time = 0;
		}

		/// <summary>
		/// 全エフェクトにかかる時間を取得
		/// </summary>
		public float GetEffectTime(GameObject groupTop)
		{
			float ollEffectTime = 0;
			Dictionary<int, float> groupList = new Dictionary<int, float>();
			foreach(PlayTweenInfo info in this.tweenList)
			{
				// 次の再生にかかる時間を足していく
				ollEffectTime += info.NextPlayTime;
				groupList.Add(info.PlayTween.tweenGroup, 0);
			}

			// Tweenのグループごとに一番Tween再生にかかる時間を取得する
			UITweener[] tweens = groupTop.GetSafeComponentsInChildren<UITweener>();
			foreach(UITweener tween in tweens)
			{
				if(groupList.ContainsKey(tween.tweenGroup))
				{
					float effectTime = tween.delay + tween.duration;
					if(groupList[tween.tweenGroup] < effectTime)
					{
						groupList[tween.tweenGroup] = effectTime;
					}
				}
			}

			// エフェクトにかかる時間を設定
			foreach(float time in groupList.Values)
			{
				ollEffectTime += time;
			}
			
			return ollEffectTime;
		}
	}
	
	#endregion
}