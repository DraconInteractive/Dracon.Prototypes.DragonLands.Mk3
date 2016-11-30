using UnityEngine;
using System.Collections;

public class Devil_NPC_0201_Script : NPC_Base_Script {

	Animator anim;

	public bool playerDetected;
	float stepTimer;
	public float stepTarget, stMod;
	int attackIndex, waypointIndex;
	public bool willPatrol;
	public GameObject[] waypoints;
	public GameObject[] castObjects, screamObjects;
	public GameObject targetPoint;

	GameObject player;
	public float playerDetectionDistance;

	bool alive;

	Devil_Scream_Behaviour dSB;

	//	Rigidbody rb;
	//	public float currentHealth, currentMana, maxHealth, maxMana;
	//	public bool healing, attacking, dodging, moving;
	//	float stepTimer;
	//	public float stepTarget, stMod;

	void Awake () {
		anim = GetComponent<Animator> ();
//		rb = GetComponent<Rigidbody> ();
	}
	// Use this for initialization
	void Start () {
		player = Player_Script.player;
		currentHealth = maxHealth;
		currentMana = maxMana;
		stepTimer = 0;
		attackIndex = 0;
		waypointIndex = 0;
		alive = true;
		dSB = anim.GetBehaviour<Devil_Scream_Behaviour> ();
		dSB.devilOwner = this.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (alive) {
			stepTimer += Time.deltaTime;
			if (stepTimer >= stepTarget - stMod) {
				stepTimer = 0;
				Step ();
			}
		}

	}

	void Step () {
		print ("step");

		LookForPlayer ();
		if (!healing && !attacking && !moving) {
			if (currentHealth <= 25) {
				CastHeal ();
			} else if (playerDetected) {
				StartCoroutine ("AttackPlayer");
			} else {
				if (willPatrol) {
					StartCoroutine (MoveToWaypoint ());
				}
			}
		} else if (moving && playerDetected) {
			StopCoroutine (MoveToWaypoint ());
			anim.SetFloat ("Locomotion_Vertical", 0);
		}

		if (currentHealth > 50 && healing) {
			EndCast ();
		}

		if (healing) {
			stMod = 1f;
			HealEnemy (10);
		} else {
			stMod = 0;
		}

	}

	void LookForPlayer () {
		if (!playerDetected) {
			if (Vector3.Distance (transform.position, player.transform.position) < playerDetectionDistance){
				Quaternion q = Quaternion.LookRotation ((player.transform.position - transform.position).normalized, Vector3.up);
				if (Quaternion.Angle(transform.rotation, q) < 45) {
					DetectPlayer ();
				}
			}
		}
	}

	public override void DetectPlayer () {
		if (!playerDetected) {
			playerDetected = true;
			anim.SetTrigger ("detectedPlayer");
			StartScream ();
		}
	}

	public override void Interact () {
		
	}

	public override void Die () {
		if (healing) {
			EndCast ();
		}
		anim.SetTrigger ("Die");
		alive = false;
	}

	void CastHeal () {
		anim.SetTrigger ("StartSpell");
		healing = true;
		for (int i = 0; i < castObjects.Length; i++){
			if (castObjects[i].GetComponent<ParticleSystem>()) {
				var em = castObjects [i].GetComponent<ParticleSystem> ().emission;
				em.enabled = true;

			}
			if (castObjects[i].GetComponent<Light>()) {
				castObjects [i].GetComponent<Light> ().enabled = true;
			}

		}
	}

	void EndCast () {
		anim.SetTrigger ("EndSpell");
		healing = false;
		for (int i = 0; i < castObjects.Length; i++){
			if (castObjects[i].GetComponent<ParticleSystem>()) {
				var em = castObjects [i].GetComponent<ParticleSystem> ().emission;
				em.enabled = false;
			}
			if (castObjects[i].GetComponent<Light>()) {
				castObjects [i].GetComponent<Light> ().enabled = false;
			}

		}
	}

	public void StartScream () {
		for (int i = 0; i < screamObjects.Length; i++) {
			if (screamObjects[i].GetComponent<ParticleSystem>()){
				var em = screamObjects [i].GetComponent<ParticleSystem> ().emission;
				em.enabled = true;
			}
			if (screamObjects[i].GetComponent<Light>()){
				screamObjects [i].GetComponent<Light> ().enabled = true;
			}
		}
	}

	public void EndScream () {
		for (int i = 0; i < screamObjects.Length; i++) {
			if (screamObjects[i].GetComponent<ParticleSystem>()){
				var em = screamObjects [i].GetComponent<ParticleSystem> ().emission;
				em.enabled = false;
			}
			if (screamObjects[i].GetComponent<Light>()){
				screamObjects [i].GetComponent<Light> ().enabled = false;
			}
		}
	}
		
	IEnumerator AttackPlayer () {
		attacking = true;

		attackIndex++;

		if (attackIndex > 3) {
			attackIndex = 0;
		}

		anim.SetInteger ("attackIndex", attackIndex);
		Quaternion q = Quaternion.LookRotation ((new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z) - transform.position).normalized, Vector3.up);
		while (Quaternion.Angle(transform.rotation, q) > 6) {
//			transform.Rotate (Vector3.up, 5f);
			Vector3 h = player.transform.position - transform.position;
			Vector3 p = Vector3.Cross (transform.forward, h);
			float d = Vector3.Dot (p, transform.up);

			if (d > 0){
				//right
				transform.Rotate (Vector3.up, 5f);
			} else if (d < 0) {
				//left
				transform.Rotate (Vector3.up, -5f);

			} else {
				//f &|| b
				transform.Rotate (Vector3.up, 5f);

			}
			yield return new WaitForSeconds (0.01f);
		}

		anim.SetTrigger ("Attack");
		attacking = false;
	}
	IEnumerator MoveToWaypoint () {
		moving = true;
		Quaternion q = Quaternion.LookRotation ((new Vector3(waypoints[waypointIndex].transform.position.x, transform.position.y, waypoints[waypointIndex].transform.position.z) - transform.position).normalized, Vector3.up);
		while (Quaternion.Angle(transform.rotation, q) > 6) {
			//			transform.rotation = Quaternion.Lerp (transform.rotation, q, 0.01f);
			Vector3 h = waypoints[waypointIndex].transform.position - transform.position;
			Vector3 p = Vector3.Cross (transform.forward, h);
			float d = Vector3.Dot (p, transform.up);

			if (d > 0){
				//right
				transform.Rotate (Vector3.up, 1.5f);
			} else if (d < 0) {
				//left
				transform.Rotate (Vector3.up, -1.5f);

			} else {
				//f &|| b
				transform.Rotate (Vector3.up, 3.0f);

			}
			yield return new WaitForSeconds (0.01f);
		}
		anim.SetFloat ("Locomotion_Vertical", 1);
		while (Vector3.Distance(transform.position, waypoints[waypointIndex].transform.position) > 1) {

			yield return new WaitForSeconds (0.01f);
		}
		anim.SetFloat ("Locomotion_Vertical", 0);
		waypointIndex++;
		if (waypointIndex >= waypoints.Length) {
			waypointIndex = 0;
		}
		moving = false;
		yield break;
	}

	IEnumerator MoveToPoint (Vector3 v) {
		moving = true;
		Quaternion q = Quaternion.LookRotation ((new Vector3(v.x, transform.position.y, v.z) - transform.position).normalized, Vector3.up);
		while (Quaternion.Angle(transform.rotation, q) > 6) {
//			transform.rotation = Quaternion.Lerp (transform.rotation, q, 0.01f);
			transform.Rotate (Vector3.up, 1f);
			yield return new WaitForSeconds (0.01f);
		}
		anim.SetFloat ("Locomotion_Vertical", 1);
		while (Vector3.Distance(transform.position, v) > 1) {
			

//			transform.position = Vector3.MoveTowards (transform.position, v, 0.1f);
//			rb.MovePosition (transform.position + (v - transform.position).normalized * 2 * Time.deltaTime);
			yield return new WaitForSeconds (0.01f);
		}
		anim.SetFloat ("Locomotion_Vertical", 0);
		moving = false;
		yield break;

	}
}
