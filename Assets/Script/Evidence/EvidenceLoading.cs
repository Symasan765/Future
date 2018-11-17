using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvidenceLoading : MonoBehaviour {

	public GameObject m_TextPrefab;
	
	/// <summary>
	/// 現在表れている労基オブジェクトを取得しておく
	/// </summary>
	public void InitLoader()
	{
		// ゲーム中にあるすべてのバズーカを取得する
		var objs = GameObject.FindGameObjectsWithTag("Bazooka");
		
		for(int i = 0; i < objs.Length; i++)
		{
			var obj = objs[i].GetComponent<BazookaRifle>();
			var TextObj = Instantiate(m_TextPrefab,obj.transform);
			TextObj.transform.position = obj.transform.position;
			TextObj.GetComponent<LoadingText>().SetInit(obj);
		}

		Debug.Log("InitLoader バズーカ数" + objs.Length + "個");
	}

	// Update is called once per frame
	void Update () {
		
	}
}
