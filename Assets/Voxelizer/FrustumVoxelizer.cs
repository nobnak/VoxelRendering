using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gist;

[ExecuteInEditMode]
public class FrustumVoxelizer : MonoBehaviour {
	public const float NEAR_DISTANCE = 10f;

	public const string RESULT = "Result";
	public const string VOXEL_SIZE = "_VoxelSize";
	public const string VOXEL_COLOR_TEX = "_VoxelColorTex";
	public const string VOXEL_FACE_TEX = "_VoxelFaceTex";

	int PROP_VOXEL_SIZE;
	int PROP_VOXEL_COLOR_TEX;
	int PROP_VOXEL_FACE_TEX;

	public TextureEvent OnCreateVoxelTexture;
	
	public Bounds bounds = new Bounds (Vector3.zero, 10f * Vector3.one);
	public Color boundsColor = Color.green;

	public int prefferedVoxelResolution = 512;
	public FilterMode voxelFilterMode = FilterMode.Bilinear;

	public ComputeShader clearCompute;
	public Shader voxelShader;

	VoxelTexture colorTex;
	VoxelTexture faceTex;
	VoxelTextureCleaner cleaner;

	ManuallyRenderCamera renderCam;

	#region Unity
	void OnEnable() {
		PROP_VOXEL_SIZE = Shader.PropertyToID(VOXEL_SIZE);
		PROP_VOXEL_COLOR_TEX = Shader.PropertyToID(VOXEL_COLOR_TEX);
		PROP_VOXEL_FACE_TEX = Shader.PropertyToID (VOXEL_FACE_TEX);

		renderCam = new ManuallyRenderCamera ((cam) => {
			var bextent = bounds.extents;
			cam.transform.position = transform.position - (bextent.z + NEAR_DISTANCE) * transform.forward;
			cam.transform.rotation = transform.rotation;

			var bsize = bounds.size;
			cam.orthographic = true;
			cam.orthographicSize = bextent.y;
			cam.nearClipPlane = NEAR_DISTANCE;
			cam.farClipPlane = bsize.z + NEAR_DISTANCE;
			cam.aspect = bsize.x / bsize.y;
		});

		colorTex = new VoxelTexture (prefferedVoxelResolution, RenderTextureFormat.ARGB32);
		colorTex.OnCreateVoxelTexture += (v) => {
			OnCreateVoxelTexture.Invoke(v.Texture);
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

	void Init () {
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
	void Release<T>(ref T resource) where T : Object {
		if (Application.isPlaying)
			Object.Destroy (resource);
		else
			Object.DestroyImmediate (resource);
	}
}
