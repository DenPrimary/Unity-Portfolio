using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI enemyScoreText;

    public string scoreFormat = "00";
    public bool showLeadingZeros = true;

    private int lastPlayerScore = -1;
    private int lastEnemyScore = -1;
    private bool isDemoMode = false;

    void Start()
    {
        isDemoMode = FindAnyObjectByType<DemoGameManager>() != null;

        if (!isDemoMode)
        {
            ScoreManager.OnScoreChanged += HandleScoreChanged;
        }

        UpdateScoreDisplay();
    }

    private void OnDestroy()
    {
        if (!isDemoMode)
        {
            ScoreManager.OnScoreChanged -= HandleScoreChanged;
        }
    }

    void Update()
    {
        if (isDemoMode)
        {
            UpdateScoreDisplay();
        }
    }

    private void HandleScoreChanged(int playerScore, int enemyScore)
    {
        UpdateScoreDisplay();
    }

    void UpdateScoreDisplay()
    {
        ScoreManager scoreManager = ScoreManager.Instance;

        if (scoreManager == null)
        {
            scoreManager = FindAnyObjectByType<ScoreManager>();
            if (scoreManager == null) return;
        }

        // Обновляем только если счет изменился
        if (scoreManager.PlayerScore != lastPlayerScore ||
            scoreManager.EnemyScore != lastEnemyScore)
        {
            lastPlayerScore = scoreManager.PlayerScore;
            lastEnemyScore = scoreManager.EnemyScore;

            playerScoreText.text = FormatScore(scoreManager.PlayerScore);
            enemyScoreText.text = FormatScore(scoreManager.EnemyScore);
        }
    }

    private string FormatScore(int score)
    {
        if (showLeadingZeros && score < 10)
            return "0" + score.ToString();
        else
            return score.ToString();
    }
}
