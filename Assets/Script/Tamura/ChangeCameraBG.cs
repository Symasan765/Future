using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCameraBG : MonoBehaviour {
    [SerializeField]
    Color BGColor;

    [SerializeField]
    float colorChangeSpeed = 0.05f;

    [SerializeField]
    Arrow cursorArrow;

    Camera renderCamera;

	// Use this for initialization
	void Start () {
        renderCamera = GetComponent<Camera>();
        renderCamera.backgroundColor = Color.gray;
	}

    void Update() {
        if (cursorArrow.GetIsCharacterSelected()) {
            ChangeToBGColor();
        }
        else {
            ChangeToGrayColor();
        }
    }

    void ChangeToBGColor() {
        renderCamera.backgroundColor = Color.Lerp(renderCamera.backgroundColor, BGColor, colorChangeSpeed);
    }

    void ChangeToGrayColor() {
        renderCamera.backgroundColor = Color.Lerp(renderCamera.backgroundColor, Color.gray, colorChangeSpeed);
    }
}
