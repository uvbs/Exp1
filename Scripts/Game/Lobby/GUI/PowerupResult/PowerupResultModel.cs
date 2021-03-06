/// <summary>
/// 強化合成結果データ
/// 
/// 2016/01/28
/// </summary>
using System;

namespace XUI
{
	namespace PowerupResult
	{
		/// <summary>
		/// 強化合成結果データインターフェイス
		/// </summary>
		public interface IModel
		{
			#region アバタータイプ
			/// <summary>
			/// アバタータイプ変更イベント
			/// </summary>
			event EventHandler OnAvatarTypeChange;

			/// <summary>
			/// アバタータイプ(character)
			/// </summary>
			AvatarType AvatarType { get; set; }
            /// <summary>
            /// Skin Id(avatar)
            /// </summary>
            int SkinId { get; set; }
            #endregion

            #region レベル
            /// <summary>
            /// 合成前のレベル
            /// </summary>
            int BeforeLv { get; }
			/// <summary>
			/// 合成後のレベル
			/// </summary>
			int AfterLv { get; }

			/// <summary>
			/// レベル変更イベント
			/// </summary>
			event EventHandler OnLvChange;
			/// <summary>
			/// レベル設定
			/// </summary>
			void SetLv(int beforeLv, int afterLv);

			/// <summary>
			/// 合成前のレベルフォーマット変更イベント
			/// </summary>
			event EventHandler OnBeforeLvFormatChange;
			/// <summary>
			/// 合成前のレベルフォーマット
			/// </summary>
			string BeforeLvFormat { get; set; }

			/// <summary>
			/// 合成後のレベルフォーマット変更イベント
			/// </summary>
			event EventHandler OnAfterLvFormatChange;
			/// <summary>
			/// 合成後のレベルフォーマット
			/// </summary>
			string AfterLvFormat { get; set; }
			#endregion

			#region 経験値バー
			/// <summary>
			/// 現在の累積経験値
			/// </summary>
			int Exp { get; }
			/// <summary>
			/// 現在のレベルになる為の累積経験値
			/// </summary>
			int TotalExp { get; }
			/// <summary>
			/// 次のレベルになる為の累積経験値
			/// </summary>
			int NextLvTotalExp { get; }

			/// <summary>
			/// 経験値バー変更イベント
			/// </summary>
			event EventHandler OnExpSliderChange;
			/// <summary>
			/// 経験値バー設定
			/// </summary>
			void SetExpSlider(int exp, int totalExp, int nextLvTotalExp);
			/// <summary>
			/// 経験値バーの値を取得する
			/// </summary>
			float GetExpSlider();
			#endregion

			#region 生命力
			/// <summary>
			/// 合成後の生命力
			/// </summary>
			int HitPoint { get; }
			/// <summary>
			/// レベルアップ時の生命力のアップ分
			/// </summary>
			int HitPointUp { get; }

			/// <summary>
			/// 生命力変更イベント
			/// </summary>
			event EventHandler OnHitPointChange;
			/// <summary>
			/// 生命力設定
			/// </summary>
			void SetHitPoint(int hp, int up);

			/// <summary>
			/// 合成後の生命力フォーマット変更イベント
			/// </summary>
			event EventHandler OnHitPointFormatChange;
			/// <summary>
			/// 合成後の生命力フォーマット
			/// </summary>
			string HitPointFormat { get; set; }

			/// <summary>
			/// レベルアップ時の生命力のアップ分フォーマット変更イベント
			/// </summary>
			event EventHandler OnHitPointUpFormatChange;
			/// <summary>
			///  レベルアップ時の生命力のアップ分フォーマット
			/// </summary>
			string HitPointUpFormat { get; set; }
			#endregion

			#region 攻撃力
			/// <summary>
			/// 合成後の攻撃力
			/// </summary>
			int Attack { get; }
			/// <summary>
			/// レベルアップ時の攻撃力のアップ分
			/// </summary>
			int AttackUp { get; }

			/// <summary>
			/// 攻撃力変更イベント
			/// </summary>
			event EventHandler OnAttackChange;
			/// <summary>
			/// 攻撃力設定
			/// </summary>
			void SetAttack(int atk, int up);

			/// <summary>
			/// 合成後の攻撃力フォーマット変更イベント
			/// </summary>
			event EventHandler OnAttackFormatChange;
			/// <summary>
			/// 合成後の攻撃力フォーマット
			/// </summary>
			string AttackFormat { get; set; }

			/// <summary>
			/// レベルアップ時の攻撃力のアップ分フォーマット変更イベント
			/// </summary>
			event EventHandler OnAttackUpFormatChange;
			/// <summary>
			///  レベルアップ時の攻撃力のアップ分フォーマット
			/// </summary>
			string AttackUpFormat { get; set; }
			#endregion

			#region 防御力
			/// <summary>
			/// 合成後の防御力
			/// </summary>
			int Defence { get; }
			/// <summary>
			/// レベルアップ時の防御力のアップ分
			/// </summary>
			int DefenceUp { get; }

			/// <summary>
			/// 防御力変更イベント
			/// </summary>
			event EventHandler OnDefenceChange;
			/// <summary>
			/// 防御力設定
			/// </summary>
			void SetDefence(int def, int up);

			/// <summary>
			/// 合成後の防御力フォーマット変更イベント
			/// </summary>
			event EventHandler OnDefenceFormatChange;
			/// <summary>
			/// 合成後の防御力フォーマット
			/// </summary>
			string DefenceFormat { get; set; }

			/// <summary>
			/// レベルアップ時の防御力のアップ分フォーマット変更イベント
			/// </summary>
			event EventHandler OnDefenceUpFormatChange;
			/// <summary>
			///  レベルアップ時の防御力のアップ分フォーマット
			/// </summary>
			string DefenceUpFormat { get; set; }
			#endregion

			#region 特殊能力
			/// <summary>
			/// 合成後の特殊能力
			/// </summary>
			int Extra { get; }
			/// <summary>
			/// レベルアップ時の特殊能力のアップ分
			/// </summary>
			int ExtraUp { get; }

			/// <summary>
			/// 特殊能力変更イベント
			/// </summary>
			event EventHandler OnExtraChange;
			/// <summary>
			/// 特殊能力設定
			/// </summary>
			void SetExtra(int def, int up);

			/// <summary>
			/// 合成後の特殊能力フォーマット変更イベント
			/// </summary>
			event EventHandler OnExtraFormatChange;
			/// <summary>
			/// 合成後の特殊能力フォーマット
			/// </summary>
			string ExtraFormat { get; set; }

			/// <summary>
			/// レベルアップ時の特殊能力のアップ分フォーマット変更イベント
			/// </summary>
			event EventHandler OnExtraUpFormatChange;
			/// <summary>
			///  レベルアップ時の特殊能力のアップ分フォーマット
			/// </summary>
			string ExtraUpFormat { get; set; }
			#endregion
		}

		/// <summary>
		/// 強化合成結果データ
		/// </summary>
		public class Model : IModel
		{
			#region アバタータイプ
			/// <summary>
			/// アバタータイプ変更イベント
			/// </summary>
			public event EventHandler OnAvatarTypeChange = (sender, e) => { };

			/// <summary>
			/// アバタータイプ
			/// </summary>
			AvatarType _avatarType = AvatarType.None;
			public AvatarType AvatarType
			{
				get
				{
					return _avatarType;
				}
				set
				{
					if (_avatarType != value)
					{
						_avatarType = value;

						// 通知
						var eventArgs = new EventArgs();
						this.OnAvatarTypeChange(this, eventArgs);
					}
				}
			}

            int _skinId = 0;
            public int SkinId {
                get {
                    return _skinId;
                }
                set {
                    if (_skinId != value) {
                        _skinId = value;

                        // 通知
                        var eventArgs = new EventArgs();
                        this.OnAvatarTypeChange(this, eventArgs);
                    }
                }
            }
			#endregion

			#region レベル
			/// <summary>
			/// 合成前のレベル
			/// </summary>
			int _beforeLv = 0;
			public int BeforeLv { get { return _beforeLv; } private set { _beforeLv = value; } }
			/// <summary>
			/// 合成後のレベル
			/// </summary>
			int _afterLv = 0;
			public int AfterLv { get { return _afterLv; } private set { _afterLv = value; } }

			/// <summary>
			/// レベル変更イベント
			/// </summary>
			public event EventHandler OnLvChange = (sender, e) => { };
			/// <summary>
			/// レベル設定
			/// </summary>
			public void SetLv(int beforeLv, int afterLv)
			{
				if (this.BeforeLv != beforeLv || this.AfterLv != afterLv)
				{
					this.BeforeLv = beforeLv;
					this.AfterLv = afterLv;

					// 通知
					var eventArgs = new EventArgs();
					this.OnLvChange(this, eventArgs);
				}
			}

			/// <summary>
			/// 合成前のレベルフォーマット変更イベント
			/// </summary>
			public event EventHandler OnBeforeLvFormatChange = (sender, e) => { };
			/// <summary>
			/// 合成前のレベルフォーマット
			/// </summary>
			string _beforeLvFormat = "";
			public string BeforeLvFormat
			{
				get { return _beforeLvFormat; }
				set
				{
					if (_beforeLvFormat != value)
					{
						_beforeLvFormat = value;

						// 通知
						var eventArgs = new EventArgs();
						this.OnBeforeLvFormatChange(this, eventArgs);
					}
				}
			}

			/// <summary>
			/// 合成後のレベルフォーマット変更イベント
			/// </summary>
			public event EventHandler OnAfterLvFormatChange = (sender, e) => { };
			/// <summary>
			/// 合成後のレベルフォーマット
			/// </summary>
			string _afterLvFormat = "";
			public string AfterLvFormat
			{
				get { return _afterLvFormat; }
				set
				{
					if (_afterLvFormat != value)
					{
						_afterLvFormat = value;

						// 通知
						var eventArgs = new EventArgs();
						this.OnAfterLvFormatChange(this, eventArgs);
					}
				}
			}
			#endregion

			#region 経験値バー
			/// <summary>
			/// 現在の累積経験値
			/// </summary>
			int _exp = 0;
			public int Exp { get { return _exp; } private set { _exp = value; } }
			/// <summary>
			/// 現在のレベルになる為の累積経験値
			/// </summary>
			int _totalExp = 0;
			public int TotalExp { get { return _totalExp; } private set { _totalExp = value; } }
			/// <summary>
			/// 次のレベルになる為の累積経験値
			/// </summary>
			int _nextLvTotalExp = 0;
			public int NextLvTotalExp { get { return _nextLvTotalExp; } private set { _nextLvTotalExp = value; } }

			/// <summary>
			/// 経験値バー変更イベント
			/// </summary>
			public event EventHandler OnExpSliderChange = (sender, e) => { };
			/// <summary>
			/// 経験値バー設定
			/// </summary>
			public void SetExpSlider(int exp, int totalExp, int nextLvTotalExp)
			{
				if (this.Exp != exp || this.TotalExp != totalExp || this.NextLvTotalExp != nextLvTotalExp)
				{
					this.Exp = exp;
					this.TotalExp = totalExp;
					this.NextLvTotalExp = nextLvTotalExp;

					// 通知
					var eventArgs = new EventArgs();
					this.OnExpSliderChange(this, eventArgs);
				}
			}
			/// <summary>
			/// 経験値バーの値を取得する
			/// </summary>
			public float GetExpSlider()
			{
				return XUI.Powerup.Util.GetLerpValue((float)this.TotalExp, (float)this.NextLvTotalExp, (float)this.Exp);
			}
			#endregion

			#region 生命力
			/// <summary>
			/// 合成後の生命力
			/// </summary>
			int _hitPoint = 0;
			public int HitPoint { get { return _hitPoint; } private set { _hitPoint = value; } }
			/// <summary>
			/// レベルアップ時の生命力のアップ分
			/// </summary>
			int _hitPointUp = 0;
			public int HitPointUp { get { return _hitPointUp; } private set { _hitPointUp = value; } }

			/// <summary>
			/// 生命力変更イベント
			/// </summary>
			public event EventHandler OnHitPointChange = (sender, e) => { };
			/// <summary>
			/// 生命力設定
			/// </summary>
			public void SetHitPoint(int hp, int up)
			{
				if (this.HitPoint != hp || this.HitPointUp != up)
				{
					this.HitPoint = hp;
					this.HitPointUp = up;

					// 通知
					var eventArgs = new EventArgs();
					this.OnHitPointChange(this, eventArgs);
				}
			}

			/// <summary>
			/// 合成後の生命力フォーマット変更イベント
			/// </summary>
			public event EventHandler OnHitPointFormatChange = (sender, e) => { };
			/// <summary>
			/// 合成後の生命力フォーマット
			/// </summary>
			string _hitPointFormat = "";
			public string HitPointFormat
			{
				get { return _hitPointFormat; }
				set
				{
					if (_hitPointFormat != value)
					{
						_hitPointFormat = value;

						// 通知
						var eventArgs = new EventArgs();
						this.OnHitPointFormatChange(this, eventArgs);
					}
				}
			}

			/// <summary>
			/// レベルアップ時の生命力のアップ分フォーマット変更イベント
			/// </summary>
			public event EventHandler OnHitPointUpFormatChange = (sender, e) => { };
			/// <summary>
			///  レベルアップ時の生命力のアップ分フォーマット
			/// </summary>
			string _hitPointUpFormat = "";
			public string HitPointUpFormat
			{
				get { return _hitPointUpFormat; }
				set
				{
					if (_hitPointUpFormat != value)
					{
						_hitPointUpFormat = value;

						// 通知
						var eventArgs = new EventArgs();
						this.OnHitPointUpFormatChange(this, eventArgs);
					}
				}
			}
			#endregion

			#region 攻撃力
			/// <summary>
			/// 合成後の攻撃力
			/// </summary>
			int _attack = 0;
			public int Attack { get { return _attack; } private set { _attack = value; } }
			/// <summary>
			/// レベルアップ時の攻撃力のアップ分
			/// </summary>
			int _attackUp = 0;
			public int AttackUp { get { return _attackUp; } private set { _attackUp = value; } }

			/// <summary>
			/// 攻撃力変更イベント
			/// </summary>
			public event EventHandler OnAttackChange = (sender, e) => { };
			/// <summary>
			/// 攻撃力設定
			/// </summary>
			public void SetAttack(int atk, int up)
			{
				if (this.Attack != atk || this.AttackUp != up)
				{
					this.Attack = atk;
					this.AttackUp = up;

					// 通知
					var eventArgs = new EventArgs();
					this.OnAttackChange(this, eventArgs);
				}
			}

			/// <summary>
			/// 合成後の攻撃力フォーマット変更イベント
			/// </summary>
			public event EventHandler OnAttackFormatChange = (sender, e) => { };
			/// <summary>
			/// 合成後の攻撃力フォーマット
			/// </summary>
			string _attackFormat = "";
			public string AttackFormat
			{
				get { return _attackFormat; }
				set
				{
					if (_attackFormat != value)
					{
						_attackFormat = value;

						// 通知
						var eventArgs = new EventArgs();
						this.OnAttackFormatChange(this, eventArgs);
					}
				}
			}

			/// <summary>
			/// レベルアップ時の攻撃力のアップ分フォーマット変更イベント
			/// </summary>
			public event EventHandler OnAttackUpFormatChange = (sender, e) => { };
			/// <summary>
			///  レベルアップ時の攻撃力のアップ分フォーマット
			/// </summary>
			string _attackUpFormat = "";
			public string AttackUpFormat
			{
				get { return _attackUpFormat; }
				set
				{
					if (_attackUpFormat != value)
					{
						_attackUpFormat = value;

						// 通知
						var eventArgs = new EventArgs();
						this.OnAttackUpFormatChange(this, eventArgs);
					}
				}
			}
			#endregion

			#region 防御力
			/// <summary>
			/// 合成後の防御力
			/// </summary>
			int _defence = 0;
			public int Defence { get { return _defence; } private set { _defence = value; } }
			/// <summary>
			/// レベルアップ時の防御力のアップ分
			/// </summary>
			int _defenceUp = 0;
			public int DefenceUp { get { return _defenceUp; } private set { _defenceUp = value; } }

			/// <summary>
			/// 防御力変更イベント
			/// </summary>
			public event EventHandler OnDefenceChange = (sender, e) => { };
			/// <summary>
			/// 防御力設定
			/// </summary>
			public void SetDefence(int def, int up)
			{
				if (this.Defence != def || this.DefenceUp != up)
				{
					this.Defence = def;
					this.DefenceUp = up;

					// 通知
					var eventArgs = new EventArgs();
					this.OnDefenceChange(this, eventArgs);
				}
			}

			/// <summary>
			/// 合成後の防御力フォーマット変更イベント
			/// </summary>
			public event EventHandler OnDefenceFormatChange = (sender, e) => { };
			/// <summary>
			/// 合成後の防御力フォーマット
			/// </summary>
			string _defenceFormat = "";
			public string DefenceFormat
			{
				get { return _defenceFormat; }
				set
				{
					if (_defenceFormat != value)
					{
						_defenceFormat = value;

						// 通知
						var eventArgs = new EventArgs();
						this.OnDefenceFormatChange(this, eventArgs);
					}
				}
			}

			/// <summary>
			/// レベルアップ時の防御力のアップ分フォーマット変更イベント
			/// </summary>
			public event EventHandler OnDefenceUpFormatChange = (sender, e) => { };
			/// <summary>
			///  レベルアップ時の防御力のアップ分フォーマット
			/// </summary>
			string _defenceUpFormat = "";
			public string DefenceUpFormat
			{
				get { return _defenceUpFormat; }
				set
				{
					if (_defenceUpFormat != value)
					{
						_defenceUpFormat = value;

						// 通知
						var eventArgs = new EventArgs();
						this.OnDefenceUpFormatChange(this, eventArgs);
					}
				}
			}
			#endregion

			#region 特殊能力
			/// <summary>
			/// 合成後の特殊能力
			/// </summary>
			int _extra = 0;
			public int Extra { get { return _extra; } private set { _extra = value; } }
			/// <summary>
			/// レベルアップ時の特殊能力のアップ分
			/// </summary>
			int _extraUp = 0;
			public int ExtraUp { get { return _extraUp; } private set { _extraUp = value; } }

			/// <summary>
			/// 特殊能力変更イベント
			/// </summary>
			public event EventHandler OnExtraChange = (sender, e) => { };
			/// <summary>
			/// 特殊能力設定
			/// </summary>
			public void SetExtra(int def, int up)
			{
				if (this.Extra != def || this.ExtraUp != up)
				{
					this.Extra = def;
					this.ExtraUp = up;

					// 通知
					var eventArgs = new EventArgs();
					this.OnExtraChange(this, eventArgs);
				}
			}

			/// <summary>
			/// 合成後の特殊能力フォーマット変更イベント
			/// </summary>
			public event EventHandler OnExtraFormatChange = (sender, e) => { };
			/// <summary>
			/// 合成後の特殊能力フォーマット
			/// </summary>
			string _extraFormat = "";
			public string ExtraFormat
			{
				get { return _extraFormat; }
				set
				{
					if (_extraFormat != value)
					{
						_extraFormat = value;

						// 通知
						var eventArgs = new EventArgs();
						this.OnExtraFormatChange(this, eventArgs);
					}
				}
			}

			/// <summary>
			/// レベルアップ時の特殊能力のアップ分フォーマット変更イベント
			/// </summary>
			public event EventHandler OnExtraUpFormatChange = (sender, e) => { };
			/// <summary>
			///  レベルアップ時の特殊能力のアップ分フォーマット
			/// </summary>
			string _extraUpFormat = "";
			public string ExtraUpFormat
			{
				get { return _extraUpFormat; }
				set
				{
					if (_extraUpFormat != value)
					{
						_extraUpFormat = value;

						// 通知
						var eventArgs = new EventArgs();
						this.OnExtraUpFormatChange(this, eventArgs);
					}
				}
			}
			#endregion
		}
	}
}
