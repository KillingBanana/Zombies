using UnityEngine;

public class Bullet : MonoBehaviour {
	private LayerMask damageMask;
	[HideInInspector] public float impactForce;
	[HideInInspector] public int damage;
	private float lifetime;
	
	private bool toDestroy;

	private void OnCollisionEnter(Collision other) {
		if (((1 << other.gameObject.layer) & damageMask) != 0) { //If is damageMask
			other.gameObject.BroadcastMessage("OnDamageTaken", this);
		}
		Destroy(gameObject);
	}

	public void Init(int damage, float impactForce, LayerMask damageMask, float lifetime) {
		this.damage = damage;
		this.impactForce = impactForce;
		this.damageMask = damageMask;
		this.lifetime = lifetime;
	}

	private void Update() {
		lifetime -= Time.deltaTime;
		if (lifetime <= 0) {
			Destroy(gameObject);
		}
	}
}