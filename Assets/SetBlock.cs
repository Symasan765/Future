using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBlock : MonoBehaviour {
    [SerializeField]
    private GameObject blockObj;

    [SerializeField]
    private int blockRechargeCnt;
    private int rechargeCnt;

	// Use this for initialization
	void Start () {
        if (blockRechargeCnt != 0) {
            rechargeCnt = blockRechargeCnt;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (XPad.Get.GetTrigger(XPad.KeyData.B, 0)) {
            SpawnBlock();
        }

        if (rechargeCnt != 0) {
            rechargeCnt--;
        }
	}

    void SpawnBlock() {
        if (rechargeCnt <= 0) {
            Instantiate(blockObj, this.transform.position, this.transform.rotation);
            rechargeCnt = blockRechargeCnt;
        }
    }
}
