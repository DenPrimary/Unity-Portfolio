using UnityEngine;

public class HighlightManager : MonoBehaviour
{
    public static HighlightManager Instance { get; private set; }

    [Header("Settings")]
    public Color highlightColor = new Color(0f, 0.8f, 1f, 1f);

    private GameObject lastHighlighted;
    private Outline lastOutline;
    private Color lastOriginalColor;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void HighlightAsteroid(GameObject asteroid)
    {
        ClearHighlight();

        if (asteroid == null)
            return;

        Outline outline = asteroid.GetComponent<Outline>();
        if (outline != null)
        {
            lastHighlighted = asteroid;
            lastOutline = outline;
            lastOriginalColor = outline.OutlineColor;
            outline.OutlineColor = highlightColor;
        }
    }

    public void ClearHighlight()
    {
        if (lastHighlighted == null || lastOutline == null)
        {
            lastOutline = null;
            lastHighlighted = null;
            return;
        }

        lastOutline.OutlineColor = lastOriginalColor;
        lastOutline = null;
        lastHighlighted = null;
    }

    public void OnAsteroidDestroyed(GameObject asteroid)
    {
        if (lastHighlighted == asteroid || lastHighlighted == null)
        {
            ClearHighlight();
        }
    }
}

