using System;
using System.CodeDom;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlayersController : MonoBehaviour {

	[Range(1, 4)] public int maxPlayers = 1;
	public int players;
	private GameObject classChoiceMenu;
	[SerializeField] private GameObject[] spawnPoints;
	[SerializeField] private GameObject playerPrefab;
	[HideInInspector] public GameObject restartButton;

	[Header("Classes stuff")] [SerializeField] private HealingDevice medicHealingDevice;

	private WavesController wavesController;

	private bool gameActive;

	private int playerId;
	private PlayerUI[] playerUIs;

	private void Start() {
		wavesController = GetComponent<WavesController>();

		restartButton = GameObject.Find("RestartButton");
		restartButton.SetActive(false);

		classChoiceMenu = GameObject.Find("ClassChoiceMenu");

		spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
		playerUIs = FindObjectsOfType<PlayerUI>().OrderBy(m => m.transform.name).ToArray();

		for (int i = 0; i < playerUIs.Length; i++) {
			if (i >= maxPlayers) {
				playerUIs[i].gameObject.SetActive(false);
			}
		}
	}

	private void Update() {
		if (gameActive && players <= 0) {
			restartButton.SetActive(true);
		}
	}

	public void CreatePlayer(string className) {
		Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].transform;
		Player player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation).GetComponent<Player>();
		player.playersController = this;
		player.id = playerId;

		if (player.id == 0) {
			Camera.main.gameObject.GetComponent<CameraController>().player = player;
		} else {
			player.useJoystick = true;
		}

		AddClass(player, className);

		playerUIs[playerId].player = player;
		playerId++;
		players++;
		if (players >= maxPlayers) {
			StartGame();
		}
	}

	private void StartGame() {
		classChoiceMenu.SetActive(false);
		wavesController.enabled = true;
		gameActive = true;
	}

	private void AddClass(Player player, string className) {
		switch (className) {
			case "Assault":
				player.AddClass<Assault>();
				break;
			case "Demolisher":
				player.AddClass<Demolisher>();
				break;
			case "Hunter":
				player.AddClass<Hunter>();
				break;
			case "Medic":
				Medic medic = player.AddClass<Medic>();
				medic.healingDevice = medicHealingDevice;
				break;
			case "Sniper":
				player.AddClass<Sniper>();
				break;
		}
	}

}