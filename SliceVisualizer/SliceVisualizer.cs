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

        var voxelBase = voxelBounds.NormalizedToLocalPosition (0f, 0f, 0f);
        var voxelRight = voxelBounds.NormalizedToLocalPosition (1f, 0f, 0f) - voxelBase;
        var voxelUp = voxelBounds.NormalizedToLocalPosition (0f, 1f, 0f) - voxelBase;
        var voxelForward = voxelBounds.NormalizedToLocalPosition (0f, 0f, 1f) - voxelBase;

        var uvToVoxelMat = Matrix4x4.zero;
		uvToVoxelMat.SetColumn (0, voxelRight);
		uvToVoxelMat.SetColumn (1, voxelUp);
        uvToVoxelMat.SetColumn (2, voxelForward);
		uvToVoxelMat.SetColumn (3, voxelBase);

		sliceByPointMat.SetFloat (shaderConstants.PROP_VERTEX_TO_DEPTH, 1f / depth);
        sliceByPointMat.SetMatrix (shaderConstants.PROP_UV_TO_VOXEL_MAT, uvToVoxelMat);
		sliceByPointMat.SetMatrix (shaderConstants.PROP_MODEL_MAT, transform.localToWorldMatrix);
		sliceByPointMat.SetTexture (shaderConstants.PROP_VOXEL_COLOR_TEX, voxelTex);
		sliceByPointMat.SetPass (0);
        Graphics.DrawProcedural (MeshTopology.Points, depth);
	}
    void OnDrawGizmos() {
        if (!IsInitialized || !viewSpaceBounds.IsInitialized)
            return;
        
        var view = (viewCamera == null ? Camera.current : viewCamera);
        viewSpaceBounds.ViewMatrix = view.transform.worldToLocalMatrix;
        viewSpaceBounds.ModelMatrix = transform.localToWorldMatrix;
        viewSpaceBounds.LocalSpaceColor = boundColor;
        viewSpaceBounds.ViewSpaceColor = boundColor;
        viewSpaceBounds.DrawGizmos ();
    }
	#endregion

	public void SetTexture(Texture tex) {
		this.voxelTex = tex;
        this.depth = tex.width;
	}
	public void Set(AbstractVoxelBounds voxelBounds) {
		this.voxelBounds = voxelBounds;

        viewSpaceBounds.voxelBounds = voxelBounds;
	}
	public bool IsInitialized {
		get { return voxelTex != null && voxelBounds != null && isActiveAndEnabled; }
	}
}
