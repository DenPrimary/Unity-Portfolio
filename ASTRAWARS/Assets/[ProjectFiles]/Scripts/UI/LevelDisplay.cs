using UnityEngine;
using TMPro;

public class LevelDisplay : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public PlayerProgress playerProgress;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerProgress = ProgressionManager.Instance?.PlayerProgress;

        if (levelText == null)
            levelText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerProgress == null || levelText == null) return;

        int currentExp = playerProgress.currentExp;
        int currentLvl = playerProgress.currentLvl;
        int requiredExp = ExpCalc.GetExpForLvl(currentLvl);

        levelText.text = $"{currentLvl} | {currentExp}/{requiredExp}";
    }
}
