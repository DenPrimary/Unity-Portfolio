using UnityEngine;
using TMPro;

public class DemoScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI enemyScoreText;

    void Start()
    {
        // Убедимся что ScoreManager существует
        EnsureScoreManagerExists();

        // Подписываемся на событие изменения счета
        ScoreManager.OnScoreChanged += HandleScoreChanged;

        // Initial update
        UpdateScoreDisplay();
    }

    private void EnsureScoreManagerExists()
    {
        if (ScoreManager.Instance == null)
        {
            // Ищем существующий ScoreManager на сцене
            ScoreManager existingManager = FindFirstObjectByType<ScoreManager>();
            if (existingManager != null)
            {
                // Если нашли, он сам установит себя как Instance в Awake()
            }
            else
            {
                // Создаем новый и он сам установит себя как Instance
                GameObject scoreManagerObj = new GameObject("ScoreManager");
                scoreManagerObj.AddComponent<ScoreManager>();
            }
        }
    }

    private void OnDestroy()
    {
        // Отписываемся от события
        ScoreManager.OnScoreChanged -= HandleScoreChanged;
    }

    private void HandleScoreChanged(int playerScore, int enemyScore)
    {
        UpdateScoreDisplay();
    }

    void UpdateScoreDisplay()
    {
        if (ScoreManager.Instance == null)
        {
            Debug.LogWarning("ScoreManager.Instance is still null!");
            return;
        }

        playerScoreText.text = ScoreManager.Instance.PlayerScore.ToString("00");
        enemyScoreText.text = ScoreManager.Instance.EnemyScore.ToString("00");

        Debug.Log($"Score updated: {ScoreManager.Instance.PlayerScore} - {ScoreManager.Instance.EnemyScore}");
    }
}
