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
	private float DashSpeed = 10.0f;		//ダッシュ速度
	[SerializeField]
	private float GroundJumpPower = 20.0f;	//ジャンプ力
	[SerializeField]
	private float AirJumpPower = 15.0f;
	[SerializeField]
	private float JumpSpeed = 2.0f;
	[SerializeField]
	private float FallSpeed = 1.0f;
	[SerializeField]
	private float FallSpeedMax = 5.0f;
	[SerializeField]
	private int AirJumpNum = 1;
	[SerializeField]
	private float Gravity = 0.4f;		//キャラ固有重力
	[SerializeField]
	private float BrakePower = 0.4f;	//ブレーキの強さ
	[SerializeField]
	private int MentalGaugeMax = 100;		//メンタルゲージ最大値
	[SerializeField]
	private int MentalGaugeAccumulateSpeed = 5;
	[SerializeField]
	private float InvincibleSec = 90;
	[SerializeField]
	private float DamageSec = 40;
	[SerializeField]
	private LayerMask OnGroundLayer;

	private float GetItemBlankSec = 0.25f;	//アイテムを持つ&捨てる時の硬直フレーム
	private float CanHoldItemDistance = 2;	//机を運べるようになる範囲
	
	private int mentalGauge = 0;
	private bool isJump = false;
	[SerializeField]private bool isHoldItem = false;
	private bool isDamage = false;
	private bool isMove = false;
	private bool isRespawn = false;
	private bool isDown = false;
	private bool isOnCollisionStay = false;
	private bool isAirjumpRotation = false;
	private bool isReleaceItem = false;
	private bool isStandUp;
	private bool checkJumpTrigger = false;
	private bool checkForwardEvidence = false;
	
	private int angleValue = 1;

	public GameObject[] EffectSweatObj = new GameObject[2];
	public GameObject ItemPosition;
	public GameObject RotateObj;
	public GameObject[] FootPositionObj = new GameObject[2];	//着地判定用にキャラの左右に配置
	public GameObject ItemHoldCollisionObj;
	public GameObject MentalGaugeObj;

	private GameObject getItemObj;
	private GameObject holdDeskObj;
	private Animator animator;
	private Rigidbody rb;
	private ParticleSystem[] effectSweetSystem = new ParticleSystem[2];
	private EffectManager effectManager;
	private FeverManager feverManager;
	private BoxCollider itemHoldCollision;

	private float nowMoveSpeed;
	private float rightSpeed;
	private float leftSpeed;
	private float jumpSpeed;
	private float cntGetItemBlankSec = 0;
	private float cntDamageSec = 0;
	public float cntJumpCheckSec = 0;
	private float cntJumpTriggerSec = 0;
	private float cntInvincibleSec = 0;
	private float cntAirJumpNum = 1;
	private float cntCantMoveSec = 0;
	private Vector3 startFootRPosition;

	private Vector3 respawnPosition;
	private float rotationValue = 0;
	private float dashBrakeSpeed = 1.0f;

	void Start ()
	{
		for (int i = 0; i < 2; i++)
		{
			effectSweetSystem[i] = EffectSweatObj[i].GetComponent<ParticleSystem>();
		}
		//とりあえずリスポン位置をゲーム開始位置に
		respawnPosition = transform.position;
		effectManager = GameObject.Find("EffectManager").GetComponent<EffectManager>();
		rb = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();
		feverManager = GameObject.Find("FeverManager").GetComponent<FeverManager>();
		itemHoldCollision = ItemHoldCollisionObj.GetComponent<BoxCollider>();
		startFootRPosition = FootPositionObj[0].transform.localPosition;
	}
	
	void Update ()
	{
		animator.SetFloat("cntGetItemBlankTime", cntGetItemBlankSec);
		animator.SetBool("isDamage", isDamage);
		animator.SetBool("isOnGround", IsOnGround());
		//操作不能時間のカウント
		if (cntCantMoveSec > 0)
		{
			cntCantMoveSec -= Time.deltaTime;
			if (cntCantMoveSec <= 0)
			{
				isStandUp = false;
				animator.SetBool("isStandUp", false);
				cntCantMoveSec = 0;
			}
		}

		if (IsOnGround())
		{
			isOnCollisionStay = false;
		}

		if (IsDown())
		{
			if (isHoldItem)
			{
				ReleaseItem();
			}
		} else
		{
			if (!isDamage)
			{
				//無敵フレームのカウント
				if (cntInvincibleSec > 0)
				{
					cntInvincibleSec -= Time.deltaTime;
					if (cntInvincibleSec <= 0)
					{
						cntInvincibleSec = 0;
					}
				}
			} else
			{
				Damage();
			}
		}
		EvidenceHoldCollision();
		Down();
		Respawn();
		//キャラのZ座標は常に0
		transform.position = new Vector3(transform.position.x, transform.position.y, 0);
		Rotate();
		PlayEffect();
	}

    void FixedUpdate()
	{
		//DamageImpact();

		if (!IsDown())
		{
			if (!isDamage && CanIMove())
			{
				if (XPad.Get.HoldTrigger(PlayerIndex))
				{
					if (isHoldItem)
					{
						isReleaceItem = true;
					}
				}

				if (isReleaceItem)
				{
					ReleaseItem();
				}

				SerchItem();
			}
			if (!isDamage && cntGetItemBlankSec == 0 && CanIMove())
			{
				Move();
				Fall();
			}

			Jump();
		}
		rb.position = new Vector3(rb.position.x, rb.position.y, 0);
    }

	//落下処理
	private void Fall()
	{
		if (rb.velocity.y < FallSpeedMax * -1)
		{
			rb.velocity = new Vector3(rb.velocity.x, FallSpeedMax * -1, rb.velocity.z);
		}
		if (!isDamage)
		{
			if (!isJump && !IsOnGround() && XPad.Get.GetLeftStick(PlayerIndex).y < -0.8f)
			{
				rb.velocity = new Vector3(rb.velocity.x, (FallSpeedMax * 2) * -1, rb.velocity.z);
			}
			rb.AddForce(Vector3.down * (Gravity * FallSpeed));
		}
	}

	//キャラの向きを変える
	private void Rotate()
	{
		if (angleValue == 1)
		{
			transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, new Vector3(0, 90, 0), 0.4f);
			RotateObj.transform.localEulerAngles = new Vector3(RotateObj.transform.localEulerAngles.x, ForwardAngle + 360, RotateObj.transform.localEulerAngles.z);
		}
		if(angleValue == -1)
		{
			transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, new Vector3(0, 270, 0), 0.4f);
			RotateObj.transform.localEulerAngles = new Vector3(RotateObj.transform.localEulerAngles.x, 360 - ForwardAngle, RotateObj.transform.localEulerAngles.z);
		}

	}

	//移動
	private void Move()
	{
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
			animator.SetBool("isDash", true);
		} else
		{
			isMove = false;
			animator.SetBool("isDash", false);
		}

		//アイテムを持った時移動速度を半減(仮)
		if (isHoldItem)
		{
			nowMoveSpeed = DashSpeed / 2 * XPad.Get.GetLeftStick(PlayerIndex).x;
		} else
		{
			if (checkForwardEvidence)
			{
				if (IsOnGround())
				{
					nowMoveSpeed = DashSpeed / 2 * XPad.Get.GetLeftStick(PlayerIndex).x;
				} else
				{
					nowMoveSpeed = DashSpeed * XPad.Get.GetLeftStick(PlayerIndex).x;
				}
			} else
			{
				nowMoveSpeed = DashSpeed * XPad.Get.GetLeftStick(PlayerIndex).x;
			}
		}

		if (feverManager.IsFever())
		{
			nowMoveSpeed *= 1.4f;
		}

		if (nowMoveSpeed < 0)
		{
			nowMoveSpeed *= -1;
		}
		if (LeftStick.x != 0)
		{
			SetMoveSpeed(angleValue, nowMoveSpeed);
		}

		if (isOnCollisionStay)
		{
	
		} else
		{
			//transform.position += Vector3.forward * angleValue * Time.deltaTime * (rightSpeed + leftSpeed);
			this.transform.Translate(Vector3.forward * angleValue * Time.deltaTime * (rightSpeed + leftSpeed));
			//rb.MovePosition(rb.position + Vector3.forward * angleValue * Time.deltaTime * (rightSpeed + leftSpeed));
		}
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

	//ボスの攻撃が自分にヒットした
	public void HitBossAttack()
	{
		if (!IsInvincible())
		{
			ShakeCamera.Impact(0.005f, 0.3f);
			animator.SetBool("isDamageTrigger", true);
			SoundManager.Get.PlaySE("hit1");
			PlayerDamage pd = GetComponent<PlayerDamage>();
			pd.StartEffect();
			//アイテムを落とす
			if (isHoldItem)
			{
				ReleaseItem();
			}
			//ジャンプしている時は停止する
			if (isJump)
			{
				isJump = false;
				jumpSpeed = 0;
			}
			cntJumpCheckSec = 0;
			isAirjumpRotation = false;

			EndAirJumpRotationTrigger();

			//左右移動速度を0に
			rightSpeed = leftSpeed = 0.0f;
			cntGetItemBlankSec = 0;
			XPad.Get.SetVibration(PlayerIndex, 1.0f, 1.0f, 0.5f);
			cntDamageSec = DamageSec;
			cntCantMoveSec = DamageSec;
			isDamage = true;
			mentalGauge += Random.Range(8, 15);
			if (mentalGauge > MentalGaugeMax)
			{
				mentalGauge = MentalGaugeMax;
			}
			cntInvincibleSec = InvincibleSec;
		}
	}

	//無敵時間かどうかを返す
	public bool IsInvincible()
	{
		if (cntInvincibleSec > 0)
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

	//メンタルゲージを回復
	public void RecoveryMentalGauge(int _recoverySpeed)
	{
		if (mentalGauge > 0)
		{
			if (Time.frameCount % _recoverySpeed == 0)
			{
				mentalGauge--;
			}
		}
	}
	//ダメージ処理
	private void Damage()
	{
		isMove = false;
		rb.velocity = Vector3.zero;
		if (cntDamageSec > 0)
		{
			cntDamageSec -= Time.deltaTime;
			if (cntDamageSec <= 0)
			{
				isDamage = false;
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
		bool flgOnGround = false;
		if (!isJump)
		{
			RaycastHit hit;
			//着地判定用のオブジェクトが2つあるので2回ループさせる
			for (int i = 0; i < 2; i++)
			{
				Vector3 checkPos = new Vector3(FootPositionObj[i].transform.position.x, FootPositionObj[i].transform.position.y, FootPositionObj[i].transform.position.z);
				Physics.Raycast(checkPos, Vector3.down, out hit, 10, OnGroundLayer);
				if (hit.collider)
				{
					if (hit.distance < 0.04f)
					{
						flgOnGround = true;
						break;
					}
				}
			}
		}
		return flgOnGround;
	}

	//ジャンプ中
	private void Jump()
	{
		Vector3 effectPos = new Vector3(transform.position.x, FootPositionObj[0].transform.position.y, FootPositionObj[0].transform.position.z);
		animator.SetBool("isAirJumpRotation", isAirjumpRotation);

		//空中ジャンプ
		if (!IsOnGround() && cntAirJumpNum > 0 && !isDamage && CanIMove())
		{
			if (XPad.Get.JumpTrigger(PlayerIndex))
			{
				Debug.Log("空中J");
				effectManager.PlayPYON(PlayerIndex, effectPos);
				rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
				cntAirJumpNum--;
				SoundManager.Get.PlaySE("jump");
				if (isHoldItem)
				{
					jumpSpeed = AirJumpPower * 0.9f;
				} else
				{
					jumpSpeed = AirJumpPower;
				}
				isJump = true;
				isAirjumpRotation = true;
				animator.SetBool("isAirJumpRotationTrigger", true);
			}
		}

		//地上ジャンプの処理
		if (IsOnGround() && !isJump && !isDamage && CanIMove() && !checkJumpTrigger)
		{
			
			if (XPad.Get.GetPress(XPad.KeyData.X, PlayerIndex))
			{
				cntJumpCheckSec += Time.deltaTime;
			}

			float nowJumpPower = GroundJumpPower;
			if (isHoldItem)
			{
				nowJumpPower = GroundJumpPower * 0.9f;
			}

			if (XPad.Get.JumpTrigger(PlayerIndex))
			{
				cntJumpCheckSec = 0;
				checkJumpTrigger = false;
				cntAirJumpNum = AirJumpNum;
				effectManager.PlayDUM(PlayerIndex, effectPos);
				SoundManager.Get.PlaySE("AirJump");
				jumpSpeed = nowJumpPower;
				isJump = true;
			}

			/*
			if (cntJumpCheckSec >= 0.07f)
			{
				cntJumpCheckSec = 0;
				checkJumpTrigger = true;
				cntAirJumpNum = AirJumpNum;
				effectManager.PlayDUM(PlayerIndex, effectPos);
				SoundManager.Get.PlaySE("AirJump");
				jumpSpeed = nowJumpPower;
				isJump = true;
			} else
			{
				if (XPad.Get.GetRelease(XPad.KeyData.X, PlayerIndex))
				{
					cntJumpCheckSec = 0;
					checkJumpTrigger = false;
					cntAirJumpNum = AirJumpNum;
					effectManager.PlayDUM(PlayerIndex, effectPos);
					SoundManager.Get.PlaySE("AirJump");
					jumpSpeed = nowJumpPower - (nowJumpPower / 3);
					isJump = true;
				}

			}*/

		}

		if (IsOnGround() && !XPad.Get.GetPress(XPad.KeyData.X, PlayerIndex) && !XPad.Get.GetTrigger(XPad.KeyData.X, PlayerIndex))
		{
			checkJumpTrigger = false;
		}

		if (checkJumpTrigger)
		{
			cntJumpCheckSec = 0;
		}
		animator.SetBool("isJump", isJump);
		if (isJump)
		{
			this.transform.Translate(Vector3.up * jumpSpeed * Time.deltaTime);
			jumpSpeed -= Gravity * JumpSpeed;
			if (jumpSpeed <= 0)
			{
				cntJumpCheckSec = 0;
				isJump = false;
			}
		}

		if (IsOnGround())
		{
			cntAirJumpNum = AirJumpNum;
		}

	}

	//持てるアイテムを探す
	private void SerchItem()
	{
		Vector3 itemPositon = new Vector3(transform.position.x - (1.0f * angleValue), ItemPosition.transform.position.y, transform.position.z);
		if (cntGetItemBlankSec > 0)
		{
			animator.SetBool("isGetItem", true);
			cntGetItemBlankSec -= Time.deltaTime;
			if (cntGetItemBlankSec <= 0)
			{
				cntGetItemBlankSec = 0;
			}
		} else
		{
			animator.SetBool("isGetItem", false);
		}

		if (!isHoldItem && cntGetItemBlankSec == 0)
		{
			RaycastHit hit;
			Physics.Raycast(itemPositon, transform.forward, out hit, CanHoldItemDistance, LayerMask.GetMask("Evidence"));
			Debug.DrawRay(itemPositon, transform.forward * CanHoldItemDistance, Color.blue);
			if (hit.collider)
			{
				checkForwardEvidence = true;
				if (XPad.Get.HoldTrigger(PlayerIndex))
				{
					GetItem(hit.collider.gameObject);		
				}
			} else
			{
				checkForwardEvidence = false;
			}
		}
	}

	private void EvidenceHoldCollision()
	{
		if (isHoldItem)
		{
			FootPositionObj[0].transform.localPosition = new Vector3(startFootRPosition.x, startFootRPosition.y, startFootRPosition.z * 2);
			itemHoldCollision.enabled = true;
		} else
		{
			FootPositionObj[0].transform.localPosition = startFootRPosition;
			itemHoldCollision.enabled = false;
		}
	}

	//アイテムを持つ
	private void GetItem(GameObject _itemObj)
	{
		Item item = _itemObj.GetComponent<Item>();
		if (item)
		{
			if (item.IsHold())
			{
				//ReceiveItem(_itemObj.transform.parent.gameObject);

			} else
			{

				SoundManager.Get.PlaySE("get");
				rightSpeed = leftSpeed = 0.0f;
				XPad.Get.SetVibration(PlayerIndex, 0.2f, 0.2f, 0.1f);
				getItemObj = _itemObj.gameObject;
				cntGetItemBlankSec = GetItemBlankSec;
				isHoldItem = true;
				_itemObj.transform.parent = transform;
				_itemObj.layer = LayerMask.NameToLayer("Player");
				Rigidbody itemRb = _itemObj.GetComponent<Rigidbody>();
				//BoxCollider col = _itemObj.GetComponent<BoxCollider>();
				//col.isTrigger = true;
				itemRb.useGravity = false;
				itemRb.velocity = Vector3.zero;
				//itemRb.isKinematic = true;
				item.holdPlayerIndex = PlayerIndex;
				item.SetItemLocalPosition(ItemPosition.transform.localPosition);
				item.flgMoveToGetPos = true;
				item.isHold = true;
			}
		}
	}

	//アイテムの親を変更
	public void ChangeItemParent(GameObject _newParentObj)
	{
		isHoldItem = false;
		cntGetItemBlankSec = GetItemBlankSec;
		getItemObj.transform.parent = _newParentObj.transform;
		getItemObj = null;
	}

	//アイテムを受け取る
	private void ReceiveItem(GameObject passPlayerObj)
	{
		Player player = passPlayerObj.GetComponent<Player>();
		if (player.isHoldItem && !player.IsDown())
		{
			SoundManager.Get.PlaySE("releace");
			XPad.Get.SetVibration(PlayerIndex, 0.2f, 0.2f, 0.1f);
			isHoldItem = true;
			getItemObj = player.getItemObj;
			player.ChangeItemParent(this.gameObject);
			cntGetItemBlankSec = GetItemBlankSec;
			rightSpeed = leftSpeed = 0.0f;
			if (player.transform.position.x > transform.position.x)
			{
				player.SetAngleValue(-1);
			} else
			{
				player.SetAngleValue(1);
			}

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
				cntGetItemBlankSec = GetItemBlankSec;
				isHoldItem = false;
				Rigidbody itemRb = getItemObj.GetComponent<Rigidbody>();
				Item item = getItemObj.GetComponent<Item>();
				getItemObj.layer = LayerMask.NameToLayer("Evidence");
				//BoxCollider col = getItemObj.GetComponent<BoxCollider>();
				//col.isTrigger = false;
				itemRb.useGravity = true;
				//itemRb.isKinematic = false;
				itemRb.AddForce(transform.forward * 1, ForceMode.Impulse);
				item.flgMoveToGetPos = false;
				getItemObj = null;
				item.isHold = false;
				isReleaceItem = false;
			}
		}
	}

	//他プレイヤーを助ける
	/*
	private void RescuePlayer()
	{
		animator.SetBool("isRescue", isRescue);
		Vector3 itemPositon = new Vector3(transform.position.x, ItemPosition.transform.position.y, transform.position.z);
		if (XPad.Get.GetRelease(XPad.KeyData.A, PlayerIndex))
		{
			isRescue = false;
		}
		if (!isHoldItem && cntGetItemBlankTime == 0)
		{
			RaycastHit hit;
			Physics.Raycast(itemPositon, transform.forward, out hit, CanHoldItemDistance);
			if (hit.collider)
			{
				if (hit.collider.tag == "Player")
				{
					Player player = hit.collider.gameObject.GetComponent<Player>();
					if (player.IsDown())
					{
						if (XPad.Get.GetPress(XPad.KeyData.A, PlayerIndex))
						{
							isRescue = true;
							player.RecoveryMentalGauge(2);
						}
					}
				}
			}
		}
	}*/

	//汗のエフェクト
	private void PlayEffect()
	{
		if (mentalGauge > MentalGaugeMax / 2)
		{
			if (angleValue == 1)
			{
				if (!effectSweetSystem[1].isPlaying)
				{
					effectSweetSystem[0].Stop();
					effectSweetSystem[1].Play();
				}
			} else
			{
				if (!effectSweetSystem[0].isPlaying)
				{
					effectSweetSystem[0].Play();
					effectSweetSystem[1].Stop();
				}
			}
			
		} else
		{
			effectSweetSystem[0].Stop();
			effectSweetSystem[1].Stop();
		}

	}

	//操作不能中処理
	private void Down()
	{
		if (IsDown())
		{
			 
		} else
		{
			if (mentalGauge >= MentalGaugeMax)
			{
				if (!isDown)
				{
					animator.SetBool("isDownTrigger", true);
				}
				isDown = true;
			}
		}
	}

	//リスポーン処理
	private void Respawn()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			HitBossAttack();
		}

		//メンタルゲージがMAXになった時リスポン
		if (mentalGauge >= MentalGaugeMax)
		{
			isRespawn = true;
		}

		if (isRespawn)
		{
			//Debug.Log(gameObject.name + "がリスポーンした");
			if (cntInvincibleSec <= 0)
			{
				effectManager.PlayDown(new Vector3(transform.position.x, transform.position.y, -2));
				if (isHoldItem)
				{
					ReleaseItem();
				}
				mentalGauge = 0;
				rightSpeed = leftSpeed = 0.0f;
				transform.position = respawnPosition;
				effectManager.PlayRespawnHeart(new Vector3(transform.position.x, transform.position.y, -2));
				cntDamageSec = 0;
				cntGetItemBlankSec = 0;
				cntJumpCheckSec = 0;
				cntAirJumpNum = 0;
				jumpSpeed = 0;
				isDamage = false;
				isHoldItem = false;
				isJump = false;
				isMove = false;
				isReleaceItem = false;
				isDown = false;
				isAirjumpRotation = false;

				isRespawn = false;
				cntInvincibleSec = 3;
				cntCantMoveSec = 3;
				isStandUp = true;
				animator.SetBool("isStandUp", true);
			}
		}
	}

	//今移動可能かどうかを返す
	public bool CanIMove()
	{
		if (cntCantMoveSec > 0)
		{
			return false;
		}
		return true;
	}

	//ダメージを受けた瞬間を返す
	public bool IsDamageTrigger()
	{
		if (cntDamageSec == DamageSec)
		{
			return true;
		}
		return false;
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

	//操作不能状態かどうか
	public bool IsDown()
	{
		return isDown;
	}

	public void SetAngleValue(int _angle)
	{
		angleValue = _angle;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (isHoldItem)
		{
			if (other.gameObject.tag == "Bazooka")
			{
				BazookaRifle br = other.gameObject.GetComponent<BazookaRifle>();
				if (!br.isSetEvidence)
				{
					br.isSetEvidence = true;
					GameObject io = getItemObj;
					Item i = io.GetComponent<Item>();
					ReleaseItem();
					i.SetBazooka(other.gameObject);
				}
			}
		}
	}

	public void OnCollisionStay(Collision other)
	{
		if (!IsOnGround() && !isJump)
		{
			if (other.gameObject.tag != "Player")
			{
				isOnCollisionStay = true;	
			}
		}
	}

	public void OnCollisionExit(Collision other)
	{
		isOnCollisionStay = false;
	}

    public int GetPlayerIndex() {
        return PlayerIndex;
    }

	private void Step()
	{
		Vector3 effectPos = new Vector3(transform.position.x, FootPositionObj[0].transform.position.y, FootPositionObj[0].transform.position.z);
		if (IsOnGround())
		{
			effectManager.PlayTap(PlayerIndex, effectPos);
			SoundManager.Get.PlaySE("dash");
		}
	}

	private void EndIsDamageTrigger()
	{
		animator.SetBool("isDamageTrigger", false);
	}

	private void EndAirJumpRotation()
	{
		isAirjumpRotation = false;
	}
	private void EndAirJumpRotationTrigger()
	{
		animator.SetBool("isAirJumpRotationTrigger", false);
	}
	private void EndIsDownTrigger()
	{
		animator.SetBool("isDownTrigger", false);
	}
	private void EndIsInvincibleInDown()
	{
		cntInvincibleSec = 0;
	}
}
