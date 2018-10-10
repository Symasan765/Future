using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThroughFloorCheck : MonoBehaviour {

	Rigidbody m_Rigid;
	CapsuleCollider m_Capsule;

	int ThroughFloorLayerNo = 0;
	int DefaultLayerNo = 0;

	float m_FallingTimeCntSec;

	private void Start()
	{
		m_Rigid = GetComponent<Rigidbody>();
		m_Capsule = GetComponent<CapsuleCollider>();
		ThroughFloorLayerNo = LayerMask.NameToLayer("ThroughFloor");
		DefaultLayerNo = LayerMask.NameToLayer("Default");
		m_FallingTimeCntSec = 0;
	}

	private void Update()
	{
		// 落下中ではない
		if(m_FallingTimeCntSec <= 0.0f)
		{
			Rising();
		}
		// 落下中
		else
		{
			Fall();
		}
	}

	/// <summary>
	/// 落下状態に変更する関数
	/// </summary>
	/// <param name="fallTimeSec">落下判定時間を指定する(秒)</param>
	public void IsFall(float fallTimeSec)
	{
		m_FallingTimeCntSec = fallTimeSec;
		gameObject.layer = ThroughFloorLayerNo;
	}

	void Rising()
	{
		// まずはキャラの足元の座標を計算する
		Vector3 capsulePos = transform.position + m_Capsule.center;
		Vector3 footPos = new Vector3(capsulePos.x, capsulePos.y - (m_Capsule.height / 2.0f) - 0.35f, capsulePos.z);   // 0.001fは地面から少し上からレイを飛ばすため

		// レイを作成
		Ray ray = new Ray(footPos, Vector3.up);

		// レイヤーの管理番号を取得

		// マスクへの変換（ビットシフト）
		int layerMask = 1 << ThroughFloorLayerNo;

		gameObject.layer = DefaultLayerNo;

		RaycastHit hit;
		// Rayが衝突したかどうか(*1.2fは少し長めに判定を取るため)
		if (Physics.Raycast(ray, out hit, m_Capsule.height * 1.2f, layerMask))
		{
			gameObject.layer = ThroughFloorLayerNo;
		}
	}

	void Fall()
	{
		m_FallingTimeCntSec -= Time.deltaTime;
		if (m_FallingTimeCntSec < 0.0f)
			m_FallingTimeCntSec = 0.0f;
	}
}
