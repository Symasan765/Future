using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingText : MonoBehaviour {

	// こいつを管理してるバズーカ
	BazookaRifle m_Bazooka;

	float m_ScaleTimeSec = 0.5f;        // 拡大縮小させるタイム
	float m_TimeCnt = 0.0f;

	bool m_StartFlag = false;       // バズーカがONになったらこいつをtrueにする

	bool m_EndFlag = false;			// バズーカがONになったあとでOFFになった

	public void SetInit(BazookaRifle bazooka)
	{
		m_Bazooka = bazooka;
		transform.localScale = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
		if (m_EndFlag)
		{
			m_TimeCnt -= Time.deltaTime;
			if (m_TimeCnt < 0.0f) m_TimeCnt = 0.0f;
			float t = m_TimeCnt / m_ScaleTimeSec;

			transform.localScale = Vector3.one * t;

			if(t <= 0.0f)
			{
				Destroy(gameObject);
			}
			return;
		}

		// バズーカに証拠が入った
		if (m_Bazooka.isSetNormalEvidence)
		{
			m_StartFlag = true;
		}


		// バズーカに証拠が入っている
		if (m_StartFlag)
		{
			m_TimeCnt += Time.deltaTime;
			if (m_TimeCnt > m_ScaleTimeSec) m_TimeCnt = m_ScaleTimeSec;
			float t = m_TimeCnt / m_ScaleTimeSec;
			transform.localScale = Vector3.one * t;

			if (!m_Bazooka.isSetNormalEvidence)
				m_EndFlag = true;
		}
	}
}
