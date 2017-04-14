#ifndef __VOXEL_INCLUDE__
#define __VOXEL_INCLUDE__


#define VOXEL_SIZE_VARIABLE _VoxelSize
#define VOXEL_TEX_VARIABLE _VoxelColorTex
#define VOXEL_NORMAL_TEX_VARIABLE _VoxelFaceTex


float4 VOXEL_SIZE_VARIABLE;

#ifdef VOXEL_CREATOR
RWTexture3D<float4> VOXEL_TEX_VARIABLE : register(u1);
RWTexture3D<float> VOXEL_NORMAL_TEX_VARIABLE : register(u2);
#else
sampler3D VOXEL_TEX_VARIABLE;
#endif


#endif