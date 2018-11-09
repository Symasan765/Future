﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeverManager : MonoBehaviour {

	public GameObject cameraObj;

	[SerializeField]
	private float FeverSec = 10;

	private bool isStart = false;
	
	private float feverSec;
	private float cntFeverSec = 0;
	private EffectManager effectManager;
	private FeverManager feverManager;
	private GameObject[] NormalEvidenceSpawnerObjects;
	private EvidenceSpawner[] evidenceSpawners;
	private GameObject BossHitPositionObj;
	void Start ()
	{
		BossHitPositionObj = GameObject.FindGameObjectWithTag("BossHitPosition");
		//ステージ上にある通常証拠スポナーを全部取得
		NormalEvidenceSpawnerObjects = GameObject.FindGameObjectsWithTag("NormalEvidenceSpawner");
		Debug.Log("ノーマル証拠の数：" + NormalEvidenceSpawnerObjects.Length);

		feverSec = FeverSec;
		effectManager = GameObject.Find("EffectManager").GetComponent<EffectManager>();
		feverManager = GetComponent<FeverManager>();
	}
	
	void Update ()
	{
		Lightning();
		if (isStart)
		{
			//フィーバータイムのカウント
			cntFeverSec -= Time.deltaTime;
			if (cntFeverSec <= 0)
			{
				cntFeverSec = 0;
				isStart = false;
			}
		}
	}

	//バズーカのショット可能かどうかを返す
	public bool CanBazookaShot()
	{
		for (int i = 0; i < NormalEvidenceSpawnerObjects.Length; i++)
		{

			EvidenceSpawner e = NormalEvidenceSpawnerObjects[i].GetComponent<EvidenceSpawner>();
			if (!e.isSetBazooka)
			{
				return false;
			}
		}
		return true;
	}

	public void StartFever(float _feverSec)
	{
		if (!isStart)
		{
			Vector3 pos = new Vector3(cameraObj.transform.position.x, cameraObj.transform.position.y, 0);
			effectManager.PlayFEVER(-1, pos, -5);
			cntFeverSec = _feverSec;
			isStart = true;
		}
	}

	private void Lightning()
	{
		if (feverManager.IsFever())
		{
			if (Time.frameCount % (int)Random.Range(5, 12) == 0)
			{
				Vector3 effpos = new Vector3(BossHitPositionObj.transform.position.x + Random.Range(-5, 5), BossHitPositionObj.transform.position.y + Random.Range(-5, 5), BossHitPositionObj.transform.position.z);
				effectManager.PlayLightning(effpos);
			}
			if (Time.frameCount % (int)Random.Range(9, 17) == 0)
			{
				SoundManager.Get.PlaySE("kanden", 0.25f);
			}
		}
	}

	public bool IsFever()
	{
		if (cntFeverSec > 0)
		{
			return true;
		}
		return false;
	}

}
