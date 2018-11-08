using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScript : MonoBehaviour {
    public Camera CameraObject;     //カメラオブジェクト

    public Vector3 MovingSpeed;     //カメラが動く速度。Unityのインスペクターで数値を設定
    public GameObject FadeImage;
    public GameObject FadeImage2;
    public enum SceneList
    {
        Title = 0,
        CharactorSelect,
        GameMain
    };
	
	void Update ()
    {
        CameraObject.transform.position += MovingSpeed;
        for (int i = 0; i < 4; i++)
        {
            //if (XPad.Get.GetTrigger(XPad.KeyData.A, i)||Input.GetKeyDown(KeyCode.A))
            //{
            //    FadeImage.GetComponent<Fade>().SetFade((int)Fade.FadeOption.FADEOUT,0.1f,true);
            //}
        }
		if (XPad.Get.AnyoneTrigger(XPad.KeyData.A))
		{
			FadeImage.GetComponent<Fade>().SetFade((int)Fade.FadeOption.FADEOUT, 0.1f, true);
		}

        if (Input.GetKeyDown(KeyCode.A))
        {
            FadeImage.GetComponent<Fade>().SetFade((int)Fade.FadeOption.FADEOUT, 0.1f, true);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            FadeImage.GetComponent<Fade>().SetFade((int)Fade.FadeOption.FADEIN,0.1f, true);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            FadeImage2.GetComponent<Fade>().SetFade((int)Fade.FadeOption.FADEOUT,0.01f, false);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            FadeImage2.GetComponent<Fade>().SetFade((int)Fade.FadeOption.FADEIN,0.01f, false);
        }
        if(FadeImage.GetComponent<Fade>().IsFadeDone()==true)
        {
			SceneManager.LoadScene("CharacterSelect");

			//SceneManager.LoadScene((int)SceneList.CharactorSelect);
        }
    }
}
