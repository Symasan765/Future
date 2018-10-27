﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyTimeManager : MonoBehaviour {
	public Material SkyMaterial;
	float SkyChangeTime = 0.5f;
	public float m_FeverTimeSec = 10.0f;
	FeverManager m_FeverManager;
	BossAttackManager m_BossAttackManager;

	float m_BossSky = 1.98f;
	float m_PartySky = 2.55f;

	// スカイドームのカラー変更用
	float m_SkyTimeSec;
	float m_SkySrc;		// 変更元
	float m_SkyDsc;		// 変更先

	enum PartyState
	{
		BossAttack = 0,
		PlayerAttack
	}

	PartyState m_NowState;
	float m_FeverCnt;

	// Use this for initialization
	void Start () {
		m_NowState = PartyState.BossAttack;

		m_SkyTimeSec = SkyChangeTime;
		// ボス空へ移動が終わったていで進めてる
		m_SkySrc = m_PartySky;
		m_SkyDsc = m_BossSky;

		SkyMaterial.mainTextureScale = new Vector2(m_SkyDsc, 1.0f);

		m_FeverManager = GameObject.Find("FeverManager").GetComponent<FeverManager>();
		m_BossAttackManager = GameObject.Find("BossAttackManager").GetComponent<BossAttackManager>();
	}
	
	// Update is called once per frame
	void Update () {
		SkyUpdate();

		switch (m_NowState)
		{
			case PartyState.BossAttack:
				BossAttackTurn();
				break;
			case PartyState.PlayerAttack:
				PlayerAttackTurn();
				break;
			default:
				Debug.Log("ゲームの状態管理で異常値を検出してるよ");
				break;
		}
	}

	void BossAttackTurn()
	{
		if (XPad.Get.GetTrigger(XPad.KeyData.UP,0))
		{
			SwitchState(PartyState.PlayerAttack);
		}
	}

	void PlayerAttackTurn()
	{
		// フィーバータイムを過ぎればボス攻撃状態に遷移
		m_FeverCnt += Time.deltaTime;
		if(m_FeverCnt > m_FeverTimeSec)
		{
			SwitchState(PartyState.BossAttack);
			// TODO ここでボスの攻撃再開を指示する
		}
	}

	// これ呼べば現在のステートが変わる
	void SwitchState(PartyState newState)
	{
		// ここにそれぞれの処理を記述する
		switch (newState)
		{
			case PartyState.BossAttack:
				m_BossAttackManager.BossBehaviorSwitching(true);
				SkySwitch();
				break;
			case PartyState.PlayerAttack:
				m_BossAttackManager.BossBehaviorSwitching(false);
				m_FeverManager.StartFever(m_FeverTimeSec);
				SkySwitch();
				break;
			default:
				Debug.Log("ゲームの状態管理で異常値を検出してるよ");
				break;
		}

		m_NowState = newState;
	}

	void SkyUpdate()
	{
		if(m_SkyTimeSec < SkyChangeTime)
		{
			float t = m_SkyTimeSec / SkyChangeTime;
			SkyMaterial.mainTextureScale = new Vector2(Mathf.Lerp(m_SkySrc, m_SkyDsc, t),1.0f);

			m_SkyTimeSec += Time.deltaTime;
			if (m_SkyTimeSec > SkyChangeTime)
			{

			}
		}
	}

	void SkySwitch()
	{
		float tmp = m_SkySrc;     // 変更元
		m_SkySrc = m_SkyDsc;     // 変更先
		m_SkyDsc = tmp;

		m_SkyTimeSec = 0.0f;
	}

	/// <summary>
	/// ゲーム中でフィーバータイムにしたい場合に呼ぶ関数
	/// </summary>
	public void LetsParty()
	{
		// ボスが攻撃中ならダウンさせてパーティを始める
		if(m_NowState == PartyState.BossAttack)
		{
			m_FeverManager.StartFever(m_FeverTimeSec);
			m_SkyTimeSec = m_FeverTimeSec;
			m_FeverCnt = 0;
			SwitchState(PartyState.PlayerAttack);
		}
	}
}