/// <summary>
/// オプションのスライダーアイテム
/// 
/// 2015/01/08
/// </summary>
using UnityEngine;
using System.Collections;

public class GUIOptionItemColor : GUIOptionItemBase
{
	#region フィールド＆プロパティ
	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UILabel descLabel;	// 説明文
		public UISlider slider;		// スライダー
		public UILabel sliderLabel;	// スライダーラベル
	}

	// 値が変化した時の処理
	System.Action<float> ChangeFunc { get; set; }
	System.Action<UILabel, float> SetLabelFunc { get; set; }
	float Min { get; set; }
	float Max { get; set; }
	// シリアライズされていないメンバーの初期化
	void MemberInit()
	{
		this.ChangeFunc = delegate { };
		this.SetLabelFunc = delegate { };
		this.Min = 0f;
		this.Max = 1f;
	}
	#endregion

	#region 初期化
	public static GUIOptionItemColor Create(GameObject prefab, Transform parent, int itemIndex)
	{
		// インスタンス化
		var go = GUIOptionItemBase.CreateBase(prefab, parent, itemIndex);
		if (go == null)
			return null;

		// コンポーネント取得
		var item = go.GetComponent(typeof(GUIOptionItemColor)) as GUIOptionItemColor;
		if (item == null)
			return null;
		// 値初期化
		item.ClearValue();

		return item;
	}
	public void ClearValue()
	{
		this.Setup("", 0f, 0f, 1f, 0, null, null);
	}
	#endregion

	#region セットアップ
	public void Setup(string descText, float now, float min, float max, int numberOfSteps, System.Action<float> changeFunc, System.Action<UILabel, float> setLabelFunc)
	{
		this.ChangeFunc = (changeFunc != null ? changeFunc : delegate { });
		this.SetLabelFunc = (setLabelFunc != null ? setLabelFunc : delegate { });
		this.Min = Mathf.Min(min, max);
		this.Max = Mathf.Max(min, max);

		// UI更新
		{
			var t = this.Attach;
			if (t.descLabel != null)
				t.descLabel.text = descText;
			if (t.slider != null)
			{
				t.slider.numberOfSteps = numberOfSteps;
				var value = (now - this.Min) / (this.Max - this.Min);
				t.slider.value = value;
			}
		}
	}
	#endregion

	#region NGUIリフレクション
	public void OnValueChange()
	{
		if (UISlider.current == null)
			return;

		// 実際の数値に変換
		var value = this.Min + (this.Max - this.Min) * UISlider.current.value;

		this.ChangeFunc(value);
		if (this.Attach.sliderLabel != null)
			this.SetLabelFunc(this.Attach.sliderLabel, value);
	}
	#endregion
}
