using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SliceVisualizer : MonoBehaviour {

	public int depth = 64;

	[SerializeField]
	Material sliceByPointMat;
	Texture voxelTex;
	AbstractVoxelBounds voxelBounds;
	ShaderConstants shaderConstants;

	#region Unity
	void OnEnable() {
		shaderConstants = ShaderConstants.Instance;
	}
	void OnRenderObject() {
		if (!IsInitialized)
			return;

		var nearBase = voxelBounds.NormalizedToLocalPosition (0f, 0f, 0f);
		var nearRight = voxelBounds.NormalizedToLocalPosition (1f, 0f, 0f) - nearBase;
		var nearUp = voxelBounds.NormalizedToLocalPosition (0f, 1f, 0f) - nearBase;
		var farBase = voxelBounds.NormalizedToLocalPosition (0f, 0f, 1f);
		var farRight = voxelBounds.NormalizedToLocalPosition (1f, 0f, 1f) - farBase;
		var farUp =voxelBounds.NormalizedToLocalPosition  (0f, 1f, 1f) - farBase;

		var uvToNearMat = Matrix4x4.zero;
		uvToNearMat.SetColumn (0, nearRight);
		uvToNearMat.SetColumn (1, nearUp);
		uvToNearMat.SetColumn (3, nearBase);

		var uvToFarMat = Matrix4x4.zero;
		uvToFarMat.SetColumn (0, farRight);
		uvToFarMat.SetColumn (1, farUp);
		uvToFarMat.SetColumn (3, farBase);

		sliceByPointMat.SetFloat (shaderConstants.PROP_VERTEX_TO_DEPTH, 1f / depth);
		sliceByPointMat.SetMatrix (shaderConstants.PROP_UV_TO_NEAR, uvToNearMat);
		sliceByPointMat.SetMatrix (shaderConstants.PROP_UV_TO_FAR, uvToFarMat);
		sliceByPointMat.SetMatrix (shaderConstants.PROP_MODEL_MAT, transform.localToWorldMatrix);
		sliceByPointMat.SetTexture (shaderConstants.PROP_VOXEL_COLOR_TEX, voxelTex);
		sliceByPointMat.SetPass (0);
        Graphics.DrawProcedural (MeshTopology.Points, depth);
	}
	#endregion

	public void SetTexture(Texture tex) {
		this.voxelTex = tex;
        this.depth = tex.width;
	}
	public void Set(AbstractVoxelBounds voxelBounds) {
		this.voxelBounds = voxelBounds;
	}
	public bool IsInitialized {
		get { return voxelTex != null && voxelBounds != null && isActiveAndEnabled; }
	}
}
