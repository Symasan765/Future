using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

	private GameObject[] SpawnPositoin = new GameObject[4];
	public GameObject[] PlayerObjects = new GameObject[4];

	const int PlayerNum = 4;

	void Awake ()
	{
		//プレイヤーの生成
		for (int i = 0; i < PlayerNum; i++)
		{
			SpawnPositoin[i] = GameObject.Find("SpawnPlayer" + (i + 1));

			GameObject playerObj = Instantiate(PlayerObjects[CharacterManager.SelectedCharacters[i]], SpawnPositoin[i].transform.position, transform.rotation);
			Player player = playerObj.GetComponent<Player>();
			player.PlayerIndex = i;
			playerObj.name = "Player" + (i + 1);
		}
	}
}
