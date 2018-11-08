using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultTransManager : MonoBehaviour {

	BossAttackManager m_BossManager;
	public GameObject m_FadeOut;

	// Use this for initialization
	void Start () {
		m_BossManager = GameObject.Find("BossAttackManager").GetComponent<BossAttackManager>();
	}
	
	// Update is called once per frame
	void Update () {
		float bossHP = m_BossManager.GetBossHP();

		if(bossHP <= 0.0f)
		{
			m_FadeOut.SetActive(true);

			// フェードアウトが終わった
			if (m_FadeOut.GetComponent<FadeObj>().IsEnd())
			{
				GetComponent<SceneBackLoder>().SceneChange();
			}
		}
	}
}
