using UnityEngine;
using UnityEngine.UIElements;

public class AsteroidAdamantine : Asteroid
{
    public float fixedSize = 1f;

    protected override void Awake(){
        base.Awake();

        transform.localScale = Vector3.one * fixedSize;
        currentSize = fixedSize;
        baseHealth = 999f;
        currentHealth = 999f;
        rotationSpeed = new Vector3(500f, 700f, 300f);

        if (rb != null) rb.mass = 100f; 
    }

    protected override float GetHealthMultiplier()
    {
        return 999f;
    }

    protected override void Die() {
    }

    public override void SetSize(float size) {
    }

    public override void OnShieldHit(){
    }

    public override void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;

        float damageDealt = Mathf.Min(damage, currentHealth);

        GiveExpForAdamantine(damageDealt * 2);

        currentHealth -= damage;

        if (currentHealth <= 0) 
        {
            DisableOutline();
            HighlightManager.Instance?.OnAsteroidDestroyed(gameObject);
            DisappearAndDestroy();
        }
    }

    private void GiveExpForAdamantine(float damageDealt) 
    {
        int expGain = Mathf.Max(1, Mathf.RoundToInt(damageDealt));
        ExpManager.GiveExp(expGain, transform.position);
    }
}
