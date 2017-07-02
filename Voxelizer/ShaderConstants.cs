using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderConstants {
	public const string RESULT = "Result";

	public const string VOXEL_SIZE = "_VoxelSize";
	public const string VOXEL_COLOR_TEX = "_VoxelColorTex";
	public const string VOXEL_FACE_TEX = "_VoxelFaceTex";
	public const string VOXEL_ROTATION_MAT = "_VoxelRotationMat";

	public const string VERTEX_TO_DEPTH = "_VertexToDepth";
    public const string BOUNDS_UV_TO_VOXEL_UV = "_BoundsUvToVoxelUv";
    public const string BOUNDS_UV_TO_LOCAL_MAT = "_BoundsUVToLocal";
    public const string BOUNDS_MODEL = "_BoundsModel";

	public readonly int PROP_VOXEL_SIZE;
	public readonly int PROP_VOXEL_COLOR_TEX;
	public readonly int PROP_VOXEL_FACE_TEX;
	public readonly int PROP_VOXEL_ROTATION_MAT;

	public readonly int PROP_VERTEX_TO_DEPTH;
    public readonly int PROP_BOUNDS_UV_TO_VOXEL_UV;
    public readonly int PROP_BOUNDS_UV_TO_LOCAL;
	public readonly int PROP_BOUNDS_MODEL;

	static ShaderConstants instance;

	public static ShaderConstants Instance {
		get { return instance == null ? (instance = new ShaderConstants ()) : instance; }
	}

	public ShaderConstants() {
		PROP_VOXEL_SIZE = Shader.PropertyToID (VOXEL_SIZE);
		PROP_VOXEL_COLOR_TEX = Shader.PropertyToID (VOXEL_COLOR_TEX);
		PROP_VOXEL_FACE_TEX = Shader.PropertyToID (VOXEL_FACE_TEX);
		PROP_VOXEL_ROTATION_MAT = Shader.PropertyToID (VOXEL_ROTATION_MAT);

		PROP_VERTEX_TO_DEPTH = Shader.PropertyToID (VERTEX_TO_DEPTH);
        PROP_BOUNDS_UV_TO_VOXEL_UV = Shader.PropertyToID (BOUNDS_UV_TO_VOXEL_UV);
        PROP_BOUNDS_UV_TO_LOCAL = Shader.PropertyToID (BOUNDS_UV_TO_LOCAL_MAT);
		PROP_BOUNDS_MODEL = Shader.PropertyToID (BOUNDS_MODEL);
	}
}
