/// <summary>
/// アソビモWeb制御
/// 
/// 2016/06/15
/// </summary>
using System;
using System.Collections.Generic;
using Asobimo.WebAPI;

namespace XUI.AsobimoWeb
{
	#region 定数
	/// <summary>
	/// アクセスタイプ
	/// </summary>
	public enum AccessType
	{
		None,
		GetReviewInfomation,	// ①審査情報取得
		CheckGameMaintenance,	// ②ゲームメンテナンス状態判定
		CheckOpenBrowser,		// ③初回ブラウザ起動可否判定
		CheckDisplayTitle,		// ④ゲームタイトル表示可否判定
		RegisterReviewUser,		// ⑦審査チェックユーザ登録
		Finish,					// 終了
	};
	#endregion

	#region イベント引数
	/// <summary>
	/// 終了イベント引数
	/// </summary>
	public class FinishEventArgs : EventArgs
	{
		/// <summary>
		/// 結果
		/// </summary>
		public bool IsResult { get; private set; }

		/// <summary>
		/// HTTPステータス
		/// </summary>
		public int HttpStatus { get; private set; }

		/// <summary>
		/// 審査中か
		/// </summary>
		public bool IsUnderReview { get; private set; }

		/// <summary>
		/// 審査バージョン
		/// </summary>
		public string ReviewVersion { get; private set; }

		/// <summary>
		/// ゲームメンテナンスフラグ
		/// </summary>
		public bool IsGameMaintenance { get; private set; }

		/// <summary>
		/// タイトル表示可否フラグ
		/// </summary>
		public bool IsDisplayTitle { get; private set; }

		/// <summary>
		/// 審査チェックユーザ登録成功フラグ
		/// </summary>
		public bool ReviewUserResult { get; private set; }

		public FinishEventArgs(bool isResult, int httpStatus, bool isUnderReview, string reviewVersion, bool isGameMaintenance, bool isDisplayTitle, bool reviewUserResult)
		{
			this.IsResult = isResult;
			this.HttpStatus = httpStatus;
			this.IsUnderReview = isUnderReview;
			this.ReviewVersion = reviewVersion;
			this.IsGameMaintenance = isGameMaintenance;
			this.IsDisplayTitle = isDisplayTitle;
			this.ReviewUserResult = ReviewUserResult;
		}
	}
	#endregion

	/// <summary>
	/// アソビモWeb制御インターフェイス
	/// </summary>
	public interface IController
	{
		#region プロパティ
		/// <summary>
		/// アクセス状態
		/// </summary>
		AccessType AccessState { get; }

		/// <summary>
		/// アクセス状態ログ表示用
		/// </summary>
		AccessType AccessStateLog { get; }

		/// <summary>
		/// Webのアクセスが終了しているかどうか
		/// </summary>
		bool IsFinish { get; }

		/// <summary>
		/// アクセス結果
		/// </summary>
		bool IsResult { get; }

		/// <summary>
		/// エラーメッセージ
		/// </summary>
		string Error { get; }

		/// <summary>
		/// HTTPステータス
		/// </summary>
		int HttpStatus { get; }

		#region 審査情報
		/// <summary>
		/// 審査中か
		/// </summary>
		bool IsUnderReview { get; }

		/// <summary>
		/// 審査バージョン
		/// </summary>
		string ReviewVersion { get; }
		#endregion

		#region ゲームメンテナンス
		/// <summary>
		/// ゲームメンテナンスフラグ
		/// </summary>
		bool IsGameMaintenance { get; }
		#endregion

		#region タイトル表示可否
		/// <summary>
		/// タイトル表示可否フラグ
		/// </summary>
		bool IsDisplayTitle { get; }
		#endregion

		#region 審査チェックユーザ登録
		/// <summary>
		/// 審査チェックユーザ登録成功フラグ
		/// </summary>
		bool ReviewUserResult { get; }
		#endregion

		#endregion

		#region アクティブ設定
		/// <summary>
		/// アクティブ設定
		/// </summary>
		void SetActive(bool isActive, bool isTweenSkip);
		#endregion

		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();
		#endregion

		#region アクセス
		/// <summary>
		/// アクセス開始
		/// </summary>
		void StartAccess(EventHandler<FinishEventArgs> callback = null);
		#endregion
	}

	/// <summary>
	/// アソビモWeb制御
	/// </summary>
	public class Controller : IController
	{
		#region フィールド&プロパティ
		/// <summary>
		/// モデル
		/// </summary>
		private readonly AsobimoWeb.IModel _model;
		private AsobimoWeb.IModel Model { get { return _model; } }

		/// <summary>
		/// ビュー
		/// </summary>
		private readonly AsobimoWeb.IView _view;
		private AsobimoWeb.IView View { get { return _view; } }

		/// <summary>
		/// 更新できる状態かどうか
		/// </summary>
		public bool CanUpdate
		{
			get
			{
				if (this.Model == null) { return false; }
				if (this.View == null) { return false; }
				if (this.WebAPI == null) { return false; }
				return true;
			}
		}

		/// <summary>
		/// アクセス状態
		/// </summary>
		public AccessType AccessState { get; private set; }

		/// <summary>
		/// アクセス状態ログ表示用
		/// </summary>
		public AccessType AccessStateLog { get; private set; }

		/// <summary>
		/// Webのアクセスが終了しているかどうか
		/// </summary>
		public bool IsFinish { get { return (this.AccessState == AccessType.None || this.AccessState == AccessType.Finish); } }

		/// <summary>
		/// アクセス結果
		/// </summary>
		public bool IsResult { get { return string.IsNullOrEmpty(this.Error); } }

		/// <summary>
		/// エラーメッセージ
		/// </summary>
		public string Error { get; private set; }

		/// <summary>
		/// HTTPステータス
		/// </summary>
		public int HttpStatus { get { return (this.Model != null) ? this.Model.HttpStatus : -1; } }

		#region 審査情報
		/// <summary>
		/// 審査中か
		/// </summary>
		public bool IsUnderReview { get { return (this.Model != null) ? this.Model.IsUnderReview : false; } }

		/// <summary>
		/// 審査バージョン
		/// </summary>
		public string ReviewVersion { get { return (this.Model != null) ? this.Model.ReviewVersion : string.Empty; } }
		#endregion

		#region ゲームメンテナンス
		/// <summary>
		/// ゲームメンテナンスフラグ
		/// </summary>
		public bool IsGameMaintenance { get { return (this.Model != null) ? this.Model.IsGameMaintenance : false; } }
		#endregion

		#region タイトル表示可否
		/// <summary>
		/// タイトル表示可否フラグ
		/// </summary>
		public bool IsDisplayTitle { get { return (this.Model != null) ? this.Model.IsDisplayTitle : false; } }
		#endregion

		#region 審査チェックユーザ登録
		/// <summary>
		/// 審査チェックユーザ登録成功フラグ
		/// </summary>
		public bool ReviewUserResult { get { return (this.Model != null) ? this.Model.ReviewUserResult : false; } }
		#endregion

		/// <summary>
		/// アソビモWebAPI
		/// </summary>
		private readonly Asobimo.WebAPI.AsobimoWebAPI _webAPI;
		private Asobimo.WebAPI.AsobimoWebAPI WebAPI { get { return _webAPI; } }

		/// <summary>
		/// アクセス終了時のコールバック用
		/// </summary>
		private event EventHandler<FinishEventArgs> OnFinishEvent = (sender, e) => { };
		#endregion

		#region 初期化
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Controller(IModel model, IView view, Asobimo.WebAPI.AsobimoWebAPI webAPI)
		{
			UnityEngine.Assertions.Assert.IsNotNull(model, "AsobimoWebController Constructor: Model is Null");
			UnityEngine.Assertions.Assert.IsNotNull(view, "AsobimoWebController Constructor: View is Null");
			UnityEngine.Assertions.Assert.IsNotNull(webAPI, "AsobimoWebController Constructor: AsobimoWebAPI is Null");
			if (model == null || view == null || webAPI == null) { return; }

			// ビュー設定
			this._view = view;

			// モデル設定
			this._model = model;

			// WebAPI設定
			this._webAPI = webAPI;

			// データ初期化
			this.ChangeAccess(AccessType.None);
			this.Error = string.Empty;
			this.AccessState = AccessType.None;
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			if(this.CanUpdate)
			{
				this.Model.Dispose();
			}
			this.OnFinishEvent = null;
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

		#region アクセス
		/// <summary>
		/// アクセス開始
		/// </summary>
		public void StartAccess(EventHandler<FinishEventArgs> callback = null)
		{
			if (!this.CanUpdate) { return; }
			
			// すでにアクセス中
			if (!this.IsFinish) { return; }

			GUIDebugLog.AddMessage(string.Format("WebAPI StartAccess AsobimoID={0} AsobimoToken={1}", this.WebAPI.AsobimoId, this.WebAPI.AsobimoToken));

			// データリセット
			this.ClearResponseData();
			this.OnFinishEvent = callback;

			// 審査情報取得開始
			this.ChangeAccess(AccessType.GetReviewInfomation);
		}

		/// <summary>
		/// 各送られてくる情報をリセットする
		/// </summary>
		private void ClearResponseData()
		{
			this.Error = string.Empty;
			this.Model.IsUnderReview = false;
			this.Model.ReviewVersion = string.Empty;
			this.Model.IsGameMaintenance = false;
			this.Model.IsDisplayTitle = false;
			this.Model.ReviewUserResult = false;
			this.Model.HttpStatus = -1;
		}

		/// <summary>
		/// アクセス状態切替
		/// </summary>
		private void ChangeAccess(AccessType state)
		{
			this.AccessState = state;

			switch(state)
			{
				case AccessType.GetReviewInfomation:
					this.GetReviewInfomation();
					break;
				case AccessType.CheckGameMaintenance:
					this.CheckGameMaintenance();
					break;
				case AccessType.CheckOpenBrowser:
#if UNITY_IOS
					// 死ぬので、スキップ
					this.ChangeAccess(AccessType.Finish);
#else
					this.CheckOpenBrowser();
#endif
					break;
				case AccessType.CheckDisplayTitle:
					this.CheckDisplayTitle();
					break;
				case AccessType.RegisterReviewUser:
					this.RegisterReviewUser();
					break;
				case AccessType.Finish:
					this.Finished();
					break;
			}
		}
		#endregion

		#region 審査情報取得
		/// <summary>
		/// 審査情報取得
		/// </summary>
		private void GetReviewInfomation()
		{
			if (!this.CanUpdate) { return; }
			// 送信
			this.AccessStateLog = this.AccessState;
			GUIDebugLog.AddMessage("WebAPI Send GetReviewInfomation");
			this.WebAPI.GetReviewInformation(this.Response);
		}

		/// <summary>
		/// 審査情報取得レスポンス
		/// </summary>
		private void Response(AsobimoWebAPI.WebAPIRequest<AsobimoWebAPI.GetReviewInformationResultJson> resultJson)
		{
			if (!this.CanUpdate) { return; }

			GUIDebugLog.AddMessage("WebAPI Response GetReviewInfomation");

			// HTTPステータス更新
			this.Model.HttpStatus = resultJson.HttpStatus;

			// エラーチェック
			if (resultJson == null)
			{
				this.Error = string.Format("AccessState={0} NULL::WebAPIRequest", this.AccessState);
				this.ChangeAccess(AccessType.Finish);
			}
			else if (resultJson.HasError)
			{
				this.Error = resultJson.Error;
				this.ChangeAccess(AccessType.Finish);
			}
			else
			{
				this.GetReviewInfomationResult(resultJson.Result);
			}
		}

		/// <summary>
		/// 審査情報取得結果
		/// </summary>
		private void GetReviewInfomationResult(AsobimoWebAPI.GetReviewInformationResultJson resultInfo)
		{
			if (resultInfo != null)
			{
				// 情報更新
				this.Model.IsUnderReview = resultInfo.IsUnderReview;
				this.Model.ReviewVersion = resultInfo.ReviewVersion;

				if (resultInfo.IsUnderReview)
				{
					// レビュー中なので審査チェックユーザ登録開始
					this.ChangeAccess(AccessType.RegisterReviewUser);
				}
				else
				{
					// レビュー中ではないのでメンテナンス状態判定開始
					this.ChangeAccess(AccessType.CheckGameMaintenance);
				}
			}
			else
			{
				this.Error = string.Format("AccessState={0} NULL::GetReviewInformationResultJson", this.AccessState);
				this.ChangeAccess(AccessType.Finish);
			}
		}
		#endregion

		#region ゲームメンテナンス状態判定
		/// <summary>
		/// ゲームメンテナンス状態判定
		/// </summary>
		private void CheckGameMaintenance()
		{
			if (!this.CanUpdate) { return; }
			// 送信
			this.AccessStateLog = this.AccessState;
			GUIDebugLog.AddMessage("WebAPI Send CheckGameMaintenance");
			this.WebAPI.CheckGameMaintenance(this.Response);
		}

		/// <summary>
		/// ゲームメンテナンス状態判定レスポンス
		/// </summary>
		private void Response(AsobimoWebAPI.WebAPIRequest<AsobimoWebAPI.CheckGameMaintenanceURLResultJson> resultJson)
		{
			if (!this.CanUpdate) { return; }

			GUIDebugLog.AddMessage("WebAPI Response CheckGameMaintenance");

			// HTTPステータス更新
			this.Model.HttpStatus = resultJson.HttpStatus;

			// エラーチェック
			if (resultJson == null)
			{
				this.Error = string.Format("AccessState={0} NULL::WebAPIRequest", this.AccessState);
				this.ChangeAccess(AccessType.Finish);
			}
			else if (resultJson.HasError)
			{
				this.Error = resultJson.Error;
				this.ChangeAccess(AccessType.Finish);
			}
			else
			{
				this.CheckGameMaintenanceURLResult(resultJson.Result);
			}
		}

		/// <summary>
		/// ゲームメンテナンス状態判定結果
		/// </summary>
		private void CheckGameMaintenanceURLResult(AsobimoWebAPI.CheckGameMaintenanceURLResultJson resultInfo)
		{
			if (resultInfo != null)
			{
				// データ更新
				this.Model.IsGameMaintenance = resultInfo.IsGameMaintenance;

				if (resultInfo.IsGameMaintenance)
				{
					// メンテナンス中なのでアクセス終了
					this.ChangeAccess(AccessType.Finish);
				}
				else
				{
					// 初回ブラウザ起動可否判定開始
					this.ChangeAccess(AccessType.CheckOpenBrowser);
				}
			}
			else
			{
				this.Error = string.Format("AccessState={0} NULL::CheckGameMaintenanceURLResultJson", this.AccessState);
				this.ChangeAccess(AccessType.Finish);
			}
		}
		#endregion

		#region 初回ブラウザ起動可否判定
		private void CheckOpenBrowser()
		{
			if (!this.CanUpdate) { return; }
			// 送信
			this.AccessStateLog = this.AccessState;
			GUIDebugLog.AddMessage("WebAPI Send CheckOpenBrowser");
			this.WebAPI.CheckOpenBrowser(this.Response);
		}

		/// <summary>
		/// 初回ブラウザ起動可否判定レスポンス
		/// </summary>
		private void Response(AsobimoWebAPI.WebAPIRequest<AsobimoWebAPI.CheckOpenBrowserResultJson> resultJson)
		{
			if (!this.CanUpdate) { return; }

			GUIDebugLog.AddMessage("WebAPI Response CheckOpenBrowser");

			// HTTPステータス更新
			this.Model.HttpStatus = resultJson.HttpStatus;

 			// エラーチェック
			if (resultJson == null)
			{
				this.Error = string.Format("WebAPI AccessState={0} NULL::WebAPIRequest AsobimoID={1} Token={2}", this.AccessState, this.WebAPI.AsobimoId, this.WebAPI.AsobimoToken);
				this.ChangeAccess(AccessType.Finish);
			}
			else if (resultJson.HasError)
			{
				this.Error = resultJson.Error;
				this.ChangeAccess(AccessType.Finish);
			}
			else
			{
				this.CheckOpenBrowserResult(resultJson.Result);
			}
		}

		/// <summary>
		/// 初回ブラウザ起動可否判定結果
		/// </summary>
		private void CheckOpenBrowserResult(AsobimoWebAPI.CheckOpenBrowserResultJson resultInfo)
		{
			if(resultInfo != null)
			{
				if(resultInfo.IsOpenBrowser)
				{
					// 初回ブラウザ起動フラグがtrueになることはない trueになっていたらログだけ出力しておく
					string warning = string.Format("WebAPI CheckOpenBrowser IsOpenBrowser::{0}", resultInfo.IsOpenBrowser);
					GUIDebugLog.AddMessage(warning);
					BugReportController.SaveLogFileWithOutStackTrace(warning);
#if UNITY_EDITOR
					UnityEngine.Debug.LogWarning(warning);
#endif
				}

				// タイトル表示可否判定開始
				this.ChangeAccess(AccessType.CheckDisplayTitle);
			}
			else
			{
				this.Error = string.Format("AccessState={0} NULL::CheckOpenBrowserResultJson", this.AccessState);
				this.ChangeAccess(AccessType.Finish);
			}
		}
		#endregion

		#region タイトル表示可否判定
		/// <summary>
		/// タイトル表示可否判定
		/// </summary>
		private void CheckDisplayTitle()
		{
			if (!this.CanUpdate) { return; }
			// 送信
			this.AccessStateLog = this.AccessState;
			GUIDebugLog.AddMessage("WebAPI Send CheckDisplayTitle");
			this.WebAPI.CheckDisplayTitle(this.Response);
		}

		/// <summary>
		/// タイトル表示可否判定レスポンス
		/// </summary>
		private void Response(AsobimoWebAPI.WebAPIRequest<AsobimoWebAPI.CheckDisplayTitleResultJson> resultJson)
		{
			if (!this.CanUpdate) { return; }

			GUIDebugLog.AddMessage("WebAPI Response CheckDisplayTitle");

			// HTTPステータス更新
			this.Model.HttpStatus = resultJson.HttpStatus;

			// エラーチェック
			if (resultJson == null)
			{
				this.Error = string.Format("AccessState={0} NULL::WebAPIRequest", this.AccessState);
				this.ChangeAccess(AccessType.Finish);
			}
			else if (resultJson.HasError)
			{
				this.Error = resultJson.Error;
				this.ChangeAccess(AccessType.Finish);
			}
			else
			{
				this.CheckDisplayTitleResult(resultJson.Result);
			}
		}

		/// <summary>
		/// タイトル表示可否判定結果
		/// </summary>
		private void CheckDisplayTitleResult(AsobimoWebAPI.CheckDisplayTitleResultJson resultInfo)
		{
			if(resultInfo != null)
			{
				// 情報更新
				this.Model.IsDisplayTitle = resultInfo.IsDisplayTitle;

				if(!resultInfo.IsDisplayTitle)
				{
					// タイトル表示不可(Web側の初期化に失敗している)
					this.Error = string.Format("CheckDisplayTitle Not DisplayTitle::{0}", resultInfo.IsDisplayTitle);
				}
			}
			else
			{
				this.Error = string.Format("AccessState={0} NULL::CheckOpenBrowserResultJson", this.AccessState);
			}

			// アクセス終了
			this.ChangeAccess(AccessType.Finish);
		}
		#endregion

		#region 審査チェックユーザ登録
		/// <summary>
		/// 審査チェックユーザ登録
		/// </summary>
		private void RegisterReviewUser()
		{
			if (!this.CanUpdate) { return; }
			// 送信
			this.AccessStateLog = this.AccessState;
			GUIDebugLog.AddMessage("WebAPI Send RegisterReviewUser");
			this.WebAPI.RegisterReviewUser(this.Model.ReviewVersion, this.Response);
		}

		/// <summary>
		/// 審査チェックユーザ登録レスポンス
		/// </summary>
		private void Response(AsobimoWebAPI.WebAPIRequest<AsobimoWebAPI.RegisterReviewUserResultJson> resultJson)
		{
			if (!this.CanUpdate) { return; }

			GUIDebugLog.AddMessage("WebAPI Response RegisterReviewUser");

			// HTTPステータス更新
			this.Model.HttpStatus = resultJson.HttpStatus;

			// エラーチェック
			if (resultJson == null)
			{
				this.Error = string.Format("AccessState={0} NULL::WebAPIRequest", this.AccessState);
				this.ChangeAccess(AccessType.Finish);
			}
			else if (resultJson.HasError)
			{
				this.Error = resultJson.Error;
				this.ChangeAccess(AccessType.Finish);
			}
			else
			{
				this.RegisterReviewUserResult(resultJson.Result);
			}
		}

		/// <summary>
		/// 審査チェックユーザ登録結果
		/// </summary>
		private void RegisterReviewUserResult(AsobimoWebAPI.RegisterReviewUserResultJson resultInfo)
		{
			if(resultInfo != null)
			{
				// 情報更新
				this.Model.ReviewUserResult = resultInfo.Result;

				if(!resultInfo.Result)
				{
					// 失敗
					this.Error = string.Format("RegisterReviewUser Failure::{0}", resultInfo.Result);
				}
			}
			else
			{
				this.Error = string.Format("AccessState={0} NULL::RegisterReviewUserResultJson", this.AccessState);
			}

			// アクセス終了
			this.ChangeAccess(AccessType.Finish);
		}
		#endregion

		#region アクセス終了
		/// <summary>
		/// 終了時
		/// </summary>
		private void Finished()
		{
			if (!this.CanUpdate) { return; }

			if (!this.IsResult)
			{
				// 失敗時はエラーログ出力
				string error = string.Format("WebAPI Failure ErrorLog {0}\n AsobimoID={1} Token={2}", this.Error, this.WebAPI.AsobimoId, this.WebAPI.AsobimoToken);
				GUIDebugLog.AddMessage(error);
				BugReportController.SaveLogFileWithOutStackTrace(error);
#if UNITY_EDITOR
				UnityEngine.Debug.LogWarning(error);
#endif
			}
			else
			{
				this.AccessStateLog = this.AccessState;
				GUIDebugLog.AddMessage("WebAPI Success Access");
			}

			GUIDebugLog.AddMessage("WebAPI EndAccess");

			// 通知
			if (this.OnFinishEvent != null)
			{
				var model = this.Model;
				var eventArgs = new FinishEventArgs(this.IsResult, model.HttpStatus, model.IsUnderReview, model.ReviewVersion, model.IsGameMaintenance, model.IsDisplayTitle, model.ReviewUserResult);
				this.OnFinishEvent(this, eventArgs);
			}

			// 状態を設定なしに
			this.ChangeAccess(AccessType.None);
		}
		#endregion
	}
}