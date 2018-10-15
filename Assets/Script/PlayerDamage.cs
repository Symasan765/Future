using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour {

	public GameObject effectObj;
	GameObject nowEffectObj = null;
	ParticleSystem ps = null;
	void Update()
	{
		if (nowEffectObj)
		{
			if(ps.isStopped)
			{
				Destroy(nowEffectObj);
				nowEffectObj = null;
				ps = null;
			}
		}
	}

	public void StartEffect()
	{
		if (!nowEffectObj)
		{
			nowEffectObj = Instantiate(effectObj, transform.position, transform.rotation);
			ps = nowEffectObj.GetComponent<ParticleSystem>();

			ps.Play();
		}
	}
}
