/// <summary>
/// 3Dオブジェクトに対するUI
/// ロックオン
/// 
/// 2014/07/07
/// </summary>

#define NO_LOCK_CENTER_CURSOR

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class OUILockon : MonoBehaviour
{
    #region 宣言

    /// <summary>
    /// 2017/03/11, Huhao, 3rd version lock-on strategy:
    /// Mode None:
    ///   * No auto lock 
    ///   * Click to manually lock, rotate to release lock
    /// Mode Single(Manually):
    ///   * Auto lock (by strategy)
    ///   * Always choose the best suitable target in angle
    ///   * When casting skills, always prefer the locked target 
    /// Mode Double(Automatically):  - this is the default mode
    ///   * Auto lock (by strategy)
    ///   * Click to manually lock, rotate to choose new target or release lock: rotate to change a target, without rotation, target won't be changed 
    ///       unless it get out of the camera's view
    ///   * When casting skills, always prefer the locked target 
    /// </summary>
    [System.Serializable]
	public enum Type
	{
		None,
		Single,
		Double,
		Max,
	}
	#endregion

	#region フィールド＆プロパティ
	// タップのロックオン距離
	const float TapLockonRange = 5000f;

	/// <summary>
	/// 距離表示フォーマット
	/// </summary>
	[SerializeField]
	string _distanceFormat = "{0:0}m";
	string DistanceFormat { get { return _distanceFormat; } }

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public GameObject root;
		public GameObject lockon1;
		public GameObject lockon2;
		public UILabel distanceLabel;
		public UIPlayTween hitPlayTween;
	}

	// 親のObjectUI
	GUIObjectUI ObjectUI { get; set; }
	// ロックオンが届く範囲
	public float LockonRange { get; set; }
	// ロックオン対象のモデルにアタッチする名前
	string ModelAttachName { get; set; }
	// ロックオン対象のTransform
	Transform LockonTransform { get; set; }
	// ロックオン対象のワールド座標（対象が消滅してもいいように分離）
	Vector3 WorldPosition { get; set; }

	// プレイヤーの消滅モニタリング
	System.Action DestroyMonitorFunc = () => { };

	/// <summary>
	/// 距離設定
	/// </summary>
	float Distance
	{
		set
		{
			this.Attach.distanceLabel.text = string.Format(this.DistanceFormat, value);
		}
	}

	/// <summary>
	/// プレイヤー
	/// </summary>
	Player _player;
	Player Player
	{
		get
		{
			if (_player == null)
			{
				_player = GameController.GetPlayer();
			}
			return _player;
		}
	}

    //Lee add
    public static OUILockon Instance;

    public static void LockNone()
    {
        if (null != Instance)
        {
            Instance.NowLockonType = OUILockon.Type.None;
        }
    }
	public bool IsSetup { get; private set; }

    private Type nowLockonType;
	public Type NowLockonType
	{
		get { return nowLockonType; }
		set { this.SetLockonType(value, true); }
	}
	void SetLockonType(Type type, bool isAddProc)
	{
        /*
		if (nowLockonType == Type.Double && type == Type.Double)
			return;*/
		OldLockonType = nowLockonType;
		nowLockonType = type;
		switch (type)
		{
		case Type.None:
			this.UpdateProc = this.Update_LockNone;
			if (isAddProc)
			{
				this.Attach.distanceLabel.text = "";
			}
			break;
		case Type.Single:
			this.UpdateProc = this.Update_LockSingle;
			break;
		case Type.Double:
			this.UpdateProc = this.Update_LockDouble;
			if (isAddProc)
			{
				//this.IsLookAtTarget = true;
				this.SetDoubleLock(true);
			}
			break;
		}
	    try
	    {
            if (null == GUITargetButton.Instance)
            {
                return;
            }
            GUITargetButton.Instance.OnToggleLockonTarget(nowLockonType);
	    }
	    catch (Exception)
	    {
	        throw;
	    }
    }
	public float LockCancelThreshold { get; private set; }
	public Type OldLockonType { get; private set; }
	public bool IsLookAtTarget { get; set; }

	public int LockonInFieldId { get; private set; }
	public ObjectBase LockonObject
	{
        get; private set;
	}

	public System.Action UpdateProc;

	/// <summary>
	/// オートロックオンの優先度を決める値.高い方が角度重視になる.
	/// </summary>
	//public float lockonPriority = 1.0f;
	#endregion

	#region 作成
	public static OUILockon Create(GUIObjectUI objectUI, OUILockon prefab, Transform parent, string modelAttachName, float lockonRange, Vector3 offset)
	{
		var com = SafeObject.Instantiate(prefab) as OUILockon;
		if (com == null)
		{
			Debug.LogWarning("OUILockon.Create: Not Found OUILockon");
			return null;
		}

        if (NetworkController.Instance != null && NetworkController.ServerValue.AreaType == Scm.Common.GameParameter.AreaType.Field) {
            com.gameObject.SetActive(true);
        } 

		// 親子付け
		var t = com.transform;
		t.parent = parent;
		t.localPosition = Vector3.zero;
		t.localRotation = Quaternion.identity;
		t.localScale = Vector3.one;

		// 設定
		com.Setup(objectUI, prefab, modelAttachName, lockonRange, offset);
	    Instance = com;
		return com;
	}
	void Setup(GUIObjectUI objectUI, OUILockon prefab, string modelAttachName, float lockonRange, Vector3 offset)
	{
		// 初期化
		this.name = prefab.name;
		this.ObjectUI = objectUI;
		this.LockonRange = lockonRange;
		this.ModelAttachName = modelAttachName;
		this.transform.localPosition = offset;

		this.Awake();
	}
	void Awake()
	{
		this.LockonTransform = null;
		this.WorldPosition = Vector3.zero;
		this.DestroyMonitorFunc = this.DestroyMonitor;

		this.NowLockonType = Type.Double;

		this.LockonInFieldId = -1;
        this.LockonObject = null;
		this.LockCancelThreshold = (float)Screen.width / 5f;
	}
	#endregion

	#region 破棄
	public void Destroy()
	{
		UnityEngine.Object.Destroy(this.gameObject);
	}
	void DestroyMonitor()
	{
		// オブジェクトが消滅したらモニタリングをやめて自分自身を削除する
		if (this.Player != null)
			return;

		this.Destroy();

		// モニター解除
		this.DestroyMonitorFunc = () => { };
	}
	#endregion

	#region フリック処理
	void OnEnable()
	{
		if (TouchSystem.Instance != null)
		{
			TouchSystem.Instance.OnFlickLockon += this.OnFlickLockon;
			TouchSystem.Instance.OnTapEvent += this.TapLockon;
		}
	}
	void OnDisable()
	{
		if (TouchSystem.Instance != null)
		{
			TouchSystem.Instance.OnFlickLockon -= this.OnFlickLockon;
			TouchSystem.Instance.OnTapEvent -= this.TapLockon;
		}
	}
	/// <summary>
	/// フリックトリガー
	/// FingerGestureのSwipeに登録する
	/// </summary>
	void OnFlickLockon(FingerGestures.SwipeDirection direction, Vector2 move)
	{
		// 左右フリックのみ
		if ((direction & FingerGestures.SwipeDirection.Horizontal) != FingerGestures.SwipeDirection.None)
		{
            // Nothing to do...
        }
	}
	#endregion

	#region 更新
	void LateUpdate()
	{
		// 削除モニタリング
		this.DestroyMonitorFunc();

		this.UpdateProc();
		this.CheckLookAtTarget();

		// 位置更新
		{
			if (this.LockonTransform != null)
				this.WorldPosition = this.LockonTransform.position;
			this.ObjectUI.UpdatePosition(this.transform, this.WorldPosition);
		}
		// 表示設定
		{
			this.UpdateVisible();
		}
	}
	/// <summary>
	/// ノーロックモード
	/// </summary>
	void Update_LockNone()
	{
        this.LockonObject = null;
		this.Lockon(null);
	}
	/// <summary>
	/// シングルロックモード
	/// </summary>
	void Update_LockSingle()
	{
		List<ObjectBase> objectList = new List<ObjectBase>();
		this.GetLockObject(ref objectList, true);
		if (objectList.Count <= 0)
		{
			this.Update_LockNone();
			return;
		}

		ObjectBase target = this.LockonObject;
        this.LockonObject = objectList[0];
		this.Lockon(target);

		// ターゲットまでの距離
		this.Distance = Vector3.Distance(this.Player.transform.position, LockonObject.transform.position);
	}
	void SetDoubleLock(bool isCompareEntrantType)
	{
        // Huhao: use new lock logic
        /*
		List<ObjectBase> objectList = new List<ObjectBase>();
		this.GetLockObject(ref objectList, isCompareEntrantType);
		if (objectList.Count <= 0)
		{
			this.NowLockonType = Type.None;
			this.Update_LockNone();
			return;
		}

		this.LockonIndex = 0;
		this.OldObjectList = objectList;
		this.Lockon(this.LockonObject);*/
	}
	
    /// <summary>
    /// ダブルロックモード
    /// </summary>
    void Update_LockDouble()
	{
        /*
		// ロック対象の存在チェック.
		ObjectBase lockonObject = this.LockonObject;
		if (lockonObject != null && 
			lockonObject.StatusType != Scm.Common.GameParameter.StatusType.Dead &&
			!lockonObject.IsDisappear)
		{
			// ターゲットまでの距離
			Player player = this.Player;
			if(player != null)
			{
				this.Distance = Vector3.Distance(player.transform.position, lockonObject.transform.position);
			}
		}
		else
		{
            // No lock object exists but do not change the mode
		}*/
        if (TouchSystem.Instance.Rotation.magnitude > 0) {

            // The player is rotating camera to select target
            List<ObjectBase> objectList = new List<ObjectBase>();
            this.GetLockObject(ref objectList, true);
            if (objectList.Count <= 0) {
                this.Update_LockNone();
                return;
            }
            ObjectBase oldTarget = this.LockonObject;
            this.LockonObject = objectList[0];
            this.Lockon(this.LockonObject);

        } else {

            bool outOfRange = false;
            if (LockonObject != null) {
                // Check if it is out of range
                if (LockonObject.StatusType == Scm.Common.GameParameter.StatusType.Dead ||
                    LockonObject.IsDisappear) {
                    outOfRange = true;
                }
                if (!outOfRange) {
                    CharacterCamera cc = GameController.CharacterCamera;
                    if (cc) {
                        // Not visible in camera view
                        Vector3 pos = cc.GetComponent<Camera>().WorldToViewportPoint(LockonObject.transform.position);
                        if (pos.z > 0 && pos.x >= 0.15f && pos.x <= 0.85f && pos.y >= 0.0f && pos.y <= 1.0f) {
                            // in view
                        } else {
                            outOfRange = true;
                        }
                    }
                }
            }

            if (LockonObject != null && outOfRange) {
                this.Update_LockNone();
                return;
            }
        }

        if (this.LockonObject != null && null != this.Player) {
            // ターゲットまでの距離
            this.Distance = Vector3.Distance(this.Player.transform.position, LockonObject.transform.position);
        }
    }

    /// <summary>
    /// カメラをロック相手に向ける.
    /// </summary>
    private bool RotateCameraToLock = true;
	public void RotateCameraToLockObj()
	{
	    if (IsDragging())
	    {
	        RotateCameraToLock = false;
	    }
        if (!RotateCameraToLock)
	    {
	        return;
	    }
		CharacterCamera cc = GameController.CharacterCamera;
		if (cc && this.LockonObject != null)
		{
			Transform transform = cc.transform;
			Vector3 r = new Vector3(this.LockonObject.transform.position.x, 0f, this.LockonObject.transform.position.z) - new Vector3(transform.position.x, 0f, transform.position.z);
			if (r == Vector3.zero)
				return;
			cc.LookAtRotation = Quaternion.LookRotation(r);
			Vector3 rotation = cc.Rotation;
			rotation.y = 0f;
			cc.Rotation = rotation;
            // Rotate character too
            if (cc.Character != null) {
                Quaternion q = cc.AdjustLookRotation(r);
                cc.Character.SetNextRotation(q);
            }
		}
	}
	void GetLockObject(ref List<ObjectBase> objectList, bool isCompareEntrantType)
	{
		if (null == this.Player)
			return;

		objectList = GameController.SearchLockonTarget(this.Player, this.LockonRange);
		if (objectList.Count <= 0)
		{
			return;
		}

		//ターゲットオブジェクトをソートする.
		this.LockonObjectSort(ref objectList, isCompareEntrantType);
	}
	void UpdateVisible()
	{
#if NO_LOCK_CENTER_CURSOR
        if (!this.IsSetup)
		{
			if(!this.Attach.lockon1.activeSelf)
            {
                this.Attach.lockon1.SetActive(true);
            }
            if(this.Attach.lockon2.activeSelf)
            {
                this.Attach.lockon2.SetActive(false);
            }
            if(this.Attach.root.activeSelf)
            {
                this.Attach.root.SetActive(false);
            }
			return;
		}
#endif
        this.Attach.root.SetActive(true);
        this.Attach.lockon1.SetActive(true);
        this.Attach.lockon2.SetActive(true);

        /*
		// 表示設定
		bool isActive = this.ObjectUI.IsInRange(this.WorldPosition);
		this.Attach.root.SetActive(isActive);
		if (isActive)
		{
			switch (this.NowLockonType)
			{
			case Type.Single:
				this.Attach.lockon1.SetActive(true);
				this.Attach.lockon2.SetActive(false);
				break;
			case Type.Double:
				this.Attach.lockon1.SetActive(false);
				this.Attach.lockon2.SetActive(true);
				break;
			case Type.None:
			default:
				this.Attach.lockon1.SetActive(false);
				this.Attach.lockon2.SetActive(false);
				break;
			}
		}
		else
		{
			this.Attach.lockon1.SetActive(false);
			this.Attach.lockon2.SetActive(false);
		}*/
    }
	void CheckLookAtTarget()
	{
		if (!this.IsLookAtTarget)
			return;
		this.IsLookAtTarget = false;

		ObjectBase lockonObject = this.LockonObject;
		if (null == lockonObject)
			return;

		// 一瞬でふりむかせる
		this.Player.SetLookAtTarget(lockonObject.transform);
		// カメラも振り向かせる
		{
			CharacterCamera cc = GameController.CharacterCamera;
			if (cc)
			{
				cc.IsCharaForward = true;
			}
		}
		//GameController.Instance.CharacterCamera.LookAtTarget_Y(lockonObject.transform);
	}
	#endregion

	#region ロックオン
	public void ToggleLockonTarget()
	{
        switch (this.NowLockonType)
		{
		case Type.None:
            this.SetLockonType(Type.Single, false);
			break;
		case Type.Single:
            this.SetLockonType(Type.Double, false);
			break;
		case Type.Double:
            this.SetLockonType(Type.None, false);
			break;
		}
	}

    private bool IsDragging() {
        float axisX = TouchSystem.Instance.Rotation.x;
        float axisY = -TouchSystem.Instance.Rotation.y;
        if (Mathf.Abs(axisX) < ROTATION_THRESHOLD && Mathf.Abs(axisY) < ROTATION_THRESHOLD) {
            return false;
        }
        return true;
    }

    public void LockOnTarget(ObjectBase target)
    {
        if (target == null)
            return;

//        // 既にロックオンされていたらロックを外す
//        if (this.LockonObject == target)
//        {
//            this.NowLockonType = Type.None;
//            return;
//        }

        // ロックオン
        this.SetLockonType(Type.Double, false);
        this.Lockon(target);
    }

    private const float ROTATION_THRESHOLD = 0.01f;

    public void Lockon(ObjectBase objectBase)
	{
		if (objectBase)
		{
			if (this.LockonInFieldId == objectBase.InFieldId)
				return;
		}
		this.IsSetup = false;
		this.LockonInFieldId = -1;
#if NO_LOCK_CENTER_CURSOR
#else
		this.Attach.lockon1.SetActive(false);
		this.Attach.lockon2.SetActive(false);
#endif
        if (objectBase == null) {
            return;
        }
			
		this.LockonInFieldId = objectBase.InFieldId;

		// セッティング
		this.LockonTransform = null;
		if (!string.IsNullOrEmpty(this.ModelAttachName))
			this.LockonTransform = objectBase.transform.Search(this.ModelAttachName);
		if (this.LockonTransform == null)
			this.LockonTransform = objectBase.transform;

		this.IsSetup = true;
        this.RotateCameraToLock = true;
	}
	#endregion

    #region ヒットアニメーション
    public void HitAnimation()
	{
		if (this.Attach.hitPlayTween != null)
		{
			this.Attach.hitPlayTween.Play(true);
		}
	}
	#endregion

	private void TapLockon(Vector3 screen)
	{
		// レイを飛ばして一番近いロックオン対象を検索する
		ObjectBase target = null;
		{
			Camera cam = this.ObjectUI.WorldCamera;
			Ray ray = cam.ScreenPointToRay(screen);
			List<RaycastHit> hits = new List<RaycastHit>(Physics.SphereCastAll(ray, 1f, TapLockonRange, GameController.BulletLayerMask));
			hits.Sort((x, y) => { return GameGlobal.AscendSort(x.distance, y.distance); });
			foreach (RaycastHit hitInfo in hits)
			{
				ObjectBase objectBase = ObjectCollider.GetCollidedObject(hitInfo.collider.gameObject);
				if (objectBase == null)
					continue;
				bool isLockon = GameController.IsLockonTarget(objectBase, this.Player, this.Player.Position, cam.transform.forward, GameConstant.TapLockonAngle, GameConstant.TapLockonRange, GameConstant.TapLockonRange);
				if (!isLockon)
					continue;
				target = objectBase;
				break;
			}
		}
		if (target == null)
			return;

		// 既にロックオンされていたらロックを外す
		if (this.LockonObject == target)
		{
            LockNone();
            return;
		}

		// ロックオン
		this.SetLockonType(Type.Double, false);
        this.LockonObject = target;
		this.Lockon(target);

        return;
	}

	/// <summary>
	/// ターゲットオブジェクトをソートする.
	/// </summary>
	private bool LockonObjectSort(ref List<ObjectBase> objectList, bool isCompareEntrantType)
	{
		if (this.Player != null && Camera.main != null)
		{
			// 真正面に近いオブジェクトを優先.
			Transform playerTransform = this.Player.transform;
			Vector3 right = Camera.main.transform.right;
			if (isCompareEntrantType)
			{
				objectList.Sort(
					(ObjectBase a, ObjectBase b) =>
					{
						int priA = a.EntrantType.GetTargetPriority();
						int priB = b.EntrantType.GetTargetPriority();
						if (priA == priB)
						{
							Vector3 defA = (a.transform.position - playerTransform.position).normalized;
							float evalA = Mathf.Abs(Vector3.Dot(right, defA));

							Vector3 defB = (b.transform.position - playerTransform.position).normalized;
							float evalB = Mathf.Abs(Vector3.Dot(right, defB));

							return (evalA < evalB) ? -1 : 1;
						}
						else
						{
							return priB - priA;
						}
					}
				);
			}
			else
			{
				objectList.Sort(
					(ObjectBase a, ObjectBase b) =>
					{
						Vector3 defA = (a.transform.position - playerTransform.position).normalized;
						float evalA = Mathf.Abs(Vector3.Dot(right, defA));
						Vector3 defB = (b.transform.position - playerTransform.position).normalized;
						float evalB = Mathf.Abs(Vector3.Dot(right, defB));

						return (evalA < evalB) ? -1 : 1;
					}
				);
			}
			return true;
		}
		else
		{
			return false;
		}
	}
}
