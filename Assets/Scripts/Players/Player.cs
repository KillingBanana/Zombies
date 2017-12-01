using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour {
	private CharacterController characterController;
	private LineRenderer lineRenderer;


	public PlayersController playersController;
	public int id;

	[Header("Movement Variables")] [SerializeField] private float walkingSpeed = 3;
	[SerializeField] private float runningSpeed = 3;
	public float maxStamina = 100;
	[HideInInspector] public float stamina;
	[SerializeField] private float staminaCostPerSecond;
	[SerializeField] private float staminaRegenPerSecond;
	public float minimumStaminaToSprint;
	[SerializeField] private bool autoSprint;
	[SerializeField] private bool midairControls = true;
	[SerializeField] private float jumpPower = 10;
	[SerializeField] private float gravity = 20;
	public Vector3 playerVelocity = Vector3.zero;
	private bool isRunning;
	private const float movingThreshold = 0.1f;

	[Header("Aiming Variables")] public bool useJoystick;
	[SerializeField] private bool useAdvancedAiming;
	private GameObject joystickCursor;
	[SerializeField] private float joystickCursorSpeed;
	public LayerMask enemiesLayer;
	[SerializeField] private Transform laserStartPoint;
	[SerializeField] private LayerMask blockLaser;
	[SerializeField] private Transform bulletSpawnPoint;
	[SerializeField] private float maxLaserLength = 10;
	private Vector3 cursorPosition = new Vector3(0, 0, 0);
	public Vector3 aimPoint;

	[Header("Combat Variables")] public int maxHealth = 100;
	public int health;
	public float tempHealth;
	public PlayerClass playerClass;

	[Header("Inventory")] public int keys;
	private List<Weapon> weaponList = new List<Weapon>();
	[SerializeField] private Transform weaponPoint;
	public int scrap = 100;
	public Dictionary<WeaponType, int> spareAmmo = new Dictionary<WeaponType, int>();
	private int currentWeaponId;
	[HideInInspector] public Weapon currentWeapon;
	public bool autoReload;

	[Header("Other")] [SerializeField] private Color hurtColor;
	public float interactDistance;

	private void Awake() {
		weaponList = GetComponentsInChildren<Weapon>(true).ToList();
		characterController = GetComponent<CharacterController>();
		lineRenderer = GetComponent<LineRenderer>();
		joystickCursor = GameObject.Find("Cursor");

		currentWeapon = weaponList[currentWeaponId];
		health = maxHealth;
		stamina = maxStamina;

		spareAmmo[WeaponType.Knife] = 0;
	}

	public T AddClass<T>() where T : PlayerClass {
		playerClass = gameObject.AddComponent<T>();
		playerClass.player = this;
		return (T) playerClass;
	}

	private void Update() {
		UpdateMovement();
		UpdateAim();
		UpdateCurrentWeapon();

		if (GetButton("Fire")) {
			Shoot();
		}

		if (GetButtonDown("Reload")) {
			Reload();
		}

		if (GetButtonDown("Interact")) {
			Interact();
		}

		if (GetButtonDown("Ability")) {
			playerClass.Ability();
		}

		if (GetButtonDown("Special")) {
			playerClass.Special();
		}

		if (tempHealth >= 1) {
			Heal(Mathf.FloorToInt(tempHealth));
			tempHealth = 0;
		}
	}

	private bool GetButton(string button) {
		return Input.GetButton(button + id);
	}

	private bool GetButtonDown(string button) {
		return Input.GetButtonDown(button + id);
	}

	private float GetAxis(string button) {
		return Input.GetAxis(button + id);
	}

	private void UpdateMovement() {
		float y = Camera.main.transform.eulerAngles.y;
		Vector3 camForward = Quaternion.AngleAxis(y, Vector3.up) * Vector3.forward;
		Vector3 camRight = Quaternion.AngleAxis(y + 90, Vector3.up) * Vector3.forward;
		float xAxis = GetAxis("Horizontal");
		float zAxis = GetAxis("Vertical");
		Vector3 movement = camRight * xAxis + zAxis * camForward;

		//UnityEngine.Debug.Log(characterController.velocity);
		bool movementInput = GetButton("Horizontal") || GetButton("Vertical");
		bool isMoving = characterController.velocity.magnitude > movingThreshold;

		//only change velocity and jump when grounded
		bool sprintKey = GetButton("Sprint");

		if (sprintKey) {
			if (stamina <= 0 || (stamina < minimumStaminaToSprint && !isRunning) || !isMoving) {
				isRunning = false;
			} else {
				isRunning = true;
			}
		} else if (stamina <= 0 || !movementInput || !autoSprint) {
			isRunning = false;
		}

		if (isRunning) {
			stamina -= staminaCostPerSecond * Time.deltaTime;
		} else {
			stamina = Mathf.Min(stamina + staminaRegenPerSecond * Time.deltaTime, maxStamina);
		}

		float speed = isRunning ? runningSpeed : walkingSpeed;

		if (midairControls || characterController.isGrounded) {
			//Limits vector length to Speed (so that diagonal is not faster)
			movement = Vector3.ClampMagnitude(movement * speed, speed);
			playerVelocity.x = movement.x;
			playerVelocity.z = movement.z;
			if (GetButtonDown("Jump")) {
				playerVelocity.y = jumpPower;
			}
		}
		if (!characterController.isGrounded) {
			//If player is not grounded
			playerVelocity.y -= gravity * Time.deltaTime;
		}
		characterController.Move(playerVelocity * Time.deltaTime);
	}

	private void UpdateAim() {
		float xAxis = 0;
		float yAxis = 0;
		if (useJoystick) {
			//Joystick controls
			xAxis = GetAxis("HorizontalR");
			yAxis = GetAxis("VerticalR");
		}

		aimPoint = Vector3.zero;
		RaycastHit hit;


		if (useAdvancedAiming) {
			if (useJoystick) {
				Vector3 cursorDirection = new Vector3(yAxis, -xAxis, 0) * joystickCursorSpeed;
				cursorPosition += cursorDirection;
				joystickCursor.SetActive(true);
				joystickCursor.transform.position = cursorPosition;
			} else {
				cursorPosition = Input.mousePosition;
				joystickCursor.SetActive(false);
			}

			if (Physics.Raycast(Camera.main.ScreenPointToRay(cursorPosition), out hit, Mathf.Infinity, enemiesLayer)) {
				aimPoint = hit.point;
			} else {
				Plane plane = new Plane(Vector3.up, bulletSpawnPoint.position);
				Ray ray = Camera.main.ScreenPointToRay(cursorPosition);
				float distance;
				if (plane.Raycast(ray, out distance)) {
					aimPoint = ray.GetPoint(distance);
				}
			}
		} else {
			joystickCursor.SetActive(false);
			if (useJoystick) {
				Vector3 aimingDirection = new Vector3(yAxis, 0, -xAxis);
				aimPoint = bulletSpawnPoint.position + aimingDirection;
			} else {
				Plane plane = new Plane(Vector3.up, bulletSpawnPoint.position);
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				float distance;
				if (plane.Raycast(ray, out distance)) {
					aimPoint = ray.GetPoint(distance);
				}
			}
		}


		lineRenderer.SetPosition(0, laserStartPoint.position);

		Vector3 laserHitPoint;
		if (Physics.Raycast(laserStartPoint.position, aimPoint - laserStartPoint.position, out hit, maxLaserLength, blockLaser)) {
			laserHitPoint = hit.point;
		} else {
			laserHitPoint = laserStartPoint.position + (aimPoint - laserStartPoint.position).normalized * maxLaserLength;
		}

		lineRenderer.SetPosition(1, laserHitPoint);
		Vector3 aimPointAtPivotLevel = new Vector3(aimPoint.x, transform.position.y, aimPoint.z);
		transform.LookAt(aimPointAtPivotLevel);
	}

	private void UpdateCurrentWeapon() {
		int weaponChange = -Sign(GetAxis("Mouse ScrollWheel"));
		if (weaponChange != 0) {
			currentWeaponId += weaponChange;
			if (currentWeaponId > weaponList.Count - 1) currentWeaponId = 0;
			if (currentWeaponId < 0) {
				currentWeaponId = weaponList.Count - 1;
			}
			currentWeapon = weaponList[currentWeaponId];
		}

		foreach (Weapon weapon in weaponList) {
			if (weapon == currentWeapon) {
				ActivateWeapon(weapon);
			} else {
				DeactivateWeapon(weapon);
			}
		}

		currentWeapon.transform.localPosition = currentWeapon.positionOffset;
		currentWeapon.transform.localEulerAngles = currentWeapon.rotationOffset;
	}

	private void Shoot() {
		currentWeapon.Shoot(GetButtonDown("Fire"));
	}

	private void Reload() {
		if (spareAmmo[currentWeapon.weaponType] > 0) {
			currentWeapon.Reload();
		}
	}

	private void Interact() {
		RaycastHit hit;
		if (Physics.Raycast(transform.position, transform.forward, out hit, interactDistance)) {
			IInteractable[] interactables = hit.transform.gameObject.GetComponentsInChildren<IInteractable>();
			foreach (IInteractable interactable in interactables) {
				interactable.Interact(this);
			}
		}
	}

	private void ActivateWeapon(Weapon weapon) {
		weapon.gameObject.SetActive(true);
		weapon.player = this;
		weapon.bulletSpawnPoint = bulletSpawnPoint;
	}

	private static void DeactivateWeapon(Weapon weapon) {
		weapon.StopAllCoroutines();
		weapon.isReloading = false;
		weapon.gameObject.SetActive(false);
	}

	public void AddWeapon(Weapon weapon) {
		weaponList.Add(weapon);
		weapon.transform.rotation = weaponPoint.rotation;
		weapon.transform.parent = weaponPoint;
		weapon.transform.localPosition = Vector3.zero;
		
		AddAmmo(0, weapon.weaponType);
	}

	public bool HasWeaponType(WeaponType weaponType) {
		return weaponList.Any(weapon => weapon.weaponType == weaponType);
	}

	///Removes that much health from player
	public void OnDamageTaken(int damage) {
		health -= damage;
		if (health <= 0) {
			Die();
		}
	}

	private void Die() {
		playersController.players--;
		Destroy(gameObject);
	}

	public bool RequestScrap(int requestedScrap) {
		if (scrap >= requestedScrap) {
			scrap -= requestedScrap;
			return true;
		} else {
			return false;
		}
	}
	///Returns whether or not the player was actually healed
	public bool Heal(int amount) {
		if (health < maxHealth) {
			health = Mathf.Min(maxHealth, health + amount);
			return true;
		}
		return false;
	}

	///Removes requested ammo, returns how much ammo is available
	public int RequestAmmo(int requestedAmmo, WeaponType weaponType) {
		int availableAmmo = Mathf.Min(requestedAmmo, spareAmmo[weaponType]); //spareAmmo[weaponType] > requestedAmmo ? requestedAmmo : spareAmmo[weaponType];
		spareAmmo[weaponType] -= availableAmmo;
		return availableAmmo;
	}

	public void AddAmmo(int amount, WeaponType weaponType) {
		if (spareAmmo.ContainsKey(weaponType)) {
			spareAmmo[weaponType] += amount;
		} else {
			spareAmmo.Add(weaponType, amount);
		}
	}

	private static int Sign(float f) {
		if (f > 0) {
			return 1;
		}
		if (f < 0) {
			return -1;
		}
		return 0;
	}

	public float GetSpeedRatio() {
		return characterController.velocity.magnitude / runningSpeed;
	}
}

public enum PlayerClassType {
	Assault,
	Medic,
	Demolisher,
	Hunter,
	Sniper
}