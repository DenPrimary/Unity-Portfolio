using UnityEngine;
using UnityEngine.UI;

public class PowerBar : MonoBehaviour
{
    [Header("UI Refs")]
    public GameObject barRoot;
    public RectTransform cursor;
    public float barWidth = 400f;

    [Header("Settings")]
    public float startSpeed = 2f;
    public float maxSpeed = 6f;
    public float acceleration = 1f;

    private bool _isCharging = false;
    private float _value = 0f;
    private int _direction = 1;
    private float _currentSpeed;

    private void Update()
    {
        if (!_isCharging) return;

        _currentSpeed = Mathf.Min(_currentSpeed + acceleration * Time.deltaTime, maxSpeed);

        _value += _direction * _currentSpeed * Time.deltaTime;

        if (_value >= 1f)
        {
            _value = 1f;
            _direction = -1;
        }
        else if (_value <= 0f)
        {
            _value = 0f;
            _direction = 1;
        }

        cursor.anchoredPosition = new Vector2(_value * barWidth, cursor.anchoredPosition.y);
    }

    public void StartCharge()
    {
        if (barRoot != null) barRoot.SetActive(true);
        _isCharging = true;
        _value = 0f;
        _direction = 1;
        _currentSpeed = startSpeed;
        cursor.anchoredPosition = new Vector2(0f, cursor.anchoredPosition.y);
    }

    public float StopAndGetTiming()
    {
        _isCharging = false;
        if (barRoot != null) barRoot.SetActive(false);
        return _value;
    }
}
