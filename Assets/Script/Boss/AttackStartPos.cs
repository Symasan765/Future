using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStartPos : MonoBehaviour {

	BossAttackManager m_Manager;

	// Use this for initialization
	void Start () {
		m_Manager = GameObject.FindGameObjectWithTag("BossManager").GetComponent<BossAttackManager>();
	}
	
	// Update is called once per frame
	void Update () {
		// このオブジェクトがいる間はボスの攻撃をOFFにし続ける
		m_Manager.m_AttackFlag = false;
	}

	private void OnTriggerEnter(Collider collider)
	{
		// プレイヤーと接触したら攻撃フラグをオンにして自殺する
		m_Manager.m_AttackFlag = true;
		Destroy(gameObject);
	}
}
