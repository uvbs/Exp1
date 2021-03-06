/// <summary>
/// アソビモWeb表示
/// 
/// 2016/06/15
/// </summary>
using UnityEngine;
using System;

namespace XUI.AsobimoWeb
{
	/// <summary>
	/// アソビモWeb表示インターフェイス
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
	}

	/// <summary>
	/// アソビモWeb表示
	/// </summary>
	public class AsobimoWebView : GUIViewBase, IView
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void OnDestroy()
		{
		}
		#endregion

		#region アクティブ設定
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
	}
}
