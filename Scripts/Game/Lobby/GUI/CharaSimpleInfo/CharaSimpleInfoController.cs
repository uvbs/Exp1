/// <summary>
/// キャラ簡易情報制御
/// 
/// 2016/01/25
/// </summary>
using UnityEngine;
using System;
using System.Collections.Generic;

namespace XUI
{
	namespace CharaSimpleInfo
	{
		/// <summary>
		/// キャラ簡易情報制御インターフェイス
		/// </summary>
		public interface IController
		{
			/// <summary>
			/// 破棄
			/// </summary>
			void Dispose();

			/// <summary>
			/// 更新できる状態かどうか
			/// </summary>
			bool CanUpdate { get; }

			/// <summary>
			/// セットアップ
			/// </summary>
			void Setup(Vector3 position, CharaInfo charaInfo);

			/// <summary>
			/// プレイヤーキャラクター情報を設定する
			/// </summary>
			void SetupPlayerCharacterInfo(CharaInfo info, int slotHitPoint, int slotAttack, int slotDefense, int slotExtra, List<CharaInfo> slotList);
		}

		/// <summary>
		/// キャラ簡易情報制御
		/// </summary>
		public class Controller : IController
		{
			#region フィールド&プロパティ
			/// <summary>
			/// モデル
			/// </summary>
			private readonly IModel _model;
			private IModel Model { get { return _model; } }

			/// <summary>
			/// ビュー
			/// </summary>
			private readonly IView _view;
			private IView View { get { return _view; } }

			/// <summary>
			/// 更新できる状態かどうか
			/// </summary>
			public bool CanUpdate
			{
				get
				{
					if (this.Model == null) { return false; }
					if (this.View == null) { return false; }
					return true;
				}
			}
			#endregion

			#region　初期化
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="model"></param>
			/// <param name="view"></param>
			public Controller(IModel model, IView view)
			{
				if (model == null || view == null) { return; }

				// ビュー設定
				this._view = view;
				this.View.OnCharaInfoClickEvent += this.HandleCharaInfoClickEvent;

				// モデル設定
				this._model = model;
				this.Model.OnCharaInfoChange += HandleCharaInfoChange;
				this.Model.OnPositionChange += HandlePositionChange;
				this.Model.OnHitPointChange += this.HandleHitPointChange;
				this.Model.OnAttackChange += this.HandleAttackChange;
				this.Model.OnDefenseChange += this.HandleDefenseChange;
				this.Model.OnExtraChange += this.HandleExtraChange;
				this.Model.OnStatusFormatChange += this.HandleStatusFormatChange;
				this.Model.OnLevelFormatChange += HandleLevelFormatChange;

				// 同期
				SyncPosition();
				SyncCharaInfo();
				SyncStatus();
			}

			/// <summary>
			/// 破棄
			/// </summary>
			public void Dispose()
			{
				if(this.CanUpdate)
				{
					this.Model.Dispose();
				}
			}
			#endregion

			#region 設定
			/// <summary>
			/// セットアップ
			/// </summary>
			public void Setup(Vector3 position, CharaInfo charaInfo)
			{
				if (!this.CanUpdate) { return; }
				
				this.Model.Position = position;
				this.Model.CharaInfo = charaInfo;
				
				// 各ステータスを初期化
				this.Model.HitPoint = 0;
				this.Model.Attack = 0;
				this.Model.Defense = 0;
				this.Model.Extra = 0;
			}
			#endregion

			#region キャラ情報
			/// <summary>
			/// キャラ情報が変更された時に呼び出される
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void HandleCharaInfoChange(object sender, EventArgs e)
			{
				SyncCharaInfo();
			}

			/// <summary>
			/// キャラ情報同期
			/// </summary>
			private void SyncCharaInfo()
			{
				if (!CanUpdate) { return; }

				// キャラ名/ランクセット
				if (this.Model.CharaInfo != null)
				{
					this.View.SetCharaName(this.Model.CharaInfo.Name);
					this.View.SetRank(this.Model.CharaInfo.Rank.ToString());
				}
				else
				{
					this.View.SetCharaName("");
					this.View.SetRank("0");
				}

				// レベルとステータスセット
				SyncLevel();
				// お気に入りセット
				SyncLock();
			}
			#endregion

			#region 表示位置
			/// <summary>
			/// 位置が変更された時に呼び出される
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void HandlePositionChange(object sender, EventArgs e)
			{
				SyncPosition();
			}

			/// <summary>
			/// 表示位置同期
			/// </summary>
			private void SyncPosition()
			{
				if (!CanUpdate) { return; }
				this.View.Position = this.Model.Position;
			}
			#endregion

			#region 強化レベル表示
			/// <summary>
			/// 強化レベルの表示フォーマットが変更された時に呼び出される
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void HandleLevelFormatChange(object sender, EventArgs e)
			{
				SyncLevel();
			}

			/// <summary>
			/// 強化レベル同期
			/// </summary>
			private void SyncLevel()
			{
				if (!CanUpdate) { return; }
				if(this.Model.CharaInfo != null)
				{
					this.View.SetLevel(string.Format(this.Model.LevelFormat, this.Model.CharaInfo.PowerupLevel, CharaInfo.GetMaxLevel(this.Model.CharaInfo.Rank)));
				}
				else
				{
					this.View.SetLevel(string.Format(this.Model.LevelFormat, 0, 0));
				}
			}
			#endregion

			#region お気に入り
			/// <summary>
			/// お気に入り処理同期
			/// </summary>
			private void SyncLock()
			{
				if (!CanUpdate || this.Model.CharaInfo == null) { return; }
				this.View.SetLockActive(this.Model.CharaInfo.IsLock);
			}
			#endregion

			#region キャラ詳細
			/// <summary>
			/// キャラ詳細ボタンが押された時のイベントハンドラー
			/// </summary>
			private void HandleCharaInfoClickEvent(object sender, EventArgs e)
			{
				if (!this.CanUpdate || this.Model.CharaInfo == null) { return; }
				ulong uuid = this.Model.CharaInfo.UUID;

				// 詳細画面表示
				var screen = new GUIScreen(() => { GUICharacterInfo.Open(uuid); }, GUICharacterInfo.Close, () => { GUICharacterInfo.ReOpen(uuid); });
				GUIController.Open(screen);

				// キャラ簡易画面は閉じる
				GUIController.SingleClose();
			}
			#endregion

			#region ステータス
			/// <summary>
			/// 生命力変更イベントハンドラー
			/// </summary>
			private void HandleHitPointChange(object sender, EventArgs e)
			{
				this.SyncHitPoint();
			}
			/// <summary>
			/// 生命力同期
			/// </summary>
			private void SyncHitPoint()
			{
				if (!this.CanUpdate) { return; }
				this.View.SetHitPoint(string.Format(this.Model.StatusFormat, this.Model.HitPoint));
			}

			/// <summary>
			/// 攻撃力変更イベントハンドラー
			/// </summary>
			private void HandleAttackChange(object sender, EventArgs e)
			{
				this.SyncAttack();
			}
			/// <summary>
			/// 攻撃力同期
			/// </summary>
			private void SyncAttack()
			{
				if (!this.CanUpdate) { return; }
				this.View.SetAttack(string.Format(this.Model.StatusFormat, this.Model.Attack));
			}

			/// <summary>
			/// 防御力変更イベントハンドラー
			/// </summary>
			private void HandleDefenseChange(object sender, EventArgs e)
			{
				this.SyncDefense();
			}
			/// <summary>
			/// 防御力同期
			/// </summary>
			private void SyncDefense()
			{
				if (!this.CanUpdate) { return; }
				this.View.SetDefense(string.Format(this.Model.StatusFormat, this.Model.Defense));
			}

			/// <summary>
			/// 特殊能力変更イベントハンドラー
			/// </summary>
			private void HandleExtraChange(object sender, EventArgs e)
			{
				this.SyncExtra();
			}
			/// <summary>
			/// 特殊能力同期
			/// </summary>
			private void SyncExtra()
			{
				if (!this.CanUpdate) { return; }
				this.View.SetExtra(string.Format(this.Model.StatusFormat, this.Model.Extra));
			}

			/// <summary>
			/// ステータスフォーマット変更イベントハンドラー
			/// </summary>
			private void HandleStatusFormatChange(object sender, EventArgs e)
			{
				this.SyncStatusFormat();
			}
			/// <summary>
			/// ステータスフォーマット同期
			/// </summary>
			private void SyncStatusFormat()
			{
				this.SyncStatus();
			}

			/// <summary>
			/// 各ステータスを同期させる
			/// </summary>
			private void SyncStatus()
			{
				this.SyncHitPoint();
				this.SyncAttack();
				this.SyncDefense();
				this.SyncExtra();
			}
			#endregion

			#region プレイヤーキャラクター取得
			/// <summary>
			/// プレイヤーキャラクター情報を設定する
			/// </summary>
			public void SetupPlayerCharacterInfo(CharaInfo info, int slotHitPoint, int slotAttack, int slotDefense, int slotExtra, List<CharaInfo> slotList)
			{
				if (info == null) { return; }
				if (this.Model.CharaInfo == null || info.UUID != this.Model.CharaInfo.UUID) { return; }

				this.Model.HitPoint = info.HitPoint;
				this.Model.Attack = info.Attack;
				this.Model.Defense = info.Defense;
				this.Model.Extra = info.Extra;
			}
			#endregion
		}
	}
}