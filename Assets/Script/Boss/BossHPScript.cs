using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHPScript : MonoBehaviour {

	BossAttackManager m_BossManager;
	Image m_Image;
	
	public Color m_Margin = Color.green;
	public Color m_Spicy = Color.yellow;
	public Color m_Dying = Color.red;

	public float m_MaxBarLenPix = 1080.0f;

	RectTransform m_RectTransform;
	// Use this for initialization
	void Start () {
		m_BossManager = GameObject.Find("BossAttackManager").GetComponent<BossAttackManager>();
		m_Image = GetComponent<Image>();

		m_RectTransform = GetComponent<RectTransform>();
		m_RectTransform.localScale = new Vector3(1.0f, 1.0f);
	}
	
	// Update is called once per frame
	void Update () {
		ColorChange();

		float t = m_BossManager.GetBossHP() / m_BossManager.m_BossMaxDamage;
		m_RectTransform.localScale = new Vector3(t, 1.0f);
	}

	void ColorChange()
	{
		switch (m_BossManager.GetCondition())
		{
			case BossAttackManager.BossCondition.Margin:
				m_Image.color = m_Margin;
				break;
			case BossAttackManager.BossCondition.Spicy:
				m_Image.color = m_Spicy;
				break;
			case BossAttackManager.BossCondition.Dying:
				m_Image.color = m_Dying;
				break;
		}
	}
}