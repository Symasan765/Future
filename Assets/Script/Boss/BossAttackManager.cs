using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class BossAttackManager : MonoBehaviour
{

	// 攻撃対象を選定するためにプレイヤーを取得しておく
	Player[] m_PlayerObjs;
	public GameObject m_RangePrefab;    // ボス攻撃範囲を指定するオブジェクトのプレハブ

	GameObject[] m_AttackObjects;

	int m_MaxAttackID = -1;

	// 攻撃範囲用に配置したオブジェクトを取得する
	AttackID[] m_AttackObjs;

	List<List<AttackID>> m_AttackList;

	// Use this for initialization
	void Start()
	{
		GameObject[] obj = GameObject.FindGameObjectsWithTag("Player");
		if (obj.Length != 4)
			Debug.Log("キャラクターの人数が４人ではありません");

		SearchAttackObj();

		m_PlayerObjs = new Player[obj.Length];
		for (int i = 0; i < obj.Length; i++)
			m_PlayerObjs[i] = obj[i].GetComponent<Player>();

		StartCoroutine("BossAttack");
	}

	/// <summary>
	/// ボスの攻撃をコルーチンで調整していく
	/// </summary>
	/// <returns></returns>
	IEnumerator BossAttack()
	{
		// TODO 将来的にはボスが生きている間、みたいな条件に変更すること
		while (true)
		{
			AttackID(0);
			yield return new WaitForSeconds(5); // これで引数分の秒数の間、処理を待つ
		}
		yield return null;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="ID">攻撃ID。攻撃範囲オブジェクトに着けているID</param>
	void AttackID(int ID)
	{
		if (ID > m_MaxAttackID)
		{
			Debug.Log("err:スクリプト上にてボス攻撃IDにシーン配置されている攻撃ID以上の値が入力されました");
			return;
		}

		for (int i = 0; i < m_AttackList[ID].Count; i++)
		{
			var obj = m_AttackList[ID][i];
			var boss = Instantiate(m_RangePrefab).GetComponent<BossAttackRange>();
			boss.AttackCommand(obj.transform.position, obj.transform.localScale, obj.m_TimeSec);
		}

		SoundManager.Get.PlaySE("BossAttackDangerous");
	}

	/// <summary>
	/// 狙うプレイヤーの座標値を返す
	/// </summary>
	/// <returns></returns>
	Vector2 GetImportantPlayerPos()
	{
		GameObject HighDamageObj = m_PlayerObjs[0].gameObject;  // 初期化で先頭のやつ入れておく
		float maxDamege = -1;   // 仕様上最も低い0よりも低い数値にして↓のループ内で確実にif文に入るようにしている

		for (int i = 0; i < m_PlayerObjs.Length; i++)
		{
			// もっともダメージを受けているプレイヤーを見つけておく
			if (m_PlayerObjs[i].GetMentalGauge() > maxDamege)
			{
				maxDamege = m_PlayerObjs[i].GetMentalGauge();
				HighDamageObj = m_PlayerObjs[i].gameObject;
			}

			// アイテム持ってたら強制的にそいつを狙う
			if (m_PlayerObjs[i].IsHoldItem())
			{
				var IsHoldPos = m_PlayerObjs[i].gameObject.transform.position;
				return new Vector2(IsHoldPos.x, IsHoldPos.y);
			}
		}

		// 誰も証拠を持っていなかった場合はダメージのおおおいプレイヤーの座標を返す
		var pos = HighDamageObj.transform.position;
		return new Vector2(pos.x, pos.y);
	}

	void SearchAttackObj()
	{
		GameObject[] attackObjects;
		// 攻撃用オブジェクトの探索を行う
		attackObjects = GameObject.FindGameObjectsWithTag("BossAttackObj");

		// 攻撃パターン数を取得する
		m_AttackObjs = new AttackID[attackObjects.Length];
		Debug.Log("数" + m_AttackObjs.Length);
		for (int i = 0; i < m_AttackObjs.Length; i++)
		{
			m_AttackObjs[i] = attackObjects[i].GetComponent<AttackID>();
			if (m_MaxAttackID < m_AttackObjs[i].m_AttackID)
			{
				m_MaxAttackID = m_AttackObjs[i].m_AttackID;
			}
		}

		m_AttackList = new List<List<AttackID>>();
		for (int nowID = 0; nowID < m_MaxAttackID + 1; nowID++)
		{
			var list = new List<AttackID>();
			for (int i = 0; i < m_AttackObjs.Length; i++)
			{
				int ID = m_AttackObjs[i].m_AttackID;
				if (nowID == ID)
				{
					list.Add(m_AttackObjs[i]);
				}
			}
			m_AttackList.Add(list);
		}
	}

	int AreaIdentification(int tagetNo)
	{
		return 0;
	}
}
