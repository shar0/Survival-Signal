using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public DataPoint CurrentPoint;
	public DataPoint Target;
	private float liftSpeed = 0.8f;
	private static Player instance;
	public int Energy = 5;
	public List<int> Inventory;

	public static Player Instance {
		get {
			return instance;
		}
	}

	void Awake ()
	{
		instance = this;
		Inventory = new List<int> ();
	}

	// Use this for initialization
	void Start ()
	{
	}

	public bool IsMove {
		get {
			return Target != null;
		}
	}

	// Update is called once per frame
	void Update ()
	{
		if (Target != null) {
			if (Vector3.Distance (transform.position, Target.transform.position) == 0) {
				Energy -= 1;
				CurrentPoint = Target;
				Target = null;
				GameManager.Instance.FinishMove ();
			} else {
				transform.position = Vector3.MoveTowards (
					transform.position, 
					Target.transform.position, 
					liftSpeed
				);
			}
		}
	}

	GameObject GetClosestObject (GameObject originObject, string tag)
	{
		GameObject[] objects = GameObject.FindGameObjectsWithTag (tag);
		int closestObjectIdx = -1;
		for (int i = 0; i < objects.Length; i++) {
			if (closestObjectIdx < 0) {
				closestObjectIdx = i;
			}
			//compares distances
			if (Vector3.Distance (originObject.transform.position, objects [i].transform.position) <= 
				Vector3.Distance (originObject.transform.position, objects [closestObjectIdx].transform.position)) {
				closestObjectIdx = i;
			}
		}
		return objects[closestObjectIdx];
	}
}
