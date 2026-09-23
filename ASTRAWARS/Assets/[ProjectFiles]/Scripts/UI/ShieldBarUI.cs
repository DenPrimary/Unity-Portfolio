using UnityEngine;
using UnityEngine.UI;

public class ShieldBarUI : MonoBehaviour
{
    public Slider shieldSlider;
    private Shield shield;

    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) 
        {
            ShieldManager sm = player.GetComponent<ShieldManager>();
            if (sm != null)
                shield = sm.GetShield();
        }

        if (shield == null || shieldSlider == null) return;

        float fill = 0f;

        if (shield.IsShieldActive())
            fill = shield.GetRemainingDuration() / shield.GetDuration();
        else if (shield.IsOnCooldown())
            fill = 1f - (shield.GetCooldownRemaining() / shield.GetCooldown());
        else
            fill = 1f;

        shieldSlider.value = Mathf.Clamp01(fill);
    }
}
