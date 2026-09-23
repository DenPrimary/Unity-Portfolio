using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIUpgradePanel : MonoBehaviour
{
    [Header("UI")]
    public GameObject upgradePanel;
    public Button[] upgradeButtons;
    public Image[] upgradeIcons;

    [Header("Refs")]
    public PlayerProgress playerProgress;
    public UpgradeApply upgradeApply;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color disabledColor = new Color(0.32f, 0.31f, 0.31f, 1f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        upgradePanel.SetActive(false);

        GameObject manager = GameObject.Find("ProgressionManager");

        playerProgress = ProgressionManager.Instance?.PlayerProgress;
        upgradeApply = ProgressionManager.Instance?.UpgradeApply;

        if (playerProgress != null)
            playerProgress.OnLvlUp += ShowUpgradePanel;
    }

    void ShowUpgradePanel() 
    {
        upgradePanel.SetActive(true);
        Time.timeScale = 0f;

        if (upgradeButtons == null || upgradeButtons.Length < 3)
            return;

        SetupButton(0, "Damage", playerProgress.CanUpgradeDamage());
        SetupButton(1, "FireRate", playerProgress.CanUpgradeFireRate());
        SetupButton(2, "BulletSpeed", playerProgress.CanUpgradeBulletSpeed());
    }

    private void SetupButton(int index, string upgradeType, bool canUpgrade) {
        if (index >= upgradeButtons.Length) return;

        Button button = upgradeButtons[index];
        button.onClick.RemoveAllListeners();

        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
            buttonImage.color = canUpgrade ? normalColor : disabledColor;

        if (upgradeIcons != null && index < upgradeIcons.Length && upgradeIcons[index] != null)
            upgradeIcons[index].color = canUpgrade ? normalColor : disabledColor;

        if (canUpgrade) {
            button.interactable = true;
            button.onClick.AddListener(() => ApplyUpgrade(upgradeType));
        }
        else
            button.interactable = false;
    }

    void ApplyUpgrade(string upgradeType) 
    {
        Time.timeScale = 1f;
        upgradePanel.SetActive (false);

        if (playerProgress != null)
            playerProgress.ApplyUpgrade(upgradeType);

        if (upgradeApply != null)
            upgradeApply.ApplyStatsToWeapon();
    }
}
