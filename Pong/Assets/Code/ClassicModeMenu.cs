using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClassicModeMenu : MonoBehaviour
{
    public Button casualButton;
    public Button classicButton;
    public Button versusButton;
    public Button backButton;

    public string casualScene = "CasualMatchScene";
    public string classicScene = "ClassicGame";
    public string versusScene = "VersusGame";
    public string mainMenuScene = "MainMenu";

    public GameObject liveGameContainer;

    void Start()
    {
        InitializeUI();
    }

    void InitializeUI()
    {
        if (casualButton != null)
            casualButton.onClick.AddListener(StartCasualGame);

        if (classicButton != null)
            classicButton.onClick.AddListener(StartClassicGame);

        if (versusButton != null)
            versusButton.onClick.AddListener(StartVersusGame);

        if (backButton != null)
            backButton.onClick.AddListener(GoBack);
    }

    private void StartCasualGame()
    {
        Debug.Log("Starting Casual Match...");

        if (liveGameContainer != null)
            liveGameContainer.SetActive(false);

        LoadScene(casualScene);
    }

    private void StartClassicGame()
    {
        Debug.Log("Starting Classic Match...");
        LoadScene(classicScene);
    }

    private void StartVersusGame()
    {
        Debug.Log("Starting Versus Match...");
        LoadScene(versusScene);
    }

    private void GoBack()
    {
        Debug.Log("Going back to Main Menu...");
        LoadScene(mainMenuScene);
    }

    private void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene name is not specified!");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            GoBack();
    }

    private void OnDestroy()
    {
        if (casualButton != null)
            casualButton.onClick.RemoveAllListeners();

        if (classicButton != null)
            classicButton.onClick.RemoveAllListeners();

        if (versusButton != null)
            versusButton.onClick.RemoveAllListeners();

        if (backButton != null)
            backButton.onClick.RemoveAllListeners();
    }
}
