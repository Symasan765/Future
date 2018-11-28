using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvidenceSpawner : MonoBehaviour {

	[SerializeField]
	private float SpawnDeraySec = 3;
	[SerializeField]
	private float FeverEvidenceLifeTime = 10.0f;

	[SerializeField]
	private bool RandomDeraySec = false;
	[SerializeField]
	private float RandomDerayMaxSec = 10.0f;
	[SerializeField]
	private float RandomDerayMinSec = 0.0f;

	[SerializeField]
	private bool RandomLifeTime = false;
	[SerializeField]
	private float RandomLifeTimeMaxSec = 10.0f;
	[SerializeField]
	private float RandomLifeTimeMinSec = 5.0f;

	private bool FeverSpawner = false;
	public GameObject evidenceObj;
	public GameObject EffectObj;

	private GameObject eviObj = null;
	private ParticleSystem particleSystem;
	private FeverManager feverManager;
	private bool isSpawn = false;
	public bool isSetBazooka = false;

	private float nowDeraySec;
	private float cntSpawnDeraySec = 0;
	private float nowLifeTime;

	private GameObject[] playerObjects = new GameObject[4];
	private Player[] playerScripts = new Player[4];

	// Use this for initialization
	void Start ()
	{
		//ランダムでディレイ時間決定
		if (RandomDeraySec)
		{
			nowDeraySec = Random.Range(RandomDerayMinSec, RandomDerayMaxSec);
		}else
		{
			nowDeraySec = SpawnDeraySec;
		}
		//ランダムで生存時間決定
		if (RandomLifeTime)
		{
			nowLifeTime = Random.Range(RandomLifeTimeMinSec, RandomLifeTimeMaxSec);
		} else
		{
			nowLifeTime = FeverEvidenceLifeTime;
		}

		if (this.tag == "NormalEvidenceSpawner")
		{
			FeverSpawner = false;
		}
		if (this.tag == "FeverEvidenceSpawner")
		{
			FeverSpawner = true;
		}
		feverManager = GameObject.Find("FeverManager").GetComponent<FeverManager>();
	}
	
	void Update ()
	{
		if (FeverSpawner)
		{
			//フィーバータイム時のスポナー処理
			if (!feverManager.IsFever())
			{
				cntSpawnDeraySec = 0;
				isSpawn = false;
			} else
			{
				cntSpawnDeraySec += Time.deltaTime;
				if (cntSpawnDeraySec >= nowDeraySec)
				{
					if (!isSpawn)
					{
						Spawn(true);
						isSpawn = true;
					}
				}

				if (isSpawn)
				{
					if (eviObj == null)
					{
						cntSpawnDeraySec = 0;
						isSpawn = false;
					}
				}
			}
		} else
		{
			//通常時のスポナー処理
			if (feverManager.IsFever())
			{
				cntSpawnDeraySec = 0;
				isSpawn = false;
			} else
			{		
				cntSpawnDeraySec += Time.deltaTime;
				if (cntSpawnDeraySec >= SpawnDeraySec)
				{
					if (!isSpawn)
					{
						Spawn(false);
						isSpawn = true;
					}
				}
			}
		}
	}

	public void SetIsFeverSpawner(bool _flg)
	{
		FeverSpawner = _flg;
	}

	private void Spawn(bool _isFever)
	{
		if (eviObj == null)
		{
			isSetBazooka = false;
			cntSpawnDeraySec = 0;
			particleSystem = Instantiate(EffectObj, transform.position, transform.rotation).GetComponent<ParticleSystem>();
			particleSystem.Play();
			eviObj = Instantiate(evidenceObj, transform.position, transform.rotation);
			AddEvidenceObjToPlayerArrows(eviObj);
			Item item = eviObj.GetComponent<Item>();
			item.SetEvidenceSpawnerObject(gameObject);
			if (_isFever)
			{
				eviObj.name = "FeverEvidence";
				item.SetFeverValue(nowLifeTime);
			} else
			{
				eviObj.name = "NormalEvidence";
			}
		}
	}

	public void DeleteEvidenceObj()
	{
		eviObj = null;
	}

	public void AddEvidenceObjToPlayerArrows(GameObject _obj)
	{
		for (int i = 0; i < playerObjects.Length; i++)
		{
			playerScripts[i].AddEvidenceObjToArrow(_obj);
		}
	}

	public void ReloadPlayerObjects()
	{
		playerObjects = GameObject.FindGameObjectsWithTag("Player");
		for (int i = 0; i < playerObjects.Length; i++)
		{
			playerScripts[i] = playerObjects[i].GetComponent<Player>();
		}
	}

}
