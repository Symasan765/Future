﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour {
    struct changeData {
        public int _playerIndex;
        public int _cursorPos;
    }

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

    [SerializeField]
    List<Animator> animatorList = new List<Animator>();

    [SerializeField]
    List<Camera> renderCamList = new List<Camera>();

    // 変更するキャラ選択データの格納先
    List<changeData> selectDataList = new List<changeData>();
    List<changeData> unselectDataList = new List<changeData>();

    // 未選択キャラのリストをこのフレームでいじるかどうか
    bool changeUnselectListFlg;

    bool sceneChangeFlg;
    SceneChanger sc;

    void Start() {
        //SoundManager.Get.PlayBGM("はりきっちゃう時のテーマ", true);
    }

	void Update () {
        UpdateCursor();
        UpdateCharacterSelectStatus();
        CheckCharacterSelected();

        if (isWaiting) {
            WaitingForStart();
        }
        //履歴書を上に飛ばしていく
        //sheetList[0].transform.position = sheetList[0].transform.position + sheetList[0].transform.up;

        sc = FindObjectOfType<SceneChanger>();
	}

    // カーソルの位置を更新
    void UpdateCursor() {
        for (int cursorIndex = 0; cursorIndex < 4; cursorIndex++) {
            Arrow aElement = arrowList[cursorIndex];

            if (!aElement.GetIsCharacterSelected()) {
                // n番のカーソルの位置を履歴書の顔写真の横に移動
                aElement.cursorTransform = Vector3.Lerp(aElement.cursorTransform, portraitList[unselectCharaList[aElement.GetCursorPos()]].arrowPosList[cursorIndex].transform.position, 0.5f);
                renderCamList[cursorIndex].transform.position = new Vector3(-60.0f + 10.0f * unselectCharaList[aElement.GetCursorPos()], 0.0f, -1.0f);
                if (aElement.cursorTransform == portraitList[unselectCharaList[aElement.GetCursorPos()]].arrowPosList[cursorIndex].transform.position) {
                    aElement.canSelect = true;
                }
                else{
                    aElement.canSelect = false;
                }
            }
        }
    }

    // キャラクターの選択状態を更新
    void UpdateCharacterSelectStatus() {
        if (changeUnselectListFlg) {
            // unselectリストからRemoveして、それが重複なく成功したらArrow側にそれを通知する
            // 重複したらIndexの小さい順にキャラを割り振り、Indexの大きい人は未選択ってことにする
            if (selectDataList.Count != 0) {
                for (int i = 0; i < selectDataList.Count; i++) {

                }
            }

            if (unselectDataList.Count != 0) {
                for (int i = 0; i < unselectDataList.Count; i++) {

                }
            }
            // unselectリストをいじり終わった後にフラグを戻す
            changeUnselectListFlg = false;
        }
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

                    for (int i = 0; i < 4; i++) {
                        Debug.Log("プレイヤー番号：" + i + "キャラ番号：" + CharacterManager.SelectedCharacters[i]);
                    }
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
                if (sceneChangeFlg == false) {
                    sc.ChangeScene("Alpha");
                    sceneChangeFlg = true;
                }
                cursorIndex = 999;
            }
            cursorIndex++;
        }
    }

    public bool GetIsWaiting() {
        return isWaiting;
    }

    // 下2つはいずれ放棄、UpdateCharacterselectStatusに統合？　これら2つの処理は同フレームで順番に行いたい
    // playerIndexとcursorPosを持つ構造体にいじり情報を格納する？
    public void SelectCharater(int _playerIndex, int _cursorPos) {
        changeUnselectListFlg = true;
        //--------------------------------------------------------------------

        // 操作が同フレームで重複しなかった場合の処理
        CharacterManager.SetCharacter(_playerIndex, unselectCharaList[_cursorPos]);
        animatorList[unselectCharaList[_cursorPos]].SetBool("isSelected", true);

        unselectCharaList.RemoveAt(_cursorPos);

        // キャラ選択フラグをオンに
        arrowList[_playerIndex].SetIsCharacterSelected(true);
    }

    public void UnselectCharacter(int _playerIndex, int _cursorPos) {
        changeUnselectListFlg = true;
        //--------------------------------------------------------------------

        // 操作が同フレームで重複しても大丈夫なように未選択リストをいじるタイミングは統一すること
        unselectCharaList.Add(CharacterManager.SelectedCharacters[_playerIndex]);
        unselectCharaList.Sort();

        CharacterManager.ResetCharacter(_playerIndex);
        animatorList[unselectCharaList[_cursorPos]].SetBool("isSelected", false);

        // キャラ選択フラグをオフに
        arrowList[_playerIndex].SetIsCharacterSelected(false);
    }
}
