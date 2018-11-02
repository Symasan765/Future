using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraVibration : MonoBehaviour {
	GameObject m_Parent;
	public float m_MaxRange = 0.01f;

	// Use this for initialization
	void Start () {
		m_Parent = GameObject.Find("CameraParent");
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = m_Parent.transform.position;
		pos.x += Random.Range(-m_MaxRange, m_MaxRange);
		pos.y += Random.Range(-m_MaxRange, m_MaxRange);
		pos.z += Random.Range(-m_MaxRange, m_MaxRange);

		m_Parent.transform.position = pos;
	}
}
