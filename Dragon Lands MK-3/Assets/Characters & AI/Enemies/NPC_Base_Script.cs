using UnityEngine;
using System.Collections;
[RequireComponent(typeof (Rigidbody))]
public abstract class NPC_Base_Script : MonoBehaviour {
	public Rigidbody rb;
	public float currentHealth, maxHealth;
	public float currentMana, maxMana;
	public bool healing, attacking, dodging, moving;


	void Awake () {
		rb = GetComponent<Rigidbody> ();
	}

	public abstract void Interact();

	public void DamageEnemy (int amount) {
		currentHealth -= amount;
		if (currentHealth <= 0) {
			Die ();
		}
		DetectPlayer ();
	}

	public void HealEnemy (int amount) {
		currentHealth += amount;
		if (currentHealth > maxHealth) {
			currentHealth = maxHealth;
		}
	}

	public abstract void Die ();

	public abstract void DetectPlayer ();
}
