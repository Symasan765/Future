using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimeState : MonoBehaviour {
	Animator m_Animator;
	BossAttackManager m_BossManager;

	// Use this for initialization
	void Start () {
		m_Animator = GetComponent<Animator>();
		m_BossManager = GameObject.FindGameObjectWithTag("BossManager").GetComponent<BossAttackManager>();
	}
	
	// Update is called once per frame
	void Update () {
		SetTag();
	}

	void SetTag()
	{
		m_Animator.SetBool("is_DownSwing", m_BossManager.m_DownSwing);
		m_Animator.SetBool("is_SideSwing", m_BossManager.m_SideSwing);
		m_Animator.SetBool("is_Beam", m_BossManager.m_Beam);
	}
}
