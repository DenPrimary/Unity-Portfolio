using UnityEngine;
using TMPro;
using System.Collections;

public class PopupManager : MonoBehaviour
{
    [Header("Popup Prefab")]
    public GameObject popupCanvasPrefab;

    [Header("Popup Settings")]
    public float displayTime = 2f;
    public float fadeDuration = 0.5f;

    [Header("Event Messages")]
    public string playerScoreMsg = "PLAYER SCORE!";
    public string enemyScoreMsg = "ENEMY SCORE!";
    public string drawMsg = "DRAW!";
    public string victoryMsg = "VICTORY!";
    public string defeatMsg = "DEFEAT!";

    public Color backgroundColor = new Color(0f, 0f, 0f, 0.8f);
    public Vector2 backgroundPadding = new Vector2(100f, 50f);

    private CanvasGroup canvasGroup;
    private GameObject currentPopup;
    private TextMeshProUGUI popupText;

    void Start()
    {
        InitializePopupSystem();
        HidePopup();
    }

    void InitializePopupSystem()
    {
        if (popupCanvasPrefab == null)
        {
            Debug.LogError("Popup Canvas Prefab is not assigned!");
            return;
        }

        currentPopup = Instantiate(popupCanvasPrefab);
        currentPopup.name = "PopupCanvas";

        popupText = currentPopup.GetComponentInChildren<TextMeshProUGUI>();
        canvasGroup = currentPopup.GetComponent<CanvasGroup>();

        if (popupText == null)
            Debug.LogError("TextMeshProUGUI component not found in prefab!");
        if (canvasGroup == null)
            Debug.LogError("CanvasGroup component not found in prefab!");


        Canvas canvas = currentPopup.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;
        }

        DontDestroyOnLoad(currentPopup);
    }

    private IEnumerator ShowAndHidePopups(string message)
    {
        if (popupText == null || canvasGroup == null)
        {
            Debug.LogError("Popup components are not initialized!");
            yield break;
        }

        // Устанавливаем сообщение
        popupText.text = message;

        // Показываем попап
        currentPopup.SetActive(true);
        currentPopup.transform.localScale = Vector3.zero;
        canvasGroup.alpha = 0f;

        Debug.Log("Popup showing: " + message);

        // Анимация появления
        float elapsed = 0f;
        while (elapsed < 0.3f)
        {
            float scale = Mathf.Lerp(0f, 1f, elapsed / 0.3f);
            currentPopup.transform.localScale = new Vector3(scale, scale, scale);
            canvasGroup.alpha = scale;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        currentPopup.transform.localScale = Vector3.one;
        canvasGroup.alpha = 1.0f;

        // Ждем указанное время
        float timer = 0f;
        while (timer < displayTime)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        // Анимация исчезновения
        elapsed = 0f;
        Vector3 startScale = currentPopup.transform.localScale;

        while (elapsed < fadeDuration)
        {
            float progress = elapsed / fadeDuration;
            currentPopup.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, progress);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, progress);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        HidePopup();
    }

    private void HidePopup()
    {
        if (currentPopup != null)
        {
            currentPopup.SetActive(false);
            if (canvasGroup != null)
                canvasGroup.alpha = 0f;
        }
    }

    public void ShowPopup(string message)
    {
        if (popupText == null)
        {
            Debug.LogError("Cannot show popup - popupText is null!");
            return;
        }

        Debug.Log("Attempting to show popup: " + message);
        StartCoroutine(ShowAndHidePopups(message));
    }

    [ContextMenu("Test Player Score")]
    void TestPlayerScorePopup() => ShowPopup(playerScoreMsg);

    [ContextMenu("Test Enemy Score")]
    void TestEnemyScorePopup() => ShowPopup(enemyScoreMsg);

    [ContextMenu("Test Draw")]
    void TestDrawPopup() => ShowPopup(drawMsg);

    [ContextMenu("Test Victory")]
    void TestVictoryPopup() => ShowPopup(victoryMsg);

    [ContextMenu("Test Defeat")]
    void TestDefeatPopup() => ShowPopup(defeatMsg);

    public void ShowPlayerScore() => ShowPopup(playerScoreMsg);
    public void ShowEnemyScore() => ShowPopup(enemyScoreMsg);
    public void ShowDraw() => ShowPopup(drawMsg);
    public void ShowVictory() => ShowPopup(victoryMsg);
    public void ShowDefeat() => ShowPopup(defeatMsg);
}
