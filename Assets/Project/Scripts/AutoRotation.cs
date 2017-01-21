using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotation : MonoBehaviour {
	public float speed = 0.5F;
	void Update() {
		transform.Rotate (Vector3.up, speed * Time.deltaTime);
	}
}
