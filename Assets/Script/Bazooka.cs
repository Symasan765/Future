using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bazooka : MonoBehaviour {
    [SerializeField]
    private GameObject BazookaPrefab;
    private GameObject BazookaObject;
    private int SyoukouNum;
    private int BazookaMaxMum = 20;

    private Vector3 TargetPos;
    
    //バズーカーのオブジェクトをPrefabからロードしておく。
	void Start () 
    {
        BazookaPrefab = Resources.Load("Prefab/Lee/Bazooka") as GameObject;
    }

    //Update関数の中で、特定の条件になるとバズーカーオブジェクトを生成する関数を呼び出す。
    void Update()
    {
        ShotBazooka();
    }
    //バズーカーを生成する。動的でオブジェクトを生成。in_posの位置に移動させる。
    void ShotBazooka()
    {
        Vector3 TargetVector = BazookaObject.transform.position - TargetPos;
        Vector3.Normalize(TargetVector);
        BazookaObject.transform.position += TargetVector;
    }

    void SetBazooka(Vector3 in_TargetPos)
    {
        TargetPos = in_TargetPos;
    }
    //証拠の数を調べる。現状では証拠タグをつけたオブジェクトの数を数えている。
    void FindSyoukou()
    {
        GameObject[] Syoukou = GameObject.FindGameObjectsWithTag("SYOUKOU");
        SyoukouNum = Syoukou.Length;
    }


}
