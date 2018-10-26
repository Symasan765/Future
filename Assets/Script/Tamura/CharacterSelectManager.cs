using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour {
    [SerializeField]
    // カーソルの位置用リスト
    List<ArrowPosList> portraitList = new List<ArrowPosList>();

    [SerializeField]
    // カーソル用リスト
    List<Arrow> arrowList = new List<Arrow>();

    [SerializeField]
    // 履歴書用リスト
    List<GameObject> sheetList = new List<GameObject>();

    public List<int> unselectCharaList = new List<int>() { 0, 1, 2, 3 };

    [SerializeField]
    Canvas TextCanvas;

    // 待機中か(全プレイヤーがキャラを選択したか)どうか
    [SerializeField]
    bool isWaiting = false;

	void Update () {
        UpdateCursor();
        UpdateCharacterSelectStatus();
        CheckCharacterSelected();

        if (isWaiting) {
            WaitingForStart();
        }
        //履歴書を上に飛ばしていく
        //sheetList[0].transform.position = sheetList[0].transform.position + sheetList[0].transform.up;
	}

    // カーソルの位置を更新
    void UpdateCursor() {
        for (int cursorIndex = 0; cursorIndex < 4; cursorIndex++) {
            Arrow aElement = arrowList[cursorIndex];

            if (!aElement.GetIsCharacterSelected()) {
                // n番のカーソルの位置を履歴書の顔写真の横に移動
                aElement.transform.position = portraitList[unselectCharaList[aElement.GetCursorPos()]].arrowPosList[cursorIndex].transform.position;
            }
        }
    }

    // キャラクターの選択状態を更新
    void UpdateCharacterSelectStatus() {
        // unselectリストからRemoveして、それが重複なく成功したらArrow側にそれを通知する
        // 重複したらどうすんの
    }

    // 全員がキャラ選択したか確認
    void CheckCharacterSelected() {
        for (int cursorIndex = 0; cursorIndex < 4;) {
            // 選択済みかどうかを確認
            if (arrowList[cursorIndex].GetIsCharacterSelected()) {
                cursorIndex++;

                // 全員キャラ選択済みなら
                if (cursorIndex == 4) {
                    // ここで全キャラ準備完了のフラグを立てる
                    isWaiting = true;
                    TextCanvas.enabled = true;
                    Debug.Log("とりあえず開始待ち");
                }
            }
            // 誰か選択済みでないならループを抜ける
            else {
                cursorIndex = 999;
                // ここで全キャラ準備完了のフラグを折る
                isWaiting = false;
                TextCanvas.enabled = false;
            }
        }
    }

    // 全員キャラ選択済み状態での待機
    void WaitingForStart() {
        // スタートボタンの入力を待つ
        // その間、PushStart的なUIを表示しておく
        for (int cursorIndex = 0; cursorIndex < 4; ) {
            if (arrowList[cursorIndex].GetStartGameFlg()) {
                SceneManager.LoadScene("MainGameScene");
                cursorIndex = 999;
            }
            cursorIndex++;
        }
    }

    public bool GetIsWaiting() {
        return isWaiting;
    }

    // 下2つはいずれ放棄、UpdateCharacterselectStatusに統合？
    public void SelectCharater(int _playerIndex, int _cursorPos) {
        unselectCharaList.RemoveAt(_cursorPos);
    }

    public void UnselectCharacter(int _playerIndex, int _cursorPos) {
        unselectCharaList.Add(_cursorPos);
        unselectCharaList.Sort();
    }
}
