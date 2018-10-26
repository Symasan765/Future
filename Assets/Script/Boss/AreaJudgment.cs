using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaJudgment : MonoBehaviour {

	// ステージ中のエリアを判定するためのスクリプト
	public int m_AreaNo = 0;
	public int m_SelectAttackNo = 0;

	private void Start()
	{
		GetComponent<MeshRenderer>().enabled = false;
	}
}
