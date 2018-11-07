using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour {

	public GameObject EffectDUM;
	public GameObject EffectPYON;
	public GameObject EffectTap;
	public GameObject EffectDelete;
	public GameObject EffectBOOM;
	public GameObject EffectCreateEvidence;
	public GameObject EffectExplosion;
	public GameObject EffectFEVER;
	public GameObject EffectSMASH;
	public GameObject EffectLightning;
	public GameObject EffectDown;
	public GameObject EffectRespawnHeart;

	private ParticleSystem psDelete;
	private ParticleSystem psCreateEvidence;
	private ParticleSystem psExplosion;
	private ParticleSystem psLightning;
	private ParticleSystem psDown;
	private ParticleSystem psRespawnHeart;

	public void PlayRespawnHeart(Vector3 _pos)
	{
		psRespawnHeart = Instantiate(EffectRespawnHeart, _pos, transform.rotation).GetComponent<ParticleSystem>();
		psRespawnHeart.Play();
	}

	public void PlayDown(Vector3 _pos)
	{
		psDown = Instantiate(EffectDown, _pos, transform.rotation).GetComponent<ParticleSystem>();
		psDown.Play();
	}

	public void PlayLightning(Vector3 _pos)
	{
		psLightning = Instantiate(EffectLightning, _pos, transform.rotation).GetComponent<ParticleSystem>();
		psLightning.Play();
	}

	public void PlayExplosion(Vector3 _pos)
	{
		psExplosion = Instantiate(EffectExplosion, _pos, transform.rotation).GetComponent<ParticleSystem>();
		psExplosion.Play();
	}

	public void PlayCreateEvidence(Vector3 _pos)
	{
		psCreateEvidence = Instantiate(EffectCreateEvidence, _pos, transform.rotation).GetComponent<ParticleSystem>();
		psCreateEvidence.Play();
	}

	public void PlayDelete(Vector3 _pos)
	{
		psDelete = Instantiate(EffectDelete, _pos, transform.rotation).GetComponent<ParticleSystem>();
		psDelete.Play();
	}

	public void PlayFEVER(int _index, Vector3 _pos, float _zpos)
	{
		CreateEffect(EffectFEVER, _index, _pos, _zpos);
	}

	public void PlaySMASH(int _index, Vector3 _pos,float _zpos)
	{
		CreateEffect(EffectSMASH, _index, _pos, _zpos);
	}

	public void PlayBOOM(int _index, Vector3 _pos)
	{
		CreateEffect(EffectBOOM, _index, _pos);
	}

	public void PlayDUM(int _index, Vector3 _pos)
	{
		CreateEffect(EffectDUM, _index, _pos);
	}

	public void PlayPYON(int _index,Vector3 _pos)
	{
		CreateEffect(EffectPYON, _index, _pos);
	}

	public void PlayTap(int _index, Vector3 _pos)
	{
		CreateEffect(EffectTap, _index, _pos);
	}
	private void CreateEffect(GameObject _obj,int _index,Vector3 _pos)
	{
		Vector3 p = new Vector3(_pos.x, _pos.y, -2);
		GameObject e = Instantiate(_obj, p, transform.rotation);
		e.transform.parent = transform;
		WordParent w = e.GetComponent<WordParent>();
		w.Play(_index);
	}
	private void CreateEffect(GameObject _obj, int _index, Vector3 _pos,float _zPos)
	{
		Vector3 p = new Vector3(_pos.x, _pos.y, _zPos);
		GameObject e = Instantiate(_obj, p, transform.rotation);
		e.transform.parent = transform;
		WordParent w = e.GetComponent<WordParent>();
		w.Play(_index);
	}
}
