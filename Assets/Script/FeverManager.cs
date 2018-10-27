using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeverManager : MonoBehaviour {

	[SerializeField]
	private float FeverSec = 10;
	[SerializeField]
	private float CreateEviDeraySec = 0.2f;

	public GameObject EvidenceObj;

	public GameObject MaxRangeObj;
	public GameObject MinRangeObj;

	private bool isStart = false;

	private float feverSec;
	private float cntFeverSec = 0;
	private float cntCreateEvidenceSec = 0;
	// Use this for initialization
	void Start ()
	{
		feverSec = FeverSec;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.F1))
		{
			StartFever(FeverSec);
		}

		if (isStart)
		{
			cntFeverSec -= Time.deltaTime;
			if (cntFeverSec <= 0)
			{
				cntCreateEvidenceSec = 0;
				cntFeverSec = 0;
				isStart = false;
			}

			cntCreateEvidenceSec += Time.deltaTime;
			if (cntCreateEvidenceSec >= CreateEviDeraySec)
			{
				cntCreateEvidenceSec = 0;
				Vector3 pos = new Vector3(Random.Range(MinRangeObj.transform.position.x,MaxRangeObj.transform.position.x),MaxRangeObj.transform.position.y,MaxRangeObj.transform.position.z);
				Item item = Instantiate(EvidenceObj, pos, transform.rotation).GetComponent<Item>();
				item.SetFeverValue(Random.Range(1, 4));
			}

		}
	}

	public void StartFever(float _feverSec)
	{
		cntFeverSec = _feverSec;
		isStart = true;
	}
}
