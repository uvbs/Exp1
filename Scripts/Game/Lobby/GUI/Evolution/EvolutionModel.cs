/// <summary>
/// 進化合成データ
/// 
/// 2016/02/02
/// </summary>
using System;
using System.Collections.Generic;

namespace XUI
{
	namespace Evolution
	{
		/// <summary>
		/// 進化合成データインターフェイス
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

			#region 費用
			/// <summary>
			/// 費用変更イベント
			/// </summary>
			event EventHandler OnNeedMoneyChange;
			/// <summary>
			/// 費用
			/// </summary>
			int NeedMoney { get; set; }

			/// <summary>
			/// 費用フォーマット変更イベント
			/// </summary>
			event EventHandler OnNeedMoneyFormatChange;
			/// <summary>
			/// 費用フォーマット
			/// </summary>
			string NeedMoneyFormat { get; set; }
			#endregion

			#region 追加料金
			/// <summary>
			/// 追加料金変更イベント
			/// </summary>
			event EventHandler OnAddOnChargeChange;
			/// <summary>
			/// 追加料金
			/// </summary>
			int AddOnCharge { get; set; }

			/// <summary>
			/// 追加料金フォーマット変更イベント
			/// </summary>
			event EventHandler OnAddOnChargeFormatChange;
			/// <summary>
			/// 追加料金フォーマット
			/// </summary>
			string AddOnChargeFormat { get; set; }
			#endregion

			#region キャラステータス
			/// <summary>
			/// ベースキャラのシンクロ合成残り回数変更イベント
			/// </summary>
			event EventHandler OnBaseSynchroRemainChange;
			/// <summary>
			/// ベースキャラのシンクロ合成残り回数
			/// </summary>
			int BaseSynchroRemain { get; set; }

			/// <summary>
			/// ベースキャラのランク変更イベント
			/// </summary>
			event EventHandler OnBaseRankChange;
			/// <summary>
			/// ベースキャラのランク
			/// </summary>
			int BaseRank { get; set; }

			/// <summary>
			/// 進化キャラのシンクロ合成残り回数変更イベント
			/// </summary>
			event EventHandler OnEvolutionSynchroRemainChange;
			/// <summary>
			/// 進化キャラのシンクロ合成残り回数
			/// </summary>
			int EvolutionSynchroRemain { get; set; }

			/// <summary>
			/// 進化キャラのランク変更イベント
			/// </summary>
			event EventHandler OnEvolutionRankChange;
			/// <summary>
			/// 進化キャラのランク
			/// </summary>
			int EvolutionRank { get; set; }
			#endregion

			#region キャラ情報リスト
			/// <summary>
			/// キャラ情報リストのセット
			/// </summary>
			event EventHandler OnCharaInfoListChange;
			void SetCharaInfoList(List<CharaInfo> charaInfoList);

			/// <summary>
			/// キャラ情報リストクリア
			/// </summary>
			event EventHandler OnClearCharaInfoList;
			void ClearCharaInfoList();

			/// <summary>
			/// キャラ情報一覧を取得
			/// </summary>
			List<CharaInfo> GetCharaInfoList();

			/// <summary>
			/// キャラマスタID指定で関連するアイテム情報一覧を取得
			/// </summary>
			bool TryGetCharaInfoByMasterId(int charaMasterId, out Dictionary<ulong, CharaInfo> uuidDic);
			#endregion
		}

		/// <summary>
		/// 進化合成データ
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
				this.OnNeedMoneyChange = null;
				this.OnNeedMoneyFormatChange = null;
				this.OnAddOnChargeChange = null;
				this.OnAddOnChargeFormatChange = null;
				this.OnAddOnChargeChange = null;
				this.OnBaseSynchroRemainChange = null;
				this.OnBaseRankChange = null;
				this.OnEvolutionSynchroRemainChange = null;
				this.OnEvolutionRankChange = null;
				this.OnCharaInfoListChange = null;
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

			#region 費用
			/// <summary>
			/// 費用変更イベント
			/// </summary>
			public event EventHandler OnNeedMoneyChange = (sender, e) => { };
			/// <summary>
			/// 費用
			/// </summary>
			private int _needMoney = 0;
			public int NeedMoney
			{
				get { return _needMoney; }
				set
				{
					if (_needMoney != value)
					{
						_needMoney = value;

						// 通知
						this.OnNeedMoneyChange(this, EventArgs.Empty);
					}
				}
			}

			/// <summary>
			/// 費用フォーマット変更イベント
			/// </summary>
			public event EventHandler OnNeedMoneyFormatChange = (sender, e) => { };
			/// <summary>
			/// 費用フォーマット
			/// </summary>
			private string _needMoneyFormat = "";
			public string NeedMoneyFormat
			{
				get { return _needMoneyFormat; }
				set
				{
					if (_needMoneyFormat != value)
					{
						_needMoneyFormat = value;

						// 通知
						this.OnNeedMoneyFormatChange(this, EventArgs.Empty);
					}
				}
			}
			#endregion

			#region 追加料金
			/// <summary>
			/// 追加料金変更イベント
			/// </summary>
			public event EventHandler OnAddOnChargeChange = (sender, e) => { };
			/// <summary>
			/// 追加料金
			/// </summary>
			private int _addOnCharge = 0;
			public int AddOnCharge
			{
				get { return _addOnCharge; }
				set
				{
					if (_addOnCharge != value)
					{
						_addOnCharge = value;

						// 通知
						this.OnAddOnChargeChange(this, EventArgs.Empty);
					}
				}
			}

			/// <summary>
			/// 追加料金フォーマット変更イベント
			/// </summary>
			public event EventHandler OnAddOnChargeFormatChange = (sender, e) => { };
			/// <summary>
			/// 追加料金フォーマット
			/// </summary>
			private string _addOnChargeFormat = "";
			public string AddOnChargeFormat
			{
				get { return _addOnChargeFormat; }
				set
				{
					if (_addOnChargeFormat != value)
					{
						_addOnChargeFormat = value;

						// 通知
						this.OnAddOnChargeFormatChange(this, EventArgs.Empty);
					}
				}
			}
			#endregion

			#region キャラステータス
			/// <summary>
			/// ベースキャラのシンクロ合成残り回数変更イベント
			/// </summary>
			public event EventHandler OnBaseSynchroRemainChange = (sender, e) => { };
			/// <summary>
			/// ベースキャラのシンクロ合成残り回数
			/// </summary>
			private int _baseSynchroRemain = 0;
			public int BaseSynchroRemain
			{
				get { return _baseSynchroRemain; }
				set
				{
					if(_baseSynchroRemain != value)
					{
						_baseSynchroRemain = value;

						// 通知
						this.OnBaseSynchroRemainChange(this, EventArgs.Empty);
					}
				}
			}

			/// <summary>
			/// ベースキャラのランク変更イベント
			/// </summary>
			public event EventHandler OnBaseRankChange = (sender, e) => { };
			/// <summary>
			/// ベースキャラのランク
			/// </summary>
			private int _baseRank = 0;
			public int BaseRank
			{
				get { return _baseRank; }
				set
				{
					if (_baseRank != value)
					{
						_baseRank = value;

						// 通知
						this.OnBaseRankChange(this, EventArgs.Empty);
					}
				}
			}

			/// <summary>
			/// 進化キャラのシンクロ合成残り回数変更イベント
			/// </summary>
			public event EventHandler OnEvolutionSynchroRemainChange = (sender, e) => { };
			/// <summary>
			/// 進化キャラのシンクロ合成残り回数
			/// </summary>
			private int _evolutionSynchroRemain = 0;
			public int EvolutionSynchroRemain
			{
				get { return _evolutionSynchroRemain; }
				set
				{
					if (_evolutionSynchroRemain != value)
					{
						_evolutionSynchroRemain = value;

						// 通知
						this.OnEvolutionSynchroRemainChange(this, EventArgs.Empty);
					}
				}
			}

			/// <summary>
			/// 進化キャラのランク変更イベント
			/// </summary>
			public event EventHandler OnEvolutionRankChange = (sender, e) => { };
			/// <summary>
			/// 進化キャラのランク
			/// </summary>
			private int _evolutionRank = 0;
			public int EvolutionRank
			{
				get { return _evolutionRank; }
				set
				{
					if (_evolutionRank != value)
					{
						_evolutionRank = value;

						// 通知
						this.OnEvolutionRankChange(this, EventArgs.Empty);
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
			#endregion
		}
	}
}