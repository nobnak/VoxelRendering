using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gist;

[ExecuteInEditMode]
public class TriadVoxelizer : System.IDisposable {
	public const float NEAR_DISTANCE = 10f;
    public const RenderTextureFormat FORMAT = RenderTextureFormat.ARGB32;
    public const FilterMode FILTER_MODE = FilterMode.Bilinear;
    public const TextureWrapMode WRAP_MODE = TextureWrapMode.Clamp;

    public static readonly VoxelCameraDirection.DirectionEnum[] DIRECTIONS = 
        (VoxelCameraDirection.DirectionEnum[])System.Enum.GetValues (typeof(VoxelCameraDirection.DirectionEnum));

	public FilterMode voxelFilterMode = FilterMode.Bilinear;

    Shader voxelizer;
	VoxelTextureCleaner cleaner;
    VoxelTextureMixer mixer;

    VoxelCameraDirection cameraDirection;
	ShaderConstants shaderConstants;
	ManuallyRenderCamera renderCam;

    VoxelTexture[] colorTextures;
    VoxelTexture resultTex;

    public TriadVoxelizer(Shader voxelizer, VoxelTextureCleaner cleaner, VoxelTextureMixer mixer,
        AbstractVoxelBounds voxelBounds, int prefferedResolution) {
        this.voxelizer = voxelizer;
        this.cleaner = cleaner;
        this.mixer = mixer;
        
		this.shaderConstants = ShaderConstants.Instance;

        this.cameraDirection = new VoxelCameraDirection ();
        this.renderCam = new ManuallyRenderCamera ((cam) => cameraDirection.FitCameraToVoxelBounds (cam, voxelBounds));

        this.colorTextures = new VoxelTexture[DIRECTIONS.Length];
        for (var i = 0; i < colorTextures.Length; i++)
            colorTextures [i] = GenerateVoxelTexture (prefferedResolution);
        this.resultTex = GenerateVoxelTexture (prefferedResolution);
	}

    #region IDisposable implementation
    public void Dispose () {
        renderCam.Dispose ();
        for (var i = 0; i < colorTextures.Length; i++)
            colorTextures [i].Dispose ();
        resultTex.Dispose ();
    }
    #endregion

    public VoxelTexture this[VoxelCameraDirection.DirectionEnum dir] {
        get {
            return colorTextures[(int)dir];
        }
    }
    public VoxelTexture ResultTexture { get { return resultTex; } }
    public void Update(int prefferedResolution) {
        for (var i = 0; i < colorTextures.Length; i++) {
            var colorTex = colorTextures [i];
            var dir = DIRECTIONS [i];
            colorTex.SetResolution(prefferedResolution);
            cleaner.Clear (colorTex.Texture);
            Render(colorTex, dir);
        }

        cleaner.Clear (resultTex.Texture);
        mixer.Mix (resultTex.Texture,
            this [VoxelCameraDirection.DirectionEnum.LookRight].Texture,
            this [VoxelCameraDirection.DirectionEnum.LookUp].Texture,
            this [VoxelCameraDirection.DirectionEnum.LookForward].Texture);
	}

    VoxelTexture GenerateVoxelTexture(int prefferedResolution) {
        return new VoxelTexture (prefferedResolution, FORMAT, FILTER_MODE, WRAP_MODE);
    }
    void Render (VoxelTexture colorTex, VoxelCameraDirection.DirectionEnum cameraDirectionMode) {
        Shader.SetGlobalVector (shaderConstants.PROP_VOXEL_SIZE, colorTex.ResolutionVector);
        cameraDirection.Direction = cameraDirectionMode;
        Shader.SetGlobalMatrix (shaderConstants.PROP_VOXEL_ROTATION_MAT, cameraDirection.VoxelDirection);

		var targetTex = RenderTexture.GetTemporary (colorTex.CurrentResolution, colorTex.CurrentResolution);
		try {
			Graphics.SetRandomWriteTarget (1, colorTex.Texture);
			renderCam.RenderWithShader (targetTex, voxelizer, null);
			Graphics.ClearRandomWriteTargets ();
		} finally {
			RenderTexture.ReleaseTemporary (targetTex);
		}
	}
}
