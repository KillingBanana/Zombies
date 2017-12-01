using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using UnityEngine;

public class Weapon : MonoBehaviour, IDroppable {

	[Header("Positioning")] public Vector3 positionOffset;
	public Vector3 rotationOffset;

	[Header("Weapon Stats")] [SerializeField] private GameObject bulletPrefab;
	[SerializeField] private float bulletSpeed = 25f;
	[SerializeField] private float impactForce;
	[SerializeField] private int damage;
	[SerializeField] private float bulletsPerSecond;
	[HideInInspector] public float fireRateMultiplier = 1;
	[SerializeField] private bool fullAuto;
	[SerializeField] private bool infiniteAmmo;
	[SerializeField] private float reloadTime;
	[SerializeField] private float bulletLifetime = Mathf.Infinity;
	[HideInInspector] public Transform bulletSpawnPoint;
	[HideInInspector] public Player player;

	[Header("Spread")] [SerializeField] private float spreadFactorMin;
	[SerializeField] private float spreadFactorMax;
	[SerializeField] private float spreadFactorPerBullet;
	[SerializeField] private float spreadFactorPerSecond;
	[SerializeField] private float spreadFactor;
	private float spreadFactorBase;
	private float spreadFactorShooting;

	[Header("Ammo")] public WeaponType weaponType = WeaponType.Knife;
	public int maxBullets;
	[HideInInspector] public int bullets;
	public bool isReloading;

	private float lastShot;

	private bool shooting;

	public bool useSpread = true;

	public void Initialize(Mesh weaponMesh, GameObject bulletPrefab, float bulletSpeed, float impactForce, int damage, float bulletsPerSecond, bool fullAuto, bool infiniteAmmo,
		float reloadTime, float bulletLifetime, float spreadFactorMin, float spreadFactorMax, float spreadFactorPerBullet, float spreadFactorPerSecond, int maxBullets) {
		this.bulletPrefab = bulletPrefab;
		this.bulletSpeed = bulletSpeed;
		this.impactForce = impactForce;
		this.damage = damage;
		this.bulletsPerSecond = bulletsPerSecond;
		this.fullAuto = fullAuto;
		this.infiniteAmmo = infiniteAmmo;
		this.reloadTime = reloadTime;
		this.bulletLifetime = bulletLifetime;
		this.spreadFactorMin = spreadFactorMin;
		this.spreadFactorMax = spreadFactorMax;
		this.spreadFactorPerBullet = spreadFactorPerBullet;
		this.spreadFactorPerSecond = spreadFactorPerSecond;
		this.maxBullets = maxBullets;
	}

	private void Start() {
		bullets = maxBullets;
	}

	private void Update() {
		if (player != null) {
			spreadFactorBase = Mathf.Lerp(spreadFactorMin, spreadFactorMax, player.GetSpeedRatio());
		}
	}

	private void LateUpdate() {
		if (!shooting) {
			spreadFactor = Mathf.Max(spreadFactor - spreadFactorPerSecond * Time.deltaTime, spreadFactorBase);
			spreadFactorShooting = Mathf.Max(spreadFactorShooting - spreadFactorPerSecond * Time.deltaTime, 0);
		}
		shooting = false;
	}

	public void Shoot(bool buttonDown) {
		shooting = true;
		if (bullets <= 0 && player.autoReload) {
			Reload();
		} else {
			bool canShoot = (fullAuto || buttonDown) && (Time.time > lastShot + (1 / (bulletsPerSecond * fireRateMultiplier))) &&
			                (bullets > 0 || infiniteAmmo) && (!isReloading);
			if (canShoot) {
				Fire();
			}
		}
	}

	private void Fire() {
		GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
		Vector3 spread = bulletSpawnPoint.right * Random.Range(-spreadFactor, spreadFactor);
		Vector3 shootDirection = (bulletSpawnPoint.forward + spread);
		bullet.GetComponent<Rigidbody>().AddForce(shootDirection * bulletSpeed, ForceMode.VelocityChange);
		bullet.GetComponent<Bullet>().Init(damage, impactForce, player.enemiesLayer, bulletLifetime);
		lastShot = Time.time;
		spreadFactorShooting += spreadFactorPerBullet;
		spreadFactor = useSpread ? Mathf.Min(spreadFactorBase + spreadFactorShooting, spreadFactorMax) : 0f;
		if (!infiniteAmmo) {
			bullets -= 1;
		}
	}

	public void Reload() {
		if (bullets < maxBullets && !isReloading && player.spareAmmo[weaponType] > 0) {
			StartCoroutine(ReloadCoroutine());
		}
	}

	private IEnumerator ReloadCoroutine() {
		//Play animation?
		isReloading = true;

		yield return new WaitForSeconds(reloadTime);
		int requestedAmmo = maxBullets - bullets;
		int availableAmmo = player.RequestAmmo(requestedAmmo, weaponType);
		bullets += availableAmmo;
		isReloading = false;
	}

	public void PickUp(Player player) {
		GetComponent<Hover>().enabled = false;
		player.AddWeapon(this);
	}

	public void OnTriggerEnter(Collider other) {
		Player hitPlayer = other.GetComponent<Player>();
		if (hitPlayer != null) {
			PickUp(hitPlayer);
		}
	}
}

public enum WeaponType {
	Knife,
	Pistol,
	Rifle
}