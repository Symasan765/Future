using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour {
    public Camera CameraObject;     //カメラオブジェクト

    public Vector3 MovingSpeed;     //カメラが動く速度。Unityのインスペクターで数値を設定

    public enum SceneList
    {
        Title = 0,
        CharactorSelect,
        GameMain
    };
	
	void Start ()
    {
		
	}
	
	
	void Update ()
    {
        CameraObject.transform.position += MovingSpeed;
        for (int i = 0; i < 4; i++)
        {
            if (XPad.Get.GetTrigger(XPad.KeyData.A, i)||Input.GetKeyDown(KeyCode.A))
            {
                SceneManager.LoadScene((int)SceneList.CharactorSelect);
                break;
            }
        }
	}
}
