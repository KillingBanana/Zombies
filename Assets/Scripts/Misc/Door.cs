using UnityEngine;

public class Door : MonoBehaviour, IInteractable {

	[SerializeField] private Vector3 targetPosition;
	[SerializeField] private float speed = 0.5f;
	private bool open;
	private bool inOpenPosition;

	private void Update() {
		if (open && !inOpenPosition) {
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, speed * Time.deltaTime);
			if (Vector3.Distance(transform.localPosition, targetPosition) < 0.01f) {
				inOpenPosition = true;
				transform.localPosition = targetPosition;
			}
		}
	}

	public void Interact(Player player) {
		if (player.keys > 0 && !open) {
			player.keys--;
			open = true;
		}
	}
}