using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvidenceSpawner : MonoBehaviour {

	[SerializeField]
	private float SpawnDeraySec = 3;
	private bool FeverSpawner = false;
	public GameObject evidenceObj;
	public GameObject EffectObj;

	private GameObject eviObj = null;
	private ParticleSystem particleSystem;
	private FeverManager feverManager;
	private bool isSpawn = false;

	private float cntSpawnDeraySec = 0;
	// Use this for initialization
	void Start ()
	{
		if (this.tag == "NormalEvidenceSpawner")
		{
			FeverSpawner = false;
		} else
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
			
		} else
		{
			//通常時のスポナー処理
			if (feverManager.IsFever())
			{
				isSpawn = false;
			} else
			{		
				cntSpawnDeraySec += Time.deltaTime;
				if (cntSpawnDeraySec >= SpawnDeraySec)
				{
					if (!isSpawn)
					{
						Spawn();
						isSpawn = true;
					}
				}
			}
		}
	}

	private void Spawn()
	{
		if (eviObj == null)
		{
			cntSpawnDeraySec = 0;
			particleSystem = Instantiate(EffectObj, transform.position, transform.rotation).GetComponent<ParticleSystem>();
			particleSystem.Play();
			eviObj = Instantiate(evidenceObj, transform.position, transform.rotation);
			Item item = eviObj.GetComponent<Item>();
			item.SetEvidenceSpawnerObject(gameObject);
			
		}
	}

	public void DeleteEvidenceObj()
	{
		eviObj = null;
	}

}
