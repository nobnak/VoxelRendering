using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Accumulator : MonoBehaviour {
    public RenderTextureEvent OnUpdateTexture;

    [SerializeField]
    ComputeShaderLinker csLinker;

    VoxelTexture resultTex;

    void OnEnable() {
        resultTex = new VoxelTexture (0, RenderTextureFormat.ARGB32);
    }

    public void UpdateVoxelTexture(RenderTexture voxelTex) {
        resultTex.SetResolution (voxelTex.width);
        csLinker.Accumulator.Accumulate (voxelTex, resultTex.Texture);
        NotifyOnUpdateTexture ();
    }

    void NotifyOnUpdateTexture() {
        OnUpdateTexture.Invoke (resultTex.Texture);
    }
}
