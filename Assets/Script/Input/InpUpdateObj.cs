using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InpUpdateObj : MonoBehaviour {

	public bool m_UpdateFlag = true;

	// Use this for initialization
	void Start () {
#if UNITY_EDITOR

#else
		XPad.Get.m_UpdateFlag = m_UpdateFlag;
#endif
	}

	// Update is called once per frame
	void Update () {
		
	}
}
