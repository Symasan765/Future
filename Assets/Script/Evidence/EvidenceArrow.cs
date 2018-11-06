using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvidenceArrow : MonoBehaviour {

	public GameObject ModelObj;

	private SpriteRenderer spRenderer;
	private MeshRenderer modelMeshRenderer;
	private BoxCollider parentBoxCollider;
	private BoxCollider boxCollider;
	private Item parentItem;

	private bool isStayTrigger = false;

	void Start()
	{
		parentItem = transform.parent.GetComponent<Item>();
		boxCollider = GetComponent<BoxCollider>();
		parentBoxCollider = transform.parent.GetComponent<BoxCollider>();
		modelMeshRenderer = ModelObj.GetComponent<MeshRenderer>();
		spRenderer = GetComponent<SpriteRenderer>();
	}

	void Update()
	{
		if (parentBoxCollider.isTrigger)
		{
			boxCollider.enabled = false;
		} else
		{
			boxCollider.enabled = true;
		}

		if (!isStayTrigger)
		{
			if (parentItem.isCheckCreatePosition)
			{
				spRenderer.enabled = false;
			} else
			{
				spRenderer.enabled = true;
			}
			//spRenderer.enabled = modelMeshRenderer.enabled;
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Player")
		{
			isStayTrigger = true;
			spRenderer.enabled = false;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			isStayTrigger = false;
			spRenderer.enabled = true;
		}
	}
}
