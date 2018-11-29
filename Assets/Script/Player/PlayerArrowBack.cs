using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArrowBack : MonoBehaviour
{
	Vector3 startPosition;
	void Start()
	{
		startPosition = transform.localPosition;
	}

	void Update ()
	{
		transform.localPosition = startPosition;
		transform.position = new Vector3(transform.position.x, transform.position.y, 0.2f);
	}
}
