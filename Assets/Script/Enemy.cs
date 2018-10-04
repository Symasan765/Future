using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	private Rigidbody rb;

	[SerializeField]
	private float DamagePower = 10;

	void Start ()
	{
		rb = GetComponent<Rigidbody>();	
	}
	
	void Update () {
		
	}

	void OnTriggerEnter(Collider other)
	{
		//プレイヤーの攻撃判定に当たったら吹っ飛ぶ
		if (other.name == "AttackCollision")
		{
			Player player = other.transform.parent.GetComponent<Player>();
			player.HitAttackCollision();
			rb.AddForce(other.transform.forward * DamagePower, ForceMode.Impulse);
		}
	}

}
