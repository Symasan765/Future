using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutinImage : MonoBehaviour {

	public Sprite[] m_Image = new Sprite[5];

	// Use this for initialization
	void Start () {
		GetComponent<Image>().sprite = m_Image[CutinScript.m_PlayerNo];
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
