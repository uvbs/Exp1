/// <summary>
/// キャラアイテム表示
/// 
/// 2016/01/08
/// </summary>
using UnityEngine;
using System;

namespace XUI
{
	namespace CharaItem
	{
		/// <summary>
		/// キャラアイテム表示インターフェイス
		/// </summary>
		public interface IView
		{
			/// <summary>
			/// アクティブ状態にする
			/// </summary>
			/// <param name="isActive"></param>
			void SetActive(bool isActive);

			/// <summary>
			/// 現在のアクティブ状態を取得する
			/// </summary>
			/// <returns></returns>
			GUIViewBase.ActiveState GetActiveState();

			/// <summary>
			/// アイコンのアクティブ設定
			/// </summary>
			/// <param name="isActive"></param>
			void SetIconActive(bool isActive);

			/// <summary>
			/// 空表示のアクティブ設定
			/// </summary>
			/// <param name="isActive"></param>
			void SetEmptyActive(bool isActive);

			/// <summary>
			/// キャラアイコンのアクティブ設定
			/// </summary>
			/// <param name="isActive"></param>
			void SetCharaIconActive(bool isActive);

			/// <summary>
			/// キャラアイコン設定
			/// </summary>
			/// <param name="atlas"></param>
			/// <param name="spriteName"></param>
			void SetCharaIcon(UIAtlas atlas, string spriteName);

			/// <summary>
			/// ランクを設定
			/// </summary>
			/// <param name="rank"></param>
			void SetRankActive(bool isActive, string rank);

			/// <summary>
			/// ランク色セット
			/// </summary>
			void SetRankColor(Color spriteColor, Color numGradientBottom, Color numGradientTop);

			/// <summary>
			/// パラメータ値設定
			/// </summary>
			/// <param name="parameterText"></param>
			void SetParameterActive(bool isActive, string parameterText, Color color);

			/// <summary>
			/// ロック表示の設定
			/// </summary>
			/// <param name="isActive"></param>
			void SetLockActive(bool isActive);

			/// <summary>
			/// Newアイコンの設定
			/// </summary>
			void SetNew(bool isNewFlag);

			/// <summary>
			/// 選択フレーム表示設定
			/// </summary>
			/// <param name="isActive"></param>
			void SetSelectFrameActive(bool isActive);

			/// <summary>
			/// 読み込みアイコンのアクティブ設定
			/// </summary>
			/// <param name="isActive"></param>
			void SetLoadingActive(bool isActive);

			/// <summary>
			/// キャラ存在アイコンアクティブ設定
			/// </summary>
			void SetExistActive(bool isActive);

			/// <summary>
			/// 無効表示のアクティブ設定
			/// </summary>
			/// <param name="isActive"></param>
			void SetDisableActive(bool isActive);

			/// <summary>
			/// 選択表示のアクティブ
			/// </summary>
			/// <param name="isActive"></param>
			void SetSelectNumberActive(bool isActive, string selectText);

			/// <summary>
			/// 無効表示時のテキストアクティブ設定
			/// </summary>
			/// <param name="isActive"></param>
			/// <param name="disableText"></param>
			void SetDisableTextActive(bool isActive, string disableText);

			/// <summary>
			/// 材料ランクの表示設定
			/// </summary>
			/// <param name="isActive"></param>
			/// <param name="rank"></param>
			void SetMaterialRankActive(bool isActive, string rank);

			/// <summary>
			/// 材料ランク色セット
			/// </summary>
			void SetMaterialRankColor(Color spriteColor, Color numGradientBottom, Color numGradientTop);

			/// <summary>
			/// 所有表示の設定
			/// </summary>
			/// <param name="isActive"></param>
			void SetPossessionActive(bool isActive, bool isPossession);

			/// <summary>
			/// ボタンの有効設定
			/// </summary>
			/// <param name="isEnable"></param>
			void SetButtonEnable(bool isEnable);

			/// <summary>
			/// アイテムが押された時の通知用
			/// </summary>
			event EventHandler OnItemClickEvent;

			/// <summary>
			/// アイテムが長押しされた時の通知用
			/// </summary>
			event EventHandler OnItemLongPressEvent;
		}

		/// <summary>
		/// キャラアイテム表示
		/// </summary>
		public class CharaItemView : GUIViewBase, IView
		{
			#region フィールド&プロパティ
			/// <summary>
			/// ボタン
			/// </summary>
			[SerializeField]
			private UIButton _button = null;
			private UIButton Button { get { return _button; } }

			/// <summary>
			/// アイコンのグループ
			/// </summary>
			[SerializeField]
			private GameObject _iconGroup = null;
			private GameObject IconGroup { get { return _iconGroup; } }

			/// <summary>
			/// BGのグループ
			/// </summary>
			[SerializeField]
			private GameObject _bgGroup = null;
			private GameObject BgGroup { get { return _bgGroup; } }

			/// <summary>
			/// 空オブジェクト
			/// </summary>
			[SerializeField]
			private EmptyAttachObject _emptyAttach = null;
			public EmptyAttachObject EmptyAttach { get { return _emptyAttach; } }
			[Serializable]
			public class EmptyAttachObject
			{
				/// <summary>
				/// 空オブジェクトのグループ
				/// </summary>
				[SerializeField]
				private GameObject _group = null;
				public GameObject Group { get { return _group; } }
			}

			/// <summary>
			/// キャラアイコンオブジェクト
			/// </summary>
			[SerializeField]
			private CharaIconAttachObject _charaIconAttach = null;
			public CharaIconAttachObject CharaIconAttach { get { return _charaIconAttach; } }
			[Serializable]
			public class CharaIconAttachObject
			{
				/// <summary>
				/// キャラアイコンのグループ
				/// </summary>
				[SerializeField]
				private GameObject _group = null;
				public GameObject Group { get { return _group; } }

				/// <summary>
				/// キャラアイコンスプライト
				/// </summary>
				[SerializeField]
				private UISprite _iconSprite = null;
				public UISprite IconSprite { get { return _iconSprite; } }

				/// <summary>
				/// ランク
				/// </summary>
				[SerializeField]
				private RankAttachObject _rankObject = null;
				public RankAttachObject RankObject { get { return _rankObject; } }

				/// <summary>
				/// パラメータグループ
				/// </summary>
				[SerializeField]
				private GameObject _parameterGroup = null;
				public GameObject ParameterGroup { get { return _parameterGroup; } }

				/// <summary>
				/// パラメータ値
				/// </summary>
				[SerializeField]
				private UILabel _parameterLabel = null;
				public UILabel ParameterLabel { get { return _parameterLabel; } }

				/// <summary>
				/// ロックアイコン
				/// </summary>
				[SerializeField]
				private UISprite _lockSprite = null;
				public UISprite LockSprite { get { return _lockSprite; } }

				/// <summary>
				/// Newグループ
				/// </summary>
				[SerializeField]
				private GameObject _newGroup = null;
				public GameObject NewGroup { get { return _newGroup; } }
			}

			/// <summary>
			/// ランクアタッチオブジェクト
			/// </summary>
			[Serializable]
			public class RankAttachObject
			{
				/// <summary>
				/// 素材ランクグループ
				/// </summary>
				[SerializeField]
				private GameObject _group = null;
				public GameObject Group { get { return _group; } }

				/// <summary>
				/// 星スプライト
				/// </summary>
				[SerializeField]
				private UISprite _starSprite = null;
				public UISprite StarSprite { get { return _starSprite; } }

				/// <summary>
				/// ランクラベル
				/// </summary>
				[SerializeField]
				private UILabel _rankLabel = null;
				public UILabel RankLabel { get { return _rankLabel; } }
			}

			/// <summary>
			/// 読み込みアイコンオブジェクト
			/// </summary>
			[SerializeField]
			private LoadIconAttachObject _loadIconAttach = null;
			public LoadIconAttachObject LoadIconAttach { get { return _loadIconAttach; } }
			[Serializable]
			public class LoadIconAttachObject
			{
				/// <summary>
				/// 読み込みアイコンのグループ
				/// </summary>
				[SerializeField]
				private GameObject _group = null;
				public GameObject Group { get { return _group; } }
			}

			/// <summary>
			/// キャラ存在アイコンオブジェクト
			/// </summary>
			[SerializeField]
			private ExistIconAttachObject _existIconAttach = null;
			public ExistIconAttachObject ExistIconAttach { get { return _existIconAttach; } }
			[Serializable]
			public class ExistIconAttachObject
			{
				/// <summary>
				/// キャラ存在アイコンのグループ
				/// </summary>
				[SerializeField]
				private GameObject _group = null;
				public GameObject Group { get { return _group; } }
			}

			/// <summary>
			/// 所有表示オブジェクト
			/// </summary>
			[SerializeField]
			private PossessionAttachObject _possessionAttach = null;
			public PossessionAttachObject PossessionAttach { get { return _possessionAttach; } }
			[Serializable]
			public class PossessionAttachObject
			{
				/// <summary>
				/// 表示グループ
				/// </summary>
				[SerializeField]
				private GameObject _group = null;
				public GameObject Group { get { return _group; } }

				/// <summary>
				/// 所有表示グループ
				/// </summary>
				[SerializeField]
				private GameObject _possessionGroup = null;
				public GameObject PossessionGroup { get { return _possessionGroup; } }

				/// <summary>
				/// 未所有表示グループ
				/// </summary>
				[SerializeField]
				private GameObject _notPossessionGroup = null;
				public GameObject NotPossessionGroup { get { return _notPossessionGroup; } }
			}

			/// <summary>
			/// 無効オブジェクト
			/// </summary>
			[SerializeField]
			private DisableAttachObject _disableAttach = null;
			public DisableAttachObject DisableAttach { get { return _disableAttach; } }
			[Serializable]
			public class DisableAttachObject
			{
				/// <summary>
				/// 無効グループ
				/// </summary>
				[SerializeField]
				private GameObject _group = null;
				public GameObject Group { get { return _group; } }

				/// <summary>
				/// 選択グループ
				/// </summary>
				[SerializeField]
				private GameObject _selectGroup = null;
				public GameObject SelectGroup { get { return _selectGroup; } }

				/// <summary>
				/// 選択ラベル
				/// </summary>
				[SerializeField]
				private UILabel _selectNumLabel = null;
				public UILabel SelectNumLabel { get { return _selectNumLabel; } }

				/// <summary>
				/// 選択時のテキストグループ
				/// </summary>
				[SerializeField]
				private GameObject _textGroup = null;
				public GameObject TextGroup { get { return _textGroup; } }

				/// <summary>
				/// 選択時のテキストラベル
				/// </summary>
				[SerializeField]
				private UILabel _textLabel = null;
				public UILabel TextLabel { get { return _textLabel; } }
			}

			/// <summary>
			/// 選択オブジェクト
			/// </summary>
			[SerializeField]
			private SelectAttachObject _selectAttach = null;
			public SelectAttachObject SelectAttach { get { return _selectAttach; } }
			[Serializable]
			public class SelectAttachObject
			{
				/// <summary>
				/// 選択グループ
				/// </summary>
				[SerializeField]
				private GameObject _group = null;
				public GameObject Group { get { return _group; } }
			}

			/// <summary>
			/// 素材ランク
			/// </summary>
			[SerializeField]
			private RankAttachObject _materialRankObject = null;
			public RankAttachObject MaterialRankObject { get { return _materialRankObject; } }

			/// <summary>
			/// アイテムが押された時の通知用
			/// </summary>
			public event EventHandler OnItemClickEvent = (sender, e) => { };

			/// <summary>
			/// アイテムが長押しされた時の通知用
			/// </summary>
			public event EventHandler OnItemLongPressEvent = (sender, e) => { };
			#endregion

			#region 破棄
			/// <summary>
			/// 破棄
			/// </summary>
			void OnDestroy()
			{
				this.OnItemClickEvent = null;
				this.OnItemLongPressEvent = null;
			}
			#endregion

			#region アクティブ設定
			/// <summary>
			/// アクティブ状態にする
			/// </summary>
			/// <param name="isActive"></param>
			public void SetActive(bool isActive)
			{
				this.SetRootActive(isActive, true);
			}
			/// <summary>
			/// 現在のアクティブ状態を取得する
			/// </summary>
			/// <returns></returns>
			public GUIViewBase.ActiveState GetActiveState()
			{
				return this.GetRootActiveState();
			}
			#endregion

			#region アイコン
			/// <summary>
			/// アイコンのアクティブ設定
			/// </summary>
			/// <param name="isActive"></param>
			public void SetIconActive(bool isActive)
			{
				if (this.IconGroup != null)
				{
					this.IconGroup.SetActive(isActive);
				}
				if (this.BgGroup != null)
				{
					this.BgGroup.SetActive(isActive);
				}
			}
			#endregion

			#region 空アイコン
			/// <summary>
			/// 空アイコンのアクティブ設定
			/// </summary>
			/// <param name="isActive"></param>
			public void SetEmptyActive(bool isActive)
			{
				if(this.EmptyAttach == null || this.EmptyAttach.Group == null)
				{
					return;
				}
				this.EmptyAttach.Group.SetActive(isActive);
			}
			#endregion

			#region キャラアイコン
			/// <summary>
			/// キャラアイコンのアクティブ設定
			/// </summary>
			/// <param name="isActive"></param>
			public void SetCharaIconActive(bool isActive)
			{
				if (this.CharaIconAttach == null || this.CharaIconAttach.Group == null) { return; }
				this.CharaIconAttach.Group.SetActive(isActive);
			}

			/// <summary>
			/// キャラアイコンの設定
			/// </summary>
			/// <param name="atlas"></param>
			/// <param name="spriteName"></param>
			public void SetCharaIcon(UIAtlas atlas, string spriteName)
			{
				if (this.CharaIconAttach == null || this.CharaIconAttach.IconSprite == null) { return; }
				
				CharaIcon.SetIconSprite(this.CharaIconAttach.IconSprite, atlas, spriteName);
				// ボタンの通常スプライトの方にも適用しないとホバーした時とか元に戻ってしまう
				if(this.Button != null)
				{
					this.Button.normalSprite = spriteName;
				}
			}

			/// <summary>
			/// ランクを設定
			/// </summary>
			public void SetRankActive(bool isActive, string rank)
			{
				if (this.CharaIconAttach == null || this.CharaIconAttach.RankObject == null || this.CharaIconAttach.RankObject.Group == null) { return; }

				this.CharaIconAttach.RankObject.Group.SetActive(isActive);
				if (this.CharaIconAttach.RankObject.Group != null)
				{
					this.CharaIconAttach.RankObject.RankLabel.text = rank;
				}
			}

			/// <summary>
			/// ランク色セット
			/// </summary>
			public void SetRankColor(Color spriteColor, Color numGradientBottom, Color numGradientTop)
			{
				if (this.CharaIconAttach == null || this.CharaIconAttach.RankObject == null) { return; }

				var rank = this.CharaIconAttach.RankObject;
				if (rank.StarSprite != null)
				{
					rank.StarSprite.color = spriteColor;
				}
				if (rank.RankLabel != null)
				{
					rank.RankLabel.gradientTop = numGradientTop;
					rank.RankLabel.gradientBottom = numGradientBottom;
				}
			}

			/// <summary>
			/// パラメータ値設定
			/// </summary>
			/// <param name="parameterText"></param>
			public void SetParameterActive(bool isActive, string parameterText, Color color)
			{
				if (this.CharaIconAttach == null || this.CharaIconAttach.ParameterGroup == null) { return; }

				this.CharaIconAttach.ParameterGroup.SetActive(isActive);
				if(this.CharaIconAttach.ParameterLabel != null)
				{
					this.CharaIconAttach.ParameterLabel.text = parameterText;
					this.CharaIconAttach.ParameterLabel.color = color;
				}
			}

			/// <summary>
			/// ロック表示の設定
			/// </summary>
			/// <param name="isActive"></param>
			public void SetLockActive(bool isActive)
			{
				if (this.CharaIconAttach == null) { return; }
				if(this.CharaIconAttach.LockSprite != null)
				{
					this.CharaIconAttach.LockSprite.gameObject.SetActive(isActive);
				}
			}

			/// <summary>
			/// Newアイコンの設定
			/// </summary>
			public void SetNew(bool isNewFlag)
			{
				if (this.CharaIconAttach == null || this.CharaIconAttach.NewGroup == null) { return; }
				this.CharaIconAttach.NewGroup.SetActive(isNewFlag);
			}
			#endregion

			#region ロード
			/// <summary>
			/// 読み込みアイコンのアクティブ設定
			/// </summary>
			/// <param name="isActive"></param>
			public void SetLoadingActive(bool isActive)
			{
				if (this.LoadIconAttach == null || this.LoadIconAttach.Group == null) { return; }
				this.LoadIconAttach.Group.SetActive(isActive);
			}
			#endregion

			#region キャラ存在アイコン
			/// <summary>
			/// キャラ存在アイコンアクティブ設定
			/// </summary>
			public void SetExistActive(bool isActive)
			{
				if (this.ExistIconAttach == null || this.ExistIconAttach.Group == null) { return; }
				this.ExistIconAttach.Group.SetActive(isActive);
			}
			#endregion

			#region 無効表示
			/// <summary>
			/// 無効表示のアクティブ設定
			/// </summary>
			/// <param name="isActive"></param>
			public void SetDisableActive(bool isActive)
			{
				if (this.DisableAttach == null || this.DisableAttach.Group == null) { return; }
				this.DisableAttach.Group.SetActive(isActive);
			}

			/// <summary>
			/// 選択番号表示のアクティブ
			/// </summary>
			/// <param name="isActive"></param>
			public void SetSelectNumberActive(bool isActive, string selectText)
			{
				if (this.DisableAttach == null) { return; }

				if(this.DisableAttach.SelectGroup != null)
				{
					this.DisableAttach.SelectGroup.SetActive(isActive);
				}
				if(this.DisableAttach.SelectNumLabel != null)
				{
					this.DisableAttach.SelectNumLabel.text = selectText;
				}
			}

			/// <summary>
			/// 無効表示時のテキストアクティブ設定
			/// </summary>
			/// <param name="isActive"></param>
			/// <param name="disableText"></param>
			public void SetDisableTextActive(bool isActive, string disableText)
			{
				if (this.DisableAttach == null) { return; }

				if(this.DisableAttach.TextGroup != null)
				{
					this.DisableAttach.TextGroup.SetActive(isActive);
				}
				if(this.DisableAttach.TextLabel != null)
				{
					this.DisableAttach.TextLabel.text = disableText;
				}
			}
			#endregion

			#region 選択
			/// <summary>
			/// 選択フレーム表示設定
			/// </summary>
			/// <param name="isActive"></param>
			public void SetSelectFrameActive(bool isActive)
			{
				if (this.SelectAttach == null) { return; }
				if (this.SelectAttach.Group != null)
				{
					this.SelectAttach.Group.SetActive(isActive);
				}
			}
			#endregion

			#region 材料ランク
			/// <summary>
			/// 材料ランクの表示設定
			/// </summary>
			/// <param name="isActive"></param>
			/// <param name="rank"></param>
			public void SetMaterialRankActive(bool isActive, string rank)
			{
				if (this.MaterialRankObject == null || this.MaterialRankObject.Group == null) { return; }

				this.MaterialRankObject.Group.SetActive(isActive);
				if (this.MaterialRankObject.RankLabel != null)
				{
					this.MaterialRankObject.RankLabel.text = rank;
				}
			}

			/// <summary>
			/// 材料ランク色セット
			/// </summary>
			public void SetMaterialRankColor(Color spriteColor, Color numGradientBottom, Color numGradientTop)
			{
				if (this.MaterialRankObject == null) { return; }

				var materialRank = this.MaterialRankObject;
				if(materialRank.StarSprite != null)
				{
					materialRank.StarSprite.color = spriteColor;
				}
				if (materialRank.RankLabel != null)
				{
					materialRank.RankLabel.gradientTop = numGradientTop;
					materialRank.RankLabel.gradientBottom = numGradientBottom;
				}
			}
			#endregion

			#region 所有表示
			/// <summary>
			/// 所有表示の設定
			/// </summary>
			/// <param name="isActive"></param>
			public void SetPossessionActive(bool isActive, bool isPossession)
			{
				if (this.PossessionAttach == null || this.PossessionAttach.Group == null) { return; }
				this.PossessionAttach.Group.SetActive(isActive);

				if (this.PossessionAttach.PossessionGroup != null || this.PossessionAttach.NotPossessionGroup != null)
				{
					if (isPossession)
					{
						// 所有表示
						this.PossessionAttach.PossessionGroup.SetActive(true);
						this.PossessionAttach.NotPossessionGroup.SetActive(false);
					}
					else
					{
						// 未所有表示
						this.PossessionAttach.PossessionGroup.SetActive(false);
						this.PossessionAttach.NotPossessionGroup.SetActive(true);
					}
				}
			}
			#endregion

			#region ボタン
			/// <summary>
			/// ボタンの有効設定
			/// </summary>
			/// <param name="isEnable"></param>
			public void SetButtonEnable(bool isEnable)
			{
				if (this.Button == null) { return; }
				this.Button.isEnabled = isEnable;
			}
			#endregion

			#region NGUIリフレクション
			/// <summary>
			/// アイテムが押された
			/// </summary>
			public void OnItemClick()
			{
				this.OnItemClickEvent(this, EventArgs.Empty);
			}

			/// <summary>
			/// アイテムが長押しされた
			/// </summary>
			public void OnLongPress()
			{
				this.OnItemLongPressEvent(this, EventArgs.Empty);
			}
			#endregion
		}
	}
}

