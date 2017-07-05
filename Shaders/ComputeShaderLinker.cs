using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ComputeShaderLinker : ScriptableObject {
    [SerializeField]
    ComputeShader clearComputeShader;
    [SerializeField]
    ComputeShader mixerComputeShader;
    [SerializeField]
    ComputeShader stepComputeShader;

    public VoxelTextureCleaner Cleaner { get; private set; }
    public VoxelTextureMixer Mixer { get; private set; }

    #region Unity
    void OnEnable() {
        Cleaner = new VoxelTextureCleaner (clearComputeShader);
        Mixer = new VoxelTextureMixer (mixerComputeShader);
    }
    void OnDisable() {
    }
    #endregion

}
