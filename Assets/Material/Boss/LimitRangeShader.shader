Shader "Unlit/LimitRangeShader"
{
	Properties{
			_Color("Main Color", Color) = (1,1,1,1)
			_MainTex("Base (RGB)", 2D) = "white" {}
			_Coefficient("Coefficient", Float) = 200
	}

		SubShader{
			Tags { "RenderType" = "Transparent" "Queue" = "Overlay"}

			Pass {
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				 #include "UnityCG.cginc"

				struct appdata {
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};
				struct v2f {
					float4 pos : SV_POSITION;
					float4 sp : TEXCOORD0;
					float2 uv : TEXCOORD1;
				};

				float4 _MainTex_ST;

				v2f vert(appdata v) {
					v2f o;
					o.sp = UnityObjectToClipPos(v.vertex);
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				sampler2D _MainTex;
				fixed4 _Color;
				float _Coefficient;

				fixed4 frag(v2f i) : SV_Target {
					float2 s = i.sp.xy / i.sp.w * 0.5 + 0.5;
					s = 1.0f - s;
					//s.y *= 1080.0f / 1920.0f;
					//clip(0.5 + (sin(s.x) + sin(s.y)) * 0.25 - (1.0 - _Color.a));

					fixed4 col = tex2D(_MainTex, s);
					col.a = 0.1f;
					return col;
				}
				ENDCG
			}
			}
}
