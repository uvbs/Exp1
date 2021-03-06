/// <summary>
/// 3Dオブジェクトに対するUIアイテム
/// 名前
/// 
/// 2014/06/23
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;

public class OUIItemName : OUIItemBase
{
	#region フィールド＆プロパティ
	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	public AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UILabel label;
		public UIPlayTween deathTween;
	}

	bool IsDeathTween { get; set; }
	#endregion

	#region 作成
	/// <summary>
	/// プレハブ取得
	/// </summary>
	public static OUIItemName GetPrefab(GUIObjectUI.AttachObject.Prefab prefab, ObjectBase o)
	{
		return prefab.name.name;
	}
	#endregion

	#region 更新
	public void UpdateName(string name, TeamType teamType, EntrantType entrantType, int level)
	{
		// カラーコード
		string colorCodeName = name;
		switch (teamType)
		{
		case TeamType.Red:
		case TeamType.Blue:
			colorCodeName = GameConstant.StringWithTeamColor(colorCodeName, teamType);
			break;
		}

		// レベル表示
		string lv = "";
		// バトル中のみレベル表示
		if (ScmParam.Common.AreaType == AreaType.Field)
		{
			switch (entrantType)
			{
			case EntrantType.Pc:
				lv = string.Format("Lv{0} ", level);
				break;
			}
		}

		//this.Attach.label.text = string.Format("{0}{1}", lv, colorCodeName);
        this.Attach.label.text = string.Format("{0}", colorCodeName);
    }
	public void UpdateDeath(bool isDeath, EntrantType entrantType)
	{
		if (entrantType != EntrantType.Pc)
			isDeath = false;
		if (this.IsDeathTween == isDeath)
			return;
		this.IsDeathTween = isDeath;
		// Tween再生開始
		this.Attach.deathTween.resetOnPlay = !isDeath;
		this.Attach.deathTween.Play(isDeath);
	}
	#endregion
}
