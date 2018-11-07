using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour {
    static public int[] SelectedCharacters = { -1, -1, -1, -1 };

    // キャラ選択を設定
    public static void SetCharacter(int _playerIndex, int _charaIndex) {
        SelectedCharacters[_playerIndex] = _charaIndex;
    }

    // キャラ選択を戻す
    public static void ResetCharacter(int _playerIndex) {
        SelectedCharacters[_playerIndex] = -1;
    }

    // キャラ選択を全員分初期化
    public static void ResetCharacters() {
        for (int index = 0; index < SelectedCharacters.Length; index++) {
            SelectedCharacters[index] = -1;
        }
    }
}
