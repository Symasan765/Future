using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bazooka : MonoBehaviour {
    [SerializeField]
    private GameObject BazookaObject;
    private int SyoukouNum;
    
    
    //バズーカーのオブジェクトをPrefabからロードしておく。
	void Start () 
    {
        BazookaObject = Resources.Load("Prefab/Lee/Bazooka") as GameObject;
    }

    //Update関数の中で、特定の条件になるとバズーカーオブジェクトを生成する関数を呼び出す。
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ShotBazooka(new Vector3(0.0f, 10.0f, 5.0f));
        }
    }

    //バズーカーを生成する。動的でオブジェクトを生成。in_posの位置に移動させる。
    void ShotBazooka(Vector3 in_pos)
    {
        GameObject BazookaBullet = Instantiate(BazookaObject);
        BazookaBullet.transform.position = in_pos;
    }

    //証拠の数を調べる。現状では証拠タグをつけたオブジェクトの数を数えている。
    void FindSyoukou()
    {
        GameObject[] Syoukou = GameObject.FindGameObjectsWithTag("SYOUKOU");
        SyoukouNum = Syoukou.Length;
    }


}
