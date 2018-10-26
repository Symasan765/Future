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


    // 初期化
    void Awake() {
        inputDelay = 0;
        cursorPos = 0;
        csManager = FindObjectOfType<CharacterSelectManager>();
    }

    void Update() {
        if (!isCharacterSelected) {
            MoveCursor();
            SelectCharacter();
        }
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

                inputDelay = 10;
            }
        }
        else {
            inputDelay--;
        }
    }

    // キャラ選択
    void SelectCharacter() {
        if (XPad.Get.GetTrigger(XPad.KeyData.A, playerIndex)) {
            CharacterManager.SetCharacter(playerIndex, cursorPos);
            csManager.SelectCharater(playerIndex, cursorPos);
            isCharacterSelected = true;
        }
    }

    // キャラ選択を解除
    void UnselectCharacter() {
        if (XPad.Get.GetTrigger(XPad.KeyData.B, playerIndex)) {
            CharacterManager.ResetCharacter(playerIndex);
            csManager.UnselectCharacter(playerIndex, cursorPos);
            isCharacterSelected = false;
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

    public bool GetIsCharacterSelected() {
        return isCharacterSelected;
    }

    public int GetCursorPos() {
        return cursorPos;
    }

    public bool GetStartGameFlg() {
        return startGameflg;
    }
}
