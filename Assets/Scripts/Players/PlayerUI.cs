using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerUI : MonoBehaviour {

	public Player player;
	[SerializeField] private Text keysText;
	[SerializeField] private Text currentAmmoText;
	[SerializeField] private Text scrapText;
	[SerializeField] private Text ammoText;
	[SerializeField] private Text reloadingText;
	[SerializeField] private RectTransform healthBar;
	[SerializeField] private RectTransform staminaBar;
	[SerializeField] private RectTransform staminaThreshold;
	[SerializeField] private float staminaFrameWidth;
	private float baseHealthBarWidth;
	private float baseStaminaBarWidth;

	// Use this for initialization
	private void Start() {
		baseHealthBarWidth = healthBar.sizeDelta.x;
		baseStaminaBarWidth = staminaBar.sizeDelta.x;
	}

	// Update is called once per frame
	private void Update() {
		if (player != null) {
			healthBar.sizeDelta = new Vector2(baseHealthBarWidth * Mathf.Clamp01((player.health + player.tempHealth) / player.maxHealth), healthBar.sizeDelta.y);

			staminaBar.sizeDelta = new Vector2(baseStaminaBarWidth * (player.stamina / player.maxStamina), staminaBar.sizeDelta.y);
			staminaThreshold.sizeDelta = new Vector2(staminaFrameWidth * (player.minimumStaminaToSprint / player.maxStamina) + 5, staminaThreshold.sizeDelta.y);

			keysText.text = "Keys: " + player.keys;
			scrapText.text = "Scrap: " + player.scrap;

			/*foreach (KeyValuePair<WeaponType, int> kvp in player.spareAmmo) {
				if (kvp.Key != WeaponType.Knife) {
					ammo += System.Enum.GetName(typeof(WeaponType), kvp.Key) + " AmmoDrop: " + kvp.Value + "\n";
				}
			}*/

			Weapon currentWeapon = player.currentWeapon;

			currentAmmoText.text = currentWeapon.bullets.ToString();
			ammoText.text = player.spareAmmo[currentWeapon.weaponType].ToString();
			reloadingText.text = currentWeapon.isReloading ? "Reloading..." : "";
		} else {
			healthBar.sizeDelta = new Vector2(0, healthBar.sizeDelta.y);
			staminaBar.sizeDelta = new Vector2(0, staminaBar.sizeDelta.y);
		}
	}
}