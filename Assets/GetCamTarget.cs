using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCamTarget : MonoBehaviour {
    Cinemachine.CinemachineTargetGroup tGroup;
    GameObject[] playerList;
	GameObject[] cubeList;

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
		cubeList = new GameObject[playerList.Length];


		for (int plIndex = 0; plIndex < playerList.Length; plIndex++) {
			// プレイヤーの前に疑似オブジェクトを作成してそれをカメラのターゲットとする
			cubeList[plIndex] = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cubeList[plIndex].transform.position = playerList[plIndex].transform.position + offset;
			cubeList[plIndex].GetComponent<BoxCollider>().enabled = false;
			cubeList[plIndex].GetComponent<MeshRenderer>().enabled = false;

			target[plIndex].target = cubeList[plIndex].transform;
            target[plIndex].weight = weight;
            target[plIndex].radius = radius;
        }

        tGroup.m_Targets = target;
	}

	private void Update()
	{
		for (int plIndex = 0; plIndex < playerList.Length; plIndex++)
		{
			cubeList[plIndex].transform.position = playerList[plIndex].transform.position + offset;
		}
	}
}
