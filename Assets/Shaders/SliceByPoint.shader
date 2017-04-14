Shader "Unlit/SliceByPoint" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Cutout ("Cutout Threshold", Range(0,1)) = 0.5
	}
	SubShader {
		Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" "IgnoreProjector"="True" }
		Cull Off ZWrite On ZTest LEqual

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom
			
			#include "UnityCG.cginc"
            #include "Voxel.cginc"
			struct appdata {
				uint vid : SV_VertexID;
			};

			struct gsin {
				float w : TEXCOORD0;
			};

			struct psin {
				float3 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float4 _Color;
			float _Cutout;

			float _VertexToDepth;
			sampler3D _VoxelTex;

			float4x4 _UVToNearPlaneMat;
			float4x4 _UVToFarPlaneMat;
			float4x4 _ModelMat;

			float4 UVWToWorldPosition(float3 uvw) {
				float4 uv = float4(uvw.xy, 0, 1);
				float3 localPos = lerp(mul(_UVToNearPlaneMat, uv), mul(_UVToFarPlaneMat, uv), uvw.z).xyz;
				return float4(mul(_ModelMat, float4(localPos, 1)).xyz, 1);
			}

			gsin vert (appdata v) {
				gsin o;
				o.w = v.vid * _VertexToDepth;
				return o;
			}

			[maxvertexcount(6)]
			void geom(point gsin input[1], inout TriangleStream<psin> output) {
				static float2 UV[4] = { float2(0,0), float2(1,0), float2(0,1), float2(1,1) };
				static uint INDICES[6] = { 0, 3, 1, 0, 2, 3 };

				float w = input[0].w;

				psin o;
				for (uint i = 0; i < 6; i++) {
					uint j = INDICES[i];
					float3 uv = float3(UV[j], w);
					float4 worldPos = UVWToWorldPosition(uv);
					o.uv = uv;
					o.vertex = mul(UNITY_MATRIX_VP, float4(worldPos.xyz, 1));
					output.Append(o);

					if ((i % 3) == 2)
						output.RestartStrip();
				}
			}

			fixed4 frag (psin i) : SV_Target {
				float4 c = tex3D(_VoxelTex, i.uv);
                float f = tex3D(VOXEL_NORMAL_TEX_VARIABLE, i.uv);
				clip(c.a - _Cutout);
				return f * _Color;
			}
			ENDCG
		}
	}
}
