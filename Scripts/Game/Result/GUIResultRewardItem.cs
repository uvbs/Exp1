/// <summary>
/// リザルトの報酬アイテム
/// 
/// 2014/11/27
/// </summary>
using UnityEngine;
using System.Collections;
using System;

public class GUIResultRewardItem : MonoBehaviour
{
	#region フィールド&プロパティ
	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public XUIButton button;
		public GameObject frontObject;
		public UIPlayTween openEffectPlayTween;
	}

	/// <summary>
	/// アイテムID
	/// </summary>
	private int itemID;

	/// <summary>
	/// アイテムが開いた時に外部から呼び出す用
	/// </summary>
	private Action<int> opened = (itemID)=>{};
	#endregion

	#region セットアップ
	/// <summary>
	/// セットアップ処理
	/// </summary>
	public void Setup(Action<int> opened)
	{
		this.opened = opened;
		this.itemID = 0;
	}
	#endregion

	#region ボタン機能ON/OFF
	/// <summary>
	/// ボタン機能のOM/OFF設定する
	/// </summary>
	public void SetButtonEnable(bool isEnable)
	{
		if(this.Attach.button == null) return;
		this.Attach.button.isEnabled = isEnable;
	}
	#endregion

	#region NGUIリフレクション
	/// <summary>
	/// アイテムが開いた時の処理
	/// </summary>
	public void Opened()
	{
		// 表のオブジェクトを表示
		if(this.Attach.frontObject != null)
		{
			this.Attach.frontObject.SetActive(true);
		}
		// アイテムの開く演出を再生
		if(this.Attach.openEffectPlayTween != null)
		{
			this.Attach.openEffectPlayTween.Play(true);
		}
		// ボタンの機能を無効にする
		SetButtonEnable(false);

		this.opened(this.itemID);
	}
	#endregion
}
