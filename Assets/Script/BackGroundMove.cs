using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMove : MonoBehaviour {

	private Vector3 startPosition;
	private Vector3 startCameraPosition;
	private GameObject CameraObj;
	public float MovePosition = 0.5f;
	public float Speed = 0.5f;

	void Start ()
	{
		CameraObj = GameObject.Find("Main Camera");
		startPosition = transform.position;
		startCameraPosition = CameraObj.transform.position;
	}

	void Update()
	{
		if (ResultTransManager.m_InTheGame == true)
		{
			Vector3 cameraMovePos = new Vector3(CameraObj.transform.position.x - startCameraPosition.x, CameraObj.transform.position.y - startCameraPosition.y, CameraObj.transform.position.z);

			transform.position = Vector3.Lerp(transform.position, new Vector3(startPosition.x + cameraMovePos.x * MovePosition, startPosition.y + cameraMovePos.y * MovePosition, startPosition.z), Speed);
		}
	}
}
