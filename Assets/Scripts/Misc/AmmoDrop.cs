using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoDrop : MonoBehaviour, IDroppable {

	public int amount;

	public void PickUp(Player player) {
		player.scrap += amount;
		Destroy(gameObject);
	}

	public void OnTriggerEnter(Collider other) {
		Player player = other.GetComponent<Player>();
		if (player != null) {
			PickUp(player);
		}
	}
}