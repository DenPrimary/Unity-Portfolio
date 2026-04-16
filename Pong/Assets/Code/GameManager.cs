using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int winScore = 11;

    public bool lastScoredPlayer = true;
    public bool isGameStarted = false;
    public bool isProcessingGoal = false;

    public Transform leftPaddle;
    public Transform rightPaddle;
    public Vector3 leftPaddleStartPosition;
    public Vector3 rightPaddleStartPosition;
    public bool resetPaddlesOnGoal = true;
    public float paddleResetSpeed = 8f;
    public PopupManager popupManager;
    public GameObject endgameCanvasPrefab;
    public string mainMenuScene = "ClassicModeMenu";

    public bool isMenuDemoMode = false;

    private BallController ballController;
    private GameTimeManager timeManager;
    private GameObject endGameOverlayInstance;

    void Awake()
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

    void Start()
    {
        InitializeManagers();
        SavePaddleStartPositions();
        isGameStarted = true;

        ScoreManager.OnPlayerScoredEvent += HandlePlayerScored;
        ScoreManager.OnEnemyScoredEvent += HandleEnemyScored;

        StartCoroutine(StartGameProcedure());
    }

    private void OnDestroy()
    {
        ScoreManager.OnPlayerScoredEvent -= HandlePlayerScored;
        ScoreManager.OnEnemyScoredEvent -= HandleEnemyScored;
    }

    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.ResumeGame();
        }

        if (StartProcedure.Instance != null)
            Destroy(StartProcedure.Instance.gameObject);

        DestroyEndGameOverlay();
        SceneManager.LoadScene(mainMenuScene);
    }

    private void HandlePlayerScored() 
    {
        if (isProcessingGoal) return;

        isProcessingGoal = true;
        lastScoredPlayer = true;

        if (popupManager != null) popupManager.ShowPlayerScore();
        if (resetPaddlesOnGoal) StartCoroutine(SoftResetPaddles());

        CheckWinConditions();
    }

    private void HandleEnemyScored()
    {
        if (isProcessingGoal) return;

        isProcessingGoal = true;
        lastScoredPlayer = false;

        if (popupManager != null) popupManager.ShowEnemyScore();
        if (resetPaddlesOnGoal) StartCoroutine(SoftResetPaddles());

        CheckWinConditions();
    }

    private IEnumerator StartGameProcedure()
    {
        yield return new WaitForEndOfFrame();

        if (StartProcedure.Instance != null)
        {
            yield return StartProcedure.Instance.StartCountdown();
        }

        if (ballController != null)
            ballController.ResetBall();
    }
    private IEnumerator RestartGameProcedure()
    {
        if (resetPaddlesOnGoal)
        {
            yield return StartCoroutine(SoftResetPaddles());
        }

        yield return StartCoroutine(StartGameProcedure());

    }

    void SavePaddleStartPositions()
    {
        if (leftPaddle != null)
            leftPaddleStartPosition = leftPaddle.position;

        if (rightPaddle != null)
            rightPaddleStartPosition = rightPaddle.position;
    }

    void InitializeManagers()
    {
        timeManager = GetComponentInChildren<GameTimeManager>();
        if (timeManager == null)
        {
            GameObject timeManagerObj = new GameObject("GameTimeManager");
            timeManagerObj.transform.SetParent(transform);
            timeManager = timeManagerObj.AddComponent<GameTimeManager>();
        }

        ballController = FindAnyObjectByType<BallController>();
        if (ballController == null)
            Debug.LogError("BallController not found!");

        if (popupManager == null)
            popupManager = FindAnyObjectByType<PopupManager>();

        if (leftPaddle == null)
            leftPaddle = GameObject.Find("Paddle_Left")?.transform;
        if (rightPaddle == null)
            rightPaddle = GameObject.Find("Paddle_Right")?.transform;
    }

    private void SetPaddlesResetState(bool resetting)
    {
        if (leftPaddle != null)
        {
            PaddleController leftController = leftPaddle.GetComponent<PaddleController>();
            if (leftController != null)
                leftController.SetResetState(resetting);
        }

        if (rightPaddle != null)
        {
            AIController rightAI = rightPaddle.GetComponent<AIController>();
            if (rightAI != null)
                rightAI.SetResetState(resetting);
        }
    }

    private IEnumerator SoftResetPaddles()
    {
        SetPaddlesResetState(true);

        BallController tempBall = ballController;
        ballController = null;

        Vector3 leftStartPos = leftPaddle != null ? leftPaddle.position : leftPaddleStartPosition;
        Vector3 rightStartPos = rightPaddle != null ? rightPaddle.position : rightPaddleStartPosition;

        float progress = 0f;
        while (progress < 1f)
        {
            progress += paddleResetSpeed * Time.unscaledDeltaTime;
            progress = Mathf.Clamp01(progress);

            if (leftPaddle != null)
                leftPaddle.position = Vector3.Lerp(leftStartPos, leftPaddleStartPosition, progress);
            if (rightPaddle != null)
                rightPaddle.position = Vector3.Lerp(rightStartPos, rightPaddleStartPosition, progress);

            yield return null;
        }

        if (leftPaddle != null) leftPaddle.position = leftPaddleStartPosition;
        if (rightPaddle != null) rightPaddle.position = rightPaddleStartPosition;

        ballController = tempBall;

        SetPaddlesResetState(false);
    }
    private bool CheckWinConditions()
    {
        if (ScoreManager.Instance == null) return false;

        if (ScoreManager.Instance.PlayerScore >= winScore)
        {
            if (popupManager != null) popupManager.ShowVictory();
            StartCoroutine(PauseAfterGameEnd(true));
            return true;
        }
        else if (ScoreManager.Instance.EnemyScore >= winScore)
        {
            if (popupManager != null) popupManager.ShowDefeat();
            StartCoroutine(PauseAfterGameEnd(false));
            return true;
        }
        else
        {
            StartCoroutine(PauseAndResetBall());
            return false;
        }
    }

    private void ShowGameEndMenu(bool isPlayerWin)
    {
        StartCoroutine(ShowEndGameOverlay());
        StartCoroutine(WaitForRestartInput());
    }

    private IEnumerator ShowEndGameOverlay()
    {
        if (endgameCanvasPrefab != null && endGameOverlayInstance == null)
        {
            endGameOverlayInstance = Instantiate(endgameCanvasPrefab);

            CanvasGroup canvasGroup = endGameOverlayInstance.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                float fadeDuration = 0.5f;
                float elapsed = 0f;

                while (elapsed < fadeDuration)
                {
                    canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                    elapsed += Time.unscaledDeltaTime;
                    yield return null;
                }

                canvasGroup.alpha = 1f;
            }
        }
    }

    private IEnumerator WaitForRestartInput()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        while (true)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                DestroyEndGameOverlay();
                RestartGame();
                yield break;
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                DestroyEndGameOverlay();
                ReturnToMainMenu();
                yield break;
            }

            yield return null;
        }
    }

    private void DestroyEndGameOverlay()
    {
        if (endGameOverlayInstance != null)
        {
            Destroy(endGameOverlayInstance);
            endGameOverlayInstance = null;
        }
    }

    private void DestroyGameSystems()
    {
        if (StartProcedure.Instance != null)
        {
            Destroy(StartProcedure.Instance.gameObject);
        }

        DestroyEndGameOverlay();
    }
    
    private IEnumerator ResetGoalProcessing()
    {
        yield return new WaitForEndOfFrame();
        isProcessingGoal = false;
    }

    private IEnumerator PauseAndResetBall()
    {
        if (timeManager != null)
            yield return timeManager.PauseAfterGoal();
        else
            yield return new WaitForSecondsRealtime(1.5f);

        if (ballController != null)
            ballController.ResetBall();

        yield return StartCoroutine(ResetGoalProcessing());
    }

    private IEnumerator PauseAndResetAfterDraw()
    {
        if (isMenuDemoMode)
        {
            yield return new WaitForSecondsRealtime(2f);
        }
        else
        {
            if (timeManager != null)
                yield return timeManager.PauseAfterDraw();
            else
                yield return new WaitForSecondsRealtime(2f);
        }

        if (ballController != null)
            ballController.ResetBall();

        yield return StartCoroutine(ResetGoalProcessing());
    }

    private IEnumerator PauseAfterGameEnd(bool isPlayerWin)
    {
        if (isMenuDemoMode)
        {
            yield return new WaitForSecondsRealtime(3f);
            RestartGame();
        }
        else
        {
            if (timeManager != null)
                yield return timeManager.PauseAfterGameEnd();
            else
                yield return new WaitForSecondsRealtime(3f);

            ShowGameEndMenu(isPlayerWin);
        }
    }

    public void Scored(bool isPlayerGoal)
    {
        if (isPlayerGoal)
            ScoreManager.Instance.PlayerScored();
        else
            ScoreManager.Instance.EnemyScored();
    }

    public void DeclareDraw()
    {
        if (isProcessingGoal) return;
        isProcessingGoal = true;

        if (popupManager != null) popupManager.ShowDraw();
        if (resetPaddlesOnGoal) StartCoroutine(SoftResetPaddles());

        StartCoroutine(PauseAndResetAfterDraw());
    }

    public void RestartGame()
    {
        DestroyEndGameOverlay();

        ScoreManager.Instance.FullReset();

        isProcessingGoal = false;
        Time.timeScale = 1f;

        StopAllCoroutines();
        StartCoroutine(RestartGameProcedure());
    }

    public void ExitGame()
    {
        Application.Quit();
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }

    public bool IsGameStarted() => isGameStarted;

    [ContextMenu("Test Player Score")]
    void TestPlayerScore() => ScoreManager.Instance.PlayerScored();

    [ContextMenu("Test Enemy Score")]
    void TestEnemyScore() => ScoreManager.Instance.EnemyScored();

    [ContextMenu("Test Draw")]
    void TestDraw() => DeclareDraw();
}
