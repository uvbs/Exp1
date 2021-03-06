/// <summary>
/// 戦略ゲージ
/// Resident Area
/// 
/// 2014/07/22
/// </summary>
using UnityEngine;
using System.Collections;

namespace TacticalGauge
{
	namespace TGUIResidentArea
	{
		/// <summary>
		/// マネージャークラス
		/// </summary>
		[System.Serializable]
		public class Manager
		{
			#region フィールド＆プロパティ
			
			/// <summary>
			/// アタッチオブジェクト
			/// </summary>
			[SerializeField]
			AttachObject _myTeam;
			public AttachObject MyTeam { get { return _myTeam; } }
            [SerializeField]
            AttachObject _enemy;
            public AttachObject Enemy { get { return _enemy; } }

            [System.Serializable]
			public class AttachObject
			{
				public GameObject root;
				public UILabel gaugeLabel;
                public UISlider standBySlider;
			}

            [SerializeField]
            RootAttackObject _attach;
            public RootAttackObject Attach { get { return _attach; } }
            [System.Serializable]
            public class RootAttackObject 
            {
                public GameObject root;
            }

            private int _roundIndex = -1;
            public int RoundIndex {
                get {
                    return _roundIndex;
                }
                set {
                    if (_roundIndex != value) {
                        _roundIndex = value;
                        RoundIndexChanged();
                    }
                }
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
				this.SetRemainingPoint(true, 0, 0, 0);
                this.SetRemainingPoint(false, 0, 0, 0);
            }
			#endregion

			#region 残りポイント
			public void SetRemainingPoint(bool isMyTeam, int remain, int total, int roundIndex)
			{
			    if (isMyTeam && MyTeam != null) {
                    MyTeam.gaugeLabel.text = remain.ToString("00");
                    //MyTeam.standBySlider.value = standBy / 100.0f;
                }
                if ((!isMyTeam) && Enemy != null) {
                    Enemy.gaugeLabel.text = remain.ToString("00");
                    //Enemy.standBySlider.value = standBy / 100.0f;
                }
                RoundIndex = roundIndex;
                ResidentArea.OnActiveRefresh();
            }
			#endregion

            private void RoundIndexChanged() {

            }
        }
	}
}