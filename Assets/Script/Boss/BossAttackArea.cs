using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// このスクリプトがアタッチされたオブジェクトの範囲をボスの攻撃範囲として別のスクリプトに渡す
/// </summary>
public class BossAttackArea : MonoBehaviour {

	public Vector2 GetAreaPos()
	{
		return transform.position;
	}

	public Vector2 GetAreaRange()
	{
		return transform.localScale;
	}
}
