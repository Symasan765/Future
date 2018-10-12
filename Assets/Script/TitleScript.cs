using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScript : MonoBehaviour {
    public Camera CameraObject;

    public Vector3 MovingSpeed;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        CameraObject.transform.position += MovingSpeed;
	}
}
