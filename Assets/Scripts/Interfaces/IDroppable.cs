using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDroppable {
	void OnTriggerEnter(Collider other);
	void PickUp(Player player);
}