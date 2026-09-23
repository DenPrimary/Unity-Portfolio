using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInputs : MonoBehaviour
{
    public string mainMenu = "MainMenu";

    public KeyCode respawnKey = KeyCode.R;
    public KeyCode quit = KeyCode.Escape;

    private PlayerSpawner playerSpawner;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(respawnKey))
            playerSpawner?.RespawnPlayer();

        if (Input.GetKeyDown(quit))
           ReturnToMainMenu();
    }

    private void RestartScene() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ReturnToMainMenu() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenu);
    }
}
