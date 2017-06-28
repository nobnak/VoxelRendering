using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gist;
using Gist.Extensions.AABB;

public class ViewSpaceBounds {
    public Color LocalSpaceColor { get; set; }
    public Color ViewSpaceColor { get; set; }

    public Matrix4x4 ModelMatrix { get; set; }
    public Matrix4x4 ViewMatrix { get; set; }

    public AbstractVoxelBounds voxelBounds { get; set; }

    public void DrawGizmos() {
        Gizmos.color = LocalSpaceColor;
        Gizmos.matrix = ModelMatrix;
        voxelBounds.DrawGizmosLocal ();

        var mvMat = ViewMatrix * ModelMatrix;
        var viewSpaceBounds = voxelBounds.LocalBounds.EncapsulateInTargetSpace (mvMat);
        Gizmos.color = ViewSpaceColor;
        Gizmos.matrix = ViewMatrix.inverse;
        Gizmos.DrawWireCube (viewSpaceBounds.center, viewSpaceBounds.size);

        Gizmos.matrix = Matrix4x4.identity;
    }

    public bool IsInitialized { get { return voxelBounds != null; } }
}
