/// <summary>
/// エネミーマーカー
/// 
/// 2013/05/17
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMaker : MonoBehaviour
{
	#region フィールド＆プロパティ
	[SerializeField]
	private float size = 1.5f;
	public float Size { get { return size; } private set { size = value; } }

	[SerializeField]
	private float shadowScale = 1.6f;

	public ObjectBase Enemy { get; private set; }
	public Player Player { get; private set; }
	public GameObject MakerObject { get; private set; }
	public GameObject ShadowObject { get; private set; }

	private Character character;
	#endregion

	#region 初期化
	public void SetupMarker()
	{
		this.character = GetComponent<Character>();

		// 影を作成
		this.ShadowObject = SafeObject.Instantiate(PlayerManager.Instance.ShadowPrefab) as GameObject;
		if (ShadowObject)
		{
			Transform t = ShadowObject.transform;
			t.parent = EffectManager.Instance.transform;
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = new Vector3(this.shadowScale, 1f, this.shadowScale);
		}

		this.Enemy = this.gameObject.GetSafeComponent<ObjectBase>();

		StartSetupMarkerCoroutine();
	}

	public void StartSetupMarkerCoroutine()
	{
		StartCoroutine(SetupMarkerCoroutine());
	}

	IEnumerator SetupMarkerCoroutine()
	{
		// プレイヤー取得.
		this.Player = GameController.GetPlayer();
		while (this.Player == null)
		{
			yield return 0;
			this.Player = GameController.GetPlayer();
		}
		// 旧マーカが存在していれば削除.
		if (this.MakerObject)
		{
			GameObject.Destroy(this.MakerObject);
		}

		// インジケータリングに登録する
		RadarRing.AddTarget(this);

		// プレイヤーと同じチームならマーカーを生成しない
		if (this.Enemy != null && this.Enemy.TeamType != this.Player.TeamType)
		{
			// マーカー生成
			GameObject markerPrefab = PersonManager.Instance.EnemyMakerPrefab.GetPrefab(this.Enemy.TeamType.GetClientTeam());
			if(markerPrefab != null)
			{
				this.MakerObject = SafeObject.Instantiate(markerPrefab) as GameObject;
				if (this.MakerObject)
				{
					Transform t = this.MakerObject.transform;
					t.parent = EffectManager.Instance.transform;
					t.localPosition = Vector3.zero;
					t.localRotation = Quaternion.identity;
					t.localScale = new Vector3(this.Size, 1f, this.Size);
				}

				this.enabled = true;
			}
		}
	}
	#endregion

	#region 更新
	void LateUpdate()
	{
		Transform rootTransform = character.AvaterModel.RootTransform;
		if(rootTransform)
		{
			Vector3 position = new Vector3(rootTransform.position.x, character.CharacterMove.GroundPosition.y, rootTransform.position.z);
			Quaternion rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0));
			if(MakerObject)
			{
				this.MakerObject.transform.position = position;
				this.MakerObject.transform.rotation = rotation;
				this.MakerObject.transform.localScale = rootTransform.localScale * Size;
			}
			if(ShadowObject)
			{
				this.ShadowObject.transform.position = position;
				this.ShadowObject.transform.rotation = rotation;
				this.ShadowObject.transform.localScale = rootTransform.localScale * shadowScale;
			}
		}
	}
	#endregion

	#region 破棄
	void OnDestroy()
	{
		RadarRing.RemoveTarget(this);
		GameObject.Destroy(this.MakerObject);
		GameObject.Destroy(this.ShadowObject);
	}
	#endregion
}
