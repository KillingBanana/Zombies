using UnityEngine;

public class Hover : MonoBehaviour {

	[SerializeField] private float baseY = 0.5f;
	[SerializeField] private float amplitude = 0.25f;
	[SerializeField] private float speed = 1f;
	[SerializeField] private float rotationSpeed = 30f;
	private float offsetY;
	private float timeOffset;

	private void Start() {
		timeOffset = Random.Range(0f, 10f);
	}
	
	private void Update() {
		offsetY = amplitude * Mathf.Sin(speed * Time.time + timeOffset);
		transform.position = new Vector3(transform.position.x, baseY + offsetY, transform.position.z);
		transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
	}
}
