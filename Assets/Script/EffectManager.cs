using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour {

	public GameObject EffectDUM;
	public GameObject EffectPYON;
	public GameObject EffectTap;
	public GameObject EffectDelete;

	private ParticleSystem psDelete;

	public void PlayDelete(Vector3 _pos)
	{
		psDelete = Instantiate(EffectDelete, _pos, transform.rotation).GetComponent<ParticleSystem>();
		psDelete.Play();
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

}
