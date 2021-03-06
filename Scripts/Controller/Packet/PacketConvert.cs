/// <summary>
/// パケットからクライアントで使うデータに変換するクラス群
/// 
/// また、パケットの項目が増減した時にこのクラスで吸収することによって
/// ソース全体に波及するのを防いだりする目的で使う
/// 
/// 2016/05/22
/// </summary>
using UnityEngine;
using System.Collections.Generic;
using System;

using Scm.Common.Packet;
using Scm.Common.GameParameter;

using Asobimo.WebAPI;
using Scm.Common.Master;

#region MemberInfo
[System.Serializable]
public class MemberInfo
{
	public int inFieldID;
	public int tacticalId;
	public AvatarType avatarType;
    public int skinId;
	public string name;
	public TeamTypeClient teamType;
	public int rank;
	public BattleRank battleRank;
	
	public int baseScore;
	public int etcScore;
	public int kill;
	public int killScore;
	public int death;
	public int subTowerDefeatCount;
	public int subTowerDefeatScore;
	public int mainTowerDefeatCount;
	public int mainTowerDefeatScore;
	public int guardNpcDefeatCount;
	public int guardNpcDefeatScore;
	public int winBonus;
	public JudgeType judge;
	public int startPlayerGradeID;
	public int endPlayerGradeID;
	public int startGradePoint;
	public int endGradePoint;
	public PlayerGradeState playerGradeState;

	public AvatarInfo avatarInfo;

    public int attack;
    public int controlTime;

    public int score;
    public int gainExp;
    public int gainEnergy;
    public int gainCoin;
    public int gainGold;
    public int gainScore;

    public GameResultRewardCharacter rewardChara;
}
#endregion

#region HitInfo
/// <summary>
/// HitEvent, HitRes, StatusEvent, EffectEvent
/// </summary>
public class HitInfo
{
	public int damage;
	public int hitPoint;
	public DamageType damageType;
	public StatusType statusType;
	public EffectType effectType;
	public int inFieldAttackerId;
	public int exp;
	//public int money;
	//public float respawnTime;
	public int blownAngleType;
	//public float dizzyTime;
	public int bulletID;
	public float bulletDirection;
	public Vector3 position;
	public int killCount;
    public float respawnTime = GameConstant.RespawnTime;
    public float remaingTime = GameConstant.RespawnTime;

    public HitInfo() { }
	public HitInfo(HitEvent packet)
	{
		DamageEvent damageEvent = packet.GetDamageEvent();

		this.damage = damageEvent.Damage;
		this.hitPoint = damageEvent.HitPoint;
		this.damageType = damageEvent.DamageType;
		this.statusType = damageEvent.StatusType;
		this.effectType = damageEvent.EffectType;
		this.inFieldAttackerId = damageEvent.InFieldAttackerId;
		this.killCount = damageEvent.Kill;
        this.respawnTime = packet.RespawnTime;
        this.remaingTime = packet.RemainingTime;

		ExpEvent expEvent = packet.GetExpEvent();

		this.exp = expEvent.Exp;

		//this.money = packet.Money;
		//this.respawnTime = packet.RespawnTime;
		this.blownAngleType = packet.BlowPatternId;
		//this.dizzyTime = 0f;
		this.bulletID = packet.BulletId;
		this.bulletDirection = packet.BulletDirection;
		this.position = packet.Position.ToVector3();
	}
	public HitInfo(HitRes packet)
	{
		DamageEvent damageEvent = packet.GetDamageEvent();

		this.damage = damageEvent.Damage;
		this.hitPoint = damageEvent.HitPoint;
		this.damageType = damageEvent.DamageType;
		this.statusType = damageEvent.StatusType;
		this.effectType = damageEvent.EffectType;
		this.inFieldAttackerId = damageEvent.InFieldAttackerId;
		this.killCount = 0;
        this.respawnTime = packet.RespawnTime;

        this.exp = 0;

		//this.money = 0;
		//this.respawnTime = packet.RespawnTime;
		this.blownAngleType = packet.BlowPatternId;
		//this.dizzyTime = packet.DizzyTime / 10f;
		this.bulletID = packet.BulletId;
		this.bulletDirection = packet.BulletDirection;
		this.position = packet.Position.ToVector3();
	}
	//  UNDONE: Common.DLL: 使用しなくなったパケットの関連コードをコメントアウト
	/*
	public HitInfo(StatusEvent packet)
	{
		this.statusType = packet.Status;
	}
	public HitInfo(EffectEvent packet)
	{
		this.effectType = packet.Effect;
	}
	*/
}
#endregion

#region ChatInfo
/// <summary>
/// ChatEvent
/// </summary>
[System.Serializable]
public class ChatInfo
{
	public long playerID;
	public ChatType chatType;
	public string name;
	public string text;
    public string type;

	public ChatInfo() { }
	public ChatInfo(ChatEvent packet)
	{
		this.playerID = packet.PlayerId;
		this.chatType = packet.ChatType;
		this.name = "[" + packet.Name + "]";
		this.text = packet.Text;

        switch (packet.ChatType)
        {
            case ChatType.Say:
                this.type = "[综合]";
                break;
            case ChatType.Whisper:
                this.type = "[私聊]";
                break;
        }
	}
}
#endregion

#region CharaInfo
/// <summary>
/// PlayerCharacterPacketParameter
/// </summary>
[System.Serializable]
public class CharaInfo
{
	/// <summary>
	/// デッキ内スロットインデックス
	/// 無効値 は -1
	/// </summary>
	[SerializeField]
	int _deckSlotIndex;
	public int DeckSlotIndex { get { return _deckSlotIndex; } set { _deckSlotIndex = value; } }

	/// <summary>
	/// プレイヤーキャラクターユニークID
	/// </summary>
	[SerializeField]
	ulong _uuid;
	public ulong UUID { get { return _uuid; } private set { _uuid = value; } }

	/// <summary>
	/// キャラクターマスターID
	/// </summary>
	[SerializeField]
	AvatarType _avatarType;
	public AvatarType AvatarType { get { return _avatarType; } private set { _avatarType = value; } }
	public int CharacterMasterID { get { return (int)_avatarType; } private set { _avatarType = (AvatarType)value; } }

	/// <summary>
	/// 使用回数
	/// </summary>
	[SerializeField]
	int _useCount;
	public int UseCount { get { return _useCount; } private set { _useCount = value; } }

	/// <summary>
	/// デッキコスト
	/// </summary>
	[SerializeField]
	int _deckCost;
	public int DeckCost { get { return _deckCost; } private set { _deckCost = value; } }

	/// <summary>
	/// リビルドタイム
	/// </summary>
	[SerializeField]
	float _rebuildTime;
	public float RebuildTime { get { return _rebuildTime; } private set { _rebuildTime = value; } }

	/// <summary>
	/// 残りリビルドタイム
	/// </summary>
	[SerializeField]
	float _remainingRebuildTime;
	public float RemainingRebuildTime { get { return _remainingRebuildTime; } private set { _remainingRebuildTime = value; } }

	/// <summary>
	/// キャラクターランク
	/// </summary>
	[SerializeField]
	int _rank;
	public int Rank
	{
		get { return _rank; }
		private set
		{
			if (_rank != value)
			{
				_rank = value;
				this.UpdatePowerupStatus();
			}
		}
	}

	/// <summary>
	/// 強化レベル
	/// </summary>
	[SerializeField]
	int _powerupLevel;
	public int PowerupLevel { get { return _powerupLevel; } private set { _powerupLevel = value; } }

	/// <summary>
	/// 強化経験値
	/// </summary>
	[SerializeField]
	int _powerupExp;
	public int PowerupExp { get { return _powerupExp; } private set { _powerupExp = value; } }

	/// <summary>
	/// スロットが開放されている数
	/// </summary>
	[SerializeField]
	int _powerupSlotNum;
	public int PowerupSlotNum { get { return _powerupSlotNum; } private set { _powerupSlotNum = value; } }

	/// <summary>
	/// 生命力
	/// </summary>
	[SerializeField]
	int _hitPoint;
	public int HitPoint { get { return _hitPoint; } private set { _hitPoint = value; } }

	/// <summary>
	/// 攻撃力
	/// </summary>
	[SerializeField]
	int _attack;
	public int Attack { get { return _attack; } private set { _attack = value; } }

	/// <summary>
	/// 防御力
	/// </summary>
	[SerializeField]
	int _defense;
	public int Defense { get { return _defense; } private set { _defense = value; } }

	/// <summary>
	/// 特殊能力
	/// </summary>
	[SerializeField]
	int _extra;
	public int Extra { get { return _extra; } private set { _extra = value; } }

	/// <summary>
	/// 生命力シンクロボーナス値
	/// </summary>
	[SerializeField]
	int _synchroHitPoint;
	public int SynchroHitPoint { get { return _synchroHitPoint; } private set { _synchroHitPoint = value; } }
	public static int SynchroMaxHitPoint { get { return MasterDataCommonSetting.Fusion.FusionMaxParameterSynchroBonus; } }

	/// <summary>
	/// 攻撃力シンクロボーナス値
	/// </summary>
	[SerializeField]
	int _synchroAttack;
	public int SynchroAttack { get { return _synchroAttack; } private set { _synchroAttack = value; } }
	public static int SynchroMaxAttack { get { return MasterDataCommonSetting.Fusion.FusionMaxParameterSynchroBonus; } }

	/// <summary>
	/// 防御力シンクロボーナス値
	/// </summary>
	[SerializeField]
	int _synchroDefense;
	public int SynchroDefense { get { return _synchroDefense; } private set { _synchroDefense = value; } }
	public static int SynchroMaxDefense { get { return MasterDataCommonSetting.Fusion.FusionMaxParameterSynchroBonus; } }

	/// <summary>
	/// 特殊能力シンクロボーナス値
	/// </summary>
	[SerializeField]
	int _synchroExtra;
	public int SynchroExtra { get { return _synchroExtra; } private set { _synchroExtra = value; } }
	public static int SynchroMaxExtra { get { return MasterDataCommonSetting.Fusion.FusionMaxParameterSynchroBonus; } }

	/// <summary>
	/// 生命力スロットボーナス値
	/// </summary>
	[SerializeField]
	int _slotHitPoint;
	public int SlotHitPoint { get { return _slotHitPoint; } private set { _slotHitPoint = value; } }

	/// <summary>
	/// 攻撃力スロットボーナス値
	/// </summary>
	[SerializeField]
	int _slotAttack;
	public int SlotAttack { get { return _slotAttack; } private set { _slotAttack = value; } }

	/// <summary>
	/// 防御力スロットボーナス値
	/// </summary>
	[SerializeField]
	int _slotDefense;
	public int SlotDefense { get { return _slotDefense; } private set { _slotDefense = value; } }

	/// <summary>
	/// 特殊能力スロットボーナス値
	/// </summary>
	[SerializeField]
	int _slotExtra;
	public int SlotExtra { get { return _slotExtra; } private set { _slotExtra = value; } }

	/// <summary>
	/// シンクロ合成残回数
	/// </summary>
	[SerializeField]
	int _synchroRemain;
	public int SynchroRemain { get { return _synchroRemain; } private set { _synchroRemain = value; } }

	/// <summary>
	/// シンクロ合成回数最大値
	/// </summary>
	public static int SynchroMaxCount { get { return MasterDataCommonSetting.Fusion.MaxSynchroCount; } }

	/// <summary>
	/// 何れかのデッキに入れているか？
	/// </summary>
	[SerializeField]
	bool _isInDeck;
	public bool IsInDeck { get { return _isInDeck; } private set { _isInDeck = value; } }

	/// <summary>
	/// ロックしているか？
	/// </summary>
	[SerializeField]
	bool _isLock;
	public bool IsLock { get { return _isLock; } set { _isLock = value; } }

	/// <summary>
	/// 別キャラのスロットに入ってるか？
	/// </summary>
	[SerializeField]
	bool _isInSlot;
	public bool IsInSlot { get { return _isInSlot; } private set { _isInSlot = value; } }

	/// <summary>
	/// 新しく入手したかどうか
	/// </summary>
	[SerializeField]
	bool _isNew;
	public bool IsNew { get { return _isNew; } private set { _isNew = value; } }

	/// <summary>
	/// キャラクター名
	/// </summary>
	[SerializeField]
	string _name = string.Empty;
	public string Name { get { return _name; } private set { _name = value; } }

	/// <summary>
	/// シンボルキャラかどうか
	/// </summary>
	[SerializeField]
	bool _isSymbol;
	public bool IsSymbol { get { return _isSymbol; } private set { _isSymbol = value; } }

	/// <summary>
	/// 選択できるキャラクターかどうか
	/// </summary>
	[SerializeField]
	bool _canSelect;
	public bool CanSelect { get { return _canSelect; } set { _canSelect = value; } }

	/// <summary>
	/// 強化レベルに応じた生命力
	/// </summary>
	[SerializeField]
	int _powerupHitPoint = 0;
	public int PowerupHitPoint { get { return _powerupHitPoint; } private set { _powerupHitPoint = value; } }

	/// <summary>
	/// 強化レベルに応じた攻撃力
	/// </summary>
	[SerializeField]
	int _powerupAttack = 0;
	public int PowerupAttack { get { return _powerupAttack; } private set { _powerupAttack = value; } }

	/// <summary>
	/// 強化レベルに応じた防御力
	/// </summary>
	[SerializeField]
	int _powerupDefense = 0;
	public int PowerupDefense { get { return _powerupDefense; } private set { _powerupDefense = value; } }

	/// <summary>
	/// 強化レベルに応じた特殊能力
	/// </summary>
	[SerializeField]
	int _powerupExtra = 0;
	public int PowerupExtra { get { return _powerupExtra; } private set { _powerupExtra = value; } }
    /// <summary>
    /// 经验
    /// </summary>
    [SerializeField]
    int _exp = 0;
    public int Exp { get { return _exp; } private set { _exp = value; } }
    /// <summary>
    /// 等级
    /// </summary>
    [SerializeField]
    int _level = 0;
    public int Level
    {
        get { return _level; }
        private set { _level = value; }
    }

    [SerializeField]
    int _avatarId = 0;
    /// <summary>
    /// 皮肤ID
    /// </summary>
    public int SkinId
    {
        
        get
        {
            AvatarMasterData avatar;
            MasterData.TryGetAvatar(this.CharacterMasterID, this._avatarId, out avatar);
            _avatarId = avatar != null ? avatar.ID : 0;

            return _avatarId;
        }
        set { _avatarId = value; }
    }

    [SerializeField]
    int _trySkinId = -1;
    /// <summary>
    /// 新的皮肤ID
    /// </summary>
    public int trySkinId
    {
        get { return _trySkinId; }
        set { _trySkinId = value; }
    }
   
  

    [SerializeField]
    bool _lockFlag = false;
    /// <summary>
    /// 是否锁定
    /// </summary>
    public bool LockFlag
    {
        get { return _lockFlag; }
        private set { _lockFlag = value; }
    }

    [SerializeField]
    int _starId = 0;
    public int StarId
    {
        get { return _starId; }
        private set { _starId = value; }
    }

    [SerializeField]
    int _totalTime = 0;
    public int TotalTime
    {
        get { return _totalTime; }
        private set { _totalTime = value; }
    }

    [SerializeField]
    long _remainTime = 0;
    public long RemainTime
    {
        get { return _remainTime; }
        private set { _remainTime = value; }
    }

	/// <summary>
	/// 強化レベルに応じたステータスの更新
	/// </summary>
	void UpdatePowerupStatus()
	{
		var powerupFusion = new Scm.Common.Fusion.PowerupFusion(this.CharacterMasterID, this.Rank);
		this.PowerupHitPoint = powerupFusion.GetPowerupHitPoint(this.PowerupLevel);
		this.PowerupAttack = powerupFusion.GetPowerupAttack(this.PowerupLevel);
		this.PowerupDefense = powerupFusion.GetPowerupDefense(this.PowerupLevel);
		this.PowerupExtra = powerupFusion.GetPowerupExtra(this.PowerupLevel);
	}

	/// <summary>
	/// 空かどうか
	/// </summary>
	public bool IsEmpty { get { return this.UUID == 0; } }
	/// <summary>
	/// デッキスロットが空かどうか
	/// </summary>
	public bool IsDeckSlotEmpty { get { return this.UUID == 0; } }
	/// <summary>
	/// デッキスロットがリーダーかどうか
	/// </summary>
	public bool IsDeckSlotLeader { get { return this.DeckSlotIndex == 0; } }
    
    /// <summary>
    /// コピー作成
    /// </summary>
    public CharaInfo Clone() { return (CharaInfo)MemberwiseClone(); }

	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public CharaInfo() { }

    public CharaInfo(AvatarType avatarType, int skinId)
    {
        this.AvatarType = avatarType;
        Debug.Log("AvatarType: " + this.AvatarType);
        this.SkinId = skinId;
    }

	/// <summary>
	/// スロットインデックスのみ初期化するコンストラクタ
	/// </summary>
	public CharaInfo(int deckSlotIndex) { this.DeckSlotIndex = deckSlotIndex; }
	/// <summary>
	/// アバタータイプのみ初期化するコンストラクタ
	/// </summary>
	/// <param name="avatarType"></param>
	public CharaInfo(AvatarType avatarType) { this.AvatarType = avatarType; }
	/// <summary>
	/// 進化後のキャラ情報を扱いたい場合に使用するコンストラクタ
	/// </summary>
	public CharaInfo(AvatarType avatarType, int rank, int synchroRemain, int powerupLv, bool isLock)
	{
		this.AvatarType = avatarType;
		this.SynchroRemain = synchroRemain;
		this.PowerupLevel = powerupLv;
		this.Rank = rank; // Rank の設定は強化レベルを先に設定してから
		this.IsLock = isLock;
	}
	/// <summary>
	/// 全て初期化するコンストラクタ
	/// </summary>
	public CharaInfo(int deckSlotIndex, ulong uuid, int avatarType, int useCount, int deckCost,
		float rebuildTime, float remainingRebuildTime, int rank, int powerupLevel, int powerupExp, int powerupSlotNum,
		int hitPoint, int attack, int defense, int extra,
		int synchroHitPoint, int synchroAttack, int synchroDefense, int synchroExtra, int synchroRemain,
		int slotHitPoint, int slotAttack, int slotDefense, int slotExtra,
		bool isInDeck, bool isLock, bool isInSlot, bool isNew,
		string name, bool isSymbol, bool canSelect
		)
	{
		this.DeckSlotIndex = deckSlotIndex;
		this.UUID = uuid;
		this.AvatarType = (AvatarType)avatarType;
		this.UseCount = useCount;
		this.DeckCost = deckCost;
		this.RebuildTime = rebuildTime;
		this.RemainingRebuildTime = remainingRebuildTime;
		this.PowerupLevel = powerupLevel;
		this.PowerupExp = powerupExp;
		this.PowerupSlotNum = powerupSlotNum;
		this.Rank = rank;	// Rank の設定は強化レベルを先に設定してから
		this.HitPoint = hitPoint;
		this.Attack = attack;
		this.Defense = defense;
		this.Extra = extra;
		this.SynchroHitPoint = synchroHitPoint;
		this.SynchroAttack = synchroAttack;
		this.SynchroDefense = synchroDefense;
		this.SynchroExtra = synchroExtra;
		this.SynchroRemain = synchroRemain;
		this.SlotHitPoint = slotHitPoint;
		this.SlotAttack = slotAttack;
		this.SlotDefense = slotDefense;
		this.SlotExtra = slotExtra;
		this.IsInDeck = isInDeck;
		this.IsLock = isLock;
		this.IsInSlot = isInSlot;
		this.IsNew = isNew;

		this.Name = name;
		this.IsSymbol = isSymbol;
		this.CanSelect = canSelect;
	}
	/// <summary>
	/// パケット変換のコストラクタ
	/// </summary>
	public CharaInfo(PlayerCharacterPacketParameter packet) : this(packet, 0) { }
	/// <summary>
	/// シンボルキャラ付きパケット変換のコンストラクタ
	/// </summary>
	public CharaInfo(PlayerCharacterPacketParameter packet, ulong symbolPlayerCharacterUuid)
	{
		string name = string.Empty;
		Scm.Common.Master.CharaMasterData chara;
		if (MasterData.TryGetChara(packet.CharacterMasterId, out chara))
			name = chara.Name;
		this.Convert(packet, symbolPlayerCharacterUuid, name);
	}
	/// <summary>
	/// パケット変換
	/// </summary>
	void Convert(PlayerCharacterPacketParameter packet, ulong symbolPlayerCharacterUuid, string name)
	{
		// パケットから変換
		this.DeckSlotIndex = packet.DeckSlotIndex;
		this.UUID = (ulong)packet.PlayerCharacterUuid;
		this.AvatarType = (AvatarType)packet.CharacterMasterId;
		this.UseCount = packet.UseCount;
		this.DeckCost = packet.DeckCost;
		this.RebuildTime = packet.RebuildTime * 0.1f;	// 小数点第一位までの下駄を履かせているため
		this.RemainingRebuildTime = packet.RemainingRebuildTime * 0.1f;	// 小数点第一位までの下駄を履かせているため
		this.PowerupLevel = packet.PowerupLevel;
		this.PowerupExp = packet.PowerupExp;
		this.PowerupSlotNum = packet.PowerupSlotNum;
		this.Rank = packet.Rank;	// Rank の設定は強化レベルを先に設定してから
		this.HitPoint = packet.HitPoint;
		this.Attack = packet.Attack;
		this.Defense = packet.Defense;
		this.Extra = packet.Extra;
		this.SynchroHitPoint = packet.SynchroHitPoint;
		this.SynchroAttack = packet.SynchroAttack;
		this.SynchroDefense = packet.SynchroDefense;
		this.SynchroExtra = packet.SynchroExtra;
		this.SynchroRemain = packet.SynchroRemain;
		this.SlotHitPoint = packet.SlotHitPoint;
		this.SlotAttack = packet.SlotAttack;
		this.SlotDefense = packet.SlotDefense;
		this.SlotExtra = packet.SlotExtra;
		this.IsInDeck = packet.DeckFlag;
		this.IsLock = packet.LockFlag;
		this.IsInSlot = packet.SlotFlag;
		this.IsNew = packet.NewFlag;
        this.Level = packet.Level;
        this.Exp = packet.Exp;
        this.SkinId = packet.AvatarId;
        this.LockFlag = packet.LockFlag;
        this.StarId = packet.StarId;
        this.TotalTime = packet.TotalTime;
        this.RemainTime = packet.RemainTime;
        
		// 付加情報
		this.Name = name;
		this.IsSymbol = this.UUID == symbolPlayerCharacterUuid;
		this.CanSelect = true;
	}

	/// <summary>
	/// PowerupSlotCharacterParameterパケット変換のコストラクタ
	/// </summary>
	public CharaInfo(PowerupSlotCharacterParameter packet)
	{
		string name = string.Empty;
		Scm.Common.Master.CharaMasterData chara;
		if (MasterData.TryGetChara(packet.CharacterMasterId, out chara))
			name = chara.Name;
		Convert(packet, name);
	}
	/// <summary>
	/// PowerupSlotCharacterParameterパケット変換
	/// </summary>
	void Convert(PowerupSlotCharacterParameter packet, string name)
	{
		// パケットから変換
		this.DeckSlotIndex = packet.SlotIndex;
		this.UUID = (ulong)packet.PlayerCharacterUuid;
		this.AvatarType = (AvatarType)packet.CharacterMasterId;
		this.PowerupLevel = packet.PowerupLevel;
		this.Rank = packet.Rank;	// Rank の設定は強化レベルを先に設定してから
		this.IsLock = packet.LockFlag;

		// 付加情報
		this.Name = name;
		this.CanSelect = true;
	}

	#region 比較
	/// <summary>
	/// ランク比較
	/// </summary>
	public static int RankCompare(CharaInfo infoX, CharaInfo infoY, bool isAscend)
	{
		if (isAscend) { return infoX.Rank.CompareTo(infoY.Rank); } else { return infoY.Rank.CompareTo(infoX.Rank); }
	}
	/// <summary>
	/// デッキコスト比較
	/// </summary>
	public static int CostCompare(CharaInfo infoX, CharaInfo infoY, bool isAscend)
	{
		if (isAscend) { return infoX.DeckCost.CompareTo(infoY.DeckCost); } else { return infoY.DeckCost.CompareTo(infoX.DeckCost); }
	}
	/// <summary>
	/// レベル比較
	/// </summary>
	public static int LevelCompare(CharaInfo infoX, CharaInfo infoY, bool isAscend)
	{
		if (isAscend) { return infoX.PowerupLevel.CompareTo(infoY.PowerupLevel); } else { return infoY.PowerupLevel.CompareTo(infoX.PowerupLevel); }
	}
	/// <summary>
	/// 種類比較
	/// </summary>
	public static int TypeCompare(CharaInfo infoX, CharaInfo infoY, bool isAscend)
	{
		if (isAscend) { return infoX.AvatarType.CompareTo(infoY.AvatarType); } else { return infoY.AvatarType.CompareTo(infoX.AvatarType); }
	}
	/// <summary>
	/// 体力比較
	/// </summary>
	public static int HitPointCompare(CharaInfo infoX, CharaInfo infoY, bool isAscend)
	{
		if (isAscend) { return infoX.HitPoint.CompareTo(infoY.HitPoint); } else { return infoY.HitPoint.CompareTo(infoX.HitPoint); }
	}
	/// <summary>
	/// 攻撃力比較
	/// </summary>
	public static int AttackCompare(CharaInfo infoX, CharaInfo infoY, bool isAscend)
	{
		if (isAscend) { return infoX.Attack.CompareTo(infoY.Attack); } else { return infoY.Attack.CompareTo(infoX.Attack); }
	}
	/// <summary>
	/// 防御力比較
	/// </summary>
	public static int DefenseCompare(CharaInfo infoX, CharaInfo infoY, bool isAscend)
	{
		if (isAscend) { return infoX.Defense.CompareTo(infoY.Defense); } else { return infoY.Defense.CompareTo(infoX.Defense); }
	}
	/// <summary>
	/// 特殊能力比較
	/// </summary>
	public static int ExtraCompare(CharaInfo infoX, CharaInfo infoY, bool isAscend)
	{
		if (isAscend) { return infoX.Extra.CompareTo(infoY.Extra); } else { return infoY.Extra.CompareTo(infoX.Extra); }
	}
	/// <summary>
	/// UUID比較
	/// </summary>
	public static int UUIDCompare(CharaInfo infoX, CharaInfo infoY, bool isAscend)
	{
		if (isAscend) { return infoX.UUID.CompareTo(infoY.UUID); } else { return infoY.UUID.CompareTo(infoX.UUID); }
	}
	/// <summary>
	/// シンクロボーナス合計値の比較
	/// </summary>
	public static int TotalSynchroBonusCompare(CharaInfo infoX, CharaInfo infoY, bool isAscend)
	{
		int bonusX = CharaInfo.GetTotalSynchroBonus(infoX);
		int bonusY = CharaInfo.GetTotalSynchroBonus(infoY);

		if (isAscend) { return bonusX.CompareTo(bonusY); } else { return bonusY.CompareTo(bonusX); }
	}
	/// <summary>
	/// スロット解放数の比較
	/// </summary>
	public static int PowerupSlotNumCompare(CharaInfo infoX, CharaInfo infoY, bool isAscend)
	{
		if (isAscend) { return infoX.PowerupSlotNum.CompareTo(infoY.PowerupSlotNum); } else { return infoY.PowerupSlotNum.CompareTo(infoX.PowerupSlotNum); }
	}
	#endregion

#if XW_DEBUG
	public void DebugSetUUID(ulong uuid) { this.UUID = uuid; }
	public void DebugSetAvatarType(int avatarType) { this.AvatarType = (AvatarType)avatarType; }
	public void DebugSetRank(int rank) { this.Rank = rank; }
	public void DebugSetName(string name) { this.Name = name; }
	public void DebugSetSynchroParam(int hitpoint, int attack, int defense, int extra) { this.SynchroHitPoint = hitpoint; this.SynchroAttack = attack; this.SynchroDefense = defense; this.SynchroExtra = extra; }
	public void DebugRandomSetup()
	{
		this.DeckSlotIndex = 0;
		this.UUID = (ulong)(DateTime.Now.Ticks * 0.0000001);
		this.AvatarType = (AvatarType)UnityEngine.Random.Range((int)AvatarType.Begin, (int)(AvatarType.End + 1));
		this.UseCount = UnityEngine.Random.Range(0, 1000);
		this.DeckCost = UnityEngine.Random.Range(0, 1000);
		this.RebuildTime = UnityEngine.Random.Range(0, 1000);
		this.RemainingRebuildTime = UnityEngine.Random.Range(0, this.RebuildTime);
		var rank = UnityEngine.Random.Range(1, 6);
		var powerupLevelMax = CharaInfo.GetMaxLevel(rank);
		this.PowerupLevel = UnityEngine.Random.Range(1, powerupLevelMax);
		this.PowerupExp = 0;
		{
			var totalExp = CharaInfo.GetTotalExp(rank, this.PowerupLevel);
			var nextLvTotalExp = CharaInfo.GetTotalExp(rank, this.PowerupLevel + 1);
			if (nextLvTotalExp == 0) nextLvTotalExp = totalExp;
			this.PowerupExp = UnityEngine.Random.Range(totalExp, nextLvTotalExp + 1);
		}
		this.Rank = rank;	// Rank の設定は強化レベルを先に設定してから
		this.HitPoint = UnityEngine.Random.Range(0, 10000);
		this.Attack = UnityEngine.Random.Range(0, 10000);
		this.Defense = UnityEngine.Random.Range(0, 10000);
		this.Extra = UnityEngine.Random.Range(0, 10000);
		this.SynchroHitPoint = UnityEngine.Random.Range(0, CharaInfo.SynchroMaxHitPoint + 1);
		this.SynchroAttack = UnityEngine.Random.Range(0, CharaInfo.SynchroMaxAttack + 1);
		this.SynchroDefense = UnityEngine.Random.Range(0, CharaInfo.SynchroMaxDefense + 1);
		this.SynchroExtra = UnityEngine.Random.Range(0, CharaInfo.SynchroMaxExtra + 1);
		this.SlotHitPoint = UnityEngine.Random.Range(0, 10000);
		this.SlotAttack = UnityEngine.Random.Range(0, 10000);
		this.SlotDefense = UnityEngine.Random.Range(0, 10000);
		this.SlotExtra = UnityEngine.Random.Range(0, 10000);
		this.SynchroRemain = UnityEngine.Random.Range(0, CharaInfo.SynchroMaxCount + 1);
		this.PowerupSlotNum = UnityEngine.Random.Range(0, 21);
		var isDisable = UnityEngine.Random.Range(0, 2) == 1;
		this.IsInSlot = isDisable ? UnityEngine.Random.Range(0, 2) == 1 : false;
		if (this.IsInSlot) isDisable = false;	// スロット設定のキャラはデッキ、ロック、シンボルにはできない
		this.IsInDeck = isDisable ? UnityEngine.Random.Range(0, 2) == 1 : false;
		this.IsLock = isDisable ? UnityEngine.Random.Range(0, 2) == 1 : false;
		this.IsNew = isDisable ? UnityEngine.Random.Range(0, 2) == 1 : false;

		// 付加情報
		this.Name = "";
		this.IsSymbol = isDisable ? UnityEngine.Random.Range(0, 2) == 1 : false;
		this.CanSelect = true;
	}
#endif

	/// <summary>
	/// 同じ値かどうか評価する
	/// </summary>
	public bool Equals(CharaInfo t)
	{
		if (this.DeckSlotIndex == t.DeckSlotIndex
			&& this.UUID == t.UUID
			&& this.AvatarType == t.AvatarType
			&& this.UseCount == t.UseCount
			&& this.DeckCost == t.DeckCost
			&& this.RebuildTime == t.RebuildTime
			&& this.RemainingRebuildTime == t.RemainingRebuildTime
			&& this.Rank == t.Rank
			&& this.PowerupLevel == t.PowerupLevel
			&& this.PowerupExp == t.PowerupExp
			&& this.PowerupSlotNum == t.PowerupSlotNum
			&& this.HitPoint == t.HitPoint
			&& this.Attack == t.Attack
			&& this.Defense == t.Defense
			&& this.Extra == t.Extra
			&& this.SynchroHitPoint == t.SynchroHitPoint
			&& this.SynchroAttack == t.SynchroAttack
			&& this.SynchroDefense == t.SynchroDefense
			&& this.SynchroExtra == t.SynchroExtra
			&& this.SynchroRemain == t.SynchroRemain
			&& this.SlotHitPoint == t.SlotHitPoint
			&& this.SlotAttack == t.SlotAttack
			&& this.SlotDefense == t.SlotDefense
			&& this.SlotExtra == t.SlotExtra
			&& this.SynchroRemain == t.SynchroRemain
			&& this.IsInDeck == t.IsInDeck
			&& this.IsLock == t.IsLock
			&& this.IsInSlot == t.IsInSlot
			&& this.IsNew == t.IsNew
			&& this.Name.Equals(t.Name)
			&& this.IsSymbol == t.IsSymbol
			&& this.CanSelect == t.CanSelect
			)
		{
			return false;
		}
		return true;
	}

	/// <summary>
	/// 指定したレベルになる為の累積経験値を取得する
	/// </summary>
	public static int GetTotalExp(int rank, int powerupLevel)
	{
		int exp = 0;
		Scm.Common.XwMaster.CharaPowerupLevelMasterData data;
		if (MasterData.TryGetCharaPowerupLevel(rank, powerupLevel, out data))
		{
			exp = data.Exp;
		}
		return exp;
	}

	/// <summary>
	/// 最大レベルを取得する
	/// </summary>
	public static int GetMaxLevel(int rank)
	{
		int maxLevel = 0;
		Scm.Common.XwMaster.CharaPowerupLevelMasterData data;
		if (MasterData.TryGetCharaMaxPowerupLevel(rank, out data))
		{
			maxLevel = data.PowerupLevel;
		}
		return maxLevel;
	}

	/// <summary>
	/// 最大レベルに達しているかどうか
	/// </summary>
	public static bool IsMaxLevel(int rank, int powerupLevel)
	{
		if (GetMaxLevel(rank) <= powerupLevel)
		{
			// 最大レベルに達している
			return true;
		}
		return false;
	}

	/// <summary>
	/// 各パラメータのシンクロボーナス最大値の合計値を取得する
	/// </summary>
	public static int GetTotalMaxSynchroBonus()
	{
		return SynchroMaxHitPoint + SynchroMaxAttack + SynchroMaxDefense + SynchroMaxExtra;
	}

	/// <summary>
	/// 現在の各パラメータのシンクロボーナスの合計値を取得する
	/// </summary>
	public static int GetTotalSynchroBonus(CharaInfo info)
	{
		int total = 0;
		if (info != null)
		{
			total = info.SynchroHitPoint + info.SynchroAttack + info.SynchroDefense + info.SynchroExtra;
		}
		return total;
	}
}
#endregion

#region TaskDaily
public class TaskDaily
{
    public enum DailyRewardType
    {
        Gold,
        Coin,
        Character,
        Energy
    }

    public enum TaskDailyStatus
    {
        Active,
        Completed,
        Comfirmed
    }

    public int Id { get; private set; }

    public string TaskType
    {
        get
        {
            string[] replace = { this.ProcessTotal.ToString() };
            return MasterData.GetText(TextCategory.QuestDescription, (int)this.DailyMasterData.QuestType, replace);
        }
    }

    private DailyQuestMasterData dailyMasterData;
    public DailyQuestMasterData DailyMasterData
    {
        get { return this.dailyMasterData; }
        private set
        {
            this.dailyMasterData = value;
            switch (this.dailyMasterData.RewardType)
            {
                case Scm.Common.GameParameter.RewardType.Gold:
                    this.RewardType = DailyRewardType.Gold;
                    break;
                case Scm.Common.GameParameter.RewardType.Coin:
                    this.RewardType = DailyRewardType.Coin;
                    break;
                case Scm.Common.GameParameter.RewardType.Character:
                    this.RewardType = DailyRewardType.Character;
                    break;
                case Scm.Common.GameParameter.RewardType.Energy:
                    this.RewardType = DailyRewardType.Energy;
                    break;
            }
            this.RewardCount = this.dailyMasterData.RewardCount;
        }
    }

    private int questId;
    public int QuestId
    {
        get
        {
            return this.questId;
        }
        set
        {
            this.questId = value;
            DailyQuestMasterData data;
            MasterData.TryGetDailyQuestMasterData(this.questId, out data);
            this.DailyMasterData = data;
        }
    }
    
    public DailyRewardType RewardType { get; private set; }
    public int RewardCount { get; private set; }

    public int Process { get; private set; }
    public int ProcessTotal { get; private set; }

    private TaskDailyStatus status;
    public TaskDailyStatus Status
    {
        get
        {
            return this.status;
        }
        set
        {
            this.status = value;
        }
    }

    public TaskDaily(QuestParameter info)
    {
        this.Id = info.Id;
        this.QuestId = info.QuestId;

        this.Process = info.Progress;
        this.ProcessTotal = info.Total;

        this.SetStatus(info.QuestStatus);
    }

    private void SetStatus(Scm.Common.GameParameter.QuestStatus s)
    {
        switch (s)
        {
            case Scm.Common.GameParameter.QuestStatus.Active:
                this.Status = TaskDailyStatus.Active;
                break;
            case Scm.Common.GameParameter.QuestStatus.Completed:
                this.Status = TaskDailyStatus.Completed;
                break;
            case Scm.Common.GameParameter.QuestStatus.Confirmed:
                this.Status = TaskDailyStatus.Comfirmed;
                break;
        }
    }
}
#endregion

#region PowerupSlotCharaInfo
/// <summary>
/// PowerupSlotCharacterParameter
/// </summary>
[System.Serializable]
public class PowerupSlotCharaInfo
{
	/// <summary>
	/// スロットインデックス
	/// 無効値 は -1
	/// </summary>
	[SerializeField]
	int _slotIndex = -1;
	public int SlotIndex { get { return _slotIndex; } set { _slotIndex = value; } }

	/// <summary>
	/// プレイヤーキャラクターユニークID
	/// </summary>
	[SerializeField]
	ulong _uuid;
	public ulong UUID { get { return _uuid; } private set { _uuid = value; } }

	/// <summary>
	/// キャラクターマスターID
	/// </summary>
	[SerializeField]
	AvatarType _avatarType;
	public AvatarType AvatarType { get { return _avatarType; } private set { _avatarType = value; } }
	public int CharacterMasterID { get { return (int)_avatarType; } private set { _avatarType = (AvatarType)value; } }

	/// <summary>
	/// キャラクターランク
	/// </summary>
	[SerializeField]
	int _rank;
	public int Rank { get { return _rank; } private set { _rank = value; } }

	/// <summary>
	/// 強化レベル
	/// </summary>
	[SerializeField]
	int _powerupLevel;
	public int PowerupLevel { get { return _powerupLevel; } private set { _powerupLevel = value; } }

	/// <summary>
	/// ロックフラグ
	/// </summary>
	[SerializeField]
	bool _isLock;
	public bool IsLock { get { return _isLock; } private set { _isLock = value; } }

	/// <summary>
	/// 空かどうか
	/// </summary>
	public bool IsEmpty { get { return this.UUID == 0; } }

	/// <summary>
	/// コピー作成
	/// </summary>
	public PowerupSlotCharaInfo Clone() { return (PowerupSlotCharaInfo)MemberwiseClone(); }

	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public PowerupSlotCharaInfo() { }
	/// <summary>
	/// スロットインデックスのみ初期化するコンストラクタ
	/// </summary>
	public PowerupSlotCharaInfo(int slotIndex) { this.SlotIndex = slotIndex; }
	/// <summary>
	/// 全て初期化するコンストラクタ
	/// </summary>
	public PowerupSlotCharaInfo(int slotIndex, ulong uuid, int avatarType)
	{
		this.SlotIndex = slotIndex;
		this.UUID = uuid;
		this.AvatarType = (AvatarType)avatarType;
	}
	/// <summary>
	/// パケット変換のコストラクタ
	/// </summary>
	public PowerupSlotCharaInfo(PowerupSlotCharacterParameter packet)
	{
		this.Convert(packet);
	}
	/// <summary>
	/// パケット変換
	/// </summary>
	void Convert(PowerupSlotCharacterParameter packet)
	{
		// パケットから変換
		this.SlotIndex = packet.SlotIndex;
		this.UUID = (ulong)packet.PlayerCharacterUuid;
		this.AvatarType = (AvatarType)packet.CharacterMasterId;
		this.Rank = packet.Rank;
		this.PowerupLevel = packet.PowerupLevel;
		this.IsLock = packet.LockFlag;
	}
#if XW_DEBUG
	public void DebugSetUUID(ulong uuid) { this.UUID = uuid; }
	public void DebugSetAvatarType(int avatarType) { this.AvatarType = (AvatarType)avatarType; }
	public void DebugRandomSetup()
	{
		this.SlotIndex = 0;
		this.UUID = (ulong)(DateTime.Now.Ticks * 0.0000001);
		this.AvatarType = (AvatarType)UnityEngine.Random.Range((int)AvatarType.Begin, (int)(AvatarType.End + 1));
	}
#endif
}
#endregion

#region PlayerStatusInfo
/// <summary>
/// PlayerStatusPacketParameter
/// </summary>
[System.Serializable]
public class PlayerStatusInfo
{
	[SerializeField]
	int _inFieldID;
	public int InFieldID { get { return _inFieldID; } private set { _inFieldID = value; } }

	[SerializeField]
	string _name;
	public string Name { get { return _name; } private set { _name = value; } }

	[SerializeField]
	AvatarType _avatarType;
	public AvatarType AvatarType { get { return _avatarType; } private set { _avatarType = value; } }

    [SerializeField]
    int _skinId;
    public int SkinId { get { return _skinId; } private set { _skinId = value; } }

	[SerializeField]
	string _guildName;
	public string GuildName { get { return _guildName; } private set { _guildName = value; } }

	[SerializeField]
	int _playerGradeID;
	public int PlayerGradeID { get { return _playerGradeID; } private set { _playerGradeID = value; } }

	[SerializeField]
	int _gradePoint;
	public int GradePoint { get { return _gradePoint; } private set { _gradePoint = value; } }

	[SerializeField]
	int _battleCount;
	public int BattleCount { get { return _battleCount; } private set { _battleCount = value; } }

	[SerializeField]
	int _winCount;
	public int WinCount { get { return _winCount; } private set { _winCount = value; } }

	[SerializeField]
	int _loseCount;
	public int LoseCount { get { return _loseCount; } private set { _loseCount = value; } }

	[SerializeField]
	int _drawCount;
	public int DrawCount { get { return _drawCount; } private set { _drawCount = value; } }

	[SerializeField]
	int _killCount;
	public int KillCount { get { return _killCount; } private set { _killCount = value; } }

	[SerializeField]
	int _towerDefeatCount;
	public int TowerDefeatCount { get { return _towerDefeatCount; } private set { _towerDefeatCount = value; } }

	[SerializeField]
	string _profile;
	public string Profile { get { return _profile; } private set { _profile = value; } }

	[SerializeField]
	int _accountMoney;
	public int AccountMoney { get { return _accountMoney; } private set { _accountMoney = value; } }

	[SerializeField]
	int _gameMoney;
	public int GameMoney { get { return _gameMoney; } private set { _gameMoney = value; } }

	[SerializeField]
	int _aliasWord1stId;
	public int AliasWord1stId { get { return _aliasWord1stId; } private set { _aliasWord1stId = value; } }

	[SerializeField]
	int _aliasWord2ndId;
	public int AliasWord2ndId { get { return _aliasWord2ndId; } private set { _aliasWord2ndId = value; } }

	[SerializeField]
	int _aliasParticleId;
	public int AliasParticleId { get { return _aliasParticleId; } private set { _aliasParticleId = value; } }

	[SerializeField]
	int _affiliationForceId;
	public int AffiliationForceId { get { return _affiliationForceId; } private set { _affiliationForceId = value; } }

	[SerializeField]
	int _lobbyId;
	public int LobbyId { get { return _lobbyId; } private set { _lobbyId = value; } }

	[SerializeField]
	bool _online;
	public bool Online { get { return _online; } private set { _online = value; } }

    [SerializeField]
    int _level;
    public int Level { get { return _level; } private set { _level = value; } }

    [SerializeField]
    int _exp;
    public int Exp { get { return _exp; } private set { _exp = value; } }

    [SerializeField]
    int _nextExp;
    public int NextExp { get { return _nextExp; } private set { _nextExp = value; } }

    [SerializeField]
    int _mvpCount;
    public int MvpCout { get { return _mvpCount; } private set { _mvpCount = value; } }

    [SerializeField]
    int _rank;
    public int Rank { get { return _rank; } private set { _rank = value; } }

    [SerializeField]
    int _maxRank;
    public int MaxRank { get { return _maxRank; } private set { _maxRank = value; } }

    [SerializeField]
    int _rankExp;
    public int RankExp { get { return _maxRank; } private set { _rankExp = value; } }

    [SerializeField]
    int _nextRankExp;
    public int NextRankExp { get { return _nextRankExp; } private set { _nextRankExp = value; } }


	public PlayerStatusInfo() { }
	public PlayerStatusInfo(PlayerStatusPacketParameter packet)
	{
		this.InFieldID = packet.InFieldId;
		this.Name = packet.Name;
		this.AvatarType = (AvatarType)packet.CharacterMasterId;
		this.GuildName = packet.GuildName;
		this.PlayerGradeID = packet.PlayerGradeId;
		this.GradePoint = packet.GradePoint;
		this.BattleCount = packet.BattleCount;
		this.WinCount = packet.WinCount;
		this.LoseCount = packet.LoseCount;
		this.DrawCount = packet.DrawCount;
		this.KillCount = packet.KillCount;
		this.TowerDefeatCount = packet.TowerDefeatCount;
		this.Profile = packet.Profile;
		this.AccountMoney = packet.AccountMoney;
		this.GameMoney = packet.GameMoney;
		this.AliasWord1stId = packet.AliasWord1stId;
		this.AliasWord2ndId = packet.AliasWord2ndId;
		this.AliasParticleId = packet.AliasParticleId;
		this.AffiliationForceId = packet.AffiliationForceId;
		this.LobbyId = packet.LobbyId;
		this.Online = packet.Online;

        this.Level = packet.Level;
        this.Exp = packet.Exp;
        this.NextExp = packet.NextExp;
        this.MvpCout = packet.MVPCount;
        this.Rank = packet.Rank;
        this.MaxRank = packet.MaxRank;
        this.RankExp = packet.RankExp;
        this.NextRankExp = packet.NextRankExp;
	}
	public void SetGameMoney(int money) { this.GameMoney = money; }
}
#endregion

#region BuffInfo
[System.Serializable]
public class BuffInfo
{
	public BuffType buffType;
	public byte level;
	public short remainingTime;
	public bool isNew;

	public BuffInfo(BuffPacketParameter parameter)
	{
		this.buffType = parameter.BuffType;
		this.level = parameter.BuffLevel;
		this.remainingTime = parameter.RemainingTime;
		this.isNew = false;
	}

	/// <summary>
	/// バフがかけられた順(SPバフも含む)に並び替えて取得する
	/// </summary>
	public static LinkedList<BuffInfo> GetBuffInfoList(BuffType[] newBuffTypes, BuffPacketParameter[] buffParams, BuffPacketParameter[] spBuffParams)
	{
		// バフ検索用に使用する
		Dictionary<BuffType, BuffInfo> buffDictionary = new Dictionary<BuffType, BuffInfo>();
		// 追加順にしたバフ情報の格納用
		LinkedList<BuffInfo> buffInfoList = new LinkedList<BuffInfo>();

		// バフとSPバフが別々になっているのでまとめて格納する
		AddBuff(buffParams, ref buffInfoList, ref buffDictionary);
		AddBuff(spBuffParams, ref buffInfoList, ref buffDictionary);

		// 現在かけられているバフと新しくかけられた(上書き分も含む)バフを分離
		LinkedList<BuffInfo> newBuffList = new LinkedList<BuffInfo>();
		foreach (BuffType buffType in newBuffTypes)
		{
			BuffInfo buffInfo;
			if (buffDictionary.TryGetValue(buffType, out buffInfo))
			{
				buffInfo.isNew = true;
				buffInfoList.Remove(buffInfo);
				newBuffList.AddLast(buffInfo);
			}
		}

		// 現在かかっているバフの後ろに新しくかかったバフを追加
		foreach (BuffInfo info in newBuffList)
		{
			buffInfoList.AddLast(info);
		}

		return buffInfoList;
	}

	/// <summary>
	/// バフを検索用のDictionaryとリストに追加する処理
	/// </summary>
	private static void AddBuff(BuffPacketParameter[] buffParams, ref LinkedList<BuffInfo> buffLinkeList, ref Dictionary<BuffType, BuffInfo> buffList)
	{
		foreach (BuffPacketParameter param in buffParams)
		{
			BuffInfo info = new BuffInfo(param);
			buffLinkeList.AddLast(info);
			buffList.Add(info.buffType, info);
		}
	}
}
#endregion

#region TargetMarkerInfo
[System.Serializable]
public class TargetMarkerInfo
{
	public int TacticalID { get; set; }
	public int TargetID { get; set; }
	public TargetMarkerActionType TargetmarkerAction { get; set; }
	//public ObjectBase targetObject;

	public TargetMarkerInfo()
	{
		TacticalID = 0;
		TargetID = 0;
		TargetmarkerAction = TargetMarkerActionType.None;
	}
	public TargetMarkerInfo(TargetMarkerEvent packet)
	{
		this.TacticalID = packet.TacticalId;
		this.TargetID = packet.InFieldTargetId;
		this.TargetmarkerAction = packet.TargetMarkerActionType;
	}
}
#endregion

#region DeckInfo
/// <summary>
/// デッキ情報
/// CharacterDeckRes, CharacterDeckEvent
/// </summary>
[System.Serializable]
public class DeckInfo
{
	[SerializeField]
	int _deckID;
	public int DeckID { get { return _deckID; } private set { _deckID = value; } }

	[SerializeField]
	string _deckName;
	public string DeckName { get { return _deckName; } private set { _deckName = value; } }

	[SerializeField]
	int _deckCapacity;
	public int DeckCapacity { get { return _deckCapacity; } private set { _deckCapacity = value; } }

	[SerializeField]
	int _currentSlotIndex = -1;
	public int CurrentSlotIndex { get { return _currentSlotIndex; } private set { _currentSlotIndex = value; } }

	[SerializeField]
	List<CharaInfo> _charaInfoList;
	public List<CharaInfo> CharaInfoList { get { return _charaInfoList; } private set { _charaInfoList = value; } }

	// 合計コスト算出
	public int DeckTotalCost
	{
		get
		{
			int totalDeckCost = 0;
			if (CharaInfoList != null)
			{
				foreach (var info in CharaInfoList)
				{
					if (info.IsDeckSlotEmpty) continue;
					totalDeckCost += info.DeckCost;
				}
			}
			return totalDeckCost;
		}
	}

	public DeckInfo()
	{
		this.CharaInfoList = new List<CharaInfo>()
		{
			// スロットは4つ必ずある
			new CharaInfo(0),
			new CharaInfo(1),
			new CharaInfo(2),
			new CharaInfo(3)
		};
		this.CurrentSlotIndex = -1;
	}
	public DeckInfo(CharacterDeckRes packet)
	{
		this.DeckID = packet.DeckId;
		this.DeckName = packet.Name;
		this.DeckCapacity = packet.Capacity;
		this.CharaInfoList = new List<CharaInfo>();
		foreach (var t in packet.GetOwnCharacterPacketParameterArray())
			this.CharaInfoList.Add(new CharaInfo(t));
		this.CurrentSlotIndex = packet.CurrentSlotIndex;
	}

    public void ReOrderCharaInfoList(List<CharaInfo> pCharaInfos)
    {
        this.CharaInfoList = pCharaInfos;
    }
	//  UNDONE: Common.DLL: 使用しなくなったパケットの関連コードをコメントアウト
	/*
	public DeckInfo(CharacterDeckEvent packet)
	{
		this.DeckID = packet.DeckId;
		this.DeckName = packet.Name;
		this.DeckCapacity = packet.Capacity;
		this.CharaInfoList = new List<CharaInfo>();
		foreach (var t in packet.GetOwnCharacterPacketParameterArray())
			this.CharaInfoList.Add(new CharaInfo(t));
		this.CurrentSlotIndex = packet.CurrentSlotIndex;
	}
	*/
}
#endregion

#region RetainPlayerInfo
/// <summary>
/// 保有プレイヤー情報
/// </summary>
[System.Serializable]
public class RetainPlayerInfo
{
	public int PlayerID { get; set; }
	public string Name { get; set; }
	public int PlayerGradeID { get; set; }
	public int GradePoint { get; set; }
	public int GameCoin { get; set; }
	public int StraightWins { get; set; }
	public long PlayTime { get; set; }
	public int CharacterID { get; set; }

	public RetainPlayerInfo(PlayerPacketParameter packet)
	{
		//  UNDONE: Common.DLL: PlayerIDの型をlongに変更すると影響範囲が広いのでここでキャストしておく
		this.PlayerID = (int)packet.PlayerId;
		this.Name = packet.Name;
		this.PlayerGradeID = 0;
		this.GradePoint = 0;
		this.GameCoin = 0;
		this.StraightWins = 0;
		this.PlayTime = packet.PlayTime;
		this.CharacterID = packet.CharacterMasterId;
	}
}
#endregion

#region LobbyInfo
/// <summary>
/// LobbyListPacketParameter
/// </summary>
[System.Serializable]
public class LobbyInfo
{
	[SerializeField]
	int _lobbyID;
	public int LobbyID { get { return _lobbyID; } private set { _lobbyID = value; } }

	[SerializeField]
	int _capacity;
	public int Capacity { get { return _capacity; } private set { _capacity = value; } }

	[SerializeField]
	int _num;
	public int Num { get { return _num; } private set { _num = value; } }

	public LobbyInfo() { }
	public LobbyInfo(int lobbyID, int capacity, int num)
	{
		this.LobbyID = lobbyID;
		this.Capacity = capacity;
		this.Num = num;
	}
	public LobbyInfo(LobbyListPacketParameter packet)
	{
		this.LobbyID = packet.LobbyId;
		this.Capacity = packet.Capacity;
		this.Num = packet.Num;
	}
}
#endregion

#region ランキング
#region RankingInfo
[System.Serializable]
public class RankingInfo
{
	[SerializeField]
	int _rankingId;
	public int RankingId { get { return this._rankingId; } set { this._rankingId = value; } }

	[SerializeField]
	RankingPeriodType _periodType;
	public RankingPeriodType PeriodType { get { return this._periodType; } private set { this._periodType = value; } }

	[SerializeField]
	int _num;
	public int Num { get { return this._num; } private set { this._num = value; } }

	
	[SerializeField]
	int _topScrore;
	public int TopScore { get { return this._topScrore; } set { this._topScrore = value; } }

	[SerializeField]
	long _startDate;
	public long StartDate { get { return this._startDate; } set { this._startDate = value; } }

	[SerializeField]
	long _endDate;
	public long EndDate { get { return this._endDate; } set { this._endDate = value; } }
    
}
#endregion


#endregion

#region ItemInfo
/// <summary>
/// PlayerItemPacketParameter
/// </summary>
[System.Serializable]
public class ItemInfo
{
	/// <summary>
	/// アイテムのインデックス
	/// </summary>
	[SerializeField]
	int _index = -1;
	public int Index { get { return _index; } set { _index = value; } }

	/// <summary>
	/// アイテムマスタID
	/// </summary>
	[SerializeField]
	int _itemMasterID = 0;
	public int ItemMasterID { get { return _itemMasterID; } set { _itemMasterID = value; } }

	/// <summary>
	/// スタック数
	/// </summary>
	[SerializeField]
	int _stack = 0;
	public int Stack { get { return _stack; } set { _stack = value; } }

	/// <summary>
	/// ロックしているか
	/// </summary>
	[SerializeField]
	bool _isLock = false;
	public bool IsLock { get { return _isLock; } set { _isLock = value; } }

	/// <summary>
	/// Newフラグ
	/// </summary>
	[SerializeField]
	bool _newFlag = false;
	public bool NewFlag { get { return _newFlag; } set { _newFlag = value; } }

	/// <summary>
	/// アイテム名
	/// </summary>
	[SerializeField]
	string _name;
	public string Name { get { return _name; } set { _name = value; } }

	/// <summary>
	/// 空かどうか
	/// </summary>
	public bool IsEmpty { get { return this.Index == -1; } }

	/// <summary>
	/// コピー作成
	/// </summary>
	public ItemInfo Clone() { return (ItemInfo)MemberwiseClone(); }

	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public ItemInfo() { }

	/// <summary>
	/// 全てを初期化するコンストラクタ
	/// </summary>
	public ItemInfo(int index, int itemMasterID, int stack, bool lockFlag)
	{
		this.Index = index;
		this.ItemMasterID = itemMasterID;
		this.Stack = stack;
		this.IsLock = lockFlag;
	}
	/// <summary>
	/// パケット変換のコンストラクタ
	/// </summary>
	public ItemInfo(PlayerItemPacketParameter packet)
	{
		string name = string.Empty;
		Scm.Common.XwMaster.ItemMasterData master;
		if (MasterData.TryGetItem(packet.ItemMasterId, out master))
			name = master.Name;
		this.Convert(packet, name);
	}

	/// <summary>
	/// メールの添付アイテム用
	/// </summary>
	/// <param name="packet"></param>
	public ItemInfo(MailParameter packet)
	{
		string name = string.Empty;
		Scm.Common.XwMaster.ItemMasterData master;
		if (MasterData.TryGetItem(packet.ItemMasterId, out master))
		{
			name = master.Name;
		}

		this.ItemMasterID = packet.ItemMasterId;
		this.Stack = packet.Stack;
		this.Name = name;
	}

	/// <summary>
	/// パケット変換
	/// </summary>
	void Convert(PlayerItemPacketParameter packet, string name)
	{
		// パケットから変換
		this.Index = packet.Index;
		this.ItemMasterID = packet.ItemMasterId;
		this.Stack = packet.Stack;
		this.IsLock = packet.LockFlag;
		this.NewFlag = packet.NewFlag;

		// 付加情報
		this.Name = name;
	}

	/// <summary>
	/// 同じ値かどうか評価する
	/// </summary>
	public bool Equals(ItemInfo t)
	{
		if (this.Index == t.Index
			&& this.ItemMasterID == t.ItemMasterID
			&& this.Stack == t.Stack
			&& this.IsLock == t.IsLock
			&& this.NewFlag == t.NewFlag)
		{
			return false;
		}

		return true;
	}

	#region 比較
	/// <summary>
	/// 名前比較
	/// </summary>
	public static int NameCompare(ItemInfo infoX, ItemInfo infoY, bool isAscend)
	{
		if (isAscend) { return infoX.Name.CompareTo(infoY.Name); } else { return infoY.Name.CompareTo(infoX.Name); }
	}
	/// <summary>
	/// 種類比較
	/// </summary>
	public static int TypeCompare(ItemInfo infoX, ItemInfo infoY, bool isAscend)
	{
		if (isAscend) { return infoX.ItemMasterID.CompareTo(infoY.ItemMasterID); } else { return infoY.ItemMasterID.CompareTo(infoX.ItemMasterID); }
	}
	/// <summary>
	/// インデックス比較
	/// </summary>
	public static int IndexCompare(ItemInfo infoX, ItemInfo infoY, bool isAscend)
	{
		if (isAscend) { return infoX.Index.CompareTo(infoY.Index); } else { return infoY.Index.CompareTo(infoX.Index); }
	}
	#endregion

#if XW_DEBUG
	public void DebugSetIndex(int index) { this.Index = index; }
	public void DebugSetStack(int stack) { this.Stack = stack; }
	public void DebugRandomSetup()
	{
		int maxItemId = 0;
		Scm.Common.XwMaster.ItemMasterData data;
		for (int id = 1; id < 1000; ++id)
		{
			if (MasterData.TryGetItem(id, out data))
			{
				maxItemId = id;
			}
		}
		this.Index = 0;
		this.ItemMasterID = UnityEngine.Random.Range(1, maxItemId + 1);
		this.Stack = UnityEngine.Random.Range(0, 100);
		this.IsLock = UnityEngine.Random.Range(0, 2) == 1 ? true : false;
	}
#endif
}
#endregion

#region MailInfo
/// <summary>
/// MailParameter
/// </summary>
[System.Serializable]
public class MailInfo
{
	public enum MailType
	{
		Admin,
		Present
	}


	#region === Field ===

	private readonly MailType type = MailType.Admin;

	private int localIndex = -1;

	private int index = -1;

	private int iconId = -1;

	private string name = "";

	private string title = "";

	private string body = "";

	private bool isRead = false;

	private bool isDeleted = false;

	private bool isLocked = false;

	private DateTime receivedTime = new DateTime();

	private ItemInfo itemInfo = null;

	private DateTime? itemDeadlineTime = null;

	private DateTime? itemReceivedTime = null;

	#endregion === Field ===

	#region === Property ===

	/// <summary>
	/// メールタイプ
	/// </summary>
	public MailType Type { get { return type; } }

	/// <summary>
	/// ローカル管理インデックス
	/// </summary>
	public int LocalIndex { get { return localIndex; } set { localIndex = value; } }

	/// <summary>
	/// インデックス
	/// </summary>
	public int Index { get { return index; } }

	/// <summary>
	/// アイコンID
	/// </summary>
	public int IconID { get { return iconId; } }

	/// <summary>
	/// 受信時間
	/// </summary>
	public DateTime ReceivedTime { get { return receivedTime; } }

	/// <summary>
	/// メールの送り主
	/// </summary>
	public string SenderName { get { return name; } }

	/// <summary>
	/// タイトル
	/// </summary>
	public string Title { get { return title; } }

	/// <summary>
	/// メール本文
	/// </summary>
	public string Body { get { return body; } }

	/// <summary>
	/// 既読か
	/// </summary>
	public bool IsRead { get { return isRead; } }

	/// <summary>
	/// 削除済みか
	/// </summary>
	public bool IsDeleted { get { return isDeleted; } }

	/// <summary>
	/// ロックしているか
	/// </summary>
	public bool IsLocked { get { return isLocked; } set { isLocked = value; } }

	/// <summary>
	/// アイテム持ってるか
	/// </summary>
	public bool HasItem { get { return itemInfo != null; } }

	/// <summary>
	/// 添付アイテムID
	/// </summary>
	public ItemInfo ItemInfo { get { return itemInfo; } }

	/// <summary>
	/// 期限があるか
	/// </summary>
	public bool HasDeadline { get { return (itemDeadlineTime != null); } }

	/// <summary>
	/// 受取期限
	/// </summary>
	public DateTime? ItemDeadlineTime
	{
		get
		{
			return itemDeadlineTime;
		}
	}



	/// <summary>
	/// 受取期限
	/// </summary>
	public TimeSpan ItemDeadlineTimeSpan
	{
		get
		{
			if (itemDeadlineTime != null)
			{
				var span = itemDeadlineTime.Value - DateTime.Now;
				return span;
			}
			return TimeSpan.Zero;
		}
	}


	/// <summary>
	/// 受取り期限が切れている
	/// </summary>
	public bool IsReceiveExpiration
	{
		get
		{
			if (!IsItemReceived && itemDeadlineTime != null)
			{
				return itemDeadlineTime < DateTime.Now;
			}
			return false;
		}
	}

	/// <summary>
	/// アイテム受取り済みか
	/// </summary>
	public bool IsItemReceived
	{
		get
		{
			return (itemReceivedTime != null);
		}
	}

	/// <summary>
	/// アイテム受取りが可能か
	/// </summary>
	public bool EnableItemReceive
	{
		get
		{
			if (IsReceiveExpiration) return false;
			if (IsItemReceived) return false;
			if (!HasItem) return false;
			return true;
		}
	}

	#endregion === Property ===

	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public MailInfo()
	{

	}


#if XW_DEBUG
	// デバッグ用Constructor
	public MailInfo(
		string name,
		string title,
		string body,
		int icon,
		DateTime received,
		int itemId,
		int itemCount,
		DateTime? deadline,
		DateTime? itemReceive)
	{
		this.iconId = icon;
		this.name = name;
		this.body = body;
		this.receivedTime = received;
		this.itemDeadlineTime = deadline;
		this.itemReceivedTime = itemReceive;
		this.title = title;

		if (itemId > 0)
		{
			itemInfo = new ItemInfo(0, itemId, itemCount, false);
		}
	}


#endif


	/// <summary>
	/// パケット変換のコンストラクタ
	/// </summary>
	public MailInfo(MailParameter packet, int localIndex, MailType type)
	{
		this.type = type;
		this.localIndex = localIndex;
		this.Convert(packet);
	}

	/// <summary>
	/// パケット変換
	/// </summary>
	private void Convert(MailParameter packet)
	{
		// パケットから変換
		index = packet.Index;
		iconId = packet.MailIconId;
		name = packet.Name;
		body = Scm.Common.Utility.ReplaceFormat(packet.Body);	// 改行コード置換
		isRead = packet.ReadFlag;
		isDeleted = packet.DeleteFlag;
		isLocked = packet.LockFlag;
		receivedTime = packet.Received;
		itemDeadlineTime = packet.Deadline;
		itemReceivedTime = packet.ItemReceived;

		if (packet.ItemMasterId > 0)
		{
			itemInfo = new ItemInfo(packet);
		}

		// タイトル部作成
		title = body.Replace("\r\n", "\n").Replace("\r", "\n");
		int brIndex = title.IndexOf("\n");

		int length = title.Length > 28 ? 28 : title.Length;
		if (brIndex >= 0)
		{
			length = Math.Min(length, brIndex);
		}
		title = title.Substring(0, length);
	}

	/// <summary>
	/// 既読にする
	/// </summary>
	public void Read()
	{
		isRead = true;
	}

	/// <summary>
	/// アイテム受け取り時間セット
	/// </summary>
	public void ItemReceive()
	{
		if (!HasItem) return;

		if (itemReceivedTime == null)
		{
			itemReceivedTime = DateTime.Now;
		}
	}
}
#endregion

#region アチーブメント

#region AchievementInfo

/// <summary>
/// AchievementParameter
/// </summary>
[System.Serializable]
public class AchievementInfo
{

	#region ==== 定数 ====

	public enum RewardStatus
	{
		NotAchieved,	// 未達成
		Unacquired,     // 未取得
		Acquired		// 取得済
	}

	#endregion ==== 定数 ====

	#region ==== フィールド ====

	/// <summary>
	/// インデックス
	/// </summary>
	private int index = 0;

	/// <summary>
	/// アチーブメントマスターID
	/// </summary>
	private int masterId = 0;

	/// <summary>
	/// アチーブメントID
	/// </summary>
	private int achievementId = 0;

	/// <summary>
	/// アチーブメントタイトル
	/// </summary>
	private string title = "";

	/// <summary>
	/// アチーブメント説明
	/// </summary>
	private string description = "";

	/// <summary>
	/// カテゴリ
	/// </summary>
	private AchievementCategory category = AchievementCategory.Daily;

	/// <summary>
	/// カテゴリマスターデータ
	/// </summary>
	private Scm.Common.XwMaster.AchievementCategoryMasterData categoryMaster = null;

	/// <summary>
	/// タブタイプ
	/// </summary>
	private XUI.Achievement.AchievementTabType tabType = XUI.Achievement.AchievementTabType.EmergencyEvent;

	/// <summary>
	/// 新規フラグ
	/// </summary>
	private bool newFlag = false;

	/// <summary>
	/// 無効フラグ
	/// </summary>
	private bool disableFlag = false;

	/// <summary>
	/// 秘密フラグ
	/// </summary>
	private bool secretFlag = false;

	/// <summary>
	/// 進捗カウント
	/// </summary>
	private int progress = 0;

	/// <summary>
	/// 進捗閾値
	/// </summary>
	private int progressThreshold = 0;

	/// <summary>
	/// リワードステータス
	/// </summary>
	private RewardStatus rewardState = RewardStatus.NotAchieved;

	/// <summary>
	/// リワード内容
	/// </summary>
	private string rewardContent = "";

	/// <summary>
	/// リワード取得期限
	/// </summary>
	private string rewardDeadline = "";


	#endregion ==== フィールド ====

	#region ==== プロパティ ====

	/// <summary>
	/// インデックス
	/// </summary>
	public int Index { get { return index; } set { index = value; } }

	/// <summary>
	/// アチーブメントマスターID
	/// </summary>
	public int MasterID { get { return masterId; } }

	/// <summary>
	/// アチーブメントID
	/// </summary>
	public int AchievementID { get { return achievementId; } }

	/// <summary>
	/// アチーブメントタイトル
	/// </summary>
	public string Title { get { return title; } }

	/// <summary>
	/// アチーブメント説明
	/// </summary>
	public string Description { get { return description; } }

	/// <summary>
	/// カテゴリ
	/// </summary>
	public AchievementCategory Category { get { return category; } }

	/// <summary>
	/// カテゴリマスターデータ
	/// </summary>
	public Scm.Common.XwMaster.AchievementCategoryMasterData CategoryMaster { get { return categoryMaster; } }

	/// <summary>
	/// タブタイプ
	/// </summary>
	public XUI.Achievement.AchievementTabType TabType { get { return tabType; } }

	/// <summary>
	/// 新規フラグ
	/// </summary>
	public bool NewFlag { get { return newFlag; } }

	/// <summary>
	/// 無効フラグ
	/// </summary>
	public bool DisableFlag { get { return disableFlag; } }

	/// <summary>
	/// 秘密フラグ
	/// </summary>
	public bool SecretFlag { get { return secretFlag; } }

	/// <summary>
	/// 進捗カウント
	/// </summary>
	public int Progress { get { return progress; } }

	/// <summary>
	/// 進捗閾値
	/// </summary>
	public int ProgressThreshold { get { return progressThreshold; } }

	/// <summary>
	/// リワードステータス
	/// </summary>
	public RewardStatus RewardState { get { return rewardState; } }

	/// <summary>
	/// リワード内容
	/// </summary>
	public string RewardContent { get { return rewardContent; } }

	/// <summary>
	/// リワード取得期限
	/// </summary>
	public string RewardDeadline { get { return rewardDeadline; } }

	#endregion ==== プロパティ ====

	#region ==== コンストラクタ ====

	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public AchievementInfo()
	{
	}

	/// <summary>
	/// パケット変換コンストラクタ
	/// </summary>
	/// <param name="packet"></param>
	public AchievementInfo(AchievementParameter packet)
	{

		// コンバート
		Convert(packet);
	}

	#region ---- デバッグ用 ----
#if XW_DEBUG
	/// <summary>
	/// デバッグ用コンストラクタ
	/// </summary>
	/// <param name="masterId">アチーブメントマスターID</param>
	/// <param name="achievementId">アチーブメントID</param>
	/// <param name="title">アチーブメントタイトル</param>
	/// <param name="description">アチーブメント説明</param>
	/// <param name="category">カテゴリ</param>
	/// <param name="categoryMaster">カテゴリマスターデータ</param>
	/// <param name="tabType">タブタイプ</param>
	/// <param name="newFlag">新規フラグ</param>
	/// <param name="disableFlag">無効フラグ</param>
	/// <param name="secretFlag">秘密フラグ</param>
	/// <param name="progress">進捗カウント</param>
	/// <param name="progressThreshold">進捗閾値</param>
	/// <param name="rewardState">リワードステータス</param>
	/// <param name="rewardContent">リワード内容</param>
	/// <param name="rewardDeadline">リワード取得期限</param>
	public AchievementInfo(
			int masterId,
			int achievementId,
			string title,
			string description,
			AchievementCategory category,
			Scm.Common.XwMaster.AchievementCategoryMasterData categoryMaster,
			XUI.Achievement.AchievementTabType tabType,
			bool newFlag,
			bool disableFlag,
			bool secretFlag,
			int progress,
			int progressThreshold,
			RewardStatus rewardState,
			string rewardContent,
			string rewardDeadline
		)
	{

		// データ設定
		this.masterId = masterId;
		this.achievementId = achievementId;
		this.title = title;
		this.description = description;
		this.category = category;
		this.categoryMaster = categoryMaster;
		this.tabType = tabType;
		this.newFlag = newFlag;
		this.disableFlag = disableFlag;
		this.secretFlag = secretFlag;
		this.progress = progress;
		this.progressThreshold = progressThreshold;
		this.rewardState = rewardState;
		this.rewardContent = rewardContent;
		this.rewardDeadline = rewardDeadline;
	}
#endif
	#endregion ---- デバッグ用 ----

	#endregion ==== コンストラクタ ====

	#region ==== コンバート ====

	/// <summary>
	/// パケットのコンバート
	/// </summary>
	/// <param name="packet"></param>
	private void Convert(AchievementParameter packet)
	{

		Scm.Common.XwMaster.AchievementMasterData data;

		// アチーブメントマスターデータ取得
		if (!MasterData.TryGetAchievement(packet.AchievementMasterId, out data)) return;

		// アチーブメントマスターID
		masterId = packet.AchievementMasterId;

		// アチーブメントID
		achievementId = data.ID;

		// アチーブメントタイトル
		title = data.Title;

		// アチーブメント説明
		description = data.Description;

		// カテゴリ
		category = data.Category;

		// カテゴリマスターデータ
		categoryMaster = data.CategoryMaster;

		// タブタイプ
		tabType = ( XUI.Achievement.AchievementTabType )data.TabId;

		// 新規フラグ
		newFlag = packet.NewFlag;

		// 無効フラグ
		disableFlag = data.DisableFlag;

		// 秘密フラグ
		secretFlag = data.SecretFlag;

		// 進捗カウント
		progress = packet.Progress;

		// 進捗閾値
		progressThreshold = data.CountThreshold;

		// リワードステータス
		if (progress < progressThreshold)
		{
			// 未達成
			rewardState = RewardStatus.NotAchieved;

		}
		else if (!packet.RewardReceived)
		{
			// 未取得
			rewardState = RewardStatus.Unacquired;

		}
		else
		{
			// 取得済み
			rewardState = RewardStatus.Acquired;
		}

		// リワード内容
		rewardContent = "";
		Scm.Common.XwMaster.ItemMasterData item = null;

		if (data.ItemSetList.Count > 0)
		{
			foreach (Scm.Common.XwMaster.AchievementItemSetMasterData i in data.ItemSetList)
			{
				// アイテムデータの取得
				MasterData.TryGetItem(i.ItemId, out item);

				// 報酬をテキストに追加
				rewardContent += String.Format("{0} x{1:00} ", item.Name, i.Stack);
			}
		}

		// リワード取得期限
		if (packet.RewardDeadline == null)
		{
			// 未達成 or 無期限
			rewardDeadline = "";

		}
		else
		{
			// 取得期限設定
			rewardDeadline = packet.RewardDeadline.Value.ToString("yyyy/MM/dd HH:mm");
		}
	}

	#endregion ==== コンバート ====
}

#endregion AchievementInfo

#region AchievementRewardInfo

/// <summary>
/// AchievementRewardParameter
/// </summary>
[System.Serializable]
public class AchievementRewardInfo
{

	#region ==== クラス ====

	public class RewardItem
	{
		public string Name { get; private set; }
		public int Num { get; private set; }

		public RewardItem(string name, int num)
		{
			Name = name;
			Num = num;
		}
	}

	#endregion ==== クラス ====

	#region ==== フィールド ====

	/// <summary>
	/// アチーブメントマスターID
	/// </summary>
	private int masterId = 0;

	/// <summary>
	/// 結果
	/// </summary>
	private bool result = false;

#if XW_DEBUG
	/// <summary>
	/// リワードアイテム
	/// </summary>
	private List<RewardItem> reward = new List<RewardItem>();
#endif

	#endregion ==== フィールド ====

	#region ==== プロパティ ====

	/// <summary>
	/// アチーブメントマスターID
	/// </summary>
	public int MasterID { get { return masterId; } }

	/// <summary>
	/// 結果
	/// </summary>
	public bool Result { get { return result; } }

	/// <summary>
	/// リワードアイテム
	/// </summary>
	public List<RewardItem> Reward = new List<RewardItem>();

	#endregion ==== プロパティ ====

	#region ==== コンストラクタ ====

	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public AchievementRewardInfo()
	{
	}

	/// <summary>
	/// パケット変換コンストラクタ
	/// </summary>
	/// <param name="packet"></param>
	public AchievementRewardInfo(AchievementRewardParameter packet)
	{

		// コンバート
		Convert(packet);
	}

	#region ---- デバッグ用 ----
#if XW_DEBUG
	/// <summary>
	/// デバッグ用コンストラクタ
	/// </summary>
	/// <param name="masterId">アチーブメントマスターID</param>
	/// <param name="name">アイテム名配列</param>
	/// <param name="num">アイテム個数配列</param>
	public AchievementRewardInfo(
			int masterId,
			string[] name,
			int[] num
		)
	{

		int max = name.Length;

		// データ設定
		this.masterId = masterId;

		for (int i = 0; i < max; i++)
		{
			this.reward.Add(new RewardItem(name[i], num[i]));
		}
	}
#endif
	#endregion ---- デバッグ用 ----

	#endregion ==== コンストラクタ ====

	#region ==== コンバート ====

	/// <summary>
	/// パケットのコンバート
	/// </summary>
	/// <param name="packet"></param>
	private void Convert(AchievementRewardParameter packet)
	{

		Scm.Common.XwMaster.AchievementMasterData data;

		// アチーブメントマスターデータ取得
		if (!MasterData.TryGetAchievement(packet.AchievementMasterId, out data)) return;

		// アチーブメントマスターID
		masterId = packet.AchievementMasterId;

		// 結果
		result = packet.Result;

		// リワード内容
		Scm.Common.XwMaster.ItemMasterData item = null;

		if (data.ItemSetList.Count > 0)
		{
			foreach (Scm.Common.XwMaster.AchievementItemSetMasterData i in data.ItemSetList)
			{
				// アイテムデータの取得
				MasterData.TryGetItem(i.ItemId, out item);

				// 報酬をリストに追加
				Reward.Add(new RewardItem(item.Name, i.Stack));
			}
		}
	}

	#endregion ==== コンバート ====
}

#endregion AchievementRewardInfo

#endregion アチーブメント

#region ショップ

#region TicketShopItemInfo

/// <summary>
/// TicketShopItemInfo
/// </summary>
public class TicketShopItemInfo {

	#region ==== フィールド ====

	private int index = 0;
	private int id = 0;
	private int price = 0;
	private string name = "";

	#endregion ==== フィールド ====

	#region ==== プロパティ ====

	/// <summary>
	/// インデックス
	/// </summary>
	public int Index { get { return index; } set { index = value; } }

	/// <summary>
	/// 商品ID
	/// </summary>
	public int ID { get { return id; } set { id = value; } }

	/// <summary>
	/// 商品価格
	/// </summary>
	public int Price { get { return price; } set { price = value; } }

	/// <summary>
	/// 商品名
	/// </summary>
	public string Name { get { return name; } set { name = value; } }

	#endregion ==== プロパティ ====

	#region ==== コンストラクタ ====

	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public TicketShopItemInfo() {
	}

	/// <summary>
	/// パケット変換コンストラクタ
	/// </summary>
	/// <param name="data"></param>
	public TicketShopItemInfo( string data ) {

		// コンバート
		Convert( data );
	}

	#region ---- デバッグ ----
#if XW_DEBUG
	/// <summary>
	/// デバッグ用コンストラクタ
	/// </summary>
	/// <param name="index">インデックス</param>
	/// <param name="id">商品ID</param>
	/// <param name="price">商品価格</param>
	/// <param name="name">商品名</param>
	public TicketShopItemInfo(
		int index,
		int id,
		int price,
		string name
		) {

		// データ設定
		this.index = index;
		this.id = id;
		this.price = price;
		this.name = name;
	}
#endif
	#endregion ---- デバッグ ----

	#endregion ==== コンストラクタ ====

	#region ==== コンバート ====

	/// <summary>
	/// データのコンバート
	/// </summary>
	/// <param name="data"></param>
	private void Convert( string data ) {

		// 商品ID
		id = 0;

		// 商品価格
		price = 0;

		// 商品名
		name = "";
	}

	#endregion ==== コンバート ====
}

#endregion TicketShopItemInfo

#region CharaShopItemInfo

/// <summary>
/// CharaShopItemInfo
/// </summary>
[System.Serializable]
public class CharaShopItemInfo {

	#region ==== 定数 ====

	/// <summary>
	/// 商品状態
	/// </summary>
	public enum Status {
		Normal,
		Improper,
		Limit,
		TicketShortage
	}

	#endregion ==== 定数 ====

	#region ==== フィールド ====

	private int index = 0;
	private int productId = 0;
	private int price = 0;
	private int charaId = 0;
	private int coolTime = 0;
	private string charaName = "";
	private string explanatory = "";
	private Status status = Status.Normal;

	#endregion ==== フィールド ====

	#region ==== プロパティ ====

	/// <summary>
	/// インデックス
	/// </summary>
	public int Index { get { return index; } set { index = value; } }

	/// <summary>
	/// 商品ID
	/// </summary>
	public int ProductID { get { return productId; } }

	/// <summary>
	/// 商品価格
	/// </summary>
	public int Price { get { return price; } }

	/// <summary>
	/// キャラクターID
	/// </summary>
	public int CharaID { get { return charaId; } }

	/// <summary>
	/// クールタイム
	/// </summary>
	public int CoolTime { get { return coolTime; } }

	/// <summary>
	/// キャラクター名
	/// </summary>
	public string CharaName { get { return charaName; } }

	/// <summary>
	/// 説明文
	/// </summary>
	public string Explanatory { get { return explanatory; } }

	/// <summary>
	/// 商品ステータス
	/// </summary>
	public Status ProductStatus { get { return status; } }

	#endregion ==== プロパティ ====

	#region ==== コンストラクタ ====

	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public CharaShopItemInfo() {
	}

	/// <summary>
	/// パケット変換コンストラクタ
	/// </summary>
	/// <param name="param"></param>
	public CharaShopItemInfo( Asobimo.WebAPI.ProductStatus param ) {

		// コンバート
		Convert( param );
	}

	#region ---- デバッグ ----
#if XW_DEBUG
	/// <summary>
	/// デバッグ用コンストラクタ
	/// </summary>
	/// <param name="index">インデックス</param>
	/// <param name="productId">商品ID</param>
	/// <param name="price">価格</param>
	/// <param name="charaId">キャラクターID</param>
	/// <param name="coolTime">クールタイム</param>
	/// <param name="charaName">キャラクター名</param>
	/// <param name="explanatory">説明</param>
	/// <param name="status">商品状態</param>
	public CharaShopItemInfo(
		int index,
		int productId,
		int price,
		int charaId,
		int coolTime,
		string charaName,
		string explanatory,
		Status status
		) {

		// データ設定
		this.index = index;
		this.productId = productId;
		this.price = price;
		this.charaId = charaId;
		this.coolTime = coolTime;
		this.charaName = charaName;
		this.explanatory = explanatory;
		this.status = status;
	}
#endif
	#endregion ---- デバッグ ----

	#endregion ==== コンストラクタ ====

	#region ==== コンバート ====

	/// <summary>
	/// データのコンバート
	/// </summary>
	/// <param name="param"></param>
	private void Convert( Asobimo.WebAPI.ProductStatus param ) {

		Scm.Common.XwMaster.ShopItemMasterData shopData;
		Scm.Common.XwMaster.CharaMasterData charaData;

		// マスターデータ取得
		if( !MasterData.TryGetShop( param.GameID, out shopData ) ) {
			Debug.LogError( "CharaShopItemInfo TryGetShop Error:" + param.GameID );
		}
		if( !MasterData.TryGetChara( shopData.CharacterId, out charaData ) ) {
			Debug.Log( "CharaShopItemInfo TryGetChara Error:" + shopData.CharacterId );
		}

		// インデックス
		index = 0;

		// 商品ID
		productId = param.ProductID;

		// 商品価格
		price = param.Price;

		// キャラクターID
		charaId = shopData.CharacterId;

		// クールタイム
		coolTime = ( int )( charaData.RebuildTime * 0.1f );

		// キャラクター名
		charaName = charaData.Name;

		// 説明文
		explanatory = shopData.Description;

		// 商品ステータス
		switch( param.ProductObtainCode ) {
			case ObtainCode.Completion:			status = Status.Normal;			break;
			case ObtainCode.NotBeRetrieved:		status = Status.Improper;		break;
			case ObtainCode.AcquisitionLimit:	status = Status.Limit;			break;
			case ObtainCode.PointShortage:		status = Status.TicketShortage;	break;
		}
	}

	#endregion ==== コンバート ====
}

#endregion CharaShopItemInfo

#endregion ショップ