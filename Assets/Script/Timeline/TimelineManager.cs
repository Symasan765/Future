using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TimelineManager : MonoBehaviour
{

	PlayableDirector m_Scene1;
	PlayableDirector m_Scene2;

	GameObject m_Parent1;
	GameObject m_Parent2;

	public GameObject m_BackLoadPrefab;
	SceneBackLoder m_BackLoder;

	bool IsEnd1;
	bool LoadFlag = false;

	float m_TimeCnt = 0.0f;
	// Use this for initialization
	void Start()
	{
		IsEnd1 = false;
		m_Scene1 = GameObject.Find("Scene1").GetComponent<PlayableDirector>();
		m_Scene2 = GameObject.Find("Scene2").GetComponent<PlayableDirector>();

		m_Parent1 = m_Scene1.gameObject.transform.root.gameObject;
		m_Parent2 = m_Scene2.gameObject.transform.root.gameObject;

		m_Parent2.SetActive(false);

		m_Scene1.Play();
	}

	// Update is called once per frame
	void Update()
	{
		if (!IsEnd1 && m_Scene1.state != PlayState.Playing)
		{
			IsEnd1 = true;
			m_Scene1.Stop();
			m_Parent1.SetActive(false);
			m_Parent2.SetActive(true);
			m_Scene2.Play();
		}

		if (IsEnd1 && m_Scene2.state != PlayState.Playing)
		{
			m_Parent2.SetActive(false);
			ResultTransManager.m_InTheGame = true;
			m_BackLoder.SceneChange();
		}

		// スタートキーとバックキーを同時に押されたらスキップさせる
		if(XPad.Get.AnyonePress(XPad.KeyData.START) && XPad.Get.AnyonePress(XPad.KeyData.BACK))
		{
			m_Parent2.SetActive(false);
			ResultTransManager.m_InTheGame = true;
			m_BackLoder.SceneChange();
		}

		m_TimeCnt += Time.deltaTime;
		if(m_TimeCnt > 0.0f)
		{
			if(LoadFlag == false)
			{
				m_BackLoder = Instantiate(m_BackLoadPrefab).GetComponent<SceneBackLoder>();
				LoadFlag = true;
			}
		}
	}

	IEnumerator GetRootObject()
	{
		SceneManager.LoadScene("A", LoadSceneMode.Additive);
		yield return null;
		Scene A_Scn = SceneManager.GetSceneByName("A");
		foreach (GameObject i in A_Scn.GetRootGameObjects())
		{
			Debug.Log(i.name);
		}
	}
}
