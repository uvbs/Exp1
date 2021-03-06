/// <summary>
/// 進化合成表示
/// 
/// 2016/02/02
/// </summary>
using UnityEngine;
using System;

namespace XUI
{
	namespace Evolution
	{
		public class HomeClickedEventArgs : EventArgs { }
		public class CloseClickedEventArgs : EventArgs { }

		/// <summary>
		/// 進化合成表示インターフェイス
		/// </summary>
		public interface IView
		{
			#region ホーム/閉じる
			// ホーム、閉じるイベント通知用
			event EventHandler<HomeClickedEventArgs> OnHome;
			event EventHandler<CloseClickedEventArgs> OnClose;
			#endregion

			#region アクティブ
			/// <summary>
			/// アクティブ状態にする
			/// </summary>
			void SetActive(bool isActive, bool isTweenSkip);

			/// <summary>
			/// 現在のアクティブ状態を取得する
			/// </summary>
			GUIViewBase.ActiveState GetActiveState();
			#endregion

			#region 所持金/費用
			/// <summary>
			/// 所持金設定
			/// </summary>
			/// <param name="money"></param>
			/// <param name="format"></param>
			void SetHaveMoney(int money, string format);

			/// <summary>
			/// 費用設定
			/// </summary>
			/// <param name="money"></param>
			/// <param name="format"></param>
			void SetNeedMoney(int money, string format);
			#endregion

			#region 合成ボタン
			/// <summary>
			/// 合成ボタンイベント通知
			/// </summary>
			event EventHandler OnFusion;

			/// <summary>
			/// 合成ボタンの有効化
			/// </summary>
			void SetFusionButtonEnable(bool isEnable);
			#endregion

			#region キャラステータス
			/// <summary>
			/// ベースキャラのシンクロ合成残り回数の設定
			/// </summary>
			void SetBaseSynchroRemain(string remain);

			/// <summary>
			/// ベースキャラのランク設定
			/// </summary>
			void SetBaseRank(string rank);

			/// <summary>
			/// 進化キャラのステータスアクティブ設定
			/// </summary>
			void SetEvolutionStatusActive(bool isActive);

			/// <summary>
			/// 進化キャラのシンクロ合成残り回数の設定
			/// </summary>
			void SetEvolutionSynchroRemain(string remain);

			/// <summary>
			/// 進化キャラのシンクロ合成残り回数カラー設定
			/// </summary>
			void SetEvolutionSynchroRemainColor(StatusColor.Type type);

			/// <summary>
			/// 進化キャラのランク設定
			/// </summary>
			void SetEvolutionRank(string rank);

			/// <summary>
			/// 進化キャラのシンクロ合成残り回数カラー設定
			/// </summary>
			void SetEvolutionRankColor(StatusColor.Type type);
			#endregion

			#region 素材リスト
			/// <summary>
			/// 素材リストを隠すための表示設定
			/// </summary>
			void SetFillMaterialListActive(bool isActive);
			#endregion
		}

		/// <summary>
		/// 進化合成表示
		/// </summary>
		public class EvolutionView : GUIScreenViewBase, IView
		{
			#region 破棄
			void OnDestroy()
			{
				this.OnHome = null;
				this.OnClose = null;
				this.OnFusion = null;

			}
			#endregion

			#region アクティブ
			/// <summary>
			/// アクティブ状態にする
			/// </summary>
			public void SetActive(bool isActive, bool isTweenSkip)
			{
				this.SetRootActive(isActive, isTweenSkip);
			}
			/// <summary>
			/// 現在のアクティブ状態を取得する
			/// </summary>
			public GUIViewBase.ActiveState GetActiveState()
			{
				return this.GetRootActiveState();
			}
			#endregion

			#region ホーム、閉じるボタンイベント
			/// <summary>
			/// ホーム、閉じるイベント通知用
			/// </summary>
			public event EventHandler<HomeClickedEventArgs> OnHome = (sender, e) => { };
			public event EventHandler<CloseClickedEventArgs> OnClose = (sender, e) => { };

			/// <summary>
			/// ホームボタンイベント
			/// </summary>
			public override void OnHomeEvent()
			{
				// 通知
				var eventArgs = new HomeClickedEventArgs();
				this.OnHome(this, eventArgs);
			}

			/// <summary>
			/// 閉じるボタンイベント
			/// </summary>
			public override void OnCloseEvent()
			{
				// 通知
				var eventArgs = new CloseClickedEventArgs();
				this.OnClose(this, eventArgs);
			}
			#endregion

			#region 所持金
			[SerializeField]
			private UILabel _haveMoneyLabel = null;
			private UILabel HaveMoneyLabel { get { return _haveMoneyLabel; } }

			/// <summary>
			/// 所持金設定
			/// </summary>
			public void SetHaveMoney(int money, string format)
			{
				if (this.HaveMoneyLabel != null)
				{
					this.HaveMoneyLabel.text = string.Format(format, money);
				}
			}
			#endregion

			#region 費用
			[SerializeField]
			private UILabel _needMoneyLabel = null;
			private UILabel NeedMoneyLabel { get { return _needMoneyLabel; } }

			/// <summary>
			/// 費用設定
			/// </summary>
			public void SetNeedMoney(int money, string format)
			{
				if (this.NeedMoneyLabel != null)
				{
					this.NeedMoneyLabel.text = string.Format(format, money);
				}
			}
			#endregion

			#region 合成ボタン
			/// <summary>
			/// イベント通知
			/// </summary>
			public event EventHandler OnFusion = (sender, e) => { };

			/// <summary>
			/// 合成ボタンイベント
			/// </summary>
			public void OnFusionEvent()
			{
				// 通知
				this.OnFusion(this, EventArgs.Empty);
			}

			/// <summary>
			/// 合成ボタン
			/// </summary>
			[SerializeField]
			private UIButton _fusionButton = null;
			private UIButton FusionButton { get { return this._fusionButton; } }

			/// <summary>
			/// 合成ボタンの有効化
			/// </summary>
			/// <param name="isEnable"></param>
			public void SetFusionButtonEnable(bool isEnable)
			{
				if (this.FusionButton == null) { return; }
				this.FusionButton.isEnabled = isEnable;
			}
			#endregion

			#region キャラステータス
			/// <summary>
			/// ベースキャラアタッチオブジェクト
			/// </summary>
			[SerializeField]
			private CharaStatusAttachObject _baseCharaStatusAttach = null;
			private CharaStatusAttachObject BaseCharaStatusAttach { get { return _baseCharaStatusAttach; } }

			/// <summary>
			/// 進化キャラアタッチオブジェクト
			/// </summary>
			[SerializeField]
			private CharaStatusAttachObject _evolutionCharaStatusAttach = null;
			private CharaStatusAttachObject EvolutionCharaStatusAttach { get { return _evolutionCharaStatusAttach; } }

			[Serializable]
			private class CharaStatusAttachObject
			{
				/// <summary>
				/// 親オブジェクト
				/// </summary>
				[SerializeField]
				private GameObject _parent = null;
				public GameObject Parent { get { return _parent; } }

				/// <summary>
				/// ランク
				/// </summary>
				[SerializeField]
				private UILabel _rankLabel = null;
				public UILabel RankLabel { get { return _rankLabel; } }

				/// <summary>
				/// ランク(Grow)
				/// </summary>
				[SerializeField]
				private UILabel _rankGrowLabel = null;
				public UILabel RankGrowLabel { get { return _rankGrowLabel; } }

				/// <summary>
				/// ランク
				/// </summary>
				[SerializeField]
				private UISprite _rankSprite = null;
				public UISprite RankSprite { get { return _rankSprite; } }

				/// <summary>
				/// ランク(Grow)
				/// </summary>
				[SerializeField]
				private UISprite _rankGrowSprite = null;
				public UISprite RankGrowSprite { get { return _rankGrowSprite; } }

				/// <summary>
				/// シンクロ合成残り回数
				/// </summary>
				[SerializeField]
				private UILabel _synchroRemainLabel = null;
				public UILabel SynchroRemainLabel { get { return _synchroRemainLabel; } }

				/// <summary>
				/// シンクロ合成残り回数(Grow)
				/// </summary>
				[SerializeField]
				private UILabel _synchroRemainGrowLabel = null;
				public UILabel SynchroRemainGrowLabel { get { return _synchroRemainGrowLabel; } }
			}

			/// <summary>
			/// ベースキャラのシンクロ合成残り回数の設定
			/// </summary>
			public void SetBaseSynchroRemain(string remain)
			{
				if (this.BaseCharaStatusAttach == null) { return; }
				if(this.BaseCharaStatusAttach.SynchroRemainLabel != null)
				{
					this.BaseCharaStatusAttach.SynchroRemainLabel.text = remain;
				}
			}

			/// <summary>
			/// ベースキャラのランク設定
			/// </summary>
			public void SetBaseRank(string rank)
			{
				if (this.BaseCharaStatusAttach == null) { return; }
				if(this.BaseCharaStatusAttach.RankLabel != null)
				{
					this.BaseCharaStatusAttach.RankLabel.text = rank;
				}
			}

			/// <summary>
			/// 進化キャラのステータスアクティブ設定
			/// </summary>
			public void SetEvolutionStatusActive(bool isActive)
			{
				if (this.EvolutionCharaStatusAttach == null) { return; }
				if(this.EvolutionCharaStatusAttach.Parent != null)
				{
					this.EvolutionCharaStatusAttach.Parent.SetActive(isActive);
				}
			}

			/// <summary>
			/// 進化キャラのシンクロ合成残り回数の設定
			/// </summary>
			public void SetEvolutionSynchroRemain(string remain)
			{
				if (this.EvolutionCharaStatusAttach == null) { return; }
				if(this.EvolutionCharaStatusAttach.SynchroRemainLabel != null)
				{
					this.EvolutionCharaStatusAttach.SynchroRemainLabel.text = remain;
				}
			}

			/// <summary>
			/// 進化キャラのシンクロ合成残り回数カラー設定
			/// </summary>
			public void SetEvolutionSynchroRemainColor(StatusColor.Type type)
			{
				var t = this.EvolutionCharaStatusAttach;
				if (t == null) { return; }

				StatusColor.Set(type, t.SynchroRemainLabel, t.SynchroRemainGrowLabel);
			}

			/// <summary>
			/// 進化キャラのランク設定
			/// </summary>
			public void SetEvolutionRank(string rank)
			{
				if (this.EvolutionCharaStatusAttach == null) { return; }
				if(this.EvolutionCharaStatusAttach.RankLabel != null)
				{
					this.EvolutionCharaStatusAttach.RankLabel.text = rank;
				}
			}

			/// <summary>
			/// 進化キャラのシンクロ合成残り回数カラー設定
			/// </summary>
			public void SetEvolutionRankColor(StatusColor.Type type)
			{
				var t = this.EvolutionCharaStatusAttach;
				if (t == null) { return; }

				StatusColor.Set(type, t.RankLabel, t.RankGrowLabel);
				StatusColor.Set(type, t.RankSprite, t.RankGrowSprite);
			}
			#endregion

			#region 素材リスト
			/// <summary>
			/// 素材リストを隠す表示物
			/// </summary>
			[SerializeField]
			private GameObject _fillMaterialListObj = null;
			private GameObject FillMaterialListObj { get { return _fillMaterialListObj; } }

			/// <summary>
			/// 素材リストを隠すための表示設定
			/// </summary>
			public void SetFillMaterialListActive(bool isActive)
			{
				if (this.FillMaterialListObj == null) { return; }
				this.FillMaterialListObj.SetActive(isActive);
			}
			#endregion
		}

	}
}