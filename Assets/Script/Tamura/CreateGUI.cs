using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// CanvasRendererとRectTransformを強制的に付けるようにする
[RequireComponent( typeof( CanvasRenderer ) )]
[RequireComponent( typeof( RectTransform ) )]
public class CreateGUI : Graphic {
    [SerializeField, HeaderAttribute("左上頂点の座標")]
    Vector3 ltPos;
    Vector2 ltUV;

    [SerializeField, HeaderAttribute("左下頂点の座標")]
    Vector3 lbPos;
    Vector2 lbUV;

    [SerializeField, HeaderAttribute("右上頂点の座標")]
    Vector3 rtPos;
    Vector2 rtUV;

    [SerializeField, HeaderAttribute("右下頂点の座標")]
    Vector3 rbPos;
    Vector2 rbUV;

    [SerializeField]
    float TexScale;
    [SerializeField]
    float UOffset;
    [SerializeField]
    float VOffset;

    // テクスチャ(今回は空だけど必要)
    Texture texture_;

    // メッシュに指定するテクスチャを渡す
    public override Texture mainTexture {
        get {
            // ここで設定したいテクスチャを返すようにする
            return texture_;
        }
    }

    void Update() {
        ltUV = Vector2.up;
        lbUV = Vector2.zero;
        rtUV = Vector2.one;
        rbUV = Vector2.right;

        // 左上と右上の頂点が横並びかどうか
        if (ltPos.y == rtPos.y) {
            lbUV.y = 1 - (ltPos.y - lbPos.y) / 350 / TexScale;
            rbUV.y = 1 - (rtPos.y - rbPos.y) / 350 / TexScale;
        }

        // 左下と右下の頂点が横並びかどうか
        if (lbPos.y == rbPos.y) {
            ltUV.y = (ltPos.y - lbPos.y) / 350 / TexScale;
            rtUV.y = (rtPos.y - rbPos.y) / 350 / TexScale;
        }

        // Vをオフセット分移動
        ltUV.y = ltUV.y - VOffset;
        lbUV.y = lbUV.y - VOffset;
        rtUV.y = rtUV.y - VOffset;
        rbUV.y = rbUV.y - VOffset;

        // Uも同様に縮めて移動
        ltUV.x = ltUV.x - UOffset;
        lbUV.x = lbUV.x - UOffset;
        rtUV.x = rtUV.x / TexScale - UOffset;
        rbUV.x = rbUV.x / TexScale - UOffset;
    }

    // OnPopulateMeshをオーバーライドして頂点を自由に設定できるように
    protected override void OnPopulateMesh(VertexHelper vh) {
        vh.Clear();

        // 左上
        UIVertex lt = new UIVertex();
        lt.position = ltPos;
        lt.uv0 = ltUV;

        // 右上
        UIVertex rt = new UIVertex();
        rt.position = rtPos;
        rt.uv0 = rtUV;

        // 右下
        UIVertex rb = new UIVertex();
        rb.position = rbPos;
        rb.uv0 = rbUV;

        // 左下
        UIVertex lb = new UIVertex();
        lb.position = lbPos;
        lb.uv0 = lbUV;

        // 四角形として頂点情報を追加
        vh.AddUIVertexQuad(new UIVertex[] { lt, rt, rb, lb });
    }
}
