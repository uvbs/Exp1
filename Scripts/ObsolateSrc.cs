/// <summary>
/// いずれ消す一時仕様のためのソース
/// 
/// 2014/11/28
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.GameParameter;

/// <summary>
/// スタイルタイプ
/// </summary>
public enum StyleType
{
	None,
	Striker,
	Guardian,
	Shooter,
	Hunter,
	Extra,

	Max,
}

/// <summary>
/// スタイル情報
/// </summary>
[System.Serializable]
public class StyleInfo
{
	public StyleType type;
	public string name;
	public string iconName;
}

public class ObsolateSrc
{
	#region テキストマスター化するもの
	/* メンバー誘致が未実装のため使用していない
	public const string TeamAddMember_MemberOver = TeamJoinRes_MemberOver;
	public const string TeamAddMember_NoAuthority = "メンバーを誘うのは\r\nチームリーダーしか行えません。";
	public const string TeamAddMember_TeamNotExist = TeamJoinRes_TeamNotExist;
	*/

	#region リザルトシーン関係
	public const string PointUnitName = "pt";
	public const string KillUnitName = "人";
	public const string BreakUnitName = "基";
	public const string CompleteWinName = "完勝";
	public const string CompleteLoseName = "完敗";
	public const string WinName = "勝利";
	public const string LoseName = "敗北";
	public const string DrawName = "引分";
	public const string GradeName = "Grade";
	public const string GradeUnitName = "GXP";
	#endregion

	public static class GUIAuthComment
	{
		public const string WebAPIRequestPlatformAndroid = "android";
		public const string WebAPIRequestPlatformiPhone = "";
	}

	public static class GUILobbySelectComment
	{
		public const string NowContentFormat = "現在のロビー No.{0:000}";
		public const string ContentFormat = "ロビー No.{0:000}\r\n{1:0} / {2:0}";
	}
	#endregion

	#region デフォルトテキスト
	// マスターデータ読み込み前にテキストマスターが必要なためクライアント側でデフォルトのテキストを用意しておく
	public static string Defalut_TX147_Auth_Progress
	{
		get
		{
			switch (Scm.Common.Utility.Language)
			{
			case Language.Japanese:				return "認証中...";
			case Language.ChineseTraditional:	return "認證中...";
			case Language.ChineseSimplified:	return "认证中...";
			default:							return "Authenticating...";
			}
		}
	}
	public static string Defalut_TX148_Auth_Error
	{
		get
		{
			switch (Scm.Common.Utility.Language)
			{
			case Language.Japanese:				return "認証に失敗しました";
			case Language.ChineseTraditional:	return "認證失敗";
			case Language.ChineseSimplified:	return "认证失败";
			default:							return "Authentication failed";
			}
		}
	}
	public static string Defalut_TX149_Web_AuthError
	{
		get
		{
			switch (Scm.Common.Utility.Language)
			{
			case Language.Japanese:				return "認証に失敗しました";
			case Language.ChineseTraditional:	return "認證失敗";
			case Language.ChineseSimplified:	return "认证失败";
			default:							return "Authentication failed";
			}
		}
	}
	public static string Defalut_TX150_Download_Progress
	{
		get
		{
			switch (Scm.Common.Utility.Language)
			{
			case Language.Japanese:				return "ダウンロード...";
			case Language.ChineseTraditional:	return "下載中...";
			case Language.ChineseSimplified:	return "下载中...";
			default:							return "Downloading...";
			}
		}
	}
	public static string Defalut_TX151_Download_Error
	{
		get
		{
			switch (Scm.Common.Utility.Language)
			{
			case Language.Japanese:				return "データの取得に失敗しました";
			case Language.ChineseTraditional:	return "無法取得數據。";
			case Language.ChineseSimplified:	return "无法取得数据。";
			default:							return "Failed to acquire data.";
			}
		}
	}
	public static string Defalut_TX153_SystemLanguage_NotJapanese
	{
		get
		{
			return "This Application can\r\nonly be played in Japan.";
		}
	}
	public static string Defalut_TX155_Error_ScreenTitle
	{
		get
		{
			switch (Scm.Common.Utility.Language)
			{
			case Language.Japanese:				return "エラー";
			case Language.ChineseTraditional:	return "錯誤";
			case Language.ChineseSimplified:	return "错误";
			default:							return "Error";
			}
		}
	}
	public static string Defalut_TX156_Error
	{
		get
		{
			switch (Scm.Common.Utility.Language)
			{
			case Language.Japanese:				return "エラーが発生しました";
			case Language.ChineseTraditional:	return "發生錯誤。";
			case Language.ChineseSimplified:	return "发生错误。";
			default:							return "An error has occurred.";
			}
		}
	}
	public static string Defalut_TX158_Common_QuitButton
	{
		get
		{
			switch (Scm.Common.Utility.Language)
			{
			case Language.Japanese:				return "終了";
			case Language.ChineseTraditional:	return "結束";
			case Language.ChineseSimplified:	return "结束";
			default:							return "Quit";
			}
		}
	}
	public static string Defalut_TX159_Common_RetryButton
	{
		get
		{
			switch (Scm.Common.Utility.Language)
			{
			case Language.Japanese:				return "リトライ";
			case Language.ChineseTraditional:	return "重試";
			case Language.ChineseSimplified:	return "重试";
			default:							return "Retry";
			}
		}
	}

    public static string Default_TX057_Common_YesButton {
        get {
            switch (Scm.Common.Utility.Language) {
            case Language.Japanese:             return "はい";
            case Language.ChineseTraditional:   return "是";
            case Language.ChineseSimplified:    return "是";
            default:                            return "Yes";
            }
        }
    }

    public static string Default_TX058_Common_YesButton {
        get {
            switch (Scm.Common.Utility.Language) {
            case Language.Japanese:             return "いいえ";
            case Language.ChineseTraditional:   return "否";
            case Language.ChineseSimplified:    return "否";
            default:                            return "No";
            }
        }
    }

    public static string Default_TX606_NetworkFeeWarningTitle {
        get {
            switch (Scm.Common.Utility.Language) {
            case Language.Japanese:             return "ダウンロード確認";
            case Language.ChineseTraditional:   return "确认下载";
            case Language.ChineseSimplified:    return "确认下载";
            default:                            return "Confirm";
            }
        }
    }

    public static string Default_TX606_NetworkFeeWarning {
        get {
            switch (Scm.Common.Utility.Language) {
            case Language.Japanese:             return "リソースファイルをダウンロードする必要があります。ネットワークプロバイダーによる費用が必要になります。続きますか。";
            case Language.ChineseTraditional: return "即将下载资源文件。请确保在wifi环境下载。是否下载？ ";
            case Language.ChineseSimplified: return "即将下载资源文件。请确保在wifi环境下载。是否下载？ ";
            default:                            return "About to download resource files, this may lead fees according to your network provider. Continue?";
            }
        }
    }

    #endregion

    #region チームコメント
    public static class TeamComment
	{
		static readonly TextType[] TeamCommentArray =
		{
			TextType.TX106_TeamComment0, TextType.TX107_TeamComment1, TextType.TX108_TeamComment2, TextType.TX109_TeamComment3, TextType.TX110_TeamComment4,
			TextType.TX111_TeamComment5, TextType.TX112_TeamComment6, TextType.TX113_TeamComment7
		};
		
		public static int TeamCommentLength { get { return TeamCommentArray.Length; } }
		
		static public string GetTeamComment(byte id)
		{
			return (id < TeamCommentArray.Length) ? MasterData.GetText(TeamCommentArray [id]) : string.Empty;
	}
	}
    #endregion

    #region Host
    //    internal const string OpenHost = "xworld-game.xiaoyougame.com:30500";          // 公開サーバー
    //    internal const string OpenHost = "192.168.1.170:30500";          // 公開サーバー
    //internal const string OpenHost = "192.168.1.227:30500";          // 公開サーバー
    internal const string BetaGameServerHost = "xworld-game.xiaoyougame.com:30500";          // 公開サーバー
    internal const string TestGameServerHost = "xworld-game-test.xiaoyougame.com:30500";          // 公開サーバー
    internal const string PersonalGameServerHost = "127.0.0.1:30500";          // 公開サーバー

#endregion

    #region URL
    #region マスターデータ

#if TEST_CDN_SERVER
    internal const string CDN_SERVER = "http://xworld.rc-test.xiaoyougame.com";
#else
    internal const string CDN_SERVER = "http://xworld.rc.xiaoyougame.com";
#endif

    // 公開サーバー
#if UNITY_EDITOR
    internal const string MasterDataURL = CDN_SERVER + "/test/Windows/Master/";
#elif UNITY_ANDROID
	internal const string MasterDataURL = CDN_SERVER + "/test/Android/Master/";
#elif UNITY_IOS
	internal const string MasterDataURL = CDN_SERVER + "/test/iOS/Master/";
#else
	internal const string MasterDataURL = CDN_SERVER + "/test/Windows/Master/";
#endif

    #endregion

    #region アセットバンドル
    // 公開サーバー
#if UNITY_EDITOR
    internal const string AssetBundleURL = CDN_SERVER + "/test/Windows/AssetBundle/";
#elif UNITY_ANDROID
	internal const string AssetBundleURL = CDN_SERVER + "/test/Android/AssetBundle/";
#elif UNITY_IOS
	internal const string AssetBundleURL = CDN_SERVER + "/test/iOS/AssetBundle/";
#else
	internal const string AssetBundleURL = CDN_SERVER + "/test/Windows/AssetBundle/";
#endif

#endregion

#region BugReport
    public const string BugReportURL = "http://59.106.106.131:30300/BugReport/log.php";
#endregion
#endregion

#region WebView関係
	//public const string HomeURL = "https://www.xworld.jp/landing/";

	public const string MailScheme = "mailto:";
	public const string To = "test@test.com";
	public const string TwitterURL = "https://twitter.com/Xworld_asobimo";
	public const string HelpURL = "file:///android_asset/play_manual/play_manual1.html";
	public const string GoOneURL = "http://www.go-one.net/";

	//public const string TermsURL = "http://test.xworld.jp/outline/rule/";
	//#if XW_DEBUG
	//public const string NewsURL = "http://test.xworld.jp/information/app/";
	//public const string EnqueteURL = "http://test.xworld.jp/shop/login/";
	//#else
	//public const string NewsURL = "https://xworld.jp/information/app/";
	//public const string EnqueteURL = "https://xworld.jp/shop/login/";
	//#endif
#endregion

#region スタイル関連
	/// <summary>
	/// AvatarTypeとスタイルの関連データ
	/// </summary>
	public readonly static Dictionary<AvatarType, StyleType> AvatarStyleDict = new Dictionary<AvatarType, StyleType>()
	{
		{ AvatarType.P003_Kuroriku, StyleType.Shooter },
		{ AvatarType.P005_Kufun, StyleType.Striker },
		{ AvatarType.P006_Shirayuki, StyleType.Striker },
		{ AvatarType.P007_Rerikus, StyleType.Hunter },
		{ AvatarType.P008_Airin, StyleType.Extra},
		{ AvatarType.P009_Kazakiri, StyleType.Hunter },
		{ AvatarType.P010_Eria, StyleType.Extra },
		{ AvatarType.P011_Meru, StyleType.Hunter },
		{ AvatarType.P012_Azurael, StyleType.Guardian },
		{ AvatarType.P013_Mineruva, StyleType.Shooter },
		{ AvatarType.P014_Sasha, StyleType.Striker },
		{ AvatarType.P015_Horus, StyleType.Shooter },
		{ AvatarType.P016_Torfin, StyleType.Extra },
		{ AvatarType.P017_Deltoa, StyleType.Shooter },
		{ AvatarType.P018_Youko, StyleType.Extra },
		{ AvatarType.P019_Cylinder, StyleType.Guardian },
		{ AvatarType.P020_Yuuka, StyleType.Extra },
		{ AvatarType.P1001_Galatia_ST, StyleType.Striker },
		{ AvatarType.P1002_Galatia_SH, StyleType.Shooter },
		{ AvatarType.P1003_Galatia_HT, StyleType.Hunter },
		{ AvatarType.P1004_Giruberuto_SH, StyleType.Shooter },
		{ AvatarType.P1005_Giruberuto_HT, StyleType.Hunter },
		{ AvatarType.P1006_Giruberuto_ST, StyleType.Striker },
		{ AvatarType.P1007_Raiback_GD, StyleType.Guardian },
		{ AvatarType.P1008_Raiback_EX, StyleType.Extra },
		{ AvatarType.P1009_Raiback_SH, StyleType.Shooter },
	};

	/// <summary>
	/// スタイルとスタイル情報の関連データ
	/// </summary>
	public readonly static Dictionary<StyleType, StyleInfo> StyleInfoDict = new Dictionary<StyleType, StyleInfo>()
	{
		{
			StyleType.Striker, new StyleInfo() {
				type = StyleType.Striker,
				name = "ストライカー",
				iconName = "ui_StyleIcon_ST_01",
			}
		},
		{
			StyleType.Guardian, new StyleInfo() {
				type = StyleType.Guardian,
				name = "ガーディアン",
				iconName = "ui_StyleIcon_GD_01",
			}
		},
		{
			StyleType.Shooter, new StyleInfo() {
				type = StyleType.Shooter,
				name = "シューター",
				iconName = "ui_StyleIcon_SH_01",
			}
		},
		{
			StyleType.Hunter, new StyleInfo() {
				type = StyleType.Hunter,
				name = "ハンター",
				iconName = "ui_StyleIcon_HT_01",
			}
		},
		{
			StyleType.Extra, new StyleInfo() {
				type = StyleType.Extra,
				name = "エクストラ",
				iconName = "ui_StyleIcon_EX_01",
			}
		},
	};

	/// <summary>
	/// AvatarType からスタイル情報を取得する
	/// </summary>
	public static bool TryGetStyleInfo(AvatarType avatarType, out StyleInfo info)
	{
		info = null;
		StyleType type;
		if (AvatarStyleDict.TryGetValue(avatarType, out type))
		{
			if (StyleInfoDict.TryGetValue(type, out info))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// StyleType からスタイル情報を取得する
	/// </summary>
	public static bool TryGetStyleInfo(StyleType styleType, out StyleInfo info)
	{
		info = null;
		if (StyleInfoDict.TryGetValue(styleType, out info))
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// AvatarType から 元になったキャラクターのAvatarTypeを取得する.
	/// </summary>
	public static AvatarType GetBaseAvatarType(AvatarType avatarType)
	{
		AvatarType ret = avatarType;
		switch (avatarType)
		{
		case AvatarType.P1001_Galatia_ST:
		case AvatarType.P1002_Galatia_SH:
		case AvatarType.P1003_Galatia_HT:
			ret = AvatarType.P001_Galatia;
			break;
		case AvatarType.P1004_Giruberuto_SH:
		case AvatarType.P1005_Giruberuto_HT:
		case AvatarType.P1006_Giruberuto_ST:
			ret = AvatarType.P002_Giruberuto;
			break;
		case AvatarType.P1007_Raiback_GD:
		case AvatarType.P1008_Raiback_EX:
		case AvatarType.P1009_Raiback_SH:
			ret = AvatarType.P004_Raiback;
			break;
		}
		return ret;
	}
#endregion

#region コンフィグ
	public static class Option
	{
		public const string ChatPopupNumDesc = "ポップアップウィンドウの数";
		public const int ChatPopupNumDefault = 3;
		public static readonly Dictionary<int, string> ChatPopupNumDict = new Dictionary<int, string>()
		{
			{ 0, "非表示" },
			{ 1, "1 個" },
			{ 2, "2 個" },
			{ 3, "3 個" },
			{ 4, "4 個" },
			{ 5, "5 個" },
			{ 6, "6 個" },
			{ 7, "7 個" },
		};

		public const string ChatPopupTimerDesc = "ポップアップウィンドウ表示時間";
		public const string ChatPopupTimerFormat = "{0:0.0} 秒";
		public const int ChatPopupTimerDefault = 80;
		public const int ChatPopupTimerMin = 5;
		public const int ChatPopupTimerMax = 160;
		public const int ChatPopupTimerSteps = 0;

		//public const string BgmDesc = "BGM";
		public const string BgmFormat = "{0:0.00}";
		public const int BgmDefault = 100;
		public const int BgmMin = 0;
		public const int BgmMax = 100;
		public const int BgmSteps = 21;

		//public const string SeDesc = "SE";
		public const string SeFormat = "{0:0.00}";
		public const int SeDefault = 100;
		public const int SeMin = 0;
		public const int SeMax = 100;
		public const int SeSteps = 21;

		//public const string VoiceDesc = "Voice";
		public const string VoiceFormat = "{0:0.00}";
		public const int VoiceDefault = 100;
		public const int VoiceMin = 0;
		public const int VoiceMax = 100;
		public const int VoiceSteps = 21;

		public const string MacroCloseDesc = "マクロ発言時にマクロを閉じる";
		public const bool MacroCloseDefault = true;

		public const string LockonRangeDesc = "ロックオン範囲";
		public const int LockonRangeDefault = 50;
		public static readonly Dictionary<int, string> LockonRangeDict = new Dictionary<int, string>()
		{
			{ 25,"25m" },
			{ 50,"50m" },
			{ 100,"100m" },
			{ 99999,"無限" },
		};

		public const string MacroButtonColumnDesc = "マクロボタンの1行の表示数";
		public const int MacroButtonColumnDefault = 4;
		public static readonly Dictionary<int, string> MacroButtonColumnDict = new Dictionary<int, string>()
		{
			{ 1, "1 個" },
			{ 2, "2 個" },
			{ 3, "3 個" },
			{ 4, "4 個" },
		};

		public const string MacroButtonDescFormat = "マクロボタン名({0})";
		public const string MacroButtonEmpty = "";
		public const UIInput.KeyboardType MacroButtonType = UIInput.KeyboardType.Default;
		public const int MacroButtonLength = 32;
		//public const string MacroDescFormat = "マクロ内容({0})";
		public const string MacroEmpty = "";
		public const UIInput.KeyboardType MacroType = UIInput.KeyboardType.Default;
		public const int MacroLength = 32;
		//public static readonly List<ChatMacroInfo> ChatMacroDefaultList = new List<ChatMacroInfo>()
		//{
		//	new ChatMacroInfo("よろしく", "よろしくお願いします！", 0),
		//	new ChatMacroInfo("ありがとう", "ありがとう！", 1),
		//	new ChatMacroInfo("ごめん", "ごめんなさい。", 2),
		//	new ChatMacroInfo("おつかれ", "おつかれさま。", 3),
		//	new ChatMacroInfo("GJ！", "Good Job！", 4),
		//	new ChatMacroInfo("", "", 5),
		//	new ChatMacroInfo("", "", 6),
		//	new ChatMacroInfo("", "", 7),
		//};
	}
#endregion

	static public class ModelSize
	{
		static Dictionary<int, float> modelSizeDic;
		static ModelSize()
		{
			modelSizeDic = new Dictionary<int, float>();

			modelSizeDic.Add(4, 1.5f);		// ライバック.
			modelSizeDic.Add(1007, 1.5f);
			modelSizeDic.Add(1008, 1.5f);
			modelSizeDic.Add(1009, 1.5f);

			modelSizeDic.Add(7, 1.4f);
			modelSizeDic.Add(1012, 1.4f);	// レリクス.

			modelSizeDic.Add(9, 1.3f);
			modelSizeDic.Add(1014, 1.3f);	// カザキリ.

			modelSizeDic.Add(10, 0.8f);
			modelSizeDic.Add(1015, 0.8f);	// えりあ.

			modelSizeDic.Add(11, 0.85f);
			modelSizeDic.Add(12, 1.3f);
			modelSizeDic.Add(15, 1.4f);
			modelSizeDic.Add(17, 1.2f);
			modelSizeDic.Add(18, 0.95f);
			modelSizeDic.Add(20, 1.05f);
			modelSizeDic.Add(21, 1.05f);
			modelSizeDic.Add(22, 1.18f);

			modelSizeDic.Add(501, 0.95f);
			modelSizeDic.Add(502, 1.03f);
			modelSizeDic.Add(503, 0.935f);
		}
		static public float GetModelSize(int avaterId)
		{
			float size;
			if(modelSizeDic.TryGetValue(avaterId, out size))
			{
				return size;
			}
			return 1;
		}
	}

	static public class SkillUsableLevel
	{
		const int Normal = 1;
		const int Skill1 = 4;
		const int Skill2 = 6;
		const int SpecialSkill = 8;
		const int TechnicalSkill = 1;

		static public bool IsSkillUsable(SkillButtonType type, int battleLevel)
		{
			int usableLevel = 0;
			/* 神域封剣試遊用
			switch (type)
			{
			case SkillButtonType.Normal:
				usableLevel = Normal;
				break;
			case SkillButtonType.Skill1:
				usableLevel = Skill1;
				break;
			case SkillButtonType.Skill2:
				usableLevel = Skill2;
				break;
			case SkillButtonType.SpecialSkill:
				usableLevel = SpecialSkill;
				break;
			case SkillButtonType.TechnicalSkill:
				usableLevel = TechnicalSkill;
				break;
			}
			*/
			return usableLevel <= battleLevel;
		}
	}
}
