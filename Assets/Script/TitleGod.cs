using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleGod : MonoBehaviour {

	public GameObject m_PushAUI;

	public GameObject m_FadeOutObj;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate() {
		if (m_PushAUI.active == true)
		{
			// タイムラインが終了している
			if (XPad.Get.AnyoneTrigger(XPad.KeyData.A))
			{
				m_FadeOutObj.SetActive(true);
			}

			// フェードアウトも終わった
			if (m_FadeOutObj.GetComponent<FadeObj>().IsEnd())
			{
				SoundManager.Get.StopBGM();
				GetComponent<SceneBackLoder>().SceneChange();
			}
		}
	}
}
