using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisabeleText : MonoBehaviour {
    [SerializeField]
    float scaleTime;

    [SerializeField]
    Arrow arrow;

    float nowScaleTime;

    Vector3 startScale = new Vector3(0.09f, 0.09f, 1.0f);
    [SerializeField]
    Vector3 basePosition;

    float animAngle;

    void Start() {
        nowScaleTime = 0.0f;
        animAngle = 0.0f;
        basePosition = this.transform.localPosition;
    }
	
	void FixedUpdate () {
        if (arrow.GetCanInput()) {
            this.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, nowScaleTime / scaleTime);
            nowScaleTime += Time.fixedDeltaTime;
        }
        else {
            float range = 0.025f;
            float xSpeed = 0.125f;

            Vector3 animTransform = new Vector3(0.0f, Mathf.Sin(animAngle) * range, 0.0f);

            animAngle += xSpeed;
            this.transform.localPosition = basePosition + animTransform;
        }
	}
}
