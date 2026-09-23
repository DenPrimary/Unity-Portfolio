using UnityEngine;

public class AsteroidSimple : Asteroid
{
    protected override float GetHealthMultiplier()
    {
        return 1f;
    }

    protected override void Die() { 
    }

    public override void ReturnToPool()
    {
        if (AsteroidSimplePool.Instance != null)
            AsteroidSimplePool.Instance.Return(this);
        else
            Destroy(gameObject);
    }
}
