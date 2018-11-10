using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackID : MonoBehaviour {

	public int m_AttackID = 0;
	public float m_TimeSec = 5.0f;

	GameObject m_BossLaunchPos;		// ビーム時のビーム発射位置

	public enum AttackType
	{
		DownSwing,
		SideSwing,
		Beam
	}

	public AttackType m_AttackType = AttackType.DownSwing;
	GameObject m_EffectPrefab;
	GameObject m_BeamObj;

	GameObject m_Beam;

	private void Start()
	{
		GetComponent<MeshRenderer>().enabled = false;

		switch (m_AttackType)
		{
			case AttackType.DownSwing:
				m_EffectPrefab = (GameObject)Resources.Load("Prefab/DownSwingEffect");
				break;
			case AttackType.SideSwing:
				m_EffectPrefab = (GameObject)Resources.Load("Prefab/SideSwingAttack");
				break;
			case AttackType.Beam:
				m_EffectPrefab = (GameObject)Resources.Load("Prefab/BeamSmoke");
				m_BossLaunchPos = GameObject.Find("BossLaunchPos").gameObject;
				m_BeamObj = (GameObject)Resources.Load("Prefab/BeamObj");
				break;
		}
	}

	public void BeamLunch()
	{
		if(m_AttackType == AttackType.Beam)
		{
			m_Beam = Instantiate(m_BeamObj);
			m_Beam.GetComponent<BeamAttack>().SetData(m_BossLaunchPos, gameObject);
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

			if(m_Beam != null)
			{
				Destroy(m_Beam, 0.0f);
				m_Beam = null;
			}
		}
	}
}
