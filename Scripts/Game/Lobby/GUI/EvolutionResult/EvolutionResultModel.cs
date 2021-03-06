/// <summary>
/// 進化合成結果データ
/// 
/// 2016/03/03
/// </summary>
using System;

namespace XUI.EvolutionResult
{
	/// <summary>
	/// 進化合成結果データインターフェイス
	/// </summary>
	public interface IModel
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();
		#endregion

		#region アバタータイプ
		/// <summary>
		/// アバタータイプ変更イベント
		/// </summary>
		event EventHandler OnAvatarTypeChange;

		/// <summary>
		/// アバタータイプ
		/// </summary>
		AvatarType AvatarType { get; set; }

        /// <summary>
        /// SkinId
        /// </summary>
        int SkinId { get; set; }
		#endregion

		#region ランク
		/// <summary>
		/// 合成前のランク
		/// </summary>
		int BeforeRank { get; }
		/// <summary>
		/// 合成後のランク
		/// </summary>
		int AfterRank { get; }

		/// <summary>
		/// ランク変更イベント
		/// </summary>
		event EventHandler OnRankChange;
		/// <summary>
		/// ランク設定
		/// </summary>
		void SetRank(int beforeRank, int afterRank);
		#endregion

		#region シンクロ残り合成回数
		/// <summary>
		/// シンクロ残り合成回数
		/// </summary>
		int SynchroRemain { get; }
		/// <summary>
		/// シンクロ残り合成回数アップ分
		/// </summary>
		int SynchroRemainUp { get; }

		/// <summary>
		/// シンクロ残り合成回数変更イベント
		/// </summary>
		event EventHandler OnSynchroRemainChange;
		/// <summary>
		/// シンクロ残り合成回数設定
		/// </summary>
		void SetSynchroRemain(int synchroRemain, int up);

		/// <summary>
		/// シンクロ残り合成回数アップフォーマット変更イベント
		/// </summary>
		event EventHandler OnSynchroRemainUpFormatChange;
		/// <summary>
		/// シンクロ残り合成回数アップ分フォーマット
		/// </summary>
		string SynchroRemainUpFormat { set; get; }
		#endregion

		#region 強化レベル
		/// <summary>
		/// 強化レベル変更イベント
		/// </summary>
		event EventHandler OnLvChange;
		/// <summary>
		/// 強化レベル
		/// </summary>
		int Lv { get; set; }

		/// <summary>
		/// 強化レベルフォーマット変更イベント
		/// </summary>
		event EventHandler OnLvFormatChange;
		/// <summary>
		/// 強化レベルフォーマット
		/// </summary>
		string LvFormat { get; set; }
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
		event EventHandler OnDefenceChnage;
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
	}

	/// <summary>
	/// 進化合成結果データ
	/// </summary>
	public class Model : IModel
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			this.OnAvatarTypeChange = null;
			this.OnRankChange = null;
			this.OnSynchroRemainChange = null;
			this.OnSynchroRemainUpFormatChange = null;
			this.OnLvChange = null;
			this.OnLvFormatChange = null;
			this.OnHitPointChange = null;
			this.OnHitPointFormatChange = null;
			this.OnAttackChange = null;
			this.OnAttackFormatChange = null;
			this.OnDefenceChnage = null;
			this.OnDefenceFormatChange = null;
			this.OnExtraChange = null;
			this.OnExtraFormatChange = null;
		}
		#endregion

		#region アバタータイプ
		/// <summary>
		/// アバタータイプ変更イベント
		/// </summary>
		public event EventHandler OnAvatarTypeChange = (sender, e) => { };

		private AvatarType _avatarType = AvatarType.None;
		public AvatarType AvatarType
		{
			get { return _avatarType; }
			set
			{
				if(_avatarType != value)
				{
					_avatarType = value;

					// 通知
					this.OnAvatarTypeChange(this, EventArgs.Empty);
				}
			}
		}

        private int _skinId = 0;
        public int SkinId {
            get {
                return _skinId;
            }
            set {
                if (_skinId != value) {
                    _skinId = value;

                    // 通知
                    this.OnAvatarTypeChange(this, EventArgs.Empty);
                }
            }
        }
		#endregion

		#region ランク
		/// <summary>
		/// 合成前のランク
		/// </summary>
		private int _beforeRank = 0;
		public int BeforeRank { get { return _beforeRank; } private set { _beforeRank = value; } }
		/// <summary>
		/// 合成後のランク
		/// </summary>
		private int _afterRank = 0;
		public int AfterRank { get { return _afterRank; } private set { _afterRank = value; } }

		/// <summary>
		/// ランク変更イベント
		/// </summary>
		public event EventHandler OnRankChange = (sender, e) => { };
		/// <summary>
		/// ランク設定
		/// </summary>
		public void SetRank(int beforeRank, int afterRank)
		{
			if(this.BeforeRank != beforeRank || this.AfterRank != afterRank)
			{
				this.BeforeRank = beforeRank;
				this.AfterRank = afterRank;

				// 通知
				this.OnRankChange(this, EventArgs.Empty);
			}
		}
		#endregion

		#region シンクロ合成残り回数
		/// <summary>
		/// シンクロ合成残り回数
		/// </summary>
		private int _synchroRemain = 0;
		public int SynchroRemain { get { return _synchroRemain; } private set { _synchroRemain = value; } }

		/// <summary>
		/// シンクロ合成残り回数アップ分
		/// </summary>
		private int _synchroRemainUp = 0;
		public int SynchroRemainUp { get { return _synchroRemainUp; } private set { _synchroRemainUp = value; } }

		/// <summary>
		/// シンクロ合成残り回数変更イベント
		/// </summary>
		public event EventHandler OnSynchroRemainChange = (sender, e) => { };
		public void SetSynchroRemain(int synchroRemain, int up)
		{
			if(this.SynchroRemain != synchroRemain || this.SynchroRemainUp != up)
			{
				this.SynchroRemain = synchroRemain;
				this.SynchroRemainUp = up;

				// 通知
				this.OnSynchroRemainChange(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// シンクロ合成残り回数アップ分フォーマット変更イベント
		/// </summary>
		public event EventHandler OnSynchroRemainUpFormatChange = (sender, e) => { };
		string _synchroRemainUpFormat = "";
		public string SynchroRemainUpFormat
		{
			get { return _synchroRemainUpFormat; }
			set
			{
				if (_synchroRemainUpFormat != value)
				{
					_synchroRemainUpFormat = value;

					// 通知
					this.OnSynchroRemainUpFormatChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region 強化レベル
		/// <summary>
		/// 強化レベル変更イベント
		/// </summary>
		public event EventHandler OnLvChange = (sender, e) => { };
		/// <summary>
		/// 強化レベル
		/// </summary>
		private int _lv = 0;
		public int Lv
		{
			get { return _lv; }
			set
			{
				if(_lv != value)
				{
					_lv = value;

					// 通知
					this.OnLvChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// 強化レベルフォーマット変更イベント
		/// </summary>
		public event EventHandler OnLvFormatChange = (sender, e) => { };
		/// <summary>
		/// 強化レベルフォーマット
		/// </summary>
		private string _lvFormat = "";
		public string LvFormat
		{
			get { return _lvFormat; }
			set
			{
				if(_lvFormat != value)
				{
					_lvFormat = value;

					// 通知
					this.OnLvFormatChange(this, EventArgs.Empty);
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
		public event EventHandler OnDefenceChnage = (sender, e) => { };
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
					this.OnDefenceChnage(this, EventArgs.Empty);
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
					this.OnExtraChange(this, EventArgs.Empty);
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
					this.OnExtraFormatChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion
	}
}