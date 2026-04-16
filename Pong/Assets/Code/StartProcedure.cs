using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class StartProcedure : MonoBehaviour
{
    public static StartProcedure Instance;

    public GameObject countdownCanvas;
    public TextMeshProUGUI countdownText;
    public Image countdownBackground;
    public float countdownTime = 3f;
    //public AudioClip countdownSound;
    //public AudioClip startSound;

    public Color backgroundColor = new Color(0.141f, 0.118f, 0.114f);
    public Vector2 numberBackgroundSize = new Vector2(300, 300);
    public Vector2 textBackgroundSize = new Vector2(900, 300);
    public float backgroundPadding = 20f;

    public float scaleEffectSize = 1.5f;
    public float scaleEffectDuration = 0.3f;

    private bool isCountingDown = false;
    private RectTransform backgroundRect;
    //private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            //audioSource = gameObject.AddComponent<AudioSource>();

            InitializeBackground();

            if (countdownCanvas != null)
                countdownCanvas.SetActive(false);
        }
        else
        { 
            Destroy(gameObject);
        }
    }

    //private void PlaySound(AudioClip clip)
    //{
    //    if (audioSource != null && clip != null)
    //    {
    //        audioSource.PlayOneShot(clip);
    //    }
    //}

    private void InitializeBackground()
    {
        if (countdownBackground != null)
        {
            backgroundRect = countdownBackground.GetComponent<RectTransform>();
            countdownBackground.color = backgroundColor;
        }
    }

    public IEnumerator StartCountdown()
    {
        if (isCountingDown) yield break;

        isCountingDown = true;
        Time.timeScale = 0f;

        if (countdownCanvas != null)
            countdownCanvas.SetActive(true);

        for (int i = (int)countdownTime; i > 0; i--)
        {
            if (countdownText != null)
            {
                countdownText.text = i.ToString();
                ResizeBackgroundForNumber(i);
                yield return StartCoroutine(ScaleTextEffect(countdownText.gameObject));
            }

            //PlaySound(countdownSound);
            yield return new WaitForSecondsRealtime(1f);
        }

        // "START!"
        if (countdownText != null)
        {
            countdownText.text = "START!";
            ResizeBackgroundForText();
            yield return new WaitForSecondsRealtime(0.5f); 
        }

        //PlaySound(startSound);
        yield return new WaitForSecondsRealtime(0.5f);

        if (countdownCanvas != null)
            countdownCanvas.SetActive(false);

        Time.timeScale = 1f;
        isCountingDown = false;
    }

    private void ResizeBackgroundForNumber(int number)
    {
        if (backgroundRect != null)
        {
            backgroundRect.sizeDelta = numberBackgroundSize;

            if (countdownText != null)
            {
                Vector2 textSize = countdownText.GetPreferredValues();
                backgroundRect.sizeDelta = textSize + new Vector2(backgroundPadding, backgroundPadding);
            }
        }
    }

    private void ResizeBackgroundForText()
    {
        if (backgroundRect != null && countdownText != null)
        {
            Vector2 textSize = countdownText.GetPreferredValues("START!");
            backgroundRect.sizeDelta = textSize + new Vector2(backgroundPadding, backgroundPadding);
        }
    }

    private IEnumerator ScaleTextEffect(GameObject textObject)
    {
        Vector3 originalScale = textObject.transform.localScale;
        Vector3 targetScale = originalScale * scaleEffectSize;

        float elapsed = 0f;
        while (elapsed < scaleEffectDuration)
        {
            textObject.transform.localScale = Vector3.Lerp(
                originalScale,
                targetScale,
                elapsed / scaleEffectDuration
            );
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < scaleEffectDuration)
        {
            textObject.transform.localScale = Vector3.Lerp(
                targetScale,
                originalScale,
                elapsed / scaleEffectDuration
            );
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        textObject.transform.localScale = originalScale;
    }

    public bool IsCountingDown()
    {
        return isCountingDown;
    }

    [ContextMenu("Test Countdown")]
    public void TestCountdown()
    {
        StartCoroutine(StartCountdown());
    }

    [ContextMenu("Test Number Background")]
    public void TestNumberBackground()
    {
        ResizeBackgroundForNumber(3);
    }

    [ContextMenu("Test Text Background")]
    public void TestTextBackground()
    {
        ResizeBackgroundForText();
    }
}