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

	private float AttackSec = 0.5f;		//攻撃持続フレーム
	private float GetItemBlankSec = 0.25f;	//アイテムを持つ&捨てる時の硬直フレーム
	private float CanHoldItemDistance = 1.0f;	//机を運べるようになる範囲
	
	private int mentalGauge = 0;
	private bool isJump = false;
	[SerializeField]private bool isHoldItem = false;
	private bool isAttack = false;
	private bool isDamage = false;
	private bool isMove = false;
	private bool isRespawn = false;
	private bool isDown = false;
	private bool isRescue = false;
	private bool isOnCollisionStay = false;

	private int angleValue = 1;

	public GameObject[] EffectSweatObj = new GameObject[2];
	public GameObject ItemPosition;
	public GameObject AttackCollisionObj;
	public GameObject RotateObj;
	public GameObject FootPositionObj;

	private GameObject getItemObj;
	private GameObject holdDeskObj;
	private GameObject bazookaObj;
	private Animator animator;
	private Rigidbody rb;
	private ParticleSystem[] effectSweetSystem = new ParticleSystem[2];
	private EffectManager effectManager;
	private BazookaRifle bazookaRifle;

	private float nowMoveSpeed;
	private float rightSpeed;
	private float leftSpeed;
	private float jumpSpeed;
	private float cntGetItemBlankTime = 0;
	private float cntAttackFrame = 0;
	private float cntDamageFrame = 0;
	private float cntJumpCheckFrame = 0;
	private float cntJumpTriggerFrame = 0;
	private float cntInvincibleFrame = 0;
	private float cntAirJumpNum = 1;
	private float cntDamageImpactTime = 0;

	private Vector3 damageImpactPower;
	private Vector3 damageImpactSpeed;
	private Vector3 holdDeskDirction;	//机を持った時の移動方向
	private Vector3 oldLeftStick;
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
		bazookaObj = GameObject.Find("BazookaRifle");
		bazookaRifle = bazookaObj.GetComponent<BazookaRifle>();
	}
	
	void Update ()
	{
		animator.SetFloat("cntGetItemBlankTime", cntGetItemBlankTime);
		animator.SetBool("isDamageTrigger", IsDamageTrigger());
		animator.SetBool("isDamage", isDamage);
		animator.SetBool("isOnGround", IsOnGround());

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
				InBazookaRange();
				RescuePlayer();
				if (XPad.Get.GetTrigger(XPad.KeyData.A, PlayerIndex))
				{
					if (cntGetItemBlankTime == 0)
					{
						ReleaseItem();
					}
				}

				SerchItem();

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
					cntInvincibleFrame -= Time.deltaTime;
					if (cntInvincibleFrame <= 0)
					{
						cntInvincibleFrame = 0;
					}
				}
			} else
			{
				Damage();
			}
		}
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
		Fall();

		if (!IsDown() && !isRescue)
		{
			if (!isDamage && cntAttackFrame == 0 && cntGetItemBlankTime == 0)
			{
				Move();
			}

			Jump();
		}
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
		if (!isDamage)
		{
			rb.AddForce(Vector3.down * (Gravity * FallSpeed));
		}
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

		//すり抜け床の処理
		if (XPad.Get.GetLeftStick(PlayerIndex).y < -0.2f)
		{
			GetComponent<ThroughFloorCheck>().IsFall(0.5f);
		}

		//アイテムを持った&机を持った時移動速度を半減(仮)
		if (isHoldItem)
		{
			nowMoveSpeed = DashSpeed / 2;
		} else
		{
			nowMoveSpeed = DashSpeed * XPad.Get.GetLeftStick(PlayerIndex).x;
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
			this.transform.Translate(Vector3.forward * angleValue * Time.deltaTime * (rightSpeed + leftSpeed));
		}

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
		if (cntGetItemBlankTime == 0 && !isJump && !isAttack && !isHoldItem && !isRescue)
		{
			XPad.Get.SetVibration(PlayerIndex, 0.2f, 0.2f, 0.1f);
			BoxCollider col = AttackCollisionObj.GetComponent<BoxCollider>();
			col.enabled = true;
			cntAttackFrame = AttackSec;
			isAttack = true;
			rightSpeed = leftSpeed = 0.0f;
		}
	}

	//攻撃処理
	private void Attack()
	{
		if (isAttack)
		{
			cntAttackFrame -= Time.deltaTime;
			if (cntAttackFrame <= 0 || isHoldItem)
			{
				BoxCollider col = AttackCollisionObj.GetComponent<BoxCollider>();
				col.enabled = false;
				isAttack = false;
				cntAttackFrame = 0;
			}
		}
		if (cntAttackFrame == 1)
		{
			animator.SetBool("isAttack", true);
		} else
		{
			animator.SetBool("isAttack", false);
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
			cntJumpCheckFrame = 0;

			cntDamageImpactTime = 0.1f;
			damageImpactPower = new Vector3(2 * angleValue, 6, 0);

			//左右移動速度を0に
			rightSpeed = leftSpeed = 0.0f;
			cntGetItemBlankTime = 0;
			XPad.Get.SetVibration(PlayerIndex, 1.0f, 1.0f, 0.5f);
			cntDamageFrame = DamageSec;
			cntAttackFrame = 0;
			isDamage = true;
			isRescue = false;
			mentalGauge += Random.Range(5, 15);
			if (mentalGauge > MentalGaugeMax)
			{
				mentalGauge = MentalGaugeMax;
			}
			cntInvincibleFrame = InvincibleSec;
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
		if (cntDamageFrame > 0)
		{
			cntDamageFrame -= Time.deltaTime;
			if (cntDamageFrame <= 0)
			{
				isDamage = false;
			}
		}
	}

	private void DamageImpact()
	{
		if (isDamage)
		{
			if (damageImpactPower.x != 0)
			{
				rb.MovePosition(rb.position + Vector3.right * Time.deltaTime * damageImpactPower.x * (angleValue * -1));
				if (damageImpactPower.x > 0)
				{
					damageImpactPower.x -= 0.05f;
					if (damageImpactPower.x <= 0)
					{
						damageImpactPower.x = 0;
					}
				}
				if (damageImpactPower.x < 0)
				{
					damageImpactPower.x += 0.05f;
					if (damageImpactPower.x >= 0)
					{
						damageImpactPower.x = 0;
					}
				}
			}
			if (damageImpactPower.y != 0)
			{
				rb.MovePosition(rb.position + Vector3.up * Time.deltaTime * damageImpactPower.y);
				if (damageImpactPower.y > 0)
				{
					damageImpactPower.y -= 0.05f;
					if (damageImpactPower.y <= 0)
					{
						damageImpactPower.y = 0;
					}
				}
			}
		}
	}

	//移動しているか
	public bool IsMove()
	{
		return isMove;
	}

	void OnDrawGizmos()
	{
		float scr = 0.4f;
		RaycastHit hit;
		CapsuleCollider cc = GetComponent<CapsuleCollider>();
		Vector3 sphirePos = new Vector3(FootPositionObj.transform.position.x, FootPositionObj.transform.position.y + scr, FootPositionObj.transform.position.z);
		Physics.SphereCast(sphirePos, scr, Vector3.down, out hit);
		if (hit.collider)
		{
			Debug.DrawRay(FootPositionObj.transform.position, Vector3.down * hit.distance);
			Gizmos.DrawWireSphere(hit.point, scr);
		}
	}
	//接地しているか
	private bool IsOnGround()
	{
		if (!isJump)
		{
			float scr = 0.4f;
			RaycastHit hit;
			CapsuleCollider cc = GetComponent<CapsuleCollider>();
			Vector3 sphirePos = new Vector3(FootPositionObj.transform.position.x, FootPositionObj.transform.position.y + scr, FootPositionObj.transform.position.z);
			Physics.SphereCast(sphirePos, scr, Vector3.down, out hit);

			if (hit.distance < 0.04f)
			{
				rb.velocity = Vector3.zero;
				return true;
			}
		}
		return false;
	}

	//ジャンプ中
	private void Jump()
	{
		//空中ジャンプ
		if (!IsOnGround() && cntAirJumpNum > 0 && !isAttack && !isDamage)
		{
			if (XPad.Get.GetTrigger(XPad.KeyData.X, PlayerIndex))
			{
				effectManager.PlayPYON(PlayerIndex, FootPositionObj.transform.position);
				rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
				cntAirJumpNum--;
				SoundManager.Get.PlaySE("jump");
				jumpSpeed = AirJumpPower;
				isJump = true;
			}
		}
		//地上ジャンプの処理
		if (IsOnGround() && !isJump && !isAttack && !isDamage)
		{
			if (XPad.Get.GetPress(XPad.KeyData.X, PlayerIndex))
			{
				cntJumpCheckFrame += Time.deltaTime;
			}

			if (cntJumpCheckFrame >= 0.1f)
			{
				cntAirJumpNum = AirJumpNum;
				effectManager.PlayDUM(PlayerIndex, FootPositionObj.transform.position);
				SoundManager.Get.PlaySE("jump");
				jumpSpeed = GroundJumpPower;
				isJump = true;
			} else
			{
				if (XPad.Get.GetRelease(XPad.KeyData.X, PlayerIndex))
				{
					cntAirJumpNum = AirJumpNum;
					effectManager.PlayDUM(PlayerIndex, FootPositionObj.transform.position);
					SoundManager.Get.PlaySE("jump");
					jumpSpeed = GroundJumpPower - (GroundJumpPower / 3);
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
		Vector3 itemPositon = new Vector3(transform.position.x, ItemPosition.transform.position.y, transform.position.z);
		Debug.DrawRay(itemPositon, transform.forward * CanHoldItemDistance, Color.blue);
		if (cntGetItemBlankTime > 0)
		{
			animator.SetBool("isGetItem", true);
			cntGetItemBlankTime -= Time.deltaTime;
			if (cntGetItemBlankTime <= 0)
			{
				cntGetItemBlankTime = 0;
			}
		} else
		{
			animator.SetBool("isGetItem", false);
		}

		if (!isRescue && !isAttack && !isHoldItem && cntGetItemBlankTime == 0)
		{
			RaycastHit hit;
			Physics.Raycast(itemPositon, transform.forward, out hit, CanHoldItemDistance);
			if (hit.collider)
			{
				Debug.Log(hit.collider.gameObject.name + "取得可能");
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
			XPad.Get.SetVibration(PlayerIndex, 0.2f, 0.2f, 0.1f);
			getItemObj = _itemObj.gameObject;
			cntGetItemBlankTime = GetItemBlankSec;
			isHoldItem = true;
			_itemObj.transform.parent = transform;
			Rigidbody itemRb = _itemObj.GetComponent<Rigidbody>();
			BoxCollider col = _itemObj.GetComponent<BoxCollider>();
			col.isTrigger = true;
			itemRb.useGravity = false;
			itemRb.isKinematic = true;

			item.SetItemLocalPosition(ItemPosition.transform.localPosition);
			item.flgMoveToGetPos = true;
			item.isHold = true;
		}
	}

	//アイテムの親を変更
	public void ChangeItemParent(GameObject _newParentObj)
	{
		isHoldItem = false;
		cntGetItemBlankTime = GetItemBlankSec;
		getItemObj.transform.parent = _newParentObj.transform;
		getItemObj = null;
	}

	//アイテムを渡す
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
			cntGetItemBlankTime = GetItemBlankSec;
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
				cntGetItemBlankTime = GetItemBlankSec;
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
				item.isHold = false;
			}
		}
	}

	private void InBazookaRange()
	{
		if (Vector3.Distance(transform.position, bazookaObj.transform.position) <= bazookaRifle.EvidenceDistance && IsOnGround())
		{
			if (isHoldItem)
			{
				ReleaseItem();
			}
		}
	}

	//他プレイヤーを助ける
	private void RescuePlayer()
	{
		if (XPad.Get.GetRelease(XPad.KeyData.A, PlayerIndex))
		{
			isRescue = false;
		}
		if (!isAttack && !isHoldItem && cntGetItemBlankTime == 0)
		{
			RaycastHit hit;
			Physics.Raycast(transform.position, transform.forward, out hit, CanHoldItemDistance*2);
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
	}

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
			isRescue = false;
			if (mentalGauge < MentalGaugeMax / 2)
			{
				isDown = false;
			}
		} else
		{
			if (mentalGauge == MentalGaugeMax)
			{
				isDown = true;
			}
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
			isHoldItem = false;
			isJump = false;
			isMove = false;

			isRespawn = false;
		}
	}

	//ダメージを受けた瞬間を返す
	public bool IsDamageTrigger()
	{
		if (cntDamageFrame == DamageSec)
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
		if (IsOnGround())
		{
			effectManager.PlayTap(PlayerIndex, FootPositionObj.transform.position);
			SoundManager.Get.PlaySE("dash");
		}
	}
}
