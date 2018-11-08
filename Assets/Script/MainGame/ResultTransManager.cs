using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultTransManager : MonoBehaviour {

	public static bool m_InTheGame = false;

	BossAttackManager m_BossManager;
	public GameObject m_FadeOut;
	public GameObject m_BackLoader;

	bool m_LoadFlag = false;

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
				m_InTheGame = false;
				SoundManager.Get.PlayBGM("Result", true);
				m_BackLoader.GetComponent<SceneBackLoder>().SceneChange();
			}
		}

		BackLoaderUpdate();
	}

	void BackLoaderUpdate()
	{
		if (m_LoadFlag == false)
		{
			// ボスの体力が半分を過ぎた
			if (m_BossManager.GetBossHP() / m_BossManager.m_BossMaxDamage <= 0.5f)
			{
				m_BackLoader.SetActive(true);
				m_LoadFlag = true;
			}
		}
	}
}
