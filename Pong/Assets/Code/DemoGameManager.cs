using UnityEngine;
using System.Collections;

public class DemoGameManager : MonoBehaviour
{
    public static DemoGameManager Instance;

    public int winScore = 11;
    public Transform leftPaddle;
    public Transform rightPaddle;
    public Vector3 leftPaddleStartPosition;
    public Vector3 rightPaddleStartPosition;
    public float paddleResetSpeed = 8f;

    private BallController ballController;
    private AIController leftAI;
    private AIController rightAI;
    private bool isProcessingGoal = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SavePaddleStartPositions();
        FindBallController();
        FindAIControllers();
        StartDemoMatch();
    }

    private void SavePaddleStartPositions()
    {
        if (leftPaddle != null)
            leftPaddleStartPosition = leftPaddle.position;
        if (rightPaddle != null)
            rightPaddleStartPosition = rightPaddle.position;
    }

    private void FindBallController()
    {
        ballController = FindAnyObjectByType<BallController>();
        if (ballController == null)
            Debug.LogError("BallController not found in demo!");
    }

    private void FindAIControllers()
    {
        if (leftPaddle != null)
            leftAI = leftPaddle.GetComponent<AIController>();
        if (rightPaddle != null)
            rightAI = rightPaddle.GetComponent<AIController>();
    }

    public void StartDemoMatch()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.FullReset();

        ResetPaddles();

        if (ballController != null)
            ballController.ResetBall();
    }

    private void ResetPaddles()
    {
        if (leftPaddle != null)
            leftPaddle.position = leftPaddleStartPosition;
        if (rightPaddle != null)
            rightPaddle.position = rightPaddleStartPosition;
    }

    private void SetPaddlesResetState(bool resetting)
    {
        if (leftAI != null)
            leftAI.SetResetState(resetting);
        if (rightAI != null)
            rightAI.SetResetState(resetting);
    }

    public void OnDemoGoalScored(bool isLeftGoal)
    {
        if (isProcessingGoal) return;

        isProcessingGoal = true;
        StartCoroutine(HandleDemoGoal(isLeftGoal));
    }

    private IEnumerator HandleDemoGoal(bool isLeftGoal)
    {
        // Устанавливаем состояние сброса для AI
        SetPaddlesResetState(true);

        // Мягкий сброс ракеток
        yield return StartCoroutine(SoftResetPaddles());

        yield return new WaitForSecondsRealtime(1.0f);

        if (ballController != null)
            ballController.ResetBall();

        // Снимаем состояние сброса
        SetPaddlesResetState(false);
        isProcessingGoal = false;

        CheckDemoWinConditions();
    }

    private IEnumerator SoftResetPaddles()
    {
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

        // Финальная точная установка позиций
        if (leftPaddle != null) leftPaddle.position = leftPaddleStartPosition;
        if (rightPaddle != null) rightPaddle.position = rightPaddleStartPosition;
    }

    private void CheckDemoWinConditions()
    {
        if (ScoreManager.Instance != null &&
            (ScoreManager.Instance.PlayerScore >= winScore ||
             ScoreManager.Instance.EnemyScore >= winScore))
        {
            StartCoroutine(RestartDemoMatch());
        }
    }

    private IEnumerator RestartDemoMatch()
    {
        yield return new WaitForSecondsRealtime(2.0f);
        StartDemoMatch();
    }

    public void DemoLeftGoal() => OnDemoGoalScored(true);
    public void DemoRightGoal() => OnDemoGoalScored(false);
}
