using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThroughFloorCheck : MonoBehaviour {
    CapsuleCollider collider;

	void Awake () {
        collider = GetComponent<CapsuleCollider>();
	}

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("ThroughFloor")) {
            Debug.Log("Enter");
            collider.isTrigger = true;
        }
    }

    void OnCollisionStay(Collision other) {
        if (other.gameObject.CompareTag("ThroughFloor")) {
            Debug.Log("Stay");
        }
    }

    void OnCollisionExit(Collision other) {
        if (other.gameObject.CompareTag("ThroughFloor")) {
            Debug.Log("Exit");
            collider.isTrigger = false;
        }
    }
}
