/// <summary>
/// 合成メッセージ制御
/// 
/// 2016/05/10
/// </summary>
using System;
using System.Collections.Generic;

namespace XUI.FusionMessage
{
	#region データ受け渡し用
	/// <summary>
	/// データ受け渡し用クラス
	/// </summary>
	public class SetupParam
	{
		// ランク
		public int RankBefore { get; set; }
		public int RankAfter { get; set; }
		// レベル
		public int LevelBefore { get; set; }
		public int LevelAfter { get; set; }
		// 経験値
		public int Exp { get; set; }				// 現在の累積経験値
		public int TotalExp { get; set; }			// 現在のレベルになる為の累積経験値
		public int NextLvTotalExp { get; set; }		// 次のレベルになる為の累積経験値
		// シンクロ可能回数
		public int SynchroRemainBefore { get; set; }
		public int SynchroRemainAfter { get; set; }
		// 生命力
		public int HitPointBefore { get; set; }
		public int HitPointAfter { get; set; }
		public int HitPointBaseBefore { get; set; }
		public int HitPointBaseAfter { get; set; }
		public int SynchroHitPoint { get; set; }
		public int SlotHitPointBefore { get; set; }
		public int SlotHitPointAfter { get; set; }
		// 攻撃力
		public int AttackBefore { get; set; }
		public int AttackAfter { get; set; }
		public int AttackBaseBefore { get; set; }
		public int AttackBaseAfter { get; set; }
		public int SynchroAttack { get; set; }
		public int SlotAttackBefore { get; set; }
		public int SlotAttackAfter { get; set; }
		// 防御力
		public int DefenseBefore { get; set; }
		public int DefenseAfter { get; set; }
		public int DefenseBaseBefore { get; set; }
		public int DefenseBaseAfter { get; set; }
		public int SynchroDefense { get; set; }
		public int SlotDefenseBefore { get; set; }
		public int SlotDefenseAfter { get; set; }
		// 特殊能力
		public int ExtraBefore { get; set; }
		public int ExtraAfter { get; set; }
		public int ExtraBaseBefore { get; set; }
		public int ExtraBaseAfter { get; set; }
		public int SynchroExtra { get; set; }
		public int SlotExtraBefore { get; set; }
		public int SlotExtraAfter { get; set; }
		// シンクロ合成かどうか
		private bool _isSynchroFusion = false;
		public bool IsSynchroFusion { get { return _isSynchroFusion; } set { _isSynchroFusion = value; } }
	}
	#endregion

	#region モード
	/// <summary>
	/// モード
	/// </summary>
	public enum ModeType
	{
		None,
		OK,		// OKボタン
		YesNo,	// YesNoボタン
	}
	#endregion

	/// <summary>
	/// 合成メッセージ制御インターフェイス
	/// </summary>
	public interface IController
	{
		#region 更新チェック
		/// <summary>
		/// 更新できる状態かどうか
		/// </summary>
		bool CanUpdate { get; }
		#endregion

		#region 初期化
		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();
		#endregion

		#region アクティブ設定
		/// <summary>
		/// アクティブ設定
		/// </summary>
		void SetActive(bool isActive, bool isTweenSkip);
		#endregion

		#region セットアップ
		/// <summary>
		/// セットアップ
		/// </summary>
		void Setup(ModeType modeType, SetupParam param, string message);

		/// <summary>
		/// パラメータ設定
		/// </summary>
		void SetupParameter(SetupParam param);
		#endregion

		#region モード
		/// <summary>
		/// モード切替
		/// </summary>
		void ChangeMode(ModeType mode);
		#endregion

		#region 確認ボタン
		/// <summary>
		/// OKボタン押下時の発生イベント設定
		/// </summary>
		void SetOkButtonEvent(Action okEvent, bool isClose);

		/// <summary>
		/// Yesボタン押下時の発生イベント設定
		/// </summary>
		void SetYesButtonEvent(Action yesEvent, bool isClose);

		/// <summary>
		/// Noボタン押下時の発生イベント設定
		/// </summary>
		void SetNoButtonEvent(Action noEvent, bool isClose);
		#endregion

		#region メッセージ
		/// <summary>
		/// メッセージ設定
		/// </summary>
		void SetMessage(string message);
		#endregion
	}

	/// <summary>
	/// 合成メッセージ制御
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

		/// <summary>
		/// 現在のモード
		/// </summary>
		private ModeType NowMode { get; set; }

		/// <summary>
		/// モード表示設定一覧
		/// </summary>
		private Dictionary<ModeType, Action<bool>> modeViewDic = new Dictionary<ModeType, Action<bool>>();

		/// <summary>
		/// OKボタン押下時のイベント
		/// </summary>
		private Action OKEvent = null;

		/// <summary>
		/// Yesボタン押下時のイベント
		/// </summary>
		private Action YesEvent = null;

		/// <summary>
		/// Noボタン押下時のイベント
		/// </summary>
		private Action NoEvent = null;

		/// <summary>
		/// 画面閉じた時のイベント
		/// </summary>
		private Action CloseEvent = null;
		#endregion

		#region 初期化
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Controller(IModel model, IView view)
		{
			if (model == null || view == null) { return; }

			// ビュー設定
			this._view = view;
			this.View.OnRootTweenFinishEvent += this.HandleRootTweenFinish;
			this.View.OnOK += this.HandleOK;
			this.View.OnYes += this.HandleYes;
			this.View.OnNo += this.HandleNo;

			// モデル設定
			this._model = model;
			this.Model.OnMessageChange += this.HandleMessageChange;
			this.Model.OnRankChange += this.HandleRankChange;
			this.Model.OnLevelChange += this.HandleLevelChange;
			this.Model.OnExpChange += this.HandleExpChange;
			this.Model.OnSynchroRemainChange += this.HandleSynchroRemainChange;
			this.Model.OnHitPointChange += this.HandleHitPointChange;
			this.Model.OnHitPointBaseChange += this.HandleHitPointBaseChange;
			this.Model.OnSynchroHitPointChange += this.HandleSynchroHitPointChange;
			this.Model.OnSlotHitPointChange += this.HandleSlotHitPointChange;
			this.Model.OnHitPointUpChange += this.HandleHitPointUpChange;
			this.Model.OnAttackChange += this.HandleAttackChange;
			this.Model.OnAttackBaseChange += this.HandleAttackBaseChange;
			this.Model.OnSynchroAttackChange += this.HandleSynchroAttackChange;
			this.Model.OnSlotAttackChange += this.HandleSlotAttackChange;
			this.Model.OnAttackUpChange += this.HandleAttackUpChange;
			this.Model.OnDefenseChange += this.HandleDefenseChange;
			this.Model.OnDefenseBaseChange += this.HandleDefenseBaseChange;
			this.Model.OnSynchroDefenseChange += this.HandleSynchroDefenseChange;
			this.Model.OnSlotDefenseChange += this.HandleSlotDefenseChange;
			this.Model.OnDefenseUpChange += this.HandleDefenseUpChange;
			this.Model.OnExtraChange += this.HandleExtraChange;
			this.Model.OnExtraBaseChange += this.HandleExtraBaseChange;
			this.Model.OnSynchroExtraChange += this.HandleSynchroExtraChange;
			this.Model.OnSlotExtraChange += this.HandleSlotExtraChange;
			this.Model.OnExtraUpChange += this.HandleExtraUpChange;
			this.Model.OnIsSynchroFusionChange += this.HandleIsSynchroFusionChange;
			this.Model.OnTotalStatusFormatChange += this.HandleTotalStatusFormatChange;
			this.Model.OnBaseStatusFormatChange += this.HandleBaseStatusFormatChange;
			this.Model.OnSynchroFormatChange += this.HandleSynchroFormatChange;
			this.Model.OnSlotFormatChange += this.HandleSlotFormatChange;
			this.Model.OnUpFormatChange += this.HandleUpFormatChange;
			this.Model.OnPendingFormatChange += this.HandlePendingFormatChange;

			// モード表示設定一覧を登録
			this.modeViewDic.Clear();
			this.modeViewDic.Add(ModeType.None, null);
			this.modeViewDic.Add(ModeType.OK, this.View.SetOKButton);
			this.modeViewDic.Add(ModeType.YesNo, this.View.SetYesNoButton);

			// 同期
			this.SetMode(ModeType.None);
			this.SyncMessage();
			this.SyncStatus();
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
			this.modeViewDic.Clear();
			this.OKEvent = null;
			this.YesEvent = null;
			this.NoEvent = null;
			this.CloseEvent = null;
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

		#region セットアップ
		/// <summary>
		/// セットアップ
		/// </summary>
		public void Setup(ModeType modeType, SetupParam param, string message)
		{
			// データセット
			this.SetupParameter(param);

			// メッセージセット
			this.SetMessage(message);

			// モードセット
			this.ChangeMode(modeType);
		}

		/// <summary>
		/// パラメータ設定
		/// </summary>
		public void SetupParameter(SetupParam param)
		{
			if (param == null) { return; }

			this.Model.SetRank(param.RankBefore, param.RankAfter);
			this.Model.SetLevel(param.LevelBefore, param.LevelAfter);
			this.Model.SetExp(param.Exp, param.TotalExp, param.NextLvTotalExp);
			this.Model.SetSynchroRemain(param.SynchroRemainBefore, param.SynchroRemainAfter);
			this.Model.IsSynchroFusion = param.IsSynchroFusion;
			// 生命力
			this.Model.SetHitPoint(param.HitPointBefore, param.HitPointAfter);
			this.Model.SetHitPointBase(param.HitPointBaseBefore, param.HitPointBaseAfter);
			this.Model.SynchroHitPoint = param.SynchroHitPoint;
			this.Model.SetSlotHitPoint(param.SlotHitPointBefore, param.SlotHitPointAfter);
			// 攻撃力
			this.Model.SetAttack(param.AttackBefore, param.AttackAfter);
			this.Model.SetAttackBase(param.AttackBaseBefore, param.AttackBaseAfter);
			this.Model.SynchroAttack = param.SynchroAttack;
			this.Model.SetSlotAttack(param.SlotAttackBefore, param.SlotAttackAfter);
			// 防御力
			this.Model.SetDefense(param.DefenseBefore, param.DefenseAfter);
			this.Model.SetDefenseBase(param.DefenseBaseBefore, param.DefenseBaseAfter);
			this.Model.SynchroDefense = param.SynchroDefense;
			this.Model.SetSlotDefense(param.SlotDefenseBefore, param.SlotDefenseAfter);
			// 特殊能力
			this.Model.SetExtra(param.ExtraBefore, param.ExtraAfter);
			this.Model.SetExtraBase(param.ExtraBaseBefore, param.ExtraBaseAfter);
			this.Model.SynchroExtra = param.SynchroExtra;
			this.Model.SetSlotExtra(param.SlotExtraBefore, param.SlotExtraAfter);
		}
		#endregion

		#region モード
		/// <summary>
		/// モード切替
		/// </summary>
		public void ChangeMode(ModeType mode)
		{
			if (this.NowMode == mode) { return; }
			this.SetMode(mode);
		}

		/// <summary>
		/// モード設定
		/// </summary>
		private void SetMode(ModeType mode)
		{
			// 表示設定
			foreach(KeyValuePair<ModeType, Action<bool>> kvp in this.modeViewDic)
			{
				bool isActive = (kvp.Key == mode) ? true : false;
				if(kvp.Value != null)
				{
					kvp.Value(isActive);
				}
			}

			this.NowMode = mode;
		}
		#endregion

		#region 確認ボタン
		/// <summary>
		/// OKボタン押下時の発生イベント設定
		/// </summary>
		public void SetOkButtonEvent(Action okEvent, bool isClose)
		{
			var okAction = (okEvent != null) ? okEvent : null;
			if(isClose)
			{
				this.OKEvent = () => { this.Close(okAction); };
			}
			else
			{
				this.OKEvent = okAction;
			}
		}

		/// <summary>
		/// Yesボタン押下時の発生イベント設定
		/// </summary>
		public void SetYesButtonEvent(Action yesEvent, bool isClose)
		{
			var action = (yesEvent != null) ? yesEvent : null;
			if (isClose)
			{
				this.YesEvent = () => { this.Close(action); };
			}
			else
			{
				this.YesEvent = action;
			}
		}

		/// <summary>
		/// Noボタン押下時の発生イベント設定
		/// </summary>
		public void SetNoButtonEvent(Action noEvent, bool isClose)
		{
			var action = (noEvent != null) ? noEvent : null;
			if (isClose)
			{
				this.NoEvent = () => { this.Close(action); };
			}
			else
			{
				this.NoEvent = action;
			}
		}

		/// <summary>
		/// OKボタン押下イベントハンドラー
		/// </summary>
		private void HandleOK(object sender, EventArgs e)
		{
			// イベント発生
			if (this.OKEvent == null) { return; }
			this.OKEvent();
		}

		/// <summary>
		/// Yesボタン押下イベントハンドラー
		/// </summary>
		private void HandleYes(object sender, EventArgs e)
		{
			// イベント発生
			if (this.YesEvent == null) { return; }
			this.YesEvent();
		}

		/// <summary>
		/// Noボタン押下イベントハンドラー
		/// </summary>
		private void HandleNo(object sender, EventArgs e)
		{
			// イベント発生
			if (this.NoEvent == null) { return; }
			this.NoEvent();
		}
		#endregion

		#region 閉じる
		/// <summary>
		/// 内部から閉じる時の処理
		/// </summary>
		private void Close(Action closeFinishEvent)
		{
			this.CloseEvent = closeFinishEvent;
			GUIFusionMessage.Close();
		}

		/// <summary>
		/// 画面閉じた時のイベントハンドラー
		/// </summary>
		private void HandleRootTweenFinish(object sender, EventArgs e)
		{
			// イベント発生
			if (this.CloseEvent == null) { return; }
			this.CloseEvent();
			this.CloseEvent = null;
		}
		#endregion

		#region メッセージ
		/// <summary>
		/// メッセージ設定
		/// </summary>
		public void SetMessage(string message)
		{
			if (!this.CanUpdate) { return; }
			this.Model.Message = message;
		}

		/// <summary>
		/// メッセージ変更イベントハンドラー
		/// </summary>
		private void HandleMessageChange(object sender, EventArgs e)
		{
			this.SyncMessage();
		}

		/// <summary>
		/// メッセージ同期
		/// </summary>
		private void SyncMessage()
		{
			if (!this.CanUpdate) { return; }
			this.View.SetMessage(this.Model.Message);
		}
		#endregion

		#region ランク
		/// <summary>
		/// ランク
		/// </summary>
		private void HandleRankChange(object sender, EventArgs e) { this.SyncRank(); }
		private void SyncRank()
		{
			if (!this.CanUpdate) { return; }
			
			// ランク変動チェック
			StatusColor.Type afterColorType = StatusColor.Type.RankNormal;
			bool isUpActive = true;
			if (this.Model.RankBefore < this.Model.RankAfter)
			{
				afterColorType = StatusColor.Type.Up;
			}
			else if (this.Model.RankBefore > this.Model.RankAfter)
			{
				afterColorType = StatusColor.Type.Down;
			}
			else
			{
				isUpActive = false;
			}

			// セット
			this.View.SetRankBefore(this.Model.RankBefore.ToString());
			this.View.SetRankAfter(isUpActive, this.Model.RankAfter.ToString());
			this.View.SetRankColor(StatusColor.Type.RankNormal, afterColorType);
		}
		#endregion

		#region レベル
		/// <summary>
		/// レベル
		/// </summary>
		private void HandleLevelChange(object sender, EventArgs e) { this.SyncLevel(); }
		private void SyncLevel()
		{
			// レベル変動チェック
			StatusColor.Type colorType = StatusColor.Type.RankNormal;
			bool isUpActive = true;
			if (this.Model.LevelBefore < this.Model.LevelAfter)
			{
				colorType = StatusColor.Type.Up;
			}
			else if (this.Model.LevelBefore > this.Model.LevelAfter)
			{
				colorType = StatusColor.Type.Down;
			}
			else
			{
				isUpActive = false;
			}

			// メッセージ取得
			string message = string.Empty;
			bool isLvMax = false;
			if(CharaInfo.IsMaxLevel(this.Model.RankBefore, this.Model.LevelAfter))
			{
				// レベルが最大レベルに達している
				message = MasterData.GetText(TextType.TX437_FusionMessage_MaxLevel);
				isLvMax = true;
			}

			// セット
			this.View.SetLevelBefore(this.Model.LevelBefore.ToString());
			this.View.SetLevelAfter(isUpActive, this.Model.LevelAfter.ToString());
			this.View.SetLevelColor(colorType);
			this.View.SetLevelMessage(message);
			this.View.SetExpActive(!isLvMax);
		}
		#endregion

		#region 経験値
		/// <summary>
		/// 経験値
		/// </summary>
		private void HandleExpChange(object sender, EventArgs e) { this.SyncExp(); }
		private void SyncExp()
		{
			if (!this.CanUpdate) { return; }

			// バーの設定
			float value = this.Model.GetExpSlider();
			this.View.SetExpBar(value);
		}
		#endregion

		#region シンクロ可能回数
		/// <summary>
		/// シンクロ可能回数
		/// </summary>
		private void HandleSynchroRemainChange(object sender, EventArgs e) { this.SyncSynchroRemain(); }
		private void SyncSynchroRemain()
		{
			if (!this.CanUpdate) { return; }

			// シンクロ可能回数変動チェック
			StatusColor.Type colorType = StatusColor.Type.RankNormal;
			bool isUpActive = true;
			if (this.Model.SynchroRemainBefore < this.Model.SynchroRemainAfter)
			{
				colorType = StatusColor.Type.Up;
			}
			else if (this.Model.SynchroRemainBefore > this.Model.SynchroRemainAfter)
			{
				colorType = StatusColor.Type.Down;
			}
			else
			{
				isUpActive = false;
			}

			// セット
			this.View.SetSynchroRemainBefore(this.Model.SynchroRemainBefore.ToString());
			this.View.SetSynchroRemainAfter(isUpActive, this.Model.SynchroRemainAfter.ToString());
			this.View.SetSynchroRemainColor(colorType);
		}
		#endregion

		#region 生命力
		/// <summary>
		/// 生命力
		/// </summary>
		private void HandleHitPointChange(object sender, EventArgs e)
		{
			this.SyncHitPoint();

			// 変化量セット
			this.Model.HitPointUp = this.Model.HitPointAfter - this.Model.HitPointBefore;
		}
		private void SyncHitPoint()
		{
			if (!this.CanUpdate) { return; }

			// 色と増減アイコンの設定
			StatusColor.Type colorType = StatusColor.Type.StatusNormal;
			FusionMessageView.StatusIcon.Type iconType = FusionMessageView.StatusIcon.Type.None;
			if(this.Model.IsSynchroFusion)
			{
				// シンクロ合成時
				if(this.Model.SynchroHitPoint < CharaInfo.SynchroMaxHitPoint)
				{
					// 変動の可能性がある
					colorType = StatusColor.Type.Random;
				}
				else
				{
					colorType = StatusColor.Type.StatusNormal;
				}

				iconType = FusionMessageView.StatusIcon.Type.None;
			}
			else
			{
				if(this.Model.HitPointBefore < this.Model.HitPointAfter)
				{
					colorType = StatusColor.Type.Up;
					iconType = FusionMessageView.StatusIcon.Type.Up;
				}
				else if(this.Model.HitPointBefore > this.Model.HitPointAfter)
				{
					colorType = StatusColor.Type.Down;
					iconType = FusionMessageView.StatusIcon.Type.Down;
				}
				else
				{
					colorType = StatusColor.Type.StatusNormal;
					iconType = FusionMessageView.StatusIcon.Type.None;
				}
			}
			// セット
			this.View.SetHitPointTotal(this.Model.HitPointAfter, this.Model.TotalStatusFormat, colorType);
			this.View.SetHitPointIcon(iconType);
		}

		/// <summary>
		/// 生命力ベース
		/// </summary>
		private void HandleHitPointBaseChange(object sender, EventArgs e) { this.SyncHitPointBase(); }
		private void SyncHitPointBase()
		{
			if (!this.CanUpdate) { return; }

			// 色取得
			StatusColor.Type colorType = StatusColor.Type.StatusNormal;
			if (this.Model.HitPointBaseBefore < this.Model.HitPointBaseAfter)
			{
				colorType = StatusColor.Type.Up;
			}
			else if (this.Model.HitPointBaseBefore > this.Model.HitPointBaseAfter)
			{
				colorType = StatusColor.Type.Down;
			}
			else
			{
				colorType = StatusColor.Type.StatusNormal;
			}
			// セット
			this.View.SetHitPointBase(this.Model.HitPointBaseAfter, this.Model.BaseStatusFormat, colorType);
		}

		/// <summary>
		/// 生命力シンクロ
		/// </summary>
		private void HandleSynchroHitPointChange(object sender, EventArgs e)
		{
			this.SyncSynchroHitPoint();
			this.SyncHitPoint();
			this.SyncHitPointUp();
		}
		private void SyncSynchroHitPoint()
		{
			if (!this.CanUpdate) { return; }

			// 色取得
			StatusColor.Type colorType = StatusColor.Type.StatusNormal;
			if(this.Model.IsSynchroFusion)
			{
				// シンクロ合成時
				if (this.Model.SynchroHitPoint < CharaInfo.SynchroMaxHitPoint)
				{
					// 変動の可能性がある
					colorType = StatusColor.Type.Random;
				}
				else
				{
					colorType = StatusColor.Type.StatusNormal;
				}
			}
			// セット
			this.View.SetSynchroHitPoint(this.Model.SynchroHitPoint, this.Model.SynchroFormat, colorType);
		}

		/// <summary>
		/// 生命力スロット
		/// </summary>
		private void HandleSlotHitPointChange(object sender, EventArgs e) { this.SyncSlotHitPoint(); }
		private void SyncSlotHitPoint()
		{
			if (!this.CanUpdate) { return; }

			// 色取得
			StatusColor.Type colorType = StatusColor.Type.StatusNormal;
			if (this.Model.SlotHitPointBefore < this.Model.SlotHitPointAfter)
			{
				colorType = StatusColor.Type.Up;
			}
			else if (this.Model.SlotHitPointBefore > this.Model.SlotHitPointAfter)
			{
				colorType = StatusColor.Type.Down;
			}
			else
			{
				colorType = StatusColor.Type.StatusNormal;
			}
			// セット
			this.View.SetSlotHitPoint(this.Model.SlotHitPointAfter, this.Model.SlotFormat, colorType);
		}

		/// <summary>
		/// 生命力増加分
		/// </summary>
		private void HandleHitPointUpChange(object sender, EventArgs e) { this.SyncHitPointUp(); }
		private void SyncHitPointUp()
		{
			if (!this.CanUpdate) { return; }

			// 色と表示するフォーマットを取得する
			string format = this.Model.UpFormat;
			StatusColor.Type colorType = StatusColor.Type.StatusNormal;
			if (this.Model.IsSynchroFusion)
			{
				// シンクロ合成時
				if (this.Model.SynchroHitPoint < CharaInfo.SynchroMaxHitPoint)
				{
					// 変動の可能性がある
					colorType = StatusColor.Type.Random;
					format = this.Model.PendingFormat;
				}
				else
				{
					// 変化しない場合は非表示
					format = string.Empty;
				}
			}
			else
			{
				if (this.Model.HitPointUp > 0)
				{
					colorType = StatusColor.Type.Up;
				}
				else if (this.Model.HitPointUp < 0)
				{
					colorType = StatusColor.Type.Down;
				}
				else
				{
					// 変化しない場合は非表示
					format = string.Empty;
				}
			}

			// セット
			this.View.SetHitPointUp(Math.Abs(this.Model.HitPointUp), format, colorType);
		}
		#endregion

		#region 攻撃力
		/// <summary>
		/// 攻撃力
		/// </summary>
		private void HandleAttackChange(object sender, EventArgs e)
		{
			this.SyncAttack();

			// 変化量セット
			this.Model.AttackUp = this.Model.AttackAfter - this.Model.AttackBefore;
		}
		private void SyncAttack()
		{
			if (!this.CanUpdate) { return; }

			// 色と増減アイコンの設定
			StatusColor.Type colorType = StatusColor.Type.StatusNormal;
			FusionMessageView.StatusIcon.Type iconType = FusionMessageView.StatusIcon.Type.None;
			if (this.Model.IsSynchroFusion)
			{
				// シンクロ合成時
				if (this.Model.SynchroAttack < CharaInfo.SynchroMaxAttack)
				{
					// 変動の可能性がある
					colorType = StatusColor.Type.Random;
				}
				else
				{
					colorType = StatusColor.Type.StatusNormal;
				}

				iconType = FusionMessageView.StatusIcon.Type.None;
			}
			else
			{
				if (this.Model.AttackBefore < this.Model.AttackAfter)
				{
					colorType = StatusColor.Type.Up;
					iconType = FusionMessageView.StatusIcon.Type.Up;
				}
				else if (this.Model.AttackBefore > this.Model.AttackAfter)
				{
					colorType = StatusColor.Type.Down;
					iconType = FusionMessageView.StatusIcon.Type.Down;
				}
				else
				{
					colorType = StatusColor.Type.StatusNormal;
					iconType = FusionMessageView.StatusIcon.Type.None;
				}
			}
			// セット
			this.View.SetAttackTotal(this.Model.AttackAfter, this.Model.TotalStatusFormat, colorType);
			this.View.SetAttackIcon(iconType);
		}

		/// <summary>
		/// 攻撃力ベース
		/// </summary>
		private void HandleAttackBaseChange(object sender, EventArgs e) { this.SyncAttackBase(); }
		private void SyncAttackBase()
		{
			if (!this.CanUpdate) { return; }

			// 色取得
			StatusColor.Type colorType = StatusColor.Type.StatusNormal;
			if (this.Model.AttackBaseBefore < this.Model.AttackBaseAfter)
			{
				colorType = StatusColor.Type.Up;
			}
			else if (this.Model.AttackBaseBefore > this.Model.AttackBaseAfter)
			{
				colorType = StatusColor.Type.Down;
			}
			else
			{
				colorType = StatusColor.Type.StatusNormal;
			}
			// セット
			this.View.SetAttackBase(this.Model.AttackBaseAfter, this.Model.BaseStatusFormat, colorType);
		}

		/// <summary>
		/// 攻撃力シンクロ
		/// </summary>
		private void HandleSynchroAttackChange(object sender, EventArgs e)
		{
			this.SyncSynchroAttack();
			this.SyncAttack();
			this.SyncAttackUp();
		}
		private void SyncSynchroAttack()
		{
			if (!this.CanUpdate) { return; }

			// 色取得
			StatusColor.Type colorType = StatusColor.Type.StatusNormal;
			if (this.Model.IsSynchroFusion)
			{
				// シンクロ合成時
				if (this.Model.SynchroAttack < CharaInfo.SynchroMaxAttack)
				{
					// 変動の可能性がある
					colorType = StatusColor.Type.Random;
				}
				else
				{
					colorType = StatusColor.Type.StatusNormal;
				}
			}
			// セット
			this.View.SetSynchroAttack(this.Model.SynchroAttack, this.Model.SynchroFormat, colorType);
		}

		/// <summary>
		/// 攻撃力スロット
		/// </summary>
		private void HandleSlotAttackChange(object sender, EventArgs e) { this.SyncSlotAttack(); }
		private void SyncSlotAttack()
		{
			if (!this.CanUpdate) { return; }

			// 色取得
			StatusColor.Type colorType = StatusColor.Type.StatusNormal;
			if (this.Model.SlotAttackBefore < this.Model.SlotAttackAfter)
			{
				colorType = StatusColor.Type.Up;
			}
			else if (this.Model.SlotAttackBefore > this.Model.SlotAttackAfter)
			{
				colorType = StatusColor.Type.Down;
			}
			else
			{
				colorType = StatusColor.Type.StatusNormal;
			}
			// セット
			this.View.SetSlotAttack(this.Model.SlotAttackAfter, this.Model.SlotFormat, colorType);
		}

		/// <summary>
		/// 攻撃力増加分
		/// </summary>
		private void HandleAttackUpChange(object sender, EventArgs e) { this.SyncAttackUp(); }
		private void SyncAttackUp()
		{
			if (!this.CanUpdate) { return; }

			// 色と表示するフォーマットを取得する
			string format = this.Model.UpFormat;
			StatusColor.Type colorType = StatusColor.Type.StatusNormal;
			if (this.Model.IsSynchroFusion)
			{
				// シンクロ合成時
				if (this.Model.SynchroAttack < CharaInfo.SynchroMaxAttack)
				{
					// 変動の可能性がある
					colorType = StatusColor.Type.Random;
					format = this.Model.PendingFormat;
				}
				else
				{
					// 変化しない場合は非表示
					format = string.Empty;
				}
			}
			else
			{
				if (this.Model.AttackUp > 0)
				{
					colorType = StatusColor.Type.Up;
				}
				else if (this.Model.AttackUp < 0)
				{
					colorType = StatusColor.Type.Down;
				}
				else
				{
					// 変化しない場合は非表示
					format = string.Empty;
				}
			}

			// セット
			this.View.SetAttackUp(Math.Abs(this.Model.AttackUp), format, colorType);
		}
		#endregion

		#region 防御力
		/// <summary>
		/// 防御力
		/// </summary>
		private void HandleDefenseChange(object sender, EventArgs e)
		{
			this.SyncDefense();

			// 変化量セット
			this.Model.DefenseUp = this.Model.DefenseAfter - this.Model.DefenseBefore;
		}
		private void SyncDefense()
		{
			if (!this.CanUpdate) { return; }

			// 色と増減アイコンの設定
			StatusColor.Type colorType = StatusColor.Type.StatusNormal;
			FusionMessageView.StatusIcon.Type iconType = FusionMessageView.StatusIcon.Type.None;
			if (this.Model.IsSynchroFusion)
			{
				// シンクロ合成時
				if (this.Model.SynchroDefense < CharaInfo.SynchroMaxDefense)
				{
					// 変動の可能性がある
					colorType = StatusColor.Type.Random;
				}
				else
				{
					colorType = StatusColor.Type.StatusNormal;
				}

				iconType = FusionMessageView.StatusIcon.Type.None;
			}
			else
			{
				if (this.Model.DefenseBefore < this.Model.DefenseAfter)
				{
					colorType = StatusColor.Type.Up;
					iconType = FusionMessageView.StatusIcon.Type.Up;
				}
				else if (this.Model.DefenseBefore > this.Model.DefenseAfter)
				{
					colorType = StatusColor.Type.Down;
					iconType = FusionMessageView.StatusIcon.Type.Down;
				}
				else
				{
					colorType = StatusColor.Type.StatusNormal;
					iconType = FusionMessageView.StatusIcon.Type.None;
				}
			}
			// セット
			this.View.SetDefenseTotal(this.Model.DefenseAfter, this.Model.TotalStatusFormat, colorType);
			this.View.SetDefenseIcon(iconType);
		}

		/// <summary>
		/// 防御力ベース
		/// </summary>
		private void HandleDefenseBaseChange(object sender, EventArgs e) { this.SyncDefenseBase(); }
		private void SyncDefenseBase()
		{
			if (!this.CanUpdate) { return; }

			// 色取得
			StatusColor.Type colorType = StatusColor.Type.StatusNormal;
			if (this.Model.DefenseBaseBefore < this.Model.DefenseBaseAfter)
			{
				colorType = StatusColor.Type.Up;
			}
			else if (this.Model.DefenseBaseBefore > this.Model.DefenseBaseAfter)
			{
				colorType = StatusColor.Type.Down;
			}
			else
			{
				colorType = StatusColor.Type.StatusNormal;
			}
			// セット
			this.View.SetDefenseBase(this.Model.DefenseBaseAfter, this.Model.BaseStatusFormat, colorType);
		}

		/// <summary>
		/// 防御力シンクロ
		/// </summary>
		private void HandleSynchroDefenseChange(object sender, EventArgs e)
		{
			this.SyncSynchroDefense();
			this.SyncDefense();
			this.SyncDefenseUp();
		}
		private void SyncSynchroDefense()
		{
			if (!this.CanUpdate) { return; }

			// 色取得
			StatusColor.Type colorType = StatusColor.Type.StatusNormal;
			if (this.Model.IsSynchroFusion)
			{
				// シンクロ合成時
				if (this.Model.SynchroDefense < CharaInfo.SynchroMaxDefense)
				{
					// 変動の可能性がある
					colorType = StatusColor.Type.Random;
				}
				else
				{
					colorType = StatusColor.Type.StatusNormal;
				}
			}
			// セット
			this.View.SetSynchroDefense(this.Model.SynchroDefense, this.Model.SynchroFormat, colorType);
		}

		/// <summary>
		/// 防御力スロット
		/// </summary>
		private void HandleSlotDefenseChange(object sender, EventArgs e) { this.SyncSlotDefense(); }
		private void SyncSlotDefense()
		{
			if (!this.CanUpdate) { return; }

			// 色取得
			StatusColor.Type colorType = StatusColor.Type.StatusNormal;
			if (this.Model.SlotDefenseBefore < this.Model.SlotDefenseAfter)
			{
				colorType = StatusColor.Type.Up;
			}
			else if (this.Model.SlotDefenseBefore > this.Model.SlotDefenseAfter)
			{
				colorType = StatusColor.Type.Down;
			}
			else
			{
				colorType = StatusColor.Type.StatusNormal;
			}
			// セット
			this.View.SetSlotDefense(this.Model.SlotDefenseAfter, this.Model.SlotFormat, colorType);
		}

		/// <summary>
		/// 防御力増加分
		/// </summary>
		private void HandleDefenseUpChange(object sender, EventArgs e) { this.SyncDefenseUp(); }
		private void SyncDefenseUp()
		{
			if (!this.CanUpdate) { return; }

			// 色と表示するフォーマットを取得する
			string format = this.Model.UpFormat;
			StatusColor.Type colorType = StatusColor.Type.StatusNormal;
			if (this.Model.IsSynchroFusion)
			{
				// シンクロ合成時
				if (this.Model.SynchroDefense < CharaInfo.SynchroMaxDefense)
				{
					// 変動の可能性がある
					colorType = StatusColor.Type.Random;
					format = this.Model.PendingFormat;
				}
				else
				{
					// 変化しない場合は非表示
					format = string.Empty;
				}
			}
			else
			{
				if (this.Model.DefenseUp > 0)
				{
					colorType = StatusColor.Type.Up;
				}
				else if (this.Model.DefenseUp < 0)
				{
					colorType = StatusColor.Type.Down;
				}
				else
				{
					// 変化しない場合は非表示
					format = string.Empty;
				}
			}

			// セット
			this.View.SetDefenseUp(Math.Abs(this.Model.DefenseUp), format, colorType);
		}
		#endregion

		#region 特殊能力
		/// <summary>
		/// 特殊能力
		/// </summary>
		private void HandleExtraChange(object sender, EventArgs e)
		{
			this.SyncExtra();

			// 変化量セット
			this.Model.ExtraUp = this.Model.ExtraAfter - this.Model.ExtraBefore;
		}
		private void SyncExtra()
		{
			if (!this.CanUpdate) { return; }

			// 色と増減アイコンの設定
			StatusColor.Type colorType = StatusColor.Type.StatusNormal;
			FusionMessageView.StatusIcon.Type iconType = FusionMessageView.StatusIcon.Type.None;
			if (this.Model.IsSynchroFusion)
			{
				// シンクロ合成時
				if (this.Model.SynchroExtra < CharaInfo.SynchroMaxExtra)
				{
					// 変動の可能性がある
					colorType = StatusColor.Type.Random;
				}
				else
				{
					colorType = StatusColor.Type.StatusNormal;
				}

				iconType = FusionMessageView.StatusIcon.Type.None;
			}
			else
			{
				if (this.Model.ExtraBefore < this.Model.ExtraAfter)
				{
					colorType = StatusColor.Type.Up;
					iconType = FusionMessageView.StatusIcon.Type.Up;
				}
				else if (this.Model.ExtraBefore > this.Model.ExtraAfter)
				{
					colorType = StatusColor.Type.Down;
					iconType = FusionMessageView.StatusIcon.Type.Down;
				}
				else
				{
					colorType = StatusColor.Type.StatusNormal;
					iconType = FusionMessageView.StatusIcon.Type.None;
				}
			}
			// セット
			this.View.SetExtraTotal(this.Model.ExtraAfter, this.Model.TotalStatusFormat, colorType);
			this.View.SetExtraIcon(iconType);
		}

		/// <summary>
		/// 特殊能力ベース
		/// </summary>
		private void HandleExtraBaseChange(object sender, EventArgs e) { this.SyncExtraBase(); }
		private void SyncExtraBase()
		{
			if (!this.CanUpdate) { return; }

			// 色取得
			StatusColor.Type colorType = StatusColor.Type.StatusNormal;
			if (this.Model.ExtraBaseBefore < this.Model.ExtraBaseAfter)
			{
				colorType = StatusColor.Type.Up;
			}
			else if (this.Model.ExtraBaseBefore > this.Model.ExtraBaseAfter)
			{
				colorType = StatusColor.Type.Down;
			}
			else
			{
				colorType = StatusColor.Type.StatusNormal;
			}
			// セット
			this.View.SetExtraBase(this.Model.ExtraBaseAfter, this.Model.BaseStatusFormat, colorType);
		}

		/// <summary>
		/// 特殊能力シンクロ
		/// </summary>
		private void HandleSynchroExtraChange(object sender, EventArgs e)
		{
			this.SyncSynchroExtra();
			this.SyncExtra();
			this.SyncExtraUp();
		}
		private void SyncSynchroExtra()
		{
			if (!this.CanUpdate) { return; }

			// 色取得
			StatusColor.Type colorType = StatusColor.Type.StatusNormal;
			if (this.Model.IsSynchroFusion)
			{
				// シンクロ合成時
				if (this.Model.SynchroExtra < CharaInfo.SynchroMaxExtra)
				{
					// 変動の可能性がある
					colorType = StatusColor.Type.Random;
				}
				else
				{
					colorType = StatusColor.Type.StatusNormal;
				}
			}
			// セット
			this.View.SetSynchroExtra(this.Model.SynchroExtra, this.Model.SynchroFormat, colorType);
		}

		/// <summary>
		/// 特殊能力スロット
		/// </summary>
		private void HandleSlotExtraChange(object sender, EventArgs e) { this.SyncSlotExtra(); }
		private void SyncSlotExtra()
		{
			if (!this.CanUpdate) { return; }

			// 色取得
			StatusColor.Type colorType = StatusColor.Type.StatusNormal;
			if (this.Model.SlotExtraBefore < this.Model.SlotExtraAfter)
			{
				colorType = StatusColor.Type.Up;
			}
			else if (this.Model.SlotExtraBefore > this.Model.SlotExtraAfter)
			{
				colorType = StatusColor.Type.Down;
			}
			else
			{
				colorType = StatusColor.Type.StatusNormal;
			}
			// セット
			this.View.SetSlotExtra(this.Model.SlotExtraAfter, this.Model.SlotFormat, colorType);
		}

		/// <summary>
		/// 特殊能力増加分
		/// </summary>
		private void HandleExtraUpChange(object sender, EventArgs e) { this.SyncExtraUp(); }
		private void SyncExtraUp()
		{
			if (!this.CanUpdate) { return; }

			// 色と表示するフォーマットを取得する
			string format = this.Model.UpFormat;
			StatusColor.Type colorType = StatusColor.Type.StatusNormal;
			if (this.Model.IsSynchroFusion)
			{
				// シンクロ合成時
				if (this.Model.SynchroExtra < CharaInfo.SynchroMaxExtra)
				{
					// 変動の可能性がある
					colorType = StatusColor.Type.Random;
					format = this.Model.PendingFormat;
				}
				else
				{
					// 変化しない場合は非表示
					format = string.Empty;
				}
			}
			else
			{
				if (this.Model.ExtraUp > 0)
				{
					colorType = StatusColor.Type.Up;
				}
				else if (this.Model.ExtraUp < 0)
				{
					colorType = StatusColor.Type.Down;
				}
				else
				{
					// 変化しない場合は非表示
					format = string.Empty;
				}
			}

			// セット
			this.View.SetExtraUp(Math.Abs(this.Model.ExtraUp), format, colorType);
		}
		#endregion

		#region シンクロ合成フラグ
		/// <summary>
		/// シンクロ合成かどうか
		/// </summary>
		private void  HandleIsSynchroFusionChange(object sender, EventArgs e)
		{
			this.SyncIsSynchroFusion();
		}
		private void SyncIsSynchroFusion()
		{
			this.SyncTotalStatus();
			this.SyncSynchroStatus();
			this.SyncUpStatus();
		}
		#endregion

		#region 各ステータス同期
		/// <summary>
		/// 全てのステータス同期
		/// </summary>
		private void SyncStatus()
		{
			this.SyncRank();
			this.SyncLevel();
			this.SyncExp();
			this.SyncSynchroRemain();
			this.SyncHitPoint();
			this.SyncHitPointBase();
			this.SyncSynchroHitPoint();
			this.SyncSlotHitPoint();
			this.SyncHitPointUp();
			this.SyncAttack();
			this.SyncAttackBase();
			this.SyncSynchroAttack();
			this.SyncSlotAttack();
			this.SyncAttackUp();
			this.SyncDefense();
			this.SyncDefenseBase();
			this.SyncSynchroDefense();
			this.SyncSlotDefense();
			this.SyncDefenseUp();
			this.SyncExtra();
			this.SyncExtraBase();
			this.SyncSynchroExtra();
			this.SyncSlotExtra();
			this.SyncExtraUp();
		}

		/// <summary>
		/// 合計値の各ステータス同期
		/// </summary>
		private void SyncTotalStatus()
		{
			this.SyncHitPoint();
			this.SyncAttack();
			this.SyncDefense();
			this.SyncExtra();
		}

		/// <summary>
		/// ベース値の各ステータス同期
		/// </summary>
		private void SyncBaseStatus()
		{
			this.SyncHitPointBase();
			this.SyncAttackBase();
			this.SyncDefenseBase();
			this.SyncExtraBase();
		}

		/// <summary>
		/// シンクロ値の各ステータス同期
		/// </summary>
		private void SyncSynchroStatus()
		{
			this.SyncSynchroHitPoint();
			this.SyncSynchroAttack();
			this.SyncSynchroDefense();
			this.SyncSynchroExtra();
		}

		/// <summary>
		/// スロット値の各ステータス同期
		/// </summary>
		private void SyncSlotStatus()
		{
			this.SyncSlotHitPoint();
			this.SyncSlotAttack();
			this.SyncSlotDefense();
			this.SyncSlotExtra();
		}

		/// <summary>
		/// 増加値の各ステータス同期
		/// </summary>
		private void SyncUpStatus()
		{
			this.SyncHitPointUp();
			this.SyncAttackUp();
			this.SyncDefenseUp();
			this.SyncExtraUp();
		}
		#endregion

		#region フォーマット
		/// <summary>
		/// 合計値フォーマット
		/// </summary>
		private void HandleTotalStatusFormatChange(object sender, EventArgs e) { this.SyncTotalStatusFormat(); }
		private void SyncTotalStatusFormat()
		{
			this.SyncTotalStatus();
		}

		/// <summary>
		/// ベース値フォーマット
		/// </summary>
		private void HandleBaseStatusFormatChange(object sender, EventArgs e) { this.SyncHitPointBase(); }
		private void SyncBaseStatusFormat()
		{
			this.SyncBaseStatus();
		}

		/// <summary>
		/// シンクロ値フォーマット
		/// </summary>
		private void HandleSynchroFormatChange(object sender, EventArgs e) { this.SyncSynchroHitPoint(); }
		private void SyncSynchroFormat()
		{
			this.SyncSynchroStatus();
		}

		/// <summary>
		/// スロット値フォーマット
		/// </summary>
		private void HandleSlotFormatChange(object sender, EventArgs e) { this.SyncSlotFormat(); }
		private void SyncSlotFormat()
		{
			this.SyncSlotHitPoint();
		}

		/// <summary>
		/// 増加値フォーマット
		/// </summary>
		private void HandleUpFormatChange(object sender, EventArgs e) { this.SyncUpStatus(); }
		private void SyncUpFormat()
		{
			this.SyncUpStatus();
		}

		/// <summary>
		/// 未確定フォーマット
		/// </summary>
		private void HandlePendingFormatChange(object sender, EventArgs e) { }
		private void SyncPendingFormat()
		{
			this.SyncUpStatus();
		}
		#endregion
	}
}