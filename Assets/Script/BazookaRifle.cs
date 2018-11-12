using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// バズーカーの砲台（？）のスクリプト。
/// バズーカーの発射に関する動作を定義している。
/// バズーカーのオブジェクトにコンポネントとして当てておく。
/// </summary>
public class BazookaRifle : MonoBehaviour{

	[SerializeField]
	private float BulletAttackPower = 10.0f;	//弾の攻撃力

	private GameObject[] Evidence_temp = new GameObject[3];
	public GameObject EffectObj;
	public GameObject BulletObj;
	private GameObject BossObj;
	public GameObject[] CurvePointObj = new GameObject[2];

	private ParticleSystem particleSystem;
	private BossAttackManager bossAttackManager;
	private PartyTimeManager partyTimeManager;
	private EffectManager effectManager;
	private BazookaBullet bazookaBullet;
	private FeverManager feverManager;
	private StageChangeManager stageChangeManager;

	public float EvidenceDistance = 2.0f;            //証拠を認識する範囲。
    private int EvidenceNum = 0;                      //全体の証拠の数。
    public int NearEvidenceNum = 0;                  //近づいた証拠の数。

	public bool nowEvidenceNormal = false;
	[SerializeField]
	private float ExplosionSec = 2;
	private float cntExplosionSec = 0;

	private int nowSetEvidenceNum = 0;

	public bool isSetEvidence = false;

	private bool isFirstFeverEvidenceHit = false;
	private bool canShot = false;
	private void Start()
	{
		stageChangeManager = GameObject.Find("StageChangeManager").GetComponent<StageChangeManager>();
		BossObj = GameObject.FindGameObjectWithTag("BossHitPosition");
		feverManager = GameObject.Find("FeverManager").GetComponent<FeverManager>();
		effectManager = GameObject.Find("EffectManager").GetComponent<EffectManager>();
		bossAttackManager = GameObject.FindGameObjectWithTag("BossManager").GetComponent<BossAttackManager>();
		partyTimeManager = GameObject.Find("PartyTimeManager").GetComponent<PartyTimeManager>();
	}

	void Update()
    {
		if (!feverManager.IsFever())
		{
			isFirstFeverEvidenceHit = false;
		}
		Explosion();

		if (stageChangeManager.CanBazookaShot() && NearEvidenceNum > 0)
		{
			//普通の証拠
			nowSetEvidenceNum = NearEvidenceNum;
			ShotBazooka(false);
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
					SoundManager.Get.PlaySE("BulletHit1",0.35f);
					SoundManager.Get.PlaySE("BulletHit2",0.35f);
				}
			}
		}
	}

	public void HitBullet(bool _isFever,Vector3 _hitPos)
	{
		if (_isFever)
		{
			bossAttackManager.BossDamage(BulletAttackPower);
			effectManager.PlaySMASH(-1, _hitPos, -1);
		} else
		{
			bossAttackManager.BossDamage(BulletAttackPower * nowSetEvidenceNum);
			isFirstFeverEvidenceHit = true;
			partyTimeManager.LetsParty();
		}
		for (int i = 0; i < 4; i++)
		{
			XPad.Get.SetVibration(i, 1.0f, 1.0f, 0.5f);
		}
		cntExplosionSec = ExplosionSec + 1;
		effectManager.PlayExplosion(_hitPos);
		SoundManager.Get.PlaySE("BulletHit1",0.7f);
		SoundManager.Get.PlaySE("BulletHit2",0.7f);
		ShakeCamera.Impact(0.05f, 1.0f);
	
	}

	private void ShotBazooka(bool _isFeverEvidence)
	{
		isSetEvidence = false;
		for (int i = 0; i < 4; i++)
		{
			XPad.Get.SetVibration(i, 0.7f, 0.7f, 0.2f);
		}
		ShakeCamera.Impact(0.05f, 0.5f);
		SoundManager.Get.PlaySE("launcher2",0.8f);
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
