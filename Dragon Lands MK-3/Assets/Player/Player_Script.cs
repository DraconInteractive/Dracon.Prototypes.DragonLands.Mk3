using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;
using System.Collections;
using System.Collections.Generic;
using MORPH3D;

public class Player_Script : MonoBehaviour {

	private Rigidbody rb;
	private Animator anim;
	private Camera mainCamera;
	public static GameObject player;
	public GameObject playerMesh;

	public float strafeSpeed, forwardSpeed, backwardSpeed;
	public float horizontalRotationSpeed;
	public float jumpForce;
	private float jumpCounter = 0.3f;
	private bool canJump = true;

	private float currentHealth, maxHealth = 100;
	private float currentMana, maxMana = 100;

	public float currentExp, targetExp;
	public int level;

	public bool canMove;

	public Slider healthSlider, manaSlider;

	public List<Item> allItems;
	public Item playerWeapon;
	public Item playerArmour;
	public List<Item> inventory;
	public int inventoryMax;

	public bool onGround;

	public bool armed = false;

	public bool rightDown;

	private M3DCharacterManager m3DCM;

	public GameObject inventoryWindow;

	public GameObject slotContainer;
	public GameObject[] slots;

	public bool ikActive;
	public Transform ikRightHandObj;
	public Transform ikLeftHandObj;
	public Transform ikLookObj;
	private float ikWeighting = 1;

	public bool possessed;

	public bool attacking;

	void Awake () {
		rb = GetComponent<Rigidbody> ();
		anim = GetComponent<Animator> ();
		mainCamera = Camera.main;
		player = this.gameObject;
		m3DCM = GetComponent<M3DCharacterManager> ();

		slots = new GameObject[slotContainer.transform.childCount];
		for (int i = 0; i < slotContainer.transform.childCount; i++) {
			slots[i] = slotContainer.transform.GetChild (i).gameObject;
		}

	}

	// Use this for initialization
	void Start () {
		currentHealth = maxHealth;
		currentMana = maxMana;

		Cursor.lockState = CursorLockMode.Locked;
		canMove = true;

		ToggleInventory (false);
	}

	void OnDrawGizmos () {
		Gizmos.DrawWireCube (transform.position + transform.forward + transform.up, Vector3.one);
	}
	void Update () {

//		if (Input.GetKey (KeyCode.Mouse1)) {
//			rightDown = true;
//		} else {
//			rightDown = false;
//		}

		if (possessed) {
			if (Input.GetKeyDown (KeyCode.Mouse0)) {
				Attack (0);
			}

			if (Input.GetKeyDown(KeyCode.E)) {
				print ("Calling Interact");
				Interact ();
			}

			if (Input.GetKeyDown(KeyCode.Z)) {
				if (armed) {
					armed = false;
					m3DCM.DetachPropFromAttachmentPoint ("PlayerSword", "rHandAttachmentPointR");
				} else {
					armed = true;
					m3DCM.AttachPropToAttachmentPoint ("PlayerSword", "rHandAttachmentPointR");
				}
			}
				
		}
		if (Input.GetKeyDown(KeyCode.I)) {
			ToggleInventory ();
		}
		if (Input.GetKeyDown(KeyCode.F7)) {
			DamagePlayer (2);
		}

		GetGround ();
	}

	// Update is called once per frame
	void FixedUpdate () {
		PlayerMovement ();
//		if (Input.GetKeyDown(KeyCode.Space)) {
//			Jump ();
//		}

	}


	private void Attack (int attackType) {
//		anim.SetInteger ("AttackType", attackType);
		if (!attacking) {
			attacking = true;
			anim.SetTrigger ("Attack");
			Collider[] colArray;
			colArray = Physics.OverlapBox (transform.position + transform.forward + transform.up, Vector3.one);
			for (int i = 0; i < colArray.Length; i++) {
				if (colArray[i].gameObject.tag == "Enemy") {
					colArray [i].SendMessage ("DamageEnemy", 20);
				}
			}
		}

	}

	#region movement

	private void PlayerMovement () {

		if (!canMove || !possessed) {
			return;
		}
		float moveSpeed;
		if (Input.GetAxis("Vertical") > 0) {
			moveSpeed = forwardSpeed;
		} else {
			moveSpeed = backwardSpeed;
		}
		rb.MovePosition (transform.position + ((transform.forward * Input.GetAxis ("Vertical") * moveSpeed * (currentHealth / 100))/* + (transform.right * Input.GetAxis ("Horizontal") * strafeSpeed)*/) * Time.deltaTime);
		Quaternion mouseRot = Quaternion.Euler(new Vector3(0f, horizontalRotationSpeed * Input.GetAxis("Mouse X"), 0f)*Time.deltaTime);
		Quaternion keyRot = Quaternion.Euler (new Vector3 (0f, horizontalRotationSpeed * Input.GetAxis ("Horizontal"), 0) * Time.deltaTime);

//		if (rightDown) {
//			rb.MoveRotation (rb.rotation * keyRot);
//		} else {
//			rb.MoveRotation (rb.rotation * mouseRot * keyRot);
//		}

		if (!rightDown) {
			rb.MoveRotation (rb.rotation * mouseRot * keyRot);
		}


//		anim.SetFloat ("Horizontal", Input.GetAxis ("Horizontal"));
		anim.SetFloat ("Vertical", Input.GetAxis ("Vertical"));
		anim.SetFloat ("Health", currentHealth / 100);

//		playerMesh.transform.rotation = rb.velocity;
//		if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0) {
//			playerMesh.transform.forward = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical")) * moveSpeed;
//		}

	}

	private void GetGround () {
		RaycastHit hit;
		if (Physics.Raycast (transform.position, -transform.up, out hit, 2.2f)) {
			onGround = true;
		} else {
			onGround = false;
		}
		anim.SetBool ("OnGround", onGround);
	}
	private void  Jump () {
		print ("Jumping");
		if (canJump && onGround){
			canJump = false;
			anim.SetTrigger ("Jump");
			StartCoroutine (JumpCounter ());
			StartCoroutine (JumpTimer ());
		} 

	}

	private IEnumerator JumpCounter () {
		yield return new WaitForSeconds (0.25f);
		Player_Script.player.GetComponent<Rigidbody> ().AddForce (Player_Script.player.transform.up * 10, ForceMode.Impulse);
		yield break;
	}

	IEnumerator JumpTimer () {
		yield return new WaitForSeconds (jumpCounter);
		canJump = true;
		yield break;
	}

	#endregion

	#region IK 

	void OnAnimatorIK () {
		if (ikActive) {
			if (ikLookObj != null) {
				anim.SetLookAtWeight (1);
				anim.SetLookAtPosition (ikLookObj.position);
			}

			if (ikRightHandObj != null) {
				anim.SetIKPositionWeight (AvatarIKGoal.RightHand, ikWeighting);
				anim.SetIKRotationWeight (AvatarIKGoal.RightHand, ikWeighting);
				anim.SetIKPosition (AvatarIKGoal.RightHand, ikRightHandObj.position);
				anim.SetIKRotation (AvatarIKGoal.RightHand, ikRightHandObj.rotation);
			}

			if (ikLeftHandObj != null) {
				anim.SetIKPositionWeight (AvatarIKGoal.LeftHand, ikWeighting);
				anim.SetIKRotationWeight (AvatarIKGoal.LeftHand, ikWeighting);
				anim.SetIKPosition (AvatarIKGoal.LeftHand, ikLeftHandObj.position);
				anim.SetIKRotation (AvatarIKGoal.LeftHand, ikLeftHandObj.rotation);
			}
		} else {
			anim.SetIKPositionWeight (AvatarIKGoal.RightHand, 0);
			anim.SetIKRotationWeight (AvatarIKGoal.RightHand, 0);
			anim.SetIKPositionWeight (AvatarIKGoal.LeftHand, 0);
			anim.SetIKRotationWeight (AvatarIKGoal.LeftHand, 0);
			anim.SetLookAtWeight (0);
		}
	}

	private void ActivateLeftHandIK (Transform leftHand, float weighting) {
		ikActive = true;
		ikLeftHandObj = leftHand;
		ikWeighting = weighting;
	}

	private void ActivateDualIK (Transform leftHand, Transform rightHand, float weighting) {
		ikActive = true;
		ikLeftHandObj = leftHand;
		ikRightHandObj = rightHand;
		ikWeighting = weighting;
	}

	private void ActivateRightHandIK (Transform rightHand, float weighting) {
		ikActive = true;
		ikRightHandObj = rightHand;
		ikWeighting = weighting;
	}
		
	#endregion

	#region stats

	public void DamagePlayer (int amount) {
		currentHealth -= amount;
		healthSlider.value = currentHealth;
		anim.SetTrigger ("GetHit");
		CheckHealth ();
	}

	public void HealPlayer (int amount) {
		currentHealth += amount;
		currentHealth = Mathf.Clamp (currentHealth, 0, 100);
		healthSlider.value = currentHealth;
	}

	public void SpendMana (int amount) {
		currentMana -= amount;
		currentMana = Mathf.Clamp (currentMana, 0, 100);
		manaSlider.value = currentMana;
	}

	public void RestoreMana (int amount) {
		currentMana += amount;
		currentMana = Mathf.Clamp (currentMana, 0, 100);
		manaSlider.value = currentMana;
	}

	private void CheckHealth () {
		if (currentHealth <= 0) {
			Die ();
		}
	}

	private void Die () {
		print ("Blood. BLOOD. BLOOD. deathhhhhh");
		currentHealth = maxHealth;
	}

	public void AddExp (float amount) {
		currentExp += amount;
		while (currentExp >= targetExp) {
			currentExp -= targetExp;
			LevelUp ();
		}
	}

	public void LevelUp () {
		level++;
		targetExp *= 1.2f;
	}
	#endregion

	#region Inventory

	public void AddToInventory (Item item, int amount){
		print ("Adding " + amount + " " + item.itemName + " to inventory");
		if (inventory.Contains(item)) {
			item.itemQuantity += amount;
			slots [item.slot].GetComponent<INV_Slot_Script> ().UpdateCountText ();
		} else {
			if (inventory.Count < inventoryMax) {
				inventory.Add (item);
				item.itemQuantity = 0;
				item.itemQuantity += amount;

				AddItemToINVSlot (item);
			}
		}
	}

	public void RemoveFromInventory (Item item, int amount) {
		print ("Removing " + item.itemQuantity + " " + item.itemName + " from inventory");
		if (inventory.Contains (item)) {
			item.itemQuantity -= amount;
			if (item.itemQuantity <= 0) {
				item.itemQuantity = 0;
				inventory.Remove (item);

				RemoveItemFromSlot (item);
			}
		}
	}

	public void RemoveFromInventory (int slotNum) {
		print ("Removing all items from slot " + slotNum);
		for (int i = 0; i < inventory.Count; i++) {
			if (inventory[i].slot == slotNum) {
				inventory [i].itemQuantity = 0;
				RemoveItemFromSlot (inventory [i]);
				inventory.Remove (inventory [i]);
			}
		}
	}

	public void EquipItem (Item item) {
		if (item == null) {
			return;
		}

		print ("Equipping " + item.itemName + " as " + item.itemType);
		switch (item.itemType) {
		case Item.elementType.ARMOUR:
			playerArmour = item;
			break;
		case Item.elementType.WEAPON:
			playerWeapon = item;
			break;
		case Item.elementType.RESOURCE:
			print ("NA");
			break;
		}
	}

	public Item GenerateItem () {
		return allItems [Random.Range (0, allItems.Count)];
	}

	public Item GenerateItem (int level) {
		List <Item> itemsOfLevel = new List<Item>();
		for (int i = 0; i < allItems.Count; i++) {
			if (allItems[i].itemLevel == level) {
				itemsOfLevel.Add (allItems [i]);
			}
		
		}
		if (itemsOfLevel.Count > 0) {
			return itemsOfLevel [Random.Range(0,itemsOfLevel.Count)];
		} else {
			return null;
		}

	}

	public Item GenerateItem (Item.elementType element) {
		List <Item> itemsOfType = new List <Item>();
		for (int i = 0; i < allItems.Count; i++) {
			if (allItems[i].itemType == element) {
				itemsOfType.Add (allItems [i]);
			}
		}

		if (itemsOfType.Count > 0) {
			return itemsOfType [Random.Range (0, itemsOfType.Count)];
		} else {
			return null;
		}

	}


	#endregion

	#region InventoryUI

	private void AddItemToINVSlot (Item itemToAdd) {
		int chosenSlot = -1;
		for (int i = 0; i < slots.Length; i++) {
			if (slots[i].GetComponent<INV_Slot_Script>().CheckIfEmpty()) {
				chosenSlot = i;
				break;
			}
		}

		if (chosenSlot != -1) {
			slots [chosenSlot].GetComponent<INV_Slot_Script> ().AddItemToSlot (itemToAdd);
			itemToAdd.slot = chosenSlot;
		}
	}

	private void RemoveItemFromSlot (Item itemToTake) {
		slots [itemToTake.slot].GetComponent<INV_Slot_Script> ().EmptySlot ();
		itemToTake.slot = -1;
	}

	#endregion

	#region windows

	public void ToggleInventory () {
		inventoryWindow.SetActive (!inventoryWindow.activeSelf);
	}

	public void ToggleInventory (bool state) {
		inventoryWindow.SetActive (state);
	}

	#endregion

	#region Interaction
	private void Interact () {
		//will go off closest to player
		Collider[] colliders = Physics.OverlapBox (transform.position + transform.forward * 1, Vector3.one);
		int closestCollider = -1;
		float closestDist = 5000;
		for (int i = 0; i < colliders.Length; i++) {
			if (colliders[i].tag == "Interactable") {
				float a = Vector3.Distance (transform.position, colliders [i].transform.position);
				if (a < closestDist){
					closestDist = a;
					closestCollider = i;
				}
			}
		}

		if (closestCollider != -1) {
			colliders [closestCollider].SendMessage ("Interact");
		}

	}

	public IEnumerator ClimbLadder (GameObject ladder, Vector3 entryPos, Transform[] leftRungs, Transform[] rightRungs ,float ladderTime, Transform finalTransform) {
		//THEORY
		//a = time / rungCount
		//after a, increase lRung or rRung;
		/*
		 * ikactive = true;
		 * 
		 * ikLeftHandObj = leftRung[lRung];
		 * ikRightHandObj = rightRung[rRung];
		 * 
		 * yield return new waitforseconds (ladderTime / leftRungs.Count);
		 * lRung++;
		 * rRung++;
		 * 
		 * rinse and repeat;
		 * */

		float ikAlpha = ladderTime / leftRungs.Length;
		int currentLRung = 0;
		int currentRRung = 1;
		ActivateDualIK (leftRungs [currentLRung], rightRungs [currentRRung], 0.2f);
		canMove = false;
		transform.position = entryPos;
		transform.LookAt (ladder.transform.position);
		anim.SetTrigger ("Ladder");
		bool leftTurn = true;
		for (int i = 0; i < (leftRungs.Length * 2); i++) {
			yield return new WaitForSeconds (ikAlpha / 2);
			if (leftTurn) {
				currentLRung++;
				leftTurn = false;
			} else {
				if (currentRRung != rightRungs.Length - 1) {
					currentRRung++;
				}
				leftTurn = true;
			}


			ikLeftHandObj = leftRungs [currentLRung - 1];
			ikRightHandObj = rightRungs [currentRRung - 1];
		}

		anim.SetTrigger ("EndLadder");
		ikActive = false;
		transform.position = finalTransform.position;
		transform.rotation = finalTransform.rotation;
		canMove = true;
		yield break;
	}

	#endregion

	public void PlayerMount () {
		canMove = false;
		rb.isKinematic = true;
	}

	public void PlayerDismount () {
		canMove = true;
		rb.isKinematic = false;
	}
}
