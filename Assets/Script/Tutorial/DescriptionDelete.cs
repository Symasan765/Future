using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescriptionDelete : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnTriggerEnter(Collider collision)
	{
		// 一度触れたらすべての攻撃オブジェクトを消滅させる
		var objs = GameObject.FindGameObjectsWithTag("Description");
		for (int i = 0; i < objs.Length; i++)
		{
			Destroy(objs[i]);
		}
	}
}
