using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRadarScript : MonoBehaviour {

	Player[] m_PlayerObjs;
	GameObject[] m_RadarObjs;

	public GameObject m_LadarPrefab;

	public float m_MaxRadius = 20.0f;
	public float m_MinRadius = 5.0f;
	public Vector3 m_Offset;

	// Use this for initialization
	void Start () {
		GameObject[] obj = GameObject.FindGameObjectsWithTag("Player");

		m_PlayerObjs = new Player[obj.Length];
		m_RadarObjs = new GameObject[obj.Length];
		for (int i = 0; i < obj.Length; i++)
		{
			m_PlayerObjs[i] = obj[i].GetComponent<Player>();
			m_RadarObjs[i] = Instantiate(m_LadarPrefab);
		}
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < m_PlayerObjs.Length; i++)
		{
			RadarUpdate(i);
		}
	}

	void RadarUpdate(int i)
	{
		if (ResultTransManager.m_InTheGame == true)
		{
			m_RadarObjs[i].transform.position = m_PlayerObjs[i].gameObject.transform.position + m_Offset;
			int gage = m_PlayerObjs[i].GetMentalGauge();
			float rate = gage / 100.0f;
			float radius = Mathf.Lerp(m_MinRadius, m_MaxRadius, rate);
			m_RadarObjs[i].transform.localScale = new Vector3(radius, radius, 0.0f);
		}
	}
}
