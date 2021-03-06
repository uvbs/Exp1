/// <summary>
/// バトルで再生させるサウンドを管理/制御するクラス
/// 
/// 2015/04/07
/// </summary>
using System.Collections;

public class BattleSound
{
	#region BGM状態タイプ
	public enum BGMStateType
	{
		None,
		Free,
		Normal,
		Limit,
		BattleEnd,
	}
	#endregion

	#region フィールド&プロパティ
	/// <summary>
	/// 現在のBGM状態
	/// </summary>
	private BGMStateType bgmState = BGMStateType.None;
	#endregion

	#region BGM再生
	/// <summary>
	/// 戦闘待ち状態で再生させるBGM
	/// </summary>
	public void PlayFreeBgm()
	{
		ChangeBgm(BGMStateType.Free, SoundController.BgmID.Stage01_Free);
	}

	/// <summary>
	/// 戦闘中のBGM再生
	/// </summary>
	public void PlayNormalBgm()
	{
		ChangeBgm(BGMStateType.Normal, SoundController.BgmID.Stage01_Normal);
	}

	/// <summary>
	/// 戦闘修了間近に再生させるBGM
	/// </summary>
	public void PlayLimitBgm(SoundController.BgmID bgmId)
	{
		ChangeBgm(BGMStateType.Limit, bgmId);
	}

	/// <summary>
	/// 勝利時に再生させるBGM
	/// </summary>
	public void PlayWinBgm()
	{
		ChangeBgm(BGMStateType.BattleEnd, SoundController.BgmID.Victory);
	}

	/// <summary>
	/// 敗北時に再生させるBGM
	/// </summary>
	public void PlayLoseBgm()
	{
		ChangeBgm(BGMStateType.BattleEnd, SoundController.BgmID.Lose);
	}

	/// <summary>
	/// 引き分け時に再生させるBGM
	/// </summary>
	public void PlayDrawBgm()
	{
		PlayLoseBgm();
	}
	#endregion

	#region BGM停止
	/// <summary>
	/// BGM停止
	/// </summary>
	public void StopBgm()
	{
		ChangeBgm(BGMStateType.None, SoundController.BgmID.None);
	}
	#endregion

	#region BGM切り替え
	/// <summary>
	/// BGM切り替え処理
	/// </summary>
	private void ChangeBgm(BGMStateType state, SoundController.BgmID bgmId)
	{
		if(state == BGMStateType.None)
		{
			// 停止
			this.bgmState = state;
			SoundController.StopBGM();
			return;
		}

		// 勝敗BGMが再生されているなら他のBGMを再生させない
		if(this.bgmState == BGMStateType.BattleEnd)
			return;

		// BGM再生
		this.bgmState = state;
		SoundController.PlayBGM(bgmId);
	}
	#endregion

}
