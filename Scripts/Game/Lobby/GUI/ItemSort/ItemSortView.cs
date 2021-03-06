/// <summary>
/// アイテムソート表示
/// 
/// 2016/04/11
/// </summary>
using UnityEngine;
using System;

namespace XUI.ItemSort
{
	/// <summary>
	/// アイテムソート表示インターフェイス
	/// </summary>
	public interface IView
	{
		#region アクティブ設定
		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		void SetActive(bool isActive, bool isTweenSkip);

		/// <summary>
		/// 現在のアクティブ状態を取得する
		/// </summary>
		GUIViewBase.ActiveState GetActiveState();
		#endregion

		#region 閉じるボタン
		/// <summary>
		/// 閉じるボタンを押した時のイベント通知
		/// </summary>
		event EventHandler OnCloseClickEvent;
		#endregion

		#region ソート項目
		/// <summary>
		/// 名前ボタンが押された時のイベント通知
		/// </summary>
		event EventHandler OnNameClickEvent;
		/// <summary>
		/// 名前の有効設定
		/// </summary>
		void SetNameEnable(bool isEnable);

		/// <summary>
		/// 種類ボタンが押された時のイベント通知
		/// </summary>
		event EventHandler OnTypeClickEvent;
		/// <summary>
		/// 種類の有効設定
		/// </summary>
		void SetTypeEnable(bool isEnable);

		/// <summary>
		/// 入手ボタンが押された時のイベント通知
		/// </summary>
		event EventHandler OnObtainingClickEvent;
		/// <summary>
		/// 入手の有効設定
		/// </summary>
		void SetObtainingEnable(bool isEnable);
		#endregion

		#region 昇順/降順
		/// <summary>
		/// 昇順ボタンが押された時のイベント通知
		/// </summary>
		event EventHandler OnAscendClickEvent;
		/// <summary>
		/// 昇順有効設定
		/// </summary>
		void SetAscendEnable(bool isEnable);

		/// <summary>
		/// 降順ボタンが押された時のイベント通知
		/// </summary>
		event EventHandler OnDescendClickEvent;
		/// <summary>
		/// 降順有効設定
		/// </summary>
		void SetDescendEnable(bool isEnable);
		#endregion

		#region OKボタン
		/// <summary>
		/// OKボタンが押された時のイベント通知
		/// </summary>
		event EventHandler OnOkClickEvent;
		#endregion
	}

	/// <summary>
	/// アイテムソート表示
	/// </summary>
	public class ItemSortView : GUIViewBase, IView
	{
		#region アクティブ設定
		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		/// <param name="isActive"></param>
		/// <param name="isTweenSkip"></param>
		public void SetActive(bool isActive, bool isTweenSkip)
		{
			this.SetRootActive(isActive, isTweenSkip);
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

		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void OnDestroy()
		{
		}
		#endregion

		#region 閉じるボタン
		/// <summary>
		/// 閉じるボタンを押した時のイベント通知
		/// </summary>
		public event EventHandler OnCloseClickEvent = (sender, e) => { };
		public void OnCloseClick()
		{
			// 通知
			this.OnCloseClickEvent(this, EventArgs.Empty);
		}
		#endregion

		#region ソート項目
		/// <summary>
		/// 項目オブジェクト
		/// </summary>
		[SerializeField]
		private SortPatternAttachObject _sortPatternAttach = null;
		private SortPatternAttachObject SortPatternAttach { get { return _sortPatternAttach; } }
		[Serializable]
		private class SortPatternAttachObject
		{
			/// <summary>
			/// 名前ボタン
			/// </summary>
			[SerializeField]
			private XUIButton _nameButton = null;
			public XUIButton NameButton { get { return _nameButton; } }

			/// <summary>
			/// 種類ボタン
			/// </summary>
			[SerializeField]
			private XUIButton _TypeButton = null;
			public XUIButton TypeButton { get { return _TypeButton; } }

			/// <summary>
			/// 入手ボタン
			/// </summary>
			[SerializeField]
			private XUIButton _obtainingButton = null;
			public XUIButton ObtainingButton { get { return _obtainingButton; } }
		}

		/// <summary>
		/// 名前ボタンが押された時のイベント通知
		/// </summary>
		public event EventHandler OnNameClickEvent = (sender, e) => { };
		public void OnNameClick()
		{
			// 通知
			this.OnNameClickEvent(this, EventArgs.Empty);
		}
		/// <summary>
		/// 名前の有効設定
		/// </summary>
		public void SetNameEnable(bool isEnable)
		{
			if (this.SortPatternAttach == null || this.SortPatternAttach.NameButton == null) { return; }
			this.SortPatternAttach.NameButton.isEnabled = !isEnable;
		}

		/// <summary>
		/// 種類ボタンが押された時のイベント通知
		/// </summary>
		public event EventHandler OnTypeClickEvent = (sender, e) => { };
		public void OnTypeClick()
		{
			// 通知
			this.OnTypeClickEvent(this, EventArgs.Empty);
		}
		/// <summary>
		/// 種類の有効設定
		/// </summary>
		public void SetTypeEnable(bool isEnable)
		{
			if (this.SortPatternAttach == null || this.SortPatternAttach.TypeButton == null) { return; }
			this.SortPatternAttach.TypeButton.isEnabled = !isEnable;
		}

		/// <summary>
		/// 入手ボタンが押された時のイベント通知
		/// </summary>
		public event EventHandler OnObtainingClickEvent = (sender, e) => { };
		public void OnObtainingClick()
		{
			// 通知
			this.OnObtainingClickEvent(this, EventArgs.Empty);
		}
		/// <summary>
		/// 入手の有効設定
		/// </summary>
		public void SetObtainingEnable(bool isEnable)
		{
			if (this.SortPatternAttach == null || this.SortPatternAttach.ObtainingButton == null) { return; }
			this.SortPatternAttach.ObtainingButton.isEnabled = !isEnable;
		}
		#endregion

		#region 昇順/降順
		/// <summary>
		/// 昇順ボタン
		/// </summary>
		[SerializeField]
		private XUIButton _ascendButton = null;
		private XUIButton AscendButton { get { return _ascendButton; } }
		/// <summary>
		/// 降順ボタン
		/// </summary>
		[SerializeField]
		private XUIButton _descendButton = null;
		private XUIButton DescendButton { get { return _descendButton; } }

		/// <summary>
		/// 昇順ボタンが押された時のイベント通知
		/// </summary>
		public event EventHandler OnAscendClickEvent = (sender, e) => { };
		public void OnAscendClick()
		{
			// 通知
			this.OnAscendClickEvent(this, EventArgs.Empty);
		}
		/// <summary>
		/// 昇順有効設定
		/// </summary>
		public void SetAscendEnable(bool isEnable)
		{
			if (this.AscendButton == null) { return; }
			this.AscendButton.isEnabled = !isEnable;
		}

		/// <summary>
		/// 降順ボタンが押された時のイベント通知
		/// </summary>
		public event EventHandler OnDescendClickEvent = (sender, e) => { };
		public void OnDescendClick()
		{
			// 通知
			this.OnDescendClickEvent(this, EventArgs.Empty);
		}
		/// <summary>
		/// 降順有効設定
		/// </summary>
		public void SetDescendEnable(bool isEnable)
		{
			if (this.DescendButton == null) { return; }
			this.DescendButton.isEnabled = !isEnable;
		}
		#endregion

		#region OKボタン
		/// <summary>
		/// OKボタンが押された時のイベント通知
		/// </summary>
		public event EventHandler OnOkClickEvent = (sender, e) => { };
		public void OnOkClick()
		{
			// 通知
			this.OnOkClickEvent(this, EventArgs.Empty);
		}
		#endregion
	}
}
