using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    [Header("Refs")]
    public PowerBar powerBar;
    public AttackerStats stats;

    private int _selectedZone = 0;
    private bool _isCharging = false;

    private void OnEnable()
    {
        TargetElement.OnTargetClicked += OnTargetClicked;
    }

    private void OnDisable()
    {
        TargetElement.OnTargetClicked -= OnTargetClicked;
    }

    private void OnTargetClicked(int zone)
    {
        _selectedZone = zone;
        Debug.Log($"Zone selected: {zone}");
    }

    private void Update()
    {
        if (_selectedZone == 0) return;

        if (Input.GetMouseButtonDown(0) && !_isCharging)
        {
            _isCharging = true;
            powerBar.StartCharge();
        }

        if (Input.GetMouseButtonUp(0) && _isCharging)
        {
            _isCharging = false;
            float timing = powerBar.StopAndGetTiming();
            Debug.Log($"Shot to zone {_selectedZone}, timing: {timing:F2}");

            if (PenaltyGameManager.Instance != null && PenaltyGameManager.Instance.IsSpawned)
            {
                float acc = stats != null ? stats.accuracy : 0.5f;
                float comp = stats != null ? stats.composure : 0.5f;
                PenaltyGameManager.Instance.SubmitShotRpc(_selectedZone, timing, acc, comp);
            }

            _selectedZone = 0;
            TargetElement.ClearSelection();
        }
    }
}
