using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vibration : MonoBehaviour {
    public float m_MaxRange = 0.01f;
    public bool VibrationTrigger = false;
    public Vector3 OldPos;

    // Update is called once per frame
    private void Start()
    {
        OldPos = this.transform.position;
    }
    void Update()
    {
        Vector3 pos = this.transform.position;
        
        if(VibrationTrigger==true)
        {

            pos.x += Random.Range(-m_MaxRange, m_MaxRange);
            pos.y += Random.Range(-m_MaxRange, m_MaxRange);
            pos.z += Random.Range(-m_MaxRange, m_MaxRange);

            this.transform.position = pos;
        }
        else
        {
            pos = OldPos;
        }
    }
}
