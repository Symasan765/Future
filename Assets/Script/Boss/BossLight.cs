using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLight : MonoBehaviour {

	public Material m_LightMaterial;

	Color m_InitColor;
	Color m_InitMainColor;

	// Use this for initialization
	void Start () {
		m_InitColor = m_LightMaterial.GetColor("_Emissive_Color");
		m_InitMainColor = m_LightMaterial.GetColor("_BaseColor");
	}
	
	public void LightChage(bool on)
	{
		Color nowColor;
		Color mainColor;

		if (on)
		{
			nowColor = m_InitColor;
			mainColor = m_InitMainColor;
		}
		else
		{
			nowColor = Color.black;
			mainColor = Color.black;
		}

		m_LightMaterial.SetColor("_Emissive_Color", nowColor);
		m_LightMaterial.SetColor("_BaseColor", mainColor);
	}
}
