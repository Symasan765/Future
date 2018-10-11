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
    private float EvidenceDistance = 3.0f;            //証拠を認識する範囲。
    private int EvidenceNum = 0;                      //全体の証拠の数。
    public int NearEvidenceNum = 0;                  //近づいた証拠の数。
    private GameObject[] Evidence_temp = new GameObject[3];
	

	void Update()
    {
        GameObject Boss = GameObject.FindGameObjectWithTag("BOSS");
        Debug.Log(Boss);
		//証拠を探す。
		DetectEvidence();

        //証拠が3つ以上だとバズーカーを発射し、証拠を削除。
        if (NearEvidenceNum == 3)
        {
            Instantiate(BazookaPrefab);
            BazookaPrefab.transform.position = this.transform.position;
            BazookaPrefab.GetComponent<BazookaBullet>().SetBazooka(Boss.gameObject.transform.position, this.transform.position);
            for(int i=0;i<3;i++)
            {
                Destroy(Evidence_temp[i]);
            }
        }


    }

    //証拠を探す関数。
    void DetectEvidence()
    {
        //とりあえずSYOUKOタグがついているオブジェクトを全部獲得。
        GameObject[] Syouko = GameObject.FindGameObjectsWithTag("SYOUKO");
		EvidenceNum = Syouko.Length;
        int NearEvidenceNum_Temp = 0;

        //証拠とバズーカーの距離がパラメータより近いと近づいた証拠だと判断する。
        for (int i = 0; i < EvidenceNum; i++)
        {
            if (Vector3.Distance(Syouko[i].gameObject.transform.position, this.gameObject.transform.position) <= EvidenceDistance)
            {
				Evidence_temp[NearEvidenceNum_Temp] = Syouko[i];        //あとで削除するために変数でオブジェクトを持っておく。
				NearEvidenceNum_Temp++;            
            }
        }
		NearEvidenceNum = NearEvidenceNum_Temp;
    }


}
