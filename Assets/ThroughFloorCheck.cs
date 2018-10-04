using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThroughFloorCheck : MonoBehaviour {
    CapsuleCollider collider;

    int ignoreLayer;
    int hitLayer;
    int throughFloorLayer;

	void Awake () {
        collider = GetComponent<CapsuleCollider>();

        ignoreLayer = LayerMask.NameToLayer("IgnoreHit");
        hitLayer = LayerMask.NameToLayer("Hit");
        throughFloorLayer = LayerMask.NameToLayer("ThroughFloor");
        Physics.IgnoreLayerCollision(ignoreLayer, throughFloorLayer);
	}

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("ThroughFloor")) {
            Debug.Log("Enter");
            this.gameObject.layer = ignoreLayer;
            Physics.IgnoreLayerCollision(ignoreLayer, throughFloorLayer);
        }
        else {
            this.gameObject.layer = hitLayer;
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
            //collider.isTrigger = false;
            //this.gameObject.layer = hitLayer;
        }
    }
}
