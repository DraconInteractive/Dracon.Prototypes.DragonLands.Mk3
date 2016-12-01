using UnityEngine;
using System.Collections;

public class Main_Camera_Script : MonoBehaviour {
	private GameObject player;
	private Player_Script ps;
	private GameObject dragon;
	private Dragon_Main_0101_Script ds;
	public float playerYOffset, playerXOffset, playerZOffset;
	public float dragon_f_YOffset, dragon_f_XOffset, dragon_f_ZOffset;
	public float dragonYOffset, dragonXOffset, dragonZOffset;

	public float movementAlpha;
	public float focusOffset, focusSensitivity;

	public GameObject target;
	public GameObject dragon_cam_target, dragon_cam_target_f;

	// Use this for initialization
	void Start () {
		player = Player_Script.player;
		ps = player.GetComponent<Player_Script> ();
		dragon = Dragon_Main_0101_Script.mainDragon;
		ds = dragon.GetComponent <Dragon_Main_0101_Script> ();

		target = player;
		ps.possessed = true;
		ds.possessed = false;
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.V)) {
			CycleTarget ();
		}

		if (target == dragon) {
			DragonCamera ();
		}
	}
	// Update is called once per frame
	void FixedUpdate () {

		if (target == player) {
			PlayerCamera ();
		} 

	}

	void PlayerCamera () {
		Vector3 desiredPosition = target.transform.position + target.transform.up * playerYOffset + target.transform.right * playerXOffset - target.transform.forward * playerZOffset;

		transform.LookAt (player.transform.position + player.transform.forward * (2 + focusOffset));
		transform.position = Vector3.Lerp (transform.position, desiredPosition, movementAlpha);

		focusOffset += Input.GetAxis ("Mouse Y") * focusSensitivity;
		focusOffset = Mathf.Clamp (focusOffset, 0, 22);
	}

	void DragonCamera () {
		Vector3 targetPos;
		if (ds.isFlying) {
//			targetPos = new Vector3 (dragon.transform.position.x + dragon_f_XOffset, dragon.transform.position.y + dragon_f_YOffset, dragon.transform.position.z + dragon_f_ZOffset);
			transform.position = Vector3.Lerp (transform.position, dragon_cam_target_f.transform.position, 0.1f);

		} else {
//			targetPos = new Vector3 (dragon.transform.position.x + dragonXOffset, dragon.transform.position.y + dragonYOffset, dragon.transform.position.z + dragonZOffset);
			transform.position = Vector3.Lerp (transform.position, dragon_cam_target.transform.position, 0.25f);

		}
//		transform.position = Vector3.Lerp (transform.position, targetPos, 0.05f);
		transform.forward = dragon.transform.forward;
	}

	public void CycleTarget () {
		if (target == player) {
			target = dragon;
//			transform.position = new Vector3 (dragon.transform.position.x + dragonXOffset, dragon.transform.position.y + dragonYOffset,dragon.transform.position.z + dragonZOffset);
//			transform.forward = dragon.transform.forward;
			ps.possessed = false;
			ds.possessed = true;
		} else {
			transform.parent = null;
			target = player;
			ps.possessed = true;
			ds.possessed = false;
		}
	}
}
