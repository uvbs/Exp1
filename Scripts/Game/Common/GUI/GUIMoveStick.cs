/// <summary>
/// 移動スティック
/// 
/// 2013/01/17
/// </summary>
using UnityEngine;
using System.Collections;

public class GUIMoveStick : Singleton<GUIMoveStick>
{
    #region フィールド＆プロパティ
    /// <summary>
    /// 初期化時のアクティブ状態
    /// </summary>
    [SerializeField]
    bool _isStartActive = false;
    public bool IsStartActive { get { return _isStartActive; } }

    /// <summary>
    /// スティックが移動できる範囲（半径）
    /// </summary>
    [SerializeField]
    float _maxRadius = -1f;
    public float MaxRadius { get { return _maxRadius; } }

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
        public UISprite stickSprite;
        public UIButton button;
        public Transform tutorialTarget;
        public UISprite stickRangeSprite;
    }

    /// <summary>
    /// ボタンの有効設定
    /// </summary>
    public static bool IsStickEnable
    {
        get { return (Instance != null ? Instance._attach.button.isEnabled : false); }
        set { if (Instance != null) Instance._attach.button.isEnabled = value; }
    }

    /// <summary>
    /// スティックが移動しているかどうか
    /// </summary>
    public static bool IsMove
    {
        get
        {
            if (Instance == null)
                return false;
            if (Instance._stickMove.sqrMagnitude <= 0f)
                return false;
            return true;
        }
    }

    /// <summary>
    /// 前回からのスティックの移動距離の比率(0.0～1.0)
    /// </summary>
    public static Vector2 Delta
    {
        get
        {
            if (Instance == null)
                return Vector2.zero;

            if (!IsStickEnable)
                return Vector2.zero;

            return Instance._delta;
        }
    }
    Vector2 _delta = Vector2.zero;

    /// <summary>
    /// スティックの実際の移動距離
    /// </summary>
    public static Vector2 StickMode
    {
        get
        {
            if (Instance == null)
                return Vector2.zero;
            return Instance._stickMove;
        }
    }
    Vector2 _stickMove = Vector2.zero;

#if UNITY_EDITOR || UNITY_STANDALONE
    public static bool IsDrag
    {
        get
        {
            if (Instance == null)
                return false;
            return Instance._isDrag;
        }
    }
    bool _isDrag = false;
#endif

    public bool IsActive { get; private set; }
    #endregion

    #region 初期化
    override protected void Awake()
    {
        base.Awake();

        // 範囲が設定されていなければスプライトの高さから算出する
        if (0f > this.MaxRadius)
            this._maxRadius = this.Attach.stickSprite.height * 0.5f;
        // 初期化
        this.OnReset();

        // 表示設定
        this._SetActive(this.IsStartActive);
        
        this.Attach.stickRangeSprite.gameObject.SetActive(false);
    }
    #endregion

    #region アクティブ設定
    public static void SetActive(bool isActive)
    {
        if (Instance != null) Instance._SetActive(isActive);
    }
    public void _SetActive(bool isActive)
    {
        this.IsActive = isActive;

        // アニメーション開始
        this.Attach.rootTween.Play(this.IsActive);
    }
    #endregion

    #region 設定
    public static void Reset()
    {
        if (Instance != null) Instance.OnReset();
    }
    public void OnReset()
    {
        this._delta = Vector2.zero;
        this._stickMove = Vector2.zero;
        this.Attach.stickSprite.transform.localPosition = new Vector3(this._stickMove.x, this._stickMove.y, 0f);
    }
    public static void Drag(Vector2 delta)
    {
        if (Instance != null) Instance.OnDrag(delta);
    }
    public void OnDrag(Vector2 delta)
    {
        this._stickMove += delta;
        if (this._stickMove.sqrMagnitude > this.MaxRadius * this.MaxRadius)
        {
            // なぜか Normalize() だと正規化できない
            //this._stickMove.Normalize();
            this._stickMove = this._stickMove.normalized;
            this._stickMove *= this.MaxRadius;
        }
        this.Attach.stickSprite.transform.localPosition = new Vector3(this._stickMove.x, this._stickMove.y, 0f);
        if (0f < this.MaxRadius)
            this._delta = this._stickMove / this.MaxRadius;
        else
            this._delta = Vector2.zero;
    }
    public void OnDragStart()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        this._isDrag = true;
#endif
        Attach.stickRangeSprite.gameObject.SetActive(true);
        this.OnReset();
    }
    public void OnDragEnd()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        this._isDrag = false;
#endif
        Attach.stickRangeSprite.gameObject.SetActive(false);
        this.OnReset();
    }
    #endregion
}
