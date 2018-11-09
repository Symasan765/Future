using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class BloomControl : MonoBehaviour {
	PostProcessingBehaviour behaviour;
	public float m_LightEmissionTime = 0.4f;
	public float m_MaxBloom = 1.0f;
	float m_TimeCnt;

	// Use this for initialization
	void Start () {
		behaviour = GameObject.Find("Camera").GetComponent<PostProcessingBehaviour>();

		var Settings = behaviour.profile.bloom.settings;
		Settings.bloom.intensity = 0.0f;
		behaviour.profile.bloom.settings = Settings;

		m_TimeCnt = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		float t = m_TimeCnt / m_LightEmissionTime;
		t = t * t * (3.0f - 2.0f * t);

		var Settings = behaviour.profile.bloom.settings;
		Settings.bloom.intensity = t * m_MaxBloom;
		behaviour.profile.bloom.settings = Settings;

		m_TimeCnt += Time.deltaTime;

		if (m_TimeCnt > m_LightEmissionTime)
			m_TimeCnt = m_LightEmissionTime;
	}
}
