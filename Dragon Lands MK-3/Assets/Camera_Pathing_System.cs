using UnityEngine;
using System.Collections;

public class Camera_Pathing_System : MonoBehaviour {

	public GameObject[] waypoints;

	public int startingPoint;
	int currentpoint = 0;

	public float speed; 

	//How far away from the point you want the camera to target the new point
	public float switchDistance;
	// Use this for initialization
	void Start () {
		currentpoint = startingPoint;
	}
	
	// Update is called once per frame
	void Update () {
		GoToPoint ();
	}

	void GoToPoint () {
//		transform.position = Vector3.Lerp (transform.position, waypoints [currentpoint].transform.position, 0.01f);
		transform.position += (waypoints [currentpoint].transform.position - transform.position).normalized * speed * Time.deltaTime;

		if (Vector3.Distance(transform.position, waypoints[currentpoint].transform.position) < switchDistance) {
			if (currentpoint < waypoints.Length - 1) {
				currentpoint++;
			} else {
				currentpoint = 0;
			}

		}
	}
}
