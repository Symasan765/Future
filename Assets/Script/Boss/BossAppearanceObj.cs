using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BossAppearanceObj : MonoBehaviour
{

	GameObject m_BossObj;
	public PlayableDirector m_Timeline;
	Vector3 m_InitPos;
	float m_TimeCnt = 0.0f;
	BossLight m_Light;

	// Use this for initialization
	void Start()
	{
		m_BossObj = GameObject.Find("Boss");
		m_InitPos = m_BossObj.transform.position;
		m_BossObj.transform.position = new Vector3(m_InitPos.x, m_InitPos.y, -100.0f);     // ボスを見つけたらカメラから見えない遥か上空へ
		m_Light = GameObject.FindGameObjectWithTag("BossManager").GetComponent<BossLight>();
	}

	// Update is called once per frame
	void Update()
	{
		// タイムラインが生きている
		if (m_Timeline.isActiveAndEnabled == true)
		{
			m_Light.LightChage(true);

			// 初期位置に戻す(若干時間を上げないと一瞬ボスが見えてしまいそう…)
			if (m_TimeCnt > 0.5f)
				m_BossObj.transform.position = m_InitPos;

			// タイムラインが終了した
			if (m_Timeline.duration < m_TimeCnt)
			{
				m_Light.LightChage(false);
				// 自身を消滅させる
				Destroy(gameObject);
				// 攻撃を開始！
				GameObject.FindGameObjectWithTag("BossManager").GetComponent<BossAttackManager>().m_AttackFlag = true;
			}
			m_TimeCnt += Time.deltaTime;
		}
	}

	public void StartTimeline()
	{
		m_Timeline.gameObject.SetActive(true);
	}
}
