using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelTextureAccumulator : AbstractVoxelTextureComputer {
    public const string PROP_DT = "Dt";
    public const string PROP_DISSIPATION = "Dissipation";
    public const string PROP_EMISSION = "Emission";

    public const string PROP_SRC_TEX = "SrcTex";
    public const string PROP_STORAGE_TEX = "ResultTex";

    protected int dtId;
    protected int dissipationId;
    protected int emissionId;

    protected int srcTexId;
    protected int resultTexId;

    public VoxelTextureAccumulator(ComputeShader cs) : this(cs, KERNEL, PROP_SRC_TEX, PROP_STORAGE_TEX, 
        PROP_DT, PROP_DISSIPATION, PROP_EMISSION) {}
    public VoxelTextureAccumulator(ComputeShader cs, string kernel, string srcTex, string storageTex, 
        string dt, string dissipation, string emission)
        : this(cs, cs.FindKernel(kernel), 
            Shader.PropertyToID(srcTex), 
            Shader.PropertyToID(storageTex),
            Shader.PropertyToID(dt),
            Shader.PropertyToID(dissipation),
            Shader.PropertyToID(emission)) {
    }
    public VoxelTextureAccumulator(ComputeShader cs, int kernelId, int srcTexId, int resultTexId, 
        int dtId, int dissipationId, int emissionId)
        : base(cs, kernelId) {
        this.srcTexId = srcTexId;
        this.resultTexId = resultTexId;

        this.dtId = dtId;
        this.dissipationId = dissipationId;
        this.emissionId = emissionId;
    }

    public void Accumulate(RenderTexture src, RenderTexture result, float dissipation, float emission) {
        compute.SetTexture (kernelId, srcTexId, src);
        compute.SetTexture (kernelId, resultTexId, result);
        compute.SetFloat (dtId, Time.deltaTime);
        compute.SetFloat (dissipationId, dissipation);
        compute.SetFloat (emissionId, emission);
        Dispatch (result.width, result.height, result.volumeDepth);
    }
}
