/// <summary>
/// ランキングメニュー表示処理.
/// .
/// 2014/04/07.
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;

public class GUIRankingMenuWindow : MonoBehaviour
{
	#region 定数.
	
	/// <summary>
	/// ランキングモード名.
	/// </summary>
	private const string TotalScoreName = "キングオブクロスワールド";
	private const string KillName = "ヒットマン";
	private const string DefeatName = "デストロイヤー";
	
	#endregion
	
	#region フィールド&プロパティ.
	
	/// <summary>
	/// Tween制御.
	/// </summary>
	[SerializeField]
	private UIPlayTween playTween;
	
	#endregion
	
	#region 開始.
	
	void Awake()
	{
		this.gameObject.SetActive(false);
	}
	
	#endregion
	
	#region ウィンドウ.
	
	/// <summary>
	/// ウィンドウを開く処理.
	/// </summary>
	public void Open()
	{
		this.playTween.Play(true);
		this.playTween.disableWhenFinished = AnimationOrTween.DisableCondition.DoNotDisable;
	}
	
	/// <summary>
	/// ウィンドウを閉じる処理.
	/// </summary>
	public void Close()
	{
		this.playTween.Play(false);
		this.playTween.disableWhenFinished = AnimationOrTween.DisableCondition.DisableAfterForward;
	}
	
	#endregion
	
	#region ボタン押された時に呼ばれる処理.
	
	/// <summary>
	/// キングオブクロスワールドボタン.
	/// </summary>
	private void OnKingOfXWorld()
	{
		//GUILobbyRanking.ModeSelect(RankingType.TotalScore, TotalScoreName);
	}
	
	/// <summary>
	/// ヒットマンボタン.
	/// </summary>
	private void OnHitMan()
	{
		//GUILobbyRanking.ModeSelect(RankingType.Kill, KillName);
	}
	
	/// <summary>
	/// デストロイヤーボタン.
	/// </summary>
	private void OnDestroyer()
	{
		//GUILobbyRanking.ModeSelect(RankingType.Defeat, DefeatName);
	}
	
	#endregion
}
