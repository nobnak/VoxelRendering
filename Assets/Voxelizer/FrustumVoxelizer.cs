using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gist;

[ExecuteInEditMode]
public class FrustumVoxelizer : MonoBehaviour {
	public const float NEAR_DISTANCE = 10f;

	public TextureEvent OnCreateVoxelTexture;
	public VoxelBoundsEvent VoxelBoundsOnChange;
	
    public VoxelCameraDirection.DirectionEnum cameraDirectionMode;
	public Bounds bounds = new Bounds (Vector3.zero, 10f * Vector3.one);
	public Color boundsColor = Color.green;

	public int prefferedVoxelResolution = 512;

	public ComputeShader clearCompute;
    public ComputeShader mixerCompute;
	public Shader voxelShader;

	VoxelTextureCleaner cleaner;
    VoxelTextureMixer mixer;
	TransformVoxelBounds voxelBounds;

    TriadVoxelizer triad;
    VoxelTexture result;

	#region Unity
	void OnEnable() {
		voxelBounds = new TransformVoxelBounds (transform);
		voxelBounds.Changed += (obj) => {
			VoxelBoundsOnChange.Invoke (obj);
		};
		cleaner = new VoxelTextureCleaner (clearCompute, 0, ShaderConstants.RESULT);
        mixer = new VoxelTextureMixer (mixerCompute);
        triad = new TriadVoxelizer (voxelShader, cleaner, mixer, voxelBounds, prefferedVoxelResolution);

		Init ();
	}
	void Update() {
		Init ();
        triad.Update (prefferedVoxelResolution);
        //OnCreateVoxelTexture.Invoke (triad [cameraDirectionMode].Texture);
        OnCreateVoxelTexture.Invoke (triad.ResultTexture.Texture);
	}
	void OnDrawGizmos() {
		if (!isActiveAndEnabled)
			return;

		Gizmos.color = boundsColor;
        voxelBounds.DrawGizmos ();
	}
	void OnDisable() {
        triad.Dispose ();
	}
	#endregion

	public AbstractVoxelBounds VoxelBounds { get { return voxelBounds; } }

	void Init () {
		voxelBounds.LocalBounds = bounds;
		voxelBounds.Update ();
	}
}
