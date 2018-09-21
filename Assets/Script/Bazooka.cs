using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bazooka : MonoBehaviour {
    [SerializeField]
    private int SyoukouNum;
    private Vector3 TargetPos;
    private Vector3 TargetVector;
    

    //バズーカーのオブジェクトをPrefabからロードしておく。
	void Start () 
    {
        //StartCoroutine(ShotBazooka());
    }

    //Update関数の中で、特定の条件になるとバズーカーオブジェクトを生成する関数を呼び出す。
    void Update()
    {
        TargetVector = TargetPos;
        Vector3.Normalize(TargetVector);
        TargetVector = TargetVector * 1 / 100;
        if (Vector3.Distance(TargetPos, this.transform.position) > 0.5f)
        {
            this.transform.position += TargetVector;
        }
        FindSyoukou();
    }
   
    

    public void SetBazooka(Vector3 in_TargetPos)
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
