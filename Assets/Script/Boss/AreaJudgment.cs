using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaJudgment : MonoBehaviour {

	// ステージ中のエリアを判定するためのスクリプト
	public int m_AreaNo = 0;
	public int[] m_SelectAttackNo;      // ここに攻撃をしかけるエリア番号を設置していく

	int m_AttackNum = 0;

	private void Start()
	{
		GetComponent<MeshRenderer>().enabled = false;
		if(m_SelectAttackNo == null || m_SelectAttackNo.Length == 0)
		{
			Debug.LogError("注意！！攻撃エリアNo" + m_AreaNo + "の配列設定がされてない!エディタ上でのみ仮で初期化するけどビルド時は実行されないので絶対に対処すること！これを見つけた時はステージ制作者へ報告！");
#if UNITY_EDITOR
			m_SelectAttackNo = new int[1];
			m_SelectAttackNo[0] = 0;
#endif
		}
	}

	/// <summary>
	/// このエリアでの次の攻撃IDを返す関数
	/// </summary>
	/// <returns></returns>
	public int NextAttackNo()
	{
		int no = m_SelectAttackNo[m_AttackNum % m_SelectAttackNo.Length];
		Debug.Log("NextAttack呼ばれた");
		m_AttackNum++;
		return no;
	}
}
