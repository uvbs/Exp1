/// <summary>
/// 戦略ゲージ
/// 共通部分
/// 
/// 2014/07/18
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Master;

using Scm.Common.GameParameter;

namespace TacticalGauge
{
	namespace TGUICommon
	{
		/// <summary>
		/// マネージャークラス
		/// </summary>
		[System.Serializable]
		public class Manager
		{
			#region フィールド＆プロパティ
			const string PlayerLabel = "我方";
			const string EnemyLabel = "敌方";

			/// <summary>
			/// 戦闘終了タイマーフォーマット
			/// </summary>
			[SerializeField]
			string _timeFormat = "{0:D2}:{1:D2}";
			string TimeFormat { get { return _timeFormat; } }

			/// <summary>
			/// アタッチオブジェクト
			/// </summary>
			[SerializeField]
			AttachObject _attach;
			AttachObject Attach { get { return _attach; } }
			[System.Serializable]
			public class AttachObject
			{
				[SerializeField]
				GameObject _root;
				public GameObject Root { get { return _root; } }

				[SerializeField]
				UILabel _timeLabel;
				public UILabel TimeLabel { get { return _timeLabel; } }

                [SerializeField]
                UILabel _roundTimeLabel;
                public UILabel RoundTimeLabel { get { return _roundTimeLabel; } }

				[SerializeField]
				UILabel _playerLabel;
				public UILabel PlayerLabel { get { return _playerLabel; } }

				[SerializeField]
				UILabel _enemyLabel;
				public UILabel EnemyLabel { get { return _enemyLabel; } }
			}

			// 残り時間
			float RemainingTime { get; set; }
            // Round's remaining time
            float RoundRemainingTime { get; set; }
			// タイマーを止める
			public bool IsStopTimer { get; set; }

			// シリアライズされていないメンバー初期化
			void MemberInit()
			{
				this.RemainingTime = 0f;
				this.IsStopTimer = false;
			}
			#endregion

			#region 初期化
			/// <summary>
			/// クリア
			/// </summary>
			public void Clear()
			{
				this.MemberInit();
				this.SetRemainingTime(0f, 0f);
			}
			#endregion

			#region アクティブ化
			/// <summary>
			/// アクティブ化
			/// </summary>
			public void SetActive(bool isActive)
			{
				var t = this.Attach;
				if (t.Root != null)
					t.Root.SetActive(isActive);
			}
			#endregion

			#region 更新
			/// <summary>
			/// 更新
			/// </summary>
			public void Update()
			{
				this.UpdateRemainingTime();
			}
			#endregion

			#region バトルタイプ設定
			/// <summary>
			/// バトルタイプ設定
			/// </summary>
			public void SetBattleType(BattleType battleType)
			{
				string playerText = PlayerLabel;
				string enemyText = EnemyLabel;
				switch (battleType)
				{
				case BattleType.BugSlayer:
					playerText = "";
					break;
				}

				// UI更新
				var t = this.Attach;
				if (t.PlayerLabel != null)
					t.PlayerLabel.text = playerText;
				if (t.EnemyLabel != null)
					t.EnemyLabel.text = enemyText;
			}
			#endregion

			#region 残り時間
			/// <summary>
			/// 残り時間更新
			/// </summary>
			void UpdateRemainingTime()
			{
				if (this.IsStopTimer)
					return;

				// 残り時間カウンター
				this.RemainingTime -= Time.deltaTime;
				this.RemainingTime = Mathf.Max(0f, this.RemainingTime);

                this.RoundRemainingTime -= Time.deltaTime;
                this.RoundRemainingTime = Mathf.Max(0f, this.RoundRemainingTime);

                // UI更新
                this.SetRemainingTimeUI((int)this.RemainingTime, (int)this.RoundRemainingTime);
			}
			/// <summary>
			/// 残り時間設定
			/// </summary>
			public void SetRemainingTime(float remainingTime, float roundRemainingTime, bool isStopTimer)
			{
				this.IsStopTimer = isStopTimer;

				this.SetRemainingTime(remainingTime, roundRemainingTime);
			}
			/// <summary>
			/// 残り時間設定
			/// </summary>
			public void SetRemainingTime(float remainingTime, float roundRemaingTime)
			{
				this.RemainingTime = remainingTime;
                this.RoundRemainingTime = roundRemaingTime;
				// UI更新
				this.SetRemainingTimeUI((int)remainingTime, (int)roundRemaingTime);
			}
			/// <summary>
			/// 残り時間UIの更新
			/// </summary>
			void SetRemainingTimeUI(int second, int roundRemainingTime)
			{
				int min = second / 60;
				int sec = second % 60;

				// UI更新
				var t = this.Attach;
				if (t.TimeLabel != null)
					t.TimeLabel.text = string.Format(this.TimeFormat, min, sec);

			    if (t.RoundTimeLabel != null)
			    {
			        if (0 == roundRemainingTime)
			        {
                        t.RoundTimeLabel.text = "";
                        return;
			        }
                    t.RoundTimeLabel.gameObject.SetActive(true);
			        min = roundRemainingTime/60;
			        sec = roundRemainingTime%60;
			        t.RoundTimeLabel.text = string.Format(this.TimeFormat, min, sec);
			    }
			}
			#endregion
		}
	}
}