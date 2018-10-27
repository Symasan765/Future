using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackRange : MonoBehaviour {

	Vector3 m_AttackPos;		// 攻撃中心位置
	Vector2 m_Range;          // 攻撃範囲
	float m_AttackTime;      // 攻撃秒数
	float m_AttackCount;        // 攻撃までのカウント

	public GameObject m_AttackRangePrefab;
	GameObject m_AttackRangeBoard;
	Material m_Material;

	bool m_AttackFlag;

	public Shader m_Shader;
	public float m_LimitTime;   // ギリギリのタイミングで攻撃範囲を全体に映す際の残り時間

	BossAttackManager m_BossAttackManager;
	
	// Update is called once per frame
	void Update () {
		// 攻撃中止！
		if(m_BossAttackManager.m_AttackFlag == false)
		{
			Destroy(gameObject);
		}

		AttackUpdate();
	}

	/// <summary>
	/// 攻撃命令を出して指定秒後にその範囲を攻撃する
	/// </summary>
	/// <param name="atackPos">攻撃中心座標</param>
	/// <param name="range">攻撃範囲</param>
	/// <param name="atackTime">攻撃までの時間</param>
	public void AttackCommand(BossAttackManager bm,Vector2 atackPos,Vector2 range,float atackTime)
	{
		// 初期化処理
		m_AttackFlag = false;
		m_AttackRangeBoard = Instantiate(m_AttackRangePrefab);
		m_BossAttackManager = bm;

		m_Material = m_AttackRangeBoard.GetComponent<Renderer>().material;

		// 攻撃情報保持
		m_AttackPos = new Vector3(atackPos.x, atackPos.y,5.5f);
		m_Range = range;
		m_AttackTime = atackTime;
		m_AttackCount = 0.0f;
		m_AttackRangeBoard.transform.position = m_AttackPos;
		m_AttackRangeBoard.transform.localScale = new Vector3(range.x, range.y, 1.0f);
	}

	void AttackUpdate()
	{
		if(m_AttackCount < m_AttackTime)
		{
			// 点滅処理
			RangeFlashing();
			// コントローラー振動処理
			DangerSignal();

			m_AttackCount += Time.deltaTime;
			m_AttackFlag = true;

			// 攻撃ギリギリのタイミングになったら一瞬だけレーダー外も映るようにシェーダーを入れ替える
			float remainingTime = m_AttackTime - m_AttackCount;
			if (remainingTime < m_LimitTime)
			{
				LimitAnimation(remainingTime);
				m_Material.shader = m_Shader;
			}
		}
		else
		{
            // タイマーが0になった時にプレイヤーを攻撃
			AttackPlayer();
            SoundManager.Get.PlaySE("BossAttack");
		}
	}

	void LimitAnimation(float remainingTime)
	{
		float t = remainingTime / m_LimitTime;
		t = t * t * (3 - 2 * t);
		m_AttackRangeBoard.transform.localScale = new Vector3(m_Range.x, m_Range.y * t, 1.0f) ;
	}

	void RangeFlashing()
	{
		// TODO 現在適当に点滅
		int cnt = (int)(m_AttackCount / 0.1f);
		if(cnt % 3 == 0)
		{
			m_AttackRangeBoard.active = false;
		}
		else
		{
			m_AttackRangeBoard.active = true;
		}
	}

	// プレイヤーに攻撃を仕掛ける関数
	void AttackPlayer()
	{
		if (m_AttackFlag)
		{
			RaycastHit[] hitInfo = Physics.BoxCastAll(m_AttackPos, new Vector3(m_Range.x / 2.0f, m_Range.y / 2.0f, 0.0f), Vector3.back, Quaternion.identity, 15.0f);
			
			// 当たったオブジェクトからプレイヤーを探してダメージ処理
			for(int i = 0; i < hitInfo.Length; i++)
			{
				if (hitInfo[i].collider.gameObject.tag == "Player")
					hitInfo[i].collider.gameObject.GetComponent<Player>().HitBossAttack();
				// TODO ボスの攻撃モーションを呼ぶならここ
				Destroy(m_AttackRangeBoard);
				Destroy(gameObject);
			}
			m_AttackFlag = false;
		}
	}

	// ボスの攻撃範囲に入っているプレイヤーのコントローラーを振動させる
	void DangerSignal()
	{
		int timeStand = (int)(m_AttackCount / 0.2f);
		if (timeStand % 2 == 0)
		{
			// 実際の攻撃範囲と同じだけ探索する
			RaycastHit[] hitInfo = Physics.BoxCastAll(m_AttackPos, new Vector3(m_Range.x / 2.0f, m_Range.y / 2.0f, 0.0f), Vector3.back, Quaternion.identity, 15.0f);

			// 当たったオブジェクトからプレイヤーを探してコントローラー振動処理
			for (int i = 0; i < hitInfo.Length; i++)
			{
				if (hitInfo[i].collider.gameObject.tag == "Player")
				{
					int idx = hitInfo[i].collider.gameObject.GetComponent<Player>().PlayerIndex;
					float t = m_AttackCount / m_AttackTime;     // 徐々に振動を強くする
					XPad.Get.SetVibration(idx, 0.0f, t, 0.2f * (1.0f -t));		// タイムの式は後半ほど振動を細切れにしている
				}
			}
		}
	}
}
