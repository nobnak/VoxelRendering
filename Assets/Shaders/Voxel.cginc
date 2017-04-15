#ifndef __VOXEL_INCLUDE__
#define __VOXEL_INCLUDE__

#define VOXEL_SIZE_VARIABLE _VoxelSize
#define VOXEL_COLOR_TEX_VARIABLE _VoxelColorTex
#define VOXEL_NORMAL_TEX_VARIABLE _VoxelFaceTex

float4 VOXEL_SIZE_VARIABLE;

float3 NormalizedFromClipPosition(float4 clipPos) {
	float3 ndcPos = clipPos.xyz / clipPos.w;
	float3 normalizedPos = float3(0.5 * (ndcPos.xy + 1.0), 1.0 - ndcPos.z);
	normalizedPos.y = (_ProjectionParams.x > 0 ? normalizedPos.y : 1.0 - normalizedPos.y);
	return normalizedPos;
}

uint3 VoxelFromNormalizedPosition(float3 normalizedPos) {
	float3 pixelPos = _VoxelSize.xyz * normalizedPos;
	uint3 id = (uint3)floor(pixelPos);
	return id;
}
uint3 VoxelFromClipPosition(float4 clipPos) {
	float3 normalizedPos = NormalizedFromClipPosition(clipPos);
	return VoxelFromNormalizedPosition(normalizedPos);
}



#ifdef VOXEL_CREATOR



RWTexture3D<float4> VOXEL_COLOR_TEX_VARIABLE : register(u1);
RWTexture3D<float> VOXEL_NORMAL_TEX_VARIABLE : register(u2);

void StoreResultByID(uint3 id, float4 resultColor, float3 resultNormal) {
	VOXEL_COLOR_TEX_VARIABLE[id] = resultColor;
	VOXEL_NORMAL_TEX_VARIABLE[id] = abs(dot(resultNormal, float3(0,0,1)));
}
void StoreResultByClipPos(float4 clipPos, float4 resultColor, float3 resultNormal) {
	uint3 id = VoxelFromClipPosition(clipPos);
	StoreResultByID(id, resultColor, resultNormal);
}



#else



sampler3D VOXEL_COLOR_TEX_VARIABLE;
sampler3D VOXEL_NORMAL_TEX_VARIABLE;



#endif
#endif