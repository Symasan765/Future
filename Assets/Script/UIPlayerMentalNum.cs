using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIPlayerMentalNum : MonoBehaviour {

	public int playerIndex = 0;
	[SerializeField]
	private GameObject UIBackNumObj;
	private GameObject playerObject;
	private Player player;
	private Text[] text = new Text[2];
	void Start ()
	{
		text[0] = GetComponent<Text>();
		text[1] = UIBackNumObj.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update ()
	{

		for (int i = 0; i < 2; i++)
		{
			text[i].text = player.GetMentalGauge().ToString();
		}

		if (player.GetMentalGauge() < 100 / 4)
		{
			text[0].color = new Color(1, 0.1f ,0.1f, 1.0f);
		} else
		{
			if (player.GetMentalGauge() < (100 / 2))
			{
				text[0].color = new Color(0.8f, 0.1f, 0.1f, 1.0f);
			} else
			{
				if (player.GetMentalGauge() < (100 / 4) * 3)
				{
					text[0].color = new Color(0.6f, 0.1f, 0.1f, 1.0f);
				} else
				{
					text[0].color = new Color(0.4f, 0.1f, 0.1f, 1.0f);
				}
			}
		}

	}

	public void SetPlayerObject(GameObject _playerObject)
	{
		playerObject = _playerObject;
		player = playerObject.GetComponent<Player>();
	}
}
