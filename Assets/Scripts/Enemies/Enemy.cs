using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(IEnemy))]
public class Enemy : MonoBehaviour {
	[Header("Enemy Stats")] public int level;
	[SerializeField] private int baseHealth = 40;
	[SerializeField] private int healthPerLevel = 20;
	[SerializeField] private int baseDamage = 10;
	[SerializeField] private int damagePerLevel = 5;
	[SerializeField] private int maxHealth = 60;
	public int damage = 15;
	private float cooldown;
	[SerializeField] private float attackRange = 2f;
	private int health;
	private bool dead;

	private Player target;
	private NavMeshAgent agent;
	[HideInInspector] public bool followPlayer;
	private bool isInRange;
	public bool canAttack;

	[Header("Drops")] [SerializeField] private GameObject ammoPrefab;
	[SerializeField] private int minAmmoAmount;
	[SerializeField] private int maxAmmoAmount;

	private const float corpseTimer = 3;
	private IEnemy enemyComponent;
	public bool isAttacking;

	private void Start() {
		target = FindObjectOfType<Player>();
		agent = GetComponent<NavMeshAgent>();
		enemyComponent = GetComponent<IEnemy>();
		maxHealth = baseHealth + level * healthPerLevel;
		health = maxHealth;
	}

	private void Update() {
		canAttack = !dead && isInRange && target != null;
		maxHealth = baseHealth + level * healthPerLevel;
		damage = baseDamage + level * damagePerLevel;
		if (!dead && target != null) {
			float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
			isInRange = distanceToTarget <= attackRange;

			if (agent.enabled && followPlayer) {
				agent.SetDestination(target.transform.position);
				agent.isStopped = isInRange; //Stop when closer than attack range
			}

			if (isInRange && !isAttacking) {
				Attack();
			}
		}
	}

	private void Attack() {
		StartCoroutine(enemyComponent.Attack(this, target));
		isAttacking = true;
	}

	// ReSharper disable once UnusedMember.Local
	private void OnDamageTaken(Bullet bullet) {
		Vector3 impactDirection = transform.position - bullet.transform.position;
		agent.velocity += impactDirection.normalized * bullet.impactForce;

		health -= bullet.damage;
		if (health <= 0 && !dead) {
			StartCoroutine(Die());
		}
	}

	private IEnumerator Die() {
		AmmoDrop ammoDrop = Instantiate(ammoPrefab, transform.position, transform.rotation).GetComponent<AmmoDrop>();
		ammoDrop.amount = Random.Range(minAmmoAmount, maxAmmoAmount);
		StopCoroutine(enemyComponent.Attack(this, target));
		dead = true;
		agent.enabled = false;
		GetComponent<ShowBehindWalls>().active = false;
		Rigidbody rb =gameObject.AddComponent<Rigidbody>();
		rb.AddForce(Random.insideUnitSphere, ForceMode.VelocityChange);
		yield return new WaitForSeconds(corpseTimer);
		Destroy(gameObject);
	}
}