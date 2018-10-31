using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

	
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

	[HideInInspector]
	public bool flgMoveToGetPos;

	private float MoveSpeed = 5.0f;

	public GameObject ModelObj;


	public bool isScaleDown = false;
	public bool isHold = false;
	public bool isFeverEvidence = false;

	private bool isInBazooka = false;
	private bool isTriggerStay = false;
	private bool isCheckCreatePosition = false;

	private float cntScaleDownTime = 0;
	private float feverFallSec = 0;
	private float cntFeverFallSec = 0;
	private float lifeTime = 0;
	private float LifeTime = 2.0f;
	private float nowLifeTimeMax;
	private float cntCheckCreatePositionSec = 0;


	void Start ()
	{
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
		if (bazookaObj == null)
		{
			bazookaObj = GameObject.Find("BazookaRifle");
		}
		bazookaRifle = bazookaObj.GetComponent<BazookaRifle>();
		if (feverManager == null)
		{
			feverManager = GameObject.Find("FeverManager").GetComponent<FeverManager>();
		}
		if (effectManager == null)
		{
			effectManager = GameObject.Find("EffectManager").GetComponent<EffectManager>();
		}
	}
	
	void Update ()
	{
		if (rb.velocity.y < -4)
		{
			rb.velocity = new Vector3(rb.velocity.x, -4, rb.velocity.z);
		}

		if (isCheckCreatePosition)
		{
			rb.velocity = Vector3.zero;
			cntCheckCreatePositionSec += Time.deltaTime;
			if (cntCheckCreatePositionSec >= 1 / 60)
			{
				if (isTriggerStay)
				{
					Destroy(gameObject);
				} else
				{
					boxCollider.isTrigger = false;
					effectManager.PlayCreateEvidence(transform.position);
					isCheckCreatePosition = false;
					meshRenderer.enabled = true;
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
					effectManager.PlayDelete(transform.position);
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
						evidenceSpawner.DeleteEvidenceObj();
					}
					bazookaRifle.SetEvidence(isFeverEvidence);
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
		if (isFeverEvidence)
		{
			if (!isHold && !isInBazooka && !feverManager.IsFever())
			{
				effectManager.PlayDelete(transform.position);
				Destroy(gameObject);
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

	public void SetNecessaryComponent(GameObject _bazookaObj, FeverManager _feverManager, EffectManager _effectManager)
	{
		rb = GetComponent<Rigidbody>();
		boxCollider = GetComponent<BoxCollider>();
		meshRenderer = ModelObj.GetComponent<MeshRenderer>();
		bazookaObj = _bazookaObj;
		feverManager = _feverManager;
		effectManager = _effectManager;
	}

	public void SetFeverValue(float _fallSec)
	{
		boxCollider.isTrigger = true;
		meshRenderer.enabled = false;
		isCheckCreatePosition = true;
		isFeverEvidence = true;
		feverFallSec = _fallSec;
		nowLifeTimeMax = LifeTime + Vector3.Distance(transform.position, bazookaObj.transform.position) * 0.5f;
		lifeTime = nowLifeTimeMax;
	}

	void OnTriggerStay(Collider other)
	{
		if (isCheckCreatePosition)
		{
			feverManager.SetCreateEvidenceSecMax();
			if (other.gameObject.tag == "Player")
			{
				Destroy(gameObject);
			}
			isTriggerStay = true;
		}
	}
}
