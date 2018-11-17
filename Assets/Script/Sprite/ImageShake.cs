using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageShake : MonoBehaviour
{

	Vector3 m_InitialScale;
	public Vector3 m_ShakeVolume = Vector3.one * 0.02f;       // 揺らす量
	public Vector3 m_ShakeTime = Vector3.one;           // 揺れるのが一周する時間

	Transform m_Transform;

	float timeCnt = 0.0f;

	// Use this for initialization
	void Start()
	{
		m_Transform = GetComponent<Transform>();
		m_InitialScale = transform.localScale;
	}

	// Update is called once per frame
	void Update()
	{
		// こいつらをsin or cos に渡せばその秒数での周期が出来る
		float shakeXT = (Mathf.Cos(Mathf.Deg2Rad * (timeCnt * 360.0f / m_ShakeTime.x)) + 1.0f) * 0.5f;
		float shakeYT = (Mathf.Cos(Mathf.Deg2Rad * (timeCnt * 360.0f / m_ShakeTime.y)) + 1.0f) * 0.5f;

		Vector3 lower = m_InitialScale - m_ShakeVolume;
		Vector3 upper = m_InitialScale + m_ShakeVolume;
		Vector3 scale = new Vector3(Mathf.Lerp(m_InitialScale.x, upper.x, shakeXT), Mathf.Lerp(m_InitialScale.y, upper.y, shakeYT), 1.0f);

		m_Transform.localScale = scale;

		timeCnt += Time.deltaTime;
	}
}
