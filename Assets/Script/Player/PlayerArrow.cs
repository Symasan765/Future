using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
	public GameObject PlayerObj;
	public GameObject ArrowObj;
	private GameObject[] BazookaObj = null;
	private BazookaRifle[] bazookaScript = null;
	private Player player;

	private GameObject nearBazookaObj;
	private GameObject oldNearBazookaObj;
	private Vector3 nearBazookaPos;
	private Vector3 startLocalScale;
	private float nowDistance = 0;

	private float cntScaleUpTime = 0;

	void Start()
	{
		startLocalScale = transform.localScale;
		transform.localScale = Vector3.zero;
		player = PlayerObj.GetComponent<Player>();
	}

	void Update ()
	{
		transform.parent.transform.eulerAngles = Vector3.zero;
		transform.parent.transform.localPosition = new Vector3(0, transform.parent.transform.localPosition.y, -0.01f);
		oldNearBazookaObj = nearBazookaObj;
		if (BazookaObj != null && player.IsHoldItem())
		{
			for (int i = 0; i < BazookaObj.Length; i++)
			{
				Vector3 bazookaPos = new Vector3(BazookaObj[i].transform.position.x, BazookaObj[i].transform.position.y, 0);
				if (!bazookaScript[i].isSetEvidence)
				{
					if (i == 0)
					{
						nearBazookaObj = BazookaObj[i];
						nearBazookaPos = bazookaPos;
						nowDistance = Vector3.Distance(transform.position, bazookaPos);
					} else
					{
						if (nowDistance > Vector3.Distance(transform.position, bazookaPos))
						{
							nowDistance = Vector3.Distance(transform.position, bazookaPos);
							nearBazookaObj = BazookaObj[i];
							nearBazookaPos = bazookaPos;
						}
					}
				} else
				{
					nowDistance = 10000;
				}
			}
			transform.rotation =Quaternion.LookRotation(nearBazookaPos - transform.position);
		}

		if (oldNearBazookaObj != nearBazookaObj)
		{
			cntScaleUpTime = 2.0f;
		}

		if (player.IsHoldItem())
		{
			if (cntScaleUpTime > 0)
			{
				float maxScale = 2.0f;
				if (cntScaleUpTime > 1)
				{
					transform.localScale = Vector3.Lerp(startLocalScale, new Vector3(startLocalScale.x * maxScale, startLocalScale.y * maxScale, startLocalScale.z * maxScale), 2 - cntScaleUpTime);
				}
				if (cntScaleUpTime <= 1)
				{
					transform.localScale = Vector3.Lerp(new Vector3(startLocalScale.x * maxScale, startLocalScale.y * maxScale, startLocalScale.z * maxScale), startLocalScale, 1 - cntScaleUpTime);
				}
				cntScaleUpTime -= Time.deltaTime * 10;
				if (cntScaleUpTime <= 0)
				{
					cntScaleUpTime = 0;
				}
			} else
			{
				transform.localScale = Vector3.Lerp(transform.localScale, startLocalScale, 0.4f);
			}
		} else
		{
			transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.4f);
		}
	}

	public void ReloadBazookaObj()
	{
		nowDistance = 0;
		BazookaObj = null;
		BazookaObj = GameObject.FindGameObjectsWithTag("Bazooka");
		bazookaScript = null;
		bazookaScript = new BazookaRifle[BazookaObj.Length];
		for (int i = 0; i < BazookaObj.Length; i++)
		{
			bazookaScript[i] = BazookaObj[i].GetComponent<BazookaRifle>();
		}
	}
}
