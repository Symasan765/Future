﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using System;

public class BossAttackManager : MonoBehaviour
{

	// 攻撃対象を選定するためにプレイヤーを取得しておく
	Player[] m_PlayerObjs;
	public GameObject m_RangePrefab;    // ボス攻撃範囲を指定するオブジェクトのプレハブ

	GameObject[] m_AttackObjects;

	public GameObject m_CutinPrefab;

	int m_MaxAttackID = -1;

	// 攻撃範囲用に配置したオブジェクトを取得する
	AttackID[] m_AttackObjs;

	List<List<AttackID>> m_AttackList;

	int m_OldAreaNo;
	bool m_XORFlag;
	float[] m_AreaTime;

	public float m_BossMaxDamage = 100.0f;
	public float m_BossDamage;

	public bool m_AttackFlag;   // ボスが攻撃してもいいかどうかのフラグ

	public float m_SecondsBeforeAttack = 0.5f;

	BossAttackManager m_This;

	BossLight m_Light;

	EvidenceLoading m_EviLoading;

	public bool m_DownSwing = false;
	public bool m_SideSwing = false;
	public bool m_Beam = false;
	public bool m_Emotion = false;
	bool m_IsEmotion = false;

	int m_StageChangeNum = 0;

	float m_NextDelaySec = 0.0f;

	int m_ContinuousAttackID = -1;  // こいつが-1以外のときはその攻撃を連続発生させる

	BossUIScript m_BossHPUI;
	Coroutine m_NowCoroutine = null;

	public enum BossCondition
	{
		Margin,     // ボス余裕
		Spicy,      //ボス辛い
		Dying       // しにかけ
	}

	BossCondition m_Condition;
	AttackID.AttackType m_NextAttackType;
	public bool m_StageSwitch = false;

	// Use this for initialization
	void Start()
	{
		m_This = this;
		m_AttackFlag = true;
		m_BossDamage = m_BossMaxDamage;
		m_Condition = BossCondition.Margin;

		m_Light = GetComponent<BossLight>();
		m_Light.LightChage(false);

		m_EviLoading = GameObject.Find("EvidenceLoading").GetComponent<EvidenceLoading>();

		m_BossHPUI = GameObject.Find("BossUIParent").GetComponent<BossUIScript>();
	}

	private void Update()
	{
		// ボスのコンディションが変わったらステージスイッチフラグをONにする
		var nowCond = m_Condition;
		ConditionUpdate();
		if (nowCond != m_Condition)
			m_StageSwitch = true;

		if (Input.GetKeyDown(KeyCode.P))
		{
			m_BossDamage = 0.0f;
		}
	}

	/// <summary>
	/// ボスの攻撃をコルーチンで調整していく
	/// </summary>
	/// <returns></returns>
	IEnumerator BossAttack()
	{
		int nowChangeNum = m_StageChangeNum;
		m_AttackFlag = false;

		// TODO 将来的にはボスが生きている間、みたいな条件に変更すること
		yield return new WaitForSeconds(2.9f);  // 開始後、すぐには攻撃しない
		m_AttackFlag = true;
		yield return new WaitForSeconds(0.1f);	// スタート位置オブジェがある場合、↑でtrueにしたものが反映してしまう場合があるので一度待つ

		GameObject[] obj = GameObject.FindGameObjectsWithTag("Player");
		if (obj.Length != 4)
			Debug.Log("キャラクターの人数が４人ではありません");

		m_PlayerObjs = new Player[obj.Length];
		for (int i = 0; i < obj.Length; i++)
			m_PlayerObjs[i] = obj[i].GetComponent<Player>();

		while (true)
		{
			// 攻撃モーションを初期化
			AnmeFlagInit();
			if (m_AttackFlag)
			{
				m_Emotion = false;
				// コルーチン対象が変わっていたら破棄
				if (nowChangeNum != m_StageChangeNum)
					yield break;

				int AreaNo = m_ContinuousAttackID;

				// 連続攻撃しない場合はプレイヤーの位置から探知させる
				if (m_ContinuousAttackID == -1)
				{
					// 狙うプレイヤーを確定し、攻撃エリアを特定させる
					AreaNo = TargetAreaNo();
				}

				// プレイヤーが特定のエリアにいればそのエリアに攻撃を発生させる
				if (AreaNo != -1)
				{
					// コルーチン対象が変わっていたら破棄
					if (nowChangeNum != m_StageChangeNum)
						yield break;

					float waitSec = AttackID(AreaNo);
					// 攻撃モーション開始まで待機してから…
					yield return new WaitForSeconds(waitSec - m_SecondsBeforeAttack); // これで引数分の秒数の間、処理を待つ

					// コルーチン対象が変わっていたら破棄
					if (nowChangeNum != m_StageChangeNum)
						yield break;
					//攻撃モーションを起動させて残り秒数待つ
					m_Light.LightChage(true);
					ChangeAnimFlag();

					yield return new WaitForSeconds(m_SecondsBeforeAttack);
					m_Light.LightChage(false);

					// 攻撃が当たっていたらボスの喜ぶモーションを行う
					while (m_Emotion)
						yield return null;

					yield return new WaitForSeconds(m_NextDelaySec);
				}
				// プレイヤーがいるエリアを特定出来なかった場合…
				else
				{
					// コルーチン対象が変わっていたら破棄
					if (nowChangeNum != m_StageChangeNum)
						yield break;
					yield return new WaitForSeconds(0.5f);
				}
			}
			// 攻撃可能フラグが立っていない
			else
			{
				// コルーチン対象が変わっていたら破棄
				if (nowChangeNum != m_StageChangeNum)
					yield break;
				yield return new WaitForSeconds(2.0f);
			}
				yield return null;
		}
		yield return null;
	}

	int TargetAreaNo()
	{
		// 攻撃対象のプレイヤーを確定させる
		int targetNo = GetImportantPlayerNo();
		// そのプレイヤーがステージのどこのエリアにいるかを特定する
		int AreaNo = AreaIdentification(targetNo);

		return AreaNo;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="ID">攻撃ID。攻撃範囲オブジェクトに着けているID</param>
	float AttackID(int ID)
	{
		float ret = 0.5f;
		Debug.Log("攻撃ID" + ID + "起動！");
		if (ID > m_MaxAttackID)
		{
			Debug.Log("err:スクリプト上にてボス攻撃IDにシーン配置されている攻撃ID以上の値が入力されました");
			return ret;
		}

		bool specialAttackFlag = false;

		for (int i = 0; i < m_AttackList[ID].Count; i++)
		{
			var obj = m_AttackList[ID][i];
			ret = obj.m_TimeSec;
			m_IsEmotion = obj.m_EmotionFlag;
			m_ContinuousAttackID = obj.m_NextAttackID;
			m_NextAttackType = obj.m_AttackType;
			m_NextDelaySec = obj.m_NextAttackDelaySec;
			if (m_NextAttackType == global::AttackID.AttackType.Special) specialAttackFlag = true;
			var boss = Instantiate(m_RangePrefab).GetComponent<BossAttackRange>();
			boss.AttackCommand(this, obj.transform.position, obj.transform.localScale, obj.m_TimeSec,obj);
		}

		// Special攻撃が来た場合、ボスのカットインを表示させる
		if (specialAttackFlag)
		{
			CutinScript.m_CharaNo = 4;
			CutinScript.m_PlayerNo = 4;
			Instantiate(m_CutinPrefab);
		}

		SoundManager.Get.PlaySE("BossAttackDangerous");
		return ret;
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

	int GetImportantPlayerNo()
	{
		int retNo = -1;
		int[] hp = new int[5];      // ソートに使用する体力値
		int[] priority = new int[5];

		hp[4] = 0;
		priority[4] = 0;

		for (int i = 0; i < m_PlayerObjs.Length; i++)
		{
			hp[i] = m_PlayerObjs[i].GetMentalGauge();
			priority[i] = i;

			// アイテム持ってたら強制的にそいつを狙う
			if (m_PlayerObjs[i].IsHoldItem())
			{
				hp[4] = 1000;
				priority[4] = i;
			}
		}

		// 優先度でソートする
		Array.Sort(hp, priority);

		// 優先度に基づいて割合付けを行っている
		int[] randomArray = new int[] { priority[4], priority[4], priority[4], priority[3], priority[3], priority[3], priority[2], priority[2], priority[1], priority[0] };

		System.Random rand = new System.Random();
		return randomArray[rand.Next(randomArray.Length)];
	}

	public void SearchAttackObj()
	{
		if (m_NowCoroutine != null)
		{
			StopCoroutine(m_NowCoroutine);
			m_NowCoroutine = null;
		}

		Debug.Log("SearchAttackObj呼び出し");
		// 参照はずし
		m_PlayerObjs = null;
		m_AttackObjs = null;
		m_AttackList = null;

		m_StageChangeNum += 1;	// ステージ変更回数カウント
		GameObject[] attackObjects;
		// 攻撃用オブジェクトの探索を行う
		attackObjects = GameObject.FindGameObjectsWithTag("BossAttackObj");

		// 攻撃パターン数を取得する
		m_AttackObjs = new AttackID[attackObjects.Length];

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

		m_AreaTime = new float[4] { 0.0f, 0.0f, 0.0f, 0.0f };

		m_NowCoroutine = StartCoroutine("BossAttack");

		// 労基オブジェクトを新しく見つける
		m_EviLoading.InitLoader();
	}

	int AreaIdentification(int targetNo)
	{
		// レイヤーマスクを設定
		// レイを画面奥方向へ飛ばして範囲オブジェクトを探索
		// オブジェクトが見つかったらエリアNoを返す
		// 見つからなかったら異常値として-1を返す

		int mask = LayerMask.GetMask("AreaJudge");
		RaycastHit info;
		Vector3 origin = m_PlayerObjs[targetNo].transform.position + (Vector3.up * 0.3f);
		if (Physics.Raycast(origin, Vector3.forward, out info, 10.0f, mask))
		{
			int newAreaNo = info.transform.GetComponent<AreaJudgment>().m_AreaNo;
			if (m_OldAreaNo == newAreaNo)
			{
				m_XORFlag = false;
			}
			else
			{
				m_XORFlag = true;
				m_OldAreaNo = newAreaNo;
			}
			return info.transform.GetComponent<AreaJudgment>().NextAttackNo();
		}
		return -1;
	}

	/// <summary>
	///  ボスへダメージを与える関数
	/// </summary>
	/// <param name="damage"></param>
	public void BossDamage(float damage)
	{
		m_BossDamage -= damage;
		m_BossHPUI.IsDamage();
		if (m_BossDamage < 0.0f) m_BossDamage = 0.0f;
		// TODO 今度、ボスの攻撃を徐々に減らす、ボスにダメージエフェクトを出すなどする場合はここをいじる
	}

	/// <summary>
	/// ボスの行動状態をコントロールするスクリプト
	/// </summary>
	/// <param name="ok"></param>
	public void BossBehaviorSwitching(bool CanYouDoIt)
	{
		m_AttackFlag = CanYouDoIt;
	}

	void ConditionUpdate()
	{
		float t = m_BossDamage / m_BossMaxDamage;

		if (t < 0.66f) m_Condition = BossCondition.Spicy;
		if (t < 0.33f) m_Condition = BossCondition.Dying;
	}

	public BossCondition GetCondition()
	{
		return m_Condition;
	}

	public float GetBossHP()
	{
		return m_BossDamage;
	}

	void AnmeFlagInit()
	{
		m_DownSwing = false;
		m_SideSwing = false;
		m_Beam = false;
	}

	void ChangeAnimFlag()
	{
		switch (m_NextAttackType)
		{
			case global::AttackID.AttackType.DownSwing:
				m_DownSwing = true;
				break;
			case global::AttackID.AttackType.SideSwing:
				m_SideSwing = true;
				break;
			case global::AttackID.AttackType.Beam:
				m_Beam = true;
				break;
		}
	}

	public void EndBoss()
	{
		// 次の周のためにライトをON
		m_Light.LightChage(true);
	}

	public void SetEmotion()
	{
		// 喜んでもいい時
		if (m_IsEmotion)
		{
			Debug.Log("ボス笑え！");
			m_Emotion = true;
		}
	}
}