/// <summary>
/// WebViewデータ
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
		/// Webモデルインターフェイス
		/// </summary>
		public interface IModel
		{
			/// <summary>
			/// WebViewを実際に表示するためのWidget
			/// </summary>
			UIWidget TargetWidget { get; }

			/// <summary>
			/// 公式ページURL
			/// </summary>
			string HomeURL { get; }

			/// <summary>
			/// お知らせページURL
			/// </summary>
			string NewsURL { get; }

			/// <summary>
			/// 公式ツイッターURL
			/// </summary>
			string TwitterURL { get; }

			/// <summary>
			/// ヘルプページURL
			/// </summary>
			string HelpURL { get; }

			/// <summary>
			/// アンケートページURL
			/// </summary>
			string EnqueteURL { get; }

			/// <summary>
			/// アンケートのポスト送信データ
			/// </summary>
			string EnquetePostData { get; }

			byte[] EnquetePostByteData { get; }

			/// <summary>
			/// GoOneページURL
			/// </summary>
			string GoOneURL { get; }

			/// <summary>
			/// 規約ページURL
			/// </summary>
			string TermsURL { get; }

			/// <summary>
			/// Webを実際に表示している短形
			/// </summary>
			Rect WebRect { get; set; }
		}

		/// <summary>
		/// Webモデル
		/// </summary>
		[System.Serializable]
		public class Model : IModel
		{
			#region フィールド&プロパティ
			/// <summary>
			/// WebViewを実際に表示するためのWidget
			/// </summary>
			[SerializeField]
			private UIWidget targetWidget = null;
			public UIWidget TargetWidget { get { return targetWidget; } }

			/// <summary>
			/// 公式ページURL
			/// </summary>
			//public string HomeURL { get { return ObsolateSrc.HomeURL; } }
			public string HomeURL { get { return ""; } }

			/// <summary>
			/// お知らせページURL
			/// </summary>
			//public string NewsURL { get { return ObsolateSrc.NewsURL; } }
			public string NewsURL { get { return ""; } }

			/// <summary>
			/// 公式ツイッターURL
			/// </summary>
			public string TwitterURL { get { return ObsolateSrc.TwitterURL; } }

			/// <summary>
			/// ヘルプページURL
			/// </summary>
			public string HelpURL { get { return ObsolateSrc.HelpURL; } }

			/// <summary>
			/// アンケートページURL
			/// </summary>
			//public string EnqueteURL { get { return ObsolateSrc.EnqueteURL; } }
			public string EnqueteURL { get { return ""; } }

			/// <summary>
			/// アンケートのポスト送信データ
			/// </summary>
			public string EnquetePostData
			{
				get
				{
					string id = string.Empty;
					string token = string.Empty;
					string platform = string.Empty;
					string distribution = string.Empty;
#if UNITY_ANDROID
					id = PluginController.AuthInfo.openID;
					token = PluginController.AuthInfo.token;
					platform = "asobimo";
					distribution = "asobimo";
#endif
					string data = string.Format("asobimo_id={0}&asobimo_token={1}&platform_code={2}&distribution_code={3}&redirect_page=/shop_enquete/?id=1",
										id, token, platform, distribution);

					return data;
				}
			}

			public byte[] EnquetePostByteData
			{
				get
				{
					WWWForm postData = new WWWForm();

					postData.AddField("asobimo_id", PluginController.AuthInfo.openID);
					postData.AddField("asobimo_token", PluginController.AuthInfo.token);
					postData.AddField("platform_code", "asobimo");
					postData.AddField("distribution_code", "asobimo");
					postData.AddField("redirect_page", "/shop_enquete/?id=1");

					return postData.data;
				}
			}


			/// <summary>
			/// GoOneページURL
			/// </summary>
			public string GoOneURL { get { return ObsolateSrc.GoOneURL; } }

			/// <summary>
			/// 規約ページURL
			/// </summary>
			//public string TermsURL { get { return ObsolateSrc.TermsURL; } }
			public string TermsURL { get { return ""; } }

			/// <summary>
			/// Webを実際に表示している短形
			/// </summary>
			public Rect WebRect { get; set; }
			#endregion
		}
	}
}