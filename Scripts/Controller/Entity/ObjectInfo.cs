/// <summary>
/// 生成前のObjectBase情報.
/// 
/// 2014/11/06
/// </summary>

using UnityEngine;
using System.Collections.Generic;
using Scm.Common.Packet;
using Scm.Common.GameParameter;
using System;

public abstract class AvatarInfo : EntrantInfo
{
	// CharacterInfo だと NGUI の クラス名と被った.
	// マップウィンドウ表示用.
	public int KillCount { get; set; }
	public int ScoreRank { get; set; }
}
public class PlayerInfo : AvatarInfo
{
	public override void CreateObject()
	{
		Entrant.AddEntrant(this);
		PlayerManager.Instance.Create(this);
	}
}

//LWZ:add for create character avatar model
public class ModelInfo : AvatarInfo
{
    public override void CreateObject()
    {
        Entrant.AddEntrant(this);

    }
}

public class PersonInfo : AvatarInfo
{
	public override void CreateObject()
	{
		Entrant.AddEntrant(this);
		PersonManager.Instance.Create(this);
	}
}
public class ItemDropInfo : EntrantInfo
{
	public override void CreateObject()
	{
		Entrant.AddEntrant(this);
		ObjectManager.Instance.CreateItem(this);
	}
}
public class NpcInfo : EntrantInfo
{
	public override void CreateObject()
	{
		Entrant.AddEntrant(this);
		NpcManager.Instance.Create(this);
	}
}
public class GadgetInfo : EntrantInfo
{
	public override void CreateObject()
	{
		Entrant.AddEntrant(this);
		ObjectManager.Instance.CreateObject(this);
	}
}
