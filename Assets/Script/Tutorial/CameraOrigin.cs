using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrigin : MonoBehaviour {

	Vector3 m_InitPos = Vector3.zero;
	public Vector3 m_Offset = Vector3.zero;

	// Use this for initialization
	void Start () {
		m_InitPos = transform.position;
		Debug.Log(m_InitPos);
		GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
		transform.parent = camera.transform;
		Debug.Log(camera.transform.position);
		transform.localPosition = new Vector3(m_InitPos.x + camera.transform.position.x / 4.0f, m_InitPos.y - camera.transform.position.y, m_InitPos.z - camera.transform.position.z);
		//transform.localPosition = transform.localPosition - m_InitPos;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
