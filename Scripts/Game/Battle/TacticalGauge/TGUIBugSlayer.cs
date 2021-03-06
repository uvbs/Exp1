/// <summary>
/// 戦略ゲージ
/// バグスレイヤー
/// 
/// 2014/07/22
/// </summary>
using UnityEngine;
using System.Collections;

namespace TacticalGauge
{
	namespace TGUIBugSlayer
	{
		/// <summary>
		/// マネージャークラス
		/// </summary>
		[System.Serializable]
		public class Manager
		{
			#region フィールド＆プロパティ
			/// <summary>
			/// スコアフォーマット
			/// </summary>
			[SerializeField]
			string _scoreFormat = "{0}/{1}";
			string ScoreFormat { get { return _scoreFormat; } }

            /// <summary>
			/// アタッチオブジェクト
			/// </summary>
			[SerializeField]
            AttachObject _myTeam;
            public AttachObject MyTeam { get { return _myTeam; } }
            [SerializeField]
            AttachObject _enemy;
            public AttachObject Enemy { get { return _enemy; } }

            /// <summary>
            /// アタッチオブジェクト
            /// </summary>
            [SerializeField]
            RootAttachObject _attach;
			public RootAttachObject Attach { get { return _attach; } }
			[System.Serializable]
			public class AttachObject
			{
				public UISprite gaugeSprite;
				public UILabel scoreLabel;
			}
            [System.Serializable]
            public class RootAttachObject {
                public GameObject root;
            }


			// メンバー初期化
			void MemberInit()
			{

			}
			#endregion

			#region 初期化
			public void Clear()
			{
				this.MemberInit();
				this.SetRemainingPoint(true, 0, 0);
                this.SetRemainingPoint(false, 0, 0);
            }
			#endregion

			#region 残りポイント
			public void SetRemainingPoint(bool isMyTeam, int remain, int total)
			{

                // UIに反映する
                UISprite sprite = isMyTeam ? MyTeam.gaugeSprite : Enemy.gaugeSprite;
                UILabel label = isMyTeam ? MyTeam.scoreLabel : Enemy.scoreLabel;
				if(sprite != null)
				{
                    sprite.fillAmount = GetFillAmount(remain, total);
				}
				if(label != null)
				{
                    label.text = string.Format(this.ScoreFormat, remain, total);
				}
			}
			#endregion

            private float GetFillAmount(int remain, int total) {
                return (total <= 0 ? 0f : (float)remain / (float)total);
            }
		}
	}
}