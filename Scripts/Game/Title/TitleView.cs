/// <summary>
/// タイトル表示
/// 
/// 2015/12/14
/// </summary>
using UnityEngine;
using System.Collections;
using System;

namespace XUI
{
	namespace Title
	{
		/// <summary>
		/// タイトル表示インターフェイス
		/// </summary>
		public interface IView
		{
			/// <summary>
			/// アクティブ状態にする
			/// </summary>
			void SetActive(bool isActive);

			/// <summary>
			/// 開始時のタイトル表示
			/// </summary>
			void OpenStartTitle();

			/// <summary>
			/// 開始時のタイトル閉じる
			/// </summary>
			void CloseStartTitle();

			/// <summary>
			/// タイトルロゴ表示
			/// </summary>
			void OpenLogo();

			/// <summary>
			/// タイトルロゴを閉じる
			/// </summary>
			void CloseLogo();

			/// <summary>
			/// タイトル情報表示
			/// </summary>
			void OpenInfo();

			/// <summary>
			/// タイトル情報を閉じる
			/// </summary>
			void CloseInfo();

			/// <summary>
			/// タイトル上部のお知らせメッセージを設定する「
			/// </summary>
			void SetNewsMessage(string msg);

			/// <summary>
			/// 閉じる
			/// </summary>
			void Close();

			/// <summary>
			/// アプリのバージョン
			/// </summary>
			string AppVersion { set; }

			/// <summary>
			/// 遷移変更が押された時の通知用
			/// </summary>
			event EventHandler OnTouchChangeStateEvent;

			/// <summary>
			/// アンケートページボタンが押された時の通知用
			/// </summary>
			event EventHandler OnEnqueteEvent;

			/// <summary>
			/// 公式ページボタンが押された時の通知用
			/// </summary>
			event EventHandler OnHomePageEvent;

			/// <summary>
			/// ツイッターページボタンが押された時の通知用
			/// </summary>
			event EventHandler OnTwitterPageEvent;

			/// <summary>
			/// お知らせページボタンが押された時の通知用
			/// </summary>
			event EventHandler OnNewsPageEvent;

			/// <summary>
			/// ショップページボタンが押された時の通知用
			/// </summary>
			event EventHandler OnShopPageEvent;

			/// <summary>
			/// GoOneページボタンが押された時の通知用
			/// </summary>
			event EventHandler OnGoOnePageEvent;

			/// <summary>
			/// アソビモアカウント設定ボタンが押された時の通知用
			/// </summary>
			event EventHandler OnAsobimoAccountEvent;

			/// <summary>
			/// お問い合わせボタンが押された時の通知用
			/// </summary>
			event EventHandler OnInquiryEvent;
		}

		/// <summary>
		/// タイトル表示
		/// </summary>
		public class TitleView : GUIViewBase, IView
		{
			#region フィールド&プロパティ
			/// <summary>
			/// 開始時のタイトルTween
			/// </summary>
			[SerializeField]
			private UIPlayTween titleStartTweenGroup = null;

			/// <summary>
			/// タイトル情報のTween
			/// </summary>
			[SerializeField]
			private UIPlayTween titleInfoTweenGroup = null;

			/// <summary>
			/// タイトルロゴのTween
			/// </summary>
			[SerializeField]
			private UIPlayTween titleLogoTweenGroup = null;

			/// <summary>
			/// バージョンラベル
			/// </summary>
			[SerializeField]
			private UILabel versionLabel = null;

			/// <summary>
			/// お知らせラベル
			/// </summary>
			[SerializeField]
			private UILabel newsLabel = null;

			/// <summary>
			/// お知らせBG
			/// </summary>
			[SerializeField]
			private GameObject newsBGObject = null;

            [SerializeField]
            private GameObject[] buttons = null;

		    public UISprite ProtocolCheck;
		    public UITextList ProtocolText;
		    public GameObject ProtocolPanel;

			/// <summary>
			/// アプリのバージョン
			/// </summary>
			public string AppVersion
			{
				set
				{
					if (this.versionLabel == null) { return; }
					this.versionLabel.text = value;
				}
			}

			/// <summary>
			/// 遷移変更が押された時の通知用
			/// </summary>
			public event EventHandler OnTouchChangeStateEvent = (sender, e) => { };

			/// <summary>
			/// アンケートページボタンが押された時の通知用
			/// </summary>
			public event EventHandler OnEnqueteEvent = (sender, e) => { };

			/// <summary>
			/// 公式ページボタンが押された時の通知用
			/// </summary>
			public event EventHandler OnHomePageEvent = (sender, e) => { };

			/// <summary>
			/// ツイッターページボタンが押された時の通知用
			/// </summary>
			public event EventHandler OnTwitterPageEvent = (sender, e) => { };

			/// <summary>
			/// お知らせページボタンが押された時の通知用
			/// </summary>
			public event EventHandler OnNewsPageEvent = (sender, e) => { };

			/// <summary>
			/// ショップページボタンが押された時の通知用
			/// </summary>
			public event EventHandler OnShopPageEvent = (sender, e) => { };

			/// <summary>
			/// GoOneページボタンが押された時の通知用
			/// </summary>
			public event EventHandler OnGoOnePageEvent = (sender, e) => { };

			/// <summary>
			/// アソビモアカウント設定ボタンが押された時の通知用
			/// </summary>
			public event EventHandler OnAsobimoAccountEvent = (sender, e) => { };

			/// <summary>
			/// お問い合わせボタンが押された時の通知用
			/// </summary>
			public event EventHandler OnInquiryEvent = (sender, e) => { };
            #endregion

            protected override void Awake() {
                base.Awake();
#if PLATE_NUMBER_REVIEW
                if (buttons != null) {
                    foreach (GameObject button in buttons) {
                        button.SetActive(false);
                    }
                }
#endif
            }

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

            #region 破棄
			/// <summary>
			/// 破棄
			/// </summary>
			void OnDestroy()
			{
				this.OnTouchChangeStateEvent = null;
				this.OnEnqueteEvent = null;
				this.OnHomePageEvent = null;
				this.OnTwitterPageEvent = null;
				this.OnNewsPageEvent = null;
				this.OnShopPageEvent = null;
				this.OnGoOnePageEvent = null;
				this.OnAsobimoAccountEvent = null;
				this.OnInquiryEvent = null;
			}
#endregion

#region 開始時のタイトル
			/// <summary>
			/// 開始時のタイトル表示
			/// </summary>
			public void OpenStartTitle()
			{
				if (this.titleStartTweenGroup != null)
				{
					this.titleStartTweenGroup.Play(false);
				}
			    ProtocolPanel.transform.parent.gameObject.SetActive(true);
                Debug.Log("===> 用户协议长度 " + Eula.Text.Length);
			    ProtocolText.Add(Eula.Text);
				ProtocolPanel.transform.localScale = Vector3.zero;
			}

			/// <summary>
			/// 開始時のタイトル閉じる
			/// </summary>
			public void CloseStartTitle()
			{
				if (this.titleStartTweenGroup != null)
				{
					this.titleStartTweenGroup.Play(true);
				}
			}
#endregion

#region タイトルロゴ
			/// <summary>
			/// タイトルロゴを表示
			/// </summary>
			public void OpenLogo()
			{
				if (this.titleLogoTweenGroup != null)
				{
					this.titleLogoTweenGroup.Play(false);
				}
			}

			/// <summary>
			/// タイトルロゴを閉じる
			/// </summary>
			public void CloseLogo()
			{
				if (this.titleLogoTweenGroup != null)
				{
					this.titleLogoTweenGroup.Play(true);
				}
			    ProtocolPanel.transform.parent.gameObject.SetActive(false);
			}
#endregion

#region タイトル情報
			/// <summary>
			/// タイトル情報を表示
			/// </summary>
			public void OpenInfo()
			{
				if (this.titleInfoTweenGroup != null)
				{
					this.titleInfoTweenGroup.Play(true);
				}
			}

			/// <summary>
			/// タイトル情報を閉じる
			/// </summary>
			public void CloseInfo()
			{
				if (this.titleInfoTweenGroup != null)
				{
					this.titleInfoTweenGroup.Play(false);
				}
			}
#endregion

			/// <summary>
			/// タイトル上部のお知らせメッセージを設定する「
			/// </summary>
			public void SetNewsMessage(string msg)
			{
				if(this.newsLabel != null)
				{
					this.newsLabel.text = msg;
				}
				if(this.newsBGObject != null)
				{
					bool isActive = !string.IsNullOrEmpty(msg);
					this.newsBGObject.SetActive(isActive);
				}
			}

#region 閉じる
			/// <summary>
			/// タイトルの表示を閉じる
			/// </summary>
			public void Close()
			{
				CloseLogo();
				CloseStartTitle();
				CloseInfo();
				ProtocolPanel.transform.localScale = Vector3.zero;
			}
#endregion

#region NGUIリフレクション
			/// <summary>
			/// 遷移変更が押された時
			/// </summary>
			public void OnTouchChangeState()
			{
			    if (!ProtocolIsCheck)
			    {
			        return;
			    }
				// コントローラ側に通知
				this.OnTouchChangeStateEvent(this, EventArgs.Empty);
			}

			/// <summary>
			/// アンケートページボタンが押された時
			/// </summary>
			public void OnEnquetePage()
			{
				this.OnEnqueteEvent(this, EventArgs.Empty);
			}

			/// <summary>
			/// 公式ページボタンが押された時
			/// </summary>
			public void OnHomePage()
			{
				this.OnHomePageEvent(this, EventArgs.Empty);
			}

			/// <summary>
			/// ツイッターページボタンが押された時
			/// </summary>
			public void OnTwitterPage()
			{
				this.OnTwitterPageEvent(this, EventArgs.Empty);
			}

			/// <summary>
			/// お知らせページボタンが押された時
			/// </summary>
			public void OnNewsPage()
			{
				this.OnNewsPageEvent(this, EventArgs.Empty);
			}

			/// <summary>
			/// ショップページボタンが押された時
			/// </summary>
			public void OnShopPage()
			{
				this.OnShopPageEvent(this, EventArgs.Empty);
			}

			/// <summary>
			/// GoOneページボタンが押された時
			/// </summary>
			public void OnGoOnePage()
			{
				this.OnGoOnePageEvent(this, EventArgs.Empty);
			}

			/// <summary>
			/// アソビモアカウント設定ボタンが押された時
			/// </summary>
			public void OnAsobimoAccount()
			{
				this.OnAsobimoAccountEvent(this, EventArgs.Empty);
			}

			/// <summary>
			/// お問い合わせボタンが押された時
			/// </summary>
			public void OnInquiry()
			{
				this.OnInquiryEvent(this, EventArgs.Empty);
			}

		    public bool ProtocolIsCheck = true;
		    public void OnProtocolCheckClick()
		    {
		        ProtocolIsCheck = !ProtocolIsCheck;
		        ProtocolCheck.alpha = ProtocolIsCheck ? 1f : 0.01f;
		    }

		    public void OnProtocolClick()
		    {
				ProtocolPanel.transform.localScale = Vector3.one;
		    }

		    public void OnProtocolBack()
		    {
				ProtocolPanel.transform.localScale = Vector3.zero;
		    }
		    #endregion
		}
	}
}