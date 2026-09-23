using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    [Header("Lvl & Exp")]
    public int currentLvl = 0;
    public int currentExp = 0;

    [Header("Stats")]
    public float bulletDmg = 2f;
    public float fireRate = 2f;
    public float firePause = 1f;
    public float bulletSpeed = 25f;

    public float minFireRate = 0.3f;
    public float maxBulletSpeed = 75f;

    public System.Action OnLvlUp;
    public System.Action OnStatsUpdated;

    void Start() 
    {
        UpdateFirePause();
    }

    private void UpdateFirePause()
    {
        float upgrades = (2f - fireRate) / 0.1f;
        float newFirePause = 1f - (upgrades * 0.05f);
        firePause = Mathf.Round(Mathf.Max(0.1f, newFirePause) * 100f) / 100f;
    }

    public void AddExp(int amount) {
        currentExp += amount;
    }

    public void ApplyUpgrade(string upgradeType) 
    {
        switch (upgradeType)
        {
            case "Damage":
                bulletDmg += 0.5f;
                bulletDmg = Mathf.Round(bulletDmg * 10f) / 10f;
                break;
            case "FireRate":
                fireRate -= 0.1f;
                fireRate = Mathf.Max(minFireRate, fireRate);
                fireRate = Mathf.Round(fireRate * 100f) / 100f;
                UpdateFirePause();
                break;
            case "BulletSpeed":
                bulletSpeed += 5f;
                bulletSpeed = Mathf.Min(maxBulletSpeed, bulletSpeed);
                break;
        }

        OnStatsUpdated?.Invoke();
    }

    public float GetFirePause() 
    { 
        return firePause;
    }

    public bool CanUpgradeDamage() => true;
    public bool CanUpgradeFireRate() => fireRate > minFireRate;
    public bool CanUpgradeBulletSpeed() => bulletSpeed < maxBulletSpeed;
}
