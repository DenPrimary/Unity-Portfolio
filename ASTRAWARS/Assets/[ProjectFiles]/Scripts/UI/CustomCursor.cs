using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public Texture2D CursorTexture;
    public Vector2 hotSpot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    void Start()
    {
        if (CursorTexture != null)
            Cursor.SetCursor(CursorTexture, hotSpot, cursorMode);
    }

    void OnDestroy() 
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
