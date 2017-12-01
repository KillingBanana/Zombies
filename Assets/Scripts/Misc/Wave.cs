using System;
using UnityEngine;

[Serializable]
public class Wave {
	[HideInInspector] public int waveId;
	public int enemyAmount;
	public int enemyLevel;
	public bool keyWave;
	public bool bossWave;
}
