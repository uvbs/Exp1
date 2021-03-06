/// <summary>
/// 3Dオブジェクトに対するUIのアイテムを管理するルート
/// 
/// 2014/06/23
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.GameParameter;
using Scm.Common.Packet;

public class OUIItemRoot : MonoBehaviour
{
	#region フィールド＆プロパティ
	/// <summary>
	/// ObjectBase が消えた時にこのオブジェクトを削除するまでの時間
	/// </summary>
	[SerializeField]
	float _destroyTimer = 1f;
	public float DestroyTimer { get { return _destroyTimer; } }

	[SerializeField]
	private UIPanel uiPanel;
	public UIPanel UIPanel { get { return uiPanel; } }

	// 親のObjectUI
	GUIObjectUI ObjectUI { get; set; }
	// 表示範囲（プレイヤーとObjectBaseの距離）
	float DrawRange { get; set; }
	// 対象となるObjectBase
	ObjectBase ObjectBase { get; set; }
	// ObjectBase内の参照するTransform（位置を設定するのに使う）
	Transform WorldParent { get; set; }
	// アタッチするボーン名.
	string ModelAttachName { get; set; }

	// アイテムのインスタンス
	ItemObject Item { get; set; }
	[System.Serializable]
	public class ItemObject
	{
		public OUIItemAttach attach;
		public OUIItemBG bg;
		public OUIItemBuff buff;
		public OUIItemDamage damage;
		public OUIItemHP hp;
		public OUIItemKill kill;
		public OUIItemName name;
		public OUIItemStatus status;
		public OUIItemRanking ranking;
		public OUIItemScoreRank scoreRank;
		public OUIItemWinLose winlose;
		public OUIItemResidentProgress process;
        public OUIItemRecruitment recruitment;
	}
	// インスタンス化したアイテムリスト
	List<OUIItemBase> ItemList { get; set; }
	// ObjectBaseのワールド座標（ObjectBaseが消滅すると消滅前の位置がずっと残る）
	Vector3 WorldPosition { get; set; }
	// ObjectBaseが消滅したかどうかモニターするメソッド
	System.Action DestroyMonitorFunc = () => { };

	/// <summary>
	/// ロックオンされているかどうか
	/// </summary>
	bool IsLockon
	{
		get
		{
			var o = GUIObjectUI.LockonObject;
			if (o == null)
				return false;
			return this.ObjectBase == o;
		}
	}
	public TeamType TeamType { get { return (this.ObjectBase != null ? this.ObjectBase.TeamType : TeamType.Unknown); } }
	#endregion

	#region 宣言
	/// <summary>
	/// 設定
	/// </summary>
	[System.Serializable]
	public class Settings
	{
		public bool isEnable = false;
		public bool isInRangeDraw = false;
		public string attachName = "";
		public Vector3 offset = Vector3.zero;
	}
	[System.Serializable]
	public class AttachSettings
	{
		public bool isEnable = false;
		public Vector3 offset = Vector3.zero;
	}
	[System.Serializable]
	public class HPSettings
	{
		public bool isEnable = false;
		public bool isInRangeDraw = false;
		public string attachName = "";
		public string myteamItemName = "";
		public string enemyItemName = "";
		public string etcItemName = "";
		public Vector3 offset = Vector3.zero;
	}
	[System.Serializable]
	public class DamageSettings
	{
		public bool isEnable = false;
		public string attachName = "";
		public Vector3 offset = Vector3.zero;
	}
	#endregion

	#region 作成
	public static OUIItemRoot Create(GUIObjectUI objectUI, ObjectBase o, string modelAttachName, float drawRange)
	{
		var go = SafeObject.Instantiate(objectUI.Attach.prefab.root) as GameObject;
		if (go == null)
			return null;
		var com = go.GetComponent(typeof(OUIItemRoot)) as OUIItemRoot;
		if (com == null)
			return null;
		go.SetActive(true);

		// 親子付け
		var t = com.transform;
		t.parent = objectUI.Attach.rootObject;
		t.localPosition = Vector3.zero;
		t.localRotation = Quaternion.identity;
		t.localScale = Vector3.one;

		// セットアップ
		com.Setup(objectUI, o, modelAttachName, drawRange);

		return com;
	}
	public void Setup(GUIObjectUI objectUI, ObjectBase o, string modelAttachName, float drawRange)
	{
		// 名前設定
		this.name = GameGlobal.GetObjectBaseName(o, null, true);

		// 一度設定したら変更はしない
		this.ObjectUI = objectUI;
		this.DrawRange = drawRange;
		this.ObjectBase = o;
		this.ModelAttachName = modelAttachName;
		// 参照するモデルの Transform を取得する
		this.ResetWorldParent();

		// 変更あり
		this.Item = new ItemObject();
		this.ItemList = new List<OUIItemBase>();
		this.WorldPosition = Vector3.zero;
		this.DestroyMonitorFunc = this.DestroyMonitor;
	}
	public void ResetWorldParent()
	{
		if(this.ObjectBase != null)
		{
			// 参照するモデルの Transform を取得する
			this.WorldParent = null;
			if (!string.IsNullOrEmpty(this.ModelAttachName))
				this.WorldParent = this.ObjectBase.transform.Search(this.ModelAttachName);
			if (this.WorldParent == null)
				this.WorldParent = this.ObjectBase.transform;
		}
	}
	#endregion

	#region 更新
	void LateUpdate()
	{
		// オブジェクトが消滅したらモニタリングをやめて自分自身を削除する
		this.DestroyMonitorFunc();

		// 位置更新
		{
			if (this.ObjectBase != null)
				this.WorldPosition = this.WorldParent.position;
			this.ObjectUI.UpdatePosition(this.transform, this.WorldPosition);
		}

		// 表示設定
		if (this.ObjectBase != null)
		{
			bool isInRange = this.ObjectUI.IsInRange(this.WorldPosition, this.DrawRange);
			bool isLockon = this.IsLockon;
			this.SetActive(isLockon || isInRange);
			foreach (var item in this.ItemList)
				item.SetActive(isLockon, isInRange);
		}
	}
	private void SetActive(bool isActive)
	{
		if(this.uiPanel != null && this.uiPanel.enabled != isActive)
		{
			this.uiPanel.enabled = isActive;
		}
	}

	void DestroyMonitor()
	{
		// オブジェクトが消滅したらモニタリングをやめて自分自身を削除する
		if (this.ObjectBase != null)
			return;

		// 自分自身はタイマーで削除
		Object.Destroy(this.gameObject, this.DestroyTimer);
		foreach (var item in this.ItemList)
			item.Destroy(this.DestroyTimer);

		// モニター解除
		this.DestroyMonitorFunc = () => { };
	}
	#endregion

	#region アイテム操作
	bool CreateItem<T>(Settings s, ref T item, System.Func<GUIObjectUI.AttachObject.Prefab, ObjectBase, T> getPrefabFunc) where T : OUIItemBase
	{
		// 使用しない場合はアイテムを作成しない
		if (!s.isEnable)
			return false;

		// インスタンス化するプレハブを取得する
		// シーンによっては必要のないアイテムがあるのでその時のプレハブは null で帰ってくる
		var prefab = getPrefabFunc(this.ObjectUI.Attach.prefab, this.ObjectBase);
		if (prefab == null)
			return false;
		// アタッチ用オブジェクトから親となるTransformを検索する
		// なければItemRootが親となる
		Transform parent = null;
		if (!string.IsNullOrEmpty(s.attachName))
		{
			if (this.Item.attach != null)
				parent = this.Item.attach.transform.Search(s.attachName);
		}
		if (parent == null)
			parent = this.transform;
		// アイテムをインスタンス化する
		var com = OUIItemBase.Create(this, prefab, parent, s.isInRangeDraw, s.offset) as T;
		if (com == null)
			return false;
		// UIWidgetを有効にする.
		var uiWidget = parent.GetComponent<UIWidget>();
		if(uiWidget != null)
		{
			uiWidget.enabled = true;
		}

		// 既にアイテムが作成されている
		if (item != null)
		{
			// 新しい方に置き換える
			Debug.LogWarning("OUIItemRoot.CreateItem:既にアイテムが作成されているので置き換える");
			this.ItemList.Remove(item);
			Object.Destroy(item.gameObject);
		}

		// アイテム格納
		item = com;
		this.ItemList.Add(com);
		return true;
	}
	#endregion

	#region 各種アイテム用アタッチ
	/// <summary>
	/// セットアップ
	/// </summary>
	public void SetupAttach(AttachSettings s)
	{
		Settings settings = new Settings
		{
			isEnable = s.isEnable,
			offset = s.offset,
		};
		this.CreateItem(settings, ref this.Item.attach, OUIItemAttach.GetPrefab);
	}
	#endregion

	#region BG
	/// <summary>
	/// セットアップ
	/// </summary>
	public void SetupBG(Settings s)
	{
		this.CreateItem(s, ref this.Item.bg, OUIItemBG.GetPrefab);
	}
	#endregion

	#region バフ
	/// <summary>
	/// セットアップ
	/// </summary>
	public void SetupBuff(Settings s)
	{
		this.CreateItem(s, ref this.Item.buff, OUIItemBuff.GetPrefab);
		this.UpdateBuff();
	}
	/// <summary>
	/// アイテム更新
	/// </summary>
	public void UpdateBuff()
	{
		var item = this.Item.buff;
		if (item == null)
			return;
		item.UpdateUI();
	}
	/// <summary>
	/// バフアイコンセット処理
	/// </summary>
	public void SetBuffIcon(LinkedList<BuffInfo> buffInfoList)
	{
		var item = this.Item.buff;
		if (item == null)
			return;
		item.SetupIcon(buffInfoList);
	}
	#endregion

	#region ダメージ
	/// <summary>
	/// セットアップ
	/// </summary>
	public void SetupDamage(DamageSettings s)
	{
		Settings settings = new Settings
		{
			isEnable = s.isEnable,
			attachName = s.attachName,
			offset = s.offset,
		};
		this.CreateItem(settings, ref this.Item.damage, OUIItemDamage.GetPrefab);
	}
	/// <summary>
	/// ダメージ表示
	/// </summary>
	public void Damage(int damage, DamageType damageType, bool atkisPlayer, bool defisPlayer)
	{
		var item = this.Item.damage;
		if (item == null)
			return;
		item.Damage(damage, damageType, atkisPlayer, defisPlayer);
	}
	#endregion

	#region HP
	/// <summary>
	/// セットアップ
	/// </summary>
	public void SetupHP(HPSettings s)
	{
		Settings settings = new Settings
		{
			isEnable = s.isEnable,
			isInRangeDraw = s.isInRangeDraw,
			attachName = s.attachName,
			offset = s.offset,
		};
		this.CreateItem(settings, ref this.Item.hp,
			(p, o) =>
			{
				return OUIItemHP.GetPrefab(this.ObjectUI.Attach.prefab.hp.root, o, s.myteamItemName, s.enemyItemName, s.etcItemName);
			});
		this.UpdateHP();
	}
	/// <summary>
	/// アイテム更新
	/// </summary>
	public void UpdateHP()
	{
		var item = this.Item.hp;
		if (item == null)
			return;
		var o = this.ObjectBase;
		if (o == null)
			return;
		item.UpdateUI(o.HitPoint, o.MaxHitPoint);
	}
	/// <summary>
	/// 揺れ演出
	/// </summary>
	public void ShakeHP()
	{
		var item = this.Item.hp;
		if (item == null)
			return;
		item.ShakeGauge();
	}
	#endregion

	#region キル数
	/// <summary>
	/// セットアップ
	/// </summary>
	public void SetupKill(Settings s)
	{
		this.CreateItem(s, ref this.Item.kill, OUIItemKill.GetPrefab);
		this.UpdateKill();
	}
	/// <summary>
	/// アイテム更新
	/// </summary>
	public void UpdateKill()
	{
		var item = this.Item.kill;
		if (item == null)
			return;
		var c = this.ObjectBase as Character;
		if (c == null)
			return;
		item.UpdateUI(c.KillCount);
	}
	#endregion

	#region 名前
	/// <summary>
	/// セットアップ
	/// </summary>
	public void SetupName(Settings s)
	{
		this.CreateItem(s, ref this.Item.name, OUIItemName.GetPrefab);
		this.UpdateName();
	}
	/// <summary>
	/// アイテム更新
	/// </summary>
	public void UpdateName()
	{
		var item = this.Item.name;
		if (item == null)
			return;
		var o = this.ObjectBase;
		if (o == null)
			return;
		string objectName = GameGlobal.GetObjectBaseName(o, null, false);
		item.UpdateName(objectName, o.TeamType, o.EntrantType, o.Level);
	}
	public void UpdateDeath()
	{
		var item = this.Item.name;
		if (item == null)
			return;
		var o = this.ObjectBase;
		if (o == null)
			return;
		item.UpdateDeath(o.StatusType == StatusType.Dead, o.EntrantType);
	}
	#endregion

	#region 状態表示
	/// <summary>
	/// セットアップ
	/// </summary>
	public void SetupStatus(Settings s)
	{
		this.CreateItem(s, ref this.Item.status, OUIItemStatus.GetPrefab);
		this.UpdateStatus();
	}
	/// <summary>
	/// アイテム更新
	/// </summary>
	public void UpdateStatus()
	{
		var item = this.Item.status;
		if (item == null)
			return;
		item.UpdateUI();
	}
	#endregion

	#region ランキング上位者特殊アイコン
	/// <summary>
	/// セットアップ
	/// </summary>
	public void SetupRanking(Settings s)
	{
		this.CreateItem(s, ref this.Item.ranking, OUIItemRanking.GetPrefab);
		this.UpdateRanking();
	}
	/// <summary>
	/// アイテム更新
	/// </summary>
	public void UpdateRanking()
	{
		var item = this.Item.ranking;
		if (item == null)
			return;
		item.UpdateUI();
	}
	#endregion

	#region 戦闘中のスコア順位アイコン
	public void SetupScoreRank( Settings s )
	{
		this.CreateItem(s,ref this.Item.scoreRank, OUIItemScoreRank.GetPrefab);
		this.UpdateScoreRank();
	}

	public void UpdateScoreRank()
	{
		var item = this.Item.scoreRank;
		if (item == null)
			return;
		var c = this.ObjectBase as Character;
		if (c == null)
			return;

		item.UpdateUI(c.ScoreRank);
	}
	#endregion

	#region 勝敗数
	/// <summary>
	/// セットアップ
	/// </summary>
	public void SetupWinLose( Settings s ) {

		this.CreateItem( s, ref this.Item.winlose, OUIItemWinLose.GetPrefab );
		this.UpdateWinLose();
	}
	/// <summary>
	/// 勝敗数更新
	/// </summary>
	public void UpdateWinLose() {

		var item = this.Item.winlose;
		if( item == null )	return;

		var o = this.ObjectBase;
		if( o == null )	return;
		item.UpdateWinLose( o.EntrantInfo.Win, o.EntrantInfo.Lose );
	}
	#endregion


    #region Resident
    public void SetupResidentProcess(Settings s)
    {

        this.CreateItem(s, ref this.Item.process, OUIItemResidentProgress.GetPrefab);
        this.UpdateResidentProcess(null, false);
    }

    public void UpdateResidentProcess(Scm.Common.Packet.ResidentAreaSideGaugeEvent pack, bool pBlue = false)
    {
        var item = this.Item.process;
        if (item == null) return;

        var o = this.ObjectBase as ResidentArea;
        if (o == null) return;
        item.UpdateUI(pack, pBlue);
    }

    public void RemoveResidentProcess()
    {
        //destroy will call 
        this.ObjectBase = null;
    }
    #endregion

    #region Recruitment
    public void SetRecruitment(Settings s) {
        this.CreateItem(s, ref this.Item.recruitment, OUIItemRecruitment.GetPrefab);
        this.UpdateRecruitment();
    }

    public void UpdateRecruitment() {
        var item = this.Item.recruitment;
        if (item == null) {
            return;
        }

        var o = this.ObjectBase;
        if (o == null) {
            return;
        }

        var recruitment = Entrant.GetRecruitment(o.InFieldId);
        Debug.Log("<color=#00ff00>GetRecruitment of:" + o.InFieldId + "=" + recruitment + "</color>");
        string text = recruitment != null ? recruitment.Text : string.Empty;
        Debug.Log("<color=#00ff00>text=" + text + "</color>");
        item.UpdateRecruitment(o.InFieldId, recruitment != null, text);
    }
    #endregion
}
