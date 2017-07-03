using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ComputeShaderLinker : ScriptableObject {
    [SerializeField]
    ComputeShader clearComputeShadeer;
    [SerializeField]
    ComputeShader mixerComputeShader;

    public VoxelTextureCleaner Cleaner { get; private set; }
    public VoxelTextureMixer Mixer { get; private set; }

    public static ComputeShaderLinker Latest { get; private set; }

    #region Unity
    void OnEnable() {
        Cleaner = new VoxelTextureCleaner (clearComputeShadeer, 0, ShaderConstants.RESULT);
        Mixer = new VoxelTextureMixer (mixerComputeShader);

        Latest = this;
    }
    void OnDisable() {
        Latest = null;
    }
    #endregion

    public static bool Clear(RenderTexture tex) {
        if (Latest != null) {
            Latest.Cleaner.Clear (tex);
            return true;
        }
        return false;
    }
    public static bool Mix(RenderTexture result, RenderTexture right, RenderTexture up, RenderTexture forward) {
        if (Latest != null) {
            Latest.Mixer.Mix (result, right, up, forward);
            return true;
        }
        return false;
    }
}
