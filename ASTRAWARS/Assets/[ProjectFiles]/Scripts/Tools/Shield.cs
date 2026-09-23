using UnityEngine;

public class Shield : MonoBehaviour
{
    [Header("Settings")]
    public float duration = 2f;
    public float cooldown = 8f;
    public float dangerRadius = 2.75f;

    [Header("References")]
    public ParticleSystem shieldParticles;
    public LayerMask asteroidLayers;

    private bool isActive = false;
    private bool isOnCooldown = false;
    private float nextActivationTime = 0f;
    private float activationTime = 0f;
    
    void Start()
    {
        if (shieldParticles == null)
            shieldParticles = GetComponent<ParticleSystem>();

        DeactivateShield();
    }

    void Update()
    {
        if (isOnCooldown && Time.time >= nextActivationTime) 
            isOnCooldown = false;
        
        if (!isActive && !isOnCooldown) 
            CheckForThreats();
    }

    void CheckForThreats()
    {
        Collider[] threats = Physics.OverlapSphere(transform.position, dangerRadius, asteroidLayers);

        if (threats.Length > 0) 
        { 
            ActivateShield();

            foreach (var threat in threats)
            {
                Asteroid asteroid = threat.GetComponent<Asteroid>();
                if (asteroid != null)
                {
                    HandleAsteroid(asteroid);
                    continue;
                }

                Fragment fragment = threat.GetComponent<Fragment>();
                if (fragment != null)
                {
                    HandleFragment(fragment);
                }
            }
        }
            
    }

    void ActivateShield() 
    {
        if (isActive) return;
        if (isOnCooldown) return;

        isActive = true;
        activationTime = Time.time;

        if (shieldParticles != null) 
        {
            shieldParticles.Clear();
            shieldParticles.Play();
        }

        Invoke(nameof(DeactivateShield), duration);
    }

    void DeactivateShield() 
    { 
        if (!isActive) return;

        isActive = false;

        if (shieldParticles != null)
        {
            shieldParticles.Stop();
            shieldParticles.Clear();
        }

        isOnCooldown = true;
        nextActivationTime = Time.time + cooldown;
    }

    void HandleAsteroid(Asteroid asteroid) 
    {
        if (asteroid == null) return;

        string type = asteroid.GetType().Name;

        asteroid.OnShieldHit();
    }

    void HandleFragment(Fragment fragment)
    {
        if (fragment == null) return;

        Destroy(fragment.gameObject);
    }

    void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dangerRadius);
    }

    public bool IsShieldActive() 
    {
        return isActive;
    }

    public bool IsOnCooldown() 
    {
        return isOnCooldown;
    }

    public float GetCooldownRemaining() 
    { 
        if (!isOnCooldown) return 0f;
        return Mathf.Max(0f, nextActivationTime - Time.time);
    }

    public void ForceActivate() 
    {
        isOnCooldown = false;
        ActivateShield();
    }

    public float GetDuration() 
    {
        return duration;
    }

    public float GetCooldown() 
    {
        return cooldown;
    }

    public float GetRemainingDuration() 
    {
        if (!isActive) return 0f;
        float elapsed = Time.time - activationTime;
        return Mathf.Max(0f, duration - elapsed);
    }
}