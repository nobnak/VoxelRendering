using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gist;

public abstract class AbstractVoxelBounds {
	public event System.Action<AbstractVoxelBounds> Changed;

	protected Bounds bounds;

	protected Vector3 worldMinPosition;
	protected Vector3 worldMaxPosition;

	public Bounds LocalBounds {
		get { return bounds; }
		set {
			if (bounds != value) {
				bounds = value;
				Rebuild ();
			}
		}
	}

	public Vector3 NormalizedToLocalPosition(float u, float v, float w) {
		var min = bounds.min;
		var max = bounds.max;
		return new Vector3 (Mathf.Lerp (min.x, max.x, u), Mathf.Lerp (min.y, max.y, v), Mathf.Lerp (min.z, max.z, w));
	}
	public Vector3 NormalizedToLocalPosition(Vector3 uvw) { return NormalizedToLocalPosition (uvw.x, uvw.y, uvw.z); }
	public Vector3 NormalizedToWorldPosition(float u, float v, float w) {
		return new Vector3 (
			Mathf.Lerp (worldMinPosition.x, worldMaxPosition.x, u), 
			Mathf.Lerp (worldMinPosition.y, worldMaxPosition.y, v), 
			Mathf.Lerp (worldMinPosition.z, worldMaxPosition.z, w));
	}
	public Vector3 NormalizedToWorldPosition(Vector3 uvw) {
		return NormalizedToWorldPosition (uvw.x, uvw.y, uvw.z);
	}

	protected abstract Vector3 TransformPoint (Vector3 pos);

	protected virtual void Rebuild() {
		Precompute ();
		NotifyChanged ();
	}
	protected virtual void Precompute() {
		worldMinPosition = TransformPoint (NormalizedToLocalPosition (0f, 0f, 0f));
		worldMaxPosition = TransformPoint (NormalizedToLocalPosition (1f, 1f, 1f));
	}
	protected virtual void NotifyChanged() {
		if (Changed != null)
			Changed(this);
	}
}


public class TransformVoxelBounds : AbstractVoxelBounds {
	protected Transform tr;

	public TransformVoxelBounds(Transform tr) {
		this.tr = tr;
		Rebuild ();
	}

	#region implemented abstract members of AbstractVoxelBounds
	protected override Vector3 TransformPoint (Vector3 pos) {
		return tr.TransformPoint (pos);
	}
	#endregion

	public void Update() {
		if (tr.hasChanged)
			Rebuild ();
	}

	protected override void Rebuild() {
		tr.localScale = Vector3.one;
		tr.hasChanged = false;
		base.Rebuild ();
	}
}
