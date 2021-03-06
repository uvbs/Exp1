/// <summary>
/// 障壁
/// 
/// 2013/10/18
/// </summary>
using UnityEngine;
using Scm.Common.Master;

public class Barrier : Gadget
{
	#region セットアップ
	/* 
	// HACK: Barrierを自陣営が通り抜けられる仕様は凍結.
	// 主な理由は自分弾丸系の技を使用した際、他人もプレイヤーと同じレイヤーになるため.
	// 青チームのプレイヤーから見た場合、赤チームのプレイヤーが赤チームの壁を通り抜けられないから.
	// 解決のためには｢他人が自分弾丸を使用した時のレイヤー」「Barrierオブジェクトのレイヤー」の２つを新設する必要有り.
	IEnumerator SetupLayer()
	{
		Player p = GameController.GetPlayer();
		while(p == null)
		{
			yield return new WaitForEndOfFrame();
			p = GameController.GetPlayer();
		}

		//this.gameObject.layer = (p.TeamType == this.TeamType) ? LayerNumber.Person : LayerNumber.Object;
	}
	*/
	#endregion
	
	#region 経験値処理
	protected override Vector3 ExpEffectOffsetMin{ get{ return new Vector3(0.0f, 1.5f, 0.0f); } }
	protected override Vector3 ExpEffectOffsetMax{ get{ return new Vector3(0.0f, 1.5f, 0.0f); } }
	#endregion
}
