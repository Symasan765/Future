using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InpUpdateObj : MonoBehaviour {

	public bool m_UpdateFlag = true;

	// Use this for initialization
	void Start () {
		XPad.Get.m_UpdateFlag = m_UpdateFlag;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
