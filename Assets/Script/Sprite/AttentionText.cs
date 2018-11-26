using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttentionText : MonoBehaviour {

	public SpriteRenderer m_Up;
	public SpriteRenderer m_Down;

	float m_Sec = 0.5f;
	float m_TimeCnt = 0.0f;

	public Vector3 m_ActivePos;
	public Vector3 m_NotActivePos;
	Vector3 m_LeftNotActivePos;
	Vector3 m_LeftActivePos;

	// Use this for initialization
	void Start () {
		m_LeftNotActivePos = new Vector3(-m_NotActivePos.x, m_NotActivePos.y, m_NotActivePos.z);
		m_LeftActivePos = new Vector3(-m_ActivePos.x, m_ActivePos.y, m_ActivePos.z);

		m_Up.transform.localPosition = m_LeftNotActivePos;
		m_Down.transform.localPosition = m_NotActivePos;
	}
	
	// Update is called once per frame
	void Update () {
		m_Up.size = new Vector2(m_Up.size.x, m_Up.size.y + Time.deltaTime * 2.0f);
		m_Down.size = new Vector2(m_Down.size.x, m_Down.size.y - Time.deltaTime * 2.0f);

		var objs = GameObject.FindGameObjectsWithTag("AttackRange");
		if (objs.Length > 0)
		{
			m_TimeCnt += Time.deltaTime;

			if (m_TimeCnt > m_Sec)
			{
				m_TimeCnt = m_Sec;
				m_Up.transform.localPosition = m_LeftActivePos;
				m_Down.transform.localPosition = m_ActivePos;
			}
		}
		else
		{
			m_TimeCnt -= Time.deltaTime;
			if (m_TimeCnt < 0.0f)
			{
				m_TimeCnt = 0.0f;
				m_Up.transform.localPosition = m_LeftNotActivePos;
				m_Down.transform.localPosition = m_NotActivePos;

				m_Up.size = new Vector2(2.56f, 10.8f);
				m_Down.size = new Vector2(2.56f, -10.8f);
			}
		}


		float t = m_TimeCnt / m_Sec;
		m_Down.transform.localPosition = Vector3.Lerp(m_NotActivePos, m_ActivePos, t);

		m_Up.transform.localPosition = Vector3.Lerp(m_LeftNotActivePos, m_LeftActivePos, t);
	}

	void PosUpdate()
	{

	}
}
