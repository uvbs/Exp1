/// <summary>
/// 経験値エフェクト
/// 
/// 2013/04/12
/// </summary>
using UnityEngine;
using System.Collections;

public class ExpEffect : MonoBehaviour
{
	#region 定数
	private const string SmallExpEffectFilePath = "Common/SmallExpEffect";
	private const string HalfExpEffectFilePath  = "Common/MediumExpEffect";
	private const string BigExpEffectFilePath   = "Common/BigExpEffect";
	private const string Exp_hitFileName        = "Common/exp_hit_001";
	private const int UpSpeed     = 5;
	private const float UpScale   = 2.0f;
	private const float MoneyRate = 0.05f;
	#region エフェクト出現数処理関連
	private const int MediumValue = 3;
	private const int BigValue    = 5;
	// 経験値の量.
	const int Sml_exp = 1;
	const int Mid_exp = MediumValue;
	const int Big_exp = MediumValue * BigValue;
	// １つエフェクトが出るのに必要な経験値係数.
	const int Big_mag = Big_exp + Mid_exp + Sml_exp;
	const int Mid_mag = Mid_exp + Sml_exp;
	const int Sml_mag = Sml_exp;
	// １つ目のエフェクトが出るのに余分に必要な係数.
	const int Big_offset = MediumValue + BigValue + 2;
	const int Mid_offset = 1;
	const int Sml_offset = 0;
	// 種類.
	private enum EffectType : int
	{
		Small,
		Medium,
		Big,
		Max
	}
	#endregion
	private enum StateProc : int
	{
		MoveSide,
		StopHoming,
		UpdateHoming,
		Max
	}
	#endregion

	#region フィールド＆プロパティ
	private const int speed = 25;
	private ObjectBase Target { get; set; }
	private Transform TargetNull { get; set; }
	private Manager Manager { get; set; }
	private Vector3 StartPosition { get; set; }
	private float startTime;
	private float stopTime;
	private float upScale = 0.0f;
	private Vector3 startObecjtPosition;
	private float playerDistance;
	#endregion

	#region Proc
	private StateProc state;
	private System.Action[] HomingProc = new System.Action[(int)StateProc.Max];
	#endregion

	#region ファクトリ
	public static void CreateExpEffect(Vector3 startPosition, Vector3 offsetMin, Vector3 offsetMax, ObjectBase target, int money)
	{
		// お金の量が多いため減らす.
		money = Mathf.CeilToInt((float)money * MoneyRate);

		// 経験値を小.中.大エフェクトに分ける.
		int[] coinTable = new int[(int)EffectType.Max];

		// 大.
		coinTable[(int)EffectType.Big] = (money - Big_offset) / Big_mag;
		money -= coinTable[(int)EffectType.Big] * Big_exp;

		// 中.
		coinTable[(int)EffectType.Medium] = (money - Mid_offset) / Mid_mag;
		money -= coinTable[(int)EffectType.Medium] * Mid_exp;

		// 小.
		coinTable[(int)EffectType.Small] = (money - Sml_offset) / Sml_mag;

		// 小.中.大の数に応じてエフェクト生成.
		string[] effectFilePathTable = new string[] { SmallExpEffectFilePath, HalfExpEffectFilePath, BigExpEffectFilePath };
		for(int coinTableCnt = 0; coinTableCnt < (int)EffectType.Max; ++coinTableCnt)
		{
			if (string.IsNullOrEmpty(effectFilePathTable[coinTableCnt])) { return; }

			for(int coinCnt = 0; coinCnt < coinTable[coinTableCnt]; ++coinCnt)
			{
				CreateEffect(startPosition, offsetMin, offsetMax, target, effectFilePathTable[coinTableCnt]);
			}
		}
	}

	/// <summary>
	/// エフェクト生成処理.
	/// </summary>
	private static void CreateEffect(Vector3 startPosition, Vector3 offsetMin, Vector3 offsetMax, ObjectBase target, string filePath)
	{
		Vector3 position = startPosition + new Vector3(Random.Range(offsetMin.x, offsetMax.x), Random.Range(offsetMin.y, offsetMax.y), Random.Range(offsetMin.z, offsetMax.z));
		Quaternion rotation = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
		EffectManager.CreateExp(target, startPosition, position, rotation, filePath);
	}
	#endregion

	#region セットアップ
	public static bool Setup(GameObject go, Manager manager, ObjectBase target, Vector3 startPosition)
	{
		// コンポーネント取得
		ExpEffect expEffect = go.GetSafeComponent<ExpEffect>();
		if (expEffect == null)
		{
			manager.Destroy(go);
			return false;
		}
		expEffect.Manager = manager;

		// 設定
		expEffect.SetupHoming(target, startPosition);

		return true;
	}

	/// <summary>
	/// ホーミング処理に使用する値等をセット.
	/// </summary>
	/// <param name='target'>
	/// ターゲット.
	/// </param>
	private void SetupHoming(ObjectBase target, Vector3 startPosition)
	{
		Target = target;
		StartPosition = this.transform.position;
		this.startTime = Time.time;
		this.stopTime = Random.Range(0.3f, 1.0f);
		this.transform.localScale = Vector3.zero;
		this.startObecjtPosition = startPosition;

		// ホーミングの各状態をセット
		this.HomingProc[(int)StateProc.MoveSide] = MoveSide;
		this.HomingProc[(int)StateProc.StopHoming] = StopHoming;
		this.HomingProc[(int)StateProc.UpdateHoming] = UpdateHoming;
		this.state = StateProc.MoveSide;

		if (this.Target)
		{
			this.TargetNull = this.Target.AvaterModel.RootTransform;
			if (this.TargetNull == null)
			{
				Debug.LogWarning("NotFound TargetNULL");
				this.TargetNull = this.Target.transform;
			}
		}
	}
	#endregion

	#region 破棄
	public void Destroy()
	{
		if (this.Manager)
		{
			this.Manager.Destroy(this.gameObject);
			this.Manager = null;
		}
		else
		{
			GameObject.Destroy(this.gameObject);
		}
	}
	void OnDestroy()
	{
		if (this.Manager)
		{
			this.Manager.Destroy(this.gameObject);
		}
	}
	#endregion

	#region 更新
	void Update()
	{
		// エフェクト拡大.
		this.UpdateScale();

		// ホーミング処理.
		if(this.HomingProc[(int)this.state] != null)
		{
			this.HomingProc[(int)this.state]();
		}
	}
	#endregion

	#region 消滅エフェクト生成
	/// <summary>
	/// 消滅エフェクト生成.
	/// </summary>
	private void CreateDisappearanceEffect()
	{
		EffectManager.CreateSelfDestroy(this.transform.position, this.transform.rotation, Exp_hitFileName, false);
	}
	#endregion

	#region 拡大
	/// <summary>
	/// 拡縮更新.
	/// </summary>
	private void UpdateScale()
	{
		this.upScale += UpScale;
		if(this.upScale > 1.0f) this.upScale = 1.0f;
		this.transform.localScale = new Vector3(this.upScale, this.upScale, this.upScale);
	}
	#endregion

	#region Homing
	/// <summary>
	/// 真横へ移動する処理.
	/// </summary>
	private void MoveSide()
	{
		if(Vector3.SqrMagnitude(StartPosition - this.transform.position) < (2.0f * 2.0f))
		{
			// エフェクト出現位置から一定の距離まで移動.
			this.UpdatePosition();
		}
		else
		{
			// エフェクト出現位置から一定の距離まで離れたら一時停止.
			this.StartPosition = this.transform.position;		// 開始位置を更新.
			this.state = StateProc.StopHoming;
		}
	}

	/// <summary>
	/// 一時停止する処理.
	/// </summary>
	private void StopHoming()
	{
		if((Time.time - this.startTime) >= this.stopTime)
		{
			// 停止する時間を超えたらターゲットとの距離に応じて斜め上へ向けホーミング開始.
			if(this.TargetNull != null)
			{
				float dis = Vector3.Distance(this.TargetNull.transform.position, this.startObecjtPosition);
				this.playerDistance = dis / 10f;
				if(dis > 45f) { dis = 45f; }
				this.transform.rotation = Quaternion.AngleAxis(dis, this.transform.TransformDirection(-Vector3.right)) * this.transform.rotation;
				this.startTime = Time.time;

				this.state = StateProc.UpdateHoming;
			}
			else
			{
				this.Destroy();
			}
		}
	}

	/// <summary>
	/// ホーミング更新処理.
	/// </summary>
	private void UpdateHoming()
	{
		// ストップする時間を超えたらホーミング処理を行う.
		if(this.TargetNull != null)
		{
			this.UpdateRotation();
			this.UpdatePosition();
		}
		else
		{
			this.Destroy();
		}
	}

	/// <summary>
	/// ホーミング時の回転処理.
	/// </summary>
	private void UpdateRotation()
	{
		{
			if(Vector3.SqrMagnitude(this.TargetNull.transform.position - this.transform.position) < 4.0f ||
				this.transform.position.y < this.Target.transform.position.y)
			{
				// エフェクトとホーミング先の距離が一定未満ならエフェクト消去して消滅エフェクト生成.
				this.Destroy();
				this.CreateDisappearanceEffect();
			}
		}

		Vector3 vecTarget = this.TargetNull.transform.position - this.transform.position;		// ターゲットへのベクトル
		Quaternion rotTarget = Quaternion.LookRotation(vecTarget);								// ターゲットへ向けるクォータニオン

		float t = (Time.time - this.startTime) / this.playerDistance;
		transform.rotation = Quaternion.Slerp(transform.rotation, rotTarget, t);
	}

	/// <summary>
	/// 位置更新.
	/// </summary>
	/// <param name="velocity"></param>
	private void UpdatePosition()
	{
		Vector3 movement;
		movement = this.transform.forward * speed * Time.deltaTime;
		this.transform.position += movement;
	}
	#endregion
}