using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class MentalTimeline : MonoBehaviour {

	public GameObject[] m_NextText = new GameObject[4];

	public GameObject m_InpuUpdate;

	bool[] m_PressFlag = new bool[4];

	PlayableDirector m_Director;

	bool AllInitFlag = false;

	// Use this for initialization
	void Start () {
		for(int i = 0; i < 4; i++)
		{
			m_PressFlag[i] = false;
		}

		m_Director = GetComponent<PlayableDirector>();
	}
	
	// Update is called once per frame
	void FixedUpdate() {
		if (m_Director.time > 30.0 / 60.0)
		{
			InputUpdate();
		}

		if(m_Director.duration-0.3 < m_Director.time)
		{
			// 入力機能を戻す
			m_InpuUpdate.SetActive(true);
			Destroy(gameObject);
			GameObject.FindGameObjectWithTag("BossManager").GetComponent<BossAttackManager>().m_AttackFlag = true;
		}
	}

	void InputUpdate()
	{
		for(int i = 0; i < 4; i++)
		{
			if (XPad.Get.GetPress(XPad.KeyData.A, i))
			{
				m_PressFlag[i] = true;
				m_NextText[i].SetActive(true);
			}
		}

		// 入力が終わっていない人がいたらタイムを固定して処理を終える
		for(int i = 0; i < 4; i++)
		{
			if (m_PressFlag[i] == false)
			{
				m_Director.time = 40.0 / 60.0;
				return;
			}
		}

		// 初めて全員が押された瞬間
		if(AllInitFlag == false)
		{
			m_Director.time = 290.0 / 60.0;
			AllInitFlag = true;
		}
	}
}
