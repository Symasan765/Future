﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

	public GameObject EviEffectObj;

	private GameObject bazookaObj = null;
	private BazookaRifle bazookaRifle = null;
	private Vector3 getPosition;
	private GameObject evidenceSpawnerObj = null;
	private EvidenceSpawner evidenceSpawner = null;
	private EffectManager effectManager = null;
	private MeshRenderer meshRenderer = null;
	private BoxCollider boxCollider = null;
	private Rigidbody rb = null;
	private FeverManager feverManager;

	private ParticleSystem eviParticle;

	[HideInInspector]
	public bool flgMoveToGetPos;

	private float MoveSpeed = 5.0f;

	public GameObject ModelObj;

	public bool isScaleDown = false;
	public bool isHold = false;
	public bool isFeverEvidence = false;

	private bool isInBazooka = false;
	private bool isTriggerStay = false;

	public int holdPlayerIndex = 0;

	private float cntScaleDownTime = 0;
	private float cntLifeTimeSec = 0;
	private float LifeTime = 0;

	private Vector3 startSize;
	private float fieldSize = 1.3f;

	private float cntScaleUpTime = 0;
	private Vector3 modelStartScale;

	void Start ()
	{
		eviParticle = EviEffectObj.GetComponent<ParticleSystem>();
		startSize = transform.localScale;
		modelStartScale = ModelObj.transform.localScale;
		if (meshRenderer == null)
		{
			meshRenderer = ModelObj.GetComponent<MeshRenderer>();
		}
		if (rb == null)
		{
			rb = GetComponent<Rigidbody>();
		}
		if (boxCollider == null)
		{
			boxCollider = GetComponent<BoxCollider>();
		}
		if (feverManager == null)
		{
			feverManager = GameObject.Find("FeverManager").GetComponent<FeverManager>();
		}
		if (effectManager == null)
		{
			effectManager = GameObject.Find("EffectManager").GetComponent<EffectManager>();
		}

		cntScaleUpTime = 2;
	}

	void Update ()
	{
		//角度とZ座標を固定
		transform.position = new Vector3(transform.position.x, transform.position.y, 0);
		//持たれていない時モデルを回転
		if (isHold || isScaleDown)
		{
			eviParticle.Stop();
			transform.eulerAngles = new Vector3(0, 0, 0);
			if (isScaleDown)
			{
				transform.eulerAngles = new Vector3(0, 0, 0);
			}
			meshRenderer.enabled = true;
			ModelObj.transform.localEulerAngles = new Vector3(0, 0, 0);
			ModelObj.transform.localScale = modelStartScale;
			transform.localScale = Vector3.Lerp(transform.localScale, startSize * 0.5f, MoveSpeed);
		} else
		{
			if (!eviParticle.isPlaying)
			{
				eviParticle.Play();
			}
			transform.eulerAngles = new Vector3(0, 0, 0);
			//transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(startSize.x * fieldSize, startSize.y * fieldSize, startSize.z * fieldSize), MoveSpeed);
			//ModelObj.transform.localEulerAngles = new Vector3(0, ModelObj.transform.localEulerAngles.y + 2, 0);

			if (cntScaleUpTime > 0)
			{
				float maxScale = 1.3f;
				if (cntScaleUpTime > 1)
				{
					ModelObj.transform.localScale = Vector3.Slerp(modelStartScale, new Vector3(modelStartScale.x * maxScale, modelStartScale.y * maxScale, modelStartScale.z * maxScale), 2 - cntScaleUpTime);
				}
				if (cntScaleUpTime <= 1)
				{
					ModelObj.transform.localScale = Vector3.Slerp(new Vector3(modelStartScale.x * maxScale, modelStartScale.y * maxScale, modelStartScale.z * maxScale), modelStartScale, 1 - cntScaleUpTime);
				}
				cntScaleUpTime -= Time.deltaTime;
				if (cntScaleUpTime <= 0)
				{
					cntScaleUpTime = 2;
				}
			}
		}

		//左右移動速度を固定
		rb.velocity = new Vector3(0, rb.velocity.y, 0);
		//上昇速度を固定
		if (rb.velocity.y > 0)
		{
			rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
		}
		//落下速度を固定
		if (rb.velocity.y < -4)
		{
			rb.velocity = new Vector3(rb.velocity.x, -4, rb.velocity.z);
		}

		if (cntLifeTimeSec > 0)
		{
			//点滅処理
			if (cntLifeTimeSec > LifeTime / 4 && cntLifeTimeSec < LifeTime / 2)
			{
				if (Time.frameCount % 8 > 4)
				{
					meshRenderer.enabled = false;
				} else
				{
					meshRenderer.enabled = true;
				}
			}
			if (cntLifeTimeSec < LifeTime / 4)
			{
				if (Time.frameCount % 3 > 1)
				{
					meshRenderer.enabled = false;
				} else
				{
					meshRenderer.enabled = true;
				}
			}

			cntLifeTimeSec -= Time.deltaTime;
			if (cntLifeTimeSec <= 0)
			{
				Delete();
			}
			if (isHold)
			{
				cntLifeTimeSec = LifeTime;
			}
		}

		//指定された場所へ移動
		if (flgMoveToGetPos)
		{
			if (isHold)
			{
				transform.localPosition = Vector3.Lerp(transform.localPosition, getPosition, 0.5f);
			} else
			{
				if (transform.localPosition != getPosition)
				{
					Vector3 vec = getPosition - transform.localPosition;
					transform.localPosition += vec * MoveSpeed * Time.deltaTime;
				}
			}
		}

		//バズーカに入った時の縮小と削除処理
		if (isScaleDown)
		{
			if(bazookaObj == null)
			{
				Delete();
			}
			isInBazooka = true;
			cntScaleDownTime += Time.deltaTime;
			if (cntScaleDownTime >= 1.0f)
			{
				cntScaleDownTime = 1.0f;
			}
			transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0, 0, 0), cntScaleDownTime);
			if (cntScaleDownTime >= 1)
			{
				if (evidenceSpawnerObj != null)
				{
					evidenceSpawner.isSetBazooka = true;
					evidenceSpawner.DeleteEvidenceObj();
				}
				bazookaRifle.SetPlayerIndex(holdPlayerIndex);
				bazookaRifle.SetEvidence(isFeverEvidence);
				Destroy(gameObject);
			}
		}

		if (evidenceSpawnerObj == null)
		{
			Delete();
		}

		//フィーバタイムが終わったら、持たれていないフィーバ証拠を全て削除
		if (isFeverEvidence)
		{
			if (!isHold && !isInBazooka && !feverManager.IsFever())
			{
				Delete();
			}
		}
	}

	public void Delete()
	{
		effectManager.PlayDelete(transform.position);
		Destroy(gameObject);
	}

	public bool IsHold()
	{
		return isHold;
	}

	public void SetItemLocalPosition(Vector3 _pos)
	{
		getPosition = _pos;
	}

	public void SetEvidenceSpawnerObject(GameObject _obj)
	{
		evidenceSpawnerObj = _obj;
		evidenceSpawner = _obj.GetComponent<EvidenceSpawner>();
	}

	public void SetFeverValue(float _lifeTime)
	{
		LifeTime = _lifeTime;
		isFeverEvidence = true;
		cntLifeTimeSec = _lifeTime;
	}

	public void SetBazooka(GameObject _obj)
	{
		bazookaObj = _obj;
		boxCollider.isTrigger = true;
		bazookaRifle = _obj.gameObject.GetComponent<BazookaRifle>();
		//isHold = true;
		SetItemLocalPosition(_obj.transform.position);
		flgMoveToGetPos = true;
		isScaleDown = true;
		if (!isFeverEvidence)
		{
			bazookaRifle.isSetNormalEvidence = true;
		}
	}

	void OnCollisionStay(Collision other)
	{
		if (!isHold)
		{
			if (other.gameObject.tag == "Bazooka")
			{
				BazookaRifle br = other.gameObject.GetComponent<BazookaRifle>();
				if (!br.isSetEvidence)
				{
					br.isSetEvidence = true;
					SetBazooka(other.gameObject);
				}
			}
		}
	}

	/*
	void OnTriggerStay(Collider other)
	{
		if (!isHold)
		{
			if (other.tag == "Bazooka")
			{
				BazookaRifle br = other.gameObject.GetComponent<BazookaRifle>();
				if (!br.isSetEvidence)
				{
					br.isSetEvidence = true;
					SetBazooka(other.gameObject);
				}
			}
		}
	}*/

}