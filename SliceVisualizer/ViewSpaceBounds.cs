using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gist;
using Gist.Extensions.AABB;

public class ViewSpaceBounds {
    public Matrix4x4 VoxelModel { get; set; }
    public Matrix4x4 View { get; set; }
    public Matrix4x4 ModelView { get; private set; }

    public Bounds ViewBounds { get; private set; }

    public void Update(AbstractVoxelBounds voxelBounds, Matrix4x4 voxelModel, Matrix4x4 view) {
        var voxelUvToLocal = voxelBounds.VoxelUvToLocalMatrix ();

        VoxelModel = voxelModel;
        View = view;
        ModelView = View * VoxelModel;

        ViewBounds = voxelBounds.LocalBounds.EncapsulateInTargetSpace (ModelView);

        var viewUvToViewBounds = ViewUvToViewBounds ();
    }
    public void DrawGizmos(AbstractVoxelBounds voxelBounds, Color localColor, Color viewColor) {
        Gizmos.color = localColor;
        Gizmos.matrix = VoxelModel;
        voxelBounds.DrawGizmosLocal ();

        var viewSpaceBounds = voxelBounds.LocalBounds.EncapsulateInTargetSpace (ModelView);
        Gizmos.color = viewColor;
        Gizmos.matrix = View.inverse;
        Gizmos.DrawWireCube (viewSpaceBounds.center, viewSpaceBounds.size);

        Gizmos.matrix = Matrix4x4.identity;
    }

    Matrix4x4 ViewUvToViewBounds () {
        var size = ViewBounds.size;
        var min = ViewBounds.min;
        var m = Matrix4x4.zero;
        m [0] = size.x;
        m [12] = min.x;
        m [5] = size.y;
        m [13] = min.y;
        m [10] = size.z;
        m [14] = min.z;
        m [15] = 1f;
        return m;
    }
}
