using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelTextureMixer {
    public const string KERNEL = "CSMain";
    public const string PROP_RIGHT_TEX = "Right";
    public const string PROP_UP_TEX = "Up";
    public const string PROP_FORWARD_TEX = "Forward";
    public const string RESULT_TEX = "Result";

    protected readonly ComputeShader mixer;
    protected readonly int kernelId;
    protected readonly int rightTexId;
    protected readonly int upTexId;
    protected readonly int forwardTexId;
    protected readonly int resultTexId;

	protected readonly uint x, y, z;

    public VoxelTextureMixer(ComputeShader mixer) : this(mixer, KERNEL, PROP_RIGHT_TEX, PROP_UP_TEX, PROP_FORWARD_TEX, RESULT_TEX) {
    }
    public VoxelTextureMixer(ComputeShader mixer, string kernelName, string rightTexName, 
        string upTexName, string forwardTexName, string resultTexName)
        : this(mixer, mixer.FindKernel(kernelName), Shader.PropertyToID(rightTexName), Shader.PropertyToID(upTexName), Shader.PropertyToID(forwardTexName), Shader.PropertyToID(resultTexName)) {
    }
    public VoxelTextureMixer(ComputeShader mixer, int kernelId, int rightTexId, int upTexId, int forwardTexId, int resultTexId) {
		this.mixer = mixer;
        this.kernelId = kernelId;
        this.rightTexId = rightTexId;
        this.upTexId = upTexId;
        this.forwardTexId = forwardTexId;
        this.resultTexId = resultTexId;

		mixer.GetKernelThreadGroupSizes (kernelId, out x, out y, out z);
	}

    public void Mix (RenderTexture result, RenderTexture right, RenderTexture up, RenderTexture forward) {
        var groupx = right.width / (int)x;
        var groupy = right.height / (int)y;
        var groupz = right.volumeDepth / (int)z;

        mixer.SetTexture (kernelId, rightTexId, right);
        mixer.SetTexture (kernelId, upTexId, up);
        mixer.SetTexture (kernelId, forwardTexId, forward);
        mixer.SetTexture (kernelId, resultTexId, result);
        mixer.Dispatch (0, groupx, groupy, groupz);
	}
}
