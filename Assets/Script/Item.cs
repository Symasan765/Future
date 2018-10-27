using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

	private float MoveSpeed = 5.0f;

	private GameObject bazookaObj;
	private BazookaRifle bazookaRifle;
	private Vector3 getPosition;
	private GameObject evidenceSpawnerObj = null;
	private EvidenceSpawner evidenceSpawner;
	[HideInInspector]
	public bool flgMoveToGetPos;

	public GameObject ModelObj;
	private MeshRenderer meshRenderer;

	public bool isScaleDown = false;
	public bool isHold = false;
	private BoxCollider boxCollider;
	private float cntScaleDownTime = 0;

	private bool isFever = false;
	public bool isFeverEvidence = false;

	private float feverFallSec = 0;
	private float cntFeverFallSec = 0;
	private bool isTriggerStay = false;
	private Rigidbody rb;
	private float lifeTime = 0;
	private float LifeTime = 5.0f;
	private float nowLifeTimeMax;

	void Start ()
	{
		meshRenderer = ModelObj.GetComponent<MeshRenderer>();
		rb = GetComponent<Rigidbody>();
		boxCollider = GetComponent<BoxCollider>();
		bazookaObj = GameObject.Find("BazookaRifle");
		bazookaRifle = bazookaObj.GetComponent<BazookaRifle>();
	}
	
	void Update ()
	{
		if (rb.velocity.y < -4)
		{
			rb.velocity = new Vector3(rb.velocity.x, -4, rb.velocity.z);
		}
		if (isFever)
		{
			if (isHold)
			{
				isFever = false;
			}
			cntFeverFallSec += Time.deltaTime;
			//フィーバータイム時に降ってくる証拠の処理
			if (cntFeverFallSec < feverFallSec)
			{
				boxCollider.isTrigger = true;
			} else
			{
				if (!isTriggerStay)
				{
					nowLifeTimeMax = LifeTime + Vector3.Distance(transform.position,bazookaObj.transform.position) * 0.5f;
					lifeTime = nowLifeTimeMax;
					isFever = false;
					boxCollider.isTrigger = false;
				}
			}
		} else
		{
			if (lifeTime > 0)
			{
				//点滅処理
				if (lifeTime > nowLifeTimeMax / 4 && lifeTime < nowLifeTimeMax / 2)
				{
					if (Time.frameCount % 10 >= 5)
					{
						meshRenderer.enabled = false;
					} else
					{
						meshRenderer.enabled = true;
					}
				}
				if (lifeTime < nowLifeTimeMax / 4)
				{
					if (Time.frameCount % 4 >= 2)
					{
						meshRenderer.enabled = false;
					} else
					{
						meshRenderer.enabled = true;
					}
				}

				lifeTime -= Time.deltaTime;
				if (lifeTime <= 0)
				{
					Destroy(gameObject);
				}
				if (isHold)
				{
					nowLifeTimeMax = LifeTime + Vector3.Distance(transform.position, bazookaObj.transform.position) * 0.5f;
					lifeTime = nowLifeTimeMax;
				}
			}

			if (isHold)
			{
				meshRenderer.enabled = true;
			}

			//証拠スポナーから生成される証拠の処理
			if (flgMoveToGetPos)
			{
				if (transform.position != getPosition)
				{
					Vector3 vec = getPosition - transform.localPosition;
					transform.localPosition += vec * MoveSpeed * Time.deltaTime;
				}
			}

			if (isScaleDown)
			{
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
						evidenceSpawner.DeleteEvidenceObj();
					}
					bazookaRifle.nowEvidenceFever = isFeverEvidence;
					bazookaRifle.NearEvidenceNum++;
					Destroy(gameObject);
				}
			}

			if (Vector3.Distance(transform.position, bazookaObj.transform.position) <= bazookaRifle.EvidenceDistance / 2 && !IsHold())
			{
				isHold = true;
				boxCollider.enabled = false;
				SetItemLocalPosition(bazookaObj.transform.position);
				flgMoveToGetPos = true;
				isScaleDown = true;
			}
		}
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

	public void SetFeverValue(float _fallSec)
	{
		isFeverEvidence = true;
		isFever = true;
		feverFallSec = _fallSec;
	}

	void OnTriggerStay(Collider other)
	{
		if (isFever)
		{
			if (cntFeverFallSec >= feverFallSec)
			{
				isTriggerStay = true;
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		isTriggerStay = false;
	}

}
