using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// バズーカーの砲台（？）のスクリプト。
/// バズーカーの発射に関する動作を定義している。
/// バズーカーのオブジェクトにコンポネントとして当てておく。
/// </summary>
public class BazookaRifle : MonoBehaviour
{
    [SerializeField]
    private GameObject BazookaPrefab;               //ここにバズーカーのPrefabをUnityのInspectorで割当しておく。（またはResourece.Loadでロードするといい）
    private float SyoukoDistance = 3.0f;            //証拠を認識する範囲。
    private int SyoukoNum = 0;                      //全体の証拠の数。
    private int NearSyoukoNum = 0;                  //近づいた証拠の数。
    private GameObject[] Syouko_temp = new GameObject[3];


    void Update()
    {
        GameObject Boss = GameObject.FindGameObjectWithTag("BOSS");
        Debug.Log(Boss);
        //証拠を探す。
        DetectSyouko();

        //証拠が3つ以上だとバズーカーを発射し、証拠を削除。
        if (NearSyoukoNum == 3)
        {
            Instantiate(BazookaPrefab);
            BazookaPrefab.transform.position = this.transform.position;
            BazookaPrefab.GetComponent<BazookaBullet>().SetBazooka(Boss.gameObject.transform.position, this.transform.position);
            for(int i=0;i<3;i++)
            {
                Destroy(Syouko_temp[i]);
            }
        }


    }

    //証拠を探す関数。
    void DetectSyouko()
    {
        //とりあえずSYOUKOタグがついているオブジェクトを全部獲得。
        GameObject[] Syouko = GameObject.FindGameObjectsWithTag("SYOUKO");
        SyoukoNum = Syouko.Length;
        int NearSyoukoNum_Temp = 0;

        //証拠とバズーカーの距離がパラメータより近いと近づいた証拠だと判断する。
        for (int i = 0; i < SyoukoNum; i++)
        {
            if (Vector3.Distance(Syouko[i].gameObject.transform.position, this.gameObject.transform.position) <= SyoukoDistance)
            {
                Syouko_temp[NearSyoukoNum_Temp] = Syouko[i];        //あとで削除するために変数でオブジェクトを持っておく。
                NearSyoukoNum_Temp++;            
            }
        }
        NearSyoukoNum = NearSyoukoNum_Temp;
    }


}
