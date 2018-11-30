﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ResultManager : MonoBehaviour
{
	public GameObject m_PushAUI;

	// Resultで必要な情報をここで取得してリザルト画面へ反映させる

	public float m_Rank2Value = 5;
	public float m_Rank3Value = 10;
	public float m_ReflectTimeSec = 1.5f;       // 数値が上昇する数値
	float m_AttackNum = 0;      // メインゲームで攻撃した回数
	float m_DamageNum = 0;      // ダメージを受けた数

	public TextMesh m_AttacNumText;
	public TextMesh m_DamageNumText;
	public TextMesh m_RankStarText;

	public GameObject m_FadeOutObj;

	// こいつらにメインシーンでの数値を入れ込む
	int m_TargetBreakValue = 50;
	int m_TargetDamageValue = 10;

	float m_AttackTimeCnt = 0;
	float m_DamageTimeCnt = 0;
	
	bool m_AttackFlag = false;
	bool m_DamageFlag = false;

	// Use this for initialization
	void Start()
	{
		// TODO ここでダメージと攻撃回数をメインシーンから取得して変数に入れる
		m_AttackNum = GameScore.m_PlayerDownNum;
		m_DamageNum = GameScore.m_AttackNum;

		// スコア点数の計算(MAX100点方式を採用)
		m_TargetBreakValue = 100 - (GameScore.m_PlayerDownNum * 10);		// 1ダウン毎に１０点マイナス
		m_TargetDamageValue = 100 - (GameScore.m_AttackNum * 5);                                // 1被弾毎の５点マイナス

		int score = m_TargetBreakValue + m_TargetDamageValue;

		// 最高評価(200点満点)
		if(score > 170)
		{
			GameScore.m_ScoreStar = "★★★";
		}
		else if(score > 100)
		{
			GameScore.m_ScoreStar = "☆★★";
		}
		else if(score > 0)
		{
			GameScore.m_ScoreStar = "☆☆★";
		}
		else
		{
			GameScore.m_ScoreStar = "☆☆☆";
		}
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		AttackTextUpdate();
		DamageTextUpdate();

		if (m_PushAUI.active == true)
		{
			// タイムラインが終了している
			if (XPad.Get.AnyoneTrigger(XPad.KeyData.A))
			{
				m_FadeOutObj.SetActive(true);
			}

			// フェードアウトも終わった
			if (m_FadeOutObj.GetComponent<FadeObj>().IsEnd())
			{
				SoundManager.Get.StopBGM();
				GetComponent<SceneBackLoder>().SceneChange();
			}
		}
	}

	// 数値スコアを徐々上げていく
	void AttackTextUpdate()
	{
		if (m_AttackFlag)
		{
			// 数値が上昇しきってなかったら数値を上げる
			float t = m_AttackTimeCnt / m_ReflectTimeSec;
			if (t > 1.0f) t = 1.0f;
			int attackText = (int)(m_TargetBreakValue * t);
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
			if (t > 1.0f) t = 1.0f;
			int damageText = (int)(m_TargetDamageValue * t);
			m_DamageNumText.text = damageText.ToString();
			m_DamageTimeCnt += Time.deltaTime;
		}
	}

	public void AttacaActive()
	{
		m_AttackFlag = true;
	}

	public void DamageActive()
	{
		m_DamageFlag = true;
	}
}
