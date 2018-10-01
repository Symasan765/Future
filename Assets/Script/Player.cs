using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
	[SerializeField]
	private int PlayerIndex = 0;		//プレイヤー番号
	[SerializeField]
	private float WalkSpeed = 2.0f;			//移動速度
	[SerializeField]
	private float JumpPower = 10.0f;	//ジャンプ力
	[SerializeField]
	private float Gravity = 1.0f;		//ジャンプ用重力
	[SerializeField]
	private int GetItemBlankTime = 30;	//アイテムを持つ&捨てる時の硬直フレーム
	[SerializeField]
	private float CanHoldItemDistance = 0.1f;	//机を運べるようになる範囲
	[SerializeField]
	private bool flgJump = false;
	[SerializeField]
	private bool flgHoldItem = false;
	private bool flgHoldDesk = false;
	[SerializeField]
	private int mentalGauge = 0;
	public GameObject ItemPosition;

	private GameObject getItemObj;
	private GameObject holdDeskObj;
	private Animator animator;
	private float nowMoveSpeed;
	private float jumpSpeed;
	private int cntGetItemBlankTime = 0;
	private Vector3 holdDeskDirction;	//机を持った時の移動方向

	void Start ()
	{
		animator = GetComponent<Animator>();
	}
	
	void Update ()
	{

		if (XPad.Get.GetTrigger(XPad.KeyData.A, PlayerIndex))
		{
			ReleaseItem();
		}
		if (XPad.Get.GetTrigger(XPad.KeyData.X, PlayerIndex))
		{
			JumpStart();
		}
		SerchItem();

		//証拠を持っている時メンタルゲージ増加(とりあえず何か持ってたら溜まる)
		if (flgHoldItem)
		{
			if (mentalGauge < 100)
			{
				mentalGauge++;
			}

		}

		//SerchMoveDesk();	//机を動かす処理、仕様変更されたので使わないかも
	}

    void FixedUpdate() {
        Move();
        Jump();
    }

	private void Move()
	{
        Vector2 LeftStick = XPad.Get.GetLeftStick(PlayerIndex);

        if (LeftStick.x != 0 || LeftStick.y != 0)
		{
			animator.SetBool("isWalk", true);
		} else
		{
			animator.SetBool("isWalk", false);
		}

		//アイテムを持った&机を持った時移動速度を半減(仮)
		if (flgHoldItem || flgHoldDesk)
			{
				nowMoveSpeed = WalkSpeed / 2;
			} else
			{
				nowMoveSpeed = WalkSpeed;
			}

        if (flgHoldDesk) {
            //机を持っている時の移動
            if (holdDeskDirction.x != 0) {
                Vector3 move = holdDeskDirction * LeftStick.x;
            }
            if (holdDeskDirction.z != 0) {
                Vector3 move = holdDeskDirction * LeftStick.y;
            }

        }
        else {
            //机をもっていない時の移動
            //倒した方向(X軸のみ)に向く
            if (LeftStick.x != 0) {
                transform.forward = new Vector3(LeftStick.x, 0, 0);
            }

            Vector3 move = Vector3.forward * Mathf.Abs(LeftStick.x);
            Debug.Log(move);
            this.transform.Translate(move * Time.deltaTime * nowMoveSpeed);
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
			Physics.Raycast(transform.position, transform.forward, out hit, CanHoldItemDistance);
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
		if (IsOnGround() && !flgJump && !flgHoldItem)
		{
			jumpSpeed = JumpPower;
			flgJump = true;
		}
	}

	private bool IsOnGround()
	{
		RaycastHit hit;
		Physics.Raycast(transform.position, transform.up * -1, out hit, 0.08f);

		if(hit.collider){
			return true;
		}
		return false;
	}

	private void Jump()
	{
		animator.SetBool("isJump", flgJump);
		if (flgJump)
		{
			transform.position += Vector3.up * jumpSpeed * Time.deltaTime;
			jumpSpeed -= Gravity;
			if (jumpSpeed <= 0)
			{
				flgJump = false;
			}
		}
	}


	//持てるアイテムを探す
	private void SerchItem()
	{
		if (cntGetItemBlankTime > 0) cntGetItemBlankTime--;

		if (!flgHoldItem)
		{
			Debug.DrawRay(transform.position, transform.forward * CanHoldItemDistance, Color.red);
			RaycastHit hit;
			Physics.Raycast(transform.position, transform.forward, out hit, CanHoldItemDistance);
			if (hit.collider)
			{
				Debug.Log(hit.collider.name + "を取得可能");
				if (XPad.Get.GetTrigger(XPad.KeyData.A, PlayerIndex))
				{
					GetItem(hit.collider.gameObject);
				}
			}
		}
	}

	//アイテムを持つ
	private void GetItem(GameObject _itemObj)
	{
		Item item = _itemObj.GetComponent<Item>();
		if (item)
		{
			if (!flgHoldItem && cntGetItemBlankTime == 0)
			{
				getItemObj = _itemObj.gameObject;
				cntGetItemBlankTime = GetItemBlankTime;
				flgHoldItem = true;
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
			if (cntGetItemBlankTime == 0 && flgHoldItem)
			{
				getItemObj.transform.parent = null;
				cntGetItemBlankTime = GetItemBlankTime;
				flgHoldItem = false;
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
		/*
		if (XPad.Get.GetTrigger(XPad.KeyData.A, PlayerIndex))
		{
			GetItem(other.gameObject);
		}*/

	}

}
