using UnityEngine;
using TMPro;

public class StatsDisplay : MonoBehaviour
{
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI fireRateText;
    public TextMeshProUGUI bulletSpeedText;

    public PlayerProgress playerProgress;

    [Header("Colors")]
    public Color statColor = new Color(0.992f, 0.698f, 0f);

    void Start()
    {
        playerProgress = ProgressionManager.Instance?.PlayerProgress;

        if (playerProgress != null)
        {
            playerProgress.OnStatsUpdated += UpdateStats;
            UpdateStats();
        }
    }

    void UpdateStats()
    {
        if (playerProgress == null) return;

        damageText.text = $"Damage: <color=#{ColorUtility.ToHtmlStringRGB(statColor)}>{playerProgress.bulletDmg:F1}</color>";
        fireRateText.text = $"FireRate: <color=#{ColorUtility.ToHtmlStringRGB(statColor)}>{playerProgress.fireRate:F2}</color>";
        bulletSpeedText.text = $"BulletSpeed: <color=#{ColorUtility.ToHtmlStringRGB(statColor)}>{playerProgress.bulletSpeed:F0}</color>";
    }
}
