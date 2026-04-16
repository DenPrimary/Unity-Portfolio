using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    public GameObject pauseCanvasPrefab;
    public KeyCode pauseKey = KeyCode.Escape;
    public KeyCode confirmKey = KeyCode.Return;
    public string mainMenuScene = "ClassicModeMenu";

    private GameObject pauseInstance;
    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        { 
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        HandlePauseInput();
    }

    private void HandlePauseInput() 
    {
        if (Input.GetKeyDown(pauseKey)) 
        {
            if (GameManager.Instance != null && GameManager.Instance.isProcessingGoal) 
                return;

            if (isPaused) 
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        if (isPaused && Input.GetKeyDown(confirmKey)) 
        { 
            ConfirmExit();
        }
    }

    private void ConfirmExit() 
    {
        ResumeGame();
        ReturnToMainMenu();
    }

    private void ReturnToMainMenu() 
    { 
        Time.timeScale = 1f;
        if (StartProcedure.Instance != null)
            Destroy(StartProcedure.Instance.gameObject);

        if (pauseInstance != null)
            Destroy(pauseInstance);

        if (SceneManager.GetActiveScene().name != mainMenuScene)
        {
            SceneManager.LoadScene(mainMenuScene);
        }
    }

    public void PauseGame() 
    {
        if (isPaused || GameManager.Instance == null || GameManager.Instance.isProcessingGoal) 
            return;

        isPaused = true;
        Time.timeScale = 0f;

        if (pauseCanvasPrefab != null && pauseInstance == null) 
        { 
            pauseInstance = Instantiate(pauseCanvasPrefab);
        }
    }

    public void ResumeGame() 
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1f;

        if (pauseInstance != null)
        {
            Destroy(pauseInstance);
            pauseInstance = null;
        }
    }

    public bool IsGamePaused() => isPaused;
}
