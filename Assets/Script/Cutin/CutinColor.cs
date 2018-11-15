using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutinColor : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Color[] m_Color = { Color.blue, Color.red, Color.green, Color.yellow, Color.gray };

		var obj = GetComponent<Image>().color = m_Color[CutinScript.m_PlayerNo];

		SoundManager.Get.PlaySE("katana-gesture2");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
