using UnityEngine;

public class FPSManager : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 120;
        QualitySettings.vSyncCount = 0;
    }
}
