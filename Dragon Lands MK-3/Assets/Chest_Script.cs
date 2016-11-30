using UnityEngine;
using System.Collections;

public class Chest_Script : MonoBehaviour {

	private Player_Script ps;

	public int chestLevel;

	private Animation anim;

	private bool open = false;

	public GameObject particleEffect;

	public Item chestItem;

	void Awake () {
		anim = GetComponent<Animation> ();
	}
	void Start () {
		ps = Player_Script.player.GetComponent<Player_Script> ();
		if (chestLevel == 0) {
			chestLevel = ps.level;
		}
	}

	public void Interact () {
		if (open) {
			print ("chest Open");
			return;
		}
		int mod = 0;

		Item itemToAdd;
		if (chestItem != null) {
			itemToAdd = chestItem;
		} else {
			itemToAdd = ps.GenerateItem (chestLevel + mod);
		}


		while (itemToAdd == null) {
			mod++;
			ps.GenerateItem (chestLevel + mod);
			if (mod > 10) {
				print ("no items of chest level - cl+10");
				break;
			}
		}
		if (itemToAdd != null) {
			ps.AddToInventory (itemToAdd, 1);
		}

		if (anim != null) {
			anim.Play ();
		}

		if (particleEffect) {
			particleEffect.GetComponent<ParticleSystem> ().Stop ();
		}
		open = true;
	}
}
