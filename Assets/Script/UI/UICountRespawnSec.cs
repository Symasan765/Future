using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UICountRespawnSec : MonoBehaviour {

	public float respawnSec = 5;
	public GameObject[] countObj = new GameObject[3];
	private TextMesh[] textMesh = new TextMesh[3];
	private int playerIndex = 0;
	void Start () 
	{
		for (int i = 0; i < 3; i++)
		{
			textMesh[i] = countObj[i].GetComponent<TextMesh>();
		}
		if (playerIndex == 0)
		{
			textMesh[0].color = Color.blue;
			textMesh[2].color = Color.blue;
		}
		if (playerIndex == 1)
		{
			textMesh[0].color = Color.red;
			textMesh[2].color = Color.red;
		}
		if (playerIndex == 2)
		{
			textMesh[0].color = Color.yellow;
			textMesh[2].color = Color.yellow;
		}
		if (playerIndex == 3)
		{
			textMesh[0].color = Color.green;
			textMesh[2].color = Color.green;
		}
	}
	
	void Update ()
	{
		if (respawnSec > 0)
		{
			respawnSec -= Time.deltaTime;
			if (respawnSec <= 0)
			{
				Destroy(gameObject);
			}
			for (int i = 0; i < 2; i++)
			{
				textMesh[i].text = ((int)respawnSec + 1).ToString();
			}
		}	
	}

	public void SetPlayerIndex(int _index)
	{
		playerIndex = _index;
	}

	public void SetRespawnSec(float _sec)
	{
		respawnSec = _sec;
	}
}
