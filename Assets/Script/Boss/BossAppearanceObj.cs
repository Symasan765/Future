using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BossAppearanceObj : MonoBehaviour
{

	GameObject m_BossObj;
	public PlayableDirector m_Timeline;
	public GameObject m_RadarTimeline;
	float m_TimeCnt = 0.0f;
	BossLight m_Light;
	SkinnedMeshRenderer[] skin;

	bool m_DrawFlag = false;

	// Use this for initialization
	void Start()
	{
		m_BossObj = GameObject.Find("Boss");
		m_Light = GameObject.FindGameObjectWithTag("BossManager").GetComponent<BossLight>();

		skin = m_BossObj.GetComponentsInChildren<SkinnedMeshRenderer>();
	}

	// Update is called once per frame
	void Update()
	{
		// タイムラインが生きている
		if (m_Timeline.isActiveAndEnabled == true)
		{
			m_Light.LightChage(true);

			// 初期位置に戻す(若干時間を上げないと一瞬ボスが見えてしまいそう…)
			if(m_TimeCnt > 1.5f && m_DrawFlag == false)
			{
				for (int i = 0; i < skin.Length; i++)
				{
					skin[i].enabled = true;
				}
				m_DrawFlag = true;
			}
			//m_BossObj.transform.position = m_InitPos;

			// タイムラインが終了した
			if (m_Timeline.duration < m_TimeCnt)
			{
				// レーダー処理へ移行！
				Instantiate(m_RadarTimeline);
				m_Light.LightChage(false);
				// 自身を消滅させる
				Destroy(gameObject);
				// 攻撃を開始！
				GameObject.FindGameObjectWithTag("BossManager").GetComponent<BossAttackManager>().m_AttackFlag = true;
			}
			m_TimeCnt += Time.deltaTime;
		}
		else
		{
			for (int i = 0; i < skin.Length; i++)
			{
				skin[i].enabled = false;
			}
		}
	}

	public void StartTimeline()
	{
		m_Timeline.gameObject.SetActive(true);
		for (int i = 0; i < skin.Length; i++)
		{
			skin[i].enabled = true;
		}
		
	}
}
