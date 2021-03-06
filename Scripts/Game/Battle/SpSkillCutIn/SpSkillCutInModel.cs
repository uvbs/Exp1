/// <summary>
/// 必殺技カットインデータ
/// 
/// 2015/12/03
/// </summary>
using UnityEngine;
using System.Collections;

namespace XUI
{
	namespace SpSkillCutIn
	{
		/// <summary>
		/// 必殺技カットインデータインターフェイス
		/// </summary>
		public interface IModel
		{
			/// <summary>
			/// 演出時間
			/// </summary>
			float PlayTime { get; }
		}

		/// <summary>
		/// 必殺技カットインデータ
		/// </summary>
		[System.Serializable]
		public class Model : IModel
		{
			#region フィールド&プロパティ
			/// <summary>
			/// 演出時間
			/// </summary>
			[SerializeField]
			private float playTime = 0;
			public float PlayTime { get { return playTime; } }
			#endregion
		}
	}
}