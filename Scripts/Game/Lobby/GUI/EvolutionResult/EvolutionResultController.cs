/// <summary>
/// 進化合成結果制御
/// 
/// 2016/03/03
/// </summary>
using UnityEngine;
using System;

namespace XUI.EvolutionResult
{
	/// <summary>
	/// 初期化パラメータ
	/// </summary>
	public struct SetupParam
	{
		public AvatarType AvatarType { get; set; }
		public int BeforeRank { get; set; }
		public int AfterRank { get; set; }
		public int SynchroRemain { get; set; }
		public int SynchroRemainUp { get; set; }
		public int Lv { get; set; }
		public int HitPoint { get; set; }
		public int Attack { get; set; }
		public int Defence { get; set; }
		public int Extra { get; set; }
	}

	/// <summary>
	/// 進化合成結果制御インターフェイス
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
		/// アクティブ設定
		/// </summary>
		void SetActive(bool isActive, bool isTweenSkip);

		/// <summary>
		/// 初期設定
		/// </summary>
		void Setup(SetupParam param);
	}

	/// <summary>
	/// 進化合成結果制御
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
		private readonly IVIew _view;
		private IVIew View { get { return _view; } }

		/// <summary>
		/// 更新できる状態化どうか
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

		/// <summary>
		/// キャラボード
		/// </summary>
		private readonly CharaBoard _charaBoard;
		private CharaBoard CharaBoard { get { return _charaBoard; } }
		#endregion

		#region 初期化
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Controller(IModel model, IVIew view, CharaBoard charaBoard)
		{
			if (model == null || view == null) { return; }

			this._charaBoard = charaBoard;

			// ビュー設定
			this._view = view;
			this.View.OnOK += this.HandleOK;

			// モデル設定
			this._model = model;
			// アバタータイプ
			this.Model.OnAvatarTypeChange += this.HandleAvatarTypeChange;
			// ランク
			this.Model.OnRankChange += this.HandleRankChange;
			// シンクロ合成残り回数
			this.Model.OnSynchroRemainChange += this.HandleSynchroRemainChange;
			this.Model.OnSynchroRemainUpFormatChange += this.HandleSynchroRemainFormatChange;
			// レベル
			this.Model.OnLvChange += this.HandleLvChange;
			this.Model.OnLvFormatChange += this.HandleLvFormatChange;
			// 生命力
			this.Model.OnHitPointChange += this.HandleHitPointChange;
			this.Model.OnHitPointFormatChange += this.HandleHitPointFormatChange;
			// 攻撃力
			this.Model.OnAttackChange += this.HandleAttackChange;
			this.Model.OnAttackFormatChange += this.HandleAttackFormatChange;
			// 防御力
			this.Model.OnDefenceChnage += this.HandleDefenceChange;
			this.Model.OnDefenceFormatChange += this.HandleDefenceFormatChange;
			// 特殊能力
			this.Model.OnExtraChange += this.HandleExtraChnage;
			this.Model.OnExtraFormatChange += this.HandleExtraFormatChange;

			// 同期
			this.SyncAvatarType();
			this.SyncRank();
			this.SyncSynchroRemain();
			this.SyncLv();
			this.SyncHitPoint();
			this.SyncAttack();
			this.SyncDefence();
			this.SyncExtra();
		}

		/// <summary>
		/// 初期設定
		/// </summary>
		public void Setup(SetupParam param)
		{
			if (!this.CanUpdate) { return; }

			if(this.Model.AvatarType == param.AvatarType)
			{
				this.View.ReplayBoard(true);
			}
			this.Model.AvatarType = param.AvatarType;
			this.Model.SetRank(param.BeforeRank, param.AfterRank);
			this.Model.SetSynchroRemain(param.SynchroRemain, param.SynchroRemainUp);
			this.Model.Lv = param.Lv;
			this.Model.HitPoint = param.HitPoint;
			this.Model.Attack = param.Attack;
			this.Model.Defence = param.Defence;
			this.Model.Extra = param.Extra;
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			if (this.CanUpdate)
			{
				this.Model.Dispose();
			}
		}
		#endregion

		#region アクティブ設定
		/// <summary>
		/// アクティブ設定
		/// </summary>
		public void SetActive(bool isActive, bool isTweenSkip)
		{
			if (this.CanUpdate)
			{
				this.View.SetActive(isActive, isTweenSkip);
			}
		}
		#endregion

		#region OKボタンイベント
		/// <summary>
		/// OKボタンが押された時に呼び出される
		/// </summary>
		private void HandleOK(object sender, EventArgs e)
		{
			GUIController.Back();
		}
		#endregion

		#region アバタータイプ
		/// <summary>
		/// アバタータイプが変更された時に呼び出される
		/// </summary>
		private void HandleAvatarTypeChange(object sender, EventArgs e)
		{
			this.SyncAvatarType();
		}
		/// <summary>
		/// アバタータイプ同期
		/// </summary>
		private void SyncAvatarType()
		{
			if (!this.CanUpdate || this.CharaBoard == null) { return; }

			if(this.Model.AvatarType != AvatarType.None)
			{
				this.CharaBoard.GetBoard(this.Model.AvatarType, this.Model.SkinId, false,
					(resource) =>
					{
						this.CreateBoard(resource, this.Model.AvatarType);
					});
			}
		}
		/// <summary>
		/// 立ち絵生成
		/// </summary>
		private void CreateBoard(GameObject resource, AvatarType avatarType)
		{
			if (!this.CanUpdate) { return; }

			// リソース読み込み完了
			if (resource == null) { return; }
			// インスタンス化
			var go = SafeObject.Instantiate(resource) as GameObject;
			if (go == null) { return; }

			// 読み込み中に別のキャラに変更していたら破棄する
			if (this.Model.AvatarType != avatarType)
			{
				UnityEngine.Object.Destroy(go);
				return;
			}

			// 名前設定
			go.name = resource.name;
			// 親子付け
			var t = go.transform;
			this.View.SetBoardRoot(t);
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
			this.View.ReplayBoard(true);
		}
		#endregion

		#region ランク
		#region ハンドラー
		private void HandleRankChange(object sender, EventArgs e)
		{
			this.SyncRank();
		}
		#endregion

		/// <summary>
		/// ランク同期
		/// </summary>
		private void SyncRank()
		{
			if (!this.CanUpdate) { return; }

			this.View.SetBeforeRank(this.Model.BeforeRank);

			// ランクアップ表示
			var isRankUp = (this.Model.BeforeRank < this.Model.AfterRank);
			this.View.SetAfterRank(this.Model.AfterRank);
			this.View.SetRankUpActive(isRankUp);
			this.View.SetEvolutionActive(isRankUp);
		}

		#endregion

		#region シンクロ合成残り回数
		#region ハンドラ
		private void HandleSynchroRemainChange(object sender, EventArgs e)
		{
			this.SyncSynchroRemain();
		}
		private void HandleSynchroRemainFormatChange(object sender, EventArgs e)
		{
			this.SyncSynchroRemain();
		}
		private void HandleSynchroRemainUpFormatChange(object sender, EventArgs e)
		{
			this.SyncSynchroRemain();
		}
		#endregion

		/// <summary>
		/// シンクロ合成残り回数同期
		/// </summary>
		private void SyncSynchroRemain()
		{
			if (!this.CanUpdate) { return; }

			this.View.SetSynchroRemain(this.Model.SynchroRemain);

			var format = (this.Model.SynchroRemainUp == 0 ? "" : this.Model.SynchroRemainUpFormat);
			this.View.SetSynchroRemainUp(this.Model.SynchroRemainUp, format);
		}
		#endregion

		#region 強化レベル
		#region ハンドラー
		private void HandleLvChange(object sender, EventArgs e)
		{
			this.SyncLv();
		}
		private void HandleLvFormatChange(object sender, EventArgs e)
		{
			this.SyncLv();
		}
		#endregion

		/// <summary>
		/// 強化レベル同期
		/// </summary>
		private void SyncLv()
		{
			if (!this.CanUpdate) { return; }
			this.View.SetLv(this.Model.Lv, this.Model.LvFormat);
		}
		#endregion

		#region 生命力
		#region ハンドラー
		private void HandleHitPointChange(object sender, EventArgs e)
		{
			this.SyncHitPoint();
		}
		private void HandleHitPointFormatChange(object sender, EventArgs e)
		{
			this.SyncHitPoint();
		}
		#endregion

		/// <summary>
		/// 生命力同期
		/// </summary>
		private void SyncHitPoint()
		{
			if (!this.CanUpdate) { return; }
			this.View.SetHitPoint(this.Model.HitPoint, this.Model.HitPointFormat);
		}
		#endregion

		#region 攻撃力
		#region ハンドラー
		private void HandleAttackChange(object sender, EventArgs e)
		{
			this.SyncAttack();
		}
		private void HandleAttackFormatChange(object sender, EventArgs e)
		{
			this.SyncAttack();
		}
		#endregion

		/// <summary>
		/// 攻撃力同期
		/// </summary>
		private void SyncAttack()
		{
			if (!this.CanUpdate) { return; }
			this.View.SetAttack(this.Model.Attack, this.Model.AttackFormat);
		}
		#endregion

		#region 防御力
		#region ハンドラー
		private void HandleDefenceChange(object sender, EventArgs e)
		{
			this.SyncDefence();
		}
		private void HandleDefenceFormatChange(object sender, EventArgs e)
		{
			this.SyncDefence();
		}
		#endregion

		/// <summary>
		/// 防御力同期
		/// </summary>
		private void SyncDefence()
		{
			if (!this.CanUpdate) { return; }
			this.View.SetDefence(this.Model.Defence, this.Model.DefenceFormat);
		}
		#endregion

		#region 特殊能力
		#region ハンドラー
		private void HandleExtraChnage(object sender, EventArgs e)
		{
			this.SyncExtra();
		}
		private void HandleExtraFormatChange(object sender, EventArgs e)
		{
			this.SyncExtra();
		}
		#endregion

		/// <summary>
		/// 特殊能力同期
		/// </summary>
		private void SyncExtra()
		{
			if (!CanUpdate) { return; }
			this.View.SetExtra(this.Model.Extra, this.Model.ExtraFormat);
		}
		#endregion
	}
}