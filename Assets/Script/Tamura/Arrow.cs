using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {
    [SerializeField]
    int playerIndex;

    int inputDelay;             // 入力待ちフレーム量
    bool isCharacterSelected;   // キャラ選択中かどうか
    int cursorPos;              // カーソルの現在位置

    bool isSelectAnimating;     // 選択後のアニメーション中か
    float zAngle;               // アニメーション用のZ軸回転角

    CharacterSelectManager csManager;

    bool startGameflg;          // 開始決定フラグ

    public Vector3 cursorTransform;
    Vector3 cursorAnimateTransform;

    float animAngle;

    public bool canSelect;

    // 初期化
    void Awake() {
        inputDelay = 0;
        cursorPos = 0;
        csManager = FindObjectOfType<CharacterSelectManager>();

        animAngle = 0.0f/* + playerIndex * 1.0f*/;
    }

    void Update() {
        // キャラ未選択状態なら
        if (!isCharacterSelected) {
            MoveCursor();
            SelectCharacter();
            AnimateCursor();
        }
        // キャラ選択済みなら
        else {
            UnselectCharacter();
            StartGame();
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

                inputDelay = 20;
            }
        }
        else {
            inputDelay--;
        }
    }

    // キャラ選択
    void SelectCharacter() {
        if (XPad.Get.GetTrigger(XPad.KeyData.A, playerIndex) && canSelect) {
            csManager.SelectCharater(playerIndex, cursorPos);
        }
    }

    // キャラ選択を解除
    void UnselectCharacter() {
        if (XPad.Get.GetTrigger(XPad.KeyData.B, playerIndex)) {
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

    // カーソルのアニメーション(左右)
    void AnimateCursor() {
        float range = 0.5f;
        float xSpeed = 0.135f; /*+ Random.Range(-0.1f, 0.1f);*/

        cursorAnimateTransform = new Vector3(Mathf.Sin(animAngle) * range, 0.0f, 0.0f);

        animAngle += xSpeed;
        this.transform.position = cursorTransform + cursorAnimateTransform;
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

    public bool GetStartGameFlg() {
        return startGameflg;
    }
}
