using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageChangeManager : MonoBehaviour {

	[SerializeField]
	const int StageNum = 3;											//ステージ数
	public GameObject[] StageObjects = new GameObject[StageNum];	//ステージプレハブを格納
	public GameObject PlayerManagerObj;
	[SerializeField]
	private int nowStageIndex = 0;									//今のステージ番号

	private BossAttackManager bossAttackManager;
	private EvidenceSpawner[] AllEvidenceSpawners;					//全ステージのノーマル証拠を格納
	private int[] NormalEvidenceSpawnerNum = new int[StageNum];

	public bool isChangeStage=false;

	private GameObject nowStageObject = null;

	void Start ()
	{
		bossAttackManager = GameObject.FindGameObjectWithTag("BossManager").GetComponent<BossAttackManager>();

		int allNormalEvidenceNum = 0;
		for (int cntStageNum = 0; cntStageNum < StageNum; cntStageNum++)
		{
			//EvidenceSpawnerコンポーネントの取得
			Debug.Log("ステージ" + cntStageNum + "の証拠スポナー読み込み処理開始");
			EvidenceSpawner[] eviSpawner = StageObjects[cntStageNum].GetComponentsInChildren<EvidenceSpawner>();
			int eviNum = eviSpawner.Length;
			int normalEviNum=0;
			Debug.Log("証拠スポナー数：" + eviNum);
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
		}
		AllEvidenceSpawners = new EvidenceSpawner[allNormalEvidenceNum];

		int nowNormalEvidenceNum=0;
		for (int cntStageNum = 0; cntStageNum < StageNum; cntStageNum++)
		{
			//EvidenceSpawnerコンポーネントの取得
			EvidenceSpawner[] eviSpawner = StageObjects[cntStageNum].GetComponentsInChildren<EvidenceSpawner>();
			int eviNum = eviSpawner.Length;
			//取得したコンポーネントを一つの配列に格納
			for (int cntEviNum = 0; cntEviNum < eviNum; cntEviNum++)
			{
				if (eviSpawner[cntEviNum].gameObject.tag == "NormalEvidenceSpawner")
				{
					AllEvidenceSpawners[nowNormalEvidenceNum] = eviSpawner[cntEviNum];
					nowNormalEvidenceNum++;
				}
			}
		}
		Debug.Log("全ステージの証拠スポナー数：" + allNormalEvidenceNum);

		//最初のステージ作成
		CreateStage(nowStageIndex);

		//最初のステージ作成後にPlayerManagerを生成
		Instantiate(PlayerManagerObj, transform.position, transform.rotation).name = "PlayerManager";
	}
	
	void Update ()
	{
		//デバッグ用Eキー押下時ステージを順に変更
		if (Input.GetKeyDown(KeyCode.E))
		{
			isChangeStage = true;
			nowStageIndex++;
			if (nowStageIndex >= StageNum)
			{
				nowStageIndex = 0;
			}
		}

		ChangeStage(nowStageIndex);
	}

	void ChangeStage(int _nextStageIndex)
	{
		if (isChangeStage)
		{
			Destroy(nowStageObject);		//ステージの削除
			CreateStage(_nextStageIndex);	//ステージ生成
			isChangeStage = false;
		}
	}

	void CreateStage(int _stageIndex)
	{
		nowStageObject = Instantiate(StageObjects[_stageIndex], transform.position, transform.rotation);
		nowStageObject.name = "Stage" + nowStageIndex;
		bossAttackManager.SearchAttackObj();//AttackObjリロード
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
			if (AllEvidenceSpawners[nowEviIndex + i].isSetBazooka)
			{
				return false;
			}
		}
		return true;
	}

}
