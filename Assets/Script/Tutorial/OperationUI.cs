using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationUI : MonoBehaviour
{

	bool m_HitFlag = false;

	float m_TimeCnt = 0.0f;     // こいつを+-して調整する

	public float m_SwitchTimeSec = 0.4f;

	GameObject m_Image0;
	GameObject m_Image1;

	// Use this for initialization
	void Start()
	{
		// 子オブジェクトを取得する
		m_Image0 = transform.GetChild(0).gameObject;
		m_Image1 = transform.GetChild(1).gameObject;

		// シーン上で見やすいようにしてるメッシュを切る
		GetComponent<MeshRenderer>().enabled = false;
	}

	// Update is called once per frame
	void Update()
	{
		HitCheck();
		ImageSwitch();
	}

	void HitCheck()
	{
		m_HitFlag = false;  // 当たっていない前提
		RaycastHit[] hitInfo = Physics.BoxCastAll(transform.position, transform.localScale, Vector3.back, Quaternion.identity, 15.0f);

		// 当たったオブジェクトからプレイヤーを探してダメージ処理
		for (int i = 0; i < hitInfo.Length; i++)
		{
			// プレイヤーに接触している
			if (hitInfo[i].collider.gameObject.tag == "Player")
			{
				m_HitFlag = true;
			}
		}
	}

	void ImageSwitch()
	{
		// プレイヤーが範囲に入ってる
		if (m_HitFlag)
		{
			m_TimeCnt += Time.deltaTime;
			if (m_TimeCnt > m_SwitchTimeSec) m_TimeCnt = m_SwitchTimeSec;
		}
		// プレイヤーが入ってない
		else
		{
			m_TimeCnt -= Time.deltaTime;
			if (m_TimeCnt < 0.0f) m_TimeCnt = 0.0f;
		}

		float t = m_TimeCnt / m_SwitchTimeSec;
		t = t * t * (3.0f - 2.0f * t);
		m_Image0.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
		m_Image1.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
	}
}
