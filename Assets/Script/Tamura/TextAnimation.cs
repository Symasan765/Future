using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextAnimation : MonoBehaviour {
    public enum TextAnimType {
        ONE_BY_ONE,     // 一文字づつ順番に表示する
        UPPER_DIFFUSION // 上方向に拡散して消えていく
    }

    private TextMeshPro textMeshPro;    // テキストメッシュプロのコンポーネント

    public TextAnimType textAnimType;   // アニメーションのしかた指定
    [SerializeField]
    private string text;    // 表示したいテキストの指定
    [SerializeField]
    private int animSpeed;  // アニメーションさせる速度の指定

	void Start () {
        textMeshPro = GetComponent<TextMeshPro>();
        textMeshPro.text = "test";
	}
	
	void Update () {
        TextAnim();
	}

    void TextAnim() {
        switch (textAnimType) {
            case TextAnimType.ONE_BY_ONE:
                AnimOneByOne();
                break;

            case TextAnimType.UPPER_DIFFUSION:
                AnimUpperDiffusion();
                break;
        }
    }

    void AnimOneByOne() {

    }

    void AnimUpperDiffusion() {

    }
}
