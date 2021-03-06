/// <summary>
/// グローバル関数
/// 
/// 2013/01/28
/// </summary>
using UnityEngine;
using System.Collections;
using System.IO;
using System.Net;

using Scm.Common.Master;
using Scm.Common.GameParameter;

public class GameGlobal
{
	/// <summary>
	/// キャラID取得
	/// </summary>
	/// <param name="avatarType"></param>
	public static int GetCharID(AvatarType avatarType)
	{
		return (int)avatarType;
	}

	/// <summary>
	/// スキルID取得
	/// </summary>
	/// <param name="avatarType"></param>
	/// <param name="skillType"></param>
	/// <param name="lv"></param>
	public static int GetSkillID(AvatarType avatarType, SkillButtonType skillButtonType, int lv)
	{
		CharaLevelMasterData charaLv;
		if (!MasterData.TryGetCharaLv((int)avatarType, lv, out charaLv))
			{ return 0; }

		int id = 0;
		switch (skillButtonType)
		{
		case SkillButtonType.Normal:			id = charaLv.AttackId;			break;
		case SkillButtonType.Skill1:			id = charaLv.Skill1Id;			break;
		case SkillButtonType.Skill2:			id = charaLv.Skill2Id;			break;
		case SkillButtonType.SpecialSkill:		id = charaLv.SpecialSkillId;	break;
		case SkillButtonType.TechnicalSkill:	id = charaLv.TechnicalSkillId;	break;
		default:
			return 0;
		}

		return id;
	}

	/// <summary>
	/// オフセット加算
	/// </summary>
	/// <returns></returns>
	/// <param name="offset"></param>
	/// <param name="position"></param>
	/// <param name="rotation"></param>
	/// <param name="scale"></param>
	public static bool AddOffset(IBulletSetMasterData bulletSet, ref Vector3 position, ref Quaternion rotation, Vector3 scale)
	{
		if (bulletSet != null)
		{
			return AddOffset(bulletSet.BulletOffset, ref position, ref rotation, scale);
		}
		return false;
	}
	private static bool AddOffset(SkillBulletOffsetMasterData offset, ref Vector3 position, ref Quaternion rotation, Vector3 scale)
	{
		if (offset == null)
			{ return false; }

		// オフセット
		Vector3 offsetPos = new Vector3(offset.PositionX, offset.PositionY, offset.PositionZ);
		Vector3 offsetRot = new Vector3(offset.RotationX, offset.RotationY, offset.RotationZ);
		// 位置補正
		Matrix4x4 mat = Matrix4x4.TRS(Vector3.zero, rotation, scale);
		position += mat.MultiplyPoint3x4(offsetPos);
		// 角度補正
		rotation *= Quaternion.Euler(offsetRot);

		return true;
	}
	public static bool AddOffset(SkillEffectMasterData effect, ref Vector3 position, ref Quaternion rotation, Vector3 scale)
	{
		if (effect == null)
			{ return false; }

		// オフセット
		Vector3 offsetPos = new Vector3(effect.OffsetPositionX, effect.OffsetPositionY, effect.OffsetPositionZ);
		Vector3 offsetRot = new Vector3(effect.OffsetRotationX, effect.OffsetRotationY, effect.OffsetRotationZ);
		// 位置補正
		Matrix4x4 mat = Matrix4x4.TRS(Vector3.zero, rotation, scale);
		position += mat.MultiplyPoint3x4(offsetPos);
		// 角度補正
		rotation *= Quaternion.Euler(offsetRot);

		return true;
	}

	/// <summary>
	/// 範囲内かどうか
	/// </summary>
	/// <returns></returns>
	/// <param name="point"></param>
	/// <param name="target"></param>
	/// <param name="angle"></param>
	public static bool IsInRange(Transform point, Vector3 targetPosition, float angle)
	{
		return IsInRange(point.position, point.forward, targetPosition, angle);
	}
	public static bool IsInRange(Vector3 position, Vector3 forward, Vector3 targetPosition, float angle)
	{
		Vector3 direction = targetPosition - position;
		float t = Vector3.Angle(forward, direction);
		if (t > angle)
			return false;
		return true;
	}

	/// <summary>
	/// 範囲内かどうか
	/// </summary>
	/// <returns></returns>
	/// <param name="point"></param>
	/// <param name="target"></param>
	/// <param name="angle"></param>
	/// <param name="distance"></param>
	public static bool IsInRange(Transform point, Vector3 targetPosition, float angle, float distance)
	{
		return IsInRange(point.position, point.forward, targetPosition, angle, distance);
	}
	public static bool IsInRange(Vector3 position, Vector3 forward, Vector3 targetPosition, float angle, float distance)
	{
		// 距離
		Vector3 direction = targetPosition - position;
		if (direction == Vector3.zero)
			return false;
		if (direction.sqrMagnitude > distance * distance)
			return false;

		// 角度
		/*
		float t = Vector3.Angle(forward, direction);
		if (t > angle)
			return false;
		*/

		// 比較だけならAngle()よりInAngle()の方が軽い.
		bool inAngle = forward.IsInAngle(direction, angle);
		if(!inAngle)
			{ return false;}

		return true;
	}

	/// <summary>
	/// ObjectBaseの名前を取得する
	/// </summary>
	public static string GetObjectBaseName(ObjectBase objectBase)
	{
		return GetObjectBaseName (objectBase, objectBase.gameObject.name, true);
	}
	public static string GetObjectBaseName(ObjectBase objectBase, string srcName, bool isInFieldID)
	{
		string name = "";

		// InFieldID
		if (isInFieldID)
			name = string.Format("{0:000}_", objectBase.InFieldId);

		// 名前
		if (string.IsNullOrEmpty(objectBase.UserName))
			name += GetObjectBaseName(objectBase.TeamType, objectBase.EntrantType);
		else
			name += objectBase.UserName;

		// 元の名前
		if (!string.IsNullOrEmpty(srcName))
			name += string.Format("({0})", srcName);
		return name;
	}
	public static string GetObjectBaseName(TeamType teamType, EntrantType entrantType)
	{
		var team = teamType.GetClientTeam();
		string teamName = (team == TeamTypeClient.Unknown ? "" : team.ToString());
		return string.Format("{0}{1}", teamName, entrantType);
	}

	/// <summary>
	/// ワールド座標系からスクリーン座標系に変換する
	/// </summary>
	public static bool WorldToScreenPosition(Camera worldCamera, Camera screenCamera, Vector3 world, out Vector3 screen)
	{
		screen = new Vector3();
		if (worldCamera == null)
			return false;
		if (screenCamera == null)
			return false;

		Vector3 viewPoint = worldCamera.WorldToViewportPoint(world);
		screen = screenCamera.ViewportToWorldPoint(viewPoint);
		
		return true;
	}

	/// <summary>
	/// ObjectUIのパスを取得する(オブジェクト用)
	/// </summary>
	public static string GetObjectUIPath(ObjectUserInterfaceMasterData data)
	{
		if (data == null)
			return null;

		// パスを取得する
		string assetPath = "";
		switch (ScmParam.Common.AreaType)
		{
		case AreaType.Lobby:
			assetPath = data.LobbyAssetPath;
			break;
		case AreaType.Field:
			assetPath = data.BattleAssetPath;
			break;
		}
		if (string.IsNullOrEmpty(assetPath))
			return null;

		return "ObjectUI/" + assetPath + ".asset";
	}

	/// <summary>
	/// ObjectUIのパスを取得する(他人用)
	/// </summary>
	public static string GetObjectUIPersonPath()
	{
		switch (ScmParam.Common.AreaType)
		{
		case AreaType.Lobby:
			return "ObjectUI/Lobby/Person.asset";
		case AreaType.Field:
			return "ObjectUI/Battle/Person.asset";
		}
		return null;
	}

	/// <summary>
	/// ObjectUIのパスを取得する(プレイヤー用)
	/// </summary>
	public static string GetObjectUIPlayerPath()
	{
		switch (ScmParam.Common.AreaType)
		{
		case AreaType.Lobby:
			return null;
		case AreaType.Field:
			return "ObjectUI/Battle/Player.asset";
		}
		return null;
	}

	#region ソート
	/// <summary>
	/// 昇順ソート
	/// </summary>
	public static int AscendSort(Vector3 position, Transform x, Transform y)
	{
		float a = Vector3.SqrMagnitude(x.position - position);
		float b = Vector3.SqrMagnitude(y.position - position);
		return GameGlobal.AscendSort(a, b);
	}
	/// <summary>
	/// 降順ソート
	/// </summary>
	public static int DescendSort(Vector3 position, Transform x, Transform y)
	{
		float a = Vector3.SqrMagnitude(x.position - position);
		float b = Vector3.SqrMagnitude(y.position - position);
		return GameGlobal.DescendSort(a, b);
	}
	/// <summary>
	/// 昇順ソート
	/// </summary>
	public static int AscendSort(float x, float y)
	{
		if (x == y)	{ return 0;	}
		if (x > y)	{ return 1; }
		else		{ return -1; }
	}
	/// <summary>
	/// 降順ソート
	/// </summary>
	public static int DescendSort(float x, float y)
	{
		if (x == y)	{ return 0;	}
		if (x > y)	{ return -1; }
		else		{ return 1; }
	}
	/// <summary>
	/// 昇順ソート
	/// </summary>
	public static int AscendSort(int x, int y)
	{
		if (x == y)	{ return 0;	}
		if (x > y)	{ return 1; }
		else		{ return -1; }
	}
	/// <summary>
	/// 降順ソート
	/// </summary>
	public static int DescendSort(int x, int y)
	{
		if (x == y)	{ return 0;	}
		if (x > y)	{ return -1; }
		else		{ return 1; }
	}
	#endregion

	#region ディレクトリ操作系
	/// <summary>
	/// 指定したディレクトリを作成する.
	/// </summary>
	public static string CreateDirectory(string directory)
	{
		string path = GetDirectory(directory);

		// ディレクトリが存在しなければ作成する
		if (!File.Exists(path))
			Directory.CreateDirectory(path);

		return path;
	}
	/// <summary>
	/// 各プラットフォームごとのディレクトリ取得
	/// </summary>
	/// <returns></returns>
	/// <param name="directory"></param>
	public static string GetDirectory(string directory)
	{
		string directoryPath = null;
		switch(Application.platform)
		{
		case RuntimePlatform.Android:
		case RuntimePlatform.IPhonePlayer:
			directoryPath = Path.Combine(Application.persistentDataPath, directory);
			break;
		default:
			directoryPath = directory;
			break;
		}
		return directoryPath;
	}
	/// <summary>
	/// ファイルパス取得
	/// </summary>
	/// <returns></returns>
	/// <param name="directory"></param>
	/// <param name="fileName"></param>
	public static string GetPath(string directory, string fileName)
	{
		string dir = GetDirectory(directory);
		return Path.Combine(dir, fileName);
	}
	/// <summary>
	/// ファイルの存在確認
	/// </summary>
	/// <returns></returns>
	/// <param name="directory"></param>
	/// <param name="fileName"></param>
	public static bool ExistFile(string directory, string fileName)
	{
		string path = GetPath(directory, fileName);
		return File.Exists(path);
	}
	#endregion

#if XW_DEBUG
	public static void AddSpherePolygon(Transform parent, SphereCollider collider, bool isSelfDestroy)
	{
		if (collider == null)
			{ return; }
		Vector3 pos = collider.center;
		Vector3 scale = new Vector3(collider.radius*2f, collider.radius*2f, collider.radius*2f);
		AddPolygon("Polygon/Sphere", parent, pos, scale, isSelfDestroy, collider);
	}
	public static void AddCubePolygon(Transform parent, BoxCollider collider, bool isSelfDestroy)
	{
		if (collider == null)
			{ return; }
		Vector3 pos = collider.center;
		Vector3 scale = collider.size;
		AddPolygon("Polygon/Cube", parent, pos, scale, isSelfDestroy, collider);
	}
	static void AddPolygon(string path, Transform parent, Vector3 position, Vector3 scale, bool isSelfDestroy, Collider collider)
	{
		GameObject resource = Resources.Load(path) as GameObject;
		if (resource == null)
		{
			Debug.LogWarning(path + "がない");
			return;
		}
		GameObject go = GameObject.Instantiate(resource) as GameObject;
		if (go == null)
		{
			Debug.LogWarning(path + "がない");
			return;
		}

		Transform transform  = go.transform;
		transform.parent = parent;
		transform.localPosition = position;
		transform.localRotation = Quaternion.identity;
		transform.localScale = scale;

		{
			SelfDestroyOnly com = go.GetComponent<SelfDestroyOnly>();
			if (com)
			{
				com.enabled = isSelfDestroy;
			}
		}
		{
			ColliderPolygon com = go.GetComponent<ColliderPolygon>();
			if (com)
			{
				com.Collider = collider;
			}
		}
	}
#else
	[System.Diagnostics.Conditional("XW_DEBUG")]
	public static void AddSpherePolygon(Transform parent, SphereCollider collider, bool isSelfDestroy) {}
	[System.Diagnostics.Conditional("XW_DEBUG")]
	public static void AddCubePolygon(Transform parent, BoxCollider collider, bool isSelfDestroy) {}
	[System.Diagnostics.Conditional("XW_DEBUG")]
	static void AddPolygon(string path, Transform parent, Vector3 position, Vector3 scale, bool isSelfDestroy, Collider collider) {}
#endif
}
