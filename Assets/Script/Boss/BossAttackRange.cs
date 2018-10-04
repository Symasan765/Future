using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackRange : MonoBehaviour {

	Vector3 m_AttackPos;		// 攻撃中心位置
	Vector2 m_Range;          // 攻撃範囲
	float m_AttackTime;      // 攻撃秒数
	float m_AttackCount;        // 攻撃までのカウント

	GameObject m_AttackRangeBoard;

	public Material m_RangeMaterial;

	private void Start()
	{
		m_AttackRangeBoard = GameObject.CreatePrimitive(PrimitiveType.Cube);
		m_AttackRangeBoard.transform.localScale = new Vector3(1.0f, 1.0f, 0.0f);

		AttackCommand(Vector3.zero, new Vector2(3.0f, 8.0f), 32.0f);
		m_AttackRangeBoard.GetComponent<Renderer>().material = m_RangeMaterial;
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
	public void AttackCommand(Vector3 atackPos,Vector2 range,float atackTime)
	{
		m_AttackPos = atackPos;
		m_Range = range;
		m_AttackTime = atackTime;
		m_AttackCount = 0.0f;

		m_AttackRangeBoard.transform.position = m_AttackPos;
		m_AttackRangeBoard.transform.localScale = new Vector3(range.x, range.y, 0.0f);
	}

	void AttackUpdate()
	{
		if(m_AttackCount < m_AttackTime)
		{
			// 点滅処理
			RangeFlashing();

			m_AttackCount += Time.deltaTime;
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
}
