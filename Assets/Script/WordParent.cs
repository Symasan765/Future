using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordParent : MonoBehaviour {

	[SerializeField]
	private bool flgPlay = false;
	private int cntDerayFrame = 0;
	[SerializeField]
	private float RotationRangeMax = 30;
	[SerializeField]
	private float RotationRangeMin = 20;

	private float nowRotate = 0;
	private Color color;
	
	void Update ()
	{
		transform.eulerAngles = new Vector3(0,0,nowRotate);
		//color = new Color(1, 1, 1, 1);
	}

	public void Play(int _playerIndex)
	{
		SetColor(_playerIndex);
		float r = Random.Range(RotationRangeMax / 2 * -1, RotationRangeMax / 2);

		if (r > 0)
		{
			if (r < RotationRangeMin / 2)
			{
				r = RotationRangeMin / 2;
			}
		} else
		{
			if (r > RotationRangeMin / 2 * -1)
			{
				r = RotationRangeMin / 2 * -1;
			}
		}

		nowRotate = r;

		flgPlay = true;
	}

	public Color GetColor()
	{
		return color;
	}

	public bool IsPlay()
	{
		return flgPlay;
	}

	public void DestroyObject()
	{
		Destroy(gameObject);
	}

	public void SetColor(int _playerIndex)
	{
		if (_playerIndex == 0)
		{
			color = Color.blue;
		}
		if (_playerIndex == 1)
		{
			color = Color.red;
		}
		if (_playerIndex == 2)
		{
			color = Color.green;
		}
		if (_playerIndex == 3)
		{
			color = Color.yellow;
		}
		if (_playerIndex == -1)
		{
			color = new Color(1, 1, 1, 1);
		}
	}
}
