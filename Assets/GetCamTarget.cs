using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCamTarget : MonoBehaviour {
    Cinemachine.CinemachineTargetGroup tGroup;
    GameObject[] playerList;

    Cinemachine.CinemachineTargetGroup.Target[] target = new Cinemachine.CinemachineTargetGroup.Target[4];

    [SerializeField]
    float weight;
    [SerializeField]
    float radius;
	[SerializeField]
	Vector3 offset;

	void Start () {
        tGroup = GetComponentInChildren<Cinemachine.CinemachineTargetGroup>();
        playerList = GameObject.FindGameObjectsWithTag("Player");

        for (int plIndex = 0; plIndex < playerList.Length; plIndex++) {
			// プレイヤーの前に疑似オブジェクトを作成してそれをカメラのターゲットとする
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.position = playerList[plIndex].transform.position + offset;
			cube.GetComponent<BoxCollider>().enabled = false;
			cube.GetComponent<MeshRenderer>().enabled = false;
			//cube.transform.parent = playerList[plIndex].transform;

			target[plIndex].target = cube.transform;
            target[plIndex].weight = weight;
            target[plIndex].radius = radius;
        }

        tGroup.m_Targets = target;
	}
}
