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
	private float DashSpeed = 10.0f;		//ダッシュ速度
	[SerializeField]
	private float JumpPower = 10.0f;	//ジャンプ力
	[SerializeField]
	private float Gravity = 1.0f;		//キャラ固有重力
	[SerializeField]
	private int AttackFrame = 10;		//攻撃持続フレーム
	[SerializeField]
	private int GetItemBlankFrame = 30;	//アイテムを持つ&捨てる時の硬直フレーム
	[SerializeField]
	private float CanHoldItemDistance = 0.1f;	//机を運べるようになる範囲
	[SerializeField]
	private int MentalGaugeMax = 100;		//メンタルゲージ最大値
	[SerializeField]
	private int MentalGaugeAccumulateSpeed = 5;

	private int mentalGauge = 0;
	private bool isJump = false;
	private bool isDash = false;
	private bool isHoldItem = false;
	private bool isHoldDesk = false;
	private bool isAttack = false;
	private bool isDamage = false;
	private bool isMove = false;
	private int angleValue = 0;
	public GameObject ItemPosition;
	public GameObject AttackCollisionObj;
	public GameObject RotateObj;

	private GameObject getItemObj;
	private GameObject holdDeskObj;
	private Animator animator;
	private Rigidbody rb;

	private float nowMoveSpeed;
	private float jumpSpeed;
	private int cntGetItemBlankTime = 0;
	private int cntAttackFrame = 0;
	private int cntDamageFrame = 0;
	private Vector3 holdDeskDirction;	//机を持った時の移動方向
	private Vector3 oldLeftStick;
	private float rotationValue = 0;
	void Start ()
	{
		rb = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();
	}
	
	void Update ()
	{
		animator.SetBool("isDamage", isDamage);
		animator.SetInteger("cntGetItemBlankTime", cntGetItemBlankTime);
		if (!isDamage)
		{
			Rotate();
			if (XPad.Get.GetTrigger(XPad.KeyData.A, PlayerIndex))
			{
				if (cntGetItemBlankTime == 0)
				{
					ReleaseItem();
				}
			}
			if (XPad.Get.GetTrigger(XPad.KeyData.X, PlayerIndex))
			{
				JumpStart();
			}

			SerchItem();

			if (XPad.Get.GetTrigger(XPad.KeyData.A, PlayerIndex))
			{
				AttackStart();
			}
			Attack();

			//証拠を持っている時メンタルゲージ増加(とりあえず何か持ってたら溜まる)
			if (isHoldItem)
			{
				if (mentalGauge < MentalGaugeMax && Time.frameCount % MentalGaugeAccumulateSpeed == 0)
				{
					mentalGauge++;
				}

			}
		} else
		{
			Damage();
		}

		//SerchMoveDesk();	//机を動かす処理、仕様変更されたので使わないかも
	}

    void FixedUpdate() {
		rb.AddForce(Vector3.down * Gravity);
		if (!isDamage && cntAttackFrame == 0 && cntGetItemBlankTime == 0)
		{
			Move();
		}
        Jump();
    }

	private void Rotate()
	{
		if (angleValue == 1)
		{
			RotateObj.transform.localEulerAngles = new Vector3(0, 55, 0);
		}
		if(angleValue == -1)
		{
			RotateObj.transform.localEulerAngles = new Vector3(0, -55, 0);
		}
	}

	//移動
	private void Move()
	{
        Vector2 LeftStick = XPad.Get.GetLeftStick(PlayerIndex);
		rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
		//キャラの向きを変える
        if (LeftStick.x != 0)
		{
			//スティックが倒れている時は動いている状態
			isMove = true;
			if (LeftStick.x > 0)
			{
				angleValue = 1;
			} else
			{
				angleValue = -1;
			}
			animator.SetBool("isWalk", true);
		} else
		{
			isMove = false;
			isDash = false;
			animator.SetBool("isWalk", false);
		}

		//すり抜け床の処理
		if (XPad.Get.GetLeftStick(PlayerIndex).y < -0.2f)
		{
			GetComponent<ThroughFloorCheck>().IsFall(0.5f);
		}

		//アイテムを持った&机を持った時移動速度を半減(仮)
		if (isHoldItem || isHoldDesk)
		{
			nowMoveSpeed = WalkSpeed / 2;
		} else
		{
			if (IsOnGround() && Mathf.Abs(LeftStick.x) - Mathf.Abs(oldLeftStick.x) > 0.75f)
			{
				Debug.Log("ダッシュ開始");
				isDash = true;
			}
			if (isDash)
			{
				nowMoveSpeed = DashSpeed;
				if (!IsOnGround())
				{
					nowMoveSpeed = WalkSpeed;
				}
			} else
			{
				nowMoveSpeed = WalkSpeed;
			}
		}

        if (isHoldDesk) {
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
            //Debug.Log(move);
			nowMoveSpeed = nowMoveSpeed * LeftStick.x;
			if (nowMoveSpeed < 0)
			{
				nowMoveSpeed *= -1;
			}
            this.transform.Translate(move * Time.deltaTime * nowMoveSpeed);
        }
		oldLeftStick = LeftStick;
	}

	private void AttackStart()
	{
		if (cntGetItemBlankTime == 0 && !isJump && !isAttack && !isHoldDesk && !isHoldItem)
		{
			XPad.Get.SetVibration(PlayerIndex, 0.2f, 0.2f, 0.1f);
			BoxCollider col = AttackCollisionObj.GetComponent<BoxCollider>();
			col.enabled = true;
			cntAttackFrame = 0;
			isAttack = true;
			isDash = false;
		}
	}
	private void Attack()
	{
		animator.SetBool("isAttack", isAttack);
		if (isAttack)
		{
			cntAttackFrame++;
			if (cntAttackFrame == AttackFrame || isHoldItem)
			{
				BoxCollider col = AttackCollisionObj.GetComponent<BoxCollider>();
				col.enabled = false;
				isAttack = false;
				cntAttackFrame = 0;
			}
		}
	}

	//攻撃が敵にヒットした
	public void HitPlayerAttack()
	{
		XPad.Get.SetVibration(PlayerIndex, 0.7f, 0.7f, 0.2f);
		mentalGauge -= Random.Range(20, 40);
		if (mentalGauge < 0)
		{
			mentalGauge = 0;
		}
	}

	//敵の攻撃が自分にヒットした
	public void HitBossAttack()
	{
		if (isHoldItem)
		{
			ReleaseItem();
		}
		XPad.Get.SetVibration(PlayerIndex, 1.0f, 1.0f, 0.5f);
		cntDamageFrame = 40;
		isDamage = true;
		mentalGauge += Random.Range(5, 15);
		if (mentalGauge > MentalGaugeMax)
		{
			mentalGauge = MentalGaugeMax;
		}
		Debug.Log("Player" + PlayerIndex + "にボスの攻撃がヒットした！");
	}

	//今のメンタルゲージを返す
	public int GetMentalGauge()
	{
		return mentalGauge;
	}

	//ダメージ処理
	private void Damage()
	{
		isMove = false;
		if (cntDamageFrame > 0)
		{
			cntDamageFrame--;
			if (cntDamageFrame == 0)
			{
				isDamage = false;
			}
		}
	}

	//前方に机があるか調べる
	private void SerchMoveDesk()
	{
		//Debug.DrawRay(transform.position, transform.forward);
		if (isHoldDesk)
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

	//ジャンプ開始
	private void JumpStart()
	{
		if (IsOnGround() && !isJump && !isAttack)
		{
			jumpSpeed = JumpPower;
			isJump = true;
		}
	}


	//移動しているか
	public bool IsMove()
	{
		return isMove;
	}

	//接地しているか
	private bool IsOnGround()
	{
		RaycastHit hit;
		Physics.Raycast(transform.position, transform.up * -1, out hit, 0.08f);

		if(hit.collider){
			rb.velocity = Vector3.zero;
			return true;
		}
		return false;
	}

	//ジャンプ中
	private void Jump()
	{
		animator.SetBool("isJump", isJump);
		if (isJump)
		{
			transform.position += Vector3.up * jumpSpeed * Time.deltaTime;
			jumpSpeed -= Gravity;
			if (jumpSpeed <= 0)
			{
				isJump = false;
			}
		}
	}


	//持てるアイテムを探す
	private void SerchItem()
	{
		if (cntGetItemBlankTime > 0) cntGetItemBlankTime--;

		if (!isAttack && !isHoldItem && cntGetItemBlankTime == 0)
		{
			//Debug.DrawRay(transform.position, transform.forward * CanHoldItemDistance, Color.red);
			RaycastHit hit;
			Physics.Raycast(transform.position, transform.forward, out hit, CanHoldItemDistance);
			if (hit.collider)
			{
				//Debug.Log(hit.collider.name + "を取得可能");
				if (XPad.Get.GetTrigger(XPad.KeyData.A, PlayerIndex))
				{
					if (hit.collider.tag == "Player")
					{
						ReceiveItem(hit.collider.gameObject);
					} else
					{
						GetItem(hit.collider.gameObject);
					}
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
			XPad.Get.SetVibration(PlayerIndex, 0.2f, 0.2f, 0.1f);
			getItemObj = _itemObj.gameObject;
			cntGetItemBlankTime = GetItemBlankFrame;
			isHoldItem = true;
			_itemObj.transform.parent = transform;
			Rigidbody itemRb = _itemObj.GetComponent<Rigidbody>();
			BoxCollider col = _itemObj.GetComponent<BoxCollider>();
			col.isTrigger = true;
			itemRb.useGravity = false;
			itemRb.isKinematic = true;

			item.SetItemLocalPosition(ItemPosition.transform.localPosition);
			item.flgMoveToGetPos = true;
		}
	}

	//アイテムの親を変更
	public void ChangeItemParent(GameObject _newParentObj)
	{
		isHoldItem = false;
		cntGetItemBlankTime = GetItemBlankFrame;
		getItemObj.transform.parent = _newParentObj.transform;
		getItemObj = null;
	}

	//アイテムを渡す
	private void ReceiveItem(GameObject passPlayerObj)
	{
		Player player = passPlayerObj.GetComponent<Player>();
		if (player.isHoldItem)
		{
			XPad.Get.SetVibration(PlayerIndex, 0.2f, 0.2f, 0.1f);
			isHoldItem = true;
			getItemObj = player.getItemObj;
			player.ChangeItemParent(this.gameObject);
			cntGetItemBlankTime = GetItemBlankFrame;
		}
	}

	//アイテムを離す
	private void ReleaseItem()
	{
		if (getItemObj)
		{
			if (isHoldItem)
			{
				XPad.Get.SetVibration(PlayerIndex, 0.2f, 0.2f, 0.1f);
				getItemObj.transform.parent = null;
				cntGetItemBlankTime = GetItemBlankFrame;
				isHoldItem = false;
				Rigidbody itemRb = getItemObj.GetComponent<Rigidbody>();
				Item item = getItemObj.GetComponent<Item>();
				BoxCollider col = getItemObj.GetComponent<BoxCollider>();
				col.isTrigger = false;
				itemRb.useGravity = true;
				itemRb.isKinematic = false;
				itemRb.AddForce(transform.forward * 1, ForceMode.Impulse);
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
			isHoldDesk = true;
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
		if (isHoldDesk)
		{
			NavMeshObstacle navMeshObstacle = holdDeskObj.GetComponent<NavMeshObstacle>();
			navMeshObstacle.enabled = true;
			isHoldDesk = false;
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

	//アイテムを取得しているかどうかを返す
	public bool IsHoldItem()
	{
		return isHoldItem;
	}

}
