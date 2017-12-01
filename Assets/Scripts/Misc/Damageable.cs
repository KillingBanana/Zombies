using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour {

	[SerializeField] private Color hurtColor = Color.red;
	private MeshRenderer meshRenderer;
	private Color defaultColor;

	private void Start() {
		meshRenderer = GetComponent<MeshRenderer>();
		defaultColor = meshRenderer.material.color;
	}

	private void Update() {
		meshRenderer.material.color = Color.Lerp(meshRenderer.material.color, defaultColor, 0.05f);
	}

	public void OnDamageTaken() {
		meshRenderer.material.color = hurtColor;
	}
}
