using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour {

    public Bazooka Bazooka1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.A))
        {
            Bazooka1 = gameObject.AddComponent<Bazooka>() as Bazooka;
            Bazooka1.SetBazooka(new Vector3(-5.0f, 10.0f, 10.0f));
        }
	}
}
