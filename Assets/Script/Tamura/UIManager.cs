using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    [SerializeField]
    Player[] playerArray;
    [SerializeField]
    Image[] GaugeArray;
    [SerializeField]
    ShakeUI[] UIArray;

    [SerializeField, HeaderAttribute("揺れの強さ")]
    float shakeMagnitude;

    [SerializeField, HeaderAttribute("揺れる時間(秒)")]
    float shakeTime;

	void Start () {
        playerArray = FindObjectsOfType<Player>();
        Array.Sort(playerArray, (a, b) => a.PlayerIndex - b.PlayerIndex);
	}
	
	void Update () {
        UpdateUI();
	}

    void UpdateUI() {
        int index = 0;
        foreach (var player in playerArray) {
            // ゲージの増減
            float mentalGauge = (float)player.GetMentalGauge();
            GaugeArray[index].fillAmount = Mathf.Lerp(GaugeArray[index].fillAmount, Mathf.Clamp((mentalGauge / 100.0f), 0.1f, 0.9f), 0.1f);
            Debug.Log(GaugeArray[2].fillAmount);
            if (GaugeArray[index].fillAmount > 0.7f) {
                GaugeArray[index].color = Color.red;
            }
            else if (GaugeArray[index].fillAmount > 0.4f) {
                GaugeArray[index].color = Color.yellow;
            }
            else {
                GaugeArray[index].color = Color.green;
            }

            // UIの位置調整(揺らし)
            if (player.IsDamage() && !UIArray[index].IsShake) {
                UIArray[index].Shake(shakeTime, shakeMagnitude);
            }

            index++;
        }
    }
}
