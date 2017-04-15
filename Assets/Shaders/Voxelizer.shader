Shader "Unlit/Voxelizer" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		ZTest Always ZWrite Off
		Cull Off

		Pass {
			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			#define VOXEL_CREATOR
			#include "Voxel.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
				float4 pos : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = mul(UNITY_MATRIX_IT_MV, float4(v.normal, 0)).xyz;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.pos = o.vertex;
				return o;
			}
			
			float4 frag (v2f i) : COLOR {
				uint3 id = VoxelFromClipPosition(i.pos);
				fixed4 col = tex2D(_MainTex, i.uv);
				float3 n = normalize(i.normal);

				StoreResult(id, col, n);
				return col;
			}
			ENDCG
		}
	}
}
