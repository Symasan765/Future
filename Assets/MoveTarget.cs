using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTarget : MonoBehaviour {
	
	public void ChangeZPos (float zAcc) {
        this.transform.localPosition = new Vector3(0.0f, 0.0f, zAcc);
	}
}
