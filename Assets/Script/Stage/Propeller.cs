using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propeller : MonoBehaviour {

	private float Speed = 200.0f;

	private Vector3 angle;
	// Use this for initialization
	void Start ()
	{
		angle = transform.eulerAngles;
		angle.y = Random.Range(0, 360);
	}
	
	// Update is called once per frame
	void Update ()
	{
		angle.y += Time.deltaTime * Speed;
		transform.eulerAngles = angle;
	}
}
