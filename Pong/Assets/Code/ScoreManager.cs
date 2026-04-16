using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    // Events for other managers
    public static event Action<int, int> OnScoreChanged;
    public static event Action OnPlayerScoredEvent;
    public static event Action OnEnemyScoredEvent;

    // Properties for data
    public int PlayerScore { get; private set; }
    public int EnemyScore { get; private set; }

    private GameManager gameManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else 
        { 
            Destroy(gameObject);
            return;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindGameManager();

        // Сбрасываем счет только для игровых сцен, не для меню с демо
        if (scene.name == "ClassicGame" || scene.name == "VersusGame" || scene.name == "CasualMatchScene")
        {
            FullReset();
        }
        // Для ClassicModeMenu (где демо) НЕ сбрасываем автоматически
    }

    private void Start()
    {
        FindGameManager();
    }

    private void FindGameManager()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    private void OnDestroy()
    {
        if (Instance == this) 
        { 
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public void PlayerScored() 
    {
        PlayerScore++;
        Debug.Log($"Player scored! Total: {PlayerScore}");

        OnScoreChanged?.Invoke(PlayerScore, EnemyScore);
        OnPlayerScoredEvent?.Invoke();
    }

    public void EnemyScored() 
    { 
        EnemyScore++;
        Debug.Log($"Enemy scored! Total: {EnemyScore}");

        OnScoreChanged?.Invoke(PlayerScore, EnemyScore);
        OnEnemyScoredEvent?.Invoke();
    }

    public void FullReset()
    {
        PlayerScore = 0;
        EnemyScore = 0;
        Debug.Log("ScoreManager: Full game state reset");

        // Уведомляем об обнулении счета
        OnScoreChanged?.Invoke(PlayerScore, EnemyScore);
    }

    public (int player, int enemy) GetScores() => (PlayerScore, EnemyScore);
    public bool IsPlayerWon() => PlayerScore > EnemyScore;
    public bool IsEnemyWon() => PlayerScore < EnemyScore;
    public bool IsDraw() => PlayerScore == EnemyScore;
}
