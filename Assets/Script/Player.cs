using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
	[SerializeField]
	public int PlayerIndex = 0;		//プレイヤー番号
	[SerializeField]
	private float ForwardAngle = 55.0f;
	[SerializeField]
	private float WalkSpeed = 2.0f;			//移動速度
	[SerializeField]
	private float DashSpeed = 10.0f;		//ダッシュ速度
	[SerializeField]
	private float JumpPower = 10.0f;	//ジャンプ力
	[SerializeField]
	private float JumpSpeed = 2.0f;
	[SerializeField]
	private float FallSpeed = 1.0f;
	[SerializeField]
	private float FallSpeedMax = 5.0f;
	[SerializeField]
	private float Gravity = 0.4f;		//キャラ固有重力
	[SerializeField]
	private float BrakePower = 0.4f;	//ブレーキの強さ
	[SerializeField]
	private int MentalGaugeMax = 100;		//メンタルゲージ最大値
	[SerializeField]
	private int MentalGaugeAccumulateSpeed = 5;
	[SerializeField]
	private int InvincibleFrame = 90;

	private int AttackFrame = 25;		//攻撃持続フレーム
	private int GetItemBlankFrame = 5;	//アイテムを持つ&捨てる時の硬直フレーム
	private float CanHoldItemDistance = 0.4f;	//机を運べるようになる範囲

	private int mentalGauge = 0;
	private bool isJump = false;
	private bool isDash = false;
	private bool isHoldItem = false;
	private bool isHoldDesk = false;
	private bool isAttack = false;
	private bool isDamage = false;
	private bool isMove = false;
	public bool isRespawn = false;
	private int angleValue = 0;
	public GameObject ItemPosition;
	public GameObject AttackCollisionObj;
	public GameObject RotateObj;

	private GameObject getItemObj;
	private GameObject holdDeskObj;
	private Animator animator;
	private Rigidbody rb;

	private float nowMoveSpeed;
	private float rightSpeed;
	private float leftSpeed;
	private float jumpSpeed;
	private int cntGetItemBlankTime = 0;
	private int cntAttackFrame = 0;
	private int cntDamageFrame = 0;
	private int cntJumpCheckFrame = 0;
	[SerializeField]private int cntInvincibleFrame = 0;
	private Vector3 holdDeskDirction;	//机を持った時の移動方向
	private Vector3 oldLeftStick;
	private Vector3 respawnPosition;
	private float rotationValue = 0;
	private float dashBrakeSpeed = 1.0f;

	void Start ()
	{
		//SoundManager.Get.PlayBGM("testBGM", true);

		//とりあえずリスポン位置をゲーム開始位置に
		respawnPosition = transform.position;

		rb = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();
	}
	
	void Update ()
	{
		animator.SetBool("isDamage", isDamage);
		animator.SetInteger("cntGetItemBlankTime", cntGetItemBlankTime);
		if (!isDamage)
		{
			if (XPad.Get.GetTrigger(XPad.KeyData.A, PlayerIndex))
			{
				if (cntGetItemBlankTime == 0)
				{
					ReleaseItem();
				}
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
			//無敵フレームのカウント
			if (cntInvincibleFrame > 0)
			{
				cntInvincibleFrame--;
			}
		} else
		{
			Damage();
		}
		Respawn();
		
		//キャラのZ座標は常に0
		transform.position = new Vector3(transform.position.x, transform.position.y, 0);
		Rotate();
	}

    void FixedUpdate()
	{
		Fall();

		if (!isDamage && cntAttackFrame == 0 && cntGetItemBlankTime == 0)
		{
			Move();
		}
        Jump();
    }

	//落下処理
	private void Fall()
	{
		if (rb.velocity.y < FallSpeedMax * -1)
		{
			rb.velocity = new Vector3(rb.velocity.x, FallSpeedMax * -1, rb.velocity.z);
		}

		if (!isJump && !IsOnGround() && XPad.Get.GetLeftStick(PlayerIndex).y < -0.8f)
		{
			rb.velocity = new Vector3(rb.velocity.x, (FallSpeedMax * 2) * -1, rb.velocity.z);
		}

		rb.AddForce(Vector3.down * (Gravity * FallSpeed));
	}

	//キャラの向きを変える
	private void Rotate()
	{
		if (angleValue == 1)
		{
			transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, new Vector3(0, 90, 0), 0.4f);
			RotateObj.transform.localEulerAngles = new Vector3(0, ForwardAngle + 360, 0);
		}
		if(angleValue == -1)
		{
			transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, new Vector3(0, 270, 0), 0.4f);
			RotateObj.transform.localEulerAngles = new Vector3(0, 360 - ForwardAngle, 0);
		}
	}

	//移動
	private void Move()
	{
		animator.SetBool("isDash", isDash);
        Vector2 LeftStick = XPad.Get.GetLeftStick(PlayerIndex);
		rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
		if (LeftStick.x <= 0)
		{
			if (rightSpeed > 0)
			{
				rightSpeed -= BrakePower;
				if (rightSpeed < 0)
				{
					rightSpeed = 0;
				}
			}
		}
		if (LeftStick.x >= 0)
		{
			if (leftSpeed < 0)
			{
				leftSpeed += BrakePower;
				if (leftSpeed > 0)
				{
					leftSpeed = 0;
				}
			}
		}

		//キャラの向きを変える
        if (LeftStick.x != 0)
		{
			//スティックが倒れている時は動いている状態
			isMove = true;
			if (LeftStick.x > 0)
			{
				//スティックが右に倒れている
				angleValue = 1;
			} else
			{
				//スティックが左に倒れている
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
			if (IsOnGround() && Mathf.Abs(LeftStick.x) - Mathf.Abs(oldLeftStick.x) > 0.65f)
			{
				SoundManager.Get.PlaySE("dash");
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
		if (nowMoveSpeed < 0)
		{
			nowMoveSpeed *= -1;
		}
		if (LeftStick.x != 0)
		{
			SetMoveSpeed(angleValue, nowMoveSpeed);
		}

		this.transform.Translate(Vector3.forward * angleValue * Time.deltaTime * (rightSpeed + leftSpeed));
		oldLeftStick = LeftStick;
	}

	//向きごとに速度をセットする
	private void SetMoveSpeed(int _angleValue, float _speed)
	{
		if (_angleValue == 1)
		{
			rightSpeed = _speed;
		}
		if (_angleValue == -1)
		{
			leftSpeed = _speed * -1;
		}
	}

	//攻撃開始
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
			rightSpeed = leftSpeed = 0.0f;
		}
	}

	//攻撃処理
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

	//ボスの攻撃が自分にヒットした
	public void HitBossAttack()
	{
		if (!IsInvincible())
		{
			SoundManager.Get.PlaySE("hit1");
			PlayerDamage pd = GetComponent<PlayerDamage>();
			pd.StartEffect();
			if (isHoldItem)
			{
				ReleaseItem();
			}
			rightSpeed = leftSpeed = 0.0f;
			cntGetItemBlankTime = 0;
			XPad.Get.SetVibration(PlayerIndex, 1.0f, 1.0f, 0.5f);
			cntDamageFrame = 40;
			isDamage = true;
			mentalGauge += Random.Range(5, 15);
			if (mentalGauge > MentalGaugeMax)
			{
				mentalGauge = MentalGaugeMax;
			}
			cntInvincibleFrame = InvincibleFrame;
		}
	}

	//無敵時間かどうかを返す
	public bool IsInvincible()
	{
		if (cntInvincibleFrame > 0)
		{
			return true;
		}
		return false;
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
		if (IsOnGround() && !isJump && !isAttack)
		{
			if (XPad.Get.GetPress(XPad.KeyData.X, PlayerIndex))
			{
				cntJumpCheckFrame++;
			}

			if (cntJumpCheckFrame > 4)
			{
				SoundManager.Get.PlaySE("jump");
				jumpSpeed = JumpPower;
				isJump = true;
			} else
			{
				if (XPad.Get.GetRelease(XPad.KeyData.X, PlayerIndex))
				{
					SoundManager.Get.PlaySE("jump");
					jumpSpeed = JumpPower - (JumpPower / 3);
					isJump = true;
				}
			}
		}

		animator.SetBool("isJump", isJump);
		if (isJump)
		{
			transform.position += Vector3.up * jumpSpeed * Time.deltaTime;
			jumpSpeed -= Gravity * JumpSpeed;
			if (jumpSpeed <= 0)
			{
				cntJumpCheckFrame = 0;
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
			RaycastHit hit;
			Physics.Raycast(transform.position, transform.forward, out hit, CanHoldItemDistance);
			if (hit.collider)
			{
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
			SoundManager.Get.PlaySE("get");
			rightSpeed = leftSpeed = 0.0f;
			isDash = false;
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
			SoundManager.Get.PlaySE("releace");
			XPad.Get.SetVibration(PlayerIndex, 0.2f, 0.2f, 0.1f);
			isHoldItem = true;
			getItemObj = player.getItemObj;
			player.ChangeItemParent(this.gameObject);
			cntGetItemBlankTime = GetItemBlankFrame;
			rightSpeed = leftSpeed = 0.0f;
		}
	}

	//アイテムを離す
	private void ReleaseItem()
	{
		if (getItemObj)
		{
			if (isHoldItem)
			{
				SoundManager.Get.PlaySE("releace");
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

	//リスポーン処理
	private void Respawn()
	{
		Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);

		if (pos.y < 0)
		{
			isRespawn = true;
		}

		if (isRespawn)
		{
			Debug.Log(gameObject.name + "がリスポーンした");
			rightSpeed = leftSpeed = 0.0f;
			transform.position = respawnPosition;
			cntAttackFrame = 0;
			cntDamageFrame = 0;
			cntGetItemBlankTime = 0;
			isAttack = false;
			isDamage = false;
			isDash = false;
			isHoldDesk = false;
			isHoldItem = false;
			isJump = false;
			isMove = false;

			isRespawn = false;
		}
	}

	//ダメージ中かどうかを返す
	public bool IsDamage()
	{
		return isDamage;
	}

	//アイテムを取得しているかどうかを返す
	public bool IsHoldItem()
	{
		return isHoldItem;
	}

    public int GetPlayerIndex() {
        return PlayerIndex;
    }
}
