using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

	public void RestartCurrentScene() {
		SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
	}
}
