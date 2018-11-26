using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageChangeManager : MonoBehaviour {

	const int PlayerNum = 4;
	const int StageNum = 3;											//ステージ数
	public GameObject[] StageObjects = new GameObject[StageNum];	//ステージプレハブを格納
	public GameObject PlayerManagerObj;

	public float PlayerRespawnSec = 1;

	private int nowStageIndex = 0;									//今のステージ番号

	private BossAttackManager bossAttackManager;
	private EvidenceSpawner[] AllEvidenceSpawners;					//全ステージのノーマル証拠を格納
	private int[] NormalEvidenceSpawnerNum = new int[StageNum];
	private GameObject[] playerObjects;
	private GameObject[] SpawnPositoins;

	private bool isChangeStage=false;

	private GameObject nowStageObject = null;
	private FeverManager feverManager;

	private float cntChangeStage = 0;

	void Start ()
	{
		//スポーンポジションの数を計算
		int allSpawnPositionNum = PlayerNum * StageNum;
		SpawnPositoins = new GameObject[allSpawnPositionNum];

		feverManager = GameObject.Find("FeverManager").GetComponent<FeverManager>();
		bossAttackManager = GameObject.FindGameObjectWithTag("BossManager").GetComponent<BossAttackManager>();

		int allNormalEvidenceNum = 0;

		for (int cntStageNum = 0; cntStageNum < StageNum; cntStageNum++)
		{
			//EvidenceSpawnerコンポーネントの取得
			Debug.Log("ステージ" + cntStageNum + "の証拠スポナー読み込み処理開始");
			EvidenceSpawner[] eviSpawner = StageObjects[cntStageNum].GetComponentsInChildren<EvidenceSpawner>();
			int eviNum = eviSpawner.Length;
			int normalEviNum = 0;
			//取得したコンポーネントを一つの配列に格納
			for (int cntEviNum = 0; cntEviNum < eviNum; cntEviNum++)
			{
				if (eviSpawner[cntEviNum].gameObject.tag == "NormalEvidenceSpawner")
				{
					normalEviNum++;
					allNormalEvidenceNum++;
				}
			}
			NormalEvidenceSpawnerNum[cntStageNum] = normalEviNum;
			
			//スポーンポジションの取得
			for (int cntChildNum = 0; cntChildNum < StageObjects[cntStageNum].transform.childCount; cntChildNum++)
			{
				if (StageObjects[cntStageNum].transform.GetChild(cntChildNum).name == "SpawnPosition")
				{
					for (int i = 0; i < StageObjects[cntStageNum].transform.GetChild(cntChildNum).transform.childCount; i++)
					{
						if (StageObjects[cntStageNum].transform.GetChild(cntChildNum).transform.GetChild(i).name == "SpawnPlayer1")
						{
							SpawnPositoins[(cntStageNum * PlayerNum)] = StageObjects[cntStageNum].transform.GetChild(cntChildNum).transform.GetChild(i).gameObject;
						}
						if (StageObjects[cntStageNum].transform.GetChild(cntChildNum).transform.GetChild(i).name == "SpawnPlayer2")
						{
							SpawnPositoins[(cntStageNum * PlayerNum) + 1] = StageObjects[cntStageNum].transform.GetChild(cntChildNum).transform.GetChild(i).gameObject;
						}
						if (StageObjects[cntStageNum].transform.GetChild(cntChildNum).transform.GetChild(i).name == "SpawnPlayer3")
						{
							SpawnPositoins[(cntStageNum * PlayerNum) + 2] = StageObjects[cntStageNum].transform.GetChild(cntChildNum).transform.GetChild(i).gameObject;
						}
						if (StageObjects[cntStageNum].transform.GetChild(cntChildNum).transform.GetChild(i).name == "SpawnPlayer4")
						{
							SpawnPositoins[(cntStageNum * PlayerNum) + 3] = StageObjects[cntStageNum].transform.GetChild(cntChildNum).transform.GetChild(i).gameObject;
						}
					}
					break;
				}
			}
		}

		AllEvidenceSpawners = new EvidenceSpawner[allNormalEvidenceNum];

		CreateStage(nowStageIndex);	

		//最初のステージ作成後にPlayerManagerを生成
		Instantiate(PlayerManagerObj, transform.position, transform.rotation).name = "PlayerManager";

		playerObjects = GameObject.FindGameObjectsWithTag("Player");
	}
	
	void Update ()
	{
		CanBazookaShot();
		//デバッグ用Eキー押下時ステージを順に変更
		if (Input.GetKeyDown(KeyCode.E))
		{
			ChangeStage();
		}

		ChangeNowStage(nowStageIndex);
	}

	public void ChangeStage()
	{
		isChangeStage = true;
		nowStageIndex++;
		if (nowStageIndex >= StageNum)
		{
			nowStageIndex = 0;
		}

		ChangeNowStage(nowStageIndex);
	}

	public int GetNowNormalEvidenceNum()
	{
		return NormalEvidenceSpawnerNum[nowStageIndex];
	}

	private void ChangeNowStage(int _nextStageIndex)
	{
		if (isChangeStage)
		{
			Destroy(nowStageObject);		//ステージの削除
			CreateStage(_nextStageIndex);	//ステージ生成
			isChangeStage = false;
			RespawnPlayers(_nextStageIndex);
		}
	}

	void CreateStage(int _stageIndex)
	{
		nowStageObject = Instantiate(StageObjects[_stageIndex], transform.position, transform.rotation);
		nowStageObject.name = "Stage" + nowStageIndex;
		bossAttackManager.SearchAttackObj();//AttackObjリロード

		//EvidenceSpawnerコンポーネントの取得
		EvidenceSpawner[] eviSpawner = nowStageObject.GetComponentsInChildren<EvidenceSpawner>();
		int eviNum = eviSpawner.Length;
		//配列の開始位置を計算
		int nowEviIndex = 0;
		for (int i = 0; i < nowStageIndex; i++)
		{
			nowEviIndex += NormalEvidenceSpawnerNum[i];
		}
		//生成したステージのコンポーネントを格納
		for (int cntEviNum = 0; cntEviNum < eviNum; cntEviNum++)
		{
			if (eviSpawner[cntEviNum].gameObject.tag == "NormalEvidenceSpawner")
			{
				AllEvidenceSpawners[nowEviIndex + cntEviNum] = eviSpawner[cntEviNum].gameObject.GetComponent<EvidenceSpawner>();
			}
		}
	}

	public bool CanBazookaShot()
	{
		int nowEviIndex=0;

		//配列の開始位置を計算
		for (int i = 0; i < nowStageIndex; i++)
		{
			nowEviIndex += NormalEvidenceSpawnerNum[i];
		}

		for (int i = 0; i < NormalEvidenceSpawnerNum[nowStageIndex]; i++)
		{
			if (!AllEvidenceSpawners[nowEviIndex + i].isSetBazooka)
			{
				return false;
			}
		}
		return true;
	}

	public int GetNowSetEvidenceNum()
	{
		int nowEviIndex = 0;

		//配列の開始位置を計算
		for (int i = 0; i < nowStageIndex; i++)
		{
			nowEviIndex += NormalEvidenceSpawnerNum[i];
		}

		int setEviNum = 0;

		for (int i = 0; i < NormalEvidenceSpawnerNum[nowStageIndex]; i++)
		{
			if (AllEvidenceSpawners[nowEviIndex + i].isSetBazooka)
			{
				setEviNum++;
			}
		}
		return GetNowNormalEvidenceNum() - setEviNum;
	}

	private void RespawnPlayers(int _stageIndex)
	{
		for (int i = 0; i < PlayerNum; i++)
		{
			if (playerObjects[i].name == "Player1")
			{
				playerObjects[i].transform.position = SpawnPositoins[(_stageIndex * PlayerNum)].transform.position;
				Player pl = playerObjects[i].GetComponent<Player>();
				pl.SetRespawnPosition(SpawnPositoins[(_stageIndex * PlayerNum)].transform.position);
			}
			if (playerObjects[i].name == "Player2")
			{
				playerObjects[i].transform.position = SpawnPositoins[(_stageIndex * PlayerNum) + 1].transform.position;
				Player pl = playerObjects[i].GetComponent<Player>();
				pl.SetRespawnPosition(SpawnPositoins[(_stageIndex * PlayerNum) + 1].transform.position);
			}
			if (playerObjects[i].name == "Player3")
			{
				playerObjects[i].transform.position = SpawnPositoins[(_stageIndex * PlayerNum) + 2].transform.position;
				Player pl = playerObjects[i].GetComponent<Player>();
				pl.SetRespawnPosition(SpawnPositoins[(_stageIndex * PlayerNum) + 2].transform.position);
			}
			if (playerObjects[i].name == "Player4")
			{
				playerObjects[i].transform.position = SpawnPositoins[(_stageIndex * PlayerNum) + 3].transform.position;
				Player pl = playerObjects[i].GetComponent<Player>();
				pl.SetRespawnPosition(SpawnPositoins[(_stageIndex * PlayerNum) + 3].transform.position);
			}
		}
	}
}
