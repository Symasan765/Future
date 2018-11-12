using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EviRainbow : MonoBehaviour
{
	Material[] m_Materials;
	Color[] rainbows;
	public float Threshold = 4.0f;
	float InitFloat;		// これにより初期の色が変わる

	// Use this for initialization
	void Start()
	{
		m_Materials = GetComponent<Renderer>().materials;
		InitFloat = Random.Range(0.0f, 6.0f);

		// 虹色データ
		rainbows = new Color[]{Color.red,new Color(1.0f, 0.65f, 0.0f),new Color(1.00f, 1.00f, 0.00f), new Color(0.00f, 0.50f, 0.00f),
		new Color(0.00f, 1.00f, 1.00f),new Color(0.00f, 0.00f, 1.00f),new Color(0.50f, 0.00f, 0.50f)};
	}

	// Update is called once per frame
	void Update()
	{
		float gTime = Time.time * Threshold + InitFloat;			// グローバルタイムを元にした閾値
		int idx = (int)gTime;					 // 小数点切り捨て
		float t = gTime - (float)idx;       // 小数点のみ

		Color newColor = rainbows[idx % rainbows.Length];
		Color oldColor = rainbows[(idx - 1 + rainbows.Length) % rainbows.Length];

		Color nowColor = Color.Lerp(oldColor, newColor, t);

		for (int i = 0; i < m_Materials.Length; i++)
		{
			Material material = m_Materials[i];
			material.SetColor("_Outline_Color", nowColor);
			material.SetFloat("_Outline_Width", 70.0f);
			material.SetFloat("_Farthest_Distance", 50.0f);
		}
	}
}
