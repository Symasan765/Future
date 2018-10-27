using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvidenceSpawner : MonoBehaviour {

	public float SpawnDeraySec = 3;
	public GameObject evidenceObj;
	public GameObject EffectObj;

	private GameObject eviObj = null;
	private ParticleSystem particleSystem;

	private float cntSpawnDeraySec = 0;
	// Use this for initialization
	void Start ()
	{
		
	}
	
	void Update ()
	{
		Spawn();
	}

	private void Spawn()
	{
		if (eviObj == null)
		{
			cntSpawnDeraySec += Time.deltaTime;
			if (cntSpawnDeraySec >= SpawnDeraySec)
			{
				cntSpawnDeraySec = 0;
				particleSystem = Instantiate(EffectObj, transform.position, transform.rotation).GetComponent<ParticleSystem>();
				particleSystem.Play();
				eviObj = Instantiate(evidenceObj, transform.position, transform.rotation);
				Item item = eviObj.GetComponent<Item>();
				item.SetEvidenceSpawnerObject(gameObject);
			}
		}
	}

	public void DeleteEvidenceObj()
	{
		eviObj = null;
	}

}
