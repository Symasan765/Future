using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageEvidenceNum : MonoBehaviour {

	public GameObject NormalTextObj;
	public GameObject FeverTextObj;
	private FeverManager feverManager;

	[SerializeField]
	private Vector3 waitPosition;
	private Vector3[] startPosition = new Vector3[2];
	private bool drawMessage = false;

	private float speed = 0.4f;

	void Start ()
	{
		startPosition[0] = startPosition[1] = new Vector3(0, 0, 0);
		feverManager = GameObject.Find("FeverManager").GetComponent<FeverManager>();
	}
	
	
	void Update ()
	{
		if (feverManager.IsFever())
		{
			NormalTextObj.transform.localPosition = Vector3.Lerp(NormalTextObj.transform.localPosition, waitPosition, speed);
			FeverTextObj.transform.localPosition = Vector3.Lerp(FeverTextObj.transform.localPosition, startPosition[1], speed);
		} else
		{
			FeverTextObj.transform.localPosition = Vector3.Lerp(FeverTextObj.transform.localPosition, waitPosition, speed);
			NormalTextObj.transform.localPosition = Vector3.Lerp(NormalTextObj.transform.localPosition, startPosition[0], speed);
		}
	}
}
