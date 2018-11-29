using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackID : MonoBehaviour {

	public int m_AttackID = 0;
	public float m_TimeSec = 5.0f;
	public float m_NextAttackDelaySec = 5.0f;
	public bool m_EmotionFlag = true;
	public int m_NextAttackID = -1;

	GameObject m_BossLaunchPos;		// ビーム時のビーム発射位置

	public enum AttackType
	{
		DownSwing,		// 振り下ろし
		SideSwing,		// 薙ぎ払い
		Beam,				// ビーム
		Special			// ボス必殺
	}

	public AttackType m_AttackType = AttackType.DownSwing;
	GameObject m_EffectPrefab;
	GameObject m_EffectSmokePrefab;
	GameObject m_BeamObj;

	GameObject m_Beam;

	private void Start()
	{
		GetComponent<MeshRenderer>().enabled = false;
		m_EffectSmokePrefab = (GameObject)Resources.Load("Prefab/EffectBossSmoke");
		/*
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
			case AttackType.Special:
				m_EffectPrefab = (GameObject)Resources.Load("Prefab/BeamSmoke");
				m_BossLaunchPos = GameObject.Find("BossLaunchPos").gameObject;
				m_BeamObj = (GameObject)Resources.Load("Prefab/BeamObj");
				break;
		}*/
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
		if (m_EffectSmokePrefab != null)
		{
			//GameObject obj = Instantiate(m_EffectPrefab, transform.position, Quaternion.identity);
			//obj.transform.localScale = new Vector3(transform.localScale.x * obj.transform.localScale.x, transform.localScale.y * obj.transform.localScale.y, transform.localScale.y * obj.transform.localScale.y);
			SmokeEffect();
			if(m_Beam != null)
			{
				Destroy(m_Beam, 0.0f);
				m_Beam = null;
			}
		}
	}

	private void SmokeEffect()
	{
		int createCountX = 0;
		int createCountY = 0;
		for (int cntY = 0; cntY < transform.localScale.y; cntY++)
		{
			if (createCountY == 0)
			{
				for (int cntX = 0; cntX < transform.localScale.x; cntX++)
				{
					if (createCountX == 0)
					{
						float effectWidth = 1.0f;
						Vector3 effectPos = new Vector3((transform.position.x - ((transform.localScale.x / 2) * effectWidth)) + (effectWidth * cntX), (transform.position.y + effectWidth - ((transform.localScale.y / 2) * effectWidth)) + (effectWidth * cntY), transform.position.z);
						Instantiate(m_EffectSmokePrefab, effectPos, Quaternion.identity);
					}
					createCountX++;
					if (createCountX > 1)
					{
						createCountX = 0;
					}
				}
			}
			createCountY++;
			if (createCountY > 1)
			{
				createCountY = 0;
			}
		}
		
	}
}
