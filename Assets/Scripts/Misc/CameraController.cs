using UnityEngine;

public class CameraController : MonoBehaviour {
	[HideInInspector] public Player player;
	[SerializeField] private float lookDistance = 10f;
	private Vector3 lookDirection;
	[SerializeField] private bool rotateCamera;
	[SerializeField] private float rotateSpeed = 1f;
	private bool rotatingView;

	private Vector3 pos;
	private Vector3 curPos;
	[Range(0, 1)] public float posRateDefault = 0.4f;
	private float posRate;

	[SerializeField] private bool followAim;
	[SerializeField] private float followAimAmount = 0.2f;

	private void Start() {
		lookDirection = transform.forward;
		//transform.position = player.transform.position - (lookDirection * lookDistance);
		posRate = posRateDefault;
	}

	private void Update() {
		lookDirection = transform.forward;
		followAim = Input.GetMouseButton(1);
	}

	private void LateUpdate() {
		if (player != null) {
			if (rotateCamera) {
				rotatingView = Input.GetButton("Rotate Camera");
				if (rotatingView) {
					lookDirection = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotateSpeed, Vector3.up) * lookDirection;
					transform.position = player.transform.position - (lookDirection * lookDistance);
					posRate = 1f;
				} else {
					posRate = posRateDefault;
				}
			}

			transform.forward = lookDirection;

			Vector3 targetPosition = ((!followAim || rotatingView)
				? player.transform.position
				: Vector3.Lerp(player.transform.position, player.aimPoint, followAimAmount)); //Don't follow mouse when rotating
			curPos = targetPosition - (lookDirection * lookDistance);
			pos = Vector3.Lerp(pos, curPos, posRate);
			transform.position = pos;
		}
	}
}