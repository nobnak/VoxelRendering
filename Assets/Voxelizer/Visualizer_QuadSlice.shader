Shader "Unlit/Visualizer/QuadSlice" {
	Properties {
		_Depth ("Depth", Range(0,1)) = 0
		_VoxelTex ("Voxel Tex", 3D) = "" {}
		_Cutout ("Cutout Threshold", Range(0,1)) = 0.5
	}
	SubShader {
		Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" "IgnoreProjector"="True" }
		//Tags { "RenderType"="Opaque" }
		Cull Off

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Voxel.cginc"

			struct appdata {
				uint vid : SV_VertexID;
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float3 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float _Depth;
			float _Cutout;

			v2f vert (appdata v) {
				static uint INDICES[6] = { 0, 3, 1, 0, 2, 3 };
				static float2 UVS[4] = { float2(0,0), float2(1,0), float2(0,1), float2(1,1) };

				uint i = INDICES[v.vid];
				float2 uv = UVS[i];
				float4 clipPos = float4(2.0 * uv - 1.0, 1.0 - _Depth, 1.0);

				v2f o;
				o.vertex = clipPos;
				o.uv = float3(uv.x, (_ProjectionParams.x > 0 ? uv.y : 1 - uv.y), _Depth);
				return o;
			}
			fixed4 frag (v2f i) : SV_Target {
				fixed4 col = tex3D(_VoxelTex, i.uv);
				clip(col.a - _Cutout);
				return col;
			}
			ENDCG
		}
	}
}
