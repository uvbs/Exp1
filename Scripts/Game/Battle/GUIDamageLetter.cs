using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;

public class GUIDamageLetter : MonoBehaviour
{
	#region Inspector用クラス定義
	#region Inspector用クラス
	[System.SerializableAttribute]
	abstract class DamageLetterParam
	{
		[SerializeField]
		private float scale = 50;
		public float Scale { get { return scale; } }

		[SerializeField]
		private TextParamColor normal;
		public TextParamColor Normal { get { return normal; } }

		[SerializeField]
		private TextParamColor magic;
		public TextParamColor Magic { get { return magic; } }

		[SerializeField]
		private TextParamColor recovery;
		public TextParamColor Recovery { get { return recovery; } }

		[SerializeField]
		private TextParamEffect critical;
		public TextParamEffect Critical { get { return critical; } }

		[SerializeField]
		private TextParamEffect guard;
		public TextParamEffect Guard { get { return guard; } }
	}
	[System.SerializableAttribute]
	class PlayerDamageLetterParam : DamageLetterParam
	{
		[SerializeField]
		private PlayerActionParam actionParam;
		public PlayerActionParam ActionParam { get { return actionParam; } }
	}
	[System.SerializableAttribute]
	class OtherDamageLetterParam : DamageLetterParam
	{
		[SerializeField]
		private OtherActionParam actionParam;
		public OtherActionParam ActionParam { get { return actionParam; } }
	}
	#endregion
	#region Inspector用クラスの内部パラメータ
	[System.SerializableAttribute]
	abstract class TextParam
	{
		[SerializeField]
		private float scaleRate = 1f;
		public float ScaleRate { get { return scaleRate; } }
	}
	[System.SerializableAttribute]
	class TextParamColor : TextParam
	{
		[SerializeField]
		private Color color = Color.white;
		public Color Color { get { return color; } }
	}
	[System.SerializableAttribute]
	class TextParamEffect : TextParam
	{
		[SerializeField]
		private GameObject effect;
		public GameObject Effect { get { return effect; } }
	}
	[System.SerializableAttribute]
	class PlayerActionParam
	{
		// ダメージ表示時間.
		[SerializeField]
		private float displayTime = 0.8f;
		public float DisplayTime { get { return displayTime; } }
		// 表示場所が一番上に来る時間.
		[SerializeField]
		private float peakTime = 0.4f;
		public float PeakTime { get { return peakTime; } }

		// スピード.
		[SerializeField]
		private float speedX = 0;
		[SerializeField]
		private float speedX_Random =  250;
		[SerializeField]
		private float speedY =  600;
		[SerializeField]
		private float speedY_Random =  200;

		public float SpeedX_Min { get { return speedX - speedX_Random; } }
		public float SpeedX_Max { get { return speedX + speedX_Random; } }
		public float SpeedY_Min { get { return speedY - speedY_Random; } }
		public float SpeedY_Max { get { return speedY + speedY_Random; } }
	}
	[System.SerializableAttribute]
	class OtherActionParam
	{
		// 各ステート時間.
		[SerializeField]
		private float scaleTime = 0.1f;
		public float ScaleTime { get { return scaleTime; } }
		[SerializeField]
		private float stopTime  = 0.4f;
		public float StopTime { get { return stopTime; } }
		[SerializeField]
		private float riseTime  = 0.4f;
		public float RiseTime { get { return riseTime; } }
		// オフセット.
		[SerializeField]
		private float offsetX = 0;
		[SerializeField]
		private float offsetX_Random =  50;
		[SerializeField]
		private float offsetY = 50;
		[SerializeField]
		private float offsetY_Random = 25;
		// スケール.
		[SerializeField]
		private float scall_Start = 3;
		public float Scall_Start { get { return scall_Start; } }
		[SerializeField]
		private float scall_End = 1;
		public float Scall_End { get { return scall_End; } }
		// 上昇幅.
		[SerializeField]
		private float riseY = 30;
		public float RiseY { get { return riseY; } }
		
		public float OffsetX_Min { get { return offsetX - offsetX_Random; } }
		public float OffsetX_Max { get { return offsetX + offsetX_Random; } }
		public float OffsetY_Min { get { return offsetY - offsetY_Random; } }
		public float OffsetY_Max { get { return offsetY + offsetY_Random; } }
		public float OffsetZ { get { return scaleTime + stopTime + riseTime + 2f; } }
	}
	#endregion
	#endregion

	#region フィールド＆プロパティ
	// 各種設定.
	[SerializeField]
	PlayerDamageLetterParam playerParam;
	[SerializeField]
	OtherDamageLetterParam otherParam;

	// UpdateType.
	bool isPlayerDamage;
	UILabel uiLabel;
	FiberSet fiberSet;
	#endregion

	#region 作成
	/* 作成部分は下のメソッドに移行.
	public static void CreateDamageLetter(GameObject parentObj, int damage, DamageType damageType, bool atkisPlayer, bool defisPlayer)
	{
		if (damage == 0)	// 0は表示しない.
			{ return; }
		if (parentObj == null)
			{ return; }
		if(!parentObj.activeSelf)
			{ return; }

		if(atkisPlayer || defisPlayer)
		{
			Transform parent = parentObj.transform;

			AssetReference assetReference = AssetReference.GetAssetReference(GameConstant.DamageLetter.AssetBundlePath);
			GameObject resource = assetReference.GetAsset<GameObject>(GameConstant.DamageLetter.AssetPath);
			if(resource != null)
			{
				GameObject go = GameObject.Instantiate(resource) as GameObject;
				if(go != null)
				{
					GUIDamageLetter damageLetter = go.GetSafeComponent<GUIDamageLetter>();
					if (damageLetter == null)
					{
						Object.Destroy(go);
						return;
					}
					damageLetter.Setup(parent, damage, damageType, defisPlayer);
				}
			}
		}
	}
	*/
	public static void CreateDamageLetter(GameObject prefab, GameObject parentObj, int damage, DamageType damageType, bool atkisPlayer, bool defisPlayer)
	{
		if (damage == 0)	// 0は表示しない.
			{ return; }
		if (parentObj == null)
			{ return; }
		if(!parentObj.activeSelf)
			{ return; }

		if(atkisPlayer || defisPlayer)
		{
			Transform parent = parentObj.transform;
			GameObject go = SafeObject.Instantiate(prefab) as GameObject;
			if (go == null)
				{ return; }
			go.SetActive(true);
			GUIDamageLetter damageLetter = go.GetSafeComponent<GUIDamageLetter>();
			if (damageLetter == null)
			{
				Debug.LogWarning("CreateDamageLetter.Create:GUIDamageLetterが見つからない");
				Object.Destroy(go);
				return;
			}
			damageLetter.Setup(parent, damage, damageType, defisPlayer);
		}
	}
	#endregion

	#region セットアップ
	private bool Setup(Transform parent, int damage, DamageType damageType, bool defisPlayer)
	{
		if(parent == null)
		{
			Object.Destroy(this.gameObject);
		}
		
		this.gameObject.transform.parent = parent;
		this.gameObject.transform.localPosition = new Vector3(0, 0, 2);
		this.gameObject.transform.localScale = Vector3.one;
		
		this.uiLabel = this.GetComponentInChildren<UILabel>();
		if(uiLabel)
		{
			string text = string.Empty;
			float fontScale;
			TextParamColor textParam;
			TextParamEffect effect = null;
			{
				DamageLetterParam damageLetterParam;
				if(defisPlayer)
				{
					this.isPlayerDamage = true;
					damageLetterParam = playerParam;
				}
				else
				{
					damageLetterParam = otherParam;
				}
				fontScale = damageLetterParam.Scale;

				// recovery,normal,magic
				if(damage < 0)
				{
					textParam = damageLetterParam.Recovery;
					text = "+";
				}
				else if((damageType & DamageType.Magic) != 0)
				{
					textParam = damageLetterParam.Magic;
				}
				else
				{
					textParam = damageLetterParam.Normal;
				}
				
				// Critical, Guard
				if((damageType & DamageType.Critical) != 0)
				{
					effect = damageLetterParam.Critical;
				}
				else if((damageType & DamageType.Guard) != 0)
				{
					effect = damageLetterParam.Guard;
				}
			}
			
			// TextParamColor
			{
				fontScale *= textParam.ScaleRate;
				uiLabel.color = textParam.Color;
			}
			// TextParamEffect
			if(effect != null)
			{
				fontScale *= effect.ScaleRate;
				if(effect.Effect != null)
				{
					// TODO:20140706井上 ObjectUIに移植のためインスタンス化ではなくアクティブ化に変更
					effect.Effect.SetActive(true);
				}
			}

			uiLabel.transform.localPosition = new Vector3(0, 0, -1);
			uiLabel.transform.localScale = Vector3.one;

			uiLabel.fontSize = (int)(fontScale);
			// TODO:20140825井上 UIの前後関係調整のためdepthの調整を外でやるのに変更
			//uiLabel.depth = 3;
			uiLabel.text = text + Mathf.Abs(damage);

			return true;
		}
		else
		{
			Object.Destroy(this.gameObject);
			return false;
		}
	}
	#endregion
	
	void Start()
	{
		fiberSet = new FiberSet();
		if(isPlayerDamage)
		{
			fiberSet.AddFiber(Update_PlayerDamage(playerParam.ActionParam));
		}
		else
		{
			fiberSet.AddFiber(Update_Other(otherParam.ActionParam));
		}
	}

	void Update()
	{
		// 自己管理じゃないと画面外に出た際にスクリプトが一時的に無効になり,コルーチンが破棄されてしまう.
		fiberSet.Update();
	}

	#region 更新
	IEnumerator Update_Other(OtherActionParam actionParam)
	{
		// 各ステート時間.
		float ScaleTime = actionParam.ScaleTime;
		float StopTime  = actionParam.StopTime;
		float RiseTime  = actionParam.RiseTime;
		// オフセット.
		float offsetX = Random.Range(actionParam.OffsetX_Min, actionParam.OffsetX_Max);
		float offsetY = Random.Range(actionParam.OffsetY_Min, actionParam.OffsetY_Max);
		float OffsetZ = actionParam.OffsetZ;
		// スケール.
		float Scall_Start = actionParam.Scall_Start;
		float Scall_End = actionParam.Scall_End;
		// 上昇幅.
		float RiseY = actionParam.RiseY;

		// 表示開始時のTime.
		float startTime = Time.time;
		try
		{
			this.gameObject.transform.localScale = new Vector3(Scall_Start, Scall_Start, 1);
			while(Time.time < startTime + ScaleTime)
			{
				float deltaTime = Time.time - startTime;
				float t = deltaTime / ScaleTime;
				float scale = Mathf.Lerp(Scall_Start, Scall_End, t);
				this.gameObject.transform.localScale = new Vector3(scale, scale, 1);
				this.gameObject.transform.localPosition = new Vector3(offsetX, offsetY, OffsetZ - deltaTime);

				yield return new WaitForEndOfFrame();
			}
			this.gameObject.transform.localScale = new Vector3(Scall_End, Scall_End, 1);

			while(Time.time < startTime + ScaleTime + StopTime)
			{
				float totalDeltaTime = Time.time - startTime;
				this.gameObject.transform.localPosition = new Vector3(offsetX, offsetY, OffsetZ - totalDeltaTime);

				yield return new WaitForEndOfFrame();
			}

			Color c = this.uiLabel.color;
			Color ec = this.uiLabel.effectColor;
			while(Time.time < startTime + ScaleTime + StopTime + RiseTime)
			{
				float totalDeltaTime = Time.time - startTime;
				float deltaTime = Time.time - (startTime + ScaleTime + StopTime);
				float t = deltaTime / RiseTime;
				this.gameObject.transform.localPosition = new Vector3(offsetX, offsetY + RiseY * t, OffsetZ - totalDeltaTime);

				c.a = 1 - t;
				ec.a = (1 - t) / 5;		// アウトラインは多重描きのようなので多めに減らす.
				this.uiLabel.color = c;
				this.uiLabel.effectColor = ec;
				
				yield return new WaitForEndOfFrame();
			}
		}
		finally
		{
			Object.Destroy(this.gameObject);
		}
	}

	IEnumerator Update_PlayerDamage(PlayerActionParam actionParam)
	{
		// ダメージ表示時間.
		float DisplayTime = actionParam.DisplayTime;
		// 表示場所が一番上に来る時間.
		float PeakYTime = actionParam.PeakTime;
		// スピード.
		float speedX = Random.Range(actionParam.SpeedX_Min, actionParam.SpeedX_Max);
		float speedY = Random.Range(actionParam.SpeedY_Min, actionParam.SpeedY_Max);

		// 表示開始時のTime.
		float startTime = Time.time;
		try
		{
			while(Time.time < startTime + DisplayTime)
			{
				float deltaTime = Time.time - startTime;
				float offsetY = speedY * PeakYTime * PeakYTime;
				float pBottomTime = deltaTime - PeakYTime;
				this.gameObject.transform.localPosition = new Vector3(speedX * deltaTime, offsetY - speedY * pBottomTime * pBottomTime, 1 - deltaTime);
				
				yield return new WaitForEndOfFrame();
			}
		}
		finally
		{
			Object.Destroy(this.gameObject);
		}
	}
	#endregion
}
