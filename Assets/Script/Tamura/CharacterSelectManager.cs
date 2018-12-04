using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour {
    class changeData {
        public int _playerIndex;
        public int _cursorPos;
    }

    [System.SerializableAttribute]
    class animatorList {
        public List<Animator> animList = new List<Animator>();
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
    List<animatorList> allAnimatorList = new List<animatorList>();

    [SerializeField]
    List<Camera> renderCamList = new List<Camera>();

    [SerializeField]
    List<CameraAnimation> camAnimList = new List<CameraAnimation>();

    [SerializeField]
    List<RawImage> portlaitImgList = new List<RawImage>();

    [SerializeField]
    List<Image> charaSheetImgList = new List<Image>();

    // 変更するキャラ選択データの格納先
    List<changeData> selectDataList = new List<changeData>();
    List<changeData> unselectDataList = new List<changeData>();

    // 未選択キャラのリストをこのフレームでいじるかどうか
    bool changeUnselectListFlg;

    bool sceneChangeFlg;
    SceneChanger sc;

    [SerializeField]
    AudioSource seStart;

    void Start() {
        //SoundManager.Get.PlayBGM("はりきっちゃう時のテーマ", true);
        sc = FindObjectOfType<SceneChanger>();
    }

	void FixedUpdate () {
        UpdateCursor();
        UpdateCharacterSelectStatus();
        CheckCharacterSelected();

        if (isWaiting) {
            WaitingForStart();
        }
        //履歴書を上に飛ばしていく
        //sheetList[0].transform.position = sheetList[0].transform.position + sheetList[0].transform.up;
        CheckCanInput();

        ForceNextScene();
	}

    // カーソルの位置を更新
    void UpdateCursor() {
        for (int cursorIndex = 0; cursorIndex < 4; cursorIndex++) {
            Arrow aElement = arrowList[cursorIndex];

            if (!aElement.GetIsCharacterSelected()) {
                float posX = -60.0f + 10.0f * unselectCharaList[aElement.GetCursorPos()];
                float posY = 10.0f * aElement.GetPlayerIndex();

                // n番のカーソルの位置を履歴書の顔写真の横に移動
                aElement.cursorTransform = Vector3.Lerp(aElement.cursorTransform, portraitList[unselectCharaList[aElement.GetCursorPos()]].arrowPosList[cursorIndex].transform.position, 0.5f);
                renderCamList[cursorIndex].transform.position = new Vector3(posX, posY, camAnimList[aElement.GetPlayerIndex()].beforeCamPos.z);
                camAnimList[aElement.GetPlayerIndex()].SetPosition(posX);
                if (aElement.cursorTransform == portraitList[unselectCharaList[aElement.GetCursorPos()]].arrowPosList[cursorIndex].transform.position) {
                    aElement.canSelect = true;
                }
                else{
                    aElement.canSelect = false;
                }
            }
            else {
                float posX = -60.0f + 10.0f * aElement.charaNum;
                float posY = 10.0f * aElement.GetPlayerIndex();

                // n番のカーソルの位置を履歴書の顔写真の横に移動
                aElement.cursorTransform = Vector3.Lerp(aElement.cursorTransform, portraitList[aElement.charaNum].arrowPosList[cursorIndex].transform.position, 0.5f);
            }
        }
    }

    // キャラクターの選択状態を更新
    void UpdateCharacterSelectStatus() {
        if (changeUnselectListFlg) {
            // unselectリストからRemoveして、それが重複なく成功したらArrow側にそれを通知する
            // 残りゼロ件になるまでやる
            for (; selectDataList.Count > 0; ) {
                //Debug.Log("残り件数" + selectDataList.Count);
                // いじるデータが残り一件の時以外
                if (selectDataList.Count != 1) {
                    // いじる位置が重複している場合はデータ登録の早かったほうを優先
                    for (int j = 1; selectDataList.Count > j;) {
                        if (selectDataList[0]._cursorPos == selectDataList[j]._cursorPos) {
                            selectDataList.RemoveAt(j);
                            //Debug.Log("重複削除");
                        }
                        else {
                            j++;
                        }
                    }
                }

                int pIndex = selectDataList[0]._playerIndex;
                int cPos = selectDataList[0]._cursorPos;
                //Debug.Log("プレイヤー番号:" + (pIndex+1) + " カーソル位置:" + (cPos+1) + " キャラ番号:" + (unselectCharaList[cPos]+1));

                arrowList[pIndex].seReady.Play();

                arrowList[pIndex].charaNum = unselectCharaList[cPos];
                arrowList[pIndex].SetImg(portlaitImgList[unselectCharaList[cPos]], charaSheetImgList[unselectCharaList[cPos]]);

                CharacterManager.SetCharacter(pIndex, unselectCharaList[cPos]);
                allAnimatorList[pIndex].animList[unselectCharaList[cPos]].SetBool("isSelected", true);
                camAnimList[pIndex].StartCameraAnimation();

                unselectCharaList.RemoveAt(cPos);

                // キャラ選択フラグをオンに
                arrowList[pIndex].SetIsCharacterSelected(true);
                //Debug.Log("選択データ削除" + selectDataList[0]._playerIndex);
                selectDataList.RemoveAt(0);

                for (int i = 0; i < 4; i++) {
                    if (i != pIndex && unselectCharaList.Count != 0) {
                        if (arrowList[i].cursorPos > arrowList[pIndex].cursorPos) {
                            arrowList[i].cursorPos = arrowList[i].cursorPos % unselectCharaList.Count - 1;
                        }
                        else {
                            arrowList[i].cursorPos = arrowList[i].cursorPos % unselectCharaList.Count;
                        }
                        
                        if (arrowList[i].cursorPos < 0) {
                            arrowList[i].cursorPos = unselectCharaList.Count - 1;
                        }
                    }
                }

                for (int i = 0; i < selectDataList.Count; i++) {
                    if (selectDataList[i]._cursorPos != 0) {
                        selectDataList[i]._cursorPos = (selectDataList[i]._cursorPos - 1) % selectDataList.Count;
                    }
                    else {
                        selectDataList[i]._cursorPos = selectDataList.Count - 1;
                    }
                }

                //Debug.Log("select処理");
            }

            if (unselectDataList.Count != 0) {
                // 残りゼロ件になるまでやる
                for (; unselectDataList.Count > 0; ) {
                    int pIndex = unselectDataList[0]._playerIndex;
                    int cNum = arrowList[pIndex].charaNum;

                    unselectCharaList.Add(CharacterManager.SelectedCharacters[pIndex]);

                    CharacterManager.ResetCharacter(pIndex);
                    allAnimatorList[pIndex].animList[cNum].SetBool("isSelected", false);

                    // キャラ選択フラグをオフに
                    //Debug.Log("キャラ番号" + cNum + "unselect処理");
                    arrowList[pIndex].SetIsCharacterSelected(false);
                    camAnimList[pIndex].StartCameraAnimation();

                    arrowList[pIndex].seCancel.Play();

                    for (int i = 0; i < 4; i++) {
                        if (i != pIndex) {
                            if (arrowList[i].cursorPos >= arrowList[pIndex].cursorPos) {
                                arrowList[i].cursorPos = arrowList[i].cursorPos % unselectCharaList.Count + 1;
                            }
                        }
                    }

                    unselectDataList.RemoveAt(0);
                }
                unselectCharaList.Sort();
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
                    seStart.Play();
                    sc.ChangeScene("TimeLineTest");
                    sceneChangeFlg = true;
                }
                cursorIndex = 999;
            }
            cursorIndex++;
        }
    }

    void ForceNextScene() {
        if (Input.GetKeyDown(KeyCode.P)) {
            for (int pIndex = 0; pIndex < 4; pIndex++) {
                CharacterManager.SetCharacter(pIndex, pIndex);
            }

            sc.ChangeScene("TimeLineTest");
            sceneChangeFlg = true;
        }
    }

    public bool GetIsWaiting() {
        return isWaiting;
    }

    // 下2つはいずれ放棄、UpdateCharacterselectStatusに統合？　これら2つの処理は同フレームで順番に行いたい
    // playerIndexとcursorPosを持つ構造体にいじり情報を格納する？
    public void SelectCharater(int _playerIndex, int _cursorPos) {
        changeUnselectListFlg = true;

        changeData cd = new changeData();
        cd._playerIndex = _playerIndex;
        cd._cursorPos = _cursorPos;

        selectDataList.Add(cd);
    }

    public void UnselectCharacter(int _playerIndex, int _cursorPos) {
        changeUnselectListFlg = true;

        changeData cd = new changeData();
        cd._playerIndex = _playerIndex;
        cd._cursorPos = _cursorPos;
        //Debug.Log("pIndex:"+_playerIndex+" cPos:"+_cursorPos);

        unselectDataList.Add(cd);
    }

    void CheckCanInput() {
        for (int cursorIndex = 0; cursorIndex < 4; cursorIndex++) {
            Arrow aElement = arrowList[cursorIndex];

            if (aElement.GetCanInput()) {
                for(int charaNum = 0; charaNum < 4; charaNum++){
                    allAnimatorList[aElement.GetPlayerIndex()].animList[charaNum].SetBool("canInput", true);
                }
            }
        }
    }
}
