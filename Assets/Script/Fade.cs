using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// フェード用スクリプト。
/// 使い方：
/// １．フェードする画像オブジェクトにこのスクリプトをつける。
/// 注意：かならず画像はImageであること。
/// ２．フェードを使うシーンのスクリプトで、フェードさせる画像のオブジェクトを確保する。
/// (FindObjectなどでオブジェクトを探す）
/// ３．その画像オブジェクト.GetComponent<Image>().SetFadeでフェードをセットアップする。
/// ４．フェードの完了後、好きな作業をする。
/// 
/// </summary>
public class Fade : MonoBehaviour {
    public enum FadeOption { FADEIN,FADEOUT};               //フェードの種類を定義
    GameObject thisObject;                               //このスクリプトがついているゲームオブジェクト
    Image thisImage;                                     //そのオブジェクトからImageコンポネントを持ってきて確保する用途
    bool DoFadeIn = false;                                //フェードイン中
    bool DoFadeOut = false;                               //フェードアウト中
    bool FadeDone = false;                               //フェードが完了したかどうか
    bool FadeAll = false;                                 //このオブジェクトが画面全体のフェードのものかどうか
    bool NowFadeing = false;
    Color FadeSpeed = new Color(0.0f, 0.0f, 0.0f, 0.01f);  //フェードする速さ

    Vector3 initPos;
	void Start ()
    {
        thisObject = this.gameObject;                      //ゲームオブジェクト確保
        thisImage = thisObject.GetComponent<Image>();      //Imageのコンポネント確保
        initPos = GetComponent<RectTransform>().localScale;

    }
	
	void Update ()
    {
        //GetComponent<RectTransform>().localScale = initPos * ((Mathf.Sin(Time.time) + 1.0f) /2.0f + 0.5f);

        //画面全体のフェードであれば
        if (FadeAll)
        {
                //フェードイン処理
                if (DoFadeIn == true)
                {
                    FadeDone = false;
                    NowFadeing = true;
                    thisImage.color -= FadeSpeed;
                    if (thisImage.color.a <= 0.0f)
                    {
                        FadeDone = true;
                        DoFadeIn = false;
                        NowFadeing = false;
                    }
                }

                //フェードアウト処理
                if (DoFadeOut == true)
                {
                    FadeDone = false;
                    NowFadeing = true;
                    thisImage.color += FadeSpeed;
                    if (thisImage.color.a >= 1.0f)
                    {
                        FadeDone = true;
                        DoFadeOut = false;
                        NowFadeing = false;
                    }
                }
            
        }

        //画面全体のフェードではなければ
        else
        {
                //フェードイン処理
                if (DoFadeIn == true)
                {
                    FadeDone = false;
                    NowFadeing = true;
                    thisImage.color += FadeSpeed;
                    if (thisImage.color.a >= 1.0f)
                    {
                        FadeDone = true;
                        DoFadeIn = false;
                        NowFadeing = false;
                    }
                }
                //フェードアウト処理
                if (DoFadeOut == true)
                {
                    FadeDone = false;
                    NowFadeing = true;
                    thisImage.color -= FadeSpeed;
                    if (thisImage.color.a <= 0.0f)
                    {
                        FadeDone = true;
                        DoFadeOut = false;
                        NowFadeing = false;
                    }
                }
                
        }		
    }

    /// <summary>
    /// フェードの種類を指定してセットする
    /// </summary>
    /// <param name="Option">フェードイン・フェードアウト</param>
    /// <param name="isThisImageAll">画面全体のフェードかどうか</param>
    public void SetFade(int Option,float Speed, bool isThisImageAll)
    {
        FadeSpeed = new Color(0.0f, 0.0f, 0.0f, Speed);
        switch (Option)
        {
            case (int)FadeOption.FADEIN:
                DoFadeIn = true;
                break;
            case (int)FadeOption.FADEOUT:
                DoFadeOut = true;
                break;
        }
        if (isThisImageAll == true)
        {
            FadeAll = true;
        }
    }
    
    /// <summary>
    /// フェードが完了したかどうか
    /// </summary>
    /// <returns></returns>
    public bool IsFadeDone()
    {
        return FadeDone;
    }

    public void SetFadeFlag(bool temp)
    {
        FadeDone = temp;
    }
}
