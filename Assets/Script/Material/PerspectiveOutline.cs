using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveOutline : MonoBehaviour {
	Material[] mats;
	Camera m_Camera;

	public float m_MaxDistance = 10.0f;
	public float m_MinWidth = 3.0f;
	public float m_MaxWidth = 100.0f;

	// Use this for initialization
	void Start()
	{
		mats = GetComponent<Renderer>().materials;
		m_Camera = GameObject.Find("Main Camera").GetComponent<Camera>();
	}

	// Update is called once per frame
	void Update()
	{
		// カメラに近づけばアウトラインを短くして遠くなれば大きくする
		Vector3 cameraPos = m_Camera.transform.position;
		Vector3 objPos = transform.position;

		// 仮に距離0の時に1、距離10の時に50の太さに変えるとする
		Vector3 Difference = cameraPos - objPos;
		float t = Difference.magnitude / m_MaxDistance;
		float val = Mathf.Lerp(m_MinWidth, m_MaxWidth, t);

		foreach (var mat in mats)
		{
			mat.SetFloat("_Outline_Width", val);

			mat.SetFloat("_Farthest_Distance", Difference.magnitude + 4.0f);

			mat.SetFloat("_Nearest_Distance", Difference.magnitude - 1.0f);

		}

	}
}
