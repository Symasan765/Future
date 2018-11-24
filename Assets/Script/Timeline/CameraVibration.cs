using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraVibration : MonoBehaviour {
	GameObject m_Parent;
	public float m_MaxRange = 0.01f;

	System.Random cRandom;

	// Use this for initialization
	void Start () {
		cRandom = new System.Random(0);
		m_Parent = GameObject.Find("CameraParent");
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = m_Parent.transform.position;
		pos.x += (float)cRandom.Next(-10000,10000) / 100000.0f;
		pos.y += (float)cRandom.Next(-10000, 10000) / 100000.0f;
		pos.z += (float)cRandom.Next(-10000, 10000) / 100000.0f;

		m_Parent.transform.position = pos;
	}
}
