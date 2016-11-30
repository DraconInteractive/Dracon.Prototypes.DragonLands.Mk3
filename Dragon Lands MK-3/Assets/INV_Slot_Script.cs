using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class INV_Slot_Script : MonoBehaviour {

	bool itemInSlot;
	public Image slotIcon;
	public Sprite defaultImage;
	public Text itemCountText;
	public Item slotItem;

	// Use this for initialization
	void Start () {
		slotIcon.sprite = defaultImage;
		UpdateCountText ();
	}

	public void AddItemToSlot (Item itemAdded) {
		print ("Adding: " + itemAdded.itemName + " to slot");
		slotItem = itemAdded;
		slotIcon.sprite = itemAdded.itemImage;
		itemInSlot = true;
		UpdateCountText ();
	}

	public void EmptySlot () {
		slotIcon.sprite = defaultImage;
		slotItem = null;
		itemInSlot = false;
	}

	public bool CheckIfEmpty () {
		return !itemInSlot;
	}

	public void UpdateCountText () {
		if (itemInSlot) {
			itemCountText.text = slotItem.itemQuantity.ToString();
		} else {
			itemCountText.text = "-";
		}
	}
}
