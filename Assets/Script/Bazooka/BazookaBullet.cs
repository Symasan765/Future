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
	private bool isFever = false;
	private float cntShotSec = 0;

	private Vector3 startPosition;
	private Vector3 EndScale = new Vector3(0.5f, 0.5f, 0.5f);
	private float speed = 0.8f;

	private Vector3[] curvePoint = new Vector3[2];
	private BazookaRifle bazookaRifle;

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


		float value = 1 - (Vector3.Distance(transform.position, TargetPos) / Vector3.Distance(startPosition, TargetPos));

		transform.localScale = Vector3.Lerp(transform.localScale, EndScale, value);

		transform.position = GetPoint(startPosition, curvePoint[0], curvePoint[1], TargetPos, cntShotSec);
		cntShotSec += Time.deltaTime * speed;
		if (value >= 1)
		{
			bazookaRifle.HitBullet(isFever, transform.position);
			Destroy(gameObject);
		}


    }

	public void SetBazookaRifleObj(GameObject _obj)
	{
		bazookaRifle = _obj.GetComponent<BazookaRifle>();
	}

    //敵の座標をセット
	public void SetCurvePointObj(GameObject _obj1, GameObject _obj2)
	{
		curvePoint[0] = _obj1.transform.position;
		curvePoint[1] = _obj2.transform.position;
	}
    public void SetBazooka(Vector3 in_TargetPos)
    {
		startPosition = transform.position;
		cntShotSec = 0;
        TargetPos = in_TargetPos;
        isPosSet = true;
	}

	public void SetFeverEvidenceFlg(bool _flg)
	{
		isFever = _flg;
	}

	Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		var a = Vector3.Lerp(p0, p1, t); // 緑色の点1
		var b = Vector3.Lerp(p1, p2, t); // 緑色の点2
		var c = Vector3.Lerp(p2, p3, t); // 緑色の点3

		var d = Vector3.Lerp(a, b, t);   // 青色の点1
		var e = Vector3.Lerp(b, c, t);   // 青色の点2

		return Vector3.Lerp(d, e, t);    // 黒色の点
	}
}
