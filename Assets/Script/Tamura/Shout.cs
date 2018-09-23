using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shout : MonoBehaviour {
    [SerializeField]
    GameObject target;

    [SerializeField]
    GameObject voicePrefab;

    [SerializeField]
    Transform shoutPos;

    [SerializeField]
    int shoutInterval;
    int shoutIntervalCount;

    bool canShout;

    void Start() {
        shoutIntervalCount = shoutInterval;
        canShout = false;
    }
	
	void Update () {
        if (shoutIntervalCount <= 0) {
            canShout = true;
        }
        else {
            shoutIntervalCount--;
        }
	}

    void FixedUpdate() {
        Vector3 targetPos = target.transform.position;
        targetPos.y = this.transform.position.y;
        this.transform.LookAt(targetPos);
        ShoutTarget();
    }

    void ShoutTarget() {
        if (!canShout) {
            return;
        }
        else {
            GameObject voice;
            voice = Instantiate(voicePrefab, shoutPos.position, shoutPos.rotation);

            shoutIntervalCount = shoutInterval;
            canShout = false;
        }
    }
}
