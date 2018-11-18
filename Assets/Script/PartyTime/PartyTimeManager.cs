using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyTimeManager : MonoBehaviour {
	public Material SkyMaterial;
	float SkyChangeTime = 0.5f;
	public float m_FeverTimeSec = 10.0f;
	FeverManager m_FeverManager;
	BossAttackManager m_BossAttackManager;

	public GameObject m_BossAudioPrefab;
	public GameObject m_PlayerAudioPrefab;
	public GameObject m_ParticlePrefab;

	public GameObject m_StartTimeline;
	public GameObject m_RadarTimeline;

	AudioSource m_NowBGM;
	AudioSource m_NextBGM;

	SoundDirector soundDirector;

	// 証拠が入ったかどうかの判定とかをする
	EvidenceLoading m_EviLoading;

	public GameObject m_StageChangeText;
	StageChangeManager m_ChangeManager;

	float m_BossSky = 1.98f;
	float m_PartySky = 2.55f;

	// スカイドームのカラー変更用
	float m_SkyTimeSec;
	float m_SkySrc;		// 変更元
	float m_SkyDsc;     // 変更先

	// ステージ切り替えパラメータ
	float m_ChangeStageSec = 1.0f;  // パーティタイム終了後にどれくらいの感覚で終了させるか？
	float m_ChangeTimeCnt = 0.0f;

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

		GameObject sd = GameObject.Find("SoundDirector");
		if (sd != null)
			soundDirector = sd.GetComponent<SoundDirector>();
		else
			soundDirector = null;

		if (soundDirector != null)
		{
			m_NextBGM = soundDirector.NextBGM();
			m_NextBGM.Play();
			Debug.Log("再生" + m_NextBGM.gameObject.name);
		}
		else
		{
			Debug.Log("サウンドディレクターなし");
			m_NowBGM = Instantiate(m_PlayerAudioPrefab).GetComponent<AudioSource>();
			m_NowBGM.Stop();
			m_NextBGM = Instantiate(m_BossAudioPrefab).GetComponent<AudioSource>();
			m_NextBGM.Play();
		}

		SkyMaterial.mainTextureScale = new Vector2(m_SkyDsc, 1.0f);

		m_FeverManager = GameObject.Find("FeverManager").GetComponent<FeverManager>();
		m_BossAttackManager = GameObject.FindGameObjectWithTag("BossManager").GetComponent<BossAttackManager>();

		// スタート演出を生成してある程度後に消滅させる
		Destroy(Instantiate(m_StartTimeline), 10.0f);
		Destroy(Instantiate(m_RadarTimeline), 15.0f);

		m_EviLoading = GameObject.Find("EvidenceLoading").GetComponent<EvidenceLoading>();

		m_ChangeManager = GameObject.Find("StageChangeManager").GetComponent<StageChangeManager>();
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
		// ステージ切り替え命令が出ている
		if(m_ChangeTimeCnt != 0.0f)
		{
			m_ChangeTimeCnt -= Time.deltaTime;
			if(m_ChangeTimeCnt < 0.0f)
			{
				m_ChangeTimeCnt = 0.0f;
				m_ChangeManager.ChangeStage();
				Instantiate(m_StageChangeText);
				m_EviLoading.InitLoader();
			}
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
				if (m_BossAttackManager.m_StageSwitch)
				{
					m_ChangeTimeCnt = m_ChangeStageSec;
					m_BossAttackManager.m_StageSwitch = false;
				}
				m_EviLoading.InitLoader();
				m_BossAttackManager.BossBehaviorSwitching(true);
				SkySwitch();
				break;
			case PartyState.PlayerAttack:
				GameObject particle = Instantiate(m_ParticlePrefab);
				Destroy(particle, m_FeverTimeSec + 1.0f);
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

			m_NowBGM.volume = 1.0f - t;
			m_NextBGM.volume = t;

			m_SkyTimeSec += Time.deltaTime;
			if (m_SkyTimeSec > SkyChangeTime)
			{
				m_NowBGM.Stop();
			}
		}
	}

	void SkySwitch()
	{
		float tmp = m_SkySrc;     // 変更元
		m_SkySrc = m_SkyDsc;     // 変更先
		m_SkyDsc = tmp;

		m_SkyTimeSec = 0.0f;

		AudioSwitch();
	}

	void AudioSwitch()
	{
		// Nowがこの関数を呼んだあとにイチから音量が下がっていく
		// Nextがこの関数を呼んだあとにゼロから音量が上がっていく
		AudioSource tmp = m_NowBGM;
		m_NowBGM = m_NextBGM;
		// サウンドディレクターを取得出来ていれば音声を切り替える
		if (soundDirector != null)
		{
			m_NextBGM = soundDirector.NextBGM();
		}
		else
		{
			m_NextBGM = tmp;
			m_NextBGM.Stop();
		}
		m_NextBGM.Play();
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

	public bool IsFever()
	{
		if (m_NowState == PartyState.PlayerAttack)
			return true;
		return false;
	}
}