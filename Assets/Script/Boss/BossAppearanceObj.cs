using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BossAppearanceObj : MonoBehaviour {

	GameObject m_BossObj;
	public PlayableDirector m_Timeline;
	Vector3 m_InitPos;
	float m_TimeCnt = 0.0f;

	// Use this for initialization
	void Start () {
		m_BossObj = GameObject.FindGameObjectWithTag("BOSS");
		m_InitPos = m_BossObj.transform.position;
		m_BossObj.transform.position = Vector3.up * 100.0f;		// ボスを見つけたらカメラから見えない遥か上空へ
	}
	
	// Update is called once per frame
	void Update () {
		// タイムラインが生きている
		if(m_Timeline.isActiveAndEnabled == true)
		{
			// 初期位置に戻す
			m_BossObj.transform.position = m_InitPos;
			//m_BossObj.transform.position = m_InitPos;
			// タイムラインが終了した
			if (m_Timeline.duration < m_TimeCnt)
			{
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
