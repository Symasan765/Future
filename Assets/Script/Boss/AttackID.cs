using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackID : MonoBehaviour {

	public int m_AttackID = 0;
	public float m_TimeSec = 5.0f;

	public enum AttackType
	{
		DownSwing,
		SideSwing,
		Beam
	}

	public AttackType m_AttackType = AttackType.DownSwing;
	GameObject m_EffectPrefab;

	private void Start()
	{
		GetComponent<MeshRenderer>().enabled = false;

		switch (m_AttackType)
		{
			case AttackType.DownSwing:
				break;
			case AttackType.SideSwing:
				break;
			case AttackType.Beam:
				m_EffectPrefab = (GameObject)Resources.Load("Prefab/BeamSmoke");
				break;
		}
	}

	/// <summary>
	/// 攻撃範囲にエフェクトを表示させる
	/// </summary>
	public void m_AttackEffect()
	{
		if (m_EffectPrefab != null)
		{
			GameObject obj = Instantiate(m_EffectPrefab, transform.position, Quaternion.identity);
			obj.transform.localScale = new Vector3(transform.localScale.x * obj.transform.localScale.x, transform.localScale.y * obj.transform.localScale.y, transform.localScale.y * obj.transform.localScale.y);
		}
	}
}
