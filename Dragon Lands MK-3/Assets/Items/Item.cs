using UnityEngine;
using System.Collections;
[CreateAssetMenu(fileName= "Item", menuName = "Inventory/Item", order = 1)]
public class Item : ScriptableObject {

	public string itemName;
	public int itemLevel;
	public int itemQuantity;

	public enum elementType {WEAPON, ARMOUR, RESOURCE};
	public elementType itemType;

	public GameObject itemPrefab;
	public Sprite itemImage;

	public int slot;

	void Start () {
		itemQuantity = 0;
		slot = -1;
	}
}
