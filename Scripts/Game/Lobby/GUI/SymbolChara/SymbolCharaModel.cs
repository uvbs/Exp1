/// <summary>
/// シンボルキャラクターデータ
/// 
/// 2016/03/28
/// </summary>

using System;

namespace XUI.SymbolChara {

	/// <summary>
	/// シンボルキャラクターデータインターフェイス
	/// </summary>
	public interface IModel {

		string SymbolNameFormat { get; set; }
		string SelectNameFormat { get; set; }
	}

	/// <summary>
	/// シンボルキャラクターデータ
	/// </summary>
	public class Model : IModel {

		private string symbolNameFormat = "";
		public string SymbolNameFormat { get { return symbolNameFormat; } set { symbolNameFormat = value; } }

		private string selectNameFormat = "";
		public string SelectNameFormat { get { return selectNameFormat; } set { selectNameFormat = value; } }
	}
}
