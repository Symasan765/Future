using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// しぇけなべいべぇ
public class ShakeUI : MonoBehaviour {
    // 揺れの強さ
    [SerializeField]
    float magnitude;
    
    // 振動し始めてから何秒か
    [SerializeField]
    float shakeTimeFromStart = 0.0f;
    
    // 振動する秒数
    [SerializeField]
    float shakeTime = 0.0f;

    // 前のフレームで揺れた量
    Vector3 beforeShake;
	
	// Update is called once per frame
	void Update () {
        // 揺れる前の位置に戻す
        transform.position -= beforeShake;

        // もし振動中なら
        if (IsShake) {
            Vector3 shake = Random.insideUnitSphere * magnitude;
            transform.position += shake;
            beforeShake = shake;

            //揺れた時間を増やす
            shakeTimeFromStart += Time.deltaTime;
        }
        else {
            beforeShake = Vector3.zero;
        }
	}

    // 揺れている最中かどうか
    public bool IsShake {
        get {
            if (shakeTimeFromStart < shakeTime) {
                return true;
            }
            return false;
        }
    }

    public void Shake(float time, float mag) {
        shakeTime = time;
        shakeTimeFromStart = 0.0f;
        magnitude = mag;
    }

    // UIを全部揺らす
    public static void ShakeAll(float time, float mag) {
        foreach (var shakeUI in FindObjectsOfType<ShakeUI>()) {
            shakeUI.Shake(time, mag);
        }
    }
}
