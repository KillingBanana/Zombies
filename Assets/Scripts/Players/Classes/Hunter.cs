﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunter : PlayerClass {

	public override void Ability() {
		Debug.Log("Started Ability");
	}

	public override void Special() {
		Debug.Log("Started Special");
	}
}