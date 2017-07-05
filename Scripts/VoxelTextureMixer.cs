using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelTextureMixer : AbstractVoxelTextureComputer {
    public const string PROP_RIGHT_TEX = "Right";
    public const string PROP_UP_TEX = "Up";
    public const string PROP_FORWARD_TEX = "Forward";
    public const string RESULT_TEX = "Result";

    protected readonly int rightTexId;
    protected readonly int upTexId;
    protected readonly int forwardTexId;
    protected readonly int resultTexId;

    public VoxelTextureMixer(ComputeShader mixer) 
        : this(mixer, KERNEL, PROP_RIGHT_TEX, PROP_UP_TEX, PROP_FORWARD_TEX, RESULT_TEX) { }
    public VoxelTextureMixer(ComputeShader mixer, string kernelName, string rightTexName, 
        string upTexName, string forwardTexName, string resultTexName)
        : this(mixer, mixer.FindKernel(kernelName), Shader.PropertyToID(rightTexName), Shader.PropertyToID(upTexName), Shader.PropertyToID(forwardTexName), Shader.PropertyToID(resultTexName)) {
    }
    public VoxelTextureMixer(ComputeShader mixer, int kernelId, int rightTexId, int upTexId, int forwardTexId, int resultTexId)
        : base(mixer, kernelId) {
        this.rightTexId = rightTexId;
        this.upTexId = upTexId;
        this.forwardTexId = forwardTexId;
        this.resultTexId = resultTexId;
	}

    public void Mix (RenderTexture result, RenderTexture right, RenderTexture up, RenderTexture forward) {
        compute.SetTexture (kernelId, rightTexId, right);
        compute.SetTexture (kernelId, upTexId, up);
        compute.SetTexture (kernelId, forwardTexId, forward);
        compute.SetTexture (kernelId, resultTexId, result);
        Dispatch (right.width, right.height, right.volumeDepth);
	}
}
