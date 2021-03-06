/// <summary>
/// ロゴ処理
/// 
/// 2015/11/13
/// </summary>
using UnityEngine;
using System;
using System.Collections.Generic;

namespace XUI.Logo
{
	/// <summary>
	/// ロゴデータ群インターフェイス
	/// </summary>
	public interface IModels
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();
		#endregion

		#region データ操作
		/// <summary>
		/// データの数
		/// </summary>
		int Count { get; }

		/// <summary>
		/// データ追加
		/// </summary>
		void Add(IModel model);

		/// <summary>
		/// 表示データ取得
		/// </summary>
		bool TryGetDrawData(int index, out IModel model);

		/// <summary>
		/// 各データに対する操作
		/// </summary>
		void ForEach(System.Action<IModel> action);
		#endregion
	}

	/// <summary>
	/// ロゴデータ群
	/// </summary>
	public class Models : IModels
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
		}
		#endregion

		#region データ操作
		// 表示リスト
		List<IModel> _drawList = new List<IModel>();
		List<IModel> DrawList { get { return _drawList; } set { _drawList = value; } }

		// データの数
		public int Count { get { return this.DrawList.Count; } }

		/// <summary>
		/// データ追加
		/// </summary>
		public void Add(IModel model)
		{
			this.DrawList.Add(model);
		}

		/// <summary>
		/// 表示データ取得
		/// </summary>
		public bool TryGetDrawData(int index, out IModel model)
		{
			model = null;
			if (0 > index) return false;
			if (index >= this.DrawList.Count) return false;

			model = this.DrawList[index];
			return true;
		}

		/// <summary>
		/// 各データに対する操作
		/// </summary>
		public void ForEach(System.Action<IModel> action)
		{
			this.DrawList.ForEach(action);
		}
		#endregion
	}



	/// <summary>
	/// ロゴデータインターフェイス
	/// </summary>
	public interface IModel
	{
		#region データ
		/// <summary>
		/// スキップできるかどうか
		/// </summary>
		bool CanSkip { get; }

		/// <summary>
		/// フェードが終わっても残留しているかどうか
		/// </summary>
		bool IsStay { get; }

		/// <summary>
		/// フェード時間
		/// </summary>
		float FadeTime { get; }

		/// <summary>
		/// フェードタイプ
		/// </summary>
		FadeType FadeType { get; }

		/// <summary>
		/// 表示したいロゴデータ
		/// </summary>
		UITweener Tween { get; }
		#endregion
	}

	/// <summary>
	/// ロゴデータ
	/// </summary>
	[System.Serializable]
	public class Model : IModel
	{
		#region データ
		// スキップできるかどうか
		[SerializeField]
		bool _canSkip = true;
		public bool CanSkip { get { return _canSkip; } }

		/// <summary>
		/// フェードが終わっても残留しているかどうか
		/// </summary>
		[SerializeField]
		bool _isStay = false;
		public bool IsStay { get { return _isStay; } }

		// フェード時間
		[SerializeField]
		float _fadeTime = 0f;
		public float FadeTime { get { return _fadeTime; } }

		/// <summary>
		/// フェードタイプ
		/// </summary>
		[SerializeField]
		FadeType _fadeType = FadeType.InOut;
		public FadeType FadeType { get { return _fadeType; } }

		// 表示したいロゴデータ
		[SerializeField]
		UITweener _tween = null;
		public UITweener Tween { get { return _tween; } }
		#endregion
	}
}
