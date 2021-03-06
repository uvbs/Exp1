/// <summary>
/// 3Dオブジェクトに対するUIを設定する(ロビー用)
/// 
/// 2014/06/25
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;

public class GUIObjectUILobbyData : GUIObjectUIBaseData
{
	#region フィールド＆プロパティ
	/// <summary>
	/// モデルのアタッチする名前
	/// </summary>
	[SerializeField]
	string _modelAttachName = "name_plate";
	public string ModelAttachName { get { return _modelAttachName; } }

	/// <summary>
	/// 表示範囲
	/// 0なら表示しない
	/// </summary>
	[SerializeField]
	float _drawRange = 50f;
	public float DrawRange { get { return _drawRange; } }

	/// <summary>
	/// 各アイテムの位置設定用
	/// </summary>
	[SerializeField]
	OUIItemRoot.AttachSettings _attach = new OUIItemRoot.AttachSettings() { isEnable = true, };
	public OUIItemRoot.AttachSettings Attach { get { return _attach; } }

	/// <summary>
	/// BG用スプライト
	/// </summary>
	[SerializeField]
	OUIItemRoot.Settings _bg = new OUIItemRoot.Settings() { isEnable = true, isInRangeDraw = true, attachName = "BG", };
	public OUIItemRoot.Settings Bg { get { return _bg; } }

	/// <summary>
	/// 名前設定
	/// </summary>
	[SerializeField]
	OUIItemRoot.Settings _name = new OUIItemRoot.Settings() { isEnable = true, isInRangeDraw = true, attachName = "Name", };
	public OUIItemRoot.Settings Name { get { return _name; } }

	/// <summary>
	/// 状態アイコン
	/// </summary>
	[SerializeField]
	OUIItemRoot.Settings _status = new OUIItemRoot.Settings() { isEnable = true, isInRangeDraw = true, attachName = "Status", };
	public OUIItemRoot.Settings Status { get { return _status; } }

	/// <summary>
	/// ランキング上位者の特殊アイコン
	/// </summary>
	[SerializeField]
	OUIItemRoot.Settings _ranking = new OUIItemRoot.Settings() { isEnable = true, isInRangeDraw = true, attachName = "Ranking" };
	public OUIItemRoot.Settings Ranking { get { return _ranking; } }

	/// <summary>
	/// 勝敗数設定
	/// </summary>
	[SerializeField]
	OUIItemRoot.Settings _winlose = new OUIItemRoot.Settings() { isEnable = true, isInRangeDraw = true, attachName = "WinLose", };
	public OUIItemRoot.Settings WinLose { get { return _winlose; } }

    [SerializeField]
    OUIItemRoot.Settings _recruitment = new OUIItemRoot.Settings() { isEnable = true, isInRangeDraw = true, attachName = "Recruitment", };
    public OUIItemRoot.Settings Recruitement { get { return _recruitment; } }
	#endregion

	#region 初期化
	public override OUIItemRoot Create(ObjectBase o)
	{
		if (o == null)
			return null;
		var r = GUIObjectUI.CreateRoot(o, this.ModelAttachName, this.DrawRange);
		if (r == null)
			return null;

		r.SetupAttach(this.Attach);
		r.SetupBG(this.Bg);
		r.SetupName(this.Name);
		r.SetupWinLose(this.WinLose);
		r.SetupStatus(this.Status);
		r.SetupRanking(this.Ranking);
        Debug.Log("<color=#00ff00>OUIItemRoot.SetRecruitment</color>");
        r.SetRecruitment(this.Recruitement);
		return r;
	}
	#endregion
}
