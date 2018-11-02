using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class BloomInit : MonoBehaviour {
	PostProcessingBehaviour behaviour;
	public float m_InitBloom = 0.0f;
	// Use this for initialization
	void Start()
	{
		behaviour = GameObject.Find("Main Camera").GetComponent<PostProcessingBehaviour>();

		var Settings = behaviour.profile.bloom.settings;
		Settings.bloom.intensity = m_InitBloom;
		behaviour.profile.bloom.settings = Settings;
	}
}
