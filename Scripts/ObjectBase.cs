/// <summary>
/// Scmオブジェクト
/// 
/// 2013/02/19
/// </summary>
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.Master;
using Scm.Common.Packet;
using Scm.Common.GameParameter;

public abstract class ObjectBase : MonoBehaviour
{
	#region フィールド＆プロパティ
	private Manager Manager { get; set; }

	private EntrantInfo entrantInfo;
	public EntrantInfo EntrantInfo
	{
		get
		{
			if(this.entrantInfo == null)
			{
				BugReportController.SaveLogFile(this.gameObject.name + " EntrantInfo is not yet set");
				this.entrantInfo = EntrantInfo.CreateError();
			}
			return entrantInfo;
		}
		private set
		{
			this.entrantInfo = value;
			this.OnSetUserName();
			this.OnSetWinLose();
			this.OnSetAvatarType();
		}
	}

	public int FieldId             { get { return this.EntrantInfo.FieldId; } }
	public int InFieldId           { get { return this.EntrantInfo.InFieldId; } }
	public EntrantType EntrantType { get { return this.EntrantInfo.EntrantType; } }
	public int Id                  { get { return this.EntrantInfo.Id; } }
	public TeamType TeamType       { get { return this.EntrantInfo.TeamType; } }
	public int TacticalId          { get { return this.EntrantInfo.TacticalId; } }
	public string UserName         { get { return this.EntrantInfo.UserName; } }
	public int Win                 { get { return this.EntrantInfo.Win; } }
	public int Lose                { get { return this.EntrantInfo.Lose; } }
	public string UserNameWithTeamColor { get { return GameConstant.StringWithTeamColor(UserName, TeamType); } }

	public Vector3 NextPosition { get; protected set; }
	public Quaternion NextRotation { get; protected set; }
	private StatusType _statusType;
	public StatusType StatusType
	{
		get { return _statusType; }
		private set
		{
			_statusType = value;
			// UI更新
			this.OnSetStatusType();
		}
	}
	public EffectType EffectType { get; private set; }

	/// <summary>
	/// 無敵かどうか
	/// </summary>
	public virtual bool IsInvincible
	{
		get { return this.StatusType == StatusType.Dead; }
	}

	private int _hitPoint;
	public int HitPoint
	{
		// 自爆時など,実際にはHPが残ったまま死んだ場合の見た目対策.
		get { return (this.StatusType == StatusType.Dead ? 0 : _hitPoint); }
		protected set
		{
			_hitPoint = value;
			// UI更新
			this.OnSetHP();
		}
	}
	private int _maxHitPoint;
	public int MaxHitPoint
	{
		get { return _maxHitPoint; }
		protected set
		{
			_maxHitPoint = value;
			// UI更新
			this.OnSetHP();
		}
	}
	public int Level
	{
		get { return this.EntrantInfo.Level; }
		protected set
		{
			this.EntrantInfo.Level = value;
			// UI更新
			this.OnSetLevel();
		}
	}

	private BuffPacketParameter[] buffList = new BuffPacketParameter[0];
	private Dictionary<BuffType, BuffEffect> buffEffectList = new Dictionary<BuffType, BuffEffect>();

	private BuffPacketParameter[] spBuffList = new BuffPacketParameter[0];
	private Dictionary<BuffType, BuffEffect> spBuffEffectList = new Dictionary<BuffType, BuffEffect>();

	public float BuffRunSpeed { get; private set; }
	public bool IsParalysis { get; private set; }
	public bool IsSeal { get; private set; }
	public bool IsPanic { get; private set; }
	public bool IsDisappear { get; private set; }

	/// <summary>
	/// モデルの描画を行うか否か.
	/// </summary>
	public abstract bool IsDrawEnable { get; }

	public bool IsBreakable { get; protected set; }

	public AssetBundleBinder MainAssetBundle { get; private set; }

	// ミニマップアイコン表示
	private GUIMinimapIconItem _minimapIconItem;
	public GUIMinimapIconItem MinimapIconItem
	{
		get
		{
			if (_minimapIconItem == null)
			{
				var s = this.GetComponent<GUIMinimapIconSettings>();
				if (s == null)
					return null;
				_minimapIconItem = s.Icon;
			}
			return _minimapIconItem;
		}
	}

	// 3DUI表示
	private OUIItemRoot _objectUIRoot;
	public OUIItemRoot ObjectUIRoot { get { return _objectUIRoot; } private set { _objectUIRoot = value; } }

	// モデルデータのTransform.
	private AvaterModel avaterModel;
	public AvaterModel AvaterModel
	{
		get
		{
			if (avaterModel == null)
			{
				this.avaterModel = this.gameObject.AddComponent<AvaterModel>();
			}
			return avaterModel;
		}
	}

	public event System.Action LoadModelCompletedEvent;

	#region プロパティ変更時に実行するメソッド.
	protected virtual void OnSetAvatarType()
	{
	}
	protected virtual void OnSetUserName()
	{
		// UI更新
		if (ObjectUIRoot != null) { ObjectUIRoot.UpdateName(); }
	}
	protected virtual void OnSetWinLose()
	{
		// UI更新
		if( ObjectUIRoot != null ) { ObjectUIRoot.UpdateWinLose(); }
	}
	protected void OnSetStatusType()
	{
		// UI更新
		if (ObjectUIRoot != null) { ObjectUIRoot.UpdateDeath(); }
	}
	protected virtual void OnSetHP()
	{
		// UI更新
		if (ObjectUIRoot != null) { ObjectUIRoot.UpdateHP(); }
	}
	protected virtual void OnSetLevel()
	{
		// UI更新
		if (ObjectUIRoot != null) { ObjectUIRoot.UpdateName(); }
	}
	#endregion
	#endregion

	#region 破棄.
	protected virtual void Destroy()
	{
		// ミニマップ連動
		if (this.MinimapIconItem != null)
		{
			this.MinimapIconItem.DeathAnimation();
		}
	}
	protected virtual void OnDestroy()
	{
		if (this.Manager)
		{
			this.Manager.Destroy(this.gameObject);
			this.Manager = null;
		}
		Entrant.RemoveEntrant(this.entrantInfo);
	}
	public void Remove()
	{
		if (this.Manager)
		{
			this.Manager.Destroy(this.gameObject);
			this.Manager = null;
		}
		GameObject.Destroy(this.gameObject);
	}
	#endregion

	#region 設定
	public static short CastInFieldID(int src)
	{
		return (short)src;
	}
	public void SetupBase(Manager manager, EntrantInfo info)
	{
		this.Manager = manager;

		this.EntrantInfo = info;

		this.StatusType = info.StatusType;
		this.EffectType = info.EffectType;
		this.MaxHitPoint = info.MaxHitPoint;
		this.HitPoint = info.HitPoint;
		this.Level = info.Level;

		this.name = GameGlobal.GetObjectBaseName(this);
		this.NextPosition = info.StartPosition;
		this.NextRotation = Quaternion.Euler(0f, info.StartRotation, 0f);

		Entrant.Replace(this);
	}
	// 派生クラスのSetupまで終わった後に呼ぶ.
	protected void SetupCompleted()
	{
		// ObjectUIの読み込み
		this.LoadObjectUI(this.GetObjectUIPath());

		this.EntrantInfo.RunUnsettledPacket(this);
	}
	protected void LoadModelData(EntrantInfo info)
	{
		CharaMasterData data;
		if(MasterData.TryGetChara(info.Id, out data))
		{
            AvatarMasterData skin;
            if (MasterData.TryGetAvatar(info.Id, info.SkinId, out skin)) {
                AssetReference assetReference = AssetReference.GetAssetReference(skin.AssetPath);
                this.SetMainAssetBundle(assetReference);
                this.AvaterModel.Setup(this, assetReference);
            }
		}
	}

    //LWZ:add for chara show
    protected void LoadCharaModel(CharaInfo charaInfo)
    {
        CharaMasterData data;
        if (MasterData.TryGetChara(charaInfo.CharacterMasterID, out data))
        {
            AvatarMasterData skin;
            if(MasterData.TryGetAvatar(charaInfo.CharacterMasterID, charaInfo.SkinId, out skin))
            {
                AssetReference assetReference = AssetReference.GetAssetReference(skin.AssetPath);
                this.SetMainAssetBundle(assetReference);
                this.AvaterModel.Setup(this, assetReference);
            }
        }
    }
    
	public virtual void LoadModelCompleted(GameObject model, AnimationReference animationData)
	{
		if(this.ObjectUIRoot != null)
		{
			this.ObjectUIRoot.ResetWorldParent();
		}
		this.AvaterModel.ChangeDrawEnable(this.IsDrawEnable);
		foreach (BuffEffect buff in buffEffectList.Values)
		{
			buff.ResetParent(this);
		}
		foreach (BuffEffect buff in spBuffEffectList.Values)
		{
			buff.ResetParent(this);
		}
		if (this.LoadModelCompletedEvent != null)
		{
			this.LoadModelCompletedEvent();
		}
	}
	protected virtual string GetObjectUIPath()
	{
		return "";
	}
	/// <summary>
	/// ObjectUI を読み込む
	/// </summary>
	void LoadObjectUI(string assetPath)
	{
		if (string.IsNullOrEmpty(assetPath))
			return;
		AssetReference assetReference = AssetReference.GetAssetReference(AssetReference.CommonAssetName);
		StartCoroutine(this.LoadObjectUICoroutine(assetPath, assetReference));
	}
	IEnumerator LoadObjectUICoroutine(string assetPath, AssetReference assetReference)
	{
		while (!assetReference.IsFinish)
		{
			yield return null;
		}

		var t = assetReference.GetAsset<GUIObjectUIBaseData>(assetPath);
		if (t != null)
		{
			this.ObjectUIRoot = t.Create(this);
		}
	}
	#endregion

	#region 移動パケット
	public virtual void Move(Vector3 oldPosition, Vector3 position, Quaternion rotation, bool forceGrounded)
	{
		this.MoveBase(position, rotation);

		this.NextPosition = position;
		this.NextRotation = rotation;
		this.transform.position = position;
		this.transform.rotation = rotation;
	}
	protected void MoveBase(Vector3 position, Quaternion rotation)
	{
	}
	/// <summary>
	/// 演出などで本来の位置とズレた場合に辻褄を合わせる.
	/// </summary>
	protected void MoveConsistent()
	{
		this.Move(this.transform.position, this.NextPosition, this.NextRotation, false);
		this.Move(this.NextPosition, this.NextPosition, this.NextRotation, false);
	}
	#endregion

	#region スキルモーションパケット
	public virtual bool SkillMotion(int skillID, ObjectBase target, Vector3 position, Quaternion rotation)
	{
		return this.SkillMotionBase(skillID, target, position, rotation);
	}
	protected bool SkillMotionBase(int skillID, ObjectBase target, Vector3 position, Quaternion rotation)
	{
		return true;
	}
	#endregion

	#region 攻撃パケット
	public virtual void Attack(int bulletSetID, ObjectBase target, Vector3 position, Quaternion rotation, Vector3? casterPos, Quaternion casterRot)
	{
		this.AttackBase(bulletSetID, target, position, rotation, casterPos, casterRot);
	}
	protected void AttackBase(int bulletSetID, ObjectBase target, Vector3 position, Quaternion rotation, Vector3? casterPos, Quaternion casterRot)
	{
		SkillBulletSetMasterData bulletSet;

		if (MasterData.TryGetBulletSet(bulletSetID, out bulletSet))
		{
			this.CreateBullet(target, null, position, rotation, bulletSet.SkillID, bulletSet);
		}
		else
		{
			BugReportController.SaveLogFile("BulletSet ID " + bulletSetID + " not Found");
		}
	}
	/// <summary>
	/// 弾丸生成
	/// </summary>
	/// <param name="skillID"></param>
	/// <param name="target"></param>
	/// <param name="position"></param>
	/// <param name="rotation"></param>
	/// <param name="skillData"></param>
	public void CreateBullet(ObjectBase target, Vector3? targetPosition, Vector3 position, Quaternion rotation, int skillID, IBulletSetMasterData bulletSet)
	{
		SkillBulletMasterData bullet = bulletSet.Bullet;
		if (bullet == null)
			return;
		// 弾丸発射音
		SoundController.CreateSeObject(position, rotation, bullet.ShotSeFile);

		this.CreateBulletStart(bullet);

		// 弾丸生成
		switch (bullet.Type)
		{
		case SkillBulletMasterData.BulletType.Normal:
		case SkillBulletMasterData.BulletType.Pierce:
		case SkillBulletMasterData.BulletType.WallPierce:
			this.CreateBulletAmmo(target, targetPosition, position, rotation, skillID, bulletSet);
			break;
		case SkillBulletMasterData.BulletType.Birth:
		case SkillBulletMasterData.BulletType.Nearest:
			this.CreateBulletBirth(target, targetPosition, position, rotation, skillID, bulletSet);
			break;
		case SkillBulletMasterData.BulletType.SelfNormal:
		case SkillBulletMasterData.BulletType.SelfPierce:
			this.CreateBulletSelf(target, targetPosition, position, rotation, skillID, bulletSet);
			break;
		case SkillBulletMasterData.BulletType.FallNormal:
		case SkillBulletMasterData.BulletType.FallPierce:
			this.CreateBulletFall(target, targetPosition, position, rotation, skillID, bulletSet);
			break;
		case SkillBulletMasterData.BulletType.WallAlong:
			this.CreateBulletAlong(target, targetPosition, position, rotation, skillID, bulletSet);
			break;
		}
	}
	/// <summary>
	/// 弾丸生成開始時処理
	/// </summary>
	/// <param name="bullet"></param>
	protected virtual void CreateBulletStart(SkillBulletMasterData bullet)
	{
		this.CreateBulletStartBase(bullet);
	}
	protected void CreateBulletStartBase(SkillBulletMasterData bullet)
	{
	}
	/// <summary>
	/// 弾丸タイプ「弾丸」生成
	/// </summary>
	/// <param name="skillID"></param>
	/// <param name="target"></param>
	/// <param name="position"></param>
	/// <param name="rotation"></param>
	/// <param name="skillData"></param>
	protected virtual void CreateBulletAmmo(ObjectBase target, Vector3? targetPosition, Vector3 position, Quaternion rotation, int skillID, IBulletSetMasterData bulletSet)
	{
		this.CreateBulletAmmoBase(target, targetPosition, position, rotation, skillID, bulletSet);
	}
	protected void CreateBulletAmmoBase(ObjectBase target, Vector3? targetPosition, Vector3 position, Quaternion rotation, int skillID, IBulletSetMasterData bulletSet)
	{
		EffectManager.CreateBulletAmmo(this.EntrantInfo, target, targetPosition, position, rotation, skillID, bulletSet);
	}
	/// <summary>
	/// 弾丸タイプ「発生」生成
	/// </summary>
	/// <param name="skillID"></param>
	/// <param name="target"></param>
	/// <param name="position"></param>
	/// <param name="rotation"></param>
	/// <param name="skillData"></param>
	protected virtual void CreateBulletBirth(ObjectBase target, Vector3? targetPosition, Vector3 position, Quaternion rotation, int skillID, IBulletSetMasterData bulletSet)
	{
		this.CreateBulletBirthBase(target, targetPosition, position, rotation, skillID, bulletSet);
	}
	protected void CreateBulletBirthBase(ObjectBase target, Vector3? targetPosition, Vector3 position, Quaternion rotation, int skillID, IBulletSetMasterData bulletSet)
	{
		EffectManager.CreateBulletBirth(this.EntrantInfo, target, targetPosition, position, rotation, skillID, bulletSet);
	}
	/// <summary>
	/// 弾丸タイプ「自分弾丸」生成
	/// </summary>
	/// <param name="skillID"></param>
	/// <param name="target"></param>
	/// <param name="position"></param>
	/// <param name="rotation"></param>
	/// <param name="skillData"></param>
	protected virtual void CreateBulletSelf(ObjectBase target, Vector3? targetPosition, Vector3 position, Quaternion rotation, int skillID, IBulletSetMasterData bulletSet)
	{
		this.CreateBulletSelfBase(target, targetPosition, position, rotation, skillID, bulletSet);
	}
	protected void CreateBulletSelfBase(ObjectBase target, Vector3? targetPosition, Vector3 position, Quaternion rotation, int skillID, IBulletSetMasterData bulletSet)
	{
		EffectManager.CreateBulletSelf(this.EntrantInfo, target, targetPosition, position, rotation, skillID, bulletSet);
	}
	/// <summary>
	/// 弾丸タイプ「弾丸落下」生成
	/// </summary>
	/// <param name="skillID"></param>
	/// <param name="target"></param>
	/// <param name="position"></param>
	/// <param name="rotation"></param>
	/// <param name="skillData"></param>
	protected virtual void CreateBulletFall(ObjectBase target, Vector3? targetPosition, Vector3 position, Quaternion rotation, int skillID, IBulletSetMasterData bulletSet)
	{
		this.CreateBulletFallBase(target, targetPosition, position, rotation, skillID, bulletSet);
	}
	protected void CreateBulletFallBase(ObjectBase target, Vector3? targetPosition, Vector3 position, Quaternion rotation, int skillID, IBulletSetMasterData bulletSet)
	{
		EffectManager.CreateBulletFall(this.EntrantInfo, target, targetPosition, position, rotation, skillID, bulletSet);
	}
	/// <summary>
	/// 弾丸タイプ「壁沿い弾丸」生成
	/// </summary>
	/// <param name="skillID"></param>
	/// <param name="target"></param>
	/// <param name="position"></param>
	/// <param name="rotation"></param>
	/// <param name="skillData"></param>
	protected virtual void CreateBulletAlong(ObjectBase target, Vector3? targetPosition, Vector3 position, Quaternion rotation, int skillID, IBulletSetMasterData bulletSet)
	{
		this.CreateBulletAlongBase(target, targetPosition, position, rotation, skillID, bulletSet);
	}
	protected void CreateBulletAlongBase(ObjectBase target, Vector3? targetPosition, Vector3 position, Quaternion rotation, int skillID, IBulletSetMasterData bulletSet)
	{
		EffectManager.CreateBulletAlong(this.EntrantInfo, target, targetPosition, position, rotation, skillID, bulletSet);
	}
	#endregion

	#region ヒットパケット
	public virtual void Hit(HitInfo hitInfo)
	{
		this.HitBase(hitInfo);
	}
	protected void HitBase(HitInfo hitInfo)
	{
		this.Effect(hitInfo);
		this.Status(hitInfo);
		this.HitPoint = hitInfo.hitPoint;

		{
			// プレイヤーが撃った弾かどうか.
			// 撃った後プレイヤーキャラクターが変わった場合も考慮し,PlayerInfoが登録されているInFieldIdかどうかで判断.
			PlayerInfo pInfo;
			var isAttackPlayer = Entrant.TryGetEntrant<PlayerInfo>(hitInfo.inFieldAttackerId, out pInfo);
			// ダメージ表示
			if (ObjectUIRoot != null)
			{
				this.ObjectUIRoot.Damage(hitInfo.damage, hitInfo.damageType, isAttackPlayer, this is Player);
			}
			// プレイヤーが攻撃を当てた時のアニメーション処理
			if (isAttackPlayer && hitInfo.damage > 0)
			{
				// UNDONE:ロックオンカーソルアニメーションはバフダメージでもアニメーションする
				// ロックオンカーソルアニメーション
				GUIObjectUI.LockonHitAnimation();
				// HPゲージの揺れ演出
				if (this.ObjectUIRoot != null)
					this.ObjectUIRoot.ShakeHP();
				// システムヒットSE.
				SoundController.PlaySe(SoundController.CueName_SysHit);
			}
		}

		ObjectBase attacker = GameController.SearchInFieldID(hitInfo.inFieldAttackerId);
		Player player = attacker as Player;
		if (player)
		{
			// 経験値
			if (hitInfo.exp > 0)
			{
				player.AddExp(hitInfo.exp);
				this.CreateExpEffectBase(attacker, hitInfo.exp);
			}
			/*
			// お金
			if (hitInfo.money > 0)
			{
				player.AddMoney(hitInfo.money);
			}
			*/

			// カメラを揺らす
			CharacterCamera.GiveDamageShake(player.transform, hitInfo);
		}

		Gadget gadget = attacker as Gadget;
		if ((gadget != null) && gadget.IsChildOfPlayer ()) 
		{
			// 経験値
			if (hitInfo.exp > 0)
			{
				Player p = GameController.GetPlayer();
				if (p != null)
				{
					p.AddExp(hitInfo.exp);
					this.CreateExpEffectBase(p, hitInfo.exp);
				}
			}
		}

		// キル情報
		if (hitInfo.statusType == StatusType.Dead)
		{
			if (this is Character && attacker is Player)
			{
				// キル情報演出
				GUIEffectMessage.SetKillInfo(hitInfo.killCount, (AvatarType)this.Id, (attacker as Player).SkinId, this.UserNameWithTeamColor, BattleMain.CharaIcon);
			}
			else if (this is Player && attacker is Character)
			{
				// 死亡情報演出
				GUIEffectMessage.SetDeadInfo((AvatarType)attacker.Id, (attacker as Character).SkinId, attacker.UserNameWithTeamColor, BattleMain.CharaIcon);
			}

			if (this is Character && attacker is Character)
			{
				// [死亡者リスト]UI演出エフェクト表示.
				//GUIKillNameList.AddKillName(attacker, this);
				GUIBreakInfo.Add(attacker, this);
			}

		}

		SkillBulletMasterData bullet;
		if (SkillBulletMasterClient.Instance.TryGetMasterData(hitInfo.bulletID, out bullet))
		{
			Quaternion rotation = Quaternion.Euler(0, hitInfo.bulletDirection, 0);
			// ヒットエフェクト
			EffectManager.CreateHit(hitInfo.position, rotation, bullet);
			// ヒットSE
			SoundManager.CreateSeObject(hitInfo.position, rotation, bullet.HitSeFile);
		}

		GrappleAttach.Create(attacker, this, hitInfo);

		// ミニマップ連動
		if (0 < hitInfo.damage)
		{
			if (this.MinimapIconItem != null) this.MinimapIconItem.DamageAnimation();
		}
	}
	#endregion

	#region 状態異常パケット
	public virtual void Status(HitInfo hitInfo)
	{
		this.StatusBase(hitInfo.statusType);
	}
	protected void StatusBase(StatusType statusType)
	{
		this.StatusType = statusType;

		// 状態異常処理
		switch(this.StatusType)
		{
		case StatusType.Dead:
			// 死亡
			this.Destroy();
			break;
		}
	}
	#endregion

	#region 効果パケット
	public virtual void Effect(HitInfo hitInfo)
	{
		this.EffectBase(hitInfo.effectType);
	}
	protected void EffectBase(EffectType effectType)
	{
		this.EffectType = effectType;
	}
	#endregion

	#region リスポーンパケット(現在はリスポーンパケットではなくSetupCharacterからしか呼ばれていない).
	public virtual void Respawn(Vector3 position, Quaternion rotation)
	{
		this.RespawnBase(position, rotation);
	}
	protected void RespawnBase(Vector3 position, Quaternion rotation)
	{
		// 位置
		this.transform.localPosition = position;
		// 方向
		this.transform.localRotation = rotation;
		// ヒットポイント
		this.HitPoint = this.MaxHitPoint;
		// ミニマップ連動
		if (this.MinimapIconItem != null) this.MinimapIconItem.CreateAnimation();
	}
	#endregion

	#region ワープパケット
	public virtual void Warp(Vector3 position, Quaternion rotation)
	{
		this.WarpBase(position, rotation);
	}
	protected void WarpBase(Vector3 position, Quaternion rotation)
	{
		// 位置
		this.transform.localPosition = position;
		// 方向
		this.transform.localRotation = rotation;
	}
	#endregion

	#region ジャンプパケット
	public virtual void Jump(Vector3 position, Quaternion rotation)
	{
		this.WarpBase(position, rotation);
	}
	public virtual void Wire(Vector3 position, Quaternion rotation)
	{
		this.WarpBase(position, rotation);
	}
	public virtual void Captured(Vector3 position, Quaternion rotation)
	{
		this.WarpBase(position, rotation);
	}
	#endregion

	#region レベルアップパケット
	public virtual void LevelUp(int level, int hitPoint, int maxHitPoint)
	{
		this.LevelUpBase(level, hitPoint);
	}
	protected void LevelUpBase(int level, int hitPoint)
	{
		this.Level = level;
		// 2014/07/22 HP全快処理を外す.
		//this.HitPoint = this.MaxHitPoint;
		//this.StatusType = StatusType.Normal;

		// レベルアップしたと時のヒットポイントを設定
		this.HitPoint = hitPoint;
	}
	#endregion

	#region モーションパケット
	public virtual void Motion(Scm.Common.GameParameter.MotionState motionstate) { }
	#endregion

	#region スキルチャージパケット
	public virtual void SkillCharge(int skillID, ObjectBase target, bool isCharge)
	{
		this.SkillChargeBase(skillID, target, isCharge);
	}
	protected void SkillChargeBase(int skillID, ObjectBase target, bool isCharge)
	{
	}
	#endregion

	#region 生成＆破壊時エフェクト.
	protected virtual void CreatePopEffect() { }
	protected virtual void CreateBrokenEffect() { }
	protected void CreateEffect(string effectName)
	{
		if(!string.IsNullOrEmpty(effectName))
		{
			EffectManager.CreateSelfDestroy(this.transform.position, this.transform.rotation, effectName, false);
		}
	}
	protected void CreateSe(string cueName)
	{
		if(!string.IsNullOrEmpty(cueName))
		{
			SoundManager.CreateSeObject(this.transform.position, this.transform.rotation, cueName);
		}
	}
	#endregion

	#region 経験値処理
	protected void CreateExpEffectBase(ObjectBase target, int money)
	{
		// ExpEffect生成.
		ExpEffect.CreateExpEffect(this.transform.position, ExpEffectOffsetMin, ExpEffectOffsetMax, target, money);
	}
	protected virtual Vector3 ExpEffectOffsetMin { get { return Vector3.zero; } }
	protected virtual Vector3 ExpEffectOffsetMax { get { return Vector3.zero; } }
	#endregion

	#region 投げ
	public virtual void Grapple(GrappleAttach grappleAttach) { }
	public virtual void GrappleFinish(GrappleAttach grappleAttach)
	{
		// ターゲットが自分.
		if (grappleAttach.Target == this)
		{
			// 投げ演出中におかしくなった位置を戻す.
			this.MoveConsistent();
		}
	}
	#endregion

	#region バフ処理
	public virtual void ChangeBuff(BuffType[] newBuffTypes, BuffPacketParameter[] buffPacket, BuffPacketParameter[] spBuffPacket, float speed)
	{
		// バフリストを更新
		buffList = buffPacket;
		spBuffList = spBuffPacket;
		try
		{
			// 各種バフに対して処理をする.
			buffEffectList = ChangeBuffParam(buffList, buffEffectList);
			spBuffEffectList = ChangeBuffParam(spBuffList, spBuffEffectList);
		}
		catch (System.Exception e)
		{
			BugReportController.SaveLogFileWithOutStackTrace(e.ToString());
		}

		// 移動速度バフの適用.
		this.BuffRunSpeed = speed;

		// SPバフの適用.
		this.IsParalysis = false;
		this.IsSeal = false;
		this.IsPanic = false;
		foreach (BuffPacketParameter spBuff in spBuffList)
		{
			switch (spBuff.BuffType)
			{
			case BuffType.PARALYZE:
				this.IsParalysis = true;
				break;
			case BuffType.SEAL:
				this.IsSeal = true;
				break;
			case BuffType.PANIC:
				this.IsPanic = true;
				break;
			}
		}
		// バフの適用.
		this.IsDisappear = false;
		foreach (BuffPacketParameter buff in buffList)
		{
			switch (buff.BuffType)
			{
			case BuffType.Disappear:
				this.IsDisappear = true;
				break;
			}
		}
		this.AvaterModel.ChangeDrawEnable(this.IsDrawEnable);
	}

	private Dictionary<BuffType, BuffEffect> ChangeBuffParam(BuffPacketParameter[] buffList, Dictionary<BuffType, BuffEffect> buffEffectList)
	{
		// 一旦全エフェクトに破棄フラグを立てる.
		foreach (BuffEffect buff in buffEffectList.Values)
		{
			buff.IsAlive = false;
		}

		// 新しいエフェクトリストの作成.
		var newBuffEffectList = new Dictionary<BuffType, BuffEffect>();
		foreach (BuffPacketParameter param in buffList)
		{
			// まだ新リストに存在しないParameterのみ処理.
			if (!newBuffEffectList.ContainsKey(param.BuffType))
			{
				BuffEffect effect;
				if (buffEffectList.TryGetValue(param.BuffType, out effect))
				{
					// 旧リストに存在するなら引き継ぎ.
					effect.IsAlive = true;
					newBuffEffectList.Add(param.BuffType, effect);
				}
				else
				{
					// 旧リストに存在しないなら新規作成.
					effect = BuffEffect.Create(this, param.BuffType);
					if (effect != null)
					{
						effect.IsAlive = true;
						newBuffEffectList.Add(param.BuffType, effect);
					}
				}
			}
		}

		return newBuffEffectList;
	}
	#endregion

	#region アセットバンドル処理
	/// <summary>
	/// メインで使うアセットバンドルを設定する.
	/// </summary>
	protected void SetMainAssetBundle(AssetReference assetReference)
	{
        if(MainAssetBundle == null)
        {
            MainAssetBundle = this.gameObject.AddComponent<AssetBundleBinder>();
        }
		
		MainAssetBundle.SetAssetReference(assetReference);
	}
	#endregion
}
