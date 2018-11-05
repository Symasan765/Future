using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextCtrl : MonoBehaviour {
	// このスクリプトは平行投影、透視投影両方で使う前提なのでビルボードでの実装を行っていない
	// 透視投影の場合はカメラの子オブジェクトにするとか注意が必要
	// 平行投影の場合はZ軸を正面に向けて配置したい座標に作成するだけでいいから楽

	Vector3 m_InitialScale;
	public Vector3 m_ShakeVolume = Vector3.one * 0.02f;       // 揺らす量
	public Vector3 m_ShakeTime = Vector3.one;			// 揺れるのが一周する時間
	public float m_StartTime = 0.2f;        // スプライトが出現しきるまでの時間
	public float m_LostTime = 2.0f;         // スプライトが消えるまでの時間
	public string m_SEName;					// ここに名前が登録されればSEを鳴らす

	float ExpansTimeRate = 0.8f;


	// Use this for initialization
	void Start () {
		m_InitialScale = transform.localScale;

		// コルーチンスタート！
		StartCoroutine("SpriteUpdate");
	}

	IEnumerator SpriteUpdate()
	{
	
		//まずはサイズがゼロの状態から少しずつ大きくしていく
		Vector3 ExpansionSize = m_InitialScale * 1.1f;
		float ExpansionTime = m_StartTime * ExpansTimeRate;

		for(float timeCnt = 0.0f; timeCnt < ExpansionTime; timeCnt+= Time.deltaTime)
		{
			float t = timeCnt / ExpansionTime;
			transform.localScale = Vector3.Lerp(Vector3.zero, ExpansionSize, t);
			yield return null;
		}   //拡大終わり


		// 次は縮小させる
		float ReducedTime = m_StartTime * (1.0f - ExpansTimeRate);
		for (float timeCnt = 0.0f; timeCnt < ReducedTime; timeCnt += Time.deltaTime)
		{
			float t = timeCnt / ReducedTime;
			transform.localScale = Vector3.Lerp(ExpansionSize, m_InitialScale, t);
			yield return null;
		}   //縮小終わり

		// ここのタイミングで音鳴らす
		if(m_SEName != "")
		{
			SoundManager.Get.PlaySE(m_SEName);
		}

		// 次は自由に拡大縮小させる
		for (float timeCnt = 0.0f; timeCnt < m_LostTime; timeCnt += Time.deltaTime)
		{
			// こいつらをsin or cos に渡せばその秒数での周期が出来る
			float shakeXT = (Mathf.Cos(Mathf.Deg2Rad * (timeCnt * 360.0f / m_ShakeTime.x)) + 1.0f) * 0.5f;
			float shakeYT = (Mathf.Cos(Mathf.Deg2Rad * (timeCnt * 360.0f / m_ShakeTime.y)) + 1.0f) * 0.5f;

			Vector3 lower = m_InitialScale - m_ShakeVolume;
			Vector3 upper = m_InitialScale + m_ShakeVolume;
			Vector3 scale = new Vector3(Mathf.Lerp(m_InitialScale.x, upper.x, shakeXT), Mathf.Lerp(m_InitialScale.y, upper.y, shakeYT), 1.0f);

			transform.localScale = scale;
			yield return null;
		}   //自由時間終わり


		// 縮小終了させる
		float endTime = 0.1f;
		for (float timeCnt = 0.0f; timeCnt < endTime; timeCnt += Time.deltaTime)
		{
			float t = timeCnt / endTime;
			transform.localScale = Vector3.Lerp(m_InitialScale, Vector3.zero, t);
			yield return null;
		}   //縮小終わり

		Destroy(gameObject);
		yield break;
	}
}
