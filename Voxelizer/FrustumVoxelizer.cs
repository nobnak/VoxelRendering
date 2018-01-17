using nobnak.Gist;
using UnityEngine;

[ExecuteInEditMode]
public class FrustumVoxelizer : MonoBehaviour {
	public const float NEAR_DISTANCE = 10f;

    public RenderTextureEvent OnUpdateVoxelTexture;
	public VoxelBoundsEvent VoxelBoundsOnChange;
	
    public VoxelCameraDirection.DirectionEnum cameraDirectionMode;
	public Bounds bounds = new Bounds (Vector3.zero, 10f * Vector3.one);
	public Color boundsColor = Color.green;

	public int prefferedVoxelResolution = 512;

    public ComputeShaderLinker csLinker;
	public Shader voxelShader;

	TransformVoxelBounds voxelBounds;

    TriadVoxelizer triad;

	#region Unity
	void OnEnable() {
		voxelBounds = new TransformVoxelBounds (transform);
		voxelBounds.Changed += (obj) => {
			VoxelBoundsOnChange.Invoke (obj);
		};
        triad = new TriadVoxelizer (csLinker, voxelShader, voxelBounds, prefferedVoxelResolution);

		Init ();
	}
	void Update() {
		Init ();
        triad.Update (prefferedVoxelResolution);
        OnUpdateVoxelTexture.Invoke (triad.ResultTexture.Texture);
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
