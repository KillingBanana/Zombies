using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medic : PlayerClass {

	public HealingDevice healingDevice;

	public override void Ability() {
		Instantiate(healingDevice, transform.position + transform.forward, transform.rotation);
	}

	public override void Special() {
		Debug.Log("Started Special");
	}
}
