using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

	private float MoveSpeed = 5.0f;

	private Vector3 getPosition;
	[HideInInspector]
	public bool flgMoveToGetPos;
	
	void Start ()
	{
		
	}
	
	void Update ()
	{
		if (flgMoveToGetPos)
		{
			if (transform.position != getPosition)
			{
				Vector3 vec = getPosition - transform.localPosition;
				transform.localPosition += vec * MoveSpeed * Time.deltaTime;
			}
		}
	}

	public void SetItemLocalPosition(Vector3 _pos)
	{
		getPosition = _pos;
	}
}
