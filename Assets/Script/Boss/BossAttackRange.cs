﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackRange : MonoBehaviour {

	Vector3 m_AttackPos;		// 攻撃中心位置
	Vector2 m_Range;          // 攻撃範囲
	float m_AttackTime;      // 攻撃秒数
	float m_AttackCount;        // 攻撃までのカウント

	public GameObject m_AttackRangePrefab;
	GameObject m_AttackRangeBoard;

	bool m_AttackFlag;

	private void Start()
	{
		m_AttackFlag = false;
		m_AttackRangeBoard = Instantiate(m_AttackRangePrefab);
		m_AttackRangeBoard.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		AttackCommand(new Vector2(-28.8f,-8.0f), new Vector2(3.0f, 8.0f), 2.0f);
	}

	// Update is called once per frame
	void Update () {
		AttackUpdate();
	}

	/// <summary>
	/// 攻撃命令を出して指定秒後にその範囲を攻撃する
	/// </summary>
	/// <param name="atackPos">攻撃中心座標</param>
	/// <param name="range">攻撃範囲</param>
	/// <param name="atackTime">攻撃までの時間</param>
	public void AttackCommand(Vector2 atackPos,Vector2 range,float atackTime)
	{
		m_AttackPos = new Vector3(atackPos.x, atackPos.y,0.5f);
		m_Range = range;
		m_AttackTime = atackTime;
		m_AttackCount = 0.0f;

		m_AttackRangeBoard.transform.position = m_AttackPos;
		m_AttackRangeBoard.transform.localScale = new Vector3(range.x, range.y, 1.0f);
	}

	void AttackUpdate()
	{
		if(m_AttackCount < m_AttackTime)
		{
			// 点滅処理
			RangeFlashing();

			m_AttackCount += Time.deltaTime;
			m_AttackFlag = true;
		}
		else
		{
			AttackPlayer();
		}
	}

	void RangeFlashing()
	{
		// TODO 現在適当に点滅
		int cnt = (int)(m_AttackCount / 0.1f);
		if(cnt % 3 == 0)
		{
			m_AttackRangeBoard.active = false;
		}
		else
		{
			m_AttackRangeBoard.active = true;
		}
	}

	// プレイヤーに攻撃を仕掛ける関数
	void AttackPlayer()
	{
		if (m_AttackFlag)
		{
			RaycastHit[] hitInfo = Physics.BoxCastAll(m_AttackPos, new Vector3(m_Range.x / 2.0f, m_Range.y / 2.0f, 0.0f), Vector3.back, Quaternion.identity, 5.0f);
			
			// 当たったオブジェクトからプレイヤーを探してダメージ処理
			for(int i = 0; i < hitInfo.Length; i++)
			{
				if (hitInfo[i].collider.gameObject.tag == "Player")
					hitInfo[i].collider.gameObject.GetComponent<Player>().HitBossAttack();
			}
			m_AttackFlag = false;
		}
	}

	
}
