using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gist;
using Gist.Extensions.AABB;

[ExecuteInEditMode]
public class SliceVisualizer : MonoBehaviour {

	public int depth = 64;

	[SerializeField]
	Material sliceByPointMat;
    [SerializeField]
    Color boundColor = Color.green;
    [SerializeField]
    Camera viewCamera;

	Texture voxelTex;
	AbstractVoxelBounds voxelBounds;
	ShaderConstants shaderConstants;
    ViewSpaceBounds viewSpaceBounds;

	#region Unity
	void OnEnable() {
		shaderConstants = ShaderConstants.Instance;
        viewSpaceBounds = new ViewSpaceBounds ();
	}
	void OnRenderObject() {
		if (!IsInitialized)
			return;

        var voxelUvToLocalMat = voxelBounds.VoxelUvToLocalMatrix ();

		sliceByPointMat.SetFloat (shaderConstants.PROP_VERTEX_TO_DEPTH, 1f / depth);
        sliceByPointMat.SetMatrix (shaderConstants.PROP_UV_TO_VOXEL_MAT, voxelUvToLocalMat);
		sliceByPointMat.SetMatrix (shaderConstants.PROP_MODEL_MAT, transform.localToWorldMatrix);
		sliceByPointMat.SetTexture (shaderConstants.PROP_VOXEL_COLOR_TEX, voxelTex);
		sliceByPointMat.SetPass (0);
        Graphics.DrawProcedural (MeshTopology.Points, depth);
	}
    void OnDrawGizmos() {
        if (!IsInitialized || voxelBounds == null)
            return;
        
        viewSpaceBounds.DrawGizmos (voxelBounds, boundColor, boundColor);
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
