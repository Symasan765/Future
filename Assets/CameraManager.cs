using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    GameObject[] playerArray;   // プレイヤーキャラの格納先
    [SerializeField]
    float longestDist;          // プレイヤーキャラ間の最長距離
    [SerializeField]
    Vector3 centerPos;          // 最も離れているキャラ間の中心地

    [SerializeField]
    float StageSize = 80.0f;

	void Start () {
        playerArray = GameObject.FindGameObjectsWithTag("Player");
        //Array.Sort(playerArray, (a, b) => a.time - b.time)
	}
	
	void Update () {
        CheckDistance();
        SetCamPos();
	}

    void CheckDistance() {
        float distance = 0; // 二点間の距離(float)
        Vector3 middlePos = Vector3.zero;

        // 配列内の各キャラ間の距離を計算
        for (int cnt1 = 0; cnt1 < playerArray.Length; cnt1++) {
            float nowDist;
            for (int cnt2 = 1; cnt2 < playerArray.Length; cnt2++) {
                nowDist = Vector3.Distance(playerArray[cnt1].transform.position, playerArray[cnt2].transform.position);

                // 今保存してある最長距離よりも長ければ更新
                if (distance < nowDist) {
                    distance = nowDist;
                    middlePos = (playerArray[cnt1].transform.position + playerArray[cnt2].transform.position) / 2;
                }
            }
        }

        longestDist = distance;
        centerPos = Vector3.Slerp(centerPos, middlePos, 0.25f);
    }

    void SetCamPos() {
        Vector3 camPos;

        if (longestDist > StageSize) {
            camPos = new Vector3(0, 0, -40);
        }
        else {
            camPos = centerPos;
            camPos.z = ((longestDist / StageSize) * -40) -5;
            camPos.z = Mathf.Clamp(camPos.z, -40, -5);
        }

        Camera.main.transform.position = Vector3.Slerp(Camera.main.transform.position, camPos, 0.25f);
    }
}
