using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

	private float MoveSpeed = 5.0f;

	private GameObject bazookaObj;
	private BazookaRifle bazookaRifle;
	private Vector3 getPosition;
	[HideInInspector]
	public bool flgMoveToGetPos;

	public bool isScaleDown = false;
	public bool isHold = false;
	private BoxCollider boxCollider;
	private float cntScaleDownTime = 0;
	void Start ()
	{
		boxCollider = GetComponent<BoxCollider>();
		bazookaObj = GameObject.Find("BazookaRifle");
		bazookaRifle = bazookaObj.GetComponent<BazookaRifle>();
	}
	
	void Update ()
	{
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
				bazookaRifle.NearEvidenceNum++;
				Destroy(gameObject);
			}
		}

		if (Vector3.Distance(transform.position, bazookaObj.transform.position) <= bazookaRifle.EvidenceDistance && !IsHold())
		{
			isHold = true;
			boxCollider.enabled = false;
			SetItemLocalPosition(bazookaObj.transform.position);
			flgMoveToGetPos = true;
			isScaleDown = true;
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
}
