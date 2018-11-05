﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// バズーカーの砲台（？）のスクリプト。
/// バズーカーの発射に関する動作を定義している。
/// バズーカーのオブジェクトにコンポネントとして当てておく。
/// </summary>
public class BazookaRifle : MonoBehaviour
{
    [SerializeField]
    private GameObject BazookaPrefab;               //ここにバズーカーのPrefabをUnityのInspectorで割当しておく。（またはResourece.Loadでロードするといい）
	private GameObject[] Evidence_temp = new GameObject[3];
	public GameObject EffectObj;
	public GameObject m_AreaPrefab;
	public GameObject BulletObj;
	GameObject m_AreaEntity;
	public GameObject BossObj;
	public GameObject[] CurvePointObj = new GameObject[2];

	private ParticleSystem particleSystem;
	private BossAttackManager bossAttackManager;
	private PartyTimeManager partyTimeManager;
	private EffectManager effectManager;
	private BazookaBullet bazookaBullet;
	private FeverManager feverManager;

	public float EvidenceDistance = 3.0f;            //証拠を認識する範囲。
    private int EvidenceNum = 0;                      //全体の証拠の数。
	[SerializeField]
	private int BazookaEvidenceNum;
    public int NearEvidenceNum = 0;                  //近づいた証拠の数。

	public bool nowEvidenceNormal = false;
	[SerializeField]
	private float ExplosionSec = 2;
	private float cntExplosionSec = 0;

	private bool isFirstFeverEvidenceHit = false;
	private void Start()
	{
		feverManager = GameObject.Find("FeverManager").GetComponent<FeverManager>();
		effectManager = GameObject.Find("EffectManager").GetComponent<EffectManager>();
		bossAttackManager = GameObject.Find("BossAttackManager").GetComponent<BossAttackManager>();
		partyTimeManager = GameObject.Find("PartyTimeManager").GetComponent<PartyTimeManager>();
		m_AreaEntity = Instantiate(m_AreaPrefab);
		m_AreaEntity.transform.parent = transform;
		BazookaAreaUpdate();
		BazookaEvidenceNum = GameObject.FindGameObjectsWithTag("NormalEvidenceSpawner").Length;
	}

	void Update()
    {
		if (!feverManager.IsFever())
		{
			isFirstFeverEvidenceHit = false;
		}
		BazookaAreaUpdate();
		Lightning();
		Explosion();
		//GameObject Boss = GameObject.FindGameObjectWithTag("BOSS");
        //Debug.Log(Boss);
		//証拠を探す。
		//DetectEvidence();
		/*
        //証拠が3つ以上だとバズーカーを発射し、証拠を削除。
        if (NearEvidenceNum == 3)
        {
            Instantiate(BazookaPrefab);
            BazookaPrefab.transform.position = this.transform.position;
            BazookaPrefab.GetComponent<BazookaBullet>().SetBazooka(Boss.gameObject.transform.position, this.transform.position);
            for(int i=0;i<3;i++)
            {
                Destroy(Evidence_temp[i]);
            }
        }*/

		if (NearEvidenceNum == BazookaEvidenceNum)
		{
			//普通の証拠
			ShotBazooka(false);
			//partyTimeManager.LetsParty();
			NearEvidenceNum = 0;
		}

    }

    //証拠を探す関数。
    void DetectEvidence()
    {
        //とりあえずSYOUKOタグがついているオブジェクトを全部獲得。
        GameObject[] Syouko = GameObject.FindGameObjectsWithTag("SYOUKO");
		EvidenceNum = Syouko.Length;
        int NearEvidenceNum_Temp = 0;

        //証拠とバズーカーの距離がパラメータより近いと近づいた証拠だと判断する。
        for (int i = 0; i < EvidenceNum; i++)
        {
            if (Vector3.Distance(Syouko[i].gameObject.transform.position, this.gameObject.transform.position) <= EvidenceDistance)
            {
				Evidence_temp[NearEvidenceNum_Temp] = Syouko[i];        //あとで削除するために変数でオブジェクトを持っておく。
				NearEvidenceNum_Temp++;            
            }
        }
		NearEvidenceNum = NearEvidenceNum_Temp;
    }

	void BazookaAreaUpdate()
	{
		m_AreaEntity.transform.localScale = new Vector3(EvidenceDistance, EvidenceDistance, 0.01f); // 0.01fは奥行方向に僅かに厚みを持たせるため
		Vector3 pos = transform.position;
		pos.z += 1.5f;		// バズーカモデルと重ねないために少し座標を奥にずらす
		m_AreaEntity.transform.position = pos;
	}

	private void Lightning()
	{
		if (feverManager.IsFever() && isFirstFeverEvidenceHit)
		{
			if (Time.frameCount % (int)Random.Range(5,12) == 0)
			{
				Vector3 effpos = new Vector3(BossObj.transform.position.x + Random.Range(-5, 5), BossObj.transform.position.y + Random.Range(-5, 5), BossObj.transform.position.z);
				effectManager.PlayLightning(effpos);
				SoundManager.Get.PlaySE("kanden");
			}
		}
	}

	private void Explosion()
	{
		if (cntExplosionSec > 0)
		{
			cntExplosionSec -= Time.deltaTime;
			if (cntExplosionSec <= 0)
			{
				cntExplosionSec = 0;
			}

			if (cntExplosionSec <= ExplosionSec)
			{
				if (Time.frameCount % 5 == 0)
				{
					ShakeCamera.Impact(0.03f, 0.2f);
					Vector3 effpos = new Vector3(BossObj.transform.position.x + Random.Range(-5, 5), BossObj.transform.position.y + Random.Range(-5, 5), BossObj.transform.position.z);
					effectManager.PlayExplosion(effpos);
					SoundManager.Get.PlaySE("BulletHit1");
					SoundManager.Get.PlaySE("BulletHit2");
				}
			}
		}
	}

	public void HitBullet(bool _isFever,Vector3 _hitPos)
	{
		if (_isFever)
		{
			effectManager.PlaySMASH(-1, _hitPos, -1);
		} else
		{
			isFirstFeverEvidenceHit = true;
			partyTimeManager.LetsParty();
		}
		for (int i = 0; i < 4; i++)
		{
			XPad.Get.SetVibration(i, 1.0f, 1.0f, 0.5f);
		}
		cntExplosionSec = ExplosionSec + 1;
		effectManager.PlayExplosion(_hitPos);
		SoundManager.Get.PlaySE("BulletHit1");
		SoundManager.Get.PlaySE("BulletHit2");
		ShakeCamera.Impact(0.05f, 1.0f);
		bossAttackManager.BossDamage(5);
	}

	private void ShotBazooka(bool _isFeverEvidence)
	{
		for (int i = 0; i < 4; i++)
		{
			XPad.Get.SetVibration(i, 0.7f, 0.7f, 0.2f);
		}
		ShakeCamera.Impact(0.05f, 0.5f);
		SoundManager.Get.PlaySE("launcher2");
		GameObject obj = Instantiate(EffectObj, transform.position, transform.rotation);
		particleSystem = obj.GetComponent<ParticleSystem>();
		particleSystem.Play();
		effectManager.PlayBOOM(-1, transform.position);

		BazookaBullet bullet = Instantiate(BulletObj, transform.position, transform.rotation).GetComponent<BazookaBullet>();
		bullet.SetBazookaRifleObj(gameObject);
		bullet.SetCurvePointObj(CurvePointObj[0], CurvePointObj[1]);
		bullet.SetBazooka(BossObj.transform.position);
		bullet.SetFeverEvidenceFlg(_isFeverEvidence);
	}

	public void SetEvidence(bool _feverEvidence)
	{
		if (_feverEvidence)
		{
			ShotBazooka(true);
		} else
		{
			NearEvidenceNum++;
		}
	}
}
