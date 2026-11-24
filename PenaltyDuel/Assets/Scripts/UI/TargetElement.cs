using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TargetElement : MonoBehaviour, IPointerEnterHandler,
    IPointerExitHandler, IPointerClickHandler
{
    [Header("Zone")]
    [Range(1, 9)]
    public int zoneNumber = 1;

    [Header("Highlight")]
    public Color normalColor = new Color(1f, 1f, 1f, 0.3f);
    public Color hoverColor = new Color(1f, 1f, 0f, 0.7f);
    public Color selectedColor = new Color(0f, 1f, 0f, 0.7f);
    public Color perfectColor = new Color(0.6f, 0f, 1f, 0.8f);

    private Image _image;
    private bool _isPerfect = false;
    private static TargetElement _currentlySelected;

    public static event System.Action<int> OnTargetClicked;

    private void Awake()
    {
        _image = GetComponent<Image>();
        ResetColor();
    }

    public void SetPerfect(bool isPerfect)
    {
        _isPerfect = isPerfect;
        ResetColor();
    }

    private void ResetColor()
    {
        if (_image == null) return;

        if (_currentlySelected == this)
            _image.color = selectedColor;
        else if (_isPerfect)
            _image.color = perfectColor;
        else
            _image.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_image != null && _currentlySelected != this && !_isPerfect)
            _image.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_image != null && _currentlySelected != this)
            ResetColor();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_currentlySelected != null && _currentlySelected != this)
            _currentlySelected.ResetColor();

        _currentlySelected = this;
        ResetColor();
        OnTargetClicked?.Invoke(zoneNumber);
    }

    public static void ClearSelection()
    {
        if (_currentlySelected != null)
        {
            var old = _currentlySelected;
            _currentlySelected = null;
            old.ResetColor();
        }
    }
}
