using UnityEngine;
using System.Collections;

public class Dragon_Red_Script : MonoBehaviour {

	public static GameObject redDragon;

	private Player_Script playerScript;

	private Animator anim;

	public bool isFlying;

	public int currentHealth, maxHealth;

	public bool dead;

	public bool onGround;

	public float turnSpeed, ascentSpeed;

	bool takingOff, landing;

	bool canMove = true;

	public Transform mountTransform, dismountTransform, raycastTransform;

	void Awake () {
		redDragon = this.gameObject;
		anim = GetComponent<Animator> ();
	}
	// Use this for initialization
	void Start () {
		playerScript = Player_Script.player.GetComponent<Player_Script> ();
	}
	
	// Update is called once per frame
	void Update () {
		CheckGround ();
		if (isFlying && onGround) {
			anim.SetTrigger ("Land");
			isFlying = false;
		}
		anim.SetBool ("onGround", onGround);
		anim.SetBool ("Flying", isFlying);

		DragonMovement ();
		if (Input.GetKeyDown (KeyCode.Keypad0)) {
			ToggleFlightState ();
		}

		if (Input.GetKeyDown (KeyCode.Comma)) {
			Mount (Player_Script.player);
		}

		if (Input.GetKeyDown (KeyCode.Period)) {
			Dismount (Player_Script.player);
		}
	}

	private void DragonMovement () {
		if (!canMove) {
			return;
		}

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

	private void ToggleFlightState () {
		isFlying = !isFlying;
		if (isFlying) {
			TakeOff ();
		} else {
			StartLand ();
		}
	}

	private void TakeOff () {
		anim.SetTrigger ("Fly");
		isFlying = true;
		StartCoroutine (TOIE());
	}

	private IEnumerator TOIE () {
		takingOff = true;
		for (int i = 0; i < 100; i++) {
			transform.position += Vector3.up * Time.deltaTime;
			yield return new WaitForSeconds(0.01f);
		}
		takingOff = false;
		yield break;
	}

	private void StartLand () {
		if (onGround) {
			anim.SetTrigger ("Land");
			isFlying = false;
		} else {
			anim.SetTrigger ("Dive");
		}
	}

	private void Attack () {
		anim.SetTrigger ("Attack");
	}

	private void GetHit () {
		anim.SetTrigger ("Hit");
	}

	private void CheckHealth () {
		if (currentHealth <= 0){
			Kill ();
		}
	}

	private Vector3 CheckGround () {
		RaycastHit hit;
		if (Physics.Raycast (raycastTransform.position, Vector3.down, out hit, 2.5f)) {
			onGround = true;
			return hit.point;
		} else if (Physics.Raycast(raycastTransform.position, Vector3.down, out hit, Mathf.Infinity)){
			onGround = false;
			return hit.point;
		} else {
			if (!takingOff) {
				anim.SetTrigger ("Land");
				transform.position += Vector3.up * 1 * Time.deltaTime;
				print ("not above ground at all");
				return Vector3.zero;
			} else {
				print ("not above ground at all");
				return Vector3.zero;
			}


		}
	}

	public void Kill () {
		anim.SetTrigger ("Death");
		dead = true;
	}

	public void Damage (int amount) {
		currentHealth -= amount;
		GetHit ();
	}

	public void Heal (int amount) {
		currentHealth += amount;
		currentHealth = Mathf.Clamp (currentHealth, 0, 100);
	}

	public void Mount (GameObject player) {
		player.transform.position = mountTransform.position;
		player.transform.rotation = mountTransform.rotation;
		playerScript.PlayerMount ();

		player.transform.parent = this.gameObject.transform;
	}

	public void Dismount (GameObject player) {
		player.transform.position = dismountTransform.position;
		player.transform.rotation = dismountTransform.rotation;
		playerScript.PlayerDismount ();

		player.transform.parent = null;
	}
}
