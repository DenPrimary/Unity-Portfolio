using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    public static ProgressionManager Instance { get; private set; }

    public PlayerProgress PlayerProgress { get; private set; }
    public UpgradeApply UpgradeApply { get; private set; }
    public LvlUpManager LvlUpManager { get; private set; }

    void Awake() {
        if (Instance == null)
            Instance = this;
        else {
            Destroy(gameObject);
            return;
        }

        PlayerProgress = GetComponent<PlayerProgress>();
        UpgradeApply = GetComponent<UpgradeApply>();
        LvlUpManager = GetComponent<LvlUpManager>();
    }
}
