using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimeState : MonoBehaviour {
	Animator m_Animator;
	BossAttackManager m_BossManager;
	PartyTimeManager m_PartyManager;

	// Use this for initialization
	void Start () {
		m_Animator = GetComponent<Animator>();
		m_BossManager = GameObject.FindGameObjectWithTag("BossManager").GetComponent<BossAttackManager>();

		var obj = GameObject.Find("PartyTimeManager");
		if (obj != null)
			m_PartyManager = obj.GetComponent<PartyTimeManager>();
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
		if(m_PartyManager != null)
			m_Animator.SetBool("isFever", m_PartyManager.IsFever());
	}
}
