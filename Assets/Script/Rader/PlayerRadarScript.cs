using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRadarScript : MonoBehaviour
{

	Player[] m_PlayerObjs;
	GameObject[] m_RadarObjs;
	GameObject[] m_FrameObjs;

	public GameObject m_LadarPrefab;
	public GameObject[] m_FramePrefab = new GameObject[4];

	public GameObject m_RadarScalerPrefab;
	public GameObject m_DamagePrefab;

	GameObject[] m_RadarScaler = new GameObject[4];
	GameObject[] m_DamageRadar = new GameObject[4];
	int[] m_PlayerOldHp = new int[4];		// 各プレイヤーの１つ前のフレームの体力を保存する

	public float m_MaxRadius = 20.0f;
	public float m_MinRadius = 5.0f;
	public Vector3 m_Offset;
	public float m_FrameSize = 0.1f;

	bool m_InitFlag = false;

	float[] m_DamageCnt = new float[4];
	float m_DamageTimeSec = 0.2f;
	float[] m_PlayerRadarCnt = new float[4];
	float m_PlayerTImeSec = 1.0f;

	float[] m_AllPlayerCnt = new float[4];
	float m_InviTimeSec = 2.0f;

	public void Init()
	{
		if (m_InitFlag == false)
		{
			GameObject[] obj = GameObject.FindGameObjectsWithTag("Player");

			if (obj == null || obj.Length == 0)
				return;     // 初期化は出来なかった

			m_PlayerObjs = new Player[obj.Length];
			m_RadarObjs = new GameObject[obj.Length];
			m_FrameObjs = new GameObject[obj.Length];

			for (int i = 0; i < obj.Length; i++)
			{
				m_PlayerObjs[i] = obj[i].GetComponent<Player>();
				m_RadarObjs[i] = Instantiate(m_LadarPrefab);
				m_RadarObjs[i].transform.localScale = new Vector3(m_MinRadius, m_MinRadius, 0.0f);
				m_FrameObjs[i] = Instantiate(m_FramePrefab[i]);
				m_RadarScaler[i] = Instantiate(m_RadarScalerPrefab);
				m_PlayerOldHp[i] = m_PlayerObjs[i].GetMentalGauge();
				m_DamageRadar[i] = Instantiate(m_DamagePrefab);
				m_AllPlayerCnt[i] = 0.0f;
				m_DamageCnt[i] = 0.0f;
				m_PlayerRadarCnt[i] = 0.0f;
			}
			if (m_PlayerObjs != null)
				m_InitFlag = true;
		}
	}

	// Use this for initialization
	void Start()
	{
		Init();
	}

	// Update is called once per frame
	void Update()
	{
		if (m_InitFlag)
		{
			for (int i = 0; i < m_PlayerObjs.Length; i++)
			{
				RadarUpdate(i);
			}
		}
		else
		{
			Init();
		}
	}

	void RadarUpdate(int i)
	{
		if (m_InitFlag)
		{
			// レーダーやメモリの位置をプレイヤー位置に変更
			m_RadarObjs[i].transform.position = m_PlayerObjs[i].gameObject.transform.position + m_Offset;
			m_FrameObjs[i].transform.position = m_PlayerObjs[i].gameObject.transform.position + m_Offset;
			m_DamageRadar[i].transform.position = m_PlayerObjs[i].gameObject.transform.position + m_Offset;
			m_RadarScaler[i].transform.position = m_PlayerObjs[i].gameObject.transform.position;

			//int gage = m_PlayerObjs[i].GetMentalGauge();
			//float rate = gage / 100.0f;
			//float radius = Mathf.Lerp(m_MinRadius, m_MaxRadius, rate);
			//m_RadarObjs[i].transform.localScale = new Vector3(radius, radius, 0.0f);
			//m_FrameObjs[i].transform.localScale = new Vector3(radius + m_FrameSize, radius + m_FrameSize, 0.0f);
			StartDamage(i);
			RadarSizeUpdate(i);
		}
	}

	void RadarSizeUpdate(int i)
	{
		// プレイヤーがダメージを受けている最中
		if (InviTimeUpdate(i))
		{
			m_DamageCnt[i] += Time.deltaTime;
			if(m_DamageCnt[i] > m_DamageTimeSec)
			{
				m_DamageCnt[i] = m_DamageTimeSec;

				m_PlayerRadarCnt[i] += Time.deltaTime;
				if(m_PlayerRadarCnt[i] > m_PlayerTImeSec)
				{
					m_PlayerRadarCnt[i] = m_PlayerTImeSec;
				}
			}
		}
		else
		{
			// ダメージ受けてない
			m_DamageCnt[i] -= Time.deltaTime;
			if(m_DamageCnt[i] < 0.0f)
			{
				m_DamageCnt[i] = 0.0f;
			}

			m_PlayerRadarCnt[i] = 1.0f;
		}

		DamageRadarUpdare(i);
		DmageScalerUpdate(i);
		PlayerRadarUpdate(i);
	}

	void DamageRadarUpdare(int i)
	{
		float t = m_DamageCnt[i] / m_DamageTimeSec;
		float rate = m_PlayerObjs[i].GetMentalGauge() / 100.0f;
		float caluce = Mathf.Lerp(m_MinRadius, m_MaxRadius, rate);
		float radius = Mathf.Lerp(0.0f, caluce, t);
		m_DamageRadar[i].transform.localScale = new Vector3(radius, radius, 0.0f);
	}

	void DmageScalerUpdate(int i)
	{
		float t = m_DamageCnt[i] / m_DamageTimeSec;
		float radius = Mathf.Lerp(0.0f, m_MaxRadius, t);
		m_RadarScaler[i].transform.localScale = new Vector3(radius, radius, 0.0f);
	}

	void PlayerRadarUpdate(int i)
	{
		// プレイヤーレーダーとレーダー枠更新
		float t = m_PlayerRadarCnt[i] / m_PlayerTImeSec;
		Vector3 nowRadius = m_RadarObjs[i].transform.localScale;       // 現在のスケール値

		int gage = m_PlayerObjs[i].GetMentalGauge();
		float rate = gage / 100.0f;
		float newRadius = Mathf.Lerp(m_MinRadius, m_MaxRadius, rate);   // 目指すべきスケール値

		Vector3 radius = new Vector3(Mathf.Lerp(nowRadius.x, newRadius, t), Mathf.Lerp(nowRadius.x, newRadius, t), 0.0f);
		m_RadarObjs[i].transform.localScale = radius;
		m_FrameObjs[i].transform.localScale = new Vector3(radius.x + m_FrameSize, radius.y + m_FrameSize, 0.0f);
	}


	void StartDamage(int i)
	{
		if(Input.GetKeyDown(KeyCode.V))
			m_AllPlayerCnt[i] = m_InviTimeSec;

		if (m_PlayerObjs[i].IsDamageTrigger())
		{
			Debug.Log("ダメージ");
			m_AllPlayerCnt[i] = m_InviTimeSec;
		}
	}

	bool InviTimeUpdate(int i)
	{
		m_AllPlayerCnt[i] -= Time.deltaTime;

		// ダメージ中
		if(m_AllPlayerCnt[i] > 0.0f)
		{
			return true;
		}

		m_AllPlayerCnt[i] = 0.0f;
		return false;
	}
}