using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectDrop : MonoBehaviour {
    [SerializeField]
    float dropTime;

    [SerializeField]
    Vector3 DropStartPos;
    [SerializeField]
    Vector3 DropEndPos;

    [SerializeField]
    Arrow arrow;

    float nowDropTime;

    void Start() {
        nowDropTime = 0.0f;
    }

	void FixedUpdate () {
        if (arrow.GetCanInput()) {
            this.transform.position = Vector3.Lerp(DropStartPos, DropEndPos, nowDropTime / dropTime);
            nowDropTime += Time.fixedDeltaTime;
        }
	}
}
