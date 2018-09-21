using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour {

    public Bazooka Bazooka1;
    public GameObject BazookaPrefab;
	// Use this for initialization
	void Start ()
    {
        if (BazookaPrefab == null)
        {   
            BazookaPrefab = Resources.Load("Prefab/Lee/Bazooka") as GameObject;
        }
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.A))
        {
            Instantiate(BazookaPrefab);
            BazookaPrefab.GetComponent<Bazooka>().SetBazooka(new Vector3(-5.0f, 10.0f, 10.0f)); 
        }
	}
}
