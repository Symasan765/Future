using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvidenceSpawner : MonoBehaviour {

	public GameObject evidenceObj;
	public GameObject EffectObj;

	private GameObject eviObj = null;
	private ParticleSystem particleSystem;
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
