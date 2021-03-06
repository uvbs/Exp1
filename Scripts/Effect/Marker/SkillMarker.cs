/// <summary>
/// スキルマーカー
/// 
/// 2013/05/16
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.Master;
using Scm.Common.GameParameter;

public class SkillMarker : MarkerBase
{
	#region フィールド＆プロパティ
	BulletBase Bullet { get; set; }
	System.Action UpdateCollider = () => {};
	#endregion

	#region 初期化
	public void Setup(BulletBase bullet, SkillMarkerMasterData markerData)
	{
		this.enabled = false;
		this.Bullet = bullet;

		// トランスフォーム設定
		Transform t = this.transform;
		t.localPosition = Vector3.zero;
		t.localRotation = Quaternion.identity;
		t.localScale = new Vector3(markerData.SizeX, markerData.SizeY, markerData.SizeZ);
		// マーカーオブジェクトをインスタンス化する
		base.CreateMarkerObject(bullet.CasterTeamType, markerData, this.SetupMarkerObject);
		// コライダー処理設定
		this.SetupCheckCollider(true);
	}
	public void SetupCheckCollider(bool isCheckStart)
	{
		if (this.Bullet == null)
			isCheckStart = false;
		if (this.Bullet.Collider == null)
			isCheckStart = false;

		// コライダー処理設定
		if (isCheckStart)
			this.UpdateCollider = this._UpdateCollider;
		else
			this.UpdateCollider = () => {};
	}
	/// <summary>
	/// マーカーオブジェクトがインスタンス化された時に呼び出される
	/// </summary>
	void SetupMarkerObject(GameObject marker)
	{
		if (marker == null)
			{ return; }

		Transform t = marker.transform;
		t.parent = this.transform;
		t.localPosition = Vector3.zero;
		t.localRotation = Quaternion.identity;
		t.localScale = Vector3.one;

		this.enabled = true;
		this.Update();
	}
	#endregion

	#region 更新
	void Update()
	{
		// 弾丸が消失したら削除
		if (this.Bullet == null)
		{
			base.MarkerObjectDestroy();
			return;
		}

		// 弾丸の移動値を反映する
		{
			Transform t = this.Bullet.transform;
			Vector3 position = new Vector3(t.position.x, 0f, t.position.z);
			this.transform.localPosition = position;
		}

		// コライダー処理
		this.UpdateCollider();
	}
	/// <summary>
	/// コライダーがある場合はコライダー処理をする
	/// </summary>
	void _UpdateCollider()
	{
		// 弾丸のコライダーが消失したら削除
		if (!this.Bullet.Collider.enabled)
		{
			base.MarkerObjectDestroy();
		}
	}
	#endregion
}
