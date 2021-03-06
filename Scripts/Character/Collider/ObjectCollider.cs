/// <summary>
/// 
/// 
/// 2015/05/21
/// </summary>

//#define HIT_ROOT_CHECK

using UnityEngine;
using System.Collections;

using Scm.Common.GameParameter;
using Scm.Common.Master;

public class ObjectCollider : MonoBehaviour
{
	#region フィールド
	/// <summary>
	/// 当たり判定をアタッチするボーン.
	/// </summary>
	const string HitRootBone = "Hit_Root";

	/// <summary>
	/// Colliderの持ち主.
	/// </summary>
	[SerializeField]
	private ObjectBase objectBase;
	#endregion

	#region staticメソッド.
	/// <summary>
	/// HitしたObjectBaseを取得する.
	/// </summary>
	static public ObjectBase GetCollidedObject(GameObject gameObject)
	{
		var objectCollider = gameObject.GetComponent<ObjectCollider>();
		if(objectCollider != null)
		{
			return objectCollider.objectBase;
		}
		return gameObject.GetComponent<ObjectBase>();
	}
	#endregion

	#region MonoBehaviourメソッド.
	void Awake()
	{
		if(objectBase != null)
		{
			// モデル読み込み終了時Eventにアタッチ処理を登録する.
			objectBase.LoadModelCompletedEvent += SetParent;
		}
	}
	/* HACK: 同フレームでの破壊時にobjectBase.transform == nullと判定できず,SetParentではnull扱いになってしまうため一旦オミット.
	void OnDestroy()
	{
		// 変身などでモデルチェンジが起きた場合,一旦規定位置に戻す.
		if(objectBase != null && objectBase.transform != null)
		{
			var newObj = GameObject.Instantiate<GameObject>(this.gameObject);
			newObj.transform.SetParent(objectBase.transform, false);
			newObj.transform.localPosition = Vector3.up;
		}
	}
	*/
	#endregion

	#region メソッド.
	void SetParent()
	{
		if(objectBase != null)
		{
			// 指定ボーンにアタッチする.
			Transform attachBone = objectBase.gameObject.transform.Search(HitRootBone);
			if(attachBone == null)
			{
				Debug.LogWarning("Not found " + HitRootBone + " : " + objectBase.gameObject.name);
				this.transform.localPosition = Vector3.up;
			}
			else
			{
				this.transform.localPosition = Vector3.zero;
				this.transform.SetParent(attachBone.transform, false);
#if HIT_ROOT_CHECK
				// TODO:Hit_Root のモーション対応が済んでないものを見分けるための処置、全部対応したら消す
#if XW_DEBUG || ENABLE_GM_COMMAND
				if (IsCheckHitRoot)
#endif
				{ 
					attachBone.localPosition = new Vector3(0f, 5f, 0f);
					this._checkCountDown = 5f;
				}
#endif
			}
		}
	}
#if XW_DEBUG || ENABLE_GM_COMMAND
	public static bool IsCheckHitRoot = true;
#endif
#if HIT_ROOT_CHECK
	// TODO:デバッグ用チェック
	float _checkCountDown = 5f;
	void Update()
	{
#if XW_DEBUG
		if (!IsCheckHitRoot) return;
#endif
		if (objectBase != null && this._checkCountDown > 0f)
		{
			var a = objectBase.AvaterModel;
			if (a != null && a.ModelTransform != null)
			{
				Transform attachBone = objectBase.gameObject.transform.Search(HitRootBone);
				if (attachBone != null)
				{
					if (attachBone.localPosition.y > 4f)
					{
						this._checkCountDown -= Time.deltaTime;
						if (this._checkCountDown <= 0f)
						{
							var msg = "Application.Quit:" + HitRootBone + " animation Not Found!!";
							BugReportController.SaveLogFile(msg);
							Debug.LogWarning(msg);
							GUISystemMessage.SetModeOK("エラー", "不正なデータを検知しました", "終了", true, Application.Quit);
						}
					}
				}
			}
		}
	}
#endif
    #endregion
}
