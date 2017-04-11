using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SliceVisualizer : MonoBehaviour {
	public const string VERTEX_TO_DEPTH = "_VertexToDepth";
	public const string VOXEL_TEX = "_VoxelTex";
	public const string UV_TO_NEAR = "_UVToNearPlaneMat";
	public const string UV_TO_FAR = "_UVToFarPlaneMat";

	public int depth = 64;
	public Camera frustumCam;

	int PROP_VERTEX_TO_DEPTH;
	int PROP_VOXEL_TEX;
	int PROP_UV_TO_NEAR;
	int PROP_UV_TO_FAR;

	[SerializeField]
	Material sliceByPointMat;
	Texture voxelTex;

	#region Unity
	void OnEnable() {
		PROP_VERTEX_TO_DEPTH = Shader.PropertyToID (VERTEX_TO_DEPTH);
		PROP_VOXEL_TEX = Shader.PropertyToID (VOXEL_TEX);
		PROP_UV_TO_NEAR = Shader.PropertyToID (UV_TO_NEAR);
		PROP_UV_TO_FAR = Shader.PropertyToID (UV_TO_FAR);
	}
	void OnRenderObject() {
		if (!IsInitialized)
			return;
		
		var nearPlane = frustumCam.nearClipPlane;
		var farPlane = frustumCam.farClipPlane;
		var nearBase = ViewportToLocalPosition (new Vector3 (0f, 0f, nearPlane));
		var nearRight = ViewportToLocalPosition (new Vector3 (1f, 0f, nearPlane)) - nearBase;
		var nearUp = ViewportToLocalPosition (new Vector3 (0f, 1f, nearPlane)) - nearBase;
		var farBase = ViewportToLocalPosition (new Vector3 (0f, 0f, farPlane));
		var farRight = ViewportToLocalPosition (new Vector3 (1f, 0f, farPlane)) - farBase;
		var farUp = ViewportToLocalPosition (new Vector3 (0f, 1f, farPlane)) - farBase;

		var uvToNearMat = Matrix4x4.zero;
		uvToNearMat.SetColumn (0, nearRight);
		uvToNearMat.SetColumn (1, nearUp);
		uvToNearMat.SetColumn (3, nearBase);

		var uvToFarMat = Matrix4x4.zero;
		uvToFarMat.SetColumn (0, farRight);
		uvToFarMat.SetColumn (1, farUp);
		uvToFarMat.SetColumn (3, farBase);

		sliceByPointMat.SetFloat (PROP_VERTEX_TO_DEPTH, 1f / (depth + 1f));
		sliceByPointMat.SetMatrix (PROP_UV_TO_NEAR, uvToNearMat);
		sliceByPointMat.SetMatrix (PROP_UV_TO_FAR, uvToFarMat);
		sliceByPointMat.SetTexture (PROP_VOXEL_TEX, voxelTex);
		sliceByPointMat.SetPass (0);
		Graphics.DrawProcedural (MeshTopology.Points, depth);
	}
	#endregion

	public void SetTexture(Texture tex) {
		voxelTex = tex;
	}
	public bool IsInitialized {
		get { return frustumCam != null && isActiveAndEnabled; }
	}
	public Vector3 ViewportToLocalPosition(Vector3 viewportPos) {
		var worldPos = frustumCam.ViewportToWorldPoint (viewportPos);
		return frustumCam.transform.InverseTransformPoint (worldPos);
	}
}
