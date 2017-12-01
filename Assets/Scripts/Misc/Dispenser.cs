using UnityEngine;
using UnityEngine.UI;

public class Dispenser : MonoBehaviour, IInteractable {

	[SerializeField] private Weapon weaponPrefab;
	private WeaponType weaponType;
	[SerializeField] private int weaponCost;
	[SerializeField] private int ammoCost;
	[SerializeField] private int ammoAmount;

	[SerializeField] private Text textPrefab;
	private Transform textAnchor;
	private Text textMesh;
	private bool showText;

	private Transform weaponDropPoint;

	private void Start() {
		weaponType = weaponPrefab.weaponType;
		textAnchor = transform.Find("TextAnchor");
		weaponDropPoint = transform.Find("WeaponDropPoint");
		transform.Find("WeaponDisplay").GetComponent<MeshFilter>().sharedMesh = weaponPrefab.GetComponent<MeshFilter>().sharedMesh;
	}

	private void Update() {
		showText = false;
		Player[] players = FindObjectsOfType<Player>();
		foreach (Player player in players) {
			if (Vector3.Distance(player.transform.position, GetComponent<Collider>().ClosestPoint(player.transform.position)) <= player.interactDistance) {
				showText = true;
				break;
			}
		}
		if (showText) {
			if (textMesh == null) {
				textMesh = Instantiate(textPrefab, transform.position, Quaternion.Euler(0, 0, 0));
				textMesh.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
			}
			textMesh.transform.position = Camera.main.WorldToScreenPoint(textAnchor.position);
			string text = "E to interact \n" + weaponPrefab.GetComponent<Weapon>().name + ": " + weaponCost + " scrap \n" + ammoAmount + " bullets: " + ammoCost + " scrap";
			textMesh.GetComponent<Text>().text = text;
		} else if (textMesh != null) {
			Destroy(textMesh.gameObject);
		}
	}

	public void Interact(Player player) {
		if (!player.HasWeaponType(weaponType)) {
			if (player.RequestScrap(weaponCost)) {
				Instantiate(weaponPrefab, weaponDropPoint.position, weaponDropPoint.rotation);
				//player.AddWeapon(weapon);
			}
		} else if (player.RequestScrap(ammoCost)) {
			player.AddAmmo(ammoAmount, weaponType);
		}
	}
}