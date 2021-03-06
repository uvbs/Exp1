/// <summary>
/// マップデータ参照
/// 
/// 2013/08/13
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReferenceMapData : ReferenceData
{
	#region 宣言
	/// <summary>
	/// 設定
	/// </summary>
	[System.Serializable]
	public class Configuration
	{
		public LightProbes lightProbes;
		public List<Texture2D> lightmaps = new List<Texture2D>();
		public bool fog;
		public Color fogColor;
		public FogMode fogMode;
		public float fogDensity;
		public float linearFogStart;
		public float linearFogEnd;
		public Color ambientLight;
	}
	#endregion

	#region フィールド＆プロパティ
	[SerializeField]
	private Configuration config;
	public Configuration Config { get { return config; } private set { config = value; } }
	#endregion

	#region Method
	protected override void Setup(GameObject go, Transform attach)
	{
		base.Setup(go, attach);
		this.SetupMap(go);
	}
	#endregion

	#region Method
	void SetupMap(GameObject go)
	{
		go.transform.parent = this.transform;
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.identity;
		go.transform.localScale = Vector3.one;

		// ライトプローブ
		LightmapSettings.lightProbes = this.Config.lightProbes;
		// ライトマップ
		LightmapData[] lightmaps = LightmapSettings.lightmaps;
		int lightmapNum = this.Config.lightmaps.Count;
		System.Array.Resize(ref lightmaps, lightmapNum);
		for(int i=0; i<lightmapNum; i++)
		{
			lightmaps[i] = new LightmapData();
			lightmaps[i].lightmapFar = this.Config.lightmaps[i];
		}
		LightmapSettings.lightmaps = lightmaps;
		// フォグ
		RenderSettings.fog = this.Config.fog;
		RenderSettings.fogColor = this.Config.fogColor;
		RenderSettings.fogMode = this.Config.fogMode;
		RenderSettings.fogDensity = this.Config.fogDensity;
		RenderSettings.fogStartDistance = this.Config.linearFogStart;
		RenderSettings.fogEndDistance = this.Config.linearFogEnd;
		// アンビエントライト
		RenderSettings.ambientLight = this.Config.ambientLight;
	}
	#endregion
}
