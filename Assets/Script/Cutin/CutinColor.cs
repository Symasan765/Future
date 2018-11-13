using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutinColor : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Color[] m_Color = { Color.red, Color.blue, Color.yellow, Color.green, Color.gray };

		var obj = GetComponent<Image>().color = m_Color[CutinScript.m_PlayerNo];

		SoundManager.Get.PlaySE("katana-gesture2");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
