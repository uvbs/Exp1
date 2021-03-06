/// <summary>
/// 戦略ゲージ
/// タワー戦
/// 
/// 2014/07/18
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.GameParameter;

namespace TacticalGauge
{
	namespace TGUITowerBattle
	{
		/// <summary>
		/// マネージャークラス
		/// </summary>
		[System.Serializable]
		public class Manager
		{
			public const float TeamSkillMaxPoint = 100f;

			#region フィールド＆プロパティ
			/// <summary>
			/// サブタワーの識別IDの開始値
			/// </summary>
			[SerializeField]
			int _subTowerStartTacticalID = 1;
			int SubTowerStartTacticalID { get { return _subTowerStartTacticalID; } }

			/// <summary>
			/// チームスキル発動までの残り人数の最大値
			/// </summary>
			[SerializeField]
			int _maxTeamSkillCount = 99;
			int MaxTeamSkillCount { get { return _maxTeamSkillCount; } }

			/// <summary>
			/// チームスキル発動までの残り人数の追加エフェクトを表示するための人数
			/// </summary>
			[SerializeField]
			int _addEffectTeamSkillCount = 3;
			int AddEffectTeamSkillCount { get { return _addEffectTeamSkillCount; } }

			/// <summary>
			/// アタッチオブジェクト
			/// </summary>
			[SerializeField]
			AttachObject _attach;
			public AttachObject Attach { get { return _attach; } }
			[System.Serializable]
			public class AttachObject
			{
				public GameObject root;
				public GameObject panel3DRoot;
			}

			/// <summary>
			/// 味方情報
			/// </summary>
			[SerializeField]
			TeamInfo _myteam;
			TeamInfo Myteam { get { return _myteam; } }

			/// <summary>
			/// 敵情報
			/// </summary>
			[SerializeField]
			TeamInfo _enemy;
			TeamInfo Enemy { get { return _enemy; } }

			public int MaxMyteamSideGauge { get { return this.Myteam.Total; } }
			public int NowMyteamSideGauge { get { return this.Myteam.Remain; } }
			public int MaxEnemySideGauge { get { return this.Enemy.Total; } }
			public int NowEnemySideGauge { get { return this.Enemy.Remain; } }

			#endregion

			#region 初期化
			public void Clear()
			{
				this.Myteam.SetTeamSkillPoint(0);
				this.Enemy.SetTeamSkillPoint(0);
			}
			#endregion

			#region タワーゲージ
			public GUITowerGauge GetTowerGauge(bool isMyteam, EntrantType entrantType, int tacticalID)
			{
				// UIからタワーゲージ取得
				GUITowerGauge towerGauge = null;
				{
					TeamInfo.AttachObject t = (isMyteam ? this.Myteam.Attach : this.Enemy.Attach);
					// メインタワーかどうか
					if (entrantType == EntrantType.MainTower)
					{
						towerGauge = t.mainTowerGauge;
					}
					//// サブタワーかどうか
					//else if (entrantType == EntrantType.SubTower)
					//{
					//    int index = tacticalID - this.SubTowerStartTacticalID;
					//    if (0 <= index && index < t.subTowerGaugeList.Count)
					//    {
					//        towerGauge = t.subTowerGaugeList[index];
					//    }
					//}
				}
				return towerGauge;
			}
			#endregion

			#region チームスキル発動カウント
			public int GetSideGaugeCount( TeamTypeClient team )
			{
				int remaing = 0;

				if(team == TeamTypeClient.Friend)
				{
					remaing = this.Myteam.GetSideGaugeCount();
				}
				else
				{
					remaing = this.Enemy.GetSideGaugeCount();
				}

				return remaing;
			}
			#endregion
			#region チームスキルポイント
			public void SetTeamSkillPoint(bool isMyteam, float point)
			{
				if (isMyteam)
					this.Myteam.SetTeamSkillPoint(point);
				else
					this.Enemy.SetTeamSkillPoint(point);
			}
			/// <summary>
			/// 正規化する前のポイントセット
			/// </summary>
			/// <param name="isMyteam"></param>
			/// <param name="point"></param>
			public void SetRawTeamSkillPoint(bool isMyteam, byte point)
			{
				if (isMyteam)
					this.Myteam.SetRawTeamSkillPoint(point);
				else
					this.Enemy.SetRawTeamSkillPoint( point);
			}
			#endregion

			#region タワー戦残りポイント
			public void SetRemainingPoint(bool isMyteam, int remain, int total)
			{
				if (isMyteam)
					this.Myteam.SetRemainingPoint(remain, total);
				else
					this.Enemy.SetRemainingPoint(remain, total);
			}
			#endregion

			#region デバッグ
#if XW_DEBUG
			public void DebugUpdateTowerGauge(bool isMyteam, float mainTower, List<float> subTowerList)
			{
				// メインタワー
				{
					var t = this.GetTowerGauge(isMyteam, EntrantType.MainTower, 0);
					if (t != null)
					{
						t.SetActive(false);
						if (0 <= mainTower)
						{
							t.UpdateGauge((int)(mainTower * 100), 100);
							t.ShakeGauge();
						}
					}
				}
				// サブタワー
				{
					{
						// 非アクティブ化
						var team = (isMyteam ? this.Myteam : this.Enemy);
						foreach (var t in team.Attach.subTowerGaugeList)
							t.SetActive(false);
					}
					for (int i = 0, max = subTowerList.Count; i < max; i++)
					{
						var gauge = subTowerList[i];
						var t = this.GetTowerGauge(isMyteam, EntrantType.SubTower, this.SubTowerStartTacticalID + i);
						if (t != null)
							t.UpdateGauge((int)(gauge * 100), 100);
					}
				}
			}
#endif
			#endregion
		}

		/// <summary>
		/// チーム情報クラス
		/// </summary>
		[System.Serializable]
		public class TeamInfo
		{
			#region フィールド＆プロパティ
			// チームスキル発動カウントのエフェクト開始のカウント
			static readonly int SideGaugeCountEffectStartCount = 3;


			/// <summary>
			/// アタッチオブジェクト
			/// </summary>
			[SerializeField]
			AttachObject _attach;
			public AttachObject Attach { get { return _attach; } }
			[System.Serializable]
			public class AttachObject
			{
				// メインタワーゲージ
				public GUITowerGauge mainTowerGauge;
				// サブタワーゲージリスト
				public List<GUITowerGauge> subTowerGaugeList = new List<GUITowerGauge>();

				// チームスキル
				public TeamSkill teamSkill;
				[System.Serializable]
				public class TeamSkill
				{
					// 残り人数ラベル
					public UILabel countLabel;
					// 演出
					public GameObject addEffectGroup;
					// アニメーション
					public UITweener addEffectTween;

					// パーティクルリンクリスト
					public List<ParticleLink> particleList = new List<ParticleLink>();

					// 切り替えグループリスト
					public List<ActiveGroup> activeGroupList = new List<ActiveGroup>();
					[System.Serializable]
					public class ActiveGroup
					{
						public float percent;	// root を表示するパーセンテージ
						public GameObject root;
					}
				}
			}

			// チームスキルポイント
			float TeamSkillPoint { get; set; }

			// 正規化する前のポイント
			public byte RawTeamSkillPoint{get; private set;}

			// フィル値
			public float FillAmout { get { return (Total <= 0 ? 0f : (float)Remain / (float)Total); } }
			// 残りゲージ
			public int Remain { get;  private set; }
			// 合計ゲージ
			public int Total { get; private set; }

			// シリアライズされていないメンバーを初期化
			void MemberInit()
			{
				this.TeamSkillPoint = 0f;
			}
			#endregion

			#region 初期化
			public TeamInfo() { this.MemberInit(); }
			#endregion

			#region チームスキルポイント
			public void SetTeamSkillPoint(float point)
			{
				this.TeamSkillPoint = point;
				try
				{
					var ts = this.Attach.teamSkill;

					// パーティクルを段階ごとに設定する
					foreach (var t in ts.particleList)
					{
						if(t != null)
						{
							t.Update(point);
						}
					}
					// 設定されているパーセンテージより下のものは表示する
					foreach (var t in ts.activeGroupList)
					{
						if(t != null)
						{
							bool isActive = (t.percent <= point);
							t.root.SetActive(isActive);
						}
					}
				}
				catch (System.Exception)
				{
				}
			}
			/// <summary>
			/// 正規化前のポイントセット
			/// </summary>
			/// <param name="point"></param>
			public void SetRawTeamSkillPoint( byte point )
			{
				this.RawTeamSkillPoint = point;
				// ポイントに変動があったのでカウントを更新する
				int count = GetSideGaugeCount();
				this.Attach.teamSkill.countLabel.text = count.ToString();

				ChangeSigeGaugeCountEffect(count);

			}
			#endregion

			#region タワー戦残りポイント
			public void SetRemainingPoint(int remain, int total)
			{
				this.Remain = remain;
				this.Total = total;

				// UIに反映する
				try
				{
					this.Attach.mainTowerGauge.UpdateGauge(remain,total);
					int count = GetSideGaugeCount();
					this.Attach.teamSkill.countLabel.text = count.ToString();

					ChangeSigeGaugeCountEffect(count);

					//this.Attach.mainTowerGauge.Attach.gaugeSprite.fillAmount = this.FillAmout;
					//this.Attach.scoreLabel.text = string.Format(format, remain, total);
				}
				catch (System.Exception)
				{
				}
			}
			#endregion

			#region チームスキル発動までの残りカウント計算
			public int GetSideGaugeCount()
			{
				if( this.Total == 0 )
					return 0;

				//Debug.Log(string.Format("Total:{0} Remain:{1} Point:{2}",this.Total,this.Remain,this.RawTeamSkillPoint));
				return Scm.Common.Utility.TeamSkill.GetBreakRemain(this.Total,this.Remain,this.RawTeamSkillPoint);
			}
			#endregion

			#region チームスキル発動カウントのエフェクト変更
			void ChangeSigeGaugeCountEffect( int count )
			{
				if( count <= SideGaugeCountEffectStartCount )
				{
					this.Attach.teamSkill.addEffectTween.PlayForward();
					foreach( Transform trans in this.Attach.teamSkill.addEffectGroup.transform )
					{
						trans.gameObject.SetActive(true);
					}
				}
				else
				{
					this.Attach.teamSkill.addEffectTween.PlayReverse();
					foreach( Transform trans in this.Attach.teamSkill.addEffectGroup.transform )
					{
						trans.gameObject.SetActive(false);
					}
				}
			}
			#endregion 
		}

		/// <summary>
		/// パーティクル切り替えシステム
		/// </summary>
		[System.Serializable]
		public class ParticleLink
		{
			#region フィールド＆プロパティ
			/// <summary>
			/// パーティクル
			/// </summary>
			[SerializeField]
			ParticleSystem _particle;
			ParticleSystem Particle { get { return _particle; } }

			/// <summary>
			/// 設定リスト
			/// </summary>
			[SerializeField]
			List<Setting> _settingList;
			List<Setting> SettingList { get { return _settingList; } }
			[System.Serializable]
			public class Setting
			{
				public float percent;
				public float size;
				public Color color;
			}

			// デフォルト設定
			Setting DefaultSetting { get; set; }

			// シリアライズされていないメンバー初期化
			void MemberInit()
			{
				this.DefaultSetting = new Setting()
				{
					percent = 0f,
					size = 0.001f,
					color = new Color(0f, 0f, 0f, 0f),
				};
			}
			#endregion

			#region 初期化
			public ParticleLink() { this.MemberInit(); }
			#endregion

			#region 設定
			public void Update(float percent)
			{
				Setting now = this.DefaultSetting;
				Setting next = null;

				// リストの中から percent の位置の前後の設定を取得する
				foreach (var t in this.SettingList)
				{
					next = t;
					if (next.percent > percent)
						break;
					now = next;
				}
				if (next == null)
					return;

				// パーティクルの設定
				{
					float nowPercent = percent - now.percent;
					float maxPercent = next.percent - now.percent;
					float lerp = (maxPercent > 0f ? nowPercent / maxPercent : 0f);
					this.Particle.startSize = Mathf.Lerp(now.size, next.size, lerp);
					this.Particle.startColor = now.color;
				}
			}
			#endregion
		}
	}
}