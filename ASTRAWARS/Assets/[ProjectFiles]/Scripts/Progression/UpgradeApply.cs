using UnityEngine;

public class UpgradeApply : MonoBehaviour
{
    public PlayerProgress playerProgress;

    void Start()
    {
        playerProgress = ProgressionManager.Instance?.PlayerProgress;
    }

    public void ApplyStatsToWeapon()
    {
        if (playerProgress == null) return;

        WeaponGauss[] weapons = FindObjectsByType<WeaponGauss>();
        foreach (var weapon in weapons)
        {
            if (weapon != null)
            {
                weapon.fireRate = playerProgress.fireRate;
                weapon.bulletSpeed = playerProgress.bulletSpeed;
                weapon.damage = playerProgress.bulletDmg;
            }
        }

        GaussBullet[] bullets = FindObjectsByType<GaussBullet>();
        foreach (var bullet in bullets)
        {
            if (bullet != null)
                bullet.damage = playerProgress.bulletDmg;
        }
    }
}
