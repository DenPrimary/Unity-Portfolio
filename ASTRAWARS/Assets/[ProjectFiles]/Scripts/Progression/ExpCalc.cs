using UnityEngine;

public class ExpCalc : MonoBehaviour
{
    public static int GetExpForLvl(int level) 
    {
        return Mathf.RoundToInt(5 * Mathf.Pow(1.3f, level));
    }

    public static int GetExpToNextLvl(PlayerProgress playerProgress) 
    {
        if (playerProgress == null) return 0;
        return GetExpForLvl(playerProgress.currentLvl);
    }

    public static bool CanLvlUp(PlayerProgress playerProgress) 
    {
        if (playerProgress == null)
        {
            Debug.LogError("CanLvlUp: playerProgress is NULL!");
            return false;
        }

        return playerProgress.currentExp >= GetExpToNextLvl(playerProgress);
    }
}
