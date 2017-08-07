using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelTextureAccumulator : AbstractVoxelTextureComputer {
    public const string SRC_TEX = "SrcTex";
    public const string STORAGE_TEX = "ResultTex";

    protected int srcTexId;
    protected int resultTexId;

    public VoxelTextureAccumulator(ComputeShader cs) : this(cs, KERNEL, SRC_TEX, STORAGE_TEX) {}
    public VoxelTextureAccumulator(ComputeShader cs, string kernel, string srcTex, string storageTex)
        : this(cs, cs.FindKernel(kernel), Shader.PropertyToID(srcTex), Shader.PropertyToID(storageTex)) {
    }
    public VoxelTextureAccumulator(ComputeShader cs, int kernelId, int srcTexId, int resultTexId)
        : base(cs, kernelId) {
        this.srcTexId = srcTexId;
        this.resultTexId = resultTexId;
    }

    public void Accumulate(RenderTexture src, RenderTexture result) {
        compute.SetTexture (kernelId, srcTexId, src);
        compute.SetTexture (kernelId, resultTexId, result);
        Dispatch (result.width, result.height, result.volumeDepth);
    }
}
