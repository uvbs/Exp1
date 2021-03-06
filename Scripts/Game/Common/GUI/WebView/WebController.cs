/// <summary>
/// WebView制御
/// 
/// 2015/12/09
/// </summary>
using UnityEngine;
using System.Collections;
using Asobimo.WebView;
using System;

namespace XUI
{
	namespace Web
	{
		/// <summary>
		/// Webコントローラインターフェイス
		/// </summary>
		public interface IController
		{
			/// <summary>
			/// Web表示
			/// </summary>
			/// <param name="url"></param>
			void Open(string url);

			/// <summary>
			/// Web表示 POSTをしながら行う
			/// </summary>
			/// <param name="url"></param>
			/// <param name="post"></param>
			void OpenPost(string url, WWWForm post);

			/// <summary>
			/// 閉じる
			/// </summary>
			void Close();

			/// <summary>
			/// お知らせを開く
			/// </summary>
			void OpenNews();

			/// <summary>
			/// お問い合わせを開く
			/// </summary>
			void OpenContact();

			/// <summary>
			/// 別アプリを起動させる
			/// </summary>
			/// <param name="url"></param>
			void OpenApplication(string url);

			/// <summary>
			/// ゲーム起動時のみ1度だけお知らせページを表示する
			/// 2回目以降このメソッドを呼び出しても表示はされない
			/// </summary>
			void OpenNewsStartAppOnly();

			/// <summary>
			/// アンケートページ表示
			/// </summary>
			void OpenEnquete();
		}

		/// <summary>
		/// Webコントローラ
		/// </summary>
		public class Controller : IController, Asobimo.WebView.PluginWebView.IWebViewListener
		{
			#region フィールド&プロパティ
			/// <summary>
			/// WebViewオブジェクト
			/// 実際にWebViewを表示/制御しているクラス
			/// </summary>
			//private WebViewObject webViewObj;

			private Asobimo.WebView.PluginWebView webView;
			
			
			/// <summary>
			/// 2Dカメラ
			/// </summary>
			private Camera screenCamera = null;

			/// <summary>
			/// UIRoot
			/// </summary>
			private UIRoot root = null;

			/// <summary>
			/// 1回でもお知らせページを開いたかどうか
			/// </summary>
			private bool isOnceOpenedNews = false;

			/// <summary>
			/// モデル
			/// </summary>
			private IModel model = null;

			/// <summary>
			/// ビュー
			/// </summary>
			private IView view = null;
			#endregion

			#region 初期化
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="model"></param>
			/// <param name="view"></param>
			public Controller(IModel model, IView view, Camera screenCamera, UIRoot root, Asobimo.WebView.PluginWebView webView)
			{
				this.model = model;
				this.view = view;

				// カメラセット
				this.screenCamera = screenCamera;
				// UIRootセット
				this.root = root;

				this.webView = webView;
			}
			#endregion

			#region 破棄
			/// <summary>
			/// デストラクタ
			/// </summary>
			~Controller()
			{
				//if(this.webViewObj != null) {
				//	this.webViewObj.Dispose();
				//}
				if(webView != null) {
					webView.Close();
				}
			}
			#endregion

			#region 表示


			/// <summary>
			/// Webを表示する
			/// </summary>
			/// <param name="url"></param>
			public void Open(string url)
			{
				if (this.model == null || this.view == null || webView == null) { return; }

				if(webView.Visible) return;

				//if (this.webViewObj != null)
				//{
				//	// WebViewが表示されている場合は生成処理を行わない
				//	if (this.webViewObj.IsShow) { return; }
				//}

				// Viewを表示
				this.view.Open();

				// WebViewObj生成
				CreateWebView(url);
			}



			/// <summary>
			/// ゲーム起動時のみ1度だけお知らせページを表示する
			/// 2回目以降このメソッドを呼び出しても表示はされない
			/// </summary>
			public void OpenNewsStartAppOnly()
			{
				if (this.isOnceOpenedNews || this.model == null) { return; }
				OpenNews();
				this.isOnceOpenedNews = true;
			}



			/// <summary>
			/// Web表示 POSTをしながら行う
			/// </summary>
			/// <param name="url"></param>
			/// <param name="post"></param>
			public void OpenPost(string url, WWWForm post)
			{
				if(this.model == null || this.view == null || webView == null) { return; }

				if(webView.Visible) return;

				//if (this.webViewObj != null)
				//{
				//	// WebViewが表示されている場合は生成処理を行わない
				//	if (this.webViewObj.IsShow) { return; }
				//}

				// Viewを表示
				this.view.Open();

				CreateWebView(url, post);
			}

			#endregion

			#region 閉じる
			public void Close()
			{
				if (this.view != null)
				{
					// Viewを閉じる
					this.view.Close();
				}

				if(webView != null) {
					webView.Close();
				}

				//if (this.webViewObj != null)
				//{
				//	// WebViewObjの削除
				//	this.webViewObj.Dispose();
				//}
			}
			#endregion

			#region お知らせ

			public void OpenNews()
			{
				string version = "";

				// iOSの時はバージョン取得
#if UNITY_IOS
				version = PluginController.PackageInfo.versionName1;
#endif

				WWWForm postData = Asobimo.WebAPI.AsobimoWebAPI.Instance.GetWebViewLoginPostData(Asobimo.WebAPI.AsobimoWebAPI.InformationURL, version);

				OpenPost(Asobimo.WebAPI.AsobimoWebAPI.Instance.GetWebViewLoginURL(), postData);
			}

			#endregion


			#region お問い合わせ

			public void OpenContact()
			{
				string version = "";

				// iOSの時はバージョン取得
#if UNITY_IOS
				version = PluginController.PackageInfo.versionName1;
#endif

				WWWForm postData = Asobimo.WebAPI.AsobimoWebAPI.Instance.GetWebViewLoginPostData(Asobimo.WebAPI.AsobimoWebAPI.ContactURL, version);

				OpenPost(Asobimo.WebAPI.AsobimoWebAPI.Instance.GetWebViewLoginURL(), postData);
			}

			#endregion


			#region 別アプリ起動
			/// <summary>
			/// 別アプリを起動させる
			/// </summary>
			/// <param name="url"></param>
			public void OpenApplication(string url)
			{
				// webViewが開いているなら閉じておく
				Close();

				// アプリ起動
				Application.OpenURL(url);
			}

			/// <summary>
			/// メーラ起動
			/// *現状使われていない*
			/// </summary>
			public void OpenMail()
			{
				string url = ObsolateSrc.MailScheme + ObsolateSrc.To;
				OpenApplication(url);
			}
			#endregion

			#region アンケート
			/// <summary>
			/// アンケートページの表示
			/// </summary>
			public void OpenEnquete()
			{
				if (this.model == null) { return; }

				// 表示
				Open(this.model.EnqueteURL);

				webView.PostURL(this.model.EnqueteURL, this.model.EnquetePostByteData);

				//if (this.webViewObj != null)
				//{
				//	// Post送信
				//	this.webViewObj.Post(this.model.EnquetePostData);
				//}
			}
			#endregion

			#region WebView生成
			/// <summary>
			/// WebViewの生成
			/// </summary>
			/// <param name="url"></param>
			private void CreateWebView(string url)
			{
				// ターゲットのWidgetを元に位置とサイズを計算.
				Rect rect = CalculateRect();

				if(!webView.IsCreated) {
					webView.Close();

					webView.Create();
				}

				webView.AbsoluteRect = true;
				webView.Rect = new Vector4(rect.x, rect.y, rect.width, rect.height);

				webView.SetListener(this);

				webView.LoadURL(url);
			}

			private void CreateWebView(string url, WWWForm post)
			{
				// ターゲットのWidgetを元に位置とサイズを計算.
				Rect rect = CalculateRect();
				
				if(!webView.IsCreated) {
					webView.Close();
					webView.Create();
				}

				webView.AbsoluteRect = true;
				webView.Rect = new Vector4(rect.x, rect.y, rect.width, rect.height);

				webView.SetListener(this);

				webView.PostURL(url, post.data);
			}
			#endregion

			#region WebViewの位置/サイズ
			/// <summary>
			/// WebViewの表示範囲を計算して取得
			/// </summary>
			/// <returns></returns>
			public Rect CalculateRect()
			{
				if (this.model == null || this.model.TargetWidget == null || this.root == null ||
					this.screenCamera == null)
					return new Rect(0, 0, 0, 0);

				UIWidget targetWidget = this.model.TargetWidget;
				UIRoot root = this.root;

				// 指定されたWidgetの左上の座標を取得しスクリーン座標に変換する.
				Vector3 screen = this.screenCamera.WorldToScreenPoint(targetWidget.worldCorners[0]);

				// UIRootで設定してあるデフォルトの画面の高さを取得する
				float defaultScreenHeight = root.activeHeight;

				// デフォルトの画面サイズと現在の画面サイズの比率を求める
				float scale = Screen.height / defaultScreenHeight;

				// 位置設定
				float y = screen.y;
				float x = screen.x;

				// サイズ.
				// 画面の比率に合わせて拡縮を行う
				float w = targetWidget.width * scale;
				float h = targetWidget.height * scale;

				Rect webRect = new Rect(x, y, w, h);

				// Webの短形を更新
				if (this.model != null)
				{
					this.model.WebRect = webRect;
				}
				if (this.view != null)
				{
					this.view.WebRect = webRect;
				}

				return webRect;
			}
			#endregion

			#region プラグイン内部→Unityコールバックメソッド.
			/// <summary>
			/// ページ読み込み開始時呼ばれる.
			/// </summary>
			public void OnPageStarted() { }

			/// <summary>
			/// ページ読み込み終了時呼ばれる.
			/// </summary>
			public void OnPageFinished()
			{
				if (this.view == null) { return; }

				webView.Visible = true;

				this.view.OnPageFinished();
			}

			/// <summary>
			/// ページ読み込みエラー時呼ばれる.
			/// </summary>
			public void OnReceivedError() { }

			/// <summary>
			/// スキーマで制限された時に呼ばれる.
			/// </summary>
			/// <param name='url'>
			/// URL.
			/// </param>
			private void OnShouldOverrideUrlLoading(string url) { }

			public bool OnShouldOverrideLoading(string url)
			{
				return true;
			}
			
			#endregion
		}
	}
}