using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChanger : SingletonMonoBehaviour<SceneChanger> {
    // timescaleを0にしてテクスチャに書き込み
    // 書き込んだテクスチャを上にフレームアウトし、下から中間コマを出す
    // 現在のシーン破棄→次のシーン読み込み(ここで再キャプチャ)
    // 次のシーン読み込みが完了したら下から次のシーンコマを出す
    // シーンコマを出し終わったらtimescaleを1に戻す

    // 問題点
    // シーン開始演出などがある場合は事前にシーン内で初期位置を確定しておく必要がある
    // キャラクターの位置なども同じ
    // テクスチャサイズが1920x1080と大きく、描画も二重になるので重そう
    // 他のUI要素と競合する
    // 背景色が他のカメラと競合しないか？

    // 要望
    // Updateで進行してほしくない処理をFixedUpdateにしてほしい
    // 呼び出しはFindObjectOfType<SceneChanger>()した後ChangeScene関数で呼び出してほしい

    [SerializeField]
    RawImage sceneImage;

    [SerializeField]
    Image cutsceneImage;

    string nextSceneName;
    Scene nowScene;

    [SerializeField]
    bool changeSceneFlg;
    [SerializeField]
    bool isMoving = false;

    [SerializeField]
    bool isSceneUIMoveMiddle = true;
    [SerializeField]
    bool isSceneUIMoveEnd = true;

    [SerializeField]
    bool isCutsceneUIMoveMiddle = true;
    [SerializeField]
    bool isCutsceneUIMoveEnd = true;

    int coutn = 0;

    void Awake() {
        var sceneChangers = FindObjectsOfType<SceneChanger>();
        if (sceneChangers.Length >= 2) {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Update() {
        //coutn++;
        //if (coutn % 100 == 0) {
            if (isMoving) {
                if (!isSceneUIMoveEnd) {
                    if (!isSceneUIMoveMiddle) {
                        Debug.Log("MSUI1");
                        MoveUI_Scene1();
                    }
                    else {
                        Debug.Log("MSUI22");
                        MoveUI_Scene2();
                    }

                    if (isSceneUIMoveEnd) {
                        // 前のシーン表示UIを画面上端まで送ったら
                        sceneImage.transform.localPosition = new Vector3(0.0f, -1080.0f, 0.0f);

                        // 次のシーンを読み込み
                        SceneManager.LoadSceneAsync(nextSceneName);
                        nowScene = SceneManager.GetSceneByName(nextSceneName);
                        Debug.Log("NEXT SCENE");
                    }
                }

                if (nowScene.isLoaded) {
                    if (!isCutsceneUIMoveEnd) {
                        if (!isCutsceneUIMoveMiddle) {
                            MoveUI_Cutscene1();
                            Debug.Log("MCUI1");
                        }
                        else {
                            MoveUI_Cutscene2();
                            Debug.Log("MCUI2");
                        }

                        if (isCutsceneUIMoveEnd) {
                            isMoving = false;
                        }
                    }
                }
            }
            else {
                if (isCutsceneUIMoveEnd) {
                    // 次のシーン表示UIを画面上に出し終わったら
                    // RawImageを非表示に
                    sceneImage.enabled = false;
                    MoveReset();

                    // ゲームの進行を再開
                    Time.timeScale = 1.0f;
                }
            }
        //}
    }

    public void ChangeScene(string _sceneName) {
        sceneImage.enabled = true;

        // ゲームの進行を停止
        Time.timeScale = 0.0f;

        changeSceneFlg = true;
        isMoving = true;
        isSceneUIMoveMiddle = false;
        isSceneUIMoveEnd = false;
        isCutsceneUIMoveMiddle = false;
        isCutsceneUIMoveEnd = false;

        nextSceneName = _sceneName;
    }

    bool MoveSceneUI(float _PosScene, float _PosCutscene) {
        sceneImage.transform.localPosition = Vector3.Lerp(sceneImage.transform.localPosition, new Vector3(0.0f, _PosScene, 0.0f), 0.2f);
        cutsceneImage.transform.localPosition = Vector3.Lerp(cutsceneImage.transform.localPosition, new Vector3(0.0f, _PosCutscene, 0.0f), 0.2f);

        if (_PosScene - 1.0f < sceneImage.transform.localPosition.y && sceneImage.transform.localPosition.y < _PosScene + 1.0f) {
            sceneImage.transform.localPosition = new Vector3(0.0f, _PosScene, 0.0f);
            cutsceneImage.transform.localPosition = new Vector3(0.0f, _PosCutscene, 0.0f);
            return true;
        }
        else {
            return false;
        }
    }

    void MoveUI_Scene1() {
        isSceneUIMoveMiddle = MoveSceneUI(300.0f, -780.0f);
    }
    void MoveUI_Scene2() {
        isSceneUIMoveEnd = MoveSceneUI(1080.0f, 0.0f);
    }

    void MoveUI_Cutscene1() {
        isCutsceneUIMoveMiddle = MoveSceneUI(-780.0f, 300.0f);
    }
    void MoveUI_Cutscene2() {
        isCutsceneUIMoveEnd = MoveSceneUI(0.0f, 1080.0f);
    }

    void MoveReset() {
        sceneImage.transform.localPosition = Vector3.zero;
        cutsceneImage.transform.localPosition = new Vector3(0.0f, 1080.0f, 0.0f);
    }
}
