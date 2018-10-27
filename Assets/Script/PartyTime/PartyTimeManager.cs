using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyTimeManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (XPad.Get.GetTrigger(XPad.KeyData.UP, 0))
			ShakeCamera.Impact(0.1f, 1.0f);
	}
}
