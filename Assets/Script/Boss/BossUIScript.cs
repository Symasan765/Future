using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossUIScript : MonoBehaviour {

	float m_VibrationSec = 0.66f;

	float m_TimeCnt = 0.0f;

	Transform m_RectTransform;
	public RectTransform m_HPImageTransform;
	public RectTransform m_NowImageTransform;

	Vector3 m_VibPos = Vector3.zero;
	Vector3 m_VivScale = Vector3.one;
	Vector3 m_ImageScale = Vector3.one;
	Vector3 m_InitPos;

	// Use this for initialization
	void Start () {
		m_RectTransform = GetComponent<Transform>();
		m_InitPos = m_RectTransform.position;
		m_RectTransform.localScale = new Vector3(1.0f, 1.0f);
		m_HPImageTransform.localScale = m_NowImageTransform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		if(m_TimeCnt > 0.0f)
		{
			UIUpdate();
		}
		else
		{
			
		}
	}

	void UIUpdate()
	{
		if(m_TimeCnt > m_VibrationSec)
		{
			// 振動処理
			float value = 2.0f;
			Vector3 pos = m_RectTransform.position;
			pos.x += Random.Range(-value, value);
			pos.y += Random.Range(-value, value);
			pos.z += Random.Range(-value, value);
			m_VibPos = pos;

			m_RectTransform.position = m_VibPos;

			// 次にUI全体の拡大を行う
			float t = (m_TimeCnt - m_VibrationSec) / m_VibrationSec;
			m_VivScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.01f, t);
			m_RectTransform.localScale = m_VivScale;
		}
		else
		{
			float t = (m_TimeCnt) / m_VibrationSec;
			t = 1.0f - t;
			// 振動で揺れた物を元に戻す
			m_RectTransform.position = Vector3.Lerp(m_VibPos, m_InitPos, t);

			// 拡大したものを元に戻す
			m_RectTransform.localScale = Vector3.Lerp(m_VivScale, Vector3.one, t);

			// ゲージ長さの調整
			m_HPImageTransform.localScale = Vector3.Lerp(m_ImageScale, m_NowImageTransform.localScale, t);
		}

		m_TimeCnt -= Time.deltaTime;
		// すべての処理が終了したので元に戻す処理
		if (m_TimeCnt < 0.0f)
		{
			m_RectTransform.localScale = Vector3.one;
			m_RectTransform.position = m_InitPos;
			m_HPImageTransform.localScale = m_NowImageTransform.localScale;
			m_TimeCnt = 0.0f;
		}
	}

	public void IsDamage()
	{
		m_TimeCnt = m_VibrationSec * 2.0f;

		// 追加でダメージを受けた場合は下地を現在のダメージへ合わせる
		m_ImageScale = m_HPImageTransform.localScale;
	}
}
