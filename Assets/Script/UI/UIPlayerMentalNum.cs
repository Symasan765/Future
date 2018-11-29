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
	private Color[] damageColor = new Color[4];
	private float nowMental = 0;
	private bool isShake = false;
	private float shakePower = 0;
	void Start ()
	{
		text[0] = GetComponent<Text>();
		text[1] = UIBackNumObj.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (nowMental > player.GetMentalGauge())
		{
			nowMental -= Time.deltaTime * 40;
			if (nowMental <= 0)
			{
				nowMental = 0;
			}
		}
		if (nowMental < player.GetMentalGauge())
		{
			nowMental += Time.deltaTime * 50;
			if (nowMental > player.GetMentalGauge())
			{
				nowMental = player.GetMentalGauge();
			}

		}
		/*
		if (nowMental != player.GetMentalGauge())
		{
			if (!isShake)
			{
				shakePower = 10;
				isShake = true;
			}
		}
		if (isShake)
		{
			if (nowMental == player.GetMentalGauge())
			{
				isShake = false;
			}

		}
		if (shakePower > 0)
		{

		}*/


		for (int i = 0; i < 2; i++)
		{
			text[i].text = ((int)nowMental).ToString();
			//text[i].text = player.GetMentalGauge().ToString();
		}

		if (player.GetMentalGauge() < 100 / 4)
		{
			//Debug.Log(player.GetMentalGauge() / (100 / 4));
			text[0].color = Color.Lerp(damageColor[0], damageColor[1], (float)player.GetMentalGauge() / (100 / 4));
			//text[0].color = damageColor[0];
		} else
		{
			if (player.GetMentalGauge() < (100 / 2))
			{
				text[0].color = Color.Lerp(damageColor[1], damageColor[2], (float)player.GetMentalGauge() / (100 / 2));
				//text[0].color = damageColor[1];
			} else
			{
				if (player.GetMentalGauge() < (100 / 4) * 3)
				{
					text[0].color = Color.Lerp(damageColor[2], damageColor[3], (float)player.GetMentalGauge() / ((100 / 4)*3));
					//text[0].color = damageColor[2];
				} else
				{
					text[0].color = damageColor[3];
				}
			}
		}

	}

	public void SetPlayerObject(GameObject _playerObject)
	{
		playerObject = _playerObject;
		player = playerObject.GetComponent<Player>();
	}

	public void SetColor(Color _c1,Color _c2,Color _c3,Color _c4)
	{
		damageColor[0] = _c1;
		damageColor[1] = _c2;
		damageColor[2] = _c3;
		damageColor[3] = _c4;
	}
}
