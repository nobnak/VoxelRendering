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
    public const string UV_TO_VOXEL_MAT = "_UVToVoxelMat";
	public const string MODEL_MAT = "_ModelMat";

	public readonly int PROP_VOXEL_SIZE;
	public readonly int PROP_VOXEL_COLOR_TEX;
	public readonly int PROP_VOXEL_FACE_TEX;
	public readonly int PROP_VOXEL_ROTATION_MAT;

	public readonly int PROP_VERTEX_TO_DEPTH;
    public readonly int PROP_UV_TO_VOXEL_MAT;
	public readonly int PROP_MODEL_MAT;

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
        PROP_UV_TO_VOXEL_MAT = Shader.PropertyToID (UV_TO_VOXEL_MAT);
		PROP_MODEL_MAT = Shader.PropertyToID (MODEL_MAT);
	}
}
