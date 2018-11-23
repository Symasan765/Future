using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIPlayerMentalNum : MonoBehaviour {

	public int playerIndex = 0;

	private GameObject playerObject;
	private Player player;
	private Text text;
	void Start ()
	{
		text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update ()
	{

		text.text = player.GetMentalGauge().ToString();

	}

	public void SetPlayerObject(GameObject _playerObject)
	{
		playerObject = _playerObject;
		player = playerObject.GetComponent<Player>();
	}
}
