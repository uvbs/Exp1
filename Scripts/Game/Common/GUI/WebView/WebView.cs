/// <summary>
/// WebView表示
/// 
/// 2015/12/09
/// </summary>
using UnityEngine;
using System;
using System.Collections;

namespace XUI
{
	namespace Web
	{
		/// <summary>
		/// Web表示インターフェイス
		/// </summary>
		public interface IView
		{
			/// <summary>
			/// アクティブ状態にする
			/// </summary>
			void SetActive(bool isActive);

			/// <summary>
			/// 開く処理
			/// </summary>
			void Open();

			/// <summary>
			/// 閉じる処理
			/// </summary>
			void Close();

			/// <summary>
			/// ページ読み込み終了時
			/// </summary>
			void OnPageFinished();

			/// <summary>
			/// 実際に表示しているWebの短形
			/// </summary>
			Rect WebRect { get; set; }
		}

		/// <summary>
		/// Web表示
		/// </summary>
		public class WebView : GUIViewBase, IView
		{
			#region フィールド&プロパティ
			/// <summary>
			/// ロードアイコン
			/// </summary>
			[SerializeField]
			private GameObject loadIcon = null;
			public GameObject LoadIcon { get { return loadIcon; } }

			/// <summary>
			/// 実際に表示しているWebの短形
			/// </summary>
			public Rect WebRect { get; set; }
			#endregion

			#region 初期化
			protected override void Awake()
			{
				base.Awake();
				SetActive(false);
			}
			#endregion

			#region アクティブ設定
			/// <summary>
			/// アクティブ状態にする
			/// </summary>
			/// <param name="isActive"></param>
			public void SetActive(bool isActive)
			{
				this.SetRootActive(isActive);
			}

			/// <summary>
			/// 現在のアクティブ状態を取得する
			/// </summary>
			/// <returns></returns>
			public GUIViewBase.ActiveState GetActiveState()
			{
				return this.GetActiveState();
			}
			#endregion

			#region 開く&閉じる
			/// <summary>
			/// 開く処理
			/// </summary>
			public void Open()
			{
				// ウィンドウのアクティブをON
				SetActive(true);

				if (this.LoadIcon != null)
				{
					// ロードアイコン表示
					this.LoadIcon.SetActive(true);
				}
			}

			/// <summary>
			/// 閉じる処理
			/// </summary>
			public void Close()
			{
				// ウィンドウのアクティブをOFF
				SetActive(false);

				if (this.LoadIcon != null)
				{
					// ロードアイコン非表示
					this.LoadIcon.SetActive(false);
				}
			}
			#endregion

			#region プラグイン内部→Unityコールバックメソッド
			/// <summary>
			/// ページ読み込み終了時
			/// </summary>
			public void OnPageFinished()
			{
				if (this.LoadIcon != null)
				{
					// ロードアイコン非表示
					this.LoadIcon.SetActive(false);
				}
			}
			#endregion
		}
	}
}