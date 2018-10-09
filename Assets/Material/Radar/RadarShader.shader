Shader "Custom/RadarShader"  {
	Properties{
			_Color("Main Color", Color) = (1,1,1,1)
	}

	SubShader{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Stencil {
			Ref 1 
			Comp NotEqual	// 一度レーダーが描画されている範囲は無視して色を重ねない
			Pass Replace    // リファレンス値をバッファに書き込みます。
		}

		Pass {

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag alpha:blend
			struct appdata {
				float4 vertex : POSITION;
			};
			struct v2f {
				float4 pos : SV_POSITION;
			};
			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 _Color;

			fixed4 frag(float4 sp:WPOS) : SV_Target {
				return _Color;
			}
			ENDCG
		}
	}
}