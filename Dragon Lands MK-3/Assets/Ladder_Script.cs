using UnityEngine;
using System.Collections;

public class Ladder_Script : MonoBehaviour {

	bool playerOnLadder = false;
	public Vector3 offset;
	Player_Script ps;
	public float ladderTime;
	Transform finalTargetChild;
	public Transform [] leftRungs;
	public Transform[] rightRungs;

	void Awake () {
		finalTargetChild = transform.GetChild (0);
	}

	void Start () {
		ps = Player_Script.player.GetComponent<Player_Script> ();


	}

	void Interact () {
		playerOnLadder = true;

		StartCoroutine (ps.ClimbLadder (this.gameObject, transform.position + offset, leftRungs, rightRungs, ladderTime, finalTargetChild));
	}


}
