using UnityEngine;
using System.Collections;

public class Dragon_Main_0101_Script : MonoBehaviour {

	public static GameObject mainDragon;

	Player_Script player;

	Animator anim;

	Vector3 lastKnownGroundPoint;

	bool takingOff, landing;

	public bool onGround, canMove, isFlying;

	public bool attacking;
	int attackIndex;

	public Transform rcPos;

	public float groundDistance, turnSpeed, ascentSpeed;

	public bool possessed;

	void Awake () {
		mainDragon = this.gameObject;
		anim = GetComponent<Animator> ();
		canMove = true;
	}
	// Use this for initialization
	void Start () {
		player = Player_Script.player.GetComponent<Player_Script> ();
		lastKnownGroundPoint = transform.position;
		attackIndex = 0;
	}
	
	// Update is called once per frame
	void Update () {
		

		GroundDetection ();
		DragonInput ();
		DragonMovement ();
		UpdateAnimationVariables ();
	}

	void GroundDetection () {
		RaycastHit hit;
		if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity)) {
			if (Vector3.Distance (transform.position, hit.point) < groundDistance) {
				onGround = true;
			} else {
				onGround = false;
				if (!isFlying && !takingOff && !landing) {
					StartCoroutine (TakeOff ());
				}
			}
			lastKnownGroundPoint = hit.point;
		} else {
			onGround = false;
			transform.position += Vector3.up * 10 * Time.deltaTime;
		}
	}

	void DragonInput () {
		
		if (possessed) {
			if (Input.GetKeyDown (KeyCode.R)) {
				if (isFlying) {
					if (!takingOff && !landing) {
						StartCoroutine (Land ());
					}

				} else {
					if (!takingOff && !landing) {
						StartCoroutine (TakeOff ());
					}

				}
			}

			if (Input.GetKeyDown (KeyCode.Mouse0)) {
				if (!attacking && !takingOff && !landing) {
					StartCoroutine (DragonAttack (0));
				}

			}

			if (Input.GetKeyDown (KeyCode.Mouse1)) {
				if (!attacking && !takingOff && !landing) {
					StartCoroutine (DragonAttack (1));
				}

			}
		}
	}

	void DragonMovement () {
		if (canMove && possessed) {
			anim.SetFloat ("Vertical", Input.GetAxis("DragonForward"));

			if (isFlying) { 

				Quaternion keyRot = Quaternion.Euler (new Vector3 (0f, turnSpeed * Input.GetAxis ("DragonHorizontal"), 0) * Time.deltaTime);
				transform.rotation = transform.rotation * keyRot;

				transform.position += new Vector3(0,Input.GetAxis ("DragonVertical") * ascentSpeed * Time.deltaTime,0);
			} else {
				if (Input.GetAxis("DragonForward") > 0) {
					Quaternion keyRot = Quaternion.Euler (new Vector3 (0f, turnSpeed * Input.GetAxis ("DragonHorizontal"), 0) * Time.deltaTime);
					transform.rotation = transform.rotation * keyRot;
				}
			}
		}
	}

	IEnumerator DragonAttack (int attackType) {
		attacking = true;
		if (isFlying) {
			attackIndex = Random.Range (0, 3);
		} else {
			attackIndex = Random.Range (0, 6);
		}
		anim.SetInteger ("attackIndex", attackIndex);
		if (attackType == 0) {
			anim.SetTrigger ("Attack");
		} else {
			anim.SetTrigger ("Attack_2");
		}


		yield break;
	}

	void UpdateAnimationVariables () {
		anim.SetBool ("onGround", onGround);
		anim.SetBool ("Flying", isFlying);
	}

	IEnumerator TakeOff () {
		takingOff = true;
		anim.SetTrigger ("Fly");
		while (onGround){
			transform.position += Vector3.up * Time.deltaTime;
			yield return null;
		}
		isFlying = true;
		takingOff = false;
		yield break;
	}

	IEnumerator Land () {
		landing = true;
		if (onGround) {
			anim.SetTrigger ("Land");
			transform.position = lastKnownGroundPoint;
		} else {
			anim.SetTrigger ("Dive");
			while (!onGround) {
				yield return null;
			}
			anim.SetTrigger ("Land");
			transform.position = lastKnownGroundPoint;
		}
		isFlying = false;
		landing = false;
		yield break;
	}

	IEnumerator GoToPlayer () {
		transform.position = player.transform.position - player.transform.forward * 5;
		yield break;
	}




}
