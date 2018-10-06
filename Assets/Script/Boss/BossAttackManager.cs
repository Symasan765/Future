using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackManager : MonoBehaviour {

	// 攻撃対象を選定するためにプレイヤーを取得しておく
	Player[] m_PlayerObjs;
	public GameObject m_RangePrefab;    // ボス攻撃範囲を指定するオブジェクトのプレハブ
	
	// Use this for initialization
	void Start () {
		Debug.Log("入った");
		GameObject[] obj = GameObject.FindGameObjectsWithTag("Player");
		if (obj.Length != 4)
			Debug.Log("キャラクターの人数が４人ではありません");

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
			var boss = Instantiate(m_RangePrefab).GetComponent<BossAttackRange>();
			boss.AttackCommand(GetImportantPlayerPos(), new Vector2(5, 5), 2);
			yield return new WaitForSeconds(5);	// これで引数分の秒数の間、処理を待つ
		}
		yield return null;
	}

	/// <summary>
	/// 狙うプレイヤーの座標値を返す
	/// </summary>
	/// <returns></returns>
	Vector2 GetImportantPlayerPos()
	{
		GameObject HighDamageObj = m_PlayerObjs[0].gameObject;	// 初期化で先頭のやつ入れておく
		float maxDamege = -1;	// 仕様上最も低い0よりも低い数値にして↓のループ内で確実にif文に入るようにしている

		for (int i = 0; i < m_PlayerObjs.Length; i++)
		{
			// もっともダメージを受けているプレイヤーを見つけておく
			if(m_PlayerObjs[i].GetMentalGauge() > maxDamege)
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
}
