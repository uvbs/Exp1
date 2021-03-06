/// <summary>
/// キャラクターボイス
/// 
/// 2013/08/15
/// </summary>
using UnityEngine;
using System.Collections.Generic;

public class CharacterVoice : MonoBehaviour
{
	#region 定義.
	const string CueSheetName_Character = "X_world_Voice_p{0:D3}";

	public const string CueName_damage_s	= "damage_s";
	public const string CueName_damage_m	= "damage_m";
	public const string CueName_damage_l	= "damage_l";
	public const string CueName_down		= "down";
	public const string CueName_dead		= "dead";

	public const string CueName_start			= "start";
	public const string CueName_win_complete	= "win_complete";
	public const string CueName_win				= "win";
	public const string CueName_draw			= "draw";
	public const string CueName_lose			= "lose";
	public const string CueName_lose_complete	= "lose_complete";
	#endregion

	#region 変数, プロパティ.
	/// <summary>
	/// Acb読み込み状態管理用辞書.
	/// </summary>
	static Dictionary<string, List<CharacterVoice>> voiceDic = new Dictionary<string, List<CharacterVoice>>();

	private CriAtomSource atomSource;
	public string CueSheetName { get; private set;}
	#endregion

	#region StaticMethod
	/// <summary>
	/// プレイヤーキャラクターのCharacterVoiceを作成する.
	/// </summary>
	static public CharacterVoice Create(GameObject go, AvatarType avatarType)
	{
		// CueSheetName作成.
		avatarType = ObsolateSrc.GetBaseAvatarType(avatarType);
		string cueSheetName = string.Format(CueSheetName_Character, (int)avatarType);

		// 登録.
		return Create(cueSheetName, go);
	}

	/// <summary>
	/// Characterクラス用のCharacterVoiceを作成する.
	/// </summary>
	static public CharacterVoice Create(Character character)
	{
		// 既にCharacterVoiceがある場合はそちらを使う.
		CharacterVoice voice = character.gameObject.GetComponent<CharacterVoice>();
		if(voice == null)
		{
			// Characterクラス用CueSheetName作成.
			AvatarType avatarType = ObsolateSrc.GetBaseAvatarType(character.AvatarType);
			string cueSheetName = string.Format(CueSheetName_Character, (int)avatarType);

			// 登録.
			voice = Create(cueSheetName, character.gameObject);
		}
		return voice;
	}
	
	/// <summary>
	/// gameObjectにCharacterVoiceを作成する.
	/// </summary>
	static private CharacterVoice Create(string cueSheetName, GameObject gameObject)
	{
		CriAtomCueSheet cueSheet = CriAtom.GetCueSheet(cueSheetName);
		if(cueSheet == null)
		{
			// cueSheetのロード.
			cueSheet = SoundController.AddAssetCueSheet(cueSheetName);
		}
		// CharacterVoice作成.
		CharacterVoice voice = gameObject.AddComponent<CharacterVoice>();
		voice.Setup(cueSheet);

		// Memo : Unity付属のプロファイラを見る限り,重複したキューシートを作った場合でもTotalSystemMemoryUsageが増えている.
		//        なので重複登録防止&必要なのに削除されるのを防ぐために管理.
		List<CharacterVoice> voiceList;
		if(voiceDic.TryGetValue(cueSheetName, out voiceList))
		{
			voiceList.Add(voice);
		}
		else
		{
			voiceList = new List<CharacterVoice>();
			voiceList.Add(voice);
			voiceDic.Add(cueSheetName, voiceList);
		}
		
		return voice;
	}
	static private void RemoveVoice(CharacterVoice voice)
	{
		List<CharacterVoice> voiceList;
		if(voiceDic.TryGetValue(voice.CueSheetName, out voiceList))
		{
			voiceList.Remove(voice);
			if(voiceList.Count == 0)
			{
				CriAtom.RemoveCueSheet(voice.CueSheetName);
			}
		}
	}
	#endregion

	#region 初期化
	private bool Setup(CriAtomCueSheet cueSheet)
	{
		if(cueSheet != null)
		{
			this.CueSheetName = cueSheet.name;
			if(cueSheet.acb != null)
			{
				// CriAtomSource作成
				atomSource = gameObject.AddComponent<CriAtomSource> ();
				atomSource.cueSheet = cueSheet.name;
				return true;
			}
		}

		// cueSheet.acbが存在しない.
		atomSource = null;
		return false;
	}
	#endregion
	
	#region 破棄
	void OnDestroy()
	{
		RemoveVoice(this);
	}

	#region 再生

	#endregion
	public void Play(string cueName)
	{
		if(atomSource != null)
		{
			// CriAtomExAcbは CriAtom.RemoveCueSheet を実行後に下手に触るとEditorレベルで落ちる！.
			// なので毎回確認する.
			CriAtomExAcb atomExAcb = CriAtom.GetAcb(atomSource.cueSheet);
			if(atomExAcb != null)
			{
				if(atomExAcb.Exists(cueName))
				{
					atomSource.Stop();
					atomSource.Play(cueName);
				}
			}
			else
			{
				BugReportController.SaveLogFile("not found Acb :" + atomSource.cueSheet);
			}
		}
	}
	#endregion
}
