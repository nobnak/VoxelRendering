using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractVoxelTextureComputer {
    public const string KERNEL = "CSMain";

	protected readonly ComputeShader compute;
	protected readonly int kernelId;

	protected readonly uint x, y, z;

    public AbstractVoxelTextureComputer(ComputeShader compute) :this(compute, KERNEL) { }
    public AbstractVoxelTextureComputer(ComputeShader compute, string kernelName)
        : this(compute, compute.FindKernel(kernelName)) { }
    public AbstractVoxelTextureComputer(ComputeShader compute, int kernelId) {
		this.compute = compute;
		this.kernelId = kernelId;
		compute.GetKernelThreadGroupSizes (kernelId, out x, out y, out z);
	}

    public virtual void Dispatch (int width, int height, int depth) {
        var groupx = width / (int)x;
        var groupy = height / (int)y;
        var groupz = depth / (int)z;
        compute.Dispatch (kernelId, groupx, groupy, groupz);
	}
}
