using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Accumulator : MonoBehaviour {
    public RenderTextureEvent OnUpdateTexture;

	[SerializeField]
	ComputeShaderLinker csLinker = null;
    [SerializeField]
    float dissipation = 0.1f;
    [SerializeField]
    float emission = 1f;

    VoxelTexture resultTex;

    void OnEnable() {
        resultTex = new VoxelTexture (0, RenderTextureFormat.ARGBHalf, FilterMode.Point, TextureWrapMode.Clamp,
            RenderTextureReadWrite.Linear);
    }

    public void UpdateVoxelTexture(RenderTexture voxelTex) {
        resultTex.SetResolution (voxelTex.width);
        csLinker.Accumulator.Accumulate (voxelTex, resultTex.Texture, dissipation, emission);
        NotifyOnUpdateTexture ();
    }

    void NotifyOnUpdateTexture() {
        OnUpdateTexture.Invoke (resultTex.Texture);
    }
}
