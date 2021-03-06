/// <summary>
/// アソビモWebデータ
/// 
/// 2016/06/15
/// </summary>
using System;

namespace XUI.AsobimoWeb
{
	/// <summary>
	/// アソビモWebデータインターフェイス
	/// </summary>
	public interface IModel
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();
		#endregion

		#region 審査情報
		/// <summary>
		/// 審査中フラグの変更イベント
		/// </summary>
		event EventHandler OnUnderReviewChange;
		/// <summary>
		/// 審査中か
		/// </summary>
		bool IsUnderReview { get; set; }

		/// <summary>
		/// 審査バージョン変更イベント
		/// </summary>
		event EventHandler OnReviewVersionChange;
		/// <summary>
		/// 審査バージョン
		/// </summary>
		string ReviewVersion { get; set; }
		#endregion
		
		#region ゲームメンテナンス
		/// <summary>
		/// ゲームメンテナンスフラグ変更イベント
		/// </summary>
		event EventHandler OnIsGameMaintenanceChange;
		/// <summary>
		/// ゲームメンテナンスフラグ
		/// </summary>
		bool IsGameMaintenance { get; set; }
		#endregion

		#region タイトル表示可否
		/// <summary>
		/// タイトル表示可能フラグの変更イベント
		/// </summary>
		event EventHandler OnIsDisplayTitleChange;
		/// <summary>
		/// タイトル表示可能か
		/// </summary>
		bool IsDisplayTitle { get; set; }
		#endregion

		#region 審査チェックユーザ登録
		/// <summary>
		/// 審査チェックユーザ登録成功フラグの変更イベント
		/// </summary>
		event EventHandler OnReviewUserResultChange;
		/// <summary>
		/// 審査チェックユーザ登録成功フラグ
		/// </summary>
		bool ReviewUserResult { get; set; }
		#endregion

		#region HTTPステータス
		/// <summary>
		/// HTTPステータス変更イベント
		/// </summary>
		event EventHandler OnHttpStatusChange;
		/// <summary>
		/// HTTPステータス
		/// </summary>
		int HttpStatus { get; set; }
		#endregion
	}

	/// <summary>
	/// アソビモWebデータ
	/// </summary>
	public class Model : IModel
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			this.OnUnderReviewChange = null;
			this.OnReviewVersionChange = null;
			this.OnIsGameMaintenanceChange = null;
			this.OnIsDisplayTitleChange = null;
			this.OnReviewUserResultChange = null;
			this.OnHttpStatusChange = null;
		}
		#endregion

		#region 審査情報
		/// <summary>
		/// 審査中フラグの変更イベント
		/// </summary>
		public event EventHandler OnUnderReviewChange = (sender, e) => { };
		/// <summary>
		/// 審査中か
		/// </summary>
		private bool _isUnderReview = false;
		public bool IsUnderReview
		{
			get { return _isUnderReview; }
			set
			{
				if(_isUnderReview != value)
				{
					_isUnderReview = value;

					// 通知
					this.OnUnderReviewChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// 審査バージョン変更イベント
		/// </summary>
		public event EventHandler OnReviewVersionChange = (sender, e) => { };
		/// <summary>
		/// 審査バージョン
		/// </summary>
		private string _reviewVersion = string.Empty;
		public string ReviewVersion
		{
			get { return _reviewVersion; }
			set
			{
				if (_reviewVersion != value)
				{
					_reviewVersion = value;

					// 通知
					this.OnReviewVersionChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region ゲームメンテナンス
		/// <summary>
		/// ゲームメンテナンスフラグ変更イベント
		/// </summary>
		public event EventHandler OnIsGameMaintenanceChange = (sender, e) => { };
		/// <summary>
		/// ゲームメンテナンスフラグ
		/// </summary>
		private bool _isGameMaintenance = false;
		public bool IsGameMaintenance
		{
			get { return _isGameMaintenance; }
			set
			{
				if(_isGameMaintenance != value)
				{
					_isGameMaintenance = value;

					// 通知
					this.OnIsGameMaintenanceChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region タイトル表示可否
		/// <summary>
		/// タイトル表示可能フラグの変更イベント
		/// </summary>
		public event EventHandler OnIsDisplayTitleChange = (sender, e) => { };
		/// <summary>
		/// タイトル表示可能か
		/// </summary>
		private bool _isDisplayTitle = false;
		public bool IsDisplayTitle
		{
			get { return _isDisplayTitle; }
			set
			{
				if(_isDisplayTitle != value)
				{
					_isDisplayTitle = value;

					// 通知
					this.OnIsDisplayTitleChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region 審査チェックユーザ登録
		/// <summary>
		/// 審査チェックユーザ登録成功フラグの変更イベント
		/// </summary>
		public event EventHandler OnReviewUserResultChange = (sender, e) => { };
		/// <summary>
		/// 審査チェックユーザ登録成功フラグ
		/// </summary>
		private bool _reviewUserResult = false;
		public bool ReviewUserResult
		{
			get { return _reviewUserResult; }
			set
			{
				if(_reviewUserResult != value)
				{
					_reviewUserResult = value;

					// 通知
					this.OnReviewUserResultChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region HTTPステータス
		/// <summary>
		/// HTTPステータス変更イベント
		/// </summary>
		public event EventHandler OnHttpStatusChange = (sender, e) => { };
		/// <summary>
		/// HTTPステータス
		/// </summary>
		private int _httpStatus = -1;
		public int HttpStatus
		{
			get { return _httpStatus; }
			set
			{
				if(_httpStatus != value)
				{
					_httpStatus = value;

					// 通知
					this.OnHttpStatusChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion
	}
}
