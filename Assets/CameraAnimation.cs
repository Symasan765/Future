using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimation : MonoBehaviour {
    [SerializeField]
    Camera renderCamera;
    [SerializeField]
    Arrow arrow;

    [SerializeField]
    Vector3 beforeCamPos;
    [SerializeField]
    Vector3 afterCamPos;

    Quaternion afterCamRot;
    [SerializeField]
    Vector3 afterAngle;

    Vector3 nextPos;
    [SerializeField]
    Vector3 offset;
    Quaternion nextRot;

    bool isAnimating;
    bool isReverse;

	[SerializeField]
	private float AnimationSpeed = 0.5f;

    void Start() {
        nextPos = beforeCamPos;
        nextRot = Quaternion.identity;

        afterCamRot = Quaternion.Euler(afterAngle);
        renderCamera.transform.position = beforeCamPos;
    }
	
	void Update () {
        CheckIsSelected();
        UpdateCamPos();
	}

    void CheckIsSelected() {
        if (arrow.GetIsCharacterSelected()) {
            nextPos = afterCamPos;
            nextRot = afterCamRot;
            isReverse = false;
        }
        else {
            nextPos = beforeCamPos;
            nextRot = Quaternion.identity;
            isReverse = true;
        }
    }

    void UpdateCamPos() {
        if (isAnimating) {
			renderCamera.transform.position = Vector3.Lerp(renderCamera.transform.position, nextPos, AnimationSpeed);
			renderCamera.transform.rotation = Quaternion.Lerp(renderCamera.transform.rotation, nextRot, AnimationSpeed);
            if (renderCamera.transform.position == nextPos && renderCamera.transform.rotation == nextRot) {
                isAnimating = false;
            }
        }
    }

    public void StartCameraAnimation() {
        isAnimating = true;
    }

    public void SetPosition(float _xPos) {
        beforeCamPos = new Vector3(_xPos, beforeCamPos.y, beforeCamPos.z);
        afterCamPos = new Vector3(_xPos + offset.x, offset.y, offset.z);
    }
}
