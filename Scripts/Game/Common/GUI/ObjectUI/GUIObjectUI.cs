/// <summary>
/// 3Dオブジェクトに対するUI
/// 基本的なパネル操作、各種アタッチオブジェクト、アイテムルート作成のみ
/// 
/// 2014/06/22
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;

public class GUIObjectUI : Singleton<GUIObjectUI>
{
    #region フィールド＆プロパティ
    /// <summary>
    /// 初期化時のアクティブ状態
    /// </summary>
    [SerializeField]
    bool _isStartActive = false;
    public bool IsStartActive { get { return _isStartActive; } }

    /// <summary>
    /// アタッチオブジェクト
    /// </summary>
    [SerializeField]
    AttachObject _attach;
    public AttachObject Attach { get { return _attach; } }
    [System.Serializable]
    public class AttachObject
    {
        public UIPlayTween rootTween;
        public Transform rootObject;

        public Prefab prefab;
        [System.Serializable]
        public class Prefab
        {
            public GameObject root;
            public OUILockon lockon;

            /// <summary>
            /// 各種アイテム用アタッチ
            /// </summary>
            public Attach attach;
            [System.Serializable]
            public class Attach
            {
                public OUIItemAttach attach;
            }

            /// <summary>
            /// 背景
            /// </summary>
            public BG bg;
            [System.Serializable]
            public class BG
            {
                public OUIItemBG bg;
            }

            /// <summary>
            /// バフデバフ
            /// </summary>
            public Buff buff;
            [System.Serializable]
            public class Buff
            {
                public OUIItemBuff buff;
            }

            /// <summary>
            /// ダメージ
            /// </summary>
            public Damage damage;
            [System.Serializable]
            public class Damage
            {
                public OUIItemDamage damage;
            }

            /// <summary>
            /// HP
            /// </summary>
            public HP hp;
            [System.Serializable]
            public class HP
            {
                public Transform root;
            }

            /// <summary>
            /// キル表示
            /// </summary>
            public Kill kill;
            [System.Serializable]
            public class Kill
            {
                public OUIItemKill kill;
            }

            /// <summary>
            /// 名前
            /// </summary>
            public Name name;
            [System.Serializable]
            public class Name
            {
                public OUIItemName name;
            }

            /// <summary>
            /// 状態アイコン
            /// </summary>
            public Status status;
            [System.Serializable]
            public class Status
            {
                public OUIItemStatus status;
            }

            /// <summary>
            /// ランキング上位者の特殊アイコン
            /// </summary>
            public Ranking ranking;
            [System.Serializable]
            public class Ranking
            {
                public OUIItemRanking ranking;
            }

            /// <summary>
            /// 戦闘中の順位アイコン
            /// </summary>
            public ScoreRank scoreRank;
            [System.Serializable]
            public class ScoreRank
            {
                public OUIItemScoreRank scoreRank;
            }

            /// <summary>
            /// 勝敗数
            /// </summary>
            public WinLose winlose;
            [System.Serializable]
            public class WinLose
            {
                public OUIItemWinLose winlose;
            }

            /// <summary>
            /// 勝敗数
            /// </summary>
            public ResidentProcess resident;
            [System.Serializable]
            public class ResidentProcess
            {
                public OUIItemResidentProgress resident;
            }

            /// <summary>
            /// Recruitment
            /// </summary>
            public Recruitment recruitment;
            [System.Serializable]
            public class Recruitment {
                public OUIItemRecruitment recruitment;
            }
        }
    }

    public bool IsActive { get; private set; }

    // 3Dカメラ
    Camera _worldCamera = null;
    public Camera WorldCamera
    {
        get
        {
            if (_worldCamera == null)
            {
                _worldCamera = Camera.main;
            }
            return _worldCamera;
        }
    }
    // 2Dカメラ
    Camera _screenCamera = null;
    Camera ScreenCamera
    {
        get
        {
            if (_screenCamera == null)
            {
                _screenCamera = NGUITools.FindCameraForLayer(this.gameObject.layer);
            }
            return _screenCamera;
        }
    }
    // ロックオンカーソル
    OUILockon _lockon = null;
    OUILockon Lockon { get { return _lockon; } set { _lockon = value; } }
    #endregion

    #region ロックオン関連
    // ロックオンオブジェクト
    public static ObjectBase LockonObject
    {
        get { return (Instance != null && Instance.Lockon != null ? Instance.Lockon.LockonObject : null); }
    }
    // 現在のロックオンタイプ
    public static OUILockon.Type NowLockonType
    {
        get { return (Instance != null && Instance.Lockon != null ? Instance.Lockon.NowLockonType : OUILockon.Type.None); }
        set { if (Instance != null && Instance.Lockon != null) Instance.Lockon.NowLockonType = value; }
    }
    // ロックオントグル
    public static void ToggleLockonTarget()
    {
        if (Instance != null && Instance.Lockon != null) Instance.Lockon.ToggleLockonTarget();
    }
    // ロックオン範囲設定
    public static void SetLockonRange(float range)
    {
        if (Instance != null && Instance.Lockon != null) Instance.Lockon.LockonRange = range;
    }
    // ロックオンのヒットアニメーション
    public static void LockonHitAnimation()
    {
        if (Instance != null && Instance.Lockon != null) Instance.Lockon.HitAnimation();
    }

    public static void LockOnTarget(ObjectBase pObjectBase)
    {
        if (Instance != null && Instance.Lockon != null)
        {
            Instance.Lockon.LockOnTarget(pObjectBase);
        }
    }
    #endregion

    #region 初期化
    override protected void Awake()
    {
        base.Awake();

        // 表示設定
        this._SetActive(this.IsStartActive);
    }
    #endregion

    #region アクティブ設定
    public static void SetActive(bool isActive)
    {
        if (Instance == null)
            return;
        Instance._SetActive(isActive);
    }
    void _SetActive(bool isActive)
    {
        this.IsActive = isActive;

        // アニメーション開始
        this.Attach.rootTween.Play(this.IsActive);
    }
    #endregion

    #region ルート作成
    public static OUIItemRoot CreateRoot(ObjectBase o, string modelAttachName, float drawRange)
    {
        if (Instance != null)
            return OUIItemRoot.Create(Instance, o, modelAttachName, drawRange);
        return null;
    }
    #endregion

    #region ロックオン作成
    public static OUILockon CreateLockon(string modelAttachName, float lockonRange, Vector3 offset)
    {
        if (Instance != null)
        {
            // 既にロックオンアイテムが存在していたら削除する
            if (Instance.Lockon != null)
            {
                Destroy(Instance.Lockon.gameObject);
                Instance.Lockon = null;
            }

            // ロックオンアイテム作成
            Instance.Lockon = OUILockon.Create(Instance, Instance.Attach.prefab.lockon, Instance.Attach.rootObject, modelAttachName, lockonRange, offset);
            return Instance.Lockon;
        }
        return null;
    }
    #endregion

    #region 更新
    /// <summary>
    /// スクリーン上のTransformの位置をワールド座標系から更新する
    /// </summary>
    public void UpdatePosition(Transform screenTransform, Vector3 worldPosition)
    {
        // 3D空間から2D空間に位置を設定する
        {
            Vector3 screenPosition;
            GameGlobal.WorldToScreenPosition(this.WorldCamera, this.ScreenCamera, worldPosition, out screenPosition);
            screenTransform.position = new Vector3(screenPosition.x, screenPosition.y, 0f);
            screenTransform.localPosition = new Vector3(screenTransform.localPosition.x, screenTransform.localPosition.y, 0f);
        }
        // 2D空間上で位置補正
        //{
        //	Vector3 offset = screenTransform.TransformPoint(screenOffset);
        //	screenTransform.position = offset;
        //}
    }
    /// <summary>
    /// 範囲内かどうか
    /// カメラから見えているかどうかとプレイヤーからの距離が一定範囲内かどうかの判定
    /// </summary>
    public bool IsInRange(Vector3 worldPosition, float drawRange)
    {
        return IsInRange(this.WorldCamera, worldPosition, drawRange);
    }
    public static bool IsInRange(Camera camera, Vector3 worldPosition, float drawRange)
    {
        if (camera == null)
            return false;

        // 画面内かどうか
        if (!GameGlobal.IsInRange(camera.transform, worldPosition, camera.fieldOfView, camera.farClipPlane))
            return false;

        // 表示範囲が指定されていなければ表示しない
        if (drawRange <= 0f)
            return false;
        // プレイヤーとの距離で表示範囲外なら表示しない
        {
            Player p = GameController.GetPlayer();
            if (p == null)
                return false;
            float sqrDistance = Vector3.SqrMagnitude(worldPosition - p.transform.position);
            if (drawRange * drawRange < sqrDistance)
                return false;
        }

        return true;
    }
    public bool IsInRange(Vector3 worldPosition)
    {
        var c = this.WorldCamera;
        return GameGlobal.IsInRange(c.transform, worldPosition, c.fieldOfView, c.farClipPlane);
    }

    private void Update()
    {
        this.ForceToLayerBG();
    }

    private void ForceToLayerBG()
    {
        if(gameObject.layer != LayerNumber.UIBG)
        {
            gameObject.layer = LayerNumber.UIBG;
            Debug.LogError("Change!!!!!!");
            foreach(Transform t in transform)
            {
                t.SetChildLayer(LayerNumber.UIBG); 
            }
        }
    }
    #endregion
}
