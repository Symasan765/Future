using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// バズーカーの弾の動きを定義したスクリプト。
/// バズーカーの弾のPrefabにコンポネントとして当てておくと良い。
/// </summary>
public class BazookaBullet : MonoBehaviour {
    
    public Vector3 TargetPos;

    public Vector3 TargetVector;

    public Vector3 MyPos;

    public bool isPosSet = false;

    
    //Updateでがターゲッティング関数をずっと呼び出す。
    void Update()
    {
        if(isPosSet)
        {
            Targetting();
        }
    }

    //的に向かっていく処理。
    void Targetting()
    {
        //自分から敵へのベクトルを求める。
        TargetVector = TargetPos - MyPos;
        Vector3.Normalize(TargetVector);
        TargetVector = TargetVector * 1 / 100;

        //敵に近づく。
        if (Vector3.Distance(TargetPos, this.transform.position) > 0.5f)
        {
            this.transform.position += TargetVector;
        }
        if (Vector3.Distance(TargetPos, this.transform.position) <= 0.5f)
        {
            Destroy(this.gameObject);
        }
    }

    //敵の座標をセット
    public void SetBazooka(Vector3 in_TargetPos, Vector3 in_MyPos)
    {
        TargetPos = in_TargetPos;
        MyPos = in_MyPos;
        TargetVector = TargetPos - MyPos;
        isPosSet = true;
    }
}
