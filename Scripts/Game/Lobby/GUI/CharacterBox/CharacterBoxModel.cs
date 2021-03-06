/// <summary>
/// キャラBOXデータ
/// 
/// 2016/04/29
/// </summary>
using UnityEngine;
using System;
using System.Collections.Generic;

namespace XUI.CharacterBox
{
	/// <summary>
	/// キャラBOXデータインターフェイス
	/// </summary>
	public interface IModel
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();
		#endregion

		#region 所持金
		/// <summary>
		/// 所持金変更イベント
		/// </summary>
		event EventHandler OnHaveMoneyChange;
		/// <summary>
		/// 所持金
		/// </summary>
		int HaveMoney { get; set; }

		/// <summary>
		/// 所持金フォーマット変更イベント
		/// </summary>
		event EventHandler OnHaveMoneyFormatChange;
		/// <summary>
		/// 所持金フォーマット
		/// </summary>
		string HaveMoneyFormat { get; set; }
		#endregion

		#region 総売却額
		/// <summary>
		/// 総売却額変更イベント
		/// </summary>
		event EventHandler OnTotalSoldPriceChange;
		int TotalSoldPrice { get; set; }

		/// <summary>
		/// 総売却額フォーマット変更イベント
		/// </summary>
		event EventHandler OnTotalSoldPriceFormatChange;
		string TotalSoldPriceFormat { get; set; }
		#endregion

		#region キャラの売却額
		/// <summary>
		/// キャラ売却額
		/// </summary>
		event EventHandler OnSoldPriceChange;
		int SoldPrice { get; set; }

		/// <summary>
		/// キャラ売却額フォーマット
		/// </summary>
		event EventHandler OnSoldPriceFormatChange;
		string SoldPriceFormat { get; set; }
		#endregion

		#region 追加金額
		/// <summary>
		/// 追加金額変更イベント
		/// </summary>
		event EventHandler OnAddOnCharge;
		/// <summary>
		/// 売却時に発生する追加料金
		/// </summary>
		int AddOnCharge { get; set; }
		#endregion

		#region キャラ名
		/// <summary>
		/// キャラ名
		/// </summary>
		event EventHandler OnCharaNameChange;
		string CharaName { get; set; }
		#endregion

		#region キャラロックフラグ
		/// <summary>
		/// キャラロックフラグ
		/// </summary>
		event EventHandler OnLockChange;
		bool IsLock { get; set; }
		#endregion

		#region リビルドタイム
		/// <summary>
		/// リビルドタイム変更イベント
		/// </summary>
		event EventHandler OnRebuildTimeChange;
		/// <summary>
		/// リビルドタイム
		/// </summary>
		float RebuildTime { get; set; }
		#endregion

		#region コスト
		/// <summary>
		/// コスト変更イベント
		/// </summary>
		event EventHandler OnCostChange;
		/// <summary>
		/// コスト
		/// </summary>
		int Cost { get; set; }
		#endregion

		#region 経験値
		/// <summary>
		/// 経験値変更イベント
		/// </summary>
		event EventHandler OnExpChange;
		/// <summary>
		/// 経験値
		/// </summary>
		int Exp { get; set; }

		/// <summary>
		/// 経験値フォーマット変更イベント
		/// </summary>
		event EventHandler OnExpFormatChange;
		/// <summary>
		/// 経験値フォーマット
		/// </summary>
		string ExpFormat { get; set; }
		#endregion

		#region シンクロ可能回数
		/// <summary>
		/// シンクロ可能回数変更イベント
		/// </summary>
		event EventHandler OnSynchroRemainChange;
		/// <summary>
		/// シンクロ可能回数
		/// </summary>
		int SynchroRemain { get; set; }
		#endregion

		#region 生命力
		/// <summary>
		/// 生命力変更イベント
		/// </summary>
		event EventHandler OnHitPointChange;
		/// <summary>
		/// 生命力
		/// </summary>
		int HitPoint { get; set; }

		/// <summary>
		/// 生命力フォーマット変更イベント
		/// </summary>
		event EventHandler OnHitPointFormatChange;
		/// <summary>
		/// 生命力フォーマット
		/// </summary>
		string HitPointFormat { get; set; }
		#endregion

		#region 攻撃力
		/// <summary>
		/// 攻撃力変更イベント
		/// </summary>
		event EventHandler OnAttackChange;
		/// <summary>
		/// 攻撃力
		/// </summary>
		int Attack { get; set; }

		/// <summary>
		/// 攻撃力フォーマット変更イベント
		/// </summary>
		event EventHandler OnAttackFormatChange;
		/// <summary>
		/// 攻撃力フォーマット
		/// </summary>
		string AttackFormat { get; set; }
		#endregion

		#region 防御力
		/// <summary>
		/// 防御力変更イベント
		/// </summary>
		event EventHandler OnDefenceChange;
		/// <summary>
		/// 防御力
		/// </summary>
		int Defence { get; set; }

		/// <summary>
		/// 防御力フォーマット変更イベント
		/// </summary>
		event EventHandler OnDefenceFormatChange;
		/// <summary>
		/// 防御力フォーマット
		/// </summary>
		string DefenceFormat { get; set; }
		#endregion

		#region 特殊能力
		/// <summary>
		/// 特殊能力変更イベント
		/// </summary>
		event EventHandler OnExtraChange;
		/// <summary>
		/// 特殊能力
		/// </summary>
		int Extra { get; set; }

		/// <summary>
		/// 特殊能力フォーマット変更イベント
		/// </summary>
		event EventHandler OnExtraFormatChange;
		/// <summary>
		/// 特殊能力フォーマット
		/// </summary>
		string ExtraFormat { get; set; }
		#endregion

		#region キャラ情報リスト
		/// <summary>
		/// キャラ情報リストのセットイベント
		/// </summary>
		event EventHandler OnCharaInfoListChange;
		/// <summary>
		/// キャラ情報リストのセット
		/// </summary>
		void SetCharaInfoList(List<CharaInfo> infoList);

		/// <summary>
		/// キャラ情報リストのクリアイベント
		/// </summary>
		event EventHandler OnClearCharaInfoList;
		/// <summary>
		/// キャラ情報リストのクリア
		/// </summary>
		void ClearCharaInfoList();

		/// <summary>
		/// キャラ情報一覧を取得
		/// </summary>
		List<CharaInfo> GetCharaInfoList();

		/// <summary>
		/// キャラマスタID指定で関連するキャラ情報一覧を取得
		/// </summary>
		bool TryGetCharaInfoByMasterId(int charaMasterId, out Dictionary<ulong, CharaInfo> uuidDic);

		/// <summary>
		/// 指定するIDとUUIDからキャラ情報を取得する
		/// </summary>
		bool TryGetCharaInfo(int charaMasterId, ulong uuid, out CharaInfo charaInfo);
		#endregion
	}

	/// <summary>
	/// キャラBOXデータ
	/// </summary>
	public class Model : IModel
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			this.OnHaveMoneyChange = null;
			this.OnHaveMoneyFormatChange = null;
			this.OnTotalSoldPriceChange = null;
			this.OnTotalSoldPriceFormatChange = null;
			this.OnSoldPriceChange = null;
			this.OnAddOnCharge = null;
			this.OnCharaNameChange = null;
			this.OnLockChange = null;
			this.OnRebuildTimeChange = null;
			this.OnCostChange = null;
			this.OnExpChange = null;
			this.OnExpFormatChange = null;
			this.OnSynchroRemainChange = null;
			this.OnHitPointChange = null;
			this.OnHitPointFormatChange = null;
			this.OnAttackChange = null;
			this.OnAttackFormatChange = null;
			this.OnDefenceChange = null;
			this.OnDefenceFormatChange = null;
			this.OnExtraChange = null;
			this.OnExtraFormatChange = null;
			this.OnCharaInfoListChange = null;
			this.OnClearCharaInfoList = null;
		}
		#endregion

		#region 所持金
		/// <summary>
		/// 所持金変更イベント
		/// </summary>
		public event EventHandler OnHaveMoneyChange = (sender, e) => { };
		/// <summary>
		/// 所持金
		/// </summary>
		private int _haveMoney = 0;
		public int HaveMoney
		{
			get { return _haveMoney; }
			set
			{
				if (_haveMoney != value)
				{
					_haveMoney = value;

					// 通知
					this.OnHaveMoneyChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// 所持金フォーマット変更イベント
		/// </summary>
		public event EventHandler OnHaveMoneyFormatChange = (sender, e) => { };
		/// <summary>
		/// 所持金フォーマット
		/// </summary>
		private string _haveMoneyFormat = "";
		public string HaveMoneyFormat
		{
			get { return _haveMoneyFormat; }
			set
			{
				if (_haveMoneyFormat != value)
				{
					_haveMoneyFormat = value;

					// 通知
					this.OnHaveMoneyFormatChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region 総売却額
		/// <summary>
		/// 総売却額変更イベント
		/// </summary>
		public event EventHandler OnTotalSoldPriceChange = (sender, e) => { };
		/// <summary>
		/// 総売却額
		/// </summary>
		private int _totalSoldPrice = 0;
		public int TotalSoldPrice
		{
			get { return _totalSoldPrice; }
			set
			{
				if (_totalSoldPrice != value)
				{
					_totalSoldPrice = Math.Min(value, MasterDataCommonSetting.Player.PlayerMaxGameMoney);

					// 通知
					this.OnTotalSoldPriceChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// 総売却額フォーマット変更イベント
		/// </summary>
		public event EventHandler OnTotalSoldPriceFormatChange = (sender, e) => { };
		/// <summary>
		/// 総売却額フォーマット
		/// </summary>
		private string _totalSoldPriceFormat = "";
		public string TotalSoldPriceFormat
		{
			get { return _totalSoldPriceFormat; }
			set
			{
				if (_totalSoldPriceFormat != value)
				{
					_totalSoldPriceFormat = value;

					// 通知
					this.OnTotalSoldPriceFormatChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region キャラの売却額
		/// <summary>
		/// キャラ売却額
		/// </summary>
		public event EventHandler OnSoldPriceChange = (sender, e) => { };
		/// <summary>
		/// キャラ売却額
		/// </summary>
		private int _soldPrice = 0;
		public int SoldPrice
		{
			get { return _soldPrice; }
			set
			{
				if (_soldPrice != value)
				{
					_soldPrice = Math.Min(value, MasterDataCommonSetting.Player.PlayerMaxGameMoney);

					// 通知
					this.OnSoldPriceChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// キャラ売却額フォーマット変更イベント
		/// </summary>
		public event EventHandler OnSoldPriceFormatChange = (sender, e) => { };
		/// <summary>
		/// キャラ売却額フォーマット
		/// </summary>
		private string _soldPriceFormat = "";
		public string SoldPriceFormat
		{
			get { return _soldPriceFormat; }
			set
			{
				if (_soldPriceFormat != value)
				{
					_soldPriceFormat = value;

					// 通知
					this.OnSoldPriceFormatChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region 追加金額
		/// <summary>
		/// 追加金額変更イベント
		/// </summary>
		public event EventHandler OnAddOnCharge = (sender, e) => { };
		/// <summary>
		/// 追加金額
		/// </summary>
		private int _addOnCharge = 0;
		/// <summary>
		/// 売却時に発生する追加料金
		/// </summary>
		public int AddOnCharge
		{
			get { return _addOnCharge; }
			set
			{
				if(_addOnCharge != value)
				{
					_addOnCharge = value;

					// 通知
					this.OnAddOnCharge(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region キャラ名
		/// <summary>
		/// キャラ名変更イベント
		/// </summary>
		public event EventHandler OnCharaNameChange = (sender, e) => { };
		/// <summary>
		/// キャラ名
		/// </summary>
		private string _charaName = "";
		public string CharaName
		{
			get { return _charaName; }
			set
			{
				if (_charaName != value)
				{
					_charaName = value;

					// 通知
					this.OnCharaNameChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region キャラロックフラグ
		/// <summary>
		/// キャラロックフラグ変更イベント
		/// </summary>
		public event EventHandler OnLockChange = (sender, e) => { };
		/// <summary>
		/// キャラロックフラグ
		/// </summary>
		private bool _isLock = false;
		public bool IsLock
		{
			get { return _isLock; }
			set
			{
				if (_isLock != value)
				{
					_isLock = value;

					// 通知
					this.OnLockChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region リビルドタイム
		/// <summary>
		/// リビルドタイム変更イベント
		/// </summary>
		public event EventHandler OnRebuildTimeChange = (sender, e) => { };
		/// <summary>
		/// リビルドタイム
		/// </summary>
		private float _rebuildTime = 0;
		public float RebuildTime
		{
			get { return _rebuildTime; }
			set
			{
				if(_rebuildTime != value)
				{
					_rebuildTime = value;

					// 通知
					this.OnRebuildTimeChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region コスト
		/// <summary>
		/// コスト変更イベント
		/// </summary>
		public event EventHandler OnCostChange = (sender, e) => { };
		/// <summary>
		/// コスト
		/// </summary>
		private int _cost = 0;
		public int Cost
		{
			get { return _cost; }
			set
			{
				if(_cost != value)
				{
					_cost = value;

					// 通知
					this.OnCostChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region 経験値
		/// <summary>
		/// 経験値変更イベント
		/// </summary>
		public event EventHandler OnExpChange = (sender, e) => { };
		/// <summary>
		/// 経験値
		/// </summary>
		private int _exp = 0;
		public int Exp
		{
			get { return _exp; }
			set
			{
				if(_exp != value)
				{
					_exp = value;

					// 通知
					this.OnExpChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// 経験値フォーマット変更イベント
		/// </summary>
		public event EventHandler OnExpFormatChange = (sender, e) => { };
		/// <summary>
		/// 経験値フォーマット
		/// </summary>
		private string _expFormat = "";
		public string ExpFormat
		{
			get { return _expFormat; }
			set
			{
				if(_expFormat != value)
				{
					_expFormat = value;

					// 通知
					this.OnExpFormatChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region シンクロ可能回数
		/// <summary>
		/// シンクロ可能回数変更イベント
		/// </summary>
		public event EventHandler OnSynchroRemainChange = (sender, e) => { };
		/// <summary>
		/// シンクロ可能回数
		/// </summary>
		private int _synchroRemain = 0;
		public int SynchroRemain
		{
			get { return _synchroRemain; }
			set
			{
				if(_synchroRemain != value)
				{
					_synchroRemain = value;

					// 通知
					this.OnSynchroRemainChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region 生命力
		/// <summary>
		/// 生命力変更イベント
		/// </summary>
		public event EventHandler OnHitPointChange = (sender, e) => { };
		/// <summary>
		/// 生命力
		/// </summary>
		private int _hitPoint = 0;
		public int HitPoint
		{
			get { return _hitPoint; }
			set
			{
				if(_hitPoint != value)
				{
					_hitPoint = value;

					// 通知
					this.OnHitPointChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// 生命力フォーマット変更イベント
		/// </summary>
		public event EventHandler OnHitPointFormatChange = (sender, e) => { };
		/// <summary>
		/// 生命力フォーマット
		/// </summary>
		private string _hitPointFormat = "";
		public string HitPointFormat
		{
			get { return _hitPointFormat; }
			set
			{
				if(_hitPointFormat != value)
				{
					_hitPointFormat = value;

					// 通知
					this.OnHitPointFormatChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region 攻撃力
		/// <summary>
		/// 攻撃力変更イベント
		/// </summary>
		public event EventHandler OnAttackChange = (sender, e) => { };
		/// <summary>
		/// 攻撃力
		/// </summary>
		private int _attack = 0;
		public int Attack
		{
			get { return _attack; }
			set
			{
				if(_attack != value)
				{
					_attack = value;

					// 通知
					this.OnAttackChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// 攻撃力フォーマット変更イベント
		/// </summary>
		public event EventHandler OnAttackFormatChange = (sender, e) => { };
		/// <summary>
		/// 攻撃力フォーマット
		/// </summary>
		private string _attackFormat = "";
		public string AttackFormat
		{
			get { return _attackFormat; }
			set
			{
				if(_attackFormat != value)
				{
					_attackFormat = value;

					// 通知
					this.OnAttackFormatChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region 防御力
		/// <summary>
		/// 防御力変更イベント
		/// </summary>
		public event EventHandler OnDefenceChange = (sender, e) => { };
		/// <summary>
		/// 防御力
		/// </summary>
		private int _defence = 0;
		public int Defence
		{
			get { return _defence; }
			set
			{
				if(_defence != value)
				{
					_defence = value;

					// 通知
					this.OnDefenceChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// 防御力フォーマット変更イベント
		/// </summary>
		public event EventHandler OnDefenceFormatChange = (sender, e) => { };
		/// <summary>
		/// 防御力フォーマット
		/// </summary>
		private string _defenceFormat = "";
		public string DefenceFormat
		{
			get { return _defenceFormat; }
			set
			{
				if(_defenceFormat != value)
				{
					_defenceFormat = value;

					// 通知
					this.OnDefenceFormatChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region 特殊能力
		/// <summary>
		/// 特殊能力変更イベント
		/// </summary>
		public event EventHandler OnExtraChange = (sender, e) => { };
		/// <summary>
		/// 特殊能力
		/// </summary>
		private int _extra = 0;
		public int Extra
		{
			get { return _extra; }
			set
			{
				if(_extra != value)
				{
					_extra = value;

					// 通知
					this.OnExpChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// 特殊能力フォーマット変更イベント
		/// </summary>
		public event EventHandler OnExtraFormatChange = (sender, e) => { };
		/// <summary>
		/// 特殊能力フォーマット
		/// </summary>
		private string _extraFormat = "";
		public string ExtraFormat
		{
			get { return _extraFormat; }
			set
			{
				if(_extraFormat != value)
				{
					_extraFormat = value;

					// 通知
					this.OnExpFormatChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region キャラ情報リスト
		/// <summary>
		/// キャラ情報をキャラマスターIDで区分けした一覧
		/// Key = CharaMasterID
		/// Value = UUIDで区分けした一覧
		/// </summary>
		private Dictionary<int, Dictionary<ulong, CharaInfo>> charaInfoDic = new Dictionary<int, Dictionary<ulong, CharaInfo>>();

		/// <summary>
		/// キャラ情報リストのセット
		/// </summary>
		public event EventHandler OnCharaInfoListChange = (sender, e) => { };
		public void SetCharaInfoList(List<CharaInfo> infoList)
		{
			this.charaInfoDic.Clear();
			foreach (var info in infoList)
			{
				if (!this.charaInfoDic.ContainsKey(info.CharacterMasterID))
				{
					this.charaInfoDic.Add(info.CharacterMasterID, new Dictionary<ulong, CharaInfo>());
				}

				Dictionary<ulong, CharaInfo> uuidCharaInfoDic;
				if (this.charaInfoDic.TryGetValue(info.CharacterMasterID, out uuidCharaInfoDic))
				{
					if (!uuidCharaInfoDic.ContainsKey(info.UUID))
					{
						uuidCharaInfoDic.Add(info.UUID, info);
					}
				}
			}

			// 通知
			this.OnCharaInfoListChange(this, EventArgs.Empty);
		}

		/// <summary>
		/// キャラ情報リストのクリア
		/// </summary>
		public event EventHandler OnClearCharaInfoList = (sender, e) => { };
		public void ClearCharaInfoList()
		{
			if (this.charaInfoDic.Count > 0)
			{
				this.charaInfoDic.Clear();

				// 通知
				OnClearCharaInfoList(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// キャラ情報一覧を取得
		/// </summary>
		public List<CharaInfo> GetCharaInfoList()
		{
			var infoList = new List<CharaInfo>();
			foreach (var uuidDic in this.charaInfoDic.Values)
			{
				foreach (var info in uuidDic.Values)
				{
					infoList.Add(info);
				}
			}

			return infoList;
		}

		/// <summary>
		/// キャラマスタID指定で関連するキャラ情報一覧を取得
		/// </summary>
		public bool TryGetCharaInfoByMasterId(int charaMasterId, out Dictionary<ulong, CharaInfo> uuidDic)
		{
			return this.charaInfoDic.TryGetValue(charaMasterId, out uuidDic);
		}

		/// <summary>
		/// 指定するIDとUUIDからキャラ情報を取得する
		/// </summary>
		public bool TryGetCharaInfo(int charaMasterId, ulong uuid, out CharaInfo charaInfo)
		{
			charaInfo = null;
			Dictionary<ulong, CharaInfo> uuidCharaInfoDic = new Dictionary<ulong, CharaInfo>();
			if(!this.TryGetCharaInfoByMasterId(charaMasterId, out uuidCharaInfoDic))
			{
				// キャラマスターIDが見つからない
				return false;
			}
			if(!uuidCharaInfoDic.TryGetValue(uuid, out charaInfo))
			{
				// UUIDが見つからない
				return false;
			}

			return true;
		}
		#endregion
	}
}