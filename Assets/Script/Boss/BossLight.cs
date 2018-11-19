using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLight : MonoBehaviour {

	public Material m_LightMaterial;

	Color m_InitColor;
	Color m_InitMainColor;

	float TimeCnt = 0;
	bool LightOn = false;
	float changeSec = 0.2f;

	// Use this for initialization
	void Start () {
		m_InitColor = Color.red;
		m_InitMainColor = Color.red;

		ColorUpdate();
	}

	private void Update()
	{
		ColorUpdate();
	}

	void ColorUpdate()
	{
		if (LightOn)
		{
			TimeCnt += Time.deltaTime;
			if (TimeCnt > changeSec) TimeCnt = changeSec;
		}
		else
		{
			TimeCnt -= Time.deltaTime;
			if (TimeCnt < 0.0f) TimeCnt = 0.0f;
		}

		float t = TimeCnt / changeSec;
		m_LightMaterial.SetColor("_Emissive_Color", Color.Lerp(Color.black,m_InitColor,t));
		m_LightMaterial.SetColor("_BaseColor", Color.Lerp(Color.gray, m_InitMainColor, t));
		m_LightMaterial.SetColor("_1st_ShadeColor", Color.Lerp(Color.black, m_InitMainColor, t));
		m_LightMaterial.SetColor("_2nd_ShadeColor", Color.Lerp(Color.black, m_InitMainColor, t));
	}

	public void LightChage(bool on)
	{
		LightOn = on;
	}
}
