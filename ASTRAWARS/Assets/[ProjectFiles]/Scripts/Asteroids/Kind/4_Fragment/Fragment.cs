using UnityEngine;

public class Fragment : MonoBehaviour
{
    public float health = 1f;
    private bool isDead = false;
    private FragmentGroup group;

    private void GiveExp() 
    {
        int expGain = Mathf.Max(1, Mathf.RoundToInt(health));
        ExpManager.GiveExp(expGain, transform.position);
    }

    public void SetGroup(FragmentGroup g) {
        group = g;
    }

    public void SetHealth(float value) 
    { 
        health = value;
    }

    public void TakeDamage(float damage) 
    {
        if (isDead) return;

        health -= damage;
        if (health <= 0) { 
            isDead = true;
            GiveExp();
            group?.EliminatedByPlayer(transform.position);
            Object.Destroy(gameObject);
        }
    }
}
