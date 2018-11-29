using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScore : MonoBehaviour {

	public static int m_PlayerDownNum = 0;
	public static int m_AttackNum = 0;

	public static string m_ScoreStar;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	static public void Init()
	{
		m_PlayerDownNum = 0;
		m_AttackNum = 0;
	}
}
