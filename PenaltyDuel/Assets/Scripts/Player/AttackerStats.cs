using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttackerStats : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text accuracyText;
    public TMP_Text composureText;

    [Header("Stats")]
    public float accuracy = 0.5f;
    public float composure = 0.5f;

    private void Start()
    {
        // rand stats 50-100, saving as 0.5-1.0
        int accRoll = Random.Range(50, 101);
        int compRoll = Random.Range(50, 101);

        accuracy = accRoll / 100f;
        composure = compRoll / 100f;

        if (accuracyText != null)
            accuracyText.text = $"Accuracy: {accRoll}";
        if (composureText != null)
            composureText.text = $"Composure: {compRoll}";

        Debug.Log($"Attacker stats: Accuracy={accRoll}, Composure={compRoll}");
    }
}
