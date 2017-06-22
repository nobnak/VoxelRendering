using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gist;

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

        var voxelBase = voxelBounds.NormalizedToLocalPosition (0f, 0f, 0f);
        var voxelRight = voxelBounds.NormalizedToLocalPosition (1f, 0f, 0f) - voxelBase;
        var voxelUp = voxelBounds.NormalizedToLocalPosition (0f, 1f, 0f) - voxelBase;
        var voxelForward = voxelBounds.NormalizedToLocalPosition (0f, 0f, 1f) - voxelBase;

		var uvToNearMat = Matrix4x4.zero;
		uvToNearMat.SetColumn (0, voxelRight);
		uvToNearMat.SetColumn (1, voxelUp);
        uvToNearMat.SetColumn (2, voxelForward);
		uvToNearMat.SetColumn (3, voxelBase);

		sliceByPointMat.SetFloat (shaderConstants.PROP_VERTEX_TO_DEPTH, 1f / depth);
        sliceByPointMat.SetMatrix (shaderConstants.PROP_UV_TO_VOXEL_MAT, uvToNearMat);
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
