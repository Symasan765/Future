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
            float mentalGauge = (float)player.GetMentalGauge();
            GaugeArray[index].fillAmount = Mathf.Clamp((mentalGauge / 100.0f), 0.1f, 0.9f);
            if (GaugeArray[index].fillAmount > 0.7f) {
                GaugeArray[index].color = Color.red;
            }
            else if (GaugeArray[index].fillAmount > 0.4f) {
                GaugeArray[index].color = Color.yellow;
            }
            else {
                GaugeArray[index].color = Color.green;
            }
            index++;
        }
    }
}
