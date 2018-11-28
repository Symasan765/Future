using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

	private GameObject[] SpawnPosition = new GameObject[4];
	public GameObject[] PlayerObjects = new GameObject[4];
	private GameObject[] UIPlayerMentalNumObjs = new GameObject[4];
	const int PlayerNum = 4;
	GameObject[] plyObjs = new GameObject[4];
	UIManager uiManager;
	StageChangeManager stageChangeManager;
	private float respawnSec = 1;
	void Awake ()
	{
		uiManager = GameObject.FindGameObjectWithTag("UIManagerObject").GetComponent<UIManager>();
		stageChangeManager = GameObject.Find("StageChangeManager").GetComponent<StageChangeManager>();
		respawnSec = stageChangeManager.PlayerRespawnSec;
		//プレイヤーの生成
		for (int i = 0; i < PlayerNum; i++)
		{
			Debug.Log("プレイヤー生成");
			Debug.Log("CharacterNum" + i + ":" + CharacterManager.SelectedCharacters[i]);
			SpawnPosition[i] = GameObject.Find("SpawnPlayer" + (i + 1));

			plyObjs[i] = Instantiate(PlayerObjects[CharacterManager.SelectedCharacters[i]], SpawnPosition[i].transform.position, transform.rotation);
			Player player = plyObjs[i].GetComponent<Player>();
			player.SetRespawnSec(respawnSec);
			player.PlayerIndex = i;
			//uiManager.SetPlayerArray(i, plyObjs[i].GetComponent<Player>());
			plyObjs[i].name = "Player" + (i + 1);
		}

		for (int i = 0; i < PlayerNum; i++)
		{
			uiManager.SetPlayerUI(i, plyObjs[i]);
		}

	}

	public void ReloadBazookaObjAndEvidenceObj()
	{
		for (int i = 0; i < PlayerNum; i++)
		{
			Player ply = plyObjs[i].GetComponent<Player>();
			ply.ReloadBazookaObjAndEvidenceObj();
		}
	}
}
