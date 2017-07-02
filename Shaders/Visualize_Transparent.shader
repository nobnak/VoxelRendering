Shader "Visualize/Transparent" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
        _DepthTexelOffset ("Depth Texel Offset", Range(0,1)) = 0
	}
	SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" }
        Cull Off ZWrite Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

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
            float _DepthTexelOffset;

			float _VertexToDepth;

            float4x4 _BoundsUvToVoxelUv;
			float4x4 _BoundsUVToLocal;
			float4x4 _BoundsModel;

			float4 BoundsUVToWorldPosition(float3 uvw) {
				float4 uv = float4(uvw.xy, uvw.z, 1);
				float3 localPos = mul(_BoundsUVToLocal, uv).xyz;
				return float4(mul(_BoundsModel, float4(localPos, 1)).xyz, 1);
			}
            float3 BoundsUVToVoxelUV(float3 boundsUv) {
                return mul(_BoundsUvToVoxelUv, float4(boundsUv, 1)).xyz;
            }

			gsin vert (appdata v) {
                float w = saturate(1.0 - v.vid * _VertexToDepth) + _DepthTexelOffset * _VertexToDepth;

				gsin o;
                o.w = w;
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
					float4 worldPos = BoundsUVToWorldPosition(uv);
					o.uv = BoundsUVToVoxelUV(uv);
					o.vertex = mul(UNITY_MATRIX_VP, float4(worldPos.xyz, 1));
					output.Append(o);

					if ((i % 3) == 2)
						output.RestartStrip();
				}
			}

			fixed4 frag (psin i) : SV_Target {
				float4 c = tex3D(VOXEL_COLOR_TEX_VARIABLE, i.uv) * _Color;
                return c;
			}
			ENDCG
		}
	}
}
