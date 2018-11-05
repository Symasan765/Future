using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stop : MonoBehaviour {
    [SerializeField]
    SceneChanger sc;

	// Use this for initialization
	void Start () {
        Time.timeScale = 0.0f;
        sc = FindObjectOfType<SceneChanger>();
	}
	
	// Update is called once per frame
	void Update () {
        if (XPad.Get.GetTrigger(XPad.KeyData.A, 0)) {
            Time.timeScale = 0.0f;
        }

        if (XPad.Get.GetTrigger(XPad.KeyData.B, 0)) {
            Time.timeScale = 1.0f;
        }
	}
}
