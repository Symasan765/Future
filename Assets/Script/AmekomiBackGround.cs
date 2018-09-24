using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmekomiBackGround : MonoBehaviour {

	Vector3[] m_Vertices;
	Vector3[] m_VerticesTmp;
	Vector3[] m_VerticesOffset;
	MeshFilter m_MeshFilter;

	Vector3 m_Center = Vector3.zero;
	Vector3 m_NowCenterPos = Vector3.zero;

	float t = 1.0f;
	void Start()
	{
		m_MeshFilter = GetComponent<MeshFilter>();
		m_Vertices = m_MeshFilter.mesh.vertices;
		m_VerticesTmp = m_MeshFilter.mesh.vertices;
		m_VerticesOffset = new Vector3[m_Vertices.Length];

		for (int i = 0; i < m_Vertices.Length; i++)
		{
			m_VerticesOffset[i] = m_Vertices[i] - m_Center;
		}

		m_MeshFilter.mesh.vertices = m_Vertices;
	}
	
	private void Update()
	{
		m_NowCenterPos += new Vector3(-0.01f, 0.0f, -0.02f) * 2.0f;
		t += 0.01f;
		for (int i = 0; i < m_Vertices.Length; i++)
		{
			if(m_VerticesTmp[i].x == -0.5f)
			{
				m_Vertices[i] = (m_VerticesOffset[i] * t) + m_NowCenterPos;
				Debug.Log(m_VerticesOffset[i]);
			}
		}

		m_MeshFilter.mesh.vertices = m_Vertices;
	}
}
