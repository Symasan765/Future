using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Word : MonoBehaviour {
	[SerializeField]
	private bool EndAnimation = false;
	[SerializeField]
	private int LifeTime = 0;
	[SerializeField]
	private int DerayTime = 0;
	[SerializeField]
	private bool FadeIn = true;
	[SerializeField]
	private float FadeInSpeed = 0.1f;
	[SerializeField]
	private bool FadeOut = true;
	[SerializeField]
	private float FadeOutSpeed = 0.1f;
	[SerializeField]
	private Vector2 ShakePower = new Vector2(0, 0);
	[SerializeField]
	private int ShakeSpeed = 1;
	[SerializeField]
	private Vector3 StartScale = new Vector3(1, 1, 1);
	[SerializeField]
	private bool ScaleFadeIn = true;
	[SerializeField]
	private Vector3 ScaleFadeInSpeed = new Vector3(0.1f, 0.1f, 0.1f);
	[SerializeField]
	private bool ScaleFadeOut = true;
	[SerializeField]
	private float ScaleFadeOutSpeed = 0.1f;
	[SerializeField]
	private bool ScaleShake = true;
	[SerializeField]
	private Vector2 ScaleShakePower = new Vector2(1, 1);
	[SerializeField]
	private int ScaleShakeSpeed = 1;
	[SerializeField]
	private float ScaleImpactPower = 1.4f;

	private WordParent wordParent;
	private SpriteRenderer spriteRenderer;

	[SerializeField]private bool flgPlay = false;

	private int cntLifeTime = 0;
	private int cntDerayTime = 0;
	private float cntFadeOutTime = 0;
	private int cntScaleShakeTime = 0;
	private Color color;
	private Vector3 startPosition;
	private Vector3 nowScale;
	private bool isFadeIn = false;
	// Use this for initialization
	void Start ()
	{
		nowScale = StartScale;
		startPosition = transform.localPosition;
		spriteRenderer = GetComponent<SpriteRenderer>();
		wordParent = transform.parent.GetComponent<WordParent>();
		color = spriteRenderer.color;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector2 scaleShakePower = new Vector2(1.0f,1.0f);

		if (ShakeSpeed < 1)
		{
			ShakeSpeed = 1;
		}

		if (flgPlay)
		{
			//描画開始までの空白期間
			if (cntDerayTime < DerayTime)
			{
				cntDerayTime++;
			}
			//描画開始
			if (cntDerayTime == DerayTime)
			{
				spriteRenderer.enabled = true;
				//フェードイン
				if (cntLifeTime < LifeTime)
				{
					if (color.a < 1.0f)
					{
						isFadeIn = true;
						color.a += FadeInSpeed;
						if (color.a >= 1.0f)
						{
							isFadeIn = false;
							color.a = 1.0f;
						}
					}

					//拡大フェードイン
					if (ScaleFadeIn)
					{
						if (nowScale.x < 1.0f)
						{
							isFadeIn = true;
							nowScale.x += ScaleFadeInSpeed.x;
							if (nowScale.x >= 1.0f)
							{
								isFadeIn = false;
								nowScale.x = 1.0f;
							}
						}
						if (nowScale.y < 1.0f)
						{
							isFadeIn = true;
							nowScale.y += ScaleFadeInSpeed.y;
							if (nowScale.y >= 1.0f)
							{
								isFadeIn = false;
								nowScale.y = 1.0f;
							}
						}
						if (nowScale.z < 1.0f)
						{
							isFadeIn = true;
							nowScale.z += ScaleFadeInSpeed.z;
							if (nowScale.z >= 1.0f)
							{
								isFadeIn = false;
								nowScale.z = 1.0f;
							}
						}
					}

				}

				//拡縮
				if (!isFadeIn && ScaleShake)
				{
					if (cntLifeTime < LifeTime)
					{
						if (cntScaleShakeTime < ScaleShakeSpeed / 2)
						{
							scaleShakePower = ScaleShakePower;
						} else
						{		
							scaleShakePower = new Vector2(1, 1);
						}
						cntScaleShakeTime++;
						if (cntScaleShakeTime >= ScaleShakeSpeed)
						{
							cntScaleShakeTime = 0;
						}
					} else
					{
						//最後少し大きく見せる
						if (ScaleImpactPower != 1 && cntFadeOutTime == 0.0f)
						{
							nowScale = nowScale * ScaleImpactPower;
							scaleShakePower = new Vector2(1, 1);
						}
					}
				} else
				{
					scaleShakePower = new Vector2(1, 1);
				}

				//文字を振動
				if (Time.frameCount % ShakeSpeed == 0)
				{
					transform.localPosition = new Vector3(startPosition.x + Random.Range(ShakePower.x / 2 * -1, ShakePower.x / 2), startPosition.y + Random.Range(ShakePower.y / 2 * -1, ShakePower.y / 2), startPosition.z);
				}

				//描画時間終了
				if (cntLifeTime == LifeTime)
				{
					//縮小
					if (ScaleFadeOut)
					{
						nowScale = Vector3.Lerp(nowScale, new Vector3(0, 0, 0), cntFadeOutTime);
						cntFadeOutTime += ScaleFadeOutSpeed;
					}
					//フェードアウト
					if (FadeOut)
					{
						if (color.a > 0)
						{
							color.a -= FadeOutSpeed;
							if (color.a <= 0)
							{
								color.a = 0;
							}
						}
					}

					//描画終了
					if (FadeOut || ScaleFadeOut)
					{
						if (cntFadeOutTime >= 1.0f)
						{
							if (EndAnimation)
							{
								wordParent.DestroyObject();
							}
							flgPlay = false;
						}
					} else
					{
						if (EndAnimation)
						{
							wordParent.DestroyObject();
						}
						flgPlay = false;
					}

				}
				//描画時間をカウント
				if (cntLifeTime < LifeTime)
				{
					cntLifeTime++;
				}
			}
		} else
		{
			flgPlay = wordParent.IsPlay();
			if (ScaleFadeIn)
			{
				nowScale = StartScale;
			} else
			{
				nowScale = new Vector3(1, 1, 1);
			}
			cntFadeOutTime = 0;
			cntDerayTime = 0;
			cntLifeTime = 0;
			color = wordParent.GetColor();
			//フェードアウト用設定
			if (FadeIn)
			{
				color.a = 0;
			} else
			{
				color.a = 1.0f;
			}
			spriteRenderer.enabled = false;
		}
		transform.localScale = new Vector3(nowScale.x * scaleShakePower.x, nowScale.y * scaleShakePower.y, nowScale.z);
		spriteRenderer.color = color;
	}
}
