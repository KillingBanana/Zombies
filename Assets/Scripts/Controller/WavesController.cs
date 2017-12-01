using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavesController : MonoBehaviour {
	[SerializeField] private GameObject zombiePrefab;
	[SerializeField] private List<Transform> enemySpawners = new List<Transform>();
	private int currentWaveId = 1;
	[SerializeField] private List<Wave> waves = new List<Wave>();
	private Wave currentWave;
	[SerializeField] private float cooldownBetweenWaves;
	private float cooldown;
	private bool waveActive;
	private bool waveStarted; //Whether or not wave started spawning

	private void Start() {
		cooldown = cooldownBetweenWaves;
	}

	private void Update() {
		if (waveActive) {
			if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0) { //If no object "Enemy" left, end wave
				EndWave();
			}
		} else if (!waveStarted) {
			cooldown -= Time.deltaTime;
			if (cooldown <= 0) {
				StartCoroutine(StartWave());
			}
		}
	}

	private IEnumerator StartWave() {
		currentWave = GetWave(currentWaveId);
		currentWave.waveId = currentWaveId;

		Debug.Log("Wave " + currentWaveId + " Started");
		waveStarted = true;

		for (int i = 0; i < currentWave.enemyAmount; i++) {
			SpawnEnemy();
			waveActive = true;
			yield return new WaitForSeconds(1);
		}

		waveStarted = false;
	}

	private void EndWave() {
		Debug.Log("Wave " + currentWaveId + " Ended");
		cooldown = cooldownBetweenWaves;
		currentWaveId++;
		waveActive = false;

		if (currentWave.keyWave) {
			Player[] players = FindObjectsOfType<Player>();
			foreach (Player player in players) {
				player.keys++;
				Debug.Log("Got a key!");
			}
		}
	}

	public void SpawnEnemy() {
		Transform enemySpawner = enemySpawners[Random.Range(0, enemySpawners.Count)];
		Enemy enemy = Instantiate(zombiePrefab, enemySpawner.position, enemySpawner.rotation).GetComponent<Enemy>();
		enemy.level = currentWave.enemyLevel;
		enemy.followPlayer = true;
	}

	private Wave GetWave(int id) {
		return waves[id - 1];
	}
}