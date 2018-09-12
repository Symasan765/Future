using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class XPad : SingletonMonoBehaviour<XPad>
{
    const int MaxControllerNum = 4;
    PlayerIndex[] playerIndex = new PlayerIndex[MaxControllerNum];            //現在つないでいるプレイヤーの番号
    GamePadState[] NowState = new GamePadState[MaxControllerNum];              //現在のフレームの入力情報
    
    //下記の二つはビットフラグを使用するので注意
    short[] NowInp = new short[MaxControllerNum];
    short[] OldInp = new short[MaxControllerNum];
    bool[] ConnectFlag = new bool[MaxControllerNum];

    // デバッグではコントローラーだけではなくキーボード入力も利用するため
    bool DebugFlag = false;
	public bool[] KeyDebugFlag = new bool[MaxControllerNum];	// キーボードでの複数人接続を行う

    /// <summary>
    /// 入力するキー番号。ビット演算を使用するため各数値は累乗
    /// </summary>
    public enum KeyData {
		A = 1,
		B = 2,
		X = 4,
		Y = 8,
		UP = 16,            //十字キー
		DOWN = 32,
		RIGHT = 64,
		LEFT = 128,
		RB = 256,       //いわゆるR1キー
		LB = 512,       //L1キー
		START = 1024,
		BACK = 2048
	}

	/// <summary>
	/// 短入力を取得する
	/// </summary>
	/// <param name="key">独自キーコード。KeyData.…で取得できる</param>
	/// <param name="GamePadNo">取得するコントローラー番号</param>
	/// <returns></returns>
	public bool GetTrigger(KeyData key,int GamePadNo)
	{
		bool Now = false;
		bool Old = false;
		//現在のフレームで指定ボタンが押されているか？
		if ((NowInp[GamePadNo] & (short)key) != 0) Now = true;
		//現在のフレームで指定ボタンが押されているか？
		if ((OldInp[GamePadNo] & (short)key) != 0) Old = true;
		//トリガー情報に変換する
		if ((Now ^ Old) & Now) return true; //今と前の入力情報をが違う状態で今がtrueのとき
		return false;
	}

	/// <summary>
	/// キーのリリース判定を行う
	/// </summary>
	/// <param name="key">独自キーコード。KeyData.…で取得できる</param>
	/// <param name="GamePadNo">取得するコントローラー番号</param>
	/// <returns></returns>
	public bool GetRelease(KeyData key, int GamePadNo)
	{
		bool Now = false;
		bool Old = false;
		//現在のフレームで指定ボタンが押されているか？
		if ((NowInp[GamePadNo] & (short)key) != 0) Now = true;
		//現在のフレームで指定ボタンが押されているか？
		if ((OldInp[GamePadNo] & (short)key) != 0) Old = true;
		//リリース情報に変換する
		if ((Now ^ Old) & Old) return true; //今と前の入力情報をが違う状態で前がtrueのとき
		return false;
	}

	/// <summary>
	/// キー押下状態を取得
	/// </summary>
	/// <param name="key">独自キーコード。KeyData.…で取得できる</param>
	/// <param name="GamePadNo">取得するコントローラー番号</param>
	/// <returns></returns>
	public bool GetPress(KeyData key, int GamePadNo)
	{
		if ((NowInp[GamePadNo] & (short)key) != 0) return true;
		return false;
	}

	void Start()
    {

		Debug.Log("Update");
		//	パラメータ初期化
		for (int i = 0; i < MaxControllerNum; i++)
        {
            NowInp[i] = 0;
            OldInp[i] = 0;
            ConnectFlag[i] = false;
			KeyDebugFlag[i] = false;

		}
		KeyDebugFlag[0] = true;		// 第一コントローラーは初期段階で入力できるようにしている
		DebugFlag = Debug.isDebugBuild;     // デバッグ状態を取得
    }

    // Update is called once per frame
    void Update () {
		AllConfirmConnection();
		DebugKeyFlagSwitching();

		AllInputUpdate();
	}

    /// <summary>
    /// コントローラーとの接続を試みる
    /// </summary>
    void AllConfirmConnection()
    {
        for (int i = 0; i < MaxControllerNum; i++)
        {
            ConfirmConnection(i);
        }       // end for (int i = 0; i < MaxControllerNum; i++)
    }

    void ConfirmConnection(int i)
    {
        // まだ接続されていなければ接続処理
        if (!ConnectFlag[i] || !NowState[i].IsConnected)
        {
            PlayerIndex testPlayerIndex = (PlayerIndex)i;
            GamePadState testState = GamePad.GetState(testPlayerIndex);
            //1度でも接続されればコネクトフラグを立てて、以降このブロックに入らないようにする
            if (testState.IsConnected)
            {
                playerIndex[i] = testPlayerIndex;
                ConnectFlag[i] = true;
            }
        }       // end if (!playerIndexSet || !prevState.IsConnected)
    }

	/// <summary>
	/// 入力情報更新
	/// </summary>
	void AllInputUpdate()
	{
		for(int i = 0; i < MaxControllerNum; i++)
		{
			NowState[i] = GamePad.GetState(playerIndex[i]);   //新しい情報を取得
			GamePadUpdate(NowState[i],i);    //更新
		}
	}

	/// <summary>
	/// ゲームパッドの入力情報を検査して取得
	/// </summary>
	/// <param name="state"></param>
	/// <returns></returns>
	void GamePadUpdate(GamePadState Data,int i)
	{
		OldInp[i] = NowInp[i];        //前のフレームの情報を保持する
		NowInp[i] = 0;             //一度入力情報をクリアする

		//ゲームパッドが接続されていたらそちらからデータを取得する
		if (ConnectFlag[i])
		{
			PadButtonUpdate(Data,i);
		}
		else
		{
			//接続されておらず、入力モードがONになっていればキーボードからデータを取得する
			if (KeyDebugFlag[i])
			{
				KeyButtonUpdate(i);
			}
		}
	}

	/// <summary>
	/// ゲームパッドのボタン情報を更新する
	/// </summary>
	/// <param name="Data"></param>
	/// <param name="i"></param>
	void PadButtonUpdate(GamePadState Data,int i)
	{
		//新しい入力情報を反映していく
		if (Data.Buttons.A == ButtonState.Pressed) NowInp[i] += (short)KeyData.A;
		if (Data.Buttons.B == ButtonState.Pressed) NowInp[i] += (short)KeyData.B;
		if (Data.Buttons.X == ButtonState.Pressed) NowInp[i] += (short)KeyData.X;
		if (Data.Buttons.Y == ButtonState.Pressed) NowInp[i] += (short)KeyData.Y;
		if (Data.DPad.Up == ButtonState.Pressed) NowInp[i] += (short)KeyData.UP;
		if (Data.DPad.Down == ButtonState.Pressed) NowInp[i] += (short)KeyData.DOWN;
		if (Data.DPad.Left == ButtonState.Pressed) NowInp[i] += (short)KeyData.LEFT;
		if (Data.DPad.Right == ButtonState.Pressed) NowInp[i] += (short)KeyData.RIGHT;
		if (Data.Buttons.RightShoulder == ButtonState.Pressed) NowInp[i] += (short)KeyData.RB;
		if (Data.Buttons.LeftShoulder == ButtonState.Pressed) NowInp[i] += (short)KeyData.LB;
		if (Data.Buttons.Start == ButtonState.Pressed) NowInp[i] += (short)KeyData.START;
		if (Data.Buttons.Back == ButtonState.Pressed) NowInp[i] += (short)KeyData.BACK;
	}

	/// <summary>
	/// キーボードのボタン情報を更新する
	/// </summary>
	/// <param name="i"></param>
	void KeyButtonUpdate(int i)
	{
		if (Input.GetKey(KeyCode.Space)) NowInp[i] += (short)KeyData.A;
		if (Input.GetKey(KeyCode.D)) NowInp[i] += (short)KeyData.B;
		if (Input.GetKey(KeyCode.P)) NowInp[i] += (short)KeyData.X;
		if (Input.GetKey(KeyCode.L)) NowInp[i] += (short)KeyData.Y;
		if (Input.GetKey(KeyCode.UpArrow)) NowInp[i] += (short)KeyData.UP;
		if (Input.GetKey(KeyCode.DownArrow)) NowInp[i] += (short)KeyData.DOWN;
		if (Input.GetKey(KeyCode.LeftArrow)) NowInp[i] += (short)KeyData.LEFT;
		if (Input.GetKey(KeyCode.RightArrow)) NowInp[i] += (short)KeyData.RIGHT;
		if (Input.GetKey(KeyCode.W)) NowInp[i] += (short)KeyData.RB;
		if (Input.GetKey(KeyCode.A)) NowInp[i] += (short)KeyData.LB;
		if (Input.GetKey(KeyCode.Return)) NowInp[i] += (short)KeyData.START;
		if (Input.GetKey(KeyCode.M)) NowInp[i] += (short)KeyData.BACK;
	}

	/// <summary>
	/// 各数値キーを押すとそのキーに対応したコントローラーをキーボードで入力できるようになる
	/// </summary>
	void DebugKeyFlagSwitching()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			KeyDebugFlag[0] = !KeyDebugFlag[0];
			Debug.Log("コントローラーNo" + 1 + "接続状態 : " + KeyDebugFlag[0]);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2)) {
			KeyDebugFlag[1] = !KeyDebugFlag[1];
			Debug.Log("コントローラーNo" + 2 + "接続状態 : " + KeyDebugFlag[1]);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3)) {
			KeyDebugFlag[2] = !KeyDebugFlag[2];
			Debug.Log("コントローラーNo" + 3 + "接続状態 : " + KeyDebugFlag[2]);
		}
		if (Input.GetKeyDown(KeyCode.Alpha4)) {
			KeyDebugFlag[3] = !KeyDebugFlag[3];
			Debug.Log("コントローラーNo" + 4 + "接続状態 : " + KeyDebugFlag[3]);
		}
	}
}