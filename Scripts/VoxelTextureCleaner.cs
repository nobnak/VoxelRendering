using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelTextureCleaner {
	protected readonly ComputeShader clear;
	protected readonly int kernelId;
	protected readonly int textureId;

	protected readonly uint x, y, z;

    public VoxelTextureCleaner(ComputeShader clear, int kernelId, string textureProp)
        : this(clear, kernelId, Shader.PropertyToID(textureProp)) { }
	public VoxelTextureCleaner(ComputeShader clear, int kernelId, int textureId) {
		this.clear = clear;
		this.kernelId = kernelId;
		this.textureId = textureId;

		clear.GetKernelThreadGroupSizes (kernelId, out x, out y, out z);
	}

	public void Clear (RenderTexture tex) {
        var groupx = tex.width / (int)x;
        var groupy = tex.height / (int)y;
        var groupz = tex.volumeDepth / (int)z;

		clear.SetTexture (0, textureId, tex);
        clear.Dispatch (0, groupx, groupy, groupz);
	}
}
