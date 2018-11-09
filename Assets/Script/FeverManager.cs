using System.Collections;
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
	void Start ()
	{
		//ステージ上にある通常証拠スポナーを全部取得
		NormalEvidenceSpawnerObjects = GameObject.FindGameObjectsWithTag("NormalEvidenceSpawner");
		Debug.Log("ノーマル証拠の数：" + NormalEvidenceSpawnerObjects.Length);

		feverSec = FeverSec;
		effectManager = GameObject.Find("EffectManager").GetComponent<EffectManager>();
		feverManager = GetComponent<FeverManager>();
	}
	
	void Update ()
	{
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

	public bool IsFever()
	{
		if (cntFeverSec > 0)
		{
			return true;
		}
		return false;
	}

}
