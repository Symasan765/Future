using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class voiceUpdate : MonoBehaviour {
    Vector3 voiceVector;
    Rigidbody rb;
    public float voiceSpeed;

	void Start () {
        rb = GetComponent<Rigidbody>();

        voiceVector = this.transform.forward;
        rb.velocity = voiceVector * voiceSpeed;
	}

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("Obstacle")) {
            Destroy(this.gameObject);
        }
    }
}
