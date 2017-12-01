using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy {
	IEnumerator Attack(Enemy enemy, Player player);
}