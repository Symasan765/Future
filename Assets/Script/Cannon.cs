using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour {

	public GameObject parentBazookaObj;

	public bool VirtivalMove = false;
	public bool HorizontalRotate = false;
	public bool VirticalRotate = false;

	[SerializeField]
	private float AnimationSpeed = 0.1f;

	[SerializeField]
	private float MoveDerayTime = 0;		//アイテムがセットされてから動き出すまでのディレイ時間

	private GameObject bossObj;

	BazookaRifle bazookaRifle = null;
	private FeverManager feverManager;
	Cannon testParentCannon;

	public bool isTestBazooka = false;
	[SerializeField]
	private bool isStart = false;
	private float cntDerayTime = 0;
	private float cntAnimationSpeed = 0;

	[SerializeField]
	private Vector3 ShotPosition;
	[SerializeField]
	private Vector3 ShotVirticalRotation;
	private Vector3 startPosition;
	private Quaternion startRotation;
	private Vector3 startEulerAngles;
	void Start ()
	{
		feverManager = GameObject.Find("FeverManager").GetComponent<FeverManager>();
		startEulerAngles = transform.localEulerAngles;
		startRotation = transform.rotation;
		startPosition = transform.localPosition;
		bossObj = GameObject.FindGameObjectWithTag("BossHitPosition");

		bazookaRifle = parentBazookaObj.GetComponent<BazookaRifle>();
	}
	
	void Update ()
	{
		if (isStart)
		{
			//動き出すまでのディレイ時間をカウント
			if (cntDerayTime < MoveDerayTime)
			{
				if (feverManager.IsFever())
				{
					cntDerayTime += Time.deltaTime * 4;
				} else
				{
					cntDerayTime += Time.deltaTime * 2;
				}
				if (cntDerayTime >= MoveDerayTime)
				{
					cntDerayTime = MoveDerayTime;
				}
			}
			//ディレイ終了して動き出す処理
			if (cntDerayTime >= MoveDerayTime)
			{
				//縦移動処理
				if (VirtivalMove)
				{
					VirticalMove(true);
				}

				//横回転処理
				if (HorizontalRotate)
				{
					HorizontalRotation(true);
				}

				//縦回転
				if (VirticalRotate)
				{
					VirticalRotation(true);
				}

			}
		} else
		{
			cntDerayTime = 0;
			//元の位置に戻す
			if (VirtivalMove)
			{
				VirticalMove(false);
			}
			if (HorizontalRotate)
			{
				VirticalRotation(false);
			}
			if (VirticalRotate)
			{
				HorizontalRotation(false);
			}	
		}

		if (bazookaRifle.isSetEvidence)
		{
			if (!isStart)
			{
				cntDerayTime = 0;
				cntAnimationSpeed = 0;
			}
			isStart = true;
		} else
		{
			if (isStart)
			{
				cntDerayTime = 0;
				cntAnimationSpeed = 0;
			}
			isStart = false;
		}
	}

	//縦回転

	void VirticalRotation(bool _flg)
	{
		if (_flg)
		{
			transform.localEulerAngles = Vector3.Lerp(startEulerAngles, ShotVirticalRotation, cntAnimationSpeed);
		} else
		{
			transform.localEulerAngles = Vector3.Lerp(ShotVirticalRotation, startEulerAngles, cntAnimationSpeed);
		}

		//X軸とZ軸を固定
		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0, 0);
		//位置を固定
		//アニメーション時間をカウント
		CountAnimationSpeed(_flg);
	}

	//横回転
	void HorizontalRotation(bool _flg)
	{
		Vector3 vec;
		vec = bossObj.transform.position - transform.position;
		Quaternion shotHorizontalRotation = Quaternion.LookRotation(vec);

		if (_flg)
		{
			transform.rotation = Quaternion.Lerp(transform.rotation, shotHorizontalRotation, cntAnimationSpeed);
		} else
		{
			transform.rotation = Quaternion.Lerp(transform.rotation, startRotation, cntAnimationSpeed);
		}

		//X軸とZ軸を固定
		transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
		//位置を固定
		transform.localPosition = new Vector3(startPosition.x, transform.localPosition.y, startPosition.z);
		
		//アニメーション時間をカウント
		CountAnimationSpeed(_flg);
	}

	//縦移動
	void VirticalMove(bool _flg)
	{
		if (_flg)
		{
			transform.localPosition = Vector3.Lerp(transform.localPosition, ShotPosition, cntAnimationSpeed);
		} else
		{
			transform.localPosition = Vector3.Lerp(transform.localPosition, startPosition, cntAnimationSpeed);
		}

		CountAnimationSpeed(_flg);

	}

	//アニメーション時間のカウント
	private void CountAnimationSpeed(bool _flg)
	{
		if (cntAnimationSpeed < 1.0f)
		{
			if (_flg)
			{
				if (feverManager.IsFever())
				{
					cntAnimationSpeed += AnimationSpeed * 4;
				} else
				{
					cntAnimationSpeed += AnimationSpeed * 2;
				}
				if (cntAnimationSpeed >= 1.0f)
				{
					cntAnimationSpeed = 1.0f;
				}
			} else
			{
				cntAnimationSpeed += AnimationSpeed * 0.5f;

				if (cntAnimationSpeed >= 1.0f)
				{
					cntAnimationSpeed = 1.0f;
				}
			}
		}
	}

}
