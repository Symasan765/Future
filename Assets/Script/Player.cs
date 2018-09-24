using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
	[SerializeField]
	private int PlayerIndex = 0;		//プレイヤー番号
	[SerializeField]
	private float Speed = 2.0f;			//移動速度
	[SerializeField]
	private float JumpPower = 10.0f;	//ジャンプ力
	[SerializeField]
	private float Gravity = 1.0f;		//ジャンプ用重力
	[SerializeField]
	private int GetItemBlankTime = 30;	//アイテムを持つ&捨てる時の硬直フレーム
	[SerializeField]
	private float CanMoveDeskDistance = 0.1f;	//机を運べるようになる範囲
	[SerializeField]
	private bool flgJump = false;
	[SerializeField]
	private bool flgGetItem = false;
	[SerializeField]
	private bool flgHoldDesk = false;

	public GameObject ItemPosition;

	private NavMeshAgent agent;
	private GameObject getItemObj;
	private GameObject holdDeskObj;
	private Animator animator;
	private float nowMoveSpeed;
	private float jumpSpeed;
	private float jumpStartYPosition;
	private int cntGetItemBlankTime = 0;
	private Vector3 holdDeskDirction;	//机を持った時の移動方向

	void Start ()
	{
		animator = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
	}
	
	void Update ()
	{
		if (cntGetItemBlankTime > 0) cntGetItemBlankTime--;

		if (XPad.Get.GetTrigger(XPad.KeyData.A, PlayerIndex))
		{
			ReleaseItem();
		}
		if (XPad.Get.GetTrigger(XPad.KeyData.X, PlayerIndex))
		{
			JumpStart();
		}
		SerchMoveDesk();
		Move();
		Jump();
	}

	private void Move()
	{

		if (XPad.Get.GetLeftStick(PlayerIndex).x != 0 || XPad.Get.GetLeftStick(PlayerIndex).y != 0)
		{
			animator.SetBool("isWalk", true);
		} else
		{
			animator.SetBool("isWalk", false);
		}

		if (agent.enabled)
		{
			//アイテムを持った&机を持った時移動速度を半減(仮)
			if (flgGetItem || flgHoldDesk)
			{
				nowMoveSpeed = Speed / 2;
			} else
			{
				nowMoveSpeed = Speed;
			}

			if (flgHoldDesk)
			{
				//机を持っている時の移動
				if (holdDeskDirction.x != 0)
				{
					Vector3 move = holdDeskDirction * XPad.Get.GetLeftStick(PlayerIndex).x;
					agent.Move(move * Time.deltaTime * nowMoveSpeed);
				}
				if (holdDeskDirction.z != 0)
				{
					Vector3 move = holdDeskDirction * XPad.Get.GetLeftStick(PlayerIndex).y;
					agent.Move(move * Time.deltaTime * nowMoveSpeed);
				}

			} else
			{
				//机をもっていない時の移動
				Vector3 move = Vector3.forward * XPad.Get.GetLeftStick(PlayerIndex).y + Vector3.right * XPad.Get.GetLeftStick(PlayerIndex).x;
				agent.Move(move * Time.deltaTime * nowMoveSpeed);
				//倒した方向に向く
				if (XPad.Get.GetLeftStick(PlayerIndex).x != 0 || XPad.Get.GetLeftStick(PlayerIndex).y != 0)
				{
					transform.forward = new Vector3(XPad.Get.GetLeftStick(PlayerIndex).x, transform.forward.y, XPad.Get.GetLeftStick(PlayerIndex).y);
				}
			}
		}
	}

	//前方に机があるか調べる
	private void SerchMoveDesk()
	{
		Debug.DrawRay(transform.position, transform.forward);
		if (flgHoldDesk)
		{
			if (XPad.Get.GetRelease(XPad.KeyData.A, PlayerIndex))
			{
				ReleaseDesk();
			}
		} else
		{
			RaycastHit hit;
			Physics.Raycast(transform.position, transform.forward, out hit, CanMoveDeskDistance);
			if (hit.collider)
			{
				if (XPad.Get.GetPress(XPad.KeyData.A, PlayerIndex))
				{
					HoldDesk(hit.collider.gameObject);
				}
			}
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

	//アイテムを持つ
	private void GetItem(GameObject _itemObj)
	{
		Item item = _itemObj.GetComponent<Item>();
		if (item)
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

				item.SetItemLocalPosition(ItemPosition.transform.localPosition);
				item.flgMoveToGetPos = true;
			}
		}
	}

	//アイテムを離す
	private void ReleaseItem()
	{
		if (getItemObj)
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
				getItemObj = null;
			}
		}
	}

	//机を持つ
	private void HoldDesk(GameObject _deskObj)
	{
		if (_deskObj.tag == "MoveDesk")
		{
			holdDeskObj = _deskObj;
			flgHoldDesk = true;
			holdDeskObj.transform.parent = transform;
			NavMeshObstacle navMeshObstacle = _deskObj.GetComponent<NavMeshObstacle>();
			navMeshObstacle.enabled = false;
			RaycastHit hit;
			BoxCollider boxColl = _deskObj.GetComponent<BoxCollider>();
			//右
			Physics.BoxCast(_deskObj.transform.position, boxColl.size, _deskObj.transform.right, out hit);
			if (hit.collider) holdDeskDirction = Vector3.right;
			//左
			Physics.BoxCast(_deskObj.transform.position, boxColl.size, _deskObj.transform.right * -1, out hit);
			if (hit.collider) holdDeskDirction = Vector3.right;
			//上
			Physics.BoxCast(_deskObj.transform.position, boxColl.size, _deskObj.transform.forward, out hit);
			if (hit.collider) holdDeskDirction = Vector3.forward;
			//下
			Physics.BoxCast(_deskObj.transform.position, boxColl.size, _deskObj.transform.forward * -1, out hit);
			if (hit.collider) holdDeskDirction = Vector3.forward;
		}
	}

	//机を離す
	private void ReleaseDesk()
	{
		if (flgHoldDesk)
		{
			NavMeshObstacle navMeshObstacle = holdDeskObj.GetComponent<NavMeshObstacle>();
			navMeshObstacle.enabled = true;
			flgHoldDesk = false;
			holdDeskObj.transform.parent = null;
			holdDeskObj = null;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (XPad.Get.GetTrigger(XPad.KeyData.A, PlayerIndex))
		{
			GetItem(other.gameObject);
		}

	}

}
