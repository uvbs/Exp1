/// <summary>
/// 強化スロット制御
/// 
/// 2016/03/02
/// </summary>
using System;
using UnityEngine;
using System.Collections.Generic;

using Scm.Common.GameParameter;

namespace XUI.PowerupSlot
{
	#region 定義
	/// <summary>
	/// OKイベント引数
	/// </summary>
	public class OKEventArgs : EventArgs
	{
		public ulong BaseCharaUUID { get; set; }
		public List<ulong> SlotCharaUUIDList { get; set; }
	}
	/// <summary>
	/// スロット試算イベント引数
	/// </summary>
	public class SlotCalcEventArgs : EventArgs
	{
		public ulong BaseCharaUUID { get; set; }
		public List<ulong> SlotCharaUUIDList { get; set; }
	}
	/// <summary>
	/// ベースキャラスロット情報取得イベント引数
	/// </summary>
	public class GetBaseCharaSlotEventArgs : EventArgs
	{
		public ulong BaseCharaUUID { get; set; }
	}
	/// <summary>
	/// プレイヤーキャラクター情報取得イベント引数
	/// </summary>
	public class PlayerCharacterEventArgs : EventArgs
	{
		public ulong BaseCharaUUID { get; set; }
	}

	/// <summary>
	/// 選択モード
	/// </summary>
	public enum SelectMode : byte
	{
		Base,		// ベースキャラ選択中
		Slot,		// スロット選択中
	}
	#endregion

	/// <summary>
	/// 強化スロット制御インターフェイス
	/// </summary>
	public interface IController
	{
		#region 初期化
		/// <summary>
		/// 初期化
		/// </summary>
		void Setup();

		/// <summary>
		/// ステータス情報設定
		/// </summary>
		void SetupStatusInfo(int haveMoney);

		/// <summary>
		/// キャラリスト枠を設定
		/// </summary>
		void SetupCapacity(int capacity, int count);

		/// <summary>
		/// キャラリストの中身設定
		/// </summary>
		void SetupItem(List<CharaInfo> list);
		#endregion

		#region 破棄
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

		#region OKボタンイベント
		#region OK実行
		/// <summary>
		/// OKボタンイベント
		/// </summary>
		event EventHandler<OKEventArgs> OnOK;

		/// <summary>
		/// OKボタン結果
		/// </summary>
		void OKResult(int money, int price, int addOnCharge, CharaInfo info);
		#endregion

		#region プレイヤーキャラクター情報取得イベント
		/// <summary>
		/// プレイヤーキャラクター情報取得イベント
		/// </summary>
		event EventHandler<PlayerCharacterEventArgs> OnPlayerCharacter;

		/// <summary>
		/// プレイヤーキャラクター情報取得結果
		/// </summary>
		void PlayerCharacterResult(CharaInfo info, List<CharaInfo> slotList);
		#endregion

		#endregion

		#region ベースキャラスロット取得イベント
		/// <summary>
		/// ベースキャラスロット取得イベント
		/// </summary>
		event EventHandler<GetBaseCharaSlotEventArgs> OnGetBaseCharaSlot;

		/// <summary>
		/// ベースキャラスロット取得結果
		/// </summary>
		void GetBaseCharaSlotResult(ulong baseCharaUUID, int hitPointBonus, int attackBonus, int defenseBonus, int extraBonus, List<PowerupSlotCharaInfo> slotInfoList);
		#endregion

		#region スロットリスト設定
		/// <summary>
		/// スロット試算イベント
		/// </summary>
		event EventHandler<SlotCalcEventArgs> OnSlotCalc;

		/// <summary>
		/// スロット試算結果
		/// </summary>
		void SlotCalcResult(int money, int hitPointBonus, int attackBonus, int defenseBonus, int extraBonus, int price, int addOnCharge);
		#endregion
	}

	/// <summary>
	/// 強化スロット制御
	/// </summary>
	public class Controller : IController
	{
		#region 文字列
		string ScreenTitle { get { return MasterData.GetText(TextType.TX363_Slot_ScreenTitle); } }
		string BaseHelpMessage { get { return MasterData.GetText(TextType.TX364_Slot_Base_HelpMessage); } }
		string SlotHelpMessage { get { return MasterData.GetText(TextType.TX365_Slot_Slot_HelpMessage); } }
		string OKMessage { get { return MasterData.GetText(TextType.TX366_Slot_OK_Message); } }
		string SlotMessage { get { return MasterData.GetText(TextType.TX367_Slot_Slot_Message); } }
		#endregion

		#region フィールド＆プロパティ
		// モデル
		readonly IModel _model;
		IModel Model { get { return _model; } }
		// ビュー
		readonly IView _view;
		IView View { get { return _view; } }
		/// <summary>
		/// 更新できる状態かどうか
		/// </summary>
		bool CanUpdate
		{
			get
			{
				if (this.Model == null) return false;
				if (this.View == null) return false;
				return true;
			}
		}

		// キャラリスト
		readonly GUICharaPageList _charaList;
		GUICharaPageList CharaList { get { return _charaList; } }

		// ベースキャラ
		readonly GUICharaItem _baseChara;
		GUICharaItem BaseChara { get { return _baseChara; } }
		// ベースキャラUUID
		ulong BaseCharaUUID
		{
			get
			{
				ulong uuid = 0;
				var info = this.BaseCharaInfo;
				if (info != null) uuid = info.UUID;
				return uuid;
			}
		}
		// ベースキャラのキャラ情報
		CharaInfo BaseCharaInfo { get { return (this.BaseChara != null ? this.BaseChara.GetCharaInfo() : null); } }
		// ベースキャラが空かどうか
		bool IsEmptyBaseChara { get { return (this.BaseCharaInfo == null ? true : false); } }

		// スロットリスト
		readonly GUICharaSlotList _slotList;
		GUICharaSlotList SlotList { get { return _slotList; } }
		// スロットキャラUUIDリスト
		List<ulong> SlotCharaUUIDList
		{
			get
			{
				var list = new List<ulong>();
				if (this.SlotList != null)
				{
					var infoList = this.SlotList.GetCharaInfoList();
					if (infoList != null)
					{
						infoList.ForEach(t => { list.Add(t.UUID); });
					}
				}
				return list;
			}
		}

		// 現在の選択モード
		SelectMode SelectMode { get { return this.IsEmptyBaseChara ? SelectMode.Base : SelectMode.Slot; } }

		// 加工していない大元のキャラリスト
		List<CharaInfo> RawList { get; set; }
		// ベースキャラのスロット情報リスト
		List<PowerupSlotCharaInfo> BaseCharaSlotList { get; set; }
		// ベースキャラが同期できるかどうか
		bool CanSyncBaseChara { get; set; }

		// シリアライズされていないメンバーの初期化
		void MemberInit()
		{
			this.RawList = new List<CharaInfo>();
			this.BaseCharaSlotList = new List<PowerupSlotCharaInfo>();
			this.CanSyncBaseChara = true;
		}
		#endregion

		#region 初期化
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Controller(IModel model, IView view, GUICharaPageList charaList, GUICharaItem baseChara, GUICharaSlotList slotList)
		{
			if (model == null || view == null) return;

			this._charaList = charaList;
			this._baseChara = baseChara;
			this._slotList = slotList;

			this.MemberInit();

			// ビュー設定
			this._view = view;
			this.View.OnHome += this.HandleHome;
			this.View.OnClose += this.HandleClose;
			this.View.OnOK += this.HandleOK;
			this.View.OnAddSlot += this.HandleAddSlot;

			// モデル設定
			this._model = model;
			// 生命力
			this.Model.OnHitPointChange += this.HandleHitPointChange;
			this.Model.OnHitPointCalcChange +=this.HandleHitPointCalcChange;
			// 攻撃力
			this.Model.OnAttackChange += this.HandleAttackChange;
			this.Model.OnAttackCalcChange += this.HandleAttackCalcChange;
			// 防御力
			this.Model.OnDefenseChange += this.HandleDefenseChange;
			this.Model.OnDefenseCalcChange += this.HandleDefenseCalcChange;
			// 特殊能力
			this.Model.OnExtraChange += this.HandleExtraChange;
			this.Model.OnExtraCalcChange += this.HandleExtraCalcChange;
			// ステータス表示
			this.Model.OnStatusFormatChange += this.HandleStatusFormatChange;
			this.Model.OnStatusCalcFormatChange += this.HandleStatusCalcFormatChange;
			// スロット数
			this.Model.OnSlotCountChange += this.HandleSlotCountChange;
			this.Model.OnSlotCapacityChange += this.HandleSlotCapacityChange;
			this.Model.OnSlotNumFormatChange += this.HandleSlotNumFormatChange;
			// 所持金
			this.Model.OnHaveMoneyChange += this.HandleHaveMoneyChange;
			this.Model.OnHaveMoneyFormatChange += this.HandleHaveMoneyFormatChange;
			// 費用
			this.Model.OnNeedMoneyChange += this.HandleNeedMoneyChange;
			this.Model.OnNeedMoneyFormatChange += this.HandleNeedMoneyFormatChange;
			// 追加料金
			this.Model.OnAddOnChargeChange += this.HandleAddOnChargeChange;
			this.Model.OnAddOnChargeFormatChange += this.HandleAddOnChargeFormatChange;

			// 同期
			this.SyncStatus();
			this.SyncStatusCalc();
			this.SyncSlotNum();
			this.SyncHaveMoney();
			this.SyncNeedMoney();
			this.SyncAddOnCharge();

			// イベント登録
			if (this.CharaList != null)
			{
				this.CharaList.OnItemChangeEvent += this.HandleCharaListItemChangeEvent;
				this.CharaList.OnItemClickEvent += this.HandleCharaListItemClickEvent;
				this.CharaList.OnItemLongPressEvent += this.HandleCharaListItemLongPressEvent;
				this.CharaList.OnUpdateItemsEvent += this.HandleCharaListUpdateItemsEvent;
			}
			if (this.BaseChara != null)
			{
				this.BaseChara.OnItemChangeEvent += this.HandleBeseCharaItemChangeEvent;
				this.BaseChara.OnItemClickEvent += this.HandleBaseCharaItemClickEvent;
				this.BaseChara.OnItemLongPressEvent += this.HandleBaseCharaItemLongPressEvent;
			}
			if (this.SlotList != null)
			{
				this.SlotList.OnItemChangeEvent += this.HandleSlotListItemChangeEvent;
				this.SlotList.OnItemClickEvent += this.HandleSlotListItemClickEvent;
				this.SlotList.OnItemLongPressEvent += this.HandleSlotListItemLongPressEvent;
				this.SlotList.OnUpdateItemsEvent += this.HandleSlotListUpdateItemsEvent;
			}
			// 同期
			this.SyncBaseChara();
			this.SyncSlotCharaList();
			this.SyncCharaList();
		}
		/// <summary>
		/// 初期化
		/// </summary>
		public void Setup()
		{
			this.MemberInit();

			this.ClearCharaList();
			this.ClearBaseChara();
			this.ClearSlotList(false);
			// 同期
			this.SyncBaseChara();
			this.SyncSlotCharaList();
			this.SyncCharaList();
		}
		/// <summary>
		/// ステータス情報設定
		/// </summary>
		public void SetupStatusInfo(int haveMoney)
		{
			if (this.CanUpdate)
			{
				this.Model.HaveMoney = haveMoney;
			}
		}
		#endregion

		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			if (this.CanUpdate)
			{
				this.Model.Dispose();
			}

			this.OnGetBaseCharaSlot = null;
			this.OnOK = null;
			this.OnSlotCalc = null;
			this.OnPlayerCharacter = null;
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

				// その他UIの表示設定
				GUILobbyResident.SetActive(!isActive);
				GUIScreenTitle.Play(isActive, this.ScreenTitle);
				// ヘルプメッセージの状態を更新する
				this.UpdateHelpMessage();
			}
		}
		#endregion

		#region 各状態を更新する
		/// <summary>
		/// OKボタンの状態を更新する
		/// </summary>
		void UpdateOKButtonEnable()
		{
			if (!this.CanUpdate) return;

			bool isEnable = true;

			//var list = this.SlotCharaUUIDList;
			// ベースキャラが選択されていない
			if (this.IsEmptyBaseChara) isEnable = false;
			//// スロットキャラが選択されていない
			//else if (list.Count <= 0) isEnable = false;
			// 所持金が足りない
			else if (this.Model.HaveMoney < this.Model.NeedMoney) isEnable = false;

			this.View.SetOKButtonEnable(isEnable);
		}
		/// <summary>
		/// スロット追加ボタンの状態を更新する
		/// </summary>
		void UpdateAddSlotButtonEnable()
		{
			if (!this.CanUpdate) return;

			bool isEnable = true;
			this.View.SetAddSlotButtonEnable(isEnable);
		}
		/// <summary>
		/// ヘルプメッセージの状態を更新する
		/// </summary>
		void UpdateHelpMessage()
		{
			if (!this.CanUpdate) return;

			var state = this.View.GetActiveState();
			var isActive = state == GUIViewBase.ActiveState.Opened || state == GUIViewBase.ActiveState.Opening;
			if (!isActive)
			{
				GUIHelpMessage.Play(false);
				return;
			}

			switch (this.SelectMode)
			{
				case SelectMode.Base:
					// ベースキャラを選択中
					GUIHelpMessage.Play(true, this.BaseHelpMessage);
					break;
				case SelectMode.Slot:
					// スロットキャラを選択中
					GUIHelpMessage.Play(true, this.SlotHelpMessage);
					break;
				default:
					GUIHelpMessage.Play(true);
					break;
			}
		}
		/// <summary>
		/// スロットフィルターを更新する
		/// </summary>
		void UpdateSlotFill()
		{
			if (!this.CanUpdate) return;

			switch (this.SelectMode)
			{
				case SelectMode.Base:
					this.View.SetSlotFillActive(true);
					break;
				case SelectMode.Slot:
					this.View.SetSlotFillActive(false);
					break;
			}
		}
		/// <summary>
		/// 選択枠を更新する
		/// </summary>
		void UpdateSelectFrame()
		{
			if (this.BaseChara == null) return;
			if (this.SlotList == null) return;

			bool isBaseSelect = false;
			int slotSelectIndex = -1;
			switch (this.SelectMode)
			{
				case SelectMode.Base:
					isBaseSelect = true;
					break;
				case SelectMode.Slot:
					slotSelectIndex = this.SlotCharaUUIDList.Count;
					if (this.SlotList.SlotCount <= slotSelectIndex)
					{
						// スロット解放分よりも多い場合はスロットにはめられない
						slotSelectIndex = -1;
					}
					break;
			}

			this.BaseChara.SetSelect(isBaseSelect);

			var list = this.SlotList.GetNowPageItemList();
			for (int i = 0, max = list.Count; i < max; i++)
			{
				bool isSelect = i == slotSelectIndex;
				list[i].SetSelect(isSelect);
			}
		}
		#endregion

		#region 表示直結系
		#region ステータス関連
		#region 生命力
		void HandleHitPointChange(object sender, EventArgs e) { this.SyncHitPoint(); }
		void SyncHitPoint()
		{
			if (this.CanUpdate)
			{
				this.View.SetHitPoint(this.Model.HitPoint, this.Model.StatusFormat);
			}
		}
		void HandleHitPointCalcChange(object sender, EventArgs e) { this.SyncHitPointCalc(); }
		void SyncHitPointCalc()
		{
			if (this.CanUpdate)
			{
				this.View.SetHitPointCalc(this.Model.HitPointCalc, this.Model.StatusCalcFormat);

				if (this.Model.HitPointCalc > this.Model.HitPoint)
				{
					this.View.SetHitPointCalcColor(StatusColor.Type.Up);
				}
				else if (this.Model.HitPointCalc < this.Model.HitPoint)
				{
					this.View.SetHitPointCalcColor(StatusColor.Type.Down);
				}
				else
				{
					this.View.SetHitPointCalcColor(StatusColor.Type.StatusNormal);
				}
			}
		}
		#endregion

		#region 攻撃力
		void HandleAttackChange(object sender, EventArgs e) { this.SyncAttack(); }
		void SyncAttack()
		{
			if (this.CanUpdate)
			{
				this.View.SetAttack(this.Model.Attack, this.Model.StatusFormat);
			}
		}
		void HandleAttackCalcChange(object sender, EventArgs e) { this.SyncAttackCalc(); }
		void SyncAttackCalc()
		{
			if (this.CanUpdate)
			{
				this.View.SetAttackCalc(this.Model.AttackCalc, this.Model.StatusCalcFormat);

				if (this.Model.AttackCalc > this.Model.Attack)
				{
					this.View.SetAttackCalcColor(StatusColor.Type.Up);
				}
				else if (this.Model.AttackCalc < this.Model.Attack)
				{
					this.View.SetAttackCalcColor(StatusColor.Type.Down);
				}
				else
				{
					this.View.SetAttackCalcColor(StatusColor.Type.StatusNormal);
				}
			}
		}
		#endregion

		#region 防御力
		void HandleDefenseChange(object sender, EventArgs e) { this.SyncDefense(); }
		void SyncDefense()
		{
			if (this.CanUpdate)
			{
				this.View.SetDefense(this.Model.Defense, this.Model.StatusFormat);
			}
		}
		void HandleDefenseCalcChange(object sender, EventArgs e) { this.SyncDefenseCalc(); }
		void SyncDefenseCalc()
		{
			if (this.CanUpdate)
			{
				this.View.SetDefenseCalc(this.Model.DefenseCalc, this.Model.StatusCalcFormat);

				if (this.Model.DefenseCalc > this.Model.Defense)
				{
					this.View.SetDefenseCalcColor(StatusColor.Type.Up);
				}
				else if (this.Model.DefenseCalc < this.Model.Defense)
				{
					this.View.SetDefenseCalcColor(StatusColor.Type.Down);
				}
				else
				{
					this.View.SetDefenseCalcColor(StatusColor.Type.StatusNormal);
				}
			}
		}
		#endregion

		#region 特殊能力
		void HandleExtraChange(object sender, EventArgs e) { this.SyncExtra(); }
		void SyncExtra()
		{
			if (this.CanUpdate)
			{
				this.View.SetExtra(this.Model.Extra, this.Model.StatusFormat);
			}
		}
		void HandleExtraCalcChange(object sender, EventArgs e) { this.SyncExtraCalc(); }
		void SyncExtraCalc()
		{
			if (this.CanUpdate)
			{
				this.View.SetExtraCalc(this.Model.ExtraCalc, this.Model.StatusCalcFormat);

				if (this.Model.ExtraCalc > this.Model.Extra)
				{
					this.View.SetExtraCalcColor(StatusColor.Type.Up);
				}
				else if (this.Model.ExtraCalc < this.Model.Extra)
				{
					this.View.SetExtraCalcColor(StatusColor.Type.Down);
				}
				else
				{
					this.View.SetExtraCalcColor(StatusColor.Type.StatusNormal);
				}
			}
		}
		#endregion

		#region ステータス表示
		void HandleStatusFormatChange(object sender, EventArgs e) { this.SyncStatus(); }
		void SyncStatus()
		{
			this.SyncHitPoint();
			this.SyncAttack();
			this.SyncDefense();
			this.SyncExtra();
		}
		void HandleStatusCalcFormatChange(object sender, EventArgs e) { this.SyncStatusCalc(); }
		void SyncStatusCalc()
		{
			this.SyncHitPointCalc();
			this.SyncAttackCalc();
			this.SyncDefenseCalc();
			this.SyncExtraCalc();
		}
		#endregion
		#endregion

		#region スロット数
		void HandleSlotCountChange(object sender, EventArgs e) { this.SyncSlotNum(); }
		void HandleSlotCapacityChange(object sender, EventArgs e)
		{
			if (this.CanUpdate)
			{
				this.SetSlotCapacity(this.Model.SlotCapacity);
			}
			this.SyncSlotNum();
		}
		void HandleSlotNumFormatChange(object sender, EventArgs e) { this.SyncSlotNum(); }
		void SyncSlotNum()
		{
			if (this.CanUpdate)
			{
				this.View.SetSlotNum(this.Model.SlotCount, this.Model.SlotCapacity, this.Model.SlotNumFormat);
			}
		}
		#endregion

		#region 所持金
		void HandleHaveMoneyChange(object sender, EventArgs e) { this.SyncHaveMoney(); }
		void HandleHaveMoneyFormatChange(object sender, EventArgs e) { this.SyncHaveMoney(); }
		void SyncHaveMoney()
		{
			if (this.CanUpdate)
			{
				this.View.SetHaveMoney(this.Model.HaveMoney, this.Model.HaveMoneyFormat);
			}
		}
		#endregion

		#region 費用
		void HandleNeedMoneyChange(object sender, EventArgs e) { this.SyncNeedMoney(); }
		void HandleNeedMoneyFormatChange(object sender, EventArgs e) { this.SyncNeedMoney(); }
		void SyncNeedMoney()
		{
			if (this.CanUpdate)
			{
				this.View.SetNeedMoney(this.Model.NeedMoney, this.Model.NeedMoneyFormat);
			}
		}
		#endregion

		#region 追加料金
		void HandleAddOnChargeChange(object sender, EventArgs e) { this.SyncAddOnCharge(); }
		void HandleAddOnChargeFormatChange(object sender, EventArgs e) { this.SyncAddOnCharge(); }
		void SyncAddOnCharge()
		{
		}
		#endregion
		#endregion

		#region ホーム、閉じるボタンイベント
		void HandleHome(object sender, EventArgs e)
		{
			if (this.CharaList != null)
			{
				// Newフラグ一括解除
				this.CharaList.DeleteAllNewFlag();
			}

			GUIController.Clear();
		}
		void HandleClose(object sender, EventArgs e)
		{
			if (this.CharaList != null)
			{
				// Newフラグ一括解除
				this.CharaList.DeleteAllNewFlag();
			}

			GUIController.Back();
		}
		#endregion

		#region OKボタンイベント
		#region OKチェック
		/// <summary>
		/// OKボタンイベントハンドラー
		/// </summary>
		void HandleOK(object sender, EventArgs e)
		{
			// ベースキャラのスロット込み情報取得
			this.PlayerCharacterRequest();
		}
		void CheckOK(FusionMessage.SetupParam p)
		{
			GUIFusionMessage.OpenYesNo(p, this.OKMessage, true, this.CheckSlot, null);
		}
		void CheckSlot()
		{
			if (this.Model.AddOnCharge > 0)
			{
				string addOnCharge = string.Format(this.Model.AddOnChargeFormat, this.Model.AddOnCharge);
				string text = string.Format(this.SlotMessage, addOnCharge);
				GUIMessageWindow.SetModeYesNo(text, true, this.OKExecute, null);
			}
			else
			{
				this.OKExecute();
			}
		}
		#endregion

		#region OK実行
		/// <summary>
		/// OKボタンイベント
		/// </summary>
		public event EventHandler<OKEventArgs> OnOK = (sender, e) => { };

		void OKExecute()
		{
			// 通知
			var eventArgs = new OKEventArgs();
			eventArgs.BaseCharaUUID = this.BaseCharaUUID;
			eventArgs.SlotCharaUUIDList = this.SlotCharaUUIDList;
			this.OnOK(this, eventArgs);
		}
		/// <summary>
		/// OKボタン結果
		/// </summary>
		public void OKResult(int money, int price, int addOnCharge, CharaInfo info)
		{
			var baseInfo = this.BaseCharaInfo;
			if (baseInfo == null) return;

			// 合成結果パラメータ
			var p = this.GetFusionResultParam(baseInfo, info);
			var screen = new GUIScreen(() => { GUIFusionResult.Open(p); }, GUIFusionResult.Close, GUIFusionResult.ReOpen);
			GUIController.Open(screen);

			// ベースキャラのスロット情報取得
			this.GetBaseCharaSlotRequest();
		}
		#endregion

		#region プレイヤーキャラクター情報取得イベント
		/// <summary>
		/// プレイヤーキャラクター情報取得イベント
		/// </summary>
		public event EventHandler<PlayerCharacterEventArgs> OnPlayerCharacter = (sender, e) => { };

		/// <summary>
		/// プレイヤーキャラクター情報取得リクエスト
		/// </summary>
		void PlayerCharacterRequest()
		{
			if (this.IsEmptyBaseChara) return;

			// 通知
			var eventArgs = new PlayerCharacterEventArgs();
			eventArgs.BaseCharaUUID = this.BaseCharaUUID;
			this.OnPlayerCharacter(this, eventArgs);
		}
		/// <summary>
		/// プレイヤーキャラクター情報取得結果
		/// </summary>
		public void PlayerCharacterResult(CharaInfo info, List<CharaInfo> slotList)
		{
			if (info == null) return;
			if (this.SlotList == null) return;

			// ベースキャラのキャラ情報を更新する
			var baseInfo = this.BaseCharaInfo;
			if (baseInfo == null) return;
			if (baseInfo.UUID == info.UUID)
			{
				// ベースキャラの同期処理を一旦切って情報だけ更新する
				this.CanSyncBaseChara = false;
				this.SetCharaItem(this.BaseChara, info);
				this.CanSyncBaseChara = true;
			}

			// 合成確認パラメータ
			var p = this.GetFusionMessageParam(info, this.SlotList.GetCharaInfoList());
			this.CheckOK(p);
		}
		#endregion

		#region 各種パラメータ取得
		/// <summary>
		/// 確認メッセージに表示するためのデータを取得する
		/// </summary>
		FusionMessage.SetupParam GetFusionMessageParam(CharaInfo beforeInfo, List<CharaInfo> afterSlotList)
		{
			var p = new FusionMessage.SetupParam();
			if (!this.CanUpdate) return p;

			// 強化後の情報
			// ベースキャラに装着されているスロット情報リストを作成
			var afterBaseInfo = new Scm.Common.Fusion.PowerupSlot(
				beforeInfo.CharacterMasterID, beforeInfo.Rank, beforeInfo.PowerupLevel,
				beforeInfo.SynchroHitPoint, beforeInfo.SynchroAttack,
				beforeInfo.SynchroExtra, beforeInfo.SynchroDefense);
			{
				var powerupSlotList = new List<Scm.Common.XwDb.IPlayerCharacter>();
				foreach (var t in afterSlotList)
				{
					if (t == null) continue;
					var ps = new Scm.Common.Fusion.PowerupSlot(
						t.CharacterMasterID, t.Rank, t.PowerupLevel,
						t.SynchroHitPoint, t.SynchroAttack,
						t.SynchroExtra, t.SynchroDefense);
					powerupSlotList.Add(ps);
				}
				afterBaseInfo.SetSlot(powerupSlotList);
			}

			// ランク
			p.RankBefore	= beforeInfo.Rank;
			p.RankAfter		= beforeInfo.Rank;
			// レベル
			p.LevelBefore	= beforeInfo.PowerupLevel;
			p.LevelAfter	= beforeInfo.PowerupLevel;
			// 経験値
			p.Exp				= beforeInfo.PowerupExp;
			p.TotalExp			= CharaInfo.GetTotalExp(beforeInfo.Rank, beforeInfo.PowerupLevel);
			p.NextLvTotalExp	= CharaInfo.GetTotalExp(beforeInfo.Rank, beforeInfo.PowerupLevel + 1);
			// シンクロ可能回数
			p.SynchroRemainBefore	= beforeInfo.SynchroRemain;
			p.SynchroRemainAfter	= beforeInfo.SynchroRemain;
			// 生命力
			p.HitPointBefore		= beforeInfo.HitPoint;
			p.HitPointAfter			= afterBaseInfo.HitPoint;
			p.HitPointBaseBefore	= beforeInfo.PowerupHitPoint;
			p.HitPointBaseAfter		= beforeInfo.PowerupHitPoint;
			p.SynchroHitPoint		= beforeInfo.SynchroHitPoint;
			p.SlotHitPointBefore	= this.Model.HitPoint;
			p.SlotHitPointAfter		= this.Model.HitPointCalc;
			// 攻撃力
			p.AttackBefore			= beforeInfo.Attack;
			p.AttackAfter			= afterBaseInfo.Attack;
			p.AttackBaseBefore		= beforeInfo.PowerupAttack;
			p.AttackBaseAfter		= beforeInfo.PowerupAttack;
			p.SynchroAttack			= beforeInfo.SynchroAttack;
			p.SlotAttackBefore		= this.Model.Attack;
			p.SlotAttackAfter		= this.Model.AttackCalc;
			// 防御力
			p.DefenseBefore			= beforeInfo.Defense;
			p.DefenseAfter			= afterBaseInfo.Defense;
			p.DefenseBaseBefore		= beforeInfo.PowerupDefense;
			p.DefenseBaseAfter		= beforeInfo.PowerupDefense;
			p.SynchroDefense		= beforeInfo.SynchroDefense;
			p.SlotDefenseBefore		= this.Model.Defense;
			p.SlotDefenseAfter		= this.Model.DefenseCalc;
			// 特殊能力
			p.ExtraBefore			= beforeInfo.Extra;
			p.ExtraAfter			= afterBaseInfo.Extra;
			p.ExtraBaseBefore		= beforeInfo.PowerupExtra;
			p.ExtraBaseAfter		= beforeInfo.PowerupExtra;
			p.SynchroExtra			= beforeInfo.SynchroExtra;
			p.SlotExtraBefore		= this.Model.Extra;
			p.SlotExtraAfter		= this.Model.ExtraCalc;
			// シンクロ合成フラグ
			p.IsSynchroFusion = false;

			return p;
		}
		/// <summary>
		/// スロット結果に表示するためのデータを取得する
		/// </summary>
		FusionResult.SetupParam GetFusionResultParam(CharaInfo beforeInfo, CharaInfo afterInfo)
		{
			var p = new FusionResult.SetupParam();
			if (!this.CanUpdate) return p;

			// アバタータイプ
			p.AvatarType	= afterInfo.AvatarType;
			// ランク
			p.RankBefore	= beforeInfo.Rank;
			p.RankAfter		= afterInfo.Rank;
			// レベル
			p.LevelBefore	= beforeInfo.PowerupLevel;
			p.LevelAfter	= afterInfo.PowerupLevel;
			// 経験値
			p.Exp				= afterInfo.PowerupExp;
			p.TotalExp			= CharaInfo.GetTotalExp(afterInfo.Rank, afterInfo.PowerupLevel);
			p.NextLvTotalExp	= CharaInfo.GetTotalExp(afterInfo.Rank, afterInfo.PowerupLevel + 1);
			// シンクロ可能回数
			p.SynchroRemainBefore	= beforeInfo.SynchroRemain;
			p.SynchroRemainAfter	= afterInfo.SynchroRemain;
			// 生命力
			p.HitPointBefore		= beforeInfo.HitPoint;
			p.HitPointAfter			= afterInfo.HitPoint;
			p.HitPointBaseBefore	= beforeInfo.PowerupHitPoint;
			p.HitPointBaseAfter		= afterInfo.PowerupHitPoint;
			p.SynchroHitPointBefore	= beforeInfo.SynchroHitPoint;
			p.SynchroHitPointAfter	= afterInfo.SynchroHitPoint;
			p.SlotHitPointBefore	= beforeInfo.SlotHitPoint;
			p.SlotHitPointAfter		= afterInfo.SlotHitPoint;
			// 攻撃力
			p.AttackBefore			= beforeInfo.Attack;
			p.AttackAfter			= afterInfo.Attack;
			p.AttackBaseBefore		= beforeInfo.PowerupAttack;
			p.AttackBaseAfter		= afterInfo.PowerupAttack;
			p.SynchroAttackBefore	= beforeInfo.SynchroAttack;
			p.SynchroAttackAfter	= afterInfo.SynchroAttack;
			p.SlotAttackBefore		= beforeInfo.SlotAttack;
			p.SlotAttackAfter		= afterInfo.SlotAttack;
			// 防御力
			p.DefenseBefore			= beforeInfo.Defense;
			p.DefenseAfter			= afterInfo.Defense;
			p.DefenseBaseBefore		= beforeInfo.PowerupDefense;
			p.DefenseBaseAfter		= afterInfo.PowerupDefense;
			p.SynchroDefenseBefore	= beforeInfo.SynchroDefense;
			p.SynchroDefenseAfter	= afterInfo.SynchroDefense;
			p.SlotDefenseBefore		= beforeInfo.SlotDefense;
			p.SlotDefenseAfter		= afterInfo.SlotDefense;
			// 特殊能力
			p.ExtraBefore			= beforeInfo.Extra;
			p.ExtraAfter			= afterInfo.Extra;
			p.ExtraBaseBefore		= beforeInfo.PowerupExtra;
			p.ExtraBaseAfter		= afterInfo.PowerupExtra;
			p.SynchroExtraBefore	= beforeInfo.SynchroExtra;
			p.SynchroExtraAfter		= afterInfo.SynchroExtra;
			p.SlotExtraBefore		= beforeInfo.SlotExtra;
			p.SlotExtraAfter		= afterInfo.SlotExtra;
			// 結果
			p.IsPowerupResultEnable = false;
			//p.PowerupResult	= Scm.Common.GameParameter.PowerupResult.Good;

			return p;
		}
		#endregion
		#endregion

		#region スロット追加ボタンイベント
		void HandleAddSlot(object sender, EventArgs e)
		{
		}
		#endregion

		#region キャラリスト設定
		#region キャラアイテム操作
		void HandleCharaListItemChangeEvent(GUICharaItem obj) { this.UpdateItemDisableType(obj); }
		void HandleCharaListItemClickEvent(GUICharaItem obj) { this.SwitchOperationItem(obj); }
		void HandleCharaListItemLongPressEvent(GUICharaItem obj) { }
		void HandleCharaListUpdateItemsEvent()
		{
			if (this.CharaList != null)
			{
				var list = this.CharaList.GetNowPageItemList();
				list.ForEach(this.UpdateItemDisableType);
			}
		}
		/// <summary>
		/// キャラリストを同期
		/// </summary>
		void SyncCharaList()
		{
			if (this.CharaList == null) return;

			GUIDebugLog.AddMessage("SyncCharaList");

			// 各キャラ情報が選択できるか設定する
			var list = this.CharaList.GetCharaInfo();
			this.UpdateCanSelect(list);

			// キャラリスト更新
			this.CharaList.SetupItems(list);
		}
		/// <summary>
		/// キャラリスト枠を設定
		/// </summary>
		public void SetupCapacity(int capacity, int count)
		{
			if (this.CharaList != null)
			{
				this.CharaList.SetupCapacity(capacity, count);
			}
		}
		/// <summary>
		/// キャラリストの中身設定
		/// </summary>
		public void SetupItem(List<CharaInfo> list)
		{
			this.RawList = list;

			this.UpdateCharaList();
		}
		/// <summary>
		/// キャラリストの中身更新
		/// </summary>
		void UpdateCharaList()
		{
			// 選択できるかどうかの情報を更新する
			this.UpdateCanSelect(this.RawList);

			// リストからベースキャラのキャラ情報を更新する
			this.UpdateBaseChara(this.RawList);
			// リストからスロットリストのキャラ情報を更新する
			this.UpdateSlotList(this.RawList);

			// キャラリストの中身を更新する
			if (this.CharaList != null)
			{
				this.CharaList.SetupItems(this.RawList);
			}
		}
		/// <summary>
		/// キャラリストからベースキャラの中身を更新する
		/// </summary>
		void UpdateBaseChara(List<CharaInfo> list)
		{
			if (list == null) return;
			if (this.BaseChara == null) return;

			var info = this.BaseChara.GetCharaInfo();
			if (info == null) return;

			// ベースキャラのキャラ情報を更新する
			var newInfo = list.Find(t => { return t.UUID == info.UUID; });
			this.SetCharaItem(this.BaseChara, newInfo);
		}
		/// <summary>
		/// キャラリストからスロットリストの中身を更新する
		/// </summary>
		void UpdateSlotList(List<CharaInfo> list)
		{
			if (list == null) return;
			if (this.SlotList == null) return;

			var uuidList = this.SlotCharaUUIDList;
			if (uuidList.Count <= 0) return;

			// 新しいスロットリスト初期化
			var newInfoList = new List<CharaInfo>(uuidList.Count);
			uuidList.ForEach(t => { newInfoList.Add(null); });

			// 新しいスロットリストを抽出する
			var count = 0;
			foreach (var info in list)
			{
				if (info == null) continue;
				var index = uuidList.FindIndex(t => { return t == info.UUID; });
				if (index == -1) continue;

				newInfoList[index] = info;
				count++;
				if (count >= uuidList.Count)
				{
					// 抽出完了
					break;
				}
			}

			// スロットリストのキャラ情報を更新する
			this.ClearSlotList(false);
			this.SetSlotList(newInfoList, false);
			//newInfoList.ForEach(info => { this.AddSlotChara(info, false); });
		}
		/// <summary>
		/// 現在の状態によってアイテムの処理を分岐させる
		/// </summary>
		void SwitchOperationItem(GUICharaItem item)
		{
			if (item == null) return;

			var disableState = item.GetDisableState();
			var info = item.GetCharaInfo();
			switch (disableState)
			{
				case CharaItem.Controller.DisableType.None:
					if (this.SelectMode == SelectMode.Base)
					{
						// ベースキャラ設定
						this.SetBaseChara(info, true);
					}
					else if (this.SelectMode == SelectMode.Slot)
					{
						// スロット追加
						this.AddSlotChara(info, true);
					}
					break;
				case CharaItem.Controller.DisableType.Base:
					// ベースキャラクリア
					this.ClearBaseChara();
					break;
				case CharaItem.Controller.DisableType.Bait:
					// スロット削除
					this.RemoveSlotChara(info, true);
					break;
				default:
					break;
			}
		}
		/// <summary>
		/// キャラリストクリア
		/// </summary>
		void ClearCharaList()
		{
			if (this.CharaList != null)
			{
				this.CharaList.SetupItems(null);
			}
		}
		#endregion

		#region キャラ情報
		/// <summary>
		/// 選択できるかどうかの情報を更新する
		/// </summary>
		void UpdateCanSelect(List<CharaInfo> list)
		{
			if (list == null) return;

			// 各キャラ情報の選択出来るかどうかの情報を更新する
			list.ForEach(this.UpdateCanSelect);
		}
		/// <summary>
		/// 選択できるかどうかの情報を更新する
		/// </summary>
		void UpdateCanSelect(CharaInfo info)
		{
			if (info == null) return;

			// 無効タイプを取得する
			var disableType = CharaItem.Controller.DisableType.None;
			var slotIndex = -1;
			switch (this.SelectMode)
			{
				case SelectMode.Base:
					this.GetBaseCharaDisableType(info, out disableType);
					break;
				case SelectMode.Slot:
					this.GetAttachCharaDisableType(info, out disableType, out slotIndex);
					break;
			}

			// 無効タイプから選択できるか設定する
			var canSelect = false;
			switch (disableType)
			{
			case CharaItem.Controller.DisableType.None:
			case CharaItem.Controller.DisableType.Base:
			case CharaItem.Controller.DisableType.Bait:
				canSelect = true;
				break;
			}
			info.CanSelect = canSelect;
		}
		/// <summary>
		/// ベースキャラ選択時の無効タイプを取得する
		/// </summary>
		void GetBaseCharaDisableType(CharaInfo info, out CharaItem.Controller.DisableType disableType)
		{
			disableType = XUI.CharaItem.Controller.DisableType.None;
			if (info == null) return;

			// 以下無効にするかチェック
			if (info.UUID == this.BaseCharaUUID)
			{
				// ベースキャラ選択中
				disableType = CharaItem.Controller.DisableType.Base;
			}
			// スロットに入っている
			else if (info.IsInSlot) disableType = XUI.CharaItem.Controller.DisableType.PowerupSlot;
		}
		/// <summary>
		/// 装着キャラ選択時の無効タイプを取得する
		/// </summary>
		void GetAttachCharaDisableType(CharaInfo info, out CharaItem.Controller.DisableType disableType, out int slotIndex)
		{
			disableType = XUI.CharaItem.Controller.DisableType.None;
			slotIndex = -1;
			if (info == null) return;

			// 以下無効にするかチェック
			// 優先順位があるので注意
			var slotUUIDList = this.SlotCharaUUIDList;
			slotIndex = slotUUIDList.FindIndex(uuid => uuid == info.UUID);
			if (info.UUID == this.BaseCharaUUID)
			{
				// ベースキャラ選択中
				disableType = CharaItem.Controller.DisableType.Base;
			}
			else if (slotIndex >= 0)
			{
				// 素材キャラ選択中
				disableType = CharaItem.Controller.DisableType.Bait;
			}
			else if (info.IsInSlot && null == this.BaseCharaSlotList.Find(s => s.UUID == info.UUID))
			{
				// スロットに入っている、かつベースキャラのスロットではない
				disableType = CharaItem.Controller.DisableType.PowerupSlot;
			}
			// デッキに入っている
			else if (info.IsInDeck) disableType = CharaItem.Controller.DisableType.Deck;
			// シンボルに設定している
			else if (info.IsSymbol) disableType = CharaItem.Controller.DisableType.Symbol;
		}
		#endregion

		#region キャラアイテム
		/// <summary>
		/// アイテムの無効タイプ更新
		/// </summary>
		void UpdateItemDisableType(GUICharaItem item)
		{
			if (item == null) return;

			// 無効タイプを取得する
			var disableType = CharaItem.Controller.DisableType.None;
			var slotIndex = -1;
			switch (this.SelectMode)
			{
				case SelectMode.Base:
					this.GetBaseCharaDisableType(item.GetCharaInfo(), out disableType);
					break;
				case SelectMode.Slot:
					this.GetAttachCharaDisableType(item.GetCharaInfo(), out disableType, out slotIndex);
					break;
			}

			// 無効タイプを設定する
			if (slotIndex >= 0)
			{
				// スロットのインデックスがある場合は餌
				item.SetBaitState(slotIndex);
			}
			else
			{
				// それ以外は無効タイプを設定する
				item.SetDisableState(disableType);
			}
		}
		/// <summary>
		/// キャラアイテムを設定する
		/// </summary>
		void SetCharaItem(GUICharaItem item, CharaInfo info)
		{
			if (item == null) return;

			var state = CharaItem.Controller.ItemStateType.Icon;
			if (info == null || info.IsEmpty)
			{
				state = CharaItem.Controller.ItemStateType.FillEmpty;
			}
			item.SetState(state, info);
		}
		#endregion
		#endregion

		#region ベースキャラ設定
		#region ベースキャラ操作
		void HandleBeseCharaItemChangeEvent(GUICharaItem obj) { this.SyncBaseChara(); this.SyncCharaList(); }
		void HandleBaseCharaItemClickEvent(GUICharaItem obj) { this.ClearBaseChara(); }
		void HandleBaseCharaItemLongPressEvent(GUICharaItem obj) { }
		/// <summary>
		/// ベースキャラ設定
		/// </summary>
		void SetBaseChara(CharaInfo info, bool isRequest)
		{
			if (this.BaseChara == null) return;

			this.SetCharaItem(this.BaseChara, info);
			if (isRequest)
			{
				this.GetBaseCharaSlotRequest();
			}
		}
		/// <summary>
		/// ベースキャラを外す
		/// </summary>
		void ClearBaseChara()
		{
			this.SetBaseChara(null, true);
		}
		/// <summary>
		/// ベースキャラを同期
		/// </summary>
		void SyncBaseChara()
		{
			if (!this.CanSyncBaseChara) return;

			GUIDebugLog.AddMessage("SyncBaseChara");

			// スロットキャラを外す
			this.ClearSlotList(false);

			// ベースキャラ情報更新
			if (this.CanUpdate)
			{
				// ベースキャラのデータ更新
				int slotCapacity = 0;
				var info = this.BaseCharaInfo;
				if (info != null)
				{
					slotCapacity = info.PowerupSlotNum;
				}
				this.SetSlotCapacity(slotCapacity);

				// 各状態を更新する
				this.UpdateOKButtonEnable();
				this.UpdateAddSlotButtonEnable();
				this.UpdateHelpMessage();
			}

			// 各状態を更新する
			this.UpdateSlotFill();
			this.UpdateSelectFrame();
		}
		#endregion

		#region ベースキャラスロット取得イベント
		/// <summary>
		/// ベースキャラスロット取得イベント
		/// </summary>
		public event EventHandler<GetBaseCharaSlotEventArgs> OnGetBaseCharaSlot = (sender, e) => { };

		/// <summary>
		/// ベースキャラスロット取得リクエスト
		/// </summary>
		void GetBaseCharaSlotRequest()
		{
			if (!this.IsEmptyBaseChara)
			{
				// 通知
				var eventArgs = new GetBaseCharaSlotEventArgs();
				eventArgs.BaseCharaUUID = this.BaseCharaUUID;
				this.OnGetBaseCharaSlot(this, eventArgs);
			}
			else
			{
				// スロット情報リストを元にスロットリストを更新する
				this.UpdateSlotList(this.RawList, null);
				// ボーナス値設定
				this.SetBaseSlotBonus(0, 0, 0, 0);
				this.SetSlotCalcResult(0, 0, 0, 0, 0, 0);
			}
		}
		/// <summary>
		/// ベースキャラスロット取得結果
		/// </summary>
		public void GetBaseCharaSlotResult(ulong baseCharaUUID, int hitPointBonus, int attackBonus, int defenseBonus, int extraBonus, List<PowerupSlotCharaInfo> slotInfoList)
		{
			// 現在設定されているベースキャラが違う
			var uuid = this.BaseCharaUUID;
			if (uuid != baseCharaUUID) return;

			// スロット情報リストを元にスロットリストを更新する
			this.UpdateSlotList(this.RawList, slotInfoList);
			// ボーナス値設定
			this.SetBaseSlotBonus(hitPointBonus, attackBonus, defenseBonus, extraBonus);
			this.SetSlotCalcResult(hitPointBonus, attackBonus, defenseBonus, extraBonus, 0, 0);
		}
		/// <summary>
		/// ベースキャラのスロットボーナスを設定する
		/// </summary>
		void SetBaseSlotBonus(int hitPointBonus, int attackBonus, int defenseBonus, int extraBonus)
		{
			if (!this.CanUpdate) return;

			this.Model.HitPoint = hitPointBonus;
			this.Model.Attack = attackBonus;
			this.Model.Defense = defenseBonus;
			this.Model.Extra = extraBonus;
		}
		#endregion
		#endregion

		#region スロットリスト設定
		#region スロット操作
		void HandleSlotListItemChangeEvent(GUICharaItem obj) { }
		void HandleSlotListItemClickEvent(GUICharaItem obj)
		{
			var info = obj != null ? obj.GetCharaInfo() : null;
			if (info != null)
			{
				this.RemoveSlotChara(info, true);
			}
		}
		void HandleSlotListItemLongPressEvent(GUICharaItem obj) { }
		void HandleSlotListUpdateItemsEvent() { this.SyncSlotCharaList(); this.SyncCharaList(); }
		/// <summary>
		/// スロット情報リストを元にスロットリストを更新する
		/// </summary>
		void UpdateSlotList(List<CharaInfo> charaList, List<PowerupSlotCharaInfo> slotInfoList)
		{
			if (charaList == null) return;
			slotInfoList = slotInfoList != null ? slotInfoList : new List<PowerupSlotCharaInfo>();

			// スロット情報リストを元にキャラリストからキャラ情報を抜き出す
			var list = new List<CharaInfo>();
			foreach (var slotInfo in slotInfoList)
			{
				if (slotInfo == null) continue;
				var charaInfo = charaList.Find(t => t.UUID == slotInfo.UUID);
				if (charaInfo == null) continue;
				list.Add(charaInfo);
			}

			// スロット情報を更新する
			this.ClearSlotList(false);
			this.SetSlotList(list, false);
			//list.ForEach(t => this.AddSlotChara(t, false));

			// スロットリストの中身があるときは
			// キャラリストを同期して番号アイコンを更新する
			this.BaseCharaSlotList = slotInfoList;
			if (this.BaseCharaSlotList.Count > 0)
			{
				this.SyncCharaList();
			}
		}
		/// <summary>
		/// スロットの枠設定
		/// </summary>
		void SetSlotCapacity(int capacity)
		{
			if (this.SlotList != null)
			{
				this.SlotList.SetupCapacity(MasterDataCommonSetting.Fusion.MaxPowerupSlotNum, capacity);
			}
			if (this.CanUpdate)
			{
				this.Model.SlotCapacity = capacity;
			}
		}
		/// <summary>
		/// スロットキャラ追加
		/// </summary>
		void AddSlotChara(CharaInfo info, bool isRequest)
		{
			if (this.SlotList != null)
			{
				this.SlotList.AddChara(info);
			}
			this.SlotChange(isRequest);
		}
		/// <summary>
		/// スロットリストを設定
		/// </summary>
		void SetSlotList(List<CharaInfo> slotList, bool isRequest)
		{
			if (this.SlotList != null)
			{
				this.SlotList.SetupItems(slotList);
			}
			this.SlotChange(isRequest);
		}
		/// <summary>
		/// スロットキャラ削除
		/// </summary>
		void RemoveSlotChara(CharaInfo info, bool isRequest)
		{
			if (this.SlotList != null)
			{
				this.SlotList.RemoveChara(info);
			}
			this.SlotChange(isRequest);
		}
		/// <summary>
		/// スロットリストクリア
		/// </summary>
		void ClearSlotList(bool isRequest)
		{
			if (this.SlotList != null)
			{
				this.SlotList.ClearChara();
			}
			this.SlotChange(isRequest);
		}
		/// <summary>
		/// スロット変更処理
		/// </summary>
		void SlotChange(bool isRequest)
		{
			var slotCount = 0;
			if (this.SlotList != null)
			{
				slotCount = this.SlotList.GetCharaInfoList().Count;
			}
			if (this.CanUpdate)
			{
				this.Model.SlotCount = slotCount;
			}
			if (isRequest)
			{
				this.SlotCalcRequest();
			}
		}
		/// <summary>
		/// スロットリストを同期
		/// </summary>
		void SyncSlotCharaList()
		{
			GUIDebugLog.AddMessage("SyncSlotCharaList");

			// 各状態を更新する
			this.UpdateSlotFill();
			this.UpdateSelectFrame();
		}
		#endregion

		#region スロット変更イベント
		/// <summary>
		/// スロット試算イベント
		/// </summary>
		public event EventHandler<SlotCalcEventArgs> OnSlotCalc = (sender, e) => { };

		/// <summary>
		/// スロット試算リクエスト
		/// </summary>
		void SlotCalcRequest()
		{
			if (!this.IsEmptyBaseChara)
			{
				// 通知
				var eventArgs = new SlotCalcEventArgs();
				eventArgs.BaseCharaUUID = this.BaseCharaUUID;
				eventArgs.SlotCharaUUIDList = this.SlotCharaUUIDList;
				this.OnSlotCalc(this, eventArgs);
			}
			else
			{
				// スロット試算結果を設定する
				this.SetSlotCalcResult(0, 0, 0, 0, 0, 0);
				// 合成ボタンの状態を更新する
				this.UpdateOKButtonEnable();
			}
		}
		/// <summary>
		/// スロット試算結果
		/// </summary>
		public void SlotCalcResult(int money, int hitPointBonus, int attackBonus, int defenseBonus, int extraBonus, int price, int addOnCharge)
		{
			// スロット試算結果を設定する
			this.SetSlotCalcResult(hitPointBonus, attackBonus, defenseBonus, extraBonus, price, addOnCharge);
			// 合成ボタンの状態を更新する
			this.UpdateOKButtonEnable();
		}
		/// <summary>
		/// スロット試算結果を設定する
		/// </summary>
		void SetSlotCalcResult(int hitPointBonus, int attackBonus, int defenseBonus, int extraBonus, int price, int addOnCharge)
		{
			if (!this.CanUpdate) return;

			this.Model.HitPointCalc = hitPointBonus;
			this.Model.AttackCalc = attackBonus;
			this.Model.DefenseCalc = defenseBonus;
			this.Model.ExtraCalc = extraBonus;
			this.Model.NeedMoney = price;
			this.Model.AddOnCharge = addOnCharge;
		}
		#endregion
		#endregion
	}
}
