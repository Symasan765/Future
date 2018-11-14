using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EviRainbow : MonoBehaviour
{
	Material[] m_Materials;
	Color[] rainbows;
	public float Threshold = 4.0f;
	float InitFloat;        // これにより初期の色が変わる

	bool m_RainbowFlag = true;
	float m_TimeCnt = 0.0f;
	float m_ChangeSec = 0.5f;

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
		float gTime = Time.time * Threshold + InitFloat;            // グローバルタイムを元にした閾値
		int idx = (int)gTime;                    // 小数点切り捨て
		float t = gTime - (float)idx;       // 小数点のみ

		Color newColor = rainbows[idx % rainbows.Length];
		Color oldColor = rainbows[(idx - 1 + rainbows.Length) % rainbows.Length];

		Color nowColor = Color.Lerp(oldColor, newColor, t);

		if (m_RainbowFlag == false)
		{
			m_TimeCnt -= Time.deltaTime;
			if (m_TimeCnt <= 0.0f)
				m_TimeCnt = 0.0f;
		}
		else
		{
			m_TimeCnt += Time.deltaTime;
			if (m_TimeCnt > m_ChangeSec) m_TimeCnt = m_ChangeSec;
		}

		float grayT = m_TimeCnt / m_ChangeSec;
		nowColor = Color.Lerp(Color.gray, nowColor, grayT);



		for (int i = 0; i < m_Materials.Length; i++)
		{
			Material material = m_Materials[i];
			material.SetColor("_Outline_Color", nowColor);
			material.SetFloat("_Outline_Width", 20.0f);
			material.SetFloat("_Farthest_Distance", 50.0f);
		}
	}

	/// <summary>
	/// 輪郭線のカラーを変える処理
	/// </summary>
	/// <param name="rainbowFlag"></param>
	public void IsRainbow(bool rainbowFlag)
	{
		m_RainbowFlag = rainbowFlag;
		if (rainbowFlag)
			m_TimeCnt = 0.0f;
		else
			m_TimeCnt = m_ChangeSec;
	}
}
