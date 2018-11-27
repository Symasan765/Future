﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Arrow : MonoBehaviour {
    [SerializeField]
    int playerIndex;

    int inputDelay;             // 入力待ちフレーム量
    bool isCharacterSelected;   // キャラ選択中かどうか
    public int cursorPos;       // カーソルの現在位置
    public int charaNum;

    bool isSelectAnimating;     // 選択後のアニメーション中か
    float zAngle;               // アニメーション用のZ軸回転角

    CharacterSelectManager csManager;

    bool startGameflg;          // 開始決定フラグ

    public Vector3 cursorTransform;
    Vector3 cursorAnimateTransform;

    float animAngle;

    public bool canSelect;
    bool canInput;

    AudioSource seCymbal;

    [SerializeField]
    GameObject effectObj;
    ParticleSystem particle;

    // 初期化
    void Awake() {
        inputDelay = 0;
        cursorPos = playerIndex;
        csManager = FindObjectOfType<CharacterSelectManager>();
        seCymbal = GetComponent<AudioSource>();

        animAngle = 0.0f/* + playerIndex * 1.0f*/;

        //canInput = true;
        this.transform.position = new Vector3(-1500.0f, 1000.0f, 0.0f);
    }

    void FixedUpdate() {
        if (canInput) {
            if (!isCharacterSelected) {
                MoveCursor();
                AnimateCursor();

                SelectCharacter();
            }
            else {
                UnselectCharacter();
                StartGame();
            }
        }
        else {
            WaitingJoin();
        }
    }

    void MoveCursor() {
        // スティックの入力を取得
        Vector2 LeftStick = XPad.Get.GetLeftStick(playerIndex);

        // 前のフレームでスティック入力が無かった場合は
        if (inputDelay == 0) {
            // スティック入力があれば
            if (LeftStick.x != 0) {
                // 左入力
                if (LeftStick.x < 0) {
                    cursorPos--;

                    if (cursorPos < 0) {
                        cursorPos = csManager.unselectCharaList.Count - 1;
                    }
                }

                // 右入力
                if (LeftStick.x > 0) {
                    cursorPos++;

                    if (cursorPos >= csManager.unselectCharaList.Count) {
                        cursorPos = 0;
                    }
                }

                inputDelay = 10;
            }
        }
        else {
            inputDelay--;
        }
    }

    // キャラ選択
    void SelectCharacter() {
        if (XPad.Get.GetTrigger(XPad.KeyData.A, playerIndex) && canSelect) {
            charaNum = cursorPos;
            csManager.SelectCharater(playerIndex, cursorPos);
        }
    }

    // キャラ選択を解除
    void UnselectCharacter() {
        if (XPad.Get.GetTrigger(XPad.KeyData.B, playerIndex)) {
            Debug.Log("pIndex:" + playerIndex + " cPos:" + cursorPos);
            csManager.UnselectCharacter(playerIndex, cursorPos);
        }
    }

    // 開始待機
    void StartGame() {
        // 開始待ち状態なら
        if (csManager.GetIsWaiting()) {
            // スタートボタンが押されたらゲーム開始フラグをオンに
            if (XPad.Get.GetTrigger(XPad.KeyData.A, playerIndex)) {
                startGameflg = true;
            }
        }
    }

    // エントリー待機
    void WaitingJoin() {
        if (XPad.Get.GetTrigger(XPad.KeyData.START, playerIndex)) {
            canInput = true;
            XPad.Get.SetVibration(playerIndex, 0.3f, 0.3f, 0.5f);

            particle = Instantiate(effectObj, cursorTransform, Quaternion.identity).GetComponent<ParticleSystem>();
            particle.Play();
        }
    }

    // カーソルのアニメーション(左右)
    void AnimateCursor() {
        float range = 0.1f;
        float xSpeed = 0.25f;

        cursorAnimateTransform = new Vector3(Mathf.Sin(animAngle) * range, Mathf.Sin(animAngle) * -0.05f, 0.0f);

        animAngle += xSpeed;
        this.transform.position = cursorTransform + cursorAnimateTransform;
    }

    public bool GetCanInput() {
        return canInput;
    }

    public bool GetIsCharacterSelected() {
        return isCharacterSelected;
    }

    public void SetIsCharacterSelected(bool _flg) {
        isCharacterSelected = _flg;
    }

    public int GetCursorPos() {
        return cursorPos;
    }

    public void SetCursorPos(int _curPos) {
        cursorPos = _curPos;
    }

    public int GetPlayerIndex() {
        return playerIndex;
    }

    public bool GetStartGameFlg() {
        return startGameflg;
    }
}
