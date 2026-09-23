using UnityEngine;

public class LvlUpManager : MonoBehaviour
{
    public PlayerProgress playerProgress;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerProgress = ProgressionManager.Instance?.PlayerProgress;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerProgress == null) return;

        if (ExpCalc.CanLvlUp(playerProgress)) 
        { 
            LvlUp();        
        }
    }

    void LvlUp() 
    {
        int requiredExp = ExpCalc.GetExpToNextLvl(playerProgress);
        playerProgress.currentExp -= requiredExp;
        playerProgress.currentLvl++;

        if (playerProgress.OnLvlUp != null)
            playerProgress.OnLvlUp.Invoke();
    }
}
