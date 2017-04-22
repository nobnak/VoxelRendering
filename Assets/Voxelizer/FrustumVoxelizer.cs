using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gist;

[ExecuteInEditMode]
public class FrustumVoxelizer : MonoBehaviour {
	public const float NEAR_DISTANCE = 10f;

	public TextureEvent OnCreateVoxelTexture;
	public VoxelBoundsEvent VoxelBoundsOnChange;
	
	public Bounds bounds = new Bounds (Vector3.zero, 10f * Vector3.one);
	public Color boundsColor = Color.green;

	public int prefferedVoxelResolution = 512;
	public FilterMode voxelFilterMode = FilterMode.Bilinear;

	public ComputeShader clearCompute;
	public Shader voxelShader;

	VoxelTexture colorTex;
	VoxelTexture faceTex;
	VoxelTextureCleaner cleaner;

	ShaderConstants shaderConstants;
	ManuallyRenderCamera renderCam;
	TransformVoxelBounds voxelBounds;
    VoxelCameraDirection cameraDirection;

	#region Unity
	void OnEnable() {
		shaderConstants = ShaderConstants.Instance;

		voxelBounds = new TransformVoxelBounds (transform);
		voxelBounds.Changed += (obj) => {
			Debug.Log("Voxel Bounds changed");
			VoxelBoundsOnChange.Invoke (obj);
		};
        cameraDirection = new VoxelCameraDirection ();
        renderCam = new ManuallyRenderCamera ((cam) => cameraDirection.FitCameraToVoxelBounds (cam, voxelBounds));

		colorTex = new VoxelTexture (prefferedVoxelResolution, RenderTextureFormat.ARGB32);
		colorTex.OnCreateVoxelTexture += (v) => {
			OnCreateVoxelTexture.Invoke(v.Texture);
		};

		faceTex = new VoxelTexture (prefferedVoxelResolution, RenderTextureFormat.R8);

		cleaner = new VoxelTextureCleaner (clearCompute, 0, ShaderConstants.RESULT);

		Init ();
	}
	void Update() {
		Init ();
		Clear ();
		Shader.SetGlobalVector (shaderConstants.PROP_VOXEL_SIZE, colorTex.ResolutionVector);
		Shader.SetGlobalTexture (shaderConstants.PROP_VOXEL_COLOR_TEX, colorTex.Texture);
		Shader.SetGlobalTexture (shaderConstants.PROP_VOXEL_FACE_TEX, faceTex.Texture);
        Shader.SetGlobalMatrix (shaderConstants.PROP_VOXEL_ROTATION_MAT, cameraDirection.VoxelDirection);
		Render();
	}
	void OnDrawGizmos() {
		if (!isActiveAndEnabled)
			return;

		Gizmos.color = boundsColor;
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireCube (bounds.center, bounds.size);
		Gizmos.matrix = Matrix4x4.identity;
	}
	void OnDisable() {
		colorTex.Dispose ();
		renderCam.Dispose ();
	}
	#endregion

	public AbstractVoxelBounds VoxelBounds { get { return voxelBounds; } }

	void Init () {
		voxelBounds.LocalBounds = bounds;
		voxelBounds.Update ();

		colorTex.SetResolution(prefferedVoxelResolution);
		faceTex.SetResolution (prefferedVoxelResolution);
	}
	void Clear () {
		cleaner.Clear (colorTex.Texture);
		cleaner.Clear (faceTex.Texture);
	}
	void Render () {
		var targetTex = RenderTexture.GetTemporary (colorTex.CurrentResolution, colorTex.CurrentResolution);
		try {
			Graphics.SetRandomWriteTarget (1, colorTex.Texture);
	        Graphics.SetRandomWriteTarget (2, faceTex.Texture);
			renderCam.RenderWithShader (targetTex, voxelShader, null);
			Graphics.ClearRandomWriteTargets ();
		} finally {
			RenderTexture.ReleaseTemporary (targetTex);
		}
	}
}
