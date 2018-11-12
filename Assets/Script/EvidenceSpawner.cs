using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvidenceSpawner : MonoBehaviour {

	[SerializeField]
	private float SpawnDeraySec = 3;
	[SerializeField]
	private float FeverEvidenceLifeTime = 10.0f;
	
	private bool FeverSpawner = false;
	public GameObject evidenceObj;
	public GameObject EffectObj;

	private GameObject eviObj = null;
	private ParticleSystem particleSystem;
	private FeverManager feverManager;
	private bool isSpawn = false;
	public bool isSetBazooka = false;

	private float cntSpawnDeraySec = 0;
	// Use this for initialization
	void Start ()
	{
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
				if (cntSpawnDeraySec >= SpawnDeraySec)
				{
					if (!isSpawn)
					{
						Spawn(true);
						isSpawn = true;
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
			Item item = eviObj.GetComponent<Item>();
			item.SetEvidenceSpawnerObject(gameObject);
			if (_isFever)
			{
				eviObj.name = "FeverEvidence";
				item.SetFeverValue(FeverEvidenceLifeTime);
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

}
