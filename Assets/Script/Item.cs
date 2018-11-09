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

	private float cntScaleDownTime = 0;
	private float lifeTime = 0;
	private float LifeTime = 8.0f;

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
		//角度とZ座標を固定
		transform.eulerAngles = new Vector3(0, 0, 0);
		transform.position = new Vector3(transform.position.x, transform.position.y, 0);
		//持たれていない時モデルを回転
		if (isHold)
		{
			ModelObj.transform.localEulerAngles = new Vector3(0, 0, 0);
		} else
		{
			ModelObj.transform.localEulerAngles = new Vector3(0, ModelObj.transform.localEulerAngles.y + 2, 0);
		}
		//落下速度を固定
		if (rb.velocity.y < -4)
		{
			rb.velocity = new Vector3(rb.velocity.x, -4, rb.velocity.z);
		}

		if (lifeTime > 0)
		{
			//点滅処理
			if (lifeTime > LifeTime / 4 && lifeTime < LifeTime / 2)
			{
				if (Time.frameCount % 8 > 4)
				{
					meshRenderer.enabled = false;
				} else
				{
					meshRenderer.enabled = true;
				}
			}
			if (lifeTime < LifeTime / 4)
			{
				if (Time.frameCount % 3 > 1)
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
				lifeTime = LifeTime;
			}
		}

		//指定された場所へ移動
		if (flgMoveToGetPos)
		{
			if (transform.position != getPosition)
			{
				Vector3 vec = getPosition - transform.localPosition;
				transform.localPosition += vec * MoveSpeed * Time.deltaTime;
			}
		}

		//バズーカに入った時の縮小と削除処理
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
					evidenceSpawner.isSetBazooka = true;
					evidenceSpawner.DeleteEvidenceObj();
				}
				bazookaRifle.SetEvidence(isFeverEvidence);
				Destroy(gameObject);
			}
		}

		//フィーバタイムが終わったら、持たれていないフィーバ証拠を全て削除
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

	public void SetFeverValue()
	{
		isFeverEvidence = true;
		lifeTime = LifeTime;
	}

	public void SetBazooka(GameObject _obj)
	{
		boxCollider.isTrigger = true;
		bazookaRifle = _obj.gameObject.GetComponent<BazookaRifle>();
		isHold = true;
		SetItemLocalPosition(_obj.transform.position);
		flgMoveToGetPos = true;
		isScaleDown = true;
	}
}
