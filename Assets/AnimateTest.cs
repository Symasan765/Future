using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateTest : MonoBehaviour {

    private Animator animator;

    void Start() {
        animator = GetComponent<Animator>();
    }
    void Update() {
        if (Input.GetButtonDown("Fire1")) {
            animator.SetBool("isSelected", true);
        }
    }

    void Step() {

    }
}
