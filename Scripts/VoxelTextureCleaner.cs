using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelTextureCleaner : AbstractVoxelTextureComputer {
    public const string RESULT_TEX = "Result";

	protected readonly int textureId;

    public VoxelTextureCleaner(ComputeShader clear) :this(clear, KERNEL, RESULT_TEX) { }
    public VoxelTextureCleaner(ComputeShader clear, string kernelName, string textureProp)
        : this(clear, clear.FindKernel(kernelName), Shader.PropertyToID(textureProp)) { }
    public VoxelTextureCleaner(ComputeShader clear, int kernelId, int textureId) : base(clear, kernelId) {
		this.textureId = textureId;
	}

	public void Clear (RenderTexture tex) {
        compute.SetTexture (kernelId, textureId, tex);
        Dispatch (tex.width, tex.height, tex.volumeDepth);
	}
}
