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
	private int MaxEvidenceNum = 1;

	private float BulletAttackPower = 5;	//弾の攻撃力

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
	private EviRainbow[] eviRainbows; 

	public float EvidenceDistance = 2.0f;            //証拠を認識する範囲。
    private int EvidenceNum = 0;                      //全体の証拠の数。
    public int setNormalEvidenceNum = 0;                  //近づいた証拠の数。

	public bool nowEvidenceNormal = false;
	[SerializeField]
	private float ExplosionSec = 2;
	private float cntExplosionSec = 0;

	private int nowSetEvidenceNum = 0;

	public bool isSetEvidence = false;
	public bool isSetNormalEvidence = false;

	private bool canShot = false;

	private Vector3 startPosition;
	private bool isShake = false;
	private float cntShakePower = 0;
	private int cutinPlayerIndex = 0;

	private float cntScaleUpTime = 0;
	private float cntScaleDownTime = 0;
	private Vector3 startLocalScale;

	private bool rotateAngle = false;
	private float rotatePower = 0;

	private void Start()
	{
		startLocalScale = transform.localScale;
		eviRainbows = GetComponentsInChildren<EviRainbow>();
		startPosition = transform.position;
		stageChangeManager = GameObject.Find("StageChangeManager").GetComponent<StageChangeManager>();
		BossObj = GameObject.FindGameObjectWithTag("BossHitPosition");
		feverManager = GameObject.Find("FeverManager").GetComponent<FeverManager>();
		effectManager = GameObject.Find("EffectManager").GetComponent<EffectManager>();
		bossAttackManager = GameObject.FindGameObjectWithTag("BossManager").GetComponent<BossAttackManager>();
		partyTimeManager = GameObject.Find("PartyTimeManager").GetComponent<PartyTimeManager>();
	}

	void Update()
    {

		if (Input.GetKeyDown(KeyCode.T))
		{
			partyTimeManager.LetsParty();
		}

		if(cntScaleUpTime > 0)
		{
			float maxScale = 2.0f;
			if (cntScaleUpTime > 1)
			{
				transform.localScale = Vector3.Lerp(startLocalScale, new Vector3(startLocalScale.x * maxScale, startLocalScale.y * maxScale * 1.5f, startLocalScale.z * maxScale), 2 - cntScaleUpTime);
			}
			if(cntScaleUpTime <= 1)
			{
				transform.localScale = Vector3.Lerp(new Vector3(startLocalScale.x * maxScale, startLocalScale.y * maxScale * 1.5f, startLocalScale.z * maxScale), startLocalScale, 1 - cntScaleUpTime);
			}
			cntScaleUpTime -= Time.deltaTime * 8;
			if (cntScaleUpTime <= 0)
			{
				cntScaleUpTime = 0;
			}
		} else
		{
			transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, 0, 0), 0.2f);
			transform.localScale = startLocalScale;
		}

		Explosion();

		if (stageChangeManager.CanBazookaShot() && setNormalEvidenceNum > 0)
		{
			//普通の証拠
			feverManager.PlayCutIn(cutinPlayerIndex);
			nowSetEvidenceNum = setNormalEvidenceNum;
			ShotBazooka(false);
			setNormalEvidenceNum = 0;
		}
		ShakeBazooka(2, 1);
    }

	private void ShakeBazooka(int _speed, float _brakePower)
	{
		if (isShake)
		{
			if (Time.frameCount % _speed == 0)
			{
				transform.position = new Vector3(startPosition.x, startPosition.y + cntShakePower, startPosition.z);
			} else
			{
				transform.position = new Vector3(startPosition.x, startPosition.y + (cntShakePower * -1), startPosition.z);
			}
			cntShakePower -= Time.deltaTime * _brakePower;
			if (cntShakePower <= 0)
			{
				cntShakePower = 0;
				isShake = false;
			}
		} else
		{
			transform.position = startPosition;
		}
	}

	public void StartShakeBazooka(float _shakePower)
	{
		if (!isShake)
		{
			cntShakePower = _shakePower;
			isShake = true;
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
			bossAttackManager.BossDamage(BulletAttackPower * 0.7f);
			effectManager.PlaySMASH(-1, _hitPos, -1);
		} else
		{
			feverManager.BossDamage((BulletAttackPower * 0.9f) / 8);
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
		StartShakeBazooka(0.4f);
		isSetEvidence = false;
		isSetNormalEvidence = false;
		setNormalEvidenceNum = 0;
		SetEviRainbow(true);
	}

	private void SetEviRainbow(bool _flg)
	{
		for (int i = 0; i < eviRainbows.Length; i++)
		{
			eviRainbows[i].IsRainbow(_flg);
		}
	}

	public void SetEvidence(bool _feverEvidence)
	{
		SetEviRainbow(false);
		if (_feverEvidence)
		{
			ShotBazooka(true);
		} else
		{
			setNormalEvidenceNum++;
			if (setNormalEvidenceNum == MaxEvidenceNum)
			{
				isSetNormalEvidence = true;
				isSetEvidence = true;
			}
		}
	}
	public void SetPlayerIndex(int _index)
	{
		cutinPlayerIndex = _index;
	}

	public void StartScaleUp()
	{
		cntScaleUpTime = 2;
	}
}
