/// <summary>
/// 強化合成データ
/// 
/// 2016/01/08
/// </summary>
using System;

namespace XUI
{
	namespace Powerup
	{
		public class TakeExpChangeEventArgs : EventArgs { }
		public class TakeExpFormatChangeEventArgs : EventArgs { }

		public class ExpChangeEventArgs : EventArgs { }
		public class ExpFormatChangeEventArgs : EventArgs { }

		public class BeforeLvDataChangeEventArgs : EventArgs { }
		public class LvFormatChangeEventArgs : EventArgs { }
		public class TotalExpFormatChangeEventArgs : EventArgs { }
		public class NextLvTotalExpFormatChangeEventArgs : EventArgs { }
		public class NextLvExpFormatChangeEventArgs : EventArgs { }

		public class AfterExpChangeEventArgs : EventArgs { }
		public class AfterExpFormatChangeEventArgs : EventArgs { }

		public class AfterLvDataChangeEventArgs : EventArgs { }
		public class AfterLvFormatChangeEventArgs : EventArgs { }
		public class AfterTotalExpFormatChangeEventArgs : EventArgs { }
		public class AfterNextLvTotalExpFormatChangeEventArgs : EventArgs { }
		public class AfterNextLvExpFormatChangeEventArgs : EventArgs { }

		public class AfterOverflowExpChangeEventArgs : EventArgs { }
		public class AfterOverflowExpFormatChangeEventArgs : EventArgs { }

		public class HaveMoneyChangeEventArgs : EventArgs { }
		public class HaveMoneyFormatChangeEventArgs : EventArgs { }

		public class NeedMoneyChangeEventArgs : EventArgs { }
		public class NeedMoneyFormatChangeEventArgs : EventArgs { }

		public class AddOnChargeChangeEventArgs : EventArgs { }
		public class AddOnChargeFormatChangeEventArgs : EventArgs { }

		/// <summary>
		/// 強化合成データインターフェイス
		/// </summary>
		public interface IModel
		{
			#region 破棄
			/// <summary>
			/// 破棄
			/// </summary>
			void Dispose();
			#endregion

			#region 獲得経験値
			/// <summary>
			/// 獲得経験値変更イベント
			/// </summary>
			event EventHandler<TakeExpChangeEventArgs> OnTakeExpChange;
			/// <summary>
			/// 獲得経験値
			/// </summary>
			int TakeExp { get; set; }

			/// <summary>
			/// 獲得経験値フォーマット変更イベント
			/// </summary>
			event EventHandler<TakeExpFormatChangeEventArgs> OnTakeExpFormatChange;
			/// <summary>
			/// 獲得経験値フォーマット
			/// </summary>
			string TakeExpFormat { get; set; }
			#endregion

			#region 現在のレベルデータ
			#region 経験値
			/// <summary>
			/// 現在の累積経験値変更イベント
			/// </summary>
			event EventHandler<ExpChangeEventArgs> OnExpChange;
			/// <summary>
			/// 現在の累積経験値
			/// </summary>
			int Exp { get; set; }

			/// <summary>
			/// 現在の累積経験値フォーマット変更イベント
			/// </summary>
			event EventHandler<ExpFormatChangeEventArgs> OnExpFormatChange;
			/// <summary>
			/// 現在の累積経験値フォーマット
			/// </summary>
			string ExpFormat { get; set; }
			#endregion

			#region 合成前のレベルデータ設定
			/// <summary>
			/// 合成前のレベルデータ変更イベント
			/// </summary>
			event EventHandler<BeforeLvDataChangeEventArgs> OnBeforeLvDataChange;
			/// <summary>
			/// 合成前のレベルデータを設定する
			/// </summary>
			void SetBeforeLvData(int lv, int thisLvTotalExp, int nextLvTotalExp);
			#endregion

			#region レベル
			/// <summary>
			/// 現在のレベル
			/// </summary>
			int Lv { get; }

			/// <summary>
			/// 現在のレベルフォーマット変更イベント
			/// </summary>
			event EventHandler<LvFormatChangeEventArgs> OnLvFormatChange;
			/// <summary>
			/// 現在のレベルフォーマット
			/// </summary>
			string LvFormat { get; set; }
			#endregion

			#region 現在のレベルになる為の累積経験値
			/// <summary>
			/// 現在のレベルになる為の累積経験値
			/// </summary>
			int TotalExp { get; }

			/// <summary>
			/// 現在のレベルになる為の累積経験値フォーマット変更イベント
			/// </summary>
			event EventHandler<TotalExpFormatChangeEventArgs> OnTotalExpFormatChange;
			/// <summary>
			/// 現在のレベルになる為の累積経験値フォーマット
			/// </summary>
			string TotalExpFormat { get; set; }
			#endregion

			#region 次のレベルになる為の累積経験値
			/// <summary>
			/// 次のレベルになる為の累積経験値
			/// </summary>
			int NextLvTotalExp { get; }

			/// <summary>
			/// 次のレベルになる為の累積経験値フォーマット変更イベント
			/// </summary>
			event EventHandler<NextLvTotalExpFormatChangeEventArgs> OnNextLvTotalExpFormatChange;
			/// <summary>
			/// 次のレベルになる為の累積経験値フォーマット
			/// </summary>
			string NextLvTotalExpFormat { get; set; }
			#endregion

			#region 次のレベルまでの経験値
			/// <summary>
			/// 次のレベルまでの経験値を取得する
			/// </summary>
			int GetNextLvExp();

			/// <summary>
			/// 次のレベルまでの経験値フォーマット変更イベント
			/// </summary>
			event EventHandler<NextLvExpFormatChangeEventArgs> OnNextLvExpFormatChange;
			/// <summary>
			/// 次のレベルまでの経験値フォーマット
			/// </summary>
			string NextLvExpFormat { get; set; }
			#endregion
			#endregion

			#region 合成後のレベルデータ
			#region 合成後の経験値
			/// <summary>
			/// 合成後の累積経験値変更イベント
			/// </summary>
			event EventHandler<AfterExpChangeEventArgs> OnAfterExpChange;
			/// <summary>
			/// 合成後の累積経験値
			/// </summary>
			int AfterExp { get; set; }

			/// <summary>
			/// 合成後の累積経験値フォーマット変更イベント
			/// </summary>
			event EventHandler<AfterExpFormatChangeEventArgs> OnAfterExpFormatChange;
			/// <summary>
			/// 合成後の累積経験値フォーマット
			/// </summary>
			string AfterExpFormat { get; set; }
			#endregion

			#region 合成後のレベルデータ設定
			/// <summary>
			/// 合成後のレベルデータ変更イベント
			/// </summary>
			event EventHandler<AfterLvDataChangeEventArgs> OnAfterLvDataChange;
			/// <summary>
			/// 合成後のレベルデータを設定する
			/// </summary>
			void SetAfterLvData(int lv, int thisLvTotalExp, int nextLvTotalExp);
			#endregion

			#region 合成後のレベル
			/// <summary>
			/// 合成後のレベル
			/// </summary>
			int AfterLv { get; }

			/// <summary>
			/// 合成後のレベルフォーマット変更イベント
			/// </summary>
			event EventHandler<AfterLvFormatChangeEventArgs> OnAfterLvFormatChange;
			/// <summary>
			/// 合成後のレベルフォーマット
			/// </summary>
			string AfterLvFormat { get; set; }
			#endregion

			#region 合成後のレベルになる為の累積経験値
			/// <summary>
			/// 合成後のレベルになる為の累積経験値
			/// </summary>
			int AfterTotalExp { get; }

			/// <summary>
			/// 合成後のレベルになる為の累積経験値フォーマット変更イベント
			/// </summary>
			event EventHandler<AfterTotalExpFormatChangeEventArgs> OnAfterTotalExpFormatChange;
			/// <summary>
			/// 合成後のレベルになる為の累積経験値フォーマット
			/// </summary>
			string AfterTotalExpFormat { get; set; }
			#endregion

			#region 合成後の次のレベルになる為の累積経験値
			/// <summary>
			/// 合成後の次のレベルになる為の累積経験値
			/// </summary>
			int AfterNextLvTotalExp { get; }

			/// <summary>
			/// 合成後の次のレベルになる為の累積経験値フォーマット変更イベント
			/// </summary>
			event EventHandler<AfterNextLvTotalExpFormatChangeEventArgs> OnAfterNextLvTotalExpFormatChange;
			/// <summary>
			/// 合成後の次のレベルになる為の累積経験値フォーマット
			/// </summary>
			string AfterNextLvTotalExpFormat { get; set; }
			#endregion

			#region 合成後の次のレベルまでの経験値
			/// <summary>
			/// 合成後の次のレベルまでの経験値を取得する
			/// </summary>
			int GetAfterNextLvExp();

			/// <summary>
			/// 次のレベルまでの経験値フォーマット変更イベント
			/// </summary>
			event EventHandler<AfterNextLvExpFormatChangeEventArgs> OnAfterNextLvExpFormatChange;
			/// <summary>
			/// 次のレベルまでの経験値フォーマット
			/// </summary>
			string AfterNextLvExpFormat { get; set; }
			#endregion

			#region 合成後の余剰経験値
			/// <summary>
			/// 合成後の余剰経験値変更イベント
			/// </summary>
			event EventHandler<AfterOverflowExpChangeEventArgs> OnAfterOverflowExpChange;
			/// <summary>
			/// 合成後の余剰経験値
			/// </summary>
			int AfterOverflowExp { get; set; }

			/// <summary>
			/// 合成後の余剰経験値フォーマット変更イベント
			/// </summary>
			event EventHandler<AfterOverflowExpFormatChangeEventArgs> OnAfterOverflowExpFormatChange;
			/// <summary>
			/// 合成後の余剰経験値フォーマット
			/// </summary>
			string AfterOverflowExpFormat { get; set; }
			#endregion
			#endregion

			#region 所持金
			/// <summary>
			/// 所持金変更イベント
			/// </summary>
			event EventHandler<HaveMoneyChangeEventArgs> OnHaveMoneyChange;
			/// <summary>
			/// 所持金
			/// </summary>
			int HaveMoney { get; set; }

			/// <summary>
			/// 所持金フォーマット変更イベント
			/// </summary>
			event EventHandler<HaveMoneyFormatChangeEventArgs> OnHaveMoneyFormatChange;
			/// <summary>
			/// 所持金フォーマット
			/// </summary>
			string HaveMoneyFormat { get; set; }
			#endregion

			#region 費用
			/// <summary>
			/// 費用変更イベント
			/// </summary>
			event EventHandler<NeedMoneyChangeEventArgs> OnNeedMoneyChange;
			/// <summary>
			/// 費用
			/// </summary>
			int NeedMoney { get; set; }

			/// <summary>
			/// 費用フォーマット変更イベント
			/// </summary>
			event EventHandler<NeedMoneyFormatChangeEventArgs> OnNeedMoneyFormatChange;
			/// <summary>
			/// 費用フォーマット
			/// </summary>
			string NeedMoneyFormat { get; set; }
			#endregion

			#region 追加料金
			/// <summary>
			/// 追加料金変更イベント
			/// </summary>
			event EventHandler<AddOnChargeChangeEventArgs> OnAddOnChargeChange;
			/// <summary>
			/// 追加料金
			/// </summary>
			int AddOnCharge { get; set; }

			/// <summary>
			/// 追加料金フォーマット変更イベント
			/// </summary>
			event EventHandler<AddOnChargeFormatChangeEventArgs> OnAddOnChargeFormatChange;
			/// <summary>
			/// 追加料金フォーマット
			/// </summary>
			string AddOnChargeFormat { get; set; }
			#endregion

			#region 経験値バーの値
			/// <summary>
			/// 現在の経験値バーの値を取得する
			/// </summary>
			float GetExpSliderValue();

			/// <summary>
			/// 合成後の現在の経験値バーの値を取得する
			/// </summary>
			float GetAfterExpSliderValue();

			/// <summary>
			/// 合成後の獲得経験値バーの値を取得する
			/// </summary>
			float GetAfterTakeExpSliderValue();
			#endregion
		}

		/// <summary>
		/// 強化合成データ
		/// </summary>
		public class Model : IModel
		{
			#region 破棄
			/// <summary>
			/// 破棄
			/// </summary>
			public void Dispose()
			{
				this.OnTakeExpChange = null;
				this.OnTakeExpFormatChange = null;
				this.OnExpChange = null;
				this.OnExpFormatChange = null;
				this.OnBeforeLvDataChange = null;
				this.OnLvFormatChange = null;
				this.OnTotalExpFormatChange = null;
				this.OnNextLvTotalExpFormatChange = null;
				this.OnNextLvExpFormatChange = null;
				this.OnAfterExpChange = null;
				this.OnAfterExpFormatChange = null;
				this.OnAfterLvDataChange = null;
				this.OnAfterLvFormatChange = null;
				this.OnAfterTotalExpFormatChange = null;
				this.OnAfterNextLvTotalExpFormatChange = null;
				this.OnAfterNextLvExpFormatChange = null;
				this.OnAfterOverflowExpChange = null;
				this.OnAfterOverflowExpFormatChange = null;
				this.OnHaveMoneyChange = null;
				this.OnHaveMoneyFormatChange = null;
				this.OnNeedMoneyChange = null;
				this.OnNeedMoneyFormatChange = null;
				this.OnAddOnChargeChange = null;
				this.OnAddOnChargeFormatChange = null;
			}
			#endregion

			#region 獲得経験値
			/// <summary>
			/// 獲得経験値変更イベント
			/// </summary>
			public event EventHandler<TakeExpChangeEventArgs> OnTakeExpChange = (sender, e) => { };
			/// <summary>
			/// 獲得経験値
			/// </summary>
			int _takeExp = 0;
			public int TakeExp
			{
				get { return _takeExp; }
				set
				{
					if (_takeExp != value)
					{
						_takeExp = value;

						// 通知
						var eventArgs = new TakeExpChangeEventArgs();
						this.OnTakeExpChange(this, eventArgs);
					}
				}
			}

			/// <summary>
			/// 獲得経験値フォーマット変更イベント
			/// </summary>
			public event EventHandler<TakeExpFormatChangeEventArgs> OnTakeExpFormatChange = (sender, e) => { };
			/// <summary>
			/// 獲得経験値フォーマット
			/// </summary>
			string _takeExpFormat = "";
			public string TakeExpFormat
			{
				get { return _takeExpFormat; }
				set
				{
					if (_takeExpFormat != value)
					{
						_takeExpFormat = value;

						// 通知
						var eventArgs = new TakeExpFormatChangeEventArgs();
						this.OnTakeExpFormatChange(this, eventArgs);
					}
				}
			}
			#endregion

			#region 現在のレベルデータ
			#region 経験値
			/// <summary>
			/// 現在の累積経験値変更イベント
			/// </summary>
			public event EventHandler<ExpChangeEventArgs> OnExpChange = (sender, e) => { };
			/// <summary>
			/// 現在の累積経験値
			/// </summary>
			int _exp = 0;
			public int Exp
			{
				get { return _exp; }
				set
				{
					if (_exp != value)
					{
						_exp = value;

						// 通知
						var eventArgs = new ExpChangeEventArgs();
						this.OnExpChange(this, eventArgs);
					}
				}
			}

			/// <summary>
			/// 経験値フォーマット変更イベント
			/// </summary>
			public event EventHandler<ExpFormatChangeEventArgs> OnExpFormatChange = (sender, e) => { };
			/// <summary>
			/// 現在の累積経験値フォーマット
			/// </summary>
			string _expFormat = "";
			public string ExpFormat
			{
				get { return _expFormat; }
				set
				{
					if (_expFormat != value)
					{
						_expFormat = value;

						// 通知
						var eventArgs = new ExpFormatChangeEventArgs();
						this.OnExpFormatChange(this, eventArgs);
					}
				}
			}
			#endregion

			#region 合成前のレベルデータ設定
			/// <summary>
			/// 合成前のレベルデータ変更イベント
			/// </summary>
			public event EventHandler<BeforeLvDataChangeEventArgs> OnBeforeLvDataChange = (sender, e) => { };

			/// <summary>
			/// 合成前のレベルデータを設定する
			/// </summary>
			public void SetBeforeLvData(int lv, int thisLvTotalExp, int nextLvTotalExp)
			{
				if (this.Lv != lv || this.TotalExp != thisLvTotalExp || this.NextLvTotalExp != nextLvTotalExp)
				{
					this.Lv = lv;
					this.TotalExp = thisLvTotalExp;
					this.NextLvTotalExp = nextLvTotalExp;

					// 通知
					var eventArgs = new BeforeLvDataChangeEventArgs();
					this.OnBeforeLvDataChange(this, eventArgs);
				}
			}
			#endregion

			#region レベル
			/// <summary>
			/// 現在のレベル
			/// </summary>
			int _lv = 0;
			public int Lv { get { return _lv; } set { _lv = value; } }

			/// <summary>
			/// レベルフォーマット変更イベント
			/// </summary>
			public event EventHandler<LvFormatChangeEventArgs> OnLvFormatChange = (sender, e) => { };
			/// <summary>
			/// 現在のレベルフォーマット
			/// </summary>
			string _lvFormat = "";
			public string LvFormat
			{
				get { return _lvFormat; }
				set
				{
					if (_lvFormat != value)
					{
						_lvFormat = value;

						// 通知
						var eventArgs = new LvFormatChangeEventArgs();
						this.OnLvFormatChange(this, eventArgs);
					}
				}
			}
			#endregion

			#region 現在のレベルになる為の累積経験値
			/// <summary>
			/// 現在のレベルになる為の累積経験値
			/// </summary>
			int _totalExp = 0;
			public int TotalExp { get { return _totalExp; } private set { _totalExp = value; } }

			/// <summary>
			/// 現在のレベルになる為の累積経験値フォーマット変更イベント
			/// </summary>
			public event EventHandler<TotalExpFormatChangeEventArgs> OnTotalExpFormatChange = (sender, e) => { };
			/// <summary>
			/// 現在のレベルになる為の累積経験値フォーマット
			/// </summary>
			string _totalExpFormat = "";
			public string TotalExpFormat
			{
				get { return _totalExpFormat; }
				set
				{
					if (_totalExpFormat != value)
					{
						_totalExpFormat = value;

						// 通知
						var eventArgs = new TotalExpFormatChangeEventArgs();
						this.OnTotalExpFormatChange(this, eventArgs);
					}
				}
			}
			#endregion

			#region 次のレベルになる為の累積経験値
			/// <summary>
			/// 次のレベルになる為の累積経験値
			/// </summary>
			int _nextLvTotalExp = 0;
			public int NextLvTotalExp { get { return _nextLvTotalExp; } private set { _nextLvTotalExp = value; } }

			/// <summary>
			/// 次のレベルになる為の累積経験値フォーマット変更イベント
			/// </summary>
			public event EventHandler<NextLvTotalExpFormatChangeEventArgs> OnNextLvTotalExpFormatChange = (sender, e) => { };
			/// <summary>
			/// 次のレベルになる為の累積経験値フォーマット
			/// </summary>
			string _nextLvTotalExpFormat = "";
			public string NextLvTotalExpFormat
			{
				get { return _nextLvTotalExpFormat; }
				set
				{
					if (_nextLvTotalExpFormat != value)
					{
						_nextLvTotalExpFormat = value;

						// 通知
						var eventArgs = new NextLvTotalExpFormatChangeEventArgs();
						this.OnNextLvTotalExpFormatChange(this, eventArgs);
					}
				}
			}
			#endregion

			#region 次のレベルまでの経験値
			/// <summary>
			/// 次のレベルまでの経験値を取得する
			/// </summary>
			public int GetNextLvExp()
			{
				if (this.NextLvTotalExp == 0)
				{
					return 0;
				}
				return this.NextLvTotalExp - this.Exp;
			}

			/// <summary>
			/// 次のレベルまでの経験値フォーマット変更イベント
			/// </summary>
			public event EventHandler<NextLvExpFormatChangeEventArgs> OnNextLvExpFormatChange = (sender, e) => { };
			/// <summary>
			/// 次のレベルまでの経験値フォーマット
			/// </summary>
			string _nextLvExpFormat = "";
			public string NextLvExpFormat
			{
				get { return _nextLvExpFormat; }
				set
				{
					if (_nextLvExpFormat != value)
					{
						_nextLvExpFormat = value;

						// 通知
						var eventArgs = new NextLvExpFormatChangeEventArgs();
						this.OnNextLvExpFormatChange(this, eventArgs);
					}
				}
			}
			#endregion
			#endregion

			#region 合成後のレベルデータ
			#region 合成後の経験値
			/// <summary>
			/// 合成後の累積経験値変更イベント
			/// </summary>
			public event EventHandler<AfterExpChangeEventArgs> OnAfterExpChange = (sender, e) => { };
			/// <summary>
			/// 合成後の累積経験値
			/// </summary>
			int _afterExp = 0;
			public int AfterExp
			{
				get { return _afterExp; }
				set
				{
					if (_afterExp != value)
					{
						_afterExp = value;

						// 通知
						var eventArgs = new AfterExpChangeEventArgs();
						this.OnAfterExpChange(this, eventArgs);
					}
				}
			}

			/// <summary>
			/// 合成後の累積経験値フォーマット変更イベント
			/// </summary>
			public event EventHandler<AfterExpFormatChangeEventArgs> OnAfterExpFormatChange = (sender, e) => { };
			/// <summary>
			/// 合成後の累積経験値フォーマット
			/// </summary>
			string _afterExpFormat = "";
			public string AfterExpFormat
			{
				get { return _afterExpFormat; }
				set
				{
					if (_afterExpFormat != value)
					{
						_afterExpFormat = value;

						// 通知
						var eventArgs = new AfterExpFormatChangeEventArgs();
						this.OnAfterExpFormatChange(this, eventArgs);
					}
				}
			}
			#endregion

			#region 合成後のレベルデータ設定
			/// <summary>
			/// 合成後のレベルデータ変更イベント
			/// </summary>
			public event EventHandler<AfterLvDataChangeEventArgs> OnAfterLvDataChange = (sender, e) => { };

			/// <summary>
			/// 合成後のレベルデータを設定する
			/// </summary>
			public void SetAfterLvData(int lv, int thisLvTotalExp, int nextLvTotalExp)
			{
				if (this.AfterLv != lv || this.AfterTotalExp != thisLvTotalExp || this.AfterNextLvTotalExp != nextLvTotalExp)
				{
					this.AfterLv = lv;
					this.AfterTotalExp = thisLvTotalExp;
					this.AfterNextLvTotalExp = nextLvTotalExp;

					// 通知
					var eventArgs = new AfterLvDataChangeEventArgs();
					this.OnAfterLvDataChange(this, eventArgs);
				}
			}
			#endregion

			#region 合成後のレベル
			/// <summary>
			/// 合成後のレベル
			/// </summary>
			int _afterLv = 0;
			public int AfterLv { get { return _afterLv; } set { _afterLv = value; } }

			/// <summary>
			/// 合成後のレベルフォーマット変更イベント
			/// </summary>
			public event EventHandler<AfterLvFormatChangeEventArgs> OnAfterLvFormatChange = (sender, e) => { };
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
						var eventArgs = new AfterLvFormatChangeEventArgs();
						this.OnAfterLvFormatChange(this, eventArgs);
					}
				}
			}
			#endregion

			#region 合成後のレベルになる為の累積経験値
			/// <summary>
			/// 合成後のレベルになる為の累積経験値
			/// </summary>
			int _afterTotalExp = 0;
			public int AfterTotalExp { get { return _afterTotalExp; } private set { _afterTotalExp = value; } }

			/// <summary>
			/// 合成後のレベルになる為の累積経験値フォーマット変更イベント
			/// </summary>
			public event EventHandler<AfterTotalExpFormatChangeEventArgs> OnAfterTotalExpFormatChange = (sender, e) => { };
			/// <summary>
			/// 合成後のレベルになる為の累積経験値フォーマット
			/// </summary>
			string _afterTotalExpFormat = "";
			public string AfterTotalExpFormat
			{
				get { return _afterTotalExpFormat; }
				set
				{
					if (_afterTotalExpFormat != value)
					{
						_afterTotalExpFormat = value;

						// 通知
						var eventArgs = new AfterTotalExpFormatChangeEventArgs();
						this.OnAfterTotalExpFormatChange(this, eventArgs);
					}
				}
			}
			#endregion

			#region 合成後の次のレベルになる為の累積経験値
			/// <summary>
			/// 合成後の次のレベルになる為の累積経験値
			/// </summary>
			int _afterNextLvTotalExp = 0;
			public int AfterNextLvTotalExp { get { return _afterNextLvTotalExp; } set { _afterNextLvTotalExp = value; } }

			/// <summary>
			/// 合成後の次のレベルになる為の累積経験値フォーマット変更イベント
			/// </summary>
			public event EventHandler<AfterNextLvTotalExpFormatChangeEventArgs> OnAfterNextLvTotalExpFormatChange = (sender, e) => { };
			/// <summary>
			/// 合成後の次のレベルになる為の累積経験値フォーマット
			/// </summary>
			string _afterNextLvTotalExpFormat = "";
			public string AfterNextLvTotalExpFormat
			{
				get { return _afterNextLvTotalExpFormat; }
				set
				{
					if (_afterNextLvTotalExpFormat != value)
					{
						_afterNextLvTotalExpFormat = value;

						// 通知
						var eventArgs = new AfterNextLvTotalExpFormatChangeEventArgs();
						this.OnAfterNextLvTotalExpFormatChange(this, eventArgs);
					}
				}
			}
			#endregion

			#region 合成後の次のレベルまでの経験値
			/// <summary>
			/// 合成後の次のレベルまでの経験値を取得する
			/// </summary>
			public int GetAfterNextLvExp()
			{
				if (this.AfterNextLvTotalExp == 0)
				{
					return 0;
				}
				return this.AfterNextLvTotalExp - this.AfterExp;
			}

			/// <summary>
			/// 次のレベルまでの経験値フォーマット変更イベント
			/// </summary>
			public event EventHandler<AfterNextLvExpFormatChangeEventArgs> OnAfterNextLvExpFormatChange = (sender, e) => { };
			/// <summary>
			/// 次のレベルまでの経験値フォーマット
			/// </summary>
			string _afterNextLvExpFormat = "";
			public string AfterNextLvExpFormat
			{
				get { return _afterNextLvExpFormat; }
				set
				{
					if (_afterNextLvExpFormat != value)
					{
						_afterNextLvExpFormat = value;

						// 通知
						var eventArgs = new AfterNextLvExpFormatChangeEventArgs();
						this.OnAfterNextLvExpFormatChange(this, eventArgs);
					}
				}
			}
			#endregion

			#region 合成後の余剰経験値
			/// <summary>
			/// 合成後の余剰経験値変更イベント
			/// </summary>
			public event EventHandler<AfterOverflowExpChangeEventArgs> OnAfterOverflowExpChange = (sender, e) => { };
			/// <summary>
			/// 合成後の余剰経験値
			/// </summary>
			int _afterOverflowExp = 0;
			public int AfterOverflowExp
			{
				get { return _afterOverflowExp; }
				set
				{
					if (_afterOverflowExp != value)
					{
						_afterOverflowExp = value;

						// 通知
						var eventArgs = new AfterOverflowExpChangeEventArgs();
						this.OnAfterOverflowExpChange(this, eventArgs);
					}
				}
			}

			/// <summary>
			/// 合成後の余剰経験値フォーマット変更イベント
			/// </summary>
			public event EventHandler<AfterOverflowExpFormatChangeEventArgs> OnAfterOverflowExpFormatChange = (sender, e) => { };
			/// <summary>
			/// 合成後の余剰経験値フォーマット
			/// </summary>
			string _afterOverflowExpFormat = "";
			public string AfterOverflowExpFormat
			{
				get { return _afterOverflowExpFormat; }
				set
				{
					if (_afterOverflowExpFormat != value)
					{
						_afterOverflowExpFormat = value;

						// 通知
						var eventArgs = new AfterOverflowExpFormatChangeEventArgs();
						this.OnAfterOverflowExpFormatChange(this, eventArgs);
					}
				}
			}
			#endregion
			#endregion

			#region 所持金
			/// <summary>
			/// 所持金変更イベント
			/// </summary>
			public event EventHandler<HaveMoneyChangeEventArgs> OnHaveMoneyChange = (sender, e) => { };
			/// <summary>
			/// 所持金
			/// </summary>
			int _haveMoney = 0;
			public int HaveMoney
			{
				get { return _haveMoney; }
				set
				{
					if (_haveMoney != value)
					{
						_haveMoney = value;

						// 通知
						var eventArgs = new HaveMoneyChangeEventArgs();
						this.OnHaveMoneyChange(this, eventArgs);
					}
				}
			}

			/// <summary>
			/// 所持金フォーマット変更イベント
			/// </summary>
			public event EventHandler<HaveMoneyFormatChangeEventArgs> OnHaveMoneyFormatChange = (sender, e) => { };
			/// <summary>
			/// 所持金フォーマット
			/// </summary>
			string _haveMoneyFormat = "";
			public string HaveMoneyFormat
			{
				get { return _haveMoneyFormat; }
				set
				{
					if (_haveMoneyFormat != value)
					{
						_haveMoneyFormat = value;

						// 通知
						var eventArgs = new HaveMoneyFormatChangeEventArgs();
						this.OnHaveMoneyFormatChange(this, eventArgs);
					}
				}
			}
			#endregion

			#region 費用
			/// <summary>
			/// 費用変更イベント
			/// </summary>
			public event EventHandler<NeedMoneyChangeEventArgs> OnNeedMoneyChange = (sender, e) => { };
			/// <summary>
			/// 費用
			/// </summary>
			int _needMoney = 0;
			public int NeedMoney
			{
				get { return _needMoney; }
				set
				{
					if (_needMoney != value)
					{
						_needMoney = value;

						// 通知
						var eventArgs = new NeedMoneyChangeEventArgs();
						this.OnNeedMoneyChange(this, eventArgs);
					}
				}
			}

			/// <summary>
			/// 費用フォーマット変更イベント
			/// </summary>
			public event EventHandler<NeedMoneyFormatChangeEventArgs> OnNeedMoneyFormatChange = (sender, e) => { };
			/// <summary>
			/// 費用フォーマット
			/// </summary>
			string _needMoneyFormat = "";
			public string NeedMoneyFormat
			{
				get { return _needMoneyFormat; }
				set
				{
					if (_needMoneyFormat != value)
					{
						_needMoneyFormat = value;

						// 通知
						var eventArgs = new NeedMoneyFormatChangeEventArgs();
						this.OnNeedMoneyFormatChange(this, eventArgs);
					}
				}
			}
			#endregion

			#region 追加料金
			/// <summary>
			/// 追加料金変更イベント
			/// </summary>
			public event EventHandler<AddOnChargeChangeEventArgs> OnAddOnChargeChange = (sender, e) => { };
			/// <summary>
			/// 追加料金
			/// </summary>
			int _addOnCharge = 0;
			public int AddOnCharge
			{
				get { return _addOnCharge; }
				set
				{
					if (_addOnCharge != value)
					{
						_addOnCharge = value;

						// 通知
						var eventArgs = new AddOnChargeChangeEventArgs();
						this.OnAddOnChargeChange(this, eventArgs);
					}
				}
			}

			/// <summary>
			/// 追加料金フォーマット変更イベント
			/// </summary>
			public event EventHandler<AddOnChargeFormatChangeEventArgs> OnAddOnChargeFormatChange = (sender, e) => { };
			/// <summary>
			/// 追加料金フォーマット
			/// </summary>
			string _addOnChargeFormat = "";
			public string AddOnChargeFormat
			{
				get { return _addOnChargeFormat; }
				set
				{
					if (_addOnChargeFormat != value)
					{
						_addOnChargeFormat = value;

						// 通知
						var eventArgs = new AddOnChargeFormatChangeEventArgs();
						this.OnAddOnChargeFormatChange(this, eventArgs);
					}
				}
			}
			#endregion

			#region 経験値バーの値
			/// <summary>
			/// 現在の経験値バーの値を取得する
			/// </summary>
			public float GetExpSliderValue()
			{
				return Util.GetLerpValue((float)this.TotalExp, (float)this.NextLvTotalExp, (float)this.Exp);
			}
			/// <summary>
			/// 合成後の現在の経験値バーの値を取得する
			/// </summary>
			public float GetAfterExpSliderValue()
			{
				if (this.Lv != this.AfterLv)
				{
					return 0f;
				}
				return this.GetExpSliderValue();
			}
			/// <summary>
			/// 合成後の獲得経験値バーの値を取得する
			/// </summary>
			public float GetAfterTakeExpSliderValue()
			{
				float takeTotalExp = (float)this.Exp + (float)this.TakeExp;
				float nextLvtotalExp = (float)this.NextLvTotalExp;
				if (this.Lv != this.AfterLv)
				{
					nextLvtotalExp = (float)this.AfterNextLvTotalExp;
				}
				return Util.GetLerpValue((float)this.AfterTotalExp, nextLvtotalExp, takeTotalExp);
			}
			#endregion
		}
	}
}
