﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class PlayerArrow : MonoBehaviour
{
	public GameObject PlayerObj;
	public GameObject ArrowObj;
	private Player player;

	private GameObject[] BazookaObj = null;
	private BazookaRifle[] bazookaScript = null;
	private GameObject nearBazookaObj;
	private GameObject oldNearBazookaObj;
	private Vector3 nearBazookaPos;
	private Vector3 startLocalScale;
	private float nowBazookaDistance = 0;

	[SerializeField]
	private List<GameObject> EvidenceObjList = new List<GameObject>();
	[SerializeField]
	private List<Item> EvidenceScriptList = new List<Item>();
	private float nowEvidenceDistance = 0;
	private GameObject nearEvidenceObj;
	private GameObject oldNearEvidenceObj;

	private float cntScaleUpTime = 0;

	void Start()
	{
		startLocalScale = transform.localScale;
		transform.localScale = Vector3.zero;
		player = PlayerObj.GetComponent<Player>();
		ChangeColor();
	}

	void Update ()
	{
		transform.parent.transform.eulerAngles = Vector3.zero;
		transform.parent.transform.localPosition = new Vector3(0, transform.parent.transform.localPosition.y, -0.1f);
		if (!player.CanIMove())
		{
			transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.4f);
		} else
		{
			if (player.IsHoldItem())
			{
				//一番近いバズーカを探してその方向に向く
				SerchNearBazooka();
			} else
			{
				SerchNearEvidence();
			}
		}
	}

	private void ChangeColor()
	{
		if (player.PlayerIndex == 0)
		{
			ArrowObj.GetComponent<TextMesh>().color = Color.blue;
		}
		if (player.PlayerIndex == 1)
		{
			ArrowObj.GetComponent<TextMesh>().color = Color.red;
		}
		if (player.PlayerIndex == 2)
		{
			ArrowObj.GetComponent<TextMesh>().color = Color.yellow;
		}
		if (player.PlayerIndex == 3)
		{
			ArrowObj.GetComponent<TextMesh>().color = Color.green;
		}
	}

	private void SerchNearBazooka()
	{
		oldNearBazookaObj = nearBazookaObj;
		if (BazookaObj != null)
		{
			for (int i = 0; i < BazookaObj.Length; i++)
			{
				if (!bazookaScript[i].isSetEvidence && BazookaObj[i] != null)
				{
					Vector3 bazookaPos = new Vector3(BazookaObj[i].transform.position.x, BazookaObj[i].transform.position.y, 0);
					if (i == 0)
					{
						nearBazookaObj = BazookaObj[i];
						nearBazookaPos = bazookaPos;
						nowBazookaDistance = Vector3.Distance(transform.position, bazookaPos);
					} else
					{
						if (nowBazookaDistance > Vector3.Distance(transform.position, bazookaPos))
						{
							nowBazookaDistance = Vector3.Distance(transform.position, bazookaPos);
							nearBazookaObj = BazookaObj[i];
							nearBazookaPos = bazookaPos;
						}
					}
				} else
				{
					nowBazookaDistance = 10000;
				}
			}
			transform.rotation = Quaternion.LookRotation(nearBazookaPos - transform.position);
			
		}
		if (oldNearBazookaObj != nearBazookaObj)
		{
			cntScaleUpTime = 2.0f;
		}
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
	}

	private void SerchNearEvidence()
	{
		bool flgLastEvi = false;
		for (int i = 0; i < EvidenceObjList.Count; i++)
		{
			if (EvidenceScriptList[i].isInBazooka || EvidenceObjList[i] == null)
			{
				EvidenceObjList.RemoveAt(i);
				EvidenceScriptList.RemoveAt(i);
				i--;
			} else
			{
				if (!EvidenceScriptList[i].isHold)
				{
					flgLastEvi = true;
				}
			}
		}

		if (flgLastEvi)
		{
			oldNearEvidenceObj = nearEvidenceObj;
			for (int i = 0; i < EvidenceObjList.Count; i++)
			{
				if (!EvidenceScriptList[i].isHold)
				{
					Vector3 evidencePos = new Vector3(EvidenceObjList[i].transform.position.x, EvidenceObjList[i].transform.position.y, 0);
					if (i == 0)
					{
						nearEvidenceObj = EvidenceObjList[i];
						nowEvidenceDistance = Vector3.Distance(transform.position, evidencePos);
					} else
					{
						if (nowEvidenceDistance > Vector3.Distance(transform.position, evidencePos))
						{
							nowEvidenceDistance = Vector3.Distance(transform.position, evidencePos);
							nearEvidenceObj = EvidenceObjList[i];
						}
					}
				} else
				{
					nowEvidenceDistance = 10000;
				}
			}
			transform.rotation = Quaternion.LookRotation(nearEvidenceObj.transform.position - transform.position);
			//Quaternion rot = Quaternion.LookRotation(nearEvidenceObj.transform.position - transform.position);

			if (oldNearEvidenceObj != nearEvidenceObj)
			{
				cntScaleUpTime = 2.0f;
			}
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
		nowBazookaDistance = 0;
		BazookaObj = null;
		BazookaObj = GameObject.FindGameObjectsWithTag("Bazooka");
		bazookaScript = null;
		bazookaScript = new BazookaRifle[BazookaObj.Length];
		for (int i = 0; i < BazookaObj.Length; i++)
		{
			bazookaScript[i] = BazookaObj[i].GetComponent<BazookaRifle>();
		}
	}

	public void ReloadEvidenceObj()
	{
		//いったんリストの全要素を削除
		EvidenceObjList.Clear();
		EvidenceScriptList.Clear();
	}

	public void AddEvidenceObjToList(GameObject _obj)
	{
		EvidenceObjList.Add(_obj);
		EvidenceScriptList.Add(_obj.GetComponent<Item>());
	}
}
