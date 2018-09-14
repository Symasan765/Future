using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
	[SerializeField]
	private float Speed = 2.0f;
	[SerializeField]
	private float JumpPower = 10.0f;
	[SerializeField]
	private float Gravity = 1.0f;
	[SerializeField]
	private int GetItemBlankTime = 30;
	[SerializeField]
	private bool flgJump = false;
	[SerializeField]
	private bool flgGetItem = false;

	public GameObject ItemPosition;

	private NavMeshAgent agent;
	private GameObject getItemObj;

	private float moveSpeed;
	private float jumpSpeed;
	private float jumpStartYPosition;
	private int cntGetItemBlankTime = 0;
	void Start ()
	{
		agent = GetComponent<NavMeshAgent>();
	}
	
	void Update ()
	{
		if (cntGetItemBlankTime > 0) cntGetItemBlankTime--;

		if (XPad.Get.GetTrigger(XPad.KeyData.A, 0))
		{
			ReleaseItem();
		}
		if (XPad.Get.GetTrigger(XPad.KeyData.X, 0))
		{
			JumpStart();
		}

		Move();
		Jump();
	}

	private void Move()
	{
		if (agent.enabled)
		{
			if (flgGetItem)
			{
				moveSpeed = Speed / 2;
			} else
			{
				moveSpeed = Speed;
			}
			Vector3 move = Vector3.forward * Input.GetAxis("Vertical") + Vector3.right * Input.GetAxis("Horizontal");
			agent.Move(move * Time.deltaTime * moveSpeed);
		}
	}

	private void JumpStart()
	{
		if (!flgJump)
		{
			agent.enabled = false;
			jumpStartYPosition = transform.position.y;
			jumpSpeed = JumpPower;
			flgJump = true;
		}
	}

	private void Jump()
	{
		if (flgJump)
		{
			transform.position += Vector3.up * jumpSpeed * Time.deltaTime;
			jumpSpeed -= Gravity;
			if (jumpSpeed <= JumpPower * -1)
			{
				agent.enabled = true;
				flgJump = false;
				transform.position = new Vector3(transform.position.x, jumpStartYPosition, transform.position.z);
			}
		}
	}

	private void GetItem(GameObject _itemObj)
	{
		if (!flgGetItem && cntGetItemBlankTime == 0)
		{
			getItemObj = _itemObj.gameObject;
			cntGetItemBlankTime = GetItemBlankTime;
			flgGetItem = true;
			_itemObj.transform.parent = transform;
			Rigidbody itemRb = _itemObj.GetComponent<Rigidbody>();
			itemRb.useGravity = false;
			itemRb.isKinematic = true;
			Item item = _itemObj.GetComponent<Item>();
			item.SetItemLocalPosition(ItemPosition.transform.localPosition);
			item.flgMoveToGetPos = true;
		}
	}

	private void ReleaseItem()
	{
		if (cntGetItemBlankTime == 0 && flgGetItem)
		{
			getItemObj.transform.parent = null;
			cntGetItemBlankTime = GetItemBlankTime;
			flgGetItem = false;
			Rigidbody itemRb = getItemObj.GetComponent<Rigidbody>();
			Item item = getItemObj.GetComponent<Item>();
			itemRb.useGravity = true;
			itemRb.isKinematic = false;
			itemRb.AddForce(transform.forward * 5, ForceMode.Impulse);
			item.flgMoveToGetPos = false;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (XPad.Get.GetTrigger(XPad.KeyData.A, 0))
		{
			GetItem(other.gameObject);
		}
	}

}
