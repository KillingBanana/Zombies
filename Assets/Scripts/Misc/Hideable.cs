using UnityEngine;

public class Hideable : MonoBehaviour {
	[SerializeField] private Material opaqueMaterial;
	[SerializeField] private Material transparentMaterial;
	[SerializeField] private Hideable[] hideablesToHide;
	private new Renderer renderer;
	public bool hidden;
	private bool transparent;

	private void Awake() {
		renderer = GetComponent<Renderer>();
	}

	private void Update() {
		if (hidden && !transparent) {
			MakeTransparent();
		}

		if (!hidden && transparent) {
			MakeOpaque();
		}
	}

	private void OnTriggerStay(Collider other) {
		if (other.GetComponent<ShowBehindWalls>() != null) {
			hidden = true;
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other.GetComponent<ShowBehindWalls>() != null) {
			hidden = false;
		}
	}

	protected void MakeOpaque() {
		if (transparent) {
			renderer.material = opaqueMaterial;
			transparent = false;
		}
		foreach (Hideable hideable in hideablesToHide) {
			hideable.MakeOpaque();
		}
	}

	protected void MakeTransparent() {
		if (!transparent) {
			renderer.material = transparentMaterial;
			transparent = true;
		}
		foreach (Hideable hideable in hideablesToHide) {
			hideable.MakeTransparent();
		}
	}
}