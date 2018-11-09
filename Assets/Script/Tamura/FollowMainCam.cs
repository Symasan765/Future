using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMainCam : MonoBehaviour {
	void Update () {
		var maincam = Camera.main;
		if(maincam != null)
		{
			this.transform.position = maincam.transform.position;
		}
	}
}
