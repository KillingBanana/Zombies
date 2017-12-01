using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShowBehindWalls : MonoBehaviour {
	private readonly List<Hideable> hiddenObjects = new List<Hideable>();

	[SerializeField] private LayerMask hideableLayerMask;

	public bool active = true;

	// Update is called once per frame
	private void Update() {
		ClearHiddenObjects();
		if (active) {
			List<RaycastHit> hits = Physics.RaycastAll(transform.position, Camera.main.transform.position - transform.position, Mathf.Infinity, hideableLayerMask).ToList();
			foreach (RaycastHit hit in hits) {
				Hideable hitHideable = hit.transform.GetComponent<Hideable>();
				if (hitHideable != null) {
					//If object is hideable
					hitHideable.hidden = true;
					hiddenObjects.Add(hitHideable);
				}
			}
		}
	}

	private void ClearHiddenObjects() {
		foreach (Hideable hiddenObject in hiddenObjects) {
			hiddenObject.hidden = false;
		}
		hiddenObjects.Clear();
	}
}