/// <summary>
/// 主にバフアイコンのスプライト処理を行う
/// 
/// 2014/07/14
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Master;

public class GUIBuffIconItem : MonoBehaviour
{
	#region アタッチオブジェクト

	[System.Serializable]
	public class AttachObject
	{
		public UISprite iconSprite;
		public UISprite timeSprite;
	}

	#endregion

	#region フィールド&プロパティ

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	private AttachObject _attach;
	public AttachObject Attach { get { return _attach; } }

	#endregion
	
	#region セットアップ

	public void SetUp(StateEffectMasterData masterData)
	{
		if(masterData == null)
			return;

		// アイコンセット
		SetStateIcon(masterData.IconFile);

		if(this.Attach.timeSprite != null)
		{
			this.Attach.timeSprite.color = new Color32((byte)masterData.Color.R, (byte)masterData.Color.G, (byte)masterData.Color.B, (byte)masterData.Color.A);
		}
	}

	/// <summary>
	/// 状態アイコンアトラスをアセットバンドルから取得しセットする
	/// </summary>
	private void SetStateIcon(string fileName)
	{
		if(this.Attach.iconSprite == null)
			return;
		if(BattleMain.StateIcon == null)
			return;

		// UIAtlas取得
		BattleMain.StateIcon.GetIcon((atlas) =>
		{
			if(!string.IsNullOrEmpty(fileName))
			{
				this.Attach.iconSprite.gameObject.SetActive(true);
				this.Attach.iconSprite.atlas = atlas;
				this.Attach.iconSprite.spriteName = fileName;
			}
			else
			{
				this.Attach.iconSprite.gameObject.SetActive(false);
			}
		});
	}

	#endregion

	#region Active

	/// <summary>
	/// 表示非表示処理
	/// </summary>
	public void SetActive(bool isActive)
	{
		// GameObjectのActiveを設定するとfalseの時にTween系が動作しなくなるので
		// スプライトに設定するようにする
		UISprite[] sprites = this.gameObject.GetSafeComponentsInChildren<UISprite>();
		foreach(UISprite sprite in sprites)
		{
			sprite.enabled = isActive;
		}
	}

	#endregion

	#region 時間

	/// <summary>
	/// 効果時間更新処理
	/// </summary>
	public void TimeUpdate(float remainingTime, float timeMax)
	{
		if(this.Attach.timeSprite == null)
			return;

		this.Attach.timeSprite.fillAmount = remainingTime / timeMax;
	}

	#endregion
}
