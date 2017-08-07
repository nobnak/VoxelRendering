using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ComputeShaderLinker : ScriptableObject {
    public ComputeShader clearComputeShader;
    public ComputeShader mixerComputeShader;
    public ComputeShader stepComputeShader;
    public ComputeShader accumulateComputeShader;

    public VoxelTextureCleaner Cleaner { get; private set; }
    public VoxelTextureMixer Mixer { get; private set; }
    public VoxelTextureAccumulator Accumulator { get; private set; }

    #region Unity
    void OnEnable() {
        Cleaner = new VoxelTextureCleaner (clearComputeShader);
        Mixer = new VoxelTextureMixer (mixerComputeShader);
        Accumulator = new VoxelTextureAccumulator (accumulateComputeShader);
    }
    void OnDisable() {
    }
    #endregion

}
