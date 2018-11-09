using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeObj : MonoBehaviour
{

	public Color m_InitColor;

	public enum Type
	{
		FadeOut,
		FadeIn
	}

	public Type m_Type = Type.FadeOut;
	public float m_ChangeSec = 1.0f;

	Image m_Image;
	float m_In = 0.0f;
	float m_Out = 0.0f;
	float m_TimeCnt = 0.0f;

	bool m_EndFlag = false;

	// Use this for initialization
	void Start()
	{
		m_Image = transform.Find("Image").GetComponent<Image>();

		switch (m_Type)
		{
			case Type.FadeOut:
				m_In = 0.0f;
				m_Out = 1.0f;
				break;
			case Type.FadeIn:
				m_In = 1.0f;
				m_Out = 0.0f;
				break;
		}

		m_Image.color = new Color(m_InitColor.r, m_InitColor.g, m_InitColor.b, m_In);
	}

	// Update is called once per frame
	void Update()
	{
		float t = m_TimeCnt / m_ChangeSec;
		if(t > 1.0f)
		{
			t = 1.0f;
			m_EndFlag = true;
		}

		m_Image.color = new Color(m_InitColor.r, m_InitColor.g, m_InitColor.b, Mathf.Lerp(m_In,m_Out,t));
		m_TimeCnt += Time.deltaTime;
	}


	public bool IsEnd()
	{
		return m_EndFlag;
	}

	public void Reverse()
	{
		m_TimeCnt = 0.0f;
		float tmp = m_In;
		m_In = m_Out;
		m_Out = tmp;
		m_EndFlag = false;
	}
}