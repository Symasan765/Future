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
	Vector2[] m_KeyboardLeftStickVal = new Vector2[MaxControllerNum];
	Vector2[] m_KeyboardRightStickVal = new Vector2[MaxControllerNum];
	bool[] ConnectFlag = new bool[MaxControllerNum];

	// デバッグではコントローラーだけではなくキーボード入力も利用するため
	bool DebugFlag = false;
	public bool[] KeyDebugFlag = new bool[MaxControllerNum];    // キーボードでの複数人接続を行う

	const float m_DeadZone = 0.3f;

	Vector3[] m_VibeData = new Vector3[MaxControllerNum];       // x = 左振動情報、y = 右振動情報,z = 残りタイム

	public bool m_UpdateFlag = true;

	/// <summary>
	/// 入力するキー番号。ビット演算を使用するため各数値は累乗
	/// </summary>
	public enum KeyData
	{
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
		BACK = 2048,
		RT = 4096
	}

	/// <summary>
	/// 短入力を取得する
	/// </summary>
	/// <param name="key">独自キーコード。KeyData.…で取得できる</param>
	/// <param name="GamePadNo">取得するコントローラー番号</param>
	/// <returns></returns>
	public bool GetTrigger(KeyData key, int GamePadNo)
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
	/// 誰かが指定のボタンを押していれば真を返す関数
	/// </summary>
	/// <param name="key"></param>
	/// <returns></returns>
	public bool AnyoneTrigger(KeyData key)
	{
		for (int i = 0; i < MaxControllerNum; i++)
		{
			if (GetTrigger(key, i))
				return true;
		}

		// ここまで来たってことは誰もボタン押してない
		return false;
	}

	public bool AnyonePress(KeyData key)
	{
		for (int i = 0; i < MaxControllerNum; i++)
		{
			if (GetPress(key, i))
				return true;
		}

		// ここまで来たってことは誰もボタン押してない
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

	/// <summary>
	/// ジャンプを取得するボタンが押されているか判定を行う
	/// </summary>
	/// <param name="GamePadNo"></param>
	/// <returns></returns>
	public bool JumpPress(int GamePadNo)
	{
		// 更新不可ならなにも返さない
		if (m_UpdateFlag == false)
			return false;

		// ジャンプ処理を行うボタンをどんどん追加していく
		if (GetPress(KeyData.A, GamePadNo))
			return true;

		if (GetPress(KeyData.B, GamePadNo))
			return true;

		if (GetPress(KeyData.Y, GamePadNo))
			return true;

		if (GetPress(KeyData.X, GamePadNo))
			return true;

		return false;
	}

	public bool JumpTrigger(int GamePadNo)
	{
		// 更新不可ならなにも返さない
		if (m_UpdateFlag == false)
			return false;

		// ジャンプ処理を行うボタンをどんどん追加していく
		if (GetTrigger(KeyData.A, GamePadNo))
			return true;

		if (GetTrigger(KeyData.B, GamePadNo))
			return true;

		if (GetTrigger(KeyData.Y, GamePadNo))
			return true;

		if (GetTrigger(KeyData.X, GamePadNo))
			return true;

		return false;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="GamePadNo"></param>
	/// <returns></returns>
	public bool HoldPress(int GamePadNo)
	{
		// 更新不可ならなにも返さない
		if (m_UpdateFlag == false)
			return false;

		// ジャンプ処理を行うボタンをどんどん追加していく
		if (GetPress(KeyData.RB, GamePadNo))
			return true;

		if (GetPress(KeyData.RT, GamePadNo))
			return true;

		return false;
	}

	public bool HoldTrigger(int GamePadNo)
	{
		// 更新不可ならなにも返さない
		if (m_UpdateFlag == false)
			return false;

		// ジャンプ処理を行うボタンをどんどん追加していく
		if (GetTrigger(KeyData.RB, GamePadNo))
			return true;

		if (GetTrigger(KeyData.RT, GamePadNo))
			return true;

		return false;
	}

	/// <summary>
	/// 右スティックの入力ベクトルを取得する
	/// </summary>
	/// <param name="GamePadNo"></param>
	/// <returns>長さ1.0f以内のX,Y成分ベクトル</returns>
	public Vector2 GetRightStick(int GamePadNo)
	{
		if (ConnectFlag[GamePadNo])
		{
			return StickJudge(NowState[GamePadNo].ThumbSticks.Right);
		}

		// ゲームパッド非接続
		return m_KeyboardRightStickVal[GamePadNo];
	}

	/// <summary>
	/// 左スティックの入力ベクトルを取得する
	/// </summary>
	/// <param name="GamePadNo"></param>
	/// <returns>長さ1.0f以内のX,Y成分ベクトル</returns>
	public Vector2 GetLeftStick(int GamePadNo)
	{
		// 更新不可処理が出ていればなにも返さない
		if (m_UpdateFlag == false)
			return Vector2.zero;

		if (ConnectFlag[GamePadNo])
		{
			// 11/11 追加仕様。アナログスティックの入力が少なければデジタルキーも見て入力がゼロでなければそちらを使う
			Vector2 ret = StickJudge(NowState[GamePadNo].ThumbSticks.Left);
			if (ret.magnitude == 0)
			{
				ret = Vector2.zero;
				if (GetPress(KeyData.UP, GamePadNo)) ret += Vector2.up;
				if (GetPress(KeyData.DOWN, GamePadNo)) ret += Vector2.down;
				if (GetPress(KeyData.RIGHT, GamePadNo)) ret += Vector2.right;
				if (GetPress(KeyData.LEFT, GamePadNo)) ret += Vector2.left;
			}

			return ret.normalized;
		}

		// ゲームパッド非接続
		return m_KeyboardLeftStickVal[GamePadNo];
	}

	Vector2 StickJudge(GamePadThumbSticks.StickValue stick)
	{
		Vector2 val = new Vector2(stick.X, stick.Y);
		// デッドゾーンより入力が少なければ入力を感知しない
		if (val.magnitude < m_DeadZone)
		{
			return Vector2.zero;
		}

		return val;
	}

	/// <summary>
	/// コントローラーの振動を有効にする。
	/// </summary>
	/// <param name="GamePadNo"></param>
	/// <param name="LeftMagnitude">0.0~1.0 粗の大きいショック性の振動</param>
	/// <param name="RightMagnitude">0.0-1.0 きめの細かい連続性の振動</param>
	/// <param name="vibeSec"></param>
	public void SetVibration(int GamePadNo, float LeftMagnitude, float RightMagnitude, float vibeSec)
	{
		if (ConnectFlag[GamePadNo])
			m_VibeData[GamePadNo] = new Vector3(LeftMagnitude, RightMagnitude, vibeSec);
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
		KeyDebugFlag[0] = true;     // 第一コントローラーは初期段階で入力できるようにしている
		DebugFlag = Debug.isDebugBuild;     // デバッグ状態を取得
	}

	// Update is called once per frame
	private void FixedUpdate()
	{
		AllConfirmConnection();
		DebugKeyFlagSwitching();

		AllInputUpdate();
		Vibration();
	}

	void Vibration()
	{
		for (int i = 0; i < MaxControllerNum; i++)
		{
			// 振動時間がまだ残っている
			if (m_VibeData[i].z > 0.0f)
			{
				m_VibeData[i].z -= Time.deltaTime;
			}
			else
			{
				m_VibeData[i] = Vector3.zero;
			}
			GamePad.SetVibration((PlayerIndex)i, m_VibeData[i].x, m_VibeData[i].y);
		}
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
		for (int i = 0; i < MaxControllerNum; i++)
		{
			NowState[i] = GamePad.GetState(playerIndex[i]);   //新しい情報を取得
			GamePadUpdate(NowState[i], i);    //更新
		}
	}

	/// <summary>
	/// ゲームパッドの入力情報を検査して取得
	/// </summary>
	/// <param name="state"></param>
	/// <returns></returns>
	void GamePadUpdate(GamePadState Data, int i)
	{
		OldInp[i] = NowInp[i];        //前のフレームの情報を保持する
		NowInp[i] = 0;             //一度入力情報をクリアする

		//ゲームパッドが接続されていたらそちらからデータを取得する
		if (ConnectFlag[i])
		{
			PadButtonUpdate(Data, i);
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
	void PadButtonUpdate(GamePadState Data, int i)
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
		if (Data.Triggers.Right > 0.1f) NowInp[i] += (short)KeyData.RT;
	}

	/// <summary>
	/// キーボードのボタン情報を更新する
	/// </summary>
	/// <param name="i"></param>
	void KeyButtonUpdate(int i)
	{
		if (Input.GetKey(KeyCode.K)) NowInp[i] += (short)KeyData.A;
		if (Input.GetKey(KeyCode.L)) NowInp[i] += (short)KeyData.B;
		if (Input.GetKey(KeyCode.I)) NowInp[i] += (short)KeyData.X;
		if (Input.GetKey(KeyCode.J)) NowInp[i] += (short)KeyData.Y;
		if (Input.GetKey(KeyCode.UpArrow)) NowInp[i] += (short)KeyData.UP;
		if (Input.GetKey(KeyCode.DownArrow)) NowInp[i] += (short)KeyData.DOWN;
		if (Input.GetKey(KeyCode.LeftArrow)) NowInp[i] += (short)KeyData.LEFT;
		if (Input.GetKey(KeyCode.RightArrow)) NowInp[i] += (short)KeyData.RIGHT;
		if (Input.GetKey(KeyCode.U)) NowInp[i] += (short)KeyData.RB;
		if (Input.GetKey(KeyCode.E)) NowInp[i] += (short)KeyData.LB;
		if (Input.GetKey(KeyCode.Space)) NowInp[i] += (short)KeyData.START;
		if (Input.GetKey(KeyCode.Escape)) NowInp[i] += (short)KeyData.BACK;
		if (Input.GetKey(KeyCode.O)) NowInp[i] += (short)KeyData.RT;

		float x = 0, y = 0;
		// 左スティックの入力
		if (Input.GetKey(KeyCode.A)) x -= 1.0f;
		if (Input.GetKey(KeyCode.D)) x += 1.0f;
		if (Input.GetKey(KeyCode.S)) y -= 1.0f;
		if (Input.GetKey(KeyCode.W)) y += 1.0f;
		m_KeyboardLeftStickVal[i] = new Vector2(x, y).normalized;

		x = 0;
		y = 0;
		if (Input.GetKey(KeyCode.B)) x -= 1.0f;
		if (Input.GetKey(KeyCode.Comma)) x += 1.0f;
		if (Input.GetKey(KeyCode.M)) y -= 1.0f;
		if (Input.GetKey(KeyCode.N)) y += 1.0f;

		m_KeyboardRightStickVal[i] = new Vector2(x, y).normalized;
	}

	/// <summary>
	/// 各数値キーを押すとそのキーに対応したコントローラーをキーボードで入力できるようになる
	/// </summary>
	void DebugKeyFlagSwitching()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			KeyDebugFlag[0] = !KeyDebugFlag[0];
			Debug.Log("コントローラーNo" + 1 + "接続状態 : " + KeyDebugFlag[0]);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			KeyDebugFlag[1] = !KeyDebugFlag[1];
			Debug.Log("コントローラーNo" + 2 + "接続状態 : " + KeyDebugFlag[1]);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			KeyDebugFlag[2] = !KeyDebugFlag[2];
			Debug.Log("コントローラーNo" + 3 + "接続状態 : " + KeyDebugFlag[2]);
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			KeyDebugFlag[3] = !KeyDebugFlag[3];
			Debug.Log("コントローラーNo" + 4 + "接続状態 : " + KeyDebugFlag[3]);
		}
	}
}