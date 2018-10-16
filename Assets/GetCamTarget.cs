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

	void Start () {
        tGroup = GetComponentInChildren<Cinemachine.CinemachineTargetGroup>();
        playerList = GameObject.FindGameObjectsWithTag("Player");

        for (int plIndex = 0; plIndex < playerList.Length; plIndex++) {
            target[plIndex].target = playerList[plIndex].transform;
            target[plIndex].weight = weight;
            target[plIndex].radius = radius;
        }

        tGroup.m_Targets = target;
	}
}
