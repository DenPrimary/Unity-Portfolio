using UnityEngine;

public class GoalkeeperAI : MonoBehaviour
{
    [Header("Refs")]
    public RectTransform[] targets = new RectTransform[9];
    public Transform goalkeeperHome;
    public Transform ball;

    [Header("Movement")]
    public float moveSpeed = 10f;
    public float returnSpeed = 8f;

    private int[] _hitHistory = new int[9];
    private int _totalHits = 0;

    private Vector3 _targetPosition;
    private bool _isMoving = false;
    private bool _isReturning = false;
    private int _chosenZone = 0;

    private const int HISTORY_SIZE = 5;
    private int[] _recentHits = new int[HISTORY_SIZE];
    private int _hitIndex = 0;
    private int _hitCount = 0;

    private float[] weights = { 1.0f, 0.7f, 0.5f, 0.35f, 0.25f };
    private const float PERFECT_ZONE_BONUS = 1.5f;
    private const float ALPHA = 1f;

    private void Start()
    {
        if (goalkeeperHome != null)
            transform.position = goalkeeperHome.position;
    }

    private void Update()
    {
        if (_isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                _targetPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, _targetPosition) < 0.01f)
            {
                transform.position = _targetPosition;
                _isMoving = false;
            }
        }
        else if (_isReturning)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                goalkeeperHome.position, returnSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, goalkeeperHome.position) < 0.01f)
            {
                transform.position = goalkeeperHome.position;
                _isReturning = false;
            }
        }
    }

    public int ChooseSaveZone(int perfectZone)
    {
        float[] weightedCounts = new float[9];
        float totalWeight = 0f;

        // История (экспоненциальное затухание)
        for (int i = 0; i < _hitCount; i++)
        {
            int idx = (_hitIndex - 1 - i + HISTORY_SIZE) % HISTORY_SIZE;
            int zone = _recentHits[idx];
            if (zone >= 1 && zone <= 9)
            {
                weightedCounts[zone - 1] += weights[i];
                totalWeight += weights[i];
            }
        }

        // Perfect Zone бонус (вратарь знает)
        if (perfectZone >= 1 && perfectZone <= 9)
        {
            weightedCounts[perfectZone - 1] += PERFECT_ZONE_BONUS;
            totalWeight += PERFECT_ZONE_BONUS;
        }

        // Вероятности с сглаживанием
        float[] probs = new float[9];
        float total = totalWeight + 9f * ALPHA;
        for (int i = 0; i < 9; i++)
            probs[i] = (weightedCounts[i] + ALPHA) / total;

        // Сэмплирование
        float r = Random.value;
        float cumulative = 0f;
        int chosen = 0;
        for (int i = 0; i < 9; i++)
        {
            cumulative += probs[i];
            if (r <= cumulative)
            {
                chosen = i;
                break;
            }
        }

        _chosenZone = chosen + 1;

        if (targets[chosen] != null)
        {
            Vector3 target = targets[chosen].position;
            target.x = transform.position.x;
            _targetPosition = target;
            _isMoving = true;
            _isReturning = false;
        }

        Debug.Log($"Goalkeeper chose zone: {_chosenZone}, perfectZone: {perfectZone}, totalWeight: {totalWeight:F2}");
        return _chosenZone;
    }

    public void RegisterHit(int zone)
    {
        if (zone < 1 || zone > 9) return;
        _recentHits[_hitIndex] = zone;
        _hitIndex = (_hitIndex + 1) % HISTORY_SIZE;
        _hitCount = Mathf.Min(_hitCount + 1, HISTORY_SIZE);
    }

    public void ResetPosition()
    {
        _isMoving = false;
        _isReturning = true;
    }

    public int GetChosenZone() => _chosenZone;
}
