//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the object's position.
/// </summary>

[RequireComponent(typeof(UISprite))]
[AddComponentMenu("Designer/NGUI/XUITweenUV")]
public class XUITweenUV : UITweener
{
	public Vector2 from;
	public Vector2 to;
	
	UISprite sprite;
	Vector2 uvOffset = new Vector2(0, 0);
	
	/// <summary>
	/// Tween's current value.
	/// </summary>

	public Vector2 value
	{
		get
		{
			return uvOffset;
		}
		set
		{
			uvOffset = value;
			if (this.sprite != null)
			{
				UpdateUV();
			}
		}
	}
	
	void Awake ()
	{
		this.sprite = this.gameObject.GetSafeComponent<UISprite>();
	}

	/// <summary>
	/// Interpolate and update the color.
	/// </summary>

	override protected void OnUpdate (float factor, bool isFinished) { value = from * (1f - factor) + to * factor; }

	/// <summary>
	/// Start the tweening operation.
	/// </summary>
	
	static public XUITweenUV Begin (GameObject go, float duration, Vector2 uv)
	{
		XUITweenUV comp = UITweener.Begin<XUITweenUV>(go, duration);
		comp.from = comp.value;
		comp.to = uv;
		
		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}
	
	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue () { from = value; }

	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue () { to = value; }

	[ContextMenu("Assume value of 'From'")]
	void SetCurrentValueToStart () { value = from; }

	[ContextMenu("Assume value of 'To'")]
	void SetCurrentValueToEnd () { value = to; }
	
	
	#region UV制御
	
	/// <summary>
	/// UV更新.
	/// </summary>
	
	void UpdateUV()
	{
		// UIWidgetのOnFill更新を停止.
		// 開始時(StartやAwake)でflaseにするとジオメトリが設定されないので描画できない
		this.sprite.fillGeometry = false;
		
		// ジオメトリの設定.
		this.sprite.geometry.Clear();
		this.sprite.OnFill(this.sprite.geometry.verts, this.sprite.geometry.uvs, this.sprite.geometry.cols);
		
		// UVスクロール.
		UVScroll();
		
		// ジオメトリ更新
		this.sprite.MarkAsChanged();
	}
	
	/// <summary>
	/// UVスクロール.
	/// </summary>
	
	void UVScroll()
	{
		for(int index = 0; index < this.sprite.geometry.uvs.size; ++index)
		{
			this.sprite.geometry.uvs[index] += this.uvOffset;
		}
	}
	
	#endregion
}

