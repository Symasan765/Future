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


	private void Start()
	{
		GetComponent<MeshRenderer>().enabled = false;
	}
}
