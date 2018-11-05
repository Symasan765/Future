using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeverManager : MonoBehaviour {

	public GameObject cameraObj;

	[SerializeField]
	private float FeverSec = 10;
	[SerializeField]
	private float CreateEviDeraySec = 0.2f;
	[SerializeField]
	private float HorizontalRange = 10;
	[SerializeField]
	private float VirticalRange = 10;
	[SerializeField]
	private float VirtivalOriginPosition = 7;
	public GameObject EvidenceObj;

	private bool isStart = false;
	
	private float feverSec;
	private float cntFeverSec = 0;
	private float cntCreateEvidenceSec = 0;
	private GameObject BazookaObj;
	private EffectManager effectManager;
	private FeverManager feverManager;
	void Start ()
	{
		feverSec = FeverSec;
		BazookaObj = GameObject.Find("BazookaRifle");
		effectManager = GameObject.Find("EffectManager").GetComponent<EffectManager>();
		feverManager = GetComponent<FeverManager>();
	}
	
	void Update ()
	{
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
				Vector3 pos = new Vector3(Random.Range(HorizontalRange / 2 * -1,HorizontalRange / 2),Random.Range(VirticalRange / 2 * -1,VirticalRange / 2) + VirtivalOriginPosition,transform.position.z);
				Item item = Instantiate(EvidenceObj, pos, transform.rotation).GetComponent<Item>();
				item.SetNecessaryComponent(BazookaObj, feverManager, effectManager);
				item.SetFeverValue(Random.Range(1, 4));
			}
		}
	}

	public void StartFever(float _feverSec)
	{
		if (!isStart)
		{
			Vector3 pos = new Vector3(cameraObj.transform.position.x, cameraObj.transform.position.y, 0);
			Debug.Log("FEVER開始(⋈◍＞◡＜◍)。✧♡");
			effectManager.PlayFEVER(-1, pos, -5);
			cntFeverSec = _feverSec;
			isStart = true;
		}
	}

	public void SetCreateEvidenceSecMax()
	{
		cntCreateEvidenceSec = CreateEviDeraySec;
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
