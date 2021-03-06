/// <summary>
/// プレイヤー情報制御
/// 
/// 2015/12/10
/// </summary>
using System;
using UnityEngine;

namespace XUI
{
	namespace PlayerInfo
	{
		/// <summary>
		/// プレイヤー情報制御インターフェイス
		/// </summary>
		public interface IController
		{
			/// <summary>
			/// 更新できる状態かどうか
			/// </summary>
			bool CanUpdate { get; }

			void Update();
		}

		/// <summary>
		/// プレイヤー情報制御
		/// </summary>
		public class Controller : IController
		{
			#region フィールド＆プロパティ
			// モデル
			readonly IModel _model;
			IModel Model { get { return _model; } }
			// ビュー
			readonly IView _view;
			IView View { get { return _view; } }
			// キャラアイコン
			CharaIcon CharaIcon { get; set; }
			/// <summary>
			/// 更新できる状態かどうか
			/// </summary>
			public bool CanUpdate
			{
				get
				{
					if (this.Model == null) return false;
					if (this.View == null) return false;
					return true;
				}
			}
			/// <summary>
			/// キャラアイコンが更新できる状態かどうか
			/// </summary>
			public bool CanCharaIconUpdate
			{
				get
				{
					if (this.CharaIcon == null) return false;
					return true;
				}
			}
			#endregion

			#region 初期化
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public Controller(IModel model, IView view, CharaIcon charaIcon)
			{
				if (model == null || view == null) return;

				// ビュー設定
				this._view = view;
				this.View.OnAddStone += this.HandleAddStone;
				this.View.OnCharaIcon += this.HandleCharaIcon;

				// モデル設定
				this._model = model;
				this.Model.OnAvatarTypeChange += this.HandleAvatarTypeChange;
				this.Model.OnNameChange += this.HandleNameChange;
				this.Model.OnLvChange += this.HandleLvChange;
				this.Model.OnGradeChange += this.HandleOnGradeChange;
				this.Model.OnGradeFormatChange += this.HandleOnGradeFormatChange;
				this.Model.OnLvFormatChange += this.HandleLvFormatChange;
				this.Model.OnExpChange += this.HandleExpChange;
				this.Model.OnExpMinChange += this.HandleExpMinChange;
				this.Model.OnExpMaxChange += this.HandleExpMaxChange;
				this.Model.OnExpFormatChange += this.HandleExpFormatChange;
				this.Model.OnStaminaChange += this.HandleStaminaChange;
				this.Model.OnStaminaMaxChange += this.HandleStaminaMaxChange;
				this.Model.OnStaminaFormatChange += this.HandleStaminaFormatChange;
				this.Model.OnStaminaTimeChange += this.HandleStaminaTimeChange;
				this.Model.OnStaminaTimeFormatChange += this.HandleStaminaTimeFormatChange;
				this.Model.OnStaminaRecoveryTimeChange += this.HandleStaminaRecoveryTimeChange;

				// キャラアイコン設定
				this.CharaIcon = charaIcon;

				// 同期
				//this.SyncAvatarType();
				this.SyncName();
				this.SyncGrade();
				this.SyncLv();
				this.SyncExp();
				this.SyncStamina();
				this.SyncStaminaTime();
			}
			#endregion

			#region 更新
			public void Update()
			{
				if (this.CanUpdate)
				{
					this.Model.CountDownStaminaTime(Time.deltaTime);
				}
			}
			#endregion

			#region 石追加ボタンイベント
			void HandleAddStone(object sender, EventArgs e)
			{
			}
			#endregion

			#region キャラアイコンボタンイベント
			void HandleCharaIcon(object sender, EventArgs e)
			{
				GUILobbyMenu.SetMode(GUILobbyMenu.MenuMode.None);
				GUIController.Open(new GUIScreen(GUISymbolChara.Open, GUISymbolChara.Close, GUISymbolChara.ReOpen, GUISymbolChara.Close));
			}
			#endregion

			#region AvatarType
			void HandleAvatarTypeChange(object sender, AvatarTypeChangeEventArgs e)
			{
				this.SyncAvatarType();
			}
			void SyncAvatarType()
			{
				if (this.CanCharaIconUpdate)
				{
					this.CharaIcon.GetIcon(this.Model.AvatarType, this.Model.SkinId, false, this.View.SetCharaIcon);
                    //Todo Lee: Remove Up When Stable
				    if (null != GUILobbyResident.Instance)
				    {
                        GUILobbyResident.SetPlayerName(this.Model.Name);
                        ScmParam.Battle.CharaIcon.GetIcon(this.Model.AvatarType, this.Model.SkinId, false, GUILobbyResident.SetIcon);
				    }
				}
			}
			#endregion

			#region 名前
			void HandleNameChange(object sender, NameChangeEventArgs e)
			{
				this.SyncName();
			}
			void SyncName()
			{
				if (this.CanUpdate)
				{
					this.View.SetName(this.Model.Name, this.Model.NameFormat);
				}
			}
			#endregion

			#region グレード
			void HandleOnGradeFormatChange(object sender, GradeFormatChangeEventArgs e)
			{
				this.SyncGrade();
			}
			void HandleOnGradeChange(object sender, GradeChangeEventArgs e)
			{
				this.SyncGrade();
			}
			void SyncGrade()
			{
				if (this.CanUpdate)
				{
					this.View.SetGrade(this.Model.Grade, this.Model.GradeFormat);
				}
			}
			#endregion

			#region プレイヤーレベル
			void HandleLvChange(object sender, LvChangeEventArgs e)
			{
				this.SyncLv();
			}
			void HandleLvFormatChange(object sender, LvFormatChangeEventArgs e)
			{
				this.SyncLv();
			}
			void SyncLv()
			{
				if (this.CanUpdate)
				{
					this.View.SetLv(this.Model.Lv, this.Model.LvFormat);
				}
			}
			#endregion

			#region プレイヤー経験値
			void HandleExpChange(object sender, ExpChangeEventArgs e)
			{
				this.SyncExp();
			}
			void HandleExpMinChange(object sender, ExpMinChangeEventArgs e)
			{
				this.SyncExp();
			}
			void HandleExpMaxChange(object sender, ExpMaxChangeEventArgs e)
			{
				this.SyncExp();
			}
			void HandleExpFormatChange(object sender, ExpFormatChangeEventArgs e)
			{
				this.SyncExp();
			}
			void SyncExp()
			{
				if (this.CanUpdate)
				{
					this.View.SetExp(this.Model.ExpSliderValue, this.Model.ExpFormat);
				}
			}
			#endregion

			#region スタミナ
			void HandleStaminaChange(object sender, StaminaChangeEventArgs e)
			{
				this.SyncStamina();
			}
			void HandleStaminaMaxChange(object sender, StaminaMaxChangeEventArgs e)
			{
				this.SyncStamina();
			}
			void HandleStaminaFormatChange(object sender, StaminaFormatChangeEventArgs e)
			{
				this.SyncStamina();
			}
			void SyncStamina()
			{
				if (this.CanUpdate)
				{
					this.View.SetStamina(this.Model.Stamina, this.Model.StaminaMax, this.Model.StaminaFormat);
				}
			}
			#endregion

			#region スタミナ回復までの残り時間
			void HandleStaminaTimeChange(object sender, StaminaTimeChangeEventArgs e)
			{
				this.SyncStaminaTime();
			}
			void HandleStaminaTimeFormatChange(object sender, StaminaTimeFormatChangeEventArgs e)
			{
				this.SyncStaminaTime();
			}
			void HandleStaminaRecoveryTimeChange(object sender, StaminaRecoveryTimeChangeEventArgs e)
			{
			}
			void SyncStaminaTime()
			{
				if (this.CanUpdate)
				{
					this.View.SetStaminaTime(this.Model.GetStaminaTime(), this.Model.StaminaTimeFormat);
				}
			}
			#endregion
		}
	}
}
