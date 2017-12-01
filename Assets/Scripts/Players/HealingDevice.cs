using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Api;
using UnityEngine;

public class HealingDevice : MonoBehaviour {

	[SerializeField] private float lifetime = 10f;
	[SerializeField] private float healPerSecond = 5;
	[SerializeField] private float healDistance = 3;

	private void Update() {
		lifetime -= Time.deltaTime;
		if (lifetime <= 0) {
			Destroy(gameObject);
		}

		Player[] players = FindObjectsOfType<Player>();
		foreach (Player player in players) {
			if (Vector3.Distance(player.transform.position, transform.position) <= healDistance) {
				player.tempHealth += healPerSecond * Time.deltaTime;
			}
		}
	}
}