using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    private WeaponGauss[] guns;
    private int currentGunIndex = 0;
    private float nextShotTime = 0f;

    public PlayerProgress playerProgress;

    void Start()
    {
        RefreshGuns();

        playerProgress = ProgressionManager.Instance?.PlayerProgress;

        if (playerProgress != null) 
        {
            playerProgress.OnStatsUpdated += OnStatsUpdated;
            UpdateFirePause();
        }
    }

    void Update()
    {
        if (guns == null || guns.Length == 0) return;

        if (Time.time < nextShotTime) return;

        if (guns[currentGunIndex] != null)
            guns[currentGunIndex].Shoot();

        currentGunIndex = (currentGunIndex + 1) % guns.Length;
        nextShotTime = Time.time + GetFirePause();
    }

    void OnStatsUpdated() 
    {
        UpdateFirePause();
    }

    void UpdateFirePause()
    {
        if (playerProgress == null) return;

        float pause = playerProgress.GetFirePause();
    }

    float GetFirePause() 
    {
        if (playerProgress != null)
            return playerProgress.firePause;
        return 1f;
    }

    public void RefreshGuns()
    {
        guns = GetComponentsInChildren<WeaponGauss>();
        currentGunIndex = 0;
        nextShotTime = 0f;
    }

    void OnDestroy() 
    { 
        if (playerProgress != null)
            playerProgress.OnStatsUpdated -= OnStatsUpdated;
    }
}
