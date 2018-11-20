using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEffect : MonoBehaviour {

	public GameObject[] TrailObjs;
	public GameObject[] ParticleObjs;

	private TrailRenderer[] trailRenderers = new TrailRenderer[2];
	private ParticleSystem[] particleSystems = new ParticleSystem[2];

	public bool isEnable = false;

	void Start ()
	{
		for (int i = 0; i < 2; i++)
		{
			trailRenderers[i] = TrailObjs[i].GetComponent<TrailRenderer>();
			particleSystems[i] = ParticleObjs[i].GetComponent<ParticleSystem>();
		}
	}
	
	void Update ()
	{
		SetEffectFlg(isEnable);
	}

	void SetEffectFlg(bool _flg)
	{
		for (int i = 0; i < 2; i++)
		{
			if (isEnable)
			{
				trailRenderers[i].enabled = true;
				particleSystems[i].Play();
			} else
			{
				trailRenderers[i].enabled = false;
				particleSystems[i].Stop();
			}
		}
	}

	void PlayEffect()
	{
		isEnable = true;
	}
	void StopEffect()
	{
		isEnable = false;
	}
}
