using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultManager : MonoBehaviour {

	// Resultで必要な情報をここで取得してリザルト画面へ反映させる

	public float m_Rank2Value = 5;
	public float m_Rank3Value = 10;
	public float m_ReflectTimeSec = 0.3f;       // 数値が上昇する数値
	float m_AttackNum = 0;      // メインゲームで攻撃した回数
	float m_DamageNum = 0;      // ダメージを受けた数

	public TextMesh m_AttacNumText;
	public TextMesh m_DamageNumText;
	public TextMesh m_RankStarText;

	// こいつらにメインシーンでの数値を入れ込む
	int m_TargetAttackValue = 0;
	int m_TargetDamageValue = 0;

	float m_AttackTimeCnt = 0;
	float m_DamageTimeCnt = 0;

	// TODO ここのフラグを外部からtrueに出来るオブジェクトを作成し、タイムラインから変更する
	bool m_AttackFlag = false;
	bool m_DamageFlag = false;

	// Use this for initialization
	void Start () {
		// TODO ここでダメージと攻撃回数をメインシーンから取得して変数に入れる
	}
	
	// Update is called once per frame
	void Update () {
		AttackTextUpdate();
		DamageTextUpdate();
	}

	// 数値スコアを徐々上げていく
	void AttackTextUpdate()
	{
		if (m_AttackFlag)
		{
			// 数値が上昇しきってなかったら数値を上げる
			float t = m_AttackTimeCnt / m_ReflectTimeSec;
			int attackText = (int)(m_TargetAttackValue * t);
			m_AttacNumText.text = attackText.ToString();
			m_AttackTimeCnt += Time.deltaTime;
		}
	}

	void DamageTextUpdate()
	{
		if (m_DamageFlag)
		{
			// 数値が上昇しきってなかったら数値を上げる
			float t = m_DamageTimeCnt / m_ReflectTimeSec;
			int damageText = (int)(m_TargetDamageValue * t);
			m_DamageNumText.text = damageText.ToString();
			m_DamageTimeCnt += Time.deltaTime;
		}
	}
}
