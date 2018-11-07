using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundDirector : MonoBehaviour {

	public GameObject[] m_FeverStartBGM;
	public GameObject[] m_FeverMiddleBGM;
	public GameObject[] m_FeverEndBGM;

	public GameObject[] m_BossStartBGM;
	public GameObject[] m_BossMiddleBGM;
	public GameObject[] m_BossEndBGM;

	BossAttackManager m_BossManager;

	int m_FeverStartCnt = 0;
	int m_FeverMiddleCnt = 0;
	int m_FeverEndCnt = 0;

	int m_BossStartCnt = 0;
	int m_BossMiddleCnt = 0;
	int m_BossEndCnt = 0;

	bool EnemyAttack = true;	// ボスの攻撃からスタート
	float m_BossHP = 0;
	float m_BossInitHp = 0;

	// Use this for initialization
	void Start () {
		m_BossManager = GameObject.Find("BossAttackManager").GetComponent<BossAttackManager>();
		m_BossInitHp = m_BossManager.m_BossMaxDamage;
		m_BossHP = m_BossInitHp;

		if (m_FeverStartBGM.Length == 0)
			Debug.LogError("BGMが登録されてない");
		if (m_FeverMiddleBGM.Length == 0)
			Debug.LogError("BGMが登録されてない");
		if (m_FeverEndBGM.Length == 0)
			Debug.LogError("BGMが登録されてない");

		if (m_BossStartBGM.Length == 0)
			Debug.LogError("BGMが登録されてない");
		if (m_BossMiddleBGM.Length == 0)
			Debug.LogError("BGMが登録されてない");
		if (m_BossEndBGM.Length == 0)
			Debug.LogError("BGMが登録されてない");
	}
	
	public AudioSource NextBGM()
	{
		AudioSource Ret = null;
		if (EnemyAttack)
		{
			// ボスの攻撃中
			Ret = m_BossBGM();
		}
		else
		{
			// フィーバータイム中
			Ret = m_FeverBGM();
		}

		EnemyAttack = !EnemyAttack; // フラグを切替
		Ret.Stop();
		return Ret;
	}

	AudioSource m_FeverBGM()
	{
		var condition = m_BossManager.GetCondition();
		AudioSource Ret = null;

		switch (condition)
		{
			case BossAttackManager.BossCondition.Margin:
				Ret = m_FeverStartBGM[m_FeverStartCnt % m_FeverStartBGM.Length].GetComponent<AudioSource>();
				m_FeverStartCnt++;
				break;
			case BossAttackManager.BossCondition.Spicy:
				Ret = m_FeverMiddleBGM[m_FeverMiddleCnt % m_FeverMiddleBGM.Length].GetComponent<AudioSource>();
				m_FeverMiddleCnt++;
				break;
			case BossAttackManager.BossCondition.Dying:
				Ret = m_FeverEndBGM[m_FeverEndCnt % m_FeverEndBGM.Length].GetComponent<AudioSource>();
				m_FeverEndCnt++;
				break;
		}

		return Ret;
	}

	AudioSource m_BossBGM()
	{
		var condition = m_BossManager.GetCondition();
		AudioSource Ret = null;

		switch (condition)
		{
			case BossAttackManager.BossCondition.Margin:
				Ret = m_BossStartBGM[m_BossStartCnt % m_BossStartBGM.Length].GetComponent<AudioSource>();
				m_BossStartCnt++;
				break;
			case BossAttackManager.BossCondition.Spicy:
				Ret = m_BossMiddleBGM[m_BossMiddleCnt % m_BossMiddleBGM.Length].GetComponent<AudioSource>();
				m_BossMiddleCnt++;
				break;
			case BossAttackManager.BossCondition.Dying:
				Ret = m_BossEndBGM[m_BossEndCnt % m_BossEndBGM.Length].GetComponent<AudioSource>();
				m_BossEndCnt++;
				break;
		}

		return Ret;
	}
}