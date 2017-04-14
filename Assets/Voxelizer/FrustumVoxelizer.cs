using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FrustumVoxelizer : MonoBehaviour {
	public const string RESULT = "Result";
	public const string VOXEL_SIZE = "_VoxelSize";
	public const string VOXEL_COLOR_TEX = "_VoxelColorTex";
	public const string VOXEL_FACE_TEX = "_VoxelFaceTex";

	int PROP_VOXEL_SIZE;
	int PROP_VOXEL_COLOR_TEX;
	int PROP_VOXEL_FACE_TEX;

	public TextureEvent OnCreateVoxelTexture;

	public int prefferedVoxelResolution = 512;
	public FilterMode voxelFilterMode = FilterMode.Bilinear;

	public ComputeShader clearCompute;
	public Shader voxelShader;

	Camera targetCam;
	VoxelTexture colorTex;
	VoxelTexture faceTex;
	VoxelTextureCleaner cleaner;

	#region Unity
	void OnEnable() {
		PROP_VOXEL_SIZE = Shader.PropertyToID(VOXEL_SIZE);
		PROP_VOXEL_COLOR_TEX = Shader.PropertyToID(VOXEL_COLOR_TEX);
		PROP_VOXEL_FACE_TEX = Shader.PropertyToID (VOXEL_FACE_TEX);

		targetCam = GetComponent<Camera> ();
		targetCam.enabled = false;

		colorTex = new VoxelTexture (prefferedVoxelResolution, RenderTextureFormat.ARGB32);
		colorTex.OnCreateVoxelTexture += (v) => {
			OnCreateVoxelTexture.Invoke(v.Texture);
			ReleaseCameraTargetTexture();
			CreateCameraTargetTexture();
		};

		faceTex = new VoxelTexture (prefferedVoxelResolution, RenderTextureFormat.R8);

        cleaner = new VoxelTextureCleaner (clearCompute, 0, RESULT);

		Init ();
	}
	void Update() {
		Init ();
		Clear ();
		Shader.SetGlobalVector (PROP_VOXEL_SIZE, colorTex.ResolutionVector);
		Shader.SetGlobalTexture (PROP_VOXEL_COLOR_TEX, colorTex.Texture);
		Shader.SetGlobalTexture (PROP_VOXEL_FACE_TEX, faceTex.Texture);
		Render();
	}
	void OnDisable() {
		colorTex.Dispose ();
		ReleaseCameraTargetTexture ();
	}
	#endregion

	void Init () {
		colorTex.SetResolution(prefferedVoxelResolution);
		faceTex.SetResolution (prefferedVoxelResolution);
	}
	void Clear () {
		cleaner.Clear (colorTex.Texture);
		cleaner.Clear (faceTex.Texture);
	}
	void Render () {
		Graphics.SetRandomWriteTarget (1, colorTex.Texture);
        Graphics.SetRandomWriteTarget (2, faceTex.Texture);
		targetCam.RenderWithShader (voxelShader, null);
		Graphics.ClearRandomWriteTargets ();
	}
	void Release<T>(ref T resource) where T : Object {
		if (Application.isPlaying)
			Object.Destroy (resource);
		else
			Object.DestroyImmediate (resource);
	}

	void ReleaseCameraTargetTexture () {
		var tex = targetCam.targetTexture;
		targetCam.targetTexture = null;
		RenderTexture.ReleaseTemporary (tex);
	}
	void CreateCameraTargetTexture() {
		var w = colorTex.CurrentResolution;
		targetCam.targetTexture = RenderTexture.GetTemporary (w, w, 0);
	}
}
