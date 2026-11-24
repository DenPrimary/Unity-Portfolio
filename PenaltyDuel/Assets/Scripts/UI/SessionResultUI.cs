using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class SessionResultUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject sessionResultPanel;
    public TMP_Text scoreText;

    private PenaltyGameManager _gameManager;

    private void Start()
    {
        sessionResultPanel.SetActive(false);

        _gameManager = PenaltyGameManager.Instance;

        if (_gameManager == null)
        {
            Debug.LogError("PenaltyGameManager not found!");
            return;
        }

        _gameManager.shotsTaken.OnValueChanged += OnShotsTakenChanged;

        if (_gameManager.shotsTaken.Value >= 5)
            ShowResult();
    }

    private void OnShotsTakenChanged(int previousValue, int newValue)
    {
        if (newValue >= 5)
            ShowResult();
    }

    private void ShowResult()
    {
        int finalScore = _gameManager.score.Value;

        int stars = finalScore >= 1000 ? 3 :
                    finalScore >= 700 ? 2 :
                    finalScore >= 400 ? 1 : 0;

        string starText = stars == 1 ? "1 Star" : $"{stars} Stars";

        scoreText.text = $"Score: {finalScore}\n\n{starText}";
        sessionResultPanel.SetActive(true);
    }

    private void OnDestroy()
    {
        if (_gameManager != null)
            _gameManager.shotsTaken.OnValueChanged -= OnShotsTakenChanged;
    }

    public void BackToMenu()
    {
        if (NetworkManager.Singleton != null &&
            NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(
                "PenaltyScene",
                LoadSceneMode.Single
            );
        }
    }
}
