using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelTextureCleaner {
	public const string RESULT = "Result";

	protected readonly ComputeShader clear;
	protected readonly int kernelId;
	protected readonly int textureId;

	protected readonly uint x, y, z;

	public VoxelTextureCleaner(ComputeShader clear, int kernelId, int textureId) {
		this.clear = clear;
		this.kernelId = kernelId;
		this.textureId = textureId;

		clear.GetKernelThreadGroupSizes (kernelId, out x, out y, out z);
	}

	public void Clear (RenderTexture tex) {
		clear.SetTexture (0, textureId, tex);
		clear.Dispatch (0, tex.width / (int)x, tex.height / (int)y, tex.depth / (int)z);
	}
}
