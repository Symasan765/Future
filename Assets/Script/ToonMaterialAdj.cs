using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToonMaterialAdj : MonoBehaviour
{

	Material[] mats;
	Camera m_Camera;

	public float m_MinDistance = 1.0f;
	public float m_MaxDistance = 11.0f;
	public float m_MinWidth = 3.0f;
	public float m_MaxWidth = 100.0f;

	public float m_FlashingSec = 0.1f;

	SkinnedMeshRenderer render;
	Player m_PlayerScript;
	int playerIndex = -1;

	public Color[] m_PlayerColor = new Color[4];

	// Use this for initialization
	void Start()
	{
		mats = GetComponent<Renderer>().materials;
		m_Camera = GameObject.Find("Main Camera").GetComponent<Camera>();

		render = GetComponent<SkinnedMeshRenderer>();
		m_PlayerScript = transform.root.gameObject.GetComponent<Player>();
		if (m_PlayerScript != null)
		{
			playerIndex = m_PlayerScript.PlayerIndex;
			m_PlayerColor[0] = new Color(0.0f, 0.0f, 1.0f);
			m_PlayerColor[1] = new Color(1.0f, 0.0f, 0.0f);
			m_PlayerColor[2] = new Color(0.0f, 1.0f, 0.0f);
			m_PlayerColor[3] = new Color(1.0f, 1.0f, 0.0f);
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (m_PlayerScript != null)
		{
			bool invincibleFlag = false;
			invincibleFlag = m_PlayerScript.IsInvincible();
			if (invincibleFlag)
			{
				InvincibleFlashing();
			}
			else
			{
				render.enabled = true;
			}
		}

		OutlineAdjustment();
	}

	void OutlineAdjustment()
	{
		// カメラに近づけばアウトラインを短くして遠くなれば大きくする
		float cameraSize = m_Camera.orthographicSize;
		
		// 仮に距離0の時に1、距離10の時に50の太さに変えるとする
		Vector3 Difference = m_Camera.transform.position - transform.position;

		// 仮に距離0の時に1、距離10の時に50の太さに変えるとする
		float t = (cameraSize - m_MinDistance) / (m_MaxDistance - m_MinDistance);
		float val = Mathf.Lerp(m_MinWidth, m_MaxWidth, t);

		if (transform.tag == "SYOUKO")
			Debug.Log("証拠カメラ" + val);

		foreach (var mat in mats)
		{
			mat.SetFloat("_Outline_Width", val);

			if (m_PlayerScript != null)
				mat.SetColor("_Outline_Color", m_PlayerColor[playerIndex]);

			mat.SetFloat("_Farthest_Distance", Difference.magnitude + 4.0f);

			mat.SetFloat("_Nearest_Distance", Difference.magnitude - 1.0f);

		}
	}

	void InvincibleFlashing()
	{
		int t = (int)(Time.time / m_FlashingSec);
		if (t % 3 == 0)
		{
			render.enabled = false;
		}
		else
		{
			render.enabled = true;
		}
	}

	void PlayerOutline()
	{
		if (m_PlayerScript != null)
		{

		}
	}
}
