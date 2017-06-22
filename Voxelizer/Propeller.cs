using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Propeller : MonoBehaviour {
	public const float ROUND = 360f;
	public const double TICKToSecond = 1e-7;
	public Vector3 timeToAngle;

	System.DateTime startTime;

	void OnEnable() {
		startTime = System.DateTime.Now;
	}
	void Update() {
		var seconds = (float)((System.DateTime.Now - startTime).Ticks * TICKToSecond);
		transform.localRotation = Quaternion.Euler (seconds * ROUND * timeToAngle);
	}
}
