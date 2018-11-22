using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class BlurChange : MonoBehaviour {

	public PostProcessingProfile m_Profile;
	public bool m_VibrationFlag = false;
	GameObject m_MainCameraObj;

	// Use this for initialization
	void Start () {
		m_MainCameraObj = GameObject.FindGameObjectWithTag("MainCamera");
		m_MainCameraObj.GetComponent<PostProcessingBehaviour>().profile = m_Profile;
	}
	
	// Update is called once per frame
	void Update () {
		// TODO ここで振動処理を行う
		if (m_VibrationFlag)
		{
			ShakeCamera.Impact(0.05f, 1.0f);
		}
	}
}
