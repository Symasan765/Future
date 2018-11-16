using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountEvidence : MonoBehaviour {

	StageChangeManager stageChangeManager;

	Text text;

	void Start () 
	{
		text = GetComponent<Text>();
		stageChangeManager = GameObject.Find("StageChangeManager").GetComponent<StageChangeManager>();
	}
	
	void Update ()
	{
		text.text = stageChangeManager.GetNowSetEvidenceNum().ToString();	
	}
}
