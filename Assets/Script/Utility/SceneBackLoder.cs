using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBackLoder : MonoBehaviour {

	public string m_NextSceneName;

	private AsyncOperation async;
	string m_NowSceneName;
	bool m_DebugFlag = false;

	// Use this for initialization
	void Start()
	{
		m_NowSceneName = SceneManager.GetActiveScene().name;

		// シーンを読み込み始める
		ScenePreLoad(m_NextSceneName);
	}

	// Update is called once per frame
	void Update()
	{
		if(async.progress >= 0.9f)
		{
			if(m_DebugFlag == false)
			{
				Debug.Log(m_NextSceneName + "シーン読み込み完了。待機中");
				m_DebugFlag = true;
			}
		}
	}

	void ScenePreLoad(string changeScn)
	{
		async = SceneManager.LoadSceneAsync(changeScn, LoadSceneMode.Additive);
		async.allowSceneActivation = false;
		Debug.Log(changeScn + "シーンの読み込みを開始！");
	}

	IEnumerator SceneLoad()
	{
		m_NowSceneName = SceneManager.GetActiveScene().name;
		SceneManager.UnloadScene(m_NowSceneName);
		async.allowSceneActivation = true;
		yield return null;
	}

	/// <summary>
	/// シーン読み込みが終わっていればシーンを切り替える
	/// </summary>
	public void SceneChange()
	{
		
		StartCoroutine("SceneLoad");
	}
}
