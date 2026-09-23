using UnityEngine;

public class GaussBullet : MonoBehaviour
{
    public float lifetime = 5.0f;
    public float damage;

    private Rigidbody rb;
    private float lifeTimer = 0f;

    void Awake() { 
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable() {
        lifeTimer = 0f;

        if (rb != null) {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void Update() { 
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifetime)
            ReturnToPool();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Asteroid"))
        {
            if (HighlightManager.Instance != null)
                HighlightManager.Instance.HighlightAsteroid(collision.gameObject);

            Asteroid asteroid = collision.gameObject.GetComponent<Asteroid>();
            if (asteroid != null)
                asteroid.TakeDamage(damage);

            Fragment fragment = collision.gameObject.GetComponent<Fragment>();
            if (fragment != null)
                fragment.TakeDamage(damage);

            ReturnToPool();
        }
    }

    private void ReturnToPool() {
        if (BulletPool.Instance != null)
            BulletPool.Instance.ReturnBullet(this);
        else
            Destroy(gameObject);
    }
}
