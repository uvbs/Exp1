/// <summary>
/// アイテム表示
/// 
/// 2016/03/16
/// </summary>
using UnityEngine;
using System;

namespace XUI.Item
{
	/// <summary>
	/// アイテム表示インターフェイス
	/// </summary>
	public interface IView
	{
		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		void SetActive(bool isActive);

		/// <summary>
		/// 現在のアクティブ状態を取得する
		/// </summary>
		GUIViewBase.ActiveState GetActiveState();

		/// <summary>
		/// 空表示のアクティブ設定
		/// </summary>
		void SetEmptyActive(bool isActive);

		/// <summary>
		/// アイテムアイコンのアクティブ設定
		/// </summary>
		void SetItemIconActive(bool isActive);

		/// <summary>
		/// アイテムアイコン設定
		/// </summary>
		void SetItemIcon(UIAtlas atlas, string spriteName);

		/// <summary>
		/// アイテムアイコンに表示する付加情報グループのアクティブ設定
		/// </summary>
		void SetIconInfo(bool isActive);

		/// <summary>
		/// スタック数設定
		/// </summary>
		void SetStack(string stack);

		/// <summary>
		/// Newアイコンの設定
		/// </summary>
		void SetNew(bool isNewFlag);

		/// <summary>
		/// 選択フレーム表示設定
		/// </summary>
		void SetSelectFrameActive(bool isActive);

		/// <summary>
		/// 読み込みアイコンのアクティブ設定
		/// </summary>
		void SetLoadingActive(bool isActive);

		/// <summary>
		/// 無効表示のアクティブ設定
		/// </summary
		void SetDisableActive(bool isActive);

		/// <summary>
		/// 選択表示のアクティブ
		/// </summary>
		void SetSelectNumberActive(bool isActive, string selectText);

		/// <summary>
		/// 無効表示時のテキストアクティブ設定
		/// </summary>
		void SetDisableTextActive(bool isActive, string disableText);

		/// <summary>
		/// アイテムが押された時の通知用
		/// </summary>
		event EventHandler OnItemClickEvent;
	}

	/// <summary>
	/// アイテム表示
	/// </summary>
	public class ItemView : GUIViewBase, IView
	{
		#region フィールド&プロパティ
		/// <summary>
		/// ボタン
		/// </summary>
		[SerializeField]
		private UIButton _button = null;
		private UIButton Button { get { return _button; } }

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
		/// アイテムアイコンオブジェクト
		/// </summary>
		[SerializeField]
		private ItemIconAttachObject _itemIconAttach = null;
		public ItemIconAttachObject ItemIconAttach { get { return _itemIconAttach; } }
		[Serializable]
		public class ItemIconAttachObject
		{
			/// <summary>
			/// アイテムアイコンのグループ
			/// </summary>
			[SerializeField]
			private GameObject _group = null;
			public GameObject Group { get { return _group; } }

			/// <summary>
			/// アイテムアイコンのスプライト
			/// </summary>
			[SerializeField]
			private UISprite _iconSprite = null;
			public UISprite IconSprite { get { return _iconSprite; } }

			/// <summary>
			/// アイコン情報グループ
			/// </summary>
			[SerializeField]
			private GameObject _groupInfo = null;
			public GameObject GroupInfo { get { return _groupInfo; } }

			/// <summary>
			/// スタック数のラベル
			/// </summary>
			[SerializeField]
			private UILabel _stackLabel = null;
			public UILabel StackLabel { get { return _stackLabel; } }

			/// <summary>
			/// Newグループ
			/// </summary>
			[SerializeField]
			private GameObject _newGroup = null;
			public GameObject NewGroup { get { return _newGroup; } }
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
			/// 選択時の番号ラベル
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
		/// アイテムが押された時の通知用
		/// </summary>
		public event EventHandler OnItemClickEvent = (sender, e) => { };
		#endregion

		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void OnDestroy()
		{
			this.OnItemClickEvent = null;
		}
		#endregion

		#region アクティブ設定
		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		public void SetActive(bool isActive)
		{
			this.SetRootActive(isActive, true);
		}
		/// <summary>
		/// 現在のアクティブ状態を取得する
		/// </summary>
		public GUIViewBase.ActiveState GetActiveState()
		{
			return this.GetRootActiveState();
		}
		#endregion

		#region 空アイコン
		/// <summary>
		/// 空アイコンのアクティブ設定
		/// </summary>
		public void SetEmptyActive(bool isActive)
		{
			if (this.EmptyAttach == null || this.EmptyAttach.Group == null) { return; }
			this.EmptyAttach.Group.SetActive(isActive);
		}
		#endregion

		#region アイテムアイコン
		/// <summary>
		/// アイテムアイコンのアクティブ設定
		/// </summary>
		public void SetItemIconActive(bool isActive)
		{
			if (this.ItemIconAttach == null || this.ItemIconAttach.Group == null) { return; }
			this.ItemIconAttach.Group.SetActive(isActive);
		}

		/// <summary>
		/// アイテムアイコンの設定
		/// </summary>
		public void SetItemIcon(UIAtlas atlas, string spriteName)
		{
			if (this.ItemIconAttach == null || this.ItemIconAttach.IconSprite == null) { return; }
			ItemIcon.SetIconSprite(this.ItemIconAttach.IconSprite, atlas, spriteName);
			// ボタンの通常スプライトの方にも適用しないとホバーした時とか元に戻ってしまう
			if (this.Button != null)
			{
				this.Button.normalSprite = spriteName;
			}
		}

		/// <summary>
		/// アイテムアイコンに表示する付加情報グループのアクティブ設定
		/// </summary>
		public void SetIconInfo(bool isActive)
		{
			if (this.ItemIconAttach == null || this.ItemIconAttach.GroupInfo == null) { return; }
			this.ItemIconAttach.GroupInfo.SetActive(isActive);
		}

		/// <summary>
		/// スタック数設定
		/// </summary>
		public void SetStack(string stack)
		{
			if (this.ItemIconAttach == null || this.ItemIconAttach.StackLabel == null) { return; }
			this.ItemIconAttach.StackLabel.text = stack;
		}

		/// <summary>
		/// Newアイコンの設定
		/// </summary>
		public void SetNew(bool isNewFlag)
		{
			if (this.ItemIconAttach == null || this.ItemIconAttach.NewGroup == null) { return; }
			this.ItemIconAttach.NewGroup.SetActive(isNewFlag);
		}
		#endregion

		#region ロード
		/// <summary>
		/// 読み込みアイコンのアクティブ設定
		/// </summary>
		public void SetLoadingActive(bool isActive)
		{
			if (this.LoadIconAttach == null || this.LoadIconAttach.Group == null) { return; }
			this.LoadIconAttach.Group.SetActive(isActive);
		}
		#endregion

		#region 無効表示
		/// <summary>
		/// 無効表示のアクティブ設定
		/// </summary>
		public void SetDisableActive(bool isActive)
		{
			if (this.DisableAttach == null || this.DisableAttach.Group == null) { return; }
			this.DisableAttach.Group.SetActive(isActive);
		}

		/// <summary>
		/// 選択番号表示のアクティブ
		/// </summary>
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
		public void SetSelectFrameActive(bool isActive)
		{
			if (this.SelectAttach == null) { return; }
			if(this.SelectAttach.Group != null)
			{
				this.SelectAttach.Group.SetActive(isActive);
			}
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
		#endregion

	}
}
