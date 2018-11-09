using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScript : MonoBehaviour {
    public Camera CameraObject;     //カメラオブジェクト

    public Vector3 MovingSpeed;     //カメラが動く速度。Unityのインスペクターで数値を設定
    public GameObject AllFadeImage; //画面全体のフェード用画像    
    public GameObject TitleLogo;    //ロゴ画像
    public GameObject StartButton;  //スタートボタン画像
    public GameObject StartFade;    //スタートボタンを点滅させるための透明画像

    bool PushedStart = false;

    //start : -271 title : 180
    [SerializeField]
    int SceneIndex = 0;

    public enum SceneList
    {
        Title = 0,
        CharactorSelect,
        GameMain
    };

    void Start()
    {
        StartCoroutine(TitleScene());
    }
    void Update ()
    {
        
        AcceptInputFunc();
        FlashingButton();
        
    }

    IEnumerator TitleScene()
    {
        while(true)
        {
            switch (SceneIndex)
            {
                case 0:
                    AllFadeImage.GetComponent<Fade>().SetFade((int)Fade.FadeOption.FADEIN, 0.02f, true,false);
                    if (AllFadeImage.GetComponent<Fade>().IsFadeDone())
                    {
                        SceneIndex++;
                        yield return new WaitForSeconds(1.0f);
                    }
                    break;
                case 1:
                    if (TitleLogo.GetComponent<RectTransform>().localPosition.y<180.0f)
                    {
                        TitleLogo.GetComponent<RectTransform>().localPosition += new Vector3(0.0f, 30.0f,0.0f);
                    }
                    else
                    {
                        SceneIndex++;
                        TitleLogo.GetComponent<Vibration>().VibrationTrigger = true;
                        yield return new WaitForSeconds(0.15f);
                    }
                    break;
                case 2:
                    TitleLogo.GetComponent<Vibration>().VibrationTrigger = false;
                    if (StartButton.GetComponent<RectTransform>().localPosition.y < -270.0f)
                    {
                        StartButton.GetComponent<RectTransform>().localPosition += new Vector3(0.0f, 30.0f, 0.0f);
                        
                    }
                    break;
            }
            yield return null;
        }
     
    }
    IEnumerator GoNextScene()
    {
        yield return new WaitForSeconds(2.0f);
        Debug.Log("GoNextScene");
    }
    void AcceptInputFunc()
    {
        for (int i = 0; i < 4; i++)
        {
            if (XPad.Get.GetTrigger(XPad.KeyData.A, i) || Input.GetKeyDown(KeyCode.A))
            {
                //SceneManager.LoadScene("CharacterSelect");
                PushedStart = true;
            }
        }
       
    }
    void FlashingButton()
    {
        Fade Start = StartFade.GetComponent<Fade>();
        
        if(PushedStart==true)
        {
            Start.SetFadeSpeed(0.1f);
            StartCoroutine(GoNextScene());
            
        }
        if(PushedStart==false)
        {
            Start.SetFade((int)Fade.FadeOption.FLASH, 0.03f, false,false);
        }
    }

}
