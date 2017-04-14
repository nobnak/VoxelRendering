using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelTexture : System.IDisposable {
	public event System.Action<VoxelTexture> OnCreateVoxelTexture;

	protected readonly int MINIMAL_RESOLUTION;
	protected int currentResolution;
	protected Vector4 v4Resolution;

	protected bool textureIsValid;
	protected RenderTexture voxelTex;

	protected RenderTextureFormat format;
	protected FilterMode filter;
	protected TextureWrapMode wrap;

	public VoxelTexture(int prefferedResolution, RenderTextureFormat format) : this(prefferedResolution, format, FilterMode.Bilinear, TextureWrapMode.Clamp) {
	}		
	public VoxelTexture(int prefferedResolution, RenderTextureFormat format, FilterMode filter, TextureWrapMode wrap) {
		this.textureIsValid = false;
		this.currentResolution = -1;
		this.format = format;
		this.filter = filter;
		this.wrap = wrap;
		SetResolution (prefferedResolution, true);
	}

	#region IDisposable implementation
	public void Dispose () {
		if (voxelTex != null)
			Release (ref voxelTex);
	}
	#endregion

	public RenderTexture Texture {
		get {
			if (!textureIsValid)
				Rebuild ();
			return voxelTex;
		}
	}

	#region Immutable Params
	public int CurrentResolution { 
		get { return currentResolution; } 
		set { SetResolution (value); } 
	}
	public void SetResolution(int prefferedResolution, bool force = false) {
		var nextResolution = Mathf.Max(MINIMAL_RESOLUTION, prefferedResolution).SmallestPowerOfTwoGreaterThan ();
		if (force || nextResolution != CurrentResolution) {
			textureIsValid = false;
			currentResolution = nextResolution;
			v4Resolution = new Vector4 (currentResolution, currentResolution, currentResolution, 0);
		}
	}
	public Vector4 ResolutionVector {
		get { return v4Resolution; }
	}
	#endregion

	#region Mutable Params
	public RenderTextureFormat Format {
		get { return format; }
		set {
			format = value;
			UpdateTextureProperties ();
		}
	}
	public FilterMode Filter {
		get { return filter; }
		set {
			filter = value;
			UpdateTextureProperties ();
		}
	}
	public TextureWrapMode Wrap { 
		get { return wrap; }
		set {
			wrap = value;
			UpdateTextureProperties ();
		}
	}
	#endregion

	void Rebuild () {
		textureIsValid = true;
		Release (ref voxelTex);
		voxelTex = new RenderTexture (currentResolution, currentResolution, 0, format);
		voxelTex.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
		voxelTex.volumeDepth = currentResolution;
		voxelTex.enableRandomWrite = true;
		UpdateTextureProperties ();
		voxelTex.Create ();

		NotifyOnCreateVoxelTexture ();
	}
	void UpdateTextureProperties() {
		if (voxelTex != null) {
			voxelTex.format = format;
			voxelTex.filterMode = filter;
			voxelTex.wrapMode = wrap;
		}
	}
	void Release<T>(ref T resource) where T : Object {
		if (Application.isPlaying)
			Object.Destroy (resource);
		else
			Object.DestroyImmediate (resource);
		resource = null;
	}
	void NotifyOnCreateVoxelTexture ()
	{
		if (OnCreateVoxelTexture != null)
			OnCreateVoxelTexture (this);
	}
}
