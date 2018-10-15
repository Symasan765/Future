using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTargetCollider : MonoBehaviour {

	void Start () {

	}

    void Update() {
        this.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0.0f);
    }
}
