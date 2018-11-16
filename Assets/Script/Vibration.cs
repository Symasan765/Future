using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vibration : MonoBehaviour {
    public float m_MaxRange = 0.01f;
    bool VibrationTrigger = false;
    public Vector3 OldPos;
    public Vector3 Pos;
    
 
    void Update()
    {
        Vector3 pos = this.transform.position;
        
        if(VibrationTrigger==true)
        {
            this.transform.position = OldPos;
            pos.x += Random.Range(-m_MaxRange, m_MaxRange);
            pos.y += Random.Range(-m_MaxRange, m_MaxRange);
            pos.z += Random.Range(-m_MaxRange, m_MaxRange);
        }
        
        this.transform.position = pos;
    }
    public void VibeOn()
    {
        OldPos = this.transform.position;
        VibrationTrigger = true;
    }
    public void VibeOff()
    {
        VibrationTrigger = false;
        this.transform.position = OldPos;
    }
}
