using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectScore : MonoBehaviour {

	// Use this for initialization
	void Start () {
		var textData = GetComponent<TextMesh>();
		textData.text = GameScore.m_ScoreStar;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
