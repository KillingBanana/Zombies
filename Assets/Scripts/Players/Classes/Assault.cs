using System.Collections;
using UnityEngine;

public class Assault : PlayerClass {

	private const float maxAbilityCooldown = 10;
	private float abilityCooldown;

	private const float abilityDuration = 4;
	private bool abilityActive;

	private const float fireRateMultiplier = 1.5f;

	private void Update() {
		player.currentWeapon.useSpread = !abilityActive;
		player.currentWeapon.fireRateMultiplier = abilityActive ? fireRateMultiplier : 1;

		abilityCooldown = Mathf.Max(abilityCooldown - Time.deltaTime, 0);
	}

	public override void Ability() {
		if (!abilityActive) {
			StartCoroutine(FocusFire());
		}
	}

	private IEnumerator FocusFire() {
		Debug.Log("Ability Started");
		abilityActive = true;
		yield return new WaitForSeconds(abilityDuration);
		abilityActive = false;
		abilityCooldown = maxAbilityCooldown;
		Debug.Log("Ability Ended");
	}

	public override void Special() {
		Debug.Log("Started Assault Special");
	}
}