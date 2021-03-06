/// <summary>
/// プレイヤーのGUI情報
/// 
/// 2014/07/22
/// </summary>

using System;
using UnityEngine;
using System.Collections;
using Scm.Common.Packet;
using Scm.Common.GameParameter;
using System.Collections.Generic;

public class GUIBattleDirectionTip : Singleton<GUIBattleDirectionTip>
{
    #region アタッチオブジェクト

    [System.Serializable]
    public class AttachObject
    {
        public Transform Root;
        public Transform ArrowPrb;
    }

    #endregion

    #region フィールド&プロパティ

    /// <summary>
    /// アタッチオブジェクト
    /// </summary>
    [SerializeField]
    private AttachObject attach;
    public AttachObject Attach { get { return attach; } }

    private Transform player;
    //    [NonSerialized]
    public Dictionary<Transform, BattleDirectionTipItem> targets = new Dictionary<Transform, BattleDirectionTipItem>();

    public void AddTarget(Transform pTransform)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        Transform go = GameObject.Instantiate(Attach.ArrowPrb);
        go.parent = Attach.Root;
        go.localScale = Vector3.one;
        go.gameObject.SetActive(true);
        targets.Add(pTransform, go.GetComponent<BattleDirectionTipItem>());
    }

    public void Remove(Transform pTransform)
    {
        if (targets.ContainsKey(pTransform))
        {
            targets[pTransform].gameObject.SetActive(false);
            targets.Remove(pTransform);
        }
    }

    private float UIDistance = 0.5f;
    private float DistanceNotShowAttack = 100f;
    private float DistanceNotShowAll = 4f;

    #endregion

    #region 初期化

    protected override void Awake()
    {
        base.Awake();
        gameObject.SetActive(false);
    }

    public static void Show(bool Active)
    {
        Instance.gameObject.SetActive(Active);
    }
    #endregion
    void Update()
    {
        if (null == player)
        {
            var playerctrl = GameController.GetPlayer();
            if (null != playerctrl)
            {
                player = playerctrl.transform;
            }
            return;
        }
        foreach (var target in targets)
        {
            DirectionTarget(target.Key, target.Value);
        }
    }

    private void DirectionTarget(Transform target, BattleDirectionTipItem ui)
    {
        if (null == target || null == ui)
        {
            return;
        }
        var playerpos = player.position;
        var targetpos = target.position;
        playerpos.y = 0;
        targetpos.y = 0;
        var forwardvector = Camera.main.transform.forward;
        var targetvector = targetpos - playerpos;

        if ((playerpos - targetpos).sqrMagnitude < DistanceNotShowAll * DistanceNotShowAll)
        {
            ui.Arrow.gameObject.SetActive(false);
            ui.Attack.gameObject.SetActive(false);
            return;
        }

        if (Vector3.Dot(forwardvector.normalized, targetvector.normalized) > 0.7)
        {
            if ((playerpos - targetpos).sqrMagnitude < DistanceNotShowAttack * DistanceNotShowAttack)
            {
                AttackTarget(target, ui);
                return;
            }
        }

        Vector3 a = WorldToUI(player.position);
        Vector3 b = WorldToUI(target.position);
        Vector3 c;
        c = a + (b - a).normalized * UIDistance;
        ui.Arrow.LookAt(b);
        ui.Arrow.position = c;
        ui.Arrow.gameObject.SetActive(true);
        ui.Attack.gameObject.SetActive(false);
    }

    private void AttackTarget(Transform target, BattleDirectionTipItem ui)
    {
        Vector3 a = WorldToUI(target.position);
        ui.Attack.position = a;
        ui.Arrow.gameObject.SetActive(false);
        ui.Attack.gameObject.SetActive(true);
    }

    private Vector3 WorldToUI(Vector3 point)
    {
        try
        {
            if (null == Camera.main || null == UICamera.currentCamera)
            {
                return Vector3.zero;
            }
            Vector3 pt = Camera.main.WorldToScreenPoint(point);
            Vector3 ff = UICamera.currentCamera.ScreenToWorldPoint(pt);
            if (ff.z < 0)
            {
                ff = -ff;
                ff.z = 0;
            }
            else
            {
                ff.z = 0;
            }
            return ff;
        }
        catch (Exception e)
        {
            Debug.LogError("===> " + e);
            //            throw;
        }
        return Vector3.zero;
    }
}
