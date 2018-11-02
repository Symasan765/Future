using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : MonoBehaviour {

	PlayableDirector m_Scene1;
	PlayableDirector m_Scene2;

	GameObject m_Parent1;
	GameObject m_Parent2;

	bool IsEnd1;
	// Use this for initialization
	void Start () {
		IsEnd1 = false;
		m_Scene1 = GameObject.Find("Scene1").GetComponent<PlayableDirector>();
		m_Scene2 = GameObject.Find("Scene2").GetComponent<PlayableDirector>();

		m_Parent1 = m_Scene1.gameObject.transform.root.gameObject;
		m_Parent2 = m_Scene2.gameObject.transform.root.gameObject;

		m_Parent2.SetActive(false);

		m_Scene1.Play();
	}
	
	// Update is called once per frame
	void Update () {
		if (!IsEnd1 && m_Scene1.state != PlayState.Playing)
		{
			IsEnd1 = true;
			m_Scene1.Stop();
			m_Parent1.SetActive(false);
			m_Parent2.SetActive(true);
			m_Scene2.Play();
		}
	}
}
