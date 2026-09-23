using UnityEngine;

public class ShieldManager : MonoBehaviour
{
    private Shield shield;

    public void SetShield(Shield shieldComponent)
    {
        shield = shieldComponent;
    }

    public Shield GetShield() 
    {
        return shield;
    }
}
