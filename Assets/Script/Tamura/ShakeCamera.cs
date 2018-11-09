using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShakeCamera : MonoBehaviour {
    static float fixShakeStrength = 1.5f;
    static float fixShakeFreqency = 50.0f;
    static Cinemachine.CinemachineVirtualCamera vcam;

    static float nowShakeTime;
    static float totalShakeTime;

    static bool isShakeCamera;

    void Start() {
        vcam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
    }

    void Update() {
        if (isShakeCamera) {
            nowShakeTime += Time.deltaTime;
            float shakePercentage = Mathf.Clamp(nowShakeTime / totalShakeTime, 0.0f, 1.0f);

            // 時間をオーバーしたら揺れ停止
            if (shakePercentage <= 0.0f) {
                vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0.0f;
                vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0.0f;
                isShakeCamera = false;
            }
            else {
                vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = Mathf.Lerp(fixShakeFreqency, 0.0f, shakePercentage);
              
            }
        }
    }

    // 瞬間的な揺れ(強さ(0.0f～1.0f)、時間(秒))
    public static void Impact(float _strength, float _time) {
        Mathf.Clamp(_strength, 0.0f, 1.0f);
        vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = _strength * 3;
        vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = fixShakeFreqency;
        nowShakeTime = 0.0f;
        totalShakeTime = _time;
        isShakeCamera = true;
    }

    // 瞬間的な揺れ(揺れの強さ固定)
    public static void ImpactFix(float _time) {
        vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = fixShakeStrength;
        vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = fixShakeFreqency;
        nowShakeTime = 0.0f;
        totalShakeTime = _time;
        isShakeCamera = true;
    }
}
