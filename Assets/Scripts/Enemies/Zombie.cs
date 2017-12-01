using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour, IEnemy {

	public IEnumerator Attack(Enemy enemy, Player player) {
		yield return new WaitForSeconds(1);
		if (enemy.canAttack) {
			//player.Damage(enemy.damage);
			player.BroadcastMessage("OnDamageTaken", enemy.damage);
		}
		enemy.isAttacking = false;
	}
}