/// <summary>
/// メインタワー
/// 
/// 2013/03/18
/// </summary>
using UnityEngine;
using System.Collections;

using Scm.Common.GameParameter;
using Scm.Common.Master;

public class MainTower : TowerBase, Turret.ITurretCarrier
{
	protected override string AnimationDamage { get { return "mainTower_damage"; } }
	protected override string AnimationDestroy { get { return ""; } }

	// 砲台オブジェクト
	public Turret Turret { get; set; }

	/// <summary>
	/// 体力が半分を超えたか.
	/// </summary>
	private bool isOverHalfHp = false;

	#region ObjectBase Override

	#region 初期化
	protected override void Setup(Manager manager, ObjectMasterData objectData, EntrantInfo info, AssetReference assetReference)
	{
		base.Setup(manager, objectData, info, assetReference);

		// UNDONE:TeamSkillPointでサイドゲージを使うようになったので一時的な対処
		// タワー戦情報
		// チームスキルポイントを取得する
		BattlePacket.SendTeamSkillPoint();
	}
	#endregion

	#region 移動パケット
	public override void Move(Vector3 oldPosition, Vector3 position, Quaternion rotation, bool forceGrounded)
	{
		this.MoveBase(position, rotation);

		this.transform.position = position;
		this.NextPosition = position;

		// 本体は回転しない.
		this.transform.rotation = NextRotation;
	}
	#endregion

	#region スキルモーションパケット
	public override bool SkillMotion(int skillID, ObjectBase target, Vector3 position, Quaternion rotation)
	{
		if (Turret != null)
		{
			Turret.SetRotation(rotation, 0.5f);
		}
		return base.SkillMotion(skillID, target, position, rotation);
	}
	#endregion

	#region ヒットパケット
	public override void Hit(HitInfo hitInfo)
	{
		base.HitTowerBase(hitInfo);

		// 画面エフェクト
		// ダメージ値が0の場合(回復等)はエフェクトの処理を行わない
		if (0 < hitInfo.damage)
		{
			Player player = GameController.GetPlayer();
			if (player)
			{
				if (player.TeamType == this.TeamType)
				{
					//TODO: メインタワーに直接ダメージが入った場合のみメッセージを表示させる
					// 現在は敵プレイヤーを倒した時もメインタワーのHitパケットが飛んでくるため攻撃者のInFieldIdが自分自身かどうか判定をいれている
					if(this.InFieldId != hitInfo.inFieldAttackerId)
					{
						if (!isOverHalfHp && base.HitPoint <= (base.MaxHitPoint / 2f))
						{
							// [メインタワーHP50%]メッセージ
							GUIEffectMessage.SetTacticalWarning(GUITacticalMessageItem.TacticalType.MainTowerHalfDamege);
							isOverHalfHp = true;
						}
						// [メインタワー攻撃]メッセージ表示
						GUIEffectMessage.SetTacticalWarning(GUITacticalMessageItem.TacticalType.MainTowerAtk);
					}
				}
			}

			// HPゲージを揺らす演出.
			this.TowerGauge.ShakeGauge();
		}
	}
	#endregion
	#endregion
}
