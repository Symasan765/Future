using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    [SerializeField]
    GameObject[] playerArray;   // プレイヤーキャラの格納先

    [SerializeField]
    int n1;
    [SerializeField]
    int n2;

    [SerializeField]
    float longestDist;          // プレイヤーキャラ間の最長距離
    [SerializeField]
    Vector3 centerPos;          // 最も離れているキャラ間の中心地

    [SerializeField]
    float stageSize = 80.0f;

    // カメラの移動可能範囲の指定用
    [SerializeField]
    float width;    // 移動可能範囲(横)の1/2
    [SerializeField]
    float height;   // 移動可能範囲(縦)の1/2

    [SerializeField]
    Vector3 fromLeftBottom; // 左下
    [SerializeField]
    Vector3 toRightUp;      // 右上

	void Start () {
        playerArray = GameObject.FindGameObjectsWithTag("Player");
	}
	
	void Update () {
        CheckDistance();
        //SetCamPos();
	}

    void CheckDistance() {
        float distance = 0; // 二点間の距離(float)
        Vector3 middlePos = Vector3.zero;

        // 配列内の各キャラ間の距離を計算
        for (int cnt1 = 0; cnt1 < playerArray.Length; cnt1++) {
            float nowDist;
            for (int cnt2 = cnt1 + 1; cnt2 < playerArray.Length; cnt2++) {
                nowDist = Vector3.Distance(playerArray[cnt1].transform.position, playerArray[cnt2].transform.position);

                // 今保存してある最長距離よりも長ければ更新
                if (distance < nowDist) {
                    distance = nowDist;
                    middlePos = (playerArray[cnt1].transform.position + playerArray[cnt2].transform.position) / 2;

                    fromLeftBottom = new Vector3(playerArray[cnt1].transform.position.x, centerPos.y - height, 0);
                    toRightUp = new Vector3(playerArray[cnt2].transform.position.x, centerPos.y + height, 0);

                    n1 = cnt1;
                    n2 = cnt2;
                }
            }
        }

        longestDist = distance;
        centerPos = Vector3.Slerp(centerPos, middlePos, 0.25f);
    }

    void SetCamPos() {
        Vector3 camPos;

        if (longestDist > stageSize) {
            camPos = new Vector3(0, 0, -40);
        }
        else {
            camPos = centerPos;
            camPos.z = ((longestDist / stageSize) * -40) -5;
            camPos.z = Mathf.Clamp(camPos.z, -40, -5);
        }

        Camera.main.transform.position = Vector3.Slerp(Camera.main.transform.position, camPos, 0.25f);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;

        //fromLeftBottom = new Vector3(centerPos.x - width, centerPos.y - height, Camera.main.transform.position.z);
        //toRightUp = new Vector3(centerPos.x + width, centerPos.y + height, Camera.main.transform.position.z);

        Vector3 leftBottom;
        Vector3 leftUp;
        Vector3 rightBottom;
        Vector3 rightUp;

        leftBottom = fromLeftBottom;
        rightUp = toRightUp;
        leftUp = new Vector3(leftBottom.x, rightUp.y, leftBottom.z);
        rightBottom = new Vector3(rightUp.x, leftBottom.y, rightUp.z);

        Gizmos.DrawLine(leftBottom, leftUp);
        Gizmos.DrawLine(leftUp, rightUp);
        Gizmos.DrawLine(rightUp, rightBottom);
        Gizmos.DrawLine(rightBottom, leftBottom);
    }
}
