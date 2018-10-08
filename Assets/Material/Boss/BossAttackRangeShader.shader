// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/BossAttackRangeShader" {
	Properties{
			_Color("Main Color", Color) = (1,1,1,1)
			_MainTex("Base (RGB)", 2D) = "white" {}
			_Coefficient("Coefficient", Float) = 200
	}

		SubShader{
			Tags { "RenderType" = "Transparent" "Queue" = "Overlay"}

			Stencil {
				Ref 1  // リファレンス値
				Comp Equal // ピクセルのリファレンス値がバッファの値と等しい場合のみレンダリングします。
			}

			Pass {
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				struct appdata {
					float4 vertex : POSITION;
				};
				struct v2f {
					float4 pos : SV_POSITION;
					float4 sp : TEXCOORD0;
				};

				v2f vert(appdata v) {
					v2f o;
					o.sp = UnityObjectToClipPos(v.vertex);
					o.pos = UnityObjectToClipPos(v.vertex);
					return o;
				}

				sampler2D _MainTex;
				fixed4 _Color;
				float _Coefficient;

				fixed4 frag(v2f i) : SV_Target {

					float2 s = i.sp.xy / i.sp.w * _Coefficient;
					s.y *= 1080.0f / 1920.0f;
					clip(0.5 + (sin(s.x) + sin(s.y)) * 0.25 - (1.0 - _Color.a));

					return fixed4(_Color.r, _Color.g, _Color.b, 1.0f);
				}
				ENDCG
			}
	}
}