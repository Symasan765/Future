using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamAttack : MonoBehaviour {

	bool InitFlag = false;

	public void SetData(GameObject origin ,GameObject target)
	{
		if (InitFlag == false)
		{
			transform.position = origin.transform.position;
			transform.localRotation.SetLookRotation(target.transform.position);
			InitFlag = true;
			GetComponent<Rigidbody>().AddForce((target.transform.position - origin.transform.position).normalized * 50.0f, ForceMode.Impulse);
		}
	}
}
