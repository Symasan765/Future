using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MentalGauge : MonoBehaviour {

	public GameObject PlayerObj;
	private GameObject cameraObj;

	private Player player;

	private Vector3 startScale;

	void Start ()
	{
		cameraObj = GameObject.Find("Main Camera");
		startScale = transform.localScale;
		player = PlayerObj.GetComponent<Player>();
	}
	

	void Update () {
		transform.forward = cameraObj.transform.forward * -1;
		transform.localEulerAngles = new Vector3(90, transform.localEulerAngles.y, transform.localEulerAngles.z);
		transform.localScale = new Vector3(startScale.x * player.GetMentalGauge()*0.01f, transform.localScale.y, transform.localScale.z);
	}
}
