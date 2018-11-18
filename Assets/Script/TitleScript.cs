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
    void FixedUpdate()
    {
        
        AcceptInputFunc();
        
    }

    IEnumerator TitleScene()
    {
        while(true)
        {
            switch (SceneIndex)
            {
                case 0:
                    AllFadeImage.GetComponent<Fade>().SetFade((int)Fade.FadeOption.FADEIN, 0.2f, true,false);
                    if (AllFadeImage.GetComponent<Fade>().IsFadeDone())
                    {
                        AllFadeImage.GetComponent<Fade>().ResetAllSetteing();
                        SceneIndex++;
                        yield return new WaitForSeconds(1.0f);
                    }
                    break;
                case 1:
                    StartCoroutine(RoopTitleScene());
                    TitleLogo.transform.position += new Vector3(0.0f, 0.0f, 1.0f);
                    if (TitleLogo.transform.position.z >= 2.0f) 
                    {
                        SceneIndex++;
                        yield return new WaitForSeconds(0.30f);
                    }
                    break;
                case 2:
                    StartButton.SetActive(true);
                    break;
            }
            yield return null;
        }
     
    }
    IEnumerator GoNextScene()
    {
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene("CharacterSelect");
    }
    void AcceptInputFunc()
    {
        for (int i = 0; i < 4; i++)
        {
            if (XPad.Get.AnyoneTrigger(XPad.KeyData.A) == true) 
            {
                StartCoroutine(GoNextScene());
            }
        }
       
    }
    IEnumerator RoopTitleScene()
    {
        yield return new WaitForSeconds(10.0f);
        AllFadeImage.GetComponent<Fade>().SetFade((int)Fade.FadeOption.FADEOUT, 0.3f, true, true);
        while (true)
        {
            if (AllFadeImage.GetComponent<Fade>().IsFadeDone())
            {
                SceneManager.LoadScene("Title");
            }
            yield return null;
        }
        
    }
}
